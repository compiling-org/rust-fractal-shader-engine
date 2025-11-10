use std::path::PathBuf;
use std::time::Instant;
use std::fs::File;
use std::io::Write;

use image::codecs::png::PngEncoder;
use image::{ImageEncoder, ExtendedColorType};
use nalgebra::Vector3;
use pollster::block_on;
use serde::{Deserialize, Serialize};
// use wgpu::util::DeviceExt; // not needed here

use fractal_generator_lib::fractal::types::{FractalFormula, QualitySettings};
use fractal_generator_lib::fractal::engine::FractalEngine;

#[derive(Debug, Clone, Serialize, Deserialize)]
struct SnapshotArgs {
    width: u32,
    height: u32,
    formula: Option<String>,
    max_iterations: Option<u32>,
    bailout: Option<f32>,
    power: Option<f32>,
    scale: Option<f32>,
    position: Option<[f32; 3]>,
    rotation: Option<[f32; 3]>,
    color_saturation: Option<f32>,
    output: Option<PathBuf>,
    state_out: Option<PathBuf>,
}

impl Default for SnapshotArgs {
    fn default() -> Self {
        Self {
            width: 1024,
            height: 768,
            formula: Some("mandelbulb".to_string()),
            max_iterations: Some(512),
            bailout: Some(8.0),
            power: Some(8.0),
            scale: Some(1.0),
            // Slightly offset and rotate camera target for more interesting view
            position: Some([0.2, -0.1, -1.2]),
            rotation: Some([0.3, 0.2, 0.0]),
            color_saturation: Some(1.1),
            output: Some(PathBuf::from("snapshot.png")),
            state_out: None,
        }
    }
}

#[derive(Debug, Clone, Serialize)]
struct RenderState {
    width: u32,
    height: u32,
    formula: String,
    device_name: String,
    backend: String,
    limits: String,
    elapsed_ms: f64,
}

