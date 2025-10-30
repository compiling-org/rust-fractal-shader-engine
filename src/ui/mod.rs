//! UI Module for Fractal Shader Editor
//! 
//! Standalone eframe + egui application for the modular fractal shader system.
//! This replaces the problematic bevy_egui with a stable standalone UI.

pub mod main;

// Re-export main application
pub use main::{FractalShaderApp, NodeGraphUI, VisualNodePane, FractalShaderInfo, ShaderParam, ShaderParamType};

// Version information
pub const VERSION: &str = "1.0.0";
pub const APP_NAME: &str = "Modular Fractal Shader Editor";
pub const AUTHOR: &str = "Kapil";

/// Quick macro for creating hash maps
#[macro_export]
macro_rules! hashmap {
    ($($key:expr => $value:expr,)*) => {
        {
            let mut map = std::collections::HashMap::new();
            $(map.insert($key, $value);)*
            map
        }
    };
    ($($key:expr => $value:expr),*) => {
        hashmap!($($key => $value,)*)
    };
}

// Export the macro for use throughout the crate
pub use hashmap;