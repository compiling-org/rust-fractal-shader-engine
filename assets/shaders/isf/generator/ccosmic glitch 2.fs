/*{
    "CATEGORIES": ["Psychedelic", "Geometric", "Abstract", "Vibrant", "Hypnotic", "Grid"],
    "DESCRIPTION": "A sharp, vibrant, and hypnotic radial pattern that warps and pulses with intense color cycling. Parameters are designed for obvious visual impact.",
    "ISFVSN": "2.0",
    "INPUTS": [
        { "NAME": "OverallSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Overall Animation Speed" },
        { "NAME": "PatternDensity", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "LABEL": "Pattern Density (Rings)" },
        { "NAME": "SpokeDensity", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Spoke Density" },
        { "NAME": "WarpMagnitude", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pattern Warp Magnitude" },
        { "NAME": "ColorCycleSpeed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0, "LABEL": "Color Cycle Speed" },
        { "NAME": "HueStart", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Hue" },
        { "NAME": "HueRange", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Hue Spread" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Saturation" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 3.0, "LABEL": "Brightness" },
        { "NAME": "LineThickness", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.2, "LABEL": "Line Thickness" }
    ]
}
*/

precision highp float;

#define PI 3.14159265359

// --- HSV to RGB Conversion ---
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 finalColor = vec3(0.0);
    
    float time = TIME * OverallSpeed;

    // --- Polar Coordinates ---
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);

    // --- Dynamic Warping ---
    // Apply a strong sine wave warp based on time and angle/radius
    float warp_x = sin(angle * SpokeDensity * 2.0 + time * 1.5) * WarpMagnitude;
    float warp_y = cos(radius * PatternDensity * 0.5 + time * 1.2) * WarpMagnitude;

    float warped_angle = angle + warp_x;
    float warped_radius = radius + warp_y;

    // --- Pattern Generation ---
    // Create sharp, repeating patterns using fractional parts and modulo.

    // Radial Rings: Based on warped_radius
    float ring_pattern = fract(warped_radius * PatternDensity - time * 0.2); // Animate rings inwards/outwards
    float ring_line = smoothstep(0.5 - LineThickness, 0.5 + LineThickness, ring_pattern);
    ring_line = abs(ring_line * 2.0 - 1.0); // Create a pulse for the line itself

    // Angular Spokes: Based on warped_angle
    float spoke_pattern = fract(warped_angle * SpokeDensity / (2.0 * PI)); // Normalize angle to 0-1
    float spoke_line = smoothstep(0.5 - LineThickness, 0.5 + LineThickness, spoke_pattern);
    spoke_line = abs(spoke_line * 2.0 - 1.0);

    // Combine patterns - choose based on effect (add, multiply, max)
    // Using sum for a grid-like overlay effect
    float pattern_value = ring_line + spoke_line;
    pattern_value = clamp(pattern_value, 0.0, 1.0); // Ensure valid range

    // Emphasize lines (invert and brighten)
    pattern_value = 1.0 - pattern_value; // Invert so lines are brighter

    // --- Psychedelic Color Mapping ---
    // Directly map the pattern value and time to a vibrant HSV color.
    float hue = mod(HueStart + pattern_value * HueRange + time * ColorCycleSpeed, 1.0);
    float saturation = Saturation;
    float value = pattern_value * Brightness; // Direct brightness control from pattern

    finalColor = hsv2rgb(vec3(hue, saturation, value)); 
    
    // Final tone mapping
    finalColor = pow(finalColor, vec3(0.7)); // Gamma correction for extra pop
    finalColor = clamp(finalColor, 0.0, 1.0); // Final clamp to valid color range

    gl_FragColor = vec4(finalColor, 1.0);
}