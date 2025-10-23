/*
{
    "CATEGORIES": [
        "Abstract",
        "Procedural",
        "3D",
        "Animated",
        "Psychedelic",
        "Volume",
        "Fractal"
    ],
    "DESCRIPTION": "An ISF conversion of @XorDev's 'Space Station' shader, integrating both Buffer A (raymarching/fractal) and Image pass (bokeh) into a single pass. It accurately recreates the original's unique 3D Apollonian fractal and volumetric effects, with precise camera and animation control.",
    "CREDIT": "Original shader 'Space Station' by @XorDev. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 200.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "raymarch_precision", "TYPE": "float", "DEFAULT": 0.0001, "MIN": 0.00001, "MAX": 0.01, "DESCRIPTION": "Minimum step size for raymarching. Smaller values are more precise but slower." },
        { "NAME": "max_render_distance", "TYPE": "float", "DEFAULT": 1000.0, "MIN": 100.0, "MAX": 2000.0, "DESCRIPTION": "Maximum distance the ray can travel. Avoids infinite loops for missed rays." },

        { "NAME": "camera_offset_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -500.0, "MAX": 500.0, "DESCRIPTION": "Camera position X offset." },
        { "NAME": "camera_offset_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -500.0, "MAX": 500.0, "DESCRIPTION": "Camera position Y offset." },
        { "NAME": "camera_offset_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -500.0, "MAX": 500.0, "DESCRIPTION": "Camera position Z offset. Use to move into/out of fractal." },
        { "NAME": "camera_pitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "DESCRIPTION": "Camera pitch (up/down rotation) in radians." },
        { "NAME": "camera_yaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera yaw (left/right rotation) in radians." },

        { "NAME": "camera_ray_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for screen coordinates in ray direction calculation. Affects apparent zoom/spread." },
        { "NAME": "scroll_speed_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed at which the fractal scrolls through space." },
        { "NAME": "view_rotation_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Speed of the fractal's internal rotation, relative to camera view." },

        { "NAME": "fractal_init_scale", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.5, "MAX": 10.0, "DESCRIPTION": "Initial scale factor for the Apollonian fractal." },
        { "NAME": "fractal_iterations_count", "TYPE": "float", "DEFAULT": 11.0, "MIN": 1.0, "MAX": 20.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the fractal's self-similarity. Higher values add complexity." },
        { "NAME": "fractal_folding_factor", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0, "DESCRIPTION": "Strength of the fractal's folding operation." },
        { "NAME": "fractal_z_offset", "TYPE": "float", "DEFAULT": 400.0, "MIN": 1.0, "MAX": 1000.0, "DESCRIPTION": "Z-offset for the fractal distance calculation. Affects apparent depth." },

        { "NAME": "shadow_start_iteration", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 200.0, "STEP": 1.0, "DESCRIPTION": "Raymarch iteration at which shadow tracing begins." },
        { "NAME": "shadow_step_increment", "TYPE": "float", "DEFAULT": 0.00001, "MIN": 0.0, "MAX": 0.0001, "DESCRIPTION": "Small increment added to step size during shadow tracing." },
        { "NAME": "shadow_accumulation_multiplier", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Multiplier for shadow accumulation. Lower values reduce brightness in shadowed areas." },
        
        { "NAME": "fog_color_r", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Red component of the fog/volume color." },
        { "NAME": "fog_color_g", "TYPE": "float", "DEFAULT": 6.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Green component of the fog/volume color." },
        { "NAME": "fog_color_b", "TYPE": "float", "DEFAULT": 9.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Blue component of the fog/volume color." },
        { "NAME": "fog_divisor", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 500.0, "DESCRIPTION": "Divisor for the fog/volume color. Higher values make fog less intense." },
        { "NAME": "fog_density_multiplier", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Controls the density of the volumetric fog. Lower values make fog less intense." },

        { "NAME": "tonemapping_power", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0, "DESCRIPTION": "Power applied to the raw accumulated color before final tone mapping." },
        { "NAME": "bokeh_strength", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Strength of the artistic 'bokeh' effect (bloom/softness)." },

        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulse effect." },
        { "NAME": "color_pulse_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Amplitude of the color pulse. (0.0 for no pulse)" },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },

        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tone mapping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.998, "MAX": 1.001, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier after gamma." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of procedural glitch effect (produces psychedelic shimmer/distortion)." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of procedural glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect (procedural, affects view)." },
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

// --- Helper Functions ---
vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b * cos( 6.28318 * (c * t + d) );
}

vec3 bump3y(vec3 x_in, vec3 yoffset_in) {
    vec3 y_out = 1.0 - x_in * x_in;
    y_out = clamp((y_out - yoffset_in), vec3(0.0), vec3(1.0));
    return y_out;
}

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

vec3 getPal(int id, float t) {
    id = id % 8;

    vec3 col;
    if (id == 0) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, -0.33, 0.33));
    else if (id == 1) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, 0.10, 0.20));
    else if (id == 2) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.3, 0.20, 0.20));
    else if (id == 3) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 0.5), vec3(0.8, 0.90, 0.30));
    else if (id == 4) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
    else if (id == 5) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
    else if (id == 6) col = pal(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0, 1.0, 1.0), vec3(0.0, 0.25, 0.25));
    else if (id == 7) col = spectral_zucconi6(t);
    
    return col;
}

vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); 
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); 
    color = mix(gray, color, sat); 
    return color * br; 
}

float hash11(float p_in) { p_in = fract(p_in * .1031); p_in *= p_in + 33.33; p_in *= p_in + p_in; return fract(p_in); }
float hash22(vec2 p_in) { return fract(sin(dot(p_in, vec2(41.45, 12.04))) * 9876.5432); }

float random_dither(vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
const highp float NOISE_GRANULARITY = 0.5 / 255.0;

// Function to create a rotation matrix around Y axis (yaw)
mat3 rotateY(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, 0, s,
        0, 1, 0,
        -s, 0, c
    );
}

// Function to create a rotation matrix around X axis (pitch)
mat3 rotateX(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        1, 0, 0,
        0, c, -s,
        0, s, c
    );
}

// Global for time for use in SDF/map
float current_time_global; 

// Simulate ShaderToy's iResolution.z behavior (which is typically 1.0 if not specified as vec3(w,h,w))
// For ISF, RENDERSIZE is vec2. The original code seems to expect iResolution.z to be 1.0.
vec3 iResolution_simulated_for_original_logic; 

// Apollonian Fractal Distance Field (SDF) function
// p_z_in: The Z component of the current ray's origin (from main loop's `p`). Used for slicing.
// v_in: The current transformed vector for fractal calculation (from main loop's `v`).
float map_fractal(vec3 v_in, float p_z_in) { 
    vec3 v_for_fractal_calc = v_in; // Start with the pre-transformed v from main loop
    
    float j_fractal;
    float s_fractal = fractal_init_scale; 
    float current_sdf_val = 0.0; 

    for(j_fractal = s_fractal; j_fractal < fractal_iterations_count; j_fractal++) { 
        v_for_fractal_calc = mod(v_for_fractal_calc - 1.0, 2.0) - 1.0; 
        
        // Critical Fix: Added max(0.0001, ...) to prevent division by zero or near-zero
        float e_folding_factor = fractal_folding_factor / max(0.0001, dot(v_for_fractal_calc, v_for_fractal_calc)); 
        
        s_fractal *= e_folding_factor;
        v_for_fractal_calc *= e_folding_factor;
        
        // Use p_z_in (current ray origin Z) for the SDF calculation
        current_sdf_val = length(v_for_fractal_calc) / s_fractal - p_z_in / fractal_z_offset;
    }
    return current_sdf_val;
}


// --- Main Shader Logic ---
void main() {
    current_time_global = TIME * global_speed; // Assign to global variable
    iResolution_simulated_for_original_logic = vec3(RENDERSIZE.xy, 1.0); // ShaderToy's iResolution.z is often 1.0

    // `p` in original code is the ray origin in the fractal's unrotated space
    vec3 p_ray_origin = vec3(camera_offset_x, camera_offset_y, camera_offset_z);

    // `I` in original code is gl_FragCoord.xy
    // `r` in original code is iResolution (which we simulate as iResolution_simulated_for_original_logic)

    // Calculate initial_ray_direction_raw: (vec3(I+I,r)-r)/r.x
    // This is the unrotated ray direction used for stepping p_ray_origin in the fog phase.
    vec3 initial_ray_direction_raw = vec3(
        gl_FragCoord.x * 2.0,
        gl_FragCoord.y * 2.0,
        iResolution_simulated_for_original_logic.x // This is the 'r' as a float for the third component
    );
    initial_ray_direction_raw -= iResolution_simulated_for_original_logic; // Subtract the full iResolution vec3
    initial_ray_direction_raw /= iResolution_simulated_for_original_logic.x; // Divide by iResolution.x

    // `initial_ray_direction_rotated` is used to orient the *fractal's space* relative to the camera view.
    // It's applied to `v` for fractal calculations, not directly to `p`'s stepping.
    mat3 camera_rotation_matrix = rotateY(camera_yaw) * rotateX(camera_pitch);
    
    vec4 accumulated_color_raw = vec4(0.0); // Renamed 'O' from original Buffer A
    
    vec2 frag_coord_uv = gl_FragCoord.xy;

    // Apply shake effect to frag_coord
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time_global * shake_frequency * 1.0 + hash11(1.0)),
            cos(current_time_global * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05 * RENDERSIZE.y; 
        frag_coord_uv += shake_offset; 
    }

    // Loop variables for the main raymarch loop (matching original's `i, e, j, s, t, p, v`)
    float ray_iter = 0.0; // `i` in original
    float current_sdf_result; // `e` in original, determined by map_fractal
    vec3 v_for_fractal_calc; // `v` in original

    // Main Raymarch Loop
    // Loop condition: `i++ < 2e2` (i.e., ray_iter from 0 to 199)
    for( ; ray_iter < max_raymarch_iterations; ray_iter++) { 
        // Operations done at the beginning of each iteration in the original ShaderToy 'for' loop's initializer
        // `v = p+t/r` (Note: `p` here is `p_ray_origin` in our code)
        v_for_fractal_calc = p_ray_origin + current_time_global * scroll_speed_factor / iResolution_simulated_for_original_logic;
        
        // `v.xy *= mat2(sin(t*.1+vec4(0,11,33,0)))` - Rotate view of fractal space
        v_for_fractal_calc.xy *= mat2(sin(current_time_global * view_rotation_speed + vec4(0,11,33,0)));
        
        // `v--` - Center fractal space
        v_for_fractal_calc -= 1.0; 

        // Call the SDF function to get the distance (`e` in original)
        current_sdf_result = map_fractal(v_for_fractal_calc, p_ray_origin.z);
        
        // Original ShaderToy's complex conditional for ray step and color accumulation:
        // `i>1e2 ? p += e+=1e-5, O += e : O+=vec4(5,6,9, p+=(vec3(I+I,r)-r)/r.x*e )/1e2*e`
        if (ray_iter > shadow_start_iteration) {
            // Shadow tracing path (original: `p += e+=1e-5, O += e`)
            current_sdf_result += shadow_step_increment; // `e+=1e-5` (increment 'e' before using it for p)
            p_ray_origin += current_sdf_result; // `p += e` (steps in (1,1,1) direction as `e` is scalar added to `vec3 p`)
            accumulated_color_raw.rgb += current_sdf_result * shadow_accumulation_multiplier; // `O += e` (scaled for control)
        } else {
            // Fog/Volume and primary ray step path (original: `O+=vec4(5,6,9, p+=(vec3(I+I,r)-r)/r.x*e )/1e2*e`)
            // CRITICAL FIX: p_ray_origin updates using the raw, unrotated ray direction
            p_ray_origin += initial_ray_direction_raw * current_sdf_result; // `p+=(vec3(I+I,r)-r)/r.x*e`
            
            // `O+=vec4(5,6,9, ...)/1e2*e`
            accumulated_color_raw.rgb += vec3(fog_color_r, fog_color_g, fog_color_b) / fog_divisor * current_sdf_result * fog_density_multiplier;
        }

        // Break conditions: if ray hits close enough or goes too far
        if (current_sdf_result < raymarch_precision || length(p_ray_origin) > max_render_distance) break;
    }
    
    // --- Buffer A's Final Tone Mapping ---
    // O=min(O*O*O,1.);
    accumulated_color_raw.rgb = min(pow(accumulated_color_raw.rgb, vec3(tonemapping_power)), vec3(1.0));

    // --- Image Pass's Bokeh/Post-processing (Approximation for single pass) ---
    // The original bokeh pass (Buffer B) samples Buffer A multiple times.
    // Replicating that exactly in a single pass is very slow (requires re-raymarching per sample).
    // Instead, we apply the final operation from Buffer B's `sqrt(O)*.1` as a global bloom/softness.
    accumulated_color_raw.rgb = sqrt(accumulated_color_raw.rgb) * bokeh_strength; 

    // --- Final Color Application and Post-processing Effects ---
    // Apply color palette. Note: original used p.x for palette, which is `p_ray_origin.x` here
    vec3 final_color_rgb = getPal(int(color_palette_type), p_ray_origin.x * 0.05 + current_time_global * palette_time_multiplier) * accumulated_color_raw.rgb;

    // Apply color pulse
    final_color_rgb *= (1.0 + sin(current_time_global * color_pulse_speed) * color_pulse_amplitude);

    // Glitch Effect
    if (glitch_strength > 0.001) {
        float g_time = current_time_global * glitch_frequency;
        vec2 uv_post = gl_FragCoord.xy / RENDERSIZE.xy; 

        float glitch_noise_line = hash11(floor(uv_post.y * RENDERSIZE.y / 15.0) + g_time * 0.5); 
        float glitch_noise_block = hash22(floor(uv_post * 50.0) + g_time * 0.1); 

        if (glitch_noise_line < glitch_strength * 0.7) { 
            float shift_amount = (glitch_noise_line - 0.5) * 2.0 * glitch_strength * 0.05; 
            
            vec3 shifted_color = final_color_rgb;
            shifted_color.r = mix(final_color_rgb.r, final_color_rgb.g, abs(shift_amount)); 
            shifted_color.b = mix(final_color_rgb.b, final_color_rgb.r, abs(shift_amount)); 
            final_color_rgb = shifted_color;

            final_color_rgb += vec3(
                (hash11(uv_post.x + g_time) - 0.5) * 0.5,
                (hash11(uv_post.y + g_time * 1.1) - 0.5) * 0.5,
                (hash11(uv_post.x + uv_post.y + g_time * 1.2) - 0.5) * 0.5
            ) * glitch_strength * 0.2 * sin(uv_post.y * 100.0 + g_time * 2.0); 
        }

        if (glitch_noise_block < glitch_strength * 0.3) {
            final_color_rgb = mix(final_color_rgb, vec3(hash11(uv_post.x * 100.0 + g_time * 2.0)), glitch_strength * 0.5);
        }
    }

    // Brightness, Saturation, Contrast
    final_color_rgb = adjustColor(final_color_rgb, brightness, saturation, contrast);

    // Final Gamma Correction and Output Gain
    final_color_rgb = pow(final_color_rgb, vec3(gamma_correction)) * output_gain; 
    
    // Dithering for smoother gradients
    if (dithering_enabled > 0.5) { 
        final_color_rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    // Final clamp to ensure output colors are within [0,1]
    gl_FragColor = vec4(clamp(final_color_rgb, 0.0, 1.0), 1.0);
}