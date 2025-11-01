//! Fractal-specific Node Types
//!
//! This module defines specialized node types for fractal generation,
//! including formula nodes, transformation nodes, and material nodes.

use super::node_editor::*;
use crate::fractal::types::*;
use std::collections::HashMap;

/// Fractal formula node types
#[derive(Debug, Clone)]
pub enum FractalNodeType {
    /// Mandelbrot set formula
    Mandelbrot {
        center: [f32; 2],
        zoom: f32,
    },
    /// 3D Mandelbulb formula
    Mandelbulb {
        power: f32,
    },
    /// Mandelbox formula
    Mandelbox {
        scale: f32,
        folding_limit: f32,
    },
    /// Iterated Function System
    IFS {
        transforms: Vec<Matrix4x4>,
    },
    /// Quaternion Julia set
    QuaternionJulia {
        c: [f32; 4],
    },
    /// Hybrid fractal combiner
    HybridCombiner {
        blend_mode: BlendMode,
        blend_factor: f32,
    },
}

/// Transformation node types
#[derive(Debug, Clone)]
pub enum TransformNodeType {
    /// 3D translation
    Translate {
        offset: [f32; 3],
    },
    /// 3D rotation (Euler angles)
    Rotate {
        angles: [f32; 3],
    },
    /// 3D scale
    Scale {
        scale: [f32; 3],
    },
    /// Spherical coordinates transformation
    Spherical,
    /// Polar coordinates transformation
    Polar,
}

/// Material and coloring node types
#[derive(Debug, Clone)]
pub enum MaterialNodeType {
    /// Physically-based material
    PBRMaterial {
        base_color: [f32; 3],
        metallic: f32,
        roughness: f32,
        emissive: [f32; 3],
    },
    /// Iteration-based coloring
    IterationColor {
        palette: Vec<[f32; 3]>,
        cycle_speed: f32,
        saturation: f32,
    },
    /// Distance-based coloring
    DistanceColor {
        near_color: [f32; 3],
        far_color: [f32; 3],
        falloff: f32,
    },
    /// Normal-based coloring
    NormalColor {
        intensity: f32,
    },
}

/// Lighting node types
#[derive(Debug, Clone)]
pub enum LightNodeType {
    /// Directional light
    Directional {
        direction: [f32; 3],
        color: [f32; 3],
        intensity: f32,
    },
    /// Point light
    Point {
        position: [f32; 3],
        color: [f32; 3],
        intensity: f32,
        range: f32,
    },
    /// Spot light
    Spot {
        position: [f32; 3],
        direction: [f32; 3],
        color: [f32; 3],
        intensity: f32,
        range: f32,
        inner_angle: f32,
        outer_angle: f32,
    },
}

/// Animation and control node types
#[derive(Debug, Clone)]
pub enum AnimationNodeType {
    /// Time input node
    Time,
    /// Parameter input node
    Parameter {
        name: String,
        default_value: f32,
        min_value: f32,
        max_value: f32,
    },
    /// LFO (Low Frequency Oscillator)
    LFO {
        frequency: f32,
        amplitude: f32,
        offset: f32,
        waveform: WaveformType,
    },
    /// Noise generator
    Noise {
        frequency: f32,
        amplitude: f32,
        seed: u32,
        noise_type: NoiseType,
    },
    /// Mathematical operations
    Math {
        operation: MathOperation,
    },
}

/// Waveform types for LFO
#[derive(Debug, Clone)]
pub enum WaveformType {
    Sine,
    Triangle,
    Square,
    Sawtooth,
    Random,
}

/// Noise types
#[derive(Debug, Clone)]
pub enum NoiseType {
    Perlin,
    Simplex,
    Worley,
    Value,
}

/// Mathematical operations
#[derive(Debug, Clone)]
pub enum MathOperation {
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Sine,
    Cosine,
    Tangent,
    Logarithm,
    Exponential,
    Absolute,
    Clamp,
}

/// Specialized fractal node implementation
#[derive(Debug, Clone)]
pub struct FractalNode {
    pub id: NodeId,
    pub position: egui::Pos2,
    pub node_type: FractalNodeType,
    pub inputs: Vec<NodePin>,
    pub outputs: Vec<NodePin>,
    pub parameters: HashMap<String, f32>,
}

impl FractalNode {
    /// Create a new Mandelbrot node
    pub fn mandelbrot(position: egui::Pos2) -> Self {
        Self {
            id: NodeId(uuid::Uuid::new_v4().as_u128() as usize),
            position,
            node_type: FractalNodeType::Mandelbrot {
                center: [0.0, 0.0],
                zoom: 1.0,
            },
            inputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Center X".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Center Y".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Zoom".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            outputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Distance".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Iterations".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            parameters: HashMap::new(),
        }
    }

