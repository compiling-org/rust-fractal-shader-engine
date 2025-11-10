use super::types::*;
use super::engine::FractalEngine;
use bevy::render::renderer::{RenderDevice, RenderQueue};
use std::sync::Arc;
use nalgebra::Vector3;
use bevy_egui::egui;

/// GPU-accelerated fractal renderer
  pub struct FractalRenderer {
    device: Arc<RenderDevice>,
    queue: Arc<RenderQueue>,
    compute_pipeline: wgpu::ComputePipeline,
    render_pipeline: wgpu::RenderPipeline,
    // Fragment pseudo‑3D pipeline (screen‑space raymarcher)
    pseudo3d_pipeline: wgpu::RenderPipeline,
    distance_field_buffer: wgpu::Buffer,
    parameter_buffer: wgpu::Buffer,
    output_texture: wgpu::Texture,
    output_texture_view: wgpu::TextureView,
    sampler: wgpu::Sampler,
    post_process_buffer: wgpu::Buffer,
    // Uniforms for pseudo‑3D fragment shader
    pseudo3d_uniform_buffer: wgpu::Buffer,
    engine: FractalEngine,
    width: u32,
    height: u32,
    // For egui texture integration
    egui_texture: Option<egui::TextureId>,
    // Camera field of view (degrees) for GPU renderer
    camera_fov: f32,
    // Camera target used to compute direction for pseudo‑3D
    camera_target: [f32; 3],
    // Lighting parameters for shaders
    light_direction: [f32; 3],
    light_color: [f32; 3],
    light_intensity: f32,
    // Simple material controls
    material_metallic: f32,
    material_roughness: f32,
    // Mode toggle for fragment pseudo‑3D rendering
    use_fragment_pseudo3d: bool,
    // Multi-formula combiner configuration
    combiner_active: bool,
    combiner_formulas: [i32; 3],
    combiner_blend_mode: crate::fractal::types::BlendMode,
    combiner_blend_factor: f32,
    combiner_smooth_k: f32,
    // Runtime monitoring
    frame_count: u64,
    last_workgroups: (u32, u32),
    last_preview_size: (u32, u32),
      last_map_time_ms: f32,
      // Persistent preview GPU texture and view (for egui paint callbacks)
      preview_texture: Option<wgpu::Texture>,
      preview_view: Option<wgpu::TextureView>,
  }

