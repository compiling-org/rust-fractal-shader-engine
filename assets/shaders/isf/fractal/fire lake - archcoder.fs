/*
{
  "DESCRIPTION": "3D Raymarched Menger Sponge Fractal with comprehensive tunable inputs and multiple color palette options.",
  "CATEGORIES": [ "Fractal", "Menger Sponge", "3D", "Raymarching", "Procedural", "Abstract", "Color Palette" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "crazyness", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls the step size in raymarching; higher values can be more 'eye-hurting' but faster." },
    { "NAME": "ambient_occlusion_steps", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for Ambient Occlusion calculation." },
    { "NAME": "ambient_occlusion_radius", "TYPE": "float", "DEFAULT": 0.165, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius for Ambient Occlusion sampling." },
    { "NAME": "ambient_occlusion_darkness", "TYPE": "float", "DEFAULT": 0.37, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of Ambient Occlusion darkening." },
    { "NAME": "vignette_strength", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the vignette effect." },
    { "NAME": "vignette_radius", "TYPE": "float", "DEFAULT": 0.925, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius of the vignette effect." },
    { "NAME": "glow_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Intensity of the glow effect." },
    { "NAME": "glow_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the glow color." },
    { "NAME": "glow_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the glow color." },
    { "NAME": "glow_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the glow color." },
    { "NAME": "glow_threshold", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 100.0, "DESCRIPTION": "Distance threshold for glow activation." },
    { "NAME": "glow_falloff", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Falloff rate of the glow effect." },
    { "NAME": "super_glow", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable super glow (0=Off, 1=On)." },
    { "NAME": "glow_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable overall glow (0=Off, 1=On)." },
    { "NAME": "fog_color_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the fog color." },
    { "NAME": "fog_color_g", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the fog color." },
    { "NAME": "fog_color_b", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the fog color." },
    { "NAME": "fog_density", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Density of the fog." },
    { "NAME": "fog_falloff", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Falloff rate of the fog." },
    { "NAME": "self_shadow_bias", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Bias for self-shadowing to prevent artifacts." },
    { "NAME": "shadow_darkness", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Darkness multiplier for shadows." },
    { "NAME": "shadow_steps", "TYPE": "float", "DEFAULT": 15.0, "MIN": 1.0, "MAX": 100.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for shadow raymarching." },
    { "NAME": "shadow_softness", "TYPE": "float", "DEFAULT": 64.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Softness of the shadows." },
    { "NAME": "min_step_size", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Minimum step size for shadow raymarching." },
    { "NAME": "light_intensity", "TYPE": "float", "DEFAULT": 3.6, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Overall intensity of lights." },
    { "NAME": "light1_position_x", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "X position of Light 1." },
    { "NAME": "light1_position_y", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Y position of Light 1." },
    { "NAME": "light1_position_z", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Z position of Light 1." },
    { "NAME": "light2_position_x", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "X position of Light 2." },
    { "NAME": "light2_position_y", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Y position of Light 2." },
    { "NAME": "light2_position_z", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Z position of Light 2." },
    { "NAME": "light1_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of Light 1 color." },
    { "NAME": "light1_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of Light 1 color." },
    { "NAME": "light1_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of Light 1 color." },
    { "NAME": "light2_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of Light 2 color." },
    { "NAME": "light2_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of Light 2 color." },
    { "NAME": "light2_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of Light 2 color." },
    { "NAME": "iterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "STEP": 1.0, "DESCRIPTION": "Number of fractal iterations." },
    { "NAME": "max_steps", "TYPE": "float", "DEFAULT": 120.0, "MIN": 1.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum raymarch steps." },
    { "NAME": "ambient_light", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Ambient light contribution." },
    { "NAME": "max_distance", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Maximum raymarch distance." },
    { "NAME": "surface_distance", "TYPE": "float", "DEFAULT": 0.004, "MIN": 0.00001, "MAX": 0.1, "DESCRIPTION": "Distance to consider a surface hit." },
    { "NAME": "raystep_multiplier", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Multiplier for raymarch step size." },
    { "NAME": "colors_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable palette coloring (0=Off, 1=On)." },
    { "NAME": "bg_color_r", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of background color." },
    { "NAME": "bg_color_g", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of background color." },
    { "NAME": "bg_color_b", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of background color." },
    { "NAME": "refraction_intensity", "TYPE": "float", "DEFAULT": 2.611, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Intensity of refraction/specular highlight." },
    { "NAME": "refraction_sharpness", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 30.0, "DESCRIPTION": "Sharpness of refraction/specular highlight." },
    { "NAME": "fractal_power", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 4.0, "DESCRIPTION": "Fractal power parameter." },
    { "NAME": "mengersponge_alpha_control", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "DESCRIPTION": "Alpha rotation for Menger Sponge (Y-axis)." },
    { "NAME": "mengersponge_beta_control", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "DESCRIPTION": "Beta rotation for Menger Sponge (X-axis)." },
    { "NAME": "mengersponge_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Fixed rotation, 1=Use alpha/beta controls." },
    { "NAME": "mengersponge_time_scale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Time scale for internal fractal animation." },
    { "NAME": "mengersponge_time_add", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Time phase offset for internal fractal animation." },
    { "NAME": "main_image_time_scale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Time scale for main image animation." },
    { "NAME": "main_image_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Animated m, 1=Use m_x/m_y controls." },
    { "NAME": "main_image_m_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "X component of 'm' parameter when toggle is on." },
    { "NAME": "main_image_m_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Y component of 'm' parameter when toggle is on." },
    { "NAME": "ray_origin_z", "TYPE": "float", "DEFAULT": -3.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z position of the ray origin." },
    { "NAME": "ray_dir_z", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Z component of the ray direction." },
    { "NAME": "raymarch_break_dist", "TYPE": "float", "DEFAULT": 0.0001, "MIN": 0.00001, "MAX": 0.01, "DESCRIPTION": "Distance threshold to break raymarch loop." },
    { "NAME": "raymarch_max_t", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 200.0, "DESCRIPTION": "Maximum total distance for raymarching." },
    { "NAME": "palette_t_mult", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.2, "DESCRIPTION": "Multiplier for 't' in palette function." },
    { "NAME": "palette_i_mult", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.001, "MAX": 0.02, "DESCRIPTION": "Multiplier for 'i' (iterations) in palette function." },
    { "NAME": "color_scheme_select", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 4.0, "STEP": 1.0, "DESCRIPTION": "Selects a color scheme: 0=Original, 1=Grayscale, 2=Warm Tones, 3=Cool Tones, 4=Custom." },
    { "NAME": "custom_palette_a_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'a' vector Red component." },
    { "NAME": "custom_palette_a_g", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'a' vector Green component." },
    { "NAME": "custom_palette_a_b", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'a' vector Blue component." },
    { "NAME": "custom_palette_b_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'b' vector Red component." },
    { "NAME": "custom_palette_b_g", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'b' vector Green component." },
    { "NAME": "custom_palette_b_b", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'b' vector Blue component." },
    { "NAME": "custom_palette_c_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'c' vector Red component." },
    { "NAME": "custom_palette_c_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'c' vector Green component." },
    { "NAME": "custom_palette_c_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'c' vector Blue component." },
    { "NAME": "custom_palette_d_r", "TYPE": "float", "DEFAULT": 0.563, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Red component." },
    { "NAME": "custom_palette_d_g", "TYPE": "float", "DEFAULT": 0.416, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Green component." },
    { "NAME": "custom_palette_d_b", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Blue component." }
  ]
}
*/

