/*{
  "DESCRIPTION": "ISF-converted fractal points and grid shader with palettes, controls, and no GLSL ES errors.",
  "CATEGORIES": [ "Fractal", "VJ", "Psychedelic" ],
  "INPUTS": [
    { "NAME": "controlXY", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 15.0, "MIN": 1.0, "MAX": 40.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "overlap", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 30.0 },
    { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.14159265
#define TAU 6.2831853

vec3 hue(float a) {
  return cos(vec3(0.0, 1.0, 2.0) + a * TAU) * 0.5 + 0.5;
}

vec3 palette(float t, float i) {
  if (i < 1.0) return hue(t);
  if (i < 2.0) return sin(vec3(1.0, 2.1, 3.5) * t * 5.0 + vec3(0, 2, 4)) * 0.5 + 0.5;
  if (i < 3.0) return vec3(sin(t * 3.14), sin(t * 2.1), sin(t * 5.4)) * 0.5 + 0.5;
  if (i < 4.0) return vec3(cos(t * 1.5), sin(t * 2.5), cos(t * 0.8)) * 0.5 + 0.5;
  if (i < 5.0) return vec3(sin(t * 4.0 + 1.0), cos(t * 2.0), sin(t * 3.0)) * 0.5 + 0.5;
  if (i < 6.0) return vec3(fract(t * 2.5), fract(t * 3.3), fract(t * 1.7));
  return hue(t * 1.5 + sin(t * 0.5));
}

vec3 colorAdjust(vec3 col, float bri, float sat, float con) {
  col *= bri;
  float lum = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(lum), col, sat);
  col = mix(vec3(0.5), col, con);
  return clamp(col, 0.0, 1.0);
}

float fwidth_approx(vec2 u) {
  return length(dFdx(u)) + length(dFdy(u));
}

float round_compat(float x) {
  return floor(x + 0.5);
}

float grid(vec2 u, float t, float s) {
  float fw = fwidth_approx(u);
  fw = max(fw, 1e-6);
  vec2 l = max(vec2(0.0), 1.0 - abs(fract(u + 0.5) - 0.5) / fw / 1.5);
  vec2 g = 1.0 - abs(sin(PI * u));
  vec2 r = vec2(round_compat(u.x), round_compat(u.y));
  vec2 v = (l + g * 0.5) * max(vec2(0.0), 1.0 - abs(sin(PI * r * t)) * s);
  return max(v.x, v.y) / max(1.0, sqrt(abs(u.x)));
}

vec3 points(vec2 u, float l, float t, float r) {
  float fw = fwidth_approx(u);
  fw = max(fw, 1e-6);
  vec3 c = vec3(0.0);
  for (float i = 1.0; i <= 30.0; i++) {
    if (i > l) break;
    float px = round_compat((u.x - i) / l) * l + i;
    float f = sin(TAU * px * t);
    vec2 p1 = vec2(px, round_compat(u.y - f) + f);
    vec2 p2 = vec2(px, f);
    c = max(c, r / length((u - p1) / fw));
    c = max(c, r / length((u - p2) / fw) * 2.0);
  }
  return c / max(1.0, abs(u.x));
}

void main() {
  vec2 R = RENDERSIZE;
  vec2 U = gl_FragCoord.xy;
  vec2 uv = (U - 0.5 * R) / R.y;

  float s = zoom;
  float l = overlap;
  float t = TIME * speed / 60.0;

  vec2 m = (controlXY * 2.0 - 1.0) * 4.0;
  if (length(controlXY) < 0.01) {
    m = vec2(cos(t * TAU) * 2.0, sin(t * TAU * 2.0));
  }
  m *= s;

  vec3 u = vec3(uv, 1.0) * s;
  u.xy = (u.xy * u.xy + u.yx * u.yx + m.x) / (abs(u.xy) + 1e-5) + m;
  u /= (abs(u.yzx) + 1e-5);

  vec3 c = vec3(0.01);
  c += points(u.xz, l, t, R.y / 1000.0) * 0.8;
  c += points(u.yz, l, t, R.y / 1000.0) * 0.8;
  c += points(u.xy, l, t, R.y / 1000.0) * 0.8;
  c += points(u.zy, l, t, R.y / 1000.0) * 0.8;
  c += points(u.yx, l, t, R.y / 1000.0) * 0.8;
  c += points(u.zx, l, t, R.y / 1000.0) * 0.8;

  c += grid(u.xy, t, morph) * 0.1;
  c += grid(u.yz, t, morph) * 0.1;
  c += grid(u.zx, t, morph) * 0.1;

  c += palette(u.z + t * pulse, paletteIndex) * c;
  c = colorAdjust(c, brightness, saturation, contrast);

  gl_FragColor = vec4(c, 1.0);
}