impl FractalRenderer {
    /// Create a new GPU-accelerated fractal renderer using provided WGPU device and queue
    pub fn new_with_wgpu_context(
        device: Arc<RenderDevice>,
        queue: Arc<RenderQueue>,
        width: u32,
        height: u32,
    ) -> Result<Self, Box<dyn std::error::Error>> {
        log::info!("Creating fractal renderer with size {}x{}", width, height);

        // Validate input parameters
        if width == 0 || height == 0 {
            return Err("Invalid texture dimensions: width and height must be greater than 0".into());
        }

        // Get the actual wgpu device and queue
        let wgpu_device = device.wgpu_device();
        let wgpu_queue = &queue.0;

        // Log device features and limits to confirm GPU capabilities/backends
        let feats = wgpu_device.features();
        let lims = wgpu_device.limits();
        log::info!("WGPU device features: {:?}", feats);
        log::info!("WGPU device limits: {:?}", lims);

        // Create compute shader for fractal evaluation with error handling
        let compute_shader = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_shader_module(wgpu::ShaderModuleDescriptor {
                label: Some("Fractal Compute Shader"),
                source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_compute.wgsl").into()),
            })
        })) {
            Ok(shader) => shader,
            Err(_) => return Err("Failed to create compute shader module".into()),
        };

        // Create render shader for displaying results with error handling
        let render_shader = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_shader_module(wgpu::ShaderModuleDescriptor {
                label: Some("Fractal Render Shader"),
                source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_render.wgsl").into()),
            })
        })) {
            Ok(shader) => shader,
            Err(_) => return Err("Failed to create render shader module".into()),
        };

        // Create compute pipeline with explicit bind group layout
        let bind_group_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
                label: Some("Fractal Compute Bind Group Layout"),
                entries: &[
                    wgpu::BindGroupLayoutEntry {
                        binding: 0,
                        visibility: wgpu::ShaderStages::COMPUTE,
                        ty: wgpu::BindingType::Buffer {
                            ty: wgpu::BufferBindingType::Storage { read_only: false },
                            has_dynamic_offset: false,
                            min_binding_size: None,
                        },
                        count: None,
                    },
                    wgpu::BindGroupLayoutEntry {
                        binding: 1,
                        visibility: wgpu::ShaderStages::COMPUTE,
                        ty: wgpu::BindingType::Buffer {
                            ty: wgpu::BufferBindingType::Uniform,
                            has_dynamic_offset: false,
                            min_binding_size: None,
                        },
                        count: None,
                    },
                    wgpu::BindGroupLayoutEntry {
                        binding: 2,
                        visibility: wgpu::ShaderStages::COMPUTE,
                        ty: wgpu::BindingType::StorageTexture {
                            access: wgpu::StorageTextureAccess::WriteOnly,
                            format: wgpu::TextureFormat::Rgba8Unorm,
                            view_dimension: wgpu::TextureViewDimension::D2,
                        },
                        count: None,
                    },
                ],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create compute bind group layout".into()),
        };

        let pipeline_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
                label: Some("Fractal Compute Pipeline Layout"),
                bind_group_layouts: &[&bind_group_layout],
                push_constant_ranges: &[],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create compute pipeline layout".into()),
        };

        let compute_pipeline = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_compute_pipeline(&wgpu::ComputePipelineDescriptor {
                label: Some("Fractal Compute Pipeline"),
                layout: Some(&pipeline_layout),
                module: &compute_shader,
                entry_point: Some("main"),
                compilation_options: wgpu::PipelineCompilationOptions::default(),
                cache: None,
            })
        })) {
            Ok(pipeline) => pipeline,
            Err(_) => return Err("Failed to create compute pipeline".into()),
        };

        // Create render pipeline with proper bind group layout for the render shader
        let render_bind_group_layout_0 = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
                label: Some("Fractal Render Bind Group Layout 0"),
                entries: &[
                    wgpu::BindGroupLayoutEntry {
                        binding: 0,
                        visibility: wgpu::ShaderStages::FRAGMENT,
                        ty: wgpu::BindingType::Texture {
                            sample_type: wgpu::TextureSampleType::Float { filterable: true },
                            view_dimension: wgpu::TextureViewDimension::D2,
                            multisampled: false,
                        },
                        count: None,
                    },
                    wgpu::BindGroupLayoutEntry {
                        binding: 1,
                        visibility: wgpu::ShaderStages::FRAGMENT,
                        ty: wgpu::BindingType::Sampler(wgpu::SamplerBindingType::Filtering),
                        count: None,
                    },
                ],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create render bind group layout 0".into()),
        };

        let render_bind_group_layout_1 = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
                label: Some("Fractal Render Bind Group Layout 1"),
                entries: &[
                    wgpu::BindGroupLayoutEntry {
                        binding: 0,
                        visibility: wgpu::ShaderStages::FRAGMENT,
                        ty: wgpu::BindingType::Buffer {
                            ty: wgpu::BufferBindingType::Uniform,
                            has_dynamic_offset: false,
                            min_binding_size: None,
                        },
                        count: None,
                    },
                ],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create render bind group layout 1".into()),
        };

        let render_pipeline_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
                label: Some("Fractal Render Pipeline Layout"),
                bind_group_layouts: &[&render_bind_group_layout_0, &render_bind_group_layout_1],
                push_constant_ranges: &[],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create render pipeline layout".into()),
        };

        let render_pipeline = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
                label: Some("Fractal Render Pipeline"),
                layout: Some(&render_pipeline_layout),
                vertex: wgpu::VertexState {
                    module: &render_shader,
                    entry_point: Some("vs_main"),
                    buffers: &[],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                },
                fragment: Some(wgpu::FragmentState {
                    module: &render_shader,
                    entry_point: Some("fs_main"),
                    targets: &[Some(wgpu::ColorTargetState {
                        format: wgpu::TextureFormat::Rgba8UnormSrgb,
                        blend: Some(wgpu::BlendState::REPLACE),
                        write_mask: wgpu::ColorWrites::ALL,
                    })],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                }),
                primitive: wgpu::PrimitiveState {
                    topology: wgpu::PrimitiveTopology::TriangleList,
                    strip_index_format: None,
                    front_face: wgpu::FrontFace::Ccw,
                    cull_mode: Some(wgpu::Face::Back),
                    unclipped_depth: false,
                    polygon_mode: wgpu::PolygonMode::Fill,
                    conservative: false,
                },
                depth_stencil: None,
                multisample: wgpu::MultisampleState::default(),
                multiview: None,
                cache: None,
            })
        })) {
            Ok(pipeline) => pipeline,
            Err(_) => return Err("Failed to create render pipeline".into()),
        };

        // Create fragment pseudo‑3D shader & pipeline
        let pseudo3d_shader = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_shader_module(wgpu::ShaderModuleDescriptor {
                label: Some("Pseudo3D Raymarch Shader"),
                source: wgpu::ShaderSource::Wgsl(include_str!("shaders/pseudo3d_raymarch.wgsl").into()),
            })
        })) {
            Ok(shader) => shader,
            Err(_) => return Err("Failed to create pseudo3D shader module".into()),
        };

        let pseudo3d_bind_group_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
                label: Some("Pseudo3D Bind Group Layout"),
                entries: &[wgpu::BindGroupLayoutEntry {
                    binding: 0,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Buffer {
                        ty: wgpu::BufferBindingType::Uniform,
                        has_dynamic_offset: false,
                        min_binding_size: None,
                    },
                    count: None,
                }],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create pseudo3D bind group layout".into()),
        };

        let pseudo3d_pipeline_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
                label: Some("Pseudo3D Pipeline Layout"),
                bind_group_layouts: &[&pseudo3d_bind_group_layout],
                push_constant_ranges: &[],
            })
        })) {
            Ok(layout) => layout,
            Err(_) => return Err("Failed to create pseudo3D pipeline layout".into()),
        };

        let pseudo3d_pipeline = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
                label: Some("Fragment Pseudo3D Pipeline"),
                layout: Some(&pseudo3d_pipeline_layout),
                vertex: wgpu::VertexState {
                    module: &pseudo3d_shader,
                    entry_point: Some("vs_main"),
                    buffers: &[],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                },
                fragment: Some(wgpu::FragmentState {
                    module: &pseudo3d_shader,
                    entry_point: Some("fs_main"),
                    targets: &[Some(wgpu::ColorTargetState {
                        format: wgpu::TextureFormat::Rgba8UnormSrgb,
                        blend: Some(wgpu::BlendState::REPLACE),
                        write_mask: wgpu::ColorWrites::ALL,
                    })],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                }),
                primitive: wgpu::PrimitiveState {
                    topology: wgpu::PrimitiveTopology::TriangleList,
                    strip_index_format: None,
                    front_face: wgpu::FrontFace::Ccw,
                    cull_mode: Some(wgpu::Face::Back),
                    unclipped_depth: false,
                    polygon_mode: wgpu::PolygonMode::Fill,
                    conservative: false,
                },
                depth_stencil: None,
                multisample: wgpu::MultisampleState::default(),
                multiview: None,
                cache: None,
            })
        })) {
            Ok(pipeline) => pipeline,
            Err(_) => return Err("Failed to create pseudo3D pipeline".into()),
        };

        // Create buffers
        // Create distance field buffer within device limits
        // Limit to maximum allowed buffer binding size (128MB)
        let max_buffer_binding_size = 134217728; // 128MB limit
        let requested_size = (width * height * std::mem::size_of::<f32>() as u32) as u64;
        let actual_size = std::cmp::min(requested_size, max_buffer_binding_size as u64);
        
        let distance_field_buffer = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Distance Field Buffer"),
                size: actual_size,
                usage: wgpu::BufferUsages::STORAGE | wgpu::BufferUsages::COPY_SRC | wgpu::BufferUsages::COPY_DST,
                mapped_at_creation: false,
            })
        })) {
            Ok(buffer) => buffer,
            Err(_) => return Err("Failed to create distance field buffer".into()),
        };

        let parameter_buffer = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Fractal Parameters Buffer"),
                size: 256 * std::mem::size_of::<f32>() as u64, // Enough space for all parameters
                usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
                mapped_at_creation: false,
            })
        })) {
            Ok(buffer) => buffer,
            Err(_) => return Err("Failed to create parameter buffer".into()),
        };

        // Create output texture with size limits
        // Limit texture dimensions to device limits (typically 8192)
        let max_dimension = 8192;
        let actual_width = std::cmp::min(width, max_dimension);
        let actual_height = std::cmp::min(height, max_dimension);
        
        let output_texture = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_texture(&wgpu::TextureDescriptor {
                label: Some("Fractal Output Texture"),
                size: wgpu::Extent3d {
                    width: actual_width,
                    height: actual_height,
                    depth_or_array_layers: 1,
                },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Rgba8Unorm,
                usage: wgpu::TextureUsages::STORAGE_BINDING
                    | wgpu::TextureUsages::TEXTURE_BINDING
                    | wgpu::TextureUsages::COPY_SRC,
                view_formats: &[],
            })
        })) {
            Ok(texture) => texture,
            Err(_) => return Err("Failed to create output texture".into()),
        };

        let output_texture_view = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            output_texture.create_view(&wgpu::TextureViewDescriptor::default())
        })) {
            Ok(view) => view,
            Err(_) => return Err("Failed to create output texture view".into()),
        };

        // Create sampler for the render shader
        let sampler = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_sampler(&wgpu::SamplerDescriptor {
                label: Some("Fractal Texture Sampler"),
                address_mode_u: wgpu::AddressMode::ClampToEdge,
                address_mode_v: wgpu::AddressMode::ClampToEdge,
                address_mode_w: wgpu::AddressMode::ClampToEdge,
                mag_filter: wgpu::FilterMode::Linear,
                min_filter: wgpu::FilterMode::Linear,
                mipmap_filter: wgpu::FilterMode::Linear,
                ..Default::default()
            })
        })) {
            Ok(sampler) => sampler,
            Err(_) => return Err("Failed to create texture sampler".into()),
        };

        // Create post-process parameters buffer
        let post_process_buffer = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Post Process Parameters Buffer"),
                size: 32 * std::mem::size_of::<f32>() as u64, // Enough space for post-process parameters
                usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
                mapped_at_creation: false,
            })
        })) {
            Ok(buffer) => buffer,
            Err(_) => return Err("Failed to create post-process buffer".into()),
        };

        // Pseudo‑3D uniforms buffer (matches CommonUniforms in WGSL)
        let pseudo3d_uniform_buffer = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Pseudo3D Uniform Buffer"),
                size: (16 * std::mem::size_of::<f32>()) as u64,
                usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
                mapped_at_creation: false,
            })
        })) {
            Ok(buffer) => buffer,
            Err(_) => return Err("Failed to create pseudo3D uniform buffer".into()),
        };

        let engine = FractalEngine::new();

        Ok(Self {
            device,
            queue,
            compute_pipeline,
            render_pipeline,
            pseudo3d_pipeline,
            distance_field_buffer,
            parameter_buffer,
            output_texture,
            output_texture_view,
            sampler,
            post_process_buffer,
            pseudo3d_uniform_buffer,
            engine,
            width: actual_width,
            height: actual_height,
            egui_texture: None,
            camera_fov: 60.0,
            camera_target: [0.0, 0.0, 0.0],
            // Lighting defaults
            light_direction: [0.4, 0.7, -0.2],
            light_color: [0.8, 0.9, 1.0],
            light_intensity: 1.0,
            // Material defaults
            material_metallic: 0.0,
            material_roughness: 0.5,
            use_fragment_pseudo3d: false,
            // Default: enable complex hybrid rendering combining multiple formulas
            combiner_active: true,
            // IDs follow shader mapping: 1 Mandelbulb, 2 Mandelbox, 4 Quaternion Julia
            combiner_formulas: [1, 2, 4],
            combiner_blend_mode: crate::fractal::types::BlendMode::SmoothUnion(0.5),
            combiner_blend_factor: 0.5,
            combiner_smooth_k: 0.3,
            frame_count: 0,
            last_workgroups: (0, 0),
            last_preview_size: (actual_width, actual_height),
            last_map_time_ms: 0.0,
            preview_texture: None,
            preview_view: None,
        })
    }

    /// Apply a predefined quality preset on the underlying engine
    pub fn apply_quality_preset(
        &mut self,
        preset: crate::fractal::types::QualityPreset,
        base_resolution: Option<[u32; 2]>,
    ) {
        self.engine.apply_quality_preset(preset, base_resolution);
    }

    /// Set explicit quality settings on the underlying engine
    pub fn set_quality_settings(&mut self, settings: crate::fractal::types::QualitySettings) {
        self.engine.set_quality_settings(settings);
    }

    /// Get current quality settings from the engine
    pub fn get_quality_settings(&self) -> crate::fractal::types::QualitySettings {
        self.engine.quality_settings.clone()
    }

    /// Render a single frame - just submits the compute pass, doesn't read back data
    pub fn render_frame(&mut self, time: f32, resolution: (u32, u32)) -> Result<(), Box<dyn std::error::Error>> {
        log::debug!("Starting render frame with time: {}, resolution: {:?}", time, resolution);
        
        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;
        
        // Create command encoder with error handling
        let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder"),
        });

        // Update parameters
        let params = self.create_parameter_data(time, resolution);
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass
        {
            log::debug!("Starting compute pass");
            let bind_group = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Compute Bind Group"),
                layout: &self.compute_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry {
                        binding: 0,
                        resource: self.distance_field_buffer.as_entire_binding(),
                    },
                    wgpu::BindGroupEntry {
                        binding: 1,
                        resource: self.parameter_buffer.as_entire_binding(),
                    },
                    wgpu::BindGroupEntry {
                        binding: 2,
                        resource: wgpu::BindingResource::TextureView(&self.output_texture_view),
                    },
                ],
            });

            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass"),
                timestamp_writes: None,
            });

            compute_pass.set_pipeline(&self.compute_pipeline);
            compute_pass.set_bind_group(0, &bind_group, &[]);
            
            let work_groups_x = (self.width + 15) / 16;
            let work_groups_y = (self.height + 15) / 16;
            log::debug!("Dispatching compute workgroups: {} x {}", work_groups_x, work_groups_y);
            compute_pass.dispatch_workgroups(work_groups_x, work_groups_y, 1);
            log::debug!("Compute pass completed");
        }

        // Submit command buffer for compute pass
        log::debug!("Submitting compute command buffer");
        wgpu_queue.submit(Some(encoder.finish()));
        
        log::debug!("Frame rendering completed successfully");
        Ok(())
    }

    /// Render a frame to a persistent GPU texture for viewport display (no CPU readback)
    pub fn render_frame_to_view(&mut self, time: f32, resolution: (u32, u32)) -> Result<(), Box<dyn std::error::Error>> {
        let (pw, ph) = (resolution.0.max(1), resolution.1.max(1));
        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;

        // If fragment pseudo‑3D mode is enabled, render directly via fragment pipeline
        if self.use_fragment_pseudo3d {
            // Create or resize persistent preview texture first
            let needs_new = match &self.preview_texture {
                Some(tex) => {
                    let size = tex.size();
                    size.width != pw || size.height != ph
                }
                None => true,
            };
            if needs_new {
                self.preview_texture = Some(wgpu_device.create_texture(&wgpu::TextureDescriptor {
                    label: Some("Fractal Preview Texture (Persistent)"),
                    size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
                    mip_level_count: 1,
                    sample_count: 1,
                    dimension: wgpu::TextureDimension::D2,
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    usage: wgpu::TextureUsages::RENDER_ATTACHMENT | wgpu::TextureUsages::TEXTURE_BINDING | wgpu::TextureUsages::COPY_SRC,
                    view_formats: &[],
                }));
                self.preview_view = self
                    .preview_texture
                    .as_ref()
                    .map(|t| t.create_view(&wgpu::TextureViewDescriptor::default()));
            }

            if let Some(preview_view) = &self.preview_view {
                // Update fragment uniforms
                let camera_pos = [
                    self.engine.parameters.position.x,
                    self.engine.parameters.position.y,
                    self.engine.parameters.position.z,
                ];
                // Compute camera direction from position/target for pseudo‑3D shaders
                let mut camera_dir = [
                    self.camera_target[0] - camera_pos[0],
                    self.camera_target[1] - camera_pos[1],
                    self.camera_target[2] - camera_pos[2],
                ];
                let len = (camera_dir[0] * camera_dir[0]
                    + camera_dir[1] * camera_dir[1]
                    + camera_dir[2] * camera_dir[2])
                    .sqrt();
                if len > 1e-6 {
                    camera_dir[0] /= len;
                    camera_dir[1] /= len;
                    camera_dir[2] /= len;
                } else {
                    camera_dir = [0.0, 0.0, -1.0];
                }
                let uniform_data: [f32; 16] = [
                    pw as f32,
                    ph as f32,
                    time,
                    0.0,
                    camera_pos[0],
                    camera_pos[1],
                    camera_pos[2],
                    0.0,
                    camera_dir[0],
                    camera_dir[1],
                    camera_dir[2],
                    self.camera_fov,
                    self.engine.parameters.scale,
                    1.0, // folding limit default; could be parameterized
                    self.engine.parameters.bailout,
                    self.engine.parameters.max_iterations as f32,
                ];
                wgpu_queue.write_buffer(
                    &self.pseudo3d_uniform_buffer,
                    0,
                    bytemuck::cast_slice(&uniform_data),
                );

                let pseudo_bg = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                    label: Some("Pseudo3D Bind Group (View)"),
                    layout: &self.pseudo3d_pipeline.get_bind_group_layout(0),
                    entries: &[wgpu::BindGroupEntry {
                        binding: 0,
                        resource: self.pseudo3d_uniform_buffer.as_entire_binding(),
                    }],
                });

                // Render pass with pseudo3D pipeline directly into the persistent preview texture
                let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                    label: Some("Pseudo3D Render Encoder (View)"),
                });
                {
                    let mut render_pass = encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                        label: Some("Pseudo3D Render Pass (View)"),
                        color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                            view: preview_view,
                            resolve_target: None,
                            depth_slice: None,
                            ops: wgpu::Operations {
                                load: wgpu::LoadOp::Clear(wgpu::Color::BLACK),
                                store: wgpu::StoreOp::Store,
                            },
                        })],
                        depth_stencil_attachment: None,
                        timestamp_writes: None,
                        occlusion_query_set: None,
                    });
                    render_pass.set_pipeline(&self.pseudo3d_pipeline);
                    render_pass.set_bind_group(0, &pseudo_bg, &[]);
                    render_pass.draw(0..6, 0..1);
                }
                wgpu_queue.submit(Some(encoder.finish()));
                self.last_preview_size = (pw, ph);
            }

            self.frame_count += 1;
            return Ok(());
        }

        // Temporary fractal output used by compute pass
        let temp_output_texture = wgpu_device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Fractal Preview Output Texture (GPU-only)"),
            size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8Unorm,
            usage: wgpu::TextureUsages::COPY_SRC | wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::TEXTURE_BINDING,
            view_formats: &[],
        });
        let temp_output_view = temp_output_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Update parameters
        let params = self.create_parameter_data(time, (pw, ph));
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass
        {
            let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                label: Some("Fractal Compute Encoder (GPU-only)"),
            });
            let bind_group = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Compute Bind Group (GPU-only)"),
                layout: &self.compute_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry { binding: 0, resource: self.distance_field_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 1, resource: self.parameter_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 2, resource: wgpu::BindingResource::TextureView(&temp_output_view) },
                ],
            });
            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass (GPU-only)"),
                timestamp_writes: None,
            });
            compute_pass.set_pipeline(&self.compute_pipeline);
            compute_pass.set_bind_group(0, &bind_group, &[]);
            let work_groups_x = (pw + 15) / 16;
            let work_groups_y = (ph + 15) / 16;
            self.last_workgroups = (work_groups_x, work_groups_y);
            self.last_preview_size = (pw, ph);
            compute_pass.dispatch_workgroups(work_groups_x, work_groups_y, 1);
            drop(compute_pass);
            wgpu_queue.submit(Some(encoder.finish()));
        }

        // Post-process parameters
        let post_params: [f32; 7] = [
            time,
            0.2, // bloom
            0.2, // vignette
            0.0, // color temperature
            1.05, // contrast
            0.0, // brightness
            self.engine.parameters.color_saturation, // saturation
        ];
        wgpu_queue.write_buffer(&self.post_process_buffer, 0, bytemuck::cast_slice(&post_params));

        // Create or resize persistent preview texture
        let needs_new = match &self.preview_texture { Some(tex) => {
                let size = tex.size();
                size.width != pw || size.height != ph
            }, None => true };
        if needs_new {
            self.preview_texture = Some(wgpu_device.create_texture(&wgpu::TextureDescriptor {
                label: Some("Fractal Preview Texture (Persistent)"),
                size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Rgba8UnormSrgb,
                usage: wgpu::TextureUsages::RENDER_ATTACHMENT | wgpu::TextureUsages::TEXTURE_BINDING | wgpu::TextureUsages::COPY_SRC,
                view_formats: &[],
            }));
            self.preview_view = self.preview_texture.as_ref().map(|t| t.create_view(&wgpu::TextureViewDescriptor::default()));
        }

        // Render pass to blit post-processed result into persistent preview texture
        if let Some(preview_view) = &self.preview_view {
            let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                label: Some("Fractal Post-Process Encoder (GPU-only)"),
            });

            let render_bg0 = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Render BG0 (GPU-only)"),
                layout: &self.render_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry { binding: 0, resource: wgpu::BindingResource::TextureView(&temp_output_view) },
                    wgpu::BindGroupEntry { binding: 1, resource: wgpu::BindingResource::Sampler(&self.sampler) },
                ],
            });

            let render_bg1 = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Render BG1 (GPU-only)"),
                layout: &self.render_pipeline.get_bind_group_layout(1),
                entries: &[
                    wgpu::BindGroupEntry { binding: 0, resource: self.post_process_buffer.as_entire_binding() },
                ],
            });

            {
                let mut render_pass = encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                    label: Some("Fractal Post-Process Render Pass (GPU-only)"),
                    color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                        view: preview_view,
                        resolve_target: None,
                        depth_slice: None,
                        ops: wgpu::Operations { load: wgpu::LoadOp::Clear(wgpu::Color::BLACK), store: wgpu::StoreOp::Store },
                    })],
                    depth_stencil_attachment: None,
                    timestamp_writes: None,
                    occlusion_query_set: None,
                });
                render_pass.set_pipeline(&self.render_pipeline);
                render_pass.set_bind_group(0, &render_bg0, &[]);
                render_pass.set_bind_group(1, &render_bg1, &[]);
                render_pass.draw(0..6, 0..1);
            }

            wgpu_queue.submit(Some(encoder.finish()));
        }

        self.frame_count += 1;
        Ok(())
    }
    
    /// Render a single frame and update the egui texture
    pub fn render_frame_to_texture(&mut self, time: f32, resolution: (u32, u32), ctx: &egui::Context) -> Result<egui::TextureId, Box<dyn std::error::Error>> {
        // Use requested preview resolution to reduce GPU and CPU workload
        let (pw, ph) = (resolution.0.max(1), resolution.1.max(1));
        log::debug!(
            "Starting render frame with time: {}, preview resolution: {}x{}",
            time, pw, ph
        );

        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;

        // Create a temporary output texture sized to the preview
        let temp_output_texture = wgpu_device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Fractal Preview Output Texture"),
            size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8Unorm,
            usage: wgpu::TextureUsages::COPY_SRC | wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::TEXTURE_BINDING,
            view_formats: &[],
        });
        let temp_output_view = temp_output_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Create command encoder
        let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder"),
        });

        // If fragment pseudo‑3D mode is enabled, render directly via fragment pipeline
        if self.use_fragment_pseudo3d {
            // Create a post‑processed texture to render into using the pseudo3D pipeline
            let post_texture = wgpu_device.create_texture(&wgpu::TextureDescriptor {
                label: Some("Pseudo3D Preview Texture"),
                size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Rgba8UnormSrgb,
                usage: wgpu::TextureUsages::RENDER_ATTACHMENT | wgpu::TextureUsages::COPY_SRC | wgpu::TextureUsages::TEXTURE_BINDING,
                view_formats: &[],
            });
            let post_view = post_texture.create_view(&wgpu::TextureViewDescriptor::default());

            // Update fragment uniforms
            let camera_pos = [
                self.engine.parameters.position.x,
                self.engine.parameters.position.y,
                self.engine.parameters.position.z,
            ];
            // Compute camera direction from position/target for pseudo‑3D shaders
            let mut camera_dir = [
                self.camera_target[0] - camera_pos[0],
                self.camera_target[1] - camera_pos[1],
                self.camera_target[2] - camera_pos[2],
            ];
            let len = (camera_dir[0] * camera_dir[0]
                + camera_dir[1] * camera_dir[1]
                + camera_dir[2] * camera_dir[2]).sqrt();
            if len > 1e-6 {
                camera_dir[0] /= len;
                camera_dir[1] /= len;
                camera_dir[2] /= len;
            } else {
                camera_dir = [0.0, 0.0, -1.0];
            }
            let uniform_data: [f32; 16] = [
                pw as f32, ph as f32, time, 0.0,
                camera_pos[0], camera_pos[1], camera_pos[2], 0.0,
                camera_dir[0], camera_dir[1], camera_dir[2], self.camera_fov,
                self.engine.parameters.scale,
                1.0, // folding limit default; could be parameterized
                self.engine.parameters.bailout,
                self.engine.parameters.max_iterations as f32,
            ];
            wgpu_queue.write_buffer(&self.pseudo3d_uniform_buffer, 0, bytemuck::cast_slice(&uniform_data));

            let pseudo_bg = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Pseudo3D Bind Group (Preview)"),
                layout: &self.pseudo3d_pipeline.get_bind_group_layout(0),
                entries: &[wgpu::BindGroupEntry {
                    binding: 0,
                    resource: self.pseudo3d_uniform_buffer.as_entire_binding(),
                }],
            });

            // Render pass with pseudo3D pipeline
            let mut enc = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                label: Some("Pseudo3D Render Encoder (Preview)"),
            });
            {
                let mut render_pass = enc.begin_render_pass(&wgpu::RenderPassDescriptor {
                    label: Some("Pseudo3D Render Pass (Preview)"),
                    color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                        view: &post_view,
                        resolve_target: None,
                        depth_slice: None,
                        ops: wgpu::Operations { load: wgpu::LoadOp::Clear(wgpu::Color::BLACK), store: wgpu::StoreOp::Store },
                    })],
                    depth_stencil_attachment: None,
                    timestamp_writes: None,
                    occlusion_query_set: None,
                });
                render_pass.set_pipeline(&self.pseudo3d_pipeline);
                render_pass.set_bind_group(0, &pseudo_bg, &[]);
                render_pass.draw(0..6, 0..1);
            }
            wgpu_queue.submit(Some(enc.finish()));

            // Copy and map to egui texture (strip row padding like in default path)
            let bytes_per_pixel: u32 = 4;
            let unpadded_bytes_per_row: u32 = pw * bytes_per_pixel;
            let align: u32 = wgpu::COPY_BYTES_PER_ROW_ALIGNMENT; // 256
            let padded_bytes_per_row: u32 = ((unpadded_bytes_per_row + align - 1) / align) * align;
            let readback_size: u64 = (padded_bytes_per_row as u64) * (ph as u64);

            let output_buffer = wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Pseudo3D Preview Readback Buffer"),
                size: readback_size,
                usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
                mapped_at_creation: false,
            });

            let mut copy_encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                label: Some("Pseudo3D Copy Encoder (Preview)"),
            });
            copy_encoder.copy_texture_to_buffer(
                wgpu::TexelCopyTextureInfo { texture: &post_texture, mip_level: 0, origin: wgpu::Origin3d::ZERO, aspect: wgpu::TextureAspect::All },
                wgpu::TexelCopyBufferInfo {
                    buffer: &output_buffer,
                    layout: wgpu::TexelCopyBufferLayout { offset: 0, bytes_per_row: Some(padded_bytes_per_row), rows_per_image: Some(ph) },
                },
                wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            );
            wgpu_queue.submit(Some(copy_encoder.finish()));

            // Map buffer and produce egui texture
            let buffer_slice = output_buffer.slice(..);
            let (sender, receiver) = futures::channel::oneshot::channel();
            buffer_slice.map_async(wgpu::MapMode::Read, move |result| { let _ = sender.send(result); });
            let _ = wgpu_device.poll(wgpu::PollType::Wait);
            match pollster::block_on(receiver) { Ok(Ok(())) => {}, Ok(Err(e)) => return Err(format!("Failed to map buffer: {}", e).into()), Err(_) => return Err("Failed to receive buffer mapping result".into()), }

            let data = buffer_slice.get_mapped_range();
            let mapped: &[u8] = &data;
            let row_stride_src = padded_bytes_per_row as usize;
            let row_stride_dst = unpadded_bytes_per_row as usize;
            let mut pixels: Vec<u8> = vec![0u8; (row_stride_dst as u64 * ph as u64) as usize];
            for row in 0..(ph as usize) {
                let src_start = row * row_stride_src;
                let dst_start = row * row_stride_dst;
                let src_slice = &mapped[src_start..src_start + row_stride_dst];
                pixels[dst_start..dst_start + row_stride_dst].copy_from_slice(src_slice);
            }
            drop(data);
            output_buffer.unmap();

            let width = pw as usize;
            let height = ph as usize;
            if pixels.len() != width * height * 4 { return Err(format!("Invalid pixel data size: expected {} ({}x{}), got {}", width * height * 4, width, height, pixels.len()).into()); }
            let mut color_pixels = Vec::with_capacity(width * height);
            for chunk in pixels.chunks_exact(4) { color_pixels.push(egui::Color32::from_rgba_premultiplied(chunk[0], chunk[1], chunk[2], chunk[3])); }
            let color_image = egui::ColorImage::new([width, height], color_pixels);

            if let Some(texture_id) = self.egui_texture {
                let image_delta = egui::epaint::ImageDelta::full(egui::ImageData::Color(std::sync::Arc::new(color_image)), egui::TextureOptions::default());
                ctx.tex_manager().write().set(texture_id, image_delta);
                return Ok(texture_id);
            } else {
                let texture_id = ctx.tex_manager().write().alloc("fractal_viewport".to_string(), egui::ImageData::Color(std::sync::Arc::new(color_image)), egui::TextureOptions::default());
                self.egui_texture = Some(texture_id);
                return Ok(texture_id);
            }
        }

        // Default path: compute + post‑process render pipeline
        // Update parameters using preview resolution
        let params = self.create_parameter_data(time, (pw, ph));
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass targeting the temporary texture
        {
            let bind_group = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Compute Bind Group (Preview)"),
                layout: &self.compute_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry { binding: 0, resource: self.distance_field_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 1, resource: self.parameter_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 2, resource: wgpu::BindingResource::TextureView(&temp_output_view) },
                ],
            });

            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass (Preview)"),
                timestamp_writes: None,
            });

            compute_pass.set_pipeline(&self.compute_pipeline);
            compute_pass.set_bind_group(0, &bind_group, &[]);
            let work_groups_x = (pw + 15) / 16;
            let work_groups_y = (ph + 15) / 16;
            self.last_workgroups = (work_groups_x, work_groups_y);
            self.last_preview_size = (pw, ph);
            compute_pass.dispatch_workgroups(work_groups_x, work_groups_y, 1);
        }

        // Submit compute
        wgpu_queue.submit(Some(encoder.finish()));

        // Update post-process parameters (basic defaults mapped from engine state)
        // Fields: time, bloom_intensity, vignette_amount, color_temperature, contrast, brightness, saturation
        let pp_time = time;
        let pp_bloom = 0.2f32;
        let pp_vignette = 0.2f32;
        let pp_temperature = 0.0f32; // neutral
        let pp_contrast = 1.05f32;
        let pp_brightness = 0.0f32;
        let pp_saturation = self.engine.parameters.color_saturation;
        let post_params: [f32; 7] = [
            pp_time,
            pp_bloom,
            pp_vignette,
            pp_temperature,
            pp_contrast,
            pp_brightness,
            pp_saturation,
        ];
        wgpu_queue.write_buffer(&self.post_process_buffer, 0, bytemuck::cast_slice(&post_params));

        // Create a post-processed texture to render into using the render pipeline
        let post_texture = wgpu_device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Fractal Preview Post-Processed Texture"),
            size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8UnormSrgb,
            usage: wgpu::TextureUsages::RENDER_ATTACHMENT | wgpu::TextureUsages::COPY_SRC | wgpu::TextureUsages::TEXTURE_BINDING,
            view_formats: &[],
        });
        let post_view = post_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Render pass to apply post-processing (FXAA, bloom, vignette, grading)
        {
            let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
                label: Some("Fractal Post-Process Render Encoder (Preview)"),
            });

            // Bind fractal texture + sampler for render pipeline group 0
            let render_bg0 = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Render Bind Group 0 (Preview)"),
                layout: &self.render_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry {
                        binding: 0,
                        resource: wgpu::BindingResource::TextureView(&temp_output_view),
                    },
                    wgpu::BindGroupEntry {
                        binding: 1,
                        resource: wgpu::BindingResource::Sampler(&self.sampler),
                    },
                ],
            });

            // Bind post-process params for render pipeline group 1
            let render_bg1 = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Render Bind Group 1 (Preview)"),
                layout: &self.render_pipeline.get_bind_group_layout(1),
                entries: &[
                    wgpu::BindGroupEntry {
                        binding: 0,
                        resource: self.post_process_buffer.as_entire_binding(),
                    },
                ],
            });

            {
                let mut render_pass = encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                    label: Some("Fractal Post-Process Render Pass (Preview)"),
                    color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                        view: &post_view,
                        resolve_target: None,
                        depth_slice: None,
                        ops: wgpu::Operations {
                            load: wgpu::LoadOp::Clear(wgpu::Color::BLACK),
                            store: wgpu::StoreOp::Store,
                        },
                    })],
                    depth_stencil_attachment: None,
                    timestamp_writes: None,
                    occlusion_query_set: None,
                });

                render_pass.set_pipeline(&self.render_pipeline);
                render_pass.set_bind_group(0, &render_bg0, &[]);
                render_pass.set_bind_group(1, &render_bg1, &[]);
                render_pass.draw(0..6, 0..1);
            }

            // Copy the post-processed texture to a readback buffer after rendering
            // Prepare readback buffer sized to preview texture
            let bytes_per_pixel: u32 = 4;
            let unpadded_bytes_per_row: u32 = pw * bytes_per_pixel;
            let align: u32 = wgpu::COPY_BYTES_PER_ROW_ALIGNMENT; // 256
            let padded_bytes_per_row: u32 = ((unpadded_bytes_per_row + align - 1) / align) * align;
            let readback_size: u64 = (padded_bytes_per_row as u64) * (ph as u64);

            let output_buffer = wgpu_device.create_buffer(&wgpu::BufferDescriptor {
                label: Some("Fractal Preview Readback Buffer"),
                size: readback_size,
                usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
                mapped_at_creation: false,
            });

            encoder.copy_texture_to_buffer(
                wgpu::TexelCopyTextureInfo { texture: &post_texture, mip_level: 0, origin: wgpu::Origin3d::ZERO, aspect: wgpu::TextureAspect::All },
                wgpu::TexelCopyBufferInfo {
                    buffer: &output_buffer,
                    layout: wgpu::TexelCopyBufferLayout { offset: 0, bytes_per_row: Some(padded_bytes_per_row), rows_per_image: Some(ph) },
                },
                wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            );

            wgpu_queue.submit(Some(encoder.finish()));

            // Map the buffer and read the data with proper error handling
            let buffer_slice = output_buffer.slice(..);
            let (sender, receiver) = futures::channel::oneshot::channel();
            buffer_slice.map_async(wgpu::MapMode::Read, move |result| {
                let _ = sender.send(result);
            });

            // Wait for the buffer to be mapped
            let map_start = std::time::Instant::now();
            let _ = wgpu_device.poll(wgpu::PollType::Wait);
            let result = match pollster::block_on(receiver) {
                Ok(result) => result,
                Err(_) => return Err("Failed to receive buffer mapping result".into()),
            };
            match result { Ok(()) => {}, Err(e) => return Err(format!("Failed to map buffer: {}", e).into()) }
            self.last_map_time_ms = map_start.elapsed().as_secs_f32() * 1000.0;

            // Get the data from the buffer and strip row padding
            let data = buffer_slice.get_mapped_range();
            let mapped: &[u8] = &data;
            let row_stride_src = padded_bytes_per_row as usize;
            let row_stride_dst = unpadded_bytes_per_row as usize;
            let mut pixels: Vec<u8> = vec![0u8; (row_stride_dst as u64 * ph as u64) as usize];
            for row in 0..(ph as usize) {
                let src_start = row * row_stride_src;
                let dst_start = row * row_stride_dst;
                let src_slice = &mapped[src_start..src_start + row_stride_dst];
                pixels[dst_start..dst_start + row_stride_dst].copy_from_slice(src_slice);
            }
            drop(data);
            output_buffer.unmap();

            // Convert the raw pixel data to egui::Color32 using internal texture size
            let width = pw as usize;
            let height = ph as usize;
            if pixels.len() != width * height * 4 {
                return Err(format!(
                    "Invalid pixel data size: expected {} ({}x{}), got {}",
                    width * height * 4,
                    width,
                    height,
                    pixels.len()
                ).into());
            }

            let mut color_pixels = Vec::with_capacity(width * height);
            for chunk in pixels.chunks_exact(4) {
                color_pixels.push(egui::Color32::from_rgba_premultiplied(chunk[0], chunk[1], chunk[2], chunk[3]));
            }
            let color_image = egui::ColorImage::new([width, height], color_pixels);

            // Return the texture ID for egui
            self.frame_count += 1;
            if let Some(texture_id) = self.egui_texture {
                let image_delta = egui::epaint::ImageDelta::full(
                    egui::ImageData::Color(std::sync::Arc::new(color_image)),
                    egui::TextureOptions::default(),
                );
                ctx.tex_manager().write().set(texture_id, image_delta);
                return Ok(texture_id);
            } else {
                let texture_id = ctx.tex_manager().write().alloc(
                    "fractal_viewport".to_string(),
                    egui::ImageData::Color(std::sync::Arc::new(color_image)),
                    egui::TextureOptions::default(),
                );
                self.egui_texture = Some(texture_id);
                return Ok(texture_id);
            }
        }
    }

    /// Toggle fragment pseudo‑3D rendering mode
    pub fn set_fragment_pseudo3d(&mut self, enabled: bool) {
        self.use_fragment_pseudo3d = enabled;
    }

    // Dynamically compile a WGSL fragment shader and swap the pseudo‑3D pipeline.
    // Expects an fs_main fragment entry point. A fullscreen‑quad vertex shader is provided.
    pub fn load_fragment_wgsl(&mut self, wgsl_source: &str) -> Result<(), String> {
        let wgpu_device = self.device.wgpu_device();

        // Create fragment module from provided WGSL
        let fragment_module = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_shader_module(wgpu::ShaderModuleDescriptor {
                label: Some("Dynamic Fragment Shader Module"),
                source: wgpu::ShaderSource::Wgsl(wgsl_source.into()),
            })
        })) {
            Ok(module) => module,
            Err(panic_payload) => {
                let msg = if let Some(s) = panic_payload.downcast_ref::<&str>() {
                    (*s).to_string()
                } else if let Some(s) = panic_payload.downcast_ref::<String>() {
                    s.clone()
                } else {
                    "Failed to create fragment shader module".to_string()
                };
                return Err(msg);
            }
        };

        // Provide a default fullscreen‑quad vertex shader that outputs uv at location 0
        const FSQ_VERTEX_WGSL: &str = r#"
            @vertex
            fn vs_main(@builtin(vertex_index) vertex_index: u32) -> (
                @builtin(position) vec4<f32>,
                @location(0) vec2<f32>
            ) {
                var positions = array<vec2<f32>, 6>(
                    vec2<f32>(-1.0, -1.0), vec2<f32>( 1.0, -1.0), vec2<f32>( 1.0,  1.0),
                    vec2<f32>(-1.0, -1.0), vec2<f32>( 1.0,  1.0), vec2<f32>(-1.0,  1.0)
                );
                let pos = positions[vertex_index];
                let uv = 0.5 * (pos + vec2<f32>(1.0, 1.0));
                return (vec4<f32>(pos, 0.0, 1.0), uv);
            }
        "#;
        let vertex_module = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_shader_module(wgpu::ShaderModuleDescriptor {
                label: Some("Fullscreen Quad Vertex Module"),
                source: wgpu::ShaderSource::Wgsl(FSQ_VERTEX_WGSL.into()),
            })
        })) {
            Ok(module) => module,
            Err(panic_payload) => {
                let msg = if let Some(s) = panic_payload.downcast_ref::<&str>() {
                    (*s).to_string()
                } else if let Some(s) = panic_payload.downcast_ref::<String>() {
                    s.clone()
                } else {
                    "Failed to create vertex shader module".to_string()
                };
                return Err(msg);
            }
        };

        // Bind group layout: single uniform buffer at binding 0 for fragment stage
        let bind_group_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
                label: Some("Dynamic Pseudo3D Bind Group Layout"),
                entries: &[wgpu::BindGroupLayoutEntry {
                    binding: 0,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Buffer {
                        ty: wgpu::BufferBindingType::Uniform,
                        has_dynamic_offset: false,
                        min_binding_size: None,
                    },
                    count: None,
                }],
            })
        })) {
            Ok(layout) => layout,
            Err(panic_payload) => {
                let msg = if let Some(s) = panic_payload.downcast_ref::<&str>() {
                    (*s).to_string()
                } else if let Some(s) = panic_payload.downcast_ref::<String>() {
                    s.clone()
                } else {
                    "Failed to create dynamic bind group layout".to_string()
                };
                return Err(msg);
            }
        };

        let pipeline_layout = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
                label: Some("Dynamic Pseudo3D Pipeline Layout"),
                bind_group_layouts: &[&bind_group_layout],
                push_constant_ranges: &[],
            })
        })) {
            Ok(layout) => layout,
            Err(panic_payload) => {
                let msg = if let Some(s) = panic_payload.downcast_ref::<&str>() {
                    (*s).to_string()
                } else if let Some(s) = panic_payload.downcast_ref::<String>() {
                    s.clone()
                } else {
                    "Failed to create dynamic pipeline layout".to_string()
                };
                return Err(msg);
            }
        };

        let pipeline = match std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            wgpu_device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
                label: Some("Dynamic Fragment Pseudo3D Pipeline"),
                layout: Some(&pipeline_layout),
                vertex: wgpu::VertexState {
                    module: &vertex_module,
                    entry_point: Some("vs_main"),
                    buffers: &[],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                },
                fragment: Some(wgpu::FragmentState {
                    module: &fragment_module,
                    entry_point: Some("fs_main"),
                    targets: &[Some(wgpu::ColorTargetState {
                        format: wgpu::TextureFormat::Rgba8UnormSrgb,
                        blend: Some(wgpu::BlendState::REPLACE),
                        write_mask: wgpu::ColorWrites::ALL,
                    })],
                    compilation_options: wgpu::PipelineCompilationOptions::default(),
                }),
                primitive: wgpu::PrimitiveState {
                    topology: wgpu::PrimitiveTopology::TriangleList,
                    strip_index_format: None,
                    front_face: wgpu::FrontFace::Ccw,
                    cull_mode: Some(wgpu::Face::Back),
                    unclipped_depth: false,
                    polygon_mode: wgpu::PolygonMode::Fill,
                    conservative: false,
                },
                depth_stencil: None,
                multisample: wgpu::MultisampleState::default(),
                multiview: None,
                cache: None,
            })
        })) {
            Ok(pipeline) => pipeline,
            Err(panic_payload) => {
                let msg = if let Some(s) = panic_payload.downcast_ref::<&str>() {
                    (*s).to_string()
                } else if let Some(s) = panic_payload.downcast_ref::<String>() {
                    s.clone()
                } else {
                    "Failed to create dynamic fragment pipeline".to_string()
                };
                return Err(msg);
            }
        };

        // Swap pipeline
        self.pseudo3d_pipeline = pipeline;
        Ok(())
    }

    /// Render a frame to a temporary texture and return raw RGBA8 pixels
    pub fn render_image_readback(
        &mut self,
        time: f32,
        resolution: (u32, u32),
    ) -> Result<(Vec<u8>, u32, u32), Box<dyn std::error::Error>> {
        let (pw, ph) = (resolution.0.max(1), resolution.1.max(1));
        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;

        // Temporary output texture
        let temp_output_texture = wgpu_device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Fractal Export Output Texture"),
            size: wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8Unorm,
            usage: wgpu::TextureUsages::COPY_SRC | wgpu::TextureUsages::STORAGE_BINDING,
            view_formats: &[],
        });
        let temp_output_view = temp_output_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Compute encoder
        let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder (Export)"),
        });

        // Update parameters
        let params = self.create_parameter_data(time, (pw, ph));
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass
        {
            let bind_group = wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
                label: Some("Fractal Compute Bind Group (Export)"),
                layout: &self.compute_pipeline.get_bind_group_layout(0),
                entries: &[
                    wgpu::BindGroupEntry { binding: 0, resource: self.distance_field_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 1, resource: self.parameter_buffer.as_entire_binding() },
                    wgpu::BindGroupEntry { binding: 2, resource: wgpu::BindingResource::TextureView(&temp_output_view) },
                ],
            });

            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass (Export)"),
                timestamp_writes: None,
            });

            compute_pass.set_pipeline(&self.compute_pipeline);
            compute_pass.set_bind_group(0, &bind_group, &[]);
            let work_groups_x = (pw + 15) / 16;
            let work_groups_y = (ph + 15) / 16;
            compute_pass.dispatch_workgroups(work_groups_x, work_groups_y, 1);
        }

        wgpu_queue.submit(Some(encoder.finish()));

        // Readback buffer
        let bytes_per_pixel: u32 = 4;
        let unpadded_bytes_per_row: u32 = pw * bytes_per_pixel;
        let align: u32 = wgpu::COPY_BYTES_PER_ROW_ALIGNMENT; // 256
        let padded_bytes_per_row: u32 = ((unpadded_bytes_per_row + align - 1) / align) * align;
        let readback_size: u64 = (padded_bytes_per_row as u64) * (ph as u64);

        let output_buffer = wgpu_device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Fractal Export Readback Buffer"),
            size: readback_size,
            usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
            mapped_at_creation: false,
        });

        // Copy texture to buffer
        let mut encoder = wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Copy Encoder (Export)"),
        });

        encoder.copy_texture_to_buffer(
            wgpu::TexelCopyTextureInfo { texture: &temp_output_texture, mip_level: 0, origin: wgpu::Origin3d::ZERO, aspect: wgpu::TextureAspect::All },
            wgpu::TexelCopyBufferInfo {
                buffer: &output_buffer,
                layout: wgpu::TexelCopyBufferLayout { offset: 0, bytes_per_row: Some(padded_bytes_per_row), rows_per_image: Some(ph) },
            },
            wgpu::Extent3d { width: pw, height: ph, depth_or_array_layers: 1 },
        );

        wgpu_queue.submit(Some(encoder.finish()));

        // Map buffer
        let buffer_slice = output_buffer.slice(..);
        let (sender, receiver) = futures::channel::oneshot::channel();
        buffer_slice.map_async(wgpu::MapMode::Read, move |result| {
            let _ = sender.send(result);
        });
        let _ = wgpu_device.poll(wgpu::PollType::Wait);
        match pollster::block_on(receiver) {
            Ok(Ok(())) => {},
            Ok(Err(e)) => return Err(format!("Failed to map buffer: {}", e).into()),
            Err(_) => return Err("Failed to receive buffer mapping result".into()),
        }

        // Read and unpad rows
        let data = buffer_slice.get_mapped_range();
        let mapped: &[u8] = &data;
        let row_stride_src = padded_bytes_per_row as usize;
        let row_stride_dst = unpadded_bytes_per_row as usize;
        let mut pixels: Vec<u8> = vec![0u8; (row_stride_dst as u64 * ph as u64) as usize];
        for row in 0..(ph as usize) {
            let src_start = row * row_stride_src;
            let dst_start = row * row_stride_dst;
            let src_slice = &mapped[src_start..src_start + row_stride_dst];
            pixels[dst_start..dst_start + row_stride_dst].copy_from_slice(src_slice);
        }
        drop(data);
        output_buffer.unmap();

        Ok((pixels, pw, ph))
    }

    /// Get the output texture for direct use in egui
    pub fn get_output_texture(&self) -> (&wgpu::Texture, u32, u32) {
        (&self.output_texture, self.width, self.height)
    }
    
    /// Set the egui texture ID for this renderer
    pub fn set_egui_texture(&mut self, texture_id: egui::TextureId) {
        self.egui_texture = Some(texture_id);
    }
    
    /// Get the egui texture ID
    pub fn get_egui_texture(&self) -> Option<egui::TextureId> {
        self.egui_texture
    }

    /// Update fractal parameters
    pub fn update_parameters(&mut self, params: &FractalParameters) {
        self.engine.parameters = params.clone();
    }

    /// Set active fractal formula
    pub fn set_formula(&mut self, formula_id: &str) -> Result<(), String> {
        let new_formula = match formula_id {
            "mandelbulb" => FractalFormula::Mandelbulb { power: 8.0 },
            "mandelbox" => FractalFormula::Mandelbox { scale: 2.0 },
            "quaternion_julia" => FractalFormula::QuaternionJulia { c: [0.3, 0.5, 0.4, 0.2], max_iterations: self.engine.parameters.max_iterations },
            _ => return Err(format!("Unknown formula: {}", formula_id)),
        };
        self.engine.parameters_mut().formula = new_formula;
        Ok(())
    }

    /// Set camera field of view (degrees) for GPU renderer
    pub fn set_camera_fov(&mut self, fov_degrees: f32) {
        let clamped = fov_degrees.clamp(10.0, 150.0);
        self.camera_fov = clamped;
    }

    /// Set camera target for pseudo‑3D fragment shaders
    pub fn set_camera_target(&mut self, target: [f32; 3]) {
        self.camera_target = target;
    }

    /// Set directional light (normalized internally)
    pub fn set_light_direction(&mut self, dir: [f32; 3]) {
        let mut d = dir;
        let len = (d[0] * d[0] + d[1] * d[1] + d[2] * d[2]).sqrt();
        if len > 1e-6 {
            d[0] /= len;
            d[1] /= len;
            d[2] /= len;
        }
        self.light_direction = d;
    }

    /// Set light color (clamped to [0..1])
    pub fn set_light_color(&mut self, color: [f32; 3]) {
        self.light_color = [
            color[0].clamp(0.0, 1.0),
            color[1].clamp(0.0, 1.0),
            color[2].clamp(0.0, 1.0),
        ];
    }

    /// Set light intensity (non‑negative)
    pub fn set_light_intensity(&mut self, intensity: f32) {
        self.light_intensity = intensity.max(0.0);
    }

    /// Set material metallic factor [0..1]
    pub fn set_material_metallic(&mut self, metallic: f32) {
        self.material_metallic = metallic.clamp(0.0, 1.0);
    }

    /// Set material roughness factor [0.02..1]
    pub fn set_material_roughness(&mut self, roughness: f32) {
        self.material_roughness = roughness.clamp(0.02, 1.0);
    }

    /// Create parameter data for GPU upload
    fn create_parameter_data(&self, time: f32, resolution: (u32, u32)) -> [f32; 256] {
        let mut params = [0.0f32; 256];

        // Basic parameters
        params[0] = self.engine.parameters.max_iterations as f32;
        params[1] = self.engine.parameters.bailout;
        // Extract power from formula if it's Mandelbulb
        params[2] = match &self.engine.parameters.formula {
            FractalFormula::Mandelbulb { power } => *power,
            _ => 2.0,
        };
        params[3] = self.engine.parameters.scale;
        // Position
        params[4] = self.engine.parameters.position.x;
        params[5] = self.engine.parameters.position.y;
        params[6] = self.engine.parameters.position.z;
        // Rotation
        params[7] = self.engine.parameters.rotation.x;
        params[8] = self.engine.parameters.rotation.y;
        params[9] = self.engine.parameters.rotation.z;

        // Color parameters - use first two colors from palette
        if !self.engine.parameters.color_palette.is_empty() {
            let base_color = self.engine.parameters.color_palette[0];
            params[10] = base_color.x;
            params[11] = base_color.y;
            params[12] = base_color.z;
            
            if self.engine.parameters.color_palette.len() > 1 {
                let secondary_color = self.engine.parameters.color_palette[1];
                params[13] = secondary_color.x;
                params[14] = secondary_color.y;
                params[15] = secondary_color.z;
            }
        }
        // Use color_saturation as cycle_frequency equivalent
        params[16] = self.engine.parameters.color_saturation;
        // Use color_saturation for saturation
        params[17] = self.engine.parameters.color_saturation;
        // Use color_saturation for value
        params[18] = self.engine.parameters.color_saturation;

        // Volumetric parameters - use VolumetricParameters if available
        // For now, use default values since the struct doesn't have these fields
        params[19] = 0.1; // density
        params[20] = 0.8; // fog_color.x
        params[21] = 0.9; // fog_color.y
        params[22] = 1.0; // fog_color.z
        params[23] = 0.5; // scattering
        params[24] = 0.1; // absorption
        params[25] = 0.0; // anisotropy

        // Rendering parameters
        params[26] = time;
        params[27] = resolution.0 as f32;
        params[28] = resolution.1 as f32;
        // Use resolution from quality settings
        params[29] = self.engine.quality_settings.resolution[0] as f32;
        // Use max_iterations as max_steps equivalent
        params[30] = self.engine.quality_settings.max_iterations as f32;
        // Use normal_epsilon as surface_epsilon equivalent
        params[31] = self.engine.quality_settings.normal_epsilon;

        // Fractal formula selector mapping
        // 1: Mandelbulb, 2: Mandelbox, 4: Quaternion Julia
        params[32] = match &self.engine.parameters.formula {
            FractalFormula::Mandelbulb { .. } => 1.0,
            FractalFormula::Mandelbox { .. } => 2.0,
            FractalFormula::QuaternionJulia { .. } => 4.0,
            // Fallback for unsupported in-shader formulas
            _ => 0.0,
        };

        // Camera parameters
        params[33] = self.camera_fov;

        // Multi-formula combiner parameters
        // 34: combiner_active
        // 35..37: combiner formula IDs (up to 3)
        // 38: blend mode id (0 union, 1 intersection, 2 subtraction, 3 smooth_union, 4 smooth_intersection, 5 smooth_subtraction)
        // 39: blend factor [0..1]
        // 40: smooth k (used for smooth modes)
        params[34] = if self.combiner_active { 1.0 } else { 0.0 };
        params[35] = self.combiner_formulas[0] as f32;
        params[36] = self.combiner_formulas[1] as f32;
        params[37] = self.combiner_formulas[2] as f32;
        params[39] = self.combiner_blend_factor;
        params[40] = self.combiner_smooth_k;

        // Map BlendMode to id and possibly override k
        params[38] = match self.combiner_blend_mode {
            crate::fractal::types::BlendMode::Union => 0.0,
            crate::fractal::types::BlendMode::Intersection => 1.0,
            crate::fractal::types::BlendMode::Subtraction => 2.0,
            crate::fractal::types::BlendMode::SmoothUnion(k) => {
                // Use provided k as baseline
                params[40] = k;
                3.0
            }
            crate::fractal::types::BlendMode::SmoothIntersection(k) => {
                params[40] = k;
                4.0
            }
            crate::fractal::types::BlendMode::SmoothSubtraction(k) => {
                params[40] = k;
                5.0
            }
        };

        // Lighting parameters (direction xyz, color rgb, intensity)
        params[41] = self.light_direction[0];
        params[42] = self.light_direction[1];
        params[43] = self.light_direction[2];
        params[44] = self.light_color[0];
        params[45] = self.light_color[1];
        params[46] = self.light_color[2];
        params[47] = self.light_intensity;

        // Material parameters
        params[48] = self.material_metallic;
        params[49] = self.material_roughness;

        params
    }

    /// Configure multi-formula combiner
    pub fn set_combiner(
        &mut self,
        active: bool,
        formulas: [i32; 3],
        mode: crate::fractal::types::BlendMode,
        blend_factor: f32,
        smooth_k: f32,
    ) {
        self.combiner_active = active;
        self.combiner_formulas = formulas;
        self.combiner_blend_mode = mode;
        self.combiner_blend_factor = blend_factor;
        self.combiner_smooth_k = smooth_k;
    }
}

