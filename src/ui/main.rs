//! Main UI Application Module
//!
//! This module provides the main egui application for the Fractal Shader Studio,
//! integrating the GPU renderer with the user interface.

use bevy::prelude::*;
use bevy_egui::EguiContexts;
use rfd::FileDialog;
use crate::ui::node_editor::NodeEditor;
use crate::fractal::FractalRenderer;
use crate::project::FractalStudioProject;
use std::sync::Arc;

/// Main application state
pub struct FractalStudioApp {
    // Fractal parameters
    pub time: f32,
    pub selected_fractal: usize,
    pub fractal_types: Vec<&'static str>,
    pub node_editor: NodeEditor,

    // GPU renderer
    pub fractal_renderer: Option<FractalRenderer>,
    pub viewport_texture: Option<egui::TextureId>,
    pub has_wgpu_support: bool,

    // Workspace management
    pub current_workspace: WorkspaceView,

    // Project management
    pub current_project: FractalStudioProject,

    // Default fractal parameters - reduced for better performance
    pub max_iterations: u32,
    pub bailout: f32,
    pub power: f32,
    pub scale: f32,
    pub position: [f32; 3],
    pub rotation: [f32; 3],
    pub color_saturation: f32,
    
    // Undo/Redo system
    pub undo_stack: Vec<AppStateSnapshot>,
    pub redo_stack: Vec<AppStateSnapshot>,
    pub max_undo_steps: usize,
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
    pub current_workspace: WorkspaceView,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum WorkspaceView {
    Modeling,
    Animation,
    Rendering,
    NodeEditor,
}

impl Default for FractalStudioApp {
    fn default() -> Self {
        Self {
            time: 0.0,
            selected_fractal: 0,
            fractal_types: vec![
                "Mandelbrot",
                "Mandelbulb",
                "Mandelbox",
                "Quaternion Julia",
                "Burning Ship",
                "Nova",
                "Phoenix",
                "Buffalo",
                "Celtic",
                "Perpendicular Mandelbrot",
                "Mandelbar",
                "Tricorn",
                "Feather",
                "Sierpinski",
                "Koch Snowflake",
                "Dragon Curve",
                "IFS Tree",
                "Lorenz Attractor",
                "Rossler Attractor",
                "Chen-Lee Attractor",
            ],
            node_editor: NodeEditor::new(),
            fractal_renderer: None,
            viewport_texture: None,
            has_wgpu_support: false,
            current_workspace: WorkspaceView::Modeling,
            current_project: FractalStudioProject::default(),
            // Default fractal parameters - reduced for better performance
            max_iterations: 50,     // Reduced from 100
            bailout: 4.0,
            power: 8.0,
            scale: 2.0,
            position: [0.0, 0.0, 0.0],
            rotation: [0.0, 0.0, 0.0],
            color_saturation: 1.0,
            // Undo/Redo system
            undo_stack: Vec::new(),
            redo_stack: Vec::new(),
            max_undo_steps: 50,
        }
    }
}

impl FractalStudioApp {
    pub fn new() -> Self {
        log::info!("Creating FractalStudioApp instance");

        let mut app = Self::default();
        app.has_wgpu_support = false; // Will be set when WGPU context is available

        log::info!("FractalStudioApp created successfully");
        app
    }

