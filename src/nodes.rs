use bevy::prelude::*;
use std::collections::HashMap;

/// Node-based fractal composition system
/// Similar to Blender's node editor but for fractal shaders

/// Unique identifier for nodes
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub struct NodeId(pub u64);

/// Connection between nodes
#[derive(Debug, Clone)]
pub struct NodeConnection {
    pub from_node: NodeId,
    pub from_output: String,
    pub to_node: NodeId,
    pub to_input: String,
}

/// Node types available in the system
#[derive(Debug, Clone)]
pub enum NodeType {
    /// Fractal generator (Mandelbrot, Julia, etc.)
    FractalGenerator(String), // shader name
    /// Mathematical operation
    MathOp(MathOperation),
    /// Color adjustment
    ColorAdjust(ColorAdjustment),
    /// Effect (blur, distortion, etc.)
    Effect(String), // effect name
    /// Audio input
    AudioInput,
    /// MIDI input
    MidiInput,
    /// Output node
    Output,
}

/// Mathematical operations
#[derive(Debug, Clone)]
pub enum MathOperation {
    Add,
    Subtract,
    Multiply,
    Divide,
    Sin,
    Cos,
    Tan,
    Pow,
    Sqrt,
    Abs,
}

/// Color adjustments
#[derive(Debug, Clone)]
pub enum ColorAdjustment {
    Brightness,
    Contrast,
    Saturation,
    Hue,
    Invert,
    Grayscale,
}

/// Node in the composition graph
#[derive(Component)]
pub struct FractalNode {
    pub id: NodeId,
    pub node_type: NodeType,
    pub position: Vec2,
    pub inputs: HashMap<String, NodeInput>,
    pub outputs: HashMap<String, NodeOutput>,
    pub parameters: HashMap<String, f32>,
}

/// Input socket on a node
#[derive(Debug, Clone)]
pub struct NodeInput {
    pub name: String,
    pub data_type: DataType,
    pub connected_from: Option<NodeConnection>,
    pub default_value: f32,
}

/// Output socket on a node
#[derive(Debug, Clone)]
pub struct NodeOutput {
    pub name: String,
    pub data_type: DataType,
}

/// Data types that can flow between nodes
#[derive(Debug, Clone, PartialEq)]
pub enum DataType {
    Float,
    Vec2,
    Vec3,
    Vec4,
    Texture,
    Audio,
}

/// The composition graph
#[derive(Resource)]
pub struct NodeGraph {
    pub nodes: HashMap<NodeId, FractalNode>,
    pub connections: Vec<NodeConnection>,
    pub next_id: u64,
}

impl Default for NodeGraph {
    fn default() -> Self {
        Self {
            nodes: HashMap::new(),
            connections: Vec::new(),
            next_id: 1,
        }
    }
}

impl NodeGraph {
    /// Create a new node
    pub fn create_node(&mut self, node_type: NodeType, position: Vec2) -> NodeId {
        let id = NodeId(self.next_id);
        self.next_id += 1;

        let node = FractalNode::new(id, node_type, position);
        self.nodes.insert(id, node);
        id
    }

    /// Connect two nodes
    pub fn connect_nodes(
        &mut self,
        from_node: NodeId,
        from_output: &str,
        to_node: NodeId,
        to_input: &str,
    ) -> Result<(), String> {
        // Validate connection
        if !self.nodes.contains_key(&from_node) {
            return Err(format!("Source node {:?} does not exist", from_node));
        }
        if !self.nodes.contains_key(&to_node) {
            return Err(format!("Target node {:?} does not exist", to_node));
        }

        let from_node_data = &self.nodes[&from_node];
        let to_node_data = &self.nodes[&to_node];

        if !from_node_data.outputs.contains_key(from_output) {
            return Err(format!("Output '{}' does not exist on source node", from_output));
        }
        if !to_node_data.inputs.contains_key(to_input) {
            return Err(format!("Input '{}' does not exist on target node", to_input));
        }

        // Check data type compatibility
        let output_type = &from_node_data.outputs[from_output].data_type;
        let input_type = &to_node_data.inputs[to_input].data_type;

        if output_type != input_type {
            return Err(format!("Data type mismatch: {:?} vs {:?}", output_type, input_type));
        }

        // Create connection
        let connection = NodeConnection {
            from_node,
            from_output: from_output.to_string(),
            to_node,
            to_input: to_input.to_string(),
        };

        // Update input connection
        if let Some(node) = self.nodes.get_mut(&to_node) {
            if let Some(input) = node.inputs.get_mut(to_input) {
                input.connected_from = Some(connection.clone());
            }
        }

        self.connections.push(connection);
        Ok(())
    }

