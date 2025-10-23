/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Hexagonal Patterns",
        "Converted from Shadertoy"
    ],
    "DESCRIPTION": "Psychedelic hexagonal patterns with tunable parameters for customization.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "MorphingIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "ColorIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "WaveFrequency",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 5.0
        },
        {
            "NAME": "Brightness",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.5
        }
    ]
}
*/



#define TWO_PI 6.2831853072
#define PI 3.14159265359

vec2 rotate(in vec2 v, in float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return v * mat2(c, -s, s, c);
}

vec3 coordToHex(in vec2 coord, in float scale, in float angle) {
    vec2 c = rotate(coord, angle);
    float q = (1.0 / 3.0 * sqrt(3.0) * c.x - 1.0 / 3.0 * c.y) * scale;
    float r = 2.0 / 3.0 * c.y * scale;
    return vec3(q, r, -q - r);
}

vec3 hexToCell(in vec3 hex, in float m) {
    return fract(hex / m) * 2.0 - 1.0;
}

float absMax(in vec3 v) {
    return max(max(abs(v.x), abs(v.y)), abs(v.z));
}

float nsin(in float value) {
    return sin(value * TWO_PI) * 0.5 + 0.5;
}

float hexToFloat(in vec3 hex, in float amt) {
    return mix(absMax(hex), 1.0 - length(hex) / sqrt(3.0), amt);
}

float calc(in vec2 tx, in float time) {
    float angle = PI * nsin(time * 0.1) + PI / 6.0;
    float len = 1.0 - length(tx) * nsin(time);
    float value = 0.0;
    vec3 hex = coordToHex(tx, Zoom * nsin(time * 0.1), angle);
    for (int i = 0; i < 3; i++) {
        float offset = float(i) / 3.0;
        vec3 cell = hexToCell(hex, 1.0 + float(i));
        value += nsin(hexToFloat(cell, nsin(len + time + offset)) * 
                  WaveFrequency * nsin(time * 0.5 + offset) + len + time);
    }
    return value / 3.0;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * Speed;
    vec3 rgb = vec3(0.0);

    for (int i = 0; i < 3; i++) {
        float time2 = time + float(i) * 0.025;
        vec3 hex = coordToHex(uv, Zoom * nsin(time * 0.25), PI * nsin(time * 0.06));
        vec3 cell = hexToCell(hex, 1.3 + float(i) * 0.45);
        float value = nsin(hexToFloat(cell, nsin(time2 * 1.1)) * WaveFrequency * nsin(time * 0.7) + time2);
        rgb[i] += pow(value, 2.8) * vec3(1.2, 0.5, 0.8)[i];
    }

    // Apply brightness and color intensity
    rgb *= Brightness * ColorIntensity;

    gl_FragColor = vec4(rgb, 1.0);
}