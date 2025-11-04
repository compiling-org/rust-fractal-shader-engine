//! Project Management Module
//!
//! This module handles project serialization, deserialization, and management
//! for the Fractal Studio application.
//!
//! ## File Format Design
//!
//! Fractal Studio uses its own proprietary project format (.fract) for several key reasons:
//!
//! 1. **Comprehensive Data Storage**: Unlike MB3D which is primarily a mesh format,
//!    our .fract format stores the complete application state including:
//!    - Node graphs and connections
//!    - Animation timelines and keyframes
//!    - Material definitions and shader parameters
//!    - Camera positions and view settings
//!    - User interface layouts and preferences
//!    - Procedural generation parameters
//!
//! 2. **Extensibility**: Our JSON-based format can easily accommodate new features
//!    without breaking compatibility with older projects.
//!
//! 3. **Cross-Platform Compatibility**: The text-based JSON format ensures projects
//!    work identically across Windows, macOS, and Linux.
//!
//! 4. **Version Control Friendly**: Text-based format works well with Git and other
//!    version control systems, allowing for meaningful diffs and merges.
//!
//! 5. **Human Readable**: Developers and power users can inspect and modify projects
//!    directly if needed.
//!
//! MB3D files, while popular in some fractal tools, are limited to storing mesh data
//! and basic parameters. They don't support our advanced features like:
//! - Node-based procedural generation
//! - Complex animation systems
//! - Real-time audio/MIDI integration
//! - Advanced material systems
//! - Custom shader parameters
//!
//! For these reasons, we've chosen to develop our own comprehensive format that can
//! fully represent all aspects of a Fractal Studio project.

use serde::{Deserialize, Serialize};
use std::path::Path;

/// Main project structure for Fractal Studio
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FractalStudioProject {
    /// Project metadata
    pub metadata: ProjectMetadata,
    
    /// 3D scene with fractal objects
    pub scene: crate::scene::Scene,
    
    /// Animation timeline
    pub animation_controller: crate::animation::AnimationController,
    
    /// Node graph for visual programming
    pub node_graph: crate::nodes::NodeGraph,
    
    /// Asset references
    pub assets: Vec<String>, // Paths to referenced assets
    
    /// Version information
    pub version: String,
    
    /// Export settings and preferences
    pub export_settings: ExportSettings,
}

/// Project metadata
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ProjectMetadata {
    /// Project name
    pub name: String,
    
    /// Project description
    pub description: String,
    
    /// Creation timestamp
    pub created_at: String, // ISO 8601 format
    
    /// Last modified timestamp
    pub modified_at: String, // ISO 8601 format
    
    /// Project author
    pub author: String,
    
    /// Project tags
    pub tags: Vec<String>,
}

/// Export settings and preferences
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ExportSettings {
    /// Default export format for images
    pub default_image_format: String,
    
    /// Default export format for meshes
    pub default_mesh_format: String,
    
    /// Default export format for animations
    pub default_animation_format: String,
    
    /// Export directory
    pub export_directory: String,
    
    /// Whether to automatically export on render
    pub auto_export: bool,
}

impl FractalStudioProject {
    /// Create a new project
    pub fn new(name: &str) -> Self {
        let now = chrono::Utc::now().to_rfc3339();
        
        Self {
            metadata: ProjectMetadata {
                name: name.to_string(),
                description: String::new(),
                created_at: now.clone(),
                modified_at: now,
                author: whoami::username(),
                tags: Vec::new(),
            },
            scene: crate::scene::Scene::new(name),
            animation_controller: crate::animation::AnimationController::new(),
            node_graph: crate::nodes::NodeGraph::new(),
            assets: Vec::new(),
            version: env!("CARGO_PKG_VERSION").to_string(),
            export_settings: ExportSettings {
                default_image_format: "png".to_string(),
                default_mesh_format: "obj".to_string(),
                default_animation_format: "mp4".to_string(),
                export_directory: "./exports".to_string(),
                auto_export: false,
            },
        }
    }
    
    /// Save project to file
    pub fn save_to_file(&self, path: &Path) -> Result<(), Box<dyn std::error::Error>> {
        let json = serde_json::to_string_pretty(self)?;
        std::fs::write(path, json)?;
        Ok(())
    }
    
    /// Load project from file
    pub fn load_from_file(path: &Path) -> Result<Self, Box<dyn std::error::Error>> {
        let json = std::fs::read_to_string(path)?;
        let project: FractalStudioProject = serde_json::from_str(&json)?;
        Ok(project)
    }
    
    /// Update modification timestamp
    pub fn update_timestamp(&mut self) {
        self.metadata.modified_at = chrono::Utc::now().to_rfc3339();
    }
    
    /// Get the default file extension for Fractal Studio projects
    pub fn file_extension() -> &'static str {
        "fract"
    }
    
    /// Get the file filter for file dialogs
    pub fn file_filter() -> (String, Vec<String>) {
        ("Fractal Studio Project".to_string(), vec!["*.fract".to_string()])
    }
    
    /// Check if a file path has the correct extension
    pub fn is_valid_project_file(path: &Path) -> bool {
        if let Some(ext) = path.extension() {
            ext == "fract"
        } else {
            false
        }
    }
    
    /// Import from other formats (limited support)
    pub fn import_from_format(path: &Path) -> Result<Self, Box<dyn std::error::Error>> {
        // For now, we only support our own format
        // In the future, we could add limited import support for:
        // - OBJ files (meshes only)
        // - Image sequences (as animation frames)
        // - Simple JSON configurations
        //
        // Note: MB3D import is not planned because:
        // 1. MB3D is primarily a mesh format with limited parameter storage
        // 2. It doesn't support our node-based workflow
        // 3. It lacks animation and material information
        // 4. Our .fract format is more comprehensive and extensible
        
        if Self::is_valid_project_file(path) {
            Self::load_from_file(path)
        } else {
            Err(format!("Unsupported file format: {:?}", path.extension()).into())
        }
    }
    
    /// Export to other formats (limited support)
    pub fn export_to_format(&self, path: &Path) -> Result<(), Box<dyn std::error::Error>> {
        // For now, we only support our own format
        // In the future, we could add export support for:
        // - OBJ (meshes only)
        // - GLTF (with limitations)
        // - Image sequences
        // - Simple JSON configurations
        
        if Self::is_valid_project_file(path) {
            self.save_to_file(path)
        } else {
            Err(format!("Unsupported file format: {:?}", path.extension()).into())
        }
    }
}

impl Default for FractalStudioProject {
    fn default() -> Self {
        Self::new("Untitled Project")
    }
}