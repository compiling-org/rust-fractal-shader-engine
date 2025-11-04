//! OSC (Open Sound Control) integration for the Fractal Shader Studio
//!
//! This module provides OSC support for controlling fractal parameters
//! and receiving data from external OSC controllers.

use bevy::prelude::*;
use std::collections::HashMap;
use std::net::{UdpSocket, SocketAddr};
use std::sync::{Arc, Mutex};
use std::thread;

/// OSC message data structure
#[derive(Debug, Clone)]
pub struct OscMessage {
    pub address: String,
    pub arguments: Vec<OscArgument>,
    pub timestamp: u64,
}

/// OSC argument types
#[derive(Debug, Clone)]
pub enum OscArgument {
    Int(i32),
    Float(f32),
    String(String),
    Blob(Vec<u8>),
    Boolean(bool),
}

/// OSC controller for parameter mapping
#[derive(Resource)]
pub struct OscController {
    pub mappings: HashMap<String, OscMapping>,
    pub current_values: HashMap<String, f32>,
    pub osc_server: Option<OscServer>,
}

/// OSC parameter mapping
#[derive(Debug, Clone)]
pub struct OscMapping {
    pub osc_address: String,
    pub parameter_name: String,
    pub min_value: f32,
    pub max_value: f32,
    pub sensitivity: f32,
    pub invert: bool,
}

/// OSC server for receiving messages
pub struct OscServer {
    socket: UdpSocket,
    running: Arc<Mutex<bool>>,
    message_queue: Arc<Mutex<Vec<OscMessage>>>,
}

impl Default for OscController {
    fn default() -> Self {
        Self {
            mappings: HashMap::new(),
            current_values: HashMap::new(),
            osc_server: None,
        }
    }
}

impl OscController {
    pub fn new() -> Self {
        Self::default()
    }

    /// Start OSC server on specified port
    pub fn start_server(&mut self, port: u16) -> Result<(), Box<dyn std::error::Error>> {
        let server = OscServer::new(port)?;
        self.osc_server = Some(server);
        Ok(())
    }

    /// Stop OSC server
    pub fn stop_server(&mut self) {
        if let Some(server) = &self.osc_server {
            server.stop();
        }
        self.osc_server = None;
    }

    /// Add an OSC mapping
    pub fn add_mapping(&mut self, mapping: OscMapping) {
        self.mappings.insert(mapping.osc_address.clone(), mapping);
    }

    /// Process incoming OSC messages
    pub fn process_messages(&mut self) {
        if let Some(server) = &self.osc_server {
            let messages = server.get_messages();
            for message in messages {
                self.process_message(&message);
            }
        }
    }

    /// Process a single OSC message
    pub fn process_message(&mut self, message: &OscMessage) {
        // Check if we have a mapping for this OSC address
        if let Some(mapping) = self.mappings.get(&message.address) {
            // Extract value from message arguments
            if let Some(value) = self.extract_value(&message.arguments) {
                // Apply sensitivity
                let mut mapped_value = value * mapping.sensitivity;
                
                // Invert if needed
                if mapping.invert {
                    mapped_value = 1.0 - mapped_value;
                }
                
                // Map to parameter range
                let final_value = mapping.min_value + mapped_value * (mapping.max_value - mapping.min_value);
                
                // Store the value
                self.current_values.insert(mapping.parameter_name.clone(), final_value);
            }
        }
    }

    /// Extract value from OSC arguments
    fn extract_value(&self, arguments: &[OscArgument]) -> Option<f32> {
        if arguments.is_empty() {
            return None;
        }
        
        match &arguments[0] {
            OscArgument::Float(f) => Some(*f),
            OscArgument::Int(i) => Some(*i as f32),
            OscArgument::Boolean(b) => Some(if *b { 1.0 } else { 0.0 }),
            _ => None,
        }
    }

    /// Get parameter value
    pub fn get_parameter(&self, name: &str) -> f32 {
        *self.current_values.get(name).unwrap_or(&0.0)
    }

    /// Create default OSC mappings
    pub fn create_default_mappings(&mut self) {
        let mappings = vec![
            OscMapping {
                osc_address: "/fractal/zoom".to_string(),
                parameter_name: "zoom".to_string(),
                min_value: 0.1,
                max_value: 10.0,
                sensitivity: 1.0,
                invert: false,
            },
            OscMapping {
                osc_address: "/fractal/iterations".to_string(),
                parameter_name: "iterations".to_string(),
                min_value: 10.0,
                max_value: 200.0,
                sensitivity: 1.0,
                invert: false,
            },
            OscMapping {
                osc_address: "/fractal/speed".to_string(),
                parameter_name: "speed".to_string(),
                min_value: 0.0,
                max_value: 5.0,
                sensitivity: 1.0,
                invert: false,
            },
            OscMapping {
                osc_address: "/fractal/brightness".to_string(),
                parameter_name: "brightness".to_string(),
                min_value: 0.0,
                max_value: 20.0,
                sensitivity: 1.0,
                invert: false,
            },
            OscMapping {
                osc_address: "/fractal/contrast".to_string(),
                parameter_name: "contrast".to_string(),
                min_value: 0.0,
                max_value: 3.0,
                sensitivity: 1.0,
                invert: false,
            },
        ];

        for mapping in mappings {
            self.add_mapping(mapping);
        }
    }
}

