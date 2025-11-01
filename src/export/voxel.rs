//! Voxel Export Module
//!
//! This module provides capabilities to export fractal data as voxel models
//! for use in voxel-based 3D software and 3D printing.

use crate::fractal::types::{FractalParameters, FractalFormula};
use crate::scene::objects::SceneObject;
use nalgebra::Vector3;
use std::collections::HashMap;

/// Voxel export settings
#[derive(Debug, Clone)]
pub struct VoxelExportSettings {
    pub resolution: [u32; 3], // Voxel grid resolution
    pub bounds: [Vector3<f32>; 2], // Min and max bounds
    pub voxel_size: f32, // Size of each voxel
    pub threshold: f32, // Surface threshold
    pub compression: VoxelCompression,
    pub format: VoxelFormat,
    pub color_data: bool, // Include color information
    pub density_data: bool, // Include density information
}

/// Supported voxel formats
#[derive(Debug, Clone)]
pub enum VoxelFormat {
    VOX,
    VXL,
    VOX2,
    BINVOX,
    ASCII_VOX,
    STL_VOXEL,
    PLY_VOXEL,
}

/// Voxel compression types
#[derive(Debug, Clone)]
pub enum VoxelCompression {
    None,
    RunLength,
    LZ4,
    GZIP,
}

/// Voxel export result
#[derive(Debug, Clone)]
pub struct VoxelExportResult {
    pub voxels: Vec<Voxel>,
    pub resolution: [u32; 3],
    pub bounds: [Vector3<f32>; 2],
    pub voxel_size: f32,
    pub metadata: VoxelMetadata,
}

#[derive(Debug, Clone)]
pub struct Voxel {
    pub position: [u32; 3],
    pub value: f32, // Distance value or density
    pub color: Option<Vector3<f32>>,
    pub material: Option<u32>,
}

#[derive(Debug, Clone)]
pub struct VoxelMetadata {
    pub total_voxels: usize,
    pub filled_voxels: usize,
    pub surface_voxels: usize,
    pub volume: f32,
    pub surface_area: f32,
    pub bounding_box: [Vector3<f32>; 2],
}

/// Voxel generator for creating 3D voxel models from fractal data
pub struct VoxelGenerator {
    settings: VoxelExportSettings,
}

impl VoxelGenerator {
    pub fn new(settings: VoxelExportSettings) -> Self {
        Self { settings }
    }

    /// Generate voxel model from fractal object
    pub fn generate_voxels(&self, fractal: &SceneObject) -> Result<VoxelExportResult, Box<dyn std::error::Error>> {
        match &fractal.object_type {
            crate::scene::objects::ObjectType::FractalObject { parameters, .. } => {
                self.generate_fractal_voxels(parameters)
            }
            _ => Err("Not a fractal object".into()),
        }
    }

    /// Generate voxel model from fractal parameters
    pub fn generate_fractal_voxels(&self, fractal_params: &FractalParameters) -> Result<VoxelExportResult, Box<dyn std::error::Error>> {
        let [min_bounds, max_bounds] = self.settings.bounds;
        let [nx, ny, nz] = self.settings.resolution;
        let voxel_size = self.settings.voxel_size;

        let mut voxels = Vec::new();

        // Calculate actual grid dimensions based on bounds and voxel size
        let grid_nx = ((max_bounds.x - min_bounds.x) / voxel_size).ceil() as u32;
        let grid_ny = ((max_bounds.y - min_bounds.y) / voxel_size).ceil() as u32;
        let grid_nz = ((max_bounds.z - min_bounds.z) / voxel_size).ceil() as u32;

        let mut filled_count = 0;
        let mut surface_count = 0;

        // Generate voxels
        for z in 0..grid_nz {
            for y in 0..grid_ny {
                for x in 0..grid_nx {
                    // Calculate world position
                    let world_pos = Vector3::new(
                        min_bounds.x + x as f32 * voxel_size,
                        min_bounds.y + y as f32 * voxel_size,
                        min_bounds.z + z as f32 * voxel_size,
                    );

                    // Calculate fractal distance
                    let distance = self.calculate_fractal_distance(&fractal_params.formula, world_pos);
                    
                    // Determine if voxel is filled based on threshold
                    let is_filled = distance < self.settings.threshold;
                    let value = -distance; // Negative inside, positive outside

                    if is_filled {
                        filled_count += 1;
                    }

                    // Check if this is a surface voxel
                    let is_surface = self.is_surface_voxel(x, y, z, [grid_nx, grid_ny, grid_nz], &|x, y, z| {
                        let pos = Vector3::new(
                            min_bounds.x + x as f32 * voxel_size,
                            min_bounds.y + y as f32 * voxel_size,
                            min_bounds.z + z as f32 * voxel_size,
                        );
                        self.calculate_fractal_distance(&fractal_params.formula, pos) < self.settings.threshold
                    });

                    if is_surface && is_filled {
                        surface_count += 1;
                    }

                    // Create voxel
                    let mut voxel = Voxel {
                        position: [x, y, z],
                        value,
                        color: None,
                        material: None,
                    };

                    // Add color if enabled
                    if self.settings.color_data {
                        voxel.color = Some(self.calculate_voxel_color(&fractal_params.formula, distance));
                    }

                    // Add material if enabled
                    if self.settings.density_data {
                        voxel.material = Some(self.calculate_voxel_material(distance, &fractal_params));
                    }

                    // Only add filled voxels or all voxels based on settings
                    voxels.push(voxel);
                }
            }
        }

        let metadata = VoxelMetadata {
            total_voxels: voxels.len(),
            filled_voxels: filled_count,
            surface_voxels: surface_count,
            volume: filled_count as f32 * voxel_size.powi(3),
            surface_area: surface_count as f32 * voxel_size.powi(2),
            bounding_box: [min_bounds, max_bounds],
        };

        Ok(VoxelExportResult {
            voxels,
            resolution: [grid_nx, grid_ny, grid_nz],
            bounds: [min_bounds, max_bounds],
            voxel_size,
            metadata,
        })
    }

