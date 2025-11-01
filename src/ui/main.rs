odular-fractal-shader/src/ui/main.rs</path>
<content lines="1-1000">
use eframe::egui;
use egui::{Color32, FontId, RichText, ScrollArea, Slider, TopBottomPanel, SidePanel, CentralPanel, ComboBox, Window, Vec2, Pos2, Rect, Stroke, Ui, Response};
use std::collections::HashMap;
use std::sync::{Arc, Mutex};

use crate::scene::SceneManager;
use crate::animation::AnimationController;
use crate::assets::AssetManager;
use crate::fractal::types::{FractalFormula, FractalParameters};
use crate::rendering::RenderingEngine;

/// Main fractal studio application
pub struct FractalStudioApp {
    // Core systems
    scene_manager: Arc<Mutex<SceneManager>>,
    animation_controller: AnimationController,
    asset_manager: AssetManager,
    rendering_engine: RenderingEngine,

    // UI state
    selected_fractal: Option<u32>,
    selected_object: Option<u32>,
    current_tool: Tool,
    viewport_mode: ViewportMode,
    show_preferences: bool,
    show_about: bool,
    show_export_dialog: bool,
    show_new_project_dialog: bool,

    // Theme and appearance
    theme_manager: ThemeManager,

    // Performance monitoring
    frame_time: f32,
    total_vertices: u32,
    total_voxels: u32,

    // Project state
    current_project: Option<Project>,
    fractal_parameters: FractalParameters,
}

#[derive(Debug, Clone, PartialEq)]
enum Tool {
    Select,
    Move,
    Rotate,
    Scale,
    Camera,
}

#[derive(Debug, Clone, PartialEq)]
enum ViewportMode {
    Perspective,
    Orthographic,
    Top,
    Front,
    Side,
}

#[derive(Debug, Clone)]
struct Project {
    name: String,
    path: Option<String>,
    created: chrono::DateTime<chrono::Utc>,
    modified: chrono::DateTime<chrono::Utc>,
}

#[derive(Debug, Clone)]
struct ThemeManager {
    current_theme: String,
    available_themes: Vec<String>,
}

impl ThemeManager {
    fn new() -> Self {
        Self {
            current_theme: "Dark Professional".to_string(),
            available_themes: vec![
                "Dark Professional".to_string(),
                "Light Professional".to_string(),
                "High Contrast".to_string(),
            ],
        }
    }

    fn current_theme(&self) -> Option<&String> {
        Some(&self.current_theme)
    }

    fn available_themes(&self) -> &[String] {
        &self.available_themes
    }

    fn set_theme(&mut self, theme: &str) {
        self.current_theme = theme.to_string();
    }
}

impl FractalStudioApp {
    pub fn new(cc: &eframe::CreationContext<'_>) -> Self {
        // Configure egui
        cc.egui_ctx.set_visuals(Self::create_dark_theme());

        let mut app = Self {
            scene_manager: Arc::new(Mutex::new(SceneManager::new())),
            animation_controller: AnimationController::new(),
            asset_manager: AssetManager::new(),
            rendering_engine: RenderingEngine::new(),

            selected_fractal: None,
            selected_object: None,
            current_tool: Tool::Select,
            viewport_mode: ViewportMode::Perspective,
            show_preferences: false,
            show_about: false,
            show_export_dialog: false,
            show_new_project_dialog: false,

            theme_manager: ThemeManager::new(),

            frame_time: 16.67, // ~60 FPS
            total_vertices: 0,
            total_voxels: 0,

            current_project: None,
            fractal_parameters: FractalParameters::default(),
        };

        // Load default assets
        app.asset_manager.load_default_assets();

        // Create a default scene
        if let Ok(mut scene) = app.scene_manager.lock() {
            // Add a default fractal object
            let fractal_object = crate::scene::SceneObject {
                id: 1,
                name: "Default Mandelbulb".to_string(),
                transform: crate::scene::Transform {
                    position: nalgebra::Vector3::new(0.0, 0.0, 0.0),
                    rotation: nalgebra::Vector3::new(0.0, 0.0, 0.0),
                    scale: nalgebra::Vector3::new(1.0, 1.0, 1.0),
                },
                visible: true,
                object_type: crate::scene::ObjectType::FractalObject {
                    formula: FractalFormula::Mandelbulb {
                        power: 8.0,
                        max_iterations: 100,
                    },
                    parameters: FractalParameters::default(),
                },
            };
            scene.add_object(fractal_object);
        }

        app
    }

