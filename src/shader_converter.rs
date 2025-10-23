use std::fs;
use std::path::Path;
use walkdir::WalkDir;

/// Batch converter for ISF shaders to WGSL
pub struct BatchShaderConverter;

impl BatchShaderConverter {
    /// Convert all ISF shaders in a directory to WGSL
    pub fn convert_directory_to_wgsl(input_dir: &str, output_dir: &str) -> Result<Vec<String>, Box<dyn std::error::Error>> {
        let mut converted_files = Vec::new();

        // Create output directory
        fs::create_dir_all(output_dir)?;

        for entry in WalkDir::new(input_dir).into_iter().filter_map(|e| e.ok()) {
            if entry.path().extension().and_then(|s| s.to_str()) == Some("fs") {
                if let Some(shader_name) = entry.path().file_stem().and_then(|s| s.to_str()) {
                    let content = fs::read_to_string(entry.path())?;

                    // Convert ISF to WGSL
                    let wgsl_content = Self::convert_isf_to_wgsl(&content)?;

                    // Save WGSL file
                    let output_path = Path::new(output_dir).join(format!("{}.wgsl", shader_name));
                    fs::write(&output_path, wgsl_content)?;

                    converted_files.push(shader_name.to_string());
                }
            }
        }

        Ok(converted_files)
    }

    /// Convert ISF shader content to WGSL
    pub fn convert_isf_to_wgsl(isf_content: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut wgsl_source = String::new();

        // Add WGSL header with uniforms
        wgsl_source.push_str("@group(0) @binding(0) var<uniform> time: f32;\n");
        wgsl_source.push_str("@group(0) @binding(1) var<uniform> resolution: vec2<f32>;\n");
        wgsl_source.push_str("@group(0) @binding(2) var<uniform> mouse: vec2<f32>;\n");
        wgsl_source.push_str("@group(0) @binding(3) var input_texture: texture_2d<f32>;\n");
        wgsl_source.push_str("@group(0) @binding(4) var texture_sampler: sampler;\n\n");

        wgsl_source.push_str("@fragment\n");
        wgsl_source.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");
        wgsl_source.push_str("    let uv = coord.xy / resolution;\n");

        // Extract the main function body from ISF shader
        let body_start = isf_content.find("void main() {").unwrap_or(0) + 12;
        let body_end = isf_content.rfind("}").unwrap_or(isf_content.len());
        let body = &isf_content[body_start..body_end];

        // Convert GLSL syntax to WGSL
        let converted_body = body
            .replace("vec2", "vec2<f32>")
            .replace("vec3", "vec3<f32>")
            .replace("vec4", "vec4<f32>")
            .replace("float", "f32")
            .replace("int", "i32")
            .replace("bool", "bool")
            .replace("mat2", "mat2x2<f32>")
            .replace("mat3", "mat3x3<f32>")
            .replace("mat4", "mat4x4<f32>")
            .replace("gl_FragCoord.xy", "coord.xy")
            .replace("gl_FragColor", "return")
            .replace("RENDERSIZE.xy", "resolution")
            .replace("RENDERSIZE.y", "resolution.y")
            .replace("TIME", "time")
            .replace("IMG_PIXEL(inputTex, ", "textureSample(input_texture, texture_sampler, ")
            .replace("mod(", "f32(")
            .replace("pmod(", "f32(")
            // Handle common GLSL functions
            .replace("sin(", "sin(")
            .replace("cos(", "cos(")
            .replace("tan(", "tan(")
            .replace("abs(", "abs(")
            .replace("length(", "length(")
            .replace("normalize(", "normalize(")
            .replace("dot(", "dot(")
            .replace("cross(", "cross(")
            .replace("mix(", "mix(")
            .replace("clamp(", "clamp(")
            .replace("smoothstep(", "smoothstep(")
            .replace("step(", "step(")
            .replace("fract(", "fract(")
            .replace("floor(", "floor(")
            .replace("ceil(", "ceil(")
            .replace("pow(", "pow(")
            .replace("exp(", "exp(")
            .replace("log(", "log(")
            .replace("sqrt(", "sqrt(")
            .replace("min(", "min(")
            .replace("max(", "max(");

        wgsl_source.push_str(&converted_body);
        wgsl_source.push_str("}\n");

        Ok(wgsl_source)
    }

