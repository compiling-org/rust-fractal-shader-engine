/*{
  "DESCRIPTION": "Faithful ISF port of neuron + Droste 'Ordered Chaos' fractal. Fully wired controls.",
  "CATEGORIES": ["Fractal", "VFX"],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 6.999 },
    { "NAME": "colorPulseStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 }
  ],
  "PASSES": [
    { "TARGET": "BufferA", "PERSISTENT": true },
    {}
  ]
}*/

#define TAU 6.28318530718
#define R1 0.15
#define R2 1.0

mat2 rot(float a) {
  return mat2(cos(a), sin(a), -sin(a), cos(a));
}

vec3 getPalette(float t, float pid) {
  int i = int(pid);
  if (i == 0) return vec3(0.5 + 0.5 * cos(TAU * (t + vec3(0., 0.33, 0.67))));
  if (i == 1) return vec3(sin(3.0*t), sin(2.0*t + 1.0), sin(1.0*t + 2.0)) * 0.5 + 0.5;
  if (i == 2) return vec3(t, t*t, sin(t * 3.14159));
  if (i == 3) return vec3(cos(t*2.5+1.5), cos(t*1.5+2.5), cos(t*3.5+0.5)) * 0.5 + 0.5;
  if (i == 4) return vec3(sin(6.3*t), sin(6.3*t+2.1), sin(6.3*t+4.1)) * 0.5 + 0.5;
  if (i == 5) return vec3(0.3 + 0.7 * sin(t * 6.0), 0.3 + 0.7 * sin(t * 4.0), 0.3 + 0.7 * sin(t * 2.0));
  if (i == 6) return vec3(0.6 + 0.4 * sin(t*4.0 + vec3(0.0, 1.0, 2.0)));
  return vec3(t);
}

vec2 drosteSpiral(vec2 uv, float zoomExp) {
  vec2 z = vec2(length(uv), atan(uv.y, uv.x));
  z.x = log(z.x / R1);
  float ratio = log(R2 / R1);
  float alpha = atan(ratio, TAU);
  mat2 m = mat2(cos(alpha), sin(alpha), -sin(alpha), cos(alpha));
  vec2 beta = m * z / cos(alpha);
  beta.x = mod(beta.x, log(R2 / R1));
  return R1 * exp(beta.x) * vec2(cos(beta.y), sin(beta.y));
}

float neurons(vec2 p, float t) {
  vec2 n = vec2(0), q;
  float S = 4.0 + morph * 3.0;
  mat2 m = rot(1.0 - 0.0001 + morph * 0.01);
  for (float j = 0.; j++ < 28.;) {
    p *= m; n *= m;
    q = p * S + j + n + t;
    n += sin(q * 0.5);
    S *= 1.12;
  }
  return pow(dot(n, vec2(1.)) / S + 0.01, 2.2);
}

float d2(vec2 v, float k) {
  return pow(pow(abs(v.x), k) + pow(abs(v.y), k), 1. / k);
}

vec3 fractalNeuron(vec2 uv, float t, float nVal, float layerFactor) {
  vec2 uv0 = uv;
  vec3 col = vec3(0);
  float pulse = sin(t * colorPulseSpeed) * colorPulseStrength;

  for (float i = 0.; i < 6.; i++) {
    uv = fract(uv * 1.9) - 0.5;
    vec3 c = getPalette(length(uv0) + t * 0.4 + i * 0.9 + pulse, palette);
    float d = d2(uv, 1.2) + 0.05 / nVal + 0.2 * exp(-length(uv0));
    d = abs(0.1*d + sin(d*(8.0+2.0*nVal)) / 10.0) + 0.001 / nVal;
    d = layerFactor * 0.1 * pow(0.02 / d, 1.2) + smoothstep(0.05, 0.0, d);
    col += max(0.0, d) * c;
  }
  return col;
}

vec3 postColor(vec3 c) {
  float l = dot(c, vec3(0.299, 0.587, 0.114));
  c = mix(c, vec3(l), 1.0 - saturation);
  c = (c - 0.5) * contrast + 0.5;
  c *= brightness;
  return clamp(c, 0.0, 1.0);
}

void main() {
  float t = TIME * speed;

  if (PASSINDEX == 0) {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    uv += shake * 0.03 * vec2(sin(t * 3.0), cos(t * 2.0));

    float expZoom = exp(-mod(t * 0.5, 10.0)) * zoom;
    vec2 drosteUV = drosteSpiral(uv * expZoom, expZoom);

    vec2 moved = drosteUV + vec2(0.1, -0.2) * sin(t * 0.2);
    moved *= rot(t * 0.3);

    float n1 = neurons(uv, t);
    float n2 = neurons(moved, t + morph * 2.0);

    vec3 col1 = fractalNeuron(moved, t, n1, 1.0);
    vec3 col2 = fractalNeuron(moved, t, n2, 1.0);

    vec3 fg = mix(col1, col2, 0.5);
    vec3 bg = fractalNeuron(moved, t, 1.0, 0.0);

    float mask = smoothstep(0.4, 0.5, length(moved));
    vec3 final = mix(fg, bg, 0.2 + 0.2 * mask);

    // Glitch
    if (glitchStrength > 0.0) {
      float scan = sin(gl_FragCoord.y * 80.0 + t * glitchSpeed) * glitchStrength;
      final += vec3(scan);
    }

    gl_FragColor = vec4(postColor(final), 1.0);
  } else {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    gl_FragColor = vec4(IMG_NORM_PIXEL(BufferA, uv).rgb, 1.0);
  }
}
