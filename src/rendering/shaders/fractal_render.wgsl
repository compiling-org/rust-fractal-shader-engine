// Fractal Render Shader - Visualization Pipeline
// Renders fractal distance fields with coloring and post-processing

@group(0) @binding(0) var distance_texture: texture_2d<f32>;
@group(0) @binding(1) var<uniform> uniforms: FractalUniforms;

struct FractalUniforms {
    resolution: vec2<f32>,
    time: f32,
    zoom: f32,
    offset: vec2<f32>,
    power: f32,
    bailout: f32,
    julia_c: vec2<f32>,
    fractal_type: u32,
    max_iterations: u32,
    color_cycle: f32,
    brightness: f32,
    contrast: f32,
    saturation: f32,
};

struct VertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0) uv: vec2<f32>,
};

@vertex
fn vs_main(@builtin(vertex_index) vertex_index: u32) -> VertexOutput {
    var positions = array<vec2<f32>, 6>(
        vec2<f32>(-1.0, -1.0),
        vec2<f32>(1.0, -1.0),
        vec2<f32>(-1.0, 1.0),
        vec2<f32>(-1.0, 1.0),
        vec2<f32>(1.0, -1.0),
        vec2<f32>(1.0, 1.0)
    );

    var uvs = array<vec2<f32>, 6>(
        vec2<f32>(0.0, 0.0),
        vec2<f32>(1.0, 0.0),
        vec2<f32>(0.0, 1.0),
        vec2<f32>(0.0, 1.0),
        vec2<f32>(1.0, 0.0),
        vec2<f32>(1.0, 1.0)
    );

    var output: VertexOutput;
    output.position = vec4<f32>(positions[vertex_index], 0.0, 1.0);
    output.uv = uvs[vertex_index];
    return output;
}

@fragment
fn fs_main(input: VertexOutput) -> @location(0) vec4<f32> {
    let tex_coords = vec2<i32>(floor(input.uv * uniforms.resolution));
    let distance_value = textureLoad(distance_texture, tex_coords, 0).r;

    // Convert distance to iteration count approximation
    let t = clamp(distance_value / f32(uniforms.max_iterations), 0.0, 1.0);

    // Apply color cycling
    let cycled_t = (t + uniforms.color_cycle * uniforms.time * 0.1) % 1.0;

    // Generate color using HSV-like approach
    let hue = cycled_t * 6.283185; // 2π
    let saturation = uniforms.saturation;
    let brightness = uniforms.brightness;

    // Convert HSV to RGB
    let c = brightness * saturation;
    let x = c * (1.0 - abs((hue / 1.047198) % 2.0 - 1.0)); // π/3
    let m = brightness - c;

    var rgb: vec3<f32>;
    if (hue < 1.047198) { // 0-60°
        rgb = vec3<f32>(c, x, 0.0);
    } else if (hue < 2.094395) { // 60-120°
        rgb = vec3<f32>(x, c, 0.0);
    } else if (hue < 3.141593) { // 120-180°
        rgb = vec3<f32>(0.0, c, x);
    } else if (hue < 4.188790) { // 180-240°
        rgb = vec3<f32>(0.0, x, c);
    } else if (hue < 5.235988) { // 240-300°
        rgb = vec3<f32>(x, 0.0, c);
    } else { // 300-360°
        rgb = vec3<f32>(c, 0.0, x);
    }

    rgb = rgb + vec3<f32>(m, m, m);

    // Apply contrast
    rgb = ((rgb - vec3<f32>(0.5, 0.5, 0.5)) * uniforms.contrast) + vec3<f32>(0.5, 0.5, 0.5);

    // Clamp to valid range
    rgb = clamp(rgb, vec3<f32>(0.0), vec3<f32>(1.0));

    // Special coloring for escaped points (iterations reached max)
    if (t >= 0.99) {
        rgb = vec3<f32>(0.0, 0.0, 0.0); // Black for interior points
    }

    return vec4<f32>(rgb, 1.0);
}