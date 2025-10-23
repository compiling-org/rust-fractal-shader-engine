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
    "DESCRIPTION": "An ISF conversion of a compact Shadertoy volumetric fractal shader. Features a unique 'abs(sin(p)) - 1' fractal, tunnel effects, and extensive tunable controls for camera, geometry, animation, color palettes, and post-processing like glitch and shake. Modified to ensure spatial color variation and responsive color controls.",
    "CREDIT": "Original shader by a user on Shadertoy (specific author unknown from snippet). Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier (original was iTime/2)." },
        { "NAME": "max_raymarch_steps", "TYPE": "float", "DEFAULT": 99.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "max_render_distance", "TYPE": "float", "DEFAULT": 1000.0, "MIN": 100.0, "MAX": 2000.0, "DESCRIPTION": "Maximum distance the ray can travel. Avoids infinite loops for missed rays." },

        { "NAME": "camera_offset_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position X offset (shifts the entire fractal world)." },
        { "NAME": "camera_offset_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position Y offset (shifts the entire fractal world)." },
        { "NAME": "camera_offset_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position Z offset (shifts the entire fractal world)." },
        { "NAME": "camera_pitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "DESCRIPTION": "Camera pitch (up/down rotation) in radians." },
        { "NAME": "camera_yaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera yaw (left/right rotation) in radians." },
        { "NAME": "camera_roll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera roll (Z-axis rotation) in radians." },
        { "NAME": "perspective_depth", "TYPE": "float", "DEFAULT": 500.0, "MIN": 100.0, "MAX": 2000.0, "DESCRIPTION": "Controls ray's Z-depth for perspective. Higher values mean wider FOV / more distortion. (Approx. RENDERSIZE.x in original)." },

        { "NAME": "sin_distortion_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the sine-based distortion applied to position. (Default 0.0 for stability)" },
        { "NAME": "tunnel_radius", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Radius of the outer tunnel shape (original: 0.6)." },
        { "NAME": "linear_z_speed_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Multiplier for the linear Z movement speed (original: 1.0, based on 't')." },
        { "NAME": "xy_scale_base", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Base scale for the XY plane distortion." },
        { "NAME": "xy_oscillation_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Amplitude of sine oscillation for XY plane scaling. (Default 0.0 for stability)" },
        { "NAME": "fractal_iter_count", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the inner fractal loop (original: 8)." },
        { "NAME": "fractal_scaling_base", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Base value for the fractal scaling factor 'l' (original: 1.5)." },
        { "NAME": "fractal_scaling_oscillation_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Amplitude of oscillation for fractal scaling. (Default 0.0 for stability)" },
        { "NAME": "fractal_scaling_oscillation_speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of oscillation for fractal scaling (original: 0.5)." },
        { "NAME": "abs_sin_p_offset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Offset applied after abs(sin(p)) in fractal iteration. Affects 'hollowness' (original: 1.0)." },

        { "NAME": "density_accumulation_divisor", "TYPE": "float", "DEFAULT": 5000000.0, "MIN": 10000.0, "MAX": 10000000.0, "DESCRIPTION": "Divisor for density accumulation. Higher values make the scene dimmer (original: 5e6)." },
        { "NAME": "tonemap_shoulder", "TYPE": "float", "DEFAULT": 0.155, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Shoulder value for filmic tonemapping. Higher values crush shadows less (original: 0.155)." },
        { "NAME": "tonemap_gain", "TYPE": "float", "DEFAULT": 1.02, "MIN": 0.5, "MAX": 2.0, "DESCRIPTION": "Gain multiplier after filmic tonemapping (original: 1.02)." },

        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulse effect." },
        { "NAME": "color_pulse_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.2, "DESCRIPTION": "Amplitude of the color pulse. (Default 0.0 for debugging)" },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "palette_spatial_contribution", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Controls how much ray depth influences color palette variation across the fractal. Set to 0.0 for time-only palette." },


        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tonemapping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier after gamma." },
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
    id = id % 8; // Ensure ID is within 0-7

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

// Function to create a rotation matrix around Z axis (roll)
mat3 rotateZ(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, -s, 0,
        s, c, 0,
        0, 0, 1
    );
}

// --- Main Shader Logic ---
void main() {
    float current_time = TIME * global_speed; // Overall time control

    // Centered, ratio-corrected screen uvs
    vec2 res = RENDERSIZE.xy;
    vec2 uv = (2.0 * gl_FragCoord.xy - res) / res.y;

    // Apply shake effect to uv coordinates
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency * 1.0 + hash11(1.0)),
            cos(current_time * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05 * RENDERSIZE.y;
        uv += shake_offset / res.y; // Apply shake to UVs, scaled by Y resolution for consistency
    }

    vec3 ray_dir_initial = normalize(vec3(uv * RENDERSIZE.y, -perspective_depth));

    // Apply camera rotation to ray direction
    mat3 camera_rotation_matrix = rotateY(camera_yaw) * rotateX(camera_pitch) * rotateZ(camera_roll);
    vec3 dir = camera_rotation_matrix * ray_dir_initial;

    float density_value = 0.0; // Accumulates density (original 'o' before final tone-mapping)
    float z = 0.0; // Raymarch depth (current distance along ray)
    float d = 0.0; // Distance field step size

    // Raymarching loop
    for (float i = 0.0; i < max_raymarch_steps; i++) {
        // Calculate sample position in world space, relative to camera offset
        vec3 p = camera_offset_x * vec3(1,0,0) + camera_offset_y * vec3(0,1,0) + camera_offset_z * vec3(0,0,1) + z * dir;

        // Apply sine-based distortion (now defaults to 0.0 amplitude)
        p += sin(current_time + p.yzx) * sin_distortion_strength;
        
        // Calculate tunnel distance estimate
        float s = tunnel_radius - length(p.xy);

        // Apply linear Z motion
        p.z -= current_time * linear_z_speed_mult;

        // Apply XY scaling (now controlled by xy_oscillation_amplitude)
        p.xy *= xy_scale_base + sin(current_time) * xy_oscillation_amplitude; 

        float current_w = xy_scale_base + sin(current_time) * xy_oscillation_amplitude; // fractal scaling factor initialization

        // Inner fractal iterations
        float l = 0.0; // Initialize l
        for (float j = 0.0; j < fractal_iter_count; j++) { // Use j for inner loop counter
            p = abs(sin(p)) - abs_sin_p_offset;
            
            // Calculate fractal scaling factor 'l' (fractal_scaling_oscillation_amplitude now defaults to 0.0)
            l = (sin(current_time * fractal_scaling_oscillation_speed) * fractal_scaling_oscillation_amplitude + fractal_scaling_base) / dot(p, p);
            
            // Apply fractal scaling
            p *= l; 
            current_w *= l; // Scale 'w' as well, as per original logic
        }
        
        // Raymarch step: max of fractal distance and tunnel distance
        d = max(length(p) / current_w, s);
        z += d; // Accumulate the step size to raymarch depth

        // Accumulate density (grayscale)
        density_value += z / d / density_accumulation_divisor;

        // Early exit for performance if ray goes too far or step is too small
        if (z > max_render_distance || d < 0.0001) break; 
    }

    // Final tonemaping
    density_value = density_value / (density_value + tonemap_shoulder) * tonemap_gain; 

    // --- Post-processing Effects ---
    // Apply color palette to the accumulated density
    float palette_t_value = current_time * palette_time_multiplier;
    palette_t_value += fract((z / max_render_distance) * palette_spatial_contribution); 
    
    vec3 final_color_rgb = getPal(int(color_palette_type), palette_t_value) * density_value;

    // Apply color pulse (color_pulse_amplitude now defaults to 0.0)
    if (color_pulse_amplitude > 0.001) { 
        final_color_rgb *= (1.0 + sin(current_time * color_pulse_speed) * color_pulse_amplitude);
    }
    
    // Glitch Effect (produces psychedelic shimmer pattern)
    if (glitch_strength > 0.001) {
        float g_time = current_time * glitch_frequency;
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

    // Final clamp to ensure output colors are strictly within [0,1] range
    gl_FragColor = vec4(clamp(final_color_rgb, 0.0, 1.0), 1.0);
}