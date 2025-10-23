/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Fractal",
        "Converted from Shadertoy"
    ],
    "DESCRIPTION": "Psychedelic fractal shader with tunable parameters for customization.",
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
        },
        {
            "NAME": "ArtifactStrength",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
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
    for (float i = 0.0; i < 8.0; i++) {
        // Rotation matrix for swirling effect
        mat2 rot = mat2(
            sin(time * 0.4), cos(time * 0.4),
            -cos(time * 0.4), sin(time * 0.4)
        );
        uv = rot * uv;

        // Fractal distortion
        uv = abs(uv) / dot(uv, uv) - (0.5 * MorphingIntensity);

        // Distance-based color modulation
        float d = length(uv) + tan(uv.y * 16.0 + time * 2.2) * 0.09;
        color += vec3(
            sin(time + i * 0.7),
            cos(time + i * 0.6),
            tan(time * 1.1)
        ) * exp(-d * 7.5) * ColorIntensity;

        // Add artifact effects
        color += ArtifactStrength * vec3(
            fract(sin(d * 100.0 + time) * 1000.0),
            fract(cos(d * 100.0 + time) * 1000.0),
            fract(tan(d * 100.0 + time) * 1000.0)
        );
    }

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}