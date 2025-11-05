//! GUI module for the Fractal Shader Studio
//!
//! This module provides the Bevy-based GUI implementation using bevy_egui
//! to replace the previous eframe/egui implementation.
//!
//! Workaround for Bevy 0.17 + bevy_egui sudden exit issue (GitHub issue #21426):
//! - Camera state is tracked to prevent QueryDoesNotMatch panics
//! - Window focus events are handled to maintain camera entities
//! - Defensive error handling is used to prevent crashes
//! - Camera cleanup is performed on application exit

use bevy::prelude::*;
use bevy::render::renderer::{RenderDevice, RenderQueue};
use bevy::window::{WindowResolution, WindowFocused};
use bevy_egui::{EguiPlugin, EguiContexts};
use fractal_generator_lib::ui::FractalStudioApp;
use fractal_generator_lib::audio::AudioMidiSystem;
use fractal_generator_lib::osc::OscSystem;
use fractal_generator_lib::gesture::GestureController;

// Resource to hold the main application state
#[derive(Resource)]
pub struct FractalStudioAppState {
    pub app: FractalStudioApp,
}

/// Resource for the audio MIDI system
#[derive(Resource)]
pub struct AudioMidiResource {
    pub system: AudioMidiSystem,
}

/// Resource for the OSC system
#[derive(Resource)]
pub struct OscResource {
    pub system: OscSystem,
}

/// Resource for the gesture controller
#[derive(Resource)]
pub struct GestureResource {
    pub controller: GestureController,
}

/// Resource to track camera state
#[derive(Resource)]
pub struct CameraState {
    pub entity: Option<Entity>,
    pub is_focused: bool,
}

impl Default for FractalStudioAppState {
    fn default() -> Self {
        Self {
            app: FractalStudioApp::new(),
        }
    }
}

impl Default for AudioMidiResource {
    fn default() -> Self {
        Self {
            system: AudioMidiSystem::new(),
        }
    }
}

impl Default for OscResource {
    fn default() -> Self {
        Self {
            system: OscSystem::new(),
        }
    }
}

impl Default for GestureResource {
    fn default() -> Self {
        Self {
            controller: GestureController::new(),
        }
    }
}

impl Default for CameraState {
    fn default() -> Self {
        Self {
            entity: None,
            is_focused: true,
        }
    }
}

/// Plugin for the fractal studio GUI
pub struct FractalStudioGuiPlugin;

impl Plugin for FractalStudioGuiPlugin {
    fn build(&self, app: &mut App) {
        app
            .init_resource::<FractalStudioAppState>()
            .init_resource::<AudioMidiResource>()
            .init_resource::<OscResource>()
            .init_resource::<GestureResource>()
            .init_resource::<CameraState>()
            .add_systems(Startup, setup)
            .add_systems(Update, update)
            .add_systems(Update, handle_window_focus)
            .add_systems(PostUpdate, maintain_camera)
            .add_systems(Last, cleanup_camera_on_exit)
            .add_systems(FixedUpdate, validate_camera_state);
    }
}

/// Run the GUI application using Bevy and bevy_egui
pub fn run_gui() -> Result<(), Box<dyn std::error::Error>> {
    println!("Starting Bevy application with bevy_egui...");
    
    // Set up panic hook specifically for the GUI thread
    std::panic::set_hook(Box::new(|panic_info| {
        eprintln!("GUI Thread panicked: {}", panic_info);
        eprintln!("This might be related to the known Bevy 0.17 + bevy_egui focus issue.");
        eprintln!("Attempting to save current state before exit...");
        // In a real implementation, you might want to save the current state here
    }));
    
    let result = std::panic::catch_unwind(|| {
        App::new()
            .add_plugins(DefaultPlugins.set(WindowPlugin {
                primary_window: Some(Window {
                    title: "Fractal Shader Studio".to_string(),
                    resolution: WindowResolution::new(1400, 900),
                    ..default()
                }),
                ..default()
            }))
            .add_plugins(EguiPlugin::default())
            .add_plugins(FractalStudioGuiPlugin)
            .run();
    });
    
    match result {
        Ok(()) => {
            println!("Bevy application exited normally");
            Ok(())
        },
        Err(panic_info) => {
            eprintln!("Bevy application panicked: {:?}", panic_info);
            eprintln!("This is likely the known Bevy 0.17 + bevy_egui focus issue.");
            eprintln!("The application may have exited when the window lost/gained focus.");
            Err("GUI application panicked".into())
        }
    }
}

