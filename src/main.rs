mod ui;

use eframe::egui;
use ui::main::FractalShaderApp;

fn main() -> eframe::Result<()> {
    let options = eframe::NativeOptions {
        viewport: egui::ViewportBuilder::default()
            .with_inner_size([1400.0, 900.0])
            .with_title("Fractal Shader Node Editor"),
        ..Default::default()
    };

    eframe::run_native(
        "Fractal Shader Node Editor",
        options,
        Box::new(|_cc| Ok(Box::new(FractalShaderApp::new()))),
    )
}
