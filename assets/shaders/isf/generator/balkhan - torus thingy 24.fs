/*
{
    "CATEGORIES": [
        "Fractal",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Raymarching",
        "SDF"
    ],
    "DESCRIPTION": "An ISF conversion of a compact Shadertoy shader creating an abstract, twisting fractal tunnel. Features extensive controls for animation, camera, fractal geometry, psychedelic color palettes, and post-processing, prioritizing rich, brilliant, white-free colors.",
    "CREDIT": "Original shader by bal-khan (Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License). Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "camera_orbit_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Speed of the camera's rotational movement." },
        { "NAME": "camera_offset_z", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Manual Z-axis offset for the camera's starting position." },
        { "NAME": "inner_circle_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the fractal's inner circle pulsing animation." },
        { "NAME": "knot_twist_speed_1", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the first fractal knot twisting effect." },
        { "NAME": "knot_twist_speed_2", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the second fractal knot twisting effect." },
        { "NAME": "distance_field_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the pulsing distortion applied to the distance field." },
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of the color pulsing effect applied to the palette." },
        { "NAME": "max_raymarch_iterations", "TYPE": "float", "DEFAULT": 300.0, "MIN": 50.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum steps for the raymarcher. Higher values increase detail but reduce performance." },
        { "NAME": "epsilon_factor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Multiplier for the raymarch hit threshold (epsilon). Lower values require closer hits." },
        { "NAME": "far_clip_distance", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 100.0, "DESCRIPTION": "Maximum distance rays will travel. Affects view depth and performance." },
        { "NAME": "ray_step_multiplier", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Multiplier for how much distance is added per raymarch step. Affects speed and detail." },
        { "NAME": "hit_color_intensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0001, "MAX": 10.0, "DESCRIPTION": "Overall intensity of colors at hit points. Increase for brighter patterns." },
        { "NAME": "hit_color_base_divisor", "TYPE": "float", "DEFAULT": 0.0071001, "MIN": 0.00001, "MAX": 0.1, "DESCRIPTION": "Base divisor for hit color. Affects the core brightness and falloff." },
        { "NAME": "hit_color_dist_sq_divisor", "TYPE": "float", "DEFAULT": 0.00051, "MIN": 0.000001, "MAX": 0.005, "DESCRIPTION": "Divisor for squared hit distance. Affects how quickly colors fade from the hit point." },
        { "NAME": "hit_brightness_limit", "TYPE": "float", "DEFAULT": 100.0, "MIN": 1.0, "MAX": 1000.0, "DESCRIPTION": "Caps maximum brightness of individual raymarch steps. Crucial for preventing pure white in the pattern. Lower for more distinct colors." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "palette_id_offset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0, "STEP": 1.0, "DESCRIPTION": "Offset to select a palette variant based on fractal ID." },
        { "NAME": "palette_time_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for time input to color palette function. Controls animation speed of colors." },
        { "NAME": "lighting_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall intensity of the lighting effect applied after color accumulation." },
        { "NAME": "tanh_divisor", "TYPE": "float", "DEFAULT": 100.0, "MIN": 100.0, "MAX": 100000000.0, "DESCRIPTION": "Controls the overall 'brilliance' and compression of the image. **Lower values (e.g., to MIN) increase vibrancy significantly and prevent white.**" },
        { "NAME": "base_brightness", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.1, "DESCRIPTION": "Base multiplier for the final accumulated color. Controls overall initial brightness." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness after tone mapping." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall brightness multiplier after gamma." },
       
        { "NAME": "structure_scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Scales the overall size of the 3D fractal structure. Higher values make it larger." },
        { "NAME": "morph_param_1", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls a primary morphing parameter of the fractal's shape." },
        { "NAME": "morph_param_2", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls a secondary morphing parameter of the fractal's shape." },
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

// Original shader constants
#define E           0.0001 // Original Epsilon

// Custom mod function to handle negative numbers consistently
float mod_custom(float x, float y) {
    return x - y * floor(x / y);
}

// from "Palettes" by iq. https://shadertoy.com/view/ll2GD3
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
    id = id % 8; // Ensure it cycles through 8 palettes (0-7)

    vec3 col;
    if (id == 0) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, -0.33, 0.33));
    else if (id == 1) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.0, 0.10, 0.20));
    else if (id == 2) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), vec3(0.3, 0.20, 0.20));
    else if (id == 3) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 0.5), vec3(0.8, 0.90, 0.30));
    else if (id == 4) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
    else if (id == 5) col = pal(t, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
    else if (id == 6) col = pal(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0, 1.0, 1.0), vec3(0.0, 0.25, 0.25));
    else if (id == 7) col = spectral_zucconi6(t); // Use the Zucconi spectral palette
    
    return col;
}

// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); 
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); 
    color = mix(gray, color, sat); 
    return color * br; 
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }

// Dithering noise
float random_dither(vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
const highp float NOISE_GRANULARITY = 0.5 / 255.0;

float mylength(vec2 p) {vec2 u = abs(p); return max(u.x, u.y);}

// Utilities
void rotate(inout vec2 v, float angle)
{
	v = vec2(cos(angle)*v.x+sin(angle)*v.y,-sin(angle)*v.x+cos(angle)*v.y);
}

// Struct to return both distance and color ID from the scene function
struct SceneResult {
    float distance;
    float color_id;
};

// Original scene function logic, modified to return a struct and accept tunable parameters
// NOW INCLUDES structure_scale, morph_param_1, morph_param_2
SceneResult scene_and_id(vec3 p_in, float time_factor, float camera_orbit_speed, float inner_circle_pulse_speed, float knot_twist_speed_1, float knot_twist_speed_2, float distance_field_pulse_speed, float structure_scale, float morph_param_1, float morph_param_2)
{  
    float v, x, id, w;
    // Apply structure scale to the incoming position (scales the coordinate space)
    vec3 p = p_in / structure_scale; // Work with a local copy, SCALED

    rotate(p.xz, -sin(time_factor * camera_orbit_speed * .5)/8. + time_factor * camera_orbit_speed / 16. ); // simple camera rotation

    vec2 q = vec2(length(p.xy)-10., p.z); // classic torus
    v = atan(p.x,p.y) ;
    w = atan(q.x, q.y);

    rotate(q, v*-4.);
    q = abs(q); // zinging the zags

    q -= 2.*abs(sin(time_factor * inner_circle_pulse_speed * .01) ); // inner circle enlarger

    rotate(q, v*2.+ w + time_factor * knot_twist_speed_1 * .5); // classic torus knot code
    q = abs(q)-2.5;
    rotate(q, v*-20. + time_factor * knot_twist_speed_2 * -2.5);
    w = -atan(q.x, q.y);
    q = abs(q)-.5;

    id = (w*20.)/10.; // coloring id

    x = sin(v*15.+id*1.+time_factor * distance_field_pulse_speed * 6.0)-1.;

    // Apply morph parameters to the distance field calculation
    // The .3 and .2 are now replaced by morph_param_1 and morph_param_2
    float mind = mylength(q) - morph_param_1 - morph_param_2 * x + sin( +id*2.5 + time_factor / 16.)/16.; // distance field

    SceneResult res;
    res.distance = mind;
    res.color_id = id;
    return res;
}

// March function returns vec4(total_distance, accumulated_color.rgb)
// NOW INCLUDES structure_scale, morph_param_1, morph_param_2
vec4 march(vec3 pos, vec3 dir, float time_factor, float color_pulse_speed, float hit_brightness_limit, float color_palette_type, float palette_id_offset, float palette_time_multiplier, float max_raymarch_iterations_param, float epsilon_factor, float far_clip_distance_param, float ray_step_multiplier, float hit_color_intensity, float hit_color_base_divisor, float hit_color_dist_sq_divisor, float camera_orbit_speed, float inner_circle_pulse_speed, float knot_twist_speed_1, float knot_twist_speed_2, float distance_field_pulse_speed, float structure_scale, float morph_param_1, float morph_param_2)
{
    float total_dist = 0.0;
    vec3 accumulated_col = vec3(0.0);

    float current_epsilon_threshold = E * 5000.0 * epsilon_factor;

    for (float i = -1.; i < max_raymarch_iterations_param; ++i)
    {
        vec3 p = pos + dir * total_dist;
        // Pass all new parameters to scene_and_id
        SceneResult sr = scene_and_id(p, time_factor, camera_orbit_speed, inner_circle_pulse_speed, knot_twist_speed_1, knot_twist_speed_2, distance_field_pulse_speed, structure_scale, morph_param_1, morph_param_2);

        float mind = sr.distance;
        float current_fractal_id = sr.color_id;

        if (mind <= current_epsilon_threshold) // near hit - accumulate color
        {
            vec3 current_palette_color = getPal(int(color_palette_type + palette_id_offset), current_fractal_id * 0.5 + time_factor * palette_time_multiplier + color_pulse_speed);
            
            vec3 color_contribution = (vec3(1.0) - current_palette_color) * hit_color_intensity / max(0.00001, (hit_color_base_divisor + mind * mind * hit_color_dist_sq_divisor));

            color_contribution = min(color_contribution, vec3(hit_brightness_limit));
            accumulated_col += color_contribution;
        }

        // Ensure the ray always moves forward by using abs(mind) for step size
        total_dist += abs(mind) * ray_step_multiplier; 
        
        // Break conditions:
        // 1. If ray is very close to a surface (mind < E) - this indicates a definitive hit.
        // 2. If ray has traveled beyond the far clip distance.
        if (mind < E || total_dist > far_clip_distance_param) 
        {
            break;
        }
    }
    return vec4(total_dist, accumulated_col);
}

// MAIN FUNCTION (SINGLE PASS)
void main()
{
    float current_time = TIME * speed; 

    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    uv /= zoom; // Apply zoom

    vec3 dir = vec3(uv, -1.);
    vec3 pos = vec3(.0, .0, 25.0 + camera_offset_z); // Apply camera_offset_z

    // Call the march function with all its parameters, including the new ones
    vec4 march_result = march(pos, dir, current_time, color_pulse_speed, hit_brightness_limit, color_palette_type, palette_id_offset, palette_time_multiplier, max_raymarch_iterations, epsilon_factor, far_clip_distance, ray_step_multiplier, hit_color_intensity, hit_color_base_divisor, hit_color_dist_sq_divisor, camera_orbit_speed, inner_circle_pulse_speed, knot_twist_speed_1, knot_twist_speed_2, distance_field_pulse_speed, structure_scale, morph_param_1, morph_param_2);

    // float final_distance = march_result.x; // Not directly used for color output in original shader
    vec3 final_accumulated_color = march_result.yzw; // Get the accumulated color

    // Apply base brightness from original shader (.01 multiplier)
    vec3 final_color_rgb = final_accumulated_color * base_brightness;

    // Apply lighting intensity
    final_color_rgb *= lighting_intensity;

    // Apply tanh tone mapping with tunable tanh_divisor
    final_color_rgb = tanh(final_color_rgb * final_color_rgb / tanh_divisor); 

    // --- Post-Processing Effects ---
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv_post = fragCoord / RENDERSIZE.xy;

    // Glitch and Shake effects are commented out.
    // Explanation: These effects typically require accessing the previous frame's rendered image.
    // In ISF, this means you'd need a multi-pass setup where one pass renders the fractal to a buffer (e.g., "BufferA"),
    // and a second pass reads from that buffer (often using a RENDERPASS_PREV_FRAME input) and applies the distortion.
    // Your current setup is a single pass ("TARGET": "Destination"), which doesn't provide easy access to a previous frame buffer.
    // I can implement this with a multi-pass setup in a *separate* step if you'd like, but it makes the shader more complex.
    /*
    if (glitch_strength > 0.001) { 
        float offset_x_noise = (hash22(uv_post * 10.0 + current_time * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(uv_post.y * 150.0 + current_time * 20.0) * 0.5 + 0.5;
        uv_post.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
        // To make this work, you'd typically sample a previous frame texture here.
        // final_color_rgb = texture(RENDERPASS_PREV_FRAME, uv_post).rgb; // Example if RENDERPASS_PREV_FRAME was available
    }
    */
    /*
    if (shake_strength > 0.001) { 
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency + hash11(1.0)),
            cos(current_time * shake_frequency * 1.1 + hash11(2.0))
        ) * shake_strength * 0.01; 
        uv_post += shake_offset;
        // To make this work, you'd typically sample a previous frame texture here.
        // final_color_rgb = texture(RENDERPASS_PREV_FRAME, uv_post).rgb; // Example if RENDERPASS_PREV_FRAME was available
    }
    */

    // Brightness, Saturation, Contrast
    final_color_rgb = adjustColor(final_color_rgb, brightness, saturation, contrast);

    // Apply gamma correction and output gain
    final_color_rgb = pow(final_color_rgb, vec3(gamma_correction)) * output_gain; 
    
    // Dithering
    if (dithering_enabled > 0.5) { 
        final_color_rgb += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
    }

    gl_FragColor = vec4(final_color_rgb, 1.0);
}