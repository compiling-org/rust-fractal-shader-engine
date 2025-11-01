//! UI Panels and Controls
//!
//! This module provides specialized panel components for fractal parameter
//! controls, asset management, performance monitoring, and other UI elements.

use super::theme::*;
use crate::fractal::types::*;
use crate::scene::objects::*;
use std::collections::HashMap;

/// Parameter control panel for fractal editing
pub struct ParameterPanel {
    pub visible: bool,
    pub fractal_parameters: HashMap<String, FractalParameter>,
    pub theme: ProfessionalTheme,
}

#[derive(Debug, Clone)]
pub struct FractalParameter {
    pub name: String,
    pub display_name: String,
    pub value: f32,
    pub min_value: f32,
    pub max_value: f32,
    pub default_value: f32,
    pub parameter_type: ParameterType,
    pub category: String,
    pub description: String,
}

#[derive(Debug, Clone)]
pub enum ParameterType {
    Float,
    Int,
    Bool,
    Color,
    Vector2,
    Vector3,
    Enum(Vec<String>),
}

impl ParameterPanel {
    pub fn new(theme: ProfessionalTheme) -> Self {
        Self {
            visible: true,
            fractal_parameters: HashMap::new(),
            theme,
        }
    }

    /// Add a fractal parameter to the panel
    pub fn add_parameter(&mut self, param: FractalParameter) {
        self.fractal_parameters.insert(param.name.clone(), param);
    }

    /// Update parameter value
    pub fn set_parameter_value(&mut self, name: &str, value: f32) {
        if let Some(param) = self.fractal_parameters.get_mut(name) {
            param.value = value.clamp(param.min_value, param.max_value);
        }
    }

    /// Get parameter value
    pub fn get_parameter_value(&self, name: &str) -> Option<f32> {
        self.fractal_parameters.get(name).map(|p| p.value)
    }

