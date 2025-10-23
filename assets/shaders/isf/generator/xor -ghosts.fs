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
    "DESCRIPTION": "An ISF conversion of XorDev's 'Ghosts' shader, exploring turbulence and an irregular gyroid in 3D. Features extensive controls for animation, camera, geometry, psychedelic color palettes, and post-processing, prioritizing rich, brilliant, white-free colors. Glitch and Shake are implemented procedurally.",
    "CREDIT": "Original shader 'Ghosts' by @XorDev. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in, making elements appear larger." },
        { "NAME": "initial_ray_depth", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Initial depth offset for the raymarch. Affects where the ghost shapes begin. Set to 0.02 to ensure initial render." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 100.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance. Original: 100." },
        { "NAME": "far_clip_distance", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 100.0, "DESCRIPTION": "Maximum distance rays will travel. Affects view depth and performance." },
        { "NAME": "scroll_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Speed at which the pattern scrolls forward (Z-axis)." },
        { "NAME": "twist_amount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Controls the strength of the twisting effect based on depth." },
        { "NAME": "twist_time_offset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time's influence on the twisting effect." },
        { "NAME": "turbulence_loop_start", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Starting frequency for the turbulence loop. Affects initial detail." },
        { "NAME": "turbulence_loop_end", "TYPE": "float", "DEFAULT": 9.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Ending frequency for the turbulence loop. Affects maximum detail." },
        { "NAME": "turbulence_decay_factor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 0.99, "DESCRIPTION": "Factor by which turbulence frequency decreases (d /= factor). Higher values mean slower decay/more iterations." },
        { "NAME": "turbulence_effect_scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall strength of the turbulence applied to the ray sample point." },
        { "NAME": "gyroid_base_step", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "DESCRIPTION": "Base step size for the raymarch. Lower values give more detail." },
        { "NAME": "gyroid_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall strength/magnitude of the irregular gyroid structure." },
        { "NAME": "gyroid_detail", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for the gyroid's internal pattern (p.yzx * .6). Higher values create finer details." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "accumulated_color_base_red_scale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Scale for the red component based on ray depth (z/7.). Original was very faint." },
        { "NAME": "accumulated_color_base_green", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Base green component for the accumulated color. Original: 2." },
        { "NAME": "accumulated_color_base_blue", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Base blue component for the accumulated color. Original: 3." },
        { "NAME": "accumulated_color_intensity", "TYPE": "float", "DEFAULT": 1000.0, "MIN": 1.0, "MAX": 10000.0, "DESCRIPTION": "Overall intensity multiplier for the accumulated color. Increased default for visibility." },
        { "NAME": "tanh_exponent_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Controls the exponent applied before tanh tone mapping. Higher values lead to more saturation." },
        { "NAME": "tanh_divisor", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 1e8, "DESCRIPTION": "Controls the overall 'brilliance' and compression of the image during tone mapping. **Lower default for immediate visibility.** Original: 1e7." },
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

void main() {
    float current_time;
    float raymarch_iterator;
    float ray_depth;
    float current_step_size;
    
    vec4 accumulated_color;

    vec2 frag_coord = gl_FragCoord.xy;
    vec2 normalized_uv;

    current_time = TIME * speed;
    raymarch_iterator = 0.0;
    ray_depth = initial_ray_depth; 
    current_step_size = 0.0;

    accumulated_color.r = 0.0; 
    accumulated_color.g = 0.0;
    accumulated_color.b = 0.0;
    accumulated_color.a = 0.0;

    normalized_uv = (frag_coord - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency * 1.0 + hash11(1.0)),
            cos(current_time * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05; 
        normalized_uv += shake_offset; 
    }
    normalized_uv /= zoom;

    for (raymarch_iterator = 0.0; raymarch_iterator < max_raymarch_iterations; raymarch_iterator++) {
        vec3 screen_offset_from_center = vec3(2.0 * (frag_coord - RENDERSIZE.xy / 2.0), -RENDERSIZE.y);
        vec3 current_sample_point = ray_depth * normalize(screen_offset_from_center / RENDERSIZE.y);
        
        current_sample_point.xy *= mat2(cos((ray_depth + current_time * twist_time_offset) * 0.1 * twist_amount + vec4(0.0, 33.0, 11.0, 0.0)));
        current_sample_point.z -= scroll_speed * current_time;
        
        for (current_step_size = turbulence_loop_start; current_step_size < turbulence_loop_end; current_step_size /= turbulence_decay_factor) {
            current_sample_point += cos(current_sample_point.yzx * current_step_size * gyroid_detail + current_time) * turbulence_effect_scale / current_step_size;
        }
        
        float gyroid_distance = gyroid_base_step + (abs(2.0 - dot(cos(current_sample_point), sin(current_sample_point.yzx * gyroid_detail * 0.6))) * gyroid_strength) / 8.0;
        
        ray_depth += current_step_size = gyroid_distance; 

        if (ray_depth > far_clip_distance) break;

        vec3 base_accum_color = vec3(
            ray_depth / (7.0 / accumulated_color_base_red_scale),
            accumulated_color_base_green,
            accumulated_color_base_blue
        );

        vec3 palette_color = getPal(int(color_palette_type), ray_depth * palette_time_multiplier + current_time);
        
        vec3 final_step_color = base_accum_color * palette_color;

        final_step_color = final_step_color * accumulated_color_intensity / max(0.00001, current_step_size);

        accumulated_color.rgb += final_step_color;
    }
    
    accumulated_color.rgb = tanh(pow(accumulated_color.rgb, vec3(tanh_exponent_factor)) / tanh_divisor);

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

    accumulated_color.rgb = adjustColor(accumulated_color.rgb, brightness, saturation, contrast);

    accumulated_color.rgb = pow(accumulated_color.rgb, vec3(gamma_correction)) * output_gain; 
    
    if (dithering_enabled > 0.5) { 
        accumulated_color.rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    gl_FragColor = accumulated_color;
}