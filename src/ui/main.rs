//! Main UI Application Module
//!
//! This module provides the main egui application for the Fractal Shader Studio,
//! integrating the GPU renderer with the user interface.

use bevy::prelude::*;
use bevy_egui::EguiContexts;
use rfd::FileDialog;
use crate::ui::node_editor::NodeEditor;
use crate::ui::fractal_ui::FractalCodeEditor;
use crate::fractal::renderer::FractalRenderer;
use crate::fractal::types::QualityPreset;
use crate::project::FractalStudioProject;
use std::sync::Arc;
use image::ColorType;
use crate::export::video::{VideoRecorder, VideoCodec, VideoQuality};
use crate::export::animation::{AnimationExporter, AnimationExportSettings, AnimationFormat, ExportFrames, AnimationQuality};
use crate::animation::timeline::TimelineProject;
use crate::metrics::PerformanceTracker;
use std::time::Instant;
use std::time::SystemTime;
use std::path::PathBuf;
use walkdir::WalkDir;

/// Main application state
pub struct FractalStudioApp {
    // Fractal parameters
    pub time: f32,
    pub selected_fractal: usize,
    pub fractal_types: Vec<&'static str>,
    pub node_editor: NodeEditor,
    pub code_editor: FractalCodeEditor,

    // GPU renderer
    pub fractal_renderer: Option<FractalRenderer>,
    pub viewport_texture: Option<egui::TextureId>,
    pub viewport_image_handle: Option<Handle<Image>>,    
    pub has_wgpu_support: bool,
    // Panel visibility toggles
    pub show_left_panel: bool,
    pub show_right_panel: bool,
    pub show_bottom_panel: bool,

    // Workspace management
    pub current_workspace: WorkspaceView,

    // Project management
    pub current_project: FractalStudioProject,
    pub current_shader_path: Option<std::path::PathBuf>,
    pub code_unsaved_changes: bool,
    // Shader library and hot-reload state
    pub shader_library: Vec<PathBuf>,
    pub shader_hot_reload_enabled: bool,
    pub shader_last_modified: Option<SystemTime>,
    pub shader_filter_text: String,
    pub last_hot_reload_poll: Option<Instant>,

    // Render settings
    pub render_scale: f32,
    pub fxaa_enabled: bool,

    // Default fractal parameters - reduced for better performance
    pub max_iterations: u32,
    pub bailout: f32,
    pub power: f32,
    pub scale: f32,
    pub position: [f32; 3],
    pub rotation: [f32; 3],
    pub color_saturation: f32,
    pub camera_fov: f32,
    pub camera_target: [f32; 3],
    // Lighting controls
    pub light_direction: [f32; 3],
    pub light_color: [f32; 3],
    pub light_intensity: f32,
    // Material controls
    pub material_metallic: f32,
    pub material_roughness: f32,
    pub use_fragment_pseudo3d: bool,
    // Viewport state
    pub last_viewport_resolution: [u32; 2],
    // Video recording state
    pub video_recorder: Option<VideoRecorder>,
    pub is_recording: bool,
    pub recording_frame_rate: u32,
    // Timeline playback state
    pub is_playing: bool,
    pub playback_speed: f32,
    pub timeline_duration: f32,
    pub last_update_instant: Option<Instant>,

    // Mapping creation form state (MIDI)
    pub midi_param_name: String,
    pub midi_channel: u8,
    pub midi_controller_cc: u8,
    pub midi_min_value: f32,
    pub midi_max_value: f32,
    pub midi_sensitivity: f32,
    pub midi_invert: bool,

    // Mapping creation form state (OSC)
    pub osc_address: String,
    pub osc_param_name: String,
    pub osc_min_value: f32,
    pub osc_max_value: f32,
    pub osc_sensitivity: f32,
    pub osc_invert: bool,

    // Mapping creation form state (Gesture)
    pub gesture_name: String,
    pub gesture_param_name: String,
    pub gesture_min_value: f32,
    pub gesture_max_value: f32,
    pub gesture_sensitivity: f32,
    pub gesture_invert: bool,
    
    // Undo/Redo system
    pub undo_stack: Vec<AppStateSnapshot>,
    pub redo_stack: Vec<AppStateSnapshot>,
    pub max_undo_steps: usize,

    // Status messaging
    pub status_message: Option<String>,

    // Performance tracking (non-visual, CSV logging only)
    pub metrics: PerformanceTracker,

    // GPU Settings UI state (GUI-only)
    pub show_gpu_settings_window: bool,
    pub gpu_backend: String,
    pub gpu_power_pref: String,
    pub gpu_dx12_compiler: String,
}

// Snapshot of application state for undo/redo
#[derive(Clone)]
pub struct AppStateSnapshot {
    pub time: f32,
    pub selected_fractal: usize,
    pub max_iterations: u32,
    pub bailout: f32,
    pub power: f32,
    pub scale: f32,
    pub position: [f32; 3],
    pub rotation: [f32; 3],
    pub color_saturation: f32,
    pub camera_fov: f32,
    pub camera_target: [f32; 3],
    pub light_direction: [f32; 3],
    pub light_color: [f32; 3],
    pub light_intensity: f32,
    pub material_metallic: f32,
    pub material_roughness: f32,
    pub current_workspace: WorkspaceView,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum WorkspaceView {
    Modeling,
    Animation,
    Rendering,
    NodeEditor,
    ShaderLoader,
}

impl Default for FractalStudioApp {
    fn default() -> Self {
        Self {
            time: 0.0,
            // Default to an intricate preset so launch shows rich 3D detail
            selected_fractal: 0,
            // 16 rich 3D presets: single-formula variants and multi-formula blends
            fractal_types: vec![
                "Mandelbulb (Power 8)",        // 0
                "Mandelbulb (Power 10)",       // 1
                "Mandelbulb (Power 6)",        // 2
                "Mandelbox (Scale 2.0)",       // 3
                "Mandelbox (Scale 1.8)",       // 4
                "Mandelbox (Scale 2.2)",       // 5
                "Quaternion Julia (Classic)",  // 6
                "Quaternion Julia (Variant)",  // 7
                "Bulb ∪ Box (Union)",          // 8
                "Bulb ∩ Box (Intersection)",   // 9
                "Bulb − Box (Subtraction)",    // 10
                "Smooth Union (Bulb, Box)",    // 11
                "Smooth Intersect (Box, QJulia)", // 12
                "Smooth Subtract (Bulb, QJulia)", // 13
                "Triplet Smooth Union (Bulb+Box+QJulia)", // 14
                "Box ∩ QJulia (Intersection)", // 15
            ],
            node_editor: NodeEditor::new(),
            code_editor: FractalCodeEditor::new(),
            fractal_renderer: None,
            viewport_texture: None,
            viewport_image_handle: None,
            has_wgpu_support: false,
            // Panels: start with left and right visible to frame the canvas
            show_left_panel: true,
            show_right_panel: true,
            show_bottom_panel: false,
            current_workspace: WorkspaceView::Rendering,
            current_project: FractalStudioProject::default(),
            current_shader_path: None,
            code_unsaved_changes: false,
            shader_library: Vec::new(),
            shader_hot_reload_enabled: false,
            shader_last_modified: None,
            shader_filter_text: String::new(),
            last_hot_reload_poll: None,
            // Render settings defaults
            render_scale: 0.75,
            fxaa_enabled: true,
            // Default fractal parameters - balanced for better detail
            max_iterations: 100,
            bailout: 4.0,
            power: 8.0,
            scale: 2.0,
            position: [0.0, 0.0, 0.0],
            rotation: [0.0, 0.0, 0.0],
            color_saturation: 1.0,
            camera_fov: 60.0,
            camera_target: [0.0, 0.0, 0.0],
            // Lighting defaults
            light_direction: [0.4, 0.7, -0.2],
            light_color: [0.8, 0.9, 1.0],
            light_intensity: 1.0,
            // Material defaults
            material_metallic: 0.0,
            material_roughness: 0.5,
            use_fragment_pseudo3d: false,
            last_viewport_resolution: [800, 600],
            // Recording defaults
            video_recorder: None,
            is_recording: false,
            recording_frame_rate: 30,
            // Playback defaults
            is_playing: false,
            playback_speed: 1.0,
            timeline_duration: 100.0,
            last_update_instant: None,

            // MIDI mapping defaults
            midi_param_name: "zoom".to_string(),
            midi_channel: 1,
            midi_controller_cc: 1,
            midi_min_value: 0.1,
            midi_max_value: 10.0,
            midi_sensitivity: 1.0,
            midi_invert: false,

            // OSC mapping defaults
            osc_address: "/fractal/zoom".to_string(),
            osc_param_name: "zoom".to_string(),
            osc_min_value: 0.1,
            osc_max_value: 10.0,
            osc_sensitivity: 1.0,
            osc_invert: false,

            // Gesture mapping defaults
            gesture_name: "pinch".to_string(),
            gesture_param_name: "zoom".to_string(),
            gesture_min_value: 0.1,
            gesture_max_value: 10.0,
            gesture_sensitivity: 1.0,
            gesture_invert: false,
            // Undo/Redo system
            undo_stack: Vec::new(),
            redo_stack: Vec::new(),
            max_undo_steps: 50,
            status_message: None,
            metrics: PerformanceTracker::default(),
            // GPU Settings defaults reflect current environment
            show_gpu_settings_window: false,
            gpu_backend: std::env::var("WGPU_BACKEND").unwrap_or_else(|_| "vulkan,dx12,gl".to_string()),
            gpu_power_pref: std::env::var("WGPU_POWER_PREF").unwrap_or_else(|_| "HighPerformance".to_string()),
            gpu_dx12_compiler: std::env::var("WGPU_DX12_COMPILER").unwrap_or_else(|_| "dxcompiler".to_string()),
        }
    }
}

impl FractalStudioApp {
    pub fn new() -> Self {
        log::info!("Creating FractalStudioApp instance");

        let mut app = Self::default();
        app.has_wgpu_support = false; // Will be set when WGPU context is available
        // Initial scan of bundled shader library
        app.refresh_shader_library();

        log::info!("FractalStudioApp created successfully");
        app
    }

    fn refresh_shader_library(&mut self) {
        let mut list: Vec<PathBuf> = Vec::new();
        for entry in WalkDir::new("assets/shaders").into_iter().filter_map(Result::ok) {
            let p = entry.path();
            if p.is_file() {
                if let Some(ext) = p.extension().and_then(|s| s.to_str()) {
                    if ext.eq_ignore_ascii_case("wgsl") {
                        list.push(p.to_path_buf());
                    }
                }
            }
        }
        list.sort();
        self.shader_library = list;
    }

    pub fn initialize_wgpu(&mut self, device: Arc<bevy::render::renderer::RenderDevice>, queue: Arc<bevy::render::renderer::RenderQueue>, width: u32, height: u32) {
        log::info!("Initializing WGPU renderer with size {}x{}", width, height);
        // Log environment-driven WGPU configuration to aid diagnosis
        let backend = std::env::var("WGPU_BACKEND").unwrap_or_else(|_| "<unset>".into());
        let power = std::env::var("WGPU_POWER_PREF").unwrap_or_else(|_| "<unset>".into());
        let dx12_comp = std::env::var("WGPU_DX12_COMPILER").unwrap_or_else(|_| "<unset>".into());
        log::info!(
            "WGPU env → BACKEND={}, POWER_PREF={}, DX12_COMPILER={}",
            backend, power, dx12_comp
        );
        
        // Limit texture dimensions to device limits (typically 8192)
        let max_dimension = 8192;
        let actual_width = std::cmp::min(width, max_dimension);
        let actual_height = std::cmp::min(height, max_dimension);
        
        // Ensure minimum dimensions
        let safe_width = std::cmp::max(actual_width, 64);
        let safe_height = std::cmp::max(actual_height, 64);
        
        match FractalRenderer::new_with_wgpu_context(
            device.clone(),
            queue.clone(),
            safe_width,
            safe_height,
        ) {
            Ok(renderer) => {
                self.fractal_renderer = Some(renderer);
                self.has_wgpu_support = true;
                log::info!("Fractal renderer initialized successfully with size {}x{}", safe_width, safe_height);
                if let Some(r) = &mut self.fractal_renderer {
                    r.set_camera_fov(self.camera_fov);
                    r.set_camera_target(self.camera_target);
                    // Sync lighting and material to renderer
                    r.set_light_direction(self.light_direction);
                    r.set_light_color(self.light_color);
                    r.set_light_intensity(self.light_intensity);
                    r.set_material_metallic(self.material_metallic);
                    r.set_material_roughness(self.material_roughness);
                    // Apply default Medium quality preset using current viewport size as base
                    r.apply_quality_preset(QualityPreset::Medium, Some([safe_width, safe_height]));
                    // Ensure true 3D raymarcher path (disable pseudo-3D fragment pipeline)
                    r.set_fragment_pseudo3d(false);
                }
            }
            Err(e) => {
                log::error!("Failed to initialize fractal renderer: {}", e);
                self.has_wgpu_support = false;
                
                // Try with smaller dimensions as fallback
                if safe_width > 512 || safe_height > 512 {
                    log::info!("Trying with smaller dimensions (512x512)");
                    match FractalRenderer::new_with_wgpu_context(
                        device,
                        queue,
                        512,
                        512,
                    ) {
                        Ok(renderer) => {
                            self.fractal_renderer = Some(renderer);
                            self.has_wgpu_support = true;
                            log::info!("Fractal renderer initialized successfully with fallback size 512x512");
                            if let Some(r) = &mut self.fractal_renderer {
                                r.set_camera_fov(self.camera_fov);
                                r.set_camera_target(self.camera_target);
                                // Sync lighting and material to renderer
                                r.set_light_direction(self.light_direction);
                                r.set_light_color(self.light_color);
                                r.set_light_intensity(self.light_intensity);
                                r.set_material_metallic(self.material_metallic);
                                r.set_material_roughness(self.material_roughness);
                                // Apply default Medium quality preset with fallback size
                                r.apply_quality_preset(QualityPreset::Medium, Some([512, 512]));
                                // Ensure true 3D raymarcher path (disable pseudo-3D fragment pipeline)
                                r.set_fragment_pseudo3d(false);
                            }
                        }
                        Err(e) => {
                            log::error!("Failed to initialize fractal renderer with fallback size: {}", e);
                        }
                    }
                }
            }
        }
    }

