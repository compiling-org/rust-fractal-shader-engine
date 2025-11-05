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
    // Sample texture with bounds checking
    var color = vec4<f32>(0.0, 0.0, 0.0, 1.0);
    
    // Check UV bounds to prevent sampling issues
    if (input.uv.x >= 0.0 && input.uv.x <= 1.0 && input.uv.y >= 0.0 && input.uv.y <= 1.0) {
        color = textureSample(fractal_texture, texture_sampler, input.uv);
    } else {
        // Return a default color for out-of-bounds sampling
        color = vec4<f32>(0.1, 0.1, 0.2, 1.0);
    }

    // Apply post-processing effects with parameter validation
    // Check for valid finite values
    if (color.r == color.r && color.g == color.g && color.b == color.b && 
        abs(color.r) < 1000000.0 && abs(color.g) < 1000000.0 && abs(color.b) < 1000000.0) {
        color = apply_bloom(color, input.uv);
        color = apply_vignette(color, input.uv);
        color = apply_color_grading(color);
    } else {
        // Return a safe default color if NaN or infinity is detected
        color = vec4<f32>(0.5, 0.0, 0.0, 1.0);
    }

    return color;
}

// Bloom effect using simple blur approximation
fn apply_bloom(color: vec4<f32>, uv: vec2<f32>) -> vec4<f32> {
    if (post_params.bloom_intensity <= 0.0) {
        return color;
    }

    // Simple 5-tap blur for bloom approximation
    var bloom_color = vec3<f32>(0.0);
    var sample_count = 0.0;
    
    // Define sample offsets
    let offsets = array<vec2<f32>, 5>(
        vec2<f32>(0.0, 0.0),
        vec2<f32>(0.01, 0.0),
        vec2<f32>(-0.01, 0.0),
        vec2<f32>(0.0, 0.01),
        vec2<f32>(0.0, -0.01)
    );
    
    // Sample all offsets
    for (var i = 0; i < 5; i = i + 1) {
        let sample_uv = clamp(uv + offsets[i] * post_params.bloom_intensity, vec2<f32>(0.0), vec2<f32>(1.0));
        // Check bounds before sampling
        if (sample_uv.x >= 0.0 && sample_uv.x <= 1.0 && sample_uv.y >= 0.0 && sample_uv.y <= 1.0) {
            let sample_color = textureSample(fractal_texture, texture_sampler, sample_uv).rgb;
            // Check for valid values
            if (sample_color.r == sample_color.r && sample_color.g == sample_color.g && sample_color.b == sample_color.b) {
                bloom_color = bloom_color + sample_color;
                sample_count = sample_count + 1.0;
            }
        }
    }
    
    // Average the samples
    if (sample_count > 0.0) {
        bloom_color = bloom_color / sample_count;
    }

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