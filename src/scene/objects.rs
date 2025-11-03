//! Scene Objects Implementation
//!
//! This module provides implementations for scene objects, transformations,
//! and object management utilities.

use super::*;
use std::time::Duration;

/// Update scene objects over time
pub trait SceneObjectUpdate {
    fn update(&mut self, delta_time: f32);
}

impl SceneObjectUpdate for SceneObject {
    fn update(&mut self, delta_time: f32) {
        match &mut self.object_type {
            ObjectType::FractalObject { fractal_params } => {
                // Update fractal animation parameters
                // TODO: Implement fractal parameter animation
            }
            ObjectType::Light { .. } => {
                // Lights don't need per-frame updates
            }
            ObjectType::Camera { .. } => {
                // Cameras don't need per-frame updates
            }
            ObjectType::Mesh { .. } => {
                // Static meshes don't need updates
            }
        }
    }
}

/// Scene management utilities
pub struct SceneManager {
    pub current_scene: Scene,
    pub selected_objects: Vec<ObjectId>,
}

impl SceneManager {
    /// Create new scene manager
    pub fn new() -> Self {
        Self {
            current_scene: Scene::create_default_scene(),
            selected_objects: Vec::new(),
        }
    }

    /// Update all scene objects
    pub fn update(&mut self, delta_time: f32) {
        for object in self.current_scene.objects.values_mut() {
            object.update(delta_time);
        }
    }

    /// Select object
    pub fn select_object(&mut self, object_id: ObjectId, additive: bool) {
        if !additive {
            self.selected_objects.clear();
        }

        if !self.selected_objects.contains(&object_id) {
            self.selected_objects.push(object_id);
        }
    }

    /// Deselect object
    pub fn deselect_object(&mut self, object_id: ObjectId) {
        self.selected_objects.retain(|&id| id != object_id);
    }

    /// Clear selection
    pub fn clear_selection(&mut self) {
        self.selected_objects.clear();
    }

    /// Get selected objects
    pub fn selected_objects(&self) -> Vec<&SceneObject> {
        self.selected_objects.iter()
            .filter_map(|&id| self.current_scene.objects.get(&id))
            .collect()
    }

    /// Duplicate selected objects
    pub fn duplicate_selected(&mut self) {
        let objects_to_duplicate: Vec<SceneObject> = self.selected_objects()
            .into_iter()
            .cloned()
            .collect();

        self.clear_selection();

        for mut object in objects_to_duplicate {
            // Generate new ID
            object.id = generate_id();
            object.name = format!("{} Copy", object.name);

            // Offset position slightly
            object.transform.position += Vector3::new(1.0, 1.0, 1.0);

            let new_id = self.current_scene.add_object(object);
            self.select_object(new_id, true);
        }
    }

    /// Delete selected objects
    pub fn delete_selected(&mut self) {
        for &object_id in &self.selected_objects.clone() {
            self.current_scene.remove_object(object_id);
        }
        self.clear_selection();
    }
}

/// Camera controller for scene navigation
pub struct CameraController {
    pub target: Vector3<f32>,
    pub distance: f32,
    pub yaw: f32,
    pub pitch: f32,
    pub sensitivity: f32,
}

impl CameraController {
    /// Create new camera controller
    pub fn new(target: Vector3<f32>, distance: f32) -> Self {
        Self {
            target,
            distance,
            yaw: 0.0,
            pitch: 0.0,
            sensitivity: 0.005,
        }
    }

