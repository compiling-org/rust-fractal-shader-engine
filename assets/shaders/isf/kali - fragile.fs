/*{
  "CATEGORIES": ["Fractal", "Raymarching", "Psychedelic"],
  "DESCRIPTION": "Glass Field by Kali with ISF conversion, palette, fractal, and visual controls",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "morph", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0 },
    { "NAME": "speed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "fractalSize", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 5.0 },
    { "NAME": "cameraPitch", "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.6 },
    { "NAME": "cameraYaw", "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.6 },
    { "NAME": "cameraRoll", "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.0 },
    { "NAME": "colorPalette", "TYPE": "float", "MIN": 0.0, "MAX": 6.999, "DEFAULT": 0.0 },
    { "NAME": "contrast", "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

#define lightcol1 vec3(1.0, 0.95, 0.85)
#define lightcol2 vec3(0.85, 0.95, 1.0)

mat3 rotmat(vec3 v, float angle) {
  float c = cos(angle);
  float s = sin(angle);
  return mat3(
    c + (1.0 - c) * v.x * v.x,      (1.0 - c) * v.x * v.y - s * v.z,  (1.0 - c) * v.x * v.z + s * v.y,
    (1.0 - c) * v.x * v.y + s * v.z,  c + (1.0 - c) * v.y * v.y,      (1.0 - c) * v.y * v.z - s * v.x,
    (1.0 - c) * v.x * v.z - s * v.y,  (1.0 - c) * v.y * v.z + s * v.x,  c + (1.0 - c) * v.z * v.z
  );
}

mat3 cameraMatrix(float pitch, float yaw, float roll) {
  float cx = cos(pitch), sx = sin(pitch);
  float cy = cos(yaw), sy = sin(yaw);
  float cz = cos(roll), sz = sin(roll);
  mat3 rx = mat3(1, 0, 0, 0, cx, -sx, 0, sx, cx);
  mat3 ry = mat3(cy, 0, sy, 0, 1, 0, -sy, 0, cy);
  mat3 rz = mat3(cz, -sz, 0, sz, cz, 0, 0, 0, 1);
  return rz * ry * rx;
}

vec3 palette(float index, float t) {
  float i = floor(index);
  if (i == 0.0) return vec3(sin(t * 6.0), cos(t * 3.5), sin(t * 2.2));
  if (i == 1.0) return vec3(0.7 + 0.3 * cos(t * 2.0), 0.5 + 0.5 * sin(t), 1.0);
  if (i == 2.0) return vec3(t * 0.8, sin(t * 3.0), cos(t * 4.0));
  if (i == 3.0) return vec3(fract(sin(t * 20.0)), fract(cos(t * 5.0)), fract(sin(t * 3.0)));
  if (i == 4.0) return vec3(abs(sin(t * 10.0)), abs(sin(t * 2.0)), 0.8 + 0.2 * sin(t));
  if (i == 5.0) return vec3(0.5 + 0.5 * sin(t * 4.0), 0.5 + 0.5 * sin(t * 2.0 + 2.0), 0.5 + 0.5 * sin(t * 3.0 + 4.0));
  if (i == 6.0) return vec3(smoothstep(0.2, 0.8, sin(t * 5.0)), fract(t), sin(t));
  return vec3(t);
}

float smin(float a, float b, float k) {
  float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
  return mix(b, a, h) - k * h * (1.0 - h);
}

float de(vec3 pos) {
  vec3 A = vec3(fractalSize);
  vec3 p = abs(A - mod(pos, 2.0 * A));
  float sph = length(p) - 2.5;
  float cyl = length(p.xy) - 0.4;
  cyl = min(cyl, length(p.xz)) - 0.4;
  cyl = min(cyl, length(p.yz)) - 0.4;
  return smin(cyl, sph, 1.5);
}

vec3 normal(vec3 pos) {
  vec3 e = vec3(0.01, 0.0, 0.0);
  return normalize(vec3(
    de(pos + e.xyy) - de(pos - e.xyy),
    de(pos + e.yxy) - de(pos - e.yxy),
    de(pos + e.yyx) - de(pos - e.yyx)
  ));
}

void main() {
  float t = TIME * speed;
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2.0 - 1.0;
  uv.y *= RENDERSIZE.y / RENDERSIZE.x;
  float fov = min(t * 0.2 + 0.05, 0.8) / zoom;

  mat3 cam = cameraMatrix(cameraPitch, cameraYaw, cameraRoll);
  vec3 from = cam * vec3(cos(t) * 2.0, sin(t * 0.5) * 10.0, t * 5.0);
  vec3 dir = normalize(cam * vec3(uv * fov, 1.0));

  float totdist = 0.0;
  float glassfade = 1.0;
  float ref = 0.0;
  float maxdist = 50.0;
  float vol = 0.0;
  vec3 spec = vec3(0.0);
  vec3 light1 = normalize(vec3(cos(t), sin(t * 3.0) * 0.5, sin(t)));
  vec3 light2 = normalize(vec3(cos(t), sin(t * 3.0) * 0.5, -sin(t)));

  for (int i = 0; i < 70; i++) {
    vec3 p = from + totdist * dir;
    float d = de(p);
    float fade = exp(-1.0 * pow(totdist / maxdist, 2.0));
    float intens = min(fade, glassfade);
    vec3 n = normal(p);

    if (d > 0.0 && ref > 0.5) {
      ref = 0.0;
      if (dot(dir, n) < -0.5) dir = normalize(refract(dir, n, 1.0 / 0.88));
      vec3 refl = reflect(dir, n);
      spec += lightcol1 * pow(max(dot(refl, light1), 0.0), 40.0) * intens * 0.75;
      spec += lightcol2 * pow(max(dot(refl, light2), 0.0), 40.0) * intens * 0.75;
    }

    if (d < 0.0 && ref < 0.05) {
      ref = 1.0;
      if (dot(dir, n) < -0.05) dir = normalize(refract(dir, n, 0.88));
      vec3 refl = reflect(dir, n);
      glassfade *= 0.6;
      spec += lightcol1 * pow(max(dot(refl, light1), 0.0), 40.0) * intens;
      spec += lightcol2 * pow(max(dot(refl, light2), 0.0), 40.0) * intens;
    }

    totdist += max(0.01, abs(d));
    vol += intens;
  }

  totdist = min(maxdist, totdist);
  vol = pow(vol, 1.5) * 0.0007;

  vec3 col = vec3(vol) + spec * 0.4 + vec3(0.05);
  vec3 palCol = palette(colorPalette, fract(t * 0.2));
  col *= palCol;
  col = mix(vec3(length(col)), col, saturation);
  col = mix(vec3(0.5), col, contrast);
  col *= brightness;
  col *= min(1.5, t);

  gl_FragColor = vec4(col, 1.0);
}