    /// Show top panel with application title and controls
    fn show_top_panel(&mut self, ui: &mut egui::Ui) {
        // Create menu bar
        egui::menu::bar(ui, |ui| {
            ui.menu_button("File", |ui| {
                if ui.button("New Project").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Create a new project
                    self.current_project = crate::project::FractalStudioProject::new("Untitled Project");
                    self.viewport_texture = None;
                    ui.close();
                }
                if ui.button("Open Project...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open file dialog for .fract files
                    if let Some(path) = rfd::FileDialog::new()
                        .add_filter("Fractal Studio Project", &["fract"])
                        .pick_file()
                    {
                        match crate::project::FractalStudioProject::load_from_file_with_report(&path) {
                            Ok((project, report)) => {
                                let (errors, warnings, infos) = report.counts();
                                self.status_message = Some(format!(
                                    "Loaded project: {} errors, {} warnings, {} info",
                                    errors, warnings, infos
                                ));
                                self.current_project = project;
                                log::info!("Project loaded successfully from {:?}", path);
                                if report.has_errors() {
                                    log::warn!("Project loaded with validation errors");
                                }
                            }
                            Err(e) => {
                                self.status_message = Some(format!("Failed to load project: {}", e));
                                log::error!("Failed to load project: {}", e);
                            }
                        }
                    }
                    ui.close();
                }
                ui.separator();
                
                // Recent files (placeholder)
                ui.label("Recent Projects:");
                if ui.button("Project1.fract").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Open recent project
                    ui.close();
                }
                if ui.button("Fractal_Animation.fract").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Open recent project
                    ui.close();
                }
                ui.separator();
                
                if ui.button("Save Project").clicked() {
                    // Save to current project file or prompt for location
                    if let Some(path) = rfd::FileDialog::new()
                        .add_filter("Fractal Studio Project", &["fract"])
                        .save_file()
                    {
                        match self.current_project.save_to_file(&path) {
                            Ok(_) => {
                                log::info!("Project saved successfully to {:?}", path);
                            }
                            Err(e) => {
                                log::error!("Failed to save project: {}", e);
                            }
                        }
                    }
                    ui.close();
                }
                if ui.button("Save Project As...").clicked() {
                    // Save project with new name
                    if let Some(path) = rfd::FileDialog::new()
                        .add_filter("Fractal Studio Project", &["fract"])
                        .save_file()
                    {
                        match self.current_project.save_to_file(&path) {
                            Ok(_) => {
                                log::info!("Project saved successfully to {:?}", path);
                            }
                            Err(e) => {
                                log::error!("Failed to save project: {}", e);
                            }
                        }
                    }
                    ui.close();
                }
                if ui.button("Save Project Copy...").clicked() {
                    // Save a copy of the project
                    if let Some(path) = rfd::FileDialog::new()
                        .add_filter("Fractal Studio Project", &["fract"])
                        .save_file()
                    {
                        match self.current_project.save_to_file(&path) {
                            Ok(_) => {
                                log::info!("Project copy saved successfully to {:?}", path);
                            }
                            Err(e) => {
                                log::error!("Failed to save project copy: {}", e);
                            }
                        }
                    }
                    ui.close();
                }
                ui.separator();

                // Shader file operations
                ui.label("Shaders:");
                if ui.button("New Shader").clicked() {
                    self.save_state_for_undo();
                    self.code_editor.code.clear();
                    self.current_shader_path = None;
                    self.code_unsaved_changes = false;
                    log::info!("Created new shader document");
                    ui.close();
                }
                if ui.button("Open Shader...").clicked() {
                    self.save_state_for_undo();
                    if let Some(path) = FileDialog::new()
                        .add_filter("WGSL Shader", &["wgsl"]) 
                        .add_filter("ISF Shader", &["fs"]) 
                        .pick_file()
                    {
                        let ext = path.extension().and_then(|e| e.to_str()).unwrap_or("");
                        match std::fs::read_to_string(&path) {
                            Ok(contents) => {
                                if ext.eq_ignore_ascii_case("fs") {
                                    match crate::ShaderConverter::isf_to_wgsl(&contents) {
                                        Ok(wgsl) => {
                                            self.code_editor.code = wgsl;
                                            self.current_shader_path = Some(path.with_extension("wgsl"));
                                            self.code_unsaved_changes = true;
                                            log::info!("Converted ISF to WGSL and loaded into editor");
                                        }
                                        Err(e) => {
                                            log::warn!("Advanced ISF->WGSL conversion failed ({}). Attempting simple GLSL->WGSL fallback.", e);
                                            match crate::ShaderConverter::glsl_to_wgsl(&contents) {
                                                Ok(wgsl) => {
                                                    self.code_editor.code = wgsl;
                                                    self.current_shader_path = Some(path.with_extension("wgsl"));
                                                    self.code_unsaved_changes = true;
                                                    log::info!("Converted ISF (fallback GLSL) to WGSL and loaded into editor");
                                                }
                                                Err(e2) => {
                                                    log::error!("ISF->WGSL conversion failed and fallback also failed: {}", e2);
                                                }
                                            }
                                        }
                                    }
                                } else {
                                    self.code_editor.code = contents;
                                    self.current_shader_path = Some(path);
                                    self.code_unsaved_changes = false;
                                    log::info!("Loaded shader: {:?}", self.current_shader_path);
                                }
                            }
                            Err(e) => log::error!("Failed to read shader file: {}", e),
                        }
                    }
                    ui.close();
                }
                if ui.button("Save Shader").clicked() {
                    let target_path = if let Some(ref p) = self.current_shader_path {
                        p.clone()
                    } else {
                        match FileDialog::new().add_filter("WGSL Shader", &["wgsl"]).save_file() {
                            Some(p) => p,
                            None => { ui.close(); return; }
                        }
                    };
                    match std::fs::write(&target_path, &self.code_editor.code) {
                        Ok(_) => { 
                            self.current_shader_path = Some(target_path.clone());
                            self.code_unsaved_changes = false;
                            log::info!("Saved shader to {:?}", target_path);
                        }
                        Err(e) => log::error!("Failed to save shader: {}", e),
                    }
                    ui.close();
                }
                if ui.button("Save Shader As...").clicked() {
                    if let Some(path) = FileDialog::new().add_filter("WGSL Shader", &["wgsl"]).save_file() {
                        match std::fs::write(&path, &self.code_editor.code) {
                            Ok(_) => { 
                                self.current_shader_path = Some(path.clone());
                                self.code_unsaved_changes = false;
                                log::info!("Saved shader as {:?}", path);
                            }
                            Err(e) => log::error!("Failed to save shader: {}", e),
                        }
                    }
                    ui.close();
                }
                
                // Import/Export
                ui.menu_button("Import", |ui| {
                    if ui.button("Import Image...").clicked() {
                        // Save current state for undo
                        self.save_state_for_undo();
                        
                        // Import image file
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Image Files", &["png", "jpg", "jpeg", "tiff", "bmp"])
                            .pick_file()
                        {
                            log::info!("Importing image from {:?}", path);
                            // TODO: Implement image import
                        }
                        ui.close();
                    }
                    if ui.button("Import Mesh...").clicked() {
                        // Save current state for undo
                        self.save_state_for_undo();
                        
                        // Import mesh file
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Mesh Files", &["obj", "stl", "fbx", "gltf"])
                            .pick_file()
                        {
                            log::info!("Importing mesh from {:?}", path);
                            // TODO: Implement mesh import
                        }
                        ui.close();
                    }
                    if ui.button("Import Shader...").clicked() {
                        // Save current state for undo
                        self.save_state_for_undo();
                        
                        // Import shader file
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Shader Files", &["glsl", "wgsl", "hlsl", "isf"])
                            .pick_file()
                        {
                            log::info!("Importing shader from {:?}", path);
                            // TODO: Implement shader import
                        }
                        ui.close();
                    }
                });
                
                ui.menu_button("Export", |ui| {
                    if ui.button("Export Image...").clicked() {
                        // Export current viewport image using GPU renderer where available
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Image Files", &["png", "jpg", "tiff"])
                            .set_title("Export Image")
                            .save_file()
                        {
                            self.save_state_for_undo();

                            // Determine format from extension (default PNG)
                            let format = match path.extension().and_then(|e| e.to_str()).map(|s| s.to_lowercase()) {
                                Some(ref ext) if ext == "jpg" || ext == "jpeg" => crate::export::ExportFormat::JPEG,
                                Some(ref ext) if ext == "tiff" || ext == "tif" => crate::export::ExportFormat::TIFF,
                                _ => crate::export::ExportFormat::PNG,
                            };

                            let [w, h] = self.last_viewport_resolution;
                            let width = w.max(1);
                            let height = h.max(1);

                            // Prefer GPU renderer for readback; fallback to CPU if unavailable
                            let result = if let Some(renderer) = self.fractal_renderer.as_mut() {
                                match renderer.render_image_readback(self.time, (width, height)) {
                                    Ok((pixels, rw, rh)) => {
                                        if rw != width || rh != height {
                                            log::warn!("Exported frame size mismatch: {}x{} vs {}x{}", rw, rh, width, height);
                                        }
                                        crate::export::ImageExporter::export_image(&pixels, rw, rh, format, &path)
                                    }
                                    Err(e) => {
                                        log::error!("GPU readback failed: {}. Falling back to CPU renderer.", e);
                                        let cpu = crate::fractal::renderer::CPUFractalRenderer::new();
                                        let pixels = cpu.render(width, height);
                                        crate::export::ImageExporter::export_image(&pixels, width, height, format, &path)
                                    }
                                }
                            } else {
                                // No GPU renderer: use CPU renderer
                                let cpu = crate::fractal::renderer::CPUFractalRenderer::new();
                                let pixels = cpu.render(width, height);
                                crate::export::ImageExporter::export_image(&pixels, width, height, format, &path)
                            };

                            match result {
                                Ok(_) => log::info!("Exported image to {:?}", path),
                                Err(e) => log::error!("Failed to export image: {}", e),
                            }
                        } else {
                            log::info!("Image export canceled by user");
                        }
                        ui.close();
                    }
                    if ui.button("Export Animation...").clicked() {
                        // Export animation sequence using AnimationExporter
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Video Files", &["mp4", "avi", "mov", "webm", "gif"])
                            .set_title("Export Animation")
                            .save_file()
                        {
                            self.save_state_for_undo();
                            // Determine format from extension
                            let format = match path.extension().and_then(|e| e.to_str()).map(|s| s.to_lowercase()) {
                                Some(ref ext) if ext == "mp4" => AnimationFormat::MP4,
                                Some(ref ext) if ext == "avi" => AnimationFormat::AVI,
                                Some(ref ext) if ext == "mov" => AnimationFormat::MOV,
                                Some(ref ext) if ext == "webm" => AnimationFormat::WEBM,
                                Some(ref ext) if ext == "gif" => AnimationFormat::GIF,
                                _ => AnimationFormat::MP4,
                            };

                            let [w, h] = self.last_viewport_resolution;
                            let frame_rate = self.recording_frame_rate as f32;
                            let default_duration = 5.0f32; // simple default
                            let settings = AnimationExportSettings {
                                format,
                                frame_rate,
                                resolution: [w.max(1), h.max(1)],
                                duration: default_duration,
                                export_frames: ExportFrames::AllFrames,
                                quality: AnimationQuality::Standard,
                                compression: crate::export::animation::AnimationCompression::H264,
                                include_audio: false,
                            };

                            // Build a basic timeline project for export
                            let mut timeline_project = TimelineProject::new(&self.current_project.metadata.name);
                            timeline_project.frame_rate = frame_rate;
                            timeline_project.total_frames = (default_duration * frame_rate) as u32;

                            // Prepare output base directory: <parent>/<name>_frames
                            let base_name = path.file_stem().and_then(|s| s.to_str()).unwrap_or("animation");
                            let parent_dir = path.parent().unwrap_or_else(|| std::path::Path::new("."));
                            let frames_dir = parent_dir.join(format!("{}{}_frames", base_name, ""));
                            if let Err(e) = std::fs::create_dir_all(&frames_dir) {
                                log::error!("Failed to create frames directory: {}", e);
                                ui.close();
                                return;
                            }

                            let exporter = AnimationExporter::new(settings);
                            match exporter.export_animation(&timeline_project, frames_dir.to_str().unwrap_or("exports/animation")) {
                                Ok(result) => {
                                    log::info!(
                                        "Exported animation: {} frames over {:.2}s; files: {:?}",
                                        result.frames_generated,
                                        result.total_duration,
                                        result.output_files
                                    );
                                }
                                Err(e) => {
                                    log::error!("Failed to export animation: {}", e);
                                }
                            }
                        }
                        ui.close();
                    }
                    if ui.button("Export Mesh...").clicked() {
                        // Export fractal as mesh
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Mesh Files", &["obj", "stl", "fbx"])
                            .save_file()
                        {
                            log::info!("Exporting mesh to {:?}", path);
                            // TODO: Implement mesh export
                        }
                        ui.close();
                    }
                    if ui.button("Export Shader...").clicked() {
                        // Export current shader
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Shader Files", &["glsl", "wgsl"])
                            .save_file()
                        {
                            log::info!("Exporting shader to {:?}", path);
                            // TODO: Implement shader export
                        }
                        ui.close();
                    }
                });
                ui.separator();
                
                // Project management
                if ui.button("Project Settings...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open project settings dialog
                    log::info!("Opening project settings");
                    // TODO: Implement project settings dialog
                    ui.close();
                }
                ui.separator();
                
                if ui.button("Quit").clicked() {
                    // Quit application
                    log::info!("Quitting application");
                    // TODO: Implement proper application shutdown
                    ui.close();
                }
            });

            ui.menu_button("Edit", |ui| {
                if ui.button("Undo").clicked() {
                    // Undo last action
                    self.undo();
                    log::info!("Undo action");
                    ui.close();
                }
                if ui.button("Redo").clicked() {
                    // Redo last undone action
                    self.redo();
                    log::info!("Redo action");
                    ui.close();
                }
                ui.separator();
                
                if ui.button("Cut").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Cut selected items
                    log::info!("Cut action");
                    // TODO: Implement cut
                    ui.close();
                }
                if ui.button("Copy").clicked() {
                    // Copy selected items
                    log::info!("Copy action");
                    // TODO: Implement copy
                    ui.close();
                }
                if ui.button("Paste").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Paste clipboard contents
                    log::info!("Paste action");
                    // TODO: Implement paste
                    ui.close();
                }
                ui.separator();
                