    /// Update camera transform based on controller state
    pub fn update_camera_transform(&self, camera: &mut SceneObject) {
        if let ObjectType::Camera { .. } = camera.object_type {
            // Calculate camera position using spherical coordinates
            let x = self.target.x + self.distance * self.yaw.cos() * self.pitch.cos();
            let y = self.target.y + self.distance * self.pitch.sin();
            let z = self.target.z + self.distance * self.yaw.sin() * self.pitch.cos();

            camera.transform.position = Vector3::new(x, y, z);

            // Calculate rotation to look at target
            let forward = (self.target - camera.transform.position).normalize();
            let right = forward.cross(&Vector3::y()).normalize();
            let up = right.cross(&forward).normalize();

            // Create rotation matrix and convert to quaternion
            let rotation_matrix = Matrix4::new(
                right.x, up.x, -forward.x, 0.0,
                right.y, up.y, -forward.y, 0.0,
                right.z, up.z, -forward.z, 0.0,
                0.0, 0.0, 0.0, 1.0,
            );

            camera.transform.rotation = UnitQuaternion::from_matrix(&rotation_matrix.fixed_view::<3, 3>(0, 0).into());
        }
    }

    /// Orbit camera around target
    pub fn orbit(&mut self, delta_yaw: f32, delta_pitch: f32) {
        self.yaw += delta_yaw * self.sensitivity;
        self.pitch = (self.pitch + delta_pitch * self.sensitivity).clamp(-std::f32::consts::PI / 2.0 + 0.1, std::f32::consts::PI / 2.0 - 0.1);
    }

    /// Zoom camera
    pub fn zoom(&mut self, delta_distance: f32) {
        self.distance = (self.distance + delta_distance).max(0.1);
    }

    /// Pan camera target
    pub fn pan(&mut self, delta_x: f32, delta_y: f32) {
        let right = Vector3::new(self.yaw.cos(), 0.0, self.yaw.sin());
        let up = Vector3::y();
        self.target += right * delta_x * self.distance * self.sensitivity;
        self.target += up * delta_y * self.distance * self.sensitivity;
    }
}

/// Scene serialization/deserialization
pub mod serialization {
    use super::*;
    use serde::{Deserialize, Serialize};

    #[derive(Serialize, Deserialize)]
    struct SerializableScene {
        objects: Vec<SerializableObject>,
        active_camera_id: Option<ObjectId>,
        background_color: [f32; 3],
        ambient_light: [f32; 3],
        name: String,
    }

    #[derive(Serialize, Deserialize)]
    struct SerializableObject {
        id: ObjectId,
        name: String,
        transform: SerializableTransform,
        material: SerializableMaterial,
        object_type: SerializableObjectType,
        parent_id: Option<ObjectId>,
        visible: bool,
    }

    #[derive(Serialize, Deserialize)]
    struct SerializableTransform {
        position: [f32; 3],
        rotation: [f32; 4], // quaternion
        scale: [f32; 3],
    }

    #[derive(Serialize, Deserialize)]
    struct SerializableMaterial {
        base_color: [f32; 3],
        metallic: f32,
        roughness: f32,
        emissive: [f32; 3],
        transparency: f32,
    }

    #[derive(Serialize, Deserialize)]
    enum SerializableObjectType {
        FractalObject { fractal_params: crate::fractal::FractalParameters },
        Mesh { vertices: Vec<[f32; 3]>, indices: Vec<u32>, normals: Vec<[f32; 3]>, uvs: Vec<[f32; 2]> },
        Light { light_type: LightType, color: [f32; 3], intensity: f32, range: f32, spot_angle: Option<f32> },
        Camera { fov: f32, near: f32, far: f32, aspect_ratio: f32 },
    }

    impl Scene {
        /// Serialize scene to JSON
        pub fn to_json(&self) -> Result<String, Box<dyn std::error::Error>> {
            let serializable = SerializableScene {
                objects: self.objects.values().map(|obj| obj.to_serializable()).collect(),
                active_camera_id: self.active_camera_id,
                background_color: self.background_color.into(),
                ambient_light: self.ambient_light.into(),
                name: self.name.clone(),
            };

            Ok(serde_json::to_string_pretty(&serializable)?)
        }

