// Fractal Compute Shader
// GPU-accelerated distance field computation for real-time fractal rendering

// Parameters are provided as a packed float array from the host to avoid
// uniform-struct alignment mismatches between WGSL and Rust.
// Index mapping:
//  0: max_iterations
//  1: bailout
//  2: power
//  3: scale
//  4..6: offset.xyz
//  7..9: rotation.xyz
// 10..12: base_color.rgb
// 13..15: secondary_color.rgb
// 16: cycle_frequency
// 17: saturation
// 18: value
// 19: density
// 20..22: fog_color.rgb
// 23: scattering
// 24: absorption
// 25: anisotropy
// 26: time
// 27: resolution_x
// 28: resolution_y
// 29: resolution_scale
// 30: max_steps
// 31: surface_epsilon
// 32: formula_id (0: Mandelbrot, 1: Mandelbulb, 2: Mandelbox, 3: Julia, 4: Quaternion Julia)

@group(0) @binding(0) var<storage, read_write> distance_field: array<f32>;
struct Params {
    data: array<vec4<f32>, 64>,
};
@group(0) @binding(1) var<uniform> params: Params;
@group(0) @binding(2) var output_texture: texture_storage_2d<rgba8unorm, write>;

fn param(index: u32) -> f32 {
    let v = params.data[index / 4u];
    let lane = index % 4u;
    return select(
        select(v.x, v.y, lane == 1u),
        select(v.z, v.w, lane == 3u),
        lane > 1u
    );
}

struct DistanceResult {
    distance: f32,
    iterations: f32,
    normal: vec3<f32>,
    ao: f32,
    material_id: f32,
};

fn mandelbrot_distance(pos: vec3<f32>) -> DistanceResult {
    // 2D Mandelbrot on XZ plane
    let c = vec2<f32>(pos.x + param(4u), pos.z + param(6u));
    var z = vec2<f32>(0.0, 0.0);
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(param(0u)); i = i + 1u) {
        if (dot(z, z) > param(1u) * param(1u)) {
            iterations = f32(i);
            break;
        }

        let x = z.x * z.x - z.y * z.y + c.x;
        let y = 2.0 * z.x * z.y + c.y;
        z = vec2<f32>(x, y);
        iterations = iterations + 1.0;
    }

    let distance = abs(pos.y); // Distance from XZ plane
    return DistanceResult(distance, iterations, vec3<f32>(0.0, sign(pos.y), 0.0), 1.0, 0.0);
}

fn mandelbulb_distance(pos: vec3<f32>) -> DistanceResult {
    var p = pos + vec3<f32>(param(4u), param(5u), param(6u));
    var dr = 1.0;
    var r = 0.0;
    var iterations: f32 = 0.0;

    // Add a small epsilon to prevent division by zero
    let epsilon = 1e-12;
    
    for (var i: u32 = 0u; i < u32(param(0u)); i = i + 1u) {
        r = length(p);
        
        // Check for bailout with proper handling
        if (r > param(1u) || r != r) { // Also check for NaN
            iterations = f32(i);
            break;
        }

        // Convert to polar coordinates with safety checks
        var theta = 0.0;
        var phi = 0.0;
        
        if (r > epsilon) {
            theta = acos(clamp(p.z / r, -1.0, 1.0));
            phi = atan2(p.y, p.x);
        }

        // Scale and rotate with safety checks
        let power_minus_one = param(2u) - 1.0;
        if (power_minus_one > 0.0) {
            dr = pow(r, power_minus_one) * param(2u) * dr + 1.0;
        } else {
            dr = dr + 1.0;
        }

        theta = theta * param(2u);
        phi = phi * param(2u);

        let zr = pow(r, param(2u));
        // Convert back to cartesian
        p = zr * vec3<f32>(
            sin(theta) * cos(phi),
            sin(theta) * sin(phi),
            cos(theta)
        );

        iterations = f32(i) + 1.0;
        
        // Safety check for runaway values
        if (length(p) > 1e10) {
            break;
        }
    }

    // Calculate distance with safety checks
    var distance = 0.5 * log(r) * r / dr;
    
    // Prevent NaN or infinity
    if (distance != distance || abs(distance) > 1e20) {
        distance = 1000.0;
    }
    
    // Provide a default normal if p is zero
    var normal = vec3<f32>(0.0, 1.0, 0.0);
    if (length(p) > epsilon) {
        normal = normalize(p);
    }
    
    return DistanceResult(distance, iterations, normal, 1.0, 0.0);
}

