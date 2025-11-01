odular-fractal-shader/src/web/mod.rs</path>
<content lines="1-150">
use wasm_bindgen::prelude::*;
use web_sys::{console, window, HtmlCanvasElement, WebGlRenderingContext};
use js_sys::Uint8Array;

/// Web deployment and WASM/WebGPU integration
#[wasm_bindgen]
pub struct WebFractalStudio {
    canvas: HtmlCanvasElement,
    gl_context: WebGlRenderingContext,
    fractal_engine: crate::fractal::engine::FractalEngine,
    animation_controller: crate::animation::AnimationController,
    last_frame_time: f64,
}

#[wasm_bindgen]
impl WebFractalStudio {
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<WebFractalStudio, JsValue> {
        // Initialize console logging
        console_error_panic_hook::set_once();

        // Get canvas element
        let document = web_sys::window()
            .ok_or("No window")?
            .document()
            .ok_or("No document")?;

        let canvas = document
            .get_element_by_id(canvas_id)
            .ok_or("Canvas not found")?
            .dyn_into::<HtmlCanvasElement>()?;

        // Get WebGL context
        let gl_context = canvas
            .get_context("webgl")?
            .ok_or("WebGL not supported")?
            .dyn_into::<WebGlRenderingContext>()?;

        // Initialize fractal engine
        let fractal_engine = crate::fractal::engine::FractalEngine::new();

        // Initialize animation controller
        let animation_controller = crate::animation::AnimationController::new();

        log("WebFractalStudio initialized successfully");

        Ok(WebFractalStudio {
            canvas,
            gl_context,
            fractal_engine,
            animation_controller,
            last_frame_time: 0.0,
        })
    }

    #[wasm_bindgen]
    pub fn load_isf_shader(&mut self, shader_source: &str) -> Result<(), JsValue> {
        // Parse ISF shader and convert to WebGL
        let wgsl_code = crate::shader_converter::ShaderConverter::isf_to_wgsl(shader_source)
            .map_err(|e| JsValue::from_str(&format!("Shader conversion failed: {}", e)))?;

        // Compile shader for WebGL
        self.compile_webgl_shader(&wgsl_code)?;

        log("ISF shader loaded and compiled");
        Ok(())
    }

    #[wasm_bindgen]
    pub fn set_fractal_parameters(&mut self, params_json: &str) -> Result<(), JsValue> {
        let params: crate::fractal::types::FractalParameters = serde_json::from_str(params_json)
            .map_err(|e| JsValue::from_str(&format!("Invalid parameters: {}", e)))?;

        self.fractal_engine.set_parameters(params);
        log("Fractal parameters updated");
        Ok(())
    }

    #[wasm_bindgen]
    pub fn render_frame(&mut self, current_time: f64) -> Result<(), JsValue> {
        // Update animation
        let delta_time = current_time - self.last_frame_time;
        self.last_frame_time = current_time;

        self.animation_controller.update(delta_time as f32);

        // Get canvas dimensions
        let width = self.canvas.width() as f32;
        let height = self.canvas.height() as f32;

        // Set viewport
        self.gl_context.viewport(0, 0, width as i32, height as i32);

        // Clear canvas
        self.gl_context.clear_color(0.1, 0.1, 0.15, 1.0);
        self.gl_context.clear(WebGlRenderingContext::COLOR_BUFFER_BIT | WebGlRenderingContext::DEPTH_BUFFER_BIT);

        // Render fractal (simplified - would use actual fractal rendering)
        self.render_fractal_to_webgl(width, height)?;

        Ok(())
    }

    #[wasm_bindgen]
    pub fn export_image(&self) -> Result<Uint8Array, JsValue> {
        // Read pixels from WebGL context
        let width = self.canvas.width();
        let height = self.canvas.height();
        let mut pixels = vec![0u8; (width * height * 4) as usize];

        self.gl_context.read_pixels_with_opt_u8_array(
            0, 0, width as i32, height as i32,
            WebGlRenderingContext::RGBA,
            WebGlRenderingContext::UNSIGNED_BYTE,
            Some(&mut pixels),
        )?;

        Ok(Uint8Array::from(&pixels[..]))
    }

