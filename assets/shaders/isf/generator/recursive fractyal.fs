/*{
  "DESCRIPTION": "Converted from Shadertoy: Recursive pulsating fractal with tunable psychedelic color.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Psychedelic", "Volumetric"],
  "INPUTS": [
    { "NAME": "PulseSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "WarpDepth", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
  ]
}*/

vec3 psychedelicPalette(float t) {
    return 0.5 + 0.5 * cos(6.28318 * (vec3(0.1, 0.3, 0.6) * t + vec3(0.0, 0.2, 0.4)));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float t = sin(TIME * PulseSpeed) + TIME * PulseSpeed;
    float d = 0.0;
    float s = 0.0, s1 = 0.0, s2 = 0.0, n = 0.0;

    vec4 acc = vec4(0.0);
    vec3 p;

    for (float i = 1.0; i <= 128.0; i++) {
        p = vec3(uv * d, d + t);

        s1 = 1.0 - length(p.xy);
        s2 = 2.0 - length(p.xy);

        p.xy -= cos(t + vec2(0.15, 0.19) * p.z) * WarpDepth;

        for (n = 0.3; n < 4.0; n += n) {
            p += cos(t + p.yzx);
            s1 += abs(dot(sin(t + p * n * 1.0), vec3(0.9))) / n;
            s2 += abs(dot(sin(t + p * n * 2.0), vec3(0.6))) / n;
        }

        float shell = 0.001 + abs(max(s1, s2)) * 0.1;
        d += shell;

        float pulseMod = mod(float(i) * ColorPulse + t, 1.0);
        vec3 col = psychedelicPalette(pulseMod);
        acc.rgb += col / shell;
    }

    float mixSelector = s1 > s2 ? 1.0 : 9.0;
    vec4 finalColor = vec4(4.0, 2.0, mixSelector, 1.0) * acc * acc / 7e8;
    gl_FragColor = tanh(finalColor * Brightness);
}
