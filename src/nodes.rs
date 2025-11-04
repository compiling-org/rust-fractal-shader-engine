//! Node-Based Visual Programming System
//!
//! This module implements a node-based editor for creating complex
//! fractal compositions through visual programming.

use std::collections::HashMap;
use nalgebra::Vector3;
use serde::{Deserialize, Serialize};

/// Unique identifier for nodes
pub type NodeId = u64;

/// Node types available in the editor
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum NodeType {
    // Fractal Generators
    FractalGenerator(FractalType),

    // Mathematical Operations
    MathOp(MathOperation),

    // Color Operations
    ColorAdjust(ColorOperation),

    // Transform Operations
    Transform,

    // Output Nodes
    Output,
}

/// Fractal generator types
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum FractalType {
    Mandelbrot,
    Julia,
    Mandelbulb,
    Mandelbox,
    IFS,
    Custom,
}

/// Mathematical operations
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum MathOperation {
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Sine,
    Cosine,
    Absolute,
    Clamp,
}

/// Color operations
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum ColorOperation {
    Brightness,
    Contrast,
    Saturation,
    HueShift,
    Invert,
    Gamma,
}

/// Node data structure
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Node {
    pub id: NodeId,
    pub node_type: NodeType,
    pub position: [f32; 2],
    pub parameters: HashMap<String, NodeParameter>,
    pub inputs: Vec<NodeInput>,
    pub outputs: Vec<NodeOutput>,
}

/// Node parameter types
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum NodeParameter {
    Float(f32),
    Vec3([f32; 3]),
    Color([f32; 4]),
    Bool(bool),
    Int(i32),
    String(String),
}