    /// Create a new Mandelbulb node
    pub fn mandelbulb(position: egui::Pos2) -> Self {
        Self {
            id: NodeId(uuid::Uuid::new_v4().as_u128() as usize),
            position,
            node_type: FractalNodeType::Mandelbulb { power: 8.0 },
            inputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Power".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            outputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Distance".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Normal".to_string(),
                    pin_type: PinType::Vec3,
                    position: egui::Pos2::ZERO,
                },
            ],
            parameters: HashMap::new(),
        }
    }

    /// Create a new Mandelbox node
    pub fn mandelbox(position: egui::Pos2) -> Self {
        Self {
            id: NodeId(uuid::Uuid::new_v4().as_u128() as usize),
            position,
            node_type: FractalNodeType::Mandelbox {
                scale: 2.0,
                folding_limit: 1.0,
            },
            inputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Scale".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Folding Limit".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            outputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Distance".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            parameters: HashMap::new(),
        }
    }

    /// Create a new PBR material node
    pub fn pbr_material(position: egui::Pos2) -> Self {
        Self {
            id: NodeId(uuid::Uuid::new_v4().as_u128() as usize),
            position,
            node_type: MaterialNodeType::PBRMaterial {
                base_color: [0.8, 0.6, 1.0],
                metallic: 0.0,
                roughness: 0.5,
                emissive: [0.0, 0.0, 0.0],
            },
            inputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Base Color".to_string(),
                    pin_type: PinType::Color,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Metallic".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Roughness".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Emissive".to_string(),
                    pin_type: PinType::Color,
                    position: egui::Pos2::ZERO,
                },
            ],
            outputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Material".to_string(),
                    pin_type: PinType::Color,
                    position: egui::Pos2::ZERO,
                },
            ],
            parameters: HashMap::new(),
        }
    }

    /// Create a new LFO node
    pub fn lfo(position: egui::Pos2) -> Self {
        Self {
            id: NodeId(uuid::Uuid::new_v4().as_u128() as usize),
            position,
            node_type: AnimationNodeType::LFO {
                frequency: 1.0,
                amplitude: 1.0,
                offset: 0.0,
                waveform: WaveformType::Sine,
            },
            inputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Frequency".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Amplitude".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Time".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            outputs: vec![
                NodePin {
                    id: PinId(uuid::Uuid::new_v4().as_u128() as usize),
                    name: "Output".to_string(),
                    pin_type: PinType::Float,
                    position: egui::Pos2::ZERO,
                },
            ],
            parameters: HashMap::new(),
        }
    }

    /// Get the title for UI display
    pub fn title(&self) -> &str {
        match &self.node_type {
            FractalNodeType::Mandelbrot { .. } => "Mandelbrot",
            FractalNodeType::Mandelbulb { .. } => "Mandelbulb",
            FractalNodeType::Mandelbox { .. } => "Mandelbox",
            FractalNodeType::IFS { .. } => "IFS Fractal",
            FractalNodeType::QuaternionJulia { .. } => "Quaternion Julia",
            FractalNodeType::HybridCombiner { .. } => "Hybrid Combiner",
        }
    }

    /// Get the color for UI display
    pub fn color(&self) -> egui::Color32 {
        match &self.node_type {
            FractalNodeType::Mandelbrot { .. } |
            FractalNodeType::Mandelbulb { .. } |
            FractalNodeType::Mandelbox { .. } |
            FractalNodeType::IFS { .. } |
            FractalNodeType::QuaternionJulia { .. } |
            FractalNodeType::HybridCombiner { .. } => egui::Color32::from_rgb(16, 185, 129), // Green for fractals
        }
    }

    /// Update node parameters from UI
    pub fn update_parameter(&mut self, param_name: &str, value: f32) {
        self.parameters.insert(param_name.to_string(), value);

        // Update internal node type based on parameters
        match &mut self.node_type {
            FractalNodeType::Mandelbulb { power } => {
                if param_name == "Power" {
                    *power = value;
                }
            }
            FractalNodeType::Mandelbox { scale, folding_limit } => {
                match param_name {
                    "Scale" => *scale = value,
                    "Folding Limit" => *folding_limit = value,
                    _ => {}
                }
            }
            _ => {}
        }
    }

    /// Get parameter value
    pub fn get_parameter(&self, param_name: &str) -> f32 {
        self.parameters.get(param_name).copied().unwrap_or_else(|| {
            match &self.node_type {
                FractalNodeType::Mandelbulb { power } => {
                    match param_name {
                        "Power" => *power,
                        _ => 0.0,
                    }
                }
                FractalNodeType::Mandelbox { scale, folding_limit } => {
                    match param_name {
                        "Scale" => *scale,
                        "Folding Limit" => *folding_limit,
                        _ => 0.0,
                    }
                }
                _ => 0.0,
            }
        })
    }

    /// Render the node in the UI
    pub fn render(&mut self, ui: &mut egui::Ui, node_rect: egui::Rect) -> egui::Response {
        let response = ui.allocate_rect(node_rect, egui::Sense::click_and_drag());

        // Node background with glassmorphism effect
        ui.painter().rect_filled(
            node_rect,
            egui::Rounding::same(8.0),
            egui::Color32::from_rgba_premultiplied(35, 35, 38, 200),
        );

        // Node border
        ui.painter().rect_stroke(
            node_rect,
            egui::Rounding::same(8.0),
            egui::Stroke::new(2.0, self.color()),
        );

        // Node title
        let title_rect = egui::Rect::from_min_size(
            node_rect.min + egui::vec2(8.0, 8.0),
            egui::vec2(node_rect.width() - 16.0, 24.0),
        );

        ui.painter().text(
            title_rect.center(),
            egui::Align2::CENTER_CENTER,
            self.title(),
            egui::FontId::proportional(14.0),
            egui::Color32::WHITE,
        );

        // Render input pins
        for (i, input) in self.inputs.iter().enumerate() {
            let pin_pos = node_rect.min + egui::vec2(-8.0, 40.0 + i as f32 * 24.0);
            ui.painter().circle_filled(pin_pos, 6.0, egui::Color32::from_rgb(100, 150, 255));

            // Pin label
            ui.painter().text(
                pin_pos + egui::vec2(12.0, -4.0),
                egui::Align2::LEFT_CENTER,
                &input.name,
                egui::FontId::proportional(12.0),
                egui::Color32::from_rgb(200, 200, 200),
            );
        }

        // Render output pins
        for (i, output) in self.outputs.iter().enumerate() {
            let pin_pos = node_rect.max + egui::vec2(8.0, -40.0 - i as f32 * 24.0);
            ui.painter().circle_filled(pin_pos, 6.0, egui::Color32::from_rgb(255, 150, 100));

            // Pin label
            ui.painter().text(
                pin_pos + egui::vec2(-12.0, -4.0),
                egui::Align2::RIGHT_CENTER,
                &output.name,
                egui::FontId::proportional(12.0),
                egui::Color32::from_rgb(200, 200, 200),
            );
        }

        // Node parameters (simplified)
        let param_rect = egui::Rect::from_min_size(
            node_rect.min + egui::vec2(8.0, 40.0),
            egui::vec2(node_rect.width() - 16.0, node_rect.height() - 80.0),
        );

        // Render parameter controls based on node type
        match &self.node_type {
            FractalNodeType::Mandelbulb { power } => {
                ui.allocate_ui_at_rect(param_rect, |ui| {
                    ui.label("Power:");
                    let mut power_val = *power;
                    if ui.add(egui::DragValue::new(&mut power_val).range(2.0..=16.0)).changed() {
                        self.update_parameter("Power", power_val);
                    }
                });
            }
            FractalNodeType::Mandelbox { scale, folding_limit } => {
                ui.allocate_ui_at_rect(param_rect, |ui| {
                    ui.label("Scale:");
                    let mut scale_val = *scale;
                    if ui.add(egui::DragValue::new(&mut scale_val).range(1.0..=4.0)).changed() {
                        self.update_parameter("Scale", scale_val);
                    }

                    ui.label("Folding Limit:");
                    let mut limit_val = *folding_limit;
                    if ui.add(egui::DragValue::new(&mut limit_val).range(0.5..=2.0)).changed() {
                        self.update_parameter("Folding Limit", limit_val);
                    }
                });
            }
            _ => {}
        }

        response
    }
}

/// Node library for fractal nodes
pub struct FractalNodeLibrary;

impl FractalNodeLibrary {
    /// Get all available fractal node types
    pub fn get_available_nodes() -> Vec<(&'static str, &'static str, fn(egui::Pos2) -> FractalNode)> {
        vec![
            ("Mandelbrot", "Classic 2D fractal", FractalNode::mandelbrot),
            ("Mandelbulb", "3D power fractal", FractalNode::mandelbulb),
            ("Mandelbox", "Folding-based fractal", FractalNode::mandelbox),
            ("PBR Material", "Physically-based material", FractalNode::pbr_material),
            ("LFO", "Low frequency oscillator", FractalNode::lfo),
        ]
    }

    /// Create node by name
    pub fn create_node(name: &str, position: egui::Pos2) -> Option<FractalNode> {
        let nodes = Self::get_available_nodes();
        nodes.iter()
            .find(|(node_name, _, _)| *node_name == name)
            .map(|(_, _, constructor)| constructor(position))
    }
}