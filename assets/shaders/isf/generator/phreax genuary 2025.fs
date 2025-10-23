/*
{
  "DESCRIPTION": "Fractal Tunnel - Trippy animated tunnel with color pulses and morphing, now with refined camera, geometry, and morphing controls. Parameters streamlined to focus on key visual aspects while preserving original fractal aesthetics.",
  "CATEGORIES": ["Fractal", "Tunnel", "Psychedelic", "Tunable", "Shadertoy Conversion"],
  "INPUTS": [
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 10.0,
      "DEFAULT": 0.4,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "OverallRotationSpeed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 10.0,
      "DEFAULT": 0.15,
      "LABEL": "Overall Rotation Speed"
    },
    {
      "NAME": "DistortionStrength",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 5.0,
      "DEFAULT": 0.8,
      "LABEL": "Distortion Strength"
    },
    {
      "NAME": "ColorPulseFreq",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 20.0,
      "DEFAULT": 0.4,
      "LABEL": "Color Pulse Freq"
    },
    {
      "NAME": "ColorPulseAmp",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5,
      "LABEL": "Color Pulse Amp"
    },
    {
      "NAME": "FlickerAmount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "LABEL": "Grain/Flicker"
    },
    {
      "NAME": "GammaAdjust",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 4.0,
      "DEFAULT": 2.0,
      "LABEL": "Gamma Correction"
    },
    { "NAME": "BaseColor1", "TYPE": "color", "DEFAULT": [0.2, 0.7, 1.0, 1.0], "LABEL": "Base Color 1 (legacy)" }, 
    { "NAME": "BaseColor2", "TYPE": "color", "DEFAULT": [1.0, 0.3, 0.8, 1.0], "LABEL": "Base Color 2 (legacy)" }, 
    { "NAME": "PaletteBlend", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Intensity" }, 
    { "NAME": "GlobalBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Global Brightness" },

    { "NAME": "CameraZOffset", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": 0.0, "LABEL": "Camera Z Offset" },
    { "NAME": "CameraZAnimationSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.35, "LABEL": "Camera Z Anim Speed" },
    { "NAME": "CameraDepthEffect", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Camera Depth Effect" },
    { "NAME": "CameraDepthPulseFreq", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.25, "LABEL": "Camera Depth Pulse Freq" },
    { "NAME": "CameraDepthPulseAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.9, "LABEL": "Camera Depth Pulse Amp" },
    { "NAME": "CameraXYOffset", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "LABEL": "Camera XY Offset" },

    { "NAME": "FractalIterations", "TYPE": "float", "MIN": 1.0, "MAX": 100.0, "DEFAULT": 100.0, "LABEL": "Fractal Iterations" },
    { "NAME": "FractalInitialScaleMorphFactor", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Initial Scale/Morph" },
    { "NAME": "FractalFoldingAnimationStrength", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.03, "LABEL": "Folding Anim Strength" },
    { "NAME": "FractalInnerMorphStrength", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Inner Morph Strength" },
    { "NAME": "FractalIterationScaleFactor", "TYPE": "float", "MIN": 0.5, "MAX": 5.0, "DEFAULT": 2.5, "LABEL": "Iteration Scale Factor" },
    { "NAME": "FractalIterationClampMin", "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 1.0, "LABEL": "Iteration Clamp Min" },
    { "NAME": "FractalIterationClampMax", "TYPE": "float", "MIN": 1.0, "MAX": 5.0, "DEFAULT": 5.0, "LABEL": "Iteration Clamp Max" },
    { "NAME": "SurfaceHitThreshold", "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 0.10, "LABEL": "Surface Hit Threshold" },
    { "NAME": "ColorFadeFactor", "TYPE": "float", "MIN": 0.001, "MAX": 0.1, "DEFAULT": 0.02, "LABEL": "Color Fade Factor" },

    { "NAME": "FlashFreqRay", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 5.0, "LABEL": "Flash Freq (Ray)" },
    { "NAME": "FlashAmpRay", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Flash Amp (Ray)" },
    { "NAME": "FlashFreqIteration", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 20.0, "LABEL": "Flash Freq (Iter)" },
    { "NAME": "FlashAmpIteration", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Flash Amp (Iter)" }
  ],
  "ISFVSN": "2"
}
*/

