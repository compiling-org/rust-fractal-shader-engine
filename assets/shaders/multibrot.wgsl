// Multibrot Fractal (2D) — general exponent power for complex iteration
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> zoom: f32;
@group(0) @binding(3) var<uniform> center_x: f32;
@group(0) @binding(4) var<uniform> center_y: f32;
@group(0) @binding(5) var<uniform> iterations: f32;
@group(0) @binding(6) var<uniform> power: f32; // e.g., 2.0 (Mandelbrot), 3.0 (Multibrot)

fn complex_pow(z: vec2<f32>, p: f32) -> vec2<f32> {
  let r = length(z);
  let a = atan2(z.y, z.x);
  let rp = pow(r, p);
  let ap = a * p;
  return vec2<f32>(rp * cos(ap), rp * sin(ap));
}

fn colorize(t: f32) -> vec3<f32> {
  // Warm-cool gradient with contrast
  let r = pow(t, 0.5);
  let g = sin(6.0 * t + 1.5) * 0.5 + 0.5;
  let b = pow(1.0 - t, 0.7);
  return vec3<f32>(r, g, b);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let asp = resolution.x / resolution.y;
  let x0 = (uv.x - 0.5) * 3.0 / max(zoom, 0.0001) * asp + center_x;
  let y0 = (uv.y - 0.5) * 2.0 / max(zoom, 0.0001) + center_y;
  let c = vec2<f32>(x0, y0);

  var z = vec2<f32>(0.0, 0.0);
  var iter_hit = iterations;
  for (var i: i32 = 0; i < 1000; i++) {
    if (f32(i) >= iterations) { break; }
    z = complex_pow(z, power) + c;
    if (dot(z, z) > 4.0) {
      iter_hit = f32(i);
      break;
    }
  }

  var col = vec3<f32>(0.0);
  if (iter_hit >= iterations) {
    col = vec3<f32>(0.0);
  } else {
    let mu = iter_hit + 1.0 - log(log(length(z))) / log(2.0);
    let t = clamp(mu / iterations, 0.0, 1.0);
    col = colorize(t);
  }

  // Gentle temporal modulation for visual richness
  col = mix(col, vec3<f32>(col.g, col.b, col.r), 0.15 * sin(time * 0.5));
  return vec4<f32>(col, 1.0);
}