/// Node input/output definitions
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NodeInput {
    pub name: String,
    pub data_type: DataType,
    pub connected_output: Option<(NodeId, usize)>, // (node_id, output_index)
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NodeOutput {
    pub name: String,
    pub data_type: DataType,
}

/// Data types that can flow between nodes
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum DataType {
    Scalar,      // Single float value
    Vector3,     // 3D vector
    Color,       // RGBA color
    DistanceField, // 3D distance field
    Texture,     // 2D texture
}

/// Node graph containing all nodes and connections
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NodeGraph {
    pub nodes: HashMap<NodeId, Node>,
    pub connections: Vec<NodeConnection>,
}

/// Node connection between output and input
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NodeConnection {
    pub from_node: NodeId,
    pub from_output: usize,
    pub to_node: NodeId,
    pub to_input: usize,
}

/// Node execution context
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ExecutionContext {
    pub time: f32,
    pub resolution: [f32; 2],
    pub mouse_position: [f32; 2],
    pub cache: HashMap<NodeId, NodeResult>,
}

/// Result of executing a node
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum NodeResult {
    Scalar(f32),
    Vector3(Vector3<f32>),
    Color([f32; 4]),
    DistanceField(Vec<f32>),
    Texture(Vec<u8>),
}

impl NodeGraph {
    /// Create new empty node graph
    pub fn new() -> Self {
        Self {
            nodes: HashMap::new(),
            connections: Vec::new(),
        }
    }

    /// Add node to graph
    pub fn add_node(&mut self, mut node: Node) -> NodeId {
        let id = generate_node_id();
        node.id = id;
        self.nodes.insert(id, node);
        id
    }

    /// Remove node from graph
    pub fn remove_node(&mut self, node_id: NodeId) {
        self.nodes.remove(&node_id);
        // Remove all connections to/from this node
        self.connections.retain(|conn| {
            conn.from_node != node_id && conn.to_node != node_id
        });
    }

    /// Add connection between nodes
    pub fn add_connection(&mut self, from_node: NodeId, from_output: usize, to_node: NodeId, to_input: usize) -> bool {
        // Validate connection
        if let (Some(from), Some(to)) = (self.nodes.get(&from_node), self.nodes.get(&to_node)) {
            if from_output < from.outputs.len() && to_input < to.inputs.len() {
                let from_type = from.outputs[from_output].data_type;
                let to_type = to.inputs[to_input].data_type;

                if from_type == to_type {
                    self.connections.push(NodeConnection {
                        from_node,
                        from_output,
                        to_node,
                        to_input,
                    });
                    return true;
                }
            }
        }
        false
    }

    /// Remove connection
    pub fn remove_connection(&mut self, from_node: NodeId, from_output: usize, to_node: NodeId, to_input: usize) {
        self.connections.retain(|conn| {
            !(conn.from_node == from_node && conn.from_output == from_output &&
              conn.to_node == to_node && conn.to_input == to_input)
        });
    }

    /// Execute the entire node graph
    pub fn execute(&self, context: &mut ExecutionContext) -> Option<NodeResult> {
        // Find output node
        let output_node = self.nodes.values().find(|node| matches!(node.node_type, NodeType::Output))?;

        // Execute recursively
        self.execute_node(output_node.id, context)
    }

    /// Execute a specific node
    pub fn execute_node(&self, node_id: NodeId, context: &mut ExecutionContext) -> Option<NodeResult> {
        // Check cache first
        if let Some(cached) = context.cache.get(&node_id) {
            return Some(cached.clone());
        }

        let node = self.nodes.get(&node_id)?;

        // Get input values
        let mut input_values = Vec::new();
        for (input_idx, input) in node.inputs.iter().enumerate() {
            if let Some((from_node, from_output)) = input.connected_output {
                if let Some(result) = self.execute_node(from_node, context) {
                    input_values.push((input_idx, result));
                }
            }
        }

        // Execute node based on type
        let result = match &node.node_type {
            NodeType::FractalGenerator(fractal_type) => {
                self.execute_fractal_generator(fractal_type, &node.parameters, &input_values, context)
            }
            NodeType::MathOp(op) => {
                self.execute_math_operation(*op, &node.parameters, &input_values)
            }
            NodeType::ColorAdjust(op) => {
                self.execute_color_operation(*op, &node.parameters, &input_values)
            }
            NodeType::Transform => {
                self.execute_transform(&node.parameters, &input_values)
            }
            NodeType::Output => {
                // Output node just passes through its input
                input_values.first().map(|(_, result)| result.clone())
            }
        };

        // Cache result
        if let Some(ref res) = result {
            context.cache.insert(node_id, res.clone());
        }

        result
    }

    /// Execute fractal generator node
    fn execute_fractal_generator(
        &self,
        fractal_type: &FractalType,
        parameters: &HashMap<String, NodeParameter>,
        inputs: &[(usize, NodeResult)],
        context: &ExecutionContext,
    ) -> Option<NodeResult> {
        // Simplified fractal generation
        // In a real implementation, this would generate actual distance fields
        let iterations = parameters.get("iterations")
            .and_then(|p| match p { NodeParameter::Float(f) => Some(*f as u32), _ => None })
            .unwrap_or(100);

        let power = parameters.get("power")
            .and_then(|p| match p { NodeParameter::Float(f) => Some(*f), _ => None })
            .unwrap_or(2.0);

        // Generate simple distance field based on fractal type
        let mut distance_field = Vec::new();
        let size = 256; // 256x256 distance field

        for y in 0..size {
            for x in 0..size {
                let uv_x = (x as f32 / size as f32 - 0.5) * 2.0;
                let uv_y = (y as f32 / size as f32 - 0.5) * 2.0;

                let distance = match fractal_type {
                    FractalType::Mandelbrot => self.mandelbrot_distance(uv_x, uv_y, iterations),
                    FractalType::Julia => self.julia_distance(uv_x, uv_y, iterations),
                    FractalType::Mandelbulb => self.mandelbulb_distance(uv_x, uv_y, 0.0, power, iterations),
                    _ => (uv_x * uv_x + uv_y * uv_y).sqrt(),
                };

                distance_field.push(distance);
            }
        }

        Some(NodeResult::DistanceField(distance_field))
    }

    /// Execute mathematical operation
    fn execute_math_operation(
        &self,
        operation: MathOperation,
        parameters: &HashMap<String, NodeParameter>,
        inputs: &[(usize, NodeResult)],
    ) -> Option<NodeResult> {
        // Get input values
        let mut values = Vec::new();
        for (_, result) in inputs {
            if let NodeResult::Scalar(val) = result {
                values.push(*val);
            }
        }

        if values.is_empty() {
            return None;
        }

        let result = match operation {
            MathOperation::Add => values.iter().sum(),
            MathOperation::Multiply => values.iter().product(),
            MathOperation::Sine => values.first().map(|v| v.sin()).unwrap_or(0.0),
            MathOperation::Cosine => values.first().map(|v| v.cos()).unwrap_or(0.0),
            MathOperation::Absolute => values.first().map(|v| v.abs()).unwrap_or(0.0),
            _ => values.first().copied().unwrap_or(0.0),
        };

        Some(NodeResult::Scalar(result))
    }

    /// Execute color operation
    fn execute_color_operation(
        &self,
        operation: ColorOperation,
        parameters: &HashMap<String, NodeParameter>,
        inputs: &[(usize, NodeResult)],
    ) -> Option<NodeResult> {
        // Get input color
        let input_color = inputs.first()
            .and_then(|(_, result)| match result {
                NodeResult::Color(c) => Some(*c),
                _ => None,
            })
            .unwrap_or([1.0, 1.0, 1.0, 1.0]);

        let result = match operation {
            ColorOperation::Invert => {
                [1.0 - input_color[0], 1.0 - input_color[1], 1.0 - input_color[2], input_color[3]]
            }
            ColorOperation::Brightness => {
                let brightness = parameters.get("brightness")
                    .and_then(|p| match p { NodeParameter::Float(f) => Some(*f), _ => None })
                    .unwrap_or(1.0);
                [input_color[0] * brightness, input_color[1] * brightness, input_color[2] * brightness, input_color[3]]
            }
            _ => input_color,
        };

        Some(NodeResult::Color(result))
    }

    /// Execute transform operation
    fn execute_transform(
        &self,
        parameters: &HashMap<String, NodeParameter>,
        inputs: &[(usize, NodeResult)],
    ) -> Option<NodeResult> {
        // Get input vector
        let input_vec = inputs.first()
            .and_then(|(_, result)| match result {
                NodeResult::Vector3(v) => Some(*v),
                _ => None,
            })
            .unwrap_or(Vector3::zeros());

        // Apply transform (simplified)
        let scale = parameters.get("scale")
            .and_then(|p| match p { NodeParameter::Float(f) => Some(*f), _ => None })
            .unwrap_or(1.0);

        Some(NodeResult::Vector3(input_vec * scale))
    }

    // Distance estimation functions (simplified)
    fn mandelbrot_distance(&self, x: f32, y: f32, max_iter: u32) -> f32 {
        let mut zx = 0.0;
        let mut zy = 0.0;
        let mut iteration = 0;

        while zx * zx + zy * zy < 4.0 && iteration < max_iter {
            let tmp = zx * zx - zy * zy + x;
            zy = 2.0 * zx * zy + y;
            zx = tmp;
            iteration += 1;
        }

        if iteration < max_iter {
            (iteration as f32 / max_iter as f32).sqrt()
        } else {
            0.0
        }
    }

    fn julia_distance(&self, x: f32, y: f32, max_iter: u32) -> f32 {
        let mut zx = x;
        let mut zy = y;
        let mut iteration = 0;

        while zx * zx + zy * zy < 4.0 && iteration < max_iter {
            let tmp = zx * zx - zy * zy - 0.7;
            zy = 2.0 * zx * zy + 0.27015;
            zx = tmp;
            iteration += 1;
        }

        if iteration < max_iter {
            (iteration as f32 / max_iter as f32).sqrt()
        } else {
            0.0
        }
    }

    fn mandelbulb_distance(&self, x: f32, y: f32, z: f32, power: f32, max_iter: u32) -> f32 {
        let mut zx = x;
        let mut zy = y;
        let mut zz = z;
        let mut iteration = 0;

        while zx * zx + zy * zy + zz * zz < 4.0 && iteration < max_iter {
            let r = (zx * zx + zy * zy + zz * zz).sqrt();
            let theta = zy.atan2(zx);
            let phi = (zz / r).asin();

            let new_r = r.powf(power);
            let new_theta = theta * power;
            let new_phi = phi * power;

            zx = new_r * new_theta.cos() * new_phi.cos();
            zy = new_r * new_theta.sin() * new_phi.cos();
            zz = new_r * new_phi.sin();

            zx += x;
            zy += y;
            zz += z;

            iteration += 1;
        }

        if iteration < max_iter {
            (iteration as f32 / max_iter as f32).sqrt()
        } else {
            0.0
        }
    }
}

/// Node factory for creating common node types
pub struct NodeFactory;

impl NodeFactory {
    /// Create Mandelbrot generator node
    pub fn create_mandelbrot_generator(position: [f32; 2]) -> Node {
        let mut parameters = HashMap::new();
        parameters.insert("iterations".to_string(), NodeParameter::Float(100.0));
        parameters.insert("zoom".to_string(), NodeParameter::Float(1.0));
        parameters.insert("center_x".to_string(), NodeParameter::Float(-0.5));
        parameters.insert("center_y".to_string(), NodeParameter::Float(0.0));

        Node {
            id: 0, // Will be set by graph
            node_type: NodeType::FractalGenerator(FractalType::Mandelbrot),
            position,
            parameters,
            inputs: vec![],
            outputs: vec![NodeOutput {
                name: "distance_field".to_string(),
                data_type: DataType::DistanceField,
            }],
        }
    }

    /// Create math operation node
    pub fn create_math_node(operation: MathOperation, position: [f32; 2]) -> Node {
        Node {
            id: 0,
            node_type: NodeType::MathOp(operation),
            position,
            parameters: HashMap::new(),
            inputs: vec![
                NodeInput {
                    name: "input_a".to_string(),
                    data_type: DataType::Scalar,
                    connected_output: None,
                },
                NodeInput {
                    name: "input_b".to_string(),
                    data_type: DataType::Scalar,
                    connected_output: None,
                },
            ],
            outputs: vec![NodeOutput {
                name: "result".to_string(),
                data_type: DataType::Scalar,
            }],
        }
    }

    /// Create output node
    pub fn create_output_node(position: [f32; 2]) -> Node {
        Node {
            id: 0,
            node_type: NodeType::Output,
            position,
            parameters: HashMap::new(),
            inputs: vec![NodeInput {
                name: "input".to_string(),
                data_type: DataType::DistanceField,
                connected_output: None,
            }],
            outputs: vec![], // Output nodes don't have outputs
        }
    }
}

/// Generate unique node ID
fn generate_node_id() -> NodeId {
    use std::sync::atomic::{AtomicU64, Ordering};
    static COUNTER: AtomicU64 = AtomicU64::new(1);
    COUNTER.fetch_add(1, Ordering::Relaxed)
}