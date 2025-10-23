/*{
  "DESCRIPTION": "Noisy Twisty Tunnel - Psychedelic upgraded version",
  "CATEGORIES": ["Psychedelic", "Tunnel", "Trippy"],
  "INPUTS": [
    { "NAME": "speed",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph",         "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "pulseIntensity","TYPE": "float", "DEFAULT": 1.3, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "twistAmount",   "TYPE": "float", "DEFAULT": 0.70, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "paletteType",   "TYPE": "float", "DEFAULT": 4.80, "MIN": 0.0, "MAX": 9.0 },
    { "NAME": "zoom",          "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "pulseSpeed",    "TYPE": "float", "DEFAULT": 4.2, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "pulseDepth",    "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "vortexAmount",  "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "radialWarp",    "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "kaleidoscope",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "brightness",    "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation",    "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.28318530718
#define R(a) mat2(cos(a+vec4(0,33,11,0)))

float hash(vec2 p) {
  return fract(sin(dot(p, vec2(41.2, 289.8))) * 43758.5453);
}

float N(vec3 x, float s, float n) {
  return abs(dot(cos(x * n), vec3(s))) / n;
}

vec3 tanhVec(vec3 x) {
  return (exp(x * 2.0) - 1.0) / (exp(x * 2.0) + 1.0);
}

vec3 getPalette(float t, vec3 p, int type) {
  if (type == 0) return 0.5 + 0.5 * cos(PI * vec3(0.3, 0.6, 0.9) + t + p.xyx);
  if (type == 1) return vec3(sin(p.z + t), sin(p.y + 0.5 * t), sin(p.x + 0.25 * t)) * 0.5 + 0.5;
  if (type == 2) return pow(abs(sin(p * 5.0 + t)), vec3(0.9, 0.6, 0.3));
  if (type == 3) return fract(p * 2.0 + t);
  if (type == 4) return vec3(abs(sin(t + p.x * 2.)), abs(cos(t + p.y * 1.5)), abs(sin(p.z * 3.)));
  if (type == 5) return vec3(1.0 - abs(sin(t + p.xyx * 3.14)));
  if (type == 6) return 0.5 + 0.5 * sin(PI * vec3(0.8, 0.3, 0.2) + t + p.yyx);
  if (type == 7) return pow(fract(p * 3.0 + t), vec3(0.7, 0.8, 0.5));
  if (type == 8) return vec3(0.5) + 0.5 * sin(p * 7.0 + t * vec3(0.6, 1.2, 0.3));
  return vec3(1.0);
}

void main() {
  float T = TIME * speed;
  float ft = fract(T);
  float t = floor(T) + sqrt(ft);
  float d = 0.2 * hash(gl_FragCoord.xy);
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

  float angle = atan(uv.y, uv.x);
  float radius = length(uv);
  float seg = TAU / kaleidoscope;
  angle = mod(angle, seg) - 0.5 * seg;
  uv = vec2(cos(angle), sin(angle)) * radius;

  uv *= zoom;
  uv *= 1.0 + vortexAmount * radius;

  vec3 w, p;
  gl_FragColor = vec4(0.0);
  float s = 0.002;
  float n;

  for (int i = 0; i < 64; ++i) {
    if (s < 0.001) break;

    d += s = 0.05 + 0.8 * abs(min(s, 4.0 - abs(w.x)));
    vec3 col = getPalette(t * pulseSpeed + pulseDepth * w.z + p.z * radialWarp, p, int(paletteType));
    gl_FragColor.rgb += pulseIntensity * col / (s / d);

    p = w = vec3(uv * d, d);
    w.xy *= R(t + w.z * twistAmount * 0.2);
    p.xy *= R(t + p.z * twistAmount * 0.5);
    n = 1.5;
    s = 5.0 - length(p.xy);

    for (int j = 0; j < 10; ++j) {
      if (n >= 16.0) break;
      s += N(3.0 * t + p, morph, n);
      w += N(6.0 * t + w, morph * 0.66, n);
      n += n;
    }
  }

  float q = pow(ft, 0.125);
  float fade = 6e1 * length(gl_FragCoord.xy) / q / length(cos(4.0 / ft * T * gl_FragCoord.xy / log((1.0 + cos(t) * 16.0 + 32.0) / d)));
  gl_FragColor.rgb = tanhVec(gl_FragColor.rgb / fade);

  float avg = dot(gl_FragColor.rgb, vec3(0.333));
  gl_FragColor.rgb = mix(vec3(avg), gl_FragColor.rgb, saturation);
  gl_FragColor.rgb = (gl_FragColor.rgb - 0.5) * contrast + 0.5;
  gl_FragColor.rgb *= brightness;

  gl_FragColor.a = 1.0;
}