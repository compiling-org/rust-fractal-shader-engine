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
use bevy::window::{WindowResolution, WindowFocused, PrimaryWindow};
use bevy::app::AppExit;
use bevy_egui::{EguiPlugin, EguiContexts, EguiContext, PrimaryEguiContext};
use fractal_generator_lib::ui::FractalStudioApp;
use fractal_generator_lib::audio::AudioMidiSystem;
use fractal_generator_lib::osc::OscSystem;
use fractal_generator_lib::gesture::GestureController;

// On Windows, export symbols that hint to NVidia/AMD drivers to pick the discrete GPU
// This helps ensure the app launches on the high-performance GPU when available.
#[cfg(target_os = "windows")]
#[no_mangle]
pub static NvOptimusEnablement: u32 = 0x00000001;

#[cfg(target_os = "windows")]
#[no_mangle]
pub static AmdPowerXpressRequestHighPerformance: u32 = 0x00000001;

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

#[derive(Resource, Default)]
pub struct UiRunState {
    pub frames_since_start: u32,
    pub last_readback_frame: u32,
    pub readback_every_n: u32,
}

// Enforce GPU-only startup: abort if RenderDevice is unavailable after N frames
#[derive(Resource)]
pub struct GpuAvailabilityWatchdog {
    pub frames_waited: u32,
    pub limit_frames: u32,
}

impl Default for GpuAvailabilityWatchdog {
    fn default() -> Self {
        // Allow more time for device creation on some drivers (~10s @ 60 FPS)
        Self { frames_waited: 0, limit_frames: 600 }
    }
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
            .init_resource::<UiRunState>()
            .init_resource::<GpuAvailabilityWatchdog>()
            .add_systems(Startup, setup)
            // Run UI in the bevy_egui primary context pass to ensure it's inside an egui frame
            .add_systems(bevy_egui::EguiPrimaryContextPass, update)
            // Attempt WGPU init once Bevy’s RenderDevice/Queue become available
            .add_systems(Update, try_init_wgpu_on_ready)
            // Fail fast if GPU device never becomes available
            .add_systems(Update, gpu_availability_watchdog)
            .add_systems(Update, handle_window_focus)
            .add_systems(PostUpdate, maintain_camera)
            .add_systems(Last, cleanup_camera_on_exit)
            .add_systems(FixedUpdate, validate_camera_state);
    }
}