    #[wasm_bindgen]
    pub fn add_animation_keyframe(&mut self, time: f32, parameter_path: &str, value: f32) -> Result<(), JsValue> {
        // Parse parameter path and add keyframe
        self.animation_controller.add_keyframe(parameter_path, time, value)
            .map_err(|e| JsValue::from_str(&format!("Failed to add keyframe: {}", e)))?;

        log(&format!("Added keyframe at {}s for {}", time, parameter_path));
        Ok(())
    }

    #[wasm_bindgen]
    pub fn play_animation(&mut self) {
        self.animation_controller.play();
        log("Animation started");
    }

    #[wasm_bindgen]
    pub fn pause_animation(&mut self) {
        self.animation_controller.pause();
        log("Animation paused");
    }

    #[wasm_bindgen]
    pub fn stop_animation(&mut self) {
        self.animation_controller.stop();
        log("Animation stopped");
    }

    fn compile_webgl_shader(&self, wgsl_source: &str) -> Result<(), JsValue> {
        // Convert WGSL to GLSL (simplified)
        let glsl_source = self.wgsl_to_glsl(wgsl_source)?;

        // Create vertex shader
        let vertex_shader = self.create_shader(WebGlRenderingContext::VERTEX_SHADER, VERTEX_SHADER_SOURCE)?;

        // Create fragment shader
        let fragment_shader = self.create_shader(WebGlRenderingContext::FRAGMENT_SHADER, &glsl_source)?;

        // Create program
        let program = self.gl_context.create_program().ok_or("Failed to create program")?;
        self.gl_context.attach_shader(&program, &vertex_shader);
        self.gl_context.attach_shader(&program, &fragment_shader);
        self.gl_context.link_program(&program);

        if !self.gl_context.get_program_parameter(&program, WebGlRenderingContext::LINK_STATUS).as_bool().unwrap_or(false) {
            let error = self.gl_context.get_program_info_log(&program).unwrap_or_else(|| "Unknown error".to_string());
            return Err(JsValue::from_str(&format!("Shader linking failed: {}", error)));
        }

        self.gl_context.use_program(Some(&program));
        log("WebGL shader compiled and linked successfully");

        Ok(())
    }

    fn create_shader(&self, shader_type: u32, source: &str) -> Result<web_sys::WebGlShader, JsValue> {
        let shader = self.gl_context.create_shader(shader_type).ok_or("Failed to create shader")?;
        self.gl_context.shader_source(&shader, source);
        self.gl_context.compile_shader(&shader);

        if !self.gl_context.get_shader_parameter(&shader, WebGlRenderingContext::COMPILE_STATUS).as_bool().unwrap_or(false) {
            let error = self.gl_context.get_shader_info_log(&shader).unwrap_or_else(|| "Unknown error".to_string());
            return Err(JsValue::from_str(&format!("Shader compilation failed: {}", error)));
        }

        Ok(shader)
    }

    fn wgsl_to_glsl(&self, wgsl_source: &str) -> Result<String, JsValue> {
        // Simplified WGSL to GLSL conversion
        let mut glsl = wgsl_source
            .replace("@group(0) @binding(0)", "uniform")
            .replace("@group(0) @binding(1)", "uniform")
            .replace("var<uniform>", "")
            .replace("f32", "float")
            .replace("vec2<f32>", "vec2")
            .replace("vec3<f32>", "vec3")
            .replace("vec4<f32>", "vec4")
            .replace("@builtin(position)", "")
            .replace("@location(0)", "")
            .replace("fn main(", "void main(")
            .replace("->", "")
            .replace("vec4<f32>", "vec4");

        // Add GLSL version and precision
        glsl = format!("#version 300 es\nprecision highp float;\n{}", glsl);

        Ok(glsl)
    }