precision highp float; // Set high precision for floats

#define PI 3.141592
#define TAU PI*2.0 // Ensure float literal

// A deeply psychedelic and trippy palette function
vec3 trippyPalette(float t_val, float time_val, float iter_val) {
    // Aggressively vary hue, saturation, and value across distance, time, and iteration
    // Using high frequencies and complex mixing for chaotic, unpredictable patterns
    float hue_base = mod(t_val * 0.7 + time_val * 0.6 + iter_val * 0.25, 1.0);
    float hue_shift1 = mod(t_val * 1.1 + time_val * 0.9 + iter_val * 0.35 + 0.33, 1.0);
    float hue_shift2 = mod(t_val * 1.5 + time_val * 1.2 + iter_val * 0.45 + 0.66, 1.0);

    // Dynamic saturation and value, pulsating very intensely and with abrupt, hard shifts
    // Using `fract` on high-frequency sin/cos for a 'glitchy', snapping color transition
    float sat_pulse = 0.3 + 0.7 * fract(sin(time_val * 15.0 + t_val * 0.8) * 500.0); // Extreme multiplier for fract for very sharp transitions
    float val_pulse = 0.1 + 0.9 * fract(cos(time_val * 13.0 + iter_val * 0.6) * 500.0); // Extreme multiplier for fract for very sharp transitions

    // Generate base colors from multiple points in the spectrum using high frequencies
    vec3 colA = (cos(hue_base * TAU * 1.5 + vec3(0.0, 2.0, 4.0)) * 0.5 + 0.5);
    vec3 colB = (cos(hue_shift1 * TAU * 1.5 + vec3(1.0, 3.0, 5.0)) * 0.5 + 0.5);
    vec3 colC = (cos(hue_shift2 * TAU * 1.5 + vec3(0.5, 2.5, 4.5)) * 0.5 + 0.5);

    // Blend them chaotically using multiple high-frequency sinusoidal factors
    float blendFactor1 = (0.5 + 0.5 * sin(t_val * 10.0 + time_val * 9.0 + iter_val * 2.0));
    float blendFactor2 = (0.5 + 0.5 * cos(t_val * 12.0 + time_val * 11.0 + iter_val * 2.5 + PI));

    vec3 final_color = mix(mix(colA, colB, blendFactor1), colC, blendFactor2);
    
    // Apply aggressive pulsating saturation and value
    return final_color * sat_pulse * val_pulse;
}

#define S(a, b, x) smoothstep(a, b, x)
#define rot(x) mat2(cos(x), -sin(x), sin(x), cos(x))

// Global variable for scaled time
float scaledTime;

