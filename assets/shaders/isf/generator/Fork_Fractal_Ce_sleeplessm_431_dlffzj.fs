/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Modified for software like TouchDesigner, Synesthesia, HeavyM",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "param1",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "param2",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.3
        }
    ]
}
*/

vec3 palette(float t) {
    vec3 a = vec3(0.6, 0.4, 0.5);
    vec3 b = vec3(0.7, 0.3, 0.5);
    vec3 c = vec3(0.8, 1.0, 0.6);
    vec3 d = vec3(0.76, 0.41, 0.96);

    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv;
    
    vec3 finalColor = vec3(0.0);
    
    for (float i = 0.0; i < 4.0; ++i) {
        uv = fract(uv * 1.6) - 0.5;
        float d = length(uv) * exp(-length(uv0));
        vec3 color = palette(length(uv0) + i * 0.2 + TIME * 0.4);
        d = sin(d * 12.0 + TIME) / 12.0;
        d = abs(d);
        d = pow(0.015 / d, 1.3);
        finalColor += color * d;
    }
    
    // Replace audio amplitude with external parameter control
    float audioReactive = param1 * 2.0; // Enhancing reactivity
    float colorShift = param2 * 5.0; // Controls psychedelic color shifts
    
    vec3 neonColor = vec3(
        0.5 + 0.5 * sin(finalColor.r * 10.0 + TIME * audioReactive * 30.0),
        0.5 + 0.5 * sin(finalColor.g * 10.0 + TIME * audioReactive * 30.0 + colorShift),
        0.5 + 0.5 * sin(finalColor.b * 10.0 + TIME * audioReactive * 30.0 + colorShift * 2.0)
    );

    gl_FragColor = vec4(neonColor, 1.0);
}
