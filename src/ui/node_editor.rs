//! Node-Based Editor for Fractal Composition
//!
//! This module provides a comprehensive node-based visual programming interface
//! for creating complex fractal compositions, adapted from TouchDesigner and Unreal Engine.

use egui::{Color32, Pos2, Vec2, Rect, Ui, Response, Painter, Stroke, FontId, RichText, FontFamily, Shape, Sense, Id, Rounding, Align2, ScrollArea};
use std::collections::HashMap;

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub struct NodeId(pub usize);

/// Main node editor state
#[derive(Clone)]
pub struct NodeEditor {
    pub nodes: Vec<Node>,
    pub connections: Vec<NodeConnection>,
    pub selected_nodes: Vec<NodeId>,
    pub dragged_node: Option<NodeId>,
    pub drag_offset: Vec2,
    pub pan_offset: Vec2,
    pub zoom: f32,
    pub show_grid: bool,
    pub grid_size: f32,
    pub pending_connection: Option<PendingConnection>,
    pub node_library: NodeLibrary,
}

#[derive(Debug, Clone)]
pub struct Node {
    pub id: NodeId,
    pub position: Pos2,
    pub size: egui::Vec2,
    pub node_type: NodeType,
    pub title: String,
    pub inputs: Vec<NodePin>,
    pub outputs: Vec<NodePin>,
    pub parameters: HashMap<String, NodeParameter>,
    pub color: Color32,
}

#[derive(Debug, Clone)]
pub struct NodePin {
    pub id: PinId,
    pub name: String,
    pub pin_type: PinType,
    pub position: Pos2,
    pub connected: bool,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub struct PinId(pub usize);

#[derive(Debug, Clone)]
pub enum PinType {
    Geometry,
    Color,
    Numeric,
    Vector,
    Texture,
    Material,
    Light,
    Camera,
    Animation,
}

#[derive(Debug, Clone)]
pub struct NodeConnection {
    pub from_node: NodeId,
    pub from_pin: PinId,
    pub to_node: NodeId,
    pub to_pin: PinId,
}

#[derive(Debug, Clone)]
pub struct PendingConnection {
    pub from_node: NodeId,
    pub from_pin: PinId,
    pub from_pos: Pos2,
    pub to_pos: Pos2,
}

#[derive(Debug, Clone)]
pub enum NodeType {
    // Fractal Generation
    Mandelbulb,
    Mandelbox,
    IFS,
    QuaternionJulia,
    Mandelbrott,
    CustomFormula,

    // Mathematical Operations
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Sin,
    Cos,
    Tan,
    Sqrt,
    Abs,

    // Vector Operations
    VectorAdd,
    VectorSubtract,
    VectorMultiply,
    VectorDot,
    VectorCross,
    VectorNormalize,
    VectorLength,

    // Transformations
    Translate,
    Rotate,
    Scale,
    TransformMatrix,

    // Color Operations
    ColorMix,
    ColorAdjust,
    ColorGradient,
    ColorRamp,

    // Material Properties
    PBRMaterial,
    Metallic,
    Roughness,
    Emission,
    NormalMap,

    // Lighting
    DirectionalLight,
    PointLight,
    SpotLight,
    EnvironmentLight,

    // Camera
    Camera,
    CameraController,

    // Animation
    Time,
    LFO,
    Noise,
    Keyframe,

    // Post Processing
    Bloom,
    DepthOfField,
    Vignette,
    ColorGrading,

    // Utility
    Constant,
    Variable,
    Switch,
    Merge,
    Split,
}

#[derive(Debug, Clone)]
pub enum NodeParameter {
    Float(f32),
    Int(i32),
    Bool(bool),
    Vec2([f32; 2]),
    Vec3([f32; 3]),
    Vec4([f32; 4]),
    Color([f32; 4]),
    String(String),
}

#[derive(Clone)]
pub struct NodeLibrary {
    pub categories: Vec<NodeCategory>,
}

#[derive(Clone)]
pub struct NodeCategory {
    pub name: String,
    pub nodes: Vec<NodeTemplate>,
}

#[derive(Clone)]
pub struct NodeTemplate {
    pub name: String,
    pub description: String,
    pub node_type: NodeType,
    pub color: Color32,
}

impl NodeEditor {
    pub fn new() -> Self {
        Self {
            nodes: Vec::new(),
            connections: Vec::new(),
            selected_nodes: Vec::new(),
            dragged_node: None,
            drag_offset: egui::Vec2::ZERO,
            pan_offset: egui::Vec2::ZERO,
            zoom: 1.0,
            show_grid: true,
            grid_size: 20.0,
            pending_connection: None,
            node_library: Self::create_node_library(),
        }
    }

