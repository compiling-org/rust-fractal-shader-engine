/*{
  "DESCRIPTION": "Circuit fractal on a KIFS structure with advanced psychedelic controls.",
  "CATEGORIES": ["Psychedelic", "Fractal", "3D"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.1, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 1.0, "MAX": 1.0, "LABEL": "Zoom" },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Fractal Morph Strength" },
    { "NAME": "twist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Twist Intensity" },
    { "NAME": "camMotion", "TYPE": "float", "DEFAULT": 7.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Camera Motion Intensity" },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.1, "MAX": 3.0, "LABEL": "Brightness" },
    { "NAME": "trailFade", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Fade Trail" },
    { "NAME": "patternFreq", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.01, "MAX": 2.0, "LABEL": "Pattern Frequency" },
    {
      "NAME": "colorPalette",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 0,
      "MAX": 8,
      "LABEL": "Color Palette",
      "VALUES": ["Classic", "Acid", "Lava", "Ice", "NeonDream", "BWDepth", "RainbowWave", "HyperColor", "PlasmaZone"]
    },
    { "NAME": "shapeType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Shape Morph Type" },
    { "NAME": "shakeIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Shake Intensity" }
  ]
}*/

float det = 0.001;
float t, boxhit;
vec3 adv, boxp;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float hash(vec2 p) {
  vec3 p3 = fract(vec3(p.xyx) * 0.1031);
  p3 += dot(p3, p3.yzx + 33.33);
  return fract((p3.x + p3.y) * p3.z);
}

vec3 path(float t) {
  vec3 p = vec3(vec2(sin(t * 0.1), cos(t * 0.05)) * 10.0, t);
  p.x += smoothstep(0.0, 0.5, abs(0.5 - fract(t * 0.02))) * 10.0;
  return p;
}

float fractal(vec2 p) {
  p = abs(5.0 - mod(p * 0.2, 10.0)) - 5.0;
  float ot = 1000.0;
  for (int i = 0; i < 7; i++) {
    p = abs(p) / clamp(p.x * p.y, 0.25, 2.0) - 1.0;
    ot = exp(-10.0 * ot * patternFreq);
    if (i > 0) ot = min(ot, abs(p.x) + 0.7 * fract(abs(p.y) * 0.05 + t * 0.05 + float(i) * 0.3));
  }
  return exp(-10.0 * ot);
}

float box(vec3 p, vec3 l) {
  vec3 c = abs(p) - l;
  return length(max(vec3(0.0), c)) + min(0.0, max(c.x, max(c.y, c.z)));
}

float de(vec3 p) {
  boxhit = 0.0;
  vec3 p2 = p - adv;
  p2.xz *= rot(t * 0.2 * twist);
  p2.xy *= rot(t * 0.1 * twist);
  p2.yz *= rot(t * 0.15 * twist);
  float b = box(p2, vec3(1.0 + 0.5 * sin(shapeType * 1.5))); // morph shape

  p.xy -= path(p.z).xy;
  float s = sign(p.y);
  p.y = -abs(p.y) - 3.0;
  p.z = mod(p.z, 20.0) - 10.0;

  for (int i = 0; i < 5; i++) {
    p = abs(p) - 1.0;
    p.xz *= rot(radians(s * -45.0 * morph));
    p.yz *= rot(radians(90.0));
  }

  float f = -box(p, vec3(5.0, 5.0, 10.0));
  float d = min(f, b);
  if (d == b) boxp = p2, boxhit = 1.0;
  return d * 0.7;
}

vec3 palette(float t, int mode) {
  if (mode == 1) return vec3(0.5 + 0.5 * sin(6.283 * t + vec3(0.0, 2.0, 4.0)));       // Acid
  if (mode == 2) return vec3(1.2, 0.3, 0.1) * sin(2.0 * 3.14159 * t);                 // Lava
  if (mode == 3) return vec3(0.2, 0.6, 1.0) * cos(2.5 * 3.14159 * t);                 // Ice
  if (mode == 4) return vec3(0.6 + 0.6 * sin(2.0 * t + vec3(2, 4, 6))) * vec3(1.2, 0.6, 1.5); // NeonDream
  if (mode == 5) return vec3(0.6 * exp(-6.0 * abs(t - 0.5)));                         // BWDepth (depth luminance)
  if (mode == 6) return 0.5 + 0.5 * sin(t * 10.0 + vec3(0, 2, 4));                    // RainbowWave
  if (mode == 7) return vec3(sin(t * 7.0), sin(t * 13.0 + 1.0), sin(t * 17.0 + 2.0)); // HyperColor
  if (mode == 8) return vec3(1.0) - abs(sin(t * 5.0 + vec3(0.3, 1.2, 2.5))) * vec3(1.5, 1.0, 0.5); // PlasmaZone
  return vec3(0.5 + 0.5 * cos(6.283 * t + vec3(0.0, 0.5, 1.0)));                      // Classic
}

vec3 march(vec3 from, vec3 dir) {
  vec3 p, g = vec3(0.0);
  float d, td = 0.0;

  for (int i = 0; i < 80; i++) {
    p = from + td * dir;
    d = de(p) * (1.0 - hash(gl_FragCoord.xy + t) * 0.3);
    g = mix(g, vec3(0.0), trailFade);
    if (d < det && boxhit < 0.5) break;
    td += max(det, abs(d));

    float f = fractal(p.xy) + fractal(p.xz) + fractal(p.yz);
    float b = fractal(boxp.xy) + fractal(boxp.xz) + fractal(boxp.yz);
    vec3 colf = palette(f * 0.5 + t * colorPulse, int(colorPalette)) * f;
    vec3 colb = palette(b + 0.1, int(colorPalette)) * 0.6;

    g += colf / (3.0 + d * d * 2.0) * exp(-0.0015 * td * td) * step(5.0, td) * (1.0 - boxhit);
    g += colb / (10.0 + d * d * 20.0) * boxhit * 0.5;
  }

  return g * brightness;
}

mat3 lookat(vec3 dir, vec3 up) {
  dir = normalize(dir);
  vec3 rt = normalize(cross(dir, normalize(up)));
  return mat3(rt, cross(rt, dir), dir);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
  uv += sin(TIME * 30.0 + uv.yx * 40.0) * 0.003 * shakeIntensity; // shake effect
  t = TIME * 7.0;

  vec3 from = path(t);
  adv = path(t + 6.0 + sin(t * 0.1) * camMotion);
  vec3 dir = normalize(vec3(uv, 0.7));
  dir = lookat(adv - from, vec3(0.0, 1.0, 0.0)) * dir;

  vec3 col = march(from / zoom, dir);
  gl_FragColor = vec4(col, 1.0);
}
