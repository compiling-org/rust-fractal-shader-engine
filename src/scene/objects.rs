//! Scene Objects
//!
//! This module defines the objects that can exist in a 3D scene,
//! including fractal objects, meshes, and other scene elements.

use super::Camera;
use nalgebra::{Matrix4, Vector3, Vector4};

/// Base transform for all scene objects
#[derive(Debug, Clone)]
pub struct Transform {
    pub position: Vector3<f32>,
    pub rotation: Vector3<f32>,  // Euler angles
    pub scale: Vector3<f32>,
}

impl Transform {
    pub fn new() -> Self {
        Self {
            position: Vector3::zeros(),
            rotation: Vector3::zeros(),
            scale: Vector3::new(1.0, 1.0, 1.0),
        }
    }

    /// Get the transformation matrix
    pub fn matrix(&self) -> Matrix4<f32> {
        let translation = Matrix4::new_translation(&self.position);
        let rotation = self.rotation_matrix();
        let scale = Matrix4::new_scaling(self.scale.x, self.scale.y, self.scale.z);
        
        translation * rotation * scale
    }

    /// Get the inverse transformation matrix
    pub fn inverse_matrix(&self) -> Matrix4<f32> {
        let translation = Matrix4::new_translation(&(-self.position));
        let rotation = self.rotation_matrix().try_inverse().unwrap_or(Matrix4::identity());
        let scale = Matrix4::new_scaling(
            if self.scale.x != 0.0 { 1.0 / self.scale.x } else { 1.0 },
            if self.scale.y != 0.0 { 1.0 / self.scale.y } else { 1.0 },
            if self.scale.z != 0.0 { 1.0 / self.scale.z } else { 1.0 },
        );
        
        translation * rotation * scale
    }

    /// Get rotation matrix from Euler angles
    fn rotation_matrix(&self) -> Matrix4<f32> {
        let (sin_x, cos_x) = self.rotation.x.sin_cos();
        let (sin_y, cos_y) = self.rotation.y.sin_cos();
        let (sin_z, cos_z) = self.rotation.z.sin_cos();

        // ZYX rotation order
        Matrix4::new(
            cos_y * cos_z,
            cos_y * sin_z,
            -sin_y,
            0.0,

            sin_x * sin_y * cos_z - cos_x * sin_z,
            sin_x * sin_y * sin_z + cos_x * cos_z,
            sin_x * cos_y,
            0.0,

            cos_x * sin_y * cos_z + sin_x * sin_z,
            cos_x * sin_y * sin_z - sin_x * cos_z,
            cos_x * cos_y,
            0.0,

            0.0,
            0.0,
            0.0,
            1.0,
        )
    }

    /// Transform a point
    pub fn transform_point(&self, point: Vector3<f32>) -> Vector3<f32> {
        let homogeneous = Vector4::new(point.x, point.y, point.z, 1.0);
        let transformed = self.matrix() * homogeneous;
        Vector3::new(transformed.x, transformed.y, transformed.z)
    }

    /// Transform a direction vector
    pub fn transform_vector(&self, vector: Vector3<f32>) -> Vector3<f32> {
        let homogeneous = Vector4::new(vector.x, vector.y, vector.z, 0.0);
        let transformed = self.matrix() * homogeneous;
        Vector3::new(transformed.x, transformed.y, transformed.z)
    }
}

impl Default for Transform {
    fn default() -> Self {
        Self::new()
    }
}

/// Material properties for rendering
#[derive(Debug, Clone)]
pub struct Material {
    pub base_color: Vector3<f32>,
    pub metallic: f32,
    pub roughness: f32,
    pub emissive: Vector3<f32>,
    pub opacity: f32,
    pub alpha_mode: AlphaMode,
}

#[derive(Debug, Clone)]
pub enum AlphaMode {
    Opaque,
    Mask,
    Blend,
}

impl Material {
    pub fn new() -> Self {
        Self {
            base_color: Vector3::new(0.8, 0.8, 0.8),
            metallic: 0.0,
            roughness: 0.5,
            emissive: Vector3::zeros(),
            opacity: 1.0,
            alpha_mode: AlphaMode::Opaque,
        }
    }
}

impl Default for Material {
    fn default() -> Self {
        Self::new()
    }
}

/// Base scene object
#[derive(Debug, Clone)]
pub struct SceneObject {
    pub id: u32,
    pub name: String,
    pub object_type: ObjectType,
    pub transform: Transform,
    pub material: Material,
    pub visible: bool,
    pub cast_shadows: bool,
    pub receive_shadows: bool,
}

#[derive(Debug, Clone)]
pub enum ObjectType {
    FractalObject {
        formula: crate::fractal::types::FractalFormula,
        parameters: crate::fractal::types::FractalParameters,
    },
    Mesh {
        vertices: Vec<Vector3<f32>>,
        indices: Vec<u32>,
        normals: Vec<Vector3<f32>>,
        uvs: Vec<[f32; 2]>,
    },
    Light {
        light_type: LightType,
        intensity: f32,
        color: Vector3<f32>,
    },
    Camera {
        fov: f32,
        near: f32,
        far: f32,
    },
    Empty,
}

#[derive(Debug, Clone)]
pub enum LightType {
    Directional,
    Point { range: f32 },
    Spot { range: f32, inner_cone: f32, outer_cone: f32 },
}

