//! 3D Scene Management Module
//!
//! This module handles 3D scene objects, hierarchies, transformations,
//! and scene graph management for fractal visualization.

use nalgebra::{Matrix4, Vector3, Vector4, UnitQuaternion};
use std::collections::HashMap;
use serde::{Deserialize, Serialize};

/// Unique identifier for scene objects
pub type ObjectId = u64;

/// 3D transformation with position, rotation, and scale
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Transform {
    pub position: Vector3<f32>,
    pub rotation: UnitQuaternion<f32>,
    pub scale: Vector3<f32>,
}

impl Default for Transform {
    fn default() -> Self {
        Self {
            position: Vector3::zeros(),
            rotation: UnitQuaternion::identity(),
            scale: Vector3::new(1.0, 1.0, 1.0),
        }
    }
}

impl Transform {
    /// Create transformation matrix
    pub fn matrix(&self) -> Matrix4<f32> {
        let translation = Matrix4::new_translation(&self.position);
        let rotation = self.rotation.to_homogeneous();
        let scale = Matrix4::new_nonuniform_scaling(&self.scale);
        translation * rotation * scale
    }

    /// Get forward direction vector
    pub fn forward(&self) -> Vector3<f32> {
        self.rotation * Vector3::z()
    }

    /// Get right direction vector
    pub fn right(&self) -> Vector3<f32> {
        self.rotation * Vector3::x()
    }

    /// Get up direction vector
    pub fn up(&self) -> Vector3<f32> {
        self.rotation * Vector3::y()
    }
}

/// Material properties for scene objects
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Material {
    pub base_color: Vector3<f32>,
    pub metallic: f32,
    pub roughness: f32,
    pub emissive: Vector3<f32>,
    pub transparency: f32,
}

impl Default for Material {
    fn default() -> Self {
        Self {
            base_color: Vector3::new(0.8, 0.8, 0.8),
            metallic: 0.0,
            roughness: 0.5,
            emissive: Vector3::zeros(),
            transparency: 1.0,
        }
    }
}

/// Types of objects that can exist in the scene
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum ObjectType {
    /// Fractal object with fractal parameters
    FractalObject {
        fractal_params: crate::fractal::FractalParameters,
    },
    /// Mesh object with geometry
    Mesh {
        vertices: Vec<Vector3<f32>>,
        indices: Vec<u32>,
        normals: Vec<Vector3<f32>>,
        uvs: Vec<Vector2<f32>>,
    },
    /// Light source
    Light {
        light_type: LightType,
        color: Vector3<f32>,
        intensity: f32,
        range: f32,
        spot_angle: Option<f32>,
    },
    /// Camera object
    Camera {
        fov: f32,
        near: f32,
        far: f32,
        aspect_ratio: f32,
    },
}

/// Light types
#[derive(Debug, Clone, Copy, Serialize, Deserialize)]
pub enum LightType {
    Directional,
    Point,
    Spot,
}

/// 2D vector for UV coordinates
use nalgebra::Vector2;

/// Scene object with transform and properties
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SceneObject {
    pub id: ObjectId,
    pub name: String,
    pub transform: Transform,
    pub material: Material,
    pub object_type: ObjectType,
    pub parent_id: Option<ObjectId>,
    pub children: Vec<ObjectId>,
    pub visible: bool,
}

impl SceneObject {
    /// Create a new fractal object
    pub fn new_fractal(name: &str, fractal_params: crate::fractal::FractalParameters) -> Self {
        Self {
            id: generate_id(),
            name: name.to_string(),
            transform: Transform::default(),
            material: Material::default(),
            object_type: ObjectType::FractalObject { fractal_params },
            parent_id: None,
            children: Vec::new(),
            visible: true,
        }
    }

    /// Create a new light object
    pub fn new_light(name: &str, light_type: LightType, color: Vector3<f32>, intensity: f32) -> Self {
        Self {
            id: generate_id(),
            name: name.to_string(),
            transform: Transform::default(),
            material: Material::default(),
            object_type: ObjectType::Light {
                light_type,
                color,
                intensity,
                range: 10.0,
                spot_angle: if matches!(light_type, LightType::Spot) { Some(45.0) } else { None },
            },
            parent_id: None,
            children: Vec::new(),
            visible: true,
        }
    }

    /// Create a new camera object
    pub fn new_camera(name: &str, fov: f32, aspect_ratio: f32) -> Self {
        Self {
            id: generate_id(),
            name: name.to_string(),
            transform: Transform::default(),
            material: Material::default(),
            object_type: ObjectType::Camera {
                fov,
                near: 0.1,
                far: 1000.0,
                aspect_ratio,
            },
            parent_id: None,
            children: Vec::new(),
            visible: true,
        }
    }

