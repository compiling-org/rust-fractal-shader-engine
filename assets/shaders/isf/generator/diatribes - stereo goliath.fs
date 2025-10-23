/*{
    "CATEGORIES": [
        "Abstract",
        "Procedural",
        "3D",
        "Animated",
        "Psychedelic",
        "Volume",
        "Fractal",
        "Cross-Eyed 3D"
    ],
    "DESCRIPTION": "An ISF conversion of a cross-eyed 3D volumetric fractal shader. Features highly tunable camera, geometry, complex animations, psychedelic color palettes, and post-processing effects like glitch and shake. Includes optional cross-eyed stereo rendering. Modified to not require external image inputs.",
    "CREDIT": "Original shader by a user on Shadertoy (specific author unknown from snippet). Converted and enhanced for ISF by Gemini. Modified for no external texture input requirement.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier (Original T was iTime * .15, so 1.0 here matches original speed relative to iTime)." },
        { "NAME": "cam_pos_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera X position offset." },
        { "NAME": "cam_pos_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera Y position offset." },
        { "NAME": "cam_pos_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera Z position offset." },
        { "NAME": "cam_pitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "DESCRIPTION": "Camera pitch (up/down rotation) in radians, applied after path." },
        { "NAME": "cam_yaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera yaw (left/right rotation) in radians, applied after path." },
        { "NAME": "cam_roll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera roll (Z-axis rotation) in radians, applied to view direction based on rotation input." },
        { "NAME": "cam_look_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of animated camera 'look around' effect (based on original LOOK_SPEED)." },
        { "NAME": "cam_look_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Strength of animated camera 'look around' effect (based on original LOOK_FREQ)." },
        { "NAME": "cam_path_amp_y", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Amplitude of the camera's sinusoidal path oscillation on Y axis." },
        { "NAME": "cam_path_freq_z", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Frequency of the camera's sinusoidal path oscillation on Z axis." },
        { "NAME": "cam_path_offset_x", "TYPE": "float", "DEFAULT": 6.5, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Offset for camera X oscillation during 'orb' movement in the original shader." },
        { "NAME": "cam_path_offset_y", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Offset for camera Y oscillation during 'orb' movement in the original shader." },
        { "NAME": "max_raymarch_steps", "TYPE": "float", "DEFAULT": 220.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "raymarch_step_scale", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Multiplier for raymarch step size. Lower values for higher precision." },
        { "NAME": "tunnel_radius", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Radius of the outer tunnel shape defined in map function (original: 4.0)." },
        { "NAME": "tunnel_vertical_offset", "TYPE": "float", "DEFAULT": 1.5, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Vertical offset applied to the fractal in map function (original: 1.5)." },
        { "NAME": "fractal_iterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the inner fractal loop (original: 8)." },
        { "NAME": "fractal_scale_factor", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Scaling factor 'l' for fractal iterations (original: 1.5)." },
        { "NAME": "fractal_abs_sin_offset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Offset applied after abs(sin(p)) in fractal iteration (original: 1.0)." },
        { "NAME": "morph_strength", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Strength of the fractal morphing effect (influences 'l' factor)." },
        { "NAME": "morph_speed", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the fractal morphing effect." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tonemaping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 2.2, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Gamma correction for final output (Original texture power was 2.2)." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier for final output." },
        { "NAME": "tonemap_shoulder", "TYPE": "float", "DEFAULT": 0.155, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Shoulder value for filmic tonemaping (original: 0.155)." },
        { "NAME": "tonemap_gain", "TYPE": "float", "DEFAULT": 1.019, "MIN": 0.5, "MAX": 2.0, "DESCRIPTION": "Gain multiplier after filmic tonemaping (original: 1.019)." },
        { "NAME": "fog_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable (1.0) or disable (0.0) atmospheric fog." },
        { "NAME": "fog_distance_base", "TYPE": "float", "DEFAULT": 1.75, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Base distance at which fog starts." },
        { "NAME": "fog_distance_speed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of fog distance oscillation." },
        { "NAME": "fog_amount_base", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Base amount/density of the fog." },
        { "NAME": "fog_amount_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of fog amount oscillation." },
        { "NAME": "fog_color_base", "TYPE": "float", "DEFAULT": 0.84, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Base intensity for the fog color (mixed with specific RGB)." },
        { "NAME": "fog_color_speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of fog color oscillation." },
        { "NAME": "light_glimmer_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable (1.0) or disable (0.0) the light glimmer effect." },
        { "NAME": "light_glimmer_strength", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the light glimmer effect." },
        { "NAME": "light_glimmer_speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of the light glimmer oscillation." },
        { "NAME": "shake_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable (1.0) or disable (0.0) screen shake effect." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Strength of the screen shake effect." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 100000.0, "MIN": 100.0, "MAX": 1000000.0, "DESCRIPTION": "Base frequency of screen shake oscillations." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of a procedural glitch effect (produces psychedelic shimmer)." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of procedural glitch disruptions." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "palette_spatial_contribution", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Controls how much ray depth influences color palette variation across the fractal. Set to 0.0 for time-only palette." },
        { "NAME": "stereo_mode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "0: Normal 2D view, 1: Cross-Eyed 3D stereo view (requires specific viewing)." },
        { "NAME": "parallax_amount", "TYPE": "float", "DEFAULT": 0.032, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Adjusts 3D depth separation in cross-eyed stereo mode (original: 0.032)." },
        { "NAME": "fractal_mode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "0: Original abs(sin(p))-1, 1: Alternative cos(p)-0.5 for fractal iteration." },
        { "NAME": "base_light_color_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Multiplier for the core light color (originally vec4(3,2,1,0)). Higher values for brighter, more tinted output." },
        { "NAME": "depth_fade_amount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls how much color fades with distance. Set to 0.0 to disable depth fade. (Reverted from previous aggressive default)." },
        { "NAME": "background_noise_intensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Intensity of the background noise when no fractal is hit. (Defaulted to 0.0 based on feedback)." },
        { "NAME": "lighting_saturation_bias", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "DESCRIPTION": "Adds a saturation bias to the lighting. Positive makes it more colorful, negative less. Original's vec4(3,2,1,0) had a desaturating effect."}
    ],
    "TEXTURES": [],
    "PASSES": [
        {
            "TARGET": "Destination",
            "FLOAT": true,
            "PERSISTENT": true
        }
    ]
}
*/