    pub fn initialize_wgpu(&mut self, device: Arc<bevy::render::renderer::RenderDevice>, queue: Arc<bevy::render::renderer::RenderQueue>, width: u32, height: u32) {
        log::info!("Initializing WGPU renderer with size {}x{}", width, height);
        
        // Limit texture dimensions to device limits (typically 8192)
        let max_dimension = 8192;
        let actual_width = std::cmp::min(width, max_dimension);
        let actual_height = std::cmp::min(height, max_dimension);
        
        match FractalRenderer::new_with_wgpu_context(
            device,
            queue,
            actual_width,
            actual_height,
        ) {
            Ok(renderer) => {
                self.fractal_renderer = Some(renderer);
                self.has_wgpu_support = true;
                log::info!("Fractal renderer initialized successfully");
            }
            Err(e) => {
                log::error!("Failed to initialize fractal renderer: {}", e);
                self.has_wgpu_support = false;
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
                        match crate::project::FractalStudioProject::load_from_file(&path) {
                            Ok(project) => {
                                self.current_project = project;
                                log::info!("Project loaded successfully from {:?}", path);
                            }
                            Err(e) => {
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
                        // Export current view as image
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Image Files", &["png", "jpg", "tiff"])
                            .save_file()
                        {
                            log::info!("Exporting image to {:?}", path);
                            // TODO: Implement image export
                        }
                        ui.close();
                    }
                    if ui.button("Export Animation...").clicked() {
                        // Export animation sequence
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Video Files", &["mp4", "avi", "mov"])
                            .save_file()
                        {
                            log::info!("Exporting animation to {:?}", path);
                            // TODO: Implement animation export
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
                if ui.button("Show/Hide Left Panel").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Toggle left panel visibility
                    log::info!("Toggle left panel");
                    // TODO: Implement panel toggle
                    ui.close();
                }
                if ui.button("Show/Hide Right Panel").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Toggle right panel visibility
                    log::info!("Toggle right panel");
                    // TODO: Implement panel toggle
                    ui.close();
                }
                if ui.button("Show/Hide Bottom Panel").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Toggle bottom panel visibility
                    log::info!("Toggle bottom panel");
                    // TODO: Implement panel toggle
                    ui.close();
                }
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
                if ui.button("Mandelbrot").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    self.selected_fractal = 0;
                    self.reset_parameters_for_fractal_type(0);
                    ui.close();
                }
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
                    
                    self.selected_fractal = 3;
                    self.reset_parameters_for_fractal_type(3);
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
                    
                    // Render current view as high-quality image
                    log::info!("Rendering image");
                    // TODO: Implement image rendering
                    ui.close();
                }
                if ui.button("Render Animation").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Render animation sequence
                    log::info!("Rendering animation");
                    // TODO: Implement animation rendering
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
                    
                    // Render current viewport
                    log::info!("Rendering viewport");
                    // TODO: Implement viewport rendering
                    ui.close();
                }
                ui.separator();
                
