odular-fractal-shader/src/assets/mod.rs</path>
<content lines="1-150">
pub mod manager;
pub mod presets;

/// Asset management system for fractal formulas, gradients, and setups
pub struct AssetManager {
    fractal_presets: Vec<FractalPreset>,
    color_gradients: Vec<ColorGradient>,
    lighting_setups: Vec<LightingSetup>,
    material_presets: Vec<MaterialPreset>,
    animation_presets: Vec<AnimationPreset>,
}

impl AssetManager {
    pub fn new() -> Self {
        Self {
            fractal_presets: Vec::new(),
            color_gradients: Vec::new(),
            lighting_setups: Vec::new(),
            material_presets: Vec::new(),
            animation_presets: Vec::new(),
        }
    }

    pub fn load_default_assets(&mut self) {
        self.load_fractal_presets();
        self.load_color_gradients();
        self.load_lighting_setups();
        self.load_material_presets();
        self.load_animation_presets();
    }

    pub fn fractal_presets(&self) -> &[FractalPreset] {
        &self.fractal_presets
    }

    pub fn color_gradients(&self) -> &[ColorGradient] {
        &self.color_gradients
    }

    pub fn lighting_setups(&self) -> &[LightingSetup] {
        &self.lighting_setups
    }

    pub fn material_presets(&self) -> &[MaterialPreset] {
        &self.material_presets
    }

    pub fn animation_presets(&self) -> &[AnimationPreset] {
        &self.animation_presets
    }

    pub fn save_fractal_preset(&mut self, preset: FractalPreset) {
        self.fractal_presets.push(preset);
    }

    pub fn save_color_gradient(&mut self, gradient: ColorGradient) {
        self.color_gradients.push(gradient);
    }

    pub fn save_lighting_setup(&mut self, setup: LightingSetup) {
        self.lighting_setups.push(setup);
    }

    pub fn save_material_preset(&mut self, preset: MaterialPreset) {
        self.material_presets.push(preset);
    }

    pub fn save_animation_preset(&mut self, preset: AnimationPreset) {
        self.animation_presets.push(preset);
    }

    fn load_fractal_presets(&mut self) {
        // Classic fractals
        self.fractal_presets.push(FractalPreset {
            name: "Mandelbrot Classic".to_string(),
            description: "The original Mandelbrot set".to_string(),
            formula: crate::fractal::types::FractalFormula::Mandelbrot {
                center: [-0.5, 0.0],
                zoom: 1.0,
                max_iterations: 100,
            },
            parameters: crate::fractal::types::FractalParameters::default(),
            thumbnail: None,
        });

        self.fractal_presets.push(FractalPreset {
            name: "Mandelbulb Power 8".to_string(),
            description: "Classic 3D Mandelbulb".to_string(),
            formula: crate::fractal::types::FractalFormula::Mandelbulb {
                power: 8.0,
                max_iterations: 100,
            },
            parameters: crate::fractal::types::FractalParameters::default(),
            thumbnail: None,
        });

        // Advanced fractals
        self.fractal_presets.push(FractalPreset {
            name: "Mandelbox Fold".to_string(),
            description: "Box folding fractal".to_string(),
            formula: crate::fractal::types::FractalFormula::Mandelbox {
                scale: 2.0,
                folding_limit: 1.0,
                max_iterations: 100,
            },
            parameters: crate::fractal::types::FractalParameters::default(),
            thumbnail: None,
        });

        self.fractal_presets.push(FractalPreset {
            name: "Quaternion Julia".to_string(),
            description: "4D quaternion fractal".to_string(),
            formula: crate::fractal::types::FractalFormula::QuaternionJulia {
                c: [0.3, 0.5, 0.4, 0.2],
                max_iterations: 100,
            },
            parameters: crate::fractal::types::FractalParameters::default(),
            thumbnail: None,
        });
    }