#define PI 3.141592653589793
#define TAU (PI * 2.0)

#define N normalize

// --- Helper Functions (Moved to top for proper definition order) ---

vec3 hash31(float p) { // 3D hash from 1D
    vec3 p3 = fract(p * vec3(0.1031, 0.1135, 0.1183));
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xyz + p3.yzx) * p3.zxy);
}

float hash11(float p_in) { p_in = fract(p_in * .1031); p_in *= p_in + 33.33; p_in *= p_in + p_in; return fract(p_in); }
float hash22(vec2 p_in) { return fract(sin(dot(p_in, vec2(41.45, 12.04))) * 9876.5432); }
vec3 hash33(vec3 p) {
    p = vec3(dot(p, vec3(127.1,311.7, 74.7)),
             dot(p, vec3(269.5,183.3,246.1)),
             dot(p, vec3(113.5,271.9,124.6)));
    return fract(sin(p)*43758.5453123);
}

mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

mat3 rotateX(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        1, 0, 0,
        0, c, -s,
        0, s, c
    );
}

mat3 rotateY(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, 0, s,
        0, 1, 0,
        -s, 0, c
    );
}

mat3 rotateZ(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, -s, 0,
        s, c, 0,
        0, 0, 1
    );
}

// Old pal function (kept for reference, but likely replaced by more complex palettes below)
vec3 old_pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d ) {
    return a + b * cos( TAU * (c * t + d) );
}

