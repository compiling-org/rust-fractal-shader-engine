/*{
"DESCRIPTION": "Recursive volumetric portal with palette, rotation, morph, and geometry control.",
"ISFVSN": "2.0",
"CATEGORIES": ["Fractal", "Psychedelic", "Volumetric"],
"INPUTS": [
{ "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 3.0 },
{ "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
{ "NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
{ "NAME": "Speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
{ "NAME": "GeometryScale", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 20.0 },
{ "NAME": "RotationRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
]
}*/


mat2 rot(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

vec3 palette(float t, float type) {
    if (type < 1.0) {
        return 0.5 + 0.5 * cos(6.28318 * t + vec3(0.0, 0.5, 1.0));
    } else if (type < 2.0) {
        return vec3(sin(t), sin(t + 1.5), cos(t * 0.5));
    } else {
        return 0.5 + 0.5 * cos(6.28318 * t + vec3(1.0, 0.0, 0.25));
    }
}

// Manual implementation of tanh for vec3
vec3 tanh(vec3 x) {
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 col = vec3(0.0);
    vec3 rd = vec3(uv, 1.0);
    float time = TIME * Speed;
    float t = 0.0;

    for (float i = 0.0; i < 100.0; i++) {
        vec3 p = t * rd + rd;
        p.z += time;
        p = abs(p) - 0.3;

        // XY rotation based on z and RotationRate
        p.xy *= rot(p.z * RotationRate);
        p = abs(p) - 0.3;

        for (float j = 0.0; j < 3.0; j++) {
            float a = exp(j) / exp2(j);
            p += cos(4.0 * p.yzx * a + time - length(p.xy) * GeometryScale) * MorphAmount / a;
        }

        float d = 0.01 + abs((p - vec3(0, 1, 0)).y - 1.0) / 10.0;
        vec3 palCol = palette(t * ColorPulse, PaletteSelect);

        col += palCol * 1e-3 / d;
        t += d / 4.0;
    }

    col *= tanh(col * 0.1); // using custom tanh
    col = pow(col, vec3(0.45));
    gl_FragColor = vec4(col, 1.0);
}