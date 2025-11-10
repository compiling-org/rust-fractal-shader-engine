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
use fractal_generator_lib::fractal::types::FractalParameters;

fn main() {
    // Set up panic hook for better error reporting
    fn safe_eprintln(msg: &str) {
        use std::io::Write;
        let _ = writeln!(std::io::stderr(), "{}", msg);
    }
    fn safe_println(msg: &str) {
        use std::io::Write;
        let _ = writeln!(std::io::stdout(), "{}", msg);
    }
    std::panic::set_hook(Box::new(|panic_info| {
        // Avoid panicking on closed pipes by using safe writes
        safe_eprintln(&format!("❌ Application panicked: {}", panic_info));
        safe_eprintln("This might be related to the known Bevy 0.17 + bevy_egui focus issue.");
        safe_eprintln("Try running with RUST_BACKTRACE=1 for more detailed information.");
        // Try to save any unsaved work here if possible
        safe_eprintln("Attempting to save current state...");
        let _ = std::fs::write("last_panic.log", format!("{}", panic_info));
    }));

    // Initialize logger so info/warn/error from the app are visible
    // This helps confirm renderer and canvas status at runtime.
    let _ = env_logger::Builder::from_env(env_logger::Env::default().default_filter_or("info"))
        .is_test(false)
        .try_init();
    
    let args: Vec<String> = std::env::args().collect();

    if args.len() > 1 {
        match args[1].as_str() {
            "benchmark" => {
                safe_println("🧪 Running performance benchmarks...");
                if let Err(e) = benchmark::run_benchmark() {
                    safe_eprintln(&format!("Benchmark failed: {}", e));
                    std::process::exit(1);
                }
                return;
            }
            "test" => {
                safe_println("🧪 Running cross-platform compatibility tests...");
                run_compatibility_tests();
                return;
            }
            _ => {}
        }
    }

    safe_println("🌀 Modular Fractal Shader - Professional Fractal Generator");
    safe_println("========================================================");
    safe_println("");
    safe_println("Features:");
    safe_println("  ✨ Real-time fractal generation (Mandelbrot, Mandelbulb, Mandelbox, IFS)");
    safe_println("  🎨 Node-based visual programming interface");
    safe_println("  🎬 Professional keyframe animation system");
    safe_println("  🌐 3D scene environment with fractal objects");
    safe_println("  📦 Export to OBJ, FBX, glTF, and voxel formats");
    safe_println("  🎵 Audio-reactive parameter control");
    safe_println("  🎭 Gesture control support (Leap Motion/MediaPipe)");
    safe_println("  🎨 Physically-based materials and lighting");
    safe_println("  📊 Real-time performance monitoring");
    safe_println("  🎪 Volumetric rendering and atmospheric effects");
    safe_println("  🔗 NFT minting with Filecoin + NEAR blockchain");
    safe_println("  🌐 Web deployment with WASM");
    safe_println("");
    safe_println("Usage:");
    safe_println("  cargo run                    # Start GUI application");
    safe_println("  cargo run -- benchmark       # Run performance benchmarks");
    safe_println("  cargo run -- test           # Run compatibility tests");
    safe_println("  cargo run --features web     # Build for web deployment");
    safe_println("");
    safe_println("Controls:");
    safe_println("  - Node Editor: Drag nodes to create fractal compositions");
    safe_println("  - Parameters: Adjust fractal properties in real-time");
    safe_println("  - Animation: Keyframe complex parameter animations");
    safe_println("  - Export: Generate 3D meshes and animations");
    safe_println("  - NFT: Mint fractal art as blockchain NFTs");
    safe_println("");

    #[cfg(feature = "gui")]
    {
        safe_println("🚀 Starting GUI application...");
        match std::panic::catch_unwind(|| gui::run_gui()) {
            Ok(Ok(())) => {
                safe_println("✅ GUI application exited normally");
            },
            Ok(Err(e)) => {
                safe_eprintln(&format!("❌ Failed to start GUI: {}", e));
                std::process::exit(1);
            },
            Err(panic_info) => {
                safe_eprintln(&format!("❌ GUI application panicked: {:?}", panic_info));
                safe_eprintln("This is likely the known Bevy 0.17 + bevy_egui focus issue.");
                safe_eprintln("The application may have exited when the window lost/gained focus.");
                safe_eprintln("Try running with RUST_BACKTRACE=1 for more detailed information.");
                std::process::exit(1);
            }
        }
    }

    #[cfg(not(feature = "gui"))]
    {
        safe_println("❌ GUI not available - build with: cargo run --features gui");
        safe_println("💡 For web deployment: cargo run --features web");
        safe_println("💡 For benchmarks: cargo run -- benchmark");
        std::process::exit(1);
    }
}

/// Run cross-platform compatibility tests
fn run_compatibility_tests() {
    use std::io::Write;
    let mut out = std::io::stdout();
    let mut err = std::io::stderr();
    let _ = writeln!(out, "🖥️  Running cross-platform compatibility tests...");

    // Test system information
    let _ = writeln!(out, "  📊 System Information:");
    let _ = writeln!(out, "    OS: {}", std::env::consts::OS);
    let _ = writeln!(out, "    Architecture: {}", std::env::consts::ARCH);
    let _ = writeln!(out, "    CPU Cores: {}", num_cpus::get());

    // Test memory allocation
    let _ = writeln!(out, "  🧠 Memory Test:");
    let mut test_vec = Vec::with_capacity(1000000);
    for i in 0..1000000 {
        test_vec.push(i as f32);
    }
    let _ = writeln!(out, "    Allocated {} MB successfully", test_vec.len() * 4 / 1024 / 1024);

    // Test fractal computation
    let _ = writeln!(out, "  🌀 Fractal Computation Test:");
    #[cfg(feature = "gui")]
    let params = fractal_generator_lib::fractal::types::FractalParameters::default();
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
    let _ = writeln!(out, "    1000 fractal computations: {:.2}ms", duration.as_millis());

    // Test file I/O
    let _ = writeln!(out, "  📁 File I/O Test:");
    let test_file = "test_compatibility.tmp";
    if std::fs::write(test_file, "compatibility test").is_ok() {
        if std::fs::read(test_file).is_ok() {
            let _ = std::fs::remove_file(test_file);
            let _ = writeln!(out, "    File I/O operations: ✅");
        }
    } else {
        let _ = writeln!(out, "    File I/O operations: ❌");
    }

    // Test threading
    let _ = writeln!(out, "  ⚡ Threading Test:");
    let handles: Vec<_> = (0..4).map(|i| {
        std::thread::spawn(move || {
            format!("Thread {} completed", i)
        })
    }).collect();

    for handle in handles {
        if let Ok(msg) = handle.join() {
            let _ = writeln!(out, "    {}", msg);
        }
    }

    let _ = writeln!(out, "✅ Cross-platform compatibility tests completed!");
}