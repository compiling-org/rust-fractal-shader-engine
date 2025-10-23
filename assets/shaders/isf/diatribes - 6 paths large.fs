/*{
  "DESCRIPTION": "Psychedelic tunnel + plasma raymarch with control over shape, shake, palettes, shimmer, contrast, etc.",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "Zoom",         "TYPE": "float", "MIN": 0.2, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "Speed",        "TYPE": "float", "MIN": 0.5, "MAX": 20.0, "DEFAULT": 6.0 },
    { "NAME": "TunnelRadius", "TYPE": "float", "MIN": 0.0, "MAX": 12.0, "DEFAULT": 6.5 },
    { "NAME": "PatternFreq",  "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "Shake",        "TYPE": "float", "MIN": 0.0, "MAX": 0.10, "DEFAULT": 0.015 },
    { "NAME": "Shimmer",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0 },
    { "NAME": "Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "Saturation",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Contrast",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Gain",         "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.5 },
    { "NAME": "ColorPulse",   "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 3.0 },
    { "NAME": "Palette",      "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 3.0 }
  ]
}*/

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define N normalize

vec3 P1(float z) {
  return vec3(
    tanh(cos(z * 0.04) * 0.3) * 24.1,
    tanh(cos(z * 0.055) * 0.6) * 26.5,
    z
  );
}

vec3 P2(float z) {
  return vec3(
    tanh(cos(z * 0.16) * 0.23) * 14.1,
    tanh(cos(z * 0.061) * 0.7) * 14.5,
    z
  );
}

vec3 plasma(vec3 p) {
  float t = p.z;
  p *= 4.0;
  float r = distance(p.xz, vec2(sin(t), sin(t)));
  float g = distance(p.xz, vec2(0, 3.0));
  float b = distance(p.xz, vec2(r, sin(t) * 25.0));
  float v = abs(sin(r + t) + sin(g + t) + sin(b + t) + sin(p.x + t) + cos(p.y + t));
  return vec3(r, g, b) / v * 0.125;
}

vec3 getPalette(int i, float t) {
  if (i == 0) return vec3(sin(t*6.0), sin(t*4.0+1.0), sin(t*2.0+2.0));
  if (i == 1) return vec3(abs(cos(t*3.0)), abs(sin(t*5.0+1.0)), sin(t*7.0+2.0));
  if (i == 2) return vec3(0.5+0.5*sin(t), 0.5+0.5*cos(t*1.5), 0.5+0.5*sin(t*0.5));
  if (i == 3) return vec3(fract(t), fract(t*2.0), fract(t*3.0));
  if (i == 4) return vec3(sin(t*2.1)*0.7+0.3, cos(t*1.5)*0.7+0.3, sin(t*3.7)*0.7+0.3);
  if (i == 5) return vec3(abs(sin(t*4.3)), abs(cos(t*3.1)), abs(sin(t*2.7)));
  if (i == 6) return vec3(0.9, 0.3 + 0.7*sin(t*2.0), 0.6 + 0.4*cos(t*3.0));
  return vec3(1.0);
}

void main() {
  vec2 u = isf_FragNormCoord * RENDERSIZE;
  vec2 r = RENDERSIZE;
  float T = TIME * Speed;
  u = (u - r * 0.5) / r.x;

  vec3 ro = P1(T);
  vec3 la = P1(T + 5.0);
  vec3 Z = normalize(la - ro);
  vec3 X = normalize(cross(Z, vec3(0.0, -1.0, 0.0)));
  vec3 Y = cross(X, Z);

  vec2 screenUv = rot(sin(T * 0.05)) * u;
  vec3 rd = vec3(screenUv, 1.0 / Zoom) * mat3(-X, Y, Z) * 0.75;

  rd += vec3(sin(T * 3.0 + u.y * 20.0), cos(T * 4.0 + u.x * 15.0), sin(T * 2.0)) * Shake * 0.15;

  float d = 0.0, s = 0.0, s1, s2;
  vec3 p;
  for (int i = 0; i < 120; i++) {
    p = ro + rd * d;

    vec3 p1 = P1(p.z);
    vec3 p2 = P2(p.z);

    s1 = min(length(p.xy - vec2(p1.x + 1.0, p1.y)),
             min(length(p.xy - p1.xy),
                 length(p.xy - vec2(p1.x, p1.y + 1.0))));

    s2 = min(length(p.xy - vec2(p2.x + 1.0, p2.y)),
             min(length(p.xy - p2.xy),
                 length(p.xy - vec2(p2.x, p2.y + 1.0))));

    s = TunnelRadius - min(s1, s2);
    s += Shimmer * dot(sin(p * PatternFreq), vec3(0.3)) * 0.2;

    d += s;
    if (d > 150.0 || s < 0.01) break;
  }

  p = ro + rd * d;
  vec3 color = plasma(sin(0.1 * p + T * 0.05)) * plasma(sin(p)) / sqrt(d);

  float pulse = sin((p.y + T) * 10.0 + ColorPulse) * 0.5 + 0.5;
  color *= getPalette(int(Palette), d * 0.05 + pulse * Shimmer);

  color = pow(color * Gain, vec3(0.45));
  float lum = dot(color, vec3(0.2126, 0.7152, 0.0722));
  color = mix(vec3(lum), color, Saturation);
  color = (color - 0.5) * Contrast + 0.5;
  color *= Brightness;

  gl_FragColor = vec4(color, 1.0);
}
