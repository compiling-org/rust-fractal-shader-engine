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
pub mod gesture;
pub mod osc;
pub mod ui;
pub mod nodes;
pub mod node_editor;
pub mod shader_converter;
pub mod nft;
pub mod benchmark;
pub mod fractal;
pub mod project;
pub mod scene;
pub mod animation;

// Web deployment module (only compiled for WASM targets)
#[cfg(target_arch = "wasm32")]
pub mod web;

// Re-export NFT integration for easy access
pub use nft::BlockchainNFTIntegration;

// Re-export web functionality when available
#[cfg(target_arch = "wasm32")]
pub use web::*;

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
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FractalParameters {
    pub iterations: u32,
    pub zoom: f32,
    pub offset: (f32, f32),
    pub color_palette: Vec<(f32, f32, f32)>,
    pub fractal_type: FractalType,
    pub power: f32,
    pub bailout: f32,
    pub julia_c: (f32, f32),
    pub rotation: f32,
    pub scale: f32,
    pub folding_limit: f32,
    pub folding_value: f32,

    // Mandelbulber-specific parameters
    pub mandelbulb_power: f32,
    pub mandelbox_scale: f32,
    pub mandelbox_folding_min: f32,
    pub mandelbox_folding_max: f32,

    // Quaternion parameters
    pub quaternion_c: (f32, f32, f32, f32),

    // IFS parameters
    pub ifs_scale: f32,
    pub ifs_rotation: (f32, f32, f32),
    pub ifs_offset: (f32, f32, f32),

    // Flame fractal parameters
    pub flame_variation: f32,
    pub flame_coefficients: Vec<f32>,

    // Generic fractal parameters (inspired by various techniques)
    pub generic_spiral_pitch: f32,
    pub generic_torus_major: f32,
    pub generic_torus_minor: f32,
    pub generic_helix_turns: f32,
    pub generic_vortex_strength: f32,

    // Generic TouchDesigner-style parameters
    pub generic_touch_spiral_density: f32,
    pub generic_touch_wave_frequency: f32,
    pub generic_touch_ripple_speed: f32,
    pub generic_touch_fractal_depth: f32,
}

/// Types of fractals supported
#[derive(Debug, Clone, Copy, Serialize, Deserialize)]
pub enum FractalType {
    // Classic 2D fractals
    Mandelbrot,
    Julia,
    BurningShip,
    Tricorn,
    Phoenix,

    // Distance Estimation fractals
    Mandelbox,
    Mandelbulb,
    BulbBox,

    // Mandelbulber formulas
    MengerSponge,
    KaleidoscopicIFS,
    MandelbulbV2,
    AmazingBox,
    Quaternion,

    // JWildfire/Flame fractals
    FlameFractal,
    SphericalFlame,

    // Fragmentarium formulas
    QuaternionJulia,
    Mandelbulb4D,
    SierpinskiTetrahedron,

    // Hybrid formulas
    MandelbulbCrossMandelbox,
    QuaternionMandelbulb,

    // Generic fractal types (inspired by various artists' techniques)
    GenericSpiral,
    GenericTorus,
    GenericHelix,
    GenericVortex,

    // Generic TouchDesigner-style effects
    GenericTouchSpiral,
    GenericTouchWave,
    GenericTouchRipple,
    GenericTouchFractal,

    // Special types
    Sierpinski,
    Apollonian,
    Custom,
}

/// Rendering state management
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct RenderState {
    pub width: u32,
    pub height: u32,
    pub frame_count: u64,
    pub output_mode: OutputMode,
    pub dome_radius: f32,
    pub fisheye_fov: f32,
}

/// Output projection modes for VR/360° rendering
#[derive(Debug, Clone, Copy, Serialize, Deserialize)]
pub enum OutputMode {
    Standard2D,
    Stereographic,
    Equirectangular180,
    Equirectangular360,
    Fisheye,
    Dome,
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
            fractal_type: FractalType::Mandelbrot,
            power: 2.0,
            bailout: 4.0,
            julia_c: (-0.7, 0.27015),
            rotation: 0.0,
            scale: 1.0,
            folding_limit: 1.0,
            folding_value: 2.0,

