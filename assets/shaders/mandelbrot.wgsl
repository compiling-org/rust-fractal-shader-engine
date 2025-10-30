// Mandelbrot Fractal Shader
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> zoom: f32;
@group(0) @binding(3) var<uniform> center_x: f32;
@group(0) @binding(4) var<uniform> center_y: f32;
@group(0) @binding(5) var<uniform> iterations: f32;

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    var color = vec3<f32>(0.0);
    
    // Normalize coordinates to fractal space
    let aspect = resolution.x / resolution.y;
    let x = (uv.x - 0.5) * 3.0 / zoom + center_x;
    let y = (uv.y - 0.5) * 2.0 / zoom + center_y;
    
    // Complex number for Mandelbrot iteration
    var cx = x;
    var cy = y;
    var zx = 0.0;
    var zy = 0.0;
    var iter = 0.0;
    
    // Mandelbrot iteration
    for (var i: i32 = 0; i < 500; i++) {
        if (f32(i) >= iterations) {
            break;
        }
        
        let zx_temp = zx * zx - zy * zy + cx;
        zy = 2.0 * zx * zy + cy;
        zx = zx_temp;
        
        if (zx * zx + zy * zy > 4.0) {
            iter = f32(i);
            break;
        }
        
        iter = iterations;
    }
    
    // Color mapping based on iteration count
    if (iter >= iterations) {
        color = vec3<f32>(0.0); // Inside the set (black)
    } else {
        // Smooth coloring
        let mu = iter + 1.0 - log(log(sqrt(zx * zx + zy * zy))) / log(2.0);
        let t = mu / iterations;
        
        // Color gradient
        let r = sin(t * 6.0) * 0.5 + 0.5;
        let g = sin(t * 6.0 + 2.0) * 0.5 + 0.5;
        let b = sin(t * 6.0 + 4.0) * 0.5 + 0.5;
        
        color = vec3<f32>(r, g, b);
    }
    
    return vec4<f32>(color, 1.0);
}