                // Selection
                if ui.button("Select All").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Select all objects
                    log::info!("Select all");
                    // TODO: Implement select all
                    ui.close();
                }
                if ui.button("Deselect All").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Deselect all objects
                    log::info!("Deselect all");
                    // TODO: Implement deselect all
                    ui.close();
                }
                ui.separator();
                
                // Preferences
                if ui.button("Preferences...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open preferences dialog
                    log::info!("Opening preferences");
                    // TODO: Implement preferences dialog
                    ui.close();
                }
            });

            ui.menu_button("View", |ui| {
                // Workspace views
                ui.label("Workspaces:");
                if ui.button("Modeling").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.current_workspace = WorkspaceView::Modeling;
                    ui.close();
                }
                if ui.button("Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.current_workspace = WorkspaceView::Animation;
                    ui.close();
                }
                if ui.button("Rendering").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.current_workspace = WorkspaceView::Rendering;
                    ui.close();
                }
                if ui.button("Node Editor").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.current_workspace = WorkspaceView::NodeEditor;
                    ui.close();
                }
                if ui.button("Shader Loader").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.current_workspace = WorkspaceView::ShaderLoader;
                    ui.close();
                }
                ui.separator();
                
                // View controls
                ui.label("View Controls:");
                if ui.button("Reset View").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Reset camera view
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    log::info!("Reset view");
                    ui.close();
                }
                if ui.button("Frame All").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Frame all objects in view
                    log::info!("Frame all objects");
                    // TODO: Implement frame all
                    ui.close();
                }
                ui.separator();
                
                // Panels
                ui.label("Panels:");
                ui.horizontal(|ui| {
                    let mut left = self.show_left_panel;
                    let mut right = self.show_right_panel;
                    let mut bottom = self.show_bottom_panel;

                    if ui.checkbox(&mut left, "Left").clicked() {
                        self.save_state_for_undo();
                        self.show_left_panel = left;
                        log::info!("Left panel visibility: {}", left);
                    }
                    if ui.checkbox(&mut right, "Right").clicked() {
                        self.save_state_for_undo();
                        self.show_right_panel = right;
                        log::info!("Right panel visibility: {}", right);
                    }
                    if ui.checkbox(&mut bottom, "Bottom").clicked() {
                        self.save_state_for_undo();
                        self.show_bottom_panel = bottom;
                        log::info!("Bottom panel visibility: {}", bottom);
                    }
                });
                ui.separator();
                
                if ui.button("Fullscreen").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Toggle fullscreen mode
                    log::info!("Toggle fullscreen");
                    // TODO: Implement fullscreen toggle
                    ui.close();
                }
            });

            ui.menu_button("Fractal", |ui| {
                // Fractal generation
                ui.label("Fractal Types:");
                if ui.button("Mandelbulb").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.selected_fractal = 1;
                    self.reset_parameters_for_fractal_type(1);
                    ui.close();
                }
                if ui.button("Mandelbox").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.selected_fractal = 2;
                    self.reset_parameters_for_fractal_type(2);
                    ui.close();
                }
                if ui.button("Quaternion Julia").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.selected_fractal = 4;
                    self.reset_parameters_for_fractal_type(4);
                    ui.close();
                }
                ui.separator();
                
                // Fractal operations
                if ui.button("Generate Fractal").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Generate the current fractal
                    log::info!("Generating fractal");
                    // TODO: Implement fractal generation
                    ui.close();
                }
                if ui.button("Optimize Parameters").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Optimize rendering parameters
                    log::info!("Optimizing parameters");
                    // TODO: Implement parameter optimization
                    ui.close();
                }
                ui.separator();
                
                // Fractal settings
                if ui.button("Fractal Settings...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open fractal settings dialog
                    log::info!("Opening fractal settings");
                    // TODO: Implement fractal settings dialog
                    ui.close();
                }
            });

            // Quick-load curated examples for immediate visual verification
            ui.menu_button("Examples", |ui| {
                if ui.button("Mandelbrot (2D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 0;
                    self.reset_parameters_for_fractal_type(0);
                    self.status_message = Some("Loaded example: Mandelbrot (GPU displays 3D fallback)".to_string());
                    ui.close();
                }
                if ui.button("Mandelbulb (3D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 1;
                    self.reset_parameters_for_fractal_type(1);
                    self.status_message = Some("Loaded example: Mandelbulb".to_string());
                    ui.close();
                }
                if ui.button("Mandelbox (3D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 2;
                    self.reset_parameters_for_fractal_type(2);
                    self.status_message = Some("Loaded example: Mandelbox".to_string());
                    ui.close();
                }
                if ui.button("Quaternion Julia (3D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 4;
                    self.reset_parameters_for_fractal_type(4);
                    self.status_message = Some("Loaded example: Quaternion Julia".to_string());
                    ui.close();
                }
                ui.separator();
                if ui.button("Tricorn (2D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 11;
                    self.reset_parameters_for_fractal_type(11);
                    self.status_message = Some("Loaded example: Tricorn (GPU displays 3D fallback)".to_string());
                    ui.close();
                }
                if ui.button("Sierpinski (3D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 13;
                    self.reset_parameters_for_fractal_type(13);
                    self.status_message = Some("Loaded example: Sierpinski (GPU displays 3D fallback)".to_string());
                    ui.close();
                }
                if ui.button("Koch Snowflake (2D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 14;
                    self.reset_parameters_for_fractal_type(14);
                    self.status_message = Some("Loaded example: Koch Snowflake (GPU displays 3D fallback)".to_string());
                    ui.close();
                }
                if ui.button("Dragon Curve (2D)").clicked() {
                    self.save_state_for_undo();
                    self.selected_fractal = 15;
                    self.reset_parameters_for_fractal_type(15);
                    self.status_message = Some("Loaded example: Dragon Curve (GPU displays 3D fallback)".to_string());
                    ui.close();
                }
            });

            ui.menu_button("Animation", |ui| {
                // Animation controls
                if ui.button("Play Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Start playing animation
                    log::info!("Playing animation");
                    // TODO: Implement animation playback
                    ui.close();
                }
                if ui.button("Pause Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Pause animation
                    log::info!("Pausing animation");
                    // TODO: Implement animation pause
                    ui.close();
                }
                if ui.button("Stop Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Stop animation
                    log::info!("Stopping animation");
                    // TODO: Implement animation stop
                    ui.close();
                }
                ui.separator();
                
                // Keyframe operations
                if ui.button("Insert Keyframe").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Insert keyframe at current time
                    log::info!("Inserting keyframe");
                    // TODO: Implement keyframe insertion
                    ui.close();
                }
                if ui.button("Delete Keyframe").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Delete selected keyframe
                    log::info!("Deleting keyframe");
                    // TODO: Implement keyframe deletion
                    ui.close();
                }
                ui.separator();
                
                // Animation settings
                if ui.button("Animation Settings...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open animation settings dialog
                    log::info!("Opening animation settings");
                    // TODO: Implement animation settings dialog
                    ui.close();
                }
                if ui.button("Timeline Editor...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open timeline editor
                    log::info!("Opening timeline editor");
                    // TODO: Implement timeline editor
                    ui.close();
                }
            });

            ui.menu_button("Render", |ui| {
                // Render operations
                if ui.button("Render Image").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Render current view as high-quality image and save to PNG
                    log::info!("Rendering image");
                    let default_dims = self.last_viewport_resolution;
                    let file = FileDialog::new()
                        .add_filter("PNG Image", &["png"])
                        .set_title("Save Rendered Image")
                        .save_file();
                    if let Some(path) = file {
                        if let Some(renderer) = &mut self.fractal_renderer {
                            let [w, h] = default_dims;
                            match renderer.render_image_readback(self.time, (w, h)) {
                                Ok((pixels, rw, rh)) => {
                                    let mut save_path = path.clone();
                                    if save_path.extension().is_none() {
                                        save_path.set_extension("png");
                                    }
                                    match image::save_buffer(&save_path, &pixels, rw, rh, ColorType::Rgba8) {
                                        Ok(_) => log::info!("Saved image to {:?}", save_path),
                                        Err(e) => log::error!("Failed to save PNG: {}", e),
                                    }
                                }
                                Err(e) => {
                                    log::error!("Failed to render image: {}", e);
                                }
                            }
                        } else {
                            log::warn!("Renderer not initialized; cannot render image.");
                        }
                    } else {
                        log::info!("Image save canceled by user");
                    }
                    ui.close();
                }
                if ui.button("Render Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Render animation sequence to video using ffmpeg via VideoRecorder
                    let file = FileDialog::new()
                        .add_filter("Video Files", &["mp4", "mov", "avi", "webm"])
                        .set_title("Save Rendered Animation")
                        .save_file();
                    if let Some(path) = file {
                        let [w, h] = self.last_viewport_resolution;
                        let frame_rate = self.recording_frame_rate;
                        let codec = match path.extension().and_then(|e| e.to_str()).map(|s| s.to_lowercase()) {
                            Some(ext) if ext == "mp4" => VideoCodec::H264,
                            Some(ext) if ext == "mov" => VideoCodec::ProRes,
                            Some(ext) if ext == "avi" => VideoCodec::H264,
                            Some(ext) if ext == "webm" => VideoCodec::VP9,
                            _ => VideoCodec::H264,
                        };
                        let quality = if self.current_workspace == WorkspaceView::Rendering { VideoQuality::High } else { VideoQuality::Medium };

                        if let Some(renderer) = &mut self.fractal_renderer {
                            match VideoRecorder::start(w.max(1), h.max(1), frame_rate.max(1), codec, quality, &path) {
                                Ok(mut recorder) => {
                                    // Determine duration from project's simple animation timeline, fallback to 5s
                                    let duration = self.current_project.animation_controller.timeline().duration();
                                    let total_frames = (duration * frame_rate as f32) as u32;

                                    for frame_idx in 0..total_frames {
                                        let t = frame_idx as f32 / frame_rate as f32;
                                        self.time = t;
                                        self.current_project.animation_controller.set_time(t);
                                        match renderer.render_image_readback(t, (w, h)) {
                                            Ok((pixels, rw, rh)) => {
                                                if rw == w && rh == h {
                                                    if let Err(e) = recorder.send_frame(&pixels) {
                                                        log::error!("Failed to send frame {}: {}", frame_idx, e);
                                                        break;
                                                    }
                                                } else {
                                                    log::warn!("Frame size mismatch: got {}x{}, expected {}x{}", rw, rh, w, h);
                                                }
                                            }
                                            Err(e) => {
                                                log::error!("Failed to render frame {}: {}", frame_idx, e);
                                                break;
                                            }
                                        }
                                    }

                                    if let Err(e) = recorder.stop() {
                                        log::error!("Failed to finalize video: {}", e);
                                    } else {
                                        log::info!("Saved animation to {:?}", path);
                                    }
                                }
                                Err(e) => {
                                    log::error!("Failed to start video recorder: {}", e);
                                }
                            }
                        } else {
                            log::warn!("Renderer not initialized; cannot render animation.");
                        }
                    } else {
                        log::info!("Animation save canceled by user");
                    }
                    ui.close();
                }
                ui.separator();
                
                // Render settings
                if ui.button("Render Settings...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open render settings dialog
                    log::info!("Opening render settings");
                    // TODO: Implement render settings dialog
                    ui.close();
                }
                if ui.button("Render Viewport").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Render current viewport once using GPU renderer if available
                    log::info!("Rendering viewport");
                    if let Some(renderer) = self.fractal_renderer.as_mut() {
                        // Use the current rect size tracked in last_viewport_resolution
                        let [w, h] = self.last_viewport_resolution;
                        let width = w.max(1);
                        let height = h.max(1);
                        if let Err(e) = renderer.render_frame(self.time, (width, height)) {
                            log::error!("Failed to render viewport: {}", e);
                        } else {
                            log::info!("Rendered viewport at {}x{}", width, height);
                        }
                    } else {
                        log::warn!("Renderer not initialized; cannot render viewport.");
                    }
                    ui.close();
                }
                ui.separator();
                // Recording controls
                ui.label("Recording:");
                if !self.is_recording {
                    if ui.button("Start Recording").clicked() {
                        self.save_state_for_undo();
                        let default_dims = self.last_viewport_resolution;
                        let file = FileDialog::new()
                            .add_filter("Video", &["mp4", "mkv"])
                            .set_title("Save Recording")
                            .save_file();
                        if let Some(mut path) = file {
                            if path.extension().is_none() {
                                path.set_extension("mp4");
                            }
                            if let Some(renderer) = &self.fractal_renderer {
                                let [w, h] = default_dims;
                                match VideoRecorder::start(
                                    w,
                                    h,
                                    self.recording_frame_rate,
                                    VideoCodec::H264,
                                    VideoQuality::Medium,
                                    &path,
                                ) {
                                    Ok(rec) => {
                                        self.video_recorder = Some(rec);
                                        self.is_recording = true;
                                        log::info!("Started recording to {:?} ({}x{}, {} fps)", path, w, h, self.recording_frame_rate);
                                    }
                                    Err(e) => {
                                        log::error!("Failed to start recorder: {:?}", e);
                                    }
                                }
                            } else {
                                log::warn!("Renderer not initialized; cannot start recording.");
                            }
                        } else {
                            log::info!("Recording save canceled by user");
                        }
                        ui.close();
                    }
                } else {
                    if ui.button("Stop Recording").clicked() {
                        self.save_state_for_undo();
                        if let Some(mut rec) = self.video_recorder.take() {
                            match rec.stop() {
                                Ok(_) => log::info!("Stopped recording and finalized file"),
                                Err(e) => log::error!("Failed to finalize recording: {:?}", e),
                            }
                        }
                        self.is_recording = false;
                        ui.close();
                    }
                }
                ui.separator();
                
                // Render quality
                ui.label("Render Quality:");
                if ui.button("Preview").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Set preview quality preset using current viewport as base resolution
                    log::info!("Setting preview quality");
                    if let Some(renderer) = &mut self.fractal_renderer {
                        // Map UI 'Preview' to Medium preset for interactive performance
                        renderer.apply_quality_preset(QualityPreset::Medium, Some(self.last_viewport_resolution));
                    } else {
                        log::warn!("Renderer not initialized; cannot apply Preview preset.");
                    }
                    ui.close();
                }
                if ui.button("Production").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Set production quality preset using current viewport as base resolution
                    log::info!("Setting production quality");
                    if let Some(renderer) = &mut self.fractal_renderer {
                        // Map UI 'Production' to Ultra preset for maximum quality
                        renderer.apply_quality_preset(QualityPreset::Ultra, Some(self.last_viewport_resolution));
                    } else {
                        log::warn!("Renderer not initialized; cannot apply Production preset.");
                    }
                    ui.close();
                }
                if ui.button("Custom").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Set custom quality
                    log::info!("Setting custom quality");
                    // TODO: Implement quality settings
                    ui.close();
                }
            });

            ui.menu_button("Nodes", |ui| {
                // Node operations
                if ui.button("Add Node").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Add new node to graph
                    log::info!("Adding node");
                    // TODO: Implement node addition
                    ui.close();
                }
                if ui.button("Delete Node").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Delete selected node
                    log::info!("Deleting node");
                    // TODO: Implement node deletion
                    ui.close();
                }
                ui.separator();
                
                // Node library
                ui.label("Node Library:");
                if ui.button("Fractal Nodes").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Show fractal nodes
                    log::info!("Showing fractal nodes");
                    // TODO: Implement node library
                    ui.close();
                }
                if ui.button("Math Nodes").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Show math nodes
                    log::info!("Showing math nodes");
                    // TODO: Implement node library
                    ui.close();
                }
                if ui.button("Color Nodes").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Show color nodes
                    log::info!("Showing color nodes");
                    // TODO: Implement node library
                    ui.close();
                }
                if ui.button("Animation Nodes").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Show animation nodes
                    log::info!("Showing animation nodes");
                    // TODO: Implement node library
                    ui.close();
                }
                ui.separator();
                
                // Node settings
                if ui.button("Node Settings...").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open node settings dialog
                    log::info!("Opening node settings");
                    // TODO: Implement node settings dialog
                    ui.close();
                }
            });

            ui.menu_button("Tools", |ui| {
                // Camera tools
                ui.label("Camera:");
                if ui.button("Orbit Camera").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate orbit camera tool
                    log::info!("Activating orbit camera tool");
                    // TODO: Implement camera tools
                    ui.close();
                }
                if ui.button("Pan Camera").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate pan camera tool
                    log::info!("Activating pan camera tool");
                    // TODO: Implement camera tools
                    ui.close();
                }
                if ui.button("Zoom Camera").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate zoom camera tool
                    log::info!("Activating zoom camera tool");
                    // TODO: Implement camera tools
                    ui.close();
                }
                ui.separator();
                
                // Selection tools
                ui.label("Selection:");
                if ui.button("Select Object").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate object selection tool
                    log::info!("Activating object selection tool");
                    // TODO: Implement selection tools
                    ui.close();
                }
                if ui.button("Select Region").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate region selection tool
                    log::info!("Activating region selection tool");
                    // TODO: Implement selection tools
                    ui.close();
                }
                ui.separator();
                
                // Utility tools
                if ui.button("Measure Tool").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate measurement tool
                    log::info!("Activating measurement tool");
                    // TODO: Implement utility tools
                    ui.close();
                }
                if ui.button("Transform Tool").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Activate transform tool
                    log::info!("Activating transform tool");
                    // TODO: Implement utility tools
                    ui.close();
                }
            });

            ui.menu_button("Window", |ui| {
                // Window management
                if ui.button("New Window").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Create new application window
                    log::info!("Creating new window");
                    // TODO: Implement window management
                    ui.close();
                }
                ui.separator();
                
                // Workspace layouts
                ui.label("Workspace Layouts:");
                if ui.button("Save Current Layout").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Save current workspace layout
                    log::info!("Saving current layout");
                    // TODO: Implement layout saving
                    ui.close();
                }
                if ui.button("Reset Layout").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Reset to default layout
                    log::info!("Resetting layout");
                    // TODO: Implement layout reset
                    ui.close();
                }
                ui.separator();
                
                if ui.button("Fullscreen").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Toggle fullscreen mode
                    log::info!("Toggling fullscreen");
                    // TODO: Implement fullscreen toggle
                    ui.close();
                }
            });

            ui.menu_button("Settings", |ui| {
                if ui.button("GPU Settings").clicked() {
                    self.show_gpu_settings_window = true;
                    ui.close();
                }
            });

            ui.menu_button("Help", |ui| {
                if ui.button("Documentation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open documentation
                    log::info!("Opening documentation");
                    // TODO: Implement documentation access
                    ui.close();
                }
                if ui.button("Tutorials").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open tutorials
                    log::info!("Opening tutorials");
                    // TODO: Implement tutorial access
                    ui.close();
                }
                if ui.button("Examples").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open examples
                    log::info!("Opening examples");
                    // TODO: Implement example access
                    ui.close();
                }
                ui.separator();
                
                if ui.button("Community Forum").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open community forum
                    log::info!("Opening community forum");
                    // TODO: Implement forum access
                    ui.close();
                }
                if ui.button("Report Bug").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Open bug reporting interface
                    log::info!("Opening bug reporting");
                    // TODO: Implement bug reporting
                    ui.close();
                }
                ui.separator();
                
                if ui.button("About Fractal Studio").clicked() {
                    // Show about dialog
                    log::info!("Showing about dialog");
                    // TODO: Implement about dialog
                    ui.close();
                }
                if ui.button("System Info").clicked() {
                    // Show system information
                    log::info!("Showing system info");
                    // TODO: Implement system info display
                    ui.close();
                }
            });

            // Spacer to push status to the right
            ui.with_layout(egui::Layout::right_to_left(egui::Align::Center), |ui| {
                ui.label(format!("FPS: {:.1}", 1.0 / ui.input(|i| i.unstable_dt)));
                ui.label(format!("Time: {:.2}s", self.time));
            });
        });
        // Quick-access workspace tabs and panel toggles
        ui.horizontal(|ui| {
            // Workspace tabs
            let modeling = ui.selectable_value(&mut self.current_workspace, WorkspaceView::Modeling, "Modeling");
            let animation = ui.selectable_value(&mut self.current_workspace, WorkspaceView::Animation, "Animation");
            let rendering = ui.selectable_value(&mut self.current_workspace, WorkspaceView::Rendering, "Rendering");
            let node_editor = ui.selectable_value(&mut self.current_workspace, WorkspaceView::NodeEditor, "Node Editor");
            let shader_loader = ui.selectable_value(&mut self.current_workspace, WorkspaceView::ShaderLoader, "Shader Loader");

            if modeling.clicked() || animation.clicked() || rendering.clicked() || node_editor.clicked() || shader_loader.clicked() {
                self.save_state_for_undo();
            }

            ui.separator();

            // Quick panel toggles
            let mut left = self.show_left_panel;
            let mut right = self.show_right_panel;
            let mut bottom = self.show_bottom_panel;

            let left_resp = ui.checkbox(&mut left, "Left");
            let right_resp = ui.checkbox(&mut right, "Right");
            let bottom_resp = ui.checkbox(&mut bottom, "Bottom");

            if left_resp.clicked() {
                self.save_state_for_undo();
                self.show_left_panel = left;
            }
            if right_resp.clicked() {
                self.save_state_for_undo();
                self.show_right_panel = right;
            }
            if bottom_resp.clicked() {
                self.save_state_for_undo();
                self.show_bottom_panel = bottom;
            }
        });

        ui.separator();
    }

    /// Show fractal controls panel
    fn show_fractal_controls(&mut self, ui: &mut egui::Ui) {
        ui.heading("Fractal Controls");

        ui.separator();

        egui::ScrollArea::vertical().show(ui, |ui| {
            // Fractal selection with search/filter capability
            ui.collapsing("Fractal Type", |ui| {
                ui.label("Select a fractal formula:");
                
                // Add a search/filter box
                let mut filter_text = String::new();
                ui.text_edit_singleline(&mut filter_text);
                
                let mut new_selection = self.selected_fractal;
                
                // Create a copy of the fractal types to avoid borrowing issues
                let fractal_types = self.fractal_types.clone();
                
                // Show filtered fractals
                for (i, fractal_name) in fractal_types.iter().enumerate() {
                    // Filter based on search text (if any)
                    if filter_text.is_empty() || fractal_name.to_lowercase().contains(&filter_text.to_lowercase()) {
                        if ui.selectable_label(self.selected_fractal == i, *fractal_name).clicked() {
                            // Save current state for undo
                            self.save_state_for_undo();
                            
                            new_selection = i;
                        }
                    }
                }
                
                if new_selection != self.selected_fractal {
                    // Directly select by index; set defaults for the chosen preset
                    self.selected_fractal = new_selection;
                    self.reset_parameters_for_fractal_type(new_selection);
                }
                
                // Lightweight preview to confirm selection, useful in Node Editor workspace
                ui.separator();
                ui.label(egui::RichText::new("Preview").strong());
                if self.has_wgpu_support {
                    if let Some(renderer) = &mut self.fractal_renderer {
                        // Build parameters similar to viewport
                        let mut params = crate::fractal::FractalParameters::default();
                        params.max_iterations = self.max_iterations;
                        params.bailout = self.bailout;
                        params.scale = self.scale;
                        params.position = nalgebra::Vector3::new(self.position[0], self.position[1], self.position[2]);
                        params.rotation = nalgebra::Vector3::new(self.rotation[0], self.rotation[1], self.rotation[2]);
                        params.color_saturation = self.color_saturation;
                        // Apply selected preset to renderer parameters and combiner
                        let mut q_c = [self.rotation[0], self.rotation[1], self.rotation[2], 0.0];
                        let mut mandelbulb_power: f32 = self.power;
                        let (base_formula_id, comb_active, comb_formulas, comb_mode, comb_blend, comb_k) = match self.selected_fractal {
                            // Single-formula presets
                            0 => { mandelbulb_power = 8.0; (1, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            1 => { mandelbulb_power = 10.0; (1, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            2 => { mandelbulb_power = 6.0; (1, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            3 => { params.scale = 2.0; (2, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            4 => { params.scale = 1.8; (2, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            5 => { params.scale = 2.2; (2, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            6 => { q_c = [0.3, 0.5, 0.4, 0.0]; (4, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            7 => { q_c = [0.6, 0.2, 0.3, 0.0]; (4, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0) }
                            // Combiner presets (use formula IDs: 1 Bulb, 2 Box, 4 QJulia)
                            8 => (1, true, [1, 2, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0),
                            9 => (2, true, [1, 2, 0], crate::fractal::types::BlendMode::Intersection, 0.0, 0.0),
                            10 => (1, true, [1, 2, 0], crate::fractal::types::BlendMode::Subtraction, 0.0, 0.0),
                            11 => (1, true, [1, 2, 0], crate::fractal::types::BlendMode::SmoothUnion(0.3), 0.5, 0.3),
                            12 => { q_c = [0.3, 0.5, 0.4, 0.0]; (2, true, [2, 4, 0], crate::fractal::types::BlendMode::SmoothIntersection(0.25), 0.6, 0.25) }
                            13 => { q_c = [0.6, 0.2, 0.3, 0.0]; (1, true, [1, 4, 0], crate::fractal::types::BlendMode::SmoothSubtraction(0.25), 0.5, 0.25) }
                            14 => { q_c = [0.4, 0.4, 0.2, 0.0]; (1, true, [1, 2, 4], crate::fractal::types::BlendMode::SmoothUnion(0.35), 0.7, 0.35) }
                            15 => { q_c = [0.35, 0.45, 0.25, 0.0]; (2, true, [2, 4, 0], crate::fractal::types::BlendMode::Intersection, 0.0, 0.0) }
                            _ => (1, false, [0, 0, 0], crate::fractal::types::BlendMode::Union, 0.0, 0.0),
                        };

                        params.formula = match base_formula_id {
                            1 => crate::fractal::FractalFormula::Mandelbulb { power: mandelbulb_power },
                            2 => crate::fractal::FractalFormula::Mandelbox { scale: params.scale },
                            4 => crate::fractal::FractalFormula::QuaternionJulia { c: q_c, max_iterations: self.max_iterations },
                            _ => crate::fractal::FractalFormula::Mandelbulb { power: mandelbulb_power },
                        };
                        renderer.update_parameters(&params);
                        renderer.set_combiner(comb_active, comb_formulas, comb_mode, comb_blend, comb_k);

                        let preview_size = egui::vec2(220.0, 150.0);
                        let w = preview_size.x.max(1.0).round() as u32;
                        let h = preview_size.y.max(1.0).round() as u32;
                        match renderer.render_frame_to_texture(self.time, (w, h), ui.ctx()) {
                            Ok(tex_id) => {
                                ui.add(egui::Image::new((tex_id, preview_size)));
                                let name = match self.selected_fractal { 1 => "Mandelbulb", 2 => "Mandelbox", 4 => "Quaternion Julia", _ => "Mandelbulb" };
                                ui.label(format!("{} • {}×{}", name, w, h));
                                if self.current_workspace == WorkspaceView::NodeEditor {
                                    ui.label("Switch to Modeling/Rendering for the full viewport.");
                                }
                            }
                            Err(_e) => {
                                ui.label("Preview rendering failed.");
                            }
                        }
                    } else {
                        ui.label("Renderer not initialized.");
                    }
                } else {
                    ui.label("GPU rendering not available.");
                }

                ui.separator();
                
                // Show fractal category information
                ui.label(egui::RichText::new("Fractal Categories:").strong());
                ui.label("• Classic 2D: Mandelbrot, Julia, Burning Ship");
                ui.label("• 3D Fractals: Mandelbulb, Mandelbox");
                ui.label("• Quaternion: Quaternion Julia");
                ui.label("• Modified: Nova, Phoenix, Buffalo");
                ui.label("• Celtic Variations: Celtic, Perpendicular");
                ui.label("• Conjugate: Mandelbar, Tricorn");
                ui.label("• Artistic: Feather");
                ui.label("• IFS Systems: Sierpinski, Koch Snowflake");
                ui.label("• L-Systems: Dragon Curve, IFS Tree");
                ui.label("• Chaotic: Lorenz, Rossler, Chen-Lee");
            });

            ui.separator();

            // Quick parameters (like Mandelbulb3D)
            ui.collapsing("Quick Parameters", |ui| {
                ui.add(egui::Slider::new(&mut self.max_iterations, 10..=500).text("Iterations"));
                ui.add(egui::Slider::new(&mut self.scale, 0.1..=10.0).text("Scale"));
                ui.add(egui::Slider::new(&mut self.power, 2.0..=16.0).text("Power"));
                ui.add(egui::Slider::new(&mut self.bailout, 1.0..=100.0).text("Bailout"));
                ui.add(egui::Slider::new(&mut self.color_saturation, 0.0..=2.0).text("Color Saturation"));
            });

            ui.separator();

            // Camera controls (like Mandelbulb3D)
            ui.collapsing("Camera", |ui| {
                ui.label("Position:");
                ui.add(egui::DragValue::new(&mut self.position[0]).speed(0.1).prefix("X: "));
                ui.add(egui::DragValue::new(&mut self.position[1]).speed(0.1).prefix("Y: "));
                ui.add(egui::DragValue::new(&mut self.position[2]).speed(0.1).prefix("Z: "));
                
                ui.label("Rotation:");
                ui.add(egui::DragValue::new(&mut self.rotation[0]).speed(1.0).prefix("X: "));
                ui.add(egui::DragValue::new(&mut self.rotation[1]).speed(1.0).prefix("Y: "));
                ui.add(egui::DragValue::new(&mut self.rotation[2]).speed(1.0).prefix("Z: "));
                
                if ui.button("Reset Camera").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                }
            });

            ui.separator();

            // Lighting (like Mandelbulb3D)
            ui.collapsing("Lighting", |ui| {
                ui.label("Directional Light");
                let mut dir = self.light_direction;
                ui.horizontal(|ui| {
                    ui.label("X");
                    ui.add(egui::DragValue::new(&mut dir[0]).speed(0.05));
                    ui.label("Y");
                    ui.add(egui::DragValue::new(&mut dir[1]).speed(0.05));
                    ui.label("Z");
                    ui.add(egui::DragValue::new(&mut dir[2]).speed(0.05));
                });
                if dir != self.light_direction {
                    self.save_state_for_undo();
                    self.light_direction = dir;
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_light_direction(self.light_direction);
                    }
                }

                ui.separator();
                ui.label("Light Color");
                let mut color = self.light_color;
                if ui.color_edit_button_rgb(&mut color).changed() {
                    self.save_state_for_undo();
                    self.light_color = color;
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_light_color(self.light_color);
                    }
                }

                ui.add(egui::Slider::new(&mut self.light_intensity, 0.0..=10.0).text("Intensity"));
                if ui.button("Apply Lighting").clicked() {
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_light_intensity(self.light_intensity);
                        r.set_light_direction(self.light_direction);
                        r.set_light_color(self.light_color);
                    }
                }

                if ui.button("Reset Lighting").clicked() {
                    self.save_state_for_undo();
                    self.light_direction = [0.6, 0.8, 0.3];
                    self.light_color = [1.0, 1.0, 1.0];
                    self.light_intensity = 1.0;
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_light_direction(self.light_direction);
                        r.set_light_color(self.light_color);
                        r.set_light_intensity(self.light_intensity);
                    }
                }
            });

            ui.separator();

            // Materials (like Mandelbulb3D)
            ui.collapsing("Materials", |ui| {
                ui.add(egui::Slider::new(&mut self.material_metallic, 0.0..=1.0).text("Metallic"));
                ui.add(egui::Slider::new(&mut self.material_roughness, 0.0..=1.0).text("Roughness"));

                if ui.button("Apply Material").clicked() {
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_material_metallic(self.material_metallic);
                        r.set_material_roughness(self.material_roughness);
                    }
                }

                if ui.button("Reset Materials").clicked() {
                    self.save_state_for_undo();
                    self.material_metallic = 0.05;
                    self.material_roughness = 0.85;
                    if let Some(r) = &mut self.fractal_renderer {
                        r.set_material_metallic(self.material_metallic);
                        r.set_material_roughness(self.material_roughness);
                    }
                }
            });
            
            ui.separator();

            // Code Editor (like TouchDesigner)
            ui.collapsing("📝 Code Editor", |ui| {
                ui.label("Fractal formula code editor");
                if ui.button("Open External Editor").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // This would launch an external editor in a real implementation
                    ui.label("External editor would open here...");
                }
                
                ui.separator();
                
                // Integrated code editor bound to app state
                self.code_editor.show(ui);
            });
        });

        // Status line
        if let Some(msg) = &self.status_message {
            ui.horizontal(|ui| {
                ui.label(msg);
            });
        }
    }

    // Compact scene overview tree used across workspaces
    fn show_scene_overview(&mut self, ui: &mut egui::Ui) {
        ui.collapsing("Fractal", |ui| {
            ui.label(format!(
                "Type: {}",
                self.selected_fractal_name()
            ));
            ui.label(format!(
                "Iterations: {} | Power: {:.2} | Bailout: {:.2}",
                self.max_iterations, self.power, self.bailout
            ));
            ui.label(format!(
                "Position: ({:.2}, {:.2}, {:.2}) | Scale: {:.2}",
                self.position[0], self.position[1], self.position[2], self.scale
            ));
        });

        ui.collapsing("Camera", |ui| {
            ui.label(format!("FOV: {:.1}°", self.camera_fov));
            ui.label(format!(
                "Target: ({:.2}, {:.2}, {:.2})",
                self.camera_target[0], self.camera_target[1], self.camera_target[2]
            ));
        });

        ui.collapsing("Lighting", |ui| {
            ui.label(format!(
                "Direction: ({:.2}, {:.2}, {:.2})",
                self.light_direction[0], self.light_direction[1], self.light_direction[2]
            ));
            ui.label(format!(
                "Color: ({:.2}, {:.2}, {:.2}) | Intensity: {:.2}",
                self.light_color[0], self.light_color[1], self.light_color[2], self.light_intensity
            ));
        });

        ui.collapsing("Materials", |ui| {
            ui.label(format!(
                "Metallic: {:.2} | Roughness: {:.2}",
                self.material_metallic, self.material_roughness
            ));
            ui.label(format!("Color Saturation: {:.2}", self.color_saturation));
        });
    }

    /// Show node editor panel
    fn show_node_editor(&mut self, ui: &mut egui::Ui) {
        ui.heading("Node Editor");
        ui.separator();
        
        // Get available size for the node editor
        let size = ui.max_rect().size();
        
        // Call node editor with context
        self.node_editor.show(ui, size);
    }

    /// Show fractal viewport
    fn show_fractal_viewport(&mut self, ui: &mut egui::Ui, ctx: &egui::Context) {
        let (rect, _response) = ui.allocate_exact_size(ui.max_rect().size(), egui::Sense::hover());
        // Precompute selected name to avoid borrowing `self` during renderer mutable borrow
        let selected_name = if self.selected_fractal < self.fractal_types.len() {
            self.fractal_types[self.selected_fractal]
        } else {
            "Mandelbulb (Power 8)"
        };
        
        // Try to get WGPU render state for GPU rendering
        if self.has_wgpu_support {
            // If we have a renderer, display it
            if let Some(renderer) = &mut self.fractal_renderer {
                // Update renderer parameters before rendering
                let mut params = crate::fractal::FractalParameters::default();
                params.max_iterations = self.max_iterations;
                params.bailout = self.bailout;
                params.scale = self.scale;
                params.position = nalgebra::Vector3::new(self.position[0], self.position[1], self.position[2]);
                params.rotation = nalgebra::Vector3::new(self.rotation[0], self.rotation[1], self.rotation[2]);
                params.color_saturation = self.color_saturation;
                
                // Set the fractal formula based on selection (aligns with fractal_types indices)
                params.formula = match self.selected_fractal {
                    // Mandelbulb variants
                    0 | 1 | 2 => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                    // Mandelbox variants
                    3 | 4 | 5 => crate::fractal::FractalFormula::Mandelbox { scale: self.scale },
                    // Quaternion Julia variants
                    6 | 7 => crate::fractal::FractalFormula::QuaternionJulia { c: [0.3, 0.5, 0.4, 0.2], max_iterations: self.max_iterations },
                    // Combiner presets and others default to single Mandelbulb for viewport
                    _ => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                };
                
                // Update renderer with current parameters
                renderer.update_parameters(&params);
                
                // Determine render target size from the allocated rect to avoid early ctx.input access
                let size = rect.size();
                let width = size.x.max(1.0).round() as u32;
                let height = size.y.max(1.0).round() as u32;
                let scaled_w = ((width as f32) * self.render_scale).clamp(1.0, 8192.0).round() as u32;
                let scaled_h = ((height as f32) * self.render_scale).clamp(1.0, 8192.0).round() as u32;
                // Track the last viewport resolution for export actions
                self.last_viewport_resolution = [scaled_w, scaled_h];
                
                // Render a frame directly to persistent GPU texture
                match renderer.render_frame_to_view(self.time, (scaled_w, scaled_h)) {
                    Ok(()) => {
                        // Display via registered Bevy Image texture if available; otherwise fallback later
                        let painter = ui.painter();
                        if let Some(texture_id) = self.viewport_texture {
                            painter.image(
                                texture_id,
                                rect,
                                egui::Rect::from_min_max(egui::pos2(0.0, 0.0), egui::pos2(1.0, 1.0)),
                                egui::Color32::WHITE,
                            );
                        } else {
                            // Ensure a visible placeholder when textures are not ready
                            painter.rect_filled(rect, 0.0, egui::Color32::from_gray(28));
                            painter.text(
                                egui::pos2(rect.center().x, rect.top()) + egui::vec2(0.0, 12.0),
                                egui::Align2::CENTER_TOP,
                                "GPU viewport texture not registered yet",
                                egui::FontId::proportional(14.0),
                                egui::Color32::from_rgb(200, 200, 200),
                            );
                        }
                        
                        // Show overlay information
                        let align_left = egui::Align2::LEFT_TOP;
                        painter.text(
                            rect.min + egui::vec2(10.0, 10.0),
                            align_left,
                            format!("Resolution: {}×{}", width, height),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(255, 255, 255),
                        );
                        
                        // Avoid borrow conflicts by computing name without borrowing `self` here
                        let name = selected_name;
                        painter.text(
                            rect.min + egui::vec2(10.0, 30.0),
                            align_left,
                            format!("Time: {:.1}s | Fractal: {}", self.time, name),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(255, 255, 255),
                        );
                        // GPU diagnostics HUD
                        let stats = renderer.stats();
    let backend = std::env::var("WGPU_BACKEND").unwrap_or_else(|_| "vulkan,dx12,gl".to_string());
                        let power = std::env::var("WGPU_POWER_PREF").unwrap_or_else(|_| "HighPerformance".to_string());
    let dx12_comp = std::env::var("WGPU_DX12_COMPILER").unwrap_or_else(|_| "fxc".to_string());
                        painter.text(
                            rect.min + egui::vec2(10.0, 50.0),
                            align_left,
                            format!(
                                "Frames: {} | Preview: {}x{} | Workgroups: {}x{}",
                                stats.frame_count,
                                stats.last_preview_size.0,
                                stats.last_preview_size.1,
                                stats.last_workgroups.0,
                                stats.last_workgroups.1
                            ),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(200, 255, 200),
                        );
                        painter.text(
                            rect.min + egui::vec2(10.0, 70.0),
                            align_left,
                            format!(
                                "Map time: {:.1} ms | Backend: {} | Power: {} | DX12: {}",
                                stats.last_map_time_ms,
                                backend,
                                power,
                                dx12_comp
                            ),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(200, 220, 255),
                        );

                        // If recording is active, pipe the current frame via CPU readback (export path unchanged)
                        if self.is_recording {
                            if let Some(recorder) = self.video_recorder.as_mut() {
                                match renderer.render_image_readback(self.time, (width, height)) {
                                    Ok((pixels, rw, rh)) => {
                                        if rw == width && rh == height {
                                            if let Err(e) = recorder.send_frame(&pixels) {
                                                log::error!("Failed to write video frame: {:?}", e);
                                            }
                                        } else {
                                            log::warn!("Recorded frame size mismatch: {}x{} vs {}x{}", rw, rh, width, height);
                                        }
                                    }
                                    Err(e) => {
                                        log::error!("Failed to render frame for recording: {}", e);
                                    }
                                }
                            }
                        }
                    }
                    Err(e) => {
                        // Renderer error
                        let painter = ui.painter();
                        painter.rect_filled(
                            rect,
                            4.0,
                            egui::Color32::from_rgb(50, 30, 30),
                        );
                        
                        let align_center = egui::Align2::CENTER_CENTER;
                        painter.text(
                            rect.center() - egui::vec2(0.0, 30.0),
                            align_center,
                            "❌ GPU Rendering Error",
                            egui::FontId::proportional(18.0),
                            egui::Color32::from_rgb(220, 100, 100),
                        );
                        
                        painter.text(
                            rect.center() - egui::vec2(0.0, 10.0),
                            align_center,
                            format!("{}", e),
                            egui::FontId::proportional(12.0),
                            egui::Color32::from_rgb(200, 180, 180),
                        );
                        
                        // Offer a retry option
                        let retry_rect = egui::Rect::from_center_size(
                            rect.center() + egui::vec2(0.0, 20.0),
                            egui::Vec2::new(100.0, 30.0)
                        );
                        
                        if ui.interact(retry_rect, egui::Id::new("retry_button"), egui::Sense::click()).clicked() {
                            // Attempt to reinitialize the renderer
                            self.has_wgpu_support = false;
                            self.viewport_texture = None;
                            self.fractal_renderer = None;
                        }
                        
                        painter.rect_filled(
                            retry_rect,
                            4.0,
                            egui::Color32::from_rgb(70, 70, 100),
                        );
                        
                        painter.text(
                            retry_rect.center(),
                            egui::Align2::CENTER_CENTER,
                            "Retry",
                            egui::FontId::proportional(14.0),
                            egui::Color32::WHITE,
                        );
                        
                        // Show additional help information
                        painter.text(
                            rect.center() + egui::vec2(0.0, 50.0),
                            align_center,
                            "Try resizing the window or restarting the application",
                            egui::FontId::proportional(10.0),
                            egui::Color32::from_rgb(180, 180, 200),
                        );
                    }
                }
            } else {
                // Renderer not initialized
                let painter = ui.painter();
                painter.rect_filled(
                    rect,
                    4.0,
                    egui::Color32::from_rgb(30, 35, 45),
                );
                
                let align_center = egui::Align2::CENTER_CENTER;
                painter.text(
                    rect.center() - egui::vec2(0.0, 20.0),
                    align_center,
                    "🎮 GPU Available",
                    egui::FontId::proportional(18.0),
                    egui::Color32::from_rgb(180, 200, 220),
                );
                
                painter.text(
                    rect.center() + egui::vec2(0.0, 20.0),
                    align_center,
                    "Initializing Renderer...",
                    egui::FontId::proportional(14.0),
                    egui::Color32::from_rgb(180, 200, 220),
                );
            }
        } else {
            // Fallback to CPU rendering or placeholder
            let painter = ui.painter();
            painter.rect_filled(
                rect,
                4.0,
                egui::Color32::from_rgb(25, 25, 35),
            );

            // Draw a simple 3D coordinate system
            let center = rect.center();
            
            // X axis (red)
            painter.line_segment(
                [center, center + egui::vec2(100.0, 0.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(255, 100, 100)),
            );
            
            // Y axis (green)
            painter.line_segment(
                [center, center + egui::vec2(0.0, -100.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(100, 255, 100)),
            );
            
            // Z axis (blue)
            painter.line_segment(
                [center, center + egui::vec2(70.0, 70.0)],
                egui::Stroke::new(3.0, egui::Color32::from_rgb(100, 100, 255)),
            );

            // Draw a simple 3D shape to represent a fractal
            let cube_size = 80.0;
            let cube_offset = egui::vec2(50.0, -50.0);
            
            // Front face
            let front_top_left = center + cube_offset + egui::vec2(-cube_size/2.0, -cube_size/2.0);
            let front_top_right = center + cube_offset + egui::vec2(cube_size/2.0, -cube_size/2.0);
            let front_bottom_left = center + cube_offset + egui::vec2(-cube_size/2.0, cube_size/2.0);
            let front_bottom_right = center + cube_offset + egui::vec2(cube_size/2.0, cube_size/2.0);
            
            painter.line_segment([front_top_left, front_top_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_top_right, front_bottom_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_bottom_right, front_bottom_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));
            painter.line_segment([front_bottom_left, front_top_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(200, 200, 220)));

            // Back face
            let back_offset = egui::vec2(30.0, 30.0);
            let back_top_left = front_top_left + back_offset;
            let back_top_right = front_top_right + back_offset;
            let back_bottom_left = front_bottom_left + back_offset;
            let back_bottom_right = front_bottom_right + back_offset;
            
            painter.line_segment([back_top_left, back_top_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_top_right, back_bottom_right], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_bottom_right, back_bottom_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([back_bottom_left, back_top_left], egui::Stroke::new(2.0, egui::Color32::from_rgb(150, 150, 180)));

            // Connecting lines
            painter.line_segment([front_top_left, back_top_left], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_top_right, back_top_right], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_bottom_left, back_bottom_left], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));
            painter.line_segment([front_bottom_right, back_bottom_right], egui::Stroke::new(1.0, egui::Color32::from_rgb(150, 150, 180)));

            // Label
            let align_center = egui::Align2::CENTER_CENTER;
            painter.text(
                rect.center() + egui::vec2(0.0, 100.0),
                align_center,
                "3D Fractal Viewport\n(GPU Not Available)",
                egui::FontId::proportional(14.0),
                egui::Color32::from_rgb(180, 180, 200),
            );
            
            // Show overlay information
            let align_left = egui::Align2::LEFT_TOP;
            painter.text(
                rect.min + egui::vec2(10.0, 10.0),
                align_left,
                format!("Time: {:.1}s | Fractal: {}", self.time, self.selected_fractal_name()),
                egui::FontId::proportional(14.0),
                egui::Color32::from_rgb(255, 255, 255),
            );
        }
    }

    /// Show timeline panel
    fn show_timeline_panel(&mut self, ui: &mut egui::Ui) {
        ui.heading("Timeline");

        ui.separator();

        ui.horizontal(|ui| {
            if ui.button("⏮").clicked() {
                self.time = 0.0;
            }
            if ui.button(if self.is_playing { "⏸" } else { "⏯" }).clicked() {
                self.is_playing = !self.is_playing;
                if self.is_playing {
                    self.last_update_instant = Some(Instant::now());
                } else {
                    self.last_update_instant = None;
                }
            }
            if ui.button("⏹").clicked() {
                self.is_playing = false;
                self.time = 0.0;
            }
            if ui.button("⏭").clicked() {
                self.time = (self.time + 1.0).min(self.timeline_duration);
            }
            ui.label(format!("Time: {:.2}s", self.time));
            
            ui.separator();
            ui.add(egui::Slider::new(&mut self.playback_speed, 0.1..=4.0).text("Speed"));
            ui.add(egui::Slider::new(&mut self.timeline_duration, 1.0..=600.0).text("Duration"));

            // Add a slider for time control
            ui.add(egui::Slider::new(&mut self.time, 0.0..=self.timeline_duration).text("Time"));
        });

        // Simple timeline visualization
        let timeline_rect_all = ui.max_rect();
        let timeline_height = 100.0;
        let timeline_rect = egui::Rect::from_min_size(
            timeline_rect_all.min,
            egui::Vec2::new(timeline_rect_all.width(), timeline_height)
        );
        // Reserve the space and draw into that rect
        let _resp = ui.allocate_rect(timeline_rect, egui::Sense::hover());
        let painter = ui.painter_at(timeline_rect);

        // Draw timeline background
        painter.rect_filled(
            timeline_rect,
            4.0,
            egui::Color32::from_rgb(30, 30, 40),
        );

        // Draw time marker
        let marker_x = timeline_rect.min.x + (self.time / self.timeline_duration.max(1.0)) * timeline_rect.width();
        let marker_pos = egui::Pos2::new(marker_x, timeline_rect.center().y);
        painter.circle_filled(
            marker_pos,
            8.0,
            egui::Color32::from_rgb(100, 200, 255),
        );

        // Draw time labels
        for i in 0..=10 {
            let x = timeline_rect.min.x + (i as f32 / 10.0) * timeline_rect.width();
            let y = timeline_rect.max.y - 20.0;
            painter.text(
                egui::Pos2::new(x, y),
                egui::Align2::CENTER_CENTER,
                format!("{:.0}s", (i as f32 / 10.0) * self.timeline_duration),
                egui::FontId::proportional(12.0),
                egui::Color32::from_rgb(200, 200, 200),
            );
        }
    }

    /// Show audio visualization panel
    fn show_audio_visualization(&mut self, ui: &mut egui::Ui, audio_data: &crate::audio::AudioData) {
        ui.heading("Audio Visualization");
        ui.separator();

        // Show audio levels
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Volume:").strong());
            ui.add(egui::ProgressBar::new(audio_data.volume).animate(true));
            ui.label(format!("{:.2}", audio_data.volume));
        });

        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Bass:").strong());
            ui.add(egui::ProgressBar::new(audio_data.bass_level).animate(true));
            ui.label(format!("{:.2}", audio_data.bass_level));
        });

        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Mid:").strong());
            ui.add(egui::ProgressBar::new(audio_data.mid_level).animate(true));
            ui.label(format!("{:.2}", audio_data.mid_level));
        });

        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Treble:").strong());
            ui.add(egui::ProgressBar::new(audio_data.treble_level).animate(true));
            ui.label(format!("{:.2}", audio_data.treble_level));
        });

        ui.separator();

        // Show beat detection
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Beat:").strong());
            let beat_color = if audio_data.beat > 0.5 {
                egui::Color32::from_rgb(255, 100, 100)
            } else {
                egui::Color32::from_rgb(100, 255, 100)
            };
            ui.add(egui::ProgressBar::new(audio_data.beat).animate(true).fill(beat_color));
            ui.label(format!("{:.2}", audio_data.beat));
        });

        ui.separator();

        // Show spectrum visualization
        ui.label("Frequency Spectrum:");
        let spectrum_rect_all = ui.max_rect();
        let spectrum_height = 100.0;
        let spectrum_rect = egui::Rect::from_min_size(
            spectrum_rect_all.min,
            egui::Vec2::new(spectrum_rect_all.width(), spectrum_height)
        );
        let _resp = ui.allocate_rect(spectrum_rect, egui::Sense::hover());
        let painter = ui.painter_at(spectrum_rect);

        // Draw spectrum background
        painter.rect_filled(
            spectrum_rect,
            4.0,
            egui::Color32::from_rgb(25, 25, 35),
        );

        // Draw spectrum bars
        let bar_width = spectrum_rect.width() / audio_data.spectrum.len() as f32;
        for (i, &value) in audio_data.spectrum.iter().enumerate() {
            let x = spectrum_rect.min.x + i as f32 * bar_width;
            let bar_height = value * spectrum_rect.height();
            let bar_rect = egui::Rect::from_min_size(
                egui::Pos2::new(x, spectrum_rect.max.y - bar_height),
                egui::Vec2::new(bar_width - 1.0, bar_height)
            );
            
            // Color based on frequency
            let color_value = (i as f32 / audio_data.spectrum.len() as f32 * 255.0) as u8;
            let bar_color = egui::Color32::from_rgb(color_value, 200, 255 - color_value);
            
            painter.rect_filled(bar_rect, 1.0, bar_color);
        }

        ui.separator();

        // Show waveform visualization
        ui.label("Waveform:");
        let waveform_rect_all = ui.max_rect();
        let waveform_height = 80.0;
        let waveform_rect = egui::Rect::from_min_size(
            waveform_rect_all.min,
            egui::Vec2::new(waveform_rect_all.width(), waveform_height)
        );
        let _resp = ui.allocate_rect(waveform_rect, egui::Sense::hover());
        let painter = ui.painter_at(waveform_rect);

        // Draw waveform background
        painter.rect_filled(
            waveform_rect,
            4.0,
            egui::Color32::from_rgb(30, 30, 40),
        );

        // Draw waveform line
        if !audio_data.waveform.is_empty() {
            let mut points = Vec::new();
            let step = waveform_rect.width() / audio_data.waveform.len() as f32;
            
            for (i, &value) in audio_data.waveform.iter().enumerate() {
                let x = waveform_rect.min.x + i as f32 * step;
                let y = waveform_rect.center().y - value * waveform_rect.height() / 2.0;
                points.push(egui::Pos2::new(x, y));
            }
            
            painter.add(egui::Shape::line(
                points,
                egui::Stroke::new(2.0, egui::Color32::from_rgb(100, 200, 255))
            ));
        }
    }

    /// Create a snapshot of the current application state
    fn create_snapshot(&self) -> AppStateSnapshot {
        AppStateSnapshot {
            time: self.time,
            selected_fractal: self.selected_fractal,
            max_iterations: self.max_iterations,
            bailout: self.bailout,
            power: self.power,
            scale: self.scale,
            position: self.position,
            rotation: self.rotation,
            color_saturation: self.color_saturation,
            camera_fov: self.camera_fov,
            camera_target: self.camera_target,
            light_direction: self.light_direction,
            light_color: self.light_color,
            light_intensity: self.light_intensity,
            material_metallic: self.material_metallic,
            material_roughness: self.material_roughness,
            current_workspace: self.current_workspace,
        }
    }

    /// Restore application state from a snapshot
    fn restore_snapshot(&mut self, snapshot: &AppStateSnapshot) {
        self.time = snapshot.time;
        self.selected_fractal = snapshot.selected_fractal;
        self.max_iterations = snapshot.max_iterations;
        self.bailout = snapshot.bailout;
        self.power = snapshot.power;
        self.scale = snapshot.scale;
        self.position = snapshot.position;
        self.rotation = snapshot.rotation;
        self.color_saturation = snapshot.color_saturation;
        self.camera_fov = snapshot.camera_fov;
        self.camera_target = snapshot.camera_target;
        self.light_direction = snapshot.light_direction;
        self.light_color = snapshot.light_color;
        self.light_intensity = snapshot.light_intensity;
        self.material_metallic = snapshot.material_metallic;
        self.material_roughness = snapshot.material_roughness;
        self.current_workspace = snapshot.current_workspace;

        if let Some(r) = &mut self.fractal_renderer {
            r.set_camera_fov(self.camera_fov);
            r.set_camera_target(self.camera_target);
            r.set_light_direction(self.light_direction);
            r.set_light_color(self.light_color);
            r.set_light_intensity(self.light_intensity);
            r.set_material_metallic(self.material_metallic);
            r.set_material_roughness(self.material_roughness);
        }
    }

    /// Save current state to undo stack
    fn save_state_for_undo(&mut self) {
        // Limit the size of the undo stack
        if self.undo_stack.len() >= self.max_undo_steps {
            self.undo_stack.remove(0);
        }
        
        self.undo_stack.push(self.create_snapshot());
        // Clear redo stack when new action is performed
        self.redo_stack.clear();
    }

    /// Undo the last action
    fn undo(&mut self) {
        if let Some(snapshot) = self.undo_stack.pop() {
            // Save current state to redo stack
            self.redo_stack.push(self.create_snapshot());
            // Restore the previous state
            self.restore_snapshot(&snapshot);
        }
    }

    /// Redo the last undone action
    fn redo(&mut self) {
        if let Some(snapshot) = self.redo_stack.pop() {
            // Save current state to undo stack
            self.undo_stack.push(self.create_snapshot());
            // Restore the redone state
            self.restore_snapshot(&snapshot);
        }
    }

    /// Reset parameters based on fractal type
    fn reset_parameters_for_fractal_type(&mut self, fractal_type: usize) {
        // First, handle presets by name to keep behavior aligned with the selection menu
        if fractal_type < self.fractal_types.len() {
            let name = self.fractal_types[fractal_type];
            match name {
                // Mandelbulb presets
                "Mandelbulb (Power 8)" => {
                    self.max_iterations = 60;
                    self.power = 8.0;
                    self.bailout = 4.0;
                    self.scale = 1.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }
                "Mandelbulb (Power 10)" => {
                    self.max_iterations = 65;
                    self.power = 10.0;
                    self.bailout = 4.0;
                    self.scale = 1.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }
                "Mandelbulb (Power 6)" => {
                    self.max_iterations = 55;
                    self.power = 6.0;
                    self.bailout = 4.0;
                    self.scale = 1.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }

                // Mandelbox presets
                "Mandelbox (Scale 2.0)" => {
                    self.max_iterations = 24;
                    self.scale = 2.0;
                    self.power = 2.0;
                    self.bailout = 4.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }
                "Mandelbox (Scale 1.8)" => {
                    self.max_iterations = 22;
                    self.scale = 1.8;
                    self.power = 2.0;
                    self.bailout = 4.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }
                "Mandelbox (Scale 2.2)" => {
                    self.max_iterations = 26;
                    self.scale = 2.2;
                    self.power = 2.0;
                    self.bailout = 4.0;
                    self.position = [0.0, 0.0, 0.0];
                    self.rotation = [0.0, 0.0, 0.0];
                    return;
                }

                // Quaternion Julia presets
                "Quaternion Julia (Classic)" => {
                    self.max_iterations = 90;
                    self.power = 2.0;
                    self.scale = 1.0;
                    self.bailout = 4.0;
                    self.rotation = [0.3, 0.5, 0.4];
                    self.position = [0.0, 0.0, 0.0];
                    return;
                }
                "Quaternion Julia (Variant)" => {
                    self.max_iterations = 90;
                    self.power = 2.0;
                    self.scale = 1.0;
                    self.bailout = 4.0;
                    self.rotation = [0.6, 0.2, 0.3];
                    self.position = [0.0, 0.0, 0.0];
                    return;
                }

                // Combiner presets – provide sensible defaults for color and iteration
                "Bulb ∪ Box (Union)" => {
                    self.max_iterations = 60;
                    self.power = 8.0; // base bulb detail
                    self.scale = 2.0; // box scale
                    self.bailout = 6.0;
                    self.color_saturation = 0.9;
                    return;
                }
                "Bulb ∩ Box (Intersection)" => {
                    self.max_iterations = 60;
                    self.power = 8.0;
                    self.scale = 2.0;
                    self.bailout = 6.0;
                    self.color_saturation = 0.85;
                    return;
                }
                "Bulb − Box (Subtraction)" => {
                    self.max_iterations = 60;
                    self.power = 8.0;
                    self.scale = 2.0;
                    self.bailout = 6.0;
                    self.color_saturation = 0.85;
                    return;
                }
                "Smooth Union (Bulb, Box)" => {
                    self.max_iterations = 70;
                    self.power = 8.0;
                    self.scale = 2.0;
                    self.bailout = 6.0;
                    self.color_saturation = 0.95;
                    return;
                }
                "Smooth Intersect (Box, QJulia)" => {
                    self.max_iterations = 85;
                    self.scale = 2.0;
                    self.rotation = [0.3, 0.5, 0.4];
                    self.bailout = 6.0;
                    self.color_saturation = 0.9;
                    return;
                }
                "Smooth Subtract (Bulb, QJulia)" => {
                    self.max_iterations = 85;
                    self.power = 8.0;
                    self.rotation = [0.6, 0.2, 0.3];
                    self.bailout = 6.0;
                    self.color_saturation = 0.9;
                    return;
                }
                "Triplet Smooth Union (Bulb+Box+QJulia)" => {
                    self.max_iterations = 90;
                    self.power = 8.0;
                    self.scale = 2.0;
                    self.rotation = [0.4, 0.4, 0.2];
                    self.bailout = 6.0;
                    self.color_saturation = 0.95;
                    return;
                }
                "Box ∩ QJulia (Intersection)" => {
                    self.max_iterations = 85;
                    self.scale = 2.0;
                    self.rotation = [0.35, 0.45, 0.25];
                    self.bailout = 6.0;
                    self.color_saturation = 0.9;
                    return;
                }
                _ => {}
            }
        }
        match fractal_type {
            0 => { // Mandelbrot
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [-0.5, 0.0, 0.0];
            }
            1 => { // Mandelbulb
                self.max_iterations = 50;
                self.scale = 1.0;
                self.power = 8.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            2 => { // Mandelbox
                self.max_iterations = 20;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            3 => { // Quaternion Julia
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            4 => { // Quaternion Julia (3D)
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            5 => { // Nova
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            6 => { // Phoenix
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            7 => { // Buffalo
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            8 => { // Celtic
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            9 => { // Perpendicular Mandelbrot
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            10 => { // Mandelbar
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            11 => { // Tricorn
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            12 => { // Feather
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            13 => { // Sierpinski
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            14 => { // Koch Snowflake
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            15 => { // Dragon Curve
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            16 => { // IFS Tree
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            17 => { // Lorenz Attractor
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            18 => { // Rossler Attractor
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            19 => { // Chen-Lee Attractor
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [0.0, 0.0, 0.0];
            }
            _ => {}
        }
    }
    
    /// Show MIDI controls panel
    fn show_midi_controls(&mut self, ui: &mut egui::Ui, midi_controller: &mut crate::audio::MidiController) {
        ui.heading("MIDI Controls");
        ui.separator();

        // Show current MIDI mappings
        ui.label(egui::RichText::new("MIDI Mappings:").strong());
        
        egui::ScrollArea::vertical().max_height(200.0).show(ui, |ui| {
            for (_, mapping) in &midi_controller.mappings {
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new(&mapping.parameter_name).monospace());
                    ui.label(format!("Channel: {}", mapping.channel));
                    ui.label(format!("Controller: {}", mapping.controller));
                    ui.label(format!("Range: {:.1} - {:.1}", mapping.min_value, mapping.max_value));
                    
                    // Show current value if available
                    if let Some(value) = midi_controller.current_values.get(&mapping.parameter_name) {
                        ui.label(egui::RichText::new(format!("Value: {:.2}", value)).strong());
                    }
                });
                ui.separator();
            }
        });

        ui.separator();

        // Show current MIDI values
        ui.label(egui::RichText::new("Current MIDI Values:").strong());
        
        egui::ScrollArea::vertical().max_height(150.0).show(ui, |ui| {
            for (param_name, value) in &midi_controller.current_values {
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new(param_name).monospace());
                    ui.add(egui::ProgressBar::new(*value / 10.0).animate(true)); // Normalize for display
                    ui.label(format!("{:.2}", value));
                });
            }
        });

        ui.separator();

        // Add mapping info (no unsafe state)
        ui.collapsing("Add MIDI Mapping", |ui| {
            ui.label("Create a new mapping:");
            ui.horizontal(|ui| {
                ui.label("Parameter:");
                ui.text_edit_singleline(&mut self.midi_param_name);
            });
            ui.horizontal(|ui| {
                ui.label("Channel:");
                let mut ch = self.midi_channel as i32;
                if ui.add(egui::DragValue::new(&mut ch).clamp_range(1..=16)).changed() {
                    self.midi_channel = ch as u8;
                }
                ui.label("CC:");
                let mut cc = self.midi_controller_cc as i32;
                if ui.add(egui::DragValue::new(&mut cc).clamp_range(0..=127)).changed() {
                    self.midi_controller_cc = cc as u8;
                }
            });
            ui.horizontal(|ui| {
                ui.label("Min:");
                ui.add(egui::DragValue::new(&mut self.midi_min_value).speed(0.01));
                ui.label("Max:");
                ui.add(egui::DragValue::new(&mut self.midi_max_value).speed(0.01));
            });
            ui.horizontal(|ui| {
                ui.label("Sensitivity:");
                ui.add(egui::DragValue::new(&mut self.midi_sensitivity).speed(0.01));
                ui.checkbox(&mut self.midi_invert, "Invert");
            });
            if ui.button("Create Mapping").clicked() {
                let mapping = crate::audio::MidiMapping {
                    channel: self.midi_channel,
                    controller: self.midi_controller_cc,
                    parameter_name: self.midi_param_name.clone(),
                    min_value: self.midi_min_value,
                    max_value: self.midi_max_value,
                };
                midi_controller.add_mapping(mapping);
                self.status_message = Some("MIDI mapping added".to_string());
            }
        });
    }

    /// Show OSC controls panel
    fn show_osc_controls(&mut self, ui: &mut egui::Ui, osc_controller: &mut crate::osc::OscController) {
        ui.heading("OSC Controls");
        ui.separator();

        // Show OSC server status
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("OSC Server:").strong());
            ui.label("Running on port 8000");
            if ui.button("Stop Server").clicked() {
                osc_controller.stop_server();
                self.status_message = Some("OSC server stopped".to_string());
            }
        });

        ui.separator();

        // Show current OSC mappings
        ui.label(egui::RichText::new("OSC Mappings:").strong());
        
        egui::ScrollArea::vertical().max_height(200.0).show(ui, |ui| {
            for (_, mapping) in &osc_controller.mappings {
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new(&mapping.osc_address).monospace());
                    ui.label(egui::RichText::new("→").weak());
                    ui.label(egui::RichText::new(&mapping.parameter_name).monospace());
                    ui.label(format!("Range: {:.1} - {:.1}", mapping.min_value, mapping.max_value));
                    
                    // Show current value if available
                    if let Some(value) = osc_controller.current_values.get(&mapping.parameter_name) {
                        ui.label(egui::RichText::new(format!("Value: {:.2}", value)).strong());
                    }
                });
                ui.separator();
            }
        });

        ui.separator();

        // Show current OSC values
        ui.label(egui::RichText::new("Current OSC Values:").strong());
        
        egui::ScrollArea::vertical().max_height(150.0).show(ui, |ui| {
            for (param_name, value) in &osc_controller.current_values {
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new(param_name).monospace());
                    ui.add(egui::ProgressBar::new(*value / 10.0).animate(true)); // Normalize for display
                    ui.label(format!("{:.2}", value));
                });
            }
        });

        ui.separator();

        // Add mapping info (no unsafe state)
        ui.collapsing("Add OSC Mapping", |ui| {
            ui.label("Create a new OSC mapping:");
            ui.horizontal(|ui| {
                ui.label("Address:");
                ui.text_edit_singleline(&mut self.osc_address);
            });
            ui.horizontal(|ui| {
                ui.label("Parameter:");
                ui.text_edit_singleline(&mut self.osc_param_name);
            });
            ui.horizontal(|ui| {
                ui.label("Min:");
                ui.add(egui::DragValue::new(&mut self.osc_min_value).speed(0.01));
                ui.label("Max:");
                ui.add(egui::DragValue::new(&mut self.osc_max_value).speed(0.01));
            });
            ui.horizontal(|ui| {
                ui.label("Sensitivity:");
                ui.add(egui::DragValue::new(&mut self.osc_sensitivity).speed(0.01));
                ui.checkbox(&mut self.osc_invert, "Invert");
            });
            if ui.button("Create Mapping").clicked() {
                let mapping = crate::osc::OscMapping {
                    osc_address: self.osc_address.clone(),
                    parameter_name: self.osc_param_name.clone(),
                    min_value: self.osc_min_value,
                    max_value: self.osc_max_value,
                    sensitivity: self.osc_sensitivity,
                    invert: self.osc_invert,
                };
                osc_controller.add_mapping(mapping);
                self.status_message = Some("OSC mapping added".to_string());
            }
        });
    }

    /// Show gesture controls panel
    fn show_gesture_controls(&mut self, ui: &mut egui::Ui, gesture_controller: &mut crate::gesture::GestureController) {
        ui.heading("Gesture Controls");
        ui.separator();

        // Show gesture device status
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("Devices:").strong());
            if gesture_controller.is_leap_motion_available {
                ui.label(egui::RichText::new("Leap Motion: Connected").color(egui::Color32::GREEN));
            } else {
                ui.label(egui::RichText::new("Leap Motion: Not Connected").color(egui::Color32::RED));
            }
            if gesture_controller.is_mediapipe_available {
                ui.label(egui::RichText::new("MediaPipe: Connected").color(egui::Color32::GREEN));
            } else {
                ui.label(egui::RichText::new("MediaPipe: Not Connected").color(egui::Color32::RED));
            }
        });

        ui.separator();

        // Show current gesture mappings
        ui.label(egui::RichText::new("Gesture Mappings:").strong());
        
        egui::ScrollArea::vertical().max_height(200.0).show(ui, |ui| {
            for (_, mapping) in &gesture_controller.parameter_mappings {
                ui.horizontal(|ui| {
                    ui.label(egui::RichText::new(&mapping.gesture_name).monospace());
                    ui.label(egui::RichText::new("→").weak());
                    ui.label(egui::RichText::new(&mapping.parameter_name).monospace());
                    ui.label(format!("Range: {:.1} - {:.1}", mapping.min_value, mapping.max_value));
                    ui.label(format!("Sensitivity: {:.1}", mapping.sensitivity));
                    
                    // Show if inverted
                    if mapping.invert {
                        ui.label(egui::RichText::new("(Inverted)").weak());
                    }
                });
                ui.separator();
            }
        });

        ui.separator();

        // Show current active gestures
        ui.label(egui::RichText::new("Active Gestures:").strong());
        
        {
            // Limit the lifetime of the lock guard to this block and avoid panics on poisoned mutex
            if let Ok(gesture_data) = gesture_controller.gesture_data.lock() {
                egui::ScrollArea::vertical().max_height(150.0).show(ui, |ui| {
                    for (gesture_name, value) in &gesture_data.active_gestures {
                        ui.horizontal(|ui| {
                            ui.label(egui::RichText::new(gesture_name).monospace());
                            ui.add(egui::ProgressBar::new(*value).animate(true));
                            ui.label(format!("{:.2}", value));
                        });
                    }

                    // Show hand positions if available
                    if !gesture_data.hand_positions.is_empty() {
                        ui.separator();
                        ui.label(egui::RichText::new("Hand Positions:").strong());
                        for (i, hand) in gesture_data.hand_positions.iter().enumerate() {
                            ui.label(format!("Hand {}: ({:.2}, {:.2}, {:.2})", i, hand.palm_position[0], hand.palm_position[1], hand.palm_position[2]));
                        }
                    }
                });
            } else {
                ui.label(egui::RichText::new("Gesture data unavailable").color(egui::Color32::RED));
            }
        }

        ui.separator();

        // Add mapping info (no unsafe state)
        ui.collapsing("Add Gesture Mapping", |ui| {
            ui.label("Create a new gesture mapping:");
            ui.horizontal(|ui| {
                ui.label("Gesture:");
                ui.text_edit_singleline(&mut self.gesture_name);
            });
            ui.horizontal(|ui| {
                ui.label("Parameter:");
                ui.text_edit_singleline(&mut self.gesture_param_name);
            });
            ui.horizontal(|ui| {
                ui.label("Min:");
                ui.add(egui::DragValue::new(&mut self.gesture_min_value).speed(0.01));
                ui.label("Max:");
                ui.add(egui::DragValue::new(&mut self.gesture_max_value).speed(0.01));
            });
            ui.horizontal(|ui| {
                ui.label("Sensitivity:");
                ui.add(egui::DragValue::new(&mut self.gesture_sensitivity).speed(0.01));
                ui.checkbox(&mut self.gesture_invert, "Invert");
            });
            if ui.button("Create Mapping").clicked() {
                let mapping = crate::gesture::GestureMapping {
                    gesture_name: self.gesture_name.clone(),
                    parameter_name: self.gesture_param_name.clone(),
                    min_value: self.gesture_min_value,
                    max_value: self.gesture_max_value,
                    sensitivity: self.gesture_sensitivity,
                    invert: self.gesture_invert,
                };
                gesture_controller.add_mapping(mapping);
                self.status_message = Some("Gesture mapping added".to_string());
            }
        });
    }
}

