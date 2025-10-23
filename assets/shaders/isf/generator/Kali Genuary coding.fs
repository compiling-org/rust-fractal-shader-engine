/*
{
  "DESCRIPTION": "Psychedelic 3D fractal with animated geometry, color pulses, and tunable parameters. Features selectable color palettes and refined fractal iteration control.",
  "CATEGORIES": ["Raymarching", "Psychedelic", "Fractal", "Animation", "Shader"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Animation Speed" },
    { "NAME": "geoTwist", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Geometry Twist" },
    { "NAME": "camOrbit", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Orbit Radius" },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Main Color Pulse Rate" },
    
    { "NAME": "FractalMorphStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Morph Strength" },
    { "NAME": "FractalMorphSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Morph Speed" },
    { "NAME": "FractalMorphOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0, "LABEL": "Fractal Morph Offset" },
   
    { "NAME": "FractalIterationsCount", "TYPE": "float", "DEFAULT": 30, "MIN": 10, "MAX": 200, "LABEL": "Fractal Iterations (Performance)" },

    { "NAME": "FlashFrequency", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Flash Frequency (0 for off)" },
    { "NAME": "FlashIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Flash Intensity" },
    { "NAME": "FlashMinBrightness", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Flash Min Brightness" },

    { "NAME": "PaletteMode", "TYPE": "float", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette Mode", "LABELS": ["Rainbow Flow", "Complementary Pulse", "Triadic Harmony", "Acid Trip Glitch"] },
    { "NAME": "PaletteBlendSpeed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0, "LABEL": "Palette Blend Speed" }, 
    { "NAME": "HueShiftSpeed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Global Hue Shift Speed" },
    { "NAME": "PulseHueAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Hue Amplitude" },
    { "NAME": "PulseSaturationAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Saturation Amplitude" },
    { "NAME": "PulseValueAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Value Amplitude" }
  ]
}
*/

#define PI 3.14159265359

float det = .01;
float maxdist = 30.;
vec3 m; // Global variable for fractal glow/density contribution

// Rotation matrix utility
mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Converts HSV (Hue, Saturation, Value) color to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Distance Estimator (DE) function for the 3D fractal
float de(vec3 p, float t, float twist) {
    vec3 pp = p; // Original point for reference if needed
    float d = 1.0; // Accumulator for scaling
    m = vec3(1000.0); // Initialize minimum distance for fractal glow calculation
    
    // Initial sphere-like distance for overall scene
    float md = length(p) - 3.0;

    // Apply initial vertical offset
    p.y += 2.0;
    
    float c = cos(t);
    float s = sin(t);

    // Apply geometric twists based on time and input parameters
    p.xz *= rot(t * 2.0 + p.y * 0.3 * twist);
    p.yz *= rot(s * c * c * s * twist);

    // Fractal iteration loop (Mandelbox-like)
    for (int i = 0; i < 200; i++) { // Max iterations match slider max
        // Exit early if current iteration exceeds the tunable count.
        if (i >= int(FractalIterationsCount)) break; 

        // Fractal folding operations
        p.xz = abs(p.xz) - 0.5;
        p.xy *= rot(-1.0 + c * c * c); // Rotating based on time
        p.xz *= rot(0.5); // Fixed rotation

        d *= 1.2; // Scaling factor
        p *= 1.2; // Scale the point

        // Apply time-dependent and morphing offset
        p -= 1.2 - s * s * 0.2 + sin(t * FractalMorphSpeed + float(i)) * FractalMorphStrength + FractalMorphOffset;
        
        // Accumulate minimum distances for glow/density (m)
        if (i < 5) m = min(m, abs(p.yzy)); 
        
        // Update minimum distance to the fractal surface
        md = min(md, (length(p) - 0.1) / d);
    }

    // Apply exponential falloff for fractal glow/density
    m = exp(-2.0 * m);
    
    return md * 0.5 - 0.2; // Final scaled distance
}