    pub fn show(&mut self, ui: &mut Ui, size: Vec2) {
        let (rect, response) = ui.allocate_exact_size(size, egui::Sense::click_and_drag());

        // Handle input
        self.handle_input(ui, &response);

        // Draw background
        self.draw_background(ui, rect);

        // Draw grid
        if self.show_grid {
            self.draw_grid(ui, rect);
        }

        // Draw connections
        self.draw_connections(ui, rect);

        // Draw pending connection
        if let Some(pending) = &self.pending_connection {
            self.draw_pending_connection(ui, pending);
        }

        // Draw nodes
        let nodes_to_draw = self.nodes.clone();
        for node in &nodes_to_draw {
            self.draw_node(ui, node, rect);
        }

        // Draw node library panel
        self.draw_node_library(ui, rect);

        // Execute node graph if we have nodes
        // self.execute_node_graph(ui);
    }

    fn handle_input(&mut self, ui: &Ui, response: &Response) {
        let input = ui.input(|i| i.clone());

        // Pan with middle mouse
        if response.dragged_by(egui::PointerButton::Middle) {
            self.pan_offset += response.drag_delta();
        }

        // Zoom with mouse wheel
        if let Some(mouse_pos) = response.hover_pos() {
            let zoom_delta = input.raw_scroll_delta.y * 0.001;
            if zoom_delta != 0.0 {
                let old_zoom = self.zoom;
                self.zoom = (self.zoom * (1.0 + zoom_delta)).clamp(0.1, 5.0);

                // Zoom towards mouse position
                let zoom_factor = self.zoom / old_zoom;
                let mouse_world = (mouse_pos - self.pan_offset) / old_zoom;
                let new_mouse_world = mouse_world * zoom_factor;
                self.pan_offset += (mouse_world - new_mouse_world) * old_zoom;
            }
        }

        // Handle node selection and dragging
        if response.clicked() && !input.modifiers.shift {
            self.selected_nodes.clear();
        }

        // Handle pending connections
        if response.clicked() && self.pending_connection.is_some() {
            self.pending_connection = None;
        }

        // Handle pin connections
        if let Some(mouse_pos) = response.hover_pos() {
            self.handle_pin_connections(ui, mouse_pos, response);
        }
    }

    fn draw_background(&self, ui: &Ui, rect: Rect) {
        ui.painter().rect_filled(
            rect,
            0.0,
            Color32::from_rgb(25, 28, 35),
        );
    }

    fn draw_grid(&self, ui: &Ui, rect: Rect) {
        let painter = ui.painter();
        let grid_color = Color32::from_rgb(45, 50, 60);
        let grid_size = self.grid_size * self.zoom;

        let start_x = (rect.left() + self.pan_offset.x) % grid_size;
        let start_y = (rect.top() + self.pan_offset.y) % grid_size;

        // Vertical lines
        for x in (0..((rect.width() / grid_size) as i32 + 2)).map(|i| start_x + i as f32 * grid_size) {
            painter.line_segment(
                [Pos2::new(x, rect.top()), Pos2::new(x, rect.bottom())],
                Stroke::new(1.0, grid_color),
            );
        }

        // Horizontal lines
        for y in (0..((rect.height() / grid_size) as i32 + 2)).map(|i| start_y + i as f32 * grid_size) {
            painter.line_segment(
                [Pos2::new(rect.left(), y), Pos2::new(rect.right(), y)],
                Stroke::new(1.0, grid_color),
            );
        }
    }

