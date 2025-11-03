//! Main UI Application Module
//!
//! This module provides the main egui application for the Fractal Shader Studio,
//! integrating the GPU renderer with the user interface.

use eframe::egui;
// use crate::shader_renderer::{GPURenderer, FractalUniforms};
// use crate::fractal::{FractalEngine, FractalParameters, FractalFormula};

/// Main application state
pub struct FractalStudioApp {
    // gpu_renderer: Option<GPURenderer>,
    // fractal_engine: FractalEngine,
    // uniforms: FractalUniforms,
    time: f32,
    show_fractal_library: bool,
    show_parameter_inspector: bool,
    show_timeline: bool,
    selected_fractal_type: usize,
    fractal_types: Vec<&'static str>,

    // Working fractal parameters
    iterations: u32,
    zoom: f32,
    offset_x: f32,
    offset_y: f32,
    power: f32,
    bailout: f32,
    julia_c_real: f32,
    julia_c_imag: f32,
    color_offset: f32,
    color_scale: f32,
}

impl Default for FractalStudioApp {
    fn default() -> Self {
        Self {
            // gpu_renderer: None,
            // fractal_engine: FractalEngine::new(),
            // uniforms: FractalUniforms::default(),
            time: 0.0,
            show_fractal_library: true,
            show_parameter_inspector: true,
            show_timeline: false,
            selected_fractal_type: 0,
            fractal_types: vec![
                "Mandelbrot",
                "Julia",
                "Mandelbulb",
                "Mandelbox",
                "IFS Dragon",
                "Quaternion Julia",
            ],
            // Default fractal parameters
            iterations: 100,
            zoom: 1.0,
            offset_x: 0.0,
            offset_y: 0.0,
            power: 2.0,
            bailout: 4.0,
            julia_c_real: -0.7,
            julia_c_imag: 0.27015,
            color_offset: 0.0,
            color_scale: 1.0,
        }
    }
}

impl FractalStudioApp {
    /// Create new application instance
    pub fn new(cc: &eframe::CreationContext<'_>) -> Self {
        log::info!("Creating FractalStudioApp instance");

        // Configure the app for better Windows compatibility
        let ctx = &cc.egui_ctx;
        ctx.set_pixels_per_point(1.0); // Ensure proper scaling

        // Additional Windows-specific fixes from online solutions
        ctx.set_visuals(egui::Visuals::dark()); // Force dark theme
        ctx.style_mut(|style| {
            style.spacing.item_spacing = egui::vec2(8.0, 6.0);
            style.spacing.button_padding = egui::vec2(8.0, 4.0);
        });

        // Initialize GPU renderer if possible
        let gpu_renderer: Option<String> = pollster::block_on(async {
            match cc.gl.as_ref() {
                Some(_gl) => {
                    // For now, create a basic renderer - full WGPU integration needs more work
                    log::info!("GL context available, but using simplified renderer for now");
                    None // TODO: Implement proper WGPU surface creation
                }
                None => {
                    log::warn!("No GL context available for GPU rendering");
                    None
                }
            }
        });

        log::info!("FractalStudioApp created successfully");
        Self {
            // gpu_renderer,
            // uniforms: FractalUniforms::default(),
            ..Default::default()
        }
    }

    /// Update fractal parameters from UI
    fn update_fractal_parameters(&mut self) {
        // TODO: Update uniforms from UI parameters when GPU renderer is connected
        // self.uniforms.fractal_type = self.selected_fractal_type as u32;
        // self.uniforms.max_iterations = self.iterations;
        // self.uniforms.zoom = self.zoom;
        // self.uniforms.offset = [self.offset_x, self.offset_y];
        // self.uniforms.power = self.power;
        // self.uniforms.bailout = self.bailout;
        // self.uniforms.julia_c = [self.julia_c_real, self.julia_c_imag];
        // self.uniforms.color_cycle = self.color_offset;
        // self.uniforms.brightness = self.color_scale;
    }
}

impl eframe::App for FractalStudioApp {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        log::debug!("Update called");

        // Update time
        self.time += ctx.input(|i| i.stable_dt);
        // self.uniforms.time = self.time;

        // Update fractal parameters
        self.update_fractal_parameters();

        // Update GPU uniforms if renderer exists
        // if let Some(renderer) = &mut self.gpu_renderer {
        //     renderer.update_uniforms(&self.uniforms);
        // }

        // Request repaint for smooth animation
        ctx.request_repaint();

        // Main UI layout - simplified for Windows compatibility
        egui::TopBottomPanel::top("menu_bar").show(ctx, |ui| {
            self.show_menu_bar(ui);
        });

