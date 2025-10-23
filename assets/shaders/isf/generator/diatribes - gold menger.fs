/*{
    "CATEGORIES": [
        "Abstract",
        "Procedural",
        "3D",
        "Animated",
        "Psychedelic",
        "Volume",
        "Fractal"
    ],
    "DESCRIPTION": "A direct ISF conversion of Shane's 'Winding Menger Tunnel', now re-integrating ISF controls for 3D camera, shake, and glitch, and preparing for color palettes.",
    "CREDIT": "Original shader 'Winding Menger Tunnel' by Shane. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "global_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier (Original T was iTime * .5, so 1.0 here matches original speed relative to iTime)." },
        { "NAME": "cam_pos_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera X Position Offset." },
        { "NAME": "cam_pos_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera Y Position Offset." },
        { "NAME": "cam_pos_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -20.0, "MAX": 20.0, "DESCRIPTION": "Camera Z Position Offset." },
        { "NAME": "cam_pitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159, "DESCRIPTION": "Camera Pitch (Rotation around X axis) in Radians." },
        { "NAME": "cam_yaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159, "DESCRIPTION": "Camera Yaw (Rotation around Y axis) in Radians." },
        { "NAME": "cam_roll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159, "DESCRIPTION": "Camera Roll (Rotation around Z axis) in Radians." },
        { "NAME": "shake_enabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable camera shake effect." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Intensity of camera shake." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Overall strength of the glitch effect." },
        { "NAME": "glitch_speed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "DESCRIPTION": "Speed of the glitch animation." },
        { "NAME": "glitch_resolution_scale", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5, "DESCRIPTION": "Resolution scaling for glitch, lower creates larger blocks." },
        { "NAME": "background_noise_intensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of background noise where fractal is not hit." },
        { "NAME": "raymarch_step_scale", "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Multiplier for raymarch step size (Original: 0.65)." },
        { "NAME": "max_raymarch_steps", "TYPE": "float", "DEFAULT": 180.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "tunnel_radius_factor", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Base radius for the tunnel (Original: 3.0)." },
        { "NAME": "menger_base_scale", "TYPE": "float", "DEFAULT": 32.0, "MIN": 1.0, "MAX": 64.0, "DESCRIPTION": "Initial scaling factor for the Menger fractal (Original: 32.0)." },
        { "NAME": "menger_center_offset_x", "TYPE": "float", "DEFAULT": 1.5, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "X offset applied to the fractal center (Original: 1.5)." },
        { "NAME": "fractal_iterations", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 20.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the inner fractal loop (Original: 7)." },
        { "NAME": "fractal_abs_sin_offset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Offset applied after abs(sin(p)) in fractal iteration (Original: 1.0)." },
        { "NAME": "fractal_l_scale_num", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Numerator for scaling factor 'l' for fractal iterations (Original: 1.7)." },
        { "NAME": "menger_layer_1_scale", "TYPE": "float", "DEFAULT": 16.0, "MIN": 1.0, "MAX": 32.0, "DESCRIPTION": "Scale for the first Menger layer (Original: 16.0)." },
        { "NAME": "menger_layer_1_hole", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "DESCRIPTION": "Hole size factor for the first Menger layer (Original: 3.0)." },
        { "NAME": "menger_layer_2_scale", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 32.0, "DESCRIPTION": "Scale for the second Menger layer (Original: 8.0)." },
        { "NAME": "menger_layer_2_hole", "TYPE": "float", "DEFAULT": 3.5, "MIN": 1.0, "MAX": 10.0, "DESCRIPTION": "Hole size factor for the second Menger layer (Original: 3.5)." },
        { "NAME": "menger_layer_3_scale", "TYPE": "float", "DEFAULT": 16.0, "MIN": 1.0, "MAX": 32.0, "DESCRIPTION": "Scale for the third Menger layer (Original: 16.0)." },
        { "NAME": "menger_layer_3_hole", "TYPE": "float", "DEFAULT": 4.5, "MIN": 1.0, "MAX": 10.0, "DESCRIPTION": "Hole size factor for the third Menger layer (Original: 4.5)." },
        { "NAME": "texture_scale_ch1", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Scale for procedural texture 1 (Original iChannel1: p*2.5)." },
        { "NAME": "texture_scale_ch2", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Scale for procedural texture 2 (Original iChannel2: p*1.5)." },
        { "NAME": "final_gamma_power", "TYPE": "float", "DEFAULT": 2.2, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Power applied to textures (Original: vec3(2.2))." },
        { "NAME": "glimmer_strength", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Strength of the light glimmer effect (Original: 0.01)." },
        { "NAME": "glimmer_speed", "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the light glimmer oscillation (Original: 0.75)." },
        { "NAME": "min_light_dot", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Minimum dot product for light, prevents full black (Original: 0.05)." },
        { "NAME": "ao_reflectivity", "TYPE": "float", "DEFAULT": 12.0, "MIN": 0.0, "MAX": 20.0, "DESCRIPTION": "Multiplier for ambient occlusion (Original: 12.0)." },
        { "NAME": "fog_distance", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Fog distance divisor (Original: exp(-d/1.5))." },
        { "NAME": "vignette_strength", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the vignette effect (Original: dot(u,u)*.3)." },
        { "NAME": "final_power_factor", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Final power applied to the image (Original: vec3(.45))." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "STEP": 1.0, "DESCRIPTION": "Selects different color palettes for the fractal." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of color palette animation." },
        { "NAME": "palette_spatial_contribution", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "How much ray depth influences palette color (0.0 for time-only)." }

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

// Shader-specific time for P() function to match original behavior
#define SHADER_TIME (TIME * .5 * global_speed) 

// P(z) from original shader
#define P(z) (vec3((cos((z) * .3) * .4) * 8., \
                   (cos((z) * .25) * .4) * 8., (z)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define N normalize
#define inf 9e9

bool orbHit = false; 

// --- Core Utility Functions ---

// From iq's shadertoy common
float hash( float n ) { return fract(sin(n)*43758.5453123); }
float noise( in vec3 x ) {
    vec3 p = floor(x);
    vec3 f = fract(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*157.0 + 113.0*p.z;
    return mix(mix(mix( hash(n+0.0), hash(n+1.0), f.x),
                   mix( hash(n+157.0), hash(n+158.0), f.x), f.y),
               mix(mix( hash(n+113.0), hash(n+114.0), f.x),
                   mix( hash(n+270.0), hash(n+271.0), f.x), f.y), f.z);
}

// Procedural 3D texture generator 
vec3 procedural_tex3D(in vec3 p, in vec3 n, float time_in, float scale_factor) {
    n = max((abs(n) - 0.2) * 7., 0.001);
    n /= (n.x + n.y + n.z); 

    float noise_r = noise(p * scale_factor * 0.7 + time_in * 0.1);
    float noise_g = noise(p * scale_factor * 0.8 + time_in * 0.15 + 100.0);
    float noise_b = noise(p * scale_factor * 0.9 + time_in * 0.2 + 200.0);

    vec3 base_color_val;
    base_color_val.r = (noise_r + noise_g * 0.5 + noise_b * 0.25) / 1.75;
    
    base_color_val.g = (noise(p.yzx * scale_factor * 0.8 + time_in * 0.05 + 300.0) + noise(p.xzy * scale_factor * 1.1 + time_in * 0.1 + 400.0)) / 2.0;
    base_color_val.b = (noise(p.zxy * scale_factor * 0.6 + time_in * 0.2 + 500.0) + noise(p.xyz * scale_factor * 1.3 + time_in * 0.25 + 600.0)) / 2.0;

    vec3 final_blended_color = base_color_val.xyz * n.x + 
                               base_color_val.yzx * n.y + 
                               base_color_val.zxy * n.z;
    
    return final_blended_color;
}

// --- Rotation Matrix Functions (DEFINED HERE) ---
mat3 rotateX(float angle) {
    return mat3(
        1.0, 0.0, 0.0,
        0.0, cos(angle), -sin(angle),
        0.0, sin(angle), cos(angle)
    );
}

mat3 rotateY(float angle) {
    return mat3(
        cos(angle), 0.0, sin(angle),
        0.0, 1.0, 0.0,
        -sin(angle), 0.0, cos(angle)
    );
}

mat3 rotateZ(float angle) {
    return mat3(
        cos(angle), -sin(angle), 0.0,
        sin(angle), cos(angle), 0.0,
        0.0, 0.0, 1.0
    );
}


// Camera Shake Utility Function
vec2 apply_shake(float current_time, float strength, float frequency) {
    vec2 shake = vec2(
        sin(current_time * frequency * 1.1) * cos(current_time * frequency * 0.7) * strength,
        cos(current_time * frequency * 1.3) * sin(current_time * frequency * 0.8) * strength
    );
    return shake;
}

// Glitch Utility Function - NOW PURELY PROCEDURAL (NO TEXTURE_BUFFER_0)
vec2 apply_glitch(vec2 frag_coord_norm, float current_time, float strength, float resolution_scale) {
    // frag_coord_norm is already normalized (0-1 range or -0.5 to 0.5 range)
    // Scale it up to create blocks
    vec2 block_coord = floor(frag_coord_norm * RENDERSIZE.xy * resolution_scale);

    // Create a time-varying offset based on block_coord
    float offset_x_noise = hash(block_coord.y * 100.0 + current_time * 50.0);
    float offset_y_noise = hash(block_coord.x * 100.0 + current_time * 50.0 + 123.45);

    // Apply a horizontal shift (scanline effect)
    float horizontal_shift = sin(block_coord.y * 100.0 + current_time * 50.0) * 0.1;

    // Apply random vertical jitter
    float vertical_jitter = fract(sin(current_time * 10.0 + block_coord.x * 5.0) * 1000.0) * 0.1;

    // Combine for a glitch offset
    vec2 glitch_offset = vec2(offset_x_noise, offset_y_noise) * 0.5; // Base noise offset
    glitch_offset.x += horizontal_shift;
    glitch_offset.y += vertical_jitter;
    
    // Scale by strength and apply to normalized coordinates
    return glitch_offset * strength;
}

// Color Palette Function
vec3 get_color_palette(float value, float type, float time_offset, float spatial_contribution) {
    vec3 color = vec3(0.0);
    float t = time_offset * palette_time_multiplier; 

    if (type < 0.5) { // Palette 0: Psychedelic spectrum
        color = 0.5 + 0.5 * cos(6.28318 * (vec3(value, value + 0.166, value + 0.333) * 2.0 + t * 0.5));
    } else if (type < 1.5) { // Palette 1: Fire/lava
        color = mix(vec3(0.1, 0.0, 0.0), vec3(1.0, 0.5, 0.0), value);
        color = mix(color, vec3(1.0, 1.0, 0.5), smoothstep(0.7, 1.0, value));
    } else if (type < 2.5) { // Palette 2: Cold/ice
        color = mix(vec3(0.0, 0.0, 0.1), vec3(0.0, 0.5, 1.0), value);
        color = mix(color, vec3(0.5, 1.0, 1.0), smoothstep(0.7, 1.0, value));
    } else { // Palette 3: Green/Purple "Cyber"
        color = mix(vec3(0.1, 0.6, 0.1), vec3(0.8, 0.2, 0.8), value);
        color = mix(color, vec3(0.2, 1.0, 0.2), smoothstep(0.8, 1.0, value));
    }
    
    float spatial_mod = spatial_contribution * sin(value * 10.0 + t * 2.0);
    color += spatial_mod; 

    return clamp(color, 0.0, 1.0);
}


// @Shane's Menger function hacked up - direct copy
#define MENGERLAYER(scale, minmax, hole)\
    s /= (scale), \
    p_local_menger_macro_var = abs(fract(q_local_menger_macro_var/s)*s - s*.5), \
    d = minmax(d, min(max(p_local_menger_macro_var.x, p_local_menger_macro_var.y), \
                      min(max(p_local_menger_macro_var.y, p_local_menger_macro_var.z), \
                      max(p_local_menger_macro_var.x, p_local_menger_macro_var.z))) - s/(hole))

float fractal(in vec3 q_in){
    float d, s = menger_base_scale; 
    
    vec3 q_local_menger_macro_var = q_in; 
    vec3 p_local_menger_macro_var; 

    vec3 p_fractal = q_in * 2.; 
    p_fractal = mod(p_fractal, 5.) - 2.5;
    p_fractal.x -= menger_center_offset_x; 
    
    float l, w = 1.;
    for (int i=0; i < int(fractal_iterations); i++, p_fractal *= l, w *= l ) {
        p_fractal = abs(sin(p_fractal)) - fractal_abs_sin_offset; 
        l = fractal_l_scale_num / dot(p_fractal, p_fractal); 
    }
    
    d = length(p_fractal)/w;

    MENGERLAYER(menger_layer_1_scale, max, menger_layer_1_hole); 
    MENGERLAYER(menger_layer_2_scale, max, menger_layer_2_hole); 
    MENGERLAYER(menger_layer_3_scale, max, menger_layer_3_hole); 
    
    return d;
}


// map function - direct copy
float map(vec3 p) {
    float s_val; 
    vec3 q_for_fractal = p; 
    
    p.xy -= P(p.z).xy; 

    s_val = inf;
    float r_tunnel = tunnel_radius_factor; 
    s_val = min(s_val, r_tunnel - (p.x) - (p.y));
    s_val = min(s_val, r_tunnel - (p.y) + (p.x));
    s_val = min(s_val, r_tunnel + (p.y) - (p.x));
    s_val = min(s_val, r_tunnel + (p.y) + (p.x));

    s_val = min(s_val, fractal(q_for_fractal)); 
    
    return s_val;
}

// AO function - direct copy
float AO(in vec3 pos, in vec3 nor) { 
	float sca = 2.0, occ = 0.0;
    for( int i=0; i<5; i++ ){
        float hr = 0.01 + float(i)*0.5/4.0;
        float dd = map(nor * hr + pos); 
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - occ, 0.0, 1.0 );
}

void main() { 
    vec4 o = vec4(0.0); 
    vec2 u_frag_coord = gl_FragCoord.xy; // Store original fragment coord for glitch

    vec2 u = u_frag_coord; // This 'u' will be normalized and potentially shaken
    
    float s_step = 0.002; 
    float d_dist = 0.0; 
    float i_loop = 0.0; 

    vec2 res = RENDERSIZE; 

    // Apply shake to normalized fragment coordinates
    if (shake_enabled > 0.5) {
        // Apply shake to the screen-space 'u' before normalization.
        // This causes the visible 'shake' effect.
        u -= apply_shake(TIME, shake_strength, shake_frequency) * res.y; 
    }

    u = (u - res / 2.0) / res.y; 

    vec3 e_normal_delta = vec3(.01,0,0); 
    vec3 p_cam_current_pos = P(SHADER_TIME); 
    vec3 ro = p_cam_current_pos; 

    vec3 Z_cam = N( P(SHADER_TIME + 1.0) - p_cam_current_pos); 

    vec3 X_cam = N(vec3(Z_cam.z,0.0,-Z_cam.x)); 
    vec3 Y_cam = cross(X_cam, Z_cam); 

    // Apply ISF camera rotations using the newly defined functions
    mat3 manual_camera_rotation = rotateY(cam_yaw) * rotateX(cam_pitch) * rotateZ(cam_roll);
    X_cam = manual_camera_rotation * X_cam;
    Y_cam = manual_camera_rotation * Y_cam;
    Z_cam = manual_camera_rotation * Z_cam;

    float cam_look_angle = tanh(sin(p_cam_current_pos.z * 0.05) * 3.0) * 3.5;
    vec3 D_ray_dir = vec3(u.x * X_cam + u.y * Y_cam + 1.0 * Z_cam); 
    
    // Apply manual camera position offsets
    ro += vec3(cam_pos_x, cam_pos_y, cam_pos_z);

    // Raymarching loop
    for(i_loop = 0.0; i_loop < max_raymarch_steps && s_step > 0.001; i_loop++) {
        vec3 p_raymarch_current = ro + D_ray_dir * d_dist;
        d_dist += (s_step = map(p_raymarch_current) * raymarch_step_scale);
    }
    
    vec3 p_hit_pos = ro + D_ray_dir * d_dist;

    vec3 surface_normal = N(vec3(map(p_hit_pos+e_normal_delta.xyy) - map(p_hit_pos-e_normal_delta.xyy),
                                 map(p_hit_pos+e_normal_delta.yxy) - map(p_hit_pos-e_normal_delta.yxy),
                                 map(p_hit_pos+e_normal_delta.yyx) - map(p_hit_pos-e_normal_delta.yyx)));
    
    // Apply base procedural texture
    vec3 texture_color;
    if (mod(p_hit_pos.z, 10.0) > 5.0) { 
        texture_color = procedural_tex3D(p_hit_pos, surface_normal, SHADER_TIME, texture_scale_ch1); 
    } else { 
        texture_color = procedural_tex3D(p_hit_pos, surface_normal, SHADER_TIME, texture_scale_ch2);
        texture_color *= 3.0;
        texture_color.rg *= 1.5;
    }

    // Apply color palette to the texture_color
    float palette_value = fract(d_dist * 0.1 + SHADER_TIME * palette_time_multiplier); 
    palette_value += palette_spatial_contribution * (p_hit_pos.z * 0.01 + p_hit_pos.x * 0.01);
    
    vec3 palette_result_color = get_color_palette(palette_value, color_palette_type, SHADER_TIME, palette_spatial_contribution);
    o.rgb = palette_result_color * texture_color; // Blend palette with texture
    o.rgb = pow(o.rgb, vec3(final_gamma_power));


    // Lights (glimmer effect)
    vec3 lights = abs((vec3(1.6, 1.2, .8)) /
                      dot(cos(glimmer_speed * SHADER_TIME + p_hit_pos * 0.3), vec3(0.01))) * glimmer_strength;

    // Lighting application
    o.rgb *= max(dot(surface_normal, N(ro - p_hit_pos)), min_light_dot);
    
    // Ambient Occlusion and Reflectivity
    o.rgb *= AO(p_hit_pos, surface_normal) * ao_reflectivity;
    
    // Apply glimmer lights
    o.rgb *= lights;

    // Background noise for unhit areas
    if (d_dist >= 1000.0) { 
        o.rgb = vec3(0.0) + noise(u.xyx * 5.0 + SHADER_TIME * 0.5) * background_noise_intensity;
    }


    // Final Post-Processing
    o.rgb = pow(tanh(vec3(1.5,1.,.7) * o.rgb * exp(-d_dist/fog_distance))
                 - dot(u,u) * vignette_strength,
                 vec3(final_power_factor));

    // Apply Glitch effect as a final post-processing step
    if (glitch_strength > 0.0) {
        vec2 glitch_offset = apply_glitch(u_frag_coord / RENDERSIZE.xy, SHADER_TIME * glitch_speed, glitch_strength, glitch_resolution_scale);
        
        // Apply color distortion based on glitch_offset
        // These are basic color manipulations that simulate visual corruption.
        o.r += sin(glitch_offset.x * 10.0 + SHADER_TIME * 50.0) * 0.2 * glitch_strength;
        o.g += cos(glitch_offset.y * 15.0 + SHADER_TIME * 60.0) * 0.2 * glitch_strength;
        o.b += sin(glitch_offset.x * glitch_offset.y * 20.0 + SHADER_TIME * 70.0) * 0.2 * glitch_strength;
        
        // Add some "scanline" or blocky noise for visual effect
        float block_noise = hash(floor(u_frag_coord.y * glitch_resolution_scale * 10.0) + SHADER_TIME * 100.0) * 0.2;
        o.rgb += block_noise * glitch_strength;

        // Optionally, some color channel swapping or intensity modulation
        // o.rgb = o.gbr; // Example of channel swap
        // o.rgb *= (1.0 - abs(glitch_offset.x)); // Dimming based on glitch
    }


    gl_FragColor = vec4(clamp(o.rgb, 0.0, 1.0), 1.0);
}