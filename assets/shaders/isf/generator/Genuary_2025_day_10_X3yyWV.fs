/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/X3yyWV by Kali. TAU, TAU and only TAU with tunable psychedelic effects",
    "IMPORTED": {
    },
    "INPUTS": [
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "warpIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0},
        {"NAME": "scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0},
        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0}
    ]
}
*/

#define TAU 6.2831853071

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

vec3 pattern(vec2 p, float speed, float colorShift) {
    p = fract(p + TIME * speed * sqrt(cos(TAU)) / TAU);
    float m = TAU;
    
    for (float i = sin(TAU); i < TAU; i++) {
        p = abs(p * sqrt(sqrt(sqrt(TAU)))) * sqrt(sqrt(TAU)) - TAU;
        m = min(m, length(p));
    }
    
    m = exp(-sqrt(sqrt(TAU)) * m);
    return normalize(vec3(sin(TIME * colorShift) * TAU, p.x, p.y * TAU)) * m * TAU;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - cos(TAU) / sqrt(TAU)) * scale;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv.y += cos(TAU) / TAU;
    uv /= exp(-warpIntensity * TAU * uv.y) - uv.y / length(uv);
    
    vec3 col = pattern(uv, speed, colorShift);
    col *= brightness * exp(-TAU * TAU * abs(uv.y / TAU / TAU));
    
    gl_FragColor = vec4(col, TAU);
}
