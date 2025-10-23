use bevy::prelude::*;
use bevy::window::PrimaryWindow;
use std::collections::HashMap;

/// UI state for the fractal shader application
#[derive(Resource, Debug, Clone)]
pub struct UiState {
    pub selected_shader: String,
    pub shader_list: Vec<String>,
    pub parameter_values: HashMap<String, f32>,
    pub show_audio_panel: bool,
    pub show_midi_panel: bool,
    pub show_shader_panel: bool,
}

impl Default for UiState {
    fn default() -> Self {
        Self {
            selected_shader: "dark_fractal".to_string(),
            shader_list: vec![
                "dark_fractal".to_string(),
                "mandelbrot".to_string(),
                "julia".to_string(),
            ],
            parameter_values: HashMap::from([
                ("speed".to_string(), 1.0),
                ("zoom".to_string(), 1.0),
                ("iterations".to_string(), 100.0),
                ("brightness".to_string(), 1.0),
                ("contrast".to_string(), 1.0),
                ("saturation".to_string(), 1.0),
            ]),
            show_audio_panel: false,
            show_midi_panel: false,
            show_shader_panel: true,
        }
    }
}

/// UI Plugin for the fractal shader application
pub struct FractalUiPlugin;

impl Plugin for FractalUiPlugin {
    fn build(&self, app: &mut App) {
        app
            .init_resource::<UiState>()
            .add_systems(Startup, setup_ui)
            .add_systems(Update, (
                update_ui,
                handle_ui_interactions,
            ));
    }
}

