/*
{
    "CATEGORIES": [
        "Fractal",
        "3D",
        "Animated",
        "Abstract",
        "Psychedelic"
    ],
    "DESCRIPTION": "An ISF conversion of a Shadertoy fractal, featuring intricate repeating 3D structures with enhanced dynamic motion and vibrant colors. Includes comprehensive controls for animation speed, zoom, camera, fractal geometry, multiple color palettes, and post-processing effects like glitch, shake, brightness, saturation, and contrast.",
    "CREDIT": "Original shader by unknown/golfed Shadertoy (similar to Mandelbox/Menger forms), converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "xy_control_x", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "X-component for camera rotation/offset (normalized 0-1, maps to 0-2PI)." },
        { "NAME": "xy_control_y", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Y-component for camera rotation/offset (normalized 0-1, maps to 0-PI)." },
        { "NAME": "raymarch_iterations", "TYPE": "float", "DEFAULT": 110.0, "MIN": 20.0, "MAX": 250.0, "STEP": 1.0, "DESCRIPTION": "Number of raymarching steps. Higher values increase detail but reduce performance." },
        { "NAME": "fractal_repetitions", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 15.0, "STEP": 1.0, "DESCRIPTION": "Number of fractal folding repetitions for detail." },
        { "NAME": "z_scroll_speed", "TYPE": "float", "DEFAULT": 13.5, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Speed of Z-axis scrolling/translation." },
        { "NAME": "mod_size", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Size of the repeating module in the fractal." },
        { "NAME": "abs_reflect_dist", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Distance for the absolute reflection step." },
        { "NAME": "scale_oscillate_speed", "TYPE": "float", "DEFAULT": 0.234, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Speed of the scaling factor oscillation." },
        { "NAME": "offset_x_base", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Base X offset for the fractal's center." },
        { "NAME": "offset_x_osc_mult", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Oscillation multiplier for X offset." },
        { "NAME": "offset_x_osc_speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of X offset oscillation." },
        { "NAME": "offset_x_cos_speed_mult", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Cosine speed multiplier for X offset (adds chaotic motion)." },
        { "NAME": "offset_y_base", "TYPE": "float", "DEFAULT": 120.0, "MIN": 0.0, "MAX": 200.0, "DESCRIPTION": "Base Y offset for the fractal's center (large value for distant view)." },
        { "NAME": "offset_z_base", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.0, "MAX": 15.0, "DESCRIPTION": "Base Z offset for the fractal's center." },
        { "NAME": "offset_z_osc_mult", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Oscillation multiplier for Z offset." },
        { "NAME": "offset_z_osc_speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of Z offset oscillation." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 7 psychedelic color palettes." },
        { "NAME": "color_mix_white", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for white in the color accumulation (0=full color, 1=more white)." },
        { "NAME": "color_hue_mult", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Multiplier for hue calculation (affects color shift speed)." },
        { "NAME": "color_intensity_divisor", "TYPE": "float", "DEFAULT": 8000.0, "MIN": 100.0, "MAX": 20000.0, "DESCRIPTION": "Divisor for final color intensity. Higher values make it dimmer." },
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of color pulsing (used in color mix factor)." },
        { "NAME": "color_mix_sin_mult", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Multiplier for sine wave in color mixing (adds pulsing intensity)." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." }
    ]
}
*/

#define R(p,a,r)mix(a*dot(p,a),p,cos(r))+sin(r)*cross(p,a)
#define H(h)(cos((h)*6.3+vec3(0,23,21))*.5+.5)

// Compact color palette function
vec3 getColorPalette(float t, float type) {
    if (type < 0.5) return H(t); // Palette 0: Default Psychedelic Flow (original H)
    if (type < 1.5) return vec3(sin(t * 5.0), sin(t * 7.0 + 1.0), sin(t * 9.0 + 2.0)) * 0.5 + 0.5; // Palette 1: Rapid Sine Waves
    if (type < 2.5) return vec3(cos(t * 4.0 + 2.0), cos(t * 2.0 + 1.0), sin(t * 6.0)) * 0.5 + 0.5; // Palette 2: Muted Cosine Blends
    if (type < 3.5) return vec3(sin(t * 2.0), sin(t * 4.0), cos(t * 8.0)) * 0.5 + 0.5; // Palette 3: Fast RGB Pulse
    if (type < 4.5) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0)); // Palette 4: Hard Edge Fractal Colors
    if (type < 5.5) return vec3(sin(t * 1.5), cos(t * 3.0), sin(t * 4.5 + cos(t * 2.0))) * 0.5 + 0.5; // Palette 5: Organic Swirl
    return mix(H(t * 0.7), H(t * 1.3 + 0.5), 0.5); // Palette 6: Dual Hue Blend
}

// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); // Adjust contrast around 0.5 (mid-gray)
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); // Calculate grayscale luminance
    color = mix(gray, color, sat); // Adjust saturation by mixing with grayscale
    return color * br; // Adjust brightness
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }


void main() {
    vec4 fragColor = vec4(0); // Initialize output color to black
    vec2 fragCoord = gl_FragCoord.xy; // Corresponds to 'C' in Shadertoy

    // RENDERSIZE is a standard ISF uniform (vec2)
    vec3 iResolution_vec3 = vec3(RENDERSIZE.xy, 1.0); // Corresponds to 'r' in Shadertoy

    float current_time = TIME * speed; // Global time with speed control

    vec3 p; // Main fractal position vector
    float a_val; // Corresponds to 'a' in original (used for mod_size)
    float s_val; // Corresponds to 's' in original (inner scale)
    float e_val; // Corresponds to 'e' in original (step estimate / scale factor)
    float g_val = 0.0; // Corresponds to 'g' in original (accumulated ray length)
    
    vec3 accumulatedColor = vec3(0.0); // Corresponds to 'O.xyz' in original

    // Directly translate original ray direction calculation:
    // d=normalize(vec3((C*2.-r.xy)/r.y,1));
    // C*2.-r.xy maps fragCoord (0 to RENDERSIZE) to (-RENDERSIZE to RENDERSIZE) range,
    // centered at 0. Then divide by RENDERSIZE.y to normalize relative to height.
    vec3 rayDirection = normalize(vec3((fragCoord * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y, 1.0));

    // Apply camera rotation based on xy_control_vec
    // Use the xy_control_x/y inputs (0-1 range) to control camera view.
    // 6.2831853 is 2 * PI (for full horizontal rotation)
    // 3.1415926 is PI (for half vertical rotation, preventing flip)
    vec2 xy_control_vec = vec2(xy_control_x, xy_control_y);
    rayDirection = R(rayDirection, vec3(0.577), xy_control_vec.x * 6.2831853);
    rayDirection = R(rayDirection, vec3(0.577), xy_control_vec.y * 3.1415926);

    // Apply camera shake directly to initial ray direction UVs
    if (shake_strength > 0.001) {
        // Use normalized UV for shake offset calculation to keep it resolution independent
        vec2 shake_uv = (fragCoord / RENDERSIZE.xy) - 0.5; // [-0.5, 0.5] range
        shake_uv.x *= RENDERSIZE.x / RENDERSIZE.y; // Maintain aspect ratio
        
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency + hash11(1.0)) * 0.1,
            cos(current_time * shake_frequency * 1.1 + hash11(2.0)) * 0.1
        ) * shake_strength;
        
        // Apply shake by rotating the ray direction slightly
        rayDirection = R(rayDirection, normalize(vec3(shake_offset.x, shake_offset.y, 0.0)), length(shake_offset) * 0.1);
    }

    // Apply glitch effect directly to initial ray direction UVs (pre-raymarching)
    if (glitch_strength > 0.001) {
        vec2 glitch_uv = fragCoord / RENDERSIZE.xy;
        float offset_x_noise = (hash22(glitch_uv * 10.0 + current_time * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(glitch_uv.y * 150.0 + current_time * 20.0) * 0.5 + 0.5;
        
        // Shift rayDirection's horizontal component
        rayDirection.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
        rayDirection = normalize(rayDirection); // Re-normalize after distortion
    }


    // Main raymarching loop: for(float i=0.,a,s,e,g=0.; ++i<110.; O.xyz+=...)
    for(int j = 0; j < int(raymarch_iterations); j++) {
        float i = float(j + 1); // 'i' in original loop starts from 1.0
        
        p = g_val * rayDirection; // p=g*d;

        p.z += current_time * z_scroll_speed; // p.z+=iTime*13.5; (updated from previous 6.5)

        a_val = mod_size; // a=10.; (tunable)
        p = mod(p - a_val, a_val * 2.0) - a_val; // p=mod(p-a,a*2.)-a;

        s_val = 6.0; // s=6.; (inner scale base)

        // Inner loop: for(int i=0;i++<8;){ ... }
        for(int k = 0; k < int(fractal_repetitions); k++) { // Loop 8 times (tunable)
            p = abs_reflect_dist - abs(p); // p=.3-abs(p); (tunable abs_reflect_dist)
            
            // Branching operations for folding/swapping axes
            // p.x<p.z?p=p.zyx:p;
            if (p.x < p.z) p = p.zyx;
            // p.z<p.y?p=p.xzy:p;
            if (p.z < p.y) p = p.xzy;
            // NEW IN THIS SHADER: p.y<p.x?p=p.zyx:p;
            if (p.y < p.x) p = p.zyx; 
            
            // s*=e=1.4+sin(iTime*.234)*.1; (e is both scale factor and a 'distance' component)
            e_val = 1.4 + sin(current_time * scale_oscillate_speed) * 0.1; // (tunable scale_oscillate_speed)
            s_val *= e_val; // Accumulate scale
            
            // p=abs(p)*e - vec3(5.+cos(iTime*.3+.5*cos(iTime*.3))*3.,120,8.+cos(iTime*.5)*5.);
            // Apply scale and complex dynamic offset. Note: tan changed to cos in X offset here.
            p = abs(p) * e_val - 
                vec3(
                    offset_x_base + cos(current_time * offset_x_osc_speed + offset_x_cos_speed_mult * cos(current_time * offset_x_osc_speed)) * offset_x_osc_mult, // Tunable, and tan->cos
                    offset_y_base, // Tunable
                    offset_z_base + cos(current_time * offset_z_osc_speed) * offset_z_osc_mult // Tunable
                );
        }
        
        // Raymarching step: g+=e=length(p.yz)/s;
        // The 'e' here is the final step distance for the raymarcher
        e_val = length(p.yz) / s_val; // Using p.yz as per original and confirmed by comment
        g_val += e_val; // Accumulate ray length

        // Color accumulation: O.xyz+=mix(vec3(1),H(g*.1),sin(.8))*1./e/8e3
        // This color logic is quite unique.
        // g*.1 for hue: Use g_val (current ray length) to get a color from H function
        float hue_input = g_val * color_hue_mult; // Tunable color_hue_mult
        vec3 fractal_color = getColorPalette(hue_input, color_palette_type); // Tunable palette
        
        // sin(.8) in original for mix factor - now using tunable `color_pulse_speed` and `color_mix_sin_mult`
        float color_mix_factor = sin(current_time * color_pulse_speed + g_val * 0.01) * color_mix_sin_mult; // Tunable
        color_mix_factor = clamp(color_mix_factor, 0.0, 1.0); // Clamp to valid mix range

        // 1./e/8e3 -> 1.0 / e_val / 8000.0. This makes it brighter if e_val is smaller.
        // This implies e_val should not be zero or tiny for this to make sense.
        float intensity = 1.0 / max(e_val, 0.0001) / color_intensity_divisor; // Tunable divisor, safe max()

        // mix(vec3(1), H(g*.1), sin(.8)) * 1./e/8e3
        accumulatedColor += mix(vec3(color_mix_white), fractal_color, color_mix_factor) * intensity;
        
        // Break conditions for raymarching: if step is too small (hit surface) or ray went too far
        // Original didn't have explicit break, but standard for raymarching performance.
        // Using a generous threshold to ensure full scene rendering.
        if (e_val < 0.0001 || g_val > 500.0) break; 
    }

    // Final color post-processing (adjustments, not fractal specific)
    accumulatedColor = adjustColor(accumulatedColor, brightness, saturation, contrast);

    // Vignette effect (remains as fixed aesthetic)
    vec2 vignette_uv = fragCoord / RENDERSIZE.xy;
    float vignette_val = pow(16.0 * vignette_uv.x * vignette_uv.y * (1.0 - vignette_uv.x) * (1.0 - vignette_uv.y), 0.3);
    accumulatedColor *= mix(1.0, vignette_val, 0.5); // Mix with original color, 0.5 intensity

    fragColor = vec4(clamp(accumulatedColor, 0.0, 1.0), 1.0);
    gl_FragColor = fragColor;
}