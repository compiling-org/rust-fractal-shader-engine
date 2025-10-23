/*
{
  "DESCRIPTION": "3D Raymarched Fractal with tunable inputs for Ambient Occlusion, Vignette, Glow, Fog, Shadows, Lighting, Rendering, Coloring, Refraction, and detailed fractal-specific parameters and camera controls. Features multiple color palette options.",
  "CATEGORIES": [ "Fractal", "3D", "Raymarching", "Procedural", "Abstract", "Color Palette" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "ambient_occlusion_steps", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for Ambient Occlusion calculation." },
    { "NAME": "ambient_occlusion_radius", "TYPE": "float", "DEFAULT": 0.165, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius for Ambient Occlusion sampling." },
    { "NAME": "ambient_occlusion_darkness", "TYPE": "float", "DEFAULT": 0.57, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of Ambient Occlusion darkening." },
    { "NAME": "vignette_strength", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the vignette effect." },
    { "NAME": "vignette_radius", "TYPE": "float", "DEFAULT": 0.925, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius of the vignette effect." },
    { "NAME": "glow_intensity", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Intensity of the glow effect." },
    { "NAME": "glow_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the glow color." },
    { "NAME": "glow_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the glow color." },
    { "NAME": "glow_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the glow color." },
    { "NAME": "glow_threshold", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 100.0, "DESCRIPTION": "Distance threshold for glow activation." },
    { "NAME": "glow_falloff", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Falloff rate of the glow effect." },
    { "NAME": "super_glow", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable super glow (0=Off, 1=On)." },
    { "NAME": "glow_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable overall glow (0=Off, 1=On)." },
    { "NAME": "fog_color_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the fog color." },
    { "NAME": "fog_color_g", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the fog color." },
    { "NAME": "fog_color_b", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the fog color." },
    { "NAME": "fog_density", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Density of the fog." },
    { "NAME": "fog_falloff", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Falloff rate of the fog." },
    { "NAME": "self_shadow_bias", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Bias for self-shadowing to prevent artifacts." },
    { "NAME": "shadow_darkness", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Darkness multiplier for shadows." },
    { "NAME": "shadow_steps", "TYPE": "float", "DEFAULT": 15.0, "MIN": 1.0, "MAX": 100.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for shadow raymarching." },
    { "NAME": "shadow_softness", "TYPE": "float", "DEFAULT": 64.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Softness of the shadows." },
    { "NAME": "min_step_size", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Minimum step size for shadow raymarching." },
    { "NAME": "light_intensity", "TYPE": "float", "DEFAULT": 5.6, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Overall intensity of lights." },
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
    { "NAME": "iterations", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of fractal iterations." },
    { "NAME": "max_steps", "TYPE": "float", "DEFAULT": 70.0, "MIN": 1.0, "MAX": 200.0, "STEP": 1.0, "DESCRIPTION": "Maximum raymarch steps." },
    { "NAME": "ambient_light", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Ambient light contribution." },
    { "NAME": "max_distance", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Maximum raymarch distance." },
    { "NAME": "surface_distance", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.00001, "MAX": 0.1, "DESCRIPTION": "Distance to consider a surface hit." },
    { "NAME": "raystep_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Multiplier for raymarch step size." },
    { "NAME": "colors_enabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable palette coloring (0=Off, 1=On)." },
    { "NAME": "palette_color1_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of first palette color (used when colors_enabled is on)." },
    { "NAME": "palette_color1_g", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of first palette color (used when colors_enabled is on)." },
    { "NAME": "palette_color1_b", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of first palette color (used when colors_enabled is on)." },
    { "NAME": "palette_color2_r", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of second palette color (used when colors_enabled is on)." },
    { "NAME": "palette_color2_g", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of second palette color (used when colors_enabled is on)." },
    { "NAME": "palette_color2_b", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of second palette color (used when colors_enabled is on)." },
    { "NAME": "bg_color_r", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of background color." },
    { "NAME": "bg_color_g", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of background color." },
    { "NAME": "bg_color_b", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of background color." },
    { "NAME": "refraction_intensity", "TYPE": "float", "DEFAULT": 2.611, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Intensity of refraction/specular highlight." },
    { "NAME": "refraction_sharpness", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 30.0, "DESCRIPTION": "Sharpness of refraction/specular highlight." },
    { "NAME": "fractal_power_animated_toggle", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Manual Fractal Power, 1=Animated Fractal Power." },
    { "NAME": "fractal_power_manual", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Manual fractal power value." },
    { "NAME": "fractal_power_time_scale", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Time scale for animated fractal power." },
    { "NAME": "fractal_power_amplitude", "TYPE": "float", "DEFAULT": 6.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Amplitude for animated fractal power." },
    { "NAME": "fractal_power_exponent", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Exponent for animated fractal power." },
    { "NAME": "fractal_power_base", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Base value for animated fractal power." },
    { "NAME": "fractal_c_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Manual C parameter, 1=Mouse-like C parameter." },
    { "NAME": "fractal_c_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "X component of fractal 'c' parameter." },
    { "NAME": "fractal_c_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Y component of fractal 'c' parameter." },
    { "NAME": "fractal_c_z", "TYPE": "float", "DEFAULT": 3.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Z component of fractal 'c' parameter." },
    { "NAME": "fractal_c_scale_xy", "TYPE": "float", "DEFAULT": -2.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Scale for X/Y components of mouse-like 'c'." },
    { "NAME": "fractal_c_mouse_z_value", "TYPE": "float", "DEFAULT": 3.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z component of fractal 'c' parameter when mouse-like toggle is on." },
    { "NAME": "triplex_pow_phase_time_scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Time scale for triplexPow phase." },
    { "NAME": "triplex_pow_phase_offset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Offset for triplexPow phase." },
    { "NAME": "clamp_val_1_min", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Min value for first clamp in fractal_sdf." },
    { "NAME": "clamp_val_1_max", "TYPE": "float", "DEFAULT": 2.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Max value for first clamp in fractal_sdf." },
    { "NAME": "clamp_val_2_min", "TYPE": "float", "DEFAULT": -3.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Min value for second clamp in fractal_sdf." },
    { "NAME": "clamp_val_2_max", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Max value for second clamp in fractal_sdf." },
    { "NAME": "fractal_break_radius", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Radius at which fractal iteration breaks." },
    { "NAME": "fractal_log_mult", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Multiplier for log(r) in fractal_sdf return." },
    { "NAME": "main_time_scale", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Time scale for main image animation." },
    { "NAME": "camera_target_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "X position of camera target." },
    { "NAME": "camera_target_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Y position of camera target." },
    { "NAME": "camera_target_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z position of camera target." },
    { "NAME": "camera_origin_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "X position of camera origin (offset from target)." },
    { "NAME": "camera_origin_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Y position of camera origin (offset from target)." },
    { "NAME": "camera_origin_z", "TYPE": "float", "DEFAULT": 6.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z position of camera origin (offset from target)." },
    { "NAME": "camera_roll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "DESCRIPTION": "Camera roll angle." },
    { "NAME": "ray_direction_z_scale", "TYPE": "float", "DEFAULT": 4.5, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Z component scale for ray direction." },
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
    { "NAME": "custom_palette_d_r", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Red component." },
    { "NAME": "custom_palette_d_g", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Green component." },
    { "NAME": "custom_palette_d_b", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Custom palette 'd' vector Blue component." }
  ]
}
*/

