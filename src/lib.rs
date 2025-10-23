//! # Rust Fractal Shader Engine
//!
//! Modular fractal shader system using Bevy and Shadplay.
//! Advanced GPU-accelerated fractal rendering and shader development.
//! Supports ISF shader loading, conversion between formats (ISF, HLSL, WGSL),
//! and editor integration for VS Code plugins.

use std::collections::HashMap;
use std::path::Path;
use std::fs;
use serde::{Deserialize, Serialize};
use walkdir::WalkDir;

// Module declarations
pub mod shader_renderer;
pub mod audio;
pub mod ui;
pub mod nodes;
pub mod node_editor;
pub mod shader_converter;
pub mod nft;

// Web support (only compiled for WASM targets)
#[cfg(target_arch = "wasm32")]
pub mod web;
#[cfg(target_arch = "wasm32")]
pub use web::*;

/// Main fractal shader engine structure
pub struct RustFractalShaderEngine {
    // Shader modules
    shader_modules: HashMap<String, ShaderModule>,

    // Fractal parameters
    fractal_params: FractalParameters,

    // Rendering state
    render_state: RenderState,
}

/// Shader module for fractal computation
pub struct ShaderModule {
    name: String,
    source: String,
    parameters: HashMap<String, f32>,
}

/// Fractal rendering parameters
pub struct FractalParameters {
    iterations: u32,
    zoom: f32,
    offset: (f32, f32),
    color_palette: Vec<(f32, f32, f32)>,
}

/// Rendering state management
pub struct RenderState {
    width: u32,
    height: u32,
    frame_count: u64,
}

/// ISF Shader metadata
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ISFMetadata {
    #[serde(rename = "CATEGORIES")]
    pub categories: Vec<String>,
    #[serde(rename = "INPUTS")]
    pub inputs: Vec<ISFInput>,
}

/// ISF Input parameter
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ISFInput {
    #[serde(rename = "NAME")]
    pub name: String,
    #[serde(rename = "TYPE")]
    pub input_type: String,
    #[serde(rename = "DEFAULT")]
    pub default: Option<f32>,
    #[serde(rename = "MIN")]
    pub min: Option<f32>,
    #[serde(rename = "MAX")]
    pub max: Option<f32>,
}

/// Shader format types
#[derive(Debug, Clone, Copy)]
pub enum ShaderFormat {
    ISF,
    GLSL,
    HLSL,
    WGSL,
}

/// Shader converter for format transformations
pub struct ShaderConverter;

impl ShaderConverter {
    /// Convert ISF shader to HLSL
    pub fn isf_to_hlsl(isf_source: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut hlsl_source = String::new();

        // Basic ISF to HLSL conversion
        hlsl_source.push_str("void main(\n");
        hlsl_source.push_str("    in float2 uv : TEXCOORD0,\n");
        hlsl_source.push_str("    out float4 color : SV_Target\n");
        hlsl_source.push_str(") {\n");

        // Convert GLSL syntax to HLSL
        let converted_body = isf_source
            .replace("vec2", "float2")
            .replace("vec3", "float3")
            .replace("vec4", "float4")
            .replace("float", "float")
            .replace("int", "int")
            .replace("bool", "bool")
            .replace("mat2", "float2x2")
            .replace("mat3", "float3x3")
            .replace("mat4", "float4x4")
            .replace("gl_FragCoord", "uv")
            .replace("gl_FragColor", "color")
            .replace("RENDERSIZE", "float2(1920, 1080)")  // Default resolution
            .replace("TIME", "time")
            .replace("IMG_PIXEL", "tex2D");

        hlsl_source.push_str(&converted_body);
        hlsl_source.push_str("}\n");

        Ok(hlsl_source)
    }

    /// Convert ISF shader to WGSL (improved version)
    pub fn isf_to_wgsl(isf_source: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut wgsl_source = String::new();

        // Add WGSL header
        wgsl_source.push_str("@group(0) @binding(0) var<uniform> time: f32;\n");
        wgsl_source.push_str("@group(0) @binding(1) var<uniform> resolution: vec2<f32>;\n");
        wgsl_source.push_str("@group(0) @binding(2) var input_texture: texture_2d<f32>;\n");
        wgsl_source.push_str("@group(0) @binding(3) var texture_sampler: sampler;\n\n");

        wgsl_source.push_str("@fragment\n");
        wgsl_source.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");
        wgsl_source.push_str("    let uv = coord.xy / resolution;\n");

        // Extract the main function body from ISF shader
        let body_start = isf_source.find("void main() {").unwrap_or(0) + 12;
        let body_end = isf_source.rfind("}").unwrap_or(isf_source.len());
        let body = &isf_source[body_start..body_end];

        // Convert GLSL syntax to WGSL
        let converted_body = body
            .replace("vec2", "vec2<f32>")
            .replace("vec3", "vec3<f32>")
            .replace("vec4", "vec4<f32>")
            .replace("float", "f32")
            .replace("int", "i32")
            .replace("bool", "bool")
            .replace("mat2", "mat2x2<f32>")
            .replace("mat3", "mat3x3<f32>")
            .replace("mat4", "mat4x4<f32>")
            .replace("gl_FragCoord.xy", "coord.xy")
            .replace("gl_FragColor", "return")
            .replace("RENDERSIZE.xy", "resolution")
            .replace("RENDERSIZE.y", "resolution.y")
            .replace("TIME", "time")
            .replace("IMG_PIXEL(inputTex, ", "textureSample(input_texture, texture_sampler, ")
            .replace("mod(", "f32(")
            .replace("pmod(", "f32(");

        wgsl_source.push_str(&converted_body);
        wgsl_source.push_str("}\n");

        Ok(wgsl_source)
    }

