//! Web deployment and WASM support for the fractal shader engine

use wasm_bindgen::prelude::*;
use web_sys::{console, window, HtmlCanvasElement, WebGlRenderingContext, WebGl2RenderingContext};
use crate::{RustFractalShaderEngine, ShaderConverter};
use std::collections::HashMap;

/// Web-specific fractal shader engine
#[wasm_bindgen]
pub struct WebFractalShaderEngine {
    engine: RustFractalShaderEngine,
    canvas: HtmlCanvasElement,
    gl_context: WebGlRenderingContext,
    shaders: HashMap<String, WebGlProgram>,
    current_time: f32,
}

#[wasm_bindgen]
impl WebFractalShaderEngine {
    /// Create a new web fractal shader engine
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<WebFractalShaderEngine, JsValue> {
        // Initialize console logging
        console_error_panic_hook::set_once();
        console::log_1(&"Initializing Web Fractal Shader Engine...".into());

        // Get canvas element
        let window = window().unwrap();
        let document = window.document().unwrap();
        let canvas = document
            .get_element_by_id(canvas_id)
            .unwrap()
            .dyn_into::<HtmlCanvasElement>()?;

        // Get WebGL context
        let gl = canvas
            .get_context("webgl2")?
            .unwrap()
            .dyn_into::<WebGl2RenderingContext>()?;

        // Create engine
        let engine = RustFractalShaderEngine::new();

        Ok(WebFractalShaderEngine {
            engine,
            canvas,
            gl_context: gl,
            shaders: HashMap::new(),
            current_time: 0.0,
        })
    }

    /// Load an ISF shader for web use
    #[wasm_bindgen]
    pub fn load_isf_shader(&mut self, name: &str, isf_source: &str) -> Result<(), JsValue> {
        console::log_1(&format!("Loading ISF shader: {}", name).into());

        // Convert ISF to GLSL for WebGL
        let glsl_source = ShaderConverter::isf_to_wgsl(isf_source)
            .map_err(|e| JsValue::from_str(&format!("Shader conversion failed: {}", e)))?;

        // Create WebGL shader program
        let program = self.create_shader_program(&glsl_source)?;

        self.shaders.insert(name.to_string(), program);
        self.engine.add_shader_module(name, isf_source)
            .map_err(|e| JsValue::from_str(&format!("Failed to add shader module: {}", e)))?;

        Ok(())
    }

    /// Render frame
    #[wasm_bindgen]
    pub fn render(&mut self, delta_time: f32) -> Result<(), JsValue> {
        self.current_time += delta_time;

        // Clear canvas
        self.gl_context.clear_color(0.0, 0.0, 0.0, 1.0);
        self.gl_context.clear(WebGlRenderingContext::COLOR_BUFFER_BIT);

        // Set viewport
        let width = self.canvas.width() as i32;
        let height = self.canvas.height() as i32;
        self.gl_context.viewport(0, 0, width, height);

        // Render active shaders
        for (name, program) in &self.shaders {
            self.render_shader(name, program)?;
        }

        Ok(())
    }

    /// Set shader parameter
    #[wasm_bindgen]
    pub fn set_parameter(&mut self, shader_name: &str, param_name: &str, value: f32) {
        self.engine.set_fractal_parameter(param_name, value);
    }

    /// Get available shaders
    #[wasm_bindgen]
    pub fn get_shader_names(&self) -> Vec<JsValue> {
        self.shaders.keys()
            .map(|name| JsValue::from_str(name))
            .collect()
    }
}

