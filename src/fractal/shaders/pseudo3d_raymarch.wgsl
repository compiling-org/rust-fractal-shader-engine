// Fragment-based pseudo-3D raymarcher for fractal SDFs (ShadPlay-style)
// Renders screen-space raymarched fractals directly in the fragment stage.

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

    var out: VertexOutput;
    out.position = vec4<f32>(positions[vertex_index], 0.0, 1.0);
    out.uv = uvs[vertex_index];
    return out;
}

struct CommonUniforms {
    resolution: vec2<f32>,
    time: f32,
    padding0: f32,
    camera_pos: vec3<f32>,
    padding1: f32,
    camera_dir: vec3<f32>,
    fov: f32,
    mandelbox_scale: f32,
    mandelbox_folding_limit: f32,
    bailout: f32,
    max_iterations: f32,
};

@group(0) @binding(0) var<uniform> uniforms: CommonUniforms;

fn rotate_y(p: vec3<f32>, a: f32) -> vec3<f32> {
    let c = cos(a);
    let s = sin(a);
    return vec3<f32>(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}

// Mandelbox distance estimator (approximate)
fn de_mandelbox(p_in: vec3<f32>, scale: f32, folding_limit: f32, max_iter: u32) -> f32 {
    var p = p_in;
    var dr = 1.0;
    var r2 = 0.0;
    let min_r = 0.5;
    let fixed_r = 1.0;

    for (var i: u32 = 0u; i < max_iter; i = i + 1u) {
        // Box fold
        p = clamp(p, vec3<f32>(-folding_limit), vec3<f32>(folding_limit)) * 2.0 - p;

        // Sphere fold
        r2 = dot(p, p);
        if (r2 < min_r) {
            let k = fixed_r / min_r;
            p = p * k;
            dr = dr * k;
        } else if (r2 < fixed_r) {
            let k = fixed_r / r2;
            p = p * k;
            dr = dr * k;
        }

        // Scale and translate
        p = p * scale + p_in;
        dr = dr * abs(scale) + 1.0;

        if (length(p) > uniforms.bailout) {
            break;
        }
    }
    return length(p) / abs(dr);
}

fn estimate_normal(p: vec3<f32>) -> vec3<f32> {
    let e = 0.001;
    let d = de_mandelbox(p, uniforms.mandelbox_scale, uniforms.mandelbox_folding_limit, u32(uniforms.max_iterations));
    let nx = de_mandelbox(p + vec3<f32>(e, 0.0, 0.0), uniforms.mandelbox_scale, uniforms.mandelbox_folding_limit, u32(uniforms.max_iterations)) - d;
    let ny = de_mandelbox(p + vec3<f32>(0.0, e, 0.0), uniforms.mandelbox_scale, uniforms.mandelbox_folding_limit, u32(uniforms.max_iterations)) - d;
    let nz = de_mandelbox(p + vec3<f32>(0.0, 0.0, e), uniforms.mandelbox_scale, uniforms.mandelbox_folding_limit, u32(uniforms.max_iterations)) - d;
    return normalize(vec3<f32>(nx, ny, nz));
}

@fragment
fn fs_main(input: VertexOutput) -> @location(0) vec4<f32> {
    let uv = (input.uv * 2.0 - vec2<f32>(1.0, 1.0));
    let aspect = uniforms.resolution.x / uniforms.resolution.y;
    let fov_scale = tan(0.5 * uniforms.fov * 3.1415926 / 180.0);

    // Camera basis (simple yaw from time for demo)
    let dir = normalize(vec3<f32>(uv.x * aspect * fov_scale, uv.y * fov_scale, -1.0));
    var ray_dir = rotate_y(dir, 0.15 * uniforms.time);
    var ray_pos = uniforms.camera_pos;

    // Raymarch
    var total_dist = 0.0;
    var hit = false;
    let max_steps = 128u;
    for (var i: u32 = 0u; i < max_steps; i = i + 1u) {
        let p = ray_pos + ray_dir * total_dist;
    let d = de_mandelbox(p, uniforms.mandelbox_scale, uniforms.mandelbox_folding_limit, u32(uniforms.max_iterations));
        if (d < 0.001) {
            hit = true;
            break;
        }
        total_dist = total_dist + d;
        if (total_dist > uniforms.bailout) {
            break;
        }
    }

    var color = vec3<f32>(0.0, 0.0, 0.0);
    if (hit) {
        let p = ray_pos + ray_dir * total_dist;
        let n = estimate_normal(p);
        let light_dir = normalize(vec3<f32>(0.4, 0.7, -0.2));
        let diff = max(dot(n, light_dir), 0.0);
        let rim = pow(1.0 - max(dot(n, -ray_dir), 0.0), 2.0);
        color = vec3<f32>(0.8, 0.9, 1.0) * diff + vec3<f32>(0.2, 0.5, 0.9) * rim;
    } else {
        // Background gradient
        color = vec3<f32>(0.05, 0.07, 0.10) + 0.05 * vec3<f32>(uv.y + 1.0);
    }

    return vec4<f32>(color, 1.0);
}