fn build_params(engine: &FractalEngine, time: f32, resolution: (u32, u32)) -> [f32; 256] {
    let mut params = [0.0f32; 256];

    // Basic parameters
    params[0] = engine.parameters.max_iterations as f32;
    params[1] = engine.parameters.bailout;
    params[2] = match &engine.parameters.formula {
        FractalFormula::Mandelbulb { power } => *power,
        _ => 2.0,
    };
    // Scale: use formula-specific scale for Mandelbox; otherwise engine scale
    params[3] = match &engine.parameters.formula {
        FractalFormula::Mandelbox { scale } => *scale,
        _ => engine.parameters.scale,
    };
    // Position
    params[4] = engine.parameters.position.x;
    params[5] = engine.parameters.position.y;
    params[6] = engine.parameters.position.z;
    // Rotation
    params[7] = engine.parameters.rotation.x;
    params[8] = engine.parameters.rotation.y;
    params[9] = engine.parameters.rotation.z;

    // Color params (use first two colors of palette)
    if !engine.parameters.color_palette.is_empty() {
        let base = engine.parameters.color_palette[0];
        params[10] = base.x;
        params[11] = base.y;
        params[12] = base.z;
        if engine.parameters.color_palette.len() > 1 {
            let sec = engine.parameters.color_palette[1];
            params[13] = sec.x;
            params[14] = sec.y;
            params[15] = sec.z;
        }
    }
    params[16] = engine.parameters.color_saturation; // cycle_frequency
    params[17] = engine.parameters.color_saturation; // saturation
    params[18] = engine.parameters.color_saturation; // value

    // Volumetric placeholders
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
    params[29] = engine.quality_settings.resolution[0] as f32; // resolution scale placeholder
    params[30] = engine.quality_settings.max_iterations as f32; // max_steps
    params[31] = engine.quality_settings.normal_epsilon; // surface_epsilon

    // Formula selector
    params[32] = match &engine.parameters.formula {
        FractalFormula::Mandelbrot { .. } => 0.0,
        FractalFormula::Mandelbulb { .. } => 1.0,
        FractalFormula::Mandelbox { .. } => 2.0,
        FractalFormula::Julia { .. } => 3.0,
        FractalFormula::QuaternionJulia { .. } => 4.0,
        _ => 0.0,
    };

    params
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Read args as JSON from STDIN or fallback to defaults
    let args: SnapshotArgs = match std::env::args().nth(1) {
        Some(arg1) => {
            if arg1 == "--help" || arg1 == "-h" {
                println!(
                    "{}",
                    "Usage: fractal-snapshot [json-args]\n\nExample:\n  fractal-snapshot '{\"width\":1024,\"height\":768,\"formula\":\"mandelbulb\",\"output\":\"snapshot.png\"}'"
                );
                return Ok(());
            }
            serde_json::from_str(&arg1).unwrap_or_default()
        }
        None => SnapshotArgs::default(),
    };

    let mut engine = FractalEngine::new();
    // Apply args to engine params
    engine.parameters.max_iterations = args.max_iterations.unwrap_or(engine.parameters.max_iterations);
    engine.parameters.bailout = args.bailout.unwrap_or(engine.parameters.bailout);
    engine.parameters.scale = args.scale.unwrap_or(engine.parameters.scale);
    engine.parameters.color_saturation = args.color_saturation.unwrap_or(engine.parameters.color_saturation);
    if let Some(pos) = args.position { engine.parameters.position = Vector3::new(pos[0], pos[1], pos[2]); }
    if let Some(rot) = args.rotation { engine.parameters.rotation = Vector3::new(rot[0], rot[1], rot[2]); }
    if let Some(formula) = args.formula.clone() {
        engine.parameters.formula = match formula.as_str() {
            "mandelbrot" => FractalFormula::Mandelbrot { center: [-0.5, 0.0], zoom: 1.0 },
            "mandelbulb" => FractalFormula::Mandelbulb { power: args.power.unwrap_or(8.0) },
            "mandelbox" => FractalFormula::Mandelbox { scale: args.scale.unwrap_or(2.0) },
            "julia" => FractalFormula::Julia { c: [-0.7, 0.27015], max_iterations: engine.parameters.max_iterations },
            "quaternion_julia" => FractalFormula::QuaternionJulia { c: [-0.2, 0.8, 0.0, 0.0], max_iterations: engine.parameters.max_iterations },
            _ => engine.parameters.formula.clone(),
        };
    }

    // Ensure quality max_iterations aligns with compute steps
    // Increase ray-march steps and tighten epsilon for sharper 3D results
    let mut qs = engine.quality_settings.clone();
    qs.max_iterations = engine.parameters.max_iterations.max(512);
    qs.normal_epsilon = 0.0005;
    engine.set_quality_settings(qs);

    let width = args.width.max(1);
    let height = args.height.max(1);

    // Initialize WGPU headless
    let instance = wgpu::Instance::default();
    let adapter = block_on(instance.request_adapter(&wgpu::RequestAdapterOptions {
        power_preference: wgpu::PowerPreference::HighPerformance,
        force_fallback_adapter: false,
        compatible_surface: None,
    }))?;

    let (device, queue) = block_on(adapter.request_device(&wgpu::DeviceDescriptor {
        label: Some("Fractal Snapshot Device"),
        required_features: wgpu::Features::empty(),
        required_limits: wgpu::Limits::default(),
        memory_hints: wgpu::MemoryHints::default(),
        trace: wgpu::Trace::default(),
    }))?;

    // Create compute shader
    let compute_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
        label: Some("Fractal Compute Shader"),
        source: wgpu::ShaderSource::Wgsl(include_str!("../fractal/shaders/fractal_compute.wgsl").into()),
    });

    // Bind group layout
    let bind_group_layout = device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
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
    });

    // Pipeline
    let pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
        label: Some("Fractal Compute Pipeline Layout"),
        bind_group_layouts: &[&bind_group_layout],
        push_constant_ranges: &[],
    });

    let compute_pipeline = device.create_compute_pipeline(&wgpu::ComputePipelineDescriptor {
        label: Some("Fractal Compute Pipeline"),
        layout: Some(&pipeline_layout),
        module: &compute_shader,
        entry_point: Some("main"),
        compilation_options: wgpu::PipelineCompilationOptions::default(),
        cache: None,
    });

    // Buffers and texture
    let distance_buffer_size = (width * height * std::mem::size_of::<f32>() as u32) as u64;
    let distance_buffer = device.create_buffer(&wgpu::BufferDescriptor {
        label: Some("Distance Field Buffer"),
        size: distance_buffer_size,
        usage: wgpu::BufferUsages::STORAGE | wgpu::BufferUsages::COPY_SRC | wgpu::BufferUsages::COPY_DST,
        mapped_at_creation: false,
    });

    let param_buffer = device.create_buffer(&wgpu::BufferDescriptor {
        label: Some("Params Buffer"),
        size: 256 * std::mem::size_of::<f32>() as u64,
        usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
        mapped_at_creation: false,
    });

    let output_texture = device.create_texture(&wgpu::TextureDescriptor {
        label: Some("Output Texture"),
        size: wgpu::Extent3d { width, height, depth_or_array_layers: 1 },
        mip_level_count: 1,
        sample_count: 1,
        dimension: wgpu::TextureDimension::D2,
        format: wgpu::TextureFormat::Rgba8Unorm,
        usage: wgpu::TextureUsages::STORAGE_BINDING | wgpu::TextureUsages::COPY_SRC,
        view_formats: &[],
    });
    let output_view = output_texture.create_view(&wgpu::TextureViewDescriptor::default());

    let bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
        label: Some("Fractal Compute Bind Group"),
        layout: &bind_group_layout,
        entries: &[
            wgpu::BindGroupEntry { binding: 0, resource: distance_buffer.as_entire_binding() },
            wgpu::BindGroupEntry { binding: 1, resource: param_buffer.as_entire_binding() },
            wgpu::BindGroupEntry { binding: 2, resource: wgpu::BindingResource::TextureView(&output_view) },
        ],
    });

    // Upload params
    let params = build_params(&engine, 0.0, (width, height));
    queue.write_buffer(&param_buffer, 0, bytemuck::cast_slice(&params));

    // Dispatch
    let start = Instant::now();
    let mut encoder = device.create_command_encoder(&wgpu::CommandEncoderDescriptor { label: Some("Fractal Snapshot Encoder") });
    {
        let mut pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor { label: Some("Compute Pass"), timestamp_writes: None });
        pass.set_pipeline(&compute_pipeline);
        pass.set_bind_group(0, &bind_group, &[]);
        let gx = (width + 15) / 16;
        let gy = (height + 15) / 16;
        pass.dispatch_workgroups(gx, gy, 1);
    }
    queue.submit(Some(encoder.finish()));

    // Read back texture
    let bytes_per_pixel = 4u32;
    let unpadded_bytes_per_row = width * bytes_per_pixel;
    let align = wgpu::COPY_BYTES_PER_ROW_ALIGNMENT; // 256
    let padded_bytes_per_row = ((unpadded_bytes_per_row + align - 1) / align) * align;
    let readback_size: u64 = padded_bytes_per_row as u64 * height as u64;

    let read_buffer = device.create_buffer(&wgpu::BufferDescriptor {
        label: Some("Readback Buffer"),
        size: readback_size,
        usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
        mapped_at_creation: false,
    });

    let mut encoder = device.create_command_encoder(&wgpu::CommandEncoderDescriptor { label: Some("Copy Encoder") });
    encoder.copy_texture_to_buffer(
        wgpu::TexelCopyTextureInfo { texture: &output_texture, mip_level: 0, origin: wgpu::Origin3d::ZERO, aspect: wgpu::TextureAspect::All },
        wgpu::TexelCopyBufferInfo {
            buffer: &read_buffer,
            layout: wgpu::TexelCopyBufferLayout { offset: 0, bytes_per_row: Some(padded_bytes_per_row), rows_per_image: Some(height) },
        },
        wgpu::Extent3d { width, height, depth_or_array_layers: 1 },
    );
    queue.submit(Some(encoder.finish()));

    // Map and extract
    {
        let slice = read_buffer.slice(..);
        let (tx, rx) = futures::channel::oneshot::channel();
        slice.map_async(wgpu::MapMode::Read, move |res| { let _ = tx.send(res); });
        let _ = device.poll(wgpu::PollType::Wait);
        // Block on the async mapping completion without making main async
        block_on(rx)??;
        let data = slice.get_mapped_range();
        let mapped: &[u8] = &data;

        let row_src = padded_bytes_per_row as usize;
        let row_dst = unpadded_bytes_per_row as usize;
        let mut pixels: Vec<u8> = vec![0u8; (row_dst as u64 * height as u64) as usize];
        for row in 0..(height as usize) {
            let src_start = row * row_src;
            let dst_start = row * row_dst;
            let src_slice = &mapped[src_start..src_start + row_dst];
            pixels[dst_start..dst_start + row_dst].copy_from_slice(src_slice);
        }
        drop(data);
        read_buffer.unmap();

        // Write PNG
        let out_path = args.output.unwrap_or(PathBuf::from("snapshot.png"));
        let mut out = File::create(&out_path)?;
        let encoder = PngEncoder::new(&mut out);
        encoder.write_image(&pixels, width, height, ExtendedColorType::Rgba8)?;

        // Write optional state json
        if let Some(state_path) = args.state_out.clone() {
            let info = adapter.get_info();
            let elapsed = start.elapsed().as_secs_f64() * 1000.0;
            let state = RenderState {
                width,
                height,
                formula: match &engine.parameters.formula {
                    FractalFormula::Mandelbrot { .. } => "mandelbrot".to_string(),
                    FractalFormula::Mandelbulb { .. } => "mandelbulb".to_string(),
                    FractalFormula::Mandelbox { .. } => "mandelbox".to_string(),
                    FractalFormula::Julia { .. } => "julia".to_string(),
                    FractalFormula::QuaternionJulia { .. } => "quaternion_julia".to_string(),
                    _ => "other".to_string(),
                },
                device_name: info.name.clone(),
                backend: format!("{:?}", info.backend),
                limits: format!("{:?}", device.limits()),
                elapsed_ms: elapsed,
            };
            let mut f = File::create(state_path)?;
            f.write_all(serde_json::to_string_pretty(&state)?.as_bytes())?;
        }

        println!("Wrote {}x{} snapshot to {}", width, height, out_path.display());
    }

    Ok(())
}