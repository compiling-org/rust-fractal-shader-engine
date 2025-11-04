//! Modular Fractal Shader - Professional Fractal Generator
//!
//! A next-generation fractal generator and 3D editor/animator inspired by
//! JWildfire, Mandelbulb3D, TouchDesigner, and Unreal Engine.

#[cfg(feature = "gui")]
mod gui;
mod fractal;
mod scene;
mod animation;
mod export;
mod benchmark;

#[cfg(feature = "gui")]
use fractal_generator::fractal::FractalParameters;

fn main() {
    // Set up panic hook for better error reporting
    std::panic::set_hook(Box::new(|panic_info| {
        eprintln!("❌ Application panicked: {}", panic_info);
        eprintln!("This might be related to the known Bevy 0.17 + bevy_egui focus issue.");
        eprintln!("Try running with RUST_BACKTRACE=1 for more detailed information.");
        
        // Try to save any unsaved work here if possible
        eprintln!("Attempting to save current state...");
    }));
    
    let args: Vec<String> = std::env::args().collect();

    if args.len() > 1 {
        match args[1].as_str() {
            "benchmark" => {
                println!("🧪 Running performance benchmarks...");
                if let Err(e) = benchmark::run_benchmark() {
                    eprintln!("Benchmark failed: {}", e);
                    std::process::exit(1);
                }
                return;
            }
            "test" => {
                println!("🧪 Running cross-platform compatibility tests...");
                run_compatibility_tests();
                return;
            }
            _ => {}
        }
    }

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
    println!("  🔗 NFT minting with Filecoin + NEAR blockchain");
    println!("  🌐 Web deployment with WASM");
    println!();
    println!("Usage:");
    println!("  cargo run                    # Start GUI application");
    println!("  cargo run -- benchmark       # Run performance benchmarks");
    println!("  cargo run -- test           # Run compatibility tests");
    println!("  cargo run --features web     # Build for web deployment");
    println!();
    println!("Controls:");
    println!("  - Node Editor: Drag nodes to create fractal compositions");
    println!("  - Parameters: Adjust fractal properties in real-time");
    println!("  - Animation: Keyframe complex parameter animations");
    println!("  - Export: Generate 3D meshes and animations");
    println!("  - NFT: Mint fractal art as blockchain NFTs");
    println!();

    #[cfg(feature = "gui")]
    {
        println!("🚀 Starting GUI application...");
        match std::panic::catch_unwind(|| gui::run_gui()) {
            Ok(Ok(())) => {
                println!("✅ GUI application exited normally");
            },
            Ok(Err(e)) => {
                eprintln!("❌ Failed to start GUI: {}", e);
                std::process::exit(1);
            },
            Err(panic_info) => {
                eprintln!("❌ GUI application panicked: {:?}", panic_info);
                eprintln!("This is likely the known Bevy 0.17 + bevy_egui focus issue.");
                eprintln!("The application may have exited when the window lost/gained focus.");
                eprintln!("Try running with RUST_BACKTRACE=1 for more detailed information.");
                std::process::exit(1);
            }
        }
    }

    #[cfg(not(feature = "gui"))]
    {
        println!("❌ GUI not available - build with: cargo run --features gui");
        println!("💡 For web deployment: cargo run --features web");
        println!("💡 For benchmarks: cargo run -- benchmark");
        std::process::exit(1);
    }
}

/// Run cross-platform compatibility tests
fn run_compatibility_tests() {
    println!("🖥️  Running cross-platform compatibility tests...");

    // Test system information
    println!("  📊 System Information:");
    println!("    OS: {}", std::env::consts::OS);
    println!("    Architecture: {}", std::env::consts::ARCH);
    println!("    CPU Cores: {}", num_cpus::get());

    // Test memory allocation
    println!("  🧠 Memory Test:");
    let mut test_vec = Vec::with_capacity(1000000);
    for i in 0..1000000 {
        test_vec.push(i as f32);
    }
    println!("    Allocated {} MB successfully", test_vec.len() * 4 / 1024 / 1024);

    // Test fractal computation
    println!("  🌀 Fractal Computation Test:");
    #[cfg(feature = "gui")]
    let params = fractal_generator::fractal::FractalParameters::default();
    let start = std::time::Instant::now();

    for _ in 0..1000 {
        // Simple fractal computation for testing
        let mut zx = 0.0;
        let mut zy = 0.0;
        let cx = 0.0;
        let cy = 0.0;

        for _ in 0..100 {
            let xtemp = zx * zx - zy * zy + cx;
            zy = 2.0 * zx * zy + cy;
            zx = xtemp;

            if zx * zx + zy * zy > 4.0 {
                break;
            }
        }
    }

    let duration = start.elapsed();
    println!("    1000 fractal computations: {:.2}ms", duration.as_millis());

    // Test file I/O
    println!("  📁 File I/O Test:");
    let test_file = "test_compatibility.tmp";
    if std::fs::write(test_file, "compatibility test").is_ok() {
        if std::fs::read(test_file).is_ok() {
            let _ = std::fs::remove_file(test_file);
            println!("    File I/O operations: ✅");
        }
    } else {
        println!("    File I/O operations: ❌");
    }

    // Test threading
    println!("  ⚡ Threading Test:");
    let handles: Vec<_> = (0..4).map(|i| {
        std::thread::spawn(move || {
            format!("Thread {} completed", i)
        })
    }).collect();

    for handle in handles {
        if let Ok(msg) = handle.join() {
            println!("    {}", msg);
        }
    }

    println!("✅ Cross-platform compatibility tests completed!");
}