    /// Render the parameter panel
    pub fn render(&mut self, ui: &mut egui::Ui, fractal_engine: &mut crate::fractal::engine::FractalEngine) {
        if !self.visible {
            return;
        }

        egui::Frame::none()
            .fill(self.theme.glass_background(self.theme.background_secondary))
            .stroke(self.theme.glass_border())
            .rounding(egui::Rounding::same(8.0))
            .shadow(egui::epaint::Shadow {
                offset: [0, 4],
                blur: 8.0,
                spread: 1,
                color: Color32::from_black_alpha(100),
            })
            .show(ui, |ui| {
                ui.set_min_width(280.0);

                // Header
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new("🎛️ Parameters")
                        .color(self.theme.text_primary)
                        .size(16.0));

                    ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                        if ui.button("⟲").on_hover_text("Reset to defaults").clicked() {
                            self.reset_to_defaults(fractal_engine);
                        }
                    });
                });

                ui.separator();

                egui::ScrollArea::vertical().show(ui, |ui| {
                    // Group parameters by category
                    let mut categories: std::collections::HashMap<String, Vec<&FractalParameter>> = std::collections::HashMap::new();

                    for param in self.fractal_parameters.values() {
                        categories.entry(param.category.clone()).or_insert(Vec::new()).push(param);
                    }

                    for (category, params) in categories {
                        self.render_category(ui, &category, params, fractal_engine);
                    }
                });
            });
    }

    fn render_category(&mut self, ui: &mut egui::Ui, category: &str, params: Vec<&FractalParameter>, fractal_engine: &mut crate::fractal::engine::FractalEngine) {
        ui.collapsing(egui::RichText::new(category).color(self.theme.accent_primary), |ui| {
            for param in params {
                self.render_parameter(ui, param, fractal_engine);
            }
        });
    }

    fn render_parameter(&mut self, ui: &mut egui::Ui, param: &FractalParameter, fractal_engine: &mut crate::fractal::engine::FractalEngine) {
        ui.horizontal(|ui| {
            // Parameter label with tooltip
            let label = ui.label(egui::RichText::new(&param.display_name).color(self.theme.text_primary));
            if !param.description.is_empty() {
                label.on_hover_text(&param.description);
            }

            ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                match param.parameter_type {
                    ParameterType::Float => {
                        let mut value = param.value;
                        if ui.add(egui::DragValue::new(&mut value)
                            .range(param.min_value..=param.max_value)
                            .speed(0.01))
                            .changed() {
                            self.set_parameter_value(&param.name, value);
                            fractal_engine.set_parameter(&param.name, value);
                        }
                    }
                    ParameterType::Int => {
                        let mut value = param.value as i32;
                        if ui.add(egui::DragValue::new(&mut value)
                            .range(param.min_value as i32..=param.max_value as i32))
                            .changed() {
                            let float_value = value as f32;
                            self.set_parameter_value(&param.name, float_value);
                            fractal_engine.set_parameter(&param.name, float_value);
                        }
                    }
                    ParameterType::Bool => {
                        let mut value = param.value > 0.0;
                        if ui.checkbox(&mut value, "").changed() {
                            let float_value = if value { 1.0 } else { 0.0 };
                            self.set_parameter_value(&param.name, float_value);
                            fractal_engine.set_parameter(&param.name, float_value);
                        }
                    }
                    ParameterType::Color => {
                        let mut color = Color32::from_rgb(
                            (param.value * 255.0) as u8,
                            ((param.value * 255.0) as u8).saturating_add(50),
                            ((param.value * 255.0) as u8).saturating_add(100),
                        );
                        if egui::color_picker::color_edit_button_srgba(ui, &mut color, egui::color_picker::Alpha::Opaque).changed() {
                            // Convert color back to float (simplified)
                            let [r, g, b, _] = color.to_srgba_unmultiplied();
                            let float_value = ((r as f32 + g as f32 + b as f32) / 3.0) / 255.0;
                            self.set_parameter_value(&param.name, float_value);
                            fractal_engine.set_parameter(&param.name, float_value);
                        }
                    }
                    ParameterType::Vector2 => {
                        ui.label("(Vector2 - Coming Soon)");
                    }
                    ParameterType::Vector3 => {
                        ui.label("(Vector3 - Coming Soon)");
                    }
                    ParameterType::Enum(ref options) => {
                        let mut selected = (param.value as usize).min(options.len().saturating_sub(1));
                        egui::ComboBox::from_label("")
                            .selected_text(&options[selected])
                            .show_ui(ui, |ui| {
                                for (i, option) in options.iter().enumerate() {
                                    if ui.selectable_label(selected == i, option).clicked() {
                                        selected = i;
                                        self.set_parameter_value(&param.name, i as f32);
                                        fractal_engine.set_parameter(&param.name, i as f32);
                                    }
                                }
                            });
                    }
                }

                // Reset button
                if ui.button("⟲").on_hover_text("Reset to default").clicked() {
                    self.set_parameter_value(&param.name, param.default_value);
                    fractal_engine.set_parameter(&param.name, param.default_value);
                }
            });
        });
    }

    fn reset_to_defaults(&mut self, fractal_engine: &mut crate::fractal::engine::FractalEngine) {
        for param in self.fractal_parameters.values_mut() {
            param.value = param.default_value;
            fractal_engine.set_parameter(&param.name, param.default_value);
        }
    }

    /// Initialize parameters for a specific fractal formula
    pub fn initialize_for_formula(&mut self, formula_name: &str, fractal_engine: &crate::fractal::engine::FractalEngine) {
        self.fractal_parameters.clear();

        let params = match formula_name {
            "mandelbrot" => vec![
                FractalParameter {
                    name: "zoom".to_string(),
                    display_name: "Zoom".to_string(),
                    value: 1.0,
                    min_value: 0.1,
                    max_value: 100.0,
                    default_value: 1.0,
                    parameter_type: ParameterType::Float,
                    category: "Basic".to_string(),
                    description: "Zoom level for the fractal view".to_string(),
                },
                FractalParameter {
                    name: "center_x".to_string(),
                    display_name: "Center X".to_string(),
                    value: -0.5,
                    min_value: -2.0,
                    max_value: 2.0,
                    default_value: -0.5,
                    parameter_type: ParameterType::Float,
                    category: "Position".to_string(),
                    description: "X coordinate of the view center".to_string(),
                },
                FractalParameter {
                    name: "center_y".to_string(),
                    display_name: "Center Y".to_string(),
                    value: 0.0,
                    min_value: -2.0,
                    max_value: 2.0,
                    default_value: 0.0,
                    parameter_type: ParameterType::Float,
                    category: "Position".to_string(),
                    description: "Y coordinate of the view center".to_string(),
                },
            ],
            "mandelbulb" => vec![
                FractalParameter {
                    name: "power".to_string(),
                    display_name: "Power".to_string(),
                    value: 8.0,
                    min_value: 2.0,
                    max_value: 16.0,
                    default_value: 8.0,
                    parameter_type: ParameterType::Float,
                    category: "Basic".to_string(),
                    description: "Power parameter for the Mandelbulb formula".to_string(),
                },
                FractalParameter {
                    name: "iterations".to_string(),
                    display_name: "Iterations".to_string(),
                    value: 100.0,
                    min_value: 10.0,
                    max_value: 500.0,
                    default_value: 100.0,
                    parameter_type: ParameterType::Int,
                    category: "Quality".to_string(),
                    description: "Maximum number of iterations".to_string(),
                },
            ],
            "mandelbox" => vec![
                FractalParameter {
                    name: "scale".to_string(),
                    display_name: "Scale".to_string(),
                    value: 2.0,
                    min_value: 1.0,
                    max_value: 5.0,
                    default_value: 2.0,
                    parameter_type: ParameterType::Float,
                    category: "Basic".to_string(),
                    description: "Scale factor for the Mandelbox".to_string(),
                },
                FractalParameter {
                    name: "folding_limit".to_string(),
                    display_name: "Folding Limit".to_string(),
                    value: 1.0,
                    min_value: 0.5,
                    max_value: 2.0,
                    default_value: 1.0,
                    parameter_type: ParameterType::Float,
                    category: "Basic".to_string(),
                    description: "Folding limit parameter".to_string(),
                },
            ],
            _ => vec![],
        };

        for param in params {
            self.fractal_parameters.insert(param.name.clone(), param);
        }
    }
}