            // Mandelbulber defaults
            mandelbulb_power: 8.0,
            mandelbox_scale: 2.0,
            mandelbox_folding_min: -1.0,
            mandelbox_folding_max: 1.0,

            // Quaternion defaults
            quaternion_c: (0.0, 0.0, 0.0, 0.0),

            // IFS defaults
            ifs_scale: 2.0,
            ifs_rotation: (0.0, 0.0, 0.0),
            ifs_offset: (0.0, 0.0, 0.0),

            // Flame defaults
            flame_variation: 1.0,
            flame_coefficients: vec![1.0, 0.5, 0.0, 0.0, 0.5, 1.0],

            // Generic fractal defaults (inspired by various techniques)
            generic_spiral_pitch: 1.0,
            generic_torus_major: 2.0,
            generic_torus_minor: 0.5,
            generic_helix_turns: 3.0,
            generic_vortex_strength: 1.0,

            // Generic TouchDesigner-style defaults
            generic_touch_spiral_density: 2.0,
            generic_touch_wave_frequency: 1.0,
            generic_touch_ripple_speed: 1.0,
            generic_touch_fractal_depth: 5.0,
        }
    }
}

impl Default for RenderState {
    fn default() -> Self {
        Self {
            width: 1920,
            height: 1080,
            frame_count: 0,
            output_mode: OutputMode::Standard2D,
            dome_radius: 1.0,
            fisheye_fov: 3.14159, // 180 degrees in radians
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
            "power" => self.fractal_params.power = value,
            "bailout" => self.fractal_params.bailout = value,
            "rotation" => self.fractal_params.rotation = value,
            "scale" => self.fractal_params.scale = value,
            "folding_limit" => self.fractal_params.folding_limit = value,
            "folding_value" => self.fractal_params.folding_value = value,
            "mandelbulb_power" => self.fractal_params.mandelbulb_power = value,
            "mandelbox_scale" => self.fractal_params.mandelbox_scale = value,
            "mandelbox_folding_min" => self.fractal_params.mandelbox_folding_min = value,
            "mandelbox_folding_max" => self.fractal_params.mandelbox_folding_max = value,
            "ifs_scale" => self.fractal_params.ifs_scale = value,
            "flame_variation" => self.fractal_params.flame_variation = value,
            "generic_spiral_pitch" => self.fractal_params.generic_spiral_pitch = value,
            "generic_torus_major" => self.fractal_params.generic_torus_major = value,
            "generic_torus_minor" => self.fractal_params.generic_torus_minor = value,
            "generic_helix_turns" => self.fractal_params.generic_helix_turns = value,
            "generic_vortex_strength" => self.fractal_params.generic_vortex_strength = value,
            "generic_touch_spiral_density" => self.fractal_params.generic_touch_spiral_density = value,
            "generic_touch_wave_frequency" => self.fractal_params.generic_touch_wave_frequency = value,
            "generic_touch_ripple_speed" => self.fractal_params.generic_touch_ripple_speed = value,
            "generic_touch_fractal_depth" => self.fractal_params.generic_touch_fractal_depth = value,
            "dome_radius" => self.render_state.dome_radius = value,
            "fisheye_fov" => self.render_state.fisheye_fov = value,
            _ => {}
        }
    }

    pub fn set_output_mode(&mut self, mode: OutputMode) {
        self.render_state.output_mode = mode;
    }

    pub fn set_fractal_type(&mut self, fractal_type: FractalType) {
        self.fractal_params.fractal_type = fractal_type;
    }

    pub fn set_julia_c(&mut self, real: f32, imag: f32) {
        self.fractal_params.julia_c = (real, imag);
    }

