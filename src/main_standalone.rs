//! Standalone Fractal Shader Editor Application
//! 
//! This is the main entry point for the standalone eframe + egui application.
//! It provides a comprehensive UI for the modular fractal shader system without
//! being dependent on the problematic bevy_egui integration.

use eframe::egui;
use std::sync::Arc;
use tokio::runtime::Runtime;

// Import our UI modules
mod ui;
use ui::FractalShaderApp;

#[cfg(feature = "eframe")]
fn main() -> Result<(), eframe::Error> {
    // Setup logging
    console_log::init_with_level(log::Level::Info).expect("Failed to initialize logging");
    
    // Create tokio runtime for async operations
    let rt = Arc::new(Runtime::new().expect("Failed to create tokio runtime"));
    
    // Application options
    let options = eframe::NativeOptions {
        viewport: egui::ViewportBuilder::default()
            .with_title("Modular Fractal Shader Editor")
            .with_inner_size([1600.0, 1000.0])
            .with_min_inner_size([800.0, 600.0])
            .with_decorations(true)
            .with_transparent(false)
            .with_resizable(true),
        renderer: eframe::Renderer::Glow,
        ..Default::default()
    };
    
    // Create and run the application
    eframe::run_native(
        "Modular Fractal Shader Editor",
        options,
        Box::new(move |_cc| {
            Box::new(FractalShaderApp::new())
        }),
    )
}

#[cfg(not(feature = "eframe"))]
fn main() {
    println!("The 'eframe' feature is not enabled. Please enable it to run the standalone UI.");
    println!("Run with: cargo run --features eframe");
}

#[cfg(test)]
mod tests {
    use super::*;
    
    #[test]
    fn test_app_creation() {
        let app = FractalShaderApp::new();
        // Basic smoke test
        assert!(app.show_shader_browser);
        assert!(app.show_node_editor);
        assert!(app.show_parameter_panels);
    }
}