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
    distance_field_buffer: wgpu::Buffer,
    parameter_buffer: wgpu::Buffer,
    output_texture: wgpu::Texture,
    output_texture_view: wgpu::TextureView,
    sampler: wgpu::Sampler,
    post_process_buffer: wgpu::Buffer,
    engine: FractalEngine,
    width: u32,
    height: u32,
    // For egui texture integration
    egui_texture: Option<egui::TextureId>,
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
                usage: wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::TEXTURE_BINDING,
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

        let engine = FractalEngine::new();

        Ok(Self {
            device,
            queue,
            compute_pipeline,
            render_pipeline,
            distance_field_buffer,
            parameter_buffer,
            output_texture,
            output_texture_view,
            sampler,
            post_process_buffer,
            engine,
            width: actual_width,
            height: actual_height,
            egui_texture: None,
        })
    }

    /// Render a single frame - just submits the compute pass, doesn't read back data
    pub fn render_frame(&mut self, time: f32, resolution: (u32, u32)) -> Result<(), Box<dyn std::error::Error>> {
        log::debug!("Starting render frame with time: {}, resolution: {:?}", time, resolution);
        
        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;
        
        // Create command encoder with error handling
        let mut encoder = match wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder"),
        }) {
            encoder => encoder,
            _ => return Err("Failed to create command encoder".into()),
        };

        // Update parameters
        let params = self.create_parameter_data(time, resolution);
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass
        {
            log::debug!("Starting compute pass");
            let bind_group = match wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
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
            }) {
                bind_group => bind_group,
                _ => return Err("Failed to create bind group".into()),
            };

            let mut compute_pass = match encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass"),
                timestamp_writes: None,
            }) {
                compute_pass => compute_pass,
                _ => return Err("Failed to begin compute pass".into()),
            };

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
    
    /// Render a single frame and update the egui texture
    pub fn render_frame_to_texture(&mut self, time: f32, resolution: (u32, u32), ctx: &egui::Context) -> Result<egui::TextureId, Box<dyn std::error::Error>> {
        log::debug!("Starting render frame with time: {}, resolution: {:?}", time, resolution);
        
        let wgpu_device = self.device.wgpu_device();
        let wgpu_queue = &self.queue.0;
        
        // Create command encoder with error handling
        let mut encoder = match wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder"),
        }) {
            encoder => encoder,
            _ => return Err("Failed to create command encoder".into()),
        };

        // Update parameters
        let params = self.create_parameter_data(time, resolution);
        wgpu_queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Compute pass
        {
            log::debug!("Starting compute pass");
            let bind_group = match wgpu_device.create_bind_group(&wgpu::BindGroupDescriptor {
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
            }) {
                bind_group => bind_group,
                _ => return Err("Failed to create bind group".into()),
            };

            let mut compute_pass = match encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass"),
                timestamp_writes: None,
            }) {
                compute_pass => compute_pass,
                _ => return Err("Failed to begin compute pass".into()),
            };

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
        
        // Create a buffer to read the texture data back to CPU
        let output_buffer = match wgpu_device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Fractal Output Readback Buffer"),
            size: (self.width * self.height * 4) as u64, // RGBA8 = 4 bytes per pixel
            usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
            mapped_at_creation: false,
        }) {
            buffer => buffer,
            _ => return Err("Failed to create output buffer".into()),
        };

        // Create a new encoder for the copy operation
        let mut encoder = match wgpu_device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Copy Encoder"),
        }) {
            encoder => encoder,
            _ => return Err("Failed to create copy encoder".into()),
        };

        // Copy texture to buffer - using a direct call with inline structs
        encoder.copy_texture_to_buffer(
            wgpu::TexelCopyTextureInfo {
                texture: &self.output_texture,
                mip_level: 0,
                origin: wgpu::Origin3d::ZERO,
                aspect: wgpu::TextureAspect::All,
            },
            wgpu::TexelCopyBufferInfo {
                buffer: &output_buffer,
                layout: wgpu::TexelCopyBufferLayout {
                    offset: 0,
                    bytes_per_row: Some(self.width * 4),
                    rows_per_image: Some(self.height),
                },
            },
            wgpu::Extent3d {
                width: self.width,
                height: self.height,
                depth_or_array_layers: 1,
            },
        );

        // Submit the copy command
        wgpu_queue.submit(Some(encoder.finish()));

        // Map the buffer and read the data with proper error handling
        let buffer_slice = output_buffer.slice(..);
        let (sender, receiver) = futures::channel::oneshot::channel();
        buffer_slice.map_async(wgpu::MapMode::Read, move |result| {
            let _ = sender.send(result);
        });

        // Wait for the buffer to be mapped
        wgpu_device.poll(wgpu::PollType::Wait);
        
        // Handle the result properly
        let result = match pollster::block_on(receiver) {
            Ok(result) => result,
            Err(_) => return Err("Failed to receive buffer mapping result".into()),
        };
        
        match result {
            Ok(()) => {},
            Err(e) => return Err(format!("Failed to map buffer: {}", e).into()),
        }

        // Get the data from the buffer
        let data = buffer_slice.get_mapped_range();
        let pixels: Vec<u8> = data.to_vec();
        drop(data);
        output_buffer.unmap();

        // Convert the raw pixel data to egui::Color32
        let width = resolution.0 as usize;
        let height = resolution.1 as usize;
        
        // Validate pixel data size
        if pixels.len() != width * height * 4 {
            return Err(format!("Invalid pixel data size: expected {}, got {}", width * height * 4, pixels.len()).into());
        }
        
        let mut color_pixels = Vec::with_capacity(width * height);
        
        for chunk in pixels.chunks_exact(4) {
            color_pixels.push(egui::Color32::from_rgba_premultiplied(
                chunk[0], chunk[1], chunk[2], chunk[3]
            ));
        }
        
        let color_image = egui::ColorImage::new([width, height], color_pixels);
        
        // Return the texture ID for egui
        if let Some(texture_id) = self.egui_texture {
            // Update the existing texture
            let image_delta = egui::epaint::ImageDelta::full(color_image, egui::TextureOptions::default());
            ctx.tex_manager().write().set(texture_id, image_delta);
            Ok(texture_id)
        } else {
            // Create a new texture ID
            let texture_id = ctx.tex_manager().write().alloc(
                "fractal_viewport".to_string(),
                egui::ImageData::Color(std::sync::Arc::new(color_image)),
                egui::TextureOptions::default(),
            );
            self.egui_texture = Some(texture_id);
            Ok(texture_id)
        }
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
            "mandelbrot" => FractalFormula::Mandelbrot { center: [-0.5, 0.0], zoom: 1.0 },
            "mandelbulb" => FractalFormula::Mandelbulb { power: 8.0 },
            "mandelbox" => FractalFormula::Mandelbox { scale: 2.0 },
            "julia" => FractalFormula::Julia { c: [-0.7, 0.27015], max_iterations: 100 },
            _ => return Err(format!("Unknown formula: {}", formula_id)),
        };
        self.engine.parameters_mut().formula = new_formula;
        Ok(())
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

        params
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
    fn calculate_color(&self, point: Vector3<f32>, result: &DistanceResult) -> [f32; 3] {
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