        egui::SidePanel::left("fractal_library")
            .default_width(200.0)
            .show(ctx, |ui| {
                self.show_fractal_library_panel(ui);
            });

        egui::SidePanel::right("parameter_inspector")
            .default_width(250.0)
            .show(ctx, |ui| {
                self.show_parameter_inspector_panel(ui);
            });

        egui::CentralPanel::default().show(ctx, |ui| {
            self.show_viewport(ui);
        });

        if self.show_timeline {
            egui::TopBottomPanel::bottom("timeline")
                .default_height(150.0)
                .show(ctx, |ui| {
                    self.show_timeline_panel(ui);
                });
        }

        log::debug!("Update completed");
    }

    fn on_exit(&mut self, _gl: Option<&eframe::glow::Context>) {
        log::info!("Application exiting");
    }

    fn save(&mut self, _storage: &mut dyn eframe::Storage) {
        // Save application state
        log::info!("Saving application state");
    }

    fn auto_save_interval(&self) -> std::time::Duration {
        std::time::Duration::from_secs(30)
    }
}

impl FractalStudioApp {
    /// Show menu bar
    fn show_menu_bar(&mut self, ui: &mut egui::Ui) {
        egui::menu::bar(ui, |ui| {
            ui.menu_button("File", |ui| {
                if ui.button("New Project").clicked() {
                    *self = Self::default();
                }
                if ui.button("Open Project").clicked() {
                    // TODO: Implement file dialog
                }
                ui.separator();
                if ui.button("Export Image").clicked() {
                    // TODO: Implement export
                }
                ui.separator();
                if ui.button("Mint NFT").clicked() {
                    // TODO: Implement NFT minting dialog
                }
            });

            ui.menu_button("View", |ui| {
                ui.checkbox(&mut self.show_fractal_library, "Fractal Library");
                ui.checkbox(&mut self.show_parameter_inspector, "Parameter Inspector");
                ui.checkbox(&mut self.show_timeline, "Timeline");
            });

            ui.menu_button("Help", |ui| {
                if ui.button("About").clicked() {
                    // TODO: Show about dialog
                }
            });
        });
    }

    /// Show fractal library panel
    fn show_fractal_library_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("Fractal Library");

        ui.separator();

        egui::ScrollArea::vertical().show(ui, |ui| {
            let mut new_selection = self.selected_fractal_type;
            for (i, fractal_name) in self.fractal_types.iter().enumerate() {
                if ui.selectable_label(self.selected_fractal_type == i, *fractal_name).clicked() {
                    new_selection = i;
                }
            }

            // Update selection and reset parameters if changed
            if new_selection != self.selected_fractal_type {
                self.selected_fractal_type = new_selection;
                self.reset_parameters_for_fractal_type(new_selection);
            }
        });