#define GOLDEN_RATIO 1.6180339887498948482
#define PI 3.14159265359

// ISF Built-in Uniforms (implicitly available, no explicit declaration needed)
// RENDERSIZE and TIME are directly available by name.

// Custom Input Uniforms (implicitly available by name from JSON INPUTS)
// All variables defined in the JSON "INPUTS" block are accessible directly by their "NAME".

// Global variable for fractal power, set in main() based on inputs
float current_fractal_power;

// Function Prototypes
mat3 setCamera( in vec3 ro, in vec3 ta, float cr );
vec3 triplexPow(vec3 z, float phase);
vec2 fractal_sdf(vec3 p);
vec3 ray_marcher(vec3 ro, vec3 rd);
float soft_shadow(vec3 p, vec3 light_pos, float k);
vec3 get_light(vec3 p, vec3 rd, vec3 ro, vec3 light_pos, vec3 light_color, vec3 normal);
vec3 calculate_normal(vec3 p);
float calculate_ambient_occlusion(vec3 p, vec3 normal);
vec3 render(vec3 ray_origin, vec3 ray_dir, vec2 screen_uv);
vec3 get_color_palette(float orbit_trap_val, float colors_enabled_val, vec3 palette1, vec3 palette2);


vec3 triplexPow(vec3 z, float phase) {
    float r = length(z);
    float theta = atan(z.y, z.x);
    float phi = acos(z.z / r);
    r = pow(r, current_fractal_power); // Use current_fractal_power
    theta *= current_fractal_power;    // Use current_fractal_power
    phi = phi * current_fractal_power + phase; // Use current_fractal_power
    return vec3(r * sin(phi) * cos(theta), r * sin(phi) * sin(theta), r * cos(phi));
}

