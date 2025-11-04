//! Gesture control system for Leap Motion and MediaPipe integration
//!
//! This module provides interfaces for gesture-based control of the fractal generator,
//! allowing users to manipulate parameters through hand gestures and movements.

use bevy::prelude::*;
use std::sync::{Arc, Mutex};
use std::collections::HashMap;

/// Gesture tracking data structure
#[derive(Debug, Clone, Resource)]
pub struct GestureData {
    pub hand_positions: Vec<HandPosition>,
    pub gestures: Vec<RecognizedGesture>,
    pub active_gestures: HashMap<String, f32>,
}

/// Hand position data
#[derive(Debug, Clone)]
pub struct HandPosition {
    pub hand_id: u32,
    pub palm_position: [f32; 3],
    pub palm_normal: [f32; 3],
    pub fingers: Vec<FingerData>,
    pub confidence: f32,
}

/// Finger tracking data
#[derive(Debug, Clone)]
pub struct FingerData {
    pub finger_type: FingerType,
    pub tip_position: [f32; 3],
    pub dip_position: [f32; 3],
    pub pip_position: [f32; 3],
    pub mcp_position: [f32; 3],
    pub is_extended: bool,
}

/// Types of fingers
#[derive(Debug, Clone)]
pub enum FingerType {
    Thumb,
    Index,
    Middle,
    Ring,
    Pinky,
}

/// Recognized gesture types
#[derive(Debug, Clone)]
pub enum RecognizedGesture {
    Pinch { hand_id: u32, strength: f32 },
    Grab { hand_id: u32, strength: f32 },
    Swipe { direction: SwipeDirection, velocity: f32 },
    Tap { position: [f32; 3], intensity: f32 },
    Circle { center: [f32; 3], radius: f32, progress: f32 },
    HandMove { delta: [f32; 3], velocity: f32 },
}

/// Swipe directions
#[derive(Debug, Clone)]
pub enum SwipeDirection {
    Up,
    Down,
    Left,
    Right,
    Forward,
    Backward,
}

/// Gesture controller for parameter mapping
#[derive(Resource)]
pub struct GestureController {
    pub gesture_data: Arc<Mutex<GestureData>>,
    pub parameter_mappings: HashMap<String, GestureMapping>,
    pub is_leap_motion_available: bool,
    pub is_mediapipe_available: bool,
}

/// Gesture to parameter mapping
#[derive(Debug, Clone)]
pub struct GestureMapping {
    pub gesture_name: String,
    pub parameter_name: String,
    pub min_value: f32,
    pub max_value: f32,
    pub sensitivity: f32,
    pub invert: bool,
}

impl Default for GestureData {
    fn default() -> Self {
        Self {
            hand_positions: Vec::new(),
            gestures: Vec::new(),
            active_gestures: HashMap::new(),
        }
    }
}

impl GestureController {
    pub fn new() -> Self {
        Self {
            gesture_data: Arc::new(Mutex::new(GestureData::default())),
            parameter_mappings: HashMap::new(),
            is_leap_motion_available: false,
            is_mediapipe_available: false,
        }
    }

    /// Initialize Leap Motion support
    pub fn init_leap_motion(&mut self) -> Result<(), Box<dyn std::error::Error>> {
        // In a real implementation, this would initialize the Leap Motion SDK
        log::info!("Initializing Leap Motion controller");
        self.is_leap_motion_available = true;
        Ok(())
    }

    /// Initialize MediaPipe support
    pub fn init_mediapipe(&mut self) -> Result<(), Box<dyn std::error::Error>> {
        // In a real implementation, this would initialize MediaPipe
        log::info!("Initializing MediaPipe controller");
        self.is_mediapipe_available = true;
        Ok(())
    }

