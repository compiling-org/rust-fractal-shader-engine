/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/clySzd by LydianLights. Enhanced with tunable parameters for animation speed, zoom, color morphing, and interactivity.",
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
            "MAX": 10.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "ColorSeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 1.0
        }
    ],
    "PASSES": [
        {},
        {}
    ]
}
*/

// Color palette function
vec3 palette(in float t, in float seed) {
    vec3 a = vec3(0.7, 0.1, 0.5);
    vec3 b = vec3(0.6, 0.7, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.0, 0.2, 0.4);
    return a + b * cos(6.28318 * (c * (t + seed) + d));
}

// Hash function for randomness
vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// Main rendering pass
vec3 mainPass(in vec2 fragCoord) {
    vec2 uv = 2.0 * (fragCoord / RENDERSIZE.xy) - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= Zoom; // Apply zoom parameter
    vec2 uv0 = uv;

    uv -= 0.5;
    float d0 = length(uv0);
    vec3 color = vec3(0.0);

    for (float i = 0.0; i < 5.0; i++) {
        uv = fract(vec2(1.0 + 0.18 * i) * uv) - 0.5;
        vec3 polar = vec3(log(length(uv)), atan(uv.y / uv.x), atan(uv.x / uv.y));

        float p = mix(0.7, 1.5, d0 * d0);
        float q = mix(0.02, 0.07, clamp(d0, 0.0, 1.0));
        vec2 d = vec2(length(polar.xy), length(polar.xz));
        d *= pow(0.03, d0 * 0.4);
        d = sin(5.0 * d + 0.2 * vec2(5.0 * TIME * Speed, 3.0 * TIME * Speed)); // Use Speed parameter
        d = abs(d);
        d = pow(q / d, vec2(p));
        float d0 = d.x + d.y;

        // Use ColorSeed to randomize the color palette
        vec3 c = palette(length(uv) + i + (0.1 + 0.2 * i) * TIME * Speed, ColorSeed) * d0;

        color += c;
    }

    return color;
}

// Anti-aliasing pass
vec3 antialias(in vec2 fragCoord) {
    const float AA_STAGES = 2.0;
    const float AA_TOTAL_PASSES = AA_STAGES * AA_STAGES + 1.0;
    const float AA_JITTER = 0.5;

    vec3 color = mainPass(fragCoord);
    for (float x = 0.0; x < AA_STAGES; x++) {
        for (float y = 0.0; y < AA_STAGES; y++) {
            vec2 offset = AA_JITTER * (2.0 * hash22(vec2(x, y)) - 1.0);
            color += mainPass(fragCoord + offset);
        }
    }
    return color / AA_TOTAL_PASSES;
}

// Gamma correction
vec3 gamma(in vec3 color) {
    return pow(color, vec3(1.0 / 2.2));
}

void main() {
    if (PASSINDEX == 0) {
        // First pass: No changes needed here.
    } else if (PASSINDEX == 1) {
        // Second pass: Render the scene
        vec3 color = antialias(gl_FragCoord.xy);
        color = gamma(color);
        gl_FragColor = vec4(color, 1.0);
    }
}