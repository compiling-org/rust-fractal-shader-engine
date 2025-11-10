// Mandelbox-style pseudo-3D raymarch fragment compatible with the dynamic pipeline.
// Receives UV at @location(0) and uses a minimal uniform layout at binding(0).

struct CommonUniforms {
    resolution: vec2<f32>,
    time: f32,
    _pad0: f32,
    camera_pos: vec3<f32>,
    camera_fov: f32,
    camera_dir: vec3<f32>,
    camera_target: f32,
    param0: f32, // scale
    param1: f32, // fixed radius
    param2: f32, // min radius
    param3: f32, // max steps or other control
};

@group(0) @binding(0) var<uniform> uniforms: CommonUniforms;

fn rotate_y(v: vec3<f32>, a: f32) -> vec3<f32> {
    let c = cos(a);
    let s = sin(a);
    return vec3<f32>(c * v.x + s * v.z, v.y, -s * v.x + c * v.z);
}

fn de_mandelbox(p: vec3<f32>) -> f32 {
    // Basic Mandelbox distance estimate
    var z = p;
    var dr = 1.0;
    let scale = max(uniforms.param0, 1.0);
    let min_r = max(uniforms.param2, 0.5);
    let fix_r = max(uniforms.param1, 1.0);

    for (var i = 0u; i < 20u; i = i + 1u) {
        // Box fold
        z = clamp(z, -vec3<f32>(1.0), vec3<f32>(1.0)) * 2.0 - z;
        // Sphere fold
        let r2 = dot(z, z);
        if (r2 < min_r) {
            let r = min_r;
            z *= (fix_r * fix_r) / r;
            dr *= (fix_r * fix_r) / r;
        } else if (r2 < fix_r * fix_r) {
            let r = r2;
            z *= (fix_r * fix_r) / r;
            dr *= (fix_r * fix_r) / r;
        }
        // Scale and translate
        z = z * scale + p;
        dr = dr * abs(scale) + 1.0;
    }
    return length(z) / abs(dr);
}

fn ray_dir(uv: vec2<f32>) -> vec3<f32> {
    let aspect = uniforms.resolution.x / uniforms.resolution.y;
    let f = uniforms.camera_fov;
    // Map uv to NDC and build a direction
    let p = vec2<f32>((uv.x * 2.0 - 1.0) * aspect, uv.y * 2.0 - 1.0);
    let dir = normalize(vec3<f32>(p.x, p.y, f));
    // Rotate around Y slowly for motion
    return rotate_y(dir, 0.2 * uniforms.time);
}

fn march(ro: vec3<f32>, rd: vec3<f32>) -> vec3<f32> {
    var t = 0.0;
    var col = vec3<f32>(0.0);
    let max_steps = u32(clamp(uniforms.param3, 32.0, 200.0));
    for (var i = 0u; i < max_steps; i = i + 1u) {
        let pos = ro + rd * t;
        let d = de_mandelbox(pos);
        if (d < 0.001) {
            // Normal by gradient approximation
            let e = 0.0005;
            let n = normalize(vec3<f32>(
                de_mandelbox(pos + vec3<f32>(e, 0.0, 0.0)) - de_mandelbox(pos - vec3<f32>(e, 0.0, 0.0)),
                de_mandelbox(pos + vec3<f32>(0.0, e, 0.0)) - de_mandelbox(pos - vec3<f32>(0.0, e, 0.0)),
                de_mandelbox(pos + vec3<f32>(0.0, 0.0, e)) - de_mandelbox(pos - vec3<f32>(0.0, 0.0, e))
            ));
            let l = normalize(vec3<f32>(0.8, 0.6, 0.4));
            let diff = clamp(dot(n, l), 0.0, 1.0);
            col = vec3<f32>(0.4, 0.7, 1.0) * diff + vec3<f32>(0.05);
            break;
        }
        t += d;
        if (t > 20.0) {
            col = vec3<f32>(0.0);
            break;
        }
    }
    return col;
}

fn tonemap_simple(color: vec3<f32>) -> vec3<f32> {
    return color / (vec3<f32>(1.0) + color);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    let ro = uniforms.camera_pos;
    let rd = ray_dir(uv);
    let color = march(ro, rd);
    let mapped = tonemap_simple(color);
    return vec4<f32>(mapped, 1.0);
}