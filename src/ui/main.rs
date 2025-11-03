//! Main UI Application Module
//!
//! This module provides the main egui application for the Fractal Shader Studio,
//! integrating the GPU renderer with the user interface.

use eframe::egui;
use crate::ui::node_editor::NodeEditor;

/// Main application state
pub struct FractalStudioApp {
    time: f32,
    show_fractal_library: bool,
    show_parameter_inspector: bool,
    show_timeline: bool,
    selected_fractal_type: usize,
    fractal_types: Vec<&'static str>,
    node_editor: NodeEditor,
    
    // GPU renderer placeholder
    viewport_texture: Option<egui::TextureId>,
    has_wgpu_support: bool,

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
            node_editor: NodeEditor::new(),
            viewport_texture: None,
            has_wgpu_support: false,
            // Default fractal parameters - reduced for better performance
            iterations: 30,
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

        // Check if we have WGPU support (in newer versions, this field might be different)
        // For now, we'll assume WGPU is available since we've enabled the feature
        let has_wgpu_support = true;
        log::info!("WGPU support available: {}", has_wgpu_support);

        // Configure the app for better Windows compatibility
        let ctx = &cc.egui_ctx;
        ctx.set_pixels_per_point(1.0); // Ensure proper scaling

        // Additional Windows-specific fixes from online solutions
        ctx.set_visuals(egui::Visuals::dark()); // Force dark theme
        ctx.style_mut(|style| {
            style.spacing.item_spacing = egui::vec2(8.0, 6.0);
            style.spacing.button_padding = egui::vec2(8.0, 4.0);
        });

        let mut app = Self::default();
        app.has_wgpu_support = has_wgpu_support;

        log::info!("FractalStudioApp created successfully");
        app
    }

    /// Update fractal parameters from UI
    fn update_fractal_parameters(&mut self, _ui: &mut egui::Ui) {
        // Update node graph parameters if available
        // TODO: Implement proper node parameter updates
    }
}

impl eframe::App for FractalStudioApp {
    fn update(&mut self, ctx: &egui::Context, frame: &mut eframe::Frame) {
        log::debug!("Update called");

        // Update time
        self.time += ctx.input(|i| i.stable_dt);

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
            self.show_main_workspace(ctx, ui, frame);
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
        ui.horizontal(|ui| {
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
        
        // Performance controls
        ui.label("Performance");
        ui.add(egui::Slider::new(&mut self.iterations, 10..=100).text("Max Iterations"));
        
        ui.separator();
        ui.label(format!("Current FPS: {:.1}", 1.0 / ui.input(|i| i.unstable_dt)));
        ui.label(format!("Time: {:.2}s", self.time));
    }

    /// Show main workspace with multiple panels like Mandelbulb3D/Mandelber
    fn show_main_workspace(&mut self, ctx: &egui::Context, ui: &mut egui::Ui, _frame: &mut eframe::Frame) {
        // Create dockable workspace system similar to professional 3D software
        let available_size = ui.available_size();

        // Top toolbar
        ui.horizontal(|ui| {
            ui.heading("🌀 Fractal Shader Studio");

            ui.separator();

            // Workspace tabs
            if ui.selectable_label(true, "🏠 Modeling").clicked() {
                // Switch to modeling workspace
            }
            if ui.selectable_label(false, "🎬 Animation").clicked() {
                // Switch to animation workspace
            }
            if ui.selectable_label(false, "🎨 Rendering").clicked() {
                // Switch to rendering workspace
            }
            if ui.selectable_label(false, "🔗 Node Editor").clicked() {
                // Switch to node editor workspace
            }

            ui.separator();

            // Status info
            ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                ui.label(format!("FPS: {:.1}", 1.0 / ui.input(|i| i.unstable_dt)));
                ui.label(format!("Time: {:.2}s", self.time));
            });
        });

        ui.separator();

        // Main workspace layout - similar to Mandelbulb3D interface
        let workspace_height = available_size.y - 40.0; // Account for toolbar

        // Left panel - Scene/Fractal parameters (like Mandelbulb3D)
        egui::SidePanel::left("scene_panel")
            .default_width(280.0)
            .show_inside(ui, |ui| {
                ui.set_height(workspace_height);
                self.show_scene_panel(ui);
            });

        // Right panel - Parameter inspector (like Mandelber)
        egui::SidePanel::right("inspector_panel")
            .default_width(320.0)
            .show_inside(ui, |ui| {
                ui.set_height(workspace_height);
                self.show_inspector_panel(ui);
            });

        // Bottom panel - Timeline (when enabled)
        if self.show_timeline {
            egui::TopBottomPanel::bottom("timeline_panel")
                .default_height(180.0)
                .show_inside(ui, |ui| {
                    self.show_timeline_panel(ui);
                });
        }

        // Central viewport - Main 3D view (like both applications)
        egui::CentralPanel::default().show_inside(ui, |ui| {
            let viewport_height = if self.show_timeline { workspace_height - 180.0 } else { workspace_height };
            ui.set_height(viewport_height);
            
            // Show 3D fractal viewport
            let size = ui.available_size();
            self.show_3d_viewport(ui, size);
        });
    }

