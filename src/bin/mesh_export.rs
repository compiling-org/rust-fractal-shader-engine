use std::env;
use std::fs;
use std::path::Path;

// Use the library crate modules
use fractal_generator_lib::export::MeshExporter;

fn print_usage() {
    eprintln!(
        "Usage: mesh_export [--output <path>] [--width <w>] [--height <h>] [--depth <d>]\n\
         Defaults: output=exports/mesh.obj, width=32, height=32, depth=32"
    );
}

fn main() {
    let mut output_path = String::from("exports/mesh.obj");
    let mut width: u32 = 32;
    let mut height: u32 = 32;
    let mut depth: u32 = 32;

    // Parse very simple CLI args
    let mut args = env::args().skip(1);
    while let Some(arg) = args.next() {
        match arg.as_str() {
            "--output" => {
                if let Some(val) = args.next() { output_path = val; } else { print_usage(); return; }
            }
            "--width" => {
                if let Some(val) = args.next() { width = val.parse().unwrap_or(width); } else { print_usage(); return; }
            }
            "--height" => {
                if let Some(val) = args.next() { height = val.parse().unwrap_or(height); } else { print_usage(); return; }
            }
            "--depth" => {
                if let Some(val) = args.next() { depth = val.parse().unwrap_or(depth); } else { print_usage(); return; }
            }
            "--help" | "-h" => { print_usage(); return; }
            _ => {
                eprintln!("Unknown argument: {}", arg);
                print_usage();
                return;
            }
        }
    }

    // Ensure output directory exists
    if let Some(parent) = Path::new(&output_path).parent() {
        if !parent.as_os_str().is_empty() {
            if let Err(e) = fs::create_dir_all(parent) {
                eprintln!("Failed to create output directory {:?}: {}", parent, e);
                std::process::exit(1);
            }
        }
    }

    // Placeholder distance field (unused by current generate_fractal_mesh implementation)
    let voxel_count = (width as usize) * (height as usize) * (depth as usize);
    let distance_field = vec![0.0f32; voxel_count];

    // Generate a simple mesh (currently a placeholder cube in MeshExporter)
    let (vertices, indices, normals) = MeshExporter::generate_fractal_mesh(
        &distance_field,
        width,
        height,
        depth,
        0.0,
    );

    // Export OBJ
    let path = Path::new(&output_path);
    match MeshExporter::export_obj(&vertices, &indices, &normals, path) {
        Ok(_) => {
            println!("Exported OBJ to {} (verts: {}, faces: {})",
                path.display(), vertices.len(), indices.len() / 3);
        }
        Err(e) => {
            eprintln!("Export failed: {:?}", e);
            std::process::exit(1);
        }
    }
}