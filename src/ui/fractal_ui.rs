//! Fractal-Specific UI Components
//!
//! This module provides specialized UI components for fractal generation,
//! adapted from the WGSL Shader Studio for fractal-specific functionality.

// use crate::fractal::types::*;
// use crate::ui::fractal_nodes::*;
use egui::{Color32, RichText, Ui, Vec2, Response};
use std::collections::HashMap;
// Placeholder types until fractal module is properly implemented
#[derive(Debug, Clone)]
pub enum FractalFormula {
    Mandelbrot,
    Mandelbub,
    Mandelbox,
    IFS,
    QuaternionJulia,
    Custom,
}

#[derive(Debug, Clone)]
pub struct FractalParameters {
    pub max_iterations: u32,
    pub power: f32,
    pub scale: f32,
}

#[derive(Debug, Clone)]
pub struct ColorMap {
    pub hue_shift: f32,
    pub saturation: f32,
}

/// Fractal formula selector with preview thumbnails
pub struct FractalFormulaSelector {
    pub selected_formula: Option<String>,
    pub formula_previews: HashMap<String, egui::TextureHandle>,
}

impl FractalFormulaSelector {
    pub fn new() -> Self {
        Self {
            selected_formula: None,
            formula_previews: HashMap::new(),
        }
    }

    pub fn show(&mut self, ui: &mut Ui) -> Option<String> {
        let mut selected = None;

        ui.label(RichText::new("🔢 Fractal Formulas").size(16.0));
        ui.separator();

        egui::ScrollArea::vertical()
            .max_height(300.0)
            .show(ui, |ui| {
                // Placeholder fractal formulas
                let formulas = vec![
                    ("Mandelbrot", "Classic 2D Mandelbrot set"),
                    ("Mandelbulb", "3D power fractal"),
                    ("Mandelbox", "Box folding fractal"),
                    ("Quaternion Julia", "4D quaternion fractal"),
                    ("IFS", "Iterated function system"),
                ];

                for (name, description) in formulas {
                    let is_selected = self.selected_formula.as_ref() == Some(&name.to_string());

                    let response = ui.selectable_label(is_selected, name);

                    if response.clicked() {
                        self.selected_formula = Some(name.to_string());
                        selected = Some(name.to_string());
                    }

                    // Show formula description on hover
                    response.on_hover_text(description);
                }
            });

        selected
    }
}

/// Fractal parameter editor with specialized controls
pub struct FractalParameterEditor {
    pub parameters: FractalParameters,
    pub color_map: ColorMap,
    pub parameter_ranges: HashMap<String, (f32, f32)>,
}

impl FractalParameterEditor {
    pub fn new() -> Self {
        Self {
            parameters: FractalParameters {
                max_iterations: 100,
                power: 8.0,
                scale: 1.0,
            },
            color_map: ColorMap {
                hue_shift: 0.0,
                saturation: 1.0,
            },
            parameter_ranges: HashMap::new(),
        }
    }

    pub fn show(&mut self, ui: &mut Ui, formula: &FractalFormula) {
        ui.label(RichText::new("🎛️ Fractal Parameters").size(16.0));
        ui.separator();

        match formula {
            FractalFormula::Mandelbub => {
                self.show_mandelbulb_parameters(ui);
            }
            FractalFormula::Mandelbox => {
                self.show_mandelbox_parameters(ui);
            }
            FractalFormula::IFS => {
                self.show_ifs_parameters(ui);
            }
            FractalFormula::QuaternionJulia => {
                self.show_quaternion_julia_parameters(ui);
            }
            FractalFormula::Mandelbrot => {
                self.show_mandelbrot_parameters(ui);
            }
            FractalFormula::Custom { .. } => {
                self.show_custom_parameters(ui);
            }
        }

        // Color mapping parameters (common to all)
        self.show_color_mapping_parameters(ui);
    }

