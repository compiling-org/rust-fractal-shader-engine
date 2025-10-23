/*{
  "DESCRIPTION": "ISF port of Angel shader with trippy params.",
  "CATEGORIES": ["Psychedelic","Fractal"],
  "INPUTS": [
    { "NAME": "xy", "TYPE": "point2D", "DEFAULT": [0.5,0.5], "LABEL": "XY Control" },
    { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "geometry", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    {
      "NAME": "paletteIndex",
      "TYPE": "float",
      "LABELS": ["Rainbow", "Psywave", "Neon Fade", "Oil Spill", "Cyber Flux", "Dream Acid", "Pulse Storm"],
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 6.0
    }
  ]
}*/

vec3 palette(float i, float idx) {
  float x = i * 0.1;
  if (idx < 0.5) return vec3(sin(x*6. + 0.0), sin(x*6. + 2.0), sin(x*6. + 4.0)) * 0.5 + 0.5; // Rainbow
  if (idx < 1.5) return vec3(sin(x*8. + 0.2), cos(x*3. + 3.2), sin(x*5. + 2.0)) * 0.5 + 0.5; // Psywave
  if (idx < 2.5) return vec3(0.5+0.5*cos(x+vec3(0,2,4))); // Neon Fade
  if (idx < 3.5) return vec3(0.5+0.5*cos(x*2.+vec3(3,1,0))); // Oil Spill
  if (idx < 4.5) return vec3(abs(sin(x*10.)), abs(cos(x*3.)), sin(x*5.)); // Cyber Flux
  if (idx < 5.5) return vec3(sin(x*6. + 1.0), sin(x*9. + 3.5), sin(x*12. + 6.0)) * 0.5 + 0.5; // Dream Acid
  return vec3(sin(x*5. + 2.0), sin(x*7. + 5.5), sin(x*3. + 1.0)) * 0.5 + 0.5; // Pulse Storm
}

void main() {
  vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  float t = TIME * speed;
  float F = 12.0 + tanh(sin(t) * 5.0) * pulse * 12.0;

  vec4 O = vec4(0.0);
  float i = 0.0;
  float z = 0.0;
  float d = 0.0;

  vec2 shak = vec2(sin(t*10.0), cos(t*8.0)) * shake * 0.1;
  vec2 inputOffset = (xy - 0.5) * 2.0;

  for (int j = 0; j < 100; ++j) {
    vec3 p = z * normalize(vec3((uv + inputOffset + shak) * zoom, 0.0) - vec3(0.0, 0.0, -1.0));
    p.z += F;

    float a = F + morph;
    float s = sin(a), c = cos(a);
    mat2 rot = mat2(c, -s, s, c);
    p.xz = rot * p.xz;

    vec3 wave = sin(vec3(p.x, p.y, p.z) + t);
    O.rgb += abs(palette(dot(wave, vec3(0.15)), paletteIndex));

    for (float dd = 1.0; dd < 6.0; dd *= 0.8) {
      p += cos((p.yzx - t * vec3(3.0,1.0,0.0)) * dd) / dd;
    }

    float geom = 0.1 + abs(length(p.xz) - 0.5 + sin(p.y + t * 2.0) * geometry * 0.2);
    z += d = geom / 30.0;

    O += (sin(z + vec4(2,3,4,0)) + 1.0) / (d + 0.001);
    i += 1.0;
  }

  O = tanh(O / 4000.0);

  // glitch effect
  float g = sin(gl_FragCoord.y * 10.0 + t * 50.0) * glitch * 0.1;
  O.rgb += g;

  // apply color adjustments
  vec3 c = O.rgb;
  float avg = dot(c, vec3(0.333));
  c = mix(vec3(avg), c, saturation);
  c = (c - 0.5) * contrast + 0.5;
  c *= brightness;

  gl_FragColor = vec4(c, 1.0);
}
