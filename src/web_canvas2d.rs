//! Web deployment module for WASM compilation
//!
//! This module provides web-specific functionality for deploying the fractal generator
//! in web browsers using WASM and WebGPU/WebGL.

use wasm_bindgen::prelude::*;
use web_sys::{console, window, CanvasRenderingContext2d, HtmlCanvasElement};

// When the `wee_alloc` feature is enabled, use `wee_alloc` as the global
// allocator.
#[cfg(feature = "wee_alloc")]
#[global_allocator]
static ALLOC: wee_alloc::WeeAlloc = wee_alloc::WeeAlloc::INIT;

/// Web-specific fractal studio application
#[wasm_bindgen]
pub struct WebFractalStudio {
    canvas: HtmlCanvasElement,
    context: CanvasRenderingContext2d,
    time: f32,
    fractal_params: crate::FractalParameters,
}

#[wasm_bindgen]
impl WebFractalStudio {
    /// Create a new web fractal studio instance
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<WebFractalStudio, JsValue> {
        // Get the canvas element
        let document = web_sys::window()
            .unwrap()
            .document()
            .unwrap();

        let canvas = document
            .get_element_by_id(canvas_id)
            .unwrap()
            .dyn_into::<web_sys::HtmlCanvasElement>()?;

        // Get the 2D rendering context
        let context = canvas
            .get_context("2d")?
            .unwrap()
            .dyn_into::<web_sys::CanvasRenderingContext2d>()?;

        console::log_1(&"Web Fractal Studio initialized".into());

        Ok(WebFractalStudio {
            canvas,
            context,
            time: 0.0,
            fractal_params: crate::FractalParameters::default(),
        })
    }

    /// Set fractal parameters
    #[wasm_bindgen]
    pub fn set_parameter(&mut self, name: &str, value: f32) {
        match name {
            "iterations" => self.fractal_params.iterations = value as u32,
            "zoom" => self.fractal_params.zoom = value,
            "power" => self.fractal_params.power = value,
            "bailout" => self.fractal_params.bailout = value,
            "offset_x" => self.fractal_params.offset.0 = value,
            "offset_y" => self.fractal_params.offset.1 = value,
            _ => {}
        }
    }

    /// Set fractal type
    #[wasm_bindgen]
    pub fn set_fractal_type(&mut self, fractal_type: &str) {
        self.fractal_params.fractal_type = match fractal_type {
            "mandelbrot" => crate::FractalType::Mandelbrot,
            "julia" => crate::FractalType::Julia,
            "mandelbulb" => crate::FractalType::Mandelbulb,
            "mandelbox" => crate::FractalType::Mandelbox,
            _ => crate::FractalType::Mandelbrot,
        };
    }

    /// Render a frame
    #[wasm_bindgen]
    pub fn render_frame(&mut self, delta_time: f32) {
        self.time += delta_time;

        // Clear canvas
        let width = self.canvas.width() as f64;
        let height = self.canvas.height() as f64;
        self.context.clear_rect(0.0, 0.0, width, height);

        // Simple fractal rendering (placeholder for full WebGPU implementation)
        self.render_simple_fractal();
    }

    /// Render a simple fractal using Canvas 2D API
    fn render_simple_fractal(&self) {
        let width = self.canvas.width();
        let height = self.canvas.height();

        // Create image data
        let image_data = web_sys::ImageData::new_with_u8_clamped_array(
            wasm_bindgen::Clamped(&self.generate_fractal_pixels(width, height)),
            width,
        ).unwrap();

        self.context.put_image_data(&image_data, 0.0, 0.0).unwrap();
    }

    /// Generate fractal pixel data
    fn generate_fractal_pixels(&self, width: u32, height: u32) -> Vec<u8> {
        let mut pixels = Vec::with_capacity((width * height * 4) as usize);

        for y in 0..height {
            for x in 0..width {
                let px = (x as f32 / width as f32 - 0.5) * 4.0 / self.fractal_params.zoom;
                let py = (y as f32 / height as f32 - 0.5) * 4.0 / self.fractal_params.zoom;

                let cx = px + self.fractal_params.offset.0;
                let cy = py + self.fractal_params.offset.1;

                let (iterations, escaped) = self.compute_fractal_point(cx, cy);

                let color = if escaped {
                    self.fractal_color(iterations)
                } else {
                    (0, 0, 0) // Black for points in the set
                };

                pixels.push(color.0); // R
                pixels.push(color.1); // G
                pixels.push(color.2); // B
                pixels.push(255);     // A
            }
        }

        pixels
    }

    /// Compute fractal value at a point
    fn compute_fractal_point(&self, cx: f32, cy: f32) -> (u32, bool) {
        match self.fractal_params.fractal_type {
            crate::FractalType::Mandelbrot => {
                let mut zx = 0.0;
                let mut zy = 0.0;
                let mut iteration = 0;

                while zx * zx + zy * zy < self.fractal_params.bailout && iteration < self.fractal_params.iterations {
                    let xtemp = zx * zx - zy * zy + cx;
                    zy = 2.0 * zx * zy + cy;
                    zx = xtemp;
                    iteration += 1;
                }

                (iteration, iteration < self.fractal_params.iterations)
            }
            crate::FractalType::Julia => {
                let mut zx = cx;
                let mut zy = cy;
                let mut iteration = 0;

                while zx * zx + zy * zy < self.fractal_params.bailout && iteration < self.fractal_params.iterations {
                    let xtemp = zx * zx - zy * zy + self.fractal_params.julia_c.0;
                    zy = 2.0 * zx * zy + self.fractal_params.julia_c.1;
                    zx = xtemp;
                    iteration += 1;
                }

                (iteration, iteration < self.fractal_params.iterations)
            }
            _ => (0, false), // Placeholder for other fractal types
        }
    }

    /// Generate color based on iteration count
    fn fractal_color(&self, iterations: u32) -> (u8, u8, u8) {
        if iterations == self.fractal_params.iterations {
            return (0, 0, 0);
        }

        let t = iterations as f32 / self.fractal_params.iterations as f32;
        let r = ((0.5 + 0.5 * (t * 6.283185).sin()) * 255.0) as u8;
        let g = ((0.5 + 0.5 * ((t * 6.283185) + 2.094395).sin()) * 255.0) as u8;
        let b = ((0.5 + 0.5 * ((t * 6.283185) + 4.188790).sin()) * 255.0) as u8;

        (r, g, b)
    }

    /// Get current time
    #[wasm_bindgen]
    pub fn get_time(&self) -> f32 {
        self.time
    }

    /// Reset time
    #[wasm_bindgen]
    pub fn reset_time(&mut self) {
        self.time = 0.0;
    }
}

/// Initialize the web module
#[wasm_bindgen(start)]
pub fn main() {
    console::log_1(&"Fractal Shader Engine Web Module Loaded".into());
}

/// Export functions for JavaScript
#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);
}

/// Web-specific utilities
pub mod utils {
    use wasm_bindgen::prelude::*;

    /// Get current timestamp
    #[wasm_bindgen]
    pub fn get_timestamp() -> f64 {
        js_sys::Date::now()
    }

    /// Request animation frame callback
    #[wasm_bindgen]
    pub fn request_animation_frame(callback: &js_sys::Function) -> i32 {
        web_sys::window()
            .unwrap()
            .request_animation_frame(callback)
            .unwrap()
    }

    /// Cancel animation frame
    #[wasm_bindgen]
    pub fn cancel_animation_frame(id: i32) {
        web_sys::window()
            .unwrap()
            .cancel_animation_frame(id);
    }
}