fn mandelbox_distance(pos: vec3<f32>) -> DistanceResult {
    var p = pos + vec3<f32>(param(4u), param(5u), param(6u));
    var dz = 1.0;
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(param(0u)); i = i + 1u) {
        // Box fold
        if (p.x > 1.0) {
            p.x = 2.0 - p.x;
        } else if (p.x < -1.0) {
            p.x = -2.0 - p.x;
        }

        if (p.y > 1.0) {
            p.y = 2.0 - p.y;
        } else if (p.y < -1.0) {
            p.y = -2.0 - p.y;
        }

        if (p.z > 1.0) {
            p.z = 2.0 - p.z;
        } else if (p.z < -1.0) {
            p.z = -2.0 - p.z;
        }

        // Sphere fold
        let r2 = dot(p, p);
        if (r2 < 0.25) {
            let temp = 0.25 / r2;
            p = p * temp;
            dz = dz * temp;
        } else if (r2 < 1.0) {
            let temp = 1.0 / sqrt(r2);
            p = p * temp;
            dz = dz * temp;
        }

        // Scale and translate
        p = p * param(3u) + pos;
        dz = dz * abs(param(3u)) + 1.0;

        let r = length(p);
        if (r > param(1u)) {
            iterations = f32(i);
            break;
        }

        iterations = f32(i) + 1.0;
    }

    let distance = length(p) / abs(dz) - 0.001;
    return DistanceResult(distance, iterations, normalize(p), 1.0, 0.0);
}

fn quaternion_julia_distance(pos: vec3<f32>) -> DistanceResult {
    // Convert 3D point to quaternion (w=0)
    var q = vec4<f32>(pos.x, pos.y, pos.z, 0.0);
    let c = vec4<f32>(-0.2, 0.8, 0.0, 0.0); // Julia constant
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(param(0u)); i = i + 1u) {
        let magnitude2 = dot(q, q);
        if (magnitude2 > param(1u) * param(1u)) {
            iterations = f32(i);
            break;
        }

        // Quaternion multiplication: q = q² + c
        let q_new = vec4<f32>(
            q.x * q.x - q.y * q.y - q.z * q.z - q.w * q.w + c.x,
            2.0 * q.x * q.y + c.y,
            2.0 * q.x * q.z + c.z,
            2.0 * q.x * q.w + c.w
        );

        q = q_new;
        iterations = f32(i) + 1.0;
    }

    let distance = sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
    return DistanceResult(distance, iterations, normalize(q.xyz), 1.0, 0.0);
}

