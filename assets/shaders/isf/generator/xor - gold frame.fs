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
    "DESCRIPTION": "An ISF conversion of a Shadertoy shader, featuring a glowing, worm-like fractal rendered via raymarching. Includes extensive controls for light, camera, fractal geometry, and psychedelic post-processing effects. Please set 'Camera Control Type' to 1 (XY Control) for interactive mouse-like camera movement.",
    "CREDIT": "Original shader source unknown (common raymarcher structure). Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier." },
        { "NAME": "max_raymarch_distance", "TYPE": "float", "DEFAULT": 120.0, "MIN": 10.0, "MAX": 500.0, "DESCRIPTION": "Maximum distance the ray will march. Controls render depth." },
        { "NAME": "surface_precision", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0001, "MAX": 0.1, "DESCRIPTION": "Precision for surface hit detection and normal calculation. Lower values are more precise but slower." },
        { "NAME": "max_raymarch_steps", "TYPE": "float", "DEFAULT": 240.0, "MIN": 50.0, "MAX": 1000.0, "STEP": 1.0, "DESCRIPTION": "Maximum number of steps for the raymarcher. Higher values increase detail but reduce performance." },

        { "NAME": "camera_control_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "0: Automatic Camera, 1: XY Mouse/Control." },
        { "NAME": "camera_yaw", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Manual camera yaw (horizontal rotation) when control type is 1. (0.5 to match original mouse center)" },
        { "NAME": "camera_pitch", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Manual camera pitch (vertical rotation) when control type is 1. (0.5 to match original mouse center)" },
        { "NAME": "camera_offset_forward_factor", "TYPE": "float", "DEFAULT": 24.0, "MIN": 0.0, "MAX": 100.0, "DESCRIPTION": "Factor for camera offset along its forward direction. Adjusts distance from origin." },
        { "NAME": "camera_scroll_speed", "TYPE": "float", "DEFAULT": 16.0, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Speed at which the camera scrolls along the X-axis." },
        { "NAME": "camera_fov", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Camera field of view. Higher values create a wider lens effect." },
        { "NAME": "zoom_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall zoom level, effectively scales the UV coordinates." },

        { "NAME": "light_orbit_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the light source's orbital movement." },
        { "NAME": "light_orbit_radius", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Radius of the light source's orbital movement." },
        { "NAME": "light_scroll_speed", "TYPE": "float", "DEFAULT": 16.0, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Speed at which the light source scrolls along the X-axis." },

        { "NAME": "fractal_iterations", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the fractal's self-similarity. Higher values add complexity." },
        { "NAME": "fractal_scale_factor", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "DESCRIPTION": "Scales the 'I/4.0' term in the fractal's distance field. Affects overall density." },
        { "NAME": "fractal_offset_factor", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Scales the 'I/8.0' term in the fractal's distance field. Affects the size of the 'holes'." },
        { "NAME": "morph_intensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Morphs the fractal geometry, blending between folded and unfolded space. (0.0 for original, 1.0 for more spherical)" },

        { "NAME": "base_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Base red component of the object's color." },
        { "NAME": "base_color_g", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Base green component of the object's color." },
        { "NAME": "base_color_b", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Base blue component of the object's color." },
        { "NAME": "diffuse_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Strength of diffuse lighting." },
        { "NAME": "specular_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Strength of specular highlights." },
        { "NAME": "specular_power", "TYPE": "float", "DEFAULT": 32.0, "MIN": 1.0, "MAX": 128.0, "STEP": 1.0, "DESCRIPTION": "Shininess of specular highlights." },
        { "NAME": "ao_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of the ambient occlusion/soft shadow effect." },
        { "NAME": "light_attenuation_factor", "TYPE": "float", "DEFAULT": 16.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Controls how quickly light fades with distance. Lower values mean faster falloff." },
        { "NAME": "light_halo_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Strength of the glow/halo effect around the light source." },
        
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulse effect." },
        { "NAME": "color_pulse_amplitude", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Amplitude of the color pulse. (0.0 for no pulse)" },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },

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

// Global Time for all animations
float T = TIME * global_speed;

// Dynamic Light Source
vec3 getLightPosition() {
    return vec3(cos(T * light_orbit_speed) - 1.0, sin(T * light_orbit_speed), cos(T * light_orbit_speed * 0.5)) * light_orbit_radius - vec3(T * light_scroll_speed, 0.0, 0.0);
}

// Mapping Function
vec3 map(vec3 p, float s, float morph) {
    vec3 A = mod(p + s / 2.0, s) - s / 2.0;
    // Morph between original folding and a more spherical/unfolded state
    return mix(A, p, morph); 
}

// Main Distance Field Function
float model(vec3 p) {
    float S_val = -1.0; // Renamed to S_val to avoid conflict with 's' parameter in map.
    for (int i = 0; i < int(fractal_iterations); i++) {
        float I = exp2(float(i));
        
        // Core SDF calculation
        float current_dist_val = I / fractal_scale_factor - length(max(abs(map(p, I, morph_intensity)) - I / fractal_offset_factor, 0.0));
        S_val = max(S_val, current_dist_val);
    }
    return S_val;
}

// Normal Function (using central differencing)
vec3 normal(vec3 p) {
    vec2 N_vec = vec2(-1.0, 1.0) * surface_precision;
    return normalize(model(p + N_vec.xyy) * N_vec.xyy +
                     model(p + N_vec.yxy) * N_vec.yxy +
                     model(p + N_vec.yyx) * N_vec.yyx +
                     model(p + N_vec.xxx) * N_vec.xxx);
}

// Simple Raymarcher
vec4 raymarch(vec3 p, vec3 d) {
    float S_accum = 0.0; // Accumulated distance for stepping
    float T_total = 0.0; // Total distance traveled, used for hit factor
    vec3 D_norm = normalize(d);
    vec3 P_curr = p; // Current ray position

    for (int i = 0; i < int(max_raymarch_steps); i++) {
        S_accum = model(P_curr); // Distance to surface from current point
        T_total += S_accum; // Accumulate total distance
        P_curr += D_norm * S_accum; // Move ray by distance
        if ((T_total > max_raymarch_distance) || (S_accum < surface_precision)) break; // Stop if too far or hit surface
    }
    return vec4(P_curr, min(T_total / max_raymarch_distance, 1.0)); // P_curr is hit position, T_total/MAX is hit factor (0-1).
}

// from "Palettes" by iq. https://www.shadertoy.com/view/ll2GD3
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


// Color/Material Function (Diffuse, AO, Light Attenuation)
vec3 color1(vec3 p, vec3 n) {
    vec3 C = vec3(base_color_r, base_color_g, base_color_b);
    vec3 light_pos = getLightPosition();
    vec3 D_light = normalize(light_pos - p);

    float M = model(p); // SDF at hit point (should be near 0)
    
    // Ambient Occlusion / Soft Shadow terms
    float L_ao = smoothstep(-0.05, 0.05, M + model(p + D_light * 0.1 * ao_intensity));
    L_ao *= smoothstep(-0.5, 0.5, M + model(p + D_light * 1.0 * ao_intensity));
    L_ao *= smoothstep(-1.0, 1.0, M + model(p + D_light * 2.0 * ao_intensity));
    
    // Diffuse lighting
    float L_diffuse = max(dot(n, D_light), -0.5) * 0.5 + 0.5;
    L_diffuse *= diffuse_strength;

    // Light attenuation based on distance
    float L_attenuation = exp2(1.0 - length(light_pos - p) / light_attenuation_factor);
    
    float total_lighting = L_ao * L_diffuse * L_attenuation;

    // Apply color pulse
    total_lighting *= (1.0 + sin(T * color_pulse_speed) * color_pulse_amplitude);

    vec3 palette_color = getPal(int(color_palette_type), p.x * 0.1 + T * palette_time_multiplier);
    C *= palette_color;

    return C * total_lighting;
}

// Full Color Function (adds specular)
vec3 color2(vec3 p, vec3 d) {
    vec3 N = normal(p);
    vec3 C = color1(p, N); // Get base color with lighting and AO
    
    vec3 light_pos = getLightPosition();
    float A_atten = exp2(1.0 - length(light_pos - p) / light_attenuation_factor);
    vec3 D_light = normalize(light_pos - p);

    vec3 reflected_light = reflect(D_light, N);
    
    // Specular highlight
    float specular = pow(max(dot(reflected_light, d), 0.0), specular_power) * A_atten;
    specular *= specular_strength;
    
    return C + specular;
}

// Camera Variables
void camera(out vec3 P_out, out vec3 D_out, out vec3 X_out, out vec3 Y_out, out vec3 Z_out) {
    vec2 A_angles;
    float is_mouse_active = 0.0;

    if (camera_control_type > 0.5) { // Manual XY Control
        A_angles = (vec2(camera_yaw, camera_pitch) - 0.5) * vec2(TAU, PI); // Scale to 2PI for yaw, PI for pitch
        is_mouse_active = 1.0;
    } else { // Automatic Camera (Original behavior)
        A_angles = (vec2(0.5, 0.5) - vec2(0.5,0.5)) * vec2(TAU, PI); // Default to looking forward
        is_mouse_active = 0.0; // No mouse influence means F is vec3(1,0,0) only if mix is used.
    }
    
    // Original: vec3 F = mix(vec3(1,0,0),vec3(cos(-A.x)*cos(A.y),sin(-A.x)*cos(A.y),sin(A.y)),M);
    // If M is 0, F is (1,0,0). If M is 1, F is based on A.
    // We'll use our `is_mouse_active` as the mix factor.
    vec3 F_base_auto = vec3(1.0, 0.0, 0.0);
    vec3 F_controlled = vec3(cos(-A_angles.x) * cos(A_angles.y), sin(-A_angles.x) * cos(A_angles.y), sin(A_angles.y));
    vec3 F = mix(F_base_auto, F_controlled, is_mouse_active);

    P_out = vec3(-T * camera_scroll_speed, 0.0, 0.0) + camera_offset_forward_factor * F;
    D_out = -F; // Camera direction is opposite of forward

    X_out = normalize(D_out); // Camera basis vectors
    Y_out = normalize(cross(X_out, vec3(0.0, 0.0, 1.0)));
    Z_out = cross(X_out, Y_out); // Should be Y, X, Z. Corrected.
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


void main() {
    vec3 P_cam, D_cam, X_cam, Y_cam, Z_cam;
    camera(P_cam, D_cam, X_cam, Y_cam, Z_cam);

    vec2 UV = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
    UV /= zoom_factor; // Apply zoom

    // Apply shake effect to UV
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(T * shake_frequency * 1.0 + hash11(1.0)),
            cos(T * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.05; 
        UV += shake_offset; 
    }

    // Original: D = normalize(mat3(X,Y,Z) * vec3(1.,UV));
    // The original uses X,Y,Z as column vectors of the matrix.
    // So mat3(X,Y,Z) is a camera-to-world matrix.
    // vec3(1.,UV) is ray direction in camera space (1.0 for Z-depth).
    D_cam = normalize(mat3(X_cam, Y_cam, Z_cam) * vec3(camera_fov, UV));

    vec4 M_hit_data = raymarch(P_cam, D_cam); // M.xyz = hit position, M.w = hit factor (0-1)
    vec3 final_color = vec3(0.01, 0.01, 0.01); // Base ambient color

    // Ensure we hit something before coloring
    if (M_hit_data.w < 0.99) { // If raymarch hit (hit factor < 1.0, meaning it hit before MAX_DIST)
        final_color += max(color2(M_hit_data.xyz, D_cam) * sqrt(1.0 - M_hit_data.w), 0.0);
    }
    
    // Add glowing halo around light source
    final_color += exp2(-length(cross(D_cam, getLightPosition() - P_cam))) * light_halo_strength;

    // --- Post-processing: Glitch Effect ---
    if (glitch_strength > 0.001) {
        float g_time = T * glitch_frequency;
        vec2 uv_post = gl_FragCoord.xy / RENDERSIZE.xy; 

        float glitch_noise_line = hash11(floor(uv_post.y * RENDERSIZE.y / 15.0) + g_time * 0.5); 
        float glitch_noise_block = hash22(floor(uv_post * 50.0) + g_time * 0.1); 

        if (glitch_noise_line < glitch_strength * 0.7) { 
            float shift_amount = (glitch_noise_line - 0.5) * 2.0 * glitch_strength * 0.05; 
            
            vec3 shifted_color = final_color;
            shifted_color.r = mix(final_color.r, final_color.g, abs(shift_amount)); 
            shifted_color.b = mix(final_color.b, final_color.r, abs(shift_amount)); 
            final_color = shifted_color;

            final_color += vec3(
                (hash11(uv_post.x + g_time) - 0.5) * 0.5,
                (hash11(uv_post.y + g_time * 1.1) - 0.5) * 0.5,
                (hash11(uv_post.x + uv_post.y + g_time * 1.2) - 0.5) * 0.5
            ) * glitch_strength * 0.2 * sin(uv_post.y * 100.0 + g_time * 2.0); 
        }

        if (glitch_noise_block < glitch_strength * 0.3) {
            final_color = mix(final_color, vec3(hash11(uv_post.x * 100.0 + g_time * 2.0)), glitch_strength * 0.5);
        }
    }

    // --- Post-processing: Brightness, Saturation, Contrast ---
    final_color = adjustColor(final_color, brightness, saturation, contrast);

    // --- Final Adjustments ---
    final_color = pow(final_color, vec3(gamma_correction)) * output_gain; 
    
    // Dithering for smoother gradients
    if (dithering_enabled > 0.5) { 
        final_color += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    gl_FragColor = vec4(final_color, 1.0); // Set alpha to 1.0
}