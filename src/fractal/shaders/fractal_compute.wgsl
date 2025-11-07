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
// 32: formula_id (1: Mandelbulb, 2: Mandelbox, 4: Quaternion Julia)
// 33: fov_degrees

@group(0) @binding(0) var<storage, read_write> distance_field: array<f32>;
struct Params {
    data: array<vec4<f32>, 64>,
};
@group(0) @binding(1) var<uniform> params: Params;
@group(0) @binding(2) var output_texture: texture_storage_2d<rgba8unorm, write>;

fn param(index: u32) -> f32 {
    let v = params.data[index / 4u];
    let lane = index % 4u;
    if (lane == 0u) {
        return v.x;
    } else if (lane == 1u) {
        return v.y;
    } else if (lane == 2u) {
        return v.z;
    }
    return v.w;
}

struct DistanceResult {
    distance: f32,
    iterations: f32,
    normal: vec3<f32>,
    ao: f32,
    material_id: f32,
};

// Helper to evaluate a single formula id at a point
fn eval_formula(fid: i32, p: vec3<f32>) -> f32 {
    if (fid == 1) {
        return mandelbulb_distance(p).distance;
    } else if (fid == 2) {
        return mandelbox_distance(p).distance;
    } else if (fid == 4) {
        return quaternion_julia_distance(p).distance;
    }
    return mandelbulb_distance(p).distance;
}