    /// Convert between shader formats
    pub fn convert(from: ShaderFormat, to: ShaderFormat, source: &str) -> Result<String, Box<dyn std::error::Error>> {
        match (from, to) {
            (ShaderFormat::ISF, ShaderFormat::WGSL) => Self::isf_to_wgsl(source),
            (ShaderFormat::ISF, ShaderFormat::HLSL) => Self::isf_to_hlsl(source),
            (ShaderFormat::GLSL, ShaderFormat::WGSL) => Self::glsl_to_wgsl(source),
            _ => Err("Unsupported conversion".into()),
        }
    }

    /// Convert GLSL to WGSL (simplified version)
    pub fn glsl_to_wgsl(glsl_source: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut wgsl_source = String::new();

        wgsl_source.push_str("@fragment\n");
        wgsl_source.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");

        // Basic GLSL to WGSL conversion
        let converted_body = glsl_source
            .replace("vec2", "vec2<f32>")
            .replace("vec3", "vec3<f32>")
            .replace("vec4", "vec4<f32>")
            .replace("float", "f32")
            .replace("int", "i32")
            .replace("bool", "bool")
            .replace("mat2", "mat2x2<f32>")
            .replace("mat3", "mat3x3<f32>")
            .replace("mat4", "mat4x4<f32>")
            .replace("gl_FragCoord", "coord.xy")
            .replace("gl_FragColor", "return");

        wgsl_source.push_str(&converted_body);
        wgsl_source.push_str("}\n");

        Ok(wgsl_source)
    }
}

impl Default for RustFractalShaderEngine {
    fn default() -> Self {
        Self {
            shader_modules: HashMap::new(),
            fractal_params: FractalParameters::default(),
            render_state: RenderState::default(),
        }
    }
}

impl Default for FractalParameters {
    fn default() -> Self {
        Self {
            iterations: 100,
            zoom: 1.0,
            offset: (0.0, 0.0),
            color_palette: vec![
                (0.0, 0.0, 1.0),
                (0.0, 1.0, 0.0),
                (1.0, 0.0, 0.0),
            ],
        }
    }
}

impl Default for RenderState {
    fn default() -> Self {
        Self {
            width: 1920,
            height: 1080,
            frame_count: 0,
        }
    }
}

impl RustFractalShaderEngine {
    pub fn new() -> Self {
        Self::default()
    }

    pub fn add_shader_module(&mut self, name: &str, source: &str) -> Result<(), Box<dyn std::error::Error>> {
        let module = ShaderModule {
            name: name.to_string(),
            source: source.to_string(),
            parameters: HashMap::new(),
        };
        self.shader_modules.insert(name.to_string(), module);
        Ok(())
    }

    pub fn set_fractal_parameter(&mut self, name: &str, value: f32) {
        match name {
            "iterations" => self.fractal_params.iterations = value as u32,
            "zoom" => self.fractal_params.zoom = value,
            _ => {}
        }
    }

    pub fn render_frame(&mut self) -> Result<(), Box<dyn std::error::Error>> {
        // Placeholder for rendering logic
        self.render_state.frame_count += 1;
        Ok(())
    }

    pub fn get_frame_count(&self) -> u64 {
        self.render_state.frame_count
    }

    /// Load ISF shaders from a directory
    pub fn load_isf_shaders_from_directory<P: AsRef<Path>>(&mut self, dir_path: P) -> Result<Vec<String>, Box<dyn std::error::Error>> {
        let mut loaded_shaders = Vec::new();

        for entry in WalkDir::new(dir_path).into_iter().filter_map(|e| e.ok()) {
            if entry.path().extension().and_then(|s| s.to_str()) == Some("fs") {
                if let Some(shader_name) = entry.path().file_stem().and_then(|s| s.to_str()) {
                    let content = fs::read_to_string(entry.path())?;
                    self.add_shader_module(shader_name, &content)?;
                    loaded_shaders.push(shader_name.to_string());
                }
            }
        }

        Ok(loaded_shaders)
    }

    /// Parse ISF metadata from shader source
    pub fn parse_isf_metadata(source: &str) -> Result<ISFMetadata, Box<dyn std::error::Error>> {
        // Extract JSON metadata from comments
        if let Some(start) = source.find("/*{") {
            if let Some(end) = source.find("}*/") {
                let json_str = &source[start + 2..end + 1];
                let metadata: ISFMetadata = serde_json::from_str(json_str)?;
                return Ok(metadata);
            }
        }
        Err("No ISF metadata found".into())
    }

