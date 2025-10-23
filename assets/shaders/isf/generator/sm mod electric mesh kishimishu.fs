/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Electric Mesh by @kishimisu (2023). Enhanced with tunable parameters for animation speed, zoom, color morphing, and interactivity.",
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
        {}
    ]
}
*/

void main() {
    // Declare output color
    vec4 O = vec4(0.0);

    // Normalize UV coordinates using RENDERSIZE
    vec2 u = abs(gl_FragCoord.xy + gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    O *= 0.0;

    // Tunable parameters
    float speed = Speed; // Controls animation speed
    float zoom = Zoom;   // Controls zoom level
    float colorSeed = ColorSeed; // Randomizes color palette

    // Adjust UV coordinates for zoom
    u *= zoom;

    // Main loop for electric mesh effect
    for (float i = 0.0, t = TIME * speed * 0.5; i < 46.0; ) {
        // Calculate distance field
        float d = abs(abs(u.x - sin(t + i * 0.17) * 0.7) + 
                      u.y - sin(t + i * 0.1) * 0.6);

        // Add color shift and pulses using ColorSeed
        vec4 colorShift = cos(i + vec4(colorSeed, colorSeed + 1.0, colorSeed + 2.0, colorSeed)) + 1.2;

        // Accumulate color
        O += 0.002 / abs(d) * colorShift;

        // Increment loop counter
        i++;
    }

    // Output the final color
    gl_FragColor = O;
}