// Define constants
#define GOLDEN_RATIO 1.6180339887498948482
#define PI 3.14 

// ISF Built-in Uniforms (implicitly available, no explicit declaration needed)
// RENDERSIZE and TIME are directly available by name.

// Custom Input Uniforms (implicitly available by name from JSON INPUTS)
// All variables defined in the JSON "INPUTS" block are accessible directly by their "NAME".

// Function Prototypes (forward declarations)
mat2 rot2D(float angle);
vec3 original_palette( float t ); // Renamed original palette function
vec3 get_color_palette(float t_val, float i_val); // New function to handle multiple palettes
float smin(float a, float b, float k);
float sdC( vec3 p, vec2 c , float r);
float sdS( vec3 p, float r);
float mengersponge_sdf(vec3 p);
float sdB(vec3 p, vec3 b);
float sdO(vec3 p,float s);
float map(vec3 p,vec2 m);


// 2D Rotation Matrix
mat2 rot2D(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat2(c, -s, s, c);
}

// Original Palette function
vec3 original_palette( float t ){
    vec3 a = vec3(0.5,.5,.5);
    vec3 b = vec3(.5,.5,.5);
    vec3 c = vec3(1.,1.,1.);
    vec3 d = vec3(0.563,0.416,0.4);
    return a + b * cos( 6.28318 * (c * t + d) );
}

