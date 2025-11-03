//! Fractal Engine Implementation
//!
//! This module provides the core fractal computation engine with distance estimation
//! algorithms for various fractal types including Mandelbrot, Mandelbulb, Mandelbox, etc.

use super::types::*;
use super::formulas::*;
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

    /// Get parameters reference
    pub fn parameters(&self) -> &FractalParameters {
        &self.parameters
    }

    /// Get mutable parameters reference
    pub fn parameters_mut(&mut self) -> &mut FractalParameters {
        &mut self.parameters
    }

    /// Get quality settings reference
    pub fn quality_settings(&self) -> &QualitySettings {
        &self.quality_settings
    }

    /// Set quality settings
    pub fn set_quality_settings(&mut self, settings: QualitySettings) {
        self.quality_settings = settings;
    }

    /// Get stats reference
    pub fn stats(&self) -> &FractalStats {
        &self.stats
    }

    /// Reset stats
    pub fn reset_stats(&mut self) {
        self.stats = FractalStats::default();
    }

    /// Compute color from distance result
    pub fn compute_color(&self, result: &DistanceResult) -> Vector3<f32> {
        self.adjust_saturation(self.compute_base_color(result), self.parameters.color_saturation)
    }

    /// Compute base color from iteration data
    fn compute_base_color(&self, result: &DistanceResult) -> Vector3<f32> {
        let t = result.iterations as f32 / self.parameters.max_iterations as f32;

        // Smooth coloring for better gradients
        let smoothed_t = if result.escaped {
            let log_zn = (result.final_z.norm() / result.final_z.norm().ln()).ln() / 2.0_f32.ln();
            (result.iterations as f32 + 1.0 - log_zn) / self.parameters.max_iterations as f32
        } else {
            t
        };

        // Apply color palette
        match self.parameters.color_palette.len() {
            0 => Vector3::new(0.0, 0.0, 0.0), // Black if no palette
            1 => self.parameters.color_palette[0],
            _ => {
                let palette_size = self.parameters.color_palette.len() as f32;
                let index = (smoothed_t * (palette_size - 1.0)) as usize;
                let frac = smoothed_t * (palette_size - 1.0) - index as f32;

                if index >= self.parameters.color_palette.len() - 1 {
                    self.parameters.color_palette.last().unwrap().clone()
                } else {
                    let color1 = &self.parameters.color_palette[index];
                    let color2 = &self.parameters.color_palette[index + 1];
                    color1.lerp(color2, frac)
                }
            }
        }
    }

    /// Adjust color saturation
    fn adjust_saturation(&self, color: Vector3<f32>, saturation: f32) -> Vector3<f32> {
        let luminance = color.dot(&Vector3::new(0.299, 0.587, 0.114));
        let desaturated = Vector3::new(luminance, luminance, luminance);
        color.lerp(&desaturated, 1.0 - saturation)
    }

    /// Get max iterations for current settings
    fn get_max_iterations(&self) -> u32 {
        self.parameters.max_iterations
    }

    /// Compute distance for Mandelbrot set
    fn compute_mandelbrot_distance(&self, point: Vector3<f32>, center: [f32; 2], zoom: f32, max_iterations: u32) -> DistanceResult {
        let c = Vector3::new(
            center[0] + (point.x - center[0]) / zoom,
            center[1] + (point.y - center[1]) / zoom,
            0.0
        );

        let mut z = Vector3::new(0.0, 0.0, 0.0);
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            // Mandelbrot iteration: z = z² + c
            let z_squared = Vector3::new(
                z.x * z.x - z.y * z.y,
                2.0 * z.x * z.y,
                0.0
            );
            z = z_squared + c;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }


    /// Compute distance for Mandelbulb fractal
    fn compute_mandelbulb_distance(&self, point: Vector3<f32>, power: f32, max_iterations: u32) -> DistanceResult {
        let mut z = point;
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            // Convert to spherical coordinates
            let r = z.norm();
            let theta = z.y.atan2(z.x);
            let phi = (z.z / r).asin();

            // Apply power
            let new_r = r.powf(power);
            let new_theta = theta * power;
            let new_phi = phi * power;

            // Convert back to cartesian
            z = Vector3::new(
                new_r * new_theta.cos() * new_phi.cos(),
                new_r * new_theta.sin() * new_phi.cos(),
                new_r * new_phi.sin()
            ) + point;

            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }

    /// Compute distance for Mandelbox fractal
    fn compute_mandelbox_distance(&self, point: Vector3<f32>, scale: f32, max_iterations: u32) -> DistanceResult {
        let mut z = point;
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            // Box folding
            z.x = z.x.clamp(-1.0, 1.0) * 2.0 - z.x;
            z.y = z.y.clamp(-1.0, 1.0) * 2.0 - z.y;
            z.z = z.z.clamp(-1.0, 1.0) * 2.0 - z.z;

            // Sphere folding
            let r2 = z.norm_squared();
            if r2 < 0.25 {
                z = z * (4.0 / r2);
            } else if r2 < 1.0 {
                z = z * (1.0 / r2);
            }

            // Scaling
            z = z * scale + point;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }

    /// Main distance computation function
    pub fn compute_distance(&self, point: Vector3<f32>) -> DistanceResult {
        match self.parameters.formula {
            FractalFormula::Mandelbrot { center, zoom } => {
                self.compute_mandelbrot_distance(point, center, zoom, self.get_max_iterations())
            }
            FractalFormula::Mandelbulb { power } => {
                self.compute_mandelbulb_distance(point, power, self.get_max_iterations())
            }
            FractalFormula::Mandelbox { scale } => {
                self.compute_mandelbox_distance(point, scale, self.get_max_iterations())
            }
            FractalFormula::Julia { c, max_iterations } => {
                self.compute_julia_distance(point, c, max_iterations)
            }
            FractalFormula::BurningShip { max_iterations } => {
                self.compute_burning_ship_distance(point, max_iterations)
            }
            FractalFormula::Tricorn { max_iterations } => {
                self.compute_tricorn_distance(point, max_iterations)
            }
            FractalFormula::MengerSponge { iterations } => {
                self.compute_menger_sponge_distance(point, iterations)
            }
            FractalFormula::Sierpinski { iterations } => {
                self.compute_sierpinski_distance(point, iterations)
            }
            FractalFormula::Apollonian { iterations } => {
                self.compute_apollonian_distance(point, iterations)
            }
            FractalFormula::QuaternionJulia { c, max_iterations } => {
                self.compute_quaternion_julia_distance(point, c, max_iterations)
            }
            FractalFormula::KaleidoscopicIFS { scale, rotation, offset } => {
                self.compute_kaleidoscopic_ifs_distance(point, scale, rotation, offset)
            }
            FractalFormula::Custom { .. } => {
                // For custom formulas, fall back to Mandelbrot
                self.compute_mandelbrot_distance(point, [0.0, 0.0], 1.0, self.get_max_iterations())
            }
        }
    }

    /// Compute Julia set distance
    fn compute_julia_distance(&self, point: Vector3<f32>, c: [f32; 2], max_iterations: u32) -> DistanceResult {
        let mut z = Vector3::new(point.x, point.y, 0.0);
        let c_vec = Vector3::new(c[0], c[1], 0.0);
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            let z_squared = Vector3::new(
                z.x * z.x - z.y * z.y,
                2.0 * z.x * z.y,
                0.0
            );
            z = z_squared + c_vec;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }

    /// Compute Burning Ship distance
    fn compute_burning_ship_distance(&self, point: Vector3<f32>, max_iterations: u32) -> DistanceResult {
        let mut z = Vector3::new(0.0, 0.0, 0.0);
        let c = Vector3::new(point.x, point.y, 0.0);
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            let z_squared = Vector3::new(
                z.x * z.x - z.y * z.y,
                f32::abs(2.0 * z.x * z.y),
                0.0
            );
            z = z_squared + c;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }

    /// Compute Tricorn distance
    fn compute_tricorn_distance(&self, point: Vector3<f32>, max_iterations: u32) -> DistanceResult {
        let mut z = Vector3::new(0.0, 0.0, 0.0);
        let c = Vector3::new(point.x, point.y, 0.0);
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            let z_squared = Vector3::new(
                z.x * z.x - z.y * z.y,
                -2.0 * z.x * z.y,
                0.0
            );
            z = z_squared + c;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: z,
        }
    }

    /// Compute Menger Sponge distance
    fn compute_menger_sponge_distance(&self, point: Vector3<f32>, iterations: u32) -> DistanceResult {
        let mut p = point;
        let mut d = DistancePrimitives::box_distance(p, Vector3::zeros(), Vector3::new(1.0, 1.0, 1.0));

        for _ in 0..iterations {
            let q = p.abs() - Vector3::new(1.0, 1.0, 1.0);
            p = p * 3.0;

            // Cross shape removal
            if p.x < q.x && p.y < q.y {
                let cross_size = Vector3::new(1.0, 1.0, 3.0);
                d = d.max(-DistancePrimitives::box_distance(p, Vector3::zeros(), cross_size));
            }

            p = p - Vector3::new(p.x.round(), p.y.round(), p.z.round());
        }

        DistanceResult {
            distance: d,
            iterations: iterations.min(self.get_max_iterations()),
            escaped: false, // Menger sponge doesn't "escape"
            final_z: p,
        }
    }

    /// Compute Sierpinski distance
    fn compute_sierpinski_distance(&self, point: Vector3<f32>, iterations: u32) -> DistanceResult {
        let mut p = point;
        let mut scale = 1.0;
        let mut iterations_done = 0u32;

        for _ in 0..iterations.min(self.get_max_iterations()) {
            if p.x + p.y < 0.0 { p = Vector3::new(-p.y, -p.x, p.z); }
            if p.x + p.z < 0.0 { p = Vector3::new(-p.z, p.y, -p.x); }
            if p.y + p.z < 0.0 { p = Vector3::new(p.x, -p.z, -p.y); }

            p = p * 2.0 - Vector3::new(1.0, 1.0, 1.0);
            scale *= 2.0;
            iterations_done += 1;
        }

        DistanceResult {
            distance: p.norm() / scale,
            iterations: iterations_done,
            escaped: false,
            final_z: p,
        }
    }

    /// Compute Apollonian distance
    fn compute_apollonian_distance(&self, point: Vector3<f32>, iterations: u32) -> DistanceResult {
        let mut p = point;
        let mut iterations_done = 0u32;

        for _ in 0..iterations.min(self.get_max_iterations()) {
            let r = p.norm();
            if r > 1.0 {
                p = p / (r * r);
            }
            iterations_done += 1;
        }

        DistanceResult {
            distance: p.norm() - 1.0,
            iterations: iterations_done,
            escaped: false,
            final_z: p,
        }
    }

    /// Compute Quaternion Julia distance
    fn compute_quaternion_julia_distance(&self, point: Vector3<f32>, c: [f32; 4], max_iterations: u32) -> DistanceResult {
        let mut z = Vector4::new(point.x, point.y, point.z, 0.0);
        let c_vec = Vector4::new(c[0], c[1], c[2], c[3]);
        let mut iterations = 0u32;

        while iterations < max_iterations && z.norm_squared() < self.parameters.bailout {
            // Quaternion multiplication: z = z² + c
            let z_squared = Vector4::new(
                z.x * z.x - z.y * z.y - z.z * z.z - z.w * z.w,
                2.0 * z.x * z.y,
                2.0 * z.x * z.z,
                2.0 * z.x * z.w
            );
            z = z_squared + c_vec;
            iterations += 1;
        }

        DistanceResult {
            distance: z.norm(),
            iterations,
            escaped: z.norm_squared() >= self.parameters.bailout,
            final_z: Vector3::new(z.x, z.y, z.z), // Convert back to 3D
        }
    }

    /// Compute Kaleidoscopic IFS distance
    fn compute_kaleidoscopic_ifs_distance(&self, point: Vector3<f32>, scale: f32, rotation: [f32; 3], offset: [f32; 3]) -> DistanceResult {
        let mut p = point;
        let mut iterations = 0u32;

        for _ in 0..self.get_max_iterations() {
            // Folding
            p = p.abs();

            // Rotations
            p = self.rotate_point(p, rotation[0], Vector3::new(0.0, 0.0, 1.0));
            p = self.rotate_point(p, rotation[1], Vector3::new(0.0, 1.0, 0.0));
            p = self.rotate_point(p, rotation[2], Vector3::new(1.0, 0.0, 0.0));

            // Scaling and offset
            p = p * scale + Vector3::new(offset[0], offset[1], offset[2]);

            if p.norm_squared() > self.parameters.bailout {
                break;
            }
            iterations += 1;
        }

        DistanceResult {
            distance: p.norm(),
            iterations,
            escaped: p.norm_squared() >= self.parameters.bailout,
            final_z: p,
        }
    }

    /// Rotate point around axis
    fn rotate_point(&self, point: Vector3<f32>, angle: f32, axis: Vector3<f32>) -> Vector3<f32> {
        let cos_a = angle.cos();
        let sin_a = angle.sin();
        let axis = axis.normalize();

        point * cos_a +
        axis.cross(&point) * sin_a +
        axis * axis.dot(&point) * (1.0 - cos_a)
    }
}

impl Default for FractalEngine {
    fn default() -> Self {
        Self::new()
    }
}