use bevy::prelude::*;
use rust_fractal_shader_engine::NodeEditorPlugin;

fn main() {
    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                title: "Fractal Node Editor - Demo".to_string(),
                resolution: (1600.0, 900.0).into(),
                ..default()
            }),
            ..default()
        }))
        .add_plugins(NodeEditorPlugin)
        .run();
}