    fn draw_connections(&self, ui: &Ui, rect: Rect) {
        let painter = ui.painter();

        for connection in &self.connections {
            if let (Some(from_node), Some(to_node)) = (
                self.nodes.iter().find(|n| n.id == connection.from_node),
                self.nodes.iter().find(|n| n.id == connection.to_node),
            ) {
                if let (Some(from_pin), Some(to_pin)) = (
                    from_node.outputs.iter().find(|p| p.id == connection.from_pin),
                    to_node.inputs.iter().find(|p| p.id == connection.to_pin),
                ) {
                    let from_pos = self.world_to_screen(from_pin.position, rect);
                    let to_pos = self.world_to_screen(to_pin.position, rect);

                    // Draw bezier curve
                    self.draw_connection_curve(painter, from_pos, to_pos, Color32::from_rgb(100, 150, 255));
                }
            }
        }
    }

    fn draw_pending_connection(&self, ui: &Ui, pending: &PendingConnection) {
        if let Some(from_node) = self.nodes.iter().find(|n| n.id == pending.from_node) {
            // Check if it's an output pin connection
            if let Some(from_pin) = from_node.outputs.iter().find(|p| p.id == pending.from_pin) {
                let painter = ui.painter();
                let from_pos = self.world_to_screen(from_pin.position + from_node.position.to_vec2(), ui.clip_rect());
                let to_pos = pending.to_pos;

                self.draw_connection_curve(painter, from_pos, to_pos, Color32::from_rgb(255, 200, 100));
            }
            // Check if it's an input pin connection (reverse connection)
            else if let Some(from_pin) = from_node.inputs.iter().find(|p| p.id == pending.from_pin) {
                let painter = ui.painter();
                let from_pos = self.world_to_screen(from_pin.position + from_node.position.to_vec2(), ui.clip_rect());
                let to_pos = pending.to_pos;

                self.draw_connection_curve(painter, from_pos, to_pos, Color32::from_rgb(255, 200, 100));
            }
        }
    }

    fn draw_connection_curve(&self, painter: &Painter, from: Pos2, to: Pos2, color: Color32) {
        let control_point_1 = Pos2::new(from.x + 50.0, from.y);
        let control_point_2 = Pos2::new(to.x - 50.0, to.y);

        // Simple bezier curve approximation
        let steps = 20;
        let mut points = Vec::new();

        for i in 0..=steps {
            let t = i as f32 / steps as f32;
            let x = (1.0 - t).powi(3) * from.x +
                   3.0 * (1.0 - t).powi(2) * t * control_point_1.x +
                   3.0 * (1.0 - t) * t.powi(2) * control_point_2.x +
                   t.powi(3) * to.x;
            let y = (1.0 - t).powi(3) * from.y +
                   3.0 * (1.0 - t).powi(2) * t * control_point_1.y +
                   3.0 * (1.0 - t) * t.powi(2) * control_point_2.y +
                   t.powi(3) * to.y;
            points.push(Pos2::new(x, y));
        }

        painter.add(egui::Shape::line(points, Stroke::new(2.0, color)));
    }

    fn draw_node(&mut self, ui: &Ui, node: &Node, canvas_rect: Rect) {
        let node = node.clone();
        let node_rect = Rect::from_min_size(
            self.world_to_screen(node.position, canvas_rect),
            node.size * self.zoom,
        );

        let painter = ui.painter();
        let is_selected = self.selected_nodes.contains(&node.id);

        // Node background
        let bg_color = if is_selected {
            Color32::from_rgb(80, 120, 180)
        } else {
            node.color
        };

        painter.rect_filled(node_rect, Rounding::same(8), bg_color);

        // Node title
        painter.text(
            node_rect.min + egui::Vec2::new(8.0, 8.0),
            egui::Align2::LEFT_TOP,
            &node.title,
            FontId::proportional(14.0),
            Color32::WHITE,
        );

        // Draw pins
        for input in &node.inputs {
            self.draw_pin(painter, input, node_rect.min, true);
        }

        for output in &node.outputs {
            self.draw_pin(painter, output, node_rect.min, false);
        }

        // Handle node interaction - make sure this works
        let response = ui.interact(node_rect, egui::Id::new(node.id), egui::Sense::click_and_drag());

        if response.clicked() {
            if ui.input(|i| i.modifiers.shift) {
                if let Some(pos) = self.selected_nodes.iter().position(|&id| id == node.id) {
                    self.selected_nodes.remove(pos);
                } else {
                    self.selected_nodes.push(node.id);
                }
            } else {
                self.selected_nodes = vec![node.id];
            }
        }

        if response.dragged() && !self.selected_nodes.is_empty() {
            let delta = response.drag_delta() / self.zoom;
            for &node_id in &self.selected_nodes {
                if let Some(node) = self.nodes.iter_mut().find(|n| n.id == node_id) {
                    node.position += delta;
                }
            }
        }
    }

