//! Fractal Engine Implementation
//!
//! This module provides the core fractal computation engine with distance estimation
//! algorithms for various fractal types including Mandelbrot, Mandelbulb, Mandelbox, etc.

use super::types::*;
use nalgebra::{Vector3, Vector4};

/// Fractal Engine struct definition
pub struct FractalEngine {
    pub parameters: FractalParameters,
    pub quality_settings: QualitySettings,
    pub stats: FractalStats,
}

impl FractalEngine {
    /// Create a new fractal engine with default parameters
    pub fn new() -> Self {
        Self {
            parameters: FractalParameters::default(),
            quality_settings: QualitySettings::default(),
            stats: FractalStats::default(),
        }
    }
}

impl Default for FractalEngine {
    fn default() -> Self {
        Self::new()
    }
}