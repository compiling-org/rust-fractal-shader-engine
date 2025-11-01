//! WGPU-based fractal renderer
//!
//! This module provides GPU-accelerated rendering for fractal visualization
//! using WGPU/WebGPU compute shaders for real-time distance field evaluation.

use super::types::*;
use super::engine::FractalEngine;
use wgpu::{self, util::DeviceExt};
use std::sync::Arc;

/// GPU-accelerated fractal renderer
pub struct FractalRenderer {
    device: wgpu::Device,
    queue: wgpu::Queue,
    compute_pipeline: wgpu::ComputePipeline,
    render_pipeline: wgpu::RenderPipeline,
    fractal_buffer: wgpu::Buffer,
    parameter_buffer: wgpu::Buffer,
    output_texture: wgpu::Texture,
    output_texture_view: wgpu::TextureView,
    bind_group: wgpu::BindGroup,
    engine: FractalEngine,
}

impl FractalRenderer {
    /// Create a new GPU-accelerated fractal renderer
    pub async fn new(width: u32, height: u32) -> Result<Self, Box<dyn std::error::Error>> {
        // Create WGPU instance and adapter
        let instance = wgpu::Instance::new(wgpu::InstanceDescriptor {
            backends: wgpu::Backends::PRIMARY,
            ..Default::default()
        });

        let adapter = instance
            .request_adapter(&wgpu::RequestAdapterOptions {
                power_preference: wgpu::PowerPreference::HighPerformance,
                compatible_surface: None,
                force_fallback_adapter: false,
            })
            .await
            .ok_or("Failed to find suitable GPU adapter")?;

        let (device, queue) = adapter
            .request_device(
                &wgpu::DeviceDescriptor {
                    required_features: wgpu::Features::empty(),
                    required_limits: wgpu::Limits::default(),
                    label: Some("Fractal Renderer Device"),
                },
                None,
            )
            .await?;

        // Create compute shader for fractal evaluation
        let compute_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Fractal Compute Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_compute.wgsl").into()),
        });

        // Create render shader for final output
        let render_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Fractal Render Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("shaders/fractal_render.wgsl").into()),
        });

        // Create compute pipeline
        let compute_pipeline = device.create_compute_pipeline(&wgpu::ComputePipelineDescriptor {
            label: Some("Fractal Compute Pipeline"),
            layout: None,
            module: &compute_shader,
            entry_point: "main",
        });

        // Create render pipeline
        let render_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Fractal Render Pipeline"),
            layout: None,
            vertex: wgpu::VertexState {
                module: &render_shader,
                entry_point: "vs_main",
                buffers: &[],
            },
            fragment: Some(wgpu::FragmentState {
                module: &render_shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
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
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
        });

        // Create buffers
        let fractal_buffer = device.create_buffer(&wgpu::BufferDescriptor {
            label: Some("Fractal Data Buffer"),
            size: (width * height * 4) as u64, // RGBA f32
            usage: wgpu::BufferUsages::STORAGE | wgpu::BufferUsages::COPY_SRC,
            mapped_at_creation: false,
        });

        let parameter_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Fractal Parameters Buffer"),
            contents: bytemuck::cast_slice(&[0.0f32; 64]), // Parameter storage
            usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
        });

        // Create output texture
        let output_texture = device.create_texture(&wgpu::TextureDescriptor {
            label: Some("Fractal Output Texture"),
            size: wgpu::Extent3d {
                width,
                height,
                depth_or_array_layers: 1,
            },
            mip_level_count: 1,
            sample_count: 1,
            dimension: wgpu::TextureDimension::D2,
            format: wgpu::TextureFormat::Rgba8UnormSrgb,
            usage: wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::TEXTURE_BINDING,
            view_formats: &[],
        });

        let output_texture_view = output_texture.create_view(&wgpu::TextureViewDescriptor::default());

        // Create bind group
        let bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
            label: Some("Fractal Bind Group"),
            layout: &compute_pipeline.get_bind_group_layout(0),
            entries: &[
                wgpu::BindGroupEntry {
                    binding: 0,
                    resource: fractal_buffer.as_entire_binding(),
                },
                wgpu::BindGroupEntry {
                    binding: 1,
                    resource: parameter_buffer.as_entire_binding(),
                },
                wgpu::BindGroupEntry {
                    binding: 2,
                    resource: wgpu::BindingResource::TextureView(&output_texture_view),
                },
            ],
        });

        Ok(Self {
            device,
            queue,
            compute_pipeline,
            render_pipeline,
            fractal_buffer,
            parameter_buffer,
            output_texture,
            output_texture_view,
            bind_group,
            engine: FractalEngine::new(),
        })
    }

    /// Render a frame with current fractal parameters
    pub fn render_frame(&mut self, time: f32, resolution: (u32, u32)) -> Result<(), Box<dyn std::error::Error>> {
        // Update parameters
        let params = self.create_parameter_data(time, resolution);
        self.queue.write_buffer(&self.parameter_buffer, 0, bytemuck::cast_slice(&params));

        // Create command encoder
        let mut encoder = self.device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("Fractal Render Encoder"),
        });

        // Compute pass
        {
            let mut compute_pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor {
                label: Some("Fractal Compute Pass"),
            });

            compute_pass.set_pipeline(&self.compute_pipeline);
            compute_pass.set_bind_group(0, &self.bind_group, &[]);

            let workgroups_x = (resolution.0 + 15) / 16; // 16x16 workgroups
            let workgroups_y = (resolution.1 + 15) / 16;

            compute_pass.dispatch_workgroups(workgroups_x, workgroups_y, 1);
        }

        // Submit commands
        self.queue.submit(std::iter::once(encoder.finish()));

        Ok(())
    }

    /// Get the output texture for display
    pub fn get_output_texture(&self) -> &wgpu::TextureView {
        &self.output_texture_view
    }

    /// Update fractal parameters
    pub fn update_parameters(&mut self, params: &FractalParams) {
        self.engine.parameters = params.clone();
    }

    /// Set active fractal formula
    pub fn set_formula(&mut self, formula_id: &str) -> Result<(), String> {
        self.engine.set_formula(formula_id)
    }

    /// Create parameter data for GPU upload
    fn create_parameter_data(&self, time: f32, resolution: (u32, u32)) -> [f32; 64] {
        let mut params = [0.0f32; 64];

        // Basic parameters
        params[0] = self.engine.parameters.max_iterations as f32;
        params[1] = self.engine.parameters.bailout;
        params[2] = self.engine.parameters.power;
        params[3] = self.engine.parameters.scale;
        params[4..7].copy_from_slice(&self.engine.parameters.offset);
        params[7..10].copy_from_slice(&self.engine.parameters.rotation);

        // Color parameters
        params[10..13].copy_from_slice(&self.engine.parameters.color_params.base_color);
        params[13..16].copy_from_slice(&self.engine.parameters.color_params.secondary_color);
        params[16] = self.engine.parameters.color_params.cycle_frequency;
        params[17] = self.engine.parameters.color_params.saturation;
        params[18] = self.engine.parameters.color_params.value;

        // Volumetric parameters
        params[19] = self.engine.parameters.volumetric_params.density;
        params[20..23].copy_from_slice(&self.engine.parameters.volumetric_params.fog_color);
        params[23] = self.engine.parameters.volumetric_params.scattering;
        params[24] = self.engine.parameters.volumetric_params.absorption;
        params[25] = self.engine.parameters.volumetric_params.anisotropy;

        // Rendering parameters
        params[26] = time;
        params[27] = resolution.0 as f32;
        params[28] = resolution.1 as f32;
        params[29] = self.engine.quality.resolution_scale;
        params[30] = self.engine.quality.max_steps as f32;
        params[31] = self.engine.quality.surface_epsilon;

        params
    }

    /// Get access to the underlying fractal engine
    pub fn engine(&self) -> &FractalEngine {
        &self.engine
    }

    /// Get mutable access to the fractal engine
    pub fn engine_mut(&mut self) -> &mut FractalEngine {
        &mut self.engine
    }
}

