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
            "NAME": "ColorIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "BackgroundColor",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.1, 0.1, 1.0]
        }
    ]
}
*/



void main() {
    // Calculate screen coordinates
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv -= 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= Zoom; // Apply zoom effect

    // Calculate moire pattern
    vec2 moire = vec2(length(uv), atan(uv.x, uv.y));

    // Calculate fractal geometry
    float fractal = 0.0;
    for (int i = 5; i < 70; i++) {
        fractal += 0.25 * abs(sin(moire.x * 5.0 + TIME * Speed));
    }

    // Calculate psychedelic colors
    vec3 color = BackgroundColor.rgb;
    color += vec3(1.0, 0.5, 0.0) * sin(moire.y * 12.0 + fractal) * ColorIntensity;
    color += vec3(0.0, 1.0, 0.5) * cos(moire.x * 24.0 + fractal) * ColorIntensity;
    color += vec3(0.5, 0.0, 1.0) * sin(moire.y * 24.0 + fractal) * ColorIntensity;

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}