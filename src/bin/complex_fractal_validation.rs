use std::fs::File;
use std::io::Write;
use std::path::PathBuf;
use std::time::Instant;

use image::codecs::png::PngEncoder;
use image::{ExtendedColorType, ImageEncoder};
use nalgebra::Vector3;
use pollster::block_on;
use serde::Serialize;

use fractal_generator_lib::fractal::engine::FractalEngine;
use fractal_generator_lib::fractal::types::FractalFormula;

#[derive(Debug, Clone, Serialize)]
struct CaseMetrics {
    width: u32,
    height: u32,
    nonblack_ratio: f32,
    luminance_mean: f32,
    luminance_stddev: f32,
    edge_mean: f32,
    elapsed_ms: f64,
    pass: bool,
}

#[derive(Debug, Clone, Serialize)]
struct ValidationReport {
    device_name: String,
    backend: String,
    cases: Vec<(String, CaseMetrics)>,
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
    // Scale: map per-formula. Mandelbox uses scale; Mandelbrot uses zoom.
    params[3] = match &engine.parameters.formula {
        FractalFormula::Mandelbox { scale } => *scale,
        FractalFormula::Mandelbrot { zoom, .. } => *zoom,
        _ => engine.parameters.scale,
    };
    // Position
    // For 2D Mandelbrot, use center as offset.xy so the shader maps correctly
    match &engine.parameters.formula {
        FractalFormula::Mandelbrot { center, .. } => {
            params[4] = center[0];
            params[5] = center[1];
            params[6] = 0.0;
        }
        _ => {
            params[4] = engine.parameters.position.x;
            params[5] = engine.parameters.position.y;
            params[6] = engine.parameters.position.z;
        }
    }
    // Rotation
    // For 2D Julia, carry the Julia constant via rotation.xy (shader expects this)
    match &engine.parameters.formula {
        FractalFormula::Julia { c, .. } => {
            params[7] = c[0];
            params[8] = c[1];
            params[9] = 0.0;
        }
        _ => {
            params[7] = engine.parameters.rotation.x;
            params[8] = engine.parameters.rotation.y;
            params[9] = engine.parameters.rotation.z;
        }
    }