    fn draw_pin(&self, painter: &Painter, pin: &NodePin, node_pos: Pos2, is_input: bool) {
        let pin_color = match pin.pin_type {
            PinType::Geometry => Color32::from_rgb(100, 200, 100),
            PinType::Color => Color32::from_rgb(200, 150, 100),
            PinType::Numeric => Color32::from_rgb(150, 150, 200),
            PinType::Vector => Color32::from_rgb(200, 100, 150),
            PinType::Texture => Color32::from_rgb(150, 200, 200),
            _ => Color32::from_rgb(180, 180, 180),
        };

        let pin_pos = if is_input {
            node_pos + egui::Vec2::new(0.0, pin.position.y)
        } else {
            node_pos + egui::Vec2::new(pin.position.x, pin.position.y)
        };

        // Highlight pin if connected
        let final_color = if pin.connected {
            Color32::from_rgb(
                (pin_color.r() as f32 * 1.2).min(255.0) as u8,
                (pin_color.g() as f32 * 1.2).min(255.0) as u8,
                (pin_color.b() as f32 * 1.2).min(255.0) as u8,
            )
        } else {
            pin_color
        };

        painter.circle_filled(pin_pos, 6.0, final_color);
        painter.circle_stroke(pin_pos, 6.0, Stroke::new(2.0, Color32::WHITE));

        // Pin label
        let label_pos = if is_input {
            pin_pos + egui::Vec2::new(12.0, -4.0)
        } else {
            pin_pos + egui::Vec2::new(-12.0 - 50.0, -4.0) // Simplified to avoid font layout issues
        };

        painter.text(
            label_pos,
            if is_input { egui::Align2::LEFT_CENTER } else { egui::Align2::RIGHT_CENTER },
            &pin.name,
            FontId::proportional(10.0),
            Color32::WHITE,
        );
    }

    fn draw_node_library(&mut self, ui: &mut Ui, canvas_rect: Rect) {
        let library_rect = Rect::from_min_size(
            canvas_rect.right_top() - egui::Vec2::new(200.0, 0.0),
            egui::Vec2::new(200.0, canvas_rect.height()),
        );

        // Semi-transparent background
        ui.painter().rect_filled(
            library_rect,
            0.0,
            Color32::from_rgba_premultiplied(30, 35, 45, 200),
        );

        ui.scope_builder(egui::UiBuilder::new().max_rect(library_rect), |ui| {
            ui.set_min_width(180.0);

            ui.label(RichText::new("🎨 Node Library").size(14.0));
            ui.separator();

            egui::ScrollArea::vertical().show(ui, |ui| {
                let categories = self.node_library.categories.clone();
                for category in &categories {
                    ui.collapsing(RichText::new(&category.name).size(12.0), |ui| {
                        for template in &category.nodes {
                            let response = ui.add(
                                egui::Button::new(&template.name)
                                    .fill(template.color.linear_multiply(0.8))
                                    .stroke(Stroke::new(1.0, template.color))
                            );

                            if response.clicked() {
                                self.add_node_from_template(template, canvas_rect.center());
                            }

                            response.on_hover_text(&template.description);
                        }
                    });
                }
            });
        });
    }

