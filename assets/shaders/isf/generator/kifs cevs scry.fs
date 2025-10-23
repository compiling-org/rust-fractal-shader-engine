/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Feedback"
    ],
    "DESCRIPTION": "A dynamic fractal box system with feedback, enhanced with extensive tunable parameters for animation, fractal geometry, color pulsing, a selection of trippy psychedelic color palettes, and post-processing controls. Original concept written in KodeLife, converted and enhanced for ISF.",
    "CREDIT": "Original Shadertoy by scry (https://www.shadertoy.com/view/wtXyWf), converted and enhanced by your AI assistant.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall animation speed multiplier for all time-based effects."
        },
        {
            "NAME": "controlX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls horizontal offset or motion of the pattern."
        },
        {
            "NAME": "controlY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls vertical offset or motion of the pattern."
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall zoom level of the fractal pattern."
        },
        {
            "NAME": "fractalDepth",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 1.0,
            "MAX": 30.0,
            "STEP": 1.0,
            "DESCRIPTION": "Controls the number of iterations for fractal complexity. Higher values mean more detail."
        },
        {
            "NAME": "patternScale",
            "TYPE": "float",
            "DEFAULT": 13.0,
            "MIN": 1.0,
            "MAX": 30.0,
            "STEP": 0.1,
            "DESCRIPTION": "Scales the repetition density of the primary box pattern."
        },
        {
            "NAME": "distortionStrength",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.2,
            "STEP": 0.005,
            "DESCRIPTION": "Strength of the time-based warping/distortion applied to the pattern."
        },
        {
            "NAME": "rotationSpeed",
            "TYPE": "float",
            "DEFAULT": 0.004,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Speed of the fractal's internal rotation."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 7.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 8 distinct psychedelic color palettes (0-7).",
            "PRAGMA": "COLOR_PALETTE_ENUM"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Base speed for all color pulsing effects."
        },
        {
            "NAME": "colorPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall intensity multiplier for all color pulsing effects."
        },
        {
            "NAME": "colorPulseHueStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of hue shifting in the color pulse."
        },
        {
            "NAME": "colorPulseSatStrength",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of saturation pulsing."
        },
        {
            "NAME": "colorPulseValStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of value (brightness) pulsing."
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
            "DESCRIPTION": "Adjusts the overall color saturation."
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
            "DEFAULT": 0.35,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the darkening effect at the edges of the screen."
        }
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

// Define the base time offset from the original shader
#define BASE_TIME_OFFSET_BUFFERA 210.0
#define BASE_TIME_OFFSET_MAIN 380.0

// --- Utility Functions ---

// 2D rotation.
vec2 rotate2d (vec2 uv, float a) {
    return uv*mat2(cos(a), sin(a),
                  -sin(a), cos(a));
}

// Basic box shape function. Returns 1.0 for border, 0.0 for inside or outside.
// This function remains binary (0 or 1) as per the original logic,
// color variation is injected in rgboxloop.
float box (vec2 uv, float bord, float size) {
    size = abs(size);

    if (uv.x < size && uv.x > -size && uv.y > -size && uv.y < size) {
        if (uv.x < size - bord && uv.x > -size + bord && uv.y < size - bord && uv.y > -size + bord) {
            return 0.0; // Inside the border (hole)
        } else {
            return 1.0; // On the border (line)
        }
    }
    return 0.0; // Outside the box
}

// Loops the box function to create layered/flickering boxes for R, G, B channels.
// NOW injects rich, time-based and UV-based colors.
vec3 rgboxloop(vec2 uv, float bs, float time_val) {
    vec3 col = vec3(0.);
    int steps = 13; // Fixed steps, could be a uniform if needed
    float s = 0.005; // Border thickness

    for (int i=0;i<steps;i++) {
        float box_val = box(uv, s, fract(bs)); // This will be 0 or 1
        
        // Generate a dynamic, vibrant color for each layer.
        // This ensures the color palettes have rich input to work with.
        vec3 layer_color = vec3(
            sin(time_val * 0.5 + uv.x * 3.0 + float(i) * 0.2),
            cos(time_val * 0.7 + uv.y * 4.0 + float(i) * 0.3),
            sin(time_val * 0.6 + uv.x * 2.0 + uv.y * 5.0 + float(i) * 0.4 + 1.57)
        ) * 0.5 + 0.5; // Map to 0-1 range for vibrant colors

        col += box_val * layer_color; // Add the color only where the box border is 1
        bs += 1./float(steps); // Advance the base size for the next layer
    }
    return col;
}

// Generates a dynamic box system pattern.
vec3 boxSys(vec2 uv, float t, float distortionStrength_in, float patternScale_in) {
    vec3 col = vec3(0.);
    float bs = t * 0.5;
    vec2 gv = fract(uv) - 0.5; // Tile UVs to -0.5 to 0.5 range
    float tc = t + uv.x + uv.y;
    gv += vec2(sin(tc), cos(tc)) * distortionStrength_in; // Apply distortion
    gv = fract(gv * patternScale_in) - 0.5; // Scale and re-tile
    col += rgboxloop(gv, bs, t); // Pass time to rgboxloop for color variation
    return col;
}

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.x, 0.0, 1.0), c.y);
}

