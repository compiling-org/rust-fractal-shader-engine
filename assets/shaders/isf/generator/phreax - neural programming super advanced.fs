/*
{
  "CATEGORIES": ["Fractal", "Neuron", "Droste"],
  "DESCRIPTION": "Faithful neuron + droste fractal with palette, pulse, and visual tuning",
  "INPUTS": [
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "colorPulseStrength", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "colorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "pulseMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "pulseColor", "TYPE": "color", "DEFAULT": [0.1, 1.0, 0.5, 1.0] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glowBoost", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ],
  "PASSES": [{ "TARGET": "BufferA", "FLOAT": true, "PERSISTENT": true }, {}]
}
*/

#define TAU 6.28318

vec3 getPalette(float t, float palId) {
  t = fract(t);
  if (palId < 1.0) return vec3(0.5 + 0.5*cos(TAU*(t + vec3(0.0, 0.33, 0.67))));
  if (palId < 2.0) return vec3(0.5 + 0.5*cos(TAU*(t + vec3(0.1, 0.2, 0.3))));
  if (palId < 3.0) return vec3(0.5 + 0.5*sin(TAU*(t + vec3(0.9, 0.5, 0.2))));
  if (palId < 4.0) return vec3(0.7 + 0.3*cos(TAU*(t + vec3(0.2, 0.5, 0.7))));
  if (palId < 5.0) return vec3(0.6 + 0.4*sin(TAU*(t + vec3(0.0, 0.25, 0.5))));
  return vec3(0.9 + 0.1*sin(TAU*(t + vec3(0.15, 0.3, 0.6))));
}

mat2 rot(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, s, -s, c);
}

float d2(vec2 v, float k) {
  return pow(pow(abs(v.x), k) + pow(abs(v.y), k), 1.0 / k);
}

vec3 fractalNeuron(vec2 uv, float t, float nVal, float layerFactor) {
  vec2 uv0 = uv;
  vec3 col = vec3(0);
  float pulseAmount = sin(t * colorPulseSpeed) * 0.5 + 0.5;
  pulseAmount *= colorPulseStrength;

  for (float i = 0.; i < 6.; i++) {
    uv = fract(uv * 1.9) - 0.5;
    vec3 baseColor = getPalette(length(uv0) + t * 0.4 + i * 0.9, palette);

    float d = d2(uv, 1.2) + 0.05 / nVal + 0.2 * exp(-length(uv0));
    d = abs(0.1 * d + sin(d * (8.0 + 2.0 * nVal)) / 10.0) + 0.001 / nVal;
    d = glowBoost * 0.1 * pow(0.02 / d, 1.2) + smoothstep(0.05, 0.0, d);

    col += max(0.0, d) * baseColor * layerFactor;
  }

  col = mix(col, pulseColor.rgb * col, pulseMix * pulseAmount);
  return col;
}

float neurons(vec2 uv, float t) {
  vec2 n = vec2(0), q;
  vec2 N = vec2(0);
  vec2 p = uv + sin(t * 0.1) / 10.0;
  float S = 5.0;
  mat2 m = rot(1.0 - 0.0001);

  for (float j = 0.; j < 30.; j++) {
    p *= m;
    n *= m;
    q = p * S + j + n + t;
    n += sin(q);
    N += cos(q) / S;
    S *= 1.15;
  }

  return pow((N.x + N.y + 0.1) + 0.0001 / length(N), 2.1);
}

vec2 drosteSpiral(vec2 uv, float t) {
  float r1 = 0.15, r2 = 1.0;
  vec2 z = vec2(length(uv), atan(uv.y, uv.x));
  z.x = log(z.x / r1);
  float ratio = log(r2 / r1);
  float alpha = atan(ratio, TAU);
  mat2 digamma = rot(alpha);
  vec2 beta = digamma * z / cos(alpha);
  beta.x = mod(beta.x, log(r2 / r1));
  return r1 * exp(beta.x) * vec2(cos(beta.y), sin(beta.y));
}

void main() {
  if (PASSINDEX == 0) {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;

    vec2 uv1 = uv * zoom;
    vec2 uvSpiral = drosteSpiral(uv1, t);
    uvSpiral *= rot(t * 0.3);

    float n1 = neurons(uv, t);
    float n2 = neurons(uvSpiral, t + morph);

    vec3 col1 = fractalNeuron(uv, t, n1, 1.0);
    vec3 col2 = fractalNeuron(uvSpiral, t, n2, 1.0);

    vec3 finalCol = mix(col1, col2, 0.5);

    // Postprocess
    finalCol = pow(finalCol * brightness, vec3(contrast));
    float luma = dot(finalCol, vec3(0.299, 0.587, 0.114));
    finalCol = mix(vec3(luma), finalCol, saturation);

    gl_FragColor = vec4(finalCol, 1.0);
  } else {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = IMG_NORM_PIXEL(BufferA, mod(uv, 1.0)).rgb;
    gl_FragColor = vec4(col, 1.0);
  }
}