// New function to select and apply color palettes
vec3 get_color_palette(float t_val, float i_val) {
    // Combine t and i for palette input, as in original mainImage
    float palette_input = t_val * palette_t_mult + i_val * palette_i_mult;

    vec3 final_palette_color;

    if (color_scheme_select < 0.5) { // Scheme 0: Original Palette
        vec3 a = vec3(0.5,.5,.5);
        vec3 b = vec3(.5,.5,.5);
        vec3 c = vec3(1.,1.,1.);
        vec3 d = vec3(0.563,0.416,0.4);
        final_palette_color = a + b * cos( 6.28318 * (c * palette_input + d) );
    } else if (color_scheme_select < 1.5) { // Scheme 1: Grayscale
        final_palette_color = vec3(palette_input);
    } else if (color_scheme_select < 2.5) { // Scheme 2: Warm Tones
        vec3 a = vec3(0.8, 0.2, 0.1);
        vec3 b = vec3(0.2, 0.3, 0.1);
        vec3 c = vec3(1.0, 0.8, 0.6);
        vec3 d = vec3(0.0, 0.33, 0.67);
        final_palette_color = a + b * cos( 6.28318 * (c * palette_input + d) );
    } else if (color_scheme_select < 3.5) { // Scheme 3: Cool Tones
        vec3 a = vec3(0.1, 0.2, 0.8);
        vec3 b = vec3(0.1, 0.3, 0.2);
        vec3 c = vec3(0.6, 0.8, 1.0);
        vec3 d = vec3(0.0, 0.33, 0.67);
        final_palette_color = a + b * cos( 6.28318 * (c * palette_input + d) );
    } else { // Scheme 4: Custom Palette
        vec3 a = vec3(custom_palette_a_r, custom_palette_a_g, custom_palette_a_b);
        vec3 b = vec3(custom_palette_b_r, custom_palette_b_g, custom_palette_b_b);
        vec3 c = vec3(custom_palette_c_r, custom_palette_c_g, custom_palette_c_b);
        vec3 d = vec3(custom_palette_d_r, custom_palette_d_g, custom_palette_d_b);
        final_palette_color = a + b * cos( 6.28318 * (c * palette_input + d) );
    }
    return final_palette_color;
}

// Smooth Minimum function
float smin(float a, float b, float k){
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * h * k * (1.0 / 6.0);
}

// Signed Distance Function for Cylinder
float sdC( vec3 p, vec2 c , float r) {
    return length(p.xz - c) - r;
}

// Signed Distance Function for Sphere
float sdS( vec3 p, float r){
    return length(p) - r;
}

