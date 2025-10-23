/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "4D",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Glitch"
    ],
    "DESCRIPTION": "An ISF conversion of a highly dynamic 4D fractal, featuring unique 'asin(cos)' folding, an animated 4D 'w' component, and a distinct 'box fold' transformation. Enhanced with vibrant psychedelic color palettes, precise color pulsing, complex motion controls, view panning/zoom, and integrated glitch/shake effects.",
    "CREDIT": "Original shader by Dave_Van_Dorn (https://www.shadertoy.com/view/WtByzc), converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall speed of the animation."
        },
        {
            "NAME": "viewOffsetX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Horizontal offset of the view/camera."
        },
        {
            "NAME": "viewOffsetY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Vertical offset of the view/camera."
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the zoom level of the view. Higher values zoom in (affects the Z component of the ray)."
        },
        {
            "NAME": "initialRayRotSpeed",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the initial ray rotation that generates the fractal's swirl."
        },
        {
            "NAME": "initialRayRotDepthInfluence",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Influences how much the initial ray rotation angle depends on ray depth."
        },
        {
            "NAME": "fractalDriftSpeedZ",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the fractal's Z-axis drift over time."
        },
        {
            "NAME": "asinCosFoldFactor",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Factor for the 'asin(cos(p*factor))' folding operation. Affects fractal geometry."
        },
        {
            "NAME": "raymarchIterations",
            "TYPE": "float",
            "DEFAULT": 99.0,
            "MIN": 10.0,
            "MAX": 200.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of raymarching steps. Higher values increase detail but reduce performance."
        },
        {
            "NAME": "innerIterations",
            "TYPE": "float",
            "DEFAULT": 8.0,
            "MIN": 1.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the inner 4D folding loop. Higher values increase complexity."
        },
        {
            "NAME": "qW_Base",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Base value for the 'w' component of the 4D fractal point 'q'."
        },
        {
            "NAME": "qW_PulseAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.005,
            "MIN": 0.0,
            "MAX": 0.05,
            "STEP": 0.0001,
            "DESCRIPTION": "Amplitude of the pulse for the 'w' component of 'q'."
        },
        {
            "NAME": "qW_PulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Speed of the pulse for the 'w' component of 'q'."
        },
        {
            "NAME": "scaleFactorMul",
            "TYPE": "float",
            "DEFAULT": 3.0,
            "MIN": 1.0,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "Multiplier for the scaling factor in fractal iterations (e.g., `mul / max(dot(q,q), 1.0)`)."
        },
        {
            "NAME": "scaleFactorMinDot",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Minimum value for `dot(q,q)` in scaling factor calculation. Affects fractal 'density'."
        },
        {
            "NAME": "boxFoldCenter",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Center point for the inner 'box fold' operation (e.g., `center - abs(q - offset)`)."
        },
        {
            "NAME": "boxFoldAbsOffset",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Offset for the inner `abs(q-offset)` part of the box fold."
        },
        {
            "NAME": "finalOffsetVecX",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "X-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "finalOffsetVecY",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Y-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "finalOffsetVecZ",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Z-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "finalOffsetVecW",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "W-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 11 predefined psychedelic color palettes (0-10)."
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed at which the colors pulse and shift (0 for no pulse)."
        },
        {
            "NAME": "colorPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls how much the colors pulse between their primary and secondary states."
        },
        {
            "NAME": "colorPulseOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.28318,
            "STEP": 0.01,
            "DESCRIPTION": "Shifts the starting phase of the color pulse."
        },
        {
            "NAME": "glowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall intensity of the glow/light emanating from the fractal."
        },
        {
            "NAME": "logScaleFactorColor",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Factor for log(s) in hue calculation. Affects color distribution with fractal detail."
        },
        {
            "NAME": "timeColorSpeed",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed at which time influences the color hue."
        },
        {
            "NAME": "initialColorMixFactor",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Initial mixing ratio between pure white and the psychedelic color for accumulated color."
        },
        {
            "NAME": "colorAccumulationBase",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Base value for color accumulation before exponential decay."
        },
        {
            "NAME": "colorDecayFactor",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Factor for exponential color decay. Higher values make colors fade faster with depth/iterations."
        },
        {
            "NAME": "finalColorPower",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 1.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Power applied to the final color for contrast adjustment. (e.g., c*c*c*c means 4.0)"
        },
        {
            "NAME": "glitchStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall strength of the digital glitch effect."
        },
        {
            "NAME": "glitchFrequency",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 0.1,
            "MAX": 50.0,
            "STEP": 0.1,
            "DESCRIPTION": "Frequency of glitch occurrences."
        },
        {
            "NAME": "glitchDisplacement",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.2,
            "STEP": 0.001,
            "DESCRIPTION": "Amount of UV displacement during a glitch."
        },
        {
            "NAME": "shakeStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall strength of the screen shake effect."
        },
        {
            "NAME": "shakeFrequency",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 0.1,
            "MAX": 50.0,
            "STEP": 0.1,
            "DESCRIPTION": "Frequency of screen shake oscillations."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image brightness."
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image color saturation."
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image contrast."
        },
        {
            "NAME": "vignetteIntensity",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the darkening vignette effect around the edges."
        }
    ]
}
*/