        ui.separator();
        ui.label("Click to select fractal type");
        ui.label("Parameters will update automatically");
    }

    /// Reset parameters based on fractal type
    fn reset_parameters_for_fractal_type(&mut self, fractal_type: usize) {
        match fractal_type {
            0 => { // Mandelbrot
                self.iterations = 100;
                self.zoom = 1.0;
                self.offset_x = 0.0;
                self.offset_y = 0.0;
                self.power = 2.0;
                self.bailout = 4.0;
            }
            1 => { // Julia
                self.iterations = 100;
                self.zoom = 1.0;
                self.offset_x = 0.0;
                self.offset_y = 0.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.julia_c_real = -0.7;
                self.julia_c_imag = 0.27015;
            }
            2 => { // Mandelbulb
                self.iterations = 50;
                self.zoom = 1.0;
                self.offset_x = 0.0;
                self.offset_y = 0.0;
                self.power = 8.0;
                self.bailout = 4.0;
            }
            3 => { // Mandelbox
                self.iterations = 20;
                self.zoom = 1.0;
                self.offset_x = 0.0;
                self.offset_y = 0.0;
                self.power = 2.0;
                self.bailout = 4.0;
            }
            _ => {}
        }
    }

    /// Show parameter inspector panel
    fn show_parameter_inspector_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("Parameters");

        ui.separator();

        egui::ScrollArea::vertical().show(ui, |ui| {
            // Fractal type specific parameters
            match self.selected_fractal_type {
                0 => self.show_mandelbrot_parameters(ui),
                1 => self.show_julia_parameters(ui),
                2 => self.show_mandelbulb_parameters(ui),
                _ => {}
            }

            ui.separator();

            // Common parameters
            self.show_common_parameters(ui);

            ui.separator();

            // Color parameters
            self.show_color_parameters(ui);

            ui.separator();

            // Render controls
            self.show_render_controls(ui);
        });
    }

    /// Show Mandelbrot parameters
    fn show_mandelbrot_parameters(&mut self, ui: &mut egui::Ui) {
        ui.label("Mandelbrot Parameters");

        ui.add(egui::Slider::new(&mut self.zoom, 0.1..=10.0).text("Zoom"));
        ui.add(egui::Slider::new(&mut self.offset_x, -2.0..=2.0).text("Center X"));
        ui.add(egui::Slider::new(&mut self.offset_y, -2.0..=2.0).text("Center Y"));
    }

    /// Show Julia parameters
    fn show_julia_parameters(&mut self, ui: &mut egui::Ui) {
        ui.label("Julia Parameters");

        ui.add(egui::Slider::new(&mut self.julia_c_real, -2.0..=2.0).text("C Real"));
        ui.add(egui::Slider::new(&mut self.julia_c_imag, -2.0..=2.0).text("C Imag"));
        ui.add(egui::Slider::new(&mut self.zoom, 0.1..=10.0).text("Zoom"));
        ui.add(egui::Slider::new(&mut self.offset_x, -2.0..=2.0).text("Center X"));
        ui.add(egui::Slider::new(&mut self.offset_y, -2.0..=2.0).text("Center Y"));
    }

    /// Show Mandelbulb parameters
    fn show_mandelbulb_parameters(&mut self, ui: &mut egui::Ui) {
        ui.label("Mandelbulb Parameters");

        ui.add(egui::Slider::new(&mut self.power, 2.0..=16.0).text("Power"));
        ui.add(egui::Slider::new(&mut self.zoom, 0.1..=5.0).text("Zoom"));
        ui.add(egui::Slider::new(&mut self.offset_x, -2.0..=2.0).text("Center X"));
        ui.add(egui::Slider::new(&mut self.offset_y, -2.0..=2.0).text("Center Y"));
    }

    /// Show common parameters
    fn show_common_parameters(&mut self, ui: &mut egui::Ui) {
        ui.label("Common Parameters");

        ui.add(egui::Slider::new(&mut self.iterations, 10..=500).text("Max Iterations"));
        ui.add(egui::Slider::new(&mut self.bailout, 2.0..=100.0).text("Bailout"));
    }

    /// Show color parameters
    fn show_color_parameters(&mut self, ui: &mut egui::Ui) {
        ui.label("Color Parameters");

        ui.add(egui::Slider::new(&mut self.color_offset, 0.0..=6.28).text("Color Offset"));
        ui.add(egui::Slider::new(&mut self.color_scale, 0.1..=5.0).text("Color Scale"));
    }

    /// Show render controls
    fn show_render_controls(&mut self, ui: &mut egui::Ui) {
        ui.label("Render Controls");

        if ui.button("Reset Parameters").clicked() {
            self.reset_parameters_for_fractal_type(self.selected_fractal_type);
        }

        if ui.button("Randomize Colors").clicked() {
            self.color_offset = rand::random::<f32>() * 6.28;
            self.color_scale = 0.5 + rand::random::<f32>() * 2.0;
        }

        ui.separator();
        ui.label(format!("Current FPS: {:.1}", 1.0 / ui.input(|i| i.unstable_dt)));
        ui.label(format!("Time: {:.2}s", self.time));
    }

    /// Show viewport (main rendering area)
    fn show_viewport(&mut self, ui: &mut egui::Ui) {
        ui.vertical_centered(|ui| {
            ui.heading("🌀 Fractal Shader Studio");

            // Show current fractal info
            ui.separator();
            ui.label(format!("Current Fractal: {}", self.fractal_types[self.selected_fractal_type]));
            ui.label(format!("Iterations: {}", self.iterations));
            ui.label(format!("Zoom: {:.2}", self.zoom));
            ui.label(format!("Time: {:.2}s", self.time));
            ui.label(format!("FPS: {:.1}", 1.0 / ui.input(|i| i.unstable_dt)));

            ui.separator();

            // Node editor viewport
            let available_size = ui.available_size();
            let node_editor_size = egui::vec2(available_size.x, available_size.y * 0.8);

            // Initialize node editor if not exists
            if !ui.memory(|mem| mem.data.get_temp::<bool>(egui::Id::new("node_editor_init")).unwrap_or(false)) {
                // Create initial nodes for demonstration
                let mut node_editor = crate::ui::node_editor::NodeEditor::new();

                // Add some example nodes
                let mandelbulb_template = node_editor.node_library.categories[0].nodes[0].clone();
                node_editor.add_node_from_template(&mandelbulb_template, egui::pos2(100.0, 100.0));

                let add_template = node_editor.node_library.categories[1].nodes[0].clone();
                node_editor.add_node_from_template(&add_template, egui::pos2(400.0, 150.0));

                ui.memory_mut(|mem| mem.data.insert_temp(egui::Id::new("node_editor_init"), true));
                ui.memory_mut(|mem| mem.data.insert_temp(egui::Id::new("node_editor"), node_editor));
            }

            // Get and show node editor
            if let Some(mut node_editor) = ui.memory_mut(|mem| mem.data.get_temp::<crate::ui::node_editor::NodeEditor>(egui::Id::new("node_editor"))) {
                node_editor.show(ui, node_editor_size);
                ui.memory_mut(|mem| mem.data.insert_temp(egui::Id::new("node_editor"), node_editor));
            }

            ui.separator();
            ui.label("🎨 Node-based fractal composition system");
            ui.label("Drag nodes from the library to create fractal networks");
            ui.label("Connect nodes by dragging from outputs to inputs");
        });
    }

    /// Draw placeholder fractal visualization
    fn draw_fractal_placeholder(&self, painter: &egui::Painter, rect: egui::Rect) {
        let center = rect.center();
        let size = rect.size().min_elem() * 0.4;

        // Draw some colorful circles to simulate fractal patterns
        for i in 0..20 {
            let angle = (i as f32 / 20.0) * std::f32::consts::TAU;
            let radius = size * (0.1 + 0.05 * (i as f32).sin() + 0.1 * self.time.sin());
            let x = center.x + radius * angle.cos();
            let y = center.y + radius * angle.sin();

            let hue = (self.color_offset + angle / std::f32::consts::TAU) % std::f32::consts::TAU;
            let color = egui::Color32::from_rgb(
                ((hue.sin() * 0.5 + 0.5) * 255.0) as u8,
                (((hue + std::f32::consts::TAU / 3.0).sin() * 0.5 + 0.5) * 255.0) as u8,
                (((hue + 2.0 * std::f32::consts::TAU / 3.0).sin() * 0.5 + 0.5) * 255.0) as u8,
            );

            painter.circle_filled(egui::pos2(x, y), 3.0 + 2.0 * (i as f32 / 20.0), color);
        }

        // Draw fractal type indicator
        let text = match self.selected_fractal_type {
            0 => "Mandelbrot",
            1 => "Julia",
            2 => "Mandelbulb",
            3 => "Mandelbox",
            _ => "Fractal",
        };

        painter.text(
            center + egui::vec2(0.0, size + 20.0),
            egui::Align2::CENTER_CENTER,
            text,
            egui::FontId::default(),
            egui::Color32::WHITE,
        );
    }

    /// Show timeline panel
    fn show_timeline_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("Timeline");

        ui.separator();

        ui.horizontal(|ui| {
            if ui.button("⏮").clicked() {
                self.time = 0.0;
            }
            if ui.button("⏯").clicked() {
                // TODO: Implement play/pause
            }
            ui.label(format!("Time: {:.2}s", self.time));
        });

        // TODO: Implement keyframe timeline
        ui.label("(Timeline implementation pending)");
    }
}