impl FractalStudioApp {
    pub fn update(&mut self, ctx: &egui::Context, audio_data: Option<&crate::audio::AudioData>, midi_controller: Option<&mut crate::audio::MidiController>, osc_controller: Option<&mut crate::osc::OscController>, gesture_controller: Option<&mut crate::gesture::GestureController>) {
        log::debug!("Update started");
        
        // Playback time progression using wall-clock delta
        if self.is_playing {
            let now = Instant::now();
            if let Some(prev) = self.last_update_instant {
                let dt = (now - prev).as_secs_f32();
                self.time = (self.time + dt * self.playback_speed).min(self.timeline_duration);
                if self.time >= self.timeline_duration {
                    self.is_playing = false;
                }
            }
            self.last_update_instant = Some(now);
        } else {
            self.last_update_instant = None;
        }
        
        // Apply controller-mapped values
        if let Some(midi) = midi_controller.as_deref() {
            let speed = midi.get_parameter("speed");
            if speed > 0.0 { self.playback_speed = speed.clamp(0.1, 4.0); }
            let zoom = midi.get_parameter("zoom");
            if zoom > 0.0 { self.scale = zoom; }
            let iters = midi.get_parameter("iterations");
            if iters > 0.0 { self.max_iterations = iters as u32; }
            let sat = midi.get_parameter("saturation");
            if sat > 0.0 { self.color_saturation = sat.clamp(0.0, 2.0); }
        }

        // WGSL shader hot-reload polling (debounced)
        if self.shader_hot_reload_enabled {
            let now = Instant::now();
            let can_poll = match self.last_hot_reload_poll {
                Some(prev) => (now - prev).as_millis() > 250,
                None => true,
            };
            if can_poll {
                self.last_hot_reload_poll = Some(now);
                if let Some(path) = &self.current_shader_path {
                    if let Ok(md) = std::fs::metadata(path) {
                        if let Ok(modified) = md.modified() {
                            let should_reload = match self.shader_last_modified {
                                Some(prev) => modified > prev,
                                None => true,
                            };
                            if should_reload {
                                match std::fs::read_to_string(path) {
                                    Ok(contents) => {
                                        if let Some(renderer) = &mut self.fractal_renderer {
                                            match renderer.load_fragment_wgsl(&contents) {
                                                Ok(()) => {
                                                    renderer.set_fragment_pseudo3d(true);
                                                    self.use_fragment_pseudo3d = true;
                                                    self.shader_last_modified = Some(modified);
                                                    self.status_message = Some(format!("Hot reloaded shader: {}", path.display()));
                                                }
                                                Err(e) => {
                                                    self.status_message = Some(format!("Hot reload failed: {}", e));
                                                }
                                            }
                                        }
                                    }
                                    Err(e) => {
                                        log::error!("Failed to read shader for hot reload: {}", e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if let Some(osc) = osc_controller.as_deref() {
            let speed = osc.get_parameter("speed");
            if speed > 0.0 { self.playback_speed = speed.clamp(0.1, 4.0); }
            let zoom = osc.get_parameter("zoom");
            if zoom > 0.0 { self.scale = zoom; }
            let iters = osc.get_parameter("iterations");
            if iters > 0.0 { self.max_iterations = iters as u32; }
            let sat = osc.get_parameter("saturation");
            if sat > 0.0 { self.color_saturation = sat.clamp(0.0, 2.0); }
        }
        if let Some(gesture) = gesture_controller.as_deref() {
            let speed = gesture.get_parameter("speed");
            if speed > 0.0 { self.playback_speed = speed.clamp(0.1, 4.0); }
            let zoom = gesture.get_parameter("zoom");
            if zoom > 0.0 { self.scale = zoom; }
            let iters = gesture.get_parameter("iterations");
            if iters > 0.0 { self.max_iterations = iters as u32; }
            let sat = gesture.get_parameter("saturation");
            if sat > 0.0 { self.color_saturation = sat.clamp(0.0, 2.0); }
        }

        // Update fractal renderer if available
        if let Some(renderer) = &mut self.fractal_renderer {
            // Update renderer parameters before rendering
            let mut params = crate::fractal::FractalParameters::default();
            params.max_iterations = self.max_iterations;
            params.bailout = self.bailout;
            params.scale = self.scale;
            // params.power = self.power; // Removed - power is part of the formula
            params.position = nalgebra::Vector3::new(self.position[0], self.position[1], self.position[2]);
            params.rotation = nalgebra::Vector3::new(self.rotation[0], self.rotation[1], self.rotation[2]);
            params.color_saturation = self.color_saturation;
            
            // Set the fractal formula based on selection
            params.formula = match self.selected_fractal {
                1 => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                2 => crate::fractal::FractalFormula::Mandelbox { scale: self.scale },
                4 => crate::fractal::FractalFormula::QuaternionJulia { c: [0.3, 0.5, 0.4, 0.2], max_iterations: self.max_iterations },
                _ => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
            };
            
            // Update renderer with current parameters (timed)
            let t0 = Instant::now();
            renderer.update_parameters(&params);
            let t1 = Instant::now();
            self.metrics.mark_renderer_update(t1.duration_since(t0));
        }

        // Main UI layout
        let t_top_0 = Instant::now();
        egui::TopBottomPanel::top("top_panel").show(ctx, |ui| {
            self.show_top_panel(ui);
        });
        let t_top_1 = Instant::now();
        self.metrics.mark_ui_top(t_top_1.duration_since(t_top_0));

        // GPU Settings window (GUI-only)
        egui::Window::new("GPU Settings")
            .open(&mut self.show_gpu_settings_window)
            .resizable(false)
            .show(ctx, |ui| {
                ui.label("Select GPU backend:");
                for backend in ["AUTO", "Vulkan", "DX12", "Metal", "GL"].iter() {
                    ui.radio_value(&mut self.gpu_backend, backend.to_string(), *backend);
                }

                ui.separator();
                ui.label("Power preference:");
                for pref in ["LowPower", "HighPerformance"].iter() {
                    ui.radio_value(&mut self.gpu_power_pref, pref.to_string(), *pref);
                }

                // DX12 compiler option (Windows only relevant)
                ui.separator();
                ui.label("DX12 compiler:");
                for comp in ["fxc", "dxcompiler"].iter() {
                    ui.radio_value(&mut self.gpu_dx12_compiler, comp.to_string(), *comp);
                }

                ui.separator();
                if ui.button("Apply Settings").clicked() {
                    // Apply environment variables so future GPU init uses them
                    std::env::set_var("WGPU_BACKEND", &self.gpu_backend);
                    std::env::set_var("WGPU_POWER_PREF", &self.gpu_power_pref);
                    std::env::set_var("WGPU_DX12_COMPILER", &self.gpu_dx12_compiler);
                    self.status_message = Some("GPU settings applied. Backend changes require restart.".to_string());
                }
                if ui.button("Apply & Restart").clicked() {
                    // Apply env and attempt a self-restart so new backend takes effect
                    std::env::set_var("WGPU_BACKEND", &self.gpu_backend);
                    std::env::set_var("WGPU_POWER_PREF", &self.gpu_power_pref);
                    std::env::set_var("WGPU_DX12_COMPILER", &self.gpu_dx12_compiler);
                    if let Ok(exe) = std::env::current_exe() {
                        let args: Vec<String> = std::env::args().skip(1).collect();
                        let mut cmd = std::process::Command::new(exe);
                        for (k, v) in [
                            ("WGPU_BACKEND", self.gpu_backend.clone()),
                            ("WGPU_POWER_PREF", self.gpu_power_pref.clone()),
                            ("WGPU_DX12_COMPILER", self.gpu_dx12_compiler.clone()),
                        ] {
                            cmd.env(k, v);
                        }
                        let _ = cmd.args(&args).spawn();
                        // Exit current process to allow the new instance to take over
                        std::process::exit(0);
                    } else {
                        self.status_message = Some("Failed to restart application. Please restart manually.".to_string());
                    }
                }
            });

        // Show workspace-specific panels
        match self.current_workspace {
            WorkspaceView::Modeling => {
                // Left panel for scene hierarchy and fractal controls
                if self.show_left_panel {
                    let t_left_0 = Instant::now();
                    egui::SidePanel::left("left_panel")
                        .default_width(250.0)
                        .show(ctx, |ui| {
                            ui.heading("Scene Hierarchy");
                            ui.separator();
                            self.show_scene_overview(ui);
                            
                            ui.separator();
                            ui.heading("Fractal Controls");
                            ui.separator();
                            self.show_fractal_controls(ui);
                        });
                    let t_left_1 = Instant::now();
                    self.metrics.mark_ui_left(t_left_1.duration_since(t_left_0));
                }

                // Right panel for properties and tools
                if self.show_right_panel {
                    let t_right_0 = Instant::now();
                    egui::SidePanel::right("right_panel")
                        .min_width(250.0)
                        .max_width(250.0)
                        .resizable(false)
                        .show(ctx, |ui| {
                            ui.heading("Properties");
                            ui.separator();
                            // Compact object properties overview
                            ui.label(format!(
                                "Fractal: {} | Iter: {} | Power: {:.2}",
                                self.selected_fractal_name(), self.max_iterations, self.power
                            ));
                            ui.label(format!(
                                "Camera FOV: {:.1}° | Target: ({:.1}, {:.1}, {:.1})",
                                self.camera_fov,
                                self.camera_target[0], self.camera_target[1], self.camera_target[2]
                            ));
                            ui.label(format!(
                                "Light dir: ({:.1}, {:.1}, {:.1}) | Intensity: {:.2}",
                                self.light_direction[0], self.light_direction[1], self.light_direction[2],
                                self.light_intensity
                            ));
                            
                            ui.separator();
                            ui.heading("Tools");
                            ui.separator();
                            ui.label("Quality Presets:");
                            ui.horizontal(|ui| {
                                if ui.button("Medium").clicked() {
                                    if let Some(renderer) = &mut self.fractal_renderer {
                                        renderer.apply_quality_preset(crate::fractal::types::QualityPreset::Medium, Some(self.last_viewport_resolution));
                                    }
                                }
                                if ui.button("Ultra").clicked() {
                                    if let Some(renderer) = &mut self.fractal_renderer {
                                        renderer.apply_quality_preset(crate::fractal::types::QualityPreset::Ultra, Some(self.last_viewport_resolution));
                                    }
                                }
                            });
                            
                            // Show audio visualization if audio data is available
                            if let Some(audio) = audio_data {
                                ui.separator();
                                self.show_audio_visualization(ui, audio);
                            }
                            
                            // Show MIDI controls if MIDI controller is available
                            if let Some(midi) = midi_controller {
                                ui.separator();
                                self.show_midi_controls(ui, midi);
                            }
                        });
                    let t_right_1 = Instant::now();
                    self.metrics.mark_ui_right(t_right_1.duration_since(t_right_0));
                }

                // Bottom panel for timeline
                if self.show_bottom_panel {
                    let t_bottom_0 = Instant::now();
                    egui::TopBottomPanel::bottom("bottom_panel")
                        .default_height(100.0)
                        .show(ctx, |ui| {
                            self.show_timeline_panel(ui);
                        });
                    let t_bottom_1 = Instant::now();
                    self.metrics.mark_ui_bottom(t_bottom_1.duration_since(t_bottom_0));
                }

                // Central panel for 3D viewport
                let t_viewport_0 = Instant::now();
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
                let t_viewport_1 = Instant::now();
                self.metrics.mark_viewport(t_viewport_1.duration_since(t_viewport_0));
            }
            
            WorkspaceView::Animation => {
                // Left panel for scene hierarchy and keyframe controls
                if self.show_left_panel {
                    egui::SidePanel::left("left_panel")
                        .default_width(250.0)
                        .show(ctx, |ui| {
                            ui.heading("Scene Hierarchy");
                            ui.separator();
                            self.show_scene_overview(ui);
                            
                            ui.separator();
                            ui.heading("Animation Controls");
                            ui.separator();
                            // Use existing timeline controls for functional content
                            self.show_timeline_panel(ui);
                        });
                }

                // Right panel for animation curves and properties
                if self.show_right_panel {
                    egui::SidePanel::right("right_panel")
                        .min_width(250.0)
                        .max_width(250.0)
                        .resizable(false)
                        .show(ctx, |ui| {
                            ui.heading("Animation Curves");
                            ui.separator();
                            // TODO: Show animation curves
                            ui.label("Animation curves would go here");
                            
                            ui.separator();
                            ui.heading("Keyframe Properties");
                            ui.separator();
                            ui.label("Keyframe properties would go here");
                            
                            // Show audio visualization if audio data is available
                            if let Some(audio) = audio_data {
                                ui.separator();
                                self.show_audio_visualization(ui, audio);
                            }
                            
                            // Show MIDI controls if MIDI controller is available
                            if let Some(midi) = midi_controller {
                                ui.separator();
                                self.show_midi_controls(ui, midi);
                            }
                            
                            // Show OSC controls if OSC controller is available
                            if let Some(osc) = osc_controller {
                                ui.separator();
                                self.show_osc_controls(ui, osc);
                            }
                            
                            // Show gesture controls if gesture controller is available
                            if let Some(gesture) = gesture_controller {
                                ui.separator();
                                self.show_gesture_controls(ui, gesture);
                            }
                        });
                }

                // Bottom panel for timeline
                if self.show_bottom_panel {
                    egui::TopBottomPanel::bottom("bottom_panel")
                        .default_height(150.0)
                        .show(ctx, |ui| {
                            self.show_timeline_panel(ui);
                        });
                }

                // Central panel for 3D viewport with animation preview
                let t_viewport_0 = Instant::now();
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
                let t_viewport_1 = Instant::now();
                self.metrics.mark_viewport(t_viewport_1.duration_since(t_viewport_0));
            }
            
            WorkspaceView::Rendering => {
                // Left panel for scene hierarchy and render settings
                if self.show_left_panel {
                    egui::SidePanel::left("left_panel")
                        .default_width(250.0)
                        .show(ctx, |ui| {
                            ui.heading("Scene Hierarchy");
                            ui.separator();
                            self.show_scene_overview(ui);
                            
                            ui.separator();
                            ui.heading("Render Settings");
                            ui.separator();
                            // Render scale control (supersampling). Scales internal render resolution.
                            let mut rs = self.render_scale;
                            if ui.add(egui::Slider::new(&mut rs, 0.5..=2.0).text("Render Scale")).changed() {
                                self.render_scale = rs;
                            }
                            ui.label("Higher values supersample for crisper edges.");
                            
                            // Fragment shader pseudo‑3D mode toggle (ShadPlay/ShaderToy style)
                            let mut toggled = self.use_fragment_pseudo3d;
                            if ui.checkbox(&mut toggled, "Fragment Pseudo‑3D Mode").changed() {
                                self.use_fragment_pseudo3d = toggled;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_fragment_pseudo3d(toggled);
                                }
                            }
                            ui.label("Uses a fragment raymarcher for fast iteration.");

                            // FXAA toggle (UI stub; renderer integration pending)
                            let mut fxaa = self.fxaa_enabled;
                            if ui.checkbox(&mut fxaa, "FXAA (anti‑aliasing)").changed() {
                                self.fxaa_enabled = fxaa;
                                // Future: wire to renderer post-process when available
                            }

                            ui.separator();
                            ui.collapsing("GPU Status", |ui| {
                                ui.label(format!("Backend: {}", self.gpu_backend));
                                ui.label(format!("WGPU support: {}", if self.has_wgpu_support { "Yes" } else { "No" }));
                                if let Some(renderer) = &self.fractal_renderer {
                                    let stats = renderer.stats();
                                    ui.label(format!("Last preview: {}x{}", stats.last_preview_size.0, stats.last_preview_size.1));
                                    ui.label(format!("Workgroups: {}x{}", stats.last_workgroups.0, stats.last_workgroups.1));
                                } else {
                                    ui.label("Renderer: Not initialized");
                                }
                                ui.label(format!("Viewport texture: {}", if self.viewport_texture.is_some() { "Ready" } else { "Not registered" }));
                            });

                            ui.separator();
                            ui.heading("Camera");
                            // Camera FOV slider (degrees)
                            let mut fov_val = self.camera_fov;
                            if ui.add(egui::Slider::new(&mut fov_val, 10.0..=120.0).text("FOV (degrees)"))
                                .changed()
                            {
                                self.camera_fov = fov_val;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_camera_fov(self.camera_fov);
                                }
                            }

                            // Camera Target controls (X, Y, Z)
                            let mut cam_target = self.camera_target;
                            ui.label("Camera Target");
                            ui.horizontal(|ui| {
                                ui.label("X");
                                ui.add(egui::DragValue::new(&mut cam_target[0]).speed(0.1));
                                ui.label("Y");
                                ui.add(egui::DragValue::new(&mut cam_target[1]).speed(0.1));
                                ui.label("Z");
                                ui.add(egui::DragValue::new(&mut cam_target[2]).speed(0.1));
                            });
                            if cam_target != self.camera_target {
                                self.camera_target = cam_target;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_camera_target(self.camera_target);
                                }
                            }

                            ui.separator();
                            ui.heading("Fractal Parameters");
                            // Scale (affects Mandelbox/Mandelbulb depending on selection)
                            let mut scale_val = self.scale;
                            if ui.add(egui::Slider::new(&mut scale_val, 0.25..=4.0).text("Scale")).changed() {
                                self.scale = scale_val;
                            }
                            // Bailout radius
                            let mut bailout_val = self.bailout;
                            if ui.add(egui::Slider::new(&mut bailout_val, 0.5..=16.0).text("Bailout")).changed() {
                                self.bailout = bailout_val;
                            }
                            // Max iterations
                            let mut iter_val = self.max_iterations;
                            if ui.add(egui::Slider::new(&mut iter_val, 1..=400).text("Max Iterations")).changed() {
                                self.max_iterations = iter_val;
                            }

                            ui.separator();
                            ui.heading("Lighting");
                            // Directional light vector
                            let mut dir = self.light_direction;
                            ui.label("Directional Light");
                            ui.horizontal(|ui| {
                                ui.label("X");
                                ui.add(egui::DragValue::new(&mut dir[0]).speed(0.05));
                                ui.label("Y");
                                ui.add(egui::DragValue::new(&mut dir[1]).speed(0.05));
                                ui.label("Z");
                                ui.add(egui::DragValue::new(&mut dir[2]).speed(0.05));
                            });
                            if dir != self.light_direction {
                                self.save_state_for_undo();
                                self.light_direction = dir;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_light_direction(self.light_direction);
                                }
                            }

                            // Light color picker
                            ui.label("Light Color");
                            let mut color = self.light_color;
                            if ui.color_edit_button_rgb(&mut color).changed() {
                                self.save_state_for_undo();
                                self.light_color = color;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_light_color(self.light_color);
                                }
                            }

                            // Light intensity
                            let mut intensity = self.light_intensity;
                            if ui.add(egui::Slider::new(&mut intensity, 0.0..=10.0).text("Intensity")).changed() {
                                self.save_state_for_undo();
                                self.light_intensity = intensity;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_light_intensity(self.light_intensity);
                                }
                            }
                        });
                }

                // Right panel for material editor and render output
                if self.show_right_panel {
                    egui::SidePanel::right("right_panel")
                        .min_width(250.0)
                        .max_width(250.0)
                        .resizable(false)
                        .show(ctx, |ui| {
                            ui.heading("Material Editor");
                            ui.separator();
                            // Materials controls
                            let mut metallic = self.material_metallic;
                            if ui.add(egui::Slider::new(&mut metallic, 0.0..=1.0).text("Metallic")).changed() {
                                self.save_state_for_undo();
                                self.material_metallic = metallic;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_material_metallic(self.material_metallic);
                                }
                            }

                            let mut roughness = self.material_roughness;
                            if ui.add(egui::Slider::new(&mut roughness, 0.0..=1.0).text("Roughness")).changed() {
                                self.save_state_for_undo();
                                self.material_roughness = roughness;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_material_roughness(self.material_roughness);
                                }
                            }

                            // Color tuning
                            let mut sat = self.color_saturation;
                            if ui.add(egui::Slider::new(&mut sat, 0.0..=2.0).text("Color Saturation")).changed() {
                                self.save_state_for_undo();
                                self.color_saturation = sat;
                                // color_saturation influences shader params during frame update
                            }
                            
                            ui.separator();
                            ui.heading("Render Output");
                            ui.separator();
                            ui.label("Render output would go here");
                            
                            // Show audio visualization if audio data is available
                            if let Some(audio) = audio_data {
                                ui.separator();
                                self.show_audio_visualization(ui, audio);
                            }
                            
                            // Show MIDI controls if MIDI controller is available
                            if let Some(midi) = midi_controller {
                                ui.separator();
                                self.show_midi_controls(ui, midi);
                            }
                            
                            // Show OSC controls if OSC controller is available
                            if let Some(osc) = osc_controller {
                                ui.separator();
                                self.show_osc_controls(ui, osc);
                            }
                            
                            // Show gesture controls if gesture controller is available
                            if let Some(gesture) = gesture_controller {
                                ui.separator();
                                self.show_gesture_controls(ui, gesture);
                            }
                        });
                }

                // Bottom panel for render queue
                if self.show_bottom_panel {
                    egui::TopBottomPanel::bottom("bottom_panel")
                        .default_height(120.0)
                        .show(ctx, |ui| {
                            ui.heading("Render Queue");
                            ui.separator();
                            ui.label("Render queue would go here");
                        });
                }

                // Central panel for render viewport
                let t_viewport_0 = Instant::now();
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
                let t_viewport_1 = Instant::now();
                self.metrics.mark_viewport(t_viewport_1.duration_since(t_viewport_0));
            }
            
            WorkspaceView::ShaderLoader => {
                // Left panel for shader loading and controls
                if self.show_left_panel {
                    egui::SidePanel::left("left_panel")
                        .default_width(260.0)
                        .show(ctx, |ui| {
                            ui.heading("Shader Loader");
                            ui.separator();

                            // Hot-reload toggle and library refresh
                            ui.horizontal(|ui| {
                                ui.checkbox(&mut self.shader_hot_reload_enabled, "Hot Reload WGSL");
                                if ui.button("Refresh Library").clicked() {
                                    self.refresh_shader_library();
                                }
                            });
                            ui.separator();

                            // Library listing from assets/shaders with search filter
                            ui.collapsing("Available Shaders (assets/shaders)", |ui| {
                                ui.horizontal(|ui| {
                                    ui.label("Filter:");
                                    ui.text_edit_singleline(&mut self.shader_filter_text);
                                });
                                if self.shader_library.is_empty() {
                                    ui.label("No WGSL shaders found.");
                                } else {
                                    let filter = self.shader_filter_text.to_lowercase();
                                    for p in &self.shader_library {
                                        let name = p.file_name().and_then(|s| s.to_str()).unwrap_or("<unnamed>");
                                        let path_str = p.to_string_lossy().to_lowercase();
                                        if !filter.is_empty() && !name.to_lowercase().contains(&filter) && !path_str.contains(&filter) {
                                            continue;
                                        }
                                        if ui.button(name).clicked() {
                                            match std::fs::read_to_string(p) {
                                                Ok(contents) => {
                                                    if let Some(renderer) = &mut self.fractal_renderer {
                                                        match renderer.load_fragment_wgsl(&contents) {
                                                            Ok(()) => {
                                                                self.use_fragment_pseudo3d = true;
                                                                renderer.set_fragment_pseudo3d(true);
                                                                self.current_shader_path = Some(p.clone());
                                                                self.shader_last_modified = std::fs::metadata(p).and_then(|m| m.modified()).ok();
                                                                self.status_message = Some(format!("Loaded shader: {}", name));
                                                            }
                                                            Err(e) => {
                                                                self.status_message = Some(format!("Failed to load WGSL: {}", e));
                                                            }
                                                        }
                                                    } else {
                                                        self.status_message = Some("Renderer not initialized".to_string());
                                                    }
                                                }
                                                Err(err) => {
                                                    self.status_message = Some(format!("Failed to read shader: {}", err));
                                                }
                                            }
                                        }
                                    }
                                }
                            });

                            if ui.button("Load WGSL Shader…").clicked() {
                                if let Some(path) = rfd::FileDialog::new()
                                    .add_filter("WGSL Shader", &["wgsl"])
                                    .set_directory(std::path::Path::new("assets/shaders"))
                                    .pick_file()
                                {
                                    match std::fs::read_to_string(&path) {
                                        Ok(contents) => {
                                            if let Some(renderer) = &mut self.fractal_renderer {
                                                match renderer.load_fragment_wgsl(&contents) {
                                                    Ok(()) => {
                                                        self.use_fragment_pseudo3d = true;
                                                        renderer.set_fragment_pseudo3d(true);
                                                        self.current_shader_path = Some(path);
                                                        // Record last modified for hot-reload
                                                        if let Some(p) = &self.current_shader_path {
                                                            self.shader_last_modified = std::fs::metadata(p).and_then(|m| m.modified()).ok();
                                                        }
                                                        self.status_message = Some("Shader loaded and applied to fragment pipeline".to_string());
                                                    }
                                                    Err(e) => {
                                                        self.status_message = Some(format!("Failed to load WGSL: {}", e));
                                                    }
                                                }
                                            } else {
                                                self.status_message = Some("Renderer not initialized".to_string());
                                            }
                                        }
                                        Err(err) => {
                                            self.status_message = Some(format!("Failed to read file: {}", err));
                                        }
                                    }
                                }
                            }

                            ui.separator();
                            ui.label("Examples");
                            ui.horizontal_wrapped(|ui| {
                                if ui.button("Load Basic Pseudo3D").clicked() {
                                    let path = std::path::PathBuf::from("assets/shaders/pseudo3d_basic.wgsl");
                                    match std::fs::read_to_string(&path) {
                                        Ok(contents) => {
                                            if let Some(renderer) = &mut self.fractal_renderer {
                                                match renderer.load_fragment_wgsl(&contents) {
                                                    Ok(()) => {
                                                        self.use_fragment_pseudo3d = true;
                                                        renderer.set_fragment_pseudo3d(true);
                                                        self.current_shader_path = Some(path);
                                                        self.shader_last_modified = std::fs::metadata(&self.current_shader_path.as_ref().unwrap()).and_then(|m| m.modified()).ok();
                                                        self.status_message = Some("Loaded example: Basic Pseudo3D".to_string());
                                                    }
                                                    Err(e) => {
                                                        self.status_message = Some(format!("Error loading example shader: {}", e));
                                                    }
                                                }
                                            } else {
                                                self.status_message = Some("Renderer not initialized".to_string());
                                            }
                                        }
                                        Err(err) => {
                                            self.status_message = Some(format!("Failed to read example shader: {}", err));
                                        }
                                    }
                                }

                                if ui.button("Load Mandelbox Pseudo3D").clicked() {
                                    let path = std::path::PathBuf::from("assets/shaders/pseudo3d_mandelbox.wgsl");
                                    match std::fs::read_to_string(&path) {
                                        Ok(contents) => {
                                            if let Some(renderer) = &mut self.fractal_renderer {
                                                match renderer.load_fragment_wgsl(&contents) {
                                                    Ok(()) => {
                                                        self.use_fragment_pseudo3d = true;
                                                        renderer.set_fragment_pseudo3d(true);
                                                        self.current_shader_path = Some(path);
                                                        self.shader_last_modified = std::fs::metadata(&self.current_shader_path.as_ref().unwrap()).and_then(|m| m.modified()).ok();
                                                        self.status_message = Some("Loaded example: Mandelbox Pseudo3D".to_string());
                                                    }
                                                    Err(e) => {
                                                        self.status_message = Some(format!("Error loading example shader: {}", e));
                                                    }
                                                }
                                            } else {
                                                self.status_message = Some("Renderer not initialized".to_string());
                                            }
                                        }
                                        Err(err) => {
                                            self.status_message = Some(format!("Failed to read example shader: {}", err));
                                        }
                                    }
                                }
                            });

                            ui.separator();
                            ui.heading("Fragment Pseudo‑3D");
                            let mut toggled = self.use_fragment_pseudo3d;
                            if ui.checkbox(&mut toggled, "Enable fragment raymarching").changed() {
                                self.use_fragment_pseudo3d = toggled;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_fragment_pseudo3d(toggled);
                                }
                            }
                            ui.label("Uses a fragment raymarcher for fast iteration.");

                            ui.separator();
                            ui.heading("Camera");
                            let mut fov_val = self.camera_fov;
                            if ui.add(egui::Slider::new(&mut fov_val, 10.0..=120.0).text("FOV (degrees)")).changed() {
                                self.camera_fov = fov_val;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_camera_fov(self.camera_fov);
                                }
                            }

                            // Camera Target controls (X, Y, Z)
                            let mut cam_target = self.camera_target;
                            ui.label("Camera Target");
                            ui.horizontal(|ui| {
                                ui.label("X");
                                ui.add(egui::DragValue::new(&mut cam_target[0]).speed(0.1));
                                ui.label("Y");
                                ui.add(egui::DragValue::new(&mut cam_target[1]).speed(0.1));
                                ui.label("Z");
                                ui.add(egui::DragValue::new(&mut cam_target[2]).speed(0.1));
                            });
                            if cam_target != self.camera_target {
                                self.camera_target = cam_target;
                                if let Some(renderer) = &mut self.fractal_renderer {
                                    renderer.set_camera_target(self.camera_target);
                                }
                            }

                            ui.separator();
                            ui.heading("Fractal Parameters");
                            let mut scale_val = self.scale;
                            if ui.add(egui::Slider::new(&mut scale_val, 0.25..=4.0).text("Scale")).changed() {
                                self.scale = scale_val;
                            }
                            let mut bailout_val = self.bailout;
                            if ui.add(egui::Slider::new(&mut bailout_val, 0.5..=16.0).text("Bailout")).changed() {
                                self.bailout = bailout_val;
                            }
                            let mut iter_val = self.max_iterations;
                            if ui.add(egui::Slider::new(&mut iter_val, 1..=400).text("Max Iterations")).changed() {
                                self.max_iterations = iter_val;
                            }
                        });
                }

                // Right panel shows status and metadata
                if self.show_right_panel {
                    egui::SidePanel::right("right_panel")
                        .min_width(250.0)
                        .max_width(250.0)
                        .resizable(false)
                        .show(ctx, |ui| {
                            ui.heading("Shader Info");
                            ui.separator();
                            if let Some(path) = &self.current_shader_path {
                                ui.label(format!("Loaded: {}", path.display()));
                            } else {
                                ui.label("No shader loaded");
                            }

                            ui.separator();
                            ui.heading("Status");
                            if let Some(msg) = &self.status_message {
                                ui.label(msg);
                            } else {
                                ui.label("Ready");
                            }
                        });
                }

                // Central panel for viewport
                let t_viewport_0 = Instant::now();
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
                let t_viewport_1 = Instant::now();
                self.metrics.mark_viewport(t_viewport_1.duration_since(t_viewport_0));
            }
            
            WorkspaceView::NodeEditor => {
                // Left panel for node library
                if self.show_left_panel {
                    egui::SidePanel::left("left_panel")
                        .default_width(250.0)
                        .show(ctx, |ui| {
                            ui.heading("Node Library");
                            ui.separator();
                            // TODO: Show node library with categories
                            ui.label("Fractal Nodes");
                            ui.label("Math Nodes");
                            ui.label("Color Nodes");
                            ui.label("Animation Nodes");
                            ui.label("Utility Nodes");
                        });
                }

                // Right panel for node properties
                if self.show_right_panel {
                    egui::SidePanel::right("right_panel")
                        .min_width(250.0)
                        .max_width(250.0)
                        .resizable(false)
                        .show(ctx, |ui| {
                            ui.heading("Node Properties");
                            ui.separator();
                            // TODO: Show selected node properties
                            ui.label("Node properties would go here");
                            
                            // Show audio visualization if audio data is available
                            if let Some(audio) = audio_data {
                                ui.separator();
                                self.show_audio_visualization(ui, audio);
                            }
                            
                            // Show MIDI controls if MIDI controller is available
                            if let Some(midi) = midi_controller {
                                ui.separator();
                                self.show_midi_controls(ui, midi);
                            }
                            
                            // Show OSC controls if OSC controller is available
                            if let Some(osc) = osc_controller {
                                ui.separator();
                                self.show_osc_controls(ui, osc);
                            }
                            
                            // Show gesture controls if gesture controller is available
                            if let Some(gesture) = gesture_controller {
                                ui.separator();
                                self.show_gesture_controls(ui, gesture);
                            }
                        });
                }

                // No bottom panel in node editor (or minimal status bar)

                // Central panel for node editor canvas
                let t_viewport_0 = Instant::now();
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_node_editor(ui);
                });
                let t_viewport_1 = Instant::now();
                self.metrics.mark_viewport(t_viewport_1.duration_since(t_viewport_0));
            }
        }

        log::debug!("Update completed");
    }
}