/// CPU-based fallback renderer for systems without GPU compute support
pub struct CPUFractalRenderer {
    pub engine: FractalEngine,
    pub camera_position: Point3D,
    pub camera_target: Point3D,
    pub fov: f32,
    pub max_distance: f32,
}

impl CPUFractalRenderer {
    pub fn new() -> Self {
        Self {
            engine: FractalEngine::new(),
            camera_position: Point3D::new(0.0, 0.0, 5.0),
            camera_target: Point3D::zero(),
            fov: 60.0,
            max_distance: 100.0,
        }
    }

    /// Render a frame using CPU ray marching
    pub fn render_frame(&self, width: u32, height: u32) -> Vec<u8> {
        let mut pixels = Vec::with_capacity((width * height * 4) as usize);

        for y in 0..height {
            for x in 0..width {
                let color = self.render_pixel(x as f32, y as f32, width as f32, height as f32);
                pixels.push((color[0] * 255.0) as u8);
                pixels.push((color[1] * 255.0) as u8);
                pixels.push((color[2] * 255.0) as u8);
                pixels.push((color[3] * 255.0) as u8);
            }
        }

        pixels
    }

    /// Render a single pixel using CPU ray marching
    fn render_pixel(&self, x: f32, y: f32, width: f32, height: f32) -> [f32; 4] {
        // Convert screen coordinates to camera ray
        let aspect = width / height;
        let tan_fov = (self.fov * 0.5).to_radians().tan();

        let ndc_x = (2.0 * x / width - 1.0) * aspect * tan_fov;
        let ndc_y = (1.0 - 2.0 * y / height) * tan_fov;

        // Camera basis vectors
        let forward = (self.camera_target - self.camera_position).normalize();
        let right = Point3D::new(0.0, 1.0, 0.0).cross(&forward).normalize();
        let up = forward.cross(&right).normalize();

        let ray_direction = (forward + right * ndc_x + up * ndc_y).normalize();

        // Ray marching
        let mut t = 0.0;
        let mut iterations = 0u32;

        for _ in 0..self.engine.quality.max_steps {
            let point = self.camera_position + ray_direction * t;
            let result = self.engine.evaluate_distance(point);

            if result.distance < self.engine.quality.surface_epsilon {
                // Hit surface - calculate color
                let color = self.calculate_color(point, &result);
                let ao = self.engine.calculate_ao(point, result.normal, 4);
                return [color[0] * ao, color[1] * ao, color[2] * ao, 1.0];
            }

            t += result.distance.abs().max(self.engine.quality.min_step);

            if t > self.max_distance {
                break;
            }

            iterations += 1;
        }

        // Background
        [0.1, 0.1, 0.2, 1.0]
    }

    /// Calculate surface color
    fn calculate_color(&self, point: Point3D, result: &DistanceResult) -> [f32; 3] {
        let color_params = &self.engine.parameters.color_params;

        // Iteration-based coloring
        let t = (result.iterations as f32) / (self.engine.parameters.max_iterations as f32);
        let color_mix = t * color_params.cycle_frequency;

        let r = (color_params.base_color[0] * (1.0 - t) + color_params.secondary_color[0] * t)
            * (color_mix.sin() * 0.5 + 0.5);
        let g = (color_params.base_color[1] * (1.0 - t) + color_params.secondary_color[1] * t)
            * ((color_mix + 2.094).sin() * 0.5 + 0.5);
        let b = (color_params.base_color[2] * (1.0 - t) + color_params.secondary_color[2] * t)
            * ((color_mix + 4.188).sin() * 0.5 + 0.5);

        [r * color_params.saturation, g * color_params.saturation, b * color_params.value]
    }
}