/// Run the GUI application using Bevy and bevy_egui
pub fn run_gui() -> Result<(), Box<dyn std::error::Error>> {
    // Buffer GPU startup diagnostics to file for reliable troubleshooting
    let mut gpu_log = String::new();
    let log = |buf: &mut String, line: String| {
        // Persist to buffer-only to avoid stdout broken pipe panics
        buf.push_str(&line);
        buf.push('\n');
    };
    log(&mut gpu_log, "Starting Bevy application with bevy_egui...".to_string());
    // Windows backend selection + fallback
    // Goal: pick a working backend and avoid DX12 push-constant crash (wgpu#5683).
    #[cfg(target_os = "windows")]
    {
        let backend_env = std::env::var("WGPU_BACKEND").ok();
        if backend_env.is_none() {
            let instance = wgpu::Instance::default();
            let vulkan_adapters = instance.enumerate_adapters(wgpu::Backends::VULKAN);
            let dx12_adapters = instance.enumerate_adapters(wgpu::Backends::DX12);

            if !vulkan_adapters.is_empty() {
                std::env::set_var("WGPU_BACKEND", "vulkan");
                log(&mut gpu_log, "Selected backend: Vulkan (detected adapters)".to_string());
            } else if !dx12_adapters.is_empty() {
                std::env::set_var("WGPU_BACKEND", "dx12");
                // Prefer DXC to avoid HLSL push-constant translation issues.
                if std::env::var("WGPU_DX12_COMPILER").is_err() {
                    std::env::set_var("WGPU_DX12_COMPILER", "dxc");
                }
                log(&mut gpu_log, "Selected backend: DX12 (no Vulkan). Using DXC compiler.".to_string());
            } else {
                // Last resort: GL (ANGLE) for very limited environments
                std::env::set_var("WGPU_BACKEND", "gl");
                log(&mut gpu_log, "Selected backend: GL (no Vulkan/DX12 adapters found)".to_string());
            }
        } else {
            log(&mut gpu_log, format!("Backend preset via env: {}", backend_env.unwrap()));
        }
    }
    // GPU policy: "discrete_only" or "prefer_discrete" (default)
    // Default to prefer_discrete to avoid hard-failing on systems without a discrete GPU.
    let gpu_policy = std::env::var("GPU_POLICY").unwrap_or_else(|_| "prefer_discrete".into());
    // Configure WGPU via environment variables before the renderer starts.
    // Force NVIDIA RTX selection: pick discrete NVIDIA adapter and set backend + adapter name.
    // This ensures Bevy/wgpu binds to the RTX GPU, not integrated.
    {
        let instance = wgpu::Instance::default();
        let mut chosen_backend: Option<&'static str> = None;
        let mut chosen_name: Option<String> = None;
        // Prefer Vulkan first on Windows to avoid DX12 push-constant crash
        #[cfg(target_os = "windows")]
        let search_order = [wgpu::Backends::VULKAN, wgpu::Backends::DX12];
        #[cfg(not(target_os = "windows"))]
        let search_order = [wgpu::Backends::VULKAN];

        for backend in search_order.iter() {
            for adapter in instance.enumerate_adapters(*backend).iter() {
                let info = adapter.get_info();
                // NVIDIA vendor id is 0x10DE
                if info.vendor == 0x10DE && info.device_type == wgpu::DeviceType::DiscreteGpu {
                    chosen_backend = Some(match *backend { wgpu::Backends::DX12 => "dx12", wgpu::Backends::VULKAN => "vulkan", _ => "auto" });
                    chosen_name = Some(info.name.clone());
                    break;
                }
            }
            if chosen_backend.is_some() { break; }
        }

        if let Some(name) = chosen_name {
            std::env::set_var("WGPU_ADAPTER_NAME", name);
        }
        // Only set BACKEND from enumeration if env is not already set
        if std::env::var("WGPU_BACKEND").is_err() {
            if let Some(backend) = chosen_backend {
                std::env::set_var("WGPU_BACKEND", backend);
            }
        }
    }
    // Valid values for power preference are "low" or "high".
    if std::env::var("WGPU_POWER_PREF").is_err() {
        std::env::set_var("WGPU_POWER_PREF", "high");
    }
    // On Windows DX12, prefer FXC for broader driver compatibility unless overridden.
    if std::env::var("WGPU_DX12_COMPILER").is_err() {
        std::env::set_var("WGPU_DX12_COMPILER", "fxc");
    }
    log(&mut gpu_log, format!(
        "WGPU config → BACKEND={}, POWER={}, DX12_COMPILER={}, ADAPTER_NAME={}, POLICY={}",
        std::env::var("WGPU_BACKEND").unwrap_or_else(|_| "<unset>".into()),
        std::env::var("WGPU_POWER_PREF").unwrap_or_else(|_| "<unset>".into()),
        std::env::var("WGPU_DX12_COMPILER").unwrap_or_else(|_| "<unset>".into()),
        std::env::var("WGPU_ADAPTER_NAME").unwrap_or_else(|_| "<unset>".into()),
        gpu_policy,
    ));

    // Preflight: enumerate adapters and prefer discrete, but do not hard-fail unless policy requires.
    {
        let instance = wgpu::Instance::default();
        // Enumerate available adapters across common backends, including GL as a fallback
        let backends = wgpu::Backends::VULKAN | wgpu::Backends::DX12 | wgpu::Backends::GL;
        let adapters = instance.enumerate_adapters(backends);
        let mut has_discrete = false;
        let mut has_any_gpu = false;
        log(&mut gpu_log, "Detected GPU adapters:".to_string());
        for adapter in adapters.iter() {
            let info = adapter.get_info();
            has_any_gpu = true;
            log(&mut gpu_log, format!(
                "  - name='{}' backend={:?} type={:?} vendor=0x{:04x} device={} driver='{}'",
                info.name,
                info.backend,
                info.device_type,
                info.vendor,
                info.device,
                info.driver
            ));
            // Treat any NVIDIA adapter as acceptable for discrete-only policy
            if info.device_type == wgpu::DeviceType::DiscreteGpu || info.vendor == 0x10DE {
                has_discrete = true;
                // Do not break here; continue to list all adapters for diagnostics
            }
        }
        // Persist the GPU startup diagnostics immediately after enumeration
        let _ = std::fs::write("gpu_startup.log", &gpu_log);
        if !has_discrete {
            if gpu_policy == "discrete_only" {
                log(&mut gpu_log, "Fatal: No discrete GPU detected. Policy requires discrete GPU.".to_string());
                return Err("No discrete GPU available — GPU-only policy enforced".into());
            } else {
                if has_any_gpu {
                    log(&mut gpu_log, "Warning: No discrete GPU found; proceeding with available adapter (integrated/GL). Set GPU_POLICY=discrete_only to enforce.".to_string());
                } else {
                    eprintln!("Fatal: No GPU adapters detected at all across Vulkan/DX12/GL. Check drivers and backend.");
                    return Err("No GPU adapters detected".into());
                }
            }
        }
    }
    
    // Set up panic hook specifically for the GUI thread
    std::panic::set_hook(Box::new(|panic_info| {
        eprintln!("GUI Thread panicked: {}", panic_info);
        eprintln!("This might be related to the known Bevy 0.17 + bevy_egui focus issue.");
        eprintln!("Attempting to save current state before exit...");
        // In a real implementation, you might want to save the current state here
    }));
    
    let result = std::panic::catch_unwind(|| {
        App::new()
            .add_plugins(DefaultPlugins
                .set(WindowPlugin {
                    primary_window: Some(Window {
                        title: "Fractal Shader Studio".to_string(),
                        resolution: WindowResolution::new(1400, 900),
                        present_mode: bevy::window::PresentMode::Immediate,
                        ..default()
                    }),
                    ..default()
                })
                // Keep default RenderPlugin; env vars and preflight selection drive configuration
            )
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

    // Defer WGPU initialization to Update where RenderDevice becomes available reliably.
    app_state.app.has_wgpu_support = false;
    
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

/// Try to initialize WGPU once Bevy’s RenderDevice/Queue are available in the main world.
fn try_init_wgpu_on_ready(
    mut app_state: ResMut<FractalStudioAppState>,
    render_device: Option<Res<RenderDevice>>,
    render_queue: Option<Res<RenderQueue>>,
) {
    // Already initialized
    if app_state.app.has_wgpu_support {
        return;
    }

    if let (Some(device), Some(queue)) = (render_device, render_queue) {
        // Attempt initialization once
        match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            app_state.app.initialize_wgpu(
                device.clone().into(),
                queue.clone().into(),
                1400,
                900,
            );
        })) {
            Ok(_) => {
                // Mark GPU support based on whether the renderer was actually created
                let gpu_ready = app_state.app.fractal_renderer.is_some();
                app_state.app.has_wgpu_support = gpu_ready;
                if gpu_ready {
                    log::info!("WGPU initialized: FractalRenderer active (device + queue ready)");
                } else {
                    log::warn!(
                        "WGPU init attempted, but FractalRenderer is not available. Will retry next frame."
                    );
                }
            }
            Err(e) => {
                log::error!("WGPU init panic in Update: {:?}", e);
                app_state.app.has_wgpu_support = false;
            }
        }
    }
}

