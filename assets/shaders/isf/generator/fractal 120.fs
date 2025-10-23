/*{
  "CATEGORIES": ["Fractal", "Raymarch", "Trippy"],
  "DESCRIPTION": "Psychedelic 3D fractal with full morphing, camera, and geometry control.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    
    { "NAME": "CameraX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "CameraY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "CameraZ", "TYPE": "float", "DEFAULT": 2.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "TargetX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "TargetY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "TargetZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },

    { "NAME": "FractalPower", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "AsinScale", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FoldBias", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "KaleidScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "IterationScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },

    { "NAME": "ShakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "GlitchAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "GlitchSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define R(p,a,t) mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a)
#define H(h) (cos((h)*6.3+vec3(0,23,21))*.5+.5)

vec3 palette(float t, float id) {
  int i = int(floor(id));
  if (i == 0) return H(t);
  if (i == 1) return vec3(sin(t*3.14), cos(t*2.1), sin(t*5.1));
  if (i == 2) return vec3(.6+.4*cos(6.28*t+0.0), .6+.4*cos(6.28*t+2.1), .6+.4*cos(6.28*t+4.2));
  if (i == 3) return vec3(.5+.5*sin(3.*t), .5+.5*sin(4.*t+2.), .5+.5*sin(5.*t+4.));
  if (i == 4) return vec3(.5+.5*cos(2.*t), .5+.5*cos(3.*t+1.), .5+.5*cos(4.*t+3.));
  if (i == 5) return vec3(sin(t*9.0)*.5+.5, cos(t*7.0)*.5+.5, sin(t*5.0)*.5+.5);
  if (i == 6) return vec3(fract(sin(t*10.)*43758.5453));
  return H(t);
}

mat3 lookAt(vec3 eye, vec3 target) {
  vec3 f = normalize(target - eye);
  vec3 r = normalize(cross(vec3(0,1,0), f));
  vec3 u = cross(f, r);
  return mat3(r, u, f);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  uv += XYControl;

  if (ShakeAmount > 0.0)
    uv += vec2(sin(TIME * 40.0), cos(TIME * 50.0)) * ShakeAmount * 0.01;

  float t = TIME * Speed;
  vec3 col = vec3(0.0);

  vec3 camPos = vec3(CameraX, CameraY, CameraZ);
  vec3 camTarget = vec3(TargetX, TargetY, TargetZ);
  vec3 ro = camPos;
  vec3 rd = normalize(lookAt(camPos, camTarget) * vec3(uv * Zoom, 1.0));

  float i = 0.0, s, e, g = 0.0;

  for (; i++ < 90.;) {
    vec3 p = g * rd;
    p = R(p, normalize(H(t * 0.1)), g * Morph * 0.1);
    p.z += t;
    p = abs(asin(AsinScale * sin(p)));
    s = FractalPower;
    vec4 q = vec4(p, 0.5);

    for (int j = 0; j++ < 7;) {
      s *= e = max(1.1 / dot(q, q), 1.2) * IterationScale;
      q = abs(q - FoldBias) * e - vec4(0.7, 1.2, 1.1, 1.2) * KaleidScale;
    }

    g += e = length(q.xz) * length(q.wy) / s;
    vec3 pal = palette(p.z * 0.8 * ColorPulse, Palette);
    col += mix(vec3(1.0), pal, 0.6) * 0.02 / exp(0.05 * i * i * e);
  }

  col *= col;

  if (GlitchAmount > 0.0) {
    float g = sin(TIME * GlitchSpeed + uv.y * 100.0) * GlitchAmount * 0.1;
    col.rg += g;
  }

  col = (col - 0.5) * Contrast + 0.5;
  float luma = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(luma), col, Saturation);
  col *= Brightness;
  col = clamp(col, 0.0, 1.0);

  gl_FragColor = vec4(col, 1.0);
}