impl SceneObject {
    pub fn new(id: u32, name: &str, object_type: ObjectType) -> Self {
        Self {
            id,
            name: name.to_string(),
            object_type,
            transform: Transform::new(),
            material: Material::new(),
            visible: true,
            cast_shadows: true,
            receive_shadows: true,
        }
    }

    /// Update the object
    pub fn update(&mut self, delta_time: f32) {
        // Update logic for animated objects
        if let ObjectType::FractalObject { parameters, .. } = &mut self.object_type {
            // Animate fractal parameters if needed
        }
    }

    /// Check if object should be rendered
    pub fn should_render(&self) -> bool {
        self.visible
    }

    /// Get bounding box for culling
    pub fn bounding_box(&self) -> Option<(Vector3<f32>, Vector3<f32>)> {
        match &self.object_type {
            ObjectType::Mesh { vertices, .. } => {
                if vertices.is_empty() {
                    return None;
                }
                
                let mut min = Vector3::new(f32::INFINITY, f32::INFINITY, f32::INFINITY);
                let mut max = Vector3::new(f32::NEG_INFINITY, f32::NEG_INFINITY, f32::NEG_INFINITY);
                
                for &vertex in vertices {
                    min.x = min.x.min(vertex.x);
                    min.y = min.y.min(vertex.y);
                    min.z = min.z.min(vertex.z);
                    max.x = max.x.max(vertex.x);
                    max.y = max.y.max(vertex.y);
                    max.z = max.z.max(vertex.z);
                }
                
                Some((min, max))
            }
            ObjectType::FractalObject { .. } => {
                // For fractal objects, return a large bounding box
                Some((
                    Vector3::new(-10.0, -10.0, -10.0),
                    Vector3::new(10.0, 10.0, 10.0),
                ))
            }
            _ => None,
        }
    }

    /// Check if point is inside object (for picking)
    pub fn contains_point(&self, point: &Vector3<f32>) -> bool {
        match &self.object_type {
            ObjectType::FractalObject { parameters, .. } => {
                // Use the fractal engine to check if point is inside
                let local_point = self.transform.inverse_matrix() * Vector4::new(point.x, point.y, point.z, 1.0);
                let local_point_vec = Vector3::new(local_point.x, local_point.y, local_point.z);
                
                // Simple distance check - could be more sophisticated
                local_point_vec.magnitude() < 2.0
            }
            ObjectType::Mesh { vertices, indices, .. } => {
                let local_point = self.transform.inverse_matrix() * Vector4::new(point.x, point.y, point.z, 1.0);
                let local_point_vec = Vector3::new(local_point.x, local_point.y, local_point.z);
                
                // Simple point-triangle intersection test
                // This is a simplified version - would need proper mesh intersection
                vertices.iter().any(|&v| (v - local_point_vec).magnitude() < 0.1)
            }
            _ => false,
        }
    }

    /// Get object type as string for debugging
    pub fn type_string(&self) -> &'static str {
        match &self.object_type {
            ObjectType::FractalObject { .. } => "Fractal",
            ObjectType::Mesh { .. } => "Mesh",
            ObjectType::Light { .. } => "Light",
            ObjectType::Camera { .. } => "Camera",
            ObjectType::Empty => "Empty",
        }
    }
}

/// Builder for creating scene objects
pub struct SceneObjectBuilder {
    id: u32,
    name: String,
    object_type: Option<ObjectType>,
}

impl SceneObjectBuilder {
    pub fn new(id: u32, name: &str) -> Self {
        Self {
            id,
            name: name.to_string(),
            object_type: None,
        }
    }

    pub fn fractal_object(mut self, formula: crate::fractal::types::FractalFormula, params: crate::fractal::types::FractalParameters) -> Self {
        self.object_type = Some(ObjectType::FractalObject {
            formula,
            parameters: params,
        });
        self
    }

    pub fn mesh_object(mut self, vertices: Vec<Vector3<f32>>, indices: Vec<u32>, normals: Vec<Vector3<f32>>, uvs: Vec<[f32; 2]>) -> Self {
        self.object_type = Some(ObjectType::Mesh {
            vertices,
            indices,
            normals,
            uvs,
        });
        self
    }

    pub fn light_object(mut self, light_type: LightType, intensity: f32, color: Vector3<f32>) -> Self {
        self.object_type = Some(ObjectType::Light {
            light_type,
            intensity,
            color,
        });
        self
    }

    pub fn camera_object(mut self, fov: f32, near: f32, far: f32) -> Self {
        self.object_type = Some(ObjectType::Camera {
            fov,
            near,
            far,
        });
        self
    }

    pub fn empty_object(mut self) -> Self {
        self.object_type = Some(ObjectType::Empty);
        self
    }

    pub fn position(mut self, position: Vector3<f32>) -> Self {
        // Would need to store this temporarily and apply in build
        self
    }

    pub fn rotation(mut self, rotation: Vector3<f32>) -> Self {
        // Would need to store this temporarily and apply in build
        self
    }

    pub fn scale(mut self, scale: Vector3<f32>) -> Self {
        // Would need to store this temporarily and apply in build
        self
    }

    pub fn material(mut self, material: Material) -> Self {
        // Would need to store this temporarily and apply in build
        self
    }

    pub fn build(self) -> Option<SceneObject> {
        Some(SceneObject::new(self.id, &self.name, self.object_type?))
    }
}