    /// Convert ISF shader to WGSL
    pub fn convert_isf_to_wgsl(&self, isf_source: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut wgsl_source = String::new();

        // Basic ISF to WGSL conversion
        // This is a simplified converter - full implementation would be more complex

        wgsl_source.push_str("@fragment\n");
        wgsl_source.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");

        // Convert GLSL syntax to WGSL
        let converted_body = isf_source
            .replace("vec2", "vec2<f32>")
            .replace("vec3", "vec3<f32>")
            .replace("vec4", "vec4<f32>")
            .replace("float", "f32")
            .replace("int", "i32")
            .replace("bool", "bool")
            .replace("mat2", "mat2x2<f32>")
            .replace("mat3", "mat3x3<f32>")
            .replace("mat4", "mat4x4<f32>")
            .replace("gl_FragCoord", "coord")
            .replace("gl_FragColor", "return");

        wgsl_source.push_str(&converted_body);
        wgsl_source.push_str("}\n");

        Ok(wgsl_source)
    }
}

impl ShaderModule {
    pub fn set_parameter(&mut self, name: &str, value: f32) {
        self.parameters.insert(name.to_string(), value);
    }

    pub fn get_parameter(&self, name: &str) -> Option<f32> {
        self.parameters.get(name).copied()
    }
}

/// Simple test function to verify the library compiles
pub fn hello_rust_fractal_shader_engine() -> &'static str {
    "Hello from Rust Fractal Shader Engine! Advanced GPU-accelerated fractal rendering."
}

// Re-export the NodeEditorPlugin for easy access
pub use node_editor::NodeEditorPlugin;

/// Test loading ISF shaders
pub fn test_load_isf_shaders() -> Result<(), Box<dyn std::error::Error>> {
    let mut engine = RustFractalShaderEngine::new();
    let loaded = engine.load_isf_shaders_from_directory("assets/shaders/isf")?;
    println!("Loaded {} ISF shaders: {:?}", loaded.len(), loaded);
    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_hello() {
        assert_eq!(hello_rust_fractal_shader_engine(), "Hello from Rust Fractal Shader Engine! Advanced GPU-accelerated fractal rendering.");
    }

    #[test]
    fn test_engine_creation() {
        let engine = RustFractalShaderEngine::new();
        assert_eq!(engine.get_frame_count(), 0);
    }

    #[test]
    fn test_add_shader_module() {
        let mut engine = RustFractalShaderEngine::new();
        let result = engine.add_shader_module("mandelbrot", "shader code here");
        assert!(result.is_ok());
        assert!(engine.shader_modules.contains_key("mandelbrot"));
    }

    #[test]
    fn test_set_fractal_parameter() {
        let mut engine = RustFractalShaderEngine::new();
        engine.set_fractal_parameter("iterations", 200.0);
        assert_eq!(engine.fractal_params.iterations, 200);
    }

    #[test]
    fn test_render_frame() {
        let mut engine = RustFractalShaderEngine::new();
        let result = engine.render_frame();
        assert!(result.is_ok());
        assert_eq!(engine.get_frame_count(), 1);
    }

    #[test]
    fn test_shader_module_parameters() {
        let mut module = ShaderModule {
            name: "test".to_string(),
            source: "test shader".to_string(),
            parameters: HashMap::new(),
        };

        module.set_parameter("scale", 2.0);
        assert_eq!(module.get_parameter("scale"), Some(2.0));
        assert_eq!(module.get_parameter("nonexistent"), None);
    }

    #[test]
    fn test_parse_isf_metadata() {
        let isf_source = r#"/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
  ]
}*/

void main() {
    // shader code
}"#;

        let metadata = RustFractalShaderEngine::parse_isf_metadata(isf_source).unwrap();
        assert_eq!(metadata.categories, vec!["Generator"]);
        assert_eq!(metadata.inputs.len(), 1);
        assert_eq!(metadata.inputs[0].name, "speed");
    }

    #[test]
    fn test_convert_isf_to_wgsl() {
        let isf_source = r#"
void main() {
    vec3 color = vec3(1.0, 0.0, 0.0);
    gl_FragColor = vec4(color, 1.0);
}
"#;

        let wgsl = ShaderConverter::isf_to_wgsl(isf_source).unwrap();
        assert!(wgsl.contains("@fragment"));
        assert!(wgsl.contains("vec3<f32>"));
        assert!(wgsl.contains("vec4<f32>"));
    }

    #[test]
    fn test_convert_isf_to_hlsl() {
        let isf_source = r#"
void main() {
    vec3 color = vec3(1.0, 0.0, 0.0);
    gl_FragColor = vec4(color, 1.0);
}
"#;

        let hlsl = ShaderConverter::isf_to_hlsl(isf_source).unwrap();
        assert!(hlsl.contains("void main("));
        assert!(hlsl.contains("float3"));
        assert!(hlsl.contains("float4"));
    }
}