//! Modular Fractal Shader - Professional Fractal Generator
//!
//! A next-generation fractal generator and 3D editor/animator inspired by
//! JWildfire, Mandelbulb3D, TouchDesigner, and Unreal Engine.

#[cfg(feature = "gui")]
mod ui;
mod fractal;
mod scene;
mod animation;
mod export;

#[cfg(feature = "gui")]
use ui::main::run_gui;

fn main() {
    println!("🌀 Modular Fractal Shader - Professional Fractal Generator");
    println!("========================================================");
    println!();
    println!("Features:");
    println!("  ✨ Real-time fractal generation (Mandelbrot, Mandelbulb, Mandelbox, IFS)");
    println!("  🎨 Node-based visual programming interface");
    println!("  🎬 Professional keyframe animation system");
    println!("  🌐 3D scene environment with fractal objects");
    println!("  📦 Export to OBJ, FBX, glTF, and voxel formats");
    println!("  🎵 Audio-reactive parameter control");
    println!("  🎭 Gesture control support (Leap Motion/MediaPipe)");
    println!("  🎨 Physically-based materials and lighting");
    println!("  📊 Real-time performance monitoring");
    println!("  🎪 Volumetric rendering and atmospheric effects");
    println!();
    println!("Controls:");
    println!("  - Node Editor: Drag nodes to create fractal compositions");
    println!("  - Parameters: Adjust fractal properties in real-time");
    println!("  - Animation: Keyframe complex parameter animations");
    println!("  - Export: Generate 3D meshes and animations");
    println!();

    #[cfg(feature = "gui")]
    {
        println!("🚀 Starting GUI application...");
        if let Err(e) = run_gui() {
            eprintln!("Failed to start GUI: {}", e);
            std::process::exit(1);
        }
    }

    #[cfg(not(feature = "gui"))]
    {
        println!("❌ GUI not available - build with: cargo run --features gui");
        println!("💡 CLI mode not yet implemented");
        std::process::exit(1);
    }
}
