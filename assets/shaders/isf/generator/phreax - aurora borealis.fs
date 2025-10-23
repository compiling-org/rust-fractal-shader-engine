/*
{
    "CATEGORIES": [
        "Fractal",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Noise",
        "Feedback"
    ],
    "DESCRIPTION": "An ISF conversion of a multi-pass Shadertoy shader, generating dynamic, warped 2D noise patterns with psychedelic spectral coloring and feedback. Features extensive controls for animation, layering, color palettes, offset, zoom, and post-processing effects like glitch, shake, brightness, saturation, contrast, and vignette.",
    "CREDIT": "Original shader by phreax, based on Shane and jackdavenport. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "offset_x", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "DESCRIPTION": "X-offset for the noise pattern." },
        { "NAME": "offset_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "DESCRIPTION": "Y-offset for the noise pattern." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Zoom level for the noise pattern." },
        { "NAME": "time_warp_speed_x", "TYPE": "float", "DEFAULT": 0.15, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "X-component of the time-based warp motion." },
        { "NAME": "time_warp_speed_y", "TYPE": "float", "DEFAULT": -0.15, "MIN": -0.5, "MAX": 0.5, "DESCRIPTION": "Y-component of the time-based warp motion." },
        { "NAME": "warp_intensity", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of the self-feedback in warped noise." },
        { "NAME": "num_layers", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 5.0, "STEP": 1.0, "DESCRIPTION": "Number of noise layers to blend (approx. from 1 to 5). Note: actual loop steps are based on 0.2 increments." },
        { "NAME": "layer_base_rotation", "TYPE": "float", "DEFAULT": 2.236067, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Base rotation for each noise layer (approx. 2*PI/PHI)." },
        { "NAME": "layer_rotation_amplitude", "TYPE": "float", "DEFAULT": 0.0833, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Amplitude of dynamic rotation for layers (PI/12. ~ 0.0833)." },
        { "NAME": "layer_rotation_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Speed of dynamic rotation for layers." },
        { "NAME": "layer_scale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Scaling factor for each noise layer." },
        { "NAME": "layer_offset_strength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Strength of random offset applied to each layer." },
        { "NAME": "layer_fade_in_time", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Start point for layer fade-in based on Z." },
        { "NAME": "layer_fade_out_time", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "End point for layer fade-out based on Z." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 0.11, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Speed of color shifting within the palette." },
        { "NAME": "highlight_intensity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of the highlight/bump effect on colors." },
        { "NAME": "feedback_strength", "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.0, "MAX": 0.99, "DESCRIPTION": "Strength of the feedback from the previous frame (higher = more trails)." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 0.4545, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color (1/2.2 ~ 0.4545)." },
        { "NAME": "output_gain", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Overall brightness multiplier after gamma correction." },
        { "NAME": "vignette_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of the vignette effect." },
        { "NAME": "dithering_enabled", "TYPE": "bool", "DEFAULT": true, "DESCRIPTION": "Enable or disable dithering for smoother gradients." }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ]
}

*/

#define PHI 1.618033988749895 // Using a more precise PHI
#define PI  3.141592653589793 // Using a more precise PI
#define TAU (2. * PI) // Added TAU for convenience

// Virtually the same as your original function, just in more compact (and possibly less reliable) form.
float smoothNoise(vec2 p) {
	
	vec2 i = floor(p); p-=i; p *= p*(3.0 - p - p); 
    
    // Note: The original shadertoy uses a different approach for hashing random values.
    // This compact form might behave slightly differently but should still be noisy.
    return dot(mat2(fract(sin(vec4(0.0, 1.0, 27.0, 28.0) + i.x + i.y * 27.0) * 1e5)) * vec2(1.0 - p.y, p.y), vec2(1.0 - p.x, p.x));

}

// Also the same as the original, but with one less layer.
float fractalNoise(vec2 p) {
    // Original weights
    return smoothNoise(p) * 0.5333 + smoothNoise(p * 2.0) * 0.2667 + smoothNoise(p * 4.0) * 0.1333 + smoothNoise(p * 8.0) * 0.0667;
}

