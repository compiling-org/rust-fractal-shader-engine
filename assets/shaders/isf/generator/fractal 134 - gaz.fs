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
    "DESCRIPTION": "An ISF conversion of a complex 4D fractal shader, featuring dynamic rotations, intricate folding, and a unique distance estimator. Enhanced with numerous 'DMT-like' psychedelic color palettes, granular color pulse controls, **full 3D camera positioning and rotation**, and dedicated glitch and shake effects.",
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
            "NAME": "cameraPosX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera position X."
        },
        {
            "NAME": "cameraPosY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera position Y."
        },
        {
            "NAME": "cameraPosZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera position Z."
        },
        {
            "NAME": "cameraLookAtX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera look-at point X."
        },
        {
            "NAME": "cameraLookAtY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera look-at point Y."
        },
        {
            "NAME": "cameraLookAtZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera look-at point Z."
        },
        {
            "NAME": "cameraRotX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -3.14159,
            "MAX": 3.14159,
            "STEP": 0.01,
            "DESCRIPTION": "Camera rotation around X-axis (pitch)."
        },
        {
            "NAME": "cameraRotY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -3.14159,
            "MAX": 3.14159,
            "STEP": 0.01,
            "DESCRIPTION": "Camera rotation around Y-axis (yaw)."
        },
        {
            "NAME": "cameraRotZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -3.14159,
            "MAX": 3.14159,
            "STEP": 0.01,
            "DESCRIPTION": "Camera rotation around Z-axis (roll)."
        },
        {
            "NAME": "cameraFOV",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Field of View (lower value means narrower angle/more zoom)."
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
            "NAME": "fractalRotationSpeed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the fractal's primary rotation."
        },
        {
            "NAME": "fractalRotAxisHueOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Offset for the hue of the fractal's dynamic rotation axis."
        },
        {
            "NAME": "initialZOffset",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Initial Z-offset for the fractal's position in 4D space."
        },
        {
            "NAME": "innerIterations",
            "TYPE": "float",
            "DEFAULT": 7.0,
            "MIN": 1.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the inner 4D folding loop. Higher values increase complexity."
        },
        {
            "NAME": "foldParamA",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "First parameter for the 4D folding operation (e.g., `value - abs(p - offset)`)."
        },
        {
            "NAME": "foldParamB",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Second parameter for the 4D folding operation (e.g., `value - abs(p - offset)`)."
        },
        {
            "NAME": "scaleFactorMin",
            "TYPE": "float",
            "DEFAULT": 1.3,
            "MIN": 1.0,
            "MAX": 3.0,
            "STEP": 0.01,
            "DESCRIPTION": "Minimum scaling factor for fractal iterations (`max(1/dot(p,p), min_val)`)."
        },
        {
            "NAME": "finalFoldOffset",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Offset applied after scaling in the inner fold loop (e.g., `*e - offset`)."
        },
        {
            "NAME": "distanceEstimatorParam",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Parameter for the final distance estimator (e.g., `abs(length(p.wz*p.x-p.y)/s-param)`)."
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
            "DEFAULT": 0.9,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Factor for log(s) in color calculation, affects hue distribution within the palette."
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
            "DESCRIPTION": "Overall strength of the camera shake effect."
        },
        {
            "NAME": "shakeFrequency",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 0.1,
            "MAX": 50.0,
            "STEP": 0.1,
            "DESCRIPTION": "Frequency of camera shake oscillations."
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

// 3D Rotation Matrix functions
mat3 rotateX(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        1.0, 0.0, 0.0,
        0.0, c, -s,
        0.0, s, c
    );
}

mat3 rotateY(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, 0.0, s,
        0.0, 1.0, 0.0,
        -s, 0.0, c
    );
}

mat3 rotateZ(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(
        c, -s, 0.0,
        s, c, 0.0,
        0.0, 0.0, 1.0
    );
}

