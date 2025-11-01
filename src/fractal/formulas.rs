//! Fractal Formula Library
//!
//! This module provides a comprehensive collection of fractal formulas
//! and distance estimation functions for various mathematical sets.

use super::types::*;
use nalgebra::{Matrix4, Vector3, Vector4};

/// Collection of predefined fractal formulas
pub struct FractalFormulaLibrary {
    formulas: Vec<FractalFormula>,
}

impl FractalFormulaLibrary {
    /// Create a new formula library with all available formulas
    pub fn new() -> Self {
        let mut formulas = Vec::new();

        // Add classic formulas
        formulas.push(Self::mandelbrot_classic());
        formulas.push(Self::mandelbulb_classic());
        formulas.push(Self::mandelbox_classic());
        formulas.push(Self::ifs_dragon_curve());
        formulas.push(Self::ifs_barnsley_fern());
        formulas.push(Self::quaternion_julia_classic());

        // Add advanced formulas
        formulas.push(Self::mandelbulb_power_tower());
        formulas.push(Self::mandelbox_spherical());
        formulas.push(Self::ifs_sierpinski_triangle());
        formulas.push(Self::quaternion_julia_cubic());

        Self { formulas }
    }

    /// Get all available formulas
    pub fn formulas(&self) -> &[FractalFormula] {
        &self.formulas
    }

    /// Get formula by name
    pub fn get_formula(&self, name: &str) -> Option<&FractalFormula> {
        self.formulas.iter().find(|f| self.get_formula_name(f) == name)
    }

    /// Get formula name
    pub fn get_formula_name(&self, formula: &FractalFormula) -> String {
        match formula {
            FractalFormula::Mandelbrot { .. } => "Mandelbrot Classic".to_string(),
            FractalFormula::Mandelbulb { power, .. } => {
                if *power == 8.0 {
                    "Mandelbulb Classic".to_string()
                } else {
                    format!("Mandelbulb Power {}", power)
                }
            }
            FractalFormula::Mandelbox { .. } => "Mandelbox Classic".to_string(),
            FractalFormula::IFS { .. } => "IFS Dragon Curve".to_string(),
            FractalFormula::QuaternionJulia { .. } => "Quaternion Julia".to_string(),
            FractalFormula::Custom { name, .. } => name.clone(),
        }
    }

    /// Classic Mandelbrot set
    fn mandelbrot_classic() -> FractalFormula {
        FractalFormula::Mandelbrot {
            center: [-0.5, 0.0],
            zoom: 1.0,
            max_iterations: 100,
        }
    }

    /// Classic Mandelbulb (power 8)
    fn mandelbulb_classic() -> FractalFormula {
        FractalFormula::Mandelbulb {
            power: 8.0,
            max_iterations: 100,
        }
    }

    /// Mandelbulb with variable power
    fn mandelbulb_power_tower() -> FractalFormula {
        FractalFormula::Mandelbulb {
            power: 2.0,
            max_iterations: 150,
        }
    }

    /// Classic Mandelbox
    fn mandelbox_classic() -> FractalFormula {
        FractalFormula::Mandelbox {
            scale: 2.0,
            folding_limit: 1.0,
            max_iterations: 100,
        }
    }

    /// Mandelbox with spherical folding
    fn mandelbox_spherical() -> FractalFormula {
        FractalFormula::Mandelbox {
            scale: 3.0,
            folding_limit: 0.5,
            max_iterations: 120,
        }
    }

    /// Dragon curve IFS
    fn ifs_dragon_curve() -> FractalFormula {
        let mut transforms = Vec::new();
        let mut probabilities = Vec::new();

        // Dragon curve transformations
        let angle1 = std::f32::consts::PI / 4.0;
        let angle2 = 3.0 * std::f32::consts::PI / 4.0;

        transforms.push(Matrix4::new(
            angle1.cos() * 0.5, -angle1.sin() * 0.5, 0.0, 0.0,
            angle1.sin() * 0.5, angle1.cos() * 0.5, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.5, 0.0, 1.0,
        ));

        transforms.push(Matrix4::new(
            angle2.cos() * 0.5, -angle2.sin() * 0.5, 0.0, 0.5,
            angle2.sin() * 0.5, angle2.cos() * 0.5, 0.0, 0.5,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));

        probabilities.push(0.5);
        probabilities.push(0.5);

        FractalFormula::IFS {
            transforms,
            probabilities,
            max_iterations: 50,
        }
    }

    /// Barnsley fern IFS
    fn ifs_barnsley_fern() -> FractalFormula {
        let mut transforms = Vec::new();
        let mut probabilities = Vec::new();

        // Barnsley fern transformations
        transforms.push(Matrix4::new(
            0.0, 0.0, 0.0, 0.0,
            0.0, 0.16, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));
        probabilities.push(0.01);

        transforms.push(Matrix4::new(
            0.85, 0.04, 0.0, 0.0,
            -0.04, 0.85, 0.0, 1.6,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));
        probabilities.push(0.85);

        transforms.push(Matrix4::new(
            0.2, -0.26, 0.0, 0.0,
            0.23, 0.22, 0.0, 1.6,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));
        probabilities.push(0.07);

        transforms.push(Matrix4::new(
            -0.15, 0.28, 0.0, 0.0,
            0.26, 0.24, 0.0, 0.44,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));
        probabilities.push(0.07);

        FractalFormula::IFS {
            transforms,
            probabilities,
            max_iterations: 50,
        }
    }