// Standard noise warping. Call the noise function, then feed a variation of the result
// into itself. Rinse and repeat, etc. Completely made up on the spot, but keeping your 
// original concept in mind, which involved combining noise layers travelling in opposing
// directions.
float warpedNoise(vec2 p, float current_time_internal) { // Pass current_time_internal
    
    vec2 m = vec2(current_time_internal * time_warp_speed_x, current_time_internal * time_warp_speed_y); // Use tunable speeds
    
    float x = fractalNoise(p + m);
    float y = fractalNoise(p + m.yx + x);
    float z = fractalNoise(p - m - x + y);
    return fractalNoise(p + vec2(x, y) + vec2(y, z) + vec2(z, x) + length(vec3(x, y, z)) * warp_intensity); // Use tunable warp_intensity
}

// Zucconis Spectra color (https://www.shadertoy.com/view/cdlSzB)
vec3 bump3y (vec3 x, vec3 yoffset) {
    vec3 y = 1.0 - x * x;
    y = clamp((y - yoffset), vec3(0.0), vec3(1.0));
    return y;
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

// Custom psychedelic color palettes
vec3 getColorPalette(float t, float type) {
    if (type < 0.5) return spectral_zucconi6(t); // Palette 0: Original Zucconi spectral
    if (type < 1.5) return vec3(sin(t * 5.0), sin(t * 7.0 + 1.0), sin(t * 9.0 + 2.0)) * 0.5 + 0.5; // Palette 1: Rapid Sine Waves
    if (type < 2.5) return vec3(cos(t * 4.0 + 2.0), cos(t * 2.0 + 1.0), sin(t * 6.0)) * 0.5 + 0.5; // Palette 2: Muted Cosine Blends
    if (type < 3.5) return vec3(sin(t * 2.0), sin(t * 4.0), cos(t * 8.0)) * 0.5 + 0.5; // Palette 3: Fast RGB Pulse
    if (type < 4.5) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0)); // Palette 4: Hard Edge Fractal Colors
    if (type < 5.5) return vec3(sin(t * 1.5), cos(t * 3.0), sin(t * 4.5 + cos(t * 2.0))) * 0.5 + 0.5; // Palette 5: Organic Swirl
    if (type < 6.5) return mix(spectral_zucconi6(t * 0.7), spectral_zucconi6(t * 1.3 + 0.5), 0.5); // Palette 6: Dual Spectral Blend
    // Palette 7: Chakra-like static colors (can be animated if desired)
    vec3 cols[7] = vec3[](vec3(0.608, 0.020, 1.000), vec3(0.169, 0.059, 1.000), vec3(0.000, 0.800, 1.000),
                          vec3(0.035, 1.000, 0.020), vec3(0.984, 1.000, 0.161), vec3(1.000, 0.463, 0.020),
                          vec3(1.000, 0.000, 0.000));
    int index = int(mod(t * 7.0, 7.0)); // Cycle through chakra colors based on t
    return cols[index];
}


vec3 colNoise(vec2 uv, float colShift, float current_time_internal) { // Pass current_time_internal
    float nl = warpedNoise(uv*2.0, current_time_internal); // Pass current_time_internal
    // Take two noise function samples near one another.
    float n = warpedNoise(uv * 6.0, current_time_internal); // Pass current_time_internal
    float n2 = warpedNoise(uv * 6.0 + 0.03 * sin(nl), current_time_internal); // Pass current_time_internal
    
    // Highlighting - Effective, but not a substitute for bump mapping.
    // Use a sample distance variation to produce some cheap and nasty highlighting.
    float bump = max(n2 - n, 0.0) / 0.02 * 0.7071;
    float bump2 = max(n - n2, 0.0) / 0.02 * 0.7071;
    
    // Ramping the bump values up.
    bump = bump * bump * 0.5 + pow(bump, 4.0) * 0.5;
    bump2 = bump2 * bump2 * 0.5 + pow(bump2, 4.0) * 0.5;
    
    // Use selected color palette, highlight intensity and color pulse speed
    vec3 col = getColorPalette(nl + n * 1.5 + colShift, color_palette_type) * (vec3(1.000, 0.800, 0.973) * vec3(1.2 * bump, (bump + bump2) * 0.4, bump2) * highlight_intensity);

    return col;
}

mat2 rot(float a) { return mat2(cos(a), sin(a), -sin(a), cos(a));}


// cheapo noise function
float n21(vec2 p) {
    p = fract(p * vec2(234.42, 725.46));
    p += dot(p, p + 54.98);
    return fract(p.x * p.y);
}