// Matrix inversion for 3x3 (needed for inverse camera transform)
mat3 inverse(mat3 m) {
    float det = dot(m[0], cross(m[1], m[2]));
    return mat3(
        cross(m[1], m[2]),
        cross(m[2], m[0]),
        cross(m[0], m[1])
    ) / det;
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

// Shake effect (applied to camera origin and look-at for genuine scene shake)
vec3 applyShake(vec3 p, float strength, float frequency, float time_val) {
    vec3 shake_offset = vec3(
        sin(time_val * frequency + hash11(1.0)) * 0.1,
        cos(time_val * frequency * 1.1 + hash11(2.0)) * 0.1,
        sin(time_val * frequency * 1.2 + hash11(3.0)) * 0.1
    ) * strength;
    return p + shake_offset;
}


void main() {
    vec4 O = vec4(0); // Output color
    vec2 C = gl_FragCoord.xy;
    vec3 r = vec3(RENDERSIZE.xy, 1.0); // ISF equivalent of iResolution

    vec2 uv_normalized = C / RENDERSIZE.xy;
    float current_time_total = TIME * animationSpeed;

    // --- Shake Effect ---
    vec3 ro_base = vec3(cameraPosX, cameraPosY, cameraPosZ);
    vec3 lk_base = vec3(cameraLookAtX, cameraLookAtY, cameraLookAtZ);

    vec3 ro = applyShake(ro_base, shakeStrength, shakeFrequency, current_time_total);
    vec3 lk = applyShake(lk_base, shakeStrength, shakeFrequency, current_time_total);

    // --- Camera Setup: Re-integrated 3D Camera ---
    vec3 fwd = normalize(lk - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd)); // Standard 'up' for right vector
    vec3 up = cross(fwd, rgt);

    // Apply camera rotations to the camera basis vectors
    mat3 cam_rot_matrix = rotateX(cameraRotX) * rotateY(cameraRotY) * rotateZ(cameraRotZ);
    fwd = cam_rot_matrix * fwd;
    rgt = cam_rot_matrix * rgt;
    up = cam_rot_matrix * up;

    // Construct the ray direction based on standard camera model and FOV
    // The distortion is applied to uv_normalized *before* ray direction calculation
    vec2 distorted_uv = uv_normalized;
    if (glitchStrength > 0.001) {
        distorted_uv = applyGlitchUV(uv_normalized, glitchStrength, current_time_total);
    }
    
    // Scale UV by aspect ratio and adjust for FOV
    vec3 rd = normalize(distorted_uv.x * r.x / r.y * rgt + distorted_uv.y * up + fwd / cameraFOV);

    float i = 0.0; // Outer loop counter for raymarching
    float g = 0.0; // Accumulated distance
    float e;      // Temp variable
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
    for (; i++ < 99.; ) {
        // Calculate the current point along the ray
        vec4 p = vec4(ro + rd * g, 0.08);

        // Apply fractal scale
        p.xyz *= fractalScale;

        // Apply inverse camera transform to the point 'p'
        // This makes the camera controls effectively move the fractal
        vec3 transformed_p = inverse(cam_rot_matrix) * (p.xyz - ro); // Subtract camera pos, then apply inverse rotation
        p.xyz = transformed_p;

        // Original fractal transformations now apply to the camera-transformed point
        p.z -= initialZOffset;
        
        vec3 rot_axis = normalize(H(t_fractal * 0.05 + fractalRotAxisHueOffset));
        p.xyz = R(p.xyz, rot_axis, t_fractal * fractalRotationSpeed);

        s = 1.0; 
        
        // Inner loop for 4D folding
        for (int j = 0; j++ < int(innerIterations); ) {
            p = foldParamA - abs(p - foldParamB);
            s *= (e = max(1.0 / dot(p, p), scaleFactorMin));
            if (p.x < p.y) {
                p = abs(p.wzxy) * e - finalFoldOffset;
            } else {
                p = abs(p.wzyx) * e - finalFoldOffset;
            }
        }

        e = abs(length(p.wz * p.x - p.y) / s - distanceEstimatorParam);
        g += e + 1e-4; 

        // Corrected Color accumulation: mix between palette colors using log(s) for mix factor
        // and incorporating color pulse.
        float color_mix_factor = H(log(s) * logScaleFactor + actual_pulse_influence).x;
        vec3 base_color = mix(palette_color_a, palette_color_b, color_mix_factor);
        
        O.xyz += base_color * glowIntensity * (colorAccumulationFactor / exp(i * i * e));

        // Break if distance is too small (hit surface) or ray is too long
        if (e < 0.0001 || g > 100.0) break;
    }

    // The original shader squares the color, which darkens it and increases contrast.
    O.xyz *= O.xyz;

    // Apply brightness, saturation, contrast
    O.rgb = adjustBrightness(O.rgb, brightness);
    O.rgb = adjustSaturation(O.rgb, saturation);
    O.rgb = adjustContrast(O.rgb, contrast);

    // Apply vignette
    float vignette_val = pow(16.0 * uv_normalized.x * uv_normalized.y * (1.0 - uv_normalized.x) * (1.0 - uv_normalized.y), 0.3);
    O.rgb *= mix(1.0, vignette_val, vignetteIntensity);

    gl_FragColor = vec4(clamp(O.xyz, 0.0, 1.0), 1.0);
}