    /// Calculate distance for fractal formula
    fn calculate_fractal_distance(&self, formula: &FractalFormula, point: Vector3<f32>) -> f32 {
        match formula {
            FractalFormula::Mandelbulb { power, max_iterations } => {
                self.mandelbulb_distance(point, *power, *max_iterations)
            }
            FractalFormula::Mandelbox { scale, folding_limit, max_iterations } => {
                self.mandelbox_distance(point, *scale, *folding_limit, *max_iterations)
            }
            FractalFormula::IFS { transforms, probabilities, max_iterations } => {
                self.ifs_distance(point, transforms, probabilities, *max_iterations)
            }
            FractalFormula::QuaternionJulia { c, max_iterations } => {
                let c_vec = Vector3::new(c[0], c[1], c[2]);
                self.quaternion_julia_distance(point, c_vec, *max_iterations)
            }
            FractalFormula::Mandelbrot { center, zoom, max_iterations } => {
                // For 2D Mandelbrot, create a simple 3D voxel
                let point_2d = Vector3::new(point.x, point.y, 0.0);
                let center_2d = Vector3::new(center[0], center[1], 0.0);
                let distance_2d = self.mandelbrot_distance_2d(point_2d, center_2d, *zoom, *max_iterations);
                distance_2d.abs() + point.z.abs() * 0.1 // Small extrusion
            }
            FractalFormula::Custom { parameters, .. } => {
                // Custom formula - simple sphere as fallback
                let radius = parameters.get("radius").copied().unwrap_or(1.0);
                point.magnitude() - radius
            }
        }
    }

    /// Check if a voxel is on the surface
    fn is_surface_voxel<F>(&self, x: u32, y: u32, z: u32, resolution: [u32; 3], distance_fn: F) -> bool
    where
        F: Fn(u32, u32, u32) -> bool,
    {
        // Check if any neighbor is empty
        let neighbors = [
            (x.wrapping_sub(1), y, z),
            (x + 1, y, z),
            (x, y.wrapping_sub(1), z),
            (x, y + 1, z),
            (x, y, z.wrapping_sub(1)),
            (x, y, z + 1),
        ];

        let [nx, ny, nz] = resolution;
        let current_filled = distance_fn(x, y, z);

        if !current_filled {
            return false;
        }

        // Check if any neighbor is not filled
        for (nx, ny, nz) in neighbors {
            if nx < resolution[0] && ny < resolution[1] && nz < resolution[2] {
                if !distance_fn(nx, ny, nz) {
                    return true; // Surface voxel has at least one empty neighbor
                }
            }
        }

        false
    }

    /// Calculate voxel color
    fn calculate_voxel_color(&self, formula: &FractalFormula, distance: f32) -> Vector3<f32> {
        match formula {
            FractalFormula::Mandelbulb { .. } => {
                // Color based on distance from surface
                let intensity = (-distance * 0.1).exp();
                Vector3::new(0.8 * intensity, 0.4 * intensity, 0.2 * intensity)
            }
            FractalFormula::Mandelbox { .. } => {
                let intensity = (distance * 0.2).sin().abs();
                Vector3::new(0.2 * intensity, 0.8 * intensity, 0.6 * intensity)
            }
            _ => Vector3::new(0.7, 0.7, 0.7), // Default gray
        }
    }

    /// Calculate voxel material ID
    fn calculate_voxel_material(&self, distance: f32, fractal_params: &FractalParameters) -> u32 {
        let material_props = &fractal_params.material_properties;
        
        // Simple material assignment based on properties
        if material_props.metallic > 0.5 {
            1 // Metallic
        } else if material_props.roughness < 0.3 {
            2 // Smooth
        } else {
            0 // Default
        }
    }

    // Simplified fractal distance functions
    fn mandelbulb_distance(&self, point: Vector3<f32>, power: f32, _max_iterations: u32) -> f32 {
        let r = point.magnitude();
        if r == 0.0 {
            return 0.0;
        }
        
        let theta = (point.z / r).acos();
        let phi = point.y.atan2(point.x);
        
        let new_r = r.powf(power);
        let new_theta = theta * power;
        let new_phi = phi * power;
        
        let z_new = Vector3::new(
            new_r * new_theta.sin() * new_phi.cos(),
            new_r * new_theta.sin() * new_phi.sin(),
            new_r * new_theta.cos(),
        );
        
        (z_new - point).magnitude()
    }

    fn mandelbox_distance(&self, mut point: Vector3<f32>, scale: f32, folding_limit: f32, _max_iterations: u32) -> f32 {
        for _ in 0..3 {
            point = Vector3::new(
                point.x.clamp(-folding_limit, folding_limit) * 2.0 - point.x,
                point.y.clamp(-folding_limit, folding_limit) * 2.0 - point.y,
                point.z.clamp(-folding_limit, folding_limit) * 2.0 - point.z,
            );

            let r2 = point.magnitude_squared();
            if r2 < 0.25 {
                point *= 4.0;
            } else if r2 < 1.0 {
                point /= r2;
            }

            point *= scale;
        }
        
        point.magnitude() - 1.0
    }

    fn ifs_distance(&self, point: Vector3<f32>, _transforms: &[nalgebra::Matrix4<f32>], _probabilities: &[f32], _max_iterations: u32) -> f32 {
        point.magnitude() - 1.0
    }

    fn quaternion_julia_distance(&self, point: Vector3<f32>, _c: Vector3<f32>, _max_iterations: u32) -> f32 {
        point.magnitude() - 1.0
    }

    fn mandelbrot_distance_2d(&self, point: Vector3<f32>, center: Vector3<f32>, zoom: f32, _max_iterations: u32) -> f32 {
        let p = (point - center) / zoom;
        p.magnitude() - 1.0
    }

    /// Export voxels to file
    pub fn export_to_file(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        match self.settings.format {
            VoxelFormat::VOX => self.export_vox(result, filename),
            VoxelFormat::VXL => self.export_vxl(result, filename),
            VoxelFormat::BINVOX => self.export_binvox(result, filename),
            VoxelFormat::ASCII_VOX => self.export_ascii_vox(result, filename),
            VoxelFormat::STL_VOXEL => self.export_stl_voxel(result, filename),
            VoxelFormat::PLY_VOXEL => self.export_ply_voxel(result, filename),
            _ => Err("Unsupported voxel format".into()),
        }
    }

    /// Export as MagicaVoxel VOX format
    fn export_vox(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        // VOX file format implementation
        // This is a simplified version - actual VOX format is more complex
        
        // Write header
        writeln!(file, "VOX {")?;
        writeln!(file, "size {} {} {}", result.resolution[0], result.resolution[1], result.resolution[2])?;
        writeln!(file, "data")?;
        
        // Write voxel data
        for voxel in &result.voxels {
            if voxel.value < 0.0 { // Filled voxel
                writeln!(file, "{} {} {} 255 255 255 255", voxel.position[0], voxel.position[1], voxel.position[2])?;
            }
        }
        
        writeln!(file, "}")?;
        
        Ok(())
    }

    /// Export as binary voxel format
    fn export_binvox(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        // Write header
        writeln!(file, "# binvox voxel file")?;
        writeln!(file, "dim {} {} {}", result.resolution[0], result.resolution[1], result.resolution[2])?;
        writeln!(file, "translate {} {} {}", result.bounds[0].x, result.bounds[0].y, result.bounds[0].z)?;
        writeln!(file, "scale {}", result.voxel_size)?;
        writeln!(file, "data")?;
        
        // Write voxel data as binary
        for z in 0..result.resolution[2] {
            for y in 0..result.resolution[1] {
                for x in 0..result.resolution[0] {
                    let index = z * result.resolution[1] * result.resolution[0] + y * result.resolution[0] + x;
                    let voxel = &result.voxels[index];
                    let filled = if voxel.value < 0.0 { 1u8 } else { 0u8 };
                    file.write_all(&[filled])?;
                }
            }
        }
        
        Ok(())
    }

    /// Export as ASCII voxel format
    fn export_ascii_vox(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        writeln!(file, "# ASCII Voxel File")?;
        writeln!(file, "size {} {} {}", result.resolution[0], result.resolution[1], result.resolution[2])?;
        
        // Write filled voxels
        for voxel in &result.voxels {
            if voxel.value < 0.0 {
                writeln!(file, "{} {} {}", voxel.position[0], voxel.position[1], voxel.position[2])?;
            }
        }
        
        Ok(())
    }

    /// Export as STL voxel format (block-based)
    fn export_stl_voxel(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        // Generate STL from voxel data
        // This would convert voxels to small cube meshes
        
        writeln!(file, "solid voxel_model")?;
        
        // For each filled voxel, add a small cube
        for voxel in &result.voxels {
            if voxel.value < 0.0 {
                let x = voxel.position[0] as f32 * result.voxel_size + result.bounds[0].x;
                let y = voxel.position[1] as f32 * result.voxel_size + result.bounds[0].y;
                let z = voxel.position[2] as f32 * result.voxel_size + result.bounds[0].z;
                let size = result.voxel_size;
                
                // Add cube faces (simplified - would generate proper triangles)
                writeln!(file, "  facet normal 0 0 0")?;
                writeln!(file, "    outer loop")?;
                writeln!(file, "      vertex {} {} {}", x, y, z)?;
                writeln!(file, "      vertex {} {} {}", x + size, y, z)?;
                writeln!(file, "      vertex {} {} {}", x + size, y + size, z)?;
                writeln!(file, "    endloop")?;
                writeln!(file, "  endfacet")?;
            }
        }
        
        writeln!(file, "endsolid voxel_model")?;
        
        Ok(())
    }

    /// Export as PLY voxel format
    fn export_ply_voxel(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        writeln!(file, "ply")?;
        writeln!(file, "format ascii 1.0")?;
        writeln!(file, "comment Generated by Modular Fractal Shader")?;
        writeln!(file, "element vertex {}", result.voxels.len())?;
        writeln!(file, "property float x")?;
        writeln!(file, "property float y")?;
        writeln!(file, "property float z")?;
        writeln!(file, "property float value")?;
        if self.settings.color_data {
            writeln!(file, "property uchar red")?;
            writeln!(file, "property uchar green")?;
            writeln!(file, "property uchar blue")?;
        }
        writeln!(file, "end_header")?;
        
        // Write vertex data
        for voxel in &result.voxels {
            let x = voxel.position[0] as f32 * result.voxel_size + result.bounds[0].x;
            let y = voxel.position[1] as f32 * result.voxel_size + result.bounds[0].y;
            let z = voxel.position[2] as f32 * result.voxel_size + result.bounds[0].z;
            
            if let Some(color) = voxel.color {
                writeln!(file, "{} {} {} {} {} {} {}", x, y, z, voxel.value, 
                        (color.x * 255.0) as u8, (color.y * 255.0) as u8, (color.z * 255.0) as u8)?;
            } else {
                writeln!(file, "{} {} {} {}", x, y, z, voxel.value)?;
            }
        }
        
        Ok(())
    }

    /// Export as VXL format (simplified)
    fn export_vxl(&self, result: &VoxelExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        // Simple VXL format implementation
        writeln!(file, "VXL1")?;
        writeln!(file, "{} {} {}", result.resolution[0], result.resolution[1], result.resolution[2])?;
        
        // Write voxel data
        for voxel in &result.voxels {
            writeln!(file, "{} {} {} {} {} {} {}", 
                    voxel.position[0], voxel.position[1], voxel.position[2],
                    (voxel.value * 255.0) as i32, 
                    voxel.color.map(|c| (c.x * 255.0) as u8).unwrap_or(0),
                    voxel.color.map(|c| (c.y * 255.0) as u8).unwrap_or(0),
                    voxel.color.map(|c| (c.z * 255.0) as u8).unwrap_or(0))?;
        }
        
        Ok(())
    }
}

impl Default for VoxelExportSettings {
    fn default() -> Self {
        Self {
            resolution: [64, 64, 64],
            bounds: [
                Vector3::new(-2.0, -2.0, -2.0),
                Vector3::new(2.0, 2.0, 2.0),
            ],
            voxel_size: 0.0625,
            threshold: 0.0,
            compression: VoxelCompression::None,
            format: VoxelFormat::VOX,
            color_data: true,
            density_data: false,
        }
    }
}