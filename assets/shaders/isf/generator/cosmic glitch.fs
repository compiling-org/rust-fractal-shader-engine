/*{
    "CATEGORIES": ["Fractal", "Psychedelic", "Dynamic", "Abstract", "Generative"],
    "DESCRIPTION": "A vibrant, dynamically evolving mathematical fractal with complex patterns and shifting colors. Uses a fixed-iteration loop for generative art to ensure compatibility with extremely strict GLSL ES 1.00 compilers.",
    "ISFVSN": "2.0",
    "INPUTS": [
        { "NAME": "AnimationSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Animation Speed" },
        { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Zoom Level" },
        { "NAME": "Density", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0, "LABEL": "Pattern Density" },
        { "NAME": "ColorShiftSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Shift Speed" },
        { "NAME": "HueStart", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Hue" },
        { "NAME": "HueRange", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Hue Range" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0, "LABEL": "Saturation" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Brightness" },
        { "NAME": "WarpFactor", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pattern Warp" }
    ]
}
*/

precision highp float;

#define PI 3.14159265359

// --- CRITICAL: FIXED FRACTAL ITERATION COUNT ---
// This MUST be a compile-time constant for your specific environment.
// You cannot change this via an INPUT slider.
#define FRACTAL_ITERATIONS 60 

// --- HSV to RGB Conversion (standard, no loops) ---
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 finalColor = vec3(0.0);
    
    float time = TIME * AnimationSpeed;

    // Scale and center UVs based on Zoom and Density
    vec2 z = uv * (4.0 / Zoom);
    z *= Density;

    // Add some dynamic warping to the coordinates
    z.x += sin(z.y * 5.0 + time * 0.7) * WarpFactor;
    z.y += cos(z.x * 5.0 + time * 0.9) * WarpFactor;

    // Initialize iteration parameter
    float iter_val = 0.0;

    // --- Main Fractal Iteration Loop ---
    // The loop limit is now a FIXED #define constant, which ISF compilers
    // expect for unrolling/optimization. This should compile.
    for (int i = 0; i < FRACTAL_ITERATIONS; i++) {
        // Simple iteration formula (can be modified for different fractals)
        z = sin(z * 1.5 + vec2(cos(time * 0.5), sin(time * 0.3))) + vec2(0.1); 

        // Accumulate a value based on the distance from origin
        iter_val += length(z); 
    }

    // Map the iterated value to a color
    float color_map_val = mod(iter_val * 0.1 + time * ColorShiftSpeed, 1.0);

    // Dynamic psychedelic hue shifting
    float hue = mod(HueStart + color_map_val * HueRange, 1.0);
    float saturation = Saturation;
    float value = fract(color_map_val * 2.0) * 0.8 + 0.2; // Create some light/dark variation

    finalColor = hsv2rgb(vec3(hue, saturation, value * Brightness)); 

    // Final color adjustments
    finalColor = pow(finalColor, vec3(0.85)); // Gamma correction
    finalColor = clamp(finalColor, 0.0, 1.0); // Ensure colors are valid

    gl_FragColor = vec4(finalColor, 1.0);
}