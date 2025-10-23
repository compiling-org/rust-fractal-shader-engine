/*{
  "DESCRIPTION": "Fractal tunnel morphing with psychedelic palette",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "Zoom",         "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "Speed",        "TYPE": "float", "MIN": 0.01, "MAX": 4.0, "DEFAULT": 1.0 },
    { "NAME": "Morph",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "TunnelTwist",  "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "TunnelFreq",   "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 10.0 },
    { "NAME": "FractalWarp",  "TYPE": "float", "MIN": 0.0, "MAX": 5.0,  "DEFAULT": 1.0 },
    { "NAME": "FractalGain",  "TYPE": "float", "MIN": 0.1, "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "Shimmer",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Shake",        "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.05 },
    { "NAME": "Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Saturation",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Contrast",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Palette",      "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 0.0 },
    { "NAME": "ColorPulse",   "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.0 },
    { "NAME": "CamTheta",     "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.0 },
    { "NAME": "CamPhi",       "TYPE": "float", "MIN": -1.57, "MAX": 1.57, "DEFAULT": 0.0 },
    { "NAME": "CamDist",      "TYPE": "float", "MIN": 0.5, "MAX": 10.0, "DEFAULT": 4.0 }
  ]
}*/

mat3 lookAt(vec3 ro, vec3 ta) {
  vec3 f = normalize(ta - ro);
  vec3 r = normalize(cross(vec3(0.0, 1.0, 0.0), f));
  vec3 u = cross(f, r);
  return mat3(r, u, f);
}

vec3 getPalette(int i, float t) {
  if (i == 0) return vec3(sin(t*6.0), sin(t*4.0+1.0), sin(t*2.0+2.0));
  if (i == 1) return vec3(sin(t*3.0)*0.5+0.5, sin(t*5.0+1.0)*0.5+0.5, sin(t*7.0+2.0)*0.5+0.5);
  if (i == 2) return vec3(0.5+0.5*sin(t), 0.5+0.3*cos(t*1.5), 0.5+0.5*sin(t*0.5));
  if (i == 3) return vec3(1.0, 0.2+0.8*sin(t*2.0), 0.5+0.5*cos(t*4.0));
  if (i == 4) return vec3(1.0, t, t*t);
  if (i == 5) return vec3(0.0, t*t, 1.0);
  if (i == 6) return vec3(sin(t*1.3)*0.5+0.5, sin(t*1.7+2.0)*0.5+0.5, sin(t*2.3+4.0)*0.5+0.5);
  return vec3(1.0);
}

vec3 fieldFunc(float z) {
  float twist = TunnelTwist * sin(z * TunnelFreq + TIME);
  return vec3(
    tanh(cos(z * 0.3 + twist) * 0.3) * 8.0,
    tanh(cos(z / 4.0 - twist) * 0.26) * 8.0,
    z
  );
}

void main() {
  vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  float t = TIME * Speed;

  float d = 0.0, i = 0.0, j = 0.0;
  float s = 0.002;

  vec3 cam = vec3(
    CamDist * cos(CamPhi) * sin(CamTheta),
    CamDist * sin(CamPhi),
    CamDist * cos(CamPhi) * cos(CamTheta)
  );
  vec3 target = vec3(0.0);
  vec3 D = normalize(vec3(uv, 1.0 / Zoom)) * lookAt(cam, target);

  vec3 p = fieldFunc(t);
  vec3 Z = normalize(fieldFunc(t + 1.5) - p);
  vec3 X = normalize(vec3(Z.z, 0.0, -Z.x));
  D *= mat3(-X, cross(X, Z), Z);

  for (; i++ < 80.0 && s > 0.001; d += s)
  {
    s = 1.5 - length(p.xy - fieldFunc(p.z).xy);
    p += s * D * 0.8;
  }

  for (p = cos(p); j++ < 7.0;
       p = abs(p) / clamp(abs(dot(p, vec3(0.75))), 0.125, 0.7) - 1.75 * Morph);

  p = p * FractalWarp + sin(p * FractalGain);

  vec3 col = tanh(4.0 * p / exp(d / 3.0));

  float shakeAmt = sin(t * 10.0) * Shake;
  uv += vec2(sin(t * 5.0 + uv.y * 20.0), cos(t * 6.0 + uv.x * 15.0)) * shakeAmt;

  float pulse = sin((uv.y + t) * 10.0 + ColorPulse) * 0.5 + 0.5;
  vec3 paletteCol = getPalette(int(Palette), d * 0.1 + pulse * Shimmer);
  col *= paletteCol;

  col = mix(vec3(dot(col, vec3(0.2126, 0.7152, 0.0722))), col, Saturation);
  col = (col - 0.5) * Contrast + 0.5;
  col *= Brightness;

  gl_FragColor = vec4(col, 1.0);
}