    pub fn add_node_from_template(&mut self, template: &NodeTemplate, position: Pos2) {
        let node_id = NodeId(self.nodes.len());

        let node = Node {
            id: node_id,
            position,
            size: egui::Vec2::new(150.0, 100.0),
            node_type: template.node_type.clone(),
            title: template.name.clone(),
            inputs: self.create_node_inputs(&template.node_type),
            outputs: self.create_node_outputs(&template.node_type),
            parameters: HashMap::new(),
            color: template.color,
        };

        self.nodes.push(node);
    }

    fn create_node_inputs(&self, node_type: &NodeType) -> Vec<NodePin> {
        match node_type {
            NodeType::Mandelbulb => vec![
                NodePin {
                    id: PinId(0),
                    name: "Power".to_string(),
                    pin_type: PinType::Numeric,
                    position: Pos2::new(0.0, 30.0),
                    connected: false,
                },
                NodePin {
                    id: PinId(1),
                    name: "Iterations".to_string(),
                    pin_type: PinType::Numeric,
                    position: Pos2::new(0.0, 50.0),
                    connected: false,
                },
            ],
            NodeType::Add => vec![
                NodePin {
                    id: PinId(0),
                    name: "A".to_string(),
                    pin_type: PinType::Numeric,
                    position: Pos2::new(0.0, 30.0),
                    connected: false,
                },
                NodePin {
                    id: PinId(1),
                    name: "B".to_string(),
                    pin_type: PinType::Numeric,
                    position: Pos2::new(0.0, 50.0),
                    connected: false,
                },
            ],
            _ => vec![],
        }
    }

    fn create_node_outputs(&self, node_type: &NodeType) -> Vec<NodePin> {
        match node_type {
            NodeType::Mandelbulb => vec![
                NodePin {
                    id: PinId(0),
                    name: "Geometry".to_string(),
                    pin_type: PinType::Geometry,
                    position: Pos2::new(150.0, 40.0),
                    connected: false,
                },
            ],
            NodeType::Add => vec![
                NodePin {
                    id: PinId(0),
                    name: "Result".to_string(),
                    pin_type: PinType::Numeric,
                    position: Pos2::new(150.0, 40.0),
                    connected: false,
                },
            ],
            _ => vec![],
        }
    }

    fn world_to_screen(&self, world_pos: Pos2, canvas_rect: Rect) -> Pos2 {
        canvas_rect.min + (world_pos.to_vec2() + self.pan_offset) * self.zoom
    }

    fn screen_to_world(&self, screen_pos: Pos2, canvas_rect: Rect) -> Pos2 {
        Pos2::new(
            (screen_pos.x - canvas_rect.min.x - self.pan_offset.x) / self.zoom,
            (screen_pos.y - canvas_rect.min.y - self.pan_offset.y) / self.zoom,
        )
    }

