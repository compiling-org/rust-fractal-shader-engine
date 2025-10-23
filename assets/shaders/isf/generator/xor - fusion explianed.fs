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
    "DESCRIPTION": "An ISF conversion of @XorDev's 'Fusion' shader, a volumetric raymarcher creating trippy, tunnel-like visuals. Includes a comprehensive set of tunable parameters for camera, fractal geometry, animation, color palettes, and post-processing effects like glitch and shake.",
    "CREDIT": "Original shader 'Fusion' by @XorDev (https://x.com/XorDev/status/1913318200001098048). Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier." },
        { "NAME": "max_raymarch_steps", "TYPE": "float", "DEFAULT": 100.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "raymarch_precision", "TYPE": "float", "DEFAULT": 0.0001, "MIN": 0.00001, "MAX": 0.01, "DESCRIPTION": "Minimum step size for raymarching. Smaller values are more precise but slower." },
        { "NAME": "max_render_distance", "TYPE": "float", "DEFAULT": 1000.0, "MIN": 100.0, "MAX": 2000.0, "DESCRIPTION": "Maximum distance the ray can travel. Avoids infinite loops for missed rays." },

        { "NAME": "camera_offset_x", "TYPE": "float", "DEFAULT": 0.15, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position X offset." },
        { "NAME": "camera_offset_y", "TYPE": "float", "DEFAULT": 0.15, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position Y offset." },
        { "NAME": "camera_offset_z", "TYPE": "float", "DEFAULT": 0.15, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Camera position Z offset. Use to move into/out of fractal." },
        { "NAME": "camera_pitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "DESCRIPTION": "Camera pitch (up/down rotation) in radians." },
        { "NAME": "camera_yaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera yaw (left/right rotation) in radians." },
        { "NAME": "camera_roll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera roll (Z-axis rotation) in radians." },
        { "NAME": "perspective_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Strength of perspective distortion (similar to FOV). Higher values zoom out." },

        { "NAME": "lin_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Linear forward movement speed of the fractal tunnel." },
        { "NAME": "ang_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Angular rotation speed of the fractal's internal structure." },
        { "NAME": "dis_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the fractal's distortion/morphing." },

        { "NAME": "z_scale", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "DESCRIPTION": "Z-axis scaling factor for the fractal's 'boxes'." },
        { "NAME": "opacity_factor", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Interior opaqueness. Higher values make the volume more transparent." },
        { "NAME": "fractal_distortion_scale", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Controls the base frequency of the fractal's internal sine/cosine distortion." },
        { "NAME": "fractal_oscillation_scale", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.001, "MAX": 0.1, "DESCRIPTION": "Controls the oscillation frequency within the fractal's distortion." },
        
        { "NAME": "color_change_speed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed at which the primary colors change based on depth." },
        { "NAME": "rgb_freq_r", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Red color wave frequency." },
        { "NAME": "rgb_freq_g", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Green color wave frequency." },
        { "NAME": "rgb_freq_b", "TYPE": "float", "DEFAULT": 9.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Blue color wave frequency." },
        { "NAME": "density_divisor", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 500.0, "DESCRIPTION": "Divisor for the step size. Affects overall density and brightness. Higher = less dense." },
        { "NAME": "tonemapping_divisor", "TYPE": "float", "DEFAULT": 200.0, "MIN": 1.0, "MAX": 5000.0, "DESCRIPTION": "Divisor for tonemapping. Affects overall brightness and contrast. Lower = brighter." },
        
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulse effect." },
        { "NAME": "color_pulse_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Amplitude of the color pulse. (0.0 for no pulse, 0.5 for max reasonable pulse)" },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },

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

// --- Helper Functions (Reused and adapted from previous successful conversions) ---
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

    // Input-controlled camera offset
    vec3 current_camera_offset = vec3(camera_offset_x, camera_offset_y, camera_offset_z);

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

    // Ray direction with perspective. Base direction is (uv.x, uv.y, -1.0)
    // -PERSPECTIVE in original means ray points into negative Z.
    vec3 dir = normalize(vec3(uv, -perspective_strength));

    // Apply camera rotation to ray direction
    mat3 camera_rotation_matrix = rotateY(camera_yaw) * rotateX(camera_pitch) * rotateZ(camera_roll);
    dir = camera_rotation_matrix * dir;

    vec3 col = vec3(0.0); // Output color accumulation
    float z = 0.0; // Raymarch depth (current distance along ray)
    float d = 0.0; // Distance field step size

    // Raymarching loop
    for (float i = 0.0; i < max_raymarch_steps; i++) {
        // Compute sample position in world space: ray origin (camera_offset) + z * ray_direction
        vec3 p = current_camera_offset + z * dir;

        // Convert to cylindrical coordinates based on original shader's atan order
        vec3 v = vec3(
            atan(p.x, p.y) - ang_speed * current_time, // Angle in XY plane
            length(p.xy),                               // Radius from Z-axis
            z_scale * p.z - lin_speed * current_time    // Scaled Z-coordinate, linearly moving with time
        );

        // Irregular boxes and shifting (core fractal-like distortion)
        v = sin(v * fractal_distortion_scale + cos(v / fractal_oscillation_scale - dis_speed * current_time));

        // Step forward with opacity factor and calculate density 'd'
        // Add a small epsilon to the length() result to prevent division by zero or infinites
        d = length(max(v, v.yzx * opacity_factor)) / density_divisor;
        z += d; // Accumulate the step size to raymarch depth

        // Add glow coloring with light attenuation
        // Divide by max(d, raymarch_precision) to prevent division by zero if d becomes very small.
        vec3 current_rgb_freq = vec3(rgb_freq_r, rgb_freq_g, rgb_freq_b); // Create vec3 from float inputs
        col += (sin((p.z - color_change_speed * current_time) * current_rgb_freq) + 1.0) / max(d, raymarch_precision);

        // Early exit for performance if ray goes too far
        if (z > max_render_distance) break;
    }

    // Exponential tonemapping
    // col = 1.0 - exp(-col / STEPS / 1e3);
    col = 1.0 - exp(-col / max_raymarch_steps / tonemapping_divisor); 

    // --- Post-processing Effects (reusing and adapting from previous implementations) ---
    // Apply color palette. Using 'col' directly as the input for palette's 't' might be too chaotic
    // Instead, using current_time for smoother palette transitions.
    vec3 final_color_rgb = getPal(int(color_palette_type), current_time * palette_time_multiplier) * col;

    // Apply color pulse (amplitude range adjusted for better control)
    final_color_rgb *= (1.0 + sin(current_time * color_pulse_speed) * color_pulse_amplitude);

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