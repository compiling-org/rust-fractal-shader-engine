/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Converted from https://www.shadertoy.com/view/7dBSR3 by NikolaErceg with tunable parameters.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0
        },
        {
            "NAME": "MorphStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "ColorPulse",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "PaletteType",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ]
}
*/

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

const float pi = acos(-1.0);
const float pi2 = pi * 2.0;

vec2 pmod(vec2 p, float r) {
    float a = atan(p.x, p.y) + pi / r;
    float n = pi2 / r;
    a = floor(a / n) * n;
    return p * rot(-a);
}

float box(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
}

// Fractal IFS Box with Morphing
float ifsBox(vec3 p) {
    for (int i = 0; i < 5; i++) {
        p = abs(p) - 1.0;
        p.xy *= rot(TIME * Speed * 0.3 + MorphStrength);
        p.xz *= rot(TIME * Speed * 0.1 - MorphStrength * 0.2);
    }
    p.xz *= rot(TIME * Speed);
    return box(p, vec3(0.4, 0.8, 0.3));
}

// Scene Mapping Function
float map(vec3 p, vec3 cPos) {
    vec3 p1 = p;
    p1.x = mod(p1.x - 5.0, 10.0) - 5.0;
    p1.y = mod(p1.y - 5.0, 10.0) - 5.0;
    p1.z = mod(p1.z, 16.0) - 8.0;
    p1.xy = pmod(p1.xy, 7.0);
    return ifsBox(p1);
}

// Color Palette Function
vec3 palette(float t, float type) {
    vec3 a, b, c, d;

    if (type < 1.0) {
        a = vec3(0.2, 0.3, 0.8);
        b = vec3(0.6, 0.4, 0.2);
        c = vec3(0.9, 1.0, 0.5);
        d = vec3(0.4, 0.2, 0.6);
    } 
    else if (type < 2.0) {
        a = vec3(0.1, 0.5, 0.2);
        b = vec3(0.8, 0.2, 0.7);
        c = vec3(0.6, 0.9, 0.8);
        d = vec3(0.5, 0.3, 0.9);
    } 
    else {
        a = vec3(0.7, 0.1, 0.3);
        b = vec3(0.2, 0.9, 0.6);
        c = vec3(1.0, 0.8, 0.5);
        d = vec3(0.9, 0.6, 0.2);
    }

    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);

    vec3 cPos = vec3(0.0, 0.0, -3.0 * TIME * Speed);
    vec3 cDir = normalize(vec3(0.0, 0.0, -1.0));
    vec3 cUp = vec3(sin(TIME * Speed * 0.3), 1.0, 0.0);
    vec3 cSide = cross(cDir, cUp);
    vec3 ray = normalize(cSide * p.x + cUp * p.y + cDir);

    float acc = 0.0;
    float acc2 = 0.0;
    float t = 0.0;
    
    for (int i = 0; i < 99; i++) {
        vec3 pos = cPos + ray * t;
        float dist = map(pos, cPos);
        dist = max(abs(dist), 0.02);
        float a = exp(-dist * (3.0 + MorphStrength));

        if (mod(length(pos) + 30.0 * TIME * Speed, 60.0) < 1.0) {
            a *= 2.0;
            acc2 += a;
        }
        
        acc += a;
        t += dist * 0.5;
    }

    float pulse = 1.0 + 0.5 * sin(TIME * Speed * ColorPulse);
    vec3 col = palette(acc * 0.05 + acc2 * 0.1, PaletteType) * pulse;

    gl_FragColor = vec4(col, 1.0 - t * 0.03);
}
