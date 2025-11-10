// Minimal pseudo-3D fragment shader compatible with the dynamic pipeline.
// It expects a uniform buffer at @group(0) @binding(0) with common params,
// and receives screen UV at @location(0) from the fullscreen quad vertex.

struct CommonUniforms {
    resolution: vec2<f32>,
    time: f32,
    _pad0: f32,
    camera_pos: vec3<f32>,
    camera_fov: f32,
    camera_dir: vec3<f32>,
    camera_target: f32,
    param0: f32,
    param1: f32,
    param2: f32,
    param3: f32,
};

@group(0) @binding(0) var<uniform> uniforms: CommonUniforms;

fn tonemap_simple(color: vec3<f32>) -> vec3<f32> {
    // Simple Reinhard tonemap
    return color / (vec3<f32>(1.0) + color);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    // Map uv from [0,1] to [-1,1] preserving aspect ratio
    let aspect = uniforms.resolution.x / uniforms.resolution.y;
    let p = vec2<f32>((uv.x * 2.0 - 1.0) * aspect, uv.y * 2.0 - 1.0);

    // Fake shading using params and time
    let t = uniforms.time * 0.5;
    let base = vec3<f32>(0.3 + 0.7 * 0.5 * (sin(t) + 1.0), 0.2 + 0.8 * uv.x, 0.2 + 0.8 * uv.y);
    let vignette = 1.0 - 0.3 * length(p);
    let color = base * vignette;

    let mapped = tonemap_simple(color);
    return vec4<f32>(mapped, 1.0);
}