/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Converted from https://www.shadertoy.com/view/lX3cDB by sleeplessmonk. Intro to shader coding tutorial with tunable inputs",
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

// Color gradient function with tunable palette
vec3 palette(float t, float type) {
    vec3 a, b, c, d;

    if (type < 1.0) {
        a = vec3(0.051, 0.047, 0.047);
        b = vec3(0.945, 0.914, 0.914);
        c = vec3(1.0, 1.0, 1.0);
        d = vec3(0.149, 0.796, 0.102);
    } 
    else if (type < 2.0) {
        a = vec3(0.1, 0.2, 0.5);
        b = vec3(0.8, 0.6, 0.4);
        c = vec3(0.6, 0.8, 1.0);
        d = vec3(0.2, 0.5, 0.8);
    } 
    else {
        a = vec3(0.3, 0.1, 0.7);
        b = vec3(0.9, 0.2, 0.5);
        c = vec3(0.5, 0.9, 0.3);
        d = vec3(0.8, 0.4, 0.2);
    }

    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;

    vec2 uv0 = uv;
    vec3 finalColor = vec3(0.0);

    float c = 1.5;
    for (float i = 0.0; i < 2.2; i++) {
        c += i * 10.0;
        
        // Apply morphing effect
        uv = (fract(uv * (1.4 + MorphStrength * 0.5)) - 0.5);

        // Distance calculation with speed factor
        float d = length(uv) * exp(-length(uv0) + (1.5 + i)) * (1.0 + 0.1 * sin(TIME * Speed));

        // Dynamic color pulsing
        vec3 col = palette(length(uv0) + i * 5.6 + TIME * 0.2 * (1.0 + ColorPulse), PaletteType);

        // Sin wave transformation
        d = sin(d * 8.8 + TIME * Speed * 0.4) / 8.0;
        d = abs(d);
        d = pow(0.001 / d, .12);
        
        finalColor += col * d;
    }

    gl_FragColor = vec4(finalColor, 2.0);
}
