// Kaleidoscopic IFS (3D) — plane folding symmetries, noisy DE, ray marching
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> fov: f32;
@group(0) @binding(3) var<uniform> fold_strength: f32; // ~1.0
@group(0) @binding(4) var<uniform> max_steps: f32;
@group(0) @binding(5) var<uniform> max_distance: f32;
@group(0) @binding(6) var<uniform> epsilon: f32;
@group(0) @binding(7) var<uniform> cam_pos: vec3<f32>;
@group(0) @binding(8) var<uniform> cam_target: vec3<f32>;

fn fold_plane(p: vec3<f32>, n: vec3<f32>) -> vec3<f32> {
  let d = dot(p, n);
  return p - 2.0 * min(0.0, d) * n;
}

fn kifs_de(p0: vec3<f32>) -> f32 {
  var p = p0;
  // Multiple plane folds to create kaleidoscopic symmetry
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    p = fold_plane(p, normalize(vec3<f32>(1.0, 0.0, 0.0)));
    p = fold_plane(p, normalize(vec3<f32>(0.0, 1.0, 0.0)));
    p = fold_plane(p, normalize(vec3<f32>(0.0, 0.0, 1.0)));
    p = fold_plane(p, normalize(vec3<f32>(1.0, 1.0, 0.0)));
    p = fold_plane(p, normalize(vec3<f32>(0.0, 1.0, 1.0)));
    p *= 1.0 + 0.1 * sin(float(i) * 0.7 + time * 0.5) * fold_strength;
    if (length(p) > 4.0) { break; }
  }
  // Distance to a carved shape (sphere minus cross-box) for visual interest
  let d_sphere = length(p) - 1.0;
  let d_box = max(abs(p.x) - 0.6, max(abs(p.y) - 0.2, abs(p.z) - 0.6));
  return max(d_sphere, -d_box);
}

fn estimate_normal(p: vec3<f32>) -> vec3<f32> {
  let e = epsilon;
  let d = kifs_de(p);
  let nx = kifs_de(p + vec3<f32>(e, 0.0, 0.0)) - d;
  let ny = kifs_de(p + vec3<f32>(0.0, e, 0.0)) - d;
  let nz = kifs_de(p + vec3<f32>(0.0, 0.0, e)) - d;
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
    let d = kifs_de(p);
    if (d < epsilon) {
      let n = estimate_normal(p);
      let light_dir = normalize(vec3<f32>(-0.5, 0.7, 0.6));
      let diff = clamp(dot(n, light_dir), 0.0, 1.0);
      let hue = 0.5 + 0.5 * sin(time + p.y * 2.0);
      let base = vec3<f32>(0.6 + 0.4 * hue, 0.3 + 0.2 * hue, 0.8 - 0.3 * hue);
      return base * (0.25 + 0.75 * diff);
    }
    t += max(d, 0.001);
    if (t > max_distance) { break; }
  }
  return vec3<f32>(0.02, 0.03, 0.05);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let ro = cam_pos;
  let rd = look_dir(uv, cam_pos, cam_target, fov);
  let col = ray_march(ro, rd);
  return vec4<f32>(col, 1.0);
}