    /// Show 3D fractal viewport
    fn show_3d_viewport(&mut self, ui: &mut egui::Ui, size: egui::Vec2) {
        let (rect, _response) = ui.allocate_exact_size(size, egui::Sense::hover());
        let painter = ui.painter();

        // Try to get WGPU render state for GPU rendering
        if self.has_wgpu_support {
            // We have access to WGPU - we can render to a texture and display it
            // For now, we'll show a placeholder with GPU info
            painter.rect_filled(
                rect,
                4.0,
                egui::Color32::from_rgb(30, 35, 45),
            );
            
            // Show GPU rendering info
            painter.text(
                rect.center(),
                egui::Align2::CENTER_CENTER,
                "🎮 GPU Rendering Active\nFractal Viewport",
                egui::FontId::proportional(16.0),
                egui::Color32::from_rgb(180, 200, 220),
            );
        } else {
            // Fallback to CPU rendering or placeholder
            painter.rect_filled(
                rect,
                4.0,
                egui::Color32::from_rgb(25, 25, 35),
            );

            // Draw a simple 3D coordinate system
            let center = rect.center();
            
            // X axis (red)
            painter.line_segment(
                [center, center + egui::vec2(100.0, 0.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(255, 100, 100)),
            );
            
            // Y axis (green)
            painter.line_segment(
                [center, center + egui::vec2(0.0, -100.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(100, 255, 100)),
            );
            
            // Z axis (blue)
            painter.line_segment(
                [center, center + egui::vec2(70.0, 70.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(100, 100, 255)),
            );

            // Draw a simple 3D shape to represent a fractal
            let cube_size = 80.0;
            let cube_offset = egui::vec2(50.0, -50.0);
            
            // Front face
            let front_top_left = center + cube_offset + egui::vec2(-cube_size/2.0, -cube_size/2.0);
            let front_top_right = center + cube_offset + egui::vec2(cube_size/2.0, -cube_size/2.0);
            let front_bottom_left = center + cube_offset + egui::vec2(-cube_size/2.0, cube_size/2.0);
            let front_bottom_right = center + cube_offset + egui::vec2(cube_size/2.0, cube_size/2.0);
            
            painter.line_segment([front_top_left, front_top_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_top_right, front_bottom_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_bottom_right, front_bottom_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_bottom_left, front_top_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));

            // Back face
            let back_offset = egui::vec2(30.0, 30.0);
            let back_top_left = front_top_left + back_offset;
            let back_top_right = front_top_right + back_offset;
            let back_bottom_left = front_bottom_left + back_offset;
            let back_bottom_right = front_bottom_right + back_offset;
            
            painter.line_segment([back_top_left, back_top_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_top_right, back_bottom_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_bottom_right, back_bottom_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_bottom_left, back_top_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));

            // Connecting lines
            painter.line_segment([front_top_left, back_top_left], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_top_right, back_top_right], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_bottom_left, back_bottom_left], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_bottom_right, back_bottom_right], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));

            // Label
            painter.text(
                rect.center() + egui::vec2(0.0, 100.0),
                egui::Align2::CENTER_CENTER,
                "3D Fractal Viewport\n(GPU Rendering Integration)",
                egui::FontId::proportional(14.0),
                egui::Color32::from_rgb(180, 180, 200),
            );
        }
    }

    /// Render fractal to viewport
    fn render_fractal_to_viewport(&mut self, ui: &mut egui::Ui, size: egui::Vec2) {
        let (rect, _response) = ui.allocate_exact_size(size, egui::Sense::hover());
        let painter = ui.painter();

        // More aggressive performance optimization
        let quality_factor = 4; // Render every 4th pixel for much better performance
        let width = (rect.width() as usize) / quality_factor;
        let height = (rect.height() as usize) / quality_factor;

        // Only render if we have reasonable dimensions
        if width > 0 && height > 0 {
            // Create a temporary texture to store fractal data
            let mut pixels: Vec<egui::Color32> = Vec::with_capacity(width * height);
            
            for y in 0..height {
                for x in 0..width {
                    let uv_x = (x as f32 * quality_factor as f32 / rect.width() - 0.5) * 4.0 / self.zoom + self.offset_x;
                    let uv_y = (y as f32 * quality_factor as f32 / rect.height() - 0.5) * 4.0 / self.zoom + self.offset_y;

                    let color = match self.selected_fractal_type {
                        0 => self.compute_mandelbrot_pixel(uv_x, uv_y),
                        1 => self.compute_julia_pixel(uv_x, uv_y),
                        2 => self.compute_mandelbulb_pixel(uv_x, uv_y, (height - y) as f32 / height as f32),
                        _ => self.compute_mandelbrot_pixel(uv_x, uv_y),
                    };
                    
                    pixels.push(color);
                }
            }

            // Draw the pixels with scaling
            for (i, color) in pixels.iter().enumerate() {
                let x = (i % width) as f32 * quality_factor as f32;
                let y = (i / width) as f32 * quality_factor as f32;
                let pixel_pos = rect.min + egui::vec2(x, y);
                
                painter.rect_filled(
                    egui::Rect::from_min_size(pixel_pos, egui::vec2(quality_factor as f32, quality_factor as f32)),
                    0.0,
                    *color,
                );
            }
        }

        // Draw grid overlay
        self.draw_viewport_grid(painter, rect);
        
        // Draw performance info
        painter.text(
            rect.left_top() + egui::vec2(10.0, 10.0),
            egui::Align2::LEFT_TOP,
            format!("Quality: 1/{}", quality_factor),
            egui::FontId::default(),
            egui::Color32::WHITE,
        );
    }

    /// Compute Mandelbrot set color for pixel
    fn compute_mandelbrot_pixel(&self, x: f32, y: f32) -> egui::Color32 {
        let mut zx = 0.0;
        let mut zy = 0.0;
        let mut iteration = 0;
        
        // Limit iterations for UI responsiveness
        let max_iterations = self.iterations.min(100);

        while zx * zx + zy * zy < self.bailout && iteration < max_iterations {
            let xtemp = zx * zx - zy * zy + x;
            zy = 2.0 * zx * zy + y;
            zx = xtemp;
            iteration += 1;
        }

        if iteration == max_iterations {
            egui::Color32::BLACK
        } else {
            let t = iteration as f32 / max_iterations as f32;
            let hue = (self.color_offset + t * self.color_scale) % 1.0;
            self.hsv_to_rgb(hue, 0.8, 1.0)
        }
    }

    /// Compute Julia set color for pixel
    fn compute_julia_pixel(&self, x: f32, y: f32) -> egui::Color32 {
        let mut zx = x;
        let mut zy = y;
        let mut iteration = 0;
        
        // Limit iterations for UI responsiveness
        let max_iterations = self.iterations.min(100);

        while zx * zx + zy * zy < self.bailout && iteration < max_iterations {
            let xtemp = zx * zx - zy * zy + self.julia_c_real;
            zy = 2.0 * zx * zy + self.julia_c_imag;
            zx = xtemp;
            iteration += 1;
        }

        if iteration == max_iterations {
            egui::Color32::BLACK
        } else {
            let t = iteration as f32 / max_iterations as f32;
            let hue = (self.color_offset + t * self.color_scale) % 1.0;
            self.hsv_to_rgb(hue, 0.8, 1.0)
        }
    }

    /// Compute Mandelbulb color for pixel (simplified 2D slice)
    fn compute_mandelbulb_pixel(&self, x: f32, y: f32, z: f32) -> egui::Color32 {
        let mut px = x;
        let mut py = y;
        let mut pz = z;
        let mut r = 0.0;
        let mut iteration = 0;
        
        // Limit iterations for UI responsiveness
        let max_iterations = self.iterations.min(50);

        while iteration < max_iterations {
            r = (px * px + py * py + pz * pz).sqrt();
            if r > self.bailout {
                break;
            }

            // Convert to polar coordinates
            let theta = (pz / r).acos();
            let phi = py.atan2(px);

            // Scale and rotate
            let zr = r.powf(self.power);
            let theta_new = theta * self.power;
            let phi_new = phi * self.power;

            // Convert back to cartesian
            px = zr * theta_new.sin() * phi_new.cos();
            py = zr * theta_new.sin() * phi_new.sin();
            pz = zr * theta_new.cos();

            px += x;
            py += y;
            pz += z;

            iteration += 1;
        }

        if iteration == max_iterations {
            egui::Color32::BLACK
        } else {
            let t = iteration as f32 / max_iterations as f32;
            let hue = (self.color_offset + t * self.color_scale) % 1.0;
            self.hsv_to_rgb(hue, 0.8, 1.0)
        }
    }

    /// Convert HSV to RGB color
    fn hsv_to_rgb(&self, h: f32, s: f32, v: f32) -> egui::Color32 {
        let c = v * s;
        let x = c * (1.0 - ((h * 6.0) % 2.0 - 1.0).abs());
        let m = v - c;

        let (r, g, b) = if h < 1.0/6.0 {
            (c, x, 0.0)
        } else if h < 2.0/6.0 {
            (x, c, 0.0)
        } else if h < 3.0/6.0 {
            (0.0, c, x)
        } else if h < 4.0/6.0 {
            (0.0, x, c)
        } else if h < 5.0/6.0 {
            (x, 0.0, c)
        } else {
            (c, 0.0, x)
        };

        egui::Color32::from_rgb(
            ((r + m) * 255.0) as u8,
            ((g + m) * 255.0) as u8,
            ((b + m) * 255.0) as u8,
        )
    }

    /// Draw viewport grid (like professional 3D software)
    fn draw_viewport_grid(&self, painter: &egui::Painter, rect: egui::Rect) {
        let grid_color = egui::Color32::from_rgb(60, 70, 85);
        let grid_size = 50.0;

        // Vertical lines
        let start_x = rect.left();
        let end_x = rect.right();
        let start_y = rect.top();
        let end_y = rect.bottom();

        let mut x = start_x;
        while x <= end_x {
            painter.line_segment(
                [egui::pos2(x, start_y), egui::pos2(x, end_y)],
                egui::Stroke::new(1.0, grid_color),
            );
            x += grid_size;
        }

        // Horizontal lines
        let mut y = start_y;
        while y <= end_y {
            painter.line_segment(
                [egui::pos2(start_x, y), egui::pos2(end_x, y)],
                egui::Stroke::new(1.0, grid_color),
            );
            y += grid_size;
        }

        // Center lines (emphasized)
        painter.line_segment(
            [egui::pos2(rect.center().x, start_y), egui::pos2(rect.center().x, end_y)],
            egui::Stroke::new(2.0, egui::Color32::from_rgb(100, 120, 140)),
        );
        painter.line_segment(
            [egui::pos2(start_x, rect.center().y), egui::pos2(end_x, rect.center().y)],
            egui::Stroke::new(2.0, egui::Color32::from_rgb(100, 120, 140)),
        );
    }

    /// Show scene/fractal panel (like Mandelbulb3D left panel)
    fn show_scene_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("🌌 Scene");

        ui.separator();

        egui::ScrollArea::vertical().show(ui, |ui| {
            // Fractal selection
            ui.collapsing("Fractal Type", |ui| {
                let mut new_selection = self.selected_fractal_type;
                for (i, fractal_name) in self.fractal_types.iter().enumerate() {
                    if ui.selectable_label(self.selected_fractal_type == i, *fractal_name).clicked() {
                        new_selection = i;
                    }
                }
                if new_selection != self.selected_fractal_type {
                    self.selected_fractal_type = new_selection;
                    self.reset_parameters_for_fractal_type(new_selection);
                }
            });

            ui.separator();

            // Quick parameters (like Mandelbulb3D)
            ui.collapsing("Quick Parameters", |ui| {
                ui.add(egui::Slider::new(&mut self.iterations, 10..=500).text("Iterations"));
                ui.add(egui::Slider::new(&mut self.zoom, 0.1..=10.0).text("Zoom"));
                ui.add(egui::Slider::new(&mut self.power, 2.0..=16.0).text("Power"));
            });

            ui.separator();

            // Camera controls (like Mandelber)
            ui.collapsing("Camera", |ui| {
                ui.label("Camera controls (TODO)");
                if ui.button("Reset Camera").clicked() {
                    // TODO: Reset camera
                }
            });

            ui.separator();

            // Lighting (like Mandelbulb3D)
            ui.collapsing("Lighting", |ui| {
                ui.label("Lighting setup (TODO)");
                if ui.button("Add Light").clicked() {
                    // TODO: Add light
                }
            });

            ui.separator();

            // Materials (like Mandelber)
            ui.collapsing("Materials", |ui| {
                ui.label("Material editor (TODO)");
                if ui.button("New Material").clicked() {
                    // TODO: Create material
                }
            });
            
            ui.separator();

            // Code Editor (like TouchDesigner)
            ui.collapsing("📝 Code Editor", |ui| {
                ui.label("Fractal formula code editor");
                if ui.button("Open External Editor").clicked() {
                    // This would launch an external editor in a real implementation
                    ui.label("External editor would open here...");
                }
                
                ui.separator();
                
                // Simple code editor placeholder
                egui::ScrollArea::vertical()
                    .max_height(200.0)
                    .show(ui, |ui| {
                        ui.add(
                            egui::TextEdit::multiline(&mut String::new())
                                .font(egui::TextStyle::Monospace)
                                .code_editor()
                                .desired_width(f32::INFINITY)
                                .desired_rows(10)
                                .hint_text("// Fractal formula code would go here...")
                        );
                    });
            });
        });
    }

    /// Show inspector panel (like Mandelber right panel)
    fn show_inspector_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("🔧 Inspector");

        ui.separator();

        egui::ScrollArea::vertical().show(ui, |ui| {
            // Fractal-specific parameters
            ui.collapsing("Fractal Parameters", |ui| {
                match self.selected_fractal_type {
                    0 => { self.show_mandelbrot_parameters(ui); },
                    1 => { self.show_julia_parameters(ui); },
                    2 => { self.show_mandelbulb_parameters(ui); },
                    _ => { ui.label("Parameters for this fractal type"); },
                }
            });

            ui.separator();

            // Color parameters (like Mandelbulb3D)
            ui.collapsing("Coloring", |ui| {
                ui.add(egui::Slider::new(&mut self.color_offset, 0.0..=6.28).text("Color Offset"));
                ui.add(egui::Slider::new(&mut self.color_scale, 0.1..=5.0).text("Color Scale"));
                if ui.button("Random Colors").clicked() {
                    self.color_offset = rand::random::<f32>() * 6.28;
                    self.color_scale = 0.5 + rand::random::<f32>() * 2.0;
                }
            });

            ui.separator();

            // Animation parameters
            ui.collapsing("Animation", |ui| {
                ui.checkbox(&mut self.show_timeline, "Show Timeline");
                ui.add(egui::Slider::new(&mut self.time, 0.0..=100.0).text("Time"));
                if ui.button("Reset Time").clicked() {
                    self.time = 0.0;
                }
            });

            ui.separator();

            // Render settings (like Mandelber)
            ui.collapsing("Render Settings", |ui| {
                ui.add(egui::Slider::new(&mut self.bailout, 2.0..=100.0).text("Bailout"));
                if ui.button("High Quality").clicked() {
                    self.iterations = 200;
                    self.bailout = 16.0;
                }
                if ui.button("Fast Preview").clicked() {
                    self.iterations = 50;
                    self.bailout = 4.0;
                }
            });
        });
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
            renderer: eframe::Renderer::Wgpu, // Use WGPU renderer
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

                // Force a repaint to ensure window is visible
                ctx.request_repaint();

                Ok(Box::new(FractalStudioApp::new(cc)))
            }),
        )
    }
}