    fn create_dark_theme() -> egui::Visuals {
        let mut visuals = egui::Visuals::dark();
        visuals.override_text_color = Some(Color32::from_rgb(220, 220, 220));
        visuals.panel_fill = Color32::from_rgb(26, 26, 26);
        visuals.window_fill = Color32::from_rgb(38, 38, 38);
        visuals.faint_bg_color = Color32::from_rgb(15, 15, 15);
        visuals.widgets.noninteractive.bg_fill = Color32::from_rgb(26, 26, 26);
        visuals.widgets.inactive.bg_fill = Color32::from_rgb(38, 38, 38);
        visuals.widgets.hovered.bg_fill = Color32::from_rgb(45, 45, 45);
        visuals.widgets.active.bg_fill = Color32::from_rgb(55, 55, 55);
        visuals.widgets.open.bg_fill = Color32::from_rgb(38, 38, 38);
        visuals
    }
}

impl eframe::App for FractalStudioApp {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        // Update animation
        self.animation_controller.update(ctx.input(|i| i.stable_dt));

        // Menu bar
        TopBottomPanel::top("menu_bar").show(ctx, |ui| {
            self.show_menu_bar(ui);
        });

        // Status bar
        TopBottomPanel::bottom("status_bar").show(ctx, |ui| {
            self.show_status_bar(ui);
        });

        // Left panel - Scene hierarchy and tools
        SidePanel::left("left_panel")
            .min_width(250.0)
            .max_width(400.0)
            .show(ctx, |ui| {
                self.show_left_panel(ui);
            });

        // Right panel - Properties and settings
        SidePanel::right("right_panel")
            .min_width(300.0)
            .max_width(500.0)
            .show(ctx, |ui| {
                self.show_right_panel(ui);
            });

        // Central panel - Main viewport
        CentralPanel::default().show(ctx, |ui| {
            self.show_central_panel(ui);
        });

        // Dialogs
        self.show_dialogs(ctx);
    }
}