    /// Update gesture data from Leap Motion
    pub fn update_from_leap_motion(&self, frame_data: LeapFrameData) {
        let mut data = self.gesture_data.lock().unwrap();
        data.hand_positions.clear();
        
        // Convert Leap Motion data to our format
        for hand in frame_data.hands {
            let hand_pos = HandPosition {
                hand_id: hand.id,
                palm_position: [hand.palm_position.x, hand.palm_position.y, hand.palm_position.z],
                palm_normal: [hand.palm_normal.x, hand.palm_normal.y, hand.palm_normal.z],
                fingers: hand.fingers.into_iter().map(|finger| FingerData {
                    finger_type: match finger.finger_type {
                        LeapFingerType::Thumb => FingerType::Thumb,
                        LeapFingerType::Index => FingerType::Index,
                        LeapFingerType::Middle => FingerType::Middle,
                        LeapFingerType::Ring => FingerType::Ring,
                        LeapFingerType::Pinky => FingerType::Pinky,
                    },
                    tip_position: [finger.tip_position.x, finger.tip_position.y, finger.tip_position.z],
                    dip_position: [finger.dip_position.x, finger.dip_position.y, finger.dip_position.z],
                    pip_position: [finger.pip_position.x, finger.pip_position.y, finger.pip_position.z],
                    mcp_position: [finger.mcp_position.x, finger.mcp_position.y, finger.mcp_position.z],
                    is_extended: finger.is_extended,
                }).collect(),
                confidence: hand.confidence,
            };
            data.hand_positions.push(hand_pos);
        }
        
        // Recognize gestures
        self.recognize_gestures(&mut data);
    }

    /// Update gesture data from MediaPipe
    pub fn update_from_mediapipe(&self, landmarks: MediaPipeLandmarks) {
        let mut data = self.gesture_data.lock().unwrap();
        data.hand_positions.clear();
        
        // Convert MediaPipe data to our format
        for (hand_id, hand_landmarks) in landmarks.hands.iter().enumerate() {
            let palm_pos = [
                (hand_landmarks.landmarks[0].x + hand_landmarks.landmarks[12].x) / 2.0,
                (hand_landmarks.landmarks[0].y + hand_landmarks.landmarks[12].y) / 2.0,
                (hand_landmarks.landmarks[0].z + hand_landmarks.landmarks[12].z) / 2.0,
            ];
            
            let hand_pos = HandPosition {
                hand_id: hand_id as u32,
                palm_position: palm_pos,
                palm_normal: [0.0, 0.0, 1.0], // Simplified
                fingers: vec![
                    FingerData {
                        finger_type: FingerType::Thumb,
                        tip_position: [
                            hand_landmarks.landmarks[4].x,
                            hand_landmarks.landmarks[4].y,
                            hand_landmarks.landmarks[4].z,
                        ],
                        dip_position: [
                            hand_landmarks.landmarks[3].x,
                            hand_landmarks.landmarks[3].y,
                            hand_landmarks.landmarks[3].z,
                        ],
                        pip_position: [
                            hand_landmarks.landmarks[2].x,
                            hand_landmarks.landmarks[2].y,
                            hand_landmarks.landmarks[2].z,
                        ],
                        mcp_position: [
                            hand_landmarks.landmarks[1].x,
                            hand_landmarks.landmarks[1].y,
                            hand_landmarks.landmarks[1].z,
                        ],
                        is_extended: true, // Simplified
                    },
                    // Add other fingers similarly...
                ],
                confidence: 1.0,
            };
            data.hand_positions.push(hand_pos);
        }
        
        // Recognize gestures
        self.recognize_gestures(&mut data);
    }

    /// Recognize gestures from hand positions
    fn recognize_gestures(&self, data: &mut GestureData) {
        data.gestures.clear();
        data.active_gestures.clear();
        
        for hand in &data.hand_positions {
            // Pinch gesture detection
            if let Some(pinch_strength) = self.detect_pinch(hand) {
                data.gestures.push(RecognizedGesture::Pinch {
                    hand_id: hand.hand_id,
                    strength: pinch_strength,
                });
                data.active_gestures.insert("pinch".to_string(), pinch_strength);
            }
            
            // Grab gesture detection
            if let Some(grab_strength) = self.detect_grab(hand) {
                data.gestures.push(RecognizedGesture::Grab {
                    hand_id: hand.hand_id,
                    strength: grab_strength,
                });
                data.active_gestures.insert("grab".to_string(), grab_strength);
            }
        }
    }

    /// Detect pinch gesture
    fn detect_pinch(&self, hand: &HandPosition) -> Option<f32> {
        // Simplified pinch detection between thumb and index finger
        if hand.fingers.len() >= 2 {
            let thumb_tip = &hand.fingers[0].tip_position;
            let index_tip = &hand.fingers[1].tip_position;
            
            let distance = ((thumb_tip[0] - index_tip[0]).powi(2) +
                           (thumb_tip[1] - index_tip[1]).powi(2) +
                           (thumb_tip[2] - index_tip[2]).powi(2)).sqrt();
            
            // Normalize distance to strength (0.0 to 1.0)
            let strength = 1.0 - (distance / 0.1).min(1.0);
            if strength > 0.3 {
                return Some(strength);
            }
        }
        None
    }

