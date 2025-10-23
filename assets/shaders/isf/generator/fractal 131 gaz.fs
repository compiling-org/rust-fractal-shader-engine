/*{
  "CATEGORIES": ["Fractal", "Raymarch", "ISF", "Trippy"],
  "DESCRIPTION": "ISF conversion of compact fractal raymarcher with full color/camera/morph controls.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "CameraZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
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
    uv += vec2(sin(TIME*40.), cos(TIME*50.)) * ShakeAmount * 0.01;

  float t = TIME * Speed;
  vec3 c = vec3(0.0);
  vec3 r = vec3(RENDERSIZE, 1.0);
  vec3 d = normalize(vec3(uv * Zoom, CameraZ));
  float i = 0.0, s, e, g = 0.0;

  for(; i++ < 90.;) {
    vec4 p = vec4(g*d, .07);
    p.z -= .5;
    p.xyz = R(p.xyz, normalize(H(t * .1)), t * Morph);
    s = 2.0;
    for(int j = 0; j++ < 6; )
      p = .02 - abs(p - .1),
      s *= e = max(1.0 / dot(p, p), 1.3),
      p = abs(p.x < p.y ? p.wzxy : p.wzyx) * e - 1.3;
    g += e = abs(length(p.xz * p.wy) - .02) / s + 1e-4;
    float lum = log(s) * 0.3;
    vec3 pal = palette(lum * ColorPulse, Palette);
    c += mix(vec3(1), pal, 0.4) * 0.04 / exp(i * i * e);
  }

  c *= c * c;

  if (GlitchAmount > 0.0) {
    float glitch = sin(TIME * GlitchSpeed + uv.y * 100.0) * GlitchAmount * 0.1;
    c.rg += glitch;
  }

  c = (c - 0.5) * Contrast + 0.5;
  float luma = dot(c, vec3(0.299, 0.587, 0.114));
  c = mix(vec3(luma), c, Saturation);
  c *= Brightness;
  c = clamp(c, 0.0, 1.0);

  gl_FragColor = vec4(c, 1.0);
}