                // Render quality
                ui.label("Render Quality:");
                if ui.button("Preview").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Set preview quality
                    log::info!("Setting preview quality");
                    // TODO: Implement quality settings
                    ui.close();
                }
                if ui.button("Production").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // Set production quality
                    log::info!("Setting production quality");
                    // TODO: Implement quality settings
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
                    self.selected_fractal = new_selection;
                    self.reset_parameters_for_fractal_type(new_selection);
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
                ui.label("Lighting setup (TODO)");
                if ui.button("Add Light").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Add light
                }
                if ui.button("Reset Lighting").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Reset lighting
                }
            });

            ui.separator();

            // Materials (like Mandelbulb3D)
            ui.collapsing("Materials", |ui| {
                ui.label("Material editor (TODO)");
                if ui.button("New Material").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Create material
                }
                if ui.button("Reset Materials").clicked() {
                    // Save current state for undo
                    self.save_state_for_undo();
                    
                    // TODO: Reset materials
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
                
                // Simple code editor placeholder
                egui::ScrollArea::vertical()
                    .max_height(200.0)
                    .show(ui, |ui| {
                        ui.add(
                            egui::TextEdit::multiline(&mut String::new())
                                .font(egui::TextStyle::Monospace)
                                .code_editor()
                                .desired_width(f32::INFINITY)
                                .desired_rows(10)
                                .hint_text("// Fractal formula code would go here...")
                        );
                    });
            });
        });
    }

    /// Show node editor panel
    fn show_node_editor(&mut self, ui: &mut egui::Ui) {
        ui.heading("Node Editor");
        ui.separator();
        
        // Get available size for the node editor
        let size = ui.available_rect_before_wrap().size();
        
        // Call node editor with context
        self.node_editor.show(ui, size);
    }

    /// Show fractal viewport
    fn show_fractal_viewport(&mut self, ui: &mut egui::Ui, ctx: &egui::Context) {
        let (rect, _response) = ui.allocate_exact_size(ui.available_size(), egui::Sense::hover());
        
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
                
                // Set the fractal formula based on selection
                params.formula = match self.selected_fractal {
                    0 => crate::fractal::FractalFormula::Mandelbrot { center: [-0.5, 0.0], zoom: 1.0 },
                    1 => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                    2 => crate::fractal::FractalFormula::Mandelbox { scale: self.scale },
                    3 => crate::fractal::FractalFormula::Julia { c: [-0.7, 0.27015], max_iterations: self.max_iterations },
                    _ => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                };
                
                // Update renderer with current parameters
                renderer.update_parameters(&params);
                
                // Actually render a frame
                let screen_size = ctx.input(|i| i.screen_rect.size());
                let width = screen_size.x as u32;
                let height = screen_size.y as u32;
                
                // Render a frame and get the texture
                match renderer.render_frame_to_texture(self.time, (width, height), ctx) {
                    Ok(texture_id) => {
                        // Display the texture
                        let painter = ui.painter();
                        painter.image(
                            texture_id,
                            rect,
                            egui::Rect::from_min_max(egui::pos2(0.0, 0.0), egui::pos2(1.0, 1.0)),
                            egui::Color32::WHITE,
                        );
                        
                        // Show overlay information
                        let align_left = egui::Align2::LEFT_TOP;
                        painter.text(
                            rect.min + egui::vec2(10.0, 10.0),
                            align_left,
                            format!("Resolution: {}×{}", width, height),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(255, 255, 255),
                        );
                        
                        painter.text(
                            rect.min + egui::vec2(10.0, 30.0),
                            align_left,
                            format!("Time: {:.1}s | Fractal: {}", self.time, self.fractal_types[self.selected_fractal]),
                            egui::FontId::proportional(14.0),
                            egui::Color32::from_rgb(255, 255, 255),
                        );
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
                            rect.center() - egui::vec2(0.0, 20.0),
                            align_center,
                            "❌ GPU Rendering Error",
                            egui::FontId::proportional(18.0),
                            egui::Color32::from_rgb(220, 100, 100),
                        );
                        
                        painter.text(
                            rect.center() + egui::vec2(0.0, 20.0),
                            align_center,
                            format!("{}", e),
                            egui::FontId::proportional(12.0),
                            egui::Color32::from_rgb(200, 180, 180),
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
                format!("Time: {:.1}s | Fractal: {}", self.time, self.fractal_types[self.selected_fractal]),
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
            if ui.button("⏯").clicked() {
                // TODO: Implement play/pause
            }
            if ui.button("⏹").clicked() {
                // TODO: Implement stop
            }
            if ui.button("⏭").clicked() {
                self.time += 1.0;
            }
            ui.label(format!("Time: {:.2}s", self.time));
            
            // Add a slider for time control
            ui.add(egui::Slider::new(&mut self.time, 0.0..=100.0).text("Time"));
        });

        // Simple timeline visualization
        let timeline_rect = ui.available_rect_before_wrap();
        let timeline_height = 100.0;
        let timeline_rect = egui::Rect::from_min_size(
            timeline_rect.min,
            egui::Vec2::new(timeline_rect.width(), timeline_height)
        );
        
        ui.allocate_ui_at_rect(timeline_rect, |ui| {
            let painter = ui.painter();
            
            // Draw timeline background
            painter.rect_filled(
                timeline_rect,
                4.0,
                egui::Color32::from_rgb(30, 30, 40),
            );
            
            // Draw time marker
            let marker_x = timeline_rect.min.x + (self.time / 100.0) * timeline_rect.width();
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
                    format!("{}s", i * 10),
                    egui::FontId::proportional(12.0),
                    egui::Color32::from_rgb(200, 200, 200),
                );
            }
        });
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
        let spectrum_rect = ui.available_rect_before_wrap();
        let spectrum_height = 100.0;
        let spectrum_rect = egui::Rect::from_min_size(
            spectrum_rect.min,
            egui::Vec2::new(spectrum_rect.width(), spectrum_height)
        );
        
        ui.allocate_ui_at_rect(spectrum_rect, |ui| {
            let painter = ui.painter();
            
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
        });

        ui.separator();

        // Show waveform visualization
        ui.label("Waveform:");
        let waveform_rect = ui.available_rect_before_wrap();
        let waveform_height = 80.0;
        let waveform_rect = egui::Rect::from_min_size(
            waveform_rect.min,
            egui::Vec2::new(waveform_rect.width(), waveform_height)
        );
        
        ui.allocate_ui_at_rect(waveform_rect, |ui| {
            let painter = ui.painter();
            
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
        });
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
        self.current_workspace = snapshot.current_workspace;
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
            4 => { // Burning Ship
                self.max_iterations = 100;
                self.scale = 1.0;
                self.power = 2.0;
                self.bailout = 4.0;
                self.position = [-0.5, -0.5, 0.0];
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
    fn show_midi_controls(&mut self, ui: &mut egui::Ui, midi_controller: &crate::audio::MidiController) {
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

        // Add mapping controls
        ui.collapsing("Add MIDI Mapping", |ui| {
            ui.label("Create new MIDI parameter mappings:");
            
            // TODO: Implement MIDI mapping creation UI
            ui.label("MIDI mapping creation would go here");
        });
    }

    /// Show OSC controls panel
    fn show_osc_controls(&mut self, ui: &mut egui::Ui, osc_controller: &crate::osc::OscController) {
        ui.heading("OSC Controls");
        ui.separator();

        // Show OSC server status
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("OSC Server:").strong());
            ui.label("Running on port 8000");
            if ui.button("Stop Server").clicked() {
                // TODO: Implement OSC server stop
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

        // Add mapping controls
        ui.collapsing("Add OSC Mapping", |ui| {
            ui.label("Create new OSC parameter mappings:");
            
            // TODO: Implement OSC mapping creation UI
            ui.label("OSC mapping creation would go here");
        });
    }

    /// Show gesture controls panel
    fn show_gesture_controls(&mut self, ui: &mut egui::Ui, gesture_controller: &crate::gesture::GestureController) {
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
        
        // Get current gesture data
        let gesture_data = gesture_controller.gesture_data.lock().unwrap();
        
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

        ui.separator();

        // Add mapping controls
        ui.collapsing("Add Gesture Mapping", |ui| {
            ui.label("Create new gesture parameter mappings:");
            
            // TODO: Implement gesture mapping creation UI
            ui.label("Gesture mapping creation would go here");
        });
    }
}

