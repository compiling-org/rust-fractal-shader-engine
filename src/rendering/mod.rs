//! GPU Rendering Module
//!
//! This module provides GPU-accelerated rendering capabilities for fractal visualization,
//! including compute shaders for distance estimation and real-time rendering pipelines.

use wgpu::{self, util::DeviceExt};
use nalgebra::Vector3;

/// GPU rendering context for fractal visualization
pub struct GPURenderer {
    device: wgpu::Device,
    queue: wgpu::Queue,
    surface: wgpu::Surface<'static>,
    surface_config: wgpu::SurfaceConfiguration,
    fractal_pipeline: wgpu::ComputePipeline,
    render_pipeline: wgpu::RenderPipeline,
    fractal_bind_group: wgpu::BindGroup,
    render_bind_group: wgpu::BindGroup,
    distance_buffer: wgpu::Buffer,
    color_buffer: wgpu::Buffer,
    uniform_buffer: wgpu::Buffer,
    output_texture: wgpu::Texture,
    output_texture_view: wgpu::TextureView,
}

impl GPURenderer {
    /// Create a new GPU renderer with the given surface
    pub async fn new(surface: wgpu::Surface<'static>) -> Result<Self, Box<dyn std::error::Error>> {
        // Create adapter and device
        let adapter = surface.get_adapter().await.unwrap();
        let (device, queue) = adapter.request_device(
            &wgpu::DeviceDescriptor {
                label: Some("Fractal GPU Device"),
                required_features: wgpu::Features::empty(),
                required_limits: wgpu::Limits::default(),
            },
            None,
        ).await?;

        // Configure surface
        let surface_caps = surface.get_capabilities(&adapter);
        let surface_format = surface_caps.formats[0];
        let size = wgpu::Extent3d {
            width: 1920,
            height: 1080,
            depth_or_array_layers: 1,
        };

        let surface_config = wgpu::SurfaceConfiguration {
            usage: wgpu::TextureUsages::RENDER_ATTACHMENT,
            format: surface_format,
            width: size.width,
            height: size.height,
            present_mode: wgpu::PresentMode::Fifo,
            alpha_mode: wgpu::CompositeAlphaMode::Auto,
            view_formats: vec![],
        };
        surface.configure(&device, &surface_config);

        // Create compute shader for fractal distance estimation
        let fractal_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Fractal Compute Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_compute.wgsl")),
        });

        // Create render shader
        let render_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Fractal Render Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_render.wgsl")),
        });

        // Create compute pipeline
        let fractal_pipeline = device.create_compute_pipeline(&wgpu::ComputePipelineDescriptor {
            label: Some("Fractal Compute Pipeline"),
            layout: None,
            module: &fractal_shader,
            entry_point: "main",
        });

        // Create render pipeline
        let render_pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
            label: Some("Render Pipeline Layout"),
            bind_group_layouts: &[],
            push_constant_ranges: &[],
        });

        let render_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Fractal Render Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &render_shader,
                entry_point: "vs_main",
                buffers: &[],
            },
            fragment: Some(wgpu::FragmentState {
                module: &render_shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format: surface_format,
                    blend: Some(wgpu::BlendState::REPLACE),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                strip_index_format: None,
                front_face: wgpu::FrontFace::Ccw,
                cull_mode: Some(wgpu::Face::Back),
                polygon_mode: wgpu::PolygonMode::Fill,
                unclipped_depth: false,
                conservative: false,
            },
            depth_stencil: None,
            multisample: wgpu::MultisampleState {
                count: 1,
                mask: !0,
                alpha_to_coverage_enabled: false,
            },
            multiview: None,
        });

        // Create buffers
        let distance_buffer = device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Distance Buffer"),
            size: (size.width * size.height * std::mem::size_of::<f32>() as u32) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::STORAGE | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        });

        let color_buffer = device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Color Buffer"),
            size: (size.width * size.height * std::mem::size_of::<[f32; 4]>() as u32) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::STORAGE | wgpu::BufferUsages::COPY_SRC,
            mapped_at_creation: false,
        });

        let uniform_buffer = device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Uniform Buffer"),
            size: std::mem::size_of::<FractalUniforms>() as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        });

        // Create output texture
        let output_texture = device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Output Texture"),
            size,
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8Unorm,
            usage: wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::TEXTURE_BINDING,
            view_formats: &[],
        });
        let output_texture_view = output_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Create bind group layouts first
        let fractal_bind_group_layout = fractal_pipeline.get_bind_group_layout(0);
        let render_bind_group_layout = render_pipeline.get_bind_group_layout(0);

        // Create bind groups
        let fractal_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
            label: Some("Fractal Bind Group"),
            layout: &fractal_bind_group_layout,
            entries: &[
                wgpu::BindGroupEntry {
                    binding: 0,
                    resource: uniform_buffer.as_entire_binding(),
                },
                wgpu::BindGroupEntry {
                    binding: 1,
                    resource: wgpu::BindingResource::Buffer(wgpu::BufferBinding {
                        buffer: &distance_buffer,
                        offset: 0,
                        size: None,
                    }),
                },
            ],
        });

        let render_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
            label: Some("Render Bind Group"),
            layout: &render_bind_group_layout,
            entries: &[
                wgpu::BindGroupEntry {
                    binding: 0,
                    resource: wgpu::BindingResource::TextureView(&output_texture_view),
                },
                wgpu::BindGroupEntry {
                    binding: 1,
                    resource: uniform_buffer.as_entire_binding(),
                },
            ],
        });

        Ok(Self {
            device,
            queue,
            surface,
            surface_config,
            fractal_pipeline,
            render_pipeline,
            fractal_bind_group,
            render_bind_group,
            distance_buffer,
            color_buffer,
            uniform_buffer,
            output_texture,
            output_texture_view,
        })
    }

    /// Update fractal parameters
    pub fn update_uniforms(&mut self, uniforms: &FractalUniforms) {
        self.queue.write_buffer(&self.uniform_buffer, 0, bytemuck::cast_slice(&[*uniforms]));
    }

    /// Render a frame
    pub fn render(&mut self) -> Result<(), wgpu::SurfaceError> {
        let output = self.surface.get_current_texture()?;
        let view = output.texture.create_view(&wgpu::TextureViewDescriptor::default());

        let mut encoder = self.device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Render Encoder"),
        });

        // Compute pass for fractal distance estimation
        {
            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass"),
            });
            compute_pass.set_pipeline(&self.fractal_pipeline);
            compute_pass.set_bind_group(0, &self.fractal_bind_group, &[]);
            compute_pass.dispatch_workgroups(self.surface_config.width / 8, self.surface_config.height / 8, 1);
        }

        // Render pass
        {
            let mut render_pass = encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                label: Some("Fractal Render Pass"),
                color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                    view: &view,
                    resolve_target: None,
                    ops: wgpu::Operations {
                        load: wgpu::LoadOp::Clear(wgpu::Color::BLACK),
                        store: wgpu::StoreOp::Store,
                    },
                })],
                depth_stencil_attachment: None,
                occlusion_query_set: None,
                timestamp_writes: None,
            });

            render_pass.set_pipeline(&self.render_pipeline);
            render_pass.set_bind_group(0, &self.render_bind_group, &[]);
            render_pass.draw(0..6, 0..1);
        }

        self.queue.submit(std::iter::once(encoder.finish()));
        output.present();

        Ok(())
    }

    /// Resize the surface
    pub fn resize(&mut self, new_size: wgpu::Extent3d) {
        self.surface_config.width = new_size.width;
        self.surface_config.height = new_size.height;
        self.surface.configure(&self.device, &self.surface_config);
    }
}

/// Uniform data for fractal computation
#[repr(C)]
#[derive(Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
pub struct FractalUniforms {
    pub resolution: [f32; 2],
    pub time: f32,
    pub zoom: f32,
    pub offset: [f32; 2],
    pub power: f32,
    pub bailout: f32,
    pub julia_c: [f32; 2],
    pub fractal_type: u32,
    pub max_iterations: u32,
    pub color_cycle: f32,
    pub brightness: f32,
    pub contrast: f32,
    pub saturation: f32,
}

impl Default for FractalUniforms {
    fn default() -> Self {
        Self {
            resolution: [1920.0, 1080.0],
            time: 0.0,
            zoom: 1.0,
            offset: [0.0, 0.0],
            power: 2.0,
            bailout: 4.0,
            julia_c: [-0.7, 0.27015],
            fractal_type: 0, // Mandelbrot
            max_iterations: 100,
            color_cycle: 1.0,
            brightness: 1.0,
            contrast: 1.0,
            saturation: 1.0,
        }
    }
}