    fn load_color_gradients(&mut self) {
        self.color_gradients.push(ColorGradient {
            name: "Fire".to_string(),
            description: "Warm fire colors".to_string(),
            colors: vec![
                nalgebra::Vector3::new(0.0, 0.0, 0.0),
                nalgebra::Vector3::new(1.0, 0.5, 0.0),
                nalgebra::Vector3::new(1.0, 1.0, 0.0),
                nalgebra::Vector3::new(1.0, 0.0, 0.0),
            ],
            positions: vec![0.0, 0.33, 0.66, 1.0],
        });

        self.color_gradients.push(ColorGradient {
            name: "Ocean".to_string(),
            description: "Cool ocean colors".to_string(),
            colors: vec![
                nalgebra::Vector3::new(0.0, 0.0, 0.2),
                nalgebra::Vector3::new(0.0, 0.5, 1.0),
                nalgebra::Vector3::new(0.0, 1.0, 1.0),
                nalgebra::Vector3::new(1.0, 1.0, 1.0),
            ],
            positions: vec![0.0, 0.33, 0.66, 1.0],
        });

        self.color_gradients.push(ColorGradient {
            name: "Electric".to_string(),
            description: "Vibrant electric colors".to_string(),
            colors: vec![
                nalgebra::Vector3::new(0.0, 0.0, 0.0),
                nalgebra::Vector3::new(0.0, 1.0, 1.0),
                nalgebra::Vector3::new(1.0, 0.0, 1.0),
                nalgebra::Vector3::new(1.0, 1.0, 0.0),
                nalgebra::Vector3::new(1.0, 1.0, 1.0),
            ],
            positions: vec![0.0, 0.25, 0.5, 0.75, 1.0],
        });
    }

    fn load_lighting_setups(&mut self) {
        self.lighting_setups.push(LightingSetup {
            name: "Studio Soft".to_string(),
            description: "Soft, even lighting".to_string(),
            directional_lights: vec![DirectionalLight {
                direction: nalgebra::Vector3::new(-0.5, -0.5, -0.5).normalize(),
                color: nalgebra::Vector3::new(1.0, 1.0, 1.0),
                intensity: 0.8,
            }],
            point_lights: vec![
                PointLight {
                    position: nalgebra::Vector3::new(2.0, 2.0, 2.0),
                    color: nalgebra::Vector3::new(1.0, 0.9, 0.8),
                    intensity: 0.3,
                    range: 10.0,
                },
                PointLight {
                    position: nalgebra::Vector3::new(-2.0, 2.0, 2.0),
                    color: nalgebra::Vector3::new(0.8, 0.9, 1.0),
                    intensity: 0.3,
                    range: 10.0,
                },
            ],
            ambient_intensity: 0.4,
        });

        self.lighting_setups.push(LightingSetup {
            name: "Dramatic Rim".to_string(),
            description: "High contrast rim lighting".to_string(),
            directional_lights: vec![
                DirectionalLight {
                    direction: nalgebra::Vector3::new(1.0, 0.5, 0.0).normalize(),
                    color: nalgebra::Vector3::new(1.0, 0.8, 0.6),
                    intensity: 1.2,
                },
                DirectionalLight {
                    direction: nalgebra::Vector3::new(-1.0, -0.5, 0.0).normalize(),
                    color: nalgebra::Vector3::new(0.6, 0.8, 1.0),
                    intensity: 0.3,
                },
            ],
            point_lights: Vec::new(),
            ambient_intensity: 0.1,
        });
    }