    fn handle_pin_connections(&mut self, _ui: &Ui, mouse_pos: Pos2, response: &Response) {
        let canvas_rect = response.rect;

        // Check for pin clicks to start connections
        for node in &self.nodes.clone() {
            let node_screen_pos = self.world_to_screen(node.position, canvas_rect);
            let _node_rect = Rect::from_min_size(node_screen_pos, node.size * self.zoom);

            // Check input pins
            for input_pin in &node.inputs {
                let pin_screen_pos = node_screen_pos + egui::Vec2::new(0.0, input_pin.position.y * self.zoom);
                let pin_rect = Rect::from_center_size(pin_screen_pos, egui::Vec2::new(12.0, 12.0));

                if pin_rect.contains(mouse_pos) && response.clicked_by(egui::PointerButton::Primary) {
                    // Start connection from input pin (unusual but allowed)
                    self.pending_connection = Some(PendingConnection {
                        from_node: node.id,
                        from_pin: input_pin.id,
                        from_pos: pin_screen_pos,
                        to_pos: mouse_pos,
                    });
                    return;
                }
            }

            // Check output pins
            for output_pin in &node.outputs {
                let pin_screen_pos = node_screen_pos + egui::Vec2::new(output_pin.position.x * self.zoom, output_pin.position.y * self.zoom);
                let pin_rect = Rect::from_center_size(pin_screen_pos, egui::Vec2::new(12.0, 12.0));

                if pin_rect.contains(mouse_pos) && response.clicked_by(egui::PointerButton::Primary) {
                    // Start connection from output pin
                    self.pending_connection = Some(PendingConnection {
                        from_node: node.id,
                        from_pin: output_pin.id,
                        from_pos: pin_screen_pos,
                        to_pos: mouse_pos,
                    });
                    return;
                }
            }
        }

        // Handle connection completion
        if let Some(pending) = &self.pending_connection.clone() {
            if response.clicked_by(egui::PointerButton::Primary) {
                // Try to complete connection
                for node in &self.nodes {
                    let node_screen_pos = self.world_to_screen(node.position, canvas_rect);
                    let _node_rect = Rect::from_min_size(node_screen_pos, node.size * self.zoom);

                    // Check input pins for connection target
                    for input_pin in &node.inputs {
                        let pin_screen_pos = node_screen_pos + egui::Vec2::new(0.0, input_pin.position.y * self.zoom);
                        let pin_rect = Rect::from_center_size(pin_screen_pos, egui::Vec2::new(12.0, 12.0));

                        if pin_rect.contains(mouse_pos) && node.id != pending.from_node {
                            // Create connection
                            self.connections.push(NodeConnection {
                                from_node: pending.from_node,
                                from_pin: pending.from_pin,
                                to_node: node.id,
                                to_pin: input_pin.id,
                            });

                            // Mark pins as connected - collect node IDs first to avoid borrowing issues
                            let from_node_id = pending.from_node;
                            let to_node_id = node.id;
                            let from_pin_id = pending.from_pin;
                            let to_pin_id = input_pin.id;

                            // Update pin connection status
                            self.mark_pin_connected(from_node_id, from_pin_id, true);
                            self.mark_pin_connected(to_node_id, to_pin_id, true);

                            self.pending_connection = None;
                            return;
                        }
                    }
                }

                // No valid connection target found, cancel pending connection
                self.pending_connection = None;
            } else if response.dragged_by(egui::PointerButton::Primary) {
                // Update pending connection position
                if let Some(pending) = self.pending_connection.as_mut() {
                    pending.to_pos = mouse_pos;
                }
            }
        }
    }

    fn mark_pin_connected(&mut self, node_id: NodeId, pin_id: PinId, connected: bool) {
        if let Some(node) = self.nodes.iter_mut().find(|n| n.id == node_id) {
            // Check outputs first
            if let Some(pin) = node.outputs.iter_mut().find(|p| p.id == pin_id) {
                pin.connected = connected;
                return;
            }
            // Then check inputs
            if let Some(pin) = node.inputs.iter_mut().find(|p| p.id == pin_id) {
                pin.connected = connected;
                return;
            }
        }
    }

    fn execute_node_graph(&mut self, ui: &Ui) {
        // Simple execution for demonstration - execute all nodes
        for node in &self.nodes {
            self.execute_node(node, ui);
        }
    }

    fn execute_node(&self, node: &Node, ui: &Ui) {
        // Basic node execution - in a real implementation this would be much more sophisticated
        match &node.node_type {
            NodeType::Mandelbulb => {
                // Simulate mandelbulb computation
                let power = node.parameters.get("power").and_then(|p| match p {
                    NodeParameter::Float(f) => Some(*f),
                    _ => None,
                }).unwrap_or(8.0);

                let iterations = node.parameters.get("iterations").and_then(|p| match p {
                    NodeParameter::Int(i) => Some(*i as u32),
                    _ => None,
                }).unwrap_or(50);

                // This would normally generate actual fractal data
                // For now, just log the execution
                log::debug!("Executing Mandelbulb: power={}, iterations={}", power, iterations);
            }
            NodeType::Add => {
                // Get connected inputs and compute result
                let mut result = 0.0;
                for connection in &self.connections {
                    if connection.to_node == node.id {
                        // Find the source node and get its output value
                        if let Some(source_node) = self.nodes.iter().find(|n| n.id == connection.from_node) {
                            if let Some(output_value) = self.get_node_output_value(source_node) {
                                result += output_value;
                            }
                        }
                    }
                }
                log::debug!("Add node result: {}", result);
            }
            _ => {
                // Other node types would be handled here
            }
        }
    }

