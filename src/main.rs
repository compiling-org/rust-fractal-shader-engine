use bevy::prelude::*;
use rust_fractal_shader_engine::{RustFractalShaderEngine, ShaderConverter, ShaderFormat};

mod shader_renderer;
mod audio;
mod ui;

use audio::AudioMidiSystem;
use ui::UiState;

fn main() {
    App::new()
        .add_plugins(DefaultPlugins)
        .add_plugins(shader_renderer::FractalShaderPlugin)
        .add_plugins(ui::FractalUiPlugin)
        .insert_resource(AudioMidiSystem::new())
        .insert_resource(UiState::default())
        .add_systems(Startup, setup)
        .add_systems(Update, update_audio_midi)
        .run();
}

fn setup(mut commands: Commands) {
    // Create the fractal shader engine
    let mut engine = RustFractalShaderEngine::new();

    // Load ISF shaders from the assets directory
    if let Ok(loaded_shaders) = engine.load_isf_shaders_from_directory("assets/shaders/isf") {
        println!("Loaded {} ISF shaders: {:?}", loaded_shaders.len(), loaded_shaders);
    }

    // Example: Convert an ISF shader to WGSL
    let sample_isf = r#"
/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
  ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;
    vec3 color = vec3(sin(t), cos(t), sin(t * 0.5));
    gl_FragColor = vec4(color, 1.0);
}
"#;

    match ShaderConverter::isf_to_wgsl(sample_isf) {
        Ok(wgsl) => println!("Converted ISF to WGSL:\n{}", wgsl),
        Err(e) => println!("Conversion failed: {}", e),
    }

    // Spawn a camera for the fractal rendering
    commands.spawn(Camera2d::default());
}

/// Update audio and MIDI systems
fn update_audio_midi(
    time: Res<Time>,
    mut audio_midi: ResMut<AudioMidiSystem>,
    mut ui_state: ResMut<UiState>,
) {
    // Generate some test audio data (in a real app, this would come from audio input)
    let sample_rate = 44100.0;
    let num_samples = (sample_rate * time.delta_secs()) as usize;
    let mut samples = Vec::with_capacity(num_samples);

    for i in 0..num_samples {
        let t = time.elapsed_secs() + i as f32 / sample_rate;
        // Generate a test signal with some harmonics
        let signal = (t * 440.0 * 2.0 * std::f32::consts::PI).sin() * 0.3
                   + (t * 880.0 * 2.0 * std::f32::consts::PI).sin() * 0.2
                   + (t * 1320.0 * 2.0 * std::f32::consts::PI).sin() * 0.1;
        samples.push(signal);
    }

    // Process audio
    audio_midi.update_audio(&samples);

    // Simulate some MIDI input (in a real app, this would come from MIDI devices)
    if time.elapsed_secs() % 2.0 < time.delta_secs() {
        audio_midi.midi_controller.process_midi_message(0, 1, 64); // Mod wheel
    }

    // Update UI state with audio/MIDI parameters
    let audio_data = audio_midi.get_audio_data();
    let speed_param = audio_midi.get_parameter("speed");

    // Update parameter values based on audio/MIDI
    if let Some(speed) = ui_state.parameter_values.get_mut("speed") {
        *speed = speed_param;
    }
    if let Some(brightness) = ui_state.parameter_values.get_mut("brightness") {
        *brightness = audio_data.volume * 20.0;
    }
}