// *** NEW PSYCHEDELIC PALETTE GENERATION - High Contrast, Non-linear ***
// Focus on vibrant, non-monotonous, "DMT-like" shifts
vec3 getPal(int id, float t) {
    id = id % 8; // Ensure ID is within bounds [0, 7]

    vec3 col;
    t = fract(t); // Ensure t wraps around for continuous animation

    // Palette 0: High-Contrast Cyber-Glow
    if (id == 0) {
        col = vec3(0.0);
        col.r = sin(t * TAU * 1.0 + 0.0) * 0.5 + 0.5;
        col.g = sin(t * TAU * 0.8 + 2.0) * 0.5 + 0.5;
        col.b = sin(t * TAU * 1.2 + 4.0) * 0.5 + 0.5;
        col = pow(col, vec3(1.5)) * 1.2; // Increase contrast and vibrancy
    }
    // Palette 1: Deep Space Nebula - Rich Blues, Purples, Hints of Green
    else if (id == 1) {
        col = old_pal(t * 1.2, vec3(0.0, 0.05, 0.1), vec3(0.2, 0.5, 0.8), vec3(1.0, 0.7, 1.2), vec3(0.5, 0.2, 0.8));
        col = pow(col, vec3(0.8)) * 1.5; // Slightly desaturate darks, boost overall
    }
    // Palette 2: Electric Impulse - Neon Greens, Yellows, and Reds
    else if (id == 2) {
        col = old_pal(t * 1.5, vec3(0.1, 0.0, 0.0), vec3(0.9, 0.9, 0.0), vec3(1.5, 0.8, 0.5), vec3(0.0, 0.3, 0.6));
        col = pow(col, vec3(1.8)) * 1.3; // Very high contrast, almost binary shifts
    }
    // Palette 3: Shifting Sands - Earthy tones with psychedelic accents
    else if (id == 3) {
        col = old_pal(t * 0.7, vec3(0.5, 0.3, 0.1), vec3(0.3, 0.5, 0.7), vec3(0.9, 1.1, 0.6), vec3(0.2, 0.5, 0.8));
        col = pow(col, vec3(1.2)); // Softer contrast, but still distinct
    }
    // Palette 4: Rainbow Warp - Full spectrum, very distinct bands
    else if (id == 4) {
        col = old_pal(t * 2.0, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(2.0, 1.0, 0.5), vec3(0.0, 0.33, 0.67));
        col = pow(col, vec3(1.0)) * 1.0; // Straightforward rainbow, keep strong
    }
    // Palette 5: Volatile Plasma - Intense hot/cold transitions
    else if (id == 5) {
        float f = fract(t * 3.0);
        if (f < 0.5) col = mix(vec3(0.0, 0.0, 0.8), vec3(1.0, 0.5, 0.0), f * 2.0); // Blue to Orange
        else col = mix(vec3(1.0, 0.5, 0.0), vec3(0.8, 0.0, 1.0), (f - 0.5) * 2.0); // Orange to Purple
        col = pow(col, vec3(1.5)); // Enhance contrast
    }
    // Palette 6: Spectral Glitch - Jagged, digital-like color shifts
    else if (id == 6) {
        float h1 = hash11(t * 10.0) * 0.5 + 0.5;
        float h2 = hash11(t * 10.0 + 1.0) * 0.5 + 0.5;
        float h3 = hash11(t * 10.0 + 2.0) * 0.5 + 0.5;
        col = vec3(h1, h2, h3);
        col = mix(col, fract(sin(t * TAU * vec3(5.0, 7.0, 9.0))) * 1.5, 0.7); // Mix with fast sines
        col = pow(col, vec3(2.0)) * 1.5; // High contrast
    }
    // Palette 7: Pure DMT Trip - Intense, unpredictable, highly saturated flashes
    else if (id == 7) {
        float t_val = t * 15.0; // Even faster cycling
        vec3 c1 = sin(t_val + vec3(0.0, 2.0, 4.0)) * 0.5 + 0.5;
        vec3 c2 = cos(t_val * 0.8 + vec3(1.0, 3.0, 5.0)) * 0.5 + 0.5;
        vec3 c3 = sin(t_val * 1.3 + vec3(0.5, 2.5, 4.5)) * 0.5 + 0.5;
        col = mix(c1 * c2, c2 + c3, 0.6); // More complex blend
        col = pow(col, vec3(0.7)); // Punchier colors
        col = clamp(col * 2.0, 0.0, 1.0); // Extreme saturation
    }
    
    return col;
}

vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = pow(color, vec3(con)); // Apply contrast (power adjustment)
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); 
    color = mix(gray, color, sat); 
    return color * br; 
}