fn setup(
    mut commands: Commands,
    mut app_state: ResMut<FractalStudioAppState>,
    _audio_midi: ResMut<AudioMidiResource>,
    mut osc_resource: ResMut<OscResource>,
    mut gesture_resource: ResMut<GestureResource>,
    mut camera_state: ResMut<CameraState>,
    render_device: Option<Res<RenderDevice>>,
    render_queue: Option<Res<RenderQueue>>,
) {
    // Add a camera for bevy_egui to work with proper components
    let camera_entity = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        commands.spawn((
            Camera2d,
            Camera {
                order: -1, // Render before other cameras
                ..default()
            },
            Name::new("FractalStudioCamera"),
        )).id()
    })) {
        Ok(entity) => entity,
        Err(e) => {
            log::error!("Failed to spawn camera entity: {:?}", e);
            // Try to continue without camera, but this will likely cause issues
            return;
        }
    };
    
    camera_state.entity = Some(camera_entity);

    // Initialize WGPU if available
    if let (Some(device), Some(queue)) = (render_device, render_queue) {
        if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            app_state.app.initialize_wgpu(
                device.clone().into(),
                queue.clone().into(),
                1400, // Default width
                900   // Default height
            );
        })) {
            log::error!("WGPU initialization panicked: {:?}", e);
        }
    }
    
    // Start OSC server on port 8000
    if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        osc_resource.system.start_server(8000)
    })) {
        log::error!("OSC server initialization panicked: {:?}", e);
    } else {
        println!("OSC server started on port 8000");
    }
    
    // Initialize gesture controller
    if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        gesture_resource.controller.init_leap_motion()
    })) {
        log::warn!("Leap Motion initialization panicked: {:?}", e);
    }
    
    if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        gesture_resource.controller.init_mediapipe()
    })) {
        log::warn!("MediaPipe initialization panicked: {:?}", e);
    }
    
    println!("Bevy setup completed");
}

fn update(
    mut egui_context: EguiContexts,
    mut app_state: ResMut<FractalStudioAppState>,
    audio_midi: Res<AudioMidiResource>,
    mut osc_resource: ResMut<OscResource>,
    gesture_resource: Res<GestureResource>,
) {
    // Get the egui context
    match egui_context.ctx_mut() {
        Ok(ctx) => {
            // Process OSC messages
            if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                osc_resource.system.process_messages();
            })) {
                log::error!("OSC processing panicked: {:?}", e);
            }
            
            // Get current audio data
            let audio_data = audio_midi.system.get_audio_data();
            
            // Update the main application with all control data
            // Wrap in panic catch to prevent crashes
            if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                app_state.app.update(ctx, Some(&audio_data), Some(&audio_midi.system.midi_controller), Some(&osc_resource.system.controller), Some(&gesture_resource.controller));
            })) {
                log::error!("Application update panicked: {:?}", e);
            }
        },
        Err(e) => {
            // Log the error but don't panic - this can happen during window focus changes
            log::warn!("Failed to get egui context: {}", e);
        }
    }
}

/// Handle window focus events to prevent camera-related panics
fn handle_window_focus(
    mut focus_events: MessageReader<WindowFocused>,
    mut camera_state: ResMut<CameraState>,
    mut commands: Commands,
) {
    for event in focus_events.read() {
        camera_state.is_focused = event.focused;
        
        // If window lost focus, we don't need to do anything special
        // If window gained focus back, ensure camera exists
        if event.focused && camera_state.entity.is_none() {
            // Recreate camera if it was lost
            let camera_entity = commands.spawn((
                Camera2d,
                Camera {
                    order: -1,
                    ..default()
                },
                Name::new("FractalStudioCamera"),
            )).id();
            
            camera_state.entity = Some(camera_entity);
        }
    }
}

/// Cleanup camera on exit to prevent issues
fn cleanup_camera_on_exit(
    mut exit_events: MessageReader<AppExit>,
    camera_state: Res<CameraState>,
    mut commands: Commands,
) {
    for _ in exit_events.read() {
        // Clean up camera entity if it exists
        if let Some(entity) = camera_state.entity {
            if commands.get_entity(entity).is_ok() {
                if let Err(e) = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                    commands.entity(entity).despawn();
                })) {
                    log::error!("Failed to despawn camera entity: {:?}", e);
                }
            }
        }
    }
}

/// Maintain camera entity to prevent issues with bevy_egui
fn maintain_camera(
    mut camera_state: ResMut<CameraState>,
    mut commands: Commands,
    query: Query<Entity, With<Camera2d>>,
) {
    // If we don't have a camera entity stored but there are Camera2d entities,
    // update our stored entity
    if camera_state.entity.is_none() {
        for entity in query.iter() {
            camera_state.entity = Some(entity);
            break;
        }
    }
    
    // If our stored camera entity no longer exists, clear it
    if let Some(entity) = camera_state.entity {
        if commands.get_entity(entity).is_err() {
            camera_state.entity = None;
        }
    }
}

/// Validate camera state periodically to ensure it's still valid
fn validate_camera_state(
    mut camera_state: ResMut<CameraState>,
    mut commands: Commands,
    query: Query<Entity, With<Camera2d>>,
) {
    // Periodically check that our camera entity is still valid
    if let Some(entity) = camera_state.entity {
        if commands.get_entity(entity).is_err() {
            log::warn!("Camera entity {:?} is no longer valid, clearing state", entity);
            camera_state.entity = None;
        }
    }
    
    // If we don't have a camera but should, try to find one
    if camera_state.entity.is_none() {
        for entity in query.iter() {
            log::info!("Found Camera2d entity {:?}, updating camera state", entity);
            camera_state.entity = Some(entity);
            break;
        }
    }
}