/*{
  "CATEGORIES": ["Fractal", "Raymarch", "Psychedelic"],
  "DESCRIPTION": "ISF shader conversion of animated fractal with full morphing, palette, camera, and glitch controls.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },

    { "NAME": "FoldX", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "FoldY", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "FoldZ", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 10.0 },

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

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  uv += XYControl;

  if (ShakeAmount > 0.0)
    uv += vec2(sin(TIME * 40.0), cos(TIME * 50.0)) * ShakeAmount * 0.01;

  float t = TIME * Speed;
  float b = fract(t * 2.0);
  vec3 col = vec3(0.0);
  vec3 r = vec3(RENDERSIZE, 1.0);
  vec3 d = normalize(vec3(uv * Zoom, 1.0));

  float i = 0.0, g = 0.0, e, s;

  for (; i++ < 99.;) {
    vec3 p = d * g;
    p.z += t * 2.0;
    p = R(p, vec3(0.577), clamp(sin(t / 4.0) * 6.0, -0.5, 0.5) + 0.7);
    p = asin(sin(p / 2.0)) * 3.0 * Morph;

    vec4 q = vec4(p, 0.5);
    s = 2.0;

    for (int j = 0; j++ < 8;) {
      q = abs(q);
      q = q.x < q.y ? q.zwxy : q.zwyx;
      s *= e = 10.0 / min(dot(q, q), 7.0);
      q = q * e - vec4(FoldX, FoldY - b, FoldZ - b, FoldX);
    }

    g += e = length(q.zw) / s;
    vec3 pal = palette(log(s) * 0.2 * ColorPulse, Palette);
    col += mix(vec3(1.0), pal, 0.8) * 0.0003 / e;
  }

  col *= col * col;

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