    fn load_material_presets(&mut self) {
        self.material_presets.push(MaterialPreset {
            name: "Plastic Shiny".to_string(),
            description: "Smooth plastic material".to_string(),
            base_color: nalgebra::Vector3::new(0.8, 0.8, 0.9),
            metallic: 0.0,
            roughness: 0.1,
            emission: nalgebra::Vector3::zeros(),
            emission_intensity: 0.0,
        });

        self.material_presets.push(MaterialPreset {
            name: "Metal Rough".to_string(),
            description: "Rough metallic surface".to_string(),
            base_color: nalgebra::Vector3::new(0.9, 0.8, 0.7),
            metallic: 1.0,
            roughness: 0.7,
            emission: nalgebra::Vector3::zeros(),
            emission_intensity: 0.0,
        });

        self.material_presets.push(MaterialPreset {
            name: "Glow Plastic".to_string(),
            description: "Glowing plastic material".to_string(),
            base_color: nalgebra::Vector3::new(0.2, 0.8, 1.0),
            metallic: 0.0,
            roughness: 0.3,
            emission: nalgebra::Vector3::new(0.2, 0.8, 1.0),
            emission_intensity: 0.5,
        });
    }

    fn load_animation_presets(&mut self) {
        self.animation_presets.push(AnimationPreset {
            name: "Slow Zoom".to_string(),
            description: "Gradual zoom into fractal".to_string(),
            duration: 10.0,
            tracks: vec![
                AnimationTrack {
                    name: "Camera Zoom".to_string(),
                    parameter_path: "camera.zoom".to_string(),
                    keyframes: vec![
                        Keyframe { time: 0.0, value: 1.0, easing: EasingType::Linear },
                        Keyframe { time: 10.0, value: 10.0, easing: EasingType::EaseIn },
                    ],
                    interpolation: InterpolationMode::Cubic,
                },
            ],
        });

        self.animation_presets.push(AnimationPreset {
            name: "Color Cycle".to_string(),
            description: "Cycling through color palette".to_string(),
            duration: 8.0,
            tracks: vec![
                AnimationTrack {
                    name: "Hue Shift".to_string(),
                    parameter_path: "fractal.color.hue_shift".to_string(),
                    keyframes: vec![
                        Keyframe { time: 0.0, value: 0.0, easing: EasingType::Linear },
                        Keyframe { time: 8.0, value: 360.0, easing: EasingType::Linear },
                    ],
                    interpolation: InterpolationMode::Linear,
                },
            ],
        });
    }
}

/// Data structures
#[derive(Debug, Clone)]
pub struct FractalPreset {
    pub name: String,
    pub description: String,
    pub formula: crate::fractal::types::FractalFormula,
    pub parameters: crate::fractal::types::FractalParameters,
    pub thumbnail: Option<Vec<u8>>, // PNG thumbnail data
}

#[derive(Debug, Clone)]
pub struct ColorGradient {
    pub name: String,
    pub description: String,
    pub colors: Vec<nalgebra::Vector3<f32>>,
    pub positions: Vec<f32>,
}

#[derive(Debug, Clone)]
pub struct LightingSetup {
    pub name: String,
    pub description: String,
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
pub struct MaterialPreset {
    pub name: String,
    pub description: String,
    pub base_color: nalgebra::Vector3<f32>,
    pub metallic: f32,
    pub roughness: f32,
    pub emission: nalgebra::Vector3<f32>,
    pub emission_intensity: f32,
}

#[derive(Debug, Clone)]
pub struct AnimationPreset {
    pub name: String,
    pub description: String,
    pub duration: f32,
    pub tracks: Vec<AnimationTrack>,
}

#[derive(Debug, Clone)]
pub struct AnimationTrack {
    pub name: String,
    pub parameter_path: String,
    pub keyframes: Vec<Keyframe>,
    pub interpolation: InterpolationMode,
}

#[derive(Debug, Clone)]
pub struct Keyframe {
    pub time: f32,
    pub value: f32,
    pub easing: EasingType,
}

#[derive(Debug, Clone)]
pub enum InterpolationMode {
    Linear,
    Cubic,
    Step,
}

#[derive(Debug, Clone)]
pub enum EasingType {
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    Cubic,
    Sine,
    Exponential,
}

impl Default for AssetManager {
    fn default() -> Self {
        Self::new()
    }
}