    /// Sierpinski triangle IFS
    fn ifs_sierpinski_triangle() -> FractalFormula {
        let mut transforms = Vec::new();
        let mut probabilities = Vec::new();

        // Sierpinski triangle transformations
        transforms.push(Matrix4::new(
            0.5, 0.0, 0.0, 0.0,
            0.0, 0.5, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.5, 0.0, 1.0,
        ));
        probabilities.push(1.0 / 3.0);

        transforms.push(Matrix4::new(
            0.5, 0.0, 0.0, 0.5,
            0.0, 0.5, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.5, 0.0, 1.0,
        ));
        probabilities.push(1.0 / 3.0);

        transforms.push(Matrix4::new(
            0.5, 0.0, 0.0, 0.25,
            0.0, 0.5, 0.0, 0.5,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0,
        ));
        probabilities.push(1.0 / 3.0);

        FractalFormula::IFS {
            transforms,
            probabilities,
            max_iterations: 30,
        }
    }

    /// Classic quaternion Julia set
    fn quaternion_julia_classic() -> FractalFormula {
        FractalFormula::QuaternionJulia {
            c: [0.3, 0.5, 0.4, 0.2],
            max_iterations: 100,
        }
    }

    /// Cubic quaternion Julia set
    fn quaternion_julia_cubic() -> FractalFormula {
        FractalFormula::QuaternionJulia {
            c: [0.1, 0.2, -0.3, 0.4],
            max_iterations: 120,
        }
    }
}

impl Default for FractalFormulaLibrary {
    fn default() -> Self {
        Self::new()
    }
}

/// Distance field operations for combining fractals
pub struct DistanceFieldOps;

impl DistanceFieldOps {
    /// Union of two distance fields
    pub fn union(a: f32, b: f32) -> f32 {
        a.min(b)
    }

    /// Intersection of two distance fields
    pub fn intersection(a: f32, b: f32) -> f32 {
        a.max(b)
    }

    /// Subtraction of distance fields (a - b)
    pub fn subtraction(a: f32, b: f32) -> f32 {
        a.max(-b)
    }

    /// Smooth union with blending
    pub fn smooth_union(a: f32, b: f32, k: f32) -> f32 {
        let h = (k - (a - b).abs()).max(0.0) / k;
        a.min(b) - h * h * k * 0.25
    }

    /// Smooth intersection with blending
    pub fn smooth_intersection(a: f32, b: f32, k: f32) -> f32 {
        let h = (k - (a - b).abs()).max(0.0) / k;
        a.max(b) + h * h * k * 0.25
    }

    /// Smooth subtraction with blending
    pub fn smooth_subtraction(a: f32, b: f32, k: f32) -> f32 {
        let h = (k - (a + b).abs()).max(0.0) / k;
        a.max(-b) + h * h * k * 0.25
    }

    /// Onion operation (shell/hollow effect)
    pub fn onion(distance: f32, thickness: f32) -> f32 {
        distance.abs() - thickness
    }

    /// Round operation (fillet effect)
    pub fn round(distance: f32, radius: f32) -> f32 {
        distance - radius
    }

    /// Displacement using noise
    pub fn displace(distance: f32, displacement: f32) -> f32 {
        distance + displacement
    }

    /// Twist operation
    pub fn twist(distance: f32, point: Vector3<f32>, amount: f32) -> f32 {
        let c = (point.x * amount).cos();
        let s = (point.x * amount).sin();
        let rotated_point = Vector3::new(
            c * point.x - s * point.z,
            point.y,
            s * point.x + c * point.z,
        );
        // This is a simplified version - actual implementation would need
        // the distance function to be evaluated at the rotated point
        distance
    }
}

/// Primitive distance functions
pub struct DistancePrimitives;

impl DistancePrimitives {
    /// Sphere distance
    pub fn sphere(point: Vector3<f32>, center: Vector3<f32>, radius: f32) -> f32 {
        (point - center).magnitude() - radius
    }

    /// Box distance
    pub fn box_distance(point: Vector3<f32>, center: Vector3<f32>, size: Vector3<f32>) -> f32 {
        let p = point - center;
        let d = Vector3::new(
            p.x.abs() - size.x,
            p.y.abs() - size.y,
            p.z.abs() - size.z,
        );
        let outside = Vector3::new(d.x.max(0.0), d.y.max(0.0), d.z.max(0.0)).magnitude();
        d.x.max(d.y.max(d.z)).min(0.0) + outside
    }

    /// Torus distance
    pub fn torus(point: Vector3<f32>, center: Vector3<f32>, major_radius: f32, minor_radius: f32) -> f32 {
        let p = point - center;
        let q = Vector3::new(
            Vector3::new(p.x, p.z, 0.0).magnitude() - major_radius,
            p.y,
            0.0,
        );
        q.magnitude() - minor_radius
    }

    /// Cylinder distance
    pub fn cylinder(point: Vector3<f32>, center: Vector3<f32>, height: f32, radius: f32) -> f32 {
        let p = point - center;
        let d = Vector3::new(
            Vector3::new(p.x, p.z, 0.0).magnitude() - radius,
            p.y.abs() - height * 0.5,
            0.0,
        );
        d.x.max(d.y).min(0.0) + Vector3::new(d.x.max(0.0), d.y.max(0.0), 0.0).magnitude()
    }

    /// Plane distance
    pub fn plane(point: Vector3<f32>, normal: Vector3<f32>, distance: f32) -> f32 {
        point.dot(&normal) + distance
    }
}