        /// Deserialize scene from JSON
        pub fn from_json(json: &str) -> Result<Self, Box<dyn std::error::Error>> {
            let serializable: SerializableScene = serde_json::from_str(json)?;

            let mut scene = Self {
                objects: HashMap::new(),
                active_camera_id: serializable.active_camera_id,
                background_color: serializable.background_color.into(),
                ambient_light: serializable.ambient_light.into(),
                name: serializable.name,
            };

            for serializable_obj in serializable.objects {
                let object = SceneObject::from_serializable(serializable_obj);
                scene.objects.insert(object.id, object);
            }

            Ok(scene)
        }
    }

    impl SceneObject {
        fn to_serializable(&self) -> SerializableObject {
            SerializableObject {
                id: self.id,
                name: self.name.clone(),
                transform: SerializableTransform {
                    position: self.transform.position.into(),
                    rotation: self.transform.rotation.as_vector().into(),
                    scale: self.transform.scale.into(),
                },
                material: SerializableMaterial {
                    base_color: self.material.base_color.into(),
                    metallic: self.material.metallic,
                    roughness: self.material.roughness,
                    emissive: self.material.emissive.into(),
                    transparency: self.material.transparency,
                },
                object_type: match &self.object_type {
                    ObjectType::FractalObject { fractal_params } => {
                        SerializableObjectType::FractalObject { fractal_params: fractal_params.clone() }
                    }
                    ObjectType::Mesh { vertices, indices, normals, uvs } => {
                        SerializableObjectType::Mesh {
                            vertices: vertices.iter().map(|v| v.into()).collect(),
                            indices: indices.clone(),
                            normals: normals.iter().map(|n| n.into()).collect(),
                            uvs: uvs.iter().map(|uv| uv.into()).collect(),
                        }
                    }
                    ObjectType::Light { light_type, color, intensity, range, spot_angle } => {
                        SerializableObjectType::Light {
                            light_type: *light_type,
                            color: color.into(),
                            intensity: *intensity,
                            range: *range,
                            spot_angle: *spot_angle,
                        }
                    }
                    ObjectType::Camera { fov, near, far, aspect_ratio } => {
                        SerializableObjectType::Camera {
                            fov: *fov,
                            near: *near,
                            far: *far,
                            aspect_ratio: *aspect_ratio,
                        }
                    }
                },
                parent_id: self.parent_id,
                visible: self.visible,
            }
        }

        fn from_serializable(serializable: SerializableObject) -> Self {
            Self {
                id: serializable.id,
                name: serializable.name,
                transform: Transform {
                    position: serializable.transform.position.into(),
                    rotation: UnitQuaternion::from_quaternion(
                        nalgebra::Quaternion::new(
                            serializable.transform.rotation[3],
                            serializable.transform.rotation[0],
                            serializable.transform.rotation[1],
                            serializable.transform.rotation[2],
                        )
                    ),
                    scale: serializable.transform.scale.into(),
                },
                material: Material {
                    base_color: serializable.material.base_color.into(),
                    metallic: serializable.material.metallic,
                    roughness: serializable.material.roughness,
                    emissive: serializable.material.emissive.into(),
                    transparency: serializable.material.transparency,
                },
                object_type: match serializable.object_type {
                    SerializableObjectType::FractalObject { fractal_params } => {
                        ObjectType::FractalObject { fractal_params }
                    }
                    SerializableObjectType::Mesh { vertices, indices, normals, uvs } => {
                        ObjectType::Mesh {
                            vertices: vertices.into_iter().map(|v| v.into()).collect(),
                            indices,
                            normals: normals.into_iter().map(|n| n.into()).collect(),
                            uvs: uvs.into_iter().map(|uv| uv.into()).collect(),
                        }
                    }
                    SerializableObjectType::Light { light_type, color, intensity, range, spot_angle } => {
                        ObjectType::Light {
                            light_type,
                            color: color.into(),
                            intensity,
                            range,
                            spot_angle,
                        }
                    }
                    SerializableObjectType::Camera { fov, near, far, aspect_ratio } => {
                        ObjectType::Camera { fov, near, far, aspect_ratio }
                    }
                },
                parent_id: serializable.parent_id,
                children: Vec::new(), // Will be reconstructed
                visible: serializable.visible,
            }
        }
    }
}