// RGB to HSV conversion
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Applies color pulsing and chosen palette to an input color.
vec3 applyColorPalette(vec3 color_in, float iTime_scaled) {
    vec3 hsv = rgb2hsv(color_in);

    // Apply granular color pulse controls, influenced by overall colorPulseIntensity
    float huePulse = iTime_scaled * colorPulseHueStrength * colorPulseIntensity;
    float satPulseFactor = (1.0 + colorPulseSatStrength * sin(iTime_scaled * 2.0) * colorPulseIntensity);
    float valPulseFactor = (1.0 + colorPulseValStrength * cos(iTime_scaled * 3.0) * colorPulseIntensity);

    hsv.x = mod(hsv.x + huePulse, 1.0);
    hsv.y = hsv.y * satPulseFactor;
    hsv.z = hsv.z * valPulseFactor;
    hsv.y = clamp(hsv.y, 0.0, 1.0); // Clamp saturation to valid range
    hsv.z = clamp(hsv.z, 0.0, 1.0); // Clamp value to valid range

    int palette = int(colorPalette);

    // Palettes now shift and influence existing vibrant colors
    if (palette == 1) { // Psychedelic Dream (Purples, Cyans, Pinks)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.6 + sin(iTime_scaled * 0.05) * 0.1, 1.0), 0.7); // Stronger mix
        hsv.y = clamp(hsv.y * 1.5 + 0.1, 0.0, 1.0); // Boost saturation
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0); // Gentle value boost
    } else if (palette == 2) { // Electric Neon (Greens, Yellows, Bright Blues)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.2 + cos(iTime_scaled * 0.07) * 0.1, 1.0), 0.8); // Stronger mix
        hsv.y = clamp(hsv.y * 1.8 + 0.15, 0.0, 1.0); // Higher saturation
        hsv.z = clamp(hsv.z * 1.2, 0.0, 1.0);
    } else if (palette == 3) { // Fiery Abyss (Deep Reds, Oranges, Dark Purples)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.0 + sin(iTime_scaled * 0.03) * 0.05, 1.0), 0.75); // Stronger mix
        hsv.y = clamp(hsv.y * 1.3 + 0.2, 0.0, 1.0); // Moderate saturation boost
        hsv.z = clamp(hsv.z * 0.9, 0.0, 1.0); // Slightly darker overall
    } else if (palette == 4) { // Oceanic Tranquility (Blues, Greens, Aquas)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.4 + cos(iTime_scaled * 0.06) * 0.1, 1.0), 0.7); // Stronger mix
        hsv.y = clamp(hsv.y * 1.4 + 0.1, 0.0, 1.0);
        hsv.z = clamp(hsv.z * 1.05, 0.0, 1.0);
    } else if (palette == 5) { // Galactic Glitch (Muted Pinks, Teals, Grays)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.3 + 0.7 + sin(iTime_scaled * 0.04) * 0.08, 1.0), 0.6);
        hsv.y = clamp(hsv.y * 0.8 + 0.05, 0.0, 1.0); // Slightly desaturated, but keeps initial color info
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 6) { // Rainbow Pulse
        hsv.x = mod(hsv.x + iTime_scaled * 0.1, 1.0); // Fast hue rotation
        hsv.y = clamp(hsv.y * 1.7 + 0.2, 0.0, 1.0); // Very high saturation
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 7) { // Monochrome Echo (Grayscale with subtle hue hints)
        hsv.y = hsv.y * 0.1; // Desaturate heavily
        hsv.x = mod(hsv.x + iTime_scaled * 0.005, 1.0); // Very slow hue shift
        hsv.z = hsv.z * 1.0;
    }
    // Default palette (palette == 0) uses base input color with only general HSV pulsing applied

    return hsv2rgb(hsv);
}

// Applies post-processing effects like brightness, saturation, contrast, and vignette.
vec3 applyPostProcessing(vec3 rgb, vec2 uv_pixel_norm) {
    rgb = rgb * brightness;
    vec3 luma = vec3(dot(vec3(0.2125, 0.7154, 0.0721), rgb));
    rgb = mix(luma, rgb, saturation);
    rgb = mix(vec3(0.5), rgb, contrast);
    
    rgb *= (1.0 - vignetteIntensity) + vignetteIntensity * pow(16.0 * uv_pixel_norm.x * uv_pixel_norm.y * (1.0 - uv_pixel_norm.x) * (1.0 - uv_pixel_norm.y), 0.125);
    
    return sqrt(clamp(rgb, 0.0, 1.0)); // Approximate gamma correction and clamp
}


// --- Main Shader Passes ---