/// Asset management panel
pub struct AssetPanel {
    pub visible: bool,
    pub assets: Vec<Asset>,
    pub selected_asset: Option<usize>,
    pub theme: ProfessionalTheme,
}

#[derive(Debug, Clone)]
pub struct Asset {
    pub id: String,
    pub name: String,
    pub asset_type: AssetType,
    pub path: String,
    pub thumbnail: Option<String>,
    pub tags: Vec<String>,
    pub metadata: HashMap<String, String>,
}

#[derive(Debug, Clone)]
pub enum AssetType {
    Fractal,
    Material,
    Texture,
    Mesh,
    Scene,
    Animation,
}

impl AssetPanel {
    pub fn new(theme: ProfessionalTheme) -> Self {
        Self {
            visible: true,
            assets: Vec::new(),
            selected_asset: None,
            theme,
        }
    }

    pub fn render(&mut self, ui: &mut egui::Ui) {
        if !self.visible {
            return;
        }

        egui::Frame::none()
            .fill(self.theme.glass_background(self.theme.background_secondary))
            .stroke(self.theme.glass_border())
            .rounding(egui::Rounding::same(8.0))
            .show(ui, |ui| {
                ui.set_min_width(250.0);
                ui.set_min_height(400.0);

                // Header
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new("📁 Assets")
                        .color(self.theme.text_primary)
                        .size(16.0));

                    ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                        if ui.button("➕").on_hover_text("Add asset").clicked() {
                            // Add asset functionality
                        }
                        if ui.button("🔍").on_hover_text("Search").clicked() {
                            // Search functionality
                        }
                    });
                });

                ui.separator();

                // Asset browser
                egui::ScrollArea::vertical().show(ui, |ui| {
                    // Group assets by type
                    let mut assets_by_type: std::collections::HashMap<AssetType, Vec<&Asset>> = std::collections::HashMap::new();

                    for asset in &self.assets {
                        assets_by_type.entry(asset.asset_type.clone()).or_insert(Vec::new()).push(asset);
                    }

                    for (asset_type, assets) in assets_by_type {
                        let type_name = match asset_type {
                            AssetType::Fractal => "🔢 Fractals",
                            AssetType::Material => "🎨 Materials",
                            AssetType::Texture => "🖼️ Textures",
                            AssetType::Mesh => "📦 Meshes",
                            AssetType::Scene => "🎬 Scenes",
                            AssetType::Animation => "🎭 Animations",
                        };

                        ui.collapsing(egui::RichText::new(type_name).color(self.theme.accent_primary), |ui| {
                            for (i, asset) in assets.iter().enumerate() {
                                self.render_asset_item(ui, asset, i);
                            }
                        });
                    }
                });
            });
    }

    fn render_asset_item(&mut self, ui: &mut egui::Ui, asset: &Asset, index: usize) {
        let selected = self.selected_asset == Some(index);

        let response = ui.selectable_label(selected, &asset.name);

        if response.clicked() {
            self.selected_asset = if selected { None } else { Some(index) };
        }

        response.context_menu(|ui| {
            if ui.button("Load").clicked() {
                // Load asset functionality
                ui.close();
            }
            if ui.button("Duplicate").clicked() {
                // Duplicate asset functionality
                ui.close();
            }
            ui.separator();
            if ui.button("Delete").clicked() {
                // Delete asset functionality
                ui.close();
            }
        });
    }

    pub fn add_asset(&mut self, asset: Asset) {
        self.assets.push(asset);
    }
}

