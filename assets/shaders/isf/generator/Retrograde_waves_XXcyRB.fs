/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Enhanced version of https://www.shadertoy.com/view/XXcyRB by ekaunt. Added tunable parameters for color shift, psychedelic pulsing, speed, morphing, and more.",
    "IMPORTED": {},
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
            "NAME": "ColorShiftSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "MorphingSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "PsychedelicIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "BackgroundColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 0.0, 1.0]
        }
    ]
}
*/
#define PI 3.1415926535
#define Pi2 (PI * 2.)
#define TIME TIME
#define RENDERSIZE RENDERSIZE

// Helper Functions
vec2 Reflect(vec2 uv, float angle) {
    vec2 N = vec2(sin(angle), cos(angle));
    return N * min(0., dot(uv, N)) * 2.;
}
mat2 rot(float angle) {
    return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}
float remap(float a1, float a2, float b1, float b2, float x) {
    return ((x - a1) / (a2 - a1)) * (b2 - b1) + b1;
}
vec3 palette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0., 0.33, 0.67);
    return a + b * cos(6.28318 * (c * t + d));
}
void main() {
    // Initialize UV coordinates
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2. - 1.;
    uv.y *= RENDERSIZE.y / RENDERSIZE.x;
    uv *= Zoom;
    uv *= rot(TIME * Speed / 10. * PI);
    // Apply reflections
    float angle = -1.13 * MorphingSpeed;
    uv -= Reflect(uv, angle * PI);
    angle = 0.12 * MorphingSpeed;
    uv -= Reflect(uv, angle * PI);
    angle = -1.13 * MorphingSpeed;
    uv -= Reflect(uv, angle * PI);
    angle = 0.12 * MorphingSpeed;
    uv -= Reflect(uv, angle * PI);
    // Add periodic distortions
    uv.y += pow(fract(1. - uv.x + TIME * Speed), 3.);
    // Compute distance field
    float d = length(uv - vec2(clamp(uv.x, -1., 2.), 0));
    float dist = 0.5 * max(0., uv.x);
    float blur = 0.1; // remap(-1., 2., 0.1, 0.25, uv.x);
    float line = smoothstep(dist, dist * blur, d);
    // Layered patterns
    float L = 4.0;
    uv.y = abs(uv.y);
    for (float j = 1.; j < L; ++j) {
        uv.x -= 0.5;
        for (float i = 0.; i < 2.; ++i) {
            uv.y -= 2. / 8.;
            uv.y += pow(fract((1. - uv.x) - TIME * Speed / 2.), 3.);
            uv -= Reflect(uv, (sin(TIME * Speed / 8.) * 0.3 + 0.3) * PI);
            dist = 0.5 * max(0., uv.x);
            blur = 0.1; // remap(-1., 2., 0.1, 0.25, uv.x);
            d = length(uv - vec2(clamp(uv.x, -1., 2.), 0));
            line += smoothstep(dist, dist * blur, d);
        }
    }
    // Generate color
    vec3 col = vec3(mod(line, 2.0)); // Fixed: Explicitly cast float to vec3
    col.rg += uv;
    col *= palette(uv.g + TIME * ColorShiftSpeed);
    col *= PsychedelicIntensity;
    // Mix with background
    vec3 bg = BackgroundColor.rgb;
    col = mix(bg, col, smoothstep(0.0, 0.1, length(uv)));
    gl_FragColor = vec4(col, 1.0);
}