vec2 fractal_sdf(vec3 p) {
    vec3 c_param;
    if (fractal_c_mouse_toggle > 0.5) { // Check toggle
        // Mimic iMouse.xy / iResolution.xy * -2.0, 3.0
        // Assuming fractal_c_x and fractal_c_y are normalized 0-1 for mouse-like behavior
        c_param = vec3(fractal_c_x * fractal_c_scale_xy, fractal_c_y * fractal_c_scale_xy, fractal_c_mouse_z_value); // FIX: Used fractal_c_mouse_z_value
    } else {
        c_param = vec3(fractal_c_x, fractal_c_y, fractal_c_z);
    }

    vec3 z = p;
    float dr = 1.0;
    float r = length(z);
    float orbit_trap = 1e20;

    for (int i = 0; i < int(iterations); i++) { // Cast iterations to int
        float theta = acos(z.z / r);
        float phi = atan(z.y, z.x);
        float zr = pow(r, current_fractal_power); // Use current_fractal_power

        dr = current_fractal_power * pow(r, current_fractal_power - 1.0) * dr + 1.0; // Use current_fractal_power
        theta *= current_fractal_power; // Use current_fractal_power
        phi *= current_fractal_power;   // Use current_fractal_power

        // Use triplex_pow_phase_time_scale and triplex_pow_phase_offset
        z = triplexPow(z, TIME * triplex_pow_phase_time_scale + triplex_pow_phase_offset) + c_param;
        
        // Use clamp_val_1_min/max and clamp_val_2_min/max
        z = 2.0 * clamp(z, clamp_val_1_min, clamp_val_1_max) - z;
        z = 2.0 * clamp(z, clamp_val_2_min, clamp_val_2_max) - z;

        r = length(z);

        orbit_trap = min(orbit_trap, dot(z, z));
        if (r > fractal_break_radius) break; // Use fractal_break_radius
    }

    return vec2(fractal_log_mult * log(r) * r / dr, orbit_trap).yx; // Use fractal_log_mult
}


vec3 ray_marcher(vec3 ro, vec3 rd) {
    // Implicitly passed float inputs: max_steps, raystep_multiplier, surface_distance, max_distance, super_glow
    float dfo = 0.0;
    float orbit_trap_distance;
    float total_marches = 0.0;

    for (int i = 0; i < int(max_steps); i++) { // Cast max_steps to int
        vec2 data = fractal_sdf(ro + rd * dfo);
        float point_distance = data.y;
        dfo += point_distance * raystep_multiplier;
        total_marches += 1.0;

        if (abs(point_distance) < surface_distance || dfo > max_distance) {
            orbit_trap_distance = data.x;
            break;
        };
    }

    if (super_glow > 0.5 && dfo < max_distance) { total_marches = float(max_steps); } // Check super_glow as float

    return vec3(dfo > max_distance ? 0.0 : orbit_trap_distance, dfo, total_marches);
}

float soft_shadow(vec3 p, vec3 light_pos, float k) {
    // Implicitly passed float inputs: surface_distance, self_shadow_bias, shadow_steps, min_step_size, max_distance
    vec3 rd = normalize(light_pos - p);
    float res = 1.0;
    float ph = 1e20;
    float t = surface_distance + self_shadow_bias;

    for (int i = 0; i < int(shadow_steps); i++) { // Cast shadow_steps to int
        float h = fractal_sdf(p + rd * t).y;

        if (h < surface_distance) {
            return 0.0;
        }

        float y = h * h / (2.0 * ph);
        float d = sqrt(h * h - y * y);
        res = min(res, k * d / max(0.0, t - y));
        ph = h;

        t += max(h, min_step_size);

        if (t >= max_distance) {
            break;
        }
    }

    return clamp(res, 0.0, 1.0);
}

