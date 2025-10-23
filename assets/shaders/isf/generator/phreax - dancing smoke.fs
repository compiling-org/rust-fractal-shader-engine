/*{
  "CATEGORIES": [ "Fractal", "Raymarch", "Trippy", "Converted" ],
  "DESCRIPTION": "Fully featured raymarching fractal with alpha blending, palette, camera, glitch, shake, and color controls.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FractalTwist", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "TorusSize", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "TorusKnotScale", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "KIFSDepth", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "CurveSharpness", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-2.0, -2.0], "MAX": [2.0, 2.0] },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "ShakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "GlitchAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "GlitchSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "CameraX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "CameraY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "CameraZ", "TYPE": "float", "DEFAULT": -8.0, "MIN": -20.0, "MAX": 0.0 },
    { "NAME": "TargetX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "TargetY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "TargetZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "BackgroundColor", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "Transparency", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

#define PI 3.14159265359

mat2 rot2(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, s, -s, c);
}

float rnd(float t) {
  return fract(sin(t * 784.685) * 827.542);
}

float curve(float t, float d) {
  float g = t / d;
  return mix(rnd(floor(g)), rnd(floor(g) + 1.0), pow(smoothstep(0.0, 1.0, fract(g)), CurveSharpness));
}

vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
  return a + b * cos(6.28318 * (c * t + d));
}

vec3 getPal(float id, float t) {
  int i = int(floor(id));
  vec3 col = pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.0, -0.33, 0.33));
  if (i == 1) col = pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.0, 0.10, 0.20));
  else if (i == 2) col = pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.3, 0.20, 0.20));
  else if (i == 3) col = pal(t, vec3(.5), vec3(.5), vec3(1.0,1.0,0.5), vec3(0.8, 0.90, 0.30));
  else if (i == 4) col = pal(t, vec3(.5), vec3(.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
  else if (i == 5) col = pal(t, vec3(.5), vec3(.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
  else if (i == 6) col = pal(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0,1.0,1.0), vec3(0.0, 0.25, 0.25));
  return col;
}

vec3 transform(vec3 p, float t) {
  float a = sin(2. * PI * curve(t, 4.));
  float a2 = sin(2. * PI * curve(t + 5., 4.));
  p.xz *= rot2(a);
  p.xy *= rot2(a2);
  return p;
}

float map(vec3 p, float t, vec3 ro) {
  vec3 bp = p;
  p = transform(p, t);

  p.x = abs(p.x) - .5 * (sin(t * .5) * .5 + .5);
  p.y = abs(p.y) - .9 * (sin(t * .8) * .5 + .5) - 0.1;
  p.y = abs(p.y) - .1;
  p.x = abs(p.x - 0.2) - .9;
  p.z = abs(p.z) - .5;

  p.zy -= 0.5;
  p.xy *= rot2(FractalTwist * t);
  p.zy *= rot2(-.04 * t);

  float r1 = TorusSize;
  float r2 = mix(0.02, 0.2, (sin(t * .3) * .5 + .5));
  vec2 cp = vec2(length(p.xz) - r1, p.y);

  float a = atan(p.z, p.x);
  cp *= rot2(3. * a + t);
  cp *= vec2(3., .4);
  cp.x = abs(cp.x) - TorusKnotScale;
  cp *= rot2(2. * a);

  for (float i = 0.; i < KIFSDepth; i++) {
    cp.y = abs(cp.y) - 0.05 * (0.5 * sin(t) + .9);
    cp *= rot2(0.1 * a * sin(0.1 * t));
    cp -= i * 0.01 / KIFSDepth;
  }

  float d = length(cp) - r2;
  return max(.09 * d, -length(bp.xy - ro.xy) - 4.);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  uv += XYControl;

  if (ShakeAmount > 0.0) {
    uv += vec2(sin(TIME * 40.0), cos(TIME * 50.0)) * ShakeAmount * 0.01;
  }

  float t = TIME * Speed;
  float tt = t * 5. * (1. + .03 * sin(curve(t, 2.))) + 2. * curve(t, 4.);
  float cz = CameraZ + Morph * (4. * (sin(2. * PI * curve(t, 3.)) * .5 + .5));
  vec3 ro = vec3(CameraX, CameraY, cz);
  vec3 lp = vec3(TargetX, TargetY + 1.0, TargetZ);
  vec3 target = vec3(TargetX, TargetY, TargetZ);
  vec3 f = normalize(target - ro);
  vec3 r = normalize(cross(vec3(0,1,0), f));
  vec3 u = cross(f, r);
  vec3 rd = normalize(uv.x * r + uv.y * u + f * Zoom);

  vec3 p = ro;
  vec3 col = vec3(0.0);
  float d = 0.1, acc = 0.001, totalD = 0.0;

  for (int i = 0; i < 200; i++) {
    d = map(p, tt, ro);
    if (totalD > 150.0) break;
    acc += 0.01 + d * 0.4;
    d = max(abs(d), 0.00002);
    totalD += d;
    p += rd * d;
    col += 2.0 * clamp(1.0, 0.0, .7 * abs(cz) / (acc * acc));
    float sl = dot(p, p);
    float pulse = 1. - 0.15 * sqrt(sl) + 0.2 * p.z + mix(3.0, 4.0, sin(curve(t, 4.)) * 0.5 + 0.5);
    col *= 0.5 * getPal(Palette, pulse * ColorPulse);
    col = clamp(vec3(.4), vec3(0.0), col);
  }

  if (d < 0.001) {
    vec2 e = vec2(0.0035, -0.0035);
    vec3 n = normalize(
      e.xyy * map(p + e.xyy, tt, ro) +
      e.yyx * map(p + e.yyx, tt, ro) +
      e.yxy * map(p + e.yxy, tt, ro) +
      e.xxx * map(p + e.xxx, tt, ro)
    );
    vec3 l = normalize(lp - p);
    float dif = max(dot(n, l), 0.0);
    float spe = pow(max(dot(reflect(-rd, n), -l), 0.0), 40.0);
    float sss = smoothstep(0.0, 1.0, map(p + l * 0.4, tt, ro)) / 0.4;
    col *= mix(1.0, 0.4 * spe + 0.8 * (dif + 2.5 * sss), 0.4);
    col = clamp(vec3(1.0), vec3(0.0), col);
  }

  // Apply color grading
  col = (col - 0.5) * Contrast + 0.5;
  float luma = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(luma), col, Saturation);
  col *= Brightness;
  col = clamp(col, 0.0, 1.0);

  if (GlitchAmount > 0.0) {
    float offset = sin(TIME * GlitchSpeed + uv.y * 100.0) * GlitchAmount * 0.1;
    col.rg += offset;
  }

  // Alpha blending with background
  col = mix(BackgroundColor.rgb, col, Transparency);
  gl_FragColor = vec4(col, Transparency);
}