vec2 apply_shake(float time_in, float strength, float frequency) {
    if (strength < 0.001) return vec2(0.0);
    float current_freq = frequency;
    return vec2(
        sin(time_in * current_freq * 1.0 + hash11(1.0)),
        cos(time_in * current_freq * 1.1 + hash11(2.0))
    ) * strength * (RENDERSIZE.y / RENDERSIZE.x); 
}

vec3 apply_glitch(vec3 color, float time_in, float strength, float frequency, vec2 frag_coords, vec2 resolution) {
    if (strength < 0.001) return color;

    float g_time = time_in * frequency;
    vec2 uv_post = frag_coords / resolution; 

    // Horizontal line distortion
    float glitch_noise_line = hash11(floor(uv_post.y * resolution.y / 15.0) + g_time * 0.5); 
    if (glitch_noise_line < strength * 0.7) { 
        float shift_amount = (glitch_noise_line - 0.5) * 2.0 * strength * 0.05; 
        
        // Simple channel shifting
        vec3 shifted_color = color;
        shifted_color.r = mix(color.r, color.g, abs(shift_amount)); 
        shifted_color.b = mix(color.b, color.r, abs(shift_amount)); 
        color = shifted_color;

        // Add some chaotic noise
        color += vec3(
            (hash11(uv_post.x + g_time) - 0.5) * 0.5,
            (hash11(uv_post.y + g_time * 1.1) - 0.5) * 0.5,
            (hash11(uv_post.x + uv_post.y + g_time * 1.2) - 0.5) * 0.5
        ) * strength * 0.2 * sin(uv_post.y * 100.0 + g_time * 2.0); 
    }

    // Blocky noise
    float glitch_noise_block = hash22(floor(uv_post * 50.0) + g_time * 0.1); 
    if (glitch_noise_block < strength * 0.3) {
        color = mix(color, vec3(hash11(uv_post.x * 100.0 + g_time * 2.0)), strength * 0.5);
    }
    return color;
}

// Procedural texture mimicking the original's texture lookups
vec3 procedural_tex(in vec3 p, in vec3 n, float time_in){
    // Combine multiple noise/pattern layers for "textured" feel
    vec3 base_color;

    // Layer 1: Base smooth noise
    float n1 = (sin(p.x * 0.5 + time_in * 0.1) + cos(p.y * 0.7 + time_in * 0.15) + sin(p.z * 0.3 + time_in * 0.2)) / 3.0;
    base_color = vec3(n1);

    // Layer 2: Warped detail based on normal
    float n2 = dot(N(p + n * 0.1), hash31(time_in * 0.2 + p.x * 0.3 + p.y * 0.5 + p.z * 0.7));
    base_color = mix(base_color, vec3(n2), 0.5 + 0.5 * sin(time_in * 0.1));

    // Layer 3: Finer, more chaotic noise for "grit"
    vec3 n3_hash = hash33(p * 10.0 + time_in * 0.3);
    base_color = mix(base_color, n3_hash, 0.3 + 0.2 * cos(time_in * 0.07));

    return clamp(base_color, 0.0, 1.0); // Ensure colors are within 0-1 range
}


vec3 apply_fog(vec3 rgb, float d, float time_in) {
    if(fog_enabled < 0.5) return rgb; // Truly disable fog if toggled off

    float current_fog_distance = fog_distance_base + sin(time_in * fog_distance_speed) * 0.5;
    float current_fog_amount = fog_amount_base + sin(time_in * fog_amount_speed) * 0.5;
    float current_fog_color_val = fog_color_base + sin(time_in * fog_color_speed) * 0.5;

    float f = d - current_fog_distance;
    if(f > 0.0) {
        f = min(1.0,f * current_fog_amount);
        // Made fog color more neutral by default unless explicitly tinted by input
        vec3 fog_color_mix = vec3(0.1 + f * current_fog_color_val); // More neutral base
        rgb = mix(rgb, fog_color_mix, f);
    }
    return rgb;
}

#define TONEMAP(x) ((x) / ((x) + tonemap_shoulder) * tonemap_gain)

// Prototype for map function
float map(in vec3 q);

float AO(in vec3 pos, in vec3 nor) { 
	float sca = 2.0, occ = 0.0;
    for( int i=0; i<5; i++ ){
        float hr = 0.01 + float(i)*0.5/4.0;
        float sample_dd = map(nor * hr + pos);
        occ += (hr - sample_dd)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - occ, 0.0, 1.0 );
}