vec3 get_light(vec3 p, vec3 rd, vec3 ro, vec3 light_pos, vec3 light_color, vec3 normal) {
    // Implicitly passed float inputs: light_intensity, ambient_light, shadow_darkness, shadow_softness, refraction_intensity, refraction_sharpness
    vec3 to_light = normalize(light_pos - p);
    float light = light_intensity * clamp(dot(to_light, normal), 0.05, 1.0);

    float shadow = soft_shadow(p, light_pos, shadow_softness);
    light *= max(shadow, shadow_darkness);
    vec3 reflection = reflect(to_light, normal);
    float specular = pow(max(dot(reflection, rd), 0.0), refraction_sharpness);
    light *= max(specular * refraction_intensity, 1.0);

    return max(light_color * light, ambient_light);
}


vec3 calculate_normal(vec3 p) {
    float h = 0.000001;
    return normalize(vec3(
        fractal_sdf(p + vec3(h, 0.0, 0.0)).y - fractal_sdf(p - vec3(h, 0.0, 0.0)).y,
        fractal_sdf(p + vec3(0.0, h, 0.0)).y - fractal_sdf(p - vec3(0.0, h, 0.0)).y,
        fractal_sdf(p + vec3(0.0, 0.0, h)).y - fractal_sdf(p - vec3(0.0, 0.0, h)).y
    ));
}

float calculate_ambient_occlusion(vec3 p, vec3 normal) {
    // Implicitly passed float inputs: ambient_occlusion_steps, ambient_occlusion_radius, ambient_occlusion_darkness
    float occlusion = 0.0;
    float weight = 1.0 / float(ambient_occlusion_steps);

    for (int i = 0; i < int(ambient_occlusion_steps); i++) { // Cast to int
        float ao_scale = float(i + 1) / float(ambient_occlusion_steps);
        vec3 sample_point = p + normal * ao_scale * ambient_occlusion_radius;
        float d = fractal_sdf(sample_point).y;
        occlusion += max(ambient_occlusion_radius - d, 0.0) * weight / ambient_occlusion_radius;
    }

    return 1.0 - clamp(occlusion, 0.0, 1.0) * ambient_occlusion_darkness; // Apply darkness
}

// New function for versatile color palettes
vec3 get_color_palette(float orbit_trap_val, float colors_enabled_val, vec3 palette1, vec3 palette2) {
    vec3 final_color;
    // Original logic: mix(palette_color1, palette_color2, mix(0.0, orbit_trap, float(int(colors))));
    vec3 original_mix = mix(palette1, palette2, mix(0.0, orbit_trap_val, colors_enabled_val));

    if (color_scheme_select < 0.5) { // Scheme 0: Original Palette (based on palette_color1/2 inputs)
        final_color = original_mix;
    } else if (color_scheme_select < 1.5) { // Scheme 1: Grayscale
        final_color = vec3(orbit_trap_val);
    } else if (color_scheme_select < 2.5) { // Scheme 2: Warm Tones
        vec3 a = vec3(0.8, 0.2, 0.1);
        vec3 b = vec3(0.2, 0.3, 0.1);
        vec3 c = vec3(1.0, 0.8, 0.6);
        vec3 d = vec3(0.0, 0.33, 0.67);
        final_color = a + b * cos( 6.28318 * (c * orbit_trap_val + d) );
    } else if (color_scheme_select < 3.5) { // Scheme 3: Cool Tones
        vec3 a = vec3(0.1, 0.2, 0.8);
        vec3 b = vec3(0.1, 0.3, 0.2);
        vec3 c = vec3(0.6, 0.8, 1.0);
        vec3 d = vec3(0.0, 0.33, 0.67);
        final_color = a + b * cos( 6.28318 * (c * orbit_trap_val + d) );
    } else { // Scheme 4: Custom Palette
        vec3 a = vec3(custom_palette_a_r, custom_palette_a_g, custom_palette_a_b);
        vec3 b = vec3(custom_palette_b_r, custom_palette_b_g, custom_palette_b_b);
        vec3 c = vec3(custom_palette_c_r, custom_palette_c_g, custom_palette_c_b);
        vec3 d = vec3(custom_palette_d_r, custom_palette_d_g, custom_palette_d_b);
        final_color = a + b * cos( 6.28318 * (c * orbit_trap_val + d) );
    }
    return final_color;
}


