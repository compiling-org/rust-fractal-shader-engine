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

    /// Convert ISF shader content to WGSL (advanced fractal-aware)
    pub fn convert_isf_to_wgsl(isf_content: &str) -> Result<String, Box<dyn std::error::Error>> {
        fn wgsl_header() -> String {
            let mut h = String::new();
            h.push_str("// Auto-generated WGSL header\n");
            h.push_str("@group(0) @binding(0) var<uniform> time: f32;\n");
            h.push_str("@group(0) @binding(1) var<uniform> resolution: vec2<f32>;\n");
            h.push_str("@group(0) @binding(2) var<uniform> mouse: vec2<f32>;\n");
            h.push_str("@group(0) @binding(3) var input_texture: texture_2d<f32>;\n");
            h.push_str("@group(0) @binding(4) var texture_sampler: sampler;\n\n");
            h.push_str("fn modf(x: f32, y: f32) -> f32 { x - y * floor(x / y) }\n");
            h.push_str("fn mod2(x: vec2<f32>, y: vec2<f32>) -> vec2<f32> { x - y * floor(x / y) }\n");
            h.push_str("fn mod3(x: vec3<f32>, y: vec3<f32>) -> vec3<f32> { x - y * floor(x / y) }\n");
            h.push_str("fn pmod3(p: vec3<f32>, s: f32) -> vec3<f32> {\n");
            h.push_str("  let half = 0.5 * s;\n");
            h.push_str("  return (mod3(p + half, vec3<f32>(s, s, s)) - half);\n");
            h.push_str("}\n");
            h.push_str("fn saturate(x: f32) -> f32 { clamp(x, 0.0, 1.0) }\n");
            h.push_str("fn palette(t: f32, a: vec3<f32>, b: vec3<f32>, c: vec3<f32>, d: vec3<f32>) -> vec3<f32> {\n");
            h.push_str("  return a + b * cos(6.28318 * (c * t + d));\n");
            h.push_str("}\n\n");
            h
        }

        // Extract main body
        let body_start = isf_content.find("void main() {").unwrap_or_else(|| {
            isf_content.find("void mainImage(").unwrap_or(0)
        });
        let body_end = isf_content.rfind("}").unwrap_or(isf_content.len());
        let mut body = if body_start > 0 {
            let start = isf_content[body_start..].find("{").map(|i| body_start + i + 1).unwrap_or(body_start);
            isf_content[start..body_end].to_string()
        } else {
            isf_content.to_string()
        };

        body = body
            .replace("gl_FragCoord.xy", "coord.xy")
            .replace("gl_FragCoord", "coord.xy")
            .replace("RENDERSIZE.xy", "resolution")
            .replace("RENDERSIZE", "vec2<f32>(resolution.x, resolution.y)")
            .replace("TIME", "time")
            .replace("iTime", "time")
            .replace("iResolution", "resolution")
            .replace("IMG_PIXEL(inputTex, ", "textureSample(input_texture, texture_sampler, ")
            .replace("texture(inputTex, ", "textureSample(input_texture, texture_sampler, ");

        // Canonicalize ++ to explicit increment (best-effort)
        body = body.replace("i++", "i = i + 1");

        // Types and constructors
        let mut converted = body
            .replace("vec2", "vec2<f32>")
            .replace("vec3", "vec3<f32>")
            .replace("vec4", "vec4<f32>")
            .replace("mat2", "mat2x2<f32>")
            .replace("mat3", "mat3x3<f32>")
            .replace("mat4", "mat4x4<f32>")
            .replace("float", "f32")
            .replace("int", "i32");

        // GLSL mod to WGSL helpers
        converted = converted
            .replace("mod(", "modf(")
            .replace("mod(vec2", "mod2(vec2")
            .replace("mod(vec3", "mod3(vec3");

        // Output handling
        converted = converted.replace("gl_FragColor =", "out_color =");

        let mut wgsl_source = String::new();
        wgsl_source.push_str(&wgsl_header());
        wgsl_source.push_str("@fragment\n");
        wgsl_source.push_str("fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {\n");
        wgsl_source.push_str("  let uv: vec2<f32> = coord.xy / resolution;\n");
        wgsl_source.push_str("  var out_color: vec4<f32> = vec4<f32>(0.0, 0.0, 0.0, 1.0);\n");
        wgsl_source.push_str("  // Converted ISF body\n");
        wgsl_source.push_str(&converted);
        wgsl_source.push_str("\n  return out_color;\n}\n");

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