/*
{
  "DESCRIPTION": "Liquid Crystal Mandala: An evolving, pulsating kaleidoscopic fractal with shimmering textures, deep palettes, and dynamic morphing. Adjusted for better visibility in ISF.video.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Kaleidoscope", "Psychedelic", "Abstract", "Liquid"],
  "INPUTS": [
    { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Animation Speed" },
    { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 0.0135, "MIN": 0.0001, "MAX": 0.05, "LABEL": "Light Emission" }, 
    { "NAME": "BaseDistortionScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Pattern Scale" },
    { "NAME": "WaveDistortionStrength", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Warp Strength" },
    { "NAME": "RotationSpeed", "TYPE": "float", "DEFAULT": 0.63, "MIN": 0.0, "MAX": 2.0, "LABEL": "Rotation Speed" },
    { "NAME": "KaleidoSymmetry", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 12.0, "LABEL": "Kaleidoscope Symmetry" },
    { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.60, "MIN": 0.0, "MAX": 2.0, "LABEL": "Color Palette" },
    { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 3.0, "LABEL": "Color Pulse Speed" },
    { "NAME": "DetailLayers", "TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 12.0, "LABEL": "Fractal Detail" },
    { "NAME": "DepthEffect", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Depth Morph" },
    { "NAME": "HueOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Global Hue Offset" }
  ]
}
*/

precision highp float;

#define PI 3.14159265359

vec4 tanh(vec4 x) {
    // Re-implementing tanh for vec4 as it's not a standard GLSL ES 1.00 function
    // Adding a small epsilon to denominator to prevent division by zero for very small x
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x) + 1e-5);
}

// Custom Palettes for vibrant, psychedelic colors
vec3 palette(float t, float type) {
    t = fract(t); // Ensure t is always between 0 and 1
    vec3 c;

    // Palette 1: Rainbow Spectrum
    if (type < 1.0) {
        c = 0.5 + 0.5 * cos(2.0 * PI * (t + vec3(0.0, 0.33, 0.67)));
    } 
    // Palette 2: Fire/Plasma
    else if (type < 2.0) {
        c = vec3(pow(t, 2.0), pow(t, 0.8) * 0.7, pow(t, 0.5) * 0.4); // Reds, oranges, yellows
        c = mix(c, vec3(1.0, 0.5, 0.0), sin(t * PI * 2.0) * 0.5 + 0.5); // Add fiery glow
    } 
    // Palette 3: Deep Ocean/Mystic
    else {
        c = vec3(0.1 + 0.4 * sin(t * 3.0), 0.2 + 0.6 * cos(t * 2.5), 0.7 + 0.3 * sin(t * 1.8));
        c = mix(c, vec3(0.0, 0.8, 1.0), abs(sin(t * PI * 4.0))); // Aqua highlights
    }
    return c;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * TimeMultiplier;

    // --- Kaleidoscope Effect ---
    // Fold the UV space into a pie slice
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);
    angle = mod(angle, 2.0 * PI / KaleidoSymmetry); // Fold to sector
    angle = abs(angle - PI / KaleidoSymmetry); // Mirror within sector
    
    uv = vec2(cos(angle), sin(angle)) * radius; // Convert back to Cartesian

    // --- Global Rotation ---
    float rot = time * RotationSpeed;
    uv *= mat2(cos(rot), -sin(rot), sin(rot), cos(rot));

    float z_depth = 0.0;
    float color_mod_value = 0.0;
    vec4 final_O_accum = vec4(0.0); // Accumulator for final color


    // --- Main Iteration Loop ---
    const int ITERATIONS = 70; 
    for (int i = 0; i < ITERATIONS; i++) {
        // Project UV into 3D space, scaled by current depth
        vec3 p = z_depth * normalize(vec3(uv * BaseDistortionScale, 0.0) - vec3(0.0, 0.0, 1.0));
        p.z += time * 0.5; // Constant Z translation for flow

        // --- Nested Fractal Distortion Loop ---
        const int MAX_DETAIL_LAYERS = 12; 
        float dj = 1.0;
        for (int j = 0; j < MAX_DETAIL_LAYERS; j++) {
            if (float(j) < DetailLayers) { 
                p.x += sin(p.y * dj + time * 0.3) * WaveDistortionStrength / dj;
                p.y += cos(p.z * dj + time * 0.4) * WaveDistortionStrength / dj;
                p.z += sin(p.x * dj + time * 0.5) * WaveDistortionStrength / dj;
            }
            dj *= 1.2; 
        }

        // Calculate a density/distance metric for the current point
        // Increased base value of d_step to keep it from getting too small too fast,
        // which could cause division by zero or very large values that clip.
        float d_step = 0.1 + 0.1 * length(p.xz); // Increased 0.01 to 0.1
        
        // Morph the depth effect for a more liquid/plasma feel
        z_depth += d_step * (1.0 + sin(time * 0.8 + z_depth * 0.1) * DepthEffect);
        
        // --- Color Modulation ---
        color_mod_value = sin(z_depth * 0.15 + time * ColorPulseSpeed);
        
        vec3 current_palette_color = palette(mod(color_mod_value + HueOffset, 1.0), PaletteSelect);

        // Accumulate color, weighting by density/distance
        final_O_accum.rgb += current_palette_color * GlowIntensity / d_step;
    }

    // --- Final Output ---
    // Apply tanh for glow and explicitly set alpha to 1.0
    vec4 final_color = tanh(final_O_accum);
    final_color.a = 1.0; // Ensure alpha is always opaque

    // Add a very small, constant non-zero value to prevent pure black in edge cases.
    // This is a last resort to ensure *something* is visible if the main logic still produces very low values.
    final_color.rgb = max(final_color.rgb, vec3(0.005)); // Ensures a faint glow even if calculations go to zero

    gl_FragColor = final_color;
}