/// Performance monitoring panel
pub struct PerformancePanel {
    pub visible: bool,
    pub fps: f32,
    pub frame_time: f32,
    pub memory_usage: f64,
    pub gpu_memory: f64,
    pub theme: ProfessionalTheme,
    pub performance_history: Vec<(f32, f32)>, // (fps, frame_time)
}

impl PerformancePanel {
    pub fn new(theme: ProfessionalTheme) -> Self {
        Self {
            visible: true,
            fps: 0.0,
            frame_time: 0.0,
            memory_usage: 0.0,
            gpu_memory: 0.0,
            theme,
            performance_history: Vec::new(),
        }
    }

    pub fn update_stats(&mut self, fps: f32, frame_time: f32) {
        self.fps = fps;
        self.frame_time = frame_time;

        self.performance_history.push((fps, frame_time));
        if self.performance_history.len() > 100 {
            self.performance_history.remove(0);
        }
    }

    pub fn render(&mut self, ui: &mut egui::Ui) {
        if !self.visible {
            return;
        }

        egui::Frame::none()
            .fill(self.theme.glass_background(self.theme.background_secondary))
            .stroke(self.theme.glass_border())
            .rounding(egui::Rounding::same(8.0))
            .show(ui, |ui| {
                ui.set_min_width(200.0);

                ui.label(egui::RichText::new("📊 Performance")
                    .color(self.theme.text_primary)
                    .size(16.0));

                ui.separator();

                // Real-time stats
                ui.horizontal(|ui| {
                    ui.label("FPS:");
                    ui.colored_label(
                        if self.fps > 30.0 { self.theme.success } else { self.theme.warning },
                        format!("{:.1}", self.fps)
                    );
                });

                ui.horizontal(|ui| {
                    ui.label("Frame Time:");
                    ui.colored_label(
                        if self.frame_time < 33.0 { self.theme.success } else { self.theme.warning },
                        format!("{:.1}ms", self.frame_time)
                    );
                });

                ui.horizontal(|ui| {
                    ui.label("Memory:");
                    ui.label(format!("{:.1} MB", self.memory_usage));
                });

                ui.horizontal(|ui| {
                    ui.label("GPU Memory:");
                    ui.label(format!("{:.1} MB", self.gpu_memory));
                });

                ui.separator();

                // Performance graph (simplified)
                ui.label("Performance History:");
                let available_size = ui.available_size();
                let graph_rect = egui::Rect::from_min_size(
                    ui.cursor().min,
                    egui::vec2(available_size.x, 60.0)
                );

                ui.painter().rect_filled(
                    graph_rect,
                    egui::Rounding::same(4.0),
                    self.theme.background_tertiary,
                );

                // Draw simple FPS graph
                if !self.performance_history.is_empty() {
                    let max_fps = 60.0;
                    let points: Vec<egui::Pos2> = self.performance_history.iter()
                        .enumerate()
                        .map(|(i, (fps, _))| {
                            let x = graph_rect.min.x + (i as f32 / self.performance_history.len() as f32) * graph_rect.width();
                            let y = graph_rect.max.y - (fps / max_fps) * graph_rect.height();
                            egui::pos2(x, y)
                        })
                        .collect();

                    if points.len() > 1 {
                        ui.painter().add(egui::Shape::line(
                            points,
                            egui::Stroke::new(2.0, self.theme.accent_primary)
                        ));
                    }
                }
            });
    }
}

/// Cinematic viewport with HUD overlays
pub struct ViewportPanel {
    pub theme: ProfessionalTheme,
    pub show_hud: bool,
    pub show_grid: bool,
    pub show_safe_zones: bool,
    pub zoom_level: f32,
    pub camera_info: CameraInfo,
}

#[derive(Debug, Clone)]
pub struct CameraInfo {
    pub position: [f32; 3],
    pub rotation: [f32; 3],
    pub fov: f32,
    pub near_clip: f32,
    pub far_clip: f32,
}

impl ViewportPanel {
    pub fn new(theme: ProfessionalTheme) -> Self {
        Self {
            theme,
            show_hud: true,
            show_grid: true,
            show_safe_zones: false,
            zoom_level: 1.0,
            camera_info: CameraInfo {
                position: [0.0, 0.0, 5.0],
                rotation: [0.0, 0.0, 0.0],
                fov: 60.0,
                near_clip: 0.1,
                far_clip: 1000.0,
            },
        }
    }

