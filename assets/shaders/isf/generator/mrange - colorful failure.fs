/*{
  "DESCRIPTION": "Colorful raymarched spiral with morphing symmetry and psychedelic color pulses.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Psychedelic", "Raymarch"],
  "INPUTS": [
    { "NAME": "LayerCount", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "SliceHeight", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5 },
    { "NAME": "Twist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "MorphSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "TubeSharpness", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "YLimit", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0 }
  ]
}*/

vec4 tanh(vec4 x) {
  return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

void main() {
  vec2 C = gl_FragCoord.xy;
  vec2 R = RENDERSIZE.xy;
  float t = TIME * MorphSpeed;
  float s = 0.0;
  float r = 0.0;
  float d, q, l, h = SliceHeight;

  vec4 c = vec4(0.0);
  vec4 p, x;

  for (int i = 0; i < 77; i++) {
    p = r * normalize(vec3(C - 0.5 * R, R.y)).xyzx;
    p.z += t;
    q = floor(p.z / h + 0.5) * h;
    d = 3.0;

    for (int j = 0; j < 10; j++) {
      if (float(j) > LayerCount) break;
      l = float(j) - LayerCount * 0.5;

      x = p;
      x.z -= q;
      float twist = Twist * (q + h * l);
      x.xy *= mat2(cos(twist), -sin(twist), sin(twist), cos(twist));
      x.x = abs(x.x);

      float tube = length(x.xz - vec2(1.0 - 0.5 * sin(0.6 * (q + h * l) + x.y), h * l)) - TubeSharpness;
      float limit = abs(x.y) - YLimit;
      d = min(d, max(tube, limit));
    }

    d = abs(d) + 0.002;
    p = 1.0 + sin(2.0 * length(p.xy) + p.z + vec4(0, 1, 2, 0) - t * ColorPulse);
    c += p.w / d * p;

    s += 1.0;
    r += 0.5 * d;
  }

  gl_FragColor = tanh(c / 1e4);
}