@compute @workgroup_size(16, 16, 1)
fn main(@builtin(global_invocation_id) global_id: vec3<u32>) {
    let pixel_coords = vec2<u32>(global_id.x, global_id.y);

    // Check bounds
    if (pixel_coords.x >= u32(param(27u)) || pixel_coords.y >= u32(param(28u))) {
        return;
    }

    // Normalized pixel coordinates
    let uv = vec2<f32>(
        f32(pixel_coords.x) / param(27u),
        f32(pixel_coords.y) / param(28u)
    );

    // Decide path by formula: 2D fractals render per-pixel; others use ray marching
    let fid = i32(param(32u));

    var color = vec4<f32>(0.1, 0.1, 0.2, 1.0); // Default background

    if (fid == 0 || fid == 3) {
        // 2D Mandelbrot / Julia rendering in screen space
        // Map UV to complex plane with simple scaling and offset
        let aspect = param(27u) / param(28u);
        let scale = max(param(3u), 0.0001);
        let x = (uv.x - 0.5) * 3.0 * aspect / scale + param(4u);
        let y = (uv.y - 0.5) * 2.0 / scale + param(5u);

        var z = vec2<f32>(0.0, 0.0);
        var c = vec2<f32>(x, y);

        if (fid == 3) {
            // Use rotation.xy as Julia constant to allow UI tweaks
            c = vec2<f32>(param(7u), param(8u));
            z = vec2<f32>(x, y);
        }

        var iter: u32 = 0u;
        for (var i: u32 = 0u; i < u32(param(0u)); i = i + 1u) {
            // z = z^2 + c
            let zx2 = z.x * z.x - z.y * z.y;
            let zy2 = 2.0 * z.x * z.y;
            z = vec2<f32>(zx2, zy2) + c;

            if (dot(z, z) > param(1u) * param(1u)) {
                iter = i;
                break;
            }
            iter = i;
        }

        // Smooth coloring based on iterations
        let normalized = f32(iter) / max(param(0u), 1.0);
        let mixv = normalized * param(16u);
        let r = (param(10u) * (1.0 - normalized) + param(13u) * normalized) *
                (0.5 + 0.5 * sin(mixv));
        let g = (param(11u) * (1.0 - normalized) + param(14u) * normalized) *
                (0.5 + 0.5 * sin(mixv + 2.0944));
        let b = (param(12u) * (1.0 - normalized) + param(15u) * normalized) *
                (0.5 + 0.5 * sin(mixv + 4.18879));

        // Interior points go darker
        let interior = select(0.0, 1.0, iter >= u32(param(0u) - 1.0));
        color = vec4<f32>(
            clamp(r * param(17u) * (1.0 - 0.8 * interior), 0.0, 1.0),
            clamp(g * param(17u) * (1.0 - 0.8 * interior), 0.0, 1.0),
            clamp(b * param(18u) * (1.0 - 0.8 * interior), 0.0, 1.0),
            1.0
        );
    } else {
        // 3D / distance-estimated fractals via ray marching
        let aspect_ratio = param(27u) / param(28u);
        let tan_fov = tan(radians(60.0) * 0.5);
        let ndc_x = (2.0 * uv.x - 1.0) * aspect_ratio * tan_fov;
        let ndc_y = (1.0 - 2.0 * uv.y) * tan_fov;

        let camera_pos = vec3<f32>(param(4u), param(5u), param(6u) + 5.0);
        let camera_target = vec3<f32>(param(4u), param(5u), param(6u));
        let camera_up = vec3<f32>(0.0, 1.0, 0.0);
        let forward = normalize(camera_target - camera_pos);
        let right = normalize(cross(camera_up, forward));
        let up = cross(forward, right);
        let ray_dir = normalize(forward + right * ndc_x + up * ndc_y);

        var t = 0.0;
        var result = DistanceResult(0.0, 0.0, vec3<f32>(0.0), 0.0, 0.0);
        let min_step = max(param(31u), 0.0001);
        let max_distance = 100.0;

        for (var i: u32 = 0u; i < u32(param(30u)); i = i + 1u) {
            let pos = camera_pos + ray_dir * t;

            if (fid == 1) {
                result = mandelbulb_distance(pos);
            } else if (fid == 2) {
                result = mandelbox_distance(pos);
            } else if (fid == 4) {
                result = quaternion_julia_distance(pos);
            } else {
                result = mandelbulb_distance(pos);
            }

            if (abs(result.distance) < param(31u)) {
                result.iterations = f32(i);
                break;
            }

            let step_size = max(abs(result.distance), min_step);
            t = t + step_size;
            if (t > max_distance || t != t) {
                break;
            }
        }

        let normalized_iterations = result.iterations / max(param(0u), 1.0);
        let color_mix = normalized_iterations * param(16u);
        let r = (param(10u) * (1.0 - normalized_iterations) + param(13u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix));
        let g = (param(11u) * (1.0 - normalized_iterations) + param(14u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix + 2.0944));
        let b = (param(12u) * (1.0 - normalized_iterations) + param(15u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix + 4.18879));

        color = vec4<f32>(
            clamp(r * param(17u), 0.0, 1.0),
            clamp(g * param(17u), 0.0, 1.0),
            clamp(b * param(18u), 0.0, 1.0),
            1.0
        );
    }
    textureStore(output_texture, vec2<i32>(pixel_coords), color);
}