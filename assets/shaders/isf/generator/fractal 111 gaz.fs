/*{
  "CATEGORIES": ["Fractal", "Raymarch", "Psychedelic"],
  "DESCRIPTION": "ISF shader conversion of animated 4D fractal with morph, palette, camera, and glitch controls.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },

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

  float t = asin(sin(TIME * Speed / 60.0)) * 60.0;
  vec3 col = vec3(0.0);
  vec3 r = vec3(RENDERSIZE, 1.0);
  vec4 d = normalize(vec4(uv * Zoom, 1.0, 0.5));
  vec4 p;

  float i = 0.0, g = 0.0, e, s;

  for (; i++ < 80.;) {
    p = g * d;

    p.xyz = R(p.xyz, normalize(H(t * 0.07) * 2.0 - 1.0), g * 0.5 * Morph);
    p.xyz = R(p.xyz, vec3(0.577), clamp(sin(t * 0.5 + sin(t * 2.0) * 0.3) * 6.0, -1.0, -0.5) + 0.6);

    p += vec4(
      sin(t * 0.3 + sin(t * 0.5) * 0.2) * 0.03,
      sin(t * 0.2 + sin(t * 0.5) * 0.1) * 0.02,
      t * 0.3,
      0.0
    );

    p = asin(sin(abs(p) * (2.0 + sin(t * 0.5 + sin(t * 0.3) * 0.2) * 0.05)));

    s = 3.0;
    for (int j = 0; j++ < 6;) {
      p = p.x < p.y ? p.wzxy : p.wzyx;
      s *= e = max(1.0 / dot(p, p), 1.4);
      p = abs(p) * e - vec4(1.2, 1.5, 1.2, 0.8);
      p.yzw = R(p.yzw, normalize(H(t * 0.05)), t * 0.2);
    }

    g += e = length(p.xw) / s + 1e-4;
    vec3 pal = palette(log(s) * 0.3 + t * 0.3, Palette);
    col += mix(vec3(1.0), pal, 0.4) * 0.02 / exp(0.08 * i * i * e);
  }

  col *= col * col * col * col;

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
