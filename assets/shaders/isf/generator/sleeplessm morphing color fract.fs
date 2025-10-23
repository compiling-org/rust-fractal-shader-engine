/*{
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
            "DEFAULT": 1,
            "MAX": 5,
            "MIN": 0.1,
            "NAME": "Speed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 5,
            "MIN": 0.1,
            "NAME": "Zoom",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 5,
            "MIN": 0.1,
            "NAME": "MorphingIntensity",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 3,
            "MIN": 0.1,
            "NAME": "ColorIntensity",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                0,
                0,
                0,
                1
            ],
            "NAME": "BackgroundColor",
            "TYPE": "color"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 1,
            "MIN": 0,
            "NAME": "ArtifactStrength",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
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
    for (float i = 0.0; i < 9.0; i++) {
        // Rotation matrix for swirling effect
        mat2 rot = mat2(
            cos(time * 0.3), sin(time * 0.3),
            -sin(time * 0.3), cos(time * 0.3)
        );
        uv = rot * uv;

        // Fractal distortion
        uv = abs(uv) / dot(uv, uv) - (0.5 * MorphingIntensity);

        // Add morphing effect
        uv += 0.1 * MorphingIntensity * sin(time + i * 0.2);

        // Distance-based color modulation
        float d = length(uv) + cos(uv.y * 18.0 + time * 2.3) * 0.1;
        color += vec3(
            sin(time + i * 0.6),
            tan(time + i * 0.4),
            cos(time * 1.1)
        ) * exp(-d * 5.8) * ColorIntensity;

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