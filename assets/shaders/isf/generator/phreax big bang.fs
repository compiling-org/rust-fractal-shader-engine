/*{
  "DESCRIPTION": "Log-spherical fractal portal with palette shifting, morphing, and dynamic control.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Volumetric", "Psychedelic"],
  "INPUTS": [
    { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "GeometryScale", "TYPE": "float", "DEFAULT": 9.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "RotationRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
  ]
}*/

#define SIN(t) (0.5 + 0.5 * sin(t))

vec3 paletteColor(float x, int palette) {
    vec3 colors[5];
    int count = 5;

    if (palette == 0) {
        colors[0] = vec3(0.33, 0.05, 0.45);
        colors[1] = vec3(0.93, 0.26, 0.40);
        colors[2] = vec3(1.00, 0.82, 0.25);
        colors[3] = vec3(0.23, 0.81, 0.67);
        colors[4] = vec3(0.05, 0.47, 0.68);
    } else if (palette == 1) {
        colors[0] = vec3(0.12, 0.12, 0.9);
        colors[1] = vec3(0.2, 0.9, 0.7);
        colors[2] = vec3(1.0, 1.0, 0.0);
        colors[3] = vec3(1.0, 0.0, 0.5);
        colors[4] = vec3(0.8, 0.0, 1.0);
    } else {
        colors[0] = vec3(0.7, 0.9, 1.0);
        colors[1] = vec3(0.9, 0.7, 0.4);
        colors[2] = vec3(0.8, 1.0, 0.6);
        colors[3] = vec3(1.0, 0.6, 0.5);
        colors[4] = vec3(0.4, 0.7, 1.0);
    }

    float stepSize = 1.0 / float(count);
    int i = int(fract(x) / stepSize);
    float f = fract(x) / stepSize - float(i);

    int i0 = int(mod(float(i), float(count)));
    int i1 = int(mod(float(i + 1), float(count)));

    vec3 c0, c1;
    if (i0 == 0) c0 = colors[0];
    else if (i0 == 1) c0 = colors[1];
    else if (i0 == 2) c0 = colors[2];
    else if (i0 == 3) c0 = colors[3];
    else c0 = colors[4];

    if (i1 == 0) c1 = colors[0];
    else if (i1 == 1) c1 = colors[1];
    else if (i1 == 2) c1 = colors[2];
    else if (i1 == 3) c1 = colors[3];
    else c1 = colors[4];

    return mix(c0, c1, pow(f, 2.0));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * TimeMultiplier;
    uv = uv.yx;
    uv *= mix(0.8, 1.2, SIN(-time + 5.0 * length(uv)));

    vec3 col = vec3(0.0);
    vec3 ray = vec3(uv, 1.0);
    vec3 p;
    float t = 0.05 + 0.1 * sin(dot(uv, uv) * 10.0);

    for (float i = 0.0; i < 100.0; i++) {
        p = ray * t;
        float z = p.z;
        float r = length(p);
        p = vec3(log(r) * 0.5, acos(p.z / r), atan(p.y, p.x));

        float morph = mix(1.2, -0.3, smoothstep(0.0, 5.0, TIME)) - 0.2 * sin(0.25 * time);
        for (float j = 0.0; j < 3.0; j++) {
            float a = exp(j) / exp2(j);
            p = abs(p) + morph * MorphAmount;
            p += cos(2.0 * p.yzx * a + time - length(p.xy) * GeometryScale) / a;
        }

        float d = 0.007 + abs((exp2(1.4 * p).y - 1.0)) / 13.0;
        float k = t * 0.7 + length(p) * 0.1 - 0.2 * time + z * 0.1;
        vec3 c = paletteColor(k * ColorPulse, int(PaletteSelect));
        c = mix(c, c * vec3(0.95, 1.0, 0.8), SIN(z * 0.5));
        col += c * 1e-3 / d;
        t += d / 4.0;
    }

    float glow = exp(-17.0 * length(uv));
    col += 0.4 * mix(vec3(0.3, 0.9, 1.0), vec3(0.9, 1.0, 0.6), SIN(glow * 2.0 - time)) * pow(glow * 11.0, 1.0);

    col = (exp(col * 0.08) - exp(-col * 0.08)) / (exp(col * 0.08) + exp(-col * 0.08));
    col = pow(col, vec3(0.45));

    gl_FragColor = vec4(col, 1.0);
}