    /// Remove a connection
    pub fn disconnect_nodes(&mut self, from_node: NodeId, to_node: NodeId, input_name: &str) {
        self.connections.retain(|conn| {
            !(conn.from_node == from_node && conn.to_node == to_node && conn.to_input == input_name)
        });

        // Clear input connection
        if let Some(node) = self.nodes.get_mut(&to_node) {
            if let Some(input) = node.inputs.get_mut(input_name) {
                input.connected_from = None;
            }
        }
    }

    /// Evaluate the node graph and generate final shader
    pub fn evaluate(&self) -> Result<String, String> {
        // Find output node
        let output_node = self.nodes.values()
            .find(|node| matches!(node.node_type, NodeType::Output))
            .ok_or("No output node found")?;

        // Traverse graph from output to inputs
        let mut evaluated_nodes = std::collections::HashSet::new();
        let mut shader_parts = Vec::new();

        self.evaluate_node(output_node.id, &mut evaluated_nodes, &mut shader_parts)?;

        // Combine shader parts
        let mut final_shader = String::new();
        final_shader.push_str("// Generated fractal shader\n");
        final_shader.push_str("#version 450\n\n");

        // Add uniforms
        final_shader.push_str("layout(set = 0, binding = 0) uniform Uniforms {\n");
        final_shader.push_str("    float time;\n");
        final_shader.push_str("    vec2 resolution;\n");
        final_shader.push_str("} uniforms;\n\n");

        // Add shader parts
        for part in shader_parts {
            final_shader.push_str(&part);
            final_shader.push_str("\n");
        }

        // Add main function
        final_shader.push_str("void main() {\n");
        final_shader.push_str("    vec2 uv = (gl_FragCoord.xy - 0.5 * uniforms.resolution) / uniforms.resolution.y;\n");
        final_shader.push_str("    vec3 color = evaluate_output(uv);\n");
        final_shader.push_str("    gl_FragColor = vec4(color, 1.0);\n");
        final_shader.push_str("}\n");

        Ok(final_shader)
    }

    /// Recursively evaluate a node
    fn evaluate_node(
        &self,
        node_id: NodeId,
        evaluated: &mut std::collections::HashSet<NodeId>,
        shader_parts: &mut Vec<String>,
    ) -> Result<String, String> {
        if evaluated.contains(&node_id) {
            return Ok(format!("node_{}", node_id.0));
        }

        let node = &self.nodes[&node_id];
        evaluated.insert(node_id);

        // Evaluate inputs first
        let mut input_values = HashMap::new();
        for (input_name, input) in &node.inputs {
            if let Some(connection) = &input.connected_from {
                let input_value = self.evaluate_node(connection.from_node, evaluated, shader_parts)?;
                input_values.insert(input_name.clone(), input_value);
            } else {
                input_values.insert(input_name.clone(), format!("{}", input.default_value));
            }
        }

        // Generate shader code for this node
        let node_code = self.generate_node_code(node, &input_values)?;
        shader_parts.push(node_code);

        Ok(format!("node_{}_output", node_id.0))
    }

    /// Generate shader code for a node
    fn generate_node_code(
        &self,
        node: &FractalNode,
        input_values: &HashMap<String, String>,
    ) -> Result<String, String> {
        let mut code = String::new();

        match &node.node_type {
            NodeType::FractalGenerator(shader_name) => {
                code.push_str(&format!("// Fractal Generator: {}\n", shader_name));
                code.push_str(&format!("vec3 node_{}_output(vec2 uv) {{\n", node.id.0));
                code.push_str(&format!("    // {} fractal implementation\n", shader_name));
                code.push_str("    return vec3(uv.x, uv.y, 0.5);\n"); // Placeholder
                code.push_str("}\n");
            }
            NodeType::MathOp(op) => {
                let a_default = "0.0".to_string();
                let b_default = "0.0".to_string();
                let a = input_values.get("a").unwrap_or(&a_default).clone();
                let b = input_values.get("b").unwrap_or(&b_default).clone();

                code.push_str(&format!("// Math Operation: {:?}\n", op));
                code.push_str(&format!("float node_{}_output(vec2 uv) {{\n", node.id.0));
                code.push_str(&format!("    float a = {};\n", a));
                code.push_str(&format!("    float b = {};\n", b));

                match op {
                    MathOperation::Add => code.push_str("    return a + b;\n"),
                    MathOperation::Subtract => code.push_str("    return a - b;\n"),
                    MathOperation::Multiply => code.push_str("    return a * b;\n"),
                    MathOperation::Divide => code.push_str("    return a / b;\n"),
                    MathOperation::Sin => code.push_str("    return sin(a);\n"),
                    MathOperation::Cos => code.push_str("    return cos(a);\n"),
                    MathOperation::Tan => code.push_str("    return tan(a);\n"),
                    MathOperation::Pow => code.push_str("    return pow(a, b);\n"),
                    MathOperation::Sqrt => code.push_str("    return sqrt(a);\n"),
                    MathOperation::Abs => code.push_str("    return abs(a);\n"),
                }
                code.push_str("}\n");
            }
            NodeType::ColorAdjust(adjust) => {
                code.push_str(&format!("// Color Adjustment: {:?}\n", adjust));
                code.push_str(&format!("vec3 node_{}_output(vec2 uv) {{\n", node.id.0));
                code.push_str("    vec3 color = vec3(uv.x, uv.y, 0.5);\n"); // Placeholder input
                match adjust {
                    ColorAdjustment::Brightness => {
                        let brightness = node.parameters.get("brightness").unwrap_or(&1.0);
                        code.push_str(&format!("    return color * {};\n", brightness));
                    }
                    ColorAdjustment::Contrast => {
                        let contrast = node.parameters.get("contrast").unwrap_or(&1.0);
                        code.push_str(&format!("    return (color - 0.5) * {} + 0.5;\n", contrast));
                    }
                    _ => code.push_str("    return color;\n"),
                }
                code.push_str("}\n");
            }
            NodeType::Output => {
                code.push_str("// Output Node\n");
                code.push_str(&format!("vec3 node_{}_output(vec2 uv) {{\n", node.id.0));
                if let Some(input_value) = input_values.get("color") {
                    code.push_str(&format!("    return {};\n", input_value));
                } else {
                    code.push_str("    return vec3(uv.x, uv.y, 0.5);\n");
                }
                code.push_str("}\n");
            }
            _ => {
                code.push_str(&format!("// Node type: {:?}\n", node.node_type));
                code.push_str(&format!("vec3 node_{}_output(vec2 uv) {{\n", node.id.0));
                code.push_str("    return vec3(0.5);\n");
                code.push_str("}\n");
            }
        }

        Ok(code)
    }
}