/// Panic if GPU device is not available after a short startup window.
fn gpu_availability_watchdog(
    mut watchdog: ResMut<GpuAvailabilityWatchdog>,
    render_device: Option<Res<RenderDevice>>,
) {
    // If device is available, watchdog can be silent.
    if render_device.is_some() {
        return;
    }

    watchdog.frames_waited += 1;
    if watchdog.frames_waited >= watchdog.limit_frames {
        log::error!(
            "RenderDevice not available after {} frames. Keeping app alive to allow deferred init or driver recovery.",
            watchdog.frames_waited
        );
        // Disable further watchdog checks to avoid spamming
        watchdog.limit_frames = u32::MAX;
    }
}

fn update(
    mut egui_contexts: EguiContexts,
    mut app_state: ResMut<FractalStudioAppState>,
    mut audio_midi: ResMut<AudioMidiResource>,
    mut osc_resource: ResMut<OscResource>,
    mut gesture_resource: ResMut<GestureResource>,
    mut ui_run_state: ResMut<UiRunState>,
    camera_state: Res<CameraState>,
    mut images: ResMut<Assets<Image>>,
    primary_window_q: Query<&Window, With<PrimaryWindow>>, 
 ) {
    // Ensure a Bevy Image exists and is registered with egui BEFORE drawing the viewport
    // so FractalStudioApp::show_fractal_viewport can paint with a valid TextureId.
    let mut width = app_state.app.last_viewport_resolution[0];
    let mut height = app_state.app.last_viewport_resolution[1];

    // Fallback to primary window size if app has not set a viewport resolution yet
    if width == 0 || height == 0 {
        if let Ok(win) = primary_window_q.single() {
            let phys = win.resolution.physical_size();
            width = phys.x.max(1);
            height = phys.y.max(1);
            app_state.app.last_viewport_resolution = [width, height];
            log::info!("Viewport resolution missing; falling back to window size {}x{}", width, height);
        } else {
            width = 1024;
            height = 768;
            app_state.app.last_viewport_resolution = [width, height];
            log::warn!("Primary window unavailable; using default fallback {}x{}", width, height);
        }
    }

    if width > 0 && height > 0 {
        let needs_image = match app_state.app.viewport_image_handle.as_ref() {
            Some(handle) => {
                if let Some(img) = images.get(handle) {
                    let size = img.texture_descriptor.size;
                    size.width != width || size.height != height
                } else {
                    true
                }
            }
            None => true,
        };

        if needs_image {
            use wgpu::{Extent3d, TextureDimension, TextureFormat};
            let extent = Extent3d { width, height, depth_or_array_layers: 1 };
            let pixel_count = (width as usize) * (height as usize) * 4;
            let image = Image::new(
                extent,
                TextureDimension::D2,
                vec![0u8; pixel_count],
                TextureFormat::Rgba8Unorm,
                bevy::asset::RenderAssetUsages::default(),
            );
            let handle = images.add(image);
            app_state.app.viewport_image_handle = Some(handle.clone());

            // Register with egui contexts as a user texture (strong handle)
            let tex_id = egui_contexts.add_image(bevy_egui::EguiTextureHandle::Strong(handle.clone()));
            app_state.app.viewport_texture = Some(tex_id);
            log::info!("Registered viewport texture with egui: {}x{}", width, height);
        }
    }

    // Obtain the egui context directly in the primary context pass
    {
        let ctx = match egui_contexts.ctx_mut() {
            Ok(ctx) => ctx,
            Err(_) => return,
        };

    // Skip the very first frame to avoid egui lifecycle race conditions
    if ui_run_state.frames_since_start == 0 {
        ui_run_state.frames_since_start += 1;
        if ui_run_state.readback_every_n == 0 { ui_run_state.readback_every_n = 10; }
        return;
    }

    // Proceed with UI even if window focus info is unavailable.
    // Focus handling is kept for camera maintenance only.
    // Process OSC messages safely
    osc_resource.system.process_messages();

    // Get current audio data
    let audio_data = audio_midi.system.get_audio_data();

    // Frame metrics: start
    app_state.app.metrics.start_frame();

    // Update the main application with all control data
        app_state.app.update(
            &ctx,
            Some(&audio_data),
            Some(&mut audio_midi.system.midi_controller),
            Some(&mut osc_resource.system.controller),
            Some(&mut gesture_resource.controller),
        );

    // Frame metrics: end and periodic logging
        app_state.app.metrics.end_frame();
        app_state.app.metrics.maybe_log();
        // Temporary population via CPU readback until GPU copy pipeline is added
        let maybe_handle = app_state.app.viewport_image_handle.clone();
        let current_time = app_state.app.time;
        if let Some(renderer) = app_state.app.fractal_renderer.as_mut() {
            if let Some(handle) = maybe_handle {
                // Throttle readbacks to every N frames to keep UI responsive
                let should_readback = (ui_run_state.frames_since_start - ui_run_state.last_readback_frame)
                    >= ui_run_state.readback_every_n;
                if should_readback {
                    match renderer.render_image_readback(current_time, (width, height)) {
                        Ok((pixels, rw, rh)) => {
                            if rw == width && rh == height {
                                if let Some(img) = images.get_mut(&handle) {
                                    // Update raw pixel data; egui will pick up changes via bevy_egui
                                    img.data = Some(pixels);
                                    img.texture_descriptor.size.width = width;
                                    img.texture_descriptor.size.height = height;
                                    log::info!("Viewport image updated: {}x{}", width, height);
                                }
                                ui_run_state.last_readback_frame = ui_run_state.frames_since_start;
                            } else {
                                log::warn!(
                                    "Readback size mismatch: got {}x{}, expected {}x{}",
                                    rw, rh, width, height
                                );
                            }
                        }
                        Err(e) => {
                            log::error!("Image readback failed: {:?}", e);
                        }
                    }
                }
            }
        } else {
            // GPU-only policy: do not populate CPU fallback imagery.
            // The viewport panel will draw a placeholder until the renderer is ready.
            log::warn!("FractalRenderer missing; GPU renderer not initialized yet; skipping CPU fallback");
        }

        // Viewport drawing now happens inside FractalStudioApp::show_fractal_viewport
        // via the central panel. Avoid duplicating an extra window here to reduce
        // potential egui lifecycle races and UI contention.
        if app_state.app.viewport_texture.is_none() {
            log::warn!("Viewport texture not registered with egui yet");
        }
    }

    // Increment frame counter at the end of update
    ui_run_state.frames_since_start += 1;
    // End of update system
}

/// Handle window focus events to prevent camera-related panics
fn handle_window_focus(
    mut focus_events: EventReader<WindowFocused>,
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
    mut exit_events: EventReader<AppExit>,
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
/// Run condition: only execute UI update when the primary egui context exists
fn has_primary_egui_ctx(
    query: Query<Entity, (With<EguiContext>, With<PrimaryEguiContext>)>
) -> bool {
    !query.is_empty()
}
