/*
{
    "CATEGORIES": [
        "Fractal",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Raymarching",
        "SDF"
    ],
    "DESCRIPTION": "An ISF conversion of a highly compact Shadertoy shader, creating an abstract, twisting fractal tunnel effect. Features extensive controls for animation, camera, fractal geometry, psychedelic color palettes, and post-processing. Tunable parameters allow for dynamic morphing, zoom, speed, and color adjustments.",
    "CREDIT": "Original shader by iq (2013) - Creative Commons Licence Attribution-NonCommercial-ShareAlike. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "camera_x_control", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "X-component for camera orbit/rotation (normalized 0-1, maps to 0-2PI). Currently disabled, kept for future use if original behavior achieved." },
        { "NAME": "camera_y_control", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Y-component for camera orbit/rotation (normalized 0-PI). Currently disabled, kept for future use if original behavior achieved." },
        { "NAME": "camera_z_offset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Manual Z-axis offset for the camera." },
        { "NAME": "fractal_fold_scale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "DESCRIPTION": "Scale factor for the fractal's mirroring folds. Affects the core geometry." },
        { "NAME": "twist_speed_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Multiplier for the fractal's twisting animation speed." },
        { "NAME": "noise_distortion_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Strength of the noise applied to distort the fractal shape." },
        { "NAME": "noise_scale_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for the scale/frequency of the noise distortion." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 200.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "noise_iterations", "TYPE": "float", "DEFAULT": 32.0, "MIN": 1.0, "MAX": 64.0, "STEP": 1.0, "DESCRIPTION": "Number of noise iterations for fractal distortion. Higher values add more detail." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_id_offset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0, "STEP": 1.0, "DESCRIPTION": "Offset to select a palette variant based on ID." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "lighting_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall intensity of the lighting effect." },
        { "NAME": "hit_brightness_limit", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 1000.0, "DESCRIPTION": "Limits how bright individual raymarch steps contribute to color. Lower values reduce 'hot spots' and prevent white in the pattern." },
        { "NAME": "tanh_divisor", "TYPE": "float", "DEFAULT": 50000.0, "MIN": 100.0, "MAX": 100000000.0, "DESCRIPTION": "Controls the overall 'brilliance' of the image after accumulation. Lower values increase vibrancy significantly." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tone mapping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier after gamma." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect (produces psychedelic shimmer). (Requires RENDERPASS_PREV_FRAME, may not work on all hosts). Set to > 0 to enable." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect. (Requires RENDERPASS_PREV_FRAME, may not work on all hosts)." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "dithering_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable (1.0) or disable (0.0) dithering for smoother gradients." }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "Destination"
        }
    ]
}
*/

#define PI 3.141592653589793
#define TAU (PI * 2.0)

// Custom mod function to handle negative numbers consistently
float mod_custom(float x, float y) {
    return x - y * floor(x / y);
}

// from "Palettes" by iq. https://shadertoy.com/view/ll2GD3
vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b * cos( 6.28318 * (c * t + d) );
}

// Helper for spectral_zucconi6 palette
vec3 bump3y(vec3 x_in, vec3 yoffset_in) {
    vec3 y_out = 1.0 - x_in * x_in;
    y_out = clamp((y_out - yoffset_in), vec3(0.0), vec3(1.0));
    return y_out;
}

// Zucconi spectral palette (from iq)
vec3 spectral_zucconi6(float x) {
    x = fract(x);
    const vec3 c1 = vec3(3.54585104, 2.93225262, 2.41593945);
    const vec3 x1 = vec3(0.69549072, 0.49228336, 0.27699880);
    const vec3 y1 = vec3(0.02312639, 0.15225084, 0.52607955);
    const vec3 c2 = vec3(3.90307140, 3.21182957, 3.96587128);
    const vec3 x2 = vec3(0.11748627, 0.86755042, 0.66077860);
    const vec3 y2 = vec3(0.84897130, 0.88445281, 0.73949448);
    return bump3y(c1 * (x - x1), y1) + bump3y(c2 * (x - x2), y2) ;
}

// Custom psychedelic color palettes (8 options)
vec3 getPal(int id, float t) {
    id = id % 8; // Ensure it cycles through 8 palettes (0-7)

    vec3 col;
    if (id == 0) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, -0.33, 0.33));
    else if (id == 1) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, 0.10, 0.20));
    else if (id == 2) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.3, 0.20, 0.20));
    else if (id == 3) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 0.5), vec3(0.8, 0.90, 0.30));
    else if (id == 4) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
    else if (id == 5) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
    else if (id == 6) col = pal(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0, 1.0, 1.0), vec3(0.0, 0.25, 0.25));
    else if (id == 7) col = spectral_zucconi6(t); // Use the Zucconi spectral palette
    
    return col;
}

// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); 
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); 
    color = mix(gray, color, sat); 
    return color * br; 
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }

// Dithering noise
float random_dither(vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
const highp float NOISE_GRANULARITY = 0.5 / 255.0;

// SDF function for the fractal
float map(vec3 p, float current_time_scaled_param, float fractal_fold_scale_param, float twist_speed_multiplier_param, float noise_distortion_strength_param, float noise_scale_multiplier_param, float noise_iterations_param) {
    // Mirror p more than once (fractal fold)
    p = fractal_fold_scale_param - abs(abs(p) - fractal_fold_scale_param);

    // RESTORED ORIGINAL TWISTING LOGIC - This is crucial for the fractal's unique shape
    p.xy *= mat2(cos(p.z * 0.2 * twist_speed_multiplier_param + vec4(0, 33, 11, 0)));

    // Base distance (sin(p.y + p.x))
    float s = sin(p.y + p.x);

    // Add noise distortion
    for (float n = 1.0; n < noise_iterations_param; n += n) {
        s -= noise_distortion_strength_param * abs(dot(cos(noise_scale_multiplier_param * current_time_scaled_param + p * n), vec3(0.3))) / n;
    }
    return s; // Return the raw signed distance
}


void main() {
    float current_time_scaled = TIME * speed; 

    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    uv /= zoom; // Apply zoom

    float total_distance = 0.0;
    vec4 accumulated_color = vec4(0.0); 

    for (float i = 0.0; i < max_raymarch_iterations; i++) {
        vec3 current_ray_pos = vec3(uv * total_distance, total_distance + (2.0 * current_time_scaled) + camera_z_offset);

        // This is the signed distance from the fractal surface (can be negative or zero)
        float signed_distance_from_map = map(current_ray_pos, current_time_scaled, fractal_fold_scale, twist_speed_multiplier, noise_distortion_strength, noise_scale_multiplier, noise_iterations);

        // --- CRITICAL FIX: Replicated original's 's' assignment for distance & color divisor ---
        // Original: `d += s = .005+abs(s)*.7`
        // The `s` on the right (from map) is used to calculate a new `s` (always positive),
        // which then updates `d` AND is used as the color divisor.
        float distance_increment_and_color_divisor = 0.005 + abs(signed_distance_from_map) * 0.7;

        total_distance += distance_increment_and_color_divisor; 

        // Apply color only when a reasonable distance increment is available, preventing division by zero/near-zero
        if (distance_increment_and_color_divisor > 0.0) { 
            vec3 palette_color = getPal(int(color_palette_type + palette_id_offset), total_distance * palette_time_multiplier);
            
            // --- MODIFIED: Ensure palette color is always significant and add dynamic pulse ---
            // This ensures the pattern is driven by the palette, not just a generic white blow-out.
            // Pulse multiplier ensures dynamism, ranging from 0.0 to 1.0.
            float pulse_multiplier = (0.5 + 0.5 * cos(total_distance * 2.0 + dot(palette_color, vec3(0.33))));
            vec3 hit_intensity = palette_color * pulse_multiplier; 

            // Scale by inverse of distance_increment_and_color_divisor (brighter for closer hits)
            hit_intensity /= distance_increment_and_color_divisor;

            // --- Limit per-step brightness to prevent "white hot spots" ---
            // This caps the maximum brilliance of any single part of the pattern,
            // allowing other colors to show through instead of blowing out to white.
            hit_intensity = min(hit_intensity, vec3(hit_brightness_limit));
            
            accumulated_color.rgb += hit_intensity;
            accumulated_color.a = 1.0; 
        }
    }

    // Final color processing
    vec4 final_color = accumulated_color;

    // Apply lighting intensity from parameter
    final_color.rgb *= lighting_intensity;

    // --- Tweaked: tanh function for overall compression with NEW default for brilliance ---
    // Lower tanh_divisor = more brilliant (closer to 1.0) after initial hot spot limiting.
    final_color.rgb = tanh(final_color.rgb * final_color.rgb / tanh_divisor); 

    // --- Post-Processing Effects ---
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv_post = fragCoord / RENDERSIZE.xy;

    // Glitch and Shake effects are commented out as they typically require RENDERPASS_PREV_FRAME
    // (multi-pass rendering) which may not be universally supported or configured in ISF hosts for
    // single-pass shaders. Uncomment at your own discretion if your host supports it.
    /*
    if (glitch_strength > 0.001) { 
        float offset_x_noise = (hash22(uv_post * 10.0 + current_time_scaled * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(uv_post.y * 150.0 + current_time_scaled * 20.0) * 0.5 + 0.5;
        
        uv_post.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
        final_color.rgb = texture(RENDERPASS_PREV_FRAME, uv_post).rgb; 
    }
    */
    /*
    if (shake_strength > 0.001) { 
        vec2 shake_offset = vec2(
            sin(current_time_scaled * shake_frequency + hash11(1.0)),
            cos(current_time_scaled * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.01; 
        
        uv_post += shake_offset;
        final_color.rgb = texture(RENDERPASS_PREV_FRAME, uv_post).rgb; 
    }
    */

    // Brightness, Saturation, Contrast
    final_color.rgb = adjustColor(final_color.rgb, brightness, saturation, contrast);

    // Apply gamma correction and output gain
    final_color.rgb = pow(final_color.rgb, vec3(gamma_correction)) * output_gain; 
    
    // Dithering
    if (dithering_enabled > 0.5) { 
        final_color.rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    gl_FragColor = final_color;
}