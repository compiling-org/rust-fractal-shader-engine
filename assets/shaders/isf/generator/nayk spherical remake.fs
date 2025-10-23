/*{
  "DESCRIPTION": "Orbit fractal with morphing geometry and vibrant psychedelic palettes.",
  "CATEGORIES": ["Fractal", "Psychedelic", "Dynamic"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "iterations", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "scale", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "rotationSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "geometryMorph", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "patternType", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Pattern Type", "VALUES": ["Default", "Wave", "Ripple"] },
    { "NAME": "colorMode", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 6.0, "LABEL": "Color Palette", "VALUES": ["Classic", "Lava", "Ice", "Rainbow", "NeonTrip", "PsyFusion", "HyperWave"] }
  ]
}
*/

vec3 palette(float t, int mode) {
  if (mode == 1) return vec3(1.2, 0.3, 0.1) * sin(6.28 * t); // Lava
  if (mode == 2) return vec3(0.2, 0.6, 1.0) * cos(6.28 * t); // Ice
  if (mode == 3) return vec3(0.5 + 0.5 * sin(t * 10.0 + vec3(0.0, 2.0, 4.0))); // Rainbow
  if (mode == 4) return vec3(0.8 + 0.2 * cos(t * 12.0 + vec3(2.0, 1.0, 4.0))) * vec3(1.2, 0.5, 1.5); // NeonTrip
  if (mode == 5) return vec3(sin(8.0 * t + vec3(1.0, 3.0, 5.0)) * 0.5 + 0.5) * vec3(1.5, 1.0, 0.8);  // PsyFusion
  if (mode == 6) return vec3(0.5 + 0.5 * cos(t * 15.0 + vec3(0.7, 1.4, 2.1))); // HyperWave
  return vec3(0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) * t + vec3(0.153, 0.56, 0.557)))); // Classic
}

void main() {
  vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
  vec2 uv0 = uv;
  vec3 finalColor = vec3(0.0);
  float time = TIME * speed;

  // Geometry morphing
  if (patternType == 1.0) {
    uv += sin(uv.yx * 4.0 + time) * geometryMorph * 0.2;
  } else if (patternType == 2.0) {
    uv += cos(length(uv) * 10.0 + time) * geometryMorph * 0.2;
  }

  for (float i = 0.0; i < 10.0; i++) {
    if (i >= iterations) break;

    float angle = time * rotationSpeed + i * 0.5 * length(uv0) + i * 0.4;
    mat2 rotMat = mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
    uv = rotMat * uv;
    uv += reflect(uv, uv);
    uv += vec2(cos(time), sin(time));
    uv = fract(uv * scale) - 0.5;

    float d = length(uv) * exp(-length(uv0));
    vec3 col = palette(length(uv0) + i * 0.4 + time * 0.4, int(colorMode));
    d = sin(d * 8.0 + time) / 8.0;
    d = abs(d);
    d = pow(0.01 / d, 1.2);
    finalColor += col * d;
  }

  finalColor = (finalColor - 0.5) * contrast + 0.5;
  finalColor *= brightness;
  gl_FragColor = vec4(finalColor, 1.0);
}
