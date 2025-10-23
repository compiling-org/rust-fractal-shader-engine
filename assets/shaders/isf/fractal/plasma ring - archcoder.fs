/*
{
  "DESCRIPTION": "2D Kaleidoscopic Noise pattern with tunable inputs, multiple color palette options, and new zoom, shake, and glitch controls.",
  "CATEGORIES": [ "2D", "Noise", "Kaleidoscope", "Abstract", "Generative", "Distortion" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "time_scale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for animation speed." },
    { "NAME": "uv_offset_x", "TYPE": "float", "DEFAULT": 0.4, "MIN": -2.0, "MAX": 2.0, "DESCRIPTION": "X offset for UV coordinates." },
    { "NAME": "zoom_level", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Overall zoom level for the pattern." },
    { "NAME": "num_octaves", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of octaves for Fractal Brownian Motion (FBM)." },
    { "NAME": "kaleido_planes_control", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls the number of kaleidoscope planes. Multiplied by 40." },
    { "NAME": "kaleido_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Fixed 20 planes, 1=Use 'kaleido_planes_control'." },
    { "NAME": "kaleido_boxness", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Boxiness factor for kaleidoscope effect." },
    { "NAME": "kaleido_expand", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Expansion factor for kaleidoscope effect." },
    { "NAME": "color_intensity", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Overall intensity multiplier for colors." },
    { "NAME": "color_blend_freq", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Frequency for color blending animation." },
    { "NAME": "color_blend_phase", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Phase offset for color blending animation." },
    { "NAME": "color_scheme_select", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "STEP": 1.0, "DESCRIPTION": "Selects a color scheme: 0=Red/Yellow, 1=Blue Mode, 2=Green/Purple, 3=Custom." },
    { "NAME": "custom_color_r_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Red multiplier for custom color scheme." },
    { "NAME": "custom_color_g_mult", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Green multiplier for custom color scheme." },
    { "NAME": "custom_color_b_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Blue multiplier for custom color scheme." },
    { "NAME": "shake_intensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Intensity of the screen shake effect." },
    { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "DESCRIPTION": "Frequency of the screen shake effect." },
    { "NAME": "glitch_enabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable the glitch effect (0=Off, 1=On)." },
    { "NAME": "glitch_intensity", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Overall intensity of the glitch effect." },
    { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "DESCRIPTION": "Frequency of glitch activation/change." },
    { "NAME": "glitch_uv_strength", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Strength of UV displacement within the glitch effect." }
  ]
}
*/

// Define constants
#define PI 3.14 

// ISF Built-in Uniforms (implicitly available, no explicit declaration needed)
// RENDERSIZE and TIME are directly available by name.

// Custom Input Uniforms (implicitly available by name from JSON INPUTS)
// All variables defined in the JSON "INPUTS" block are accessible directly by their "NAME".

// Noise and FBM functions from original source
// Renamed rand to custom_rand to avoid potential conflicts
float custom_rand(in vec2 _st) {
    // FIX: Corrected vec1 to vec2 here
    return fract(sin(dot(_st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(vec2 p){
	vec2 ip = floor(p);
	vec2 u = fract(p);
	u = u*u*(3.0-2.0*u);
	
	float res = mix(
		mix(custom_rand(ip),custom_rand(ip+vec2(1.0,0.0)),u.x),
		mix(custom_rand(ip+vec2(0.0,1.0)),custom_rand(ip+vec2(1.0,1.0)),u.x),u.y);
	return res*res;
}

float fbm(vec2 x) {
	float v = 0.0;
	float a = 0.3;
	vec2 shift = vec2(1000);
	for (int i = 0; i < int(num_octaves); ++i) { // Use num_octaves input
		v += a * noise(x);
		x = x * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}

// Kaleidoscope function - adapted for ISF inputs
vec2 kalaido(vec2 p) {
    float num_planes;
    if (kaleido_mouse_toggle > 0.5) { // Use kaleido_mouse_toggle input
        num_planes = kaleido_planes_control * 40.0; // Use kaleido_planes_control input
    } else {
        num_planes = 20.0;
    }
    
    float angle_step = PI / num_planes; // Use PI constant
    
    for (int i = 0; i < int(num_planes); i++) { // Cast num_planes to int
        float angle = float(i) * angle_step;
        vec2 n = vec2(cos(angle), sin(angle));
        float d = dot(p, n);
        
        if (d < 0.0) {
            // Kaleidoscope effect with tunable boxness and expand
            p = clamp(p, -1.0 - kaleido_expand, 1.0 - kaleido_expand) - (2.0 + kaleido_boxness) * vec2(1.0 - (kaleido_boxness * 0.5), 1.0) * d * n;
        }
    }
    
    return p;
}

void main() { // ISF entry point
    float t = TIME * time_scale; // Use TIME and time_scale inputs
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.y - 0.5; // Use RENDERSIZE

    // Apply Shake Effect
    vec2 shake_offset = vec2(sin(t * shake_frequency) * shake_intensity, cos(t * shake_frequency * 1.2) * shake_intensity);
    uv += shake_offset;

    // Apply Glitch Effect (UV Displacement)
    float glitch_active = step(0.5, fract(t * glitch_frequency)) * glitch_enabled; // Intermittent activation, controlled by glitch_enabled
    // FIX: Corrected assignment from float to vec2 for glitch_uv_offset
    vec2 glitch_uv_offset = (vec2(custom_rand(uv * 100.0 + t), custom_rand(uv * 100.0 + t + 7.0)) * 2.0 - 1.0) * glitch_uv_strength * glitch_active;
    uv += glitch_uv_offset;

    uv.x -= uv_offset_x; // Use uv_offset_x input
    uv = kalaido(uv);
    uv *= zoom_level; // Use zoom_level input (renamed from uv_scale)
    
    float r_val = distance(uv, vec2(sin(t), 0.0)) * fbm(uv);
    float g_val = distance(uv, vec2(0.0, r_val));
    float b_val = distance(uv, vec2(r_val, sin(t)));
    
    float value = abs(sin(r_val + t) + sin(g_val + t) + sin(b_val + t) + sin(uv.x + t) + cos(uv.y + t));
    value *= color_intensity; // Use color_intensity input
    
    r_val /= value;
    g_val /= value;
    b_val /= value;

    vec4 final_color;
    float blend_factor = sin(t * color_blend_freq) * 0.5 + color_blend_phase; // Use color_blend_freq and color_blend_phase

    if (color_scheme_select < 0.5) { // Scheme 0: Original Red/Yellow
        final_color = vec4(b_val, mix(r_val, g_val * 0.8, blend_factor), r_val, 1.0);
    } else if (color_scheme_select < 1.5) { // Scheme 1: Blue Mode
        final_color = vec4(r_val, mix(r_val, g_val * 0.8, blend_factor), b_val, 1.0);
    } else if (color_scheme_select < 2.5) { // Scheme 2: Green/Purple
        final_color = vec4(mix(b_val, r_val * 0.5, blend_factor), g_val, mix(r_val, b_val * 1.2, blend_factor), 1.0);
    } else { // Scheme 3: Custom
        final_color = vec4(b_val * custom_color_r_mult, mix(r_val * custom_color_g_mult, g_val * 0.8, blend_factor), r_val * custom_color_b_mult, 1.0);
    }

    // Apply Glitch Effect (Color Noise/Static)
    float color_glitch_noise = custom_rand(gl_FragCoord.xy * 0.1 + t * 5.0) * glitch_active * glitch_intensity;
    final_color.rgb += color_glitch_noise;
    
    gl_FragColor = final_color;
}