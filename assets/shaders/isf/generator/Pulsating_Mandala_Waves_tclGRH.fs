/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Pulsating Mandala Waves with tunable parameters for customization.",
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
            "NAME": "SoundReactivity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
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

    // Concentric Wave Pattern
    for (float i = 0.0; i < 6.0; i++) {
        uv = sin(uv * Zoom * 3.5 + time * 0.4) * 0.8;
        float d = length(uv);
        color += vec3(
            sin(time + i * 0.6) * 0.5 + 0.5,
            cos(time + i * 0.4) * 0.5 + 0.5,
            sin(time * 0.8) * 0.5 + 0.5
        ) * exp(-d * 4.0) * ColorIntensity;
    }

    // Refracted Grid Distortion
    uv = fract(uv * 5.0) - 0.5;
    float grid = length(uv);
    color += vec3(0.2, 0.8, 1.0) * exp(-grid * 3.0);

    // Morphing Pulses
    uv += MorphingIntensity * 0.3 * sin(uv.yx * 5.0 + time);
    float morph = length(uv);
    color += vec3(0.9, 0.2, 0.6) * exp(-morph * 5.0);

    // Apply global intensity modulation
    color *= 1.6 + 0.5 * sin(TIME * 0.7 * Speed);

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}