void main() {
    // Initialize global scaledTime at the start of main
    scaledTime = TIME * AnimationSpeed;
    scaledTime = mod(scaledTime, 8.0); // Modulate after initial scaling
    
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.x; 
    
    // Camera Position (ro) and Direction (rd) now use new tunable parameters
    vec3 ro = vec3(CameraXYOffset.x, CameraXYOffset.y, CameraZOffset + scaledTime * CameraZAnimationSpeed);
    vec3 rd = normalize(vec3(uv, CameraDepthEffect - CameraDepthPulseAmplitude * (0.5+0.5*sin(PI + CameraDepthPulseFreq * scaledTime)))); 
    
    vec3 lp = vec3(0.0, 0.0, 2.0); // Light position, still fixed for now.
    
    vec3 col_final = vec3(0.0); // Initialize final accumulated color to black
    float t = 0.0; // Total raymarch distance
    
    vec3 p_ray = ro; // Current point along the ray
    vec3 p_prev = ro; // Store previous point for normal approximation
    vec3 hit_position = ro; // Store the actual hit position or final ray position
    vec3 surface_normal = vec3(0.0); // Store the calculated surface normal

    // Main Raymarching Loop
    // Iterates up to a fixed maximum (e.g., 200) for GLSL ES 1.00 compatibility.
    // FractalIterations uniform acts as a soft limit to control actual rendered complexity.
    for(int i_ray_int = 0; i_ray_int < 200; i_ray_int++) { // Use fixed int for loop, then cast to float for calculations
        float i_ray = float(i_ray_int); // Convert to float for calculations

        if(i_ray >= FractalIterations) break; // Soft limit for tunable iterations

        p_prev = p_ray; // Save current point before advancing p_ray
        vec3 p_transformed_current_iter = p_ray; // Point for fractal transformation in this iter
        
        // --- Fractal Transformation Logic (copied directly from original shader's core) ---
        float len = mix(0.5 * (abs(p_transformed_current_iter.x) + abs(p_transformed_current_iter.y)), length(p_transformed_current_iter.xz), 0.4); 
        p_transformed_current_iter.zy += vec2(0.2, 0.1) * sin(0.2 * scaledTime + len) * DistortionStrength;
        p_transformed_current_iter.xy *= mix(1.2, 0.5, (0.5+0.5*sin(len * FractalInitialScaleMorphFactor * 5.0 + FractalInitialScaleMorphFactor * 0.5 * scaledTime))); 
        
        vec3 fold_offset1 = vec3(0.1 + FractalFoldingAnimationStrength * S(3.0, 7.0, scaledTime),
                                 0.4 - FractalFoldingAnimationStrength * S(3.0, 7.0, scaledTime),
                                 1.9);
        vec3 fold_offset2 = vec3(1.0, 1.2, 1.08 + FractalFoldingAnimationStrength * sin(scaledTime));
        p_transformed_current_iter = abs(p_transformed_current_iter - fold_offset1) - fold_offset2;

        float s_accumulated_scale = 2.7; // This 's' accumulates fractal scaling factor
        float d_inner_temp_scale = 0.0; // Temporary 'd' used as a scaling factor in inner loop
        
        // Inner fractal iteration loop (hardcoded 10 times, also with soft limit)
        for(int j_loop_int = 0; j_loop_int < 10; j_loop_int++) { 
            float j_loop = float(j_loop_int); // Convert to float

            if (j_loop >= FractalIterations) break; // Soft limit for inner loop as well (using same uniform for simplicity)

            p_transformed_current_iter.xy *= mix(1.05, 0.95, (0.5+0.5*sin(length(p_transformed_current_iter.xy) * FractalInnerMorphStrength * 0.5 + FractalInnerMorphStrength * 0.1 * scaledTime)));
            
            vec3 inner_fold_offset = vec3(1.2, 1.3, 1.08 + 0.04 * sin(scaledTime));
            p_transformed_current_iter = abs(p_transformed_current_iter - inner_fold_offset) - inner_fold_offset; 

            // 'd_inner_temp_scale' is used as a scaling factor here, not a raymarching distance
            d_inner_temp_scale = FractalIterationScaleFactor / clamp(dot(p_transformed_current_iter, p_transformed_current_iter), FractalIterationClampMin, FractalIterationClampMax); 
            s_accumulated_scale *= d_inner_temp_scale; 
            p_transformed_current_iter = abs(p_transformed_current_iter) * d_inner_temp_scale; 
        }

        p_transformed_current_iter.xy *= rot(0.5 * PI * sin(scaledTime * OverallRotationSpeed));

        // --- Raymarching Step Calculation ---
        // Simplified distance calculation to ensure basic visibility
        float d_step_size = length(p_transformed_current_iter.xz) / s_accumulated_scale; 

        // Check for hit or max raymarch distance.
        if(d_step_size < 0.001) { // Hit detected (very close to surface)
            hit_position = p_ray; // Store the current world position where hit was detected
            surface_normal = normalize(p_ray - p_prev); // Approximate normal at hit point
            
            // Color contribution at hit point based on current point and accumulated iterations
            vec3 psychedelic_color = trippyPalette(t, scaledTime, i_ray); 

            // Aggressive Flashing like rays tracing on top of the fractal
            float flash_intensity = (0.5+0.5*sin(t * FlashFreqRay * 7.0 + scaledTime * 70.0)) * FlashAmpRay * 7.0; 
            flash_intensity += (0.5+0.5*sin(i_ray * FlashFreqIteration * 7.0 + scaledTime * 140.0)) * FlashAmpIteration * 10.0; 
            flash_intensity = clamp(flash_intensity, 0.0, 1.0); 

            // Final color contribution is now purely psychedelic, scaled by PaletteBlend
            vec3 current_iteration_color = psychedelic_color * PaletteBlend; 
            
            // Accumulate color with tunable fade factor and very strong flash intensity
            col_final += current_iteration_color * (ColorFadeFactor * exp(-0.7 * i_ray * i_ray * d_step_size) * (1.0 + flash_intensity * 10.0)); 

            break; // Break on hit for a clear surface
        }
        
        t += d_step_size; // Advance ray by the calculated step size
        p_ray += rd * d_step_size; // Update p_ray to the new position
        
        // If raymarch distance exceeds maximum, stop and assume no hit
        if (t > 100.0) { 
            hit_position = p_ray; // Store final position if max distance reached
            break;
        }
    }
    
    // Fallback if no hit occurred within max iterations/distance (dark background)
    if(t >= 100.0) { 
        vec3 fog_color = trippyPalette(t * 0.05, scaledTime, 0.0) * 0.1 * PaletteBlend; 
        col_final = mix(col_final, fog_color, 1.0); 
    }


    // Standard normal calculation and lighting
    if (dot(surface_normal, surface_normal) == 0.0) { 
        surface_normal = normalize(hit_position - ro); 
    }

    // Apply lighting only if there was some accumulation of color
    if (length(col_final) > 0.001) { 

        vec3 l = normalize(lp - hit_position); 
        float dif = max(dot(surface_normal, l), 0.0); 
        float spe = pow(max(dot(reflect(-l, surface_normal), -rd), 0.0), 40.0); 
        
        float ao_approx = smoothstep(0.0, 1.0, clamp(t / 50.0, 0.0, 1.0)); 
        
        vec3 al_base = trippyPalette(scaledTime * 0.4, scaledTime, 0.0) * PaletteBlend * 2.5; 
        vec3 al = al_base;

        col_final = col_final * al * mix(1.0, spe + 1.2 * dif, 0.6); 
        col_final = mix(col_final, col_final * ao_approx, 0.6); 
        
        float fog = 1.0 - exp(-t * 0.05); 
        vec3 fog_color_dynamic = trippyPalette(t * 0.04, scaledTime, 0.0) * 0.4 * PaletteBlend; 
        col_final = mix(col_final, fog_color_dynamic, fog); 
    }

    // Apply FlickerAmount
    float grain = fract(sin(gl_FragCoord.x * 12.9898 + gl_FragCoord.y * 75.5914 + scaledTime * 2.0)) * FlickerAmount;
    col_final += grain;

    // The global Color Pulse is applied as a final effect
    float pulse_factor = 1.0 + ColorPulseAmp * (0.5+0.5*sin(scaledTime * ColorPulseFreq * 2.0)); 
    col_final *= pulse_factor;

    // Apply Gamma Adjustment
    col_final = pow(col_final, vec3(1.0 / GammaAdjust));

    // Apply Global Brightness
    col_final *= GlobalBrightness;

    gl_FragColor = vec4(col_final, 1.0); 
}