    fn render_fractal_to_webgl(&self, width: f32, height: f32) -> Result<(), JsValue> {
        // Create a simple quad for rendering
        let vertices: [f32; 12] = [
            -1.0, -1.0, 0.0,
             1.0, -1.0, 0.0,
            -1.0,  1.0, 0.0,
             1.0,  1.0, 0.0,
        ];

        let indices: [u16; 6] = [0, 1, 2, 1, 3, 2];

        // Create buffers
        let vertex_buffer = self.gl_context.create_buffer().ok_or("Failed to create vertex buffer")?;
        self.gl_context.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&vertex_buffer));
        unsafe {
            let vert_array = js_sys::Float32Array::view(&vertices);
            self.gl_context.buffer_data_with_array_buffer_view(
                WebGlRenderingContext::ARRAY_BUFFER,
                &vert_array,
                WebGlRenderingContext::STATIC_DRAW,
            );
        }

        let index_buffer = self.gl_context.create_buffer().ok_or("Failed to create index buffer")?;
        self.gl_context.bind_buffer(WebGlRenderingContext::ELEMENT_ARRAY_BUFFER, Some(&index_buffer));
        unsafe {
            let index_array = js_sys::Uint16Array::view(&indices);
            self.gl_context.buffer_data_with_array_buffer_view(
                WebGlRenderingContext::ELEMENT_ARRAY_BUFFER,
                &index_array,
                WebGlRenderingContext::STATIC_DRAW,
            );
        }

        // Set up vertex attributes
        let position_attrib = self.gl_context.get_attrib_location(self.gl_context.get_parameter(WebGlRenderingContext::CURRENT_PROGRAM).unwrap().as_ref(), "position") as u32;
        self.gl_context.vertex_attrib_pointer_with_i32(position_attrib, 3, WebGlRenderingContext::FLOAT, false, 0, 0);
        self.gl_context.enable_vertex_attrib_array(position_attrib);

        // Set uniforms
        let time_uniform = self.gl_context.get_uniform_location(self.gl_context.get_parameter(WebGlRenderingContext::CURRENT_PROGRAM).unwrap().as_ref(), "time");
        let resolution_uniform = self.gl_context.get_uniform_location(self.gl_context.get_parameter(WebGlRenderingContext::CURRENT_PROGRAM).unwrap().as_ref(), "resolution");

        if let Some(time_loc) = time_uniform {
            self.gl_context.uniform1f(Some(&time_loc), self.animation_controller.current_time());
        }

        if let Some(res_loc) = resolution_uniform {
            self.gl_context.uniform2f(Some(&res_loc), width, height);
        }

        // Draw
        self.gl_context.draw_elements_with_i32(
            WebGlRenderingContext::TRIANGLES,
            6,
            WebGlRenderingContext::UNSIGNED_SHORT,
            0,
        );

        Ok(())
    }
}

/// Web-specific utilities
#[wasm_bindgen]
pub fn initialize_web_interface() {
    log("Web interface initialized");
}

#[wasm_bindgen]
pub fn get_supported_features() -> String {
    serde_json::json!({
        "webgl": true,
        "webgpu": false, // TODO: Add WebGPU support
        "audio": true,
        "fullscreen": true,
        "export": true,
        "animation": true
    }).to_string()
}

/// WebGL shader sources
const VERTEX_SHADER_SOURCE: &str = r#"
attribute vec3 position;
void main() {
    gl_Position = vec4(position, 1.0);
}
"#;

/// Utility function for logging to browser console
fn log(message: &str) {
    console::log_1(&JsValue::from_str(message));
}

/// Web-specific asset loading
pub struct WebAssetLoader;

impl WebAssetLoader {
    pub fn load_texture_from_url(url: &str) -> Result<Vec<u8>, JsValue> {
        // TODO: Implement texture loading from URL
        log(&format!("Loading texture from: {}", url));
        Err(JsValue::from_str("Texture loading not implemented"))
    }

    pub fn load_shader_from_url(url: &str) -> Result<String, JsValue> {
        // TODO: Implement shader loading from URL
        log(&format!("Loading shader from: {}", url));
        Err(JsValue::from_str("Shader loading not implemented"))
    }
}

/// Web event handling
pub struct WebEventHandler;

impl WebEventHandler {
    pub fn setup_event_listeners() {
        // TODO: Set up keyboard, mouse, and touch event listeners
        log("Web event listeners setup");
    }

    pub fn handle_file_drop(callback: &Closure<dyn Fn(Vec<u8>)>) {
        // TODO: Implement file drop handling
        log("File drop handler registered");
    }
}

use wasm_bindgen::closure::Closure;