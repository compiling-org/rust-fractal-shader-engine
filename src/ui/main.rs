use eframe::egui;
use std::collections::HashMap;
use uuid::Uuid;

use crate::hashmap;

/// Main application for the fractal shader node editor
pub struct FractalShaderApp {
    // Node graph state
    pub node_graph: NodeGraphUI,
    
    // Shader registry
    pub fractal_shaders: HashMap<String, FractalShaderInfo>,
    
    // Application state
    pub show_shader_browser: bool,
    pub show_node_editor: bool,
    pub show_parameter_panels: bool,
    pub selected_shader: Option<String>,
    pub shader_parameters: HashMap<String, f32>,
    
    // Window management
    pub main_window_pos: egui::Pos2,
    pub main_window_size: egui::Vec2,
    
    // Performance metrics
    pub fps: f32,
    pub frame_time: f64,
}

impl FractalShaderApp {
    pub fn new() -> Self {
        let mut app = Self {
            node_graph: NodeGraphUI::new(),
            fractal_shaders: HashMap::new(),
            show_shader_browser: true,
            show_node_editor: true,
            show_parameter_panels: true,
            selected_shader: None,
            shader_parameters: HashMap::new(),
            main_window_pos: egui::Pos2::new(100.0, 100.0),
            main_window_size: egui::vec2(1400.0, 900.0),
            fps: 0.0,
            frame_time: 0.0,
        };
        
        // Initialize fractal shader library
        app.initialize_fractal_shaders();
        
        app
    }
    
    fn initialize_fractal_shaders(&mut self) {
        // Abstract shaders
        self.fractal_shaders.insert("psychedelic_swirl".to_string(), FractalShaderInfo {
            name: "Psychedelic Swirl".to_string(),
            category: "Abstract".to_string(),
            description: "Colorful swirling patterns with audio reactivity".to_string(),
            parameters: hashmap![
                "speed".to_string() => ShaderParam {
                    name: "Speed".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.1,
                    max: 5.0,
                    default: 1.0,
                    description: "Rotation speed".to_string(),
                },
                "intensity".to_string() => ShaderParam {
                    name: "Intensity".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.0,
                    max: 2.0,
                    default: 1.0,
                    description: "Color intensity".to_string(),
                },
                "audio_reactivity".to_string() => ShaderParam {
                    name: "Audio Reactivity".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.0,
                    max: 1.0,
                    default: 0.5,
                    description: "How much audio affects the pattern".to_string(),
                },
            ],
            code: include_str!("../../assets/shaders/psychedelic_swirl.wgsl").to_string(),
            vertex_code: None,
            uniform_layout: vec![
                "time".to_string(),
                "resolution".to_string(),
                "mouse".to_string(),
                "audio_level".to_string(),
            ],
        });

        self.fractal_shaders.insert("liquid_metal".to_string(), FractalShaderInfo {
            name: "Liquid Metal".to_string(),
            category: "Abstract".to_string(),
            description: "Flowing metallic liquid effect".to_string(),
            parameters: hashmap![
                "flow_speed".to_string() => ShaderParam {
                    name: "Flow Speed".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.1,
                    max: 3.0,
                    default: 1.0,
                    description: "Liquid flow speed".to_string(),
                },
                "metallicity".to_string() => ShaderParam {
                    name: "Metallicity".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.0,
                    max: 1.0,
                    default: 0.8,
                    description: "Metallic reflection intensity".to_string(),
                },
                "turbulence".to_string() => ShaderParam {
                    name: "Turbulence".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.0,
                    max: 2.0,
                    default: 0.5,
                    description: "Liquid turbulence amount".to_string(),
                },
            ],
            code: include_str!("../../assets/shaders/liquid_metal.wgsl").to_string(),
            vertex_code: None,
            uniform_layout: vec![
                "time".to_string(),
                "resolution".to_string(),
                "flow_speed".to_string(),
                "metallicity".to_string(),
                "turbulence".to_string(),
            ],
        });

        // Geometric shaders
        self.fractal_shaders.insert("mandelbrot".to_string(), FractalShaderInfo {
            name: "Mandelbrot".to_string(),
            category: "Fractal".to_string(),
            description: "Classic Mandelbrot fractal".to_string(),
            parameters: hashmap![
                "zoom".to_string() => ShaderParam {
                    name: "Zoom".to_string(),
                    param_type: ShaderParamType::Float,
                    min: 0.1,
                    max: 100.0,
                    default: 1.0,
                    description: "Fractal zoom level".to_string(),
                },
                "iterations".to_string() => ShaderParam {
                    name: "Iterations".to_string(),
                    param_type: ShaderParamType::Int,
                    min: 10.0,
                    max: 500.0,
                    default: 100.0,
                    description: "Fractal calculation iterations".to_string(),
                },
                "center_x".to_string() => ShaderParam {
                    name: "Center X".to_string(),
                    param_type: ShaderParamType::Float,
                    min: -2.0,
                    max: 2.0,
                    default: -0.5,
                    description: "Fractal center X coordinate".to_string(),
                },
                "center_y".to_string() => ShaderParam {
                    name: "Center Y".to_string(),
                    param_type: ShaderParamType::Float,
                    min: -2.0,
                    max: 2.0,
                    default: 0.0,
                    description: "Fractal center Y coordinate".to_string(),
                },
            ],
            code: include_str!("../../assets/shaders/mandelbrot.wgsl").to_string(),
            vertex_code: None,
            uniform_layout: vec![
                "time".to_string(),
                "resolution".to_string(),
                "zoom".to_string(),
                "center_x".to_string(),
                "center_y".to_string(),
                "iterations".to_string(),
            ],
        });
    }
}