/// Setup the UI
fn setup_ui(mut commands: Commands, asset_server: Res<AssetServer>) {
    // Camera for UI
    commands.spawn(Camera2dBundle::default());

    // Root UI node
    commands.spawn(NodeBundle {
        style: Style {
            width: Val::Percent(100.0),
            height: Val::Percent(100.0),
            flex_direction: FlexDirection::Column,
            ..default()
        },
        ..default()
    }).with_children(|parent| {
        // Top bar
        parent.spawn(NodeBundle {
            style: Style {
                width: Val::Percent(100.0),
                height: Val::Px(50.0),
                flex_direction: FlexDirection::Row,
                align_items: AlignItems::Center,
                padding: UiRect::all(Val::Px(10.0)),
                ..default()
            },
            background_color: Color::rgba(0.1, 0.1, 0.1, 0.8).into(),
            ..default()
        }).with_children(|parent| {
            // Title
            parent.spawn(TextBundle::from_section(
                "Rust Fractal Shader Engine",
                TextStyle {
                    font_size: 24.0,
                    color: Color::WHITE,
                    ..Default::default()
                },
            ));

            // Control buttons
            parent.spawn(NodeBundle {
                style: Style {
                    flex_direction: FlexDirection::Row,
                    margin: UiRect::left(Val::Auto),
                    ..default()
                },
                ..default()
            }).with_children(|parent| {
                spawn_button(parent, "Shaders", UiButton::ToggleShaderPanel);
                spawn_button(parent, "Audio", UiButton::ToggleAudioPanel);
                spawn_button(parent, "MIDI", UiButton::ToggleMidiPanel);
            });
        });

        // Main content area
        parent.spawn(NodeBundle {
            style: Style {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Row,
                ..default()
            },
            ..default()
        }).with_children(|parent| {
            // Left panel (parameters)
            parent.spawn(NodeBundle {
                style: Style {
                    width: Val::Px(300.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    padding: UiRect::all(Val::Px(10.0)),
                    ..default()
                },
                background_color: Color::rgba(0.15, 0.15, 0.15, 0.9).into(),
                ..default()
            }).with_children(|parent| {
                // Shader selector
                parent.spawn(NodeBundle {
                    style: Style {
                        width: Val::Percent(100.0),
                        height: Val::Px(200.0),
                        flex_direction: FlexDirection::Column,
                        margin: UiRect::bottom(Val::Px(20.0)),
                        ..default()
                    },
                    ..default()
                }).with_children(|parent| {
                    parent.spawn(TextBundle::from_section(
                        "Shader Selection",
                        TextStyle {
                            font_size: 18.0,
                            color: Color::WHITE,
                            ..default()
                        },
                    ));

                    // Shader list would be populated dynamically
                    parent.spawn(NodeBundle {
                        style: Style {
                            width: Val::Percent(100.0),
                            height: Val::Percent(100.0),
                            flex_direction: FlexDirection::Column,
                            ..default()
                        },
                        ..default()
                    });
                });

                // Parameters panel
                parent.spawn(NodeBundle {
                    style: Style {
                        width: Val::Percent(100.0),
                        height: Val::Percent(100.0),
                        flex_direction: FlexDirection::Column,
                        ..default()
                    },
                    ..default()
                }).with_children(|parent| {
                    parent.spawn(TextBundle::from_section(
                        "Parameters",
                        TextStyle {
                            font_size: 18.0,
                            color: Color::WHITE,
                            ..default()
                        },
                    ));

                    // Parameter sliders would be added here
                    spawn_parameter_slider(parent, "Speed", 0.1, 5.0, 1.0);
                    spawn_parameter_slider(parent, "Zoom", 0.2, 5.0, 1.0);
                    spawn_parameter_slider(parent, "Iterations", 10.0, 200.0, 100.0);
                    spawn_parameter_slider(parent, "Brightness", 0.0, 20.0, 1.0);
                    spawn_parameter_slider(parent, "Contrast", 0.0, 3.0, 1.0);
                    spawn_parameter_slider(parent, "Saturation", 0.0, 2.0, 1.0);
                });
            });

            // Right panels (audio/midi)
            parent.spawn(NodeBundle {
                style: Style {
                    width: Val::Px(300.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    ..default()
                },
                ..default()
            });
        });
    });
}

/// Spawn a button
fn spawn_button(parent: &mut ChildBuilder, text: &str, button_type: UiButton) {
    parent.spawn((
        ButtonBundle {
            style: Style {
                width: Val::Px(80.0),
                height: Val::Px(30.0),
                margin: UiRect::horizontal(Val::Px(5.0)),
                justify_content: JustifyContent::Center,
                align_items: AlignItems::Center,
                ..default()
            },
            background_color: Color::rgb(0.3, 0.3, 0.3).into(),
            ..default()
        },
        button_type,
    )).with_children(|parent| {
        parent.spawn(TextBundle::from_section(
            text,
            TextStyle {
                font_size: 14.0,
                color: Color::WHITE,
                ..Default::default()
            },
        ));
    });
}

/// Spawn a parameter slider
fn spawn_parameter_slider(
    parent: &mut ChildBuilder,
    name: &str,
    min: f32,
    max: f32,
    default: f32
) {
    parent.spawn(NodeBundle {
        style: Style {
            width: Val::Percent(100.0),
            height: Val::Px(50.0),
            flex_direction: FlexDirection::Column,
            margin: UiRect::bottom(Val::Px(10.0)),
            ..Default::default()
        },
        ..Default::default()
    }).with_children(|parent| {
        // Label
        parent.spawn(TextBundle::from_section(
            name,
            TextStyle {
                font_size: 14.0,
                color: Color::WHITE,
                ..Default::default()
            },
        ));

        // Slider
        parent.spawn((
            NodeBundle {
                style: Style {
                    width: Val::Percent(100.0),
                    height: Val::Px(20.0),
                    ..Default::default()
                },
                background_color: Color::rgb(0.5, 0.5, 0.5).into(),
                ..Default::default()
            },
            ParameterSlider {
                name: name.to_string(),
                min,
                max,
                value: default,
            },
        ));
    });
}

/// UI Button types
#[derive(Component)]
pub enum UiButton {
    ToggleShaderPanel,
    ToggleAudioPanel,
    ToggleMidiPanel,
}

/// Parameter slider component
#[derive(Component)]
pub struct ParameterSlider {
    pub name: String,
    pub min: f32,
    pub max: f32,
    pub value: f32,
}

/// Update UI elements
fn update_ui(
    ui_state: Res<UiState>,
    mut text_query: Query<&mut Text>,
    mut slider_query: Query<(&mut ParameterSlider, &mut BackgroundColor)>,
) {
    // Update parameter sliders
    for (mut slider, mut color) in slider_query.iter_mut() {
        if let Some(&value) = ui_state.parameter_values.get(&slider.name) {
            slider.value = value;
            // Update slider visual (simplified)
            let normalized = (value - slider.min) / (slider.max - slider.min);
            *color = Color::rgb(0.3 + normalized * 0.4, 0.3, 0.3).into();
        }
    }
}

/// Handle UI interactions
fn handle_ui_interactions(
    mut interaction_query: Query<
        (&Interaction, &UiButton),
        (Changed<Interaction>, With<Button>),
    >,
    mut ui_state: ResMut<UiState>,
) {
    for (interaction, button_type) in &mut interaction_query {
        if *interaction == Interaction::Pressed {
            match button_type {
                UiButton::ToggleShaderPanel => {
                    ui_state.show_shader_panel = !ui_state.show_shader_panel;
                }
                UiButton::ToggleAudioPanel => {
                    ui_state.show_audio_panel = !ui_state.show_audio_panel;
                }
                UiButton::ToggleMidiPanel => {
                    ui_state.show_midi_panel = !ui_state.show_midi_panel;
                }
            }
        }
    }
}

/// Audio visualization component
#[derive(Component)]
pub struct AudioVisualizer {
    pub spectrum_bars: Vec<Entity>,
}

/// MIDI mapping display component
#[derive(Component)]
pub struct MidiMappingDisplay;

/// System to update audio visualization
pub fn update_audio_visualization(
    audio_midi: Res<crate::audio::AudioMidiSystem>,
    mut query: Query<&mut AudioVisualizer>,
    mut commands: Commands,
) {
    let audio_data = audio_midi.get_audio_data();
    // Update audio visualization bars based on spectrum data
    // This would create/update visual bars representing the audio spectrum
}

/// System to update MIDI mappings display
pub fn update_midi_display(
    audio_midi: Res<crate::audio::AudioMidiSystem>,
    mut query: Query<&mut MidiMappingDisplay>,
) {
    // Update MIDI mapping display
    // This would show current MIDI controller mappings and values
}