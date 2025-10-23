/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "3D",
        "Animated",
        "Abstract"
    ],
    "DESCRIPTION": "A psychedelic 3D fractal with a unique cosine-based folding and dynamic color palettes. Features tunable animation, camera controls, morphing geometry, and post-processing effects. Correctly uses standard ISF uniforms for maximum compatibility and includes all requested controls.",
    "CREDIT": "Original shader based on the idea of Jarble (https://www.shadertoy.com/view/3ttyzB), converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "xy_control_x", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "X-component for camera rotation/offset (normalized 0-1, maps to 0-2PI)." },
        { "NAME": "xy_control_y", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Y-component for camera rotation/offset (normalized 0-1, maps to 0-PI)." },
        { "NAME": "morph_factor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Influences various aspects of fractal morphing (e.g., offsets, folding)." },
        { "NAME": "raymarch_iterations", "TYPE": "float", "DEFAULT": 90.0, "MIN": 10.0, "MAX": 200.0, "STEP": 1.0, "DESCRIPTION": "Number of raymarching steps. Higher values increase detail but reduce performance." },
        { "NAME": "fractal_inner_iterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 15.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the inner fractal folding loop." },
        { "NAME": "base_rotation_speed_1", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Speed for the first base rotation of the fractal." },
        { "NAME": "base_rotation_speed_2", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Speed for the second base rotation of the fractal." },
        { "NAME": "cosine_fold_mult_1", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the first cosine fold term." },
        { "NAME": "cosine_fold_mult_2", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Multiplier for the second cosine fold term." },
        { "NAME": "cosine_fold_mult_3", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Multiplier for the nested cosine fold term." },
        { "NAME": "q_sin_time_mult", "TYPE": "float", "DEFAULT": 0.2, "MIN": -1.0, "MAX": 1.0, "DESCRIPTION": "Multiplier for sin(TIME) added to Q vector (morphs fractal)." },
        { "NAME": "clamp_limit", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Clamping limit for the fractal folding operation." },
        { "NAME": "inner_scale_factor", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Scaling factor for the inner fractal iterations." },
        { "NAME": "inner_scale_clamp_val", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Clamp value used in inner scale calculation (prevents over-scaling)." },
        { "NAME": "dist_offset_start", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.01, "MAX": 1.0, "DESCRIPTION": "Initial distance offset for the raymarcher (affects starting 'view' depth)." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 7 psychedelic color palettes." },
        { "NAME": "color_pulse_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of color pulsing." },
        { "NAME": "color_pulse_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of color pulsing." },
        { "NAME": "log_scale_color_factor", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Factor for log(s) in hue calculation (influences color 'density')." },
        { "NAME": "base_color_mix", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor between white and fractal color (0=white, 1=fractal color)." },
        { "NAME": "base_color_multiplier", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.001, "MAX": 0.1, "DESCRIPTION": "Base intensity multiplier for accumulated color." },
        { "NAME": "exponent_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Intensity of the exponent term in color calculation (affects fading with depth)." },
        { "NAME": "final_color_power", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "DESCRIPTION": "Power applied to final color for contrast (higher values increase contrast)." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect (0=none, 1=max)." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect (0=none, 1=max)." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." }
    ]
}
*/

#define R(p,a,r) mix(a*dot(p,a),p,cos(r))+sin(r)*cross(p,a)
#define H(h)(cos((h)*6.3+vec3(0,23,21))*.5+.5)

// Compact color palette function
vec3 getColorPalette(float t, float type) {
    if (type < 0.5) return H(t); // Palette 0: Default Psychedelic Flow
    if (type < 1.5) return vec3(sin(t * 5.0), sin(t * 7.0 + 1.0), sin(t * 9.0 + 2.0)) * 0.5 + 0.5; // Palette 1: Rapid Sine Waves
    if (type < 2.5) return vec3(cos(t * 4.0 + 2.0), cos(t * 2.0 + 1.0), sin(t * 6.0)) * 0.5 + 0.5; // Palette 2: Muted Cosine Blends
    if (type < 3.5) return vec3(sin(t * 2.0), sin(t * 4.0), cos(t * 8.0)) * 0.5 + 0.5; // Palette 3: Fast RGB Pulse
    if (type < 4.5) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0)); // Palette 4: Hard Edge Fractal Colors
    if (type < 5.5) return vec3(sin(t * 1.5), cos(t * 3.0), sin(t * 4.5 + cos(t * 2.0))) * 0.5 + 0.5; // Palette 5: Organic Swirl
    return mix(H(t * 0.7), H(t * 1.3 + 0.5), 0.5); // Palette 6: Dual Hue Blend
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

void main() {
    vec4 fragColor;
    vec2 fragCoord = gl_FragCoord.xy;

    // Use the standard RENDERSIZE uniform directly (provided by ISF host as vec2)
    // and TIME uniform (provided by ISF host as float).
    // These do NOT need to be listed in INPUTS in the JSON metadata.
    vec3 iResolution_vec3 = vec3(RENDERSIZE.xy, 1.0); // Mimics Shadertoy's iResolution.xyz

    float current_time = TIME * speed; // Global time with speed control

    // Directly match Shadertoy's (C-.5*r.xy)/r.y normalization for base UV
    // This is the core of the ray direction calculation for viewing geometry
    vec2 uv = (fragCoord - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv /= zoom; // Apply zoom to the normalized UVs

    // Create vec2 from separate float controls for camera rotation (for maximum host compatibility)
    vec2 xy_control_vec = vec2(xy_control_x, xy_control_y);

    // Apply camera shake directly to initial ray direction UVs
    if (shake_strength > 0.001) {
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency + hash11(1.0)) * 0.1,
            cos(current_time * shake_frequency * 1.1 + hash11(2.0)) * 0.1
        ) * shake_strength;
        uv += shake_offset;
    }

    // Apply glitch effect directly to initial ray direction UVs
    if (glitch_strength > 0.001) {
        float offset_x = (hash22(uv * 10.0 + current_time * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(uv.y * 150.0 + current_time * 20.0) * 0.5 + 0.5;
        uv.x += offset_x * scanline_strength * glitch_strength * 0.05;
    }

    // Normalize the 3D ray direction vector
    vec3 rayDirection = normalize(vec3(uv, 1.0));
    
    // Apply camera rotation based on xy_control_vec (mapped 0-1 range to full rotations)
    // 6.2831853 is 2 * PI (for full horizontal rotation)
    // 3.1415926 is PI (for half vertical rotation, preventing flip)
    rayDirection = R(rayDirection, vec3(0.577), xy_control_vec.x * 6.2831853);
    rayDirection = R(rayDirection, vec3(0.577), xy_control_vec.y * 3.1415926);


    vec3 currentPoint; // 'p' in original
    vec3 accumulatedColor = vec3(0.0);
    
    float i = 0.0; // Iteration count for raymarch loop
    float innerScale; // Corresponds to 's' in original inner loop
    float distanceEstimate; // Corresponds to 'e' in original's inner loop and step calculation
    float currentRayLength = dist_offset_start; // Corresponds to 'g' in original, initial value from tunable

    // Main raymarching loop
    for(int j = 0; j < int(raymarch_iterations); j++) {
        i = float(j + 1); // Increments 'i' for effects and color, starting from 1
        
        currentPoint = currentRayLength * rayDirection; // p=g*d;

        // Apply base rotation for fractal, influenced by morph_factor
        vec3 fractal_offset_base = vec3(1.0, 0.8, 1.2);
        vec3 fractal_offset = mix(fractal_offset_base, fractal_offset_base * (1.0 + morph_factor * 0.5), morph_factor);

        currentPoint += R(fractal_offset, vec3(0.577), current_time * base_rotation_speed_1);
        currentPoint = R(currentPoint, vec3(0.577), current_time * base_rotation_speed_2);
        
        // --- Core Fractal Folding Logic, with tunable multipliers ---
        currentPoint = cos(currentPoint * cosine_fold_mult_1 + cosine_fold_mult_2 * cos(currentPoint * cosine_fold_mult_3));
        
        // Add sin(time) morphing to Q vector (vec4(p, sin(iTime)*.2))
        vec4 q = vec4(currentPoint, sin(current_time) * q_sin_time_mult);
        innerScale = 3.0; // Initial 's' value for inner loop

        // Inner loop for self-similarity, with tunable iterations and factors
        for(int k = 0; k < int(fractal_inner_iterations); k++) {
            // q=clamp(q,-.5,.5)*2.-q,
            q = clamp(q, -clamp_limit, clamp_limit) * 2.0 - q; // Use tunable clamp_limit
            
            // s*=e=7.*clamp(.3/min(dot(q,q),1.),.0,1.), q*=e;
            // Calculate scaling factor 'e' for this iteration
            // inner_scale_factor (7.0) and inner_scale_clamp_val (0.3) are tunable
            distanceEstimate = inner_scale_factor * clamp(inner_scale_clamp_val / max(dot(q.xyz, q.xyz), 0.000001), 0.0, 1.0);
            innerScale *= distanceEstimate;
            q *= distanceEstimate; // Apply scaling to q
        }
        
        // --- Distance Accumulation (Raymarching Step) ---
        // g+=e=length(q)/s;
        distanceEstimate = length(q) / innerScale;
        currentRayLength += distanceEstimate; // Accumulate ray length

        // --- Color Calculation, with tunable parameters ---
        float pulse_val = sin(i * 0.1 + current_time * color_pulse_speed) * color_pulse_intensity;
        // Ensure log(max(0.001, innerScale)) to prevent log(0)
        vec3 fractal_color = getColorPalette(log(max(0.001, innerScale)) * log_scale_color_factor + pulse_val, color_palette_type);
        
        // The `exp(-i*i*e)` term is very sensitive. 'exponent_intensity' multiplies `distanceEstimate`.
        // A small constant multiplier (0.0001) is applied here to keep values in a visible range.
        float fading_factor = exp(-i * i * distanceEstimate * exponent_intensity * 0.0001); 

        // O.xyz+=mix(vec3(1),H(log(s)*.3),.8)*.03*exp(-i*i*e)
        accumulatedColor += mix(vec3(1.0), fractal_color, base_color_mix) * base_color_multiplier * fading_factor;
        
        // Break conditions for raymarching: if hit surface or ray went too far
        if (distanceEstimate < 0.0001 || currentRayLength > 100.0) break; 
    }

    // Final color post-processing, with tunable power, brightness, saturation, contrast
    // Original: O*=O*O*O; -> equivalent to pow(O, 4.0)
    accumulatedColor = pow(accumulatedColor, vec3(final_color_power));
    accumulatedColor = adjustColor(accumulatedColor, brightness, saturation, contrast);

    // Vignette effect (remains as fixed aesthetic, applied to final color)
    // Calculate UV based on a 0-1 range for vignette center
    vec2 vignette_uv = fragCoord / RENDERSIZE.xy;
    float vignette_val = pow(16.0 * vignette_uv.x * vignette_uv.y * (1.0 - vignette_uv.x) * (1.0 - vignette_uv.y), 0.3);
    accumulatedColor *= mix(1.0, vignette_val, 0.5); // Mix with original color, 0.5 intensity

    fragColor = vec4(clamp(accumulatedColor, 0.0, 1.0), 1.0);
    gl_FragColor = fragColor;
}