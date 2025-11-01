//! UI Module for Fractal Generator
//!
//! This module provides the comprehensive user interface for the fractal generator,
//! adapted from the WGSL Shader Studio architecture for fractal-specific functionality.

pub mod main;
pub mod theme;
pub mod node_editor;
pub mod fractal_ui;

pub use main::*;
pub use theme::*;
pub use node_editor::*;
pub use fractal_ui::*;