/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Space-like depth, swirly nebula patterns, immersive fluid effect with tunable parameters.",
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
            "NAME": "BackgroundColor",
            "TYPE": "color",
            "DEFAULT": [0.2, 0.2, 0.2, 1.0]
        }
    ]
}
*/



void main() {
    // Normalized UV coordinates
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= Zoom; // Apply zoom effect

    // Time scaled by Speed
    float time = TIME * Speed;

    // Initialize color with background
    vec3 color = BackgroundColor.rgb;

    // Recursive fractal warping
    for (float i = 0.0; i < 10.0; i++) {
        // Rotation matrix for swirling effect
        mat2 rot = mat2(cos(time * 0.6), sin(time * 0.6), -sin(time * 0.6), cos(time * 0.6));
        uv = rot * uv;

        // Fractal distortion
        uv = abs(uv) / dot(uv, uv) - (0.55 * MorphingIntensity);

        // Add morphing effect
        uv += 0.15 * MorphingIntensity * sin(time + i * 0.3);

        // Distance-based color modulation
        float d = length(uv) + sin(uv.x * 15.0 + time * 2.2) * 0.12;
        color += vec3(
            sin(time + i * 0.9),
            cos(time * 0.7 + i * 0.4),
            tan(time * 1.3)
        ) * exp(-d * 6.8) * ColorIntensity;
    }

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}