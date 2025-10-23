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

#define PI 3.141592
#define TAU (2.0 * PI) // Define TAU as 2 * PI
#define pal(x) (cos(x * 2. * PI + vec3(0, 22, 14)) * 0.5 + 0.5)
#define SIN(x) (0.5 + 0.5 * sin(x))
#define tt TIME
#define S(a, b, x) smoothstep(a, b, x)
#define rot(x) (mat2(cos(x), -sin(x), -sin(x), cos(x)))

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    // Ray direction and origin
    vec3 p, rd = normalize(vec3(uv, 1.0 - 0.2 * SIN(PI + 0.25 * tt)));
    vec3 ro = vec3(0.0, 0.0, tt * 0.05);
    vec3 col = vec3(0.0);

    // Tunable parameters replacing audio input
    float reactiveParam = param1; // Used in place of audio input
    float distortionParam = param2; // Controls distortions

    // Raymarching loop
    for (float i = 0.0, s, d, t = 0.0; i < 100.0; i++) {
        p = t * rd + ro;
        
        // Geometry and distortion
        float len = mix(0.5 * (abs(p.x) + abs(p.y)), length(p.xz), 0.4);
        p.zy += vec2(0.2, 0.1) * sin(0.2 * tt + len + reactiveParam);
        p.xy *= mix(1.2, 0.5, SIN(len * 5.0 + 0.5 * tt + reactiveParam * 2.0));

        // Recursive fractal
        p = abs(p) - vec3(0.1 + 0.02 * S(3.0, 7.0, tt), 0.4 - 0.02 * S(3.0, 7.0, tt), 1.9);
        s = 2.7;
        for (int j = 0; j < 5; j++) {
            p.xy *= mix(1.05, 0.95, SIN(length(p.xy) * 0.5 + 0.1 * tt));
            p = abs(p - vec3(1.2, 1.3, 1.1)) - vec3(1.0, 1.2, 1.08 + 0.04 * sin(tt + reactiveParam));
            s *= d = 2.5 / clamp(dot(p, p), 0.5, 1.7);
            p = abs(p) * d;
        }

        // Apply rotation and time modulation
        p.xy *= rot(0.5 * PI * SIN(tt + reactiveParam));

        // Distance field and accumulation
        t += d = (max(length(p.xz), -(length(p - ro) - 0.3))) / s;
        col += mix(vec3(0.992, 0.765, 0.447), pal(log(s * 0.4 + 1.0 + tt) * 0.5), 0.7) *
               0.02 * exp(-0.7 * i * i * d);
    }

    // Color enhancements
    col = pow(col, vec3(2.0)) + 0.2 * sin(TAU * (tt + reactiveParam));
    gl_FragColor.xyz = col;
}
