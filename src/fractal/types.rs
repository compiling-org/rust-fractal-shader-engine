//! Fractal Types and Data Structures
//!
//! This module defines the core types used throughout the fractal generation system.

use nalgebra::{Matrix4, Vector3, Vector4};
use std::collections::HashMap;

/// Core fractal formula types
#[derive(Debug, Clone)]
pub enum FractalFormula {
    /// Classic Mandelbrot set (2D)
    Mandelbrot {
        center: [f32; 2],
        zoom: f32,
        max_iterations: u32,
    },
    /// 3D Mandelbulb
    Mandelbulb {
        power: f32,
        max_iterations: u32,
    },
    /// Mandelbox (folding-based)
    Mandelbox {
        scale: f32,
        folding_limit: f32,
        max_iterations: u32,
    },
    /// Iterated Function System
    IFS {
        transforms: Vec<Matrix4<f32>>,
        probabilities: Vec<f32>,
        max_iterations: u32,
    },
    /// Quaternion Julia set
    QuaternionJulia {
        c: [f32; 4],
        max_iterations: u32,
    },
    /// Custom formula with user-defined distance function
    Custom {
        name: String,
        distance_function: String,
        parameters: HashMap<String, f32>,
    },
}

/// Distance estimation result
#[derive(Debug, Clone)]
pub struct DistanceResult {
    pub distance: f32,
    pub iterations: u32,
    pub orbit_trap: Option<Vector3<f32>>,
}

/// Fractal rendering parameters
#[derive(Debug, Clone)]
pub struct FractalParameters {
    pub formula: FractalFormula,
    pub position: Vector3<f32>,
    pub rotation: Vector3<f32>,
    pub scale: f32,
    pub color_map: ColorMapping,
    pub lighting: LightingParameters,
    pub volumetric: VolumetricParameters,
}

impl Default for FractalParameters {
    fn default() -> Self {
        Self {
            formula: FractalFormula::Mandelbulb {
                power: 8.0,
                max_iterations: 100,
            },
            position: Vector3::zeros(),
            rotation: Vector3::zeros(),
            scale: 1.0,
            color_map: ColorMapping::default(),
            lighting: LightingParameters::default(),
            volumetric: VolumetricParameters::default(),
        }
    }
}

/// Color mapping for fractal visualization
#[derive(Debug, Clone)]
pub struct ColorMapping {
    pub palette: Vec<Vector3<f32>>,
    pub cycle_speed: f32,
    pub saturation: f32,
    pub brightness: f32,
    pub contrast: f32,
}

impl Default for ColorMapping {
    fn default() -> Self {
        Self {
            palette: vec![
                Vector3::new(0.0, 0.0, 0.0),    // Black
                Vector3::new(0.2, 0.0, 0.4),    // Dark purple
                Vector3::new(0.4, 0.0, 0.8),    // Purple
                Vector3::new(0.8, 0.2, 1.0),    // Light purple
                Vector3::new(1.0, 0.4, 0.8),    // Pink
                Vector3::new(1.0, 0.8, 0.4),    // Orange
                Vector3::new(1.0, 1.0, 0.8),    // Yellow
                Vector3::new(1.0, 1.0, 1.0),    // White
            ],
            cycle_speed: 1.0,
            saturation: 1.0,
            brightness: 1.0,
            contrast: 1.0,
        }
    }
}

/// Lighting parameters
#[derive(Debug, Clone)]
pub struct LightingParameters {
    pub ambient_intensity: f32,
    pub diffuse_intensity: f32,
    pub specular_intensity: f32,
    pub shininess: f32,
    pub light_direction: Vector3<f32>,
    pub light_color: Vector3<f32>,
}

impl Default for LightingParameters {
    fn default() -> Self {
        Self {
            ambient_intensity: 0.2,
            diffuse_intensity: 0.8,
            specular_intensity: 0.5,
            shininess: 32.0,
            light_direction: Vector3::new(1.0, 1.0, 1.0).normalize(),
            light_color: Vector3::new(1.0, 1.0, 1.0),
        }
    }
}