void main() {
    // Calculate total time with animation speed
    float totalTime = TIME * animationSpeed;

    if (PASSINDEX == 0) { // BufferA: Calculates the fractal pattern
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        
        // Normalize UVs to -1 to 1 range and correct aspect ratio
        vec2 uv_norm_aspect = -1.0 + 2.0 * uv;
        uv_norm_aspect.x *= RENDERSIZE.x / RENDERSIZE.y;

        // Apply controlX/Y to shift the view
        uv_norm_aspect += vec2(controlX, controlY) * 0.2;

        vec3 col = vec3(0.);
        float bs = (totalTime + BASE_TIME_OFFSET_BUFFERA) * 0.5;

        // Apply distortion based on control parameters
        float t = totalTime + uv_norm_aspect.x + uv_norm_aspect.y; // Time value for `boxSys` internal distortion
        vec2 gv = fract(uv_norm_aspect) - 0.5; // Tiled UVs
        gv += vec2(sin(t), cos(t)) * distortionStrength; // Apply distortion
        gv = fract(gv * patternScale) - 0.5; // Scale and re-tile

        // Main fractal loop
        float a = rotationSpeed + totalTime * 0.004; // Base rotation speed
        int depth = int(fractalDepth); // Cast to int for loop
        
        vec2 fv = abs(uv_norm_aspect);
        fv *= 0.5; // Initial scale

        for (int i = 0; i < depth; i++) {
            fv = abs(fract(rotate2d((fv * 1.05), a)) - 0.5);
            a *= 1.2; // Accelerate rotation for deeper layers
        }
        
        // Combine multiple box systems for richer patterns
        float bt = totalTime * 0.01; // Base time for boxSys
        col += boxSys(fv * 0.5, bt - 0.001, distortionStrength, patternScale);
        col += boxSys(fv * 0.4, bt - 0.001, distortionStrength, patternScale);

        col *= zoomFactor; // Apply zoom to the color output.

        gl_FragColor = vec4(col, 1.0); // Store vibrant fractal pattern in BufferA
    }
    else if (PASSINDEX == 1) { // Main Image: Applies feedback, palettes, and post-processing
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy; // Normalized pixel coordinates (0-1)
        
        // Normalized UVs with aspect ratio for effects that depend on view-space
        vec2 uv_norm_aspect = -1.0 + 2.0 * uv;
        uv_norm_aspect.x *= RENDERSIZE.x / RENDERSIZE.y;

        // Apply controlX/Y to shift the view for the final image
        uv_norm_aspect += vec2(controlX, controlY) * 0.2;

        // Time for main image effects (different offset than BufferA's internal time)
        float t_main = (totalTime + BASE_TIME_OFFSET_MAIN) * 0.5; 

        // Sample BufferA for main feedback ('bak')
        vec2 tv_feedback = uv;
        tv_feedback -= 0.5; // Center for rotation
        tv_feedback = abs(fract(rotate2d((tv_feedback * 1.5), t_main * 0.01))); // Rotate and tile feedback UV
        tv_feedback *= 1.1;
        tv_feedback += 0.5; // Re-center for sampling
        vec3 bak = IMG_NORM_PIXEL(BufferA, mod(tv_feedback, 1.0)).rgb;

        // Sample BufferA for additional feedback layers (zoom-in/out effects)
        vec2 tv1 = uv - 0.5;
        tv1 *= 1.002; // Slight zoom in
        tv1 += 0.5;
        vec3 bak1 = IMG_NORM_PIXEL(BufferA, mod(tv1, 1.0)).rgb;

        vec2 tv2 = uv - 0.5;
        tv2 *= 0.995; // Slight zoom out
        tv2 += 0.5;
        vec3 bak2 = IMG_NORM_PIXEL(BufferA, mod(tv2, 1.0)).rgb;

        vec3 col = vec3(0.);
        
        // This part recreates the base fractal pattern for the current frame,
        // ensuring new details and colors are always introduced.
        float current_a = rotationSpeed + totalTime * 0.004; // Base rotation speed
        int current_depth = int(fractalDepth); 
        vec2 current_fv = abs(uv_norm_aspect);
        current_fv *= 0.5;

        for (int i = 0; i < current_depth; i++) {
            current_fv = abs(fract(rotate2d((current_fv * 1.05), current_a)) - 0.5);
            current_a *= 1.2;
        }

        float current_bt = totalTime * 0.01;
        vec3 current_col_pattern = vec3(0.);
        current_col_pattern += boxSys(current_fv * 0.5, current_bt - 0.001, distortionStrength, patternScale);
        current_col_pattern += boxSys(current_fv * 0.4, current_bt - 0.001, distortionStrength, patternScale);
        current_col_pattern *= zoomFactor;
        
        // Apply color palette and pulsing *before* mixing with feedback for better effect
        current_col_pattern = applyColorPalette(current_col_pattern, totalTime * colorPulseSpeed);

        // Mix current pattern with feedback from BufferA
        col = mix(current_col_pattern, bak, 0.4); // Blend with main feedback
        
        // Combine zoomed feedback layers
        bak2 = mix(bak1, bak2, 0.5);
        col = mix(col, bak2, 0.5); // Mix the main color with blended feedback layers (reduced strength)
        
        // Apply final post-processing
        gl_FragColor = vec4(applyPostProcessing(col, uv), 1.0);
    }
}