    /// Detect grab gesture
    fn detect_grab(&self, hand: &HandPosition) -> Option<f32> {
        // Simplified grab detection based on finger extension
        let extended_fingers = hand.fingers.iter().filter(|f| f.is_extended).count();
        let grab_strength = 1.0 - (extended_fingers as f32 / hand.fingers.len() as f32);
        if grab_strength > 0.5 {
            Some(grab_strength)
        } else {
            None
        }
    }

    /// Add a gesture mapping
    pub fn add_mapping(&mut self, mapping: GestureMapping) {
        self.parameter_mappings.insert(mapping.gesture_name.clone(), mapping);
    }

    /// Get parameter value based on gesture
    pub fn get_parameter(&self, name: &str) -> f32 {
        // Get gesture data
        let data = self.gesture_data.lock().unwrap();
        
        // Check if we have a mapping for this parameter
        for (_, mapping) in &self.parameter_mappings {
            if mapping.parameter_name == name {
                // Check if the gesture is active
                if let Some(gesture_value) = data.active_gestures.get(&mapping.gesture_name) {
                    let mut value = *gesture_value;
                    
                    // Apply sensitivity
                    value *= mapping.sensitivity;
                    
                    // Invert if needed
                    if mapping.invert {
                        value = 1.0 - value;
                    }
                    
                    // Map to parameter range
                    return mapping.min_value + value * (mapping.max_value - mapping.min_value);
                }
            }
        }
        
        0.0
    }

    /// Create default gesture mappings
    pub fn create_default_mappings(&mut self) {
        let mappings = vec![
            GestureMapping {
                gesture_name: "pinch".to_string(),
                parameter_name: "zoom".to_string(),
                min_value: 0.1,
                max_value: 10.0,
                sensitivity: 1.0,
                invert: false,
            },
            GestureMapping {
                gesture_name: "grab".to_string(),
                parameter_name: "rotation_speed".to_string(),
                min_value: 0.0,
                max_value: 5.0,
                sensitivity: 1.0,
                invert: false,
            },
            GestureMapping {
                gesture_name: "hand_move_x".to_string(),
                parameter_name: "position_x".to_string(),
                min_value: -5.0,
                max_value: 5.0,
                sensitivity: 2.0,
                invert: false,
            },
            GestureMapping {
                gesture_name: "hand_move_y".to_string(),
                parameter_name: "position_y".to_string(),
                min_value: -5.0,
                max_value: 5.0,
                sensitivity: 2.0,
                invert: false,
            },
        ];

        for mapping in mappings {
            self.add_mapping(mapping);
        }
    }
}

/// Leap Motion frame data (simplified)
pub struct LeapFrameData {
    pub hands: Vec<LeapHand>,
    pub timestamp: u64,
}

/// Leap Motion hand data
pub struct LeapHand {
    pub id: u32,
    pub palm_position: LeapVector,
    pub palm_normal: LeapVector,
    pub fingers: Vec<LeapFinger>,
    pub confidence: f32,
}

/// Leap Motion vector
pub struct LeapVector {
    pub x: f32,
    pub y: f32,
    pub z: f32,
}

/// Leap Motion finger data
pub struct LeapFinger {
    pub finger_type: LeapFingerType,
    pub tip_position: LeapVector,
    pub dip_position: LeapVector,
    pub pip_position: LeapVector,
    pub mcp_position: LeapVector,
    pub is_extended: bool,
}

/// Leap Motion finger types
pub enum LeapFingerType {
    Thumb,
    Index,
    Middle,
    Ring,
    Pinky,
}

/// MediaPipe landmarks data
pub struct MediaPipeLandmarks {
    pub hands: Vec<HandLandmarks>,
}

/// Hand landmarks
pub struct HandLandmarks {
    pub landmarks: Vec<Landmark>,
}

/// Individual landmark
pub struct Landmark {
    pub x: f32,
    pub y: f32,
    pub z: f32,
    pub visibility: f32,
}