/// Volumetric rendering parameters
#[derive(Debug, Clone)]
pub struct VolumetricParameters {
    pub density: f32,
    pub absorption: f32,
    pub scattering: f32,
    pub fog_color: Vector3<f32>,
    pub fog_density: f32,
    pub max_steps: u32,
    pub step_size: f32,
}

impl Default for VolumetricParameters {
    fn default() -> Self {
        Self {
            density: 0.1,
            absorption: 0.1,
            scattering: 0.5,
            fog_color: Vector3::new(0.8, 0.9, 1.0),
            fog_density: 0.01,
            max_steps: 100,
            step_size: 0.01,
        }
    }
}

/// Rendering quality settings
#[derive(Debug, Clone)]
pub struct QualitySettings {
    pub resolution: (u32, u32),
    pub samples_per_pixel: u32,
    pub max_ray_depth: u32,
    pub adaptive_sampling: bool,
    pub denoising: bool,
}

impl Default for QualitySettings {
    fn default() -> Self {
        Self {
            resolution: (1920, 1080),
            samples_per_pixel: 1,
            max_ray_depth: 8,
            adaptive_sampling: false,
            denoising: false,
        }
    }
}

/// Fractal statistics for performance monitoring
#[derive(Debug, Clone)]
pub struct FractalStats {
    pub total_rays: u64,
    pub total_samples: u64,
    pub render_time: f64,
    pub average_iterations: f32,
    pub max_iterations: u32,
    pub convergence_rate: f32,
}

impl Default for FractalStats {
    fn default() -> Self {
        Self {
            total_rays: 0,
            total_samples: 0,
            render_time: 0.0,
            average_iterations: 0.0,
            max_iterations: 0,
            convergence_rate: 0.0,
        }
    }
}

/// Blend modes for fractal composition
#[derive(Debug, Clone, Copy, PartialEq)]
pub enum BlendMode {
    Union,
    Intersection,
    Subtraction,
    SmoothUnion(f32),
    SmoothIntersection(f32),
    SmoothSubtraction(f32),
}

/// Material properties for fractal surfaces
#[derive(Debug, Clone)]
pub struct MaterialProperties {
    pub base_color: Vector3<f32>,
    pub metallic: f32,
    pub roughness: f32,
    pub emissive: Vector3<f32>,
    pub subsurface: f32,
    pub specular_tint: f32,
    pub anisotropic: f32,
    pub sheen: f32,
    pub sheen_tint: f32,
    pub clearcoat: f32,
    pub clearcoat_gloss: f32,
}

impl Default for MaterialProperties {
    fn default() -> Self {
        Self {
            base_color: Vector3::new(0.8, 0.8, 0.8),
            metallic: 0.0,
            roughness: 0.5,
            emissive: Vector3::zeros(),
            subsurface: 0.0,
            specular_tint: 0.0,
            anisotropic: 0.0,
            sheen: 0.0,
            sheen_tint: 0.0,
            clearcoat: 0.0,
            clearcoat_gloss: 0.0,
        }
    }
}

/// Texture coordinate transformation
#[derive(Debug, Clone)]
pub struct TextureTransform {
    pub offset: Vector3<f32>,
    pub scale: Vector3<f32>,
    pub rotation: Vector3<f32>,
}

impl Default for TextureTransform {
    fn default() -> Self {
        Self {
            offset: Vector3::zeros(),
            scale: Vector3::new(1.0, 1.0, 1.0),
            rotation: Vector3::zeros(),
        }
    }
}

/// Noise parameters for procedural effects
#[derive(Debug, Clone)]
pub struct NoiseParameters {
    pub frequency: f32,
    pub amplitude: f32,
    pub octaves: u32,
    pub lacunarity: f32,
    pub persistence: f32,
    pub seed: u32,
}

impl Default for NoiseParameters {
    fn default() -> Self {
        Self {
            frequency: 1.0,
            amplitude: 1.0,
            octaves: 4,
            lacunarity: 2.0,
            persistence: 0.5,
            seed: 0,
        }
    }
}