impl FractalStudioApp {
    fn show_menu_bar(&mut self, ui: &mut Ui) {
        egui::menu::bar(ui, |ui| {
            ui.menu_button("File", |ui| {
                if ui.button("New Project").clicked() {
                    self.show_new_project_dialog = true;
                }
                if ui.button("Open Project").clicked() {
                    self.open_project();
                }
                if ui.button("Save Project").clicked() {
                    self.save_project();
                }
                if ui.button("Save Project As...").clicked() {
                    self.save_project_as();
                }
                ui.separator();
                if ui.button("Import Fractal").clicked() {
                    self.import_fractal();
                }
                ui.separator();
                if ui.button("Export").clicked() {
                    self.show_export_dialog = true;
                }
                ui.separator();
                if ui.button("Preferences").clicked() {
                    self.show_preferences = true;
                }
                ui.separator();
                if ui.button("Exit").clicked() {
                    std::process::exit(0);
                }
            });

            ui.menu_button("Edit", |ui| {
                if ui.button("Undo").clicked() {
                    self.undo();
                }
                if ui.button("Redo").clicked() {
                    self.redo();
                }
                ui.separator();
                if ui.button("Cut").clicked() {
                    // TODO: Implement cut
                }
                if ui.button("Copy").clicked() {
                    // TODO: Implement copy
                }
                if ui.button("Paste").clicked() {
                    // TODO: Implement paste
                }
            });

            ui.menu_button("Fractal", |ui| {
                if ui.button("Add Mandelbulb").clicked() {
                    self.add_fractal_object(FractalFormula::Mandelbulb {
                        power: 8.0,
                        max_iterations: 100,
                    });
                }
                if ui.button("Add Mandelbox").clicked() {
                    self.add_fractal_object(FractalFormula::Mandelbox {
                        scale: -1.7,
                        folding_limit: 1.0,
                        max_iterations: 50,
                    });
                }
                if ui.button("Add Julia Set").clicked() {
                    self.add_fractal_object(FractalFormula::QuaternionJulia {
                        c: [-0.8, 0.156, 0.0, 0.0],
                        max_iterations: 80,
                    });
                }
                ui.separator();
                if ui.button("Generate Mesh").clicked() {
                    // TODO: Implement mesh generation
                }
                if ui.button("Generate Voxels").clicked() {
                    // TODO: Implement voxel generation
                }
            });

            ui.menu_button("Animation", |ui| {
                if ui.button("Play").clicked() {
                    self.animation_controller.play();
                }
                if ui.button("Pause").clicked() {
                    self.animation_controller.pause();
                }
                if ui.button("Stop").clicked() {
                    self.animation_controller.stop();
                }
                ui.separator();
                if ui.button("Add Keyframe").clicked() {
                    // TODO: Implement keyframe addition
                }
                if ui.button("Clear Animation").clicked() {
                    // TODO: Implement animation clearing
                }
            });

            ui.menu_button("View", |ui| {
                ui.menu_button("Viewport Mode", |ui| {
                    if ui.selectable_value(&mut self.viewport_mode, ViewportMode::Perspective, "Perspective").clicked() {}
                    if ui.selectable_value(&mut self.viewport_mode, ViewportMode::Orthographic, "Orthographic").clicked() {}
                    ui.separator();
                    if ui.selectable_value(&mut self.viewport_mode, ViewportMode::Top, "Top").clicked() {}
                    if ui.selectable_value(&mut self.viewport_mode, ViewportMode::Front, "Front").clicked() {}
                    if ui.selectable_value(&mut self.viewport_mode, ViewportMode::Side, "Side").clicked() {}
                });

                ui.menu_button("Tool", |ui| {
                    if ui.selectable_value(&mut self.current_tool, Tool::Select, "Select").clicked() {}
                    if ui.selectable_value(&mut self.current_tool, Tool::Move, "Move").clicked() {}
                    if ui.selectable_value(&mut self.current_tool, Tool::Rotate, "Rotate").clicked() {}
                    if ui.selectable_value(&mut self.current_tool, Tool::Scale, "Scale").clicked() {}
                    if ui.selectable_value(&mut self.current_tool, Tool::Camera, "Camera").clicked() {}
                });

                ui.separator();
                if ui.button("Reset Camera").clicked() {
                    // TODO: Implement camera reset
                }
                if ui.button("Fit to View").clicked() {
                    // TODO: Implement fit to view
                }
            });

            ui.menu_button("Help", |ui| {
                if ui.button("Documentation").clicked() {
                    self.open_documentation();
                }
                if ui.button("Tutorials").clicked() {
                    self.open_tutorials();
                }
                ui.separator();
                if ui.button("About").clicked() {
                    self.show_about = true;
                }
            });
        });
    }

