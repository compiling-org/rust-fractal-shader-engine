/*{
  "DESCRIPTION": "Spiral grid shader with morphing, palettes, pulse, and deep VJ controls",
  "CATEGORIES": [ "Psychedelic", "Grid", "Fractal", "VJ" ],
  "INPUTS": [
    { "NAME": "controlXY",    "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] },
    { "NAME": "zoom",         "TYPE": "float",   "DEFAULT": 15.0, "MIN": 1.0, "MAX": 40.0 },
    { "NAME": "speed",        "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "pulse",        "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph",        "TYPE": "float",   "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "gridStrength", "TYPE": "float",   "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "warpScale",    "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "gridLayers",   "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "hueShift",     "TYPE": "float",   "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "palette",      "TYPE": "float",   "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "brightness",   "TYPE": "float",   "DEFAULT": 0.20, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast",     "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "saturation",   "TYPE": "float",   "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define TAU 6.2831853
#define PI 3.1415926

mat2 rot(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

vec3 paletteFunc(float t, float i) {
  t = fract(t);
  if (i < 1.0) return sin(vec3(0.0, 1.047, 2.094) + t * TAU) * 0.5 + 0.5;
  if (i < 2.0) return cos(vec3(t, t * 2.0, t * 1.5)) * 0.5 + 0.5;
  if (i < 3.0) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0));
  if (i < 4.0) return abs(sin(vec3(t * 4.0 + 1.0, t * 2.0, t * 3.0)));
  if (i < 5.0) return vec3(t * t, sin(t * 2.1), abs(cos(t * 3.2))) * 0.6 + 0.2;
  if (i < 6.0) return vec3(0.5 + 0.5 * sin(t * 8.0), 0.5 + 0.5 * cos(t * 5.0), sin(t * 3.0) * 0.5 + 0.5);
  return vec3(sin(t * 1.5), sin(t * 2.1 + 1.0), cos(t * 2.8)) * 0.5 + 0.5;
}

vec3 adjustColor(vec3 c, float bri, float sat, float con) {
  c *= bri;
  float luma = dot(c, vec3(0.299, 0.587, 0.114));
  c = mix(vec3(luma), c, sat);
  c = mix(vec3(0.5), c, con);
  return clamp(c, 0.0, 1.0);
}

float softGrid(float x, float t, float strength) {
  float fx = abs(fract(x - t + 0.5) - 0.5);
  float line = smoothstep(0.01, 0.02, 0.5 - fx);  // soft line
  float glow = 1.0 - abs(sin((x - t) * PI));
  float darken = min(1.0, 1.0 / max(abs(x), 1e-3));
  return (line + glow * 0.3 + 0.1) * pow(darken, strength);
}

vec3 gridColor(float x, float t, float hueOffset, float strength, float paletteID) {
  return paletteFunc(x - t + hueOffset, paletteID) * softGrid(x, t, strength);
}

void main() {
  vec2 R = RENDERSIZE;
  vec2 uv = (gl_FragCoord.xy - 0.5 * R) / R.y;

  float t = TIME * speed;
  float hue = hueShift + sin(t * pulse) * 0.2;
  vec2 m = (controlXY * 4.0 - 2.0);

  if (length(controlXY - 0.5) < 0.01) {
    m = vec2(sin(t * 0.5) * 0.6, sin(t) * 0.4);
  }

  mat2 pitch = rot(m.y * PI * 0.5);
  mat2 yaw   = rot(m.x * PI * 0.5);

  vec3 col = vec3(0.0);
  for (float i = 0.1; i <= gridLayers; i += 0.1) {
    float d = i * warpScale;
    vec3 u = normalize(vec3(uv, 0.6 * sqrt(i))) * zoom;
    u.yz = pitch * u.yz;
    u.xz = yaw * u.xz;

    vec3 g = sin(u * d * TAU);
    g = vec3(g.x * g.y * g.z);

    u = mix(u, g, morph);  // warp based on morph strength

    vec3 v = max(max(gridColor(u.x, t, hue, gridStrength, palette),
                     gridColor(u.y, t, hue, gridStrength, palette)),
                 gridColor(u.z, t, hue, gridStrength, palette));

    col += v * (1.0 - i);
  }

  col *= col * 0.3; // bloom-ish glow
  col = adjustColor(col, brightness, saturation, contrast);

  gl_FragColor = vec4(col, 1.0);
}
