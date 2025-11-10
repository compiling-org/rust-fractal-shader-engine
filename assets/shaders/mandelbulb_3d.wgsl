// Mandelbulb 3D — ray marching with distance estimator and lighting
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> fov: f32;          // field of view (e.g., 1.2)
@group(0) @binding(3) var<uniform> power: f32;        // mandelbulb power (e.g., 8.0)
@group(0) @binding(4) var<uniform> bailout: f32;      // bailout radius (e.g., 8.0)
@group(0) @binding(5) var<uniform> max_steps: f32;    // iteration steps (e.g., 200)
@group(0) @binding(6) var<uniform> max_distance: f32; // ray march max distance (e.g., 100.0)
@group(0) @binding(7) var<uniform> epsilon: f32;      // surface epsilon (e.g., 0.0005)
@group(0) @binding(8) var<uniform> cam_pos: vec3<f32>;// camera position
@group(0) @binding(9) var<uniform> cam_target: vec3<f32>;// camera target

fn mandelbulb_de(pos: vec3<f32>, p: f32, bailout_r: f32, max_iter: i32) -> f32 {
  var z = pos;
  var dr = 1.0;
  var r = length(z);
  for (var i: i32 = 0; i < max_iter; i++) {
    if (r > bailout_r) { break; }
    let theta = acos(z.z / max(r, 1e-6));
    let phi = atan2(z.y, z.x);
    dr = pow(r, p - 1.0) * p * dr + 1.0;
    let zr = pow(r, p);
    let theta_p = theta * p;
    let phi_p = phi * p;
    z = zr * vec3<f32>(sin(theta_p) * cos(phi_p), sin(theta_p) * sin(phi_p), cos(theta_p)) + pos;
    r = length(z);
  }
  return 0.5 * log(r) * r / dr;
}

fn estimate_normal(p: vec3<f32>) -> vec3<f32> {
  let e = epsilon;
  let d = mandelbulb_de(p, power, bailout, i32(max_steps));
  let nx = mandelbulb_de(p + vec3<f32>(e, 0.0, 0.0), power, bailout, i32(max_steps)) - d;
  let ny = mandelbulb_de(p + vec3<f32>(0.0, e, 0.0), power, bailout, i32(max_steps)) - d;
  let nz = mandelbulb_de(p + vec3<f32>(0.0, 0.0, e), power, bailout, i32(max_steps)) - d;
  return normalize(vec3<f32>(nx, ny, nz));
}

fn look_dir(uv: vec2<f32>, pos: vec3<f32>, target: vec3<f32>, fov_y: f32) -> vec3<f32> {
  let f: vec3<f32> = normalize(target - pos);
  let r: vec3<f32> = normalize(cross(vec3<f32>(0.0, 1.0, 0.0), f));
  let u: vec3<f32> = cross(f, r);
  let aspect = resolution.x / resolution.y;
  let px = (uv.x * 2.0 - 1.0) * aspect;
  let py = (uv.y * 2.0 - 1.0);
  let dir = normalize(r * px * fov_y + u * py * fov_y + f);
  return dir;
}

fn ray_march(ro: vec3<f32>, rd: vec3<f32>) -> vec3<f32> {
  var t = 0.0;
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    let p = ro + rd * t;
    let d = mandelbulb_de(p, power, bailout, i32(max_steps));
    if (d < epsilon) {
      let n = estimate_normal(p);
      let light_dir = normalize(vec3<f32>(0.8, 0.6, 0.4));
      let diff = clamp(dot(n, light_dir), 0.0, 1.0);
      let view = normalize(-rd);
      let half_v = normalize(light_dir + view);
      let spec = pow(clamp(dot(n, half_v), 0.0, 1.0), 64.0);
      let base = vec3<f32>(0.2, 0.4, 1.0);
      return base * (0.2 + 0.8 * diff) + vec3<f32>(spec);
    }
    t += d;
    if (t > max_distance) { break; }
  }
  // Background/sky
  let sky = vec3<f32>(0.05, 0.07, 0.12) + 0.15 * vec3<f32>(sin(time * 0.3), sin(time * 0.2 + 2.0), sin(time * 0.25 + 4.0));
  return sky;
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let ro = cam_pos;
  let rd = look_dir(uv, cam_pos, cam_target, fov);
  let col = ray_march(ro, rd);
  // Mild contrast and saturation tweak
  let col2 = clamp((col - 0.5) * 1.2 + 0.5, vec3<f32>(0.0), vec3<f32>(1.0));
  return vec4<f32>(col2, 1.0);
}