// Rodrigues' rotation formula: R(p, a, t) = p * cos(t) + cross(a, p) * sin(t) + a * dot(a, p) * (1.0 - cos(t))
// Simplified macro for unit vector 'a'.
#define R(p,a,t) (mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a))

// Helper to generate a psychedelic color based on a hue input
vec3 H(float h) {
    return (cos(h * 6.3 + vec3(0, 23, 21)) * .5 + .5);
}

// Function to adjust brightness
vec3 adjustBrightness(vec3 color, float val) {
    return color * val;
}

// Function to adjust saturation
vec3 adjustSaturation(vec3 color, float val) {
    vec3 gray = vec3(dot(color, vec3(0.2126, 0.7152, 0.0722)));
    return mix(gray, color, val);
}

// Function to adjust contrast
vec3 adjustContrast(vec3 color, float val) {
    return (color - 0.5) * val + 0.5;
}

// Simple hash function for noise
float hash11(float p) {
    p = fract(p * .1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash22(vec2 p) {
    return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432);
}

// Glitch effect (modifies UV for ray direction distortion)
vec2 applyGlitchUV(vec2 uv, float strength, float time_val) {
    // Horizontal shifts
    float offset_x = (hash22(uv * 10.0 + time_val * glitchFrequency) - 0.5) * 2.0; // Random horizontal shift
    float scanline_strength = sin(uv.y * 150.0 + time_val * 20.0) * 0.5 + 0.5; // Animated scanline pattern
    uv.x += offset_x * scanline_strength * strength * glitchDisplacement;

    // Vertical jumps (less common, more abrupt)
    float jump_chance = 0.99; // Higher value means less frequent jumps
    if (hash22(uv.yx * 7.0 + time_val * 3.0 + 123.45) > jump_chance) {
        uv.y += (hash22(uv.xy * 2.0 + time_val * 3.0 + 54.321) * 2.0 - 1.0) * strength * glitchDisplacement * 0.5;
    }
    return uv;
}

// Shake effect (applied to view offsets for a screen shake)
vec2 applyShakeOffset(vec2 offset_in, float strength, float frequency, float time_val) {
    vec2 shake_offset = vec2(
        sin(time_val * frequency + hash11(1.0)) * 0.1,
        cos(time_val * frequency * 1.1 + hash11(2.0)) * 0.1
    ) * strength;
    return offset_in + shake_offset;
}


void main() {
    vec4 O = vec4(0); // Output color
    vec2 C = gl_FragCoord.xy;
    vec3 r = vec3(RENDERSIZE.xy, 1.0); // ISF equivalent of iResolution

    vec2 uv_normalized = C / RENDERSIZE.xy;
    float current_time_total = TIME * animationSpeed;

    // --- Shake Effect on View ---
    vec2 current_view_offset = vec2(viewOffsetX, viewOffsetY);
    if (shakeStrength > 0.001) {
        current_view_offset = applyShakeOffset(current_view_offset, shakeStrength, shakeFrequency, current_time_total);
    }

    // --- Glitch Effect (on UVs for ray direction distortion) ---
    vec2 distorted_uv = uv_normalized;
    if (glitchStrength > 0.001) {
        distorted_uv = applyGlitchUV(uv_normalized, glitchStrength, current_time_total);
    }

    // Normalized ray direction from camera, consistent with original Shadertoy shader.
    // (C-.5*r.xy)/r.y normalizes the screen coordinates relative to height, then Z component is fixed.
    // Zoom is applied via zoomFactor, affecting the Z component of the ray.
    vec3 d = normalize(vec3((distorted_uv - 0.5 + current_view_offset) * r.xy / r.y, 3.0 / zoomFactor));

    vec3 p;
    vec4 q; // Declare q here for broader scope
    float i = 0.0; // Outer loop counter for raymarching
    float g = 0.0; // Accumulated distance
    float e;      // Temp variable (reused for various calculations)
    float s;      // Scaling factor for fractal detail
    float t_fractal = current_time_total; // Time for fractal animation

    // Define color palettes (expanded to 11 palettes, 0-10)
    vec3 palette_color_a, palette_color_b;
    float pulse_phase = (sin(current_time_total * colorPulseSpeed + colorPulseOffset) * 0.5 + 0.5); // 0 to 1 for pulsing
    float actual_pulse_influence = pulse_phase * colorPulseIntensity; // Control intensity of pulse

    if (colorPalette < 0.5) { // Palette 0: Neon Dreams (Blue/Pink/Green)
        palette_color_a = vec3(0.1, 0.5, 1.0); // Electric Blue
        palette_color_b = vec3(1.0, 0.2, 0.8); // Hot Pink
    } else if (colorPalette < 1.5) { // Palette 1: Acid Trip (Yellow/Green/Purple)
        palette_color_a = vec3(0.8, 1.0, 0.1); // Acid Yellow-Green
        palette_color_b = vec3(0.6, 0.1, 0.9); // Deep Purple
    } else if (colorPalette < 2.5) { // Palette 2: Lava Lamp (Orange/Red/Teal)
        palette_color_a = vec3(1.0, 0.4, 0.0); // Fiery Orange
        palette_color_b = vec3(0.0, 0.8, 0.7); // Bright Teal
    } else if (colorPalette < 3.5) { // Palette 3: Galactic Swirl (Deep Blue/Magenta/Cyan)
        palette_color_a = vec3(0.1, 0.1, 0.6); // Deep Indigo
        palette_color_b = vec3(0.9, 0.1, 0.9); // Vibrant Magenta
    } else if (colorPalette < 4.5) { // Palette 4: Rainbow Spectrum (DMT-like 1)
        palette_color_a = H(current_time_total * 0.1 + 0.0);
        palette_color_b = H(current_time_total * 0.1 + 1.0);
    } else if (colorPalette < 5.5) { // Palette 5: Hyper-Fluorescence (DMT-like 2 - Neons with strong contrast)
        palette_color_a = vec3(0.0, 1.0, 0.0) * (0.5 + 0.5 * sin(current_time_total * 2.0)); // Pulsing Green
        palette_color_b = vec3(1.0, 0.0, 1.0) * (0.5 + 0.5 * cos(current_time_total * 2.0)); // Pulsing Magenta
    } else if (colorPalette < 6.5) { // Palette 6: Shimmering Gold/Blue (DMT-like 3 - Iridescent)
        palette_color_a = vec3(1.0, 0.8, 0.2) * (0.7 + 0.3 * sin(current_time_total * 1.5)); // Warm Gold shimmer
        palette_color_b = vec3(0.2, 0.4, 1.0) * (0.7 + 0.3 * cos(current_time_total * 1.5)); // Cool Blue shimmer
    } else if (colorPalette < 7.5) { // Palette 7: Psychedelic Sunset (Warm, shifting, vibrant)
        palette_color_a = vec3(1.0, 0.1, 0.0); // Deep Orange-Red
        palette_color_b = vec3(0.8, 0.9, 0.1); // Bright Lime-Yellow
    } else if (colorPalette < 8.5) { // Palette 8: Glitchy Grayscale (Subtle color tint with high contrast)
        palette_color_a = vec3(0.2, 0.2, 0.2) + vec3(0.1, 0.0, 0.0) * sin(current_time_total * 5.0);
        palette_color_b = vec3(0.8, 0.8, 0.8) + vec3(0.0, 0.0, 0.1) * cos(current_time_total * 5.0);
    } else if (colorPalette < 9.5) { // Palette 9: Alien Tech (Electric Blues and Greens)
        palette_color_a = vec3(0.0, 0.7, 1.0); // Bright Sky Blue
        palette_color_b = vec3(0.5, 1.0, 0.0); // Chartreuse Green
    } else { // Palette 10: Deep Space Nebula (Purples, Cyans, Pinks)
        palette_color_a = vec3(0.6, 0.0, 0.8); // Deep Purple
        palette_color_b = vec3(0.0, 0.8, 1.0); // Cyan
    }

    // Main raymarching loop
    for (; i++ < raymarchIterations; ) {
        p = g * d; // Current point along the ray

        // Apply initial rotation to 'p' based on time and ray depth
        // Original: p.xyz=R(p.xyz,normalize(H(t*.05)*2.-1.),g*.2);
        vec3 rot_axis = normalize(H(t_fractal * initialRayRotSpeed) * 2.0 - 1.0);
        p.xyz = R(p.xyz, rot_axis, g * initialRayRotDepthInfluence);

        // Apply time-based fractal Z-drift
        // Original: p.z+=t*.3;
        p.z += t_fractal * fractalDriftSpeedZ;

        // Apply the 'asin(cos(p*factor))' fold
        // Original: p=asin(cos(p*.8));
        p = asin(cos(p * asinCosFoldFactor));

        // Initialize 4D vector 'q' with animated 'w' component
        // Original: q=vec4(p,.7+.005*sin(t*.05));
        q = vec4(p, qW_Base + qW_PulseAmplitude * sin(t_fractal * qW_PulseSpeed));

        s = 1.0; 
        
        // Inner loop for 4D folding (Box Fold / Mandelbox-like)
        // Original: for(int i=0;i++<8;)
        // Changed inner loop variable 'i' to 'j' for clarity.
        vec4 final_offset_vec = vec4(finalOffsetVecX, finalOffsetVecY, finalOffsetVecZ, finalOffsetVecW);
        for (int j = 0; j++ < int(innerIterations); ) {
            // Original: s*=e=max(3./dot(q,q),1.),
            s *= (e = scaleFactorMul / max(dot(q, q), scaleFactorMinDot));
            
            // Original: q=.1-abs(q-.1),
            // Box fold / Mirror fold
            q = boxFoldCenter - abs(q - boxFoldAbsOffset);
            
            // Original: q=abs(q.x<q.y?q.wzxy:q.wzyx)*e-vec4(.3,.8,1.2,1.2);
            // Conditional swizzle, then scale and offset
            if (q.x < q.y) {
                q = q.wzxy;
            } else {
                q = q.wzyx;
            }
            q = abs(q) * e - final_offset_vec;
        }

        // Distance Estimator for this fractal
        // Original: g+=e=max(-q.w,q.z)/s;
        e = max(-q.w, q.z) / s;
        g += e; // Accumulate distance

        // Color accumulation
        // Original: c+=mix(vec3(1),H(log(s*.1)+t*2.),.6)*.02/exp(.2*i*i*e);
        // Combine log(s) and time for dynamic hue, and integrate pulse influence.
        float current_hue_input = log(s * logScaleFactorColor) + t_fractal * timeColorSpeed;
        vec3 psychedelic_color = H(current_hue_input + actual_pulse_influence); // Apply pulse to the base hue
        
        vec3 current_color_contribution = mix(vec3(1.0), mix(palette_color_a, palette_color_b, psychedelic_color.x), initialColorMixFactor);
        
        O.xyz += current_color_contribution * glowIntensity * (colorAccumulationBase / exp(colorDecayFactor * i * i * e));

        // Break if distance is too small (hit surface) or ray is too long
        if (e < 0.0001 || g > 100.0) break; 
    }

    // Final color adjustment
    // Original: c*=c*c*c;
    O.xyz = pow(O.xyz, vec3(finalColorPower));

    // Apply brightness, saturation, contrast
    O.rgb = adjustBrightness(O.rgb, brightness);
    O.rgb = adjustSaturation(O.rgb, saturation);
    O.rgb = adjustContrast(O.rgb, contrast);

    // Apply vignette
    float vignette_val = pow(16.0 * uv_normalized.x * uv_normalized.y * (1.0 - uv_normalized.x) * (1.0 - uv_normalized.y), 0.3);
    O.rgb *= mix(1.0, vignette_val, vignetteIntensity);

    gl_FragColor = vec4(clamp(O.xyz, 0.0, 1.0), 1.0);
}