    fn show_status_bar(&self, ui: &mut Ui) {
        ui.horizontal(|ui| {
            ui.label(format!("Ready | FPS: {:.1} | Vertices: {} | Voxels: {}",
                1000.0 / self.frame_time.max(1.0), self.total_vertices, self.total_voxels));

            ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                ui.label("Fractal Shader Studio v1.0.0");
            });
        });
    }

    fn show_left_panel(&mut self, ui: &mut Ui) {
        ui.vertical(|ui| {
            // Tool selection
            ui.horizontal(|ui| {
                ui.label("Tool:");
                ui.selectable_value(&mut self.current_tool, Tool::Select, "👆");
                ui.selectable_value(&mut self.current_tool, Tool::Move, "↔");
                ui.selectable_value(&mut self.current_tool, Tool::Rotate, "🔄");
                ui.selectable_value(&mut self.current_tool, Tool::Scale, "📏");
                ui.selectable_value(&mut self.current_tool, Tool::Camera, "📷");
            });

            ui.separator();

            // Scene hierarchy
            ui.collapsing("Scene Hierarchy", |ui| {
                ScrollArea::vertical()
                    .id_source("scene_hierarchy")
                    .max_height(200.0)
                    .show(ui, |ui| {
                        self.render_scene_hierarchy(ui);
                    });
            });

            ui.separator();

            // Fractal library
            ui.collapsing("Fractal Library", |ui| {
                ScrollArea::vertical()
                    .id_source("fractal_library")
                    .max_height(150.0)
                    .show(ui, |ui| {
                        self.render_fractal_library(ui);
                    });
            });

            ui.separator();

            // Shader templates
            ui.collapsing("Shader Templates", |ui| {
                ScrollArea::vertical()
                    .id_source("shader_templates")
                    .max_height(150.0)
                    .show(ui, |ui| {
                        self.render_shader_templates(ui);
                    });
            });

            ui.separator();

            // Materials
            ui.collapsing("Materials", |ui| {
                self.render_materials_panel(ui);
            });
        });
    }

    fn show_right_panel(&mut self, ui: &mut Ui) {
        ui.vertical(|ui| {
            // Object properties
            if let Some(object_id) = self.selected_object {
                ui.collapsing("Object Properties", |ui| {
                    self.render_object_properties(ui, object_id);
                });
                ui.separator();
            }

            // Fractal parameters
            ui.collapsing("Fractal Parameters", |ui| {
                self.render_fractal_parameters(ui);
            });

            ui.separator();

            // Material properties
            ui.collapsing("Material Properties", |ui| {
                self.render_material_properties(ui);
            });

            ui.separator();

            // Lighting
            ui.collapsing("Lighting", |ui| {
                self.render_lighting_properties(ui);
            });

            ui.separator();

            // Performance
            ui.collapsing("Performance", |ui| {
                self.render_performance_panel(ui);
            });
        });
    }

    fn show_central_panel(&mut self, ui: &mut Ui) {
        ui.vertical(|ui| {
            // Viewport toolbar
            ui.horizontal(|ui| {
                ui.label("Viewport:");
                ui.selectable_value(&mut self.viewport_mode, ViewportMode::Perspective, "Perspective");
                ui.selectable_value(&mut self.viewport_mode, ViewportMode::Orthographic, "Ortho");
                ui.separator();
                if ui.button("Reset View").clicked() {
                    // TODO: Reset camera
                }
                if ui.button("Fit").clicked() {
                    // TODO: Fit to view
                }
            });

            ui.separator();

            // Main viewport
            let viewport_rect = ui.available_rect_before_wrap();
            self.render_viewport(ui, viewport_rect);

            ui.separator();

            // Timeline
            ui.collapsing("Timeline", |ui| {
                self.render_timeline(ui);
            });
        });
    }

    fn render_viewport(&self, ui: &mut Ui, rect: Rect) {
        // Create a painter for the viewport
        let painter = ui.painter();

        // Draw viewport background
        painter.rect_filled(rect, 0.0, Color32::from_rgb(10, 10, 15));

        // Draw grid
        self.render_viewport_grid(painter, rect);

        // Draw fractal visualization (placeholder)
        self.render_fractal_preview(painter, rect);

        // Draw HUD
        self.render_viewport_hud(painter, rect);

        // Handle viewport interactions
        let response = ui.allocate_rect(rect, egui::Sense::click_and_drag());
        self.handle_viewport_interaction(response);
    }

    fn render_viewport_grid(&self, painter: &egui::Painter, rect: Rect) {
        let grid_size = 50.0;
        let stroke = Stroke::new(1.0, Color32::from_gray(40));

        // Vertical lines
        for x in (0..rect.width() as usize).step_by(grid_size as usize) {
            let x_pos = rect.left() + x as f32;
            painter.line_segment(
                [Pos2::new(x_pos, rect.top()), Pos2::new(x_pos, rect.bottom())],
                stroke,
            );
        }

        // Horizontal lines
        for y in (0..rect.height() as usize).step_by(grid_size as usize) {
            let y_pos = rect.top() + y as f32;
            painter.line_segment(
                [Pos2::new(rect.left(), y_pos), Pos2::new(rect.right(), y_pos)],
                stroke,
            );
        }
    }

    fn render_fractal_preview(&self, painter: &egui::Painter, rect: Rect) {
        // Placeholder fractal visualization
        let center = rect.center();
        let radius = rect.width().min(rect.height()) * 0.3;

        // Draw a simple spiral pattern as placeholder
        let mut angle = 0.0;
        let mut points = Vec::new();

        for i in 0..100 {
            let r = radius * (angle / (2.0 * std::f32::consts::PI));
            let x = center.x + r * angle.cos();
            let y = center.y + r * angle.sin();
            points.push(Pos2::new(x, y));
            angle += 0.1;
        }

        if points.len() > 1 {
            painter.add(egui::Shape::line(
                points,
                Stroke::new(2.0, Color32::from_rgb(0, 255, 255)),
            ));
        }
    }

    fn render_viewport_hud(&self, painter: &egui::Painter, rect: Rect) {
        let hud_text = format!(
            "3D Viewport | {} | Zoom: 100% | FPS: {:.1}",
            match self.viewport_mode {
                ViewportMode::Perspective => "Perspective",
                ViewportMode::Orthographic => "Orthographic",
                ViewportMode::Top => "Top",
                ViewportMode::Front => "Front",
                ViewportMode::Side => "Side",
            },
            1000.0 / self.frame_time.max(1.0)
        );

        painter.text(
            rect.left_top() + Vec2::new(10.0, 10.0),
            egui::Align2::LEFT_TOP,
            hud_text,
            FontId::monospace(12.0),
            Color32::from_gray(180)
        );
    }

    fn handle_viewport_interaction(&mut self, response: Response) {
        // Handle camera controls
        if response.dragged() {
            // TODO: Implement camera orbiting/panning
        }

        if response.hovered() {
            // Handle mouse wheel for zooming
            // TODO: Implement zoom
        }
    }

    fn render_timeline(&mut self, ui: &mut Ui) {
        if let Some(project) = &self.current_project {
            ui.horizontal(|ui| {
                if ui.button("⏮").clicked() {
                    self.animation_controller.stop();
                }
                if ui.button(if self.animation_controller.is_playing() { "⏸" } else { "▶" }).clicked() {
                    if self.animation_controller.is_playing() {
                        self.animation_controller.pause();
                    } else {
                        self.animation_controller.play();
                    }
                }
                if ui.button("⏹").clicked() {
                    self.animation_controller.stop();
                }

                ui.separator();

                let mut time = self.animation_controller.current_time();
                if ui.add(Slider::new(&mut time, 0.0..=project.duration)).changed() {
                    self.animation_controller.set_time(time);
                }

                ui.label(format!("{:.2}s / {:.1}s", time, project.duration));
            });

            ui.separator();

            // Timeline tracks
            ScrollArea::horizontal()
                .id_source("timeline_scroll")
                .show(ui, |ui| {
                    ui.set_min_height(120.0);

                    // Render timeline tracks (simplified)
                    for track in self.animation_controller.timeline().tracks() {
                        ui.group(|ui| {
                            ui.set_width(200.0);
                            ui.label(&track.name);

                            // Track content would go here
                            ui.horizontal(|ui| {
                                ui.add(egui::ProgressBar::new(
                                    self.animation_controller.current_time() / project.duration
                                ).show_percentage());
                            });
                        });
                    }
                });
        } else {
            ui.centered_and_justified(|ui| {
                ui.label("No project loaded. Create a new project to start animating.");
                if ui.button("New Project").clicked() {
                    self.show_new_project_dialog = true;
                }
            });
        }
    }

    // Helper methods for rendering specific panels
    fn render_scene_hierarchy(&mut self, ui: &mut Ui) {
        if let Ok(scene) = self.scene_manager.lock() {
            for object in scene.objects() {
                ui.horizontal(|ui| {
                    // Object type icon
                    let icon = match &object.object_type {
                        crate::scene::ObjectType::FractalObject { .. } => "🔶",
                        crate::scene::ObjectType::MeshObject { .. } => "📐",
                        crate::scene::ObjectType::LightObject { .. } => "💡",
                    };
                    ui.label(icon);

                    // Object name
                    let is_selected = self.selected_object == Some(object.id);
                    if ui.selectable_label(is_selected, &object.name).clicked() {
                        self.selected_object = Some(object.id);
                    }

                    // Visibility toggle
                    let mut visible = object.visible;
                    if ui.checkbox(&mut visible, "").changed() {
                        // Would update object visibility
                    }
                });
            }
        }
    }

    fn render_fractal_library(&mut self, ui: &mut Ui) {
        ui.horizontal(|ui| {
            ui.label("Quick Add:");
        });

        ui.vertical(|ui| {
            for preset in self.asset_manager.fractal_presets() {
                if ui.button(&preset.name).clicked() {
                    self.add_fractal_object(preset.formula.clone());
                }
            }
        });
    }

    fn render_shader_templates(&mut self, ui: &mut Ui) {
        ui.label("Fractal Shader Templates:");

        ScrollArea::vertical()
            .max_height(200.0)
            .show(ui, |ui| {
                for preset in self.asset_manager.fractal_presets() {
                    ui.group(|ui| {
                        ui.set_width(240.0);
                        ui.label(RichText::new(&preset.name).strong());
                        ui.small(&preset.description);

                        if ui.button("Load").clicked() {
                            self.fractal_parameters = preset.parameters.clone();
                        }
                    });
                    ui.add_space(2.0);
                }
            });
    }

    fn render_materials_panel(&mut self, ui: &mut Ui) {
        ui.label("Material Library (Coming Soon)");
        ui.label("• PBR Materials");
        ui.label("• Procedural Textures");
        ui.label("• Custom Materials");
        ui.label("• Material Presets");
    }

    fn render_object_properties(&mut self, ui: &mut Ui, object_id: u32) {
        if let Ok(mut scene) = self.scene_manager.lock() {
            if let Some(object) = scene.get_object_mut(object_id) {
                // Transform properties
                ui.horizontal(|ui| {
                    ui.label("Position:");
                    let mut pos = object.transform.position;
                    if ui.drag_value(&mut pos.x).changed() |
                       ui.drag_value(&mut pos.y).changed() |
                       ui.drag_value(&mut pos.z).changed() {
                        object.transform.position = pos;
                    }
                });

                ui.horizontal(|ui| {
                    ui.label("Rotation:");
                    let mut rot = object.transform.rotation;
                    if ui.drag_value(&mut rot.x).changed() |
                       ui.drag_value(&mut rot.y).changed() |
                       ui.drag_value(&mut rot.z).changed() {
                        object.transform.rotation = rot;
                    }
                });

                ui.horizontal(|ui| {
                    ui.label("Scale:");
                    let mut scale = object.transform.scale;
                    if ui.drag_value(&mut scale.x).changed() |
                       ui.drag_value(&mut scale.y).changed() |
                       ui.drag_value(&mut scale.z).changed() {
                        object.transform.scale = scale;
                    }
                });
            }
        }
    }

    fn render_fractal_parameters(&mut self, ui: &mut Ui) {
        ui.label("Fractal Formula Parameters:");

        // Power parameter (for Mandelbulb, etc.)
        ui.horizontal(|ui| {
            ui.label("Power:");
            let mut power = self.fractal_parameters.power;
            ui.add(Slider::new(&mut power, 2.0..=16.0));
            if power != self.fractal_parameters.power {
                self.fractal_parameters.power = power;
            }
        });

        // Iterations
        ui.horizontal(|ui| {
            ui.label("Max Iterations:");
            let mut iterations = self.fractal_parameters.max_iterations as f32;
            ui.add(Slider::new(&mut iterations, 10.0..=500.0));
            let new_iterations = iterations as u32;
            if new_iterations != self.fractal_parameters.max_iterations {
                self.fractal_parameters.max_iterations = new_iterations;
            }
        });

        // Scale
        ui.horizontal(|ui| {
            ui.label("Scale:");
            let mut scale = self.fractal_parameters.scale;
            ui.add(Slider::new(&mut scale, 0.1..=5.0));
            if scale != self.fractal_parameters.scale {
                self.fractal_parameters.scale = scale;
            }
        });

        // Color parameters
        ui.horizontal(|ui| {
            ui.label("Hue Shift:");
            let mut hue = self.fractal_parameters.color_map.hue_shift;
            ui.add(Slider::new(&mut hue, -180.0..=180.0));
            if hue != self.fractal_parameters.color_map.hue_shift {
                self.fractal_parameters.color_map.hue_shift = hue;
            }
        });

        ui.horizontal(|ui| {
            ui.label("Saturation:");
            let mut sat = self.fractal_parameters.color_map.saturation;
            ui.add(Slider::new(&mut sat, 0.0..=2.0));
            if sat != self.fractal_parameters.color_map.saturation {
                self.fractal_parameters.color_map.saturation = sat;
            }
        });
    }

    fn render_material_properties(&mut self, ui: &mut Ui) {
        let material = &mut self.fractal_parameters.material_properties;

        ui.horizontal(|ui| {
            ui.label("Metallic:");
            ui.add(Slider::new(&mut material.metallic, 0.0..=1.0));
        });

        ui.horizontal(|ui| {
            ui.label("Roughness:");
            ui.add(Slider::new(&mut material.roughness, 0.0..=1.0));
        });

        ui.horizontal(|ui| {
            ui.label("Emission:");
            let mut emission = material.emission_intensity;
            ui.add(Slider::new(&mut emission, 0.0..=5.0));
            if emission != material.emission_intensity {
                material.emission_intensity = emission;
            }
        });
    }

    fn render_lighting_properties(&mut self, ui: &mut Ui) {
        ui.label("Scene Lighting:");

        if let Ok(scene) = self.scene_manager.lock() {
            let lighting = scene.lighting();

            for (i, light) in lighting.directional_lights.iter().enumerate() {
                ui.collapsing(format!("Directional Light {}", i + 1), |ui| {
                    ui.label(format!("Direction: {:?}", light.direction));
                    ui.label(format!("Intensity: {:.2}", light.intensity));
                    ui.label(format!("Color: {:?}", light.color));
                });
            }
        }
    }

    fn render_performance_panel(&mut self, ui: &mut Ui) {
        ui.label("Performance Metrics:");
        ui.label(format!("Frame Time: {:.2} ms", self.frame_time));
        ui.label(format!("FPS: {:.1}", 1000.0 / self.frame_time.max(1.0)));
        ui.label(format!("Vertices: {}", self.total_vertices));
        ui.label(format!("Voxels: {}", self.total_voxels));

        // Memory usage
        ui.label("Memory Usage:");
        ui.add(egui::ProgressBar::new(0.3).show_percentage());
    }

    fn show_dialogs(&mut self, ctx: &egui::Context) {
        self.render_preferences_dialog(ctx);
        self.render_about_dialog(ctx);
        self.render_export_dialog(ctx);
        self.render_new_project_dialog(ctx);
    }

    // Dialog rendering methods
    fn render_preferences_dialog(&mut self, ctx: &egui::Context) {
        Window::new("Preferences")
            .default_size(Vec2::new(500.0, 400.0))
            .open(&mut self.show_preferences)
            .show(ctx, |ui| {
                ui.horizontal(|ui| {
                    ui.label("Theme:");
                    ComboBox::from_id_source("theme_selector")
                        .selected_text(self.theme_manager.current_theme().map_or("Unknown", |t| t.clone()))
                        .show_ui(ui, |ui| {
                            for theme_name in self.theme_manager.available_themes() {
                                if ui.selectable_value(&mut self.theme_manager.current_theme, theme_name.clone(), theme_name).clicked() {
                                    self.theme_manager.set_theme(theme_name);
                                }
                            }
                        });
                });

                ui.separator();
                ui.label("Preferences content would go here");
            });
    }

    fn render_about_dialog(&mut self, ctx: &egui::Context) {
        Window::new("About")
            .default_size(Vec2::new(400.0, 300.0))
            .open(&mut self.show_about)
            .show(ctx, |ui| {
                ui.heading("Fractal Shader Studio");
                ui.label("Next-generation fractal generator and 3D editor");
                ui.label("Version 1.0.0");
                ui.separator();
                ui.label("Features:");
                ui.label("• Node-based fractal composition");
                ui.label("• Real-time 3D rendering");
                ui.label("• Professional animation timeline");
                ui.label("• Mesh and voxel export");
                ui.label("• Audio-reactive capabilities");
            });
    }

    fn render_export_dialog(&mut self, ctx: &egui::Context) {
        Window::new("Export")
            .default_size(Vec2::new(600.0, 400.0))
            .open(&mut self.show_export_dialog)
            .show(ctx, |ui| {
                ui.label("Export fractal as:");

                ui.horizontal(|ui| {
                    if ui.button("3D Mesh (OBJ)").clicked() {
                        self.export_mesh();
                    }
                    if ui.button("Voxel Model").clicked() {
                        self.export_voxels();
                    }
                });

                ui.horizontal(|ui| {
                    if ui.button("Animation (MP4)").clicked() {
                        self.export_animation();
                    }
                    if ui.button("Image Sequence").clicked() {
                        self.export_image_sequence();
                    }
                });
            });
    }

    fn render_new_project_dialog(&mut self, ctx: &egui::Context) {
        Window::new("New Project")
            .default_size(Vec2::new(400.0, 300.0))
            .open(&mut self.show_new_project_dialog)
            .show(ctx, |ui| {
                ui.label("Create a new fractal project:");

                if ui.button("Empty Project").clicked() {
                    self.create_empty_project();
                    self.show_new_project_dialog = false;
                }

                if ui.button("Project with Mandelbulb").clicked() {
                    self.create_mandelbulb_project();
                    self.show_new_project_dialog = false;
                }

                if ui.button("Project with Mandelbox").clicked() {
                    self.create_mandelbox_project();
                    self.show_new_project_dialog = false;
                }
            });
    }

    // Action methods
    fn add_fractal_object(&mut self, formula: FractalFormula) {
        let mut params = FractalParameters::default();
        params.formula = formula.clone();

        let object = crate::scene::SceneObject {
            id: rand::random(),
            name: "New Fractal".to_string(),
            transform: crate::scene::Transform {
                position: nalgebra::Vector3::new(0.0, 0.0, 0.0),
                rotation: nalgebra::Vector3::new(0.0, 0.0, 0.0),
                scale: nalgebra::Vector3::new(1.0, 1.0, 1.0),
            },
            visible: true,
            object_type: crate::scene::ObjectType::FractalObject { formula, parameters: params }
        };

        if let Ok(mut scene) = self.scene_manager.lock() {
            scene.add_object(object);
        }
    }

    // Placeholder methods for menu actions
    fn open_project() {}
    fn save_project() {}
    fn save_project_as() {}
    fn import_fractal() {}
    fn undo() {}
    fn redo() {}
    fn open_documentation() {}
    fn open_tutorials() {}
    fn export_mesh() {}
    fn export_voxels() {}
    fn export_animation() {}
    fn export_image_sequence() {}
    fn create_empty_project() {}
    fn create_mandelbulb_project() {}
    fn create_mandelbox_project() {}
}

impl Default for FractalStudioApp {
    fn default() -> Self {
        Self::new(&eframe::CreationContext::default())
    }
}