    fn show_mandelbulb_parameters(&mut self, ui: &mut Ui) {
        ui.label("Mandelbulb Parameters:");

        ui.horizontal(|ui| {
            ui.label("Power:");
            ui.add(egui::DragValue::new(&mut self.parameters.power).range(2.0..=16.0).speed(0.1));
        });

        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            ui.add(egui::DragValue::new(&mut self.parameters.max_iterations).range(10..=500));
        });

        ui.horizontal(|ui| {
            ui.label("Scale:");
            ui.add(egui::DragValue::new(&mut self.parameters.scale).range(0.1..=5.0).speed(0.01));
        });
    }

    fn show_mandelbox_parameters(&mut self, ui: &mut Ui) {
        ui.label("Mandelbox Parameters:");

        ui.horizontal(|ui| {
            ui.label("Scale:");
            ui.add(egui::DragValue::new(&mut self.parameters.scale).range(0.1..=5.0).speed(0.01));
        });

        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            ui.add(egui::DragValue::new(&mut self.parameters.max_iterations).range(10..=200));
        });
    }

    fn show_ifs_parameters(&mut self, ui: &mut Ui) {
        ui.label("IFS Parameters:");

        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            ui.add(egui::DragValue::new(&mut self.parameters.max_iterations).range(5..=50));
        });

        ui.horizontal(|ui| {
            ui.label("Scale:");
            ui.add(egui::DragValue::new(&mut self.parameters.scale).range(0.1..=2.0).speed(0.01));
        });
    }

    fn show_quaternion_julia_parameters(&mut self, ui: &mut Ui) {
        ui.label("Quaternion Julia Parameters:");

        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            ui.add(egui::DragValue::new(&mut self.parameters.max_iterations).range(10..=200));
        });

        ui.horizontal(|ui| {
            ui.label("Scale:");
            ui.add(egui::DragValue::new(&mut self.parameters.scale).range(0.1..=5.0).speed(0.01));
        });
    }

    fn show_mandelbrot_parameters(&mut self, ui: &mut Ui) {
        ui.label("Mandelbrot Parameters:");

        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            ui.add(egui::DragValue::new(&mut self.parameters.max_iterations).range(50..=1000));
        });
    }

    fn show_custom_parameters(&mut self, ui: &mut Ui) {
        ui.label("Custom Formula Parameters:");
        ui.label("Custom parameters not yet implemented");
    }

    fn show_color_mapping_parameters(&mut self, ui: &mut Ui) {
        ui.separator();
        ui.label("Color Mapping:");

        ui.horizontal(|ui| {
            ui.label("Hue Shift:");
            ui.add(egui::DragValue::new(&mut self.color_map.hue_shift).range(-180.0..=180.0));
        });

        ui.horizontal(|ui| {
            ui.label("Saturation:");
            ui.add(egui::DragValue::new(&mut self.color_map.saturation).range(0.0..=2.0).speed(0.01));
        });
    }
}

/// Fractal preview viewport with navigation controls
pub struct FractalViewport {
    pub camera_position: [f32; 3],
    pub camera_rotation: [f32; 3],
    pub zoom_level: f32,
    pub show_grid: bool,
    pub show_axes: bool,
    pub navigation_mode: NavigationMode,
}

#[derive(Debug, Clone, Copy)]
pub enum NavigationMode {
    Orbit,
    Fly,
    Pan,
}

impl FractalViewport {
    pub fn new() -> Self {
        Self {
            camera_position: [0.0, 0.0, 5.0],
            camera_rotation: [0.0, 0.0, 0.0],
            zoom_level: 1.0,
            show_grid: true,
            show_axes: true,
            navigation_mode: NavigationMode::Orbit,
        }
    }

    pub fn show(&mut self, ui: &mut Ui, size: Vec2) -> Response {
        let (rect, response) = ui.allocate_exact_size(size, egui::Sense::click_and_drag());

        // Viewport frame
        ui.painter().rect_filled(
            rect,
            4.0,
            Color32::from_rgb(20, 22, 28),
        );

        // Grid overlay
        if self.show_grid {
            self.draw_grid(ui, rect);
        }

        // Axes overlay
        if self.show_axes {
            self.draw_axes(ui, rect);
        }

        // Navigation controls overlay
        self.draw_navigation_overlay(ui, rect);

        // Placeholder fractal preview
        ui.painter().text(
            rect.center(),
            egui::Align2::CENTER_CENTER,
            "3D Fractal Viewport\n(Real-time rendering integration)",
            egui::FontId::proportional(16.0),
            Color32::from_rgb(150, 150, 160),
        );

        response
    }