vec3 P(float z_in, float time_in) {
    return (vec3(cam_pos_x,
                  cam_pos_y + tanh(cos(z_in * cam_path_freq_z) * .4) * cam_path_amp_y,
                  cam_pos_z + z_in));
}

vec3 orb(vec3 p_in, float time_in) {
    float t_look = time_in * cam_look_speed;
    return (p_in - vec3(
                P(p_in.z, time_in).x + tanh(cos(t_look * .5)*2.) * cam_path_offset_x,
                P(p_in.z, time_in).y + tanh(cos(t_look * .7)*3.) * cam_path_offset_y,
                5. + time_in));
}

bool fractalHit_global = false;

float map(in vec3 q){
    vec3 p = q;
    float s, f, l; 
    float current_tunnel_radius_calc = tunnel_radius - length(p.xy); 
    
    p.xy -= P(p.z, 0.0).xy; 

    p.y -= tunnel_vertical_offset; 
    
    float w_fractal_scale = 1.0; 
    float current_morph_freq = (tanh(cos(TIME * global_speed * morph_speed)*1.3)*1.7)*morph_strength; 

    for (int i=0; i < int(fractal_iterations); i++) { 
        if (fractal_mode < 0.5) { // Original abs(sin) mode
            p = abs(sin(p)) - fractal_abs_sin_offset; 
        } else { // Alternative mode
            p = cos(p * 1.5) - 0.5; // Example alternative fractal
        }
        l = fractal_scale_factor / dot(p,p) - current_morph_freq; 
        p *= l; 
        w_fractal_scale *= l; 
    }
    f = length(p) / w_fractal_scale;

    fractalHit_global = f < current_tunnel_radius_calc;
    return min(f, current_tunnel_radius_calc);
}


