/*{
  "CATEGORIES": ["Raymarching", "Psychedelic"],
  "DESCRIPTION": "Raymarched pulsing tunnel (Xor style) with tunable parameters.",
  "ISFVSN": "2",
  "INPUTS": [
    {
      "NAME": "Speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 5.0
    },
    {
      "NAME": "Pulse",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 10.0
    },
    {
      "NAME": "Brightness",
      "TYPE": "float",
      "DEFAULT": 0.03,
      "MIN": 0.01,
      "MAX": 0.4
    },
    {
      "NAME": "CeilingMod",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 1.0,
      "MAX": 10.0
    },
    {
      "NAME": "ColorMix",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0.1,
      "MAX": 2.0
    }
  ]
}*/

vec3 softToneMap(vec3 color) {
  return color / (color + vec3(1.0));
}

void main() {
    float t = TIME * Speed;
    float i = 0.0;
    float z = 0.0;
    float d = 0.0;
    vec3 col = vec3(0.0);

    for (int step = 0; step < 30; step++) {
        vec3 r = normalize(vec3(2.1 * gl_FragCoord.xy, 0.0) - RENDERSIZE.xyy);
        vec3 p = z * r;
        vec3 w = r / abs(r.y);

        // Scroll & shift
        w.z -= t;
        p.z -= t;
        vec3 cameraShift = ++p;

        // Reflective ceiling
        p.y = abs(mod(p.y - 2.0, CeilingMod) - 2.0);

        // Raymarch distance field
        z += d = 0.01 +
            0.5 * length(sin(p + p).xz + p.y * 0.4) +
            0.1 * exp(sin(length(sin(w)) * 40.0 * Pulse + p.z)) * length(p - cameraShift);

        col += (sin(p * ColorMix) + 1.1) / d / z;
    }

    col *= Brightness;
    col = softToneMap(col);  // gamma-friendly tone mapping
    gl_FragColor = vec4(col, 1.0);
}