impl OscServer {
    /// Create a new OSC server on the specified port
    pub fn new(port: u16) -> Result<Self, Box<dyn std::error::Error>> {
        let addr = SocketAddr::from(([0, 0, 0, 0], port));
        let socket = UdpSocket::bind(addr)?;
        socket.set_nonblocking(true)?;
        
        let running = Arc::new(Mutex::new(true));
        let message_queue = Arc::new(Mutex::new(Vec::new()));
        let running_clone = running.clone();
        let message_queue_clone = message_queue.clone();
        let socket_clone = socket.try_clone()?;
        
        // Start listening thread
        thread::spawn(move || {
            Self::listen(socket_clone, running_clone, message_queue_clone);
        });
        
        Ok(Self {
            socket,
            running,
            message_queue,
        })
    }
    
    /// Listen for OSC messages
    fn listen(
        socket: UdpSocket,
        running: Arc<Mutex<bool>>,
        message_queue: Arc<Mutex<Vec<OscMessage>>>,
    ) {
        let mut buf = [0; 1024];
        
        while *running.lock().unwrap() {
            match socket.recv_from(&mut buf) {
                Ok((size, _addr)) => {
                    // Parse OSC message (simplified implementation)
                    if let Some(message) = Self::parse_osc_message(&buf[..size]) {
                        message_queue.lock().unwrap().push(message);
                    }
                }
                Err(ref e) if e.kind() == std::io::ErrorKind::WouldBlock => {
                    // No data available, sleep briefly
                    std::thread::sleep(std::time::Duration::from_millis(1));
                }
                Err(e) => {
                    log::error!("OSC receive error: {}", e);
                    break;
                }
            }
        }
    }
    
    /// Parse OSC message from buffer (simplified implementation)
    fn parse_osc_message(buf: &[u8]) -> Option<OscMessage> {
        // This is a very simplified OSC parser
        // In a real implementation, you would use a proper OSC library
        
        // Check if buffer has minimum OSC message structure
        if buf.len() < 8 {
            return None;
        }
        
        // Extract address pattern (null-terminated string)
        let mut address_end = 0;
        while address_end < buf.len() && buf[address_end] != 0 {
            address_end += 1;
        }
        
        if address_end >= buf.len() {
            return None;
        }
        
        let address = String::from_utf8_lossy(&buf[0..address_end]).to_string();
        
        // Skip null padding to 4-byte boundary
        let mut type_start = (address_end + 4) & !3; // Align to 4-byte boundary
        
        if type_start >= buf.len() {
            return None;
        }
        
        // Extract type tags
        if buf[type_start] != b',' {
            return None;
        }
        
        type_start += 1;
        let mut type_end = type_start;
        while type_end < buf.len() && buf[type_end] != 0 {
            type_end += 1;
        }
        
        if type_end >= buf.len() {
            return None;
        }
        
        let type_tags = String::from_utf8_lossy(&buf[type_start..type_end]).to_string();
        
        // Skip null padding to 4-byte boundary
        let mut data_start = (type_end + 4) & !3; // Align to 4-byte boundary
        
        // Parse arguments based on type tags
        let mut arguments = Vec::new();
        let mut data_pos = data_start;
        
        for tag in type_tags.chars() {
            match tag {
                'f' => {
                    if data_pos + 4 <= buf.len() {
                        let bytes = [
                            buf[data_pos],
                            buf[data_pos + 1],
                            buf[data_pos + 2],
                            buf[data_pos + 3],
                        ];
                        let value = f32::from_be_bytes(bytes);
                        arguments.push(OscArgument::Float(value));
                        data_pos += 4;
                    }
                }
                'i' => {
                    if data_pos + 4 <= buf.len() {
                        let bytes = [
                            buf[data_pos],
                            buf[data_pos + 1],
                            buf[data_pos + 2],
                            buf[data_pos + 3],
                        ];
                        let value = i32::from_be_bytes(bytes);
                        arguments.push(OscArgument::Int(value));
                        data_pos += 4;
                    }
                }
                's' => {
                    let mut string_end = data_pos;
                    while string_end < buf.len() && buf[string_end] != 0 {
                        string_end += 1;
                    }
                    if string_end < buf.len() {
                        let value = String::from_utf8_lossy(&buf[data_pos..string_end]).to_string();
                        arguments.push(OscArgument::String(value));
                        data_pos = (string_end + 4) & !3; // Align to 4-byte boundary
                    }
                }
                'T' => {
                    arguments.push(OscArgument::Boolean(true));
                }
                'F' => {
                    arguments.push(OscArgument::Boolean(false));
                }
                _ => {
                    // Unsupported type, skip
                }
            }
        }
        
        Some(OscMessage {
            address,
            arguments,
            timestamp: std::time::SystemTime::now()
                .duration_since(std::time::UNIX_EPOCH)
                .unwrap()
                .as_millis() as u64,
        })
    }
    
    /// Stop the OSC server
    pub fn stop(&self) {
        *self.running.lock().unwrap() = false;
    }
    
    /// Get pending messages
    pub fn get_messages(&self) -> Vec<OscMessage> {
        let mut queue = self.message_queue.lock().unwrap();
        std::mem::take(&mut *queue)
    }
}

/// Combined OSC system
#[derive(Resource)]
pub struct OscSystem {
    pub controller: OscController,
}

impl OscSystem {
    pub fn new() -> Self {
        let mut controller = OscController::new();
        controller.create_default_mappings();
        
        Self {
            controller,
        }
    }
    
    /// Start OSC server
    pub fn start_server(&mut self, port: u16) -> Result<(), Box<dyn std::error::Error>> {
        self.controller.start_server(port)
    }
    
    /// Stop OSC server
    pub fn stop_server(&mut self) {
        self.controller.stop_server();
    }
    
    /// Process incoming OSC messages
    pub fn process_messages(&mut self) {
        self.controller.process_messages();
    }
    
    /// Get parameter value
    pub fn get_parameter(&self, name: &str) -> f32 {
        self.controller.get_parameter(name)
    }
}