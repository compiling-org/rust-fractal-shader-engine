/*
{
    "CATEGORIES": [
        "Abstract",
        "Procedural",
        "3D",
        "Animated",
        "Psychedelic",
        "Volume"
    ],
    "DESCRIPTION": "An ISF conversion of @XorDev's 'Angel' shader, an experiment based on their '3D Fire'. It features a raymarched, twisting, distorted cylinder-like form with vibrant colors and various customizable effects.",
    "CREDIT": "Original shader 'Angel' by @XorDev. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 100.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        
        { "NAME": "camera_z_offset", "TYPE": "float", "DEFAULT": 6.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Shifts the camera backward along the Z-axis, making the object appear further away." },
        { "NAME": "camera_ray_multiplier", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplies the screen coordinates for the initial ray direction calculation. Affects perspective." },
        { "NAME": "zoom_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall zoom level, effectively scales the UV coordinates." },

        { "NAME": "twist_strength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Strength of the twisting effect applied to the object based on its Y position." },

        { "NAME": "distortion_loop_start", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Starting frequency for the distortion (turbulence) loop." },
        { "NAME": "distortion_loop_end", "TYPE": "float", "DEFAULT": 9.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Ending frequency for the distortion (turbulence) loop." },
        { "NAME": "distortion_decay_factor", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 0.99, "DESCRIPTION": "Factor by which distortion frequency decreases (d /= factor). Higher values mean slower decay/more iterations." },
        { "NAME": "distortion_speed_x", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Speed multiplier for distortion animation along X-axis." },
        { "NAME": "distortion_speed_y", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Speed multiplier for distortion animation along Y-axis." },
        { "NAME": "distortion_speed_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Speed multiplier for distortion animation along Z-axis." },
        
        { "NAME": "cylinder_base_offset", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Base offset for the cylinder's distance field. Affects its 'solidity'." },
        { "NAME": "cylinder_radius", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Radius of the base cylinder shape." },
        { "NAME": "cylinder_scale", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Divisor for the cylinder's distance field. Higher values make the object appear thinner/more diffuse." },

        { "NAME": "color_sin_offset_r", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Phase offset for Red component in color calculation." },
        { "NAME": "color_sin_offset_g", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Phase offset for Green component in color calculation." },
        { "NAME": "color_sin_offset_b", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Phase offset for Blue component in color calculation." },
        { "NAME": "color_add_constant", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Constant added to color components, affecting overall brightness." },
        
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulse effect." },
        { "NAME": "color_pulse_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Amplitude of the color pulse. (0.0 for no pulse)" },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },

        { "NAME": "tanh_divisor", "TYPE": "float", "DEFAULT": 4000.0, "MIN": 1.0, "MAX": 10000.0, "DESCRIPTION": "Divisor for the tanh tone mapping. Lower values increase contrast and brightness." },
        
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tone mapping." },
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

// --- Helper Functions (reused from previous conversions) ---
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

// --- Main Shader Logic ---
void main() {
    //Time for animation
    float current_time = TIME * global_speed;
    //Raymarch iterator
    float raymarch_iterator = 0.0;
    //Raymarch depth
    float ray_depth = 0.0;
    //Raymarch step size
    float raymarch_step_size = 0.0;
    
    vec4 accumulated_color = vec4(0.0); // Renamed O to accumulated_color

    vec2 frag_coord_scaled = gl_FragCoord.xy / zoom_factor;

    // Apply shake effect to frag_coord if enabled
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency * 1.0 + hash11(1.0)),
            cos(current_time * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05 * RENDERSIZE.y; // Scale shake by screen height
        frag_coord_scaled += shake_offset; 
    }

    //Raymarch loop (100 iterations)
    for(raymarch_iterator = 0.0; raymarch_iterator < max_raymarch_iterations; raymarch_iterator++) {
        //Raymarch sample position
        // Equivalent to z * normalize(vec3(2*I.x - R.x, 2*I.y - R.y, -R.y))
        vec3 ray_direction = normalize(vec3(camera_ray_multiplier * frag_coord_scaled, 0.0) - RENDERSIZE.xyy);
        vec3 current_sample_point = ray_depth * ray_direction;
        
        //Shift camera back
        current_sample_point.z += camera_z_offset;
        
        //Twist shape
        current_sample_point.xz *= mat2(cos(current_sample_point.y * twist_strength + vec4(0,33,11,0)));
        
        //Distortion (turbulence) loop
        float distort_loop_step_size; // Separated from raymarch_step_size
        for(distort_loop_step_size = distortion_loop_start; distort_loop_step_size < distortion_loop_end; distort_loop_step_size /= distortion_decay_factor) {
            //Add distortion waves
            current_sample_point += cos((current_sample_point.yzx - current_time * vec3(distortion_speed_x, distortion_speed_y, distortion_speed_z)) * distort_loop_step_size) / distort_loop_step_size;
        }
        
        //Compute distorted distance field of cylinder
        // (.1+abs(length(p.xz)-.5))/2e1
        ray_depth += raymarch_step_size = (cylinder_base_offset + abs(length(current_sample_point.xz) - cylinder_radius)) / cylinder_scale;

        // Stop if step size is too small or ray_depth exceeds practical limits
        if (raymarch_step_size < 0.0001 || ray_depth > 1000.0) break; // Added a practical far clip for stability
        
        //Sample coloring and glow attenuation
        vec4 sample_color_contribution = (sin(ray_depth + vec4(color_sin_offset_r, color_sin_offset_g, color_sin_offset_b, 0.0)) + color_add_constant) / max(0.0001, raymarch_step_size);
        
        // Apply color palette
        vec3 palette_effect = getPal(int(color_palette_type), ray_depth * 0.1 + current_time * palette_time_multiplier);
        sample_color_contribution.rgb *= palette_effect;

        // Apply color pulse
        sample_color_contribution.rgb *= (1.0 + sin(current_time * color_pulse_speed) * color_pulse_amplitude);

        accumulated_color += sample_color_contribution;
    }
    
    //Tanh tonemapping
    accumulated_color = tanh(accumulated_color / tanh_divisor);

    // --- Post-processing: Glitch Effect ---
    if (glitch_strength > 0.001) {
        float g_time = current_time * glitch_frequency;
        vec2 uv_post = gl_FragCoord.xy / RENDERSIZE.xy; 

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
            accumulated_color.rgb = mix(accumulated_color.rgb, vec3(hash11(uv_post.x * 100.0 + g_time * 2.0)), glitch_strength * 0.5);
        }
    }

    // --- Post-processing: Brightness, Saturation, Contrast ---
    accumulated_color.rgb = adjustColor(accumulated_color.rgb, brightness, saturation, contrast);

    // --- Final Adjustments ---
    accumulated_color.rgb = pow(accumulated_color.rgb, vec3(gamma_correction)) * output_gain; 
    
    // Dithering for smoother gradients
    if (dithering_enabled > 0.5) { 
        accumulated_color.rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    gl_FragColor = accumulated_color;
}