vec3 render(vec3 ray_origin, vec3 ray_dir, vec2 screen_uv) {
    // Reconstruct vec3 colors and positions from float inputs
    vec3 reconstructed_palette_color1 = vec3(palette_color1_r, palette_color1_g, palette_color1_b);
    vec3 reconstructed_palette_color2 = vec3(palette_color2_r, palette_color2_g, palette_color2_b);
    vec3 reconstructed_bg_color = vec3(bg_color_r, bg_color_g, bg_color_b);
    vec3 reconstructed_glow_color = vec3(glow_color_r, glow_color_g, glow_color_b);
    vec3 reconstructed_fog_color = vec3(fog_color_r, fog_color_g, fog_color_b);
    vec3 reconstructed_light1_color = vec3(light1_color_r, light1_color_g, light1_color_b);
    vec3 reconstructed_light2_color = vec3(light2_color_r, light2_color_g, light2_color_b);
    vec3 reconstructed_light1_position = vec3(light1_position_x, light1_position_y, light1_position_z);
    vec3 reconstructed_light2_position = vec3(light2_position_x, light2_position_y, light2_position_z);

    vec3 data = ray_marcher(ray_origin, ray_dir);
    float orbit_trap = data.x;
    float dfo = data.y;
    float total_marches = data.z;
    
    vec3 final_color;

    if (dfo >= max_distance) {
        float vignette = smoothstep(vignette_radius, vignette_radius - vignette_strength, length(screen_uv - vec2(0.5)));
        final_color = reconstructed_bg_color * vignette;
    } else {
        vec3 p = ray_origin + ray_dir * dfo;
        vec3 normal = calculate_normal(p);

        float ao = max(calculate_ambient_occlusion(p, normal), 0.0);
        vec3 light1 = get_light(p, ray_dir, ray_origin, reconstructed_light1_position, reconstructed_light1_color, normal);
        vec3 light2 = get_light(p, ray_dir, ray_origin, reconstructed_light2_position, reconstructed_light2_color, normal);

        // Use the new get_color_palette function
        vec3 palette_col = get_color_palette(orbit_trap, colors_enabled, reconstructed_palette_color1, reconstructed_palette_color2);

        float vignette = smoothstep(vignette_radius, vignette_radius - vignette_strength, length(screen_uv - vec2(0.5)));
        final_color = palette_col * ao * (light1 + light2) * vignette;
    }

    if (glow_enabled > 0.5 && float(total_marches) * raystep_multiplier > glow_threshold) { // Check glow_enabled as float
        float final_glow_intensity = (glow_intensity - 0.2) * smoothstep(glow_threshold, 100.0, float(total_marches) * raystep_multiplier);
        vec3 final_glow_color = reconstructed_glow_color * 3.0;
        final_color += final_glow_color * pow(final_glow_intensity, glow_falloff);
    }

    float fog_distance = dfo < max_distance ? dfo : max_distance;
    float fog_amount = 1.0 - exp(-fog_density * fog_distance);
    final_color = mix(final_color, reconstructed_fog_color, pow(fog_amount, fog_falloff));

    return final_color;
}

mat3 setCamera( in vec3 ro, in vec3 ta, float cr )
{
    vec3 cw = normalize(ta-ro);
    vec3 cp = vec3(sin(cr), cos(cr),0.0);
    vec3 cu = normalize( cross(cw,cp) );
    vec3 cv =          ( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    
    // Set current_fractal_power based on toggle
    if (fractal_power_animated_toggle > 0.5) {
        current_fractal_power = pow(sin(TIME * fractal_power_time_scale) * 0.5 + 0.5, fractal_power_exponent) * fractal_power_amplitude + fractal_power_base;
    } else {
        current_fractal_power = fractal_power_manual;
    }

    // Camera setup using ISF inputs
    vec3 ta = vec3( camera_target_x, camera_target_y, camera_target_z );
    vec3 ro = ta + vec3(camera_origin_x, camera_origin_y, camera_origin_z);
    mat3 ca = setCamera( ro, ta, camera_roll );
    
    vec2 p = (2.0*gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 rd = ca * normalize(vec3(p, ray_direction_z_scale));
    vec3 col = render(ro, rd, uv);

    col = col * 3.0 / (2.5 + col);
    col = pow( col, vec3(0.4545) );
    
    gl_FragColor = vec4(col, 1.0);
}