/// Run the GUI application
pub fn run_gui() -> eframe::Result<()> {
    // Set up logging for debugging
    env_logger::init();

    // Windows-specific options to fix visibility issues
    // Based on common eframe/egui Windows issues and solutions
    let options = eframe::NativeOptions {
        viewport: egui::ViewportBuilder::default()
            .with_inner_size([1400.0, 900.0])
            .with_title("Fractal Shader Studio")
            .with_visible(true) // Explicitly set visible
            .with_active(true)  // Make it active/focused
            .with_resizable(true)
            .with_min_inner_size([800.0, 600.0])
            .with_max_inner_size([1920.0, 1080.0])
            .with_position(egui::Pos2::new(200.0, 200.0)) // Position away from corner
            .with_transparent(false) // Disable transparency
            .with_decorations(true), // Enable window decorations
        ..Default::default()
    };

    log::info!("Starting Fractal Shader Studio GUI application");

    eframe::run_native(
        "Fractal Shader Studio",
        options,
        Box::new(|cc| {
            log::info!("GUI context created successfully");

            // Additional Windows-specific setup from online solutions
            let ctx = &cc.egui_ctx;
            ctx.set_pixels_per_point(1.0); // Ensure proper scaling
            ctx.set_debug_on_hover(false); // Disable debug to avoid issues

            // Force a repaint to ensure window is visible
            ctx.request_repaint();

            Ok(Box::new(FractalStudioApp::new(cc)))
        }),
    )
}