// Raymarching function to traverse the scene and accumulate color
vec3 march(vec3 from, vec3 dir, float t, float pulseRate) {
    float d, td = 0.0; // d: distance step, td: total distance traveled
    vec3 p, col = vec3(0.0); // p: current ray position, col: accumulated color

    // Raymarching loop (max 300 steps for performance)
    for (int i = 0; i < 300; i++) {
        p = from + dir * td; // Current point along the ray
        d = de(p, t, geoTwist); // Get distance to the fractal surface
        
        // Ensure distance is positive and above a minimum threshold to prevent artifacts
        d = max(0.003, abs(d)); 
        
        if (td > maxdist) break; // Break if ray travels too far (out of bounds)
        
        // Main color accumulation logic
        float current_pulse = sin(t * pulseRate + length(p) * 0.1);
        float base_hue_offset = fract(t * HueShiftSpeed + current_pulse * PulseHueAmplitude);

        vec3 fractal_color;

        // Apply different color palettes based on PaletteMode using if/else if
        if (int(PaletteMode) == 0) { // Rainbow Flow - Vibrant, continuous, high saturation/value
            fractal_color = hsv2rgb(vec3(fract(base_hue_offset + t * PaletteBlendSpeed * 1.5), // Faster internal blend
                                         0.95 + current_pulse * 0.05 * PulseSaturationAmplitude, // High saturation, subtle pulse
                                         0.9 + current_pulse * 0.1 * PulseValueAmplitude));    // High value
        } else if (int(PaletteMode) == 1) { // Complementary Pulse - Strong primary/complementary contrast
            float h1 = fract(base_hue_offset + t * PaletteBlendSpeed * 0.8); // Slower, more deliberate pulse
            float h2 = fract(h1 + 0.5); // Complementary hue
            fractal_color = hsv2rgb(vec3(mix(h1, h2, abs(sin(t * PaletteBlendSpeed * 3.0 + length(p) * 0.02))), // More distinct switch based on time & position
                                         1.0, // Full saturation for strong colors
                                         0.7 + current_pulse * 0.3 * PulseValueAmplitude)); // Medium value, more value pulse
        } else if (int(PaletteMode) == 2) { // Triadic Harmony - Cycles through three distinct hues
            float h_base = fract(base_hue_offset + t * PaletteBlendSpeed * 0.6); // Moderate blend speed
            vec3 tri_hues = vec3(h_base, fract(h_base + 1.0/3.0), fract(h_base + 2.0/3.0));
            
            // Ensure integer-like index for clear hue selection
            float hue_index = floor(mod(t * PaletteBlendSpeed * 2.0 + length(p) * 0.08, 3.0)); 
            float selected_hue;
            if (hue_index == 0.0) selected_hue = tri_hues.x; 
            else if (hue_index == 1.0) selected_hue = tri_hues.y; 
            else selected_hue = tri_hues.z; 

            fractal_color = hsv2rgb(vec3(selected_hue, 
                                         0.85 + current_pulse * 0.15 * PulseSaturationAmplitude, // Good saturation, subtle pulse
                                         0.75 + current_pulse * 0.25 * PulseValueAmplitude));    // Medium value, more value pulse
        } else if (int(PaletteMode) == 3) { // Acid Trip Glitch - Chaotic, high contrast, fast flicker
            float glitch_factor = fract(sin(t * 15.0 + length(p) * 0.3) * 200.0); // More frequent/intense glitches
            fractal_color = hsv2rgb(vec3(fract(base_hue_offset + glitch_factor * PaletteBlendSpeed * 2.5), // Faster, more erratic hue shifts
                                         1.0, // Max saturation
                                         0.5 + current_pulse * 0.5 * PulseValueAmplitude)); // Brighter value, strong value pulse
            fractal_color *= (0.6 + sin(t * 35.0) * 0.4); // More aggressive, faster flicker
        } else { // Fallback (should not be hit if input range is 0-3)
            fractal_color = hsv2rgb(vec3(fract(base_hue_offset), 0.5, 0.5)); // Distinct fallback color
        }

        // Accumulate color based on distance, fractal glow (m), and calculated color
        col += exp(-0.1 * d) * 0.02 * m * exp(-0.2 * td) * fractal_color;
        
        td += d; // Advance the ray
    }

    // Final color post-processing
    col.rb *= rot(0.5); // Apply a fixed rotation to red and blue channels for a color shift
    col = mix(length(col) * vec3(0.5), col, 0.7); // Mix between luminance and the full color for contrast
    
    return col;
}

void main() {
    // Normalize fragment coordinates to UV space (0 to 1, centered)
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    // Calculate time with global animation speed
    float t = TIME * speed;

    // Camera position: orbiting around the origin in XZ plane, fixed Y
    vec3 from = vec3(sin(t) * camOrbit, cos(t * 0.5) * 0.5, -8.0);
    // Ray direction: from camera through UV point, with a fixed Z depth for perspective
    vec3 dir = normalize(vec3(uv, 0.5));

    // Perform raymarching to get the color for this fragment
    vec3 final_color = march(from, dir, t, colorPulse);

    // Apply flashing effect
    if (FlashFrequency > 0.0) {
        float flash_factor = sin(t * FlashFrequency * PI * 2.0) * 0.5 + 0.5; 
        flash_factor = mix(FlashMinBrightness, 1.0, flash_factor); 
        final_color *= (1.0 - FlashIntensity) + flash_factor * FlashIntensity; 
    }

    // Set the final fragment color
    gl_FragColor = vec4(final_color, 1.0);
}

