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
    "DESCRIPTION": "An ISF conversion of a dynamic 4D fractal, featuring an 'asin(sin)' folding operation and a ray-dependent initial rotation. Enhanced with vibrant psychedelic color palettes, precise color pulsing, morphing geometry controls, view panning/perspective, and integrated glitch/shake effects.",
    "CREDIT": "Original shader by Dave_Van_Dorn (https://www.shadertoy.com/view/wsGyzc), converted and enhanced for ISF by Gemini.",
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
            "NAME": "perspectiveFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the perspective/depth of the view. Lower value for wider FOV."
        },
        {
            "NAME": "fractalScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Scales the overall size of the fractal geometry."
        },
        {
            "NAME": "initialRayRotSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the initial ray rotation that generates the fractal's swirl."
        },
        {
            "NAME": "initialRayRotAxisHueOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Offset for the hue of the initial ray's dynamic rotation axis."
        },
        {
            "NAME": "initialRayRotDepthInfluence",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Influences how much the initial ray rotation angle depends on ray depth."
        },
        {
            "NAME": "initialP_W",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Initial 'w' component of the 4D fractal point."
        },
        {
            "NAME": "zTimeOffsetSpeed",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed at which the fractal's Z-position changes over time."
        },
        {
            "NAME": "raymarchIterations",
            "TYPE": "float",
            "DEFAULT": 90.0,
            "MIN": 10.0,
            "MAX": 200.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of raymarching steps. Higher values increase detail but reduce performance."
        },
        {
            "NAME": "innerIterations",
            "TYPE": "float",
            "DEFAULT": 6.0,
            "MIN": 1.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the inner 4D folding loop. Higher values increase complexity."
        },
        {
            "NAME": "foldOffsetVecX",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "X-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "foldOffsetVecY",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Y-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "foldOffsetVecZ",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Z-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "foldOffsetVecW",
            "TYPE": "float",
            "DEFAULT": 1.7,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "W-component of the constant offset vector applied in inner fold loop."
        },
        {
            "NAME": "scaleFactorMul",
            "TYPE": "float",
            "DEFAULT": 2.7,
            "MIN": 1.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Multiplier for the scaling factor in fractal iterations (e.g., `mul / min(dot(p,p), 2.0)`)."
        },
        {
            "NAME": "scaleFactorMinDot",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Minimum value for `dot(p,p)` in scaling factor calculation. Affects fractal 'density'."
        },
        {
            "NAME": "distEstimatorParam",
            "TYPE": "float",
            "DEFAULT": 0.0001,
            "MIN": 0.0,
            "MAX": 0.001,
            "STEP": 0.00001,
            "DESCRIPTION": "Parameter for the final distance estimator (e.g., `abs(p.w)/s + param`)."
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
            "NAME": "logScaleFactor",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Factor for `log(s)` in color calculation, affects hue distribution within the palette."
        },
        {
            "NAME": "initialColorMixFactor",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Initial mixing ratio between pure white and the psychedelic color for accumulated color."
        },
        {
            "NAME": "colorAccumulationFactor",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Base factor for color accumulation (e.g., `0.04 / exp(i*i*e)`)."
        },
        {
            "NAME": "finalColorPower",
            "TYPE": "float",
            "DEFAULT": 3.0,
            "MIN": 1.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Power applied to the final color for contrast adjustment. (c*c*c means 3.0)"
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
    // Panning (xy control) implemented via current_view_offset.
    // Zoom (perspective) implemented via perspectiveFactor.
    vec3 d = normalize(vec3((distorted_uv - 0.5 + current_view_offset) * r.xy / r.y, perspectiveFactor));

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
        // Original: vec4 p=vec4(R(g*d,normalize(H(t*.1)),g*.1),.2 );
        // Apply initial rotation to the ray point based on distance 'g' and time 't'
        vec3 rot_axis = normalize(H(t_fractal * initialRayRotSpeed + initialRayRotAxisHueOffset));
        vec3 rotated_ray_point = R(g * d, rot_axis, g * initialRayRotDepthInfluence);
        
        vec4 p = vec4(rotated_ray_point, initialP_W);

        // Original: p.z+=t*.4;
        p.z += t_fractal * zTimeOffsetSpeed;

        // Original: p=asin(sin(p));
        p = asin(sin(p));

        // Apply fractal scale
        p.xyz *= fractalScale;

        s = 1.0; 
        
        // Inner loop for 4D folding
        // Original: for(int i=0;i++<6;p=p*e-vec4(1,.3,.5,1.7))
        // Changed inner loop variable 'i' to 'j' for clarity, avoids shadowing outer 'i'.
        vec4 fold_offset_vec = vec4(foldOffsetVecX, foldOffsetVecY, foldOffsetVecZ, foldOffsetVecW);
        for (int j = 0; j++ < int(innerIterations); ) {
            p = abs(p); // Original: p=abs(p),
            
            // Original: p=p.x<p.y?p.zwxy:p.zwyx,
            // Conditional swizzle
            if (p.x < p.y) {
                p = p.zwxy;
            } else {
                p = p.zwyx;
            }

            // Original: s*=e=2.7/min(dot(p,p),2.);
            s *= (e = scaleFactorMul / min(dot(p, p), scaleFactorMinDot));

            p = p * e - fold_offset_vec; // Post-loop modification moved inside the loop as per original syntax
        }

        // Original: g+=e=abs(p.w)/s+1e-4;
        e = abs(p.w) / s + distEstimatorParam;
        g += e; // Accumulate distance

        // Original: c+=mix(vec3(1),H(log(s)*.3),.4)*.04/exp(i*i*e);
        // Corrected Color accumulation: mix between palette colors using log(s) for mix factor
        // and incorporating color pulse.
        float color_mix_factor = H(log(s) * logScaleFactor + actual_pulse_influence).x;
        vec3 current_color_contribution = mix(vec3(1.0), mix(palette_color_a, palette_color_b, color_mix_factor), initialColorMixFactor);
        
        O.xyz += current_color_contribution * glowIntensity * (colorAccumulationFactor / exp(i * i * e));

        // Break if distance is too small (hit surface) or ray is too long
        if (e < 0.0001 || g > 100.0) break; 
    }

    // Original: c*=c*c;
    // Apply final color power for contrast
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