vec2 n22(vec2 p) {
    float n = n21(p);
    return vec2(n, n21(p + n));
}

// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); // Adjust contrast around 0.5 (mid-gray)
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); // Calculate grayscale luminance
    color = mix(gray, color, sat); // Adjust saturation by mixing with grayscale
    return color * br; // Adjust brightness
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }

// Dithering noise
float random_dither(vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
const highp float NOISE_GRANULARITY = 0.5 / 255.0;


void main() {
    float current_time_scaled = TIME * speed; // Use global speed parameter

	if (PASSINDEX == 0)	{
	    vec2 uv = (gl_FragCoord.xy + vec2(offset_x, offset_y) * RENDERSIZE.y - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y; // Apply offset
	    uv /= zoom; // Apply zoom
	    
	    uv.x = abs(uv.x);
	   
	    vec3 col = vec3(0.0);
	    
	    // The original loop iterates for fixed 0.2 increments
	    // num_layers parameter scales the loop iterations indirectly
	    for(float i = 0.0; i < 1.0; i += 0.2) { 
	        if (i / 0.2 >= num_layers) break; // Control number of layers
	        
	        float z = fract(i - 0.1 * current_time_scaled); // Use scaled time
	        float fade = smoothstep(layer_fade_in_time, layer_fade_out_time, z); // Tunable fade times
	        
	        vec2 layer_uv = uv; // Start with current UV for this layer
	        
	        layer_uv *= rot(layer_base_rotation + layer_rotation_amplitude * sin(layer_rotation_speed * current_time_scaled)); // Tunable rotation
	        
	        vec2 noise_offset = n22(vec2(i * 51.0, 4213.0 * i));
	        layer_uv = layer_uv * layer_scale * z + noise_offset * layer_offset_strength; // Tunable scale and offset strength
	      
	        col += colNoise(layer_uv, current_time_scaled * color_pulse_speed + z, current_time_scaled) * fade; // Tunable color pulse speed
	    }
	    
	    // Exact gamma correction, with tunable output gain
		col = vec3(pow(max(col, 0.0), vec3(gamma_correction))) * output_gain;
	    
	    // Feedback from BufferA
	    vec4 tex = IMG_NORM_PIXEL(BufferA, gl_FragCoord.xy / RENDERSIZE.xy); // No mod, ISF automatically wraps UVs for image inputs
	    // Removed the "tex.a != RENDERSIZE.x" check as it's Shadertoy specific.
	    
	    col = mix(col, tex.rgb, feedback_strength); // Tunable feedback strength
	    gl_FragColor = vec4(col, 1.0); // Output to BufferA with alpha 1.0
	        
	}
	else if (PASSINDEX == 1)	{
	 
	    vec2 uv_frag = gl_FragCoord.xy / RENDERSIZE.xy;
	    vec3 col = IMG_NORM_PIXEL(BufferA, uv_frag).rgb; // Read from BufferA
	    
	    // Apply camera shake effect (if enabled)
        if (shake_strength > 0.001) {
            vec2 shake_offset = vec2(
                sin(current_time_scaled * shake_frequency + hash11(1.0)),
                cos(current_time_scaled * shake_frequency * 1.1 + hash11(2.0))
            ) * shake_strength * 0.01; // Scale shake magnitude
            uv_frag += shake_offset;
            col = IMG_NORM_PIXEL(BufferA, uv_frag).rgb; // Resample with offset UV
        }

        // Apply glitch effect (if enabled)
        if (glitch_strength > 0.001) {
            float offset_x_noise = (hash22(uv_frag * 10.0 + current_time_scaled * glitch_frequency) - 0.5) * 2.0;
            float scanline_strength = sin(uv_frag.y * 150.0 + current_time_scaled * 20.0) * 0.5 + 0.5;
            
            uv_frag.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
            col = IMG_NORM_PIXEL(BufferA, uv_frag).rgb; // Resample with glitch UV
        }

        // Brightness, Saturation, Contrast
        col = adjustColor(col, brightness, saturation, contrast);
	    
        // Vignette
        vec2 uv_vignette = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
        col *= mix(1.0, 0.2, vignette_intensity * pow(dot(uv_vignette, uv_vignette), 0.5));
	    
	    // Dithering
        if (dithering_enabled) {
            col += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither(gl_FragCoord.xy));
        }

	    gl_FragColor = vec4(col, 1.0);
	}

}