    fn get_node_output_value(&self, node: &Node) -> Option<f32> {
        match &node.node_type {
            NodeType::Constant => {
                node.parameters.get("value").and_then(|p| match p {
                    NodeParameter::Float(f) => Some(*f),
                    _ => None,
                })
            }
            NodeType::Sin => {
                // Get input from connections
                let mut input = 0.0;
                for connection in &self.connections {
                    if connection.to_node == node.id {
                        if let Some(source_node) = self.nodes.iter().find(|n| n.id == connection.from_node) {
                            if let Some(val) = self.get_node_output_value(source_node) {
                                input = val;
                                break;
                            }
                        }
                    }
                }
                Some(input.sin())
            }
            _ => None,
        }
    }

    fn create_node_library() -> NodeLibrary {
        NodeLibrary {
            categories: vec![
                NodeCategory {
                    name: "Fractals".to_string(),
                    nodes: vec![
                        NodeTemplate {
                            name: "Mandelbulb".to_string(),
                            description: "3D Mandelbrot fractal".to_string(),
                            node_type: NodeType::Mandelbulb,
                            color: Color32::from_rgb(100, 150, 200),
                        },
                        NodeTemplate {
                            name: "Mandelbox".to_string(),
                            description: "Box folding fractal".to_string(),
                            node_type: NodeType::Mandelbox,
                            color: Color32::from_rgb(150, 100, 200),
                        },
                        NodeTemplate {
                            name: "IFS".to_string(),
                            description: "Iterated function system".to_string(),
                            node_type: NodeType::IFS,
                            color: Color32::from_rgb(200, 150, 100),
                        },
                        NodeTemplate {
                            name: "Quaternion Julia".to_string(),
                            description: "4D quaternion fractal".to_string(),
                            node_type: NodeType::QuaternionJulia,
                            color: Color32::from_rgb(200, 100, 150),
                        },
                    ],
                },
                NodeCategory {
                    name: "Math".to_string(),
                    nodes: vec![
                        NodeTemplate {
                            name: "Add".to_string(),
                            description: "Add two values".to_string(),
                            node_type: NodeType::Add,
                            color: Color32::from_rgb(100, 200, 100),
                        },
                        NodeTemplate {
                            name: "Sin".to_string(),
                            description: "Sine function".to_string(),
                            node_type: NodeType::Sin,
                            color: Color32::from_rgb(200, 100, 100),
                        },
                        NodeTemplate {
                            name: "Constant".to_string(),
                            description: "Constant numeric value".to_string(),
                            node_type: NodeType::Constant,
                            color: Color32::from_rgb(150, 150, 150),
                        },
                    ],
                },
                NodeCategory {
                    name: "Color".to_string(),
                    nodes: vec![
                        NodeTemplate {
                            name: "Color Mix".to_string(),
                            description: "Mix two colors".to_string(),
                            node_type: NodeType::ColorMix,
                            color: Color32::from_rgb(200, 150, 100),
                        },
                        NodeTemplate {
                            name: "Color Adjust".to_string(),
                            description: "Adjust brightness/contrast".to_string(),
                            node_type: NodeType::ColorAdjust,
                            color: Color32::from_rgb(150, 200, 100),
                        },
                    ],
                },
                NodeCategory {
                    name: "Animation".to_string(),
                    nodes: vec![
                        NodeTemplate {
                            name: "Time".to_string(),
                            description: "Current time value".to_string(),
                            node_type: NodeType::Time,
                            color: Color32::from_rgb(100, 100, 200),
                        },
                        NodeTemplate {
                            name: "LFO".to_string(),
                            description: "Low frequency oscillator".to_string(),
                            node_type: NodeType::LFO,
                            color: Color32::from_rgb(200, 100, 200),
                        },
                    ],
                },
            ],
        }
    }
}