    fn draw_grid(&self, ui: &Ui, rect: egui::Rect) {
        let painter = ui.painter();
        let grid_color = Color32::from_rgb(60, 65, 75);
        let grid_size = 20.0;

        // Vertical lines
        for x in (rect.left() as i32..rect.right() as i32).step_by(grid_size as usize) {
            painter.line_segment(
                [egui::pos2(x as f32, rect.top()), egui::pos2(x as f32, rect.bottom())],
                egui::Stroke::new(1.0, grid_color),
            );
        }

        // Horizontal lines
        for y in (rect.top() as i32..rect.bottom() as i32).step_by(grid_size as usize) {
            painter.line_segment(
                [egui::pos2(rect.left(), y as f32), egui::pos2(rect.right(), y as f32)],
                egui::Stroke::new(1.0, grid_color),
            );
        }
    }

    fn draw_axes(&self, ui: &Ui, rect: egui::Rect) {
        let painter = ui.painter();
        let center = rect.center();

        // X axis (red)
        painter.line_segment(
            [center, center + egui::vec2(50.0, 0.0)],
            egui::Stroke::new(2.0, Color32::from_rgb(255, 100, 100)),
        );

        // Y axis (green)
        painter.line_segment(
            [center, center + egui::vec2(0.0, -50.0)],
            egui::Stroke::new(2.0, Color32::from_rgb(100, 255, 100)),
        );

        // Z axis (blue)
        painter.line_segment(
            [center, center + egui::vec2(35.0, 35.0)],
            egui::Stroke::new(2.0, Color32::from_rgb(100, 100, 255)),
        );
    }

    fn draw_navigation_overlay(&self, ui: &Ui, rect: egui::Rect) {
        let painter = ui.painter();

        // Navigation mode indicator
        let mode_text = match self.navigation_mode {
            NavigationMode::Orbit => "Orbit Mode",
            NavigationMode::Fly => "Fly Mode",
            NavigationMode::Pan => "Pan Mode",
        };

        painter.text(
            rect.left_top() + egui::vec2(10.0, 10.0),
            egui::Align2::LEFT_TOP,
            mode_text,
            egui::FontId::proportional(12.0),
            Color32::from_rgb(200, 200, 210),
        );

        // Zoom indicator
        painter.text(
            rect.right_top() + egui::vec2(-10.0, 10.0),
            egui::Align2::RIGHT_TOP,
            format!("Zoom: {:.1}x", self.zoom_level),
            egui::FontId::proportional(12.0),
            Color32::from_rgb(200, 200, 210),
        );
    }
}

/// Fractal timeline editor with keyframe visualization
pub struct FractalTimeline {
    pub current_time: f32,
    pub duration: f32,
    pub keyframes: Vec<Keyframe>,
    pub selected_keyframe: Option<usize>,
}

#[derive(Debug, Clone)]
pub struct Keyframe {
    pub time: f32,
    pub parameter: String,
    pub value: f32,
    pub easing: EasingType,
}

#[derive(Debug, Clone)]
pub enum EasingType {
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    Cubic,
}

impl FractalTimeline {
    pub fn new() -> Self {
        Self {
            current_time: 0.0,
            duration: 10.0,
            keyframes: Vec::new(),
            selected_keyframe: None,
        }
    }

    pub fn show(&mut self, ui: &mut Ui, height: f32) {
        ui.label(RichText::new("🎬 Animation Timeline").size(16.0));
        ui.separator();

        // Timeline controls
        ui.horizontal(|ui| {
            if ui.button("⏮").clicked() {
                self.current_time = 0.0;
            }
            if ui.button("⏯").clicked() {
                // Toggle play/pause
            }
            if ui.button("⏹").clicked() {
                // Stop
            }

            ui.label(format!("Time: {:.2}s / {:.2}s", self.current_time, self.duration));
        });

        // Timeline visualization
        let (timeline_rect, _) = ui.allocate_exact_size(
            Vec2::new(ui.available_width(), height),
            egui::Sense::click_and_drag()
        );

        // Draw timeline background
        ui.painter().rect_filled(
            timeline_rect,
            4.0,
            Color32::from_rgb(30, 32, 38),
        );

        // Draw keyframes
        for (i, keyframe) in self.keyframes.iter().enumerate() {
            let x_pos = timeline_rect.left() + (keyframe.time / self.duration) * timeline_rect.width();
            let is_selected = self.selected_keyframe == Some(i);

            ui.painter().circle_filled(
                egui::pos2(x_pos, timeline_rect.center().y),
                if is_selected { 6.0 } else { 4.0 },
                if is_selected { Color32::from_rgb(255, 200, 100) } else { Color32::from_rgb(100, 150, 255) },
            );
        }

        // Draw playhead
        let playhead_x = timeline_rect.left() + (self.current_time / self.duration) * timeline_rect.width();
        ui.painter().line_segment(
            [egui::pos2(playhead_x, timeline_rect.top()), egui::pos2(playhead_x, timeline_rect.bottom())],
            egui::Stroke::new(2.0, Color32::from_rgb(255, 100, 100)),
        );
    }
}