// Menger Sponge SDF function - adapted for ISF inputs
float mengersponge_sdf(vec3 p) {
    float scale = fractal_power; // Use fractal_power input
    float colorind = 0.0; // Unused in original, keeping for consistency
    float alpha_rot;
    float beta_rot;

    if (mengersponge_mouse_toggle > 0.5) { // Use mengersponge_mouse_toggle input
        alpha_rot = mengersponge_alpha_control; // Use mengersponge_alpha_control input
        beta_rot = mengersponge_beta_control;   // Use mengersponge_beta_control input
    } else {
        alpha_rot = 0.0;
        beta_rot = 0.0;
    }
    
    float orbit_trap = 1000000.0;
    int i = 0;
    
    // Use mengersponge_time_scale and mengersponge_time_add inputs
    float current_time_scale = mengersponge_time_scale;
    float current_time_add = mengersponge_time_add;

    for (; i < int(2.0); i++) { // Original loop runs only 2 times, hardcoded
        p.xz = vec2(cos(alpha_rot) * p.x - sin(alpha_rot) * p.z, sin(alpha_rot) * p.x + cos(alpha_rot) * p.z); // Y-axis rotation
        p.yz = vec2(cos(beta_rot) * p.y - sin(beta_rot) * p.z, sin(beta_rot) * p.y + cos(beta_rot) * p.z);      // X-axis rotation

        p = mix(p, abs(p), sin(TIME * current_time_scale * 0.25 + current_time_add) * 0.5 + 0.5);
        vec2 target_x = mix(p.xy, -p.xy, 0.0); // Original had 0.0 here, not time-dependent
        vec2 target_y = mix(p.yz, -p.yz, 0.0); // Original had 0.0 here, not time-dependent
        
        // Original logic for conditional mixing with time-dependent factors
        if (p.y > p.x) p.yx = mix(p.yx, target_x, sin(TIME * current_time_scale * 0.5 + current_time_add) * 0.5 + 0.5);
        if (p.z > p.y) p.zy = mix(p.yz, target_y, sin(TIME * current_time_scale + current_time_add) * 0.5 + 0.5);

        orbit_trap = min(orbit_trap, dot(p, p));

        p *= scale;
        if (p.z > 0.5 * (scale - 1.0)) {
            p -= vec3(1.0) * (scale - 1.0);
        } else {
            p -= vec3(1.0, 1.0, 0.0) * (scale - 1.0);
        }
    }

    float d = length(p) * pow(scale, -float(i));
    return d; // Original returns float d, not vec2. Orbit trap is not used as return.
}

// Signed Distance Function for Box
float sdB(vec3 p, vec3 b){
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Signed Distance Function for Octahedron
float sdO(vec3 p,float s){
    p = abs(p);
    return (p.x + p.y + p.z - s) * 0.57735027;
}

// Map function - adapted for ISF inputs
float map(vec3 p, vec2 m) {
    // Implicitly passed float inputs: main_image_time_scale

    vec3 q = p;
    
    q.xy *= rot2D(q.z * m.y * 0.1);
    
    q.xy = mod(q.xy, 5.0) - 2.5;
    
    q.z = fract(q.z) - 0.5;
    
    q.xy *= rot2D(p.z * m.x + TIME * main_image_time_scale); // Use TIME and main_image_time_scale
    
    float box = mengersponge_sdf(q);

    return box;
}


void main() { // ISF entry point
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y; // Use RENDERSIZE
    vec2 m;
    
    if (main_image_mouse_toggle < 0.5) { // Use main_image_mouse_toggle input
        // Original animated 'm'
        m = vec2(cos(TIME * main_image_time_scale), sin(TIME * main_image_time_scale)); // Use TIME and main_image_time_scale
    } else {
        // Use custom m_x/m_y inputs
        m = vec2(main_image_m_x, main_image_m_y);
    }
    
    //initialization
    vec3 ro = vec3(0.0, 0.0, ray_origin_z); // Use ray_origin_z input
    vec3 rd = normalize(vec3(uv, ray_dir_z)); // Use ray_dir_z input
    vec3 col = vec3(0.0);
    
    float t = 0.0; // total distance travelled
    
    //Raymarching
    
    int i;
    
    for (i = 0; i < int(max_steps); i++) { // Cast max_steps to int
        vec3 p = ro + rd * t;  //pos along the ray

        float d = map(p, m);  //current dist to scene

        t += d * crazyness;    //march the ray
        
        if (d < raymarch_break_dist || t > raymarch_max_t) break; // Use raymarch_break_dist and raymarch_max_t
        
    }
    
    //coloring
    // Use the new get_color_palette function
    col = get_color_palette(t, float(i));
    
    gl_FragColor = vec4(col, 1.0);
}
