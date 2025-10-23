use rust_fractal_shader_engine::test_load_isf_shaders;

fn main() {
    if let Err(e) = test_load_isf_shaders() {
        eprintln!("Error loading ISF shaders: {}", e);
        std::process::exit(1);
    }
}