/*{
  "DESCRIPTION": "3D fractal tunnel with full fractal + camera + color + FX control",
  "CATEGORIES": [ "Fractal", "Tunnel", "Psychedelic" ],
  "INPUTS": [
    { "NAME": "zoom",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.1,  "MAX": 5.0 },
    { "NAME": "speed",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.1,  "MAX": 5.0 },
    { "NAME": "morph",      "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "pulse",      "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "geometry",   "TYPE": "float", "DEFAULT": 1.7,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "iters",      "TYPE": "float", "DEFAULT": 6.0,  "MIN": 1.0,  "MAX": 10.0 },
    { "NAME": "depth",      "TYPE": "float", "DEFAULT": 150.0,"MIN": 10.0, "MAX": 300.0 },
    { "NAME": "camX",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZ",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "shake",      "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "glitch",     "TYPE": "float", "DEFAULT": 0.1,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "fractalScale","TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5,  "MAX": 5.0 },
    { "NAME": "fractalBias", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "fractalShift","TYPE": "float", "DEFAULT": 0.5, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "contrast",   "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "palette",    "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 9.0 }
  ]
}*/

vec3 getColorPalette(float id, float t) {
  t *= 6.2831;
  if(id < 0.5) return vec3(1.0, 0.0, 0.5);
  else if(id < 1.5) return vec3(0.2, 0.9, 0.1);
  else if(id < 2.5) return vec3(0.0, 0.7, 1.0);
  else if(id < 3.5) return vec3(1.0, 0.8, 0.0);
  else if(id < 4.5) return vec3(0.9, 0.2, 0.2);
  else if(id < 5.5) return vec3(0.6, 0.1, 1.0);
  else if(id < 6.5) return vec3(1.0, 0.4, 0.0);
  else if(id < 7.5) return 0.5 + 0.5*sin(t + vec3(0,2,4));
  else if(id < 8.5) return vec3(sin(t*3.0), sin(t*1.2), cos(t*2.5));
  return 0.5 + 0.5*sin(t + vec3(4,2,1));
}

vec3 sat(vec3 c, float s) {
  float l = dot(c, vec3(0.2126, 0.7152, 0.0722));
  return mix(vec3(l), c, s);
}

void main() {
  vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  float T = TIME * speed;
  vec3 ro = vec3(camX, camY, camZ);
  vec3 rd = normalize(vec3(uv * zoom, 1.0) - ro);

  rd += shake * vec3(
    sin(T * 23.1 + uv.y * 12.0),
    sin(T * 17.7 + uv.x * 13.0),
    sin(T * 19.3 + uv.x * uv.y * 30.0)
  ) * 0.01;

  vec4 o = vec4(0.0);
  float d = 0.0;
  vec3 lastQ = vec3(0.0);

  for (int j = 0; j < 300; j++) {
    if (float(j) > depth) break;

    vec3 p = d * rd;
    vec3 q = p;
    lastQ = q;

    p.x += fractalShift + sin(T * 0.1 + morph) * 0.1;
    p.z += T;

    float w = geometry;
    float l = 1.0;

    for (int i = 0; i < int(iters); i++) {
      p = cos(p - morph + fractalBias);
      l = fractalScale / dot(p, p);
      p *= l;
      w *= l;
    }

    float s = length(p) / w;
    d += s;
    o += vec4(d / s / 5e5);
  }

  float pulseMod = sin(T * 3.0 + dot(rd.xy, vec2(10.0))) * 0.5 + 0.5;
  vec3 col = getColorPalette(palette, T + length(rd.xy) * 2.0 + pulse * pulseMod);

  o.rgb *= col;
  o.rgb = tanh(o.rgb * abs(1.0 + cos(d + vec3(3,2,1)) / dot(cos(T + lastQ), vec3(1))) * exp(-d / 6.0));

  o.rgb = pow(o.rgb * brightness, vec3(contrast));
  o.rgb = sat(o.rgb, saturation);

  gl_FragColor = vec4(o.rgb, 1.0);
}
