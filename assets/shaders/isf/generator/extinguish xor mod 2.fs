/*
{
  "DESCRIPTION": "Extinguish XL kaleidoscopic fractal fire with palettes, pulses, and morphing.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Kaleidoscope", "Psychedelic"],
  "INPUTS": [
    { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0001, "MAX": 0.1 }, 
    { "NAME": "DistortionScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "RotationSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "KaleidoSymmetry", "TYPE": "float", "DEFAULT": 1.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "DEBUG_MODE", "TYPE": "bool", "DEFAULT": true, "LABEL": "Enable Debug Mode" } 
  ]
}
*/

// Custom tanh function, as it's not a standard GLSL built-in.
vec4 tanh(vec4 x) {
    // Clamping the input to tanh to prevent numerical overflow issues
    // if 'x' becomes too large, which can lead to NaN or Inf results.
    x = clamp(x, vec4(-10.0), vec4(10.0)); // Clamp between -10 and 10 to keep exp() stable
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

// Generates different color palettes based on 'type' and 't' (time/position).
vec3 palette(float t, float type) {
    t = fract(t); // Ensure t is between 0 and 1
    if (type < 1.0) {
        // Palette 1: Smooth color shift
        return vec3(0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67))));
    } else if (type < 2.0) {
        // Palette 2: More dynamic, sine/cosine based colors
        return vec3(sin(t * 3.14159), cos(t * 6.28318), sin(t * 1.5));
    } else {
        // Palette 3: Linear gradient with some variation
        return vec3(t, 0.8 * t, 1.0 - t);
    }
}

void main() {
    // --- IMPORTANT CHANGE FOR COMPATIBILITY ---
    // Instead of using gl_FragCoord and RENDERSIZE directly,
    // which can behave inconsistently across different ISF hosts,
    // we use isf_FragNormCoord. This ISF-specific uniform provides
    // normalized coordinates (0.0 to 1.0) and is designed for portability.
    // We then center and adjust for aspect ratio.
    vec2 uv = (isf_FragNormCoord - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    float time = TIME * TimeMultiplier;

    // Kaleidoscopic effect:
    // Convert UV to polar coordinates (angle and radius).
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);
    // Modulo the angle by a fraction of PI to create symmetry.
    angle = mod(angle, 6.28318 / KaleidoSymmetry);
    // Convert back to Cartesian coordinates.
    uv = vec2(cos(angle), sin(angle)) * radius;

    // Rotation effect:
    float rot = time * RotationSpeed;
    // Apply a 2D rotation matrix to the UV coordinates.
    uv *= mat2(cos(rot), -sin(rot), sin(rot), cos(rot));

    float z = 0.0;
    float d = 0.0;
    float colorMod = 0.0;
    // --- KEY FIX: Initialize O.a (alpha) to 1.0 to ensure opacity ---
    vec4 O = vec4(0.0, 0.0, 0.0, 1.0); 
    vec3 baseColor;

    // Fixed iteration count for compatibility and performance consistency.
    const int ITERATIONS = 50;
    for (int i = 0; i < ITERATIONS; i++) {
        // Project a point 'p' based on 'z' and 'uv'.
        // The normalize function ensures 'p' is a unit vector.
        vec3 p = z * normalize(vec3(uv * 2.0, 0.0) - vec3(0.0, 0.0, 1.0));
        p.z += 6.0; // Offset the Z-component.

        // Apply a series of distortions to 'p'.
        // This creates the fractal-like visual complexity.
        float dj = 1.0;
        for (int j = 0; j < 9; j++) {
            // Add cosine waves to p, scaled by dj and DistortionScale.
            // p.yzx provides a component permutation for varied distortion.
            p += cos(p.yzx * dj - vec3(time, time * 0.5, 0.0)) / (dj * DistortionScale);
            dj *= 1.25; // Increase dj for subsequent iterations, affecting the scale of distortion.
        }

        // Update 'z' and 'd' based on the length of p.xz.
        z += d = 0.01 + 0.1 * length(p.xz);
        
        // Calculate color modulation based on 'z', 'time', and ColorPulse.
        colorMod = sin(z * 0.1 + time * ColorPulse);
        // Get the base color from the palette function.
        baseColor = palette(colorMod, PaletteSelect);
        
        // --- MODIFIED GLOW ACCUMULATION ---
        // Using d*d in the denominator for a sharper falloff,
        // which helps to make the glow more visible if d grows large quickly.
        // Also ensure d is not zero to prevent division by zero.
        O.rgb += baseColor * GlowIntensity / max(0.0001, d * d); // Use max to prevent division by zero
    }

    // Apply the custom tanh function to the final accumulated color.
    // This helps in mapping the potentially wide range of accumulated color values
    // into a more visually pleasing output range, often creating a glowing/bloom effect.
    gl_FragColor = tanh(O);

    // --- ENHANCED DEBUG MODE VISUALIZATION ---
    if (DEBUG_MODE) {
        // Visualize the raw accumulated 'O.rgb' value directly.
        // If this still results in a black screen, it confirms that 'O.rgb' is not accumulating
        // enough (or any) visible color data during the iterations.
        gl_FragColor = vec4(clamp(O.rgb * 0.1, 0.0, 1.0), 1.0); // Scale down for visualization
    }
}