impl FractalStudioApp {
    pub fn update(&mut self, ctx: &egui::Context, audio_data: Option<&crate::audio::AudioData>, midi_controller: Option<&crate::audio::MidiController>, osc_controller: Option<&crate::osc::OscController>, gesture_controller: Option<&crate::gesture::GestureController>) {
        log::debug!("Update started");
        
        // Update time for animations
        self.time += ctx.input(|i| i.unstable_dt);
        
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
                0 => crate::fractal::FractalFormula::Mandelbrot { center: [-0.5, 0.0], zoom: 1.0 },
                1 => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
                2 => crate::fractal::FractalFormula::Mandelbox { scale: self.scale },
                3 => crate::fractal::FractalFormula::Julia { c: [-0.7, 0.27015], max_iterations: self.max_iterations },
                _ => crate::fractal::FractalFormula::Mandelbulb { power: self.power },
            };
            
            // Update renderer with current parameters
            renderer.update_parameters(&params);
        }

        // Main UI layout
        egui::TopBottomPanel::top("top_panel").show(ctx, |ui| {
            self.show_top_panel(ui);
        });

        // Show workspace-specific panels
        match self.current_workspace {
            WorkspaceView::Modeling => {
                // Left panel for scene hierarchy and fractal controls
                egui::SidePanel::left("left_panel")
                    .default_width(250.0)
                    .show(ctx, |ui| {
                        ui.heading("Scene Hierarchy");
                        ui.separator();
                        // TODO: Show scene hierarchy
                        ui.label("Scene objects would go here");
                        
                        ui.separator();
                        ui.heading("Fractal Controls");
                        ui.separator();
                        self.show_fractal_controls(ui);
                    });

                // Right panel for properties and tools
                egui::SidePanel::right("right_panel")
                    .default_width(250.0)
                    .show(ctx, |ui| {
                        ui.heading("Properties");
                        ui.separator();
                        // TODO: Show selected object properties
                        ui.label("Object properties would go here");
                        
                        ui.separator();
                        ui.heading("Tools");
                        ui.separator();
                        ui.label("Modeling tools would go here");
                        
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

                // Bottom panel for timeline
                egui::TopBottomPanel::bottom("bottom_panel")
                    .default_height(100.0)
                    .show(ctx, |ui| {
                        self.show_timeline_panel(ui);
                    });

                // Central panel for 3D viewport
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
            }
            
            WorkspaceView::Animation => {
                // Left panel for scene hierarchy and keyframe controls
                egui::SidePanel::left("left_panel")
                    .default_width(250.0)
                    .show(ctx, |ui| {
                        ui.heading("Scene Hierarchy");
                        ui.separator();
                        // TODO: Show scene hierarchy
                        ui.label("Scene objects would go here");
                        
                        ui.separator();
                        ui.heading("Animation Controls");
                        ui.separator();
                        ui.label("Animation controls would go here");
                    });

                // Right panel for animation curves and properties
                egui::SidePanel::right("right_panel")
                    .default_width(250.0)
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

                // Bottom panel for timeline
                egui::TopBottomPanel::bottom("bottom_panel")
                    .default_height(150.0)
                    .show(ctx, |ui| {
                        self.show_timeline_panel(ui);
                    });

                // Central panel for 3D viewport with animation preview
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
            }
            
            WorkspaceView::Rendering => {
                // Left panel for scene hierarchy and render settings
                egui::SidePanel::left("left_panel")
                    .default_width(250.0)
                    .show(ctx, |ui| {
                        ui.heading("Scene Hierarchy");
                        ui.separator();
                        // TODO: Show scene hierarchy
                        ui.label("Scene objects would go here");
                        
                        ui.separator();
                        ui.heading("Render Settings");
                        ui.separator();
                        ui.label("Render settings would go here");
                    });

                // Right panel for material editor and render output
                egui::SidePanel::right("right_panel")
                    .default_width(250.0)
                    .show(ctx, |ui| {
                        ui.heading("Material Editor");
                        ui.separator();
                        // TODO: Show material editor
                        ui.label("Material editor would go here");
                        
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

                // Bottom panel for render queue
                egui::TopBottomPanel::bottom("bottom_panel")
                    .default_height(120.0)
                    .show(ctx, |ui| {
                        ui.heading("Render Queue");
                        ui.separator();
                        ui.label("Render queue would go here");
                    });

                // Central panel for render viewport
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_fractal_viewport(ui, ctx);
                });
            }
            
            WorkspaceView::NodeEditor => {
                // Left panel for node library
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

                // Right panel for node properties
                egui::SidePanel::right("right_panel")
                    .default_width(250.0)
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

                // No bottom panel in node editor (or minimal status bar)

                // Central panel for node editor canvas
                egui::CentralPanel::default().show(ctx, |ui| {
                    self.show_node_editor(ui);
                });
            }
        }

        log::debug!("Update completed");
    }
}

/// Run the GUI application
pub fn run_gui() -> Result<(), Box<dyn std::error::Error>> {
    // This function is now a placeholder as the actual implementation is in gui.rs
    // The Bevy-based GUI is started from src/gui.rs
    log::info!("GUI functionality is implemented in the Bevy-based system");
    Ok(())
}