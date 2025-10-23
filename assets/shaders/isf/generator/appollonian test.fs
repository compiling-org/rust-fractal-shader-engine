/*{
  "CATEGORIES": [ "Fractal", "Psychedelic", "Raymarching" ],
  "DESCRIPTION": "Apollonian fractal with psychedelic palettes, full control, converted from Shadertoy",
  "INPUTS": [
    { "NAME": "Zoom",        "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed",       "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph",       "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ColorPulse",  "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "PulseSpeed",  "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "PaletteIndex","TYPE": "float",  "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "Brightness",  "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation",  "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast",    "TYPE": "float",  "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "XYControl",   "TYPE": "point2D","DEFAULT": [0.5, 0.5] }
  ]
}*/

#define EPS 0.001
#define MAX_ITER 100.0

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, -s, s, c);
}

vec3 palette(float t, float i) {
  t *= ColorPulse * PulseSpeed;
  if (i < 1.0) return vec3(0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.15, 0.25))) * vec3(1.0, 1.4, 2.5));
  if (i < 2.0) return vec3(sin(t * 7.0), sin(t * 2.0 + 1.0), sin(t * 3.0 + 2.0)) * 0.5 + 0.5;
  if (i < 3.0) return abs(vec3(sin(t * 4.0), cos(t * 6.0), sin(t * 9.0)));
  if (i < 4.0) return vec3(sin(t * 5.0 + sin(t)), cos(t * 2.1), sin(t * 1.7 + cos(t)));
  if (i < 5.0) return vec3(sin(t * 9.0), sin(t * 0.5 + 2.0), cos(t * 2.0 + 1.0)) * 0.5 + 0.5;
  if (i < 6.0) return 0.5 + 0.5 * sin(t * vec3(2.0, 4.0, 6.0) + vec3(0.0, 0.3, 0.7));
  return vec3(sin(t * 10.0 + sin(t * 0.5)), cos(t * 3.5 + cos(t)), sin(t * 5.0));
}

float apollonian(vec3 p, float t) {
  p.z += t * Speed;
  float i = 0.0, s = 1.0, k;
  for (; i < 6.0; i++) {
    p *= k = (1.5 + Morph * 0.5) / dot(p = mod(p - 1.0, 2.0) - 1.0, p);
    s *= k;
  }
  return length(p) / s - 0.01;
}

vec3 applyPost(vec3 col) {
  col = mix(vec3(0.5), col, Contrast);
  float luma = dot(col, vec3(0.2126, 0.7152, 0.0722));
  col = mix(vec3(luma), col, Saturation);
  return clamp(col * Brightness, 0.0, 1.0);
}

void main() {
  float t = TIME;

  vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5);
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  // Ray setup
  vec3 dir = normalize(vec3(uv * Zoom, 1.0));
  vec3 ori = vec3(1.0);

  // Apply XY control to rotate camera ray direction
  float yaw = (XYControl.x - 0.5) * 6.2831;
  float pitch = (XYControl.y - 0.5) * 3.1416;

  dir.xz = rot(yaw) * dir.xz;
  dir.yz = rot(pitch) * dir.yz;

  // Raymarching
  float iter = 0.0;
  for (; iter < MAX_ITER; iter++) {
    float d = apollonian(ori, t);
    ori += d * dir;
    if (d < EPS) break;
  }

  // Fake lighting
  float glow = apollonian(ori - dir * 0.01, t) * apollonian(ori - dir, t);
  float l = (glow * 40.0 - 2.0) / ori.z + 1.1;

  vec3 col = palette(t, PaletteIndex) * l;
  col = applyPost(col);
  gl_FragColor = vec4(col, 1.0);
}