// --- Main Shader Logic ---
void main() {
    float current_time = TIME * global_speed; 

    vec2 res = RENDERSIZE.xy;
    vec2 U = gl_FragCoord.xy; 

    float xsize = res.x * 0.5;
    bool stereo = (stereo_mode > 0.5); 
    vec2 U_modified = U;

    if (stereo) {
        if (U_modified.x >= xsize) { 
            U_modified.x -= xsize;    
        }
    }
    
    vec2 u_norm = (U_modified - res.xy / 2.) / res.y;

    // Camera Roll based on input
    u_norm = rot(cam_roll) * u_norm;

    if (shake_enabled > 0.5) {
        u_norm -= apply_shake(current_time, shake_strength, shake_frequency);
    }
    
    vec3 ray_origin_base = P(current_time, current_time); 
    vec3 ray_origin = ray_origin_base; 

    vec3 look_target = P(current_time + 3.0, current_time) - orb(ray_origin_base, current_time) - ray_origin_base; 
    vec3 camera_Z_axis = N(look_target); 
    vec3 camera_X_axis = N(vec3(camera_Z_axis.z, 0.0, -camera_Z_axis.x)); 
    vec3 camera_Y_axis = cross(camera_X_axis, camera_Z_axis); 

    // Apply pitch and yaw rotations
    mat3 camera_rotation = rotateY(cam_yaw) * rotateX(cam_pitch);
    camera_X_axis = camera_rotation * camera_X_axis;
    camera_Y_axis = camera_rotation * camera_Y_axis;
    camera_Z_axis = camera_rotation * camera_Z_axis;


    vec3 ray_dir = vec3(u_norm, 1.0) * mat3(-camera_X_axis, camera_Y_axis, camera_Z_axis); 


    if (stereo) {
        vec3 cameraOffset_stereo = -camera_X_axis * parallax_amount; 
        if (U.x < xsize) { 
            ray_origin += cameraOffset_stereo;
        } else { 
            ray_origin -= cameraOffset_stereo;
        }
    }

    ray_origin += vec3(cam_pos_x, cam_pos_y, cam_pos_z);


    vec4 final_color = vec4(0.0); 
    float current_ray_distance = 0.0; 
    float step_size = 0.0; 

    for (float i = 0.0; i < max_raymarch_steps; i++) {
        vec3 p_ray = ray_origin + ray_dir * current_ray_distance; 
        
        step_size = map(p_ray) * raymarch_step_scale; 
        current_ray_distance += step_size; 

        if (step_size < 0.001 || current_ray_distance > 1000.0) break; 
    }

    vec3 hit_pos = ray_origin + ray_dir * current_ray_distance;
    
    vec3 e = vec3(.01,0,0);
    vec3 surface_normal = N(map(hit_pos) - vec3(map(hit_pos-e.xyy), map(hit_pos-e.yxy), map(hit_pos-e.yyx)));
    
    vec3 core_render_color = vec3(0.0);

    if (fractalHit_global) { 
        // Use procedural texture as the base for fractal surface color
        core_render_color = pow(procedural_tex(hit_pos, surface_normal, current_time), vec3(gamma_correction)); 
    } else {
        // Background noise for tunnel - now controlled by background_noise_intensity input
        core_render_color -= background_noise_intensity - abs(dot(sin(hit_pos * 1. * 32.), vec3( .05))); 
        core_render_color -= background_noise_intensity - abs(dot(sin(hit_pos * 1. * 64.), vec3( .05))); 
    }

    // Apply lighting and AO. Refined lighting_base for more neutral default.
    // lighting_saturation_bias influences how much color is mixed in.
    vec3 lighting_base = vec3(1.0); // Start with neutral white light
    lighting_base = mix(vec3(dot(lighting_base, vec3(0.333))), lighting_base, 1.0 + lighting_saturation_bias);

    vec4 current_lights = vec4(1.0);
    if (light_glimmer_enabled > 0.5 && fractalHit_global) {
        current_lights = vec4(abs((vec3(1.6, 1.2, .8)) /
                        dot(cos(light_glimmer_speed * current_time + hit_pos),vec3(.3)))*.2,0);
    }
    
    // Apply AO and refined lighting_base
    core_render_color *= AO(hit_pos, surface_normal) * lighting_base * current_lights.rgb; 
    
    // Depth fade: Now directly controlled by depth_fade_amount input. Default is 0.0 (off).
    core_render_color *= exp(-current_ray_distance * depth_fade_amount);

    // Apply fog. This function conditionally modifies color based on fog_enabled.
    core_render_color = apply_fog(core_render_color, current_ray_distance, current_time);

    // Now, apply the psychedelic color palette and its spatial "texture"
    float palette_t_value = current_time * palette_time_multiplier;
    // Add spatial influence from ray distance for "textured colors"
    palette_t_value += fract((current_ray_distance / 50.0) * palette_spatial_contribution); // Divide by 50.0 for more distinct spatial changes

    vec3 psychedelic_palette_color = getPal(int(color_palette_type), palette_t_value);

    // *** REVISED PALETTE MIXING LOGIC ***
    // This is crucial for avoiding monotonous, disgusting filter effects and preserving contrast.
    if (fractalHit_global) {
        // Option 1 (chosen): Multiply to layer palette color onto the fractal detail
        // This preserves light/shadow and texture while coloring.
        // The core_render_color already has AO and basic lighting.
        // By multiplying, we tint it with the palette. A multiplier > 1.0 can boost vibrancy.
        final_color.rgb = core_render_color * psychedelic_palette_color * 1.5; 
        
        // Add a bit more contrast at this stage
        final_color.rgb = pow(final_color.rgb, vec3(0.8)); 
        
    } else {
        // For background/tunnel, a subtle mix or direct palette color
        final_color.rgb = mix(core_render_color, psychedelic_palette_color, 0.3); // Subtle palette influence on background
    }
    
    // Apply tonemaping after all lighting and color generation
    final_color.rgb = TONEMAP(final_color.rgb);

    // Apply glitch effect
    final_color.rgb = apply_glitch(final_color.rgb, current_time, glitch_strength, glitch_frequency, gl_FragCoord.xy, RENDERSIZE.xy);

    // Apply brightness, saturation, contrast
    final_color.rgb = adjustColor(final_color.rgb, brightness, saturation, contrast);

    // Final output gain
    final_color.rgb *= output_gain;
    
    // Center line for stereo viewing
    if (stereo && abs(U.x - xsize) < 1.0) {
        final_color.rgb = vec3(1.0); 
    }

    gl_FragColor = vec4(clamp(final_color.rgb, 0.0, 1.0), 1.0);
}