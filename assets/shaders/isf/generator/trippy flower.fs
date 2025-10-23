/*{
  "CATEGORIES": ["Generator", "Trippy", "Psychedelic"],
  "DESCRIPTION": "Morphing DMT-style visual with palette cycling and shape modulation.",
  "INPUTS": [
    { "NAME": "speed",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "paletteShift",  "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "geometryMorph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "scale",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "saturation",    "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "brightness",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 }
  ]
}*/

vec3 palette(float t) {
  return 0.5 + 0.5 * cos(6.2831 * (vec3(0.3, 0.5, 0.9) + t));
}

vec3 applyContrastSaturationBrightness(vec3 color, float con, float sat, float brt) {
  const vec3 averageLuminance = vec3(0.2126, 0.7152, 0.0722);
  float intensity = dot(color, averageLuminance);
  vec3 grey = vec3(intensity);
  color = mix(grey, color, sat);
  color = (color - 0.5) * con + 0.5;
  return color * brt;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

  float t = TIME * speed;

  // Zoom in/out
  uv *= scale;

  // Morphing shape
  float r = length(uv);
  float angle = atan(uv.y, uv.x);
  float morph = sin(t + r * 10.0 + cos(angle * 6.0)) * geometryMorph;

  // Make base pattern
  float shape = sin(r * 10.0 + morph * 5.0 - t * 2.0);
  shape += 0.5 * cos(r * 5.0 - t * 3.0 + sin(angle * 4.0));
  shape = abs(shape);

  // Apply palette
  float paletteT = t * 0.2 + paletteShift + shape;
  vec3 col = palette(paletteT);

  // Shape-based masking
  col *= smoothstep(1.2, 0.2, shape);

  // Post-processing
  col = applyContrastSaturationBrightness(col, contrast, saturation, brightness);

  gl_FragColor = vec4(col, 1.0);
}
