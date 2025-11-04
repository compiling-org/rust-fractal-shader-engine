// Fractal Render Shader
// Final rendering pass for fractal visualization with post-processing

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
        vec2<f32>(0.0, 1.0),
        vec2<f32>(1.0, 1.0),
        vec2<f32>(0.0, 0.0),
        vec2<f32>(0.0, 0.0),
        vec2<f32>(1.0, 1.0),
        vec2<f32>(1.0, 0.0)
    );

    var output: VertexOutput;
    output.position = vec4<f32>(positions[vertex_index], 0.0, 1.0);
    output.uv = uvs[vertex_index];
    return output;
}

@group(0) @binding(0) var fractal_texture: texture_2d<f32>;
@group(0) @binding(1) var texture_sampler: sampler;

struct PostProcessParams {
    time: f32,
    bloom_intensity: f32,
    vignette_amount: f32,
    color_temperature: f32,
    contrast: f32,
    brightness: f32,
    saturation: f32,
};

@group(1) @binding(0) var<uniform> post_params: PostProcessParams;

@fragment
fn fs_main(input: VertexOutput) -> @location(0) vec4<f32> {
    var color = textureSample(fractal_texture, texture_sampler, input.uv);

    // Apply post-processing effects
    color = apply_bloom(color, input.uv);
    color = apply_vignette(color, input.uv);
    color = apply_color_grading(color);

    return color;
}

// Bloom effect using simple blur approximation
fn apply_bloom(color: vec4<f32>, uv: vec2<f32>) -> vec4<f32> {
    if (post_params.bloom_intensity <= 0.0) {
        return color;
    }

    // Simple 5-tap blur for bloom approximation
    // Using constant indexing to avoid WGSL validation errors
    var bloom_color = vec3<f32>(0.0);
    
    // Sample 0
    let sample_uv_0 = clamp(uv + vec2<f32>(0.0, 0.0) * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
    let sample_color_0 = textureSample(fractal_texture, texture_sampler, sample_uv_0).rgb;
    bloom_color = bloom_color + sample_color_0;
    
    // Sample 1
    let sample_uv_1 = clamp(uv + vec2<f32>(0.01, 0.0) * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
    let sample_color_1 = textureSample(fractal_texture, texture_sampler, sample_uv_1).rgb;
    bloom_color = bloom_color + sample_color_1;
    
    // Sample 2
    let sample_uv_2 = clamp(uv + vec2<f32>(-0.01, 0.0) * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
    let sample_color_2 = textureSample(fractal_texture, texture_sampler, sample_uv_2).rgb;
    bloom_color = bloom_color + sample_color_2;
    
    // Sample 3
    let sample_uv_3 = clamp(uv + vec2<f32>(0.0, 0.01) * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
    let sample_color_3 = textureSample(fractal_texture, texture_sampler, sample_uv_3).rgb;
    bloom_color = bloom_color + sample_color_3;
    
    // Sample 4
    let sample_uv_4 = clamp(uv + vec2<f32>(0.0, -0.01) * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
    let sample_color_4 = textureSample(fractal_texture, texture_sampler, sample_uv_4).rgb;
    bloom_color = bloom_color + sample_color_4;
    
    bloom_color = bloom_color / 5.0;

    // Add bloom to bright areas
    let luminance = dot(bloom_color, vec3<f32>(0.299, 0.587, 0.114));
    let bloom_mix = smoothstep(0.5, 1.0, luminance);

    return vec4<f32>(
        mix(color.rgb, bloom_color, bloom_mix * 0.3),
        color.a
    );
}

// Vignette effect
fn apply_vignette(color: vec4<f32>, uv: vec2<f32>) -> vec4<f32> {
    if (post_params.vignette_amount <= 0.0) {
        return color;
    }

    let center = vec2<f32>(0.5, 0.5);
    let dist = distance(uv, center);
    let vignette = 1.0 - smoothstep(0.5, 1.0, dist * post_params.vignette_amount);

    return vec4<f32>(color.rgb * vignette, color.a);
}

// Color grading and tone mapping
fn apply_color_grading(color: vec4<f32>) -> vec4<f32> {
    var rgb = color.rgb;

    // Brightness
    rgb = rgb + post_params.brightness;

    // Contrast
    rgb = (rgb - 0.5) * post_params.contrast + 0.5;

    // Saturation
    let luminance = dot(rgb, vec3<f32>(0.299, 0.587, 0.114));
    rgb = mix(vec3<f32>(luminance), rgb, post_params.saturation);

    // Color temperature (simple approximation)
    if (post_params.color_temperature > 0.0) {
        // Warmer
        rgb.r = rgb.r * (1.0 + post_params.color_temperature * 0.1);
        rgb.b = rgb.b * (1.0 - post_params.color_temperature * 0.05);
    } else if (post_params.color_temperature < 0.0) {
        // Cooler
        rgb.r = rgb.r * (1.0 + post_params.color_temperature * 0.05);
        rgb.b = rgb.b * (1.0 - post_params.color_temperature * 0.1);
    }

    // Clamp to valid range
    rgb = clamp(rgb, vec3<f32>(0.0), vec3<f32>(1.0));

    return vec4<f32>(rgb, color.a);
}