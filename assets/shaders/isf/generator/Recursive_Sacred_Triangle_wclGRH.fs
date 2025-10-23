/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Recursive Sacred Triangle with tunable parameters for customization.",
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

    // Time scaled by Speed
    float time = TIME * Speed;

    // Initialize color with background
    vec3 color = BackgroundColor.rgb;

    // Recursive Fractal Warping
    for (float i = 0.0; i < 5.0; i++) {
        uv = abs(uv) / dot(uv, uv) - (0.6 * MorphingIntensity);
        float d = length(uv);
        color += vec3(
            sin(time + i * 0.7) * 0.5 + 0.5,
            cos(time + i * 0.5) * 0.5 + 0.5,
            sin(time * 0.9) * 0.5 + 0.5
        ) * exp(-d * 4.5) * ColorIntensity;
    }

    // Geometric Distortion
    uv = fract(uv * 4.0 * Zoom) - 0.5;
    float pattern = length(uv);
    color += vec3(0.4, 0.5, 1.2) * exp(-pattern * 2.5);

    // Flowing Light Pulses
    uv += 0.2 * MorphingIntensity * sin(uv.yx * 3.5 + time);
    float pulse = length(uv);
    color += vec3(1.0, 0.3, 0.5) * exp(-pulse * 6.0);

    // Apply global intensity modulation
    color *= 1.7 + 0.6 * sin(TIME * 0.5 * Speed);

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}