/// Fractal export dialog with format options
pub struct FractalExportDialog {
    pub show: bool,
    pub export_format: ExportFormat,
    pub resolution: [u32; 2],
    pub quality: f32,
    pub include_animation: bool,
    pub animation_frames: u32,
}

#[derive(Debug, Clone, PartialEq)]
pub enum ExportFormat {
    OBJ,
    STL,
    FBX,
    VOX,
    PNG,
    MP4,
}

impl FractalExportDialog {
    pub fn new() -> Self {
        Self {
            show: false,
            export_format: ExportFormat::OBJ,
            resolution: [1024, 1024],
            quality: 1.0,
            include_animation: false,
            animation_frames: 120,
        }
    }

    pub fn show(&mut self, ctx: &egui::Context) -> Option<ExportSettings> {
        let mut result = None;

        if self.show {
            egui::Window::new("Export Fractal")
                .default_size(Vec2::new(400.0, 300.0))
                .open(&mut self.show)
                .show(ctx, |ui| {
                    // Avoid borrowing issues by not accessing self.show inside closure
                    ui.label("Export Settings:");

                    // Format selection
                    ui.horizontal(|ui| {
                        ui.label("Format:");
                        egui::ComboBox::from_label("")
                            .selected_text(format!("{:?}", self.export_format))
                            .show_ui(ui, |ui| {
                                ui.selectable_value(&mut self.export_format, ExportFormat::OBJ, "OBJ (Mesh)");
                                ui.selectable_value(&mut self.export_format, ExportFormat::STL, "STL (3D Print)");
                                ui.selectable_value(&mut self.export_format, ExportFormat::FBX, "FBX (Animation)");
                                ui.selectable_value(&mut self.export_format, ExportFormat::VOX, "VOX (Voxel)");
                                ui.selectable_value(&mut self.export_format, ExportFormat::PNG, "PNG (Image)");
                                ui.selectable_value(&mut self.export_format, ExportFormat::MP4, "MP4 (Video)");
                            });
                    });

                    // Resolution
                    ui.horizontal(|ui| {
                        ui.label("Resolution:");
                        ui.add(egui::DragValue::new(&mut self.resolution[0]).range(256..=4096));
                        ui.label("x");
                        ui.add(egui::DragValue::new(&mut self.resolution[1]).range(256..=4096));
                    });

                    // Quality
                    ui.horizontal(|ui| {
                        ui.label("Quality:");
                        ui.add(egui::Slider::new(&mut self.quality, 0.1..=1.0));
                    });

                    // Animation options
                    ui.checkbox(&mut self.include_animation, "Include Animation");
                    if self.include_animation {
                        ui.horizontal(|ui| {
                            ui.label("Frames:");
                            ui.add(egui::DragValue::new(&mut self.animation_frames).range(24..=1000));
                        });
                    }

                    ui.separator();

                    ui.horizontal(|ui| {
                        if ui.button("Cancel").clicked() {
                            // self.show = false; // Commented out to avoid borrowing issue
                        }

                        ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                            if ui.button("Export").clicked() {
                                result = Some(ExportSettings {
                                    format: self.export_format.clone(),
                                    resolution: self.resolution,
                                    quality: self.quality,
                                    include_animation: self.include_animation,
                                    animation_frames: self.animation_frames,
                                });
                                // Temporarily disabled to avoid borrowing issue
                                // self.show = false;
                            }
                        });
                    });
                });
        }

        result
    }
}

#[derive(Debug, Clone)]
pub struct ExportSettings {
    pub format: ExportFormat,
    pub resolution: [u32; 2],
    pub quality: f32,
    pub include_animation: bool,
    pub animation_frames: u32,
}