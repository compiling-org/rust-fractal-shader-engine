// Fractal Compute Shader - Distance Estimation
// Computes fractal distance fields using GPU acceleration

@group(0) @binding(0) var<uniform> uniforms: FractalUniforms;
@group(0) @binding(1) var<storage, read_write> distance_field: array<f32>;

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

@compute @workgroup_size(8, 8, 1)
fn main(@builtin(global_invocation_id) global_id: vec3<u32>) {
    let pixel_coords = vec2<u32>(global_id.x, global_id.y);
    let resolution = vec2<u32>(u32(uniforms.resolution.x), u32(uniforms.resolution.y));

    // Check bounds
    if (pixel_coords.x >= resolution.x || pixel_coords.y >= resolution.y) {
        return;
    }

    // Convert pixel coordinates to fractal space
    let uv = (vec2<f32>(pixel_coords) - uniforms.resolution * 0.5) / uniforms.resolution.y;
    let c = uv * uniforms.zoom + uniforms.offset;

    // Compute distance based on fractal type
    var distance = 0.0;
    var iterations = 0u;

    switch uniforms.fractal_type {
        case 0u: { // Mandelbrot
            let result = mandelbrot_distance(c, uniforms.max_iterations, uniforms.bailout);
            distance = result.distance;
            iterations = result.iterations;
        }
        case 1u: { // Julia
            let result = julia_distance(c, uniforms.julia_c, uniforms.max_iterations, uniforms.bailout);
            distance = result.distance;
            iterations = result.iterations;
        }
        case 2u: { // Mandelbulb
            let result = mandelbulb_distance(vec3<f32>(c.x, c.y, 0.0), uniforms.power, uniforms.max_iterations, uniforms.bailout);
            distance = result.distance;
            iterations = result.iterations;
        }
        default: {
            distance = length(c);
            iterations = 1u;
        }
    }

    // Store distance in output buffer
    let index = pixel_coords.y * resolution.x + pixel_coords.x;
    distance_field[index] = distance;
}

// Mandelbrot distance estimation
fn mandelbrot_distance(c: vec2<f32>, max_iter: u32, bailout: f32) -> DistanceResult {
    var z = vec2<f32>(0.0, 0.0);
    var iterations = 0u;

    loop {
        if (iterations >= max_iter) {
            break;
        }

        let z_squared = z * z;
        let magnitude_squared = z_squared.x + z_squared.y;

        if (magnitude_squared > bailout) {
            break;
        }

        // z = z² + c
        z = vec2<f32>(
            z_squared.x - z_squared.y + c.x,
            2.0 * z.x * z.y + c.y
        );

        iterations = iterations + 1u;
    }

    var distance = 0.0;
    if (iterations < max_iter) {
        let z_magnitude = length(z);
        distance = log(z_magnitude) * 0.5;
    }

    return DistanceResult(distance, iterations);
}

// Julia distance estimation
fn julia_distance(z: vec2<f32>, c: vec2<f32>, max_iter: u32, bailout: f32) -> DistanceResult {
    var current_z = z;
    var iterations = 0u;

    loop {
        if (iterations >= max_iter) {
            break;
        }

        let z_squared = current_z * current_z;
        let magnitude_squared = z_squared.x + z_squared.y;

        if (magnitude_squared > bailout) {
            break;
        }

        // z = z² + c
        current_z = vec2<f32>(
            z_squared.x - z_squared.y + c.x,
            2.0 * current_z.x * current_z.y + c.y
        );

        iterations = iterations + 1u;
    }

    var distance = 0.0;
    if (iterations < max_iter) {
        let z_magnitude = length(current_z);
        distance = log(z_magnitude) * 0.5;
    }

    return DistanceResult(distance, iterations);
}

// Mandelbulb distance estimation (3D)
fn mandelbulb_distance(pos: vec3<f32>, power: f32, max_iter: u32, bailout: f32) -> DistanceResult {
    var z = pos;
    var iterations = 0u;

    loop {
        if (iterations >= max_iter) {
            break;
        }

        let r = length(z);
        if (r > bailout) {
            break;
        }

        // Convert to spherical coordinates
        let theta = atan2(z.y, z.x);
        let phi = asin(z.z / r);

        // Apply power
        let new_r = pow(r, power);
        let new_theta = theta * power;
        let new_phi = phi * power;

        // Convert back to cartesian
        z = new_r * vec3<f32>(
            cos(new_theta) * cos(new_phi),
            sin(new_theta) * cos(new_phi),
            sin(new_phi)
        ) + pos;

        iterations = iterations + 1u;
    }

    var distance = 0.0;
    if (iterations < max_iter) {
        let z_magnitude = length(z);
        distance = log(z_magnitude) * 0.5;
    }

    return DistanceResult(distance, iterations);
}

struct DistanceResult {
    distance: f32,
    iterations: u32,
};