impl FractalStudioApp {
    pub fn selected_fractal_name(&self) -> &'static str {
        if self.selected_fractal < self.fractal_types.len() {
            self.fractal_types[self.selected_fractal]
        } else {
            "Mandelbulb (Power 8)"
        }
    }

    pub fn fractal_name_for(selected_fractal: usize) -> &'static str {
        // Static fallback list aligned with default presets
        const PRESETS: [&'static str; 16] = [
            "Mandelbulb (Power 8)",
            "Mandelbulb (Power 10)",
            "Mandelbulb (Power 6)",
            "Mandelbox (Scale 2.0)",
            "Mandelbox (Scale 1.8)",
            "Mandelbox (Scale 2.2)",
            "Quaternion Julia (Classic)",
            "Quaternion Julia (Variant)",
            "Bulb ∪ Box (Union)",
            "Bulb ∩ Box (Intersection)",
            "Bulb − Box (Subtraction)",
            "Smooth Union (Bulb, Box)",
            "Smooth Intersect (Box, QJulia)",
            "Smooth Subtract (Bulb, QJulia)",
            "Triplet Smooth Union (Bulb+Box+QJulia)",
            "Box ∩ QJulia (Intersection)",
        ];
        if selected_fractal < PRESETS.len() { PRESETS[selected_fractal] } else { PRESETS[0] }
    }
}

/// Run the GUI application
pub fn run_gui() -> Result<(), Box<dyn std::error::Error>> {
    // This function is now a placeholder as the actual implementation is in gui.rs
    // The Bevy-based GUI is started from src/gui.rs
    log::info!("GUI functionality is implemented in the Bevy-based system");
    Ok(())
}
