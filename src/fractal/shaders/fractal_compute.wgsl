// Fractal Compute Shader
// GPU-accelerated distance field computation for real-time fractal rendering

struct Params {
    max_iterations: f32,
    bailout: f32,
    power: f32,
    scale: f32,
    offset: vec3<f32>,
    rotation: vec3<f32>,
    base_color: vec3<f32>,
    secondary_color: vec3<f32>,
    cycle_frequency: f32,
    saturation: f32,
    value: f32,
    density: f32,
    fog_color: vec3<f32>,
    scattering: f32,
    absorption: f32,
    anisotropy: f32,
    time: f32,
    resolution_x: f32,
    resolution_y: f32,
    resolution_scale: f32,
    max_steps: f32,
    surface_epsilon: f32,
};

@group(0) @binding(0) var<storage, read_write> distance_field: array<f32>;
@group(0) @binding(1) var<uniform> params: Params;
@group(0) @binding(2) var output_texture: texture_storage_2d<rgba8unorm, write>;

struct DistanceResult {
    distance: f32,
    iterations: f32,
    normal: vec3<f32>,
    ao: f32,
    material_id: f32,
};

// Distance estimation functions for different fractal types
fn mandelbrot_distance(pos: vec3<f32>) -> DistanceResult {
    // 2D Mandelbrot on XZ plane
    let c = vec2<f32>(pos.x + params.offset.x, pos.z + params.offset.z);
    var z = vec2<f32>(0.0, 0.0);
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(params.max_iterations); i = i + 1u) {
        if (dot(z, z) > params.bailout * params.bailout) {
            break;
        }

        // z = z² + c
        let x = z.x * z.x - z.y * z.y + c.x;
        let y = 2.0 * z.x * z.y + c.y;
        z = vec2<f32>(x, y);
        iterations = iterations + 1.0;
    }

    let distance = abs(pos.y); // Distance from XZ plane
    return DistanceResult(distance, iterations, vec3<f32>(0.0, sign(pos.y), 0.0), 1.0, 0.0);
}

fn mandelbulb_distance(pos: vec3<f32>) -> DistanceResult {
    var p = pos + params.offset;
    var dr = 1.0;
    var r = 0.0;
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(params.max_iterations); i = i + 1u) {
        r = length(p);
        if (r > params.bailout) {
            iterations = f32(i);
            break;
        }

        // Convert to polar coordinates
        var theta = acos(p.z / r);
        var phi = atan2(p.y, p.x);

        // Scale and rotate
        dr = pow(r, params.power - 1.0) * params.power * dr + 1.0;

        let zr = pow(r, params.power);
        theta = theta * params.power;
        phi = phi * params.power;

        // Convert back to cartesian
        p = zr * vec3<f32>(
            sin(theta) * cos(phi),
            sin(theta) * sin(phi),
            cos(theta)
        );

        iterations = f32(i) + 1.0;
    }

    let distance = 0.5 * log(r) * r / dr;
    return DistanceResult(distance, iterations, normalize(p), 1.0, 0.0);
}

fn mandelbox_distance(pos: vec3<f32>) -> DistanceResult {
    var p = pos + params.offset;
    var dz = 1.0;
    var iterations: f32 = 0.0;

    for (var i: u32 = 0u; i < u32(params.max_iterations); i = i + 1u) {
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
        p = p * params.scale + pos;
        dz = dz * abs(params.scale) + 1.0;

        let r = length(p);
        if (r > params.bailout) {
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

    for (var i: u32 = 0u; i < u32(params.max_iterations); i = i + 1u) {
        let magnitude2 = dot(q, q);
        if (magnitude2 > params.bailout * params.bailout) {
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

// Main compute shader entry point
@compute @workgroup_size(16, 16, 1)
fn main(@builtin(global_invocation_id) global_id: vec3<u32>) {
    let pixel_coords = vec2<u32>(global_id.x, global_id.y);

    // Check bounds
    if (pixel_coords.x >= u32(params.resolution_x) || pixel_coords.y >= u32(params.resolution_y)) {
        return;
    }

    // Convert pixel coordinates to world space
    let uv = vec2<f32>(
        f32(pixel_coords.x) / params.resolution_x,
        f32(pixel_coords.y) / params.resolution_y
    );

    // Generate ray from camera
    let aspect_ratio = params.resolution_x / params.resolution_y;
    let tan_fov = tan(radians(60.0) * 0.5); // 60 degree FOV

    let ndc_x = (2.0 * uv.x - 1.0) * aspect_ratio * tan_fov;
    let ndc_y = (1.0 - 2.0 * uv.y) * tan_fov;

    // Simple camera setup (can be made configurable)
    let camera_pos = vec3<f32>(0.0, 0.0, 5.0);
    let camera_target = vec3<f32>(0.0, 0.0, 0.0);
    let camera_up = vec3<f32>(0.0, 1.0, 0.0);

    let forward = normalize(camera_target - camera_pos);
    let right = normalize(cross(camera_up, forward));
    let up = cross(forward, right);

    let ray_dir = normalize(forward + right * ndc_x + up * ndc_y);

    // Ray marching
    var t = 0.0;
    var result = DistanceResult(0.0, 0.0, vec3<f32>(0.0), 0.0, 0.0);
    var hit = false;

    for (var i: u32 = 0u; i < u32(params.max_steps); i = i + 1u) {
        let pos = camera_pos + ray_dir * t;

        // Select fractal type (can be made dynamic)
        result = mandelbulb_distance(pos);

        if (result.distance < params.surface_epsilon) {
            hit = true;
            result.iterations = f32(i);
            break;
        }

        t = t + max(result.distance, 0.001);
        if (t > 100.0) { // Max distance
            break;
        }
    }

    // Calculate final color
    var color = vec4<f32>(0.1, 0.1, 0.2, 1.0); // Background

    if (hit) {
        // Iteration-based coloring
        let t = result.iterations / params.max_iterations;
        let color_mix = t * params.cycle_frequency;

        let r = (params.base_color.x * (1.0 - t) + params.secondary_color.x * t) *
                (sin(color_mix) * 0.5 + 0.5);
        let g = (params.base_color.y * (1.0 - t) + params.secondary_color.y * t) *
                (sin(color_mix + 2.0944) * 0.5 + 0.5);
        let b = (params.base_color.z * (1.0 - t) + params.secondary_color.z * t) *
                (sin(color_mix + 4.18879) * 0.5 + 0.5);

        color = vec4<f32>(
            r * params.saturation,
            g * params.saturation,
            b * params.value,
            1.0
        );
    }

    // Write to output texture
    textureStore(output_texture, vec2<i32>(pixel_coords), color);
}