impl eframe::App for FractalShaderApp {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        // Update performance metrics
        self.frame_time = ctx.input(|i| i.time);
        
        // Set application theme
        self.setup_theme(ctx);
        
        // Main application layout
        self.render_main_layout(ctx);
    }
    
}

impl FractalShaderApp {
    fn setup_theme(&self, ctx: &egui::Context) {
        let mut style = (*ctx.style()).clone();
        
        // Customize the theme for a professional VJ tool look
        style.visuals.widgets.inactive.bg_fill = egui::Color32::from_gray(25);
        style.visuals.widgets.inactive.fg_stroke.color = egui::Color32::from_gray(200);
        style.visuals.widgets.active.bg_fill = egui::Color32::from_rgb(60, 120, 200);
        style.visuals.widgets.hovered.bg_fill = egui::Color32::from_rgb(40, 80, 140);
        
        // Set dark theme with accent colors
        style.visuals.dark_mode = true;
        style.visuals.panel_fill = egui::Color32::from_gray(20);
        style.visuals.window_fill = egui::Color32::from_gray(30);
        style.visuals.faint_bg_color = egui::Color32::from_gray(15);
        
        ctx.set_style(style);
    }
    
    fn render_main_layout(&mut self, ctx: &egui::Context) {
        // Top menu bar
        self.render_menu_bar(ctx);
        
        // Main layout with resizable panels
        egui::CentralPanel::default().show(ctx, |ui| {
            self.render_main_content(ui);
        });
    }
    
