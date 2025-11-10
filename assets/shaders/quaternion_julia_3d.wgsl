// Quaternion Julia (3D slice) — ray marching distance estimate using 4D quaternion iteration
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> fov: f32;
@group(0) @binding(3) var<uniform> c_param: vec4<f32>; // quaternion constant (cx, cy, cz, cw)
@group(0) @binding(4) var<uniform> slice_w: f32;       // 4th dimension slice
@group(0) @binding(5) var<uniform> bailout: f32;
@group(0) @binding(6) var<uniform> max_steps: f32;
@group(0) @binding(7) var<uniform> max_distance: f32;
@group(0) @binding(8) var<uniform> epsilon: f32;
@group(0) @binding(9) var<uniform> cam_pos: vec3<f32>;
@group(0) @binding(10) var<uniform> cam_target: vec3<f32>;

fn qmul(a: vec4<f32>, b: vec4<f32>) -> vec4<f32> {
  // (ax + ay*i + az*j + aw*k) * (bx + by*i + bz*j + bw*k)
  return vec4<f32>(
    a.x*b.x - a.y*b.y - a.z*b.z - a.w*b.w,
    a.x*b.y + a.y*b.x + a.z*b.w - a.w*b.z,
    a.x*b.z - a.y*b.w + a.z*b.x + a.w*b.y,
    a.x*b.w + a.y*b.z - a.z*b.y + a.w*b.x
  );
}

fn qlen(a: vec4<f32>) -> f32 { return sqrt(dot(a, a)); }

fn quaternion_julia_de(p3: vec3<f32>, c: vec4<f32>) -> f32 {
  var z = vec4<f32>(p3.x, p3.y, p3.z, slice_w);
  var dr = 1.0;
  var r = qlen(z);
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    if (r > bailout) { break; }
    // z = z*z + c
    z = qmul(z, z) + c;
    dr = 2.0 * r * dr;
    r = qlen(z);
  }
  return 0.5 * log(r) * r / max(dr, 1e-6);
}

fn estimate_normal(p: vec3<f32>, c: vec4<f32>) -> vec3<f32> {
  let e = epsilon;
  let d = quaternion_julia_de(p, c);
  let nx = quaternion_julia_de(p + vec3<f32>(e, 0.0, 0.0), c) - d;
  let ny = quaternion_julia_de(p + vec3<f32>(0.0, e, 0.0), c) - d;
  let nz = quaternion_julia_de(p + vec3<f32>(0.0, 0.0, e), c) - d;
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

fn ray_march(ro: vec3<f32>, rd: vec3<f32>, c: vec4<f32>) -> vec3<f32> {
  var t = 0.0;
  for (var i: i32 = 0; i < i32(max_steps); i++) {
    let p = ro + rd * t;
    let d = quaternion_julia_de(p, c);
    if (d < epsilon) {
      let n = estimate_normal(p, c);
      let light_dir = normalize(vec3<f32>(0.9, 0.5, 0.3));
      let diff = clamp(dot(n, light_dir), 0.0, 1.0);
      let base = vec3<f32>(0.6, 0.2, 0.9);
      let spec = pow(clamp(dot(n, normalize(light_dir + normalize(-rd))), 0.0, 1.0), 48.0);
      return base * (0.2 + 0.8 * diff) + vec3<f32>(spec);
    }
    t += d;
    if (t > max_distance) { break; }
  }
  return vec3<f32>(0.04, 0.05, 0.08);
}

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
  let ro = cam_pos;
  let rd = look_dir(uv, cam_pos, cam_target, fov);
  let col = ray_march(ro, rd, c_param);
  return vec4<f32>(col, 1.0);
}