/// Lightweight runtime monitoring data for diagnostics HUD
pub struct RendererStats {
    pub frame_count: u64,
    pub last_preview_size: (u32, u32),
    pub last_workgroups: (u32, u32),
    pub last_map_time_ms: f32,
}

impl FractalRenderer {
    /// Return runtime stats useful for GPU usage monitoring
    pub fn stats(&self) -> RendererStats {
        RendererStats {
            frame_count: self.frame_count,
            last_preview_size: self.last_preview_size,
            last_workgroups: self.last_workgroups,
            last_map_time_ms: self.last_map_time_ms,
        }
    }
}

/// CPU-based fallback renderer for systems without GPU compute support
pub struct CPUFractalRenderer {
    pub engine: FractalEngine,
    pub camera_position: Vector3<f32>,
    pub camera_target: Vector3<f32>,
    pub fov: f32,
    pub max_distance: f32,
}

impl CPUFractalRenderer {
    pub fn new() -> Self {
        Self {
            engine: FractalEngine::new(),
            camera_position: Vector3::new(0.0, 0.0, 5.0),
            camera_target: Vector3::new(0.0, 0.0, 0.0),
            fov: 60.0,
            max_distance: 100.0,
        }
    }

    pub fn render(&self, width: u32, height: u32) -> Vec<u8> {
        let mut pixels = vec![0; (width * height * 4) as usize];
        
        for y in 0..height {
            for x in 0..width {
                let uv = (
                    x as f32 / width as f32,
                    y as f32 / height as f32,
                );
                
                // Convert UV to NDC
                let ndc_x = (2.0 * uv.0 - 1.0) * (width as f32 / height as f32);
                let ndc_y = 1.0 - 2.0 * uv.1;
                
                // Generate ray
                let forward = (self.camera_target - self.camera_position).normalize();
                let right = Vector3::new(0.0, 1.0, 0.0).cross(&forward).normalize();
                let up = forward.cross(&right).normalize();
                
                let ray_direction = (forward + right * ndc_x + up * ndc_y).normalize();
                
                // Ray marching
                let mut t = 0.0;
                let mut iterations = 0u32;
                
                for _ in 0..self.engine.quality_settings.max_iterations {
                    let point = self.camera_position + ray_direction * t;
                    let result = self.engine.compute_distance(point);
                    
                    // Use normal_epsilon since quality_settings doesn't have surface_epsilon
                    if result.distance < self.engine.quality_settings.normal_epsilon {
                        // Hit surface - calculate color
                        let color = self.calculate_color(point, &result);
                        // Skip AO calculation since it's not available
                        let idx = ((y * width + x) * 4) as usize;
                        pixels[idx] = (color[0] * 255.0) as u8;
                        pixels[idx + 1] = (color[1] * 255.0) as u8;
                        pixels[idx + 2] = (color[2] * 255.0) as u8;
                        pixels[idx + 3] = 255;
                        break;
                    }
                    
                    // Use distance_threshold since quality_settings doesn't have min_step
                    t += result.distance.abs().max(self.engine.quality_settings.distance_threshold);
                    
                    if t > self.max_distance {
                        // Background color
                        let idx = ((y * width + x) * 4) as usize;
                        pixels[idx] = 10;
                        pixels[idx + 1] = 10;
                        pixels[idx + 2] = 20;
                        pixels[idx + 3] = 255;
                        break;
                    }
                    
                    iterations += 1;
                    if iterations > self.engine.quality_settings.max_iterations {
                        break;
                    }
                }
            }
        }
        
        pixels
    }
    
    /// Calculate surface color
    fn calculate_color(&self, _point: Vector3<f32>, result: &DistanceResult) -> [f32; 3] {
        // Use color_palette and color_saturation instead of color_params
        if self.engine.parameters.color_palette.is_empty() {
            return [0.0, 0.0, 0.0];
        }
        
        let base_color = self.engine.parameters.color_palette[0];
        
        // Simple coloring based on iterations
        let t = (result.iterations as f32) / (self.engine.parameters.max_iterations as f32);
        
        [base_color.x * t, base_color.y * t, base_color.z * t]
    }
}