    /// Get world transformation matrix
    pub fn world_matrix(&self, scene: &Scene) -> Matrix4<f32> {
        let mut matrix = self.transform.matrix();

        let mut current_id = self.parent_id;
        while let Some(parent_id) = current_id {
            if let Some(parent) = scene.objects.get(&parent_id) {
                matrix = parent.transform.matrix() * matrix;
                current_id = parent.parent_id;
            } else {
                break;
            }
        }

        matrix
    }

    /// Get world position
    pub fn world_position(&self, scene: &Scene) -> Vector3<f32> {
        let world_matrix = self.world_matrix(scene);
        Vector3::new(world_matrix[(0, 3)], world_matrix[(1, 3)], world_matrix[(2, 3)])
    }
}

/// 3D scene containing all objects and settings
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Scene {
    pub objects: HashMap<ObjectId, SceneObject>,
    pub active_camera_id: Option<ObjectId>,
    pub background_color: Vector3<f32>,
    pub ambient_light: Vector3<f32>,
    pub name: String,
}

impl Scene {
    /// Create a new empty scene
    pub fn new(name: &str) -> Self {
        Self {
            objects: HashMap::new(),
            active_camera_id: None,
            background_color: Vector3::new(0.1, 0.1, 0.15),
            ambient_light: Vector3::new(0.1, 0.1, 0.1),
            name: name.to_string(),
        }
    }

    /// Add object to scene
    pub fn add_object(&mut self, mut object: SceneObject) -> ObjectId {
        let id = object.id;
        self.objects.insert(id, object);
        id
    }

    /// Remove object from scene
    pub fn remove_object(&mut self, id: ObjectId) -> bool {
        if let Some(object) = self.objects.remove(&id) {
            // Remove from parent's children list
            if let Some(parent_id) = object.parent_id {
                if let Some(parent) = self.objects.get_mut(&parent_id) {
                    parent.children.retain(|&child_id| child_id != id);
                }
            }

            // Remove children recursively
            for child_id in &object.children.clone() {
                self.remove_object(*child_id);
            }

            true
        } else {
            false
        }
    }

    /// Set parent-child relationship
    pub fn set_parent(&mut self, child_id: ObjectId, parent_id: Option<ObjectId>) -> bool {
        // First, collect the old parent ID
        let old_parent_id = self.objects.get(&child_id)
            .map(|child| child.parent_id)
            .flatten();

        // Remove from old parent if exists
        if let Some(old_parent_id) = old_parent_id {
            if let Some(old_parent) = self.objects.get_mut(&old_parent_id) {
                old_parent.children.retain(|&id| id != child_id);
            }
        }

        // Set new parent
        if let Some(child) = self.objects.get_mut(&child_id) {
            child.parent_id = parent_id;

            // Add to new parent if exists
            if let Some(new_parent_id) = parent_id {
                if let Some(new_parent) = self.objects.get_mut(&new_parent_id) {
                    new_parent.children.push(child_id);
                }
            }

            true
        } else {
            false
        }
    }

    /// Get active camera
    pub fn active_camera(&self) -> Option<&SceneObject> {
        self.active_camera_id.and_then(|id| self.objects.get(&id))
    }

    /// Set active camera
    pub fn set_active_camera(&mut self, camera_id: ObjectId) {
        if self.objects.contains_key(&camera_id) {
            if let Some(obj) = self.objects.get(&camera_id) {
                if matches!(obj.object_type, ObjectType::Camera { .. }) {
                    self.active_camera_id = Some(camera_id);
                }
            }
        }
    }

    /// Get all fractal objects
    pub fn fractal_objects(&self) -> Vec<&SceneObject> {
        self.objects.values()
            .filter(|obj| matches!(obj.object_type, ObjectType::FractalObject { .. }))
            .collect()
    }

    /// Get all light objects
    pub fn lights(&self) -> Vec<&SceneObject> {
        self.objects.values()
            .filter(|obj| matches!(obj.object_type, ObjectType::Light { .. }))
            .collect()
    }

    /// Create default scene with basic setup
    pub fn create_default_scene() -> Self {
        let mut scene = Self::new("Default Scene");

        // Add default camera
        let camera = SceneObject::new_camera("Main Camera", 60.0, 16.0 / 9.0);
        let camera_id = scene.add_object(camera);
        scene.set_active_camera(camera_id);

        // Add directional light
        let light = SceneObject::new_light("Directional Light", LightType::Directional,
                                         Vector3::new(1.0, 1.0, 1.0), 1.0);
        scene.add_object(light);

        // Add default fractal
        let fractal_params = crate::fractal::FractalParameters::default();
        let fractal = SceneObject::new_fractal("Mandelbulb", fractal_params);
        scene.add_object(fractal);

        scene
    }
}

/// Generate unique object ID
fn generate_id() -> ObjectId {
    use std::sync::atomic::{AtomicU64, Ordering};
    static COUNTER: AtomicU64 = AtomicU64::new(1);
    COUNTER.fetch_add(1, Ordering::Relaxed)
}