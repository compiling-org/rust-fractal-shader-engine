/*
{
  "DESCRIPTION": "2D Domain Warped Noise pattern with comprehensive tunable inputs and multiple color palette options.",
  "CATEGORIES": [ "2D", "Noise", "Domain Warp", "FBM", "Kaleidoscope", "Abstract", "Generative", "Color Palette" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "time_global_scale", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Overall time multiplier for animation speed." },
    { "NAME": "uv_scale_main", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Overall scale for UV coordinates after kaleidoscope." },
    { "NAME": "warp_layers", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 4.0, "STEP": 1.0, "DESCRIPTION": "Number of domain warp layers (0 to 4)." },
    { "NAME": "fbm_octaves_default", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Default number of octaves for FBM." },
    { "NAME": "fbm_lacunarity_default", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 4.0, "DESCRIPTION": "Default lacunarity for FBM." },
    { "NAME": "fbm_gain_default", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Default gain for FBM." },
    { "NAME": "fbm_octaves_warp_offset2_components", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Octaves for FBM in warp_offset2 components." },
    { "NAME": "fbm_octaves_warp_offset3_components", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Octaves for FBM in warp_offset3 components." },
    { "NAME": "fbm_octaves_warp_offset4_components", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Octaves for FBM in warp_offset4 components." },
    { "NAME": "fbm_octaves_layer3_final", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Octaves for final FBM in layer 3." },
    { "NAME": "fbm_octaves_layer4_final", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Octaves for final FBM in layer 4." },
    { "NAME": "fbm_lacunarity_alt", "TYPE": "float", "DEFAULT": 2.2, "MIN": 1.0, "MAX": 4.0, "DESCRIPTION": "Alternate lacunarity for specific FBM calls." },
    { "NAME": "fbm_gain_alt", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Alternate gain for specific FBM calls." },
    { "NAME": "domain_warp_time_offset", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Base time offset for domain warp layers." },
    { "NAME": "domain_warp_time_mult_layer2_pos", "TYPE": "float", "DEFAULT": 0.15, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "Positive time multiplier for layer 2 warp offset." },
    { "NAME": "domain_warp_time_mult_layer2_neg", "TYPE": "float", "DEFAULT": 0.126, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "Negative time multiplier for layer 2 warp offset." },
    { "NAME": "domain_warp_time_mult_layer3", "TYPE": "float", "DEFAULT": 0.1, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "Time multiplier for layer 3 warp offset." },
    { "NAME": "domain_warp_time_mult_layer4", "TYPE": "float", "DEFAULT": 0.05, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "Time multiplier for layer 4 warp offset." },
    { "NAME": "warp_offset1_multiplier", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the first warp offset." },
    { "NAME": "warp_offset2_multiplier", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the second warp offset." },
    { "NAME": "warp_offset3_multiplier", "TYPE": "float", "DEFAULT": 3.5, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the third warp offset." },
    { "NAME": "warp_offset4_multiplier", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the fourth warp offset." },
    { "NAME": "kalaido_planes_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Fixed planes (10), 1=Use 'kalaido_planes_control'." },
    { "NAME": "kalaido_planes_control", "TYPE": "float", "DEFAULT": 0.333, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls number of kaleidoscope planes (multiplied by 30) when toggle is on." },
    { "NAME": "kalaido_planes_default", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "STEP": 1.0, "DESCRIPTION": "Default number of kaleidoscope planes when toggle is off." },
    { "NAME": "palette_theme_mouse_toggle", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Toggle: 0=Fixed palette theme (0.0), 1=Use 'palette_theme_control'." },
    { "NAME": "palette_theme_control", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Controls palette theme when toggle is on." },
    { "NAME": "palette_t_mult", "TYPE": "float", "DEFAULT": 2.7, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Multiplier for 't' in palette calculation." },
    { "NAME": "palette_t_add", "TYPE": "float", "DEFAULT": 1.3, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Additive for 't' in palette calculation." },
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

#define PI 3.14

// ISF Built-in Uniforms (implicitly available, no explicit declaration needed)
// RENDERSIZE and TIME are directly available by name.

// Custom Input Uniforms (implicitly available by name from JSON INPUTS)
// All variables defined in the JSON "INPUTS" block are accessible directly by their "NAME".

// Function Prototypes
vec2 hash22(vec2 p);
float hash12(vec2 p);
vec3 pal(in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d);
vec3 get_color_palette(float t_val, float theme_val);
float perlin_noise(vec2 p);
float fbm(vec2 p, float octaves, float lacunarity, float gain);
float domain_warp(vec2 p, float num_layers);
vec2 kalaido(vec2 p);


vec2 hash22(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453123);
}

float hash12(vec2 p) {
    float h = dot(p, vec2(127.1, 311.7));
    return fract(sin(h) * 43758.5453123);
}

// Original palette function from Inigo Quilez
vec3 pal(in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d) {
    return a + b*cos(6.28318*(c*t+d));
}

// Extended palette selection function
vec3 get_color_palette(float t_val, float theme_val) {
    vec3 final_color;

    if (color_scheme_select < 0.5) { // Scheme 0: Original Palette (with theme control)
        vec3 c_theme = mix(vec3(0.0,0.10,0.20), vec3(0.3,0.20,0.20), theme_val);
        final_color = pal(t_val, vec3(0.5,0.5,0.5), vec3(0.5,0.5,0.5), vec3(1.0,1.0,1.0), c_theme);
    } else if (color_scheme_select < 1.5) { // Scheme 1: Grayscale
        final_color = vec3(t_val);
    } else if (color_scheme_select < 2.5) { // Scheme 2: Warm Tones
        final_color = pal(t_val, vec3(0.8, 0.2, 0.1), vec3(0.2, 0.3, 0.1), vec3(1.0, 0.8, 0.6), vec3(0.0, 0.33, 0.67));
    } else if (color_scheme_select < 3.5) { // Scheme 3: Cool Tones
        final_color = pal(t_val, vec3(0.1, 0.2, 0.8), vec3(0.1, 0.3, 0.2), vec3(0.6, 0.8, 1.0), vec3(0.0, 0.33, 0.67));
    } else { // Scheme 4: Custom Palette
        vec3 a = vec3(custom_palette_a_r, custom_palette_a_g, custom_palette_a_b);
        vec3 b = vec3(custom_palette_b_r, custom_palette_b_g, custom_palette_b_b);
        vec3 c = vec3(custom_palette_c_r, custom_palette_c_g, custom_palette_c_b);
        vec3 d = vec3(custom_palette_d_r, custom_palette_d_g, custom_palette_d_b);
        final_color = pal(t_val, a, b, c, d);
    }
    return final_color;
}

float perlin_noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    
    vec2 u = f * f * (3.0 - 2.0 * f);
    
    vec2 ga = hash22(i) * 2.0 - 1.0;
    vec2 gb = hash22(i + vec2(1.0, 0.0)) * 2.0 - 1.0;
    vec2 gc = hash22(i + vec2(0.0, 1.0)) * 2.0 - 1.0;
    vec2 gd = hash22(i + vec2(1.0, 1.0)) * 2.0 - 1.0;
    
    float va = dot(ga, f);
    float vb = dot(gb, f - vec2(1.0, 0.0));
    float vc = dot(gc, f - vec2(0.0, 1.0));
    float vd = dot(gd, f - vec2(1.0, 1.0));
    
    return mix(mix(va, vb, u.x), mix(vc, vd, u.x), u.y) * 0.5 + 0.5;
}

float fbm(vec2 p, float octaves, float lacunarity, float gain) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    float normalization = 0.0;
    
    for (int i = 0; i < int(octaves); i++) { // Cast octaves to int
        value += amplitude * perlin_noise(p * frequency);
        normalization += amplitude;
        amplitude *= gain;
        frequency *= lacunarity;
    }
    
    return value / normalization;
}

float domain_warp(vec2 p, float num_layers) { // num_layers is float input
    float current_time = (TIME * time_global_scale * 0.3) + domain_warp_time_offset; // Use time_global_scale and domain_warp_time_offset

    if (num_layers < 0.5) { // Layer 0
        return fbm(p, fbm_octaves_default, fbm_lacunarity_default, fbm_gain_default);
    }
    
    vec2 warp_offset1 = vec2(
        fbm(p + vec2(0.0, 0.0), fbm_octaves_default, fbm_lacunarity_default, fbm_gain_default),
        fbm(p + vec2(5.2, 1.3), fbm_octaves_default, fbm_lacunarity_default, fbm_gain_default)
    );
    
    if (num_layers < 1.5) { // Layer 1
        return fbm(p + warp_offset1_multiplier * warp_offset1, fbm_octaves_default, fbm_lacunarity_alt, fbm_gain_alt); // Use inputs
    }
    
    vec2 warp_offset2 = vec2(
        fbm(p + warp_offset1_multiplier * warp_offset1 + vec2(1.7, 9.2) + domain_warp_time_mult_layer2_pos * current_time, fbm_octaves_warp_offset2_components, fbm_lacunarity_default, fbm_gain_default), // Use inputs
        fbm(p + warp_offset1_multiplier * warp_offset1 + vec2(8.3, 2.8) - domain_warp_time_mult_layer2_neg * current_time, fbm_octaves_warp_offset2_components, fbm_lacunarity_default, fbm_gain_default)  // Use inputs
    );
    
    if (num_layers < 2.5) { // Layer 2
        return fbm(p + warp_offset2_multiplier * warp_offset2, fbm_octaves_default, fbm_lacunarity_alt, fbm_gain_alt); // Use inputs
    }
    
    vec2 warp_offset3 = vec2(
        fbm(p + warp_offset2_multiplier * warp_offset2 + vec2(8.1, 4.3) + domain_warp_time_mult_layer3 * current_time, fbm_octaves_warp_offset3_components, fbm_lacunarity_default, fbm_gain_default), // Use inputs
        fbm(p + warp_offset2_multiplier * warp_offset2 + vec2(2.8, 5.6) - domain_warp_time_mult_layer3 * current_time, fbm_octaves_warp_offset3_components, fbm_lacunarity_default, fbm_gain_default)  // Use inputs
    );
    
    if (num_layers < 3.5) { // Layer 3
        return fbm(p + warp_offset3_multiplier * warp_offset3, fbm_octaves_layer3_final, fbm_lacunarity_alt, fbm_gain_alt); // Use inputs
    }
    
    vec2 warp_offset4 = vec2(
        fbm(p + warp_offset3_multiplier * warp_offset3 + vec2(13.5, 7.7) + domain_warp_time_mult_layer4 * current_time, fbm_octaves_warp_offset4_components, fbm_lacunarity_default, fbm_gain_default), // Use inputs
        fbm(p + warp_offset3_multiplier * warp_offset3 + vec2(3.4, 1.2) - domain_warp_time_mult_layer4 * current_time, fbm_octaves_warp_offset4_components, fbm_lacunarity_default, fbm_gain_default)  // Use inputs
    );
    
    return fbm(p + warp_offset4_multiplier * warp_offset4, fbm_octaves_layer4_final, fbm_lacunarity_alt, fbm_gain_alt); // Use inputs
}

vec2 kalaido(vec2 p) {
    float num_planes;
    if (kalaido_planes_mouse_toggle > 0.5) { // Use toggle input
        num_planes = floor(kalaido_planes_control * 30.0); // Use control input
    } else {
        num_planes = kalaido_planes_default; // Use default input
    }
    
    float angle_step = PI / num_planes;
    
    for (int i = 0; i < int(num_planes); i++) { // Cast num_planes to int
        float angle = float(i) * angle_step;
        vec2 n = vec2(cos(angle), sin(angle));
        float d = dot(p, n);
        
        if (d < 0.0) {
            p = p - 2.0 * d * n;
        }
    }
    
    return p;
}

void main() {
    // Corrected UV calculation to match original shader's centering and scaling
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    uv *= uv_scale_main; // Use uv_scale_main input
    
    uv = kalaido(uv);
    
    float warp_value = domain_warp(uv, warp_layers); // Use warp_layers input
    float warp_value2 = domain_warp(uv * 1000.0, warp_layers) * 0.5 + 0.5; // Use warp_layers input
    
    float t1 = TIME * time_global_scale + 78.0; // Use TIME and time_global_scale
    float t_for_palette = warp_value * ((sin(t1 * 0.2) * 0.5 + 0.5) * palette_t_mult + palette_t_add); // Use palette_t_mult and palette_t_add
    
    float current_palette_theme;
    if (palette_theme_mouse_toggle > 0.5) { // Use toggle input
        current_palette_theme = palette_theme_control; // Use control input
    } else {
        current_palette_theme = 0.0; // Default theme
    }

    vec3 color = get_color_palette(t_for_palette, current_palette_theme);
    
    gl_FragColor = vec4(color, 1.0);
}