    // Color params (use first two colors of palette if present)
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
    params[16] = engine.parameters.color_saturation;
    params[17] = engine.parameters.color_saturation;
    params[18] = engine.parameters.color_saturation;

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
    params[29] = engine.quality_settings.resolution[0] as f32; // placeholder
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

fn compute_metrics(pixels: &[u8], width: u32, height: u32, elapsed_ms: f64, is_3d: bool) -> CaseMetrics {
    let mut nonblack = 0u64;
    let mut lum_sum = 0f64;
    let mut lum_sq_sum = 0f64;
    let mut edge_sum = 0f64;

    let w = width as usize;
    let h = height as usize;
    let bpr = (width * 4) as usize;

    // Precompute luminance per pixel
    let mut lums = vec![0f64; (w * h)];
    for y in 0..h {
        for x in 0..w {
            let idx = y * bpr + x * 4;
            let r = pixels[idx] as f64;
            let g = pixels[idx + 1] as f64;
            let b = pixels[idx + 2] as f64;
            let lum = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            lums[y * w + x] = lum;
            lum_sum += lum;
            lum_sq_sum += lum * lum;
            if r > 5.0 || g > 5.0 || b > 5.0 {
                nonblack += 1;
            }
        }
    }

    // Simple edge metric: mean absolute diff of luminance with right and down neighbors
    for y in 0..(h - 1) {
        for x in 0..(w - 1) {
            let i = y * w + x;
            let dr = (lums[i] - lums[i + 1]).abs();
            let dd = (lums[i] - lums[i + w]).abs();
            edge_sum += dr + dd;
        }
    }

    let total = (w * h) as f64;
    let nonblack_ratio = nonblack as f64 / total;
    let lum_mean = lum_sum / total;
    let lum_var = (lum_sq_sum / total) - lum_mean * lum_mean;
    let lum_stddev = lum_var.max(0.0).sqrt();
    let edge_mean = edge_sum / ((w - 1) * (h - 1)) as f64;

    // Basic thresholds
    let min_nonblack = if is_3d { 0.05 } else { 0.01 };
    let min_edge = 1.0; // arbitrary but useful sanity check
    let min_stddev = 5.0;
    let pass = nonblack_ratio >= min_nonblack && edge_mean >= min_edge && lum_stddev >= min_stddev;

    CaseMetrics {
        width,
        height,
        nonblack_ratio: nonblack_ratio as f32,
        luminance_mean: lum_mean as f32,
        luminance_stddev: lum_stddev as f32,
        edge_mean: edge_mean as f32,
        elapsed_ms,
        pass,
    }
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Output directory
    let out_dir = PathBuf::from("validation_outputs");
    std::fs::create_dir_all(&out_dir)?;

    // Resolution
    let width: u32 = 1024;
    let height: u32 = 768;

    // Initialize engine
    let mut engine = FractalEngine::new();
    engine.parameters.max_iterations = 768;
    engine.parameters.bailout = 8.0;
    engine.parameters.scale = 1.0;
    engine.parameters.color_saturation = 1.1;
    engine.parameters.position = Vector3::new(0.2, -0.1, -1.2);
    engine.parameters.rotation = Vector3::new(0.3, 0.2, 0.0);
    let mut qs = engine.quality_settings.clone();
    qs.max_iterations = engine.parameters.max_iterations.max(512);
    qs.normal_epsilon = 0.0005;
    engine.set_quality_settings(qs);

    // Headless WGPU
    let instance = wgpu::Instance::default();
    let adapter = block_on(instance.request_adapter(&wgpu::RequestAdapterOptions {
        power_preference: wgpu::PowerPreference::HighPerformance,
        force_fallback_adapter: false,
        compatible_surface: None,
    }))?;
    let (device, queue) = block_on(adapter.request_device(&wgpu::DeviceDescriptor {
        label: Some("Complex Fractal Validation Device"),
        required_features: wgpu::Features::empty(),
        required_limits: wgpu::Limits::default(),
        memory_hints: wgpu::MemoryHints::default(),
        trace: wgpu::Trace::default(),
    }))?;

    // Shader & pipeline
    let compute_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
        label: Some("Fractal Compute Shader"),
        source: wgpu::ShaderSource::Wgsl(include_str!("../fractal/shaders/fractal_compute.wgsl").into()),
    });
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

    // Buffers & texture
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

    // Readback setup
    let bytes_per_pixel = 4u32;
    let unpadded_bytes_per_row = width * bytes_per_pixel;
    let align = wgpu::COPY_BYTES_PER_ROW_ALIGNMENT;
    let padded_bytes_per_row = ((unpadded_bytes_per_row + align - 1) / align) * align;
    let readback_size: u64 = padded_bytes_per_row as u64 * height as u64;
    let read_buffer = device.create_buffer(&wgpu::BufferDescriptor {
        label: Some("Readback Buffer"),
        size: readback_size,
        usage: wgpu::BufferUsages::COPY_DST | wgpu::BufferUsages::MAP_READ,
        mapped_at_creation: false,
    });

    // Test cases
    let mut cases: Vec<(String, FractalFormula, bool)> = vec![
        ("mandelbrot".to_string(), FractalFormula::Mandelbrot { center: [-0.5, 0.0], zoom: 1.0 }, false),
        ("julia".to_string(), FractalFormula::Julia { c: [-0.7, 0.27015], max_iterations: engine.parameters.max_iterations }, false),
        ("mandelbulb".to_string(), FractalFormula::Mandelbulb { power: 8.0 }, true),
        ("mandelbox".to_string(), FractalFormula::Mandelbox { scale: 2.0 }, true),
        ("quaternion_julia".to_string(), FractalFormula::QuaternionJulia { c: [-0.2, 0.8, 0.0, 0.0], max_iterations: engine.parameters.max_iterations }, true),
    ];

    let mut report = ValidationReport { device_name: adapter.get_info().name.clone(), backend: format!("{:?}", adapter.get_info().backend), cases: vec![] };

    for (name, formula, is_3d) in cases.drain(..) {
        engine.parameters.formula = formula.clone();

        // Provide tuned defaults for 3D formulas to ensure crisp results
        if is_3d {
            // Increase ray-march steps and tighten surface epsilon baseline
            engine.parameters.max_iterations = 1024;
            engine.parameters.bailout = 2.0;
            engine.parameters.position = Vector3::new(0.0, 0.0, 0.0);

            // Formula-specific tuning
            match engine.parameters.formula {
                FractalFormula::QuaternionJulia { .. } => {
                    // Use a well-known Julia constant and higher quality for better structure
                    engine.parameters.rotation = Vector3::new(0.35, 0.55, 0.20);
                    engine.parameters.max_iterations = 2048;
                    engine.parameters.bailout = 3.0;
                    let mut qs = engine.quality_settings.clone();
                    qs.max_iterations = engine.parameters.max_iterations;
                    qs.normal_epsilon = 0.00035;
                    engine.set_quality_settings(qs);
                }
                FractalFormula::Mandelbox { scale } => {
                    // Mandelbox uses its own scale in param[3]
                    engine.parameters.scale = scale;
                    engine.parameters.rotation = Vector3::new(0.0, 0.0, 0.0);
                    let mut qs = engine.quality_settings.clone();
                    qs.max_iterations = 2048;
                    qs.normal_epsilon = 0.0012; // slightly larger epsilon to register hits
                    engine.parameters.bailout = 3.0;
                    engine.set_quality_settings(qs);
                }
                _ => {
                    engine.parameters.rotation = Vector3::new(0.0, 0.0, 0.0);
                    engine.parameters.scale = 1.0;
                    let mut qs = engine.quality_settings.clone();
                    qs.max_iterations = engine.parameters.max_iterations;
                    qs.normal_epsilon = 0.0004;
                    engine.set_quality_settings(qs);
                }
            }

            // Palette: warm-to-cool gradient
            engine.parameters.color_palette = vec![
                Vector3::new(0.9, 0.6, 0.35),
                Vector3::new(0.2, 0.4, 0.95),
            ];
        }

        // Upload params
        let params = build_params(&engine, 0.0, (width, height));
        queue.write_buffer(&param_buffer, 0, bytemuck::cast_slice(&params));

        // Dispatch
        let start = Instant::now();
        let mut encoder = device.create_command_encoder(&wgpu::CommandEncoderDescriptor { label: Some("Validation Encoder") });
        {
            let mut pass = encoder.begin_compute_pass(&wgpu::ComputePassDescriptor { label: Some("Compute Pass"), timestamp_writes: None });
            pass.set_pipeline(&compute_pipeline);
            pass.set_bind_group(0, &bind_group, &[]);
            let gx = (width + 15) / 16;
            let gy = (height + 15) / 16;
            pass.dispatch_workgroups(gx, gy, 1);
        }
        queue.submit(Some(encoder.finish()));

        // Copy to readback
        let mut encoder = device.create_command_encoder(&wgpu::CommandEncoderDescriptor { label: Some("Copy Encoder") });
        encoder.copy_texture_to_buffer(
            wgpu::TexelCopyTextureInfo { texture: &output_texture, mip_level: 0, origin: wgpu::Origin3d::ZERO, aspect: wgpu::TextureAspect::All },
            wgpu::TexelCopyBufferInfo { buffer: &read_buffer, layout: wgpu::TexelCopyBufferLayout { offset: 0, bytes_per_row: Some(padded_bytes_per_row), rows_per_image: Some(height) } },
            wgpu::Extent3d { width, height, depth_or_array_layers: 1 },
        );
        queue.submit(Some(encoder.finish()));

        // Map and extract
        let slice = read_buffer.slice(..);
        let (tx, rx) = futures::channel::oneshot::channel();
        slice.map_async(wgpu::MapMode::Read, move |res| { let _ = tx.send(res); });
        let _ = device.poll(wgpu::PollType::Wait);
        block_on(rx)??;
        let data = slice.get_mapped_range();
        let mapped: &[u8] = &data;
        let row_src = padded_bytes_per_row as usize;
        let row_dst = (width * 4) as usize;
        let mut pixels: Vec<u8> = vec![0u8; (row_dst as u64 * height as u64) as usize];
        for row in 0..(height as usize) {
            let src_start = row * row_src;
            let dst_start = row * row_dst;
            let src_slice = &mapped[src_start..src_start + row_dst];
            pixels[dst_start..dst_start + row_dst].copy_from_slice(src_slice);
        }
        drop(data);
        read_buffer.unmap();

        // Metrics
        let elapsed = start.elapsed().as_secs_f64() * 1000.0;
        let metrics = compute_metrics(&pixels, width, height, elapsed, is_3d);

        // Write PNG
        let out_path = out_dir.join(format!("{}.png", name));
        let mut out = File::create(&out_path)?;
        let encoder = PngEncoder::new(&mut out);
        encoder.write_image(&pixels, width, height, ExtendedColorType::Rgba8)?;

        println!(
            "Case {}: {}x{} | nonblack={:.3} edge={:.2} stddev={:.2} elapsed={:.1}ms | {}",
            name,
            width,
            height,
            metrics.nonblack_ratio,
            metrics.edge_mean,
            metrics.luminance_stddev,
            metrics.elapsed_ms,
            if metrics.pass { "PASS" } else { "FAIL" }
        );

        report.cases.push((name, metrics));
    }

    // Write report JSON
    let mut f = File::create(out_dir.join("report.json"))?;
    f.write_all(serde_json::to_string_pretty(&report)?.as_bytes())?;

    Ok(())
}