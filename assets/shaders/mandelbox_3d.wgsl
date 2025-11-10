// Mandelbox 3D — sphere/box folds with distance estimator and ray marching
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> fov: f32;
@group(0) @binding(3) var<uniform> scale: f32;        // typical ~1.7
@group(0) @binding(4) var<uniform> min_radius: f32;   // ~0.5
@group(0) @binding(5) var<uniform> fixed_radius: f32; // ~1.0
@group(0) @binding(6) var<uniform> bailout: f32;      // ~8.0
@group(0) @binding(7) var<uniform> max_steps: f32;    // iterations, e.g., 200
@group(0) @binding(8) var<uniform> max_distance: f32; // ray march cap
@group(0) @binding(9) var<uniform> epsilon: f32;      // surface epsilon
@group(0) @binding(10) var<uniform> cam_pos: vec3<f32>;
@group(0) @binding(11) var<uniform> cam_target: vec3<f32>;

fn box_fold(p: vec3<f32>) -> vec3<f32> {
  return clamp(p, vec3<f32>(-1.0), vec3<f32>(1.0)) * 2.0 - p;
}

fn mandelbox_de(pos: vec3<f32>) -> f32 {
  var z = pos;
  var dr = 1.0;
  let minR2 = min_radius * min_radius;
  let fixedR2 = fixed_radius * fixed_radius;
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    // Box fold
    z = box_fold(z);
    // Sphere fold
    let r2 = dot(z, z);
    if (r2 < minR2) {
      let t = fixedR2 / minR2;
      z *= t;
      dr *= t;
    } else if (r2 < fixedR2) {
      let t = fixedR2 / r2;
      z *= t;
      dr *= t;
    }
    // Scale and translate
    z = z * scale + pos;
    dr = dr * abs(scale) + 1.0;
    if (length(z) > bailout) { break; }
  }
  return length(z) / abs(dr);
}

fn estimate_normal(p: vec3<f32>) -> vec3<f32> {
  let e = epsilon;
  let d = mandelbox_de(p);
  let nx = mandelbox_de(p + vec3<f32>(e, 0.0, 0.0)) - d;
  let ny = mandelbox_de(p + vec3<f32>(0.0, e, 0.0)) - d;
  let nz = mandelbox_de(p + vec3<f32>(0.0, 0.0, e)) - d;
  return normalize(vec3<f32>(nx, ny, nz));
}

fn look_dir(uv: vec2<f32>, pos: vec3<f32>, target: vec3<f32>, fov_y: f32) -> vec3<f32> {
  let f: vec3<f32> = normalize(target - pos);
  let r: vec3<f32> = normalize(cross(vec3<f32>(0.0, 1.0, 0.0), f));
  let u: vec3<f32> = cross(f, r);
  let aspect = resolution.x / resolution.y;
  let px = (uv.x * 2.0 - 1.0) * aspect;
  let py = (uv.y * 2.0 - 1.0);
  return normalize(r * px * fov_y + u * py * fov_y + f);
}

fn ray_march(ro: vec3<f32>, rd: vec3<f32>) -> vec3<f32> {
  var t = 0.0;
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    let p = ro + rd * t;
    let d = mandelbox_de(p);
    if (d < epsilon) {
      let n = estimate_normal(p);
      let light_dir = normalize(vec3<f32>(0.6, 0.7, 0.4));
      let diff = clamp(dot(n, light_dir), 0.0, 1.0);
      let base = vec3<f32>(0.95, 0.9, 0.85);
      let col = base * (0.25 + 0.75 * diff);
      // Subtle AO hack via total distance traveled
      col *= clamp(1.0 - t * 0.02, 0.4, 1.0);
      return col;
    }
    t += d;
    if (t > max_distance) { break; }
  }
  return vec3<f32>(0.02, 0.03, 0.06);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let ro = cam_pos;
  let rd = look_dir(uv, cam_pos, cam_target, fov);
  let col = ray_march(ro, rd);
  return vec4<f32>(col, 1.0);
}