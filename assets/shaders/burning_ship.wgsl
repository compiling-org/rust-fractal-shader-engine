// Burning Ship Fractal (2D) — high-iteration, sharp structures
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> zoom: f32;
@group(0) @binding(3) var<uniform> center_x: f32;
@group(0) @binding(4) var<uniform> center_y: f32;
@group(0) @binding(5) var<uniform> iterations: f32;

fn palette(t: f32) -> vec3<f32> {
  let r = pow(t, 0.2);
  let g = sin(3.0 * t) * 0.5 + 0.5;
  let b = pow(1.0 - t, 0.4);
  return vec3<f32>(r, g, b);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let asp = resolution.x / resolution.y;
  let x0 = (uv.x - 0.5) * 3.2 / max(zoom, 0.0001) * asp + center_x;
  let y0 = (uv.y - 0.5) * 2.4 / max(zoom, 0.0001) + center_y;

  var zx = 0.0;
  var zy = 0.0;
  var iter_hit = iterations;

  for (var i: i32 = 0; i < 1000; i++) {
    if (f32(i) >= iterations) { break; }
    // Burning Ship uses abs for both real and imaginary parts
    zx = abs(zx);
    zy = abs(zy);
    let xt = zx * zx - zy * zy + x0;
    zy = 2.0 * zx * zy + y0;
    zx = xt;
    if (zx * zx + zy * zy > 4.0) {
      iter_hit = f32(i);
      break;
    }
  }

  var col = vec3<f32>(0.0);
  if (iter_hit >= iterations) {
    col = vec3<f32>(0.02, 0.02, 0.02);
  } else {
    // Smooth iteration
    let r2 = zx * zx + zy * zy;
    let mu = iter_hit + 1.0 - log(log(sqrt(r2))) / log(2.0);
    let t = clamp(mu / iterations, 0.0, 1.0);
    col = palette(t);
  }

  // A subtle glow modulation
  let glow = 0.25 + 0.25 * sin(time * 0.6);
  col *= vec3<f32>(1.0 + glow * 0.5);

  return vec4<f32>(col, 1.0);
}