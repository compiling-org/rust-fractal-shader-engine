/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Pulsating Hexagonal Fractal with tunable parameters for customization.",
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
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= Zoom; // Apply zoom effect

    // Time scaled by Speed
    float time = TIME * Speed;

    // Recursive fractal warping
    for (float i = 1.0; i < 5.0; i++) {
        uv = abs(uv) / dot(uv, uv) - (0.5 * MorphingIntensity); // Apply morphing intensity
    }

    // Generate color based on sine waves
    float col = sin(uv.x * 8.0 + time) * cos(uv.y * 8.0 - time);

    // Apply psychedelic color effects
    vec3 color = BackgroundColor.rgb;
    color += vec3(
        0.5 + 0.5 * sin(col * 6.0 * ColorIntensity), // Red channel
        0.5 + 0.5 * cos(col * 6.0 * ColorIntensity), // Green channel
        0.5 + 0.5 * sin(col * 6.0 * ColorIntensity + time) // Blue channel
    );

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}