    pub fn set_offset(&mut self, x: f32, y: f32) {
        self.fractal_params.offset = (x, y);
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

    /// Generate fractal shader code based on parameters
    pub fn generate_fractal_shader(&self) -> Result<String, Box<dyn std::error::Error>> {
        let mut shader = String::new();

        // WGSL header with extended uniforms
        shader.push_str("@group(0) @binding(0) var<uniform> time: f32;\n");
        shader.push_str("@group(0) @binding(1) var<uniform> resolution: vec2<f32>;\n");
        shader.push_str("@group(0) @binding(2) var<uniform> mouse: vec2<f32>;\n");
        shader.push_str("@group(0) @binding(3) var<uniform> zoom: f32;\n");
        shader.push_str("@group(0) @binding(4) var<uniform> offset: vec2<f32>;\n");
        shader.push_str("@group(0) @binding(5) var<uniform> power: f32;\n");
        shader.push_str("@group(0) @binding(6) var<uniform> bailout: f32;\n");
        shader.push_str("@group(0) @binding(7) var<uniform> julia_c: vec2<f32>;\n");
        shader.push_str("@group(0) @binding(8) var<uniform> rotation: f32;\n");
        shader.push_str("@group(0) @binding(9) var<uniform> scale: f32;\n");
        shader.push_str("@group(0) @binding(10) var<uniform> folding_limit: f32;\n");
        shader.push_str("@group(0) @binding(11) var<uniform> folding_value: f32;\n");
        shader.push_str("@group(0) @binding(12) var<uniform> iterations: u32;\n");
        shader.push_str("@group(0) @binding(13) var<uniform> mandelbulb_power: f32;\n");
        shader.push_str("@group(0) @binding(14) var<uniform> mandelbox_scale: f32;\n");
        shader.push_str("@group(0) @binding(15) var<uniform> mandelbox_folding_min: f32;\n");
        shader.push_str("@group(0) @binding(16) var<uniform> mandelbox_folding_max: f32;\n");
        shader.push_str("@group(0) @binding(17) var<uniform> quaternion_c: vec4<f32>;\n");
        shader.push_str("@group(0) @binding(18) var<uniform> ifs_scale: f32;\n");
        shader.push_str("@group(0) @binding(19) var<uniform> ifs_rotation: vec3<f32>;\n");
        shader.push_str("@group(0) @binding(20) var<uniform> ifs_offset: vec3<f32>;\n");
        shader.push_str("@group(0) @binding(21) var<uniform> flame_variation: f32;\n");
        shader.push_str("@group(0) @binding(22) var<uniform> output_mode: u32;\n");
        shader.push_str("@group(0) @binding(23) var<uniform> dome_radius: f32;\n");
        shader.push_str("@group(0) @binding(24) var<uniform> fisheye_fov: f32;\n\n");

        shader.push_str("@fragment\n");
        shader.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");
        shader.push_str("    let uv = (coord.xy - 0.5 * resolution) / resolution.y;\n");
        shader.push_str("    let c = uv * zoom + offset;\n\n");

        // Apply output projection transformations
        shader.push_str("    // Output projection transformations\n");
        shader.push_str("    var projected_uv = uv;\n");
        shader.push_str("    if (output_mode == 1u) { // Stereographic\n");
        shader.push_str("        let r = length(uv);\n");
        shader.push_str("        let theta = atan2(uv.y, uv.x);\n");
        shader.push_str("        projected_uv = vec2<f32>(theta / 3.14159, 2.0 * atan(r) / 3.14159 - 1.0);\n");
        shader.push_str("    } else if (output_mode == 2u) { // Equirectangular 180°\n");
        shader.push_str("        let theta = atan2(uv.y, uv.x);\n");
        shader.push_str("        let phi = atan(length(uv));\n");
        shader.push_str("        projected_uv = vec2<f32>(theta / 3.14159, phi / 1.5708);\n");
        shader.push_str("    } else if (output_mode == 3u) { // Equirectangular 360°\n");
        shader.push_str("        let theta = atan2(uv.y, uv.x);\n");
        shader.push_str("        let phi = atan(length(uv) * 2.0);\n");
        shader.push_str("        projected_uv = vec2<f32>(theta / 3.14159, phi / 1.5708);\n");
        shader.push_str("    } else if (output_mode == 4u) { // Fisheye\n");
        shader.push_str("        let r = length(uv);\n");
        shader.push_str("        let theta = atan2(uv.y, uv.x);\n");
        shader.push_str("        let fisheye_r = atan(r * fisheye_fov) / fisheye_fov;\n");
        shader.push_str("        projected_uv = vec2<f32>(cos(theta) * fisheye_r, sin(theta) * fisheye_r);\n");
        shader.push_str("    } else if (output_mode == 5u) { // Dome\n");
        shader.push_str("        let r = length(uv);\n");
        shader.push_str("        if (r <= dome_radius) {\n");
        shader.push_str("            let height = sqrt(dome_radius * dome_radius - r * r);\n");
        shader.push_str("            projected_uv = uv * (height / dome_radius);\n");
        shader.push_str("        } else {\n");
        shader.push_str("            projected_uv = vec2<f32>(0.0);\n");
        shader.push_str("        }\n");
        shader.push_str("    }\n");
        shader.push_str("    let c = projected_uv * zoom + offset;\n\n");

        // Generate fractal computation based on type
        match self.fractal_params.fractal_type {
            FractalType::Mandelbrot => {
                shader.push_str("    // Mandelbrot Set\n");
                shader.push_str("    var z = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + c.x;\n");
                shader.push_str("        z.y = 2.0 * z.x * z.y + c.y;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::Julia => {
                shader.push_str("    // Julia Set\n");
                shader.push_str("    var z = c;\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + julia_c.x;\n");
                shader.push_str("        z.y = 2.0 * z.x * z.y + julia_c.y;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::BurningShip => {
                shader.push_str("    // Burning Ship Fractal\n");
                shader.push_str("    var z = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + c.x;\n");
                shader.push_str("        z.y = abs(2.0 * z.x * z.y) + c.y;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::Tricorn => {
                shader.push_str("    // Tricorn Fractal\n");
                shader.push_str("    var z = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + c.x;\n");
                shader.push_str("        z.y = -2.0 * z.x * z.y + c.y;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::Phoenix => {
                shader.push_str("    // Phoenix Fractal\n");
                shader.push_str("    var z = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var z_prev = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + c.x + 0.269 * z_prev.x;\n");
                shader.push_str("        z.y = 2.0 * z.x * z.y + c.y + 0.269 * z_prev.y;\n");
                shader.push_str("        z_prev = z;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::Mandelbox => {
                shader.push_str("    // Mandelbox Fractal\n");
                shader.push_str("    var z = c;\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        // Box folding\n");
                shader.push_str("        if (z.x > folding_limit) { z.x = folding_value - z.x; }\n");
                shader.push_str("        else if (z.x < -folding_limit) { z.x = -folding_value - z.x; }\n");
                shader.push_str("        if (z.y > folding_limit) { z.y = folding_value - z.y; }\n");
                shader.push_str("        else if (z.y < -folding_limit) { z.y = -folding_value - z.y; }\n");
                shader.push_str("        // Sphere folding\n");
                shader.push_str("        let r2 = dot(z, z);\n");
                shader.push_str("        if (r2 < folding_limit) {\n");
                shader.push_str("            z = z * (folding_value / r2);\n");
                shader.push_str("        } else if (r2 < folding_value) {\n");
                shader.push_str("            z = z * (folding_value / r2);\n");
                shader.push_str("        }\n");
                shader.push_str("        z = scale * z + c;\n");
                shader.push_str("    }\n");
            }
            FractalType::Mandelbulb => {
                shader.push_str("    // Mandelbulb Fractal (3D power fractal)\n");
                shader.push_str("    var z = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let r = length(z);\n");
                shader.push_str("        let theta = atan2(z.y, z.x);\n");
                shader.push_str("        let phi = asin(z.z / r);\n");
                shader.push_str("        let new_r = pow(r, mandelbulb_power);\n");
                shader.push_str("        let new_theta = theta * mandelbulb_power;\n");
                shader.push_str("        let new_phi = phi * mandelbulb_power;\n");
                shader.push_str("        z = new_r * vec3<f32>(cos(new_theta) * cos(new_phi), sin(new_theta) * cos(new_phi), sin(new_phi));\n");
                shader.push_str("        z = z + vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    }\n");
            }
            FractalType::BulbBox => {
                shader.push_str("    // BulbBox (Mandelbulb + Mandelbox hybrid)\n");
                shader.push_str("    var z = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        // Mandelbox folding\n");
                shader.push_str("        if (z.x > folding_limit) { z.x = folding_value - z.x; }\n");
                shader.push_str("        else if (z.x < -folding_limit) { z.x = -folding_value - z.x; }\n");
                shader.push_str("        if (z.y > folding_limit) { z.y = folding_value - z.y; }\n");
                shader.push_str("        else if (z.y < -folding_limit) { z.y = -folding_value - z.y; }\n");
                shader.push_str("        // Mandelbulb power\n");
                shader.push_str("        let r = length(z);\n");
                shader.push_str("        let theta = atan2(z.y, z.x);\n");
                shader.push_str("        let phi = asin(z.z / r);\n");
                shader.push_str("        let new_r = pow(r, mandelbulb_power);\n");
                shader.push_str("        let new_theta = theta * mandelbulb_power;\n");
                shader.push_str("        let new_phi = phi * mandelbulb_power;\n");
                shader.push_str("        z = new_r * vec3<f32>(cos(new_theta) * cos(new_phi), sin(new_theta) * cos(new_phi), sin(new_phi));\n");
                shader.push_str("        z = mandelbox_scale * z + vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    }\n");
            }
            FractalType::MengerSponge => {
                shader.push_str("    // Menger Sponge IFS\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var scale = 1.0;\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        p = abs(p);\n");
                shader.push_str("        if (p.x < p.y) { let temp = p.x; p.x = p.y; p.y = temp; }\n");
                shader.push_str("        if (p.x < p.z) { let temp = p.x; p.x = p.z; p.z = temp; }\n");
                shader.push_str("        if (p.y < p.z) { let temp = p.y; p.y = p.z; p.z = temp; }\n");
                shader.push_str("        p = p * 3.0 - 2.0;\n");
                shader.push_str("        if (p.z < -1.0) { p.z = p.z + 2.0; }\n");
                shader.push_str("        scale = scale * 3.0;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::AmazingBox => {
                shader.push_str("    // Amazing Box (Mandelbox variant)\n");
                shader.push_str("    var z = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        // Box folding\n");
                shader.push_str("        z = clamp(z, mandelbox_folding_min, mandelbox_folding_max) * 2.0 - z;\n");
                shader.push_str("        // Sphere folding\n");
                shader.push_str("        let r2 = dot(z, z);\n");
                shader.push_str("        if (r2 < folding_limit) {\n");
                shader.push_str("            z = z * (folding_value / r2);\n");
                shader.push_str("        } else if (r2 < folding_value) {\n");
                shader.push_str("            z = z * (folding_value / r2);\n");
                shader.push_str("        }\n");
                shader.push_str("        z = mandelbox_scale * z + vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    }\n");
            }
            FractalType::Quaternion => {
                shader.push_str("    // Quaternion Julia Set\n");
                shader.push_str("    var z = vec4<f32>(c.x, c.y, 0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        // Quaternion multiplication: z = z*z + c\n");
                shader.push_str("        let z_temp = vec4<f32>(\n");
                shader.push_str("            z.x*z.x - z.y*z.y - z.z*z.z - z.w*z.w + quaternion_c.x,\n");
                shader.push_str("            2.0*z.x*z.y + 2.0*z.z*z.w + quaternion_c.y,\n");
                shader.push_str("            2.0*z.x*z.z + 2.0*z.y*z.w + quaternion_c.z,\n");
                shader.push_str("            2.0*z.x*z.w + 2.0*z.y*z.z + quaternion_c.w\n");
                shader.push_str("        );\n");
                shader.push_str("        z = z_temp;\n");
                shader.push_str("    }\n");
            }
            FractalType::KaleidoscopicIFS => {
                shader.push_str("    // Kaleidoscopic IFS (Mandelbulber-inspired)\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var scale = ifs_scale;\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        // Folding and scaling\n");
                shader.push_str("        p = abs(p);\n");
                shader.push_str("        if (p.x < p.y) { let temp = p.x; p.x = p.y; p.y = temp; }\n");
                shader.push_str("        if (p.x < p.z) { let temp = p.x; p.x = p.z; p.z = temp; }\n");
                shader.push_str("        if (p.y < p.z) { let temp = p.y; p.y = p.z; p.z = temp; }\n");
                shader.push_str("        p = p * scale - ifs_offset;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::MandelbulbV2 => {
                shader.push_str("    // Mandelbulb V2 (improved version)\n");
                shader.push_str("    var z = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let r = length(z);\n");
                shader.push_str("        let theta = atan2(z.y, z.x);\n");
                shader.push_str("        let phi = asin(z.z / r);\n");
                shader.push_str("        let new_r = pow(r, mandelbulb_power);\n");
                shader.push_str("        let new_theta = theta * mandelbulb_power + ifs_rotation.x;\n");
                shader.push_str("        let new_phi = phi * mandelbulb_power + ifs_rotation.y;\n");
                shader.push_str("        z = new_r * vec3<f32>(\n");
                shader.push_str("            cos(new_theta) * cos(new_phi),\n");
                shader.push_str("            sin(new_theta) * cos(new_phi),\n");
                shader.push_str("            sin(new_phi)\n");
                shader.push_str("        );\n");
                shader.push_str("        z = z + vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericSpiral => {
                shader.push_str("    // Generic Spiral Fractal (inspired by various techniques)\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        let angle = atan2(p.y, p.x) + time * generic_spiral_pitch;\n");
                shader.push_str("        let radius = length(p.xy);\n");
                shader.push_str("        p.x = cos(angle) * radius;\n");
                shader.push_str("        p.y = sin(angle) * radius;\n");
                shader.push_str("        p.z = p.z + generic_spiral_pitch * 0.1;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericTorus => {
                shader.push_str("    // Generic Torus Fractal (inspired by various techniques)\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        let q = vec2<f32>(length(p.xz) - generic_torus_major, p.y);\n");
                shader.push_str("        let len = length(q) - generic_torus_minor;\n");
                shader.push_str("        p = p + normalize(p) * len * 0.5;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericHelix => {
                shader.push_str("    // Generic Helix Fractal (inspired by various techniques)\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        let angle = atan2(p.y, p.x) + p.z * generic_helix_turns;\n");
                shader.push_str("        let radius = length(p.xy);\n");
                shader.push_str("        p.x = cos(angle) * radius;\n");
                shader.push_str("        p.y = sin(angle) * radius;\n");
                shader.push_str("        p.z = p.z + 0.1;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericVortex => {
                shader.push_str("    // Generic Vortex Fractal (inspired by various techniques)\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        let angle = atan2(p.y, p.x);\n");
                shader.push_str("        let radius = length(p.xy);\n");
                shader.push_str("        let vortex_angle = angle + generic_vortex_strength / radius;\n");
                shader.push_str("        p.x = cos(vortex_angle) * radius;\n");
                shader.push_str("        p.y = sin(vortex_angle) * radius;\n");
                shader.push_str("        p.z = p.z * 0.99 + sin(time + radius) * 0.01;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericTouchSpiral => {
                shader.push_str("    // Generic TouchDesigner-style Spiral Fractal\n");
                shader.push_str("    var p = vec2<f32>(c.x, c.y);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(p, p) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let angle = atan2(p.y, p.x);\n");
                shader.push_str("        let radius = length(p);\n");
                shader.push_str("        let spiral_angle = angle + radius * generic_touch_spiral_density;\n");
                shader.push_str("        p.x = cos(spiral_angle) * radius;\n");
                shader.push_str("        p.y = sin(spiral_angle) * radius;\n");
                shader.push_str("        p = p * scale + c;\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericTouchWave => {
                shader.push_str("    // Generic TouchDesigner-style Wave Fractal\n");
                shader.push_str("    var p = vec2<f32>(c.x, c.y);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(p, p) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let wave = sin(p.x * generic_touch_wave_frequency + time) + cos(p.y * generic_touch_wave_frequency + time);\n");
                shader.push_str("        p.x = p.x + wave * 0.1;\n");
                shader.push_str("        p.y = p.y + cos(p.x * generic_touch_wave_frequency + time * 0.5) * 0.1;\n");
                shader.push_str("        p = p * scale + c;\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericTouchRipple => {
                shader.push_str("    // Generic TouchDesigner-style Ripple Fractal\n");
                shader.push_str("    var p = vec2<f32>(c.x, c.y);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(p, p) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let dist = length(p);\n");
                shader.push_str("        let ripple = sin(dist * generic_touch_ripple_speed - time) / dist;\n");
                shader.push_str("        p = p + normalize(p) * ripple * 0.1;\n");
                shader.push_str("        p = p * scale + c;\n");
                shader.push_str("    }\n");
            }
            FractalType::GenericTouchFractal => {
                shader.push_str("    // Generic TouchDesigner-style Deep Fractal\n");
                shader.push_str("    var p = vec3<f32>(c.x, c.y, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations; iter = iter + 1u) {\n");
                shader.push_str("        // Box folding\n");
                shader.push_str("        p = abs(p) - vec3<f32>(1.0);\n");
                shader.push_str("        // Sphere folding with variable radius\n");
                shader.push_str("        let r2 = dot(p, p);\n");
                shader.push_str("        if (r2 < 0.25) {\n");
                shader.push_str("            p = p * (4.0 / r2);\n");
                shader.push_str("        } else if (r2 < 1.0) {\n");
                shader.push_str("            p = p * (1.0 / r2);\n");
                shader.push_str("        }\n");
                shader.push_str("        p = p * generic_touch_fractal_depth + c;\n");
                shader.push_str("        if (dot(p, p) > bailout) { break; }\n");
                shader.push_str("    }\n");
            }
            _ => {
                shader.push_str("    // Default Mandelbrot\n");
                shader.push_str("    var z = vec2<f32>(0.0, 0.0);\n");
                shader.push_str("    var iter: u32 = 0u;\n");
                shader.push_str("    for (; iter < iterations && dot(z, z) < bailout; iter = iter + 1u) {\n");
                shader.push_str("        let z_temp = z.x * z.x - z.y * z.y + c.x;\n");
                shader.push_str("        z.y = 2.0 * z.x * z.y + c.y;\n");
                shader.push_str("        z.x = z_temp;\n");
                shader.push_str("    }\n");
            }
        }

        // Color computation
        shader.push_str("\n    // Color computation\n");
        shader.push_str("    let t = f32(iter) / f32(iterations);\n");
        shader.push_str("    let color = vec3<f32>(\n");
        shader.push_str("        0.5 + 0.5 * cos(6.283185 * t + time),\n");
        shader.push_str("        0.5 + 0.5 * cos(6.283185 * t + time + 2.094395),\n");
        shader.push_str("        0.5 + 0.5 * cos(6.283185 * t + time + 4.188790)\n");
        shader.push_str("    );\n");
        shader.push_str("    return vec4<f32>(color, 1.0);\n");
        shader.push_str("}\n");

        Ok(shader)
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

// Re-export the NodeEditorPlugin for easy access (placeholder)
// pub use node_editor::NodeEditorPlugin;

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
    fn test_generate_fractal_shader() {
        let engine = RustFractalShaderEngine::new();
        let shader = engine.generate_fractal_shader().unwrap();
        assert!(shader.contains("@fragment"));
        assert!(shader.contains("Mandelbrot"));
        assert!(shader.contains("vec2<f32>"));
    }

    #[test]
    fn test_fractal_parameters() {
        let mut engine = RustFractalShaderEngine::new();
        engine.set_fractal_type(FractalType::Julia);
        engine.set_julia_c(0.5, 0.5);
        engine.set_fractal_parameter("power", 3.0);

        assert!(matches!(engine.fractal_params.fractal_type, FractalType::Julia));
        assert_eq!(engine.fractal_params.julia_c, (0.5, 0.5));
        assert_eq!(engine.fractal_params.power, 3.0);
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