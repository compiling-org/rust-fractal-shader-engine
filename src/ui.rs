use bevy::prelude::*;
use bevy::ui::{Node, Val, JustifyContent, AlignItems, FlexDirection, UiRect, BackgroundColor};
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
    commands.spawn(Camera2d::default());

    // Root UI node
    commands.spawn((
        Node {
            width: Val::Percent(100.0),
            height: Val::Percent(100.0),
            flex_direction: FlexDirection::Column,
            ..default()
        },
    )).with_children(|parent| {
        // Top bar
        parent.spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Px(50.0),
                flex_direction: FlexDirection::Row,
                align_items: AlignItems::Center,
                padding: UiRect::all(Val::Px(10.0)),
                ..default()
            },
            BackgroundColor(Color::srgba(0.1, 0.1, 0.1, 0.8)),
        )).with_children(|parent| {
            // Title
            parent.spawn((
                Text::new("Rust Fractal Shader Engine"),
                TextFont {
                    font_size: 24.0,
                    ..default()
                },
                TextColor(Color::srgb(1.0, 1.0, 1.0)),
            ));

            // Control buttons
            parent.spawn((
                Node {
                    flex_direction: FlexDirection::Row,
                    margin: UiRect::left(Val::Auto),
                    ..default()
                },
            )).with_children(|parent| {
                // Shaders button
                parent.spawn((
                    Button,
                    Node {
                        width: Val::Px(80.0),
                        height: Val::Px(30.0),
                        margin: UiRect::horizontal(Val::Px(5.0)),
                        justify_content: JustifyContent::Center,
                        align_items: AlignItems::Center,
                        ..default()
                    },
                    BackgroundColor(Color::srgb(0.3, 0.3, 0.3)),
                    UiButton::ToggleShaderPanel,
                )).with_children(|parent| {
                    parent.spawn((
                        Text::new("Shaders"),
                        TextFont {
                            font_size: 14.0,
                            ..default()
                        },
                        TextColor(Color::WHITE),
                    ));
                });

                // Audio button
                parent.spawn((
                    Button,
                    Node {
                        width: Val::Px(80.0),
                        height: Val::Px(30.0),
                        margin: UiRect::horizontal(Val::Px(5.0)),
                        justify_content: JustifyContent::Center,
                        align_items: AlignItems::Center,
                        ..default()
                    },
                    BackgroundColor(Color::srgb(0.3, 0.3, 0.3)),
                    UiButton::ToggleAudioPanel,
                )).with_children(|parent| {
                    parent.spawn((
                        Text::new("Audio"),
                        TextFont {
                            font_size: 14.0,
                            ..default()
                        },
                        TextColor(Color::WHITE),
                    ));
                });

                // MIDI button
                parent.spawn((
                    Button,
                    Node {
                        width: Val::Px(80.0),
                        height: Val::Px(30.0),
                        margin: UiRect::horizontal(Val::Px(5.0)),
                        justify_content: JustifyContent::Center,
                        align_items: AlignItems::Center,
                        ..default()
                    },
                    BackgroundColor(Color::srgb(0.3, 0.3, 0.3)),
                    UiButton::ToggleMidiPanel,
                )).with_children(|parent| {
                    parent.spawn((
                        Text::new("MIDI"),
                        TextFont {
                            font_size: 14.0,
                            ..default()
                        },
                        TextColor(Color::WHITE),
                    ));
                });
            });
        });

        // Main content area
        parent.spawn((
            Node {
                width: Val::Percent(100.0),
                height: Val::Percent(100.0),
                flex_direction: FlexDirection::Row,
                ..default()
            },
        )).with_children(|parent| {
            // Left panel (parameters)
            parent.spawn((
                Node {
                    width: Val::Px(300.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    padding: UiRect::all(Val::Px(10.0)),
                    ..default()
                },
                BackgroundColor(Color::srgba(0.15, 0.15, 0.15, 0.9)),
            )).with_children(|parent| {
                // Shader selector
                parent.spawn((
                    Node {
                        width: Val::Percent(100.0),
                        height: Val::Px(200.0),
                        flex_direction: FlexDirection::Column,
                        margin: UiRect::bottom(Val::Px(20.0)),
                        ..default()
                    },
                )).with_children(|parent| {
                    parent.spawn((
                        Text::new("Shader Selection"),
                        TextFont {
                            font_size: 18.0,
                            ..default()
                        },
                        TextColor(Color::WHITE),
                    ));

                    // Shader list would be populated dynamically
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Percent(100.0),
                            flex_direction: FlexDirection::Column,
                            ..default()
                        },
                    ));
                });

                // Parameters panel
                parent.spawn((
                    Node {
                        width: Val::Percent(100.0),
                        height: Val::Percent(100.0),
                        flex_direction: FlexDirection::Column,
                        ..default()
                    },
                )).with_children(|parent| {
                    parent.spawn((
                        Text::new("Parameters"),
                        TextFont {
                            font_size: 18.0,
                            ..Default::default()
                        },
                        TextColor(Color::WHITE),
                    ));

                    // Parameter sliders would be added here
                    // Speed slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Speed"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Speed".to_string(),
                                min: 0.1,
                                max: 5.0,
                                value: 1.0,
                            },
                        ));
                    });

                    // Zoom slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Zoom"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Zoom".to_string(),
                                min: 0.2,
                                max: 5.0,
                                value: 1.0,
                            },
                        ));
                    });

                    // Iterations slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Iterations"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Iterations".to_string(),
                                min: 10.0,
                                max: 200.0,
                                value: 100.0,
                            },
                        ));
                    });

                    // Brightness slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Brightness"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Brightness".to_string(),
                                min: 0.0,
                                max: 20.0,
                                value: 1.0,
                            },
                        ));
                    });

                    // Contrast slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Contrast"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Contrast".to_string(),
                                min: 0.0,
                                max: 3.0,
                                value: 1.0,
                            },
                        ));
                    });

                    // Saturation slider
                    parent.spawn((
                        Node {
                            width: Val::Percent(100.0),
                            height: Val::Px(50.0),
                            flex_direction: FlexDirection::Column,
                            margin: UiRect::bottom(Val::Px(10.0)),
                            ..default()
                        },
                    )).with_children(|parent| {
                        parent.spawn((
                            Text::new("Saturation"),
                            TextFont {
                                font_size: 14.0,
                                ..default()
                            },
                            TextColor(Color::WHITE),
                        ));
                        parent.spawn((
                            Node {
                                width: Val::Percent(100.0),
                                height: Val::Px(20.0),
                                ..default()
                            },
                            BackgroundColor(Color::srgb(0.5, 0.5, 0.5)),
                            ParameterSlider {
                                name: "Saturation".to_string(),
                                min: 0.0,
                                max: 2.0,
                                value: 1.0,
                            },
                        ));
                    });
                });
            });

            // Right panels (audio/midi)
            parent.spawn((
                Node {
                    width: Val::Px(300.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    ..default()
                },
            ));
        });
    });
}

// Button spawning function removed - now inlined

// Parameter slider spawning function removed - now inlined

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
            *color = BackgroundColor(Color::srgb(0.3 + normalized * 0.4, 0.3, 0.3));
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