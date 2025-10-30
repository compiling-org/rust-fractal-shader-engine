use bevy::prelude::*;
use std::collections::HashMap;

/// Component for fractal shader entities
#[derive(Component)]
pub struct FractalShader {
    pub shader_name: String,
    pub parameters: HashMap<String, f32>,
}

/// Resource for managing shader materials
#[derive(Resource)]
pub struct ShaderMaterials {
    pub materials: HashMap<String, Handle<StandardMaterial>>,
}

/// Shader uniform buffer for fractal parameters
#[derive(Clone, Copy, bytemuck::Pod, bytemuck::Zeroable)]
#[repr(C)]
pub struct FractalUniforms {
    pub time: f32,
    pub resolution: [f32; 2],
    pub mouse: [f32; 2],
    pub zoom: f32,
    pub iterations: f32,
    pub speed: f32,
    pub brightness: f32,
    pub contrast: f32,
    pub saturation: f32,
    pub hue_shift: f32,
    pub audio_bass: f32,
    pub audio_mid: f32,
    pub audio_treble: f32,
    pub audio_volume: f32,
    pub midi_cc1: f32,
    pub midi_cc2: f32,
    pub midi_cc3: f32,
    pub midi_cc4: f32,
}

/// Resource for managing shader uniforms
#[derive(Resource)]
pub struct ShaderUniforms {
    pub buffer: FractalUniforms,
}

/// System to update fractal shader parameters
pub fn update_fractal_shaders(
    time: Res<Time>,
    mut shader_uniforms: ResMut<ShaderUniforms>,
    audio_data: Option<Res<crate::audio::AudioData>>,
    midi_controller: Option<Res<crate::audio::MidiController>>,
    windows: Query<&Window>,
) {
    let window = windows.single().unwrap();

    // Update time and resolution
    shader_uniforms.buffer.time = time.elapsed_secs();
    shader_uniforms.buffer.resolution = [window.width(), window.height()];

    // Update mouse position (normalized)
    if let Some(cursor_pos) = window.cursor_position() {
        shader_uniforms.buffer.mouse = [
            cursor_pos.x / window.width(),
            1.0 - (cursor_pos.y / window.height()), // Flip Y coordinate
        ];
    }

    // Update audio data
    if let Some(audio) = audio_data {
        shader_uniforms.buffer.audio_bass = audio.bass_level;
        shader_uniforms.buffer.audio_mid = audio.mid_level;
        shader_uniforms.buffer.audio_treble = audio.treble_level;
        shader_uniforms.buffer.audio_volume = audio.volume;
    }

    // Update MIDI data
    if let Some(midi) = midi_controller {
        if let Some(cc1) = midi.current_values.get("cc1") {
            shader_uniforms.buffer.midi_cc1 = *cc1;
        }
        if let Some(cc2) = midi.current_values.get("cc2") {
            shader_uniforms.buffer.midi_cc2 = *cc2;
        }
        if let Some(cc3) = midi.current_values.get("cc3") {
            shader_uniforms.buffer.midi_cc3 = *cc3;
        }
        if let Some(cc4) = midi.current_values.get("cc4") {
            shader_uniforms.buffer.midi_cc4 = *cc4;
        }
    }
}

/// System to spawn fractal shader entities
pub fn spawn_fractal_shader(
    mut commands: Commands,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<StandardMaterial>>,
) {
    // Create a quad mesh for rendering the shader
    let mesh = meshes.add(Mesh::from(bevy::math::primitives::Rectangle::new(2.0, 2.0)));

    // Create a basic material (in a real implementation, this would use custom shaders)
    let material = materials.add(StandardMaterial {
        base_color: Color::srgb(1.0, 1.0, 1.0),
        ..default()
    });

    commands.spawn((
        Mesh3d(mesh),
        MeshMaterial3d(material),
        FractalShader {
            shader_name: "dark_fractal".to_string(),
            parameters: HashMap::from([
                ("speed".to_string(), 1.0),
                ("zoom".to_string(), 1.0),
                ("iterations".to_string(), 100.0),
            ]),
        },
    ));
}

/// Plugin for fractal shader rendering
pub struct FractalShaderPlugin;

impl Plugin for FractalShaderPlugin {
    fn build(&self, app: &mut App) {
        app
            .insert_resource(ShaderMaterials {
                materials: HashMap::new(),
            })
            .insert_resource(ShaderUniforms {
                buffer: FractalUniforms {
                    time: 0.0,
                    resolution: [800.0, 600.0],
                    mouse: [0.0, 0.0],
                    zoom: 1.0,
                    iterations: 100.0,
                    speed: 1.0,
                    brightness: 1.0,
                    contrast: 1.0,
                    saturation: 1.0,
                    hue_shift: 0.0,
                    audio_bass: 0.0,
                    audio_mid: 0.0,
                    audio_treble: 0.0,
                    audio_volume: 0.0,
                    midi_cc1: 0.0,
                    midi_cc2: 0.0,
                    midi_cc3: 0.0,
                    midi_cc4: 0.0,
                },
            })
            .add_systems(Startup, spawn_fractal_shader)
            .add_systems(Update, update_fractal_shaders);
    }
}