impl FractalNode {
    /// Create a new node
    pub fn new(id: NodeId, node_type: NodeType, position: Vec2) -> Self {
        let (inputs, outputs) = Self::get_sockets(&node_type);

        Self {
            id,
            node_type,
            position,
            inputs,
            outputs,
            parameters: HashMap::new(),
        }
    }

    /// Get input and output sockets for a node type
    fn get_sockets(node_type: &NodeType) -> (HashMap<String, NodeInput>, HashMap<String, NodeOutput>) {
        match node_type {
            NodeType::FractalGenerator(_) => (
                HashMap::from([
                    ("time".to_string(), NodeInput {
                        name: "time".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("zoom".to_string(), NodeInput {
                        name: "zoom".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                ]),
                HashMap::from([
                    ("color".to_string(), NodeOutput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::MathOp(_) => (
                HashMap::from([
                    ("a".to_string(), NodeInput {
                        name: "a".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("b".to_string(), NodeInput {
                        name: "b".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("result".to_string(), NodeOutput {
                        name: "result".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::ColorAdjust(_) => (
                HashMap::from([
                    ("color".to_string(), NodeInput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("color".to_string(), NodeOutput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::Output => (
                HashMap::from([
                    ("color".to_string(), NodeInput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::new(),
            ),
            _ => (HashMap::new(), HashMap::new()),
        }
    }
}

/// Plugin for the node system
pub struct NodeSystemPlugin;

impl Plugin for NodeSystemPlugin {
    fn build(&self, app: &mut App) {
        app
            .init_resource::<NodeGraph>()
            .add_systems(Update, update_node_graph);
    }
}

/// Update the node graph
fn update_node_graph(
    mut node_graph: ResMut<NodeGraph>,
    time: Res<Time>,
) {
    // Update time-based parameters
    for node in node_graph.nodes.values_mut() {
        if let Some(time_param) = node.parameters.get_mut("time") {
            *time_param = time.elapsed_secs();
        }
    }
}

/// Create a simple fractal composition
pub fn create_sample_composition(node_graph: &mut NodeGraph) {
    // Create nodes
    let mandelbrot_id = node_graph.create_node(
        NodeType::FractalGenerator("mandelbrot".to_string()),
        Vec2::new(100.0, 100.0)
    );

    let brightness_id = node_graph.create_node(
        NodeType::ColorAdjust(ColorAdjustment::Brightness),
        Vec2::new(300.0, 100.0)
    );

    let output_id = node_graph.create_node(
        NodeType::Output,
        Vec2::new(500.0, 100.0)
    );

    // Connect nodes
    let _ = node_graph.connect_nodes(mandelbrot_id, "color", brightness_id, "color");
    let _ = node_graph.connect_nodes(brightness_id, "color", output_id, "color");

    // Set parameters
    if let Some(node) = node_graph.nodes.get_mut(&brightness_id) {
        node.parameters.insert("brightness".to_string(), 1.5);
    }
}