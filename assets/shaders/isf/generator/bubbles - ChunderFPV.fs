/*{
  "DESCRIPTION": "Raymarching 3D grid spheres with glow, palette, pulse and ISF controls",
  "CATEGORIES": ["raymarch", "psychedelic", "grid"],
  "INPUTS": [
    { "NAME": "inputXY", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "glowPower", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "marchSteps", "TYPE": "float", "DEFAULT": 64.0, "MIN": 8.0, "MAX": 128.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define PI 3.14159265
#define TAU 6.2831853

mat2 rot(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

vec3 getPalette(float t, float p) {
  t = fract(t);
  if (p < 1.0) return sin(vec3(0.0, 1.047, 2.094) + t * TAU) * 0.5 + 0.5;
  if (p < 2.0) return vec3(0.5 + 0.5 * cos(t * TAU + vec3(0.0, 2.0, 4.0)));
  if (p < 3.0) return abs(sin(vec3(t * 2.0, t * 3.0, t * 4.0)));
  if (p < 4.0) return vec3(fract(t * 1.5), fract(t * 2.5), fract(t * 3.0));
  if (p < 5.0) return vec3(sin(t * 4.0), cos(t * 5.0), sin(t * 3.0)) * 0.5 + 0.5;
  if (p < 6.0) return vec3(sin(t * 6.0), cos(t * 6.3), sin(t * 2.0)) * 0.5 + 0.5;
  return vec3(fract(t), fract(t * 1.3), fract(t * 1.7));
}

vec3 postProcess(vec3 c) {
  float lum = dot(c, vec3(0.299, 0.587, 0.114));
  c = mix(vec3(lum), c, saturation);
  c = mix(vec3(0.5), c, contrast);
  c *= brightness;
  return clamp(c, 0.0, 1.0);
}

void main() {
  vec2 R = RENDERSIZE;
  vec2 uv = (gl_FragCoord.xy - 0.5 * R) / R.y;

  float t = TIME * speed;
  vec2 m = (inputXY - 0.5) * 4.0;
  if (inputXY == vec2(0.0)) {
    m = vec2(sin(t * 0.6) * 0.4, cos(t * 0.7) * 0.3);
  }

  mat2 pitch = rot(m.y * PI);
  mat2 yaw   = rot(m.x * PI);

  vec3 ro = vec3(0.5, 0.5, t);  // camera origin
  vec3 rd = normalize(vec3(uv * zoom, 1.0));
  rd.yz = pitch * rd.yz;
  rd.xz = yaw * rd.xz;

  float d = 0.0;
  float steps = marchSteps;
  vec3 col = vec3(0.0);

  for (float i = 0.0; i < 128.0; i++) {
    if (i >= steps) break;

    vec3 p = ro + rd * d;
    vec3 warped = p;
    warped.xz += floor(p.y + 0.5) * t * morph;

    vec3 cell = abs(fract(warped) - 0.5);
    float sdf = length(cell) - 0.2;
    if (sdf < 0.005) break;
    d += sdf * 0.8;
  }

  float distNorm = d / steps;
  float glow = exp(-distNorm * glowPower) * 0.6;

  vec3 paletteColor = getPalette(distNorm + colorPulse * t, palette);
  col += paletteColor * glow;
  col += vec3(1.0, 0.5, 0.2) * (0.02 + glow * 0.1);

  col = postProcess(col);
  gl_FragColor = vec4(col, 1.0);
}