    /// Convert ISF shader to Rust/WGPU shader module
    pub fn convert_isf_to_rust_wgpu(isf_content: &str, shader_name: &str) -> Result<String, Box<dyn std::error::Error>> {
        let wgsl_content = Self::convert_isf_to_wgsl(isf_content)?;

        let mut rust_code = String::new();

        rust_code.push_str(&format!("// Auto-generated from ISF shader: {}\n", shader_name));
        rust_code.push_str("use wgpu::*;\n\n");

        rust_code.push_str(&format!("pub const {}_SHADER: &str = r#\"\n", shader_name.to_uppercase()));
        rust_code.push_str(&wgsl_content);
        rust_code.push_str("\"#;\n\n");

        rust_code.push_str(&format!("pub struct {}Shader {{\n", shader_name));
        rust_code.push_str("    pub pipeline: RenderPipeline,\n");
        rust_code.push_str("    pub bind_group: BindGroup,\n");
        rust_code.push_str("}\n\n");

        rust_code.push_str(&format!("impl {}Shader {{\n", shader_name));
        rust_code.push_str("    pub fn new(device: &Device, config: &SurfaceConfiguration) -> Self {\n");
        rust_code.push_str("        let shader = device.create_shader_module(ShaderModuleDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader\"),\n");
        rust_code.push_str(&format!("            source: ShaderSource::Wgsl(Cow::Borrowed({}_SHADER)),\n", shader_name.to_uppercase()));
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        // Create bind group layout\n");
        rust_code.push_str("        let bind_group_layout = device.create_bind_group_layout(&BindGroupLayoutDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader Bind Group Layout\"),\n");
        rust_code.push_str("            entries: &[\n");
        rust_code.push_str("                BindGroupLayoutEntry {\n");
        rust_code.push_str("                    binding: 0,\n");
        rust_code.push_str("                    visibility: ShaderStages::FRAGMENT,\n");
        rust_code.push_str("                    ty: BindingType::Buffer {\n");
        rust_code.push_str("                        ty: BufferBindingType::Uniform,\n");
        rust_code.push_str("                        has_dynamic_offset: false,\n");
        rust_code.push_str("                        min_binding_size: None,\n");
        rust_code.push_str("                    },\n");
        rust_code.push_str("                },\n");
        rust_code.push_str("                // Add more bindings as needed\n");
        rust_code.push_str("            ],\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        // Create pipeline layout\n");
        rust_code.push_str("        let pipeline_layout = device.create_pipeline_layout(&PipelineLayoutDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader Pipeline Layout\"),\n");
        rust_code.push_str("            bind_group_layouts: &[&bind_group_layout],\n");
        rust_code.push_str("            push_constant_ranges: &[],\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        // Create render pipeline\n");
        rust_code.push_str("        let pipeline = device.create_render_pipeline(&RenderPipelineDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader Pipeline\"),\n");
        rust_code.push_str("            layout: Some(&pipeline_layout),\n");
        rust_code.push_str("            vertex: VertexState {\n");
        rust_code.push_str("                module: &shader,\n");
        rust_code.push_str("                entry_point: \"vs_main\",\n");
        rust_code.push_str("                buffers: &[],\n");
        rust_code.push_str("            },\n");
        rust_code.push_str("            fragment: Some(FragmentState {\n");
        rust_code.push_str("                module: &shader,\n");
        rust_code.push_str("                entry_point: \"main\",\n");
        rust_code.push_str("                targets: &[Some(ColorTargetState {\n");
        rust_code.push_str("                    format: config.format,\n");
        rust_code.push_str("                    blend: Some(BlendState::REPLACE),\n");
        rust_code.push_str("                    write_mask: ColorWrites::ALL,\n");
        rust_code.push_str("                })],\n");
        rust_code.push_str("            }),\n");
        rust_code.push_str("            primitive: PrimitiveState {\n");
        rust_code.push_str("                topology: PrimitiveTopology::TriangleList,\n");
        rust_code.push_str("                strip_index_format: None,\n");
        rust_code.push_str("                front_face: FrontFace::Ccw,\n");
        rust_code.push_str("                cull_mode: Some(Face::Back),\n");
        rust_code.push_str("                polygon_mode: PolygonMode::Fill,\n");
        rust_code.push_str("                unclipped_depth: false,\n");
        rust_code.push_str("                conservative: false,\n");
        rust_code.push_str("            },\n");
        rust_code.push_str("            depth_stencil: None,\n");
        rust_code.push_str("            multisample: MultisampleState {\n");
        rust_code.push_str("                count: 1,\n");
        rust_code.push_str("                mask: !0,\n");
        rust_code.push_str("                alpha_to_coverage_enabled: false,\n");
        rust_code.push_str("            },\n");
        rust_code.push_str("            multiview: None,\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        // Create uniform buffer\n");
        rust_code.push_str("        let uniform_buffer = device.create_buffer(&BufferDescriptor {\n");
        rust_code.push_str("            label: Some(\"Uniform Buffer\"),\n");
        rust_code.push_str("            size: 64, // Adjust size as needed\n");
        rust_code.push_str("            usage: BufferUsages::UNIFORM | BufferUsages::COPY_DST,\n");
        rust_code.push_str("            mapped_at_creation: false,\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        // Create bind group\n");
        rust_code.push_str("        let bind_group = device.create_bind_group(&BindGroupDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader Bind Group\"),\n");
        rust_code.push_str("            layout: &bind_group_layout,\n");
        rust_code.push_str("            entries: &[BindGroupEntry {\n");
        rust_code.push_str("                binding: 0,\n");
        rust_code.push_str("                resource: uniform_buffer.as_entire_binding(),\n");
        rust_code.push_str("            }],\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        Self {\n");
        rust_code.push_str("            pipeline,\n");
        rust_code.push_str("            bind_group,\n");
        rust_code.push_str("        }\n");
        rust_code.push_str("    }\n\n");

        rust_code.push_str("    pub fn render(&self, encoder: &mut CommandEncoder, view: &TextureView) {\n");
        rust_code.push_str("        let mut render_pass = encoder.begin_render_pass(&RenderPassDescriptor {\n");
        rust_code.push_str("            label: Some(\"Shader Render Pass\"),\n");
        rust_code.push_str("            color_attachments: &[Some(RenderPassColorAttachment {\n");
        rust_code.push_str("                view,\n");
        rust_code.push_str("                resolve_target: None,\n");
        rust_code.push_str("                ops: Operations {\n");
        rust_code.push_str("                    load: LoadOp::Clear(Color::BLACK),\n");
        rust_code.push_str("                    store: StoreOp::Store,\n");
        rust_code.push_str("                },\n");
        rust_code.push_str("            })],\n");
        rust_code.push_str("            depth_stencil_attachment: None,\n");
        rust_code.push_str("            timestamp_writes: None,\n");
        rust_code.push_str("            occlusion_query_set: None,\n");
        rust_code.push_str("        });\n\n");

        rust_code.push_str("        render_pass.set_pipeline(&self.pipeline);\n");
        rust_code.push_str("        render_pass.set_bind_group(0, &self.bind_group, &[]);\n");
        rust_code.push_str("        render_pass.draw(0..3, 0..1);\n");
        rust_code.push_str("    }\n");
        rust_code.push_str("}\n");

        Ok(rust_code)
    }
}