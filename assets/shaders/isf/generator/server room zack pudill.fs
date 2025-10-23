/*{
  "CATEGORIES": [ "Fractal", "Tunnel", "Trippy" ],
  "DESCRIPTION": "Psychedelic tunnel fractal with morphing patterns and customizable palettes",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "FractalType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "PaletteIndex", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] }
  ]
}*/

const float tmax = 20.0;

float hash(float n) {
  return fract(sin(n) * 43758.5453);
}

vec3 hash(vec3 p) {
  float n = sin(dot(p, vec3(7, 157, 113)));
  return fract(vec3(2097152, 262144, 32768) * n);
}

float noise(float g) {
  float p = floor(g);
  float f = fract(g);
  return mix(hash(p), hash(p + 1.0), f);
}

float voronoi(vec3 x) {
  vec3 p = floor(x);
  vec3 f = fract(x);
  vec2 res = vec2(8.0);

  for (int i = -1; i <= 1; i++)
    for (int j = -1; j <= 1; j++)
      for (int k = -1; k <= 1; k++) {
        vec3 g = vec3(float(i), float(j), float(k));
        vec3 r = g + hash(p + g) - f;
        float d = max(abs(r.x), max(abs(r.y), abs(r.z)));
        if (d < res.x) {
          res.y = res.x;
          res.x = d;
        } else if (d < res.y) {
          res.y = d;
        }
      }

  return res.y - res.x;
}

vec2 path(float z) {
  return vec2(cos(z / 8.0) * sin(z / 12.0) * 12.0, 0.0);
}

float map(vec3 p) {
  vec4 q = vec4(p, 1.0 + Morph * 4.0);
  q.x += 1.0;

  for (int i = 0; i < 6; i++) {
    q.xyz = -1.0 + 2.0 * fract(0.5 + 0.5 * q.xyz);
    q = (1.2 + Morph) * q / max(dot(q.xyz, q.xyz), 0.1);
  }

  vec2 tun = abs(p.xy - path(p.z)) * vec2(0.6, 0.5);
  return min(0.25 * abs(q.y) / q.w, 1.0 - max(tun.x, tun.y));
}

float march(vec3 ro, vec3 rd, float mx) {
  float t = 0.0;
  for (int i = 0; i < 200; i++) {
    float d = map(ro + rd * t);
    if (d < 0.001 || t >= mx) break;
    t += d * 0.75;
  }
  return t;
}

vec3 normal(vec3 p) {
  vec2 h = vec2(0.001, 0.0);
  vec3 n = vec3(
    map(p + h.xyy) - map(p - h.xyy),
    map(p + h.yxy) - map(p - h.yxy),
    map(p + h.yyx) - map(p - h.yyx)
  );
  return normalize(n);
}

vec3 palette(float t, float i) {
  t *= ColorPulse;
  if (i < 1.0)
    return vec3(0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.67))));
  if (i < 2.0)
    return vec3(sin(t * 3.1), cos(t * 2.0), sin(t * 1.2));
  if (i < 3.0)
    return vec3(0.5 + 0.5 * sin(t + vec3(0.1, 0.3, 0.7)));
  if (i < 4.0)
    return vec3(abs(sin(t * 3.0)), abs(sin(t * 2.0 + 1.0)), abs(sin(t)));
  if (i < 5.0)
    return normalize(vec3(sin(t), sin(t + 2.0), sin(t + 4.0))) * 0.5 + 0.5;
  if (i < 6.0)
    return vec3(1.0 - exp(-2.0 * vec3(sin(t), cos(t), sin(t + 1.0))));
  return vec3(0.5 + 0.5 * sin(t * vec3(0.7, 0.5, 0.3)));
}

vec3 material(vec3 p) {
  float v = 0.0;
  float a = 0.7, f = 1.0;

  for (int i = 0; i < 4; i++) {
    float v1 = voronoi(p * f + 5.0);
    float v2 = 0.0;
    if (i > 0) {
      v2 = voronoi(p * f * 0.1 + 50.0 + 0.15 * TIME * Speed);
      float va = 1.0 - smoothstep(0.0, 0.1, v1);
      float vb = 1.0 - smoothstep(0.0, 0.08, v2);
      v += a * pow(va * (0.5 + vb), 4.0);
    }
    v1 = 1.0 - smoothstep(0.0, 0.3, v1);
    v2 = a * noise(v1 * 5.5 + 0.1);
    v += v2;
    f *= 3.0;
    a *= 0.5;
  }

  return palette(v * 2.0 + TIME * 0.3, PaletteIndex) * pow(v, 2.0);
}

mat3 camera(vec3 eye, vec3 lat) {
  vec3 ww = normalize(lat - eye);
  vec3 uu = normalize(cross(vec3(0, 1, 0), ww));
  vec3 vv = normalize(cross(ww, uu));
  return mat3(uu, vv, ww);
}

vec3 applyPost(vec3 c) {
  c = mix(vec3(0.5), c, Contrast);
  float l = dot(c, vec3(0.2126, 0.7152, 0.0722));
  c = mix(vec3(l), c, Saturation);
  return clamp(c * Brightness, 0.0, 1.0);
}

void main() {
  vec2 uv = -1.0 + 2.0 * (gl_FragCoord.xy / RENDERSIZE.xy);
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  vec3 col = vec3(0.0);

  float time = TIME * Speed;
  vec3 ro = vec3(0.63 * cos(time * 0.1), 0.67, time * 0.5 * Zoom);
  vec3 la = ro + vec3(0, 0, 0.3);
  ro.xy += path(ro.z);
  la.xy += path(la.z);

  vec3 rd = normalize(camera(ro, la) * vec3(uv, 1.97));
  float i = march(ro, rd, tmax);

  if (i < tmax) {
    vec3 pos = ro + rd * i;
    vec3 nor = normal(pos);
    vec3 rig = ro + vec3(0, 0, 3);
    rig.xy += path(rig.z);
    vec3 key = normalize(pos - rig);
    col = 0.1 * vec3(0, 0, 1);
    col += 0.9 * clamp(dot(key, nor), 0.0, 1.0);
    col += 0.4 * clamp(dot(-key, nor), 0.0, 1.0);
    col *= material(pos);
  }

  col = mix(col, vec3(0), 1.0 - exp(-0.6 * i));
  col = 1.0 - exp(-0.5 * col);
  col = pow(abs(col), vec3(1.0 / 2.2));
  col = applyPost(col);

  gl_FragColor = vec4(col, 1.0);
}
