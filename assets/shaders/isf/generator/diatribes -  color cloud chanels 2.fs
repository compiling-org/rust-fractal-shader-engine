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
    "DESCRIPTION": "An ISF conversion of a compact Shadertoy shader creating an abstract, twisting fractal tunnel. Features extensive controls for animation, camera, fractal geometry, psychedelic color palettes, and post-processing, prioritizing rich, brilliant, white-free colors. Glitch and Shake are implemented procedurally without requiring multi-pass or previous frame feedback.",
    "CREDIT": "Original shader by bal-khan. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "camera_offset_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Manual Z-axis offset for the camera's starting position." },
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulsing effect applied to the palette." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 100.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance. Original: 100." },
        { "NAME": "epsilon_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Multiplier for the raymarch hit threshold. Affects detail and smoothness. Original: 1.0 (for .04 step)" },
        { "NAME": "far_clip_distance", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 100.0, "DESCRIPTION": "Maximum distance rays will travel. Affects view depth and performance." },
        { "NAME": "ray_step_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.01, "MAX": 2.0, "DESCRIPTION": "Multiplier for how much distance is added per raymarch step. Affects speed and detail. Original: 1.0 (for *.2)" },
        { "NAME": "hit_color_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.01, "MAX": 10.0, "DESCRIPTION": "Overall intensity of colors at hit points." },
        { "NAME": "hit_color_base_divisor", "TYPE": "float", "DEFAULT": 0.0071001, "MIN": 0.00001, "MAX": 0.1, "DESCRIPTION": "Base divisor for hit color. Affects the core brightness and falloff. Lower = brighter." },
        { "NAME": "hit_color_dist_sq_divisor", "TYPE": "float", "DEFAULT": 0.00051, "MIN": 0.000001, "MAX": 0.005, "DESCRIPTION": "Divisor for squared hit distance. Affects how quickly colors fade from the hit point. Lower = longer fade." },
        { "NAME": "hit_brightness_limit", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 1000.0, "DESCRIPTION": "Caps maximum brightness of individual raymarch steps. Crucial for preventing pure white in the pattern. Lower for more distinct colors." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_id_offset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0, "STEP": 1.0, "DESCRIPTION": "Offset to select a palette variant based on fractal ID." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "lighting_intensity", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall intensity of the lighting effect applied after color accumulation." },
        { "NAME": "tanh_divisor", "TYPE": "float", "DEFAULT": 200.0, "MIN": 100.0, "MAX": 100000000.0, "DESCRIPTION": "Controls the overall 'brilliance' and compression of the image. **Lower values (e.g., to MIN) increase vibrancy significantly and prevent white.** Original: 200." },
        { "NAME": "base_brightness", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.001, "MAX": 5.0, "DESCRIPTION": "Base multiplier for the final accumulated color. Controls overall initial brightness." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tone mapping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier after gamma." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of procedural glitch effect (produces psychedelic shimmer/distortion)." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of procedural glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect (procedural, affects view)." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "structure_scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Scales the overall size of the 3D fractal structure. Higher values make it larger." },
        { "NAME": "morph_param_1", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "DESCRIPTION": "Modifies the initial distance calculation (s = 2. - ...). Positive values can make the structure 'thicker' or closer." },
        { "NAME": "morph_param_2", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Modifies the scale of the min distance. Higher values compress or 'thin' the structure." },
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

// Original shader's spiral path function
#define P(z_in) vec3(cos(vec2(.15,.2)*(z_in))*5., z_in)

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
float hash11(float p_in) { p_in = fract(p_in * .1031); p_in *= p_in + 33.33; p_in *= p_in + p_in; return fract(p_in); }
float hash22(vec2 p_in) { return fract(sin(dot(p_in, vec2(41.45, 12.04))) * 9876.5432); }

// Dithering noise
float random_dither(vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
const highp float NOISE_GRANULARITY = 0.5 / 255.0;

// MAIN ISF SHADER FUNCTION
void main() {
    // --- Variable Declarations ---
    float raymarch_step_count;   // Corresponds to original 'i' (main loop counter)
    float total_distance_marched; // Corresponds to original 'd' (accumulated ray distance)
    float current_sdf_value;     // Corresponds to original 's' (SDF result or step distance)
    float inner_fractal_scale;   // Corresponds to original 'n' (inner loop fractal scale)
    float current_time;          // Corresponds to original 't' (time variable)

    vec3 main_q_resolution;      // Corresponds to original 'q = iResolution' (used for initial setup)
    vec3 ray_position;           // Corresponds to original 'p' (ray's current position in 3D space)
    vec3 cam_forward_dir;        // Corresponds to original 'Z'
    vec3 cam_right_dir;          // Corresponds to original 'X'
    vec3 ray_direction;          // Corresponds to original 'D'

    vec4 accumulated_color;      // Corresponds to original 'o' (accumulated output color)

    // Fragment UVs
    vec2 uv_frag = gl_FragCoord.xy;
    vec2 uv_norm; // Normalized UVs for camera

    // --- Variable Initializations ---
    raymarch_step_count = 0.0;
    total_distance_marched = 0.0;
    current_sdf_value = 0.0;
    inner_fractal_scale = 0.0; // Not used until inner loop, but initialized for safety.
    current_time = TIME * speed * 3.0; // Original: t=iTime*3.

    // Initialize resolution_vec, this was a previous error cause (vec3 = vec2)
    main_q_resolution = vec3(RENDERSIZE.x, RENDERSIZE.y, 1.0); 

    // Explicit component-wise initialization for accumulated_color (original 'o')
    accumulated_color.r = 0.0; 
    accumulated_color.g = 0.0;
    accumulated_color.b = 0.0;
    accumulated_color.a = 0.0;


    // Apply Shake effect by offsetting the camera's view
    uv_norm = (uv_frag - main_q_resolution.xy / 2.0) / main_q_resolution.y; // Initial normalization
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency * 1.0 + hash11(1.0)),
            cos(current_time * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05; 
        uv_norm += shake_offset; 
    }
    uv_norm /= zoom; // Apply zoom


    // --- Camera and Ray Setup (from original shader logic) ---
    // Original: p = P(t)
    ray_position = P(current_time); 
    ray_position.z += camera_offset_z; // Apply Z offset

    // Original: Z = normalize( P(t+1.)- p)
    cam_forward_dir = normalize( P(current_time + 1.0) - P(current_time) ); 
    
    // Original: X = normalize(vec3(Z.z,0,-Z.x)) - note Z.x from original, not Z as vec3
    cam_right_dir = normalize(vec3(cam_forward_dir.z, 0.0, -cam_forward_dir.x)); 

    // Original: D = vec3((u-q.xy/2.)/q.y, 1) * mat3(-X, cross(X, Z), Z);
    // Combined UV normalization and ray direction matrix multiplication
    mat3 camera_orientation_matrix = mat3(
        -cam_right_dir,     // X axis (inverted from original compact shader logic)
        cross(cam_right_dir, cam_forward_dir), // Y axis (original: cross(X,Z))
        cam_forward_dir     // Z axis (forward)
    );
    ray_direction = normalize(vec3(uv_norm, 1.0) * camera_orientation_matrix); 

    // --- Main Raymarching Loop ---
    // Original: for(o*=i; i++<1e2;)
    // `o*=i` removed, `accumulated_color` is initialized to 0.0.
    for(raymarch_step_count = 0.0; raymarch_step_count < max_raymarch_iterations; raymarch_step_count++) {
        // Original: p += D * s;
        // The 's' here is the previous 'step distance' from `d += s = ...`
        ray_position += ray_direction * current_sdf_value; 

        // Check for far clip distance
        if (total_distance_marched > far_clip_distance) break;

        // --- Fractal SDF Calculation (Corresponds to original 'q = P(p.z)+cos(t+p.yzx)*.3; s = 2. - min(length...)) ---
        // Original: q = P(p.z)+cos(t+p.yzx)*.3;
        vec3 fractal_shape_pos = P(ray_position.z * structure_scale) + cos(current_time + ray_position.yzx) * 0.3;

        // Original: s = 2. - min(length((p-q).xy), min(length(p.xy - q.y) , length(p.xy - q.x)));
        // FIX for "swizzles on scalar expressions" by explicitly casting floats to vec2
        float dist_xy_p_q = length((ray_position - fractal_shape_pos).xy);
        float dist_p_y_plane = length(ray_position.xy - vec2(fractal_shape_pos.y)); 
        float dist_p_x_plane = length(ray_position.xy - vec2(fractal_shape_pos.x));
        
        // Apply morph parameters
        current_sdf_value = (2.0 + morph_param_1 * 2.0) - min(dist_xy_p_q, min(dist_p_y_plane, dist_p_x_plane)) * (1.0 - morph_param_2 * 0.8);

        // --- Inner loop for fractal noise/detail ---
        // Original: for (n = .1; n < 1.; s -= abs(dot(sin(p * n * 16.), q-q+.03)) / n, n += n);
        for (inner_fractal_scale = 0.1; inner_fractal_scale < 1.0; inner_fractal_scale *= 2.0) {
            // q-q+.03 simplifies to vec3(0.03)
            current_sdf_value -= abs(dot(sin(ray_position * inner_fractal_scale * 16.0), vec3(0.03))) / inner_fractal_scale;
        }

        // --- Update total distance and next step size ---
        // Original: d += s = .04 + abs(s)*.2;
        // Ensure current_sdf_value (s) is never too small, which could cause large steps/jumps
        current_sdf_value = (0.04 * epsilon_factor) + abs(current_sdf_value) * (0.2 * ray_step_multiplier);
        total_distance_marched += current_sdf_value;

        // --- Color Accumulation ---
        // Original: o += (1.+cos(d+vec4(4,2,1,0))) / s / d;
        // Normalize original cosine pattern (0-2 range -> 0-1)
        vec3 original_color_pattern = (1.0 + cos(total_distance_marched + vec4(4.0, 2.0, 1.0, 0.0)).rgb) / 2.0; 
        
        // Generate palette color based on accumulated distance and time
        vec3 palette_color = getPal(int(color_palette_type + palette_id_offset), total_distance_marched * palette_time_multiplier + current_time * color_pulse_speed);
        
        vec3 step_color = original_color_pattern * palette_color; // Blend palette into the fractal pattern
        
        // Apply intensity and inverse proportion to step distance and total distance
        // Original: `/ s / d` = `/(s*d)`. Using custom divisors for more control and preventing div by zero.
        float color_divisor = max(0.00001, (hit_color_base_divisor + current_sdf_value * total_distance_marched * hit_color_dist_sq_divisor));
        step_color = step_color * hit_color_intensity / color_divisor;
        
        // Cap brightness of individual contributions to prevent oversaturation/pure white
        step_color = min(step_color, vec3(hit_brightness_limit));
        
        accumulated_color.rgb += step_color;
    }
    
    accumulated_color.rgb *= base_brightness; // Apply base brightness
    accumulated_color.rgb *= lighting_intensity; // Apply lighting intensity

    // --- Tone Mapping ---
    // Original: o = tanh(o / 2e2);
    accumulated_color.rgb = tanh(accumulated_color.rgb / tanh_divisor);

    // --- Post-processing: Glitch Effect ---
    if (glitch_strength > 0.001) {
        float g_time = current_time * glitch_frequency;
        // Use uv_frag (raw fragment coords) for more consistent screen-space effects
        vec2 uv_post = uv_frag / RENDERSIZE.xy; 

        float glitch_noise_line = hash11(floor(uv_post.y * RENDERSIZE.y / 15.0) + g_time * 0.5); 
        float glitch_noise_block = hash22(floor(uv_post * 50.0) + g_time * 0.1); 

        if (glitch_noise_line < glitch_strength * 0.7) { 
            float shift_amount = (glitch_noise_line - 0.5) * 2.0 * glitch_strength * 0.05; 
            
            vec3 shifted_color = accumulated_color.rgb;
            shifted_color.r = mix(accumulated_color.r, accumulated_color.g, abs(shift_amount)); 
            shifted_color.b = mix(accumulated_color.b, accumulated_color.r, abs(shift_amount)); 
            accumulated_color.rgb = shifted_color;

            accumulated_color.rgb += vec3(
                (hash11(uv_post.x + g_time) - 0.5) * 0.5,
                (hash11(uv_post.y + g_time * 1.1) - 0.5) * 0.5,
                (hash11(uv_post.x + uv_post.y + g_time * 1.2) - 0.5) * 0.5
            ) * glitch_strength * 0.2 * sin(uv_post.y * 100.0 + g_time * 2.0); 
        }

        if (glitch_noise_block < glitch_strength * 0.3) {
            // Explicitly create vec3 to avoid "swizzle on scalar" error
            accumulated_color.rgb = mix(accumulated_color.rgb, vec3(hash11(uv_post.x * 100.0 + g_time * 2.0)), glitch_strength * 0.5);
        }
    }

    // --- Post-processing: Brightness, Saturation, Contrast ---
    accumulated_color.rgb = adjustColor(accumulated_color.rgb, brightness, saturation, contrast);

    // --- Final Adjustments ---
    // Apply gamma correction and output gain
    accumulated_color.rgb = pow(accumulated_color.rgb, vec3(gamma_correction)) * output_gain; 
    
    // Dithering for smoother gradients
    if (dithering_enabled > 0.5) { 
        accumulated_color.rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(uv_frag));
    }

    gl_FragColor = accumulated_color;
}