impl WebFractalShaderEngine {
    /// Create WebGL shader program from GLSL source
    fn create_shader_program(&self, glsl_source: &str) -> Result<WebGlProgram, JsValue> {
        let gl = &self.gl_context;

        // Create vertex shader
        let vertex_shader = gl.create_shader(WebGlRenderingContext::VERTEX_SHADER).unwrap();
        let vertex_source = r#"
            attribute vec2 a_position;
            varying vec2 v_uv;

            void main() {
                v_uv = a_position * 0.5 + 0.5;
                gl_Position = vec4(a_position, 0.0, 1.0);
            }
        "#;

        gl.shader_source(&vertex_shader, vertex_source);
        gl.compile_shader(&vertex_shader);

        if !gl.get_shader_parameter(&vertex_shader, WebGlRenderingContext::COMPILE_STATUS).as_bool().unwrap() {
            let error = gl.get_shader_info_log(&vertex_shader).unwrap();
            return Err(JsValue::from_str(&format!("Vertex shader compilation failed: {}", error)));
        }

        // Create fragment shader
        let fragment_shader = gl.create_shader(WebGlRenderingContext::FRAGMENT_SHADER).unwrap();

        // Convert WGSL-style GLSL to WebGL GLSL
        let webgl_source = self.convert_to_webgl_glsl(glsl_source);

        gl.shader_source(&fragment_shader, &webgl_source);
        gl.compile_shader(&fragment_shader);

        if !gl.get_shader_parameter(&fragment_shader, WebGlRenderingContext::COMPILE_STATUS).as_bool().unwrap() {
            let error = gl.get_shader_info_log(&fragment_shader).unwrap();
            return Err(JsValue::from_str(&format!("Fragment shader compilation failed: {}", error)));
        }

        // Create program
        let program = gl.create_program().unwrap();
        gl.attach_shader(&program, &vertex_shader);
        gl.attach_shader(&program, &fragment_shader);
        gl.link_program(&program);

        if !gl.get_program_parameter(&program, WebGlRenderingContext::LINK_STATUS).as_bool().unwrap() {
            let error = gl.get_program_info_log(&program).unwrap();
            return Err(JsValue::from_str(&format!("Program linking failed: {}", error)));
        }

        Ok(program)
    }

    /// Convert WGSL-style GLSL to WebGL-compatible GLSL
    fn convert_to_webgl_glsl(&self, wgsl_glsl: &str) -> String {
        let mut webgl_source = String::from("#version 300 es\nprecision highp float;\n");

        // Convert vec2<f32> to vec2, etc.
        let converted = wgsl_glsl
            .replace("vec2<f32>", "vec2")
            .replace("vec3<f32>", "vec3")
            .replace("vec4<f32>", "vec4")
            .replace("f32", "float")
            .replace("i32", "int")
            .replace("mat2x2<f32>", "mat2")
            .replace("mat3x3<f32>", "mat3")
            .replace("mat4x4<f32>", "mat4")
            .replace("@builtin(position)", "")
            .replace("@location(0)", "")
            .replace("fn main(", "void main(")
            .replace("-> vec4<f32> {", ") {")
            .replace("return", "gl_FragColor =")
            .replace("coord.xy", "gl_FragCoord.xy")
            .replace("resolution", "u_resolution")
            .replace("time", "u_time");

        webgl_source.push_str("uniform vec2 u_resolution;\n");
        webgl_source.push_str("uniform float u_time;\n");
        webgl_source.push_str("varying vec2 v_uv;\n\n");
        webgl_source.push_str(&converted);

        webgl_source
    }

    /// Render a specific shader
    fn render_shader(&self, name: &str, program: &WebGlProgram) -> Result<(), JsValue> {
        let gl = &self.gl_context;

        gl.use_program(Some(program));

        // Set uniforms
        let time_loc = gl.get_uniform_location(program, "u_time");
        gl.uniform1f(time_loc.as_ref(), self.current_time);

        let resolution_loc = gl.get_uniform_location(program, "u_resolution");
        gl.uniform2f(resolution_loc.as_ref(),
                    self.canvas.width() as f32,
                    self.canvas.height() as f32);

        // Create quad vertices
        let vertices: [f32; 8] = [
            -1.0, -1.0,
             1.0, -1.0,
            -1.0,  1.0,
             1.0,  1.0,
        ];

        let buffer = gl.create_buffer().unwrap();
        gl.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&buffer));
        gl.buffer_data_with_array_buffer_view(
            WebGlRenderingContext::ARRAY_BUFFER,
            &js_sys::Float32Array::from(&vertices[..]),
            WebGlRenderingContext::STATIC_DRAW,
        );

        let position_loc = gl.get_attrib_location(program, "a_position") as u32;
        gl.enable_vertex_attrib_array(position_loc);
        gl.vertex_attrib_pointer_with_i32(position_loc, 2, WebGlRenderingContext::FLOAT, false, 0, 0);

        gl.draw_arrays(WebGlRenderingContext::TRIANGLE_STRIP, 0, 4);

        Ok(())
    }
}

/// Initialize web exports
#[wasm_bindgen(start)]
pub fn main() {
    console::log_1(&"Web Fractal Shader Engine loaded!".into());
}