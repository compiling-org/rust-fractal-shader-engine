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

#define PI 3.141592
#define LINES 12.
mat2 rotate(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

void main() {
    // Normalize UV coordinates using RENDERSIZE
    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;

    // Initialize output color
    vec4 fragColor = vec4(0.);

    // Apply zoom level
    float zoom = Zoom; // Controls zoom level
    uv /= vec2(1.1 * dot(uv, 0.01 * uv)) * zoom;

    // Animation speed
    float speed = Speed; // Controls animation speed

    // Main loop for electric mesh effect
    for (float i = 0.; i < LINES; i++) {
        // Rotate UV coordinates based on time and speed
        uv *= rotate(TIME * speed / 20.);

        // Compute x and y values for the pattern
        float x = 8.79 * sin(0.01 * uv.x - TIME * speed) * uv.y * 2.1;
        float y = length(0.1 * log(abs(uv)));

        // Define point p
        vec2 p = vec2(x, y);

        // Add color shift and pulses using ColorSeed
        vec3 col = cos(vec3(-2, 0, -1) * PI * 2. / 3. + PI * i * TIME * speed / 10. + i / 70. + ColorSeed) * 0.5 + 0.5;

        // Accumulate color
        fragColor += vec4(21.264 / length(uv - p * 0.9) * col, 2.45);
    }

    // Apply gamma correction
    fragColor.xyz = pow(abs(fragColor.xyz), vec3(2.45));

    // Set alpha to 1.0
    fragColor.w = 1.0;

    // Output the final color
    gl_FragColor = fragColor;
}