// Smooth min (union) per IQ
fn smin(a: f32, b: f32, k: f32) -> f32 {
    let h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// Smooth max (intersection)
fn smax(a: f32, b: f32, k: f32) -> f32 {
    return -smin(-a, -b, k);
}

// Combine two distances with a mode
fn combine2(a: f32, b: f32, mode: i32, k: f32) -> f32 {
    if (mode == 0) { // Union
        return min(a, b);
    } else if (mode == 1) { // Intersection
        return max(a, b);
    } else if (mode == 2) { // Subtraction A - B
        return max(a, -b);
    } else if (mode == 3) { // Smooth Union
        return smin(a, b, k);
    } else if (mode == 4) { // Smooth Intersection
        return smax(a, b, k);
    } else if (mode == 5) { // Smooth Subtraction
        return smax(a, -b, k);
    }
    return min(a, b);
}


fn mandelbulb_distance(pos: vec3<f32>) -> DistanceResult {
    // Evaluate at world position; translation is handled by caller
    var p = pos;
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
    // Evaluate at world position; translation is handled by caller
    var p = pos;
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
    // Convert 3D point to quaternion (w=0), evaluated at world position
    var q = vec4<f32>(pos.x, pos.y, pos.z, 0.0);
    // Julia constant from params: use rotation.xyz to carry c
    let c = vec4<f32>(param(7u), param(8u), param(9u), 0.0);
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

    let fid = i32(param(32u));
    var color = vec4<f32>(0.1, 0.1, 0.2, 1.0); // Default background
    {
        // 3D / distance-estimated fractals via ray marching
        let aspect_ratio = param(27u) / param(28u);
        let tan_fov = tan(radians(param(33u)) * 0.5);
        let ndc_x = (2.0 * uv.x - 1.0) * aspect_ratio * tan_fov;
        let ndc_y = (1.0 - 2.0 * uv.y) * tan_fov;

        // Camera looks toward the translation offset; world-space evaluation
        let camera_pos = vec3<f32>(param(4u), param(5u), param(6u) + 3.0);
        let camera_target = vec3<f32>(param(4u), param(5u), param(6u));
        let camera_up = vec3<f32>(0.0, 1.0, 0.0);
        let forward = normalize(camera_target - camera_pos);
        let right = normalize(cross(camera_up, forward));
        let up = cross(forward, right);
        let ray_dir = normalize(forward + right * ndc_x + up * ndc_y);

        var t = 0.0;
        var hit = false;
        var result = DistanceResult(0.0, 0.0, vec3<f32>(0.0), 0.0, 0.0);
        let min_step = max(param(31u), 0.0001);
        let max_distance = 50.0;

        for (var i: u32 = 0u; i < u32(param(30u)); i = i + 1u) {
            let pos_world = camera_pos + ray_dir * t;
            // Compute distance at world position compensating translation
            let d = distance_only(fid, pos_world);
            result.distance = d;

            if (abs(result.distance) < param(31u)) {
                result.iterations = f32(i);
                // Estimate normal at hit position
                result.normal = estimate_normal(fid, pos_world);
                hit = true;
                break;
            }

            // Per-formula step tuning: smaller factor for Mandelbox to resolve fine detail
            var step_factor = 0.9;
            if (fid == 2) {
                step_factor = 0.6;
            }
            let step_size = max(abs(result.distance) * step_factor, min_step);
            t = t + step_size;
            if (t > max_distance || t != t) {
                break;
            }
        }

        let normalized_iterations = result.iterations / max(param(0u), 1.0);
        let color_mix = normalized_iterations * param(16u);
        var r = (param(10u) * (1.0 - normalized_iterations) + param(13u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix));
        var g = (param(11u) * (1.0 - normalized_iterations) + param(14u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix + 2.0944));
        var b = (param(12u) * (1.0 - normalized_iterations) + param(15u) * normalized_iterations) *
                (0.5 + 0.5 * sin(color_mix + 4.18879));

        // Fallback palette if host-provided palette is zero
        let palette_sum = param(10u) + param(11u) + param(12u) + param(13u) + param(14u) + param(15u);
        if (palette_sum == 0.0) {
            let hue = fract(normalized_iterations);
            let k = 6.2831853 * hue;
            r = 0.3 + 0.7 * (0.5 + 0.5 * sin(k));
            g = 0.3 + 0.7 * (0.5 + 0.5 * sin(k + 2.0944));
            b = 0.3 + 0.7 * (0.5 + 0.5 * sin(k + 4.18879));
        }

        if (hit) {
            // Lighting and material from params:
            // 41..43: light_dir (xyz), 44..46: light_color (rgb), 47: intensity
            // 48: metallic, 49: roughness
            let light_dir = normalize(vec3<f32>(param(41u), param(42u), param(43u)));
            let light_color = vec3<f32>(param(44u), param(45u), param(46u));
            let light_intensity = max(param(47u), 0.0);
            let metallic = clamp(param(48u), 0.0, 1.0);
            let roughness = clamp(param(49u), 0.0, 1.0);

            let nrm = normalize(result.normal);
            let diff = clamp(dot(nrm, light_dir), 0.0, 1.0);

            // Ambient occlusion to bring out creases
            let ao = estimate_ao(fid, camera_pos + ray_dir * t, nrm);

            // Base albedo from palette
            let base = vec3<f32>(r * param(17u), g * param(17u), b * param(18u));

            // Diffuse term, modulated by light color and intensity
            let ambient = base * 0.15;
            let diffuse = base * diff * light_color * (0.85 * light_intensity);

            // Specular: Blinn-Phong with roughness controlling exponent
            let view_dir = normalize(-ray_dir);
            let half_vec = normalize(light_dir + view_dir);
            let shininess = mix(4.0, 64.0, 1.0 - roughness);
            let spec = pow(max(dot(nrm, half_vec), 0.0), shininess) * (0.25 * metallic * light_intensity);
            let spec_rgb = light_color * spec;

            var rgb = clamp((ambient + diffuse + spec_rgb) * ao, vec3<f32>(0.0), vec3<f32>(1.0));
            // Gamma correction
            rgb = vec3<f32>(pow(rgb.x, 1.0 / 2.2), pow(rgb.y, 1.0 / 2.2), pow(rgb.z, 1.0 / 2.2));
            color = vec4<f32>(rgb, 1.0);
        } else {
            // Background gradient for sky
            color = vec4<f32>(0.1 + 0.25 * uv.x, 0.1 + 0.25 * uv.y, 0.25, 1.0);
        }
    }
    textureStore(output_texture, vec2<i32>(pixel_coords), color);
}

// Helper: get distance at world position by compensating translation
fn distance_only(fid_local: i32, pos_world: vec3<f32>) -> f32 {
    // Translate to local object space
    let translated = pos_world - vec3<f32>(param(4u), param(5u), param(6u));

    // If combiner is active, evaluate multiple formulas and combine
    if (param(34u) > 0.5) {
        let f1 = i32(param(35u));
        let f2 = i32(param(36u));
        let f3 = i32(param(37u));

        var d1 = 1e6;
        if (f1 != 0) {
            d1 = eval_formula(f1, translated);
        }
        var d2 = 1e6;
        if (f2 != 0) {
            d2 = eval_formula(f2, translated);
        }
        var d3 = 1e6;
        if (f3 != 0) {
            d3 = eval_formula(f3, translated);
        }

        let mode = i32(param(38u));
        let k = max(param(40u), 1e-4);
        let blend = clamp(param(39u), 0.0, 1.0);

        var d12 = combine2(d1, d2, mode, k);
        var d123 = combine2(d12, d3, mode, k);
        // Simple blending to bias towards the first two vs third
        return mix(d12, d123, blend);
    }

    // Single formula path
    if (fid_local == 1) {
        return mandelbulb_distance(translated).distance;
    } else if (fid_local == 2) {
        return mandelbox_distance(translated).distance;
    } else if (fid_local == 4) {
        return quaternion_julia_distance(translated).distance;
    }
    return mandelbulb_distance(translated).distance;
}

// Estimate surface normal via distance field gradient
fn estimate_normal(fid_local: i32, pos_world: vec3<f32>) -> vec3<f32> {
    let e = max(param(31u) * 2.0, 0.0005);
    let dx = distance_only(fid_local, pos_world + vec3<f32>(e, 0.0, 0.0)) -
             distance_only(fid_local, pos_world - vec3<f32>(e, 0.0, 0.0));
    let dy = distance_only(fid_local, pos_world + vec3<f32>(0.0, e, 0.0)) -
             distance_only(fid_local, pos_world - vec3<f32>(0.0, e, 0.0));
    let dz = distance_only(fid_local, pos_world + vec3<f32>(0.0, 0.0, e)) -
             distance_only(fid_local, pos_world - vec3<f32>(0.0, 0.0, e));
    return normalize(vec3<f32>(dx, dy, dz));
}

// Estimate simple ambient occlusion by sampling along the normal
fn estimate_ao(fid_local: i32, pos_world: vec3<f32>, nrm: vec3<f32>) -> f32 {
    let eps = max(param(31u), 0.0001);
    var occ = 0.0;
    var weight = 1.0;
    for (var i: i32 = 1; i <= 4; i = i + 1) {
        let t = f32(i) * eps * 2.5;
        let d = distance_only(fid_local, pos_world + nrm * t);
        occ = occ + (t - d) * weight;
        weight = weight * 0.7;
    }
    let ao = clamp(1.0 - occ * 0.5, 0.0, 1.0);
    return ao;
}