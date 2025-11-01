odular-fractal-shader/src/scene/mod.rs</path>
<content lines="1-50">
pub mod objects;

/// Scene management system for 3D fractal environments
pub struct SceneManager {
    objects: Vec<SceneObject>,
    lighting: LightingSetup,
    camera: Camera,
    background: Background,
}

impl SceneManager {
    pub fn new() -> Self {
        Self {
            objects: Vec::new(),
            lighting: LightingSetup::default(),
            camera: Camera::default(),
            background: Background::default(),
        }
    }

    pub fn add_object(&mut self, object: SceneObject) {
        self.objects.push(object);
    }

    pub fn objects(&self) -> &[SceneObject] {
        &self.objects
    }

    pub fn lighting(&self) -> &LightingSetup {
        &self.lighting
    }

    pub fn get_object(&self, id: u32) -> Option<&SceneObject> {
        self.objects.iter().find(|obj| obj.id == id)
    }

    pub fn get_object_mut(&mut self, id: u32) -> Option<&mut SceneObject> {
        self.objects.iter_mut().find(|obj| obj.id == id)
    }
}

#[derive(Debug, Clone)]
pub struct SceneObject {
    pub id: u32,
    pub name: String,
    pub transform: Transform,
    pub visible: bool,
    pub object_type: ObjectType,
}

#[derive(Debug, Clone)]
pub struct Transform {
    pub position: nalgebra::Vector3<f32>,
    pub rotation: nalgebra::Vector3<f32>,
    pub scale: nalgebra::Vector3<f32>,
}

#[derive(Debug, Clone)]
pub enum ObjectType {
    FractalObject { formula: crate::fractal::types::FractalFormula, parameters: crate::fractal::types::FractalParameters },
    MeshObject { mesh_path: String },
    LightObject { light_type: LightType },
}

#[derive(Debug, Clone)]
pub enum LightType {
    Directional,
    Point,
    Spot,
}

#[derive(Debug, Clone)]
pub struct LightingSetup {
    pub directional_lights: Vec<DirectionalLight>,
    pub point_lights: Vec<PointLight>,
    pub ambient_intensity: f32,
}

#[derive(Debug, Clone)]
pub struct DirectionalLight {
    pub direction: nalgebra::Vector3<f32>,
    pub color: nalgebra::Vector3<f32>,
    pub intensity: f32,
}

#[derive(Debug, Clone)]
pub struct PointLight {
    pub position: nalgebra::Vector3<f32>,
    pub color: nalgebra::Vector3<f32>,
    pub intensity: f32,
    pub range: f32,
}

#[derive(Debug, Clone)]
pub struct Camera {
    pub position: nalgebra::Vector3<f32>,
    pub target: nalgebra::Vector3<f32>,
    pub up: nalgebra::Vector3<f32>,
    pub fov: f32,
    pub near: f32,
    pub far: f32,
}

#[derive(Debug, Clone)]
pub struct Background {
    pub color: nalgebra::Vector3<f32>,
    pub gradient: Option<Gradient>,
}

#[derive(Debug, Clone)]
pub struct Gradient {
    pub top_color: nalgebra::Vector3<f32>,
    pub bottom_color: nalgebra::Vector3<f32>,
}

impl Default for LightingSetup {
    fn default() -> Self {
        Self {
            directional_lights: vec![DirectionalLight {
                direction: nalgebra::Vector3::new(-1.0, -1.0, -1.0).normalize(),
                color: nalgebra::Vector3::new(1.0, 1.0, 1.0),
                intensity: 1.0,
            }],
            point_lights: Vec::new(),
            ambient_intensity: 0.2,
        }
    }
}

impl Default for Camera {
    fn default() -> Self {
        Self {
            position: nalgebra::Vector3::new(0.0, 0.0, 5.0),
            target: nalgebra::Vector3::zeros(),
            up: nalgebra::Vector3::new(0.0, 1.0, 0.0),
            fov: 45.0,
            near: 0.1,
            far: 1000.0,
        }
    }
}

impl Default for Background {
    fn default() -> Self {
        Self {
            color: nalgebra::Vector3::new(0.1, 0.1, 0.15),
            gradient: None,
        }
    }
}