    fn render_menu_bar(&mut self, ctx: &egui::Context) {
        egui::TopBottomPanel::top("menu_bar").show(ctx, |ui| {
            ui.horizontal(|ui| {
                ui.label(egui::RichText::new("🎨 Fractal Shader Editor").size(16.0));
                
                ui.separator();
                
                ui.checkbox(&mut self.show_shader_browser, "Shader Browser");
                ui.checkbox(&mut self.show_node_editor, "Node Editor");
                ui.checkbox(&mut self.show_parameter_panels, "Parameters");
                
                ui.separator();
                
                if ui.button("🔄 Reload Shaders").clicked() {
                    self.reload_shaders();
                }
                
                if ui.button("💾 Save Graph").clicked() {
                    self.save_node_graph();
                }
                
                if ui.button("📂 Load Graph").clicked() {
                    self.load_node_graph();
                }
                
                ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                    ui.label(format!("FPS: {:.0}", self.fps));
                });
            });
        });
    }
    
    fn render_main_content(&mut self, ui: &mut egui::Ui) {
        egui::Grid::new("main_layout_grid")
            .spacing([4.0, 4.0])
            .show(ui, |ui| {
                // Left panel - Shader Browser
                if self.show_shader_browser {
                    ui.vertical(|ui| {
                        self.render_shader_browser_panel(ui);
                    });
                    ui.add_space(4.0);
                }
                
                // Center panel - Node Editor
                if self.show_node_editor {
                    ui.vertical(|ui| {
                        self.render_node_editor_panel(ui);
                    });
                    ui.add_space(4.0);
                }
                
                // Right panel - Parameters
                if self.show_parameter_panels {
                    ui.vertical(|ui| {
                        self.render_parameters_panel(ui);
                    });
                }
            });
    }
    
    fn render_shader_browser_panel(&mut self, ui: &mut egui::Ui) {
        egui::Frame::group(ui.style())
            .inner_margin(8.0)
            .show(ui, |ui| {
                ui.set_min_width(280.0);
                
                ui.label(egui::RichText::new("🎨 Shader Library").size(14.0));
                ui.separator();
                
                // Shader list
                egui::ScrollArea::vertical()
                    .max_height(500.0)
                    .show(ui, |ui| {
                        let shader_names: Vec<String> = self.fractal_shaders.keys().cloned().collect();
                        for shader_name in shader_names {
                            let description = self.fractal_shaders.get(&shader_name)
                                .map(|info| info.description.clone())
                                .unwrap_or_else(|| "No description".to_string());

                            ui.group(|ui| {
                                ui.set_width(260.0);

                                ui.horizontal(|ui| {
                                    ui.label(egui::RichText::new(&shader_name).strong());
                                    if ui.button("➕").clicked() {
                                        self.add_shader_to_node(&shader_name);
                                    }
                                });

                                ui.small(egui::RichText::new(&description).italics());
                            });
                        }
                    });
            });
    }
    
    fn render_node_editor_panel(&mut self, ui: &mut egui::Ui) {
        egui::Frame::group(ui.style())
            .inner_margin(8.0)
            .show(ui, |ui| {
                ui.set_min_size(egui::vec2(800.0, 600.0));
                
                ui.label(egui::RichText::new("🔗 Node Graph").size(14.0));
                ui.separator();
                
                // Toolbar
                ui.horizontal(|ui| {
                    if ui.button("🆕 New").clicked() {
                        self.node_graph.clear();
                    }
                    
                    ui.checkbox(&mut self.node_graph.show_grid, "Grid");
                    ui.add(egui::Slider::new(&mut self.node_graph.zoom, 0.5..=2.0).text("Zoom"));
                });
                
                ui.add_space(8.0);
                
                // Node graph canvas
                let canvas_rect = ui.available_rect_before_wrap();
                let painter = ui.painter_at(canvas_rect);
                
                // Draw nodes
                for node in &self.node_graph.nodes {
                    self.draw_node(&painter, node, canvas_rect);
                }
            });
    }
    
    fn render_parameters_panel(&mut self, ui: &mut egui::Ui) {
        egui::Frame::group(ui.style())
            .inner_margin(8.0)
            .show(ui, |ui| {
                ui.set_min_width(250.0);
                
                ui.label(egui::RichText::new("🎛️ Parameters").size(14.0));
                ui.separator();

                // Selected shader parameters
                if let Some(ref shader_name) = self.selected_shader {
                    if let Some(shader_info) = self.fractal_shaders.get(shader_name) {
                        ui.label(format!("⚙️ {}", shader_info.name));
                        ui.separator();
                        
                        for (param_name, param_info) in &shader_info.parameters {
                            let current_value = self.shader_parameters
                                .get(param_name)
                                .copied()
                                .unwrap_or(param_info.default);
                            
                            let mut new_value = current_value;
                            
                            match param_info.param_type {
                                ShaderParamType::Float => {
                                    ui.add(egui::Slider::new(&mut new_value, param_info.min..=param_info.max)
                                        .text(&param_info.name));
                                }
                                ShaderParamType::Int => {
                                    let int_value = current_value as i32;
                                    let mut int_new = int_value;
                                    ui.add(egui::Slider::new(&mut int_new, param_info.min as i32..=param_info.max as i32)
                                        .text(&param_info.name));
                                    new_value = int_new as f32;
                                }
                                _ => {
                                    ui.label(format!("{}: {}", param_info.name, current_value));
                                }
                            }
                            
                            if new_value != current_value {
                                self.shader_parameters.insert(param_name.clone(), new_value);
                            }
                        }
                    }
                } else {
                    ui.label("No shader selected");
                }
                
                ui.separator();
                
                // Performance info
                ui.label("📊 Performance");
                ui.label(format!("FPS: {:.0}", self.fps));
            });
    }
    
    fn draw_node(&self, painter: &egui::Painter, node: &VisualNodePane, canvas_rect: egui::Rect) {
        let world_pos = node.position * self.node_graph.zoom + self.node_graph.pan_offset;
        let world_size = node.size * self.node_graph.zoom;
        
        let node_rect = egui::Rect::from_min_size(
            canvas_rect.min + world_pos.to_vec2(),
            world_size,
        );
        
        if !canvas_rect.intersects(node_rect) {
            return;
        }
        
        // Node background color based on type
        let node_color = match node.node_type {
            VisualNodeType::FractalShader => egui::Color32::from_rgb(150, 100, 200),
            VisualNodeType::AudioInput => egui::Color32::from_rgb(100, 150, 255),
            VisualNodeType::ParameterSource => egui::Color32::from_rgb(255, 200, 100),
            VisualNodeType::Output => egui::Color32::from_rgb(150, 255, 100),
        };
        
        // Draw node background
        painter.rect_filled(
            node_rect,
            egui::Rounding::same(8),
            node_color,
        );
        
        // Draw node border
        painter.rect_stroke(
            node_rect,
            egui::Rounding::same(8),
            egui::Stroke::new(2.0, egui::Color32::GRAY),
            egui::StrokeKind::Outside,
        );
        
        // Draw node title
        let title_rect = egui::Rect::from_min_size(
            node_rect.min + egui::vec2(8.0, 4.0),
            egui::vec2(node_rect.width() - 16.0, 20.0),
        );
        
        painter.text(
            title_rect.center(),
            egui::Align2::CENTER_CENTER,
            &format!("{:?}", node.node_type),
            egui::FontId::proportional(12.0),
            egui::Color32::WHITE,
        );
        
        // Draw sockets
        self.draw_node_sockets(painter, &node_rect);
    }
    
    fn draw_node_sockets(&self, painter: &egui::Painter, node_rect: &egui::Rect) {
        let socket_radius = 6.0;
        let socket_color = egui::Color32::from_rgb(220, 220, 220);
        
        // Input socket (left)
        let input_pos = egui::pos2(node_rect.min.x, node_rect.center().y);
        painter.circle(
            input_pos,
            socket_radius,
            socket_color,
            egui::Stroke::new(2.0, egui::Color32::BLACK),
        );
        
        // Output socket (right)
        let output_pos = egui::pos2(node_rect.max.x, node_rect.center().y);
        painter.circle(
            output_pos,
            socket_radius,
            socket_color,
            egui::Stroke::new(2.0, egui::Color32::BLACK),
        );
    }
    
    // Placeholder methods
    fn reload_shaders(&mut self) {
        println!("🔄 Reloading shaders...");
    }
    
    fn save_node_graph(&self) {
        println!("💾 Saving node graph...");
    }
    
    fn load_node_graph(&self) {
        println!("📂 Loading node graph...");
    }
    
    fn add_shader_to_node(&mut self, shader_name: &str) {
        println!("➕ Adding shader: {}", shader_name);
    }
}

/// Supporting data structures
#[derive(Clone, Debug)]
pub struct VisualNodePane {
    pub node_id: uuid::Uuid,
    pub shader_name: String,
    pub node_type: VisualNodeType,
    pub position: egui::Pos2,
    pub size: egui::Vec2,
    pub parameters: HashMap<String, f32>,
    pub selected: bool,
}

#[derive(Clone, Debug)]
pub enum VisualNodeType {
    FractalShader,
    AudioInput,
    ParameterSource,
    Output,
}

#[derive(Clone, Debug)]
pub struct FractalShaderInfo {
    pub name: String,
    pub category: String,
    pub description: String,
    pub parameters: HashMap<String, ShaderParam>,
    pub code: String,
    pub vertex_code: Option<String>,
    pub uniform_layout: Vec<String>,
}

#[derive(Clone, Debug)]
pub struct ShaderParam {
    pub name: String,
    pub param_type: ShaderParamType,
    pub min: f32,
    pub max: f32,
    pub default: f32,
    pub description: String,
}

#[derive(Clone, Debug)]
pub enum ShaderParamType {
    Float,
    Int,
    Bool,
    Color,
    Vec2,
    Vec3,
}

/// Node graph UI state
#[derive(Clone)]
pub struct NodeGraphUI {
    pub nodes: Vec<VisualNodePane>,
    pub connections: Vec<Connection>,
    pub selected_nodes: Vec<uuid::Uuid>,
    pub show_grid: bool,
    pub zoom: f32,
    pub pan_offset: egui::Vec2,
}

impl NodeGraphUI {
    pub fn new() -> Self {
        Self {
            nodes: Vec::new(),
            connections: Vec::new(),
            selected_nodes: Vec::new(),
            show_grid: true,
            zoom: 1.0,
            pan_offset: egui::Vec2::ZERO,
        }
    }
    
    pub fn clear(&mut self) {
        self.nodes.clear();
        self.connections.clear();
        self.selected_nodes.clear();
    }
}

#[derive(Clone)]
pub struct Connection {
    pub from_node: uuid::Uuid,
    pub to_node: uuid::Uuid,
}