    pub fn render(&mut self, ui: &mut egui::Ui, viewport_texture: Option<&egui::TextureHandle>) {
        egui::Frame::none()
            .fill(self.theme.background_primary)
            .stroke(egui::Stroke::new(2.0, self.theme.border))
            .rounding(egui::Rounding::same(4.0))
            .show(ui, |ui| {
                let available_size = ui.available_size();
                let viewport_rect = egui::Rect::from_min_size(
                    ui.cursor().min,
                    available_size
                );

                // Render viewport content
                if let Some(texture) = viewport_texture {
                    ui.painter().image(
                        texture.id(),
                        viewport_rect,
                        egui::Rect::from_min_max(egui::pos2(0.0, 0.0), egui::pos2(1.0, 1.0)),
                        Color32::WHITE,
                    );
                } else {
                    // Placeholder
                    ui.painter().rect_filled(
                        viewport_rect,
                        egui::Rounding::same(0.0),
                        self.theme.background_tertiary,
                    );
                    ui.painter().text(
                        viewport_rect.center(),
                        egui::Align2::CENTER_CENTER,
                        "🎬 Viewport\n(Real-time preview coming soon)",
                        egui::FontId::proportional(16.0),
                        self.theme.text_secondary,
                    );
                }

                // Render HUD overlays
                if self.show_hud {
                    self.render_hud(ui, viewport_rect);
                }

                if self.show_grid {
                    self.render_grid(ui, viewport_rect);
                }

                if self.show_safe_zones {
                    self.render_safe_zones(ui, viewport_rect);
                }
            });
    }

    fn render_hud(&self, ui: &mut egui::Ui, viewport_rect: egui::Rect) {
        // Top-left corner info
        let info_rect = egui::Rect::from_min_size(
            viewport_rect.min + egui::vec2(10.0, 10.0),
            egui::vec2(200.0, 80.0)
        );

        ui.painter().rect_filled(
            info_rect,
            egui::Rounding::same(4.0),
            Color32::from_black_alpha(150),
        );

        ui.painter().text(
            info_rect.min + egui::vec2(8.0, 8.0),
            egui::Align2::LEFT_TOP,
            "Camera Info",
            egui::FontId::proportional(12.0),
            self.theme.text_primary,
        );

        ui.painter().text(
            info_rect.min + egui::vec2(8.0, 28.0),
            egui::Align2::LEFT_TOP,
            &format!("Pos: {:.1}, {:.1}, {:.1}", self.camera_info.position[0], self.camera_info.position[1], self.camera_info.position[2]),
            egui::FontId::proportional(10.0),
            self.theme.text_secondary,
        );

        ui.painter().text(
            info_rect.min + egui::vec2(8.0, 48.0),
            egui::Align2::LEFT_TOP,
            &format!("FOV: {:.0}°", self.camera_info.fov),
            egui::FontId::proportional(10.0),
            self.theme.text_secondary,
        );
    }

    fn render_grid(&self, ui: &mut egui::Ui, viewport_rect: egui::Rect) {
        let grid_spacing = 50.0 * self.zoom_level;

        // Vertical lines
        let mut x = viewport_rect.min.x;
        while x <= viewport_rect.max.x {
            ui.painter().line_segment(
                [egui::pos2(x, viewport_rect.min.y), egui::pos2(x, viewport_rect.max.y)],
                egui::Stroke::new(1.0, Color32::from_white_alpha(50)),
            );
            x += grid_spacing;
        }

        // Horizontal lines
        let mut y = viewport_rect.min.y;
        while y <= viewport_rect.max.y {
            ui.painter().line_segment(
                [egui::pos2(viewport_rect.min.x, y), egui::pos2(viewport_rect.max.x, y)],
                egui::Stroke::new(1.0, Color32::from_white_alpha(50)),
            );
            y += grid_spacing;
        }
    }

    fn render_safe_zones(&self, ui: &mut egui::Ui, viewport_rect: egui::Rect) {
        let center = viewport_rect.center();
        let safe_zone_ratio = 0.9; // 90% safe zone

        let safe_width = viewport_rect.width() * safe_zone_ratio;
        let safe_height = viewport_rect.height() * safe_zone_ratio;

        let safe_rect = egui::Rect::from_center_size(
            center,
            egui::vec2(safe_width, safe_height)
        );

        ui.painter().rect_stroke(
            safe_rect,
            egui::Rounding::same(0.0),
            egui::Stroke::new(2.0, Color32::from_rgb(100, 200, 100)),
        );
    }
}