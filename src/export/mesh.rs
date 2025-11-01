//! Mesh Export Module
//!
//! This module provides capabilities to export fractal data as 3D meshes
//! in various formats like OBJ, STL, and FBX for use in other 3D software.

use crate::fractal::types::{FractalParameters, FractalFormula};
use crate::scene::objects::SceneObject;
use nalgebra::{Vector3, Matrix4};
use std::collections::HashMap;

/// Export settings for mesh generation
#[derive(Debug, Clone)]
pub struct MeshExportSettings {
    pub resolution: [u32; 3], // Grid resolution for mesh generation
    pub bounds: [Vector3<f32>; 2], // Min and max bounds
    pub surface_threshold: f32, // Surface distance threshold
    pub smoothing: bool, // Apply mesh smoothing
    pub merge_vertices: bool, // Merge duplicate vertices
    pub calculate_normals: bool, // Recalculate vertex normals
    pub export_materials: bool, // Export material information
    pub format: MeshFormat,
}

/// Supported mesh export formats
#[derive(Debug, Clone)]
pub enum MeshFormat {
    OBJ,
    STL,
    FBX,
    PLY,
    X3D,
}

/// Mesh export result
#[derive(Debug, Clone)]
pub struct MeshExportResult {
    pub vertices: Vec<Vector3<f32>>,
    pub normals: Vec<Vector3<f32>>,
    pub uvs: Vec<[f32; 2]>,
    pub indices: Vec<u32>,
    pub materials: Vec<MeshMaterial>,
    pub metadata: MeshMetadata,
}

#[derive(Debug, Clone)]
pub struct MeshMaterial {
    pub name: String,
    pub diffuse_color: Vector3<f32>,
    pub specular_color: Vector3<f32>,
    pub emission_color: Vector3<f32>,
    pub metallic: f32,
    pub roughness: f32,
    pub opacity: f32,
}

#[derive(Debug, Clone)]
pub struct MeshMetadata {
    pub vertex_count: usize,
    pub face_count: usize,
    pub surface_area: f32,
    pub volume: f32,
    pub bounding_box: [Vector3<f32>; 2],
}

/// Mesh generator for creating 3D meshes from fractal data
pub struct MeshGenerator {
    settings: MeshExportSettings,
}

impl MeshGenerator {
    pub fn new(settings: MeshExportSettings) -> Self {
        Self { settings }
    }

    /// Generate mesh from fractal object
    pub fn generate_mesh(&self, fractal: &SceneObject) -> Result<MeshExportResult, Box<dyn std::error::Error>> {
        match &fractal.object_type {
            crate::scene::objects::ObjectType::FractalObject { parameters, .. } => {
                self.generate_fractal_mesh(parameters)
            }
            _ => Err("Not a fractal object".into()),
        }
    }

    /// Generate mesh from fractal parameters
    pub fn generate_fractal_mesh(&self, fractal_params: &FractalParameters) -> Result<MeshExportResult, Box<dyn std::error::Error>> {
        let [min_bounds, max_bounds] = self.settings.bounds;
        let [nx, ny, nz] = self.settings.resolution;

        let mut vertices = Vec::new();
        let mut normals = Vec::new();
        let mut uvs = Vec::new();
        let mut indices = Vec::new();
        let mut materials = Vec::new();

        // Generate vertices using marching cubes or similar algorithm
        let dx = (max_bounds.x - min_bounds.x) / nx as f32;
        let dy = (max_bounds.y - min_bounds.y) / ny as f32;
        let dz = (max_bounds.z - min_bounds.z) / nz as f32;

        let mut grid_data = Vec::new();
        grid_data.resize((nx + 1) * (ny + 1) * (nz + 1), 0.0f32);

        // Calculate distance field for each grid point
        for z in 0..=nz {
            for y in 0..=ny {
                for x in 0..=nx {
                    let point = Vector3::new(
                        min_bounds.x + x as f32 * dx,
                        min_bounds.y + y as f32 * dy,
                        min_bounds.z + z as f32 * dz,
                    );

                    let distance = self.calculate_fractal_distance(&fractal_params.formula, point);
                    let index = z * (ny + 1) * (nx + 1) + y * (nx + 1) + x;
                    grid_data[index] = distance;
                }
            }
        }

        // Apply marching cubes algorithm to extract surface
        let result = self.marching_cubes(&grid_data, [nx, ny, nz], min_bounds, [dx, dy, dz])?;
        
        vertices = result.vertices;
        normals = result.normals;
        uvs = result.uvs;
        indices = result.indices;

        // Create default material
        materials.push(MeshMaterial {
            name: "FractalSurface".to_string(),
            diffuse_color: Vector3::new(0.8, 0.8, 0.8),
            specular_color: Vector3::new(0.2, 0.2, 0.2),
            emission_color: Vector3::zeros(),
            metallic: fractal_params.material_properties.metallic,
            roughness: fractal_params.material_properties.roughness,
            opacity: 1.0,
        });

        // Calculate metadata
        let metadata = self.calculate_mesh_metadata(&vertices, &indices);

        Ok(MeshExportResult {
            vertices,
            normals,
            uvs,
            indices,
            materials,
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
                // For 2D Mandelbrot, extrude along z-axis
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

    /// Marching cubes implementation for surface extraction
    fn marching_cubes(&self, grid_data: &[f32], resolution: [u32; 3], origin: Vector3<f32>, spacing: [f32; 3]) -> Result<MeshExtractResult, Box<dyn std::error::Error>> {
        let [nx, ny, nz] = resolution;
        let [dx, dy, dz] = spacing;

        let mut vertices = Vec::new();
        let mut normals = Vec::new();
        let mut uvs = Vec::new();
        let mut indices = Vec::new();

        // For each cube in the grid
        for z in 0..nz-1 {
            for y in 0..ny-1 {
                for x in 0..nx-1 {
                    // Get 8 corner values
                    let cube_values = [
                        self.get_grid_value(grid_data, [nx, ny, nz], [x, y, z]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x+1, y, z]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x+1, y, z+1]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x, y, z+1]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x, y+1, z]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x+1, y+1, z]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x+1, y+1, z+1]),
                        self.get_grid_value(grid_data, [nx, ny, nz], [x, y+1, z+1]),
                    ];

                    // Calculate edge intersection points
                    let edge_points = self.calculate_edge_intersections(&cube_values, origin, [dx, dy, dz], [x, y, z]);

                    // Connect edge points into triangles
                    if !edge_points.is_empty() {
                        let base_index = vertices.len() as u32;
                        for point in &edge_points {
                            vertices.push(point.position);
                            normals.push(point.normal);
                            uvs.push(point.uv);
                        }

                        // Create triangles (simplified triangulation)
                        for i in (0..edge_points.len()).step_by(3) {
                            if i + 2 < edge_points.len() {
                                indices.push(base_index + i as u32);
                                indices.push(base_index + (i + 1) as u32);
                                indices.push(base_index + (i + 2) as u32);
                            }
                        }
                    }
                }
            }
        }

        Ok(MeshExtractResult {
            vertices,
            normals,
            uvs,
            indices,
        })
    }

    /// Get grid value at coordinates
    fn get_grid_value(&self, grid_data: &[f32], resolution: [u32; 3], coords: [u32; 3]) -> f32 {
        let [nx, ny, nz] = resolution;
        let [x, y, z] = coords;
        let index = z * ny * nx + y * nx + x;
        grid_data.get(index as usize).copied().unwrap_or(0.0)
    }

    /// Calculate edge intersections for marching cubes
    fn calculate_edge_intersections(&self, cube_values: &[f32; 8], origin: Vector3<f32>, spacing: [f32; 3], grid_pos: [u32; 3]) -> Vec<EdgePoint> {
        let [dx, dy, dz] = spacing;
        let [x, y, z] = grid_pos;

        let mut edge_points = Vec::new();

        // Check each edge for intersection
        for edge in 0..12 {
            let (p1, p2, val1, val2) = self.get_edge_points(edge, origin, [dx, dy, dz], [x, y, z]);
            
            if (val1 > 0.0) != (val2 > 0.0) {
                // Linear interpolation to find exact intersection
                let t = val1.abs() / (val1.abs() + val2.abs());
                let intersection = p1.lerp(&p2, t);
                let normal = self.calculate_normal_at_point(intersection);
                
                edge_points.push(EdgePoint {
                    position: intersection,
                    normal,
                    uv: self.calculate_uv(intersection, origin, [dx, dy, dz]),
                });
            }
        }

        edge_points
    }

    /// Get edge endpoints and values
    fn get_edge_points(&self, edge: usize, origin: Vector3<f32>, spacing: [f32; 3], grid_pos: [u32; 3]) -> (Vector3<f32>, Vector3<f32>, f32, f32) {
        let [dx, dy, dz] = spacing;
        let [x, y, z] = grid_pos;

        match edge {
            0 => (Vector3::new(origin.x, origin.y, origin.z), Vector3::new(origin.x + dx, origin.y, origin.z), 0.0, 0.0),
            1 => (Vector3::new(origin.x + dx, origin.y, origin.z), Vector3::new(origin.x + dx, origin.y, origin.z + dz), 0.0, 0.0),
            2 => (Vector3::new(origin.x + dx, origin.y, origin.z + dz), Vector3::new(origin.x, origin.y, origin.z + dz), 0.0, 0.0),
            3 => (Vector3::new(origin.x, origin.y, origin.z + dz), Vector3::new(origin.x, origin.y, origin.z), 0.0, 0.0),
            4 => (Vector3::new(origin.x, origin.y + dy, origin.z), Vector3::new(origin.x + dx, origin.y + dy, origin.z), 0.0, 0.0),
            5 => (Vector3::new(origin.x + dx, origin.y + dy, origin.z), Vector3::new(origin.x + dx, origin.y + dy, origin.z + dz), 0.0, 0.0),
            6 => (Vector3::new(origin.x + dx, origin.y + dy, origin.z + dz), Vector3::new(origin.x, origin.y + dy, origin.z + dz), 0.0, 0.0),
            7 => (Vector3::new(origin.x, origin.y + dy, origin.z + dz), Vector3::new(origin.x, origin.y + dy, origin.z), 0.0, 0.0),
            8 => (Vector3::new(origin.x, origin.y, origin.z), Vector3::new(origin.x, origin.y + dy, origin.z), 0.0, 0.0),
            9 => (Vector3::new(origin.x + dx, origin.y, origin.z), Vector3::new(origin.x + dx, origin.y + dy, origin.z), 0.0, 0.0),
            10 => (Vector3::new(origin.x + dx, origin.y, origin.z + dz), Vector3::new(origin.x + dx, origin.y + dy, origin.z + dz), 0.0, 0.0),
            11 => (Vector3::new(origin.x, origin.y, origin.z + dz), Vector3::new(origin.x, origin.y + dy, origin.z + dz), 0.0, 0.0),
            _ => (Vector3::zeros(), Vector3::zeros(), 0.0, 0.0),
        }
    }

    /// Calculate normal at a point using gradient
    fn calculate_normal_at_point(&self, point: Vector3<f32>) -> Vector3<f32> {
        // Simple gradient approximation
        let epsilon = 0.001;
        let dx = self.calculate_surface_function(point + Vector3::new(epsilon, 0.0, 0.0)) - 
                 self.calculate_surface_function(point - Vector3::new(epsilon, 0.0, 0.0));
        let dy = self.calculate_surface_function(point + Vector3::new(0.0, epsilon, 0.0)) - 
                 self.calculate_surface_function(point - Vector3::new(0.0, epsilon, 0.0));
        let dz = self.calculate_surface_function(point + Vector3::new(0.0, 0.0, epsilon)) - 
                 self.calculate_surface_function(point - Vector3::new(0.0, 0.0, epsilon));

        Vector3::new(dx, dy, dz).normalize()
    }

    /// Calculate surface function value (distance field)
    fn calculate_surface_function(&self, point: Vector3<f32>) -> f32 {
        point.magnitude() // Simplified - would use actual fractal distance
    }

    /// Calculate UV coordinates
    fn calculate_uv(&self, point: Vector3<f32>, origin: Vector3<f32>, spacing: [f32; 3]) -> [f32; 2] {
        let [dx, dy, dz] = spacing;
        let u = (point.x - origin.x) / dx;
        let v = (point.y - origin.y) / dy;
        [u.fract(), v.fract()]
    }

    // Fractal distance functions (simplified versions)
    fn mandelbulb_distance(&self, point: Vector3<f32>, power: f32, _max_iterations: u32) -> f32 {
        // Simplified Mandelbulb distance estimation
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
        // Simplified Mandelbox distance
        for _ in 0..5 { // Reduced iterations for speed
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

    fn ifs_distance(&self, point: Vector3<f32>, _transforms: &[Matrix4<f32>], _probabilities: &[f32], _max_iterations: u32) -> f32 {
        // Simplified IFS distance
        point.magnitude() - 1.0
    }

    fn quaternion_julia_distance(&self, point: Vector3<f32>, _c: Vector3<f32>, _max_iterations: u32) -> f32 {
        // Simplified quaternion Julia distance
        point.magnitude() - 1.0
    }

    fn mandelbrot_distance_2d(&self, point: Vector3<f32>, center: Vector3<f32>, zoom: f32, _max_iterations: u32) -> f32 {
        // Simplified 2D Mandelbrot distance
        let p = (point - center) / zoom;
        p.magnitude() - 1.0
    }

    /// Calculate mesh metadata
    fn calculate_mesh_metadata(&self, vertices: &[Vector3<f32>], indices: &[u32]) -> MeshMetadata {
        let vertex_count = vertices.len();
        let face_count = indices.len() / 3;

        let mut min_bounds = Vector3::new(f32::INFINITY, f32::INFINITY, f32::INFINITY);
        let mut max_bounds = Vector3::new(f32::NEG_INFINITY, f32::NEG_INFINITY, f32::NEG_INFINITY);

        for &vertex in vertices {
            min_bounds.x = min_bounds.x.min(vertex.x);
            min_bounds.y = min_bounds.y.min(vertex.y);
            min_bounds.z = min_bounds.z.min(vertex.z);
            max_bounds.x = max_bounds.x.max(vertex.x);
            max_bounds.y = max_bounds.y.max(vertex.y);
            max_bounds.z = max_bounds.z.max(vertex.z);
        }

        let surface_area = self.calculate_surface_area(vertices, indices);
        let volume = self.calculate_volume(vertices, indices);

        MeshMetadata {
            vertex_count,
            face_count,
            surface_area,
            volume,
            bounding_box: [min_bounds, max_bounds],
        }
    }

    /// Calculate surface area
    fn calculate_surface_area(&self, vertices: &[Vector3<f32>], indices: &[u32]) -> f32 {
        let mut area = 0.0;
        for tri in indices.chunks(3) {
            if tri.len() == 3 {
                let v0 = vertices[tri[0] as usize];
                let v1 = vertices[tri[1] as usize];
                let v2 = vertices[tri[2] as usize];
                
                let edge1 = v1 - v0;
                let edge2 = v2 - v0;
                area += edge1.cross(&edge2).magnitude() * 0.5;
            }
        }
        area
    }

    /// Calculate volume
    fn calculate_volume(&self, vertices: &[Vector3<f32>], indices: &[u32]) -> f32 {
        let mut volume = 0.0;
        for tri in indices.chunks(3) {
            if tri.len() == 3 {
                let v0 = vertices[tri[0] as usize];
                let v1 = vertices[tri[1] as usize];
                let v2 = vertices[tri[2] as usize];
                
                volume += v0.dot(&(v1.cross(&v2))) / 6.0;
            }
        }
        volume.abs()
    }

    /// Export mesh to file
    pub fn export_to_file(&self, result: &MeshExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        match self.settings.format {
            MeshFormat::OBJ => self.export_obj(result, filename),
            MeshFormat::STL => self.export_stl(result, filename),
            MeshFormat::FBX => self.export_fbx(result, filename),
            MeshFormat::PLY => self.export_ply(result, filename),
            MeshFormat::X3D => self.export_x3d(result, filename),
        }
    }

    /// Export as OBJ format
    fn export_obj(&self, result: &MeshExportResult, filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        let mut file = std::fs::File::create(filename)?;
        
        // Write header
        writeln!(file, "# Generated by Modular Fractal Shader")?;
        writeln!(file, "# Vertices: {}", result.vertices.len())?;
        writeln!(file, "# Faces: {}", result.indices.len() / 3)?;
        
        // Write vertices
        for vertex in &result.vertices {
            writeln!(file, "v {} {} {}", vertex.x, vertex.y, vertex.z)?;
        }
        
        // Write normals if available
        if !result.normals.is_empty() {
            for normal in &result.normals {
                writeln!(file, "vn {} {} {}", normal.x, normal.y, normal.z)?;
            }
        }
        
        // Write UVs if available
        if !result.uvs.is_empty() {
            for uv in &result.uvs {
                writeln!(file, "vt {} {}", uv[0], uv[1])?;
            }
        }
        
        // Write faces
        for tri in result.indices.chunks(3) {
            if tri.len() == 3 {
                let v0 = tri[0] + 1; // OBJ uses 1-based indexing
                let v1 = tri[1] + 1;
                let v2 = tri[2] + 1;
                writeln!(file, "f {}/{} {} {} {}", v0, v0, v1, v1, v2, v2)?;
            }
        }
        
        Ok(())
    }

    /// Export as STL format
    fn export_stl(&self, _result: &MeshExportResult, _filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        // STL export implementation would go here
        Err("STL export not yet implemented".into())
    }

    /// Export as FBX format
    fn export_fbx(&self, _result: &MeshExportResult, _filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        // FBX export implementation would go here
        Err("FBX export not yet implemented".into())
    }

    /// Export as PLY format
    fn export_ply(&self, _result: &MeshExportResult, _filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        // PLY export implementation would go here
        Err("PLY export not yet implemented".into())
    }

    /// Export as X3D format
    fn export_x3d(&self, _result: &MeshExportResult, _filename: &str) -> Result<(), Box<dyn std::error::Error>> {
        // X3D export implementation would go here
        Err("X3D export not yet implemented".into())
    }
}

#[derive(Debug, Clone)]
struct EdgePoint {
    position: Vector3<f32>,
    normal: Vector3<f32>,
    uv: [f32; 2],
}

#[derive(Debug, Clone)]
struct MeshExtractResult {
    vertices: Vec<Vector3<f32>>,
    normals: Vec<Vector3<f32>>,
    uvs: Vec<[f32; 2]>,
    indices: Vec<u32>,
}

impl Default for MeshExportSettings {
    fn default() -> Self {
        Self {
            resolution: [64, 64, 64],
            bounds: [
                Vector3::new(-2.0, -2.0, -2.0),
                Vector3::new(2.0, 2.0, 2.0),
            ],
            surface_threshold: 0.0,
            smoothing: true,
            merge_vertices: true,
            calculate_normals: true,
            export_materials: true,
            format: MeshFormat::OBJ,
        }
    }
}