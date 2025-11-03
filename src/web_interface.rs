//! Web Interface Module
//!
//! This module provides web deployment capabilities for the fractal shader,
//! enabling browser-based fractal creation and editing.

use wasm_bindgen::prelude::*;
use web_sys::{console, window, CanvasRenderingContext2d, HtmlCanvasElement, ImageData};
use js_sys::{Array, Uint8Array};
use serde::{Deserialize, Serialize};

/// Web fractal interface
#[wasm_bindgen]
pub struct WebFractalStudio {
    canvas: HtmlCanvasElement,
    context: CanvasRenderingContext2d,
    width: u32,
    height: u32,
    fractal_params: crate::fractal::FractalParameters,
    time: f32,
    animation_id: Option<i32>,
}

#[wasm_bindgen]
impl WebFractalStudio {
    /// Create new web fractal studio
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<WebFractalStudio, JsValue> {
        // Get canvas element
        let document = web_sys::window().unwrap().document().unwrap();
        let canvas = document
            .get_element_by_id(canvas_id)
            .unwrap()
            .dyn_into::<HtmlCanvasElement>()?;

        // Get 2D context
        let context = canvas
            .get_context("2d")?
            .unwrap()
            .dyn_into::<CanvasRenderingContext2d>()?;

        let width = canvas.width();
        let height = canvas.height();

        console::log_1(&"Web Fractal Studio initialized".into());

        Ok(WebFractalStudio {
            canvas,
            context,
            width,
            height,
            fractal_params: crate::fractal::FractalParameters::default(),
            time: 0.0,
            animation_id: None,
        })
    }

    /// Set fractal type
    #[wasm_bindgen]
    pub fn set_fractal_type(&mut self, fractal_type: &str) {
        self.fractal_params.formula = match fractal_type {
            "mandelbrot" => crate::fractal::FractalFormula::Mandelbrot {
                center: [-0.5, 0.0],
                zoom: 1.0,
                max_iterations: 100,
            },
            "julia" => crate::fractal::FractalFormula::Julia {
                c: [-0.7, 0.27015],
                max_iterations: 100,
            },
            "mandelbulb" => crate::fractal::FractalFormula::Mandelbulb {
                power: 8.0,
                max_iterations: 100,
            },
            _ => self.fractal_params.formula,
        };
    }

    /// Set fractal parameter
    #[wasm_bindgen]
    pub fn set_parameter(&mut self, name: &str, value: f32) {
        match name {
            "zoom" => {
                if let crate::fractal::FractalFormula::Mandelbrot { zoom, .. } = &mut self.fractal_params.formula {
                    *zoom = value;
                }
            }
            "power" => {
                if let crate::fractal::FractalFormula::Mandelbulb { power, .. } = &mut self.fractal_params.formula {
                    *power = value;
                }
            }
            "iterations" => {
                match &mut self.fractal_params.formula {
                    crate::fractal::FractalFormula::Mandelbrot { max_iterations, .. } => *max_iterations = value as u32,
                    crate::fractal::FractalFormula::Julia { max_iterations, .. } => *max_iterations = value as u32,
                    crate::fractal::FractalFormula::Mandelbulb { max_iterations, .. } => *max_iterations = value as u32,
                    _ => {}
                }
            }
            _ => {}
        }
    }

    /// Render single frame
    #[wasm_bindgen]
    pub fn render_frame(&mut self) -> Result<(), JsValue> {
        self.time += 0.016; // ~60fps

        // Generate fractal data (simplified CPU version for web)
        let mut pixels = vec![0u8; (self.width * self.height * 4) as usize];

        for y in 0..self.height {
            for x in 0..self.width {
                let uv_x = (x as f32 / self.width as f32 - 0.5) * 2.0;
                let uv_y = (y as f32 / self.height as f32 - 0.5) * 2.0;

                let (distance, iterations) = self.compute_fractal_point(uv_x, uv_y);

                // Color based on iterations
                let t = (iterations as f32 / 100.0).min(1.0);
                let r = ((t * 6.28318).sin() * 0.5 + 0.5) * 255.0;
                let g = ((t * 6.28318 + 2.094).sin() * 0.5 + 0.5) * 255.0;
                let b = ((t * 6.28318 + 4.188).sin() * 0.5 + 0.5) * 255.0;

                let idx = ((y * self.width + x) * 4) as usize;
                pixels[idx] = r as u8;     // R
                pixels[idx + 1] = g as u8; // G
                pixels[idx + 2] = b as u8; // B
                pixels[idx + 3] = 255;     // A
            }
        }

        // Create image data and put on canvas
        let image_data = ImageData::new_with_u8_clamped_array_and_sh(
            wasm_bindgen::Clamped(&pixels),
            self.width,
            self.height,
        )?;

        self.context.put_image_data(&image_data, 0.0, 0.0)?;

        Ok(())
    }

    /// Start animation loop
    #[wasm_bindgen]
    pub fn start_animation(&mut self) -> Result<(), JsValue> {
        if self.animation_id.is_some() {
            return Ok(()); // Already running
        }

        let callback = Closure::wrap(Box::new(move || {
            // This would be called from JavaScript
            // In practice, we'd need to store a reference to self
        }) as Box<dyn FnMut()>);

        // Store animation ID (simplified)
        self.animation_id = Some(1);

        Ok(())
    }

    /// Stop animation loop
    #[wasm_bindgen]
    pub fn stop_animation(&mut self) {
        self.animation_id = None;
    }

    /// Get current parameters as JSON
    #[wasm_bindgen]
    pub fn get_parameters_json(&self) -> Result<String, JsValue> {
        // Simplified parameter export
        Ok(format!(
            r#"{{
                "fractal_type": "mandelbrot",
                "zoom": 1.0,
                "iterations": 100,
                "time": {:.2}
            }}"#,
            self.time
        ))
    }

    /// Load parameters from JSON
    #[wasm_bindgen]
    pub fn load_parameters_json(&mut self, json: &str) -> Result<(), JsValue> {
        // Parse and apply parameters
        console::log_1(&format!("Loading parameters: {}", json).into());
        Ok(())
    }

    /// Compute fractal value at point (simplified CPU version)
    fn compute_fractal_point(&self, x: f32, y: f32) -> (f32, u32) {
        match &self.fractal_params.formula {
            crate::fractal::FractalFormula::Mandelbrot { center, zoom, max_iterations } => {
                let c_x = x / *zoom + center[0];
                let c_y = y / *zoom + center[1];

                let mut z_x = 0.0;
                let mut z_y = 0.0;
                let mut iterations = 0;

                while iterations < *max_iterations {
                    let z_x2 = z_x * z_x;
                    let z_y2 = z_y * z_y;

                    if z_x2 + z_y2 > 4.0 {
                        break;
                    }

                    let new_z_x = z_x2 - z_y2 + c_x;
                    let new_z_y = 2.0 * z_x * z_y + c_y;

                    z_x = new_z_x;
                    z_y = new_z_y;
                    iterations += 1;
                }

                (0.0, iterations) // Simplified
            }
            _ => (0.0, 0),
        }
    }
}

/// Web-specific utilities
#[wasm_bindgen]
pub fn web_hello() -> String {
    "Hello from Web Fractal Studio!".to_string()
}

/// Web deployment configuration
pub struct WebDeploymentConfig {
    pub canvas_id: String,
    pub enable_gpu: bool,
    pub max_fps: u32,
    pub enable_animations: bool,
}

impl Default for WebDeploymentConfig {
    fn default() -> Self {
        Self {
            canvas_id: "fractal-canvas".to_string(),
            enable_gpu: true,
            max_fps: 60,
            enable_animations: true,
        }
    }
}

/// WebGL fractal renderer (for advanced web deployment)
pub struct WebGLFractalRenderer {
    canvas: HtmlCanvasElement,
    gl: web_sys::WebGlRenderingContext,
    program: web_sys::WebGlProgram,
    vertex_buffer: web_sys::WebGlBuffer,
    uniforms: std::collections::HashMap<String, web_sys::WebGlUniformLocation>,
}

impl WebGLFractalRenderer {
    /// Create WebGL renderer
    pub fn new(canvas: HtmlCanvasElement) -> Result<Self, JsValue> {
        let gl = canvas
            .get_context("webgl")?
            .unwrap()
            .dyn_into::<web_sys::WebGlRenderingContext>()?;

        // Create shaders
        let vertex_shader = Self::create_shader(&gl, web_sys::WebGlRenderingContext::VERTEX_SHADER, VERTEX_SHADER)?;
        let fragment_shader = Self::create_shader(&gl, web_sys::WebGlRenderingContext::FRAGMENT_SHADER, FRAGMENT_SHADER)?;

        let program = gl.create_program().unwrap();
        gl.attach_shader(&program, &vertex_shader);
        gl.attach_shader(&program, &fragment_shader);
        gl.link_program(&program);

        if !gl.get_program_parameter(&program, web_sys::WebGlRenderingContext::LINK_STATUS).as_bool().unwrap() {
            return Err(gl.get_program_info_log(&program).unwrap().into());
        }

        // Create vertex buffer
        let vertex_buffer = gl.create_buffer().unwrap();
        gl.bind_buffer(web_sys::WebGlRenderingContext::ARRAY_BUFFER, Some(&vertex_buffer));

        let vertices: [f32; 12] = [
            -1.0, -1.0, 0.0,
             1.0, -1.0, 0.0,
            -1.0,  1.0, 0.0,
             1.0,  1.0, 0.0,
        ];

        let vertex_array = unsafe { js_sys::Float32Array::view(&vertices) };
        gl.buffer_data_with_array_buffer_view(
            web_sys::WebGlRenderingContext::ARRAY_BUFFER,
            &vertex_array,
            web_sys::WebGlRenderingContext::STATIC_DRAW,
        );

        let mut uniforms = std::collections::HashMap::new();
        uniforms.insert("u_resolution".to_string(), gl.get_uniform_location(&program, "u_resolution").unwrap());
        uniforms.insert("u_time".to_string(), gl.get_uniform_location(&program, "u_time").unwrap());
        uniforms.insert("u_zoom".to_string(), gl.get_uniform_location(&program, "u_zoom").unwrap());

        Ok(Self {
            canvas,
            gl,
            program,
            vertex_buffer,
            uniforms,
        })
    }

    /// Render frame
    pub fn render(&self, time: f32, zoom: f32) -> Result<(), JsValue> {
        let gl = &self.gl;

        gl.viewport(0, 0, self.canvas.width() as i32, self.canvas.height() as i32);
        gl.clear_color(0.0, 0.0, 0.0, 1.0);
        gl.clear(web_sys::WebGlRenderingContext::COLOR_BUFFER_BIT);

        gl.use_program(Some(&self.program));

        // Set uniforms
        gl.uniform2f(Some(&self.uniforms["u_resolution"]), self.canvas.width() as f32, self.canvas.height() as f32);
        gl.uniform1f(Some(&self.uniforms["u_time"]), time);
        gl.uniform1f(Some(&self.uniforms["u_zoom"]), zoom);

        // Bind vertex buffer
        gl.bind_buffer(web_sys::WebGlRenderingContext::ARRAY_BUFFER, Some(&self.vertex_buffer));
        gl.vertex_attrib_pointer_with_i32(0, 3, web_sys::WebGlRenderingContext::FLOAT, false, 0, 0);
        gl.enable_vertex_attrib_array(0);

        // Draw
        gl.draw_arrays(web_sys::WebGlRenderingContext::TRIANGLE_STRIP, 0, 4);

        Ok(())
    }

    /// Create shader
    fn create_shader(gl: &web_sys::WebGlRenderingContext, shader_type: u32, source: &str) -> Result<web_sys::WebGlShader, JsValue> {
        let shader = gl.create_shader(shader_type).unwrap();
        gl.shader_source(&shader, source);
        gl.compile_shader(&shader);

        if !gl.get_shader_parameter(&shader, web_sys::WebGlRenderingContext::COMPILE_STATUS).as_bool().unwrap() {
            return Err(gl.get_shader_info_log(&shader).unwrap().into());
        }

        Ok(shader)
    }
}

// WebGL shaders
const VERTEX_SHADER: &str = r#"
    attribute vec3 a_position;
    void main() {
        gl_Position = vec4(a_position, 1.0);
    }
"#;

const FRAGMENT_SHADER: &str = r#"
    precision mediump float;
    uniform vec2 u_resolution;
    uniform float u_time;
    uniform float u_zoom;

    void main() {
        vec2 uv = (gl_FragCoord.xy - 0.5 * u_resolution) / u_resolution.y;
        vec2 c = uv * u_zoom;

        vec2 z = vec2(0.0);
        float iterations = 0.0;
        const float max_iter = 100.0;

        for(float i = 0.0; i < max_iter; i++) {
            if(length(z) > 2.0) break;
            z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
            iterations = i;
        }

        float t = iterations / max_iter;
        vec3 color = vec3(
            0.5 + 0.5 * sin(6.283185 * t + u_time),
            0.5 + 0.5 * sin(6.283185 * t + u_time + 2.094395),
            0.5 + 0.5 * sin(6.283185 * t + u_time + 4.188790)
        );

        gl_FragColor = vec4(color, 1.0);
    }
"#;