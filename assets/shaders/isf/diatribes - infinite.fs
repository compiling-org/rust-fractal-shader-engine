/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals", "Psychedelic", "Raymarching", "3D", "Tunnel"],
    "DESCRIPTION": "A highly customizable psychedelic abstract shader featuring raymarched forms, dynamic color palettes, and various post-processing effects. Control morphing, zoom, speed, camera, and introduce shake and shimmer effects.",
    "INPUTS": [
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Brightness", "DESCRIPTION": "Overall brightness of the output."},

        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Palette Intensity", "DESCRIPTION": "Overall intensity multiplier for palette colors."},

        {"NAME": "pulseLineIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Line Intensity", "DESCRIPTION": "Strength of the moving line of light effect (overlay)."},
        {"NAME": "pulseLineSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Pulse Line Speed", "DESCRIPTION": "Speed of the moving line of light."},
        {"NAME": "pulseLineThickness", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Pulse Line Thickness", "DESCRIPTION": "Thickness of the moving line of light."},
        {"NAME": "pulseLineDirection", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Pulse Line Direction", "DESCRIPTION": "0=Horizontal, 1=Vertical, 2=Diagonal A, 3=Diagonal B."},

        {"NAME": "enableAutoCamera", "TYPE": "bool", "DEFAULT": true, "LABEL": "Auto Camera Movement", "DESCRIPTION": "Enable or disable automatic camera movement."},
        {"NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Cam Position X", "DESCRIPTION": "Manual camera X position."},
        {"NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Cam Position Y", "DESCRIPTION": "Manual camera Y position."},
        {"NAME": "camZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Cam Position Z", "DESCRIPTION": "Manual camera Z position."},
        {"NAME": "lookAtX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At X", "DESCRIPTION": "Manual look-at X position."},
        {"NAME": "lookAtY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Y", "DESCRIPTION": "Manual look-at Y position."},
        {"NAME": "lookAtZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Z", "DESCRIPTION": "Manual look-at Z position."},
        {"NAME": "zoomFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom", "DESCRIPTION": "Adjusts the field of view (zoom level)."},
        {"NAME": "cameraShiftAmp", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Shift Amount", "DESCRIPTION": "Amplitude of sine wave camera shift (auto mode)."},
        {"NAME": "cameraShiftSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Camera Shift Speed", "DESCRIPTION": "Speed of sine wave camera shift (auto mode)."},
        {"NAME": "cameraRotSpeed", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "LABEL": "Camera Rot Speed", "DESCRIPTION": "Speed of camera rotation."},
        {"NAME": "cameraRotAmount", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rot Amount", "DESCRIPTION": "Amount of camera rotation."},

        {"NAME": "raymarchSteps", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Raymarch Steps", "DESCRIPTION": "Number of raymarching iterations."},
        {"NAME": "minStepDistance", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Min Step Distance", "DESCRIPTION": "Minimum distance for raymarch step."},
        {"NAME": "stepMultiplier", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 1.5, "LABEL": "Step Multiplier", "DESCRIPTION": "Multiplier for raymarch step distance."},
        
        {"NAME": "morphSpeed", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Morph Speed", "DESCRIPTION": "Speed of fractal morphing (Z component offset)."},
        {"NAME": "structureRotationSpeed", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "LABEL": "Structure Rot Speed", "DESCRIPTION": "Speed of the primary structure's rotation."},
        {"NAME": "structureRotationAmount", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Structure Rot Amount", "DESCRIPTION": "Amount of the primary structure's rotation (based on Z)."},
        {"NAME": "tanhModifier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Tanh Modifier", "DESCRIPTION": "Modifier for the tanh function in fractal distance."},
        {"NAME": "densityFactor", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 10.0, "LABEL": "Density Factor", "DESCRIPTION": "Controls density of elements in the noise function (n)."},
        {"NAME": "densityIncrement", "TYPE": "float", "DEFAULT": 1.37, "MIN": 1.1, "MAX": 2.0, "LABEL": "Density Increment", "DESCRIPTION": "Multiplier for density factor in each iteration."},

        {"NAME": "finalColorScaleD", "TYPE": "float", "DEFAULT": 50.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Final Color Scale D", "DESCRIPTION": "Scaling for 'd' component in final color."},
        {"NAME": "finalColorScaleY", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Final Color Scale Y", "DESCRIPTION": "Scaling for 'y' component in final color."},
        {"NAME": "finalColorScaleInvD", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "LABEL": "Final Color Scale Inv D", "DESCRIPTION": "Scaling for inverse 'd' component in final color."},
        {"NAME": "finalColorDivisor", "TYPE": "float", "DEFAULT": 10000.0, "MIN": 1000.0, "MAX": 50000.0, "LABEL": "Final Color Divisor", "DESCRIPTION": "Divisor for the final accumulated color."},
        {"NAME": "vignetteMinLength", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.1, "LABEL": "Vignette Min Length", "DESCRIPTION": "Minimum length for vignette calculation."},

        {"NAME": "psychedelicShimmerStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Shimmer Strength", "DESCRIPTION": "Intensity of the psychedelic shimmer effect."},
        {"NAME": "psychedelicShimmerSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Shimmer Speed", "DESCRIPTION": "Speed of the psychedelic shimmer effect."},
        {"NAME": "psychedelicShimmerScale", "TYPE": "float", "DEFAULT": 50.0, "MIN": 10.0, "MAX": 100.0, "LABEL": "Shimmer Scale", "DESCRIPTION": "Scale/frequency of the shimmer pattern."},

        {"NAME": "screenShakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.05, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of the screen shake effect."},
        {"NAME": "screenShakeSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 30.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of the screen shake effect."},

        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."}
    ]
}
*/

#define PI 3.14159265359
#define TAU 6.28318530718 // 2 * PI

// Global animation time, derived from ISF's TIME and masterSpeed
float time_i;

// Global dynamic palette storage
vec3 dynamicPalette[7]; 

// --- Utility Functions ---
mat2 rot2D(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); } // Standard 2D rotation

// --- Psychedelic Palette Generator ---
vec3 generatePsychedelicColor(float v_in, float time_val, float palette_idx) {
    vec3 col;
    float v = fract(v_in); 
    float anim_v = v + time_val * paletteAnimSpeed;

    if (palette_idx < 0.5) { // Palette 0: "Technicolor Dream" - Vibrant, high contrast primary/secondary
        col = 0.5 + 0.5 * cos(TAU * anim_v * vec3(1.0, 0.5, 0.25) + vec3(0.0, 2.0, 4.0));
        col.r += sin(anim_v * 10.0) * 0.1; 
        col.b -= cos(anim_v * 15.0) * 0.1;
    } else if (palette_idx < 1.5) { // Palette 1: "Neon Abyss" - Deep blues, electric purples, lime greens
        col = 0.5 + 0.5 * sin(TAU * anim_v * vec3(0.8, 1.2, 0.5) + vec3(0.5, 3.5, 1.5));
        col.g = pow(col.g, 1.5); 
        col.r *= 0.8; 
    } else if (palette_idx < 2.5) { // Palette 2: "Molten Galaxy" - Fiery reds, oranges, and deep space blacks/purples
        col = 0.5 + 0.5 * cos(TAU * anim_v * vec3(0.6, 0.3, 0.9) + vec3(1.0, 0.0, 5.0));
        col.b = mix(col.b, 0.0, 0.3); 
        col.r = pow(col.r, 0.8); 
    } else if (palette_idx < 3.5) { // Palette 3: "Mystic Forest Glitch" - Ethereal greens, teals, and unexpected pink/yellow flashes
        col = 0.5 + 0.5 * sin(TAU * anim_v * vec3(0.7, 1.5, 1.0) + vec3(0.2, 4.0, 2.5));
        col.r += sin(anim_v * 20.0) * 0.2; 
        col.b -= cos(anim_v * 25.0) * 0.1;
    } else if (palette_idx < 4.5) { // Palette 4: "Quantum Foam" - Iridescent pastels, glowing and shifting
        col = 0.5 + 0.5 * cos(TAU * anim_v * vec3(0.9, 0.6, 1.1) + vec3(0.8, 1.8, 0.3));
        col = pow(col, vec3(0.9, 1.1, 0.8)); 
    } else if (palette_idx < 5.5) { // Palette 5: "Hypnotic Vortex" - Spiraling purples, greens, and oranges with deep shadows
        col = 0.5 + 0.5 * sin(TAU * anim_v * vec3(1.1, 0.7, 1.3) + vec3(3.0, 1.0, 5.0));
        col.g += cos(anim_v * 12.0) * 0.15; 
        col.b *= 0.7; 
    } else if (palette_idx < 6.5) { // Palette 6: "Chromatic Overload" - Rapid, intense shifts across the entire spectrum
        col = 0.5 + 0.5 * cos(TAU * anim_v * vec3(2.0, 3.0, 4.0) + vec3(0.0, 2.0, 4.0)); 
        col = pow(col, vec3(0.9)); 
        col.r += sin(anim_v * 50.0) * 0.05; 
    } else { // Fallback/default: "Cosmic Glow" (palette_idx >= 6.5)
        col = 0.5 + 0.5 * sin(anim_v * TAU * vec3(0.3, 0.2, 0.4) + vec3(0.0, PI, PI/2.0));
        col.b = pow(col.b, 0.7); 
        col.g = mix(col.g, 0.0, 0.2); 
    }
    return col * paletteIntensity;
}

// Function to get smoothly interpolated palette color
vec3 getPaletteColor(float v_in, float time_val) {
    float scaled_v_in = mod(v_in, 7.0); 
    
    int idx1 = int(floor(scaled_v_in));
    int idx2 = (idx1 + 1) % 7; 

    vec3 color1 = dynamicPalette[idx1];
    vec3 color2 = dynamicPalette[idx2];

    float blend_factor = smoothstep(0.0, 1.0, fract(scaled_v_in));
    
    return mix(color1, color2, blend_factor);
}

// Applies brightness, saturation, and contrast adjustments
vec3 applyColorAdjustments(vec3 col) {
    // Contrast pivot around 0.5
    col = (col - 0.5) * contrast + 0.5; 
    
    // Saturation
    float luma = dot(col, vec3(0.2126, 0.7152, 0.0722)); 
    col = mix(vec3(luma), col, saturation); 
    
    // Brightness
    col *= brightness; 
    return col;
}


void main() {
    time_i = TIME * masterSpeed; // Use TIME and masterSpeed for overall animation control

    // Populate the dynamic palette array
    for (int j = 0; j < 7; j++) {
        dynamicPalette[j] = generatePsychedelicColor(float(j) / 7.0, time_i, paletteSelect);
    }

    vec2 uv_frag = gl_FragCoord.xy;
    vec4 final_color = vec4(0.0);
    bool surfaceHit = false; // To track if a surface was 'hit' for conditional effects

    // Normalized UV coordinates
    vec2 uv = (uv_frag - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    uv /= zoomFactor; // Apply zoom

    // Auto camera movement
    if (enableAutoCamera) {
        uv += vec2(cos(time_i * cameraShiftSpeed) * cameraShiftAmp, sin(time_i * cameraShiftSpeed * 0.6) * cameraShiftAmp * 0.75);
    } else {
        // Manual camera control (apply offset for manual control)
        // This shader's camera model is very simple (just UV distortion + Z offset)
        // so direct camX/Y/Z mapping is tricky. We'll interpret camX/Y as direct UV offsets
        // and camZ as an additional time/morph offset.
        uv += vec2(camX, camY);
        time_i += camZ; // Affects d+t*4. and other time-dependent parts
    }


    float d_total = 0.0; // Total distance raymarched
    float s_current_step_dist = 0.0; // Current step distance
    vec3 p_ray = vec3(0.0); // Point in 3D space along the ray
    vec4 accumulated_output = vec4(0.0); // Accumulates raymarch contributions


    // Raymarch loop
    for(float i = 0.0; i < raymarchSteps; i++) {
        // Calculate current position along the ray
        p_ray = vec3(uv * d_total, d_total + time_i * morphSpeed);

        // Apply rotation to the XY plane based on time and Z-depth
        // Original: p.xy *= mat2(cos(.02*t-p.z*.1+vec4(0,33,11,0)))
        // 33 and 11 are arbitrary offsets, so we'll convert to proper rotation matrix.
        float rot_angle = structureRotationSpeed * time_i - p_ray.z * structureRotationAmount;
        p_ray.xy *= rot2D(rot_angle);

        // Calculate 's' based on tanh and nested loop
        float s_val_accum = tanh(tanhModifier + p_ray.y); // Original: s = tanh(1.+p.y)

        for (float n = densityFactor; n < 16.0; n *= densityIncrement ) { // Original: n = 2.; n < 16.; n *= 1.37
            // Original: s += abs(dot(step(1./d, cos(t+p.z+p*n)), vec3(.4))) / n;
            // Here, 1./d is 1./d_total. cos(t+p.z+p*n) is a complex pattern.
            // Using a palette color for the dot product instead of fixed vec3(.4)
            vec3 palette_influence = getPaletteColor(p_ray.z * 0.05 + time_i * 0.1 + n * 0.01, time_i);
            s_val_accum += abs(dot(step(1.0/d_total, cos(time_i + p_ray.z + p_ray * n)), palette_influence)) / n;
        }
        
        s_current_step_dist = minStepDistance + abs(s_val_accum) * stepMultiplier; // Original: .001+abs(s)*.8
        
        // Accumulate color. Original: o += 1./s
        // We'll scale this and apply palette for trippy colors.
        vec3 ray_color = getPaletteColor(d_total * 0.05 + s_val_accum * 0.1, time_i);
        accumulated_output.rgb += ray_color / s_current_step_dist;

        d_total += s_current_step_dist;

        if (d_total > 500.0) { // Arbitrary far clipping for performance
            surfaceHit = true; // Consider it a "hit" if it traveled far enough
            break;
        }
    }

    // Final color calculation: tanh(vec4(d/5e1,.2, 1e1/d,0)*o / 1e4 / max(length(u), .01));
    // The `vec4(d/5e1,.2, 1e1/d,0)` term acts as a color multiplier based on distance.
    vec4 distance_color_multiplier = vec4(d_total / finalColorScaleD, finalColorScaleY, finalColorScaleInvD / d_total, 0.0);
    final_color = tanh(distance_color_multiplier * accumulated_output / finalColorDivisor / max(length(uv), vignetteMinLength));
    
    final_color.a = 1.0; // Ensure alpha is 1.0

    // --- Psychedelic Shimmer Pattern ---
    // Applies to the whole image as per original context.
    if (psychedelicShimmerStrength > 0.001) {
        float shimmer_noise_x = sin(uv_frag.x * psychedelicShimmerScale + time_i * psychedelicShimmerSpeed * 0.7);
        float shimmer_noise_y = cos(uv_frag.y * psychedelicShimmerScale * 1.2 + time_i * psychedelicShimmerSpeed * 1.1);
        float shimmer_val = fract(shimmer_noise_x + shimmer_noise_y) * 0.5 + 0.5; // Range 0 to 1
        
        // Generate a shimmer color using the palette, varying with shimmer_val
        vec3 shimmer_color = getPaletteColor(shimmer_val * 3.0 + time_i * 0.5, time_i); 
        final_color.rgb = mix(final_color.rgb, shimmer_color, psychedelicShimmerStrength * 0.25); 
    }

    // --- Screen Shake Effect ---
    // Applied by subtly offsetting final color or adding noise.
    if (screenShakeAmount > 0.001) {
        float shake_offset_x = sin(time_i * screenShakeSpeed * 15.0) * cos(time_i * screenShakeSpeed * 10.0) * screenShakeAmount;
        float shake_offset_y = cos(time_i * screenShakeSpeed * 12.0) * sin(time_i * screenShakeSpeed * 18.0) * screenShakeAmount;
        final_color.rgb += vec3(shake_offset_x, shake_offset_y, 0.0) * 0.5; // Add subtle color shift
    }

    // --- Color Pulse (Moving Line of Light - independent overlay) ---
    if (pulseLineIntensity > 0.001) {
        float pulse_coord;
        if (pulseLineDirection < 0.5) { // Horizontal
            pulse_coord = uv_frag.y / RENDERSIZE.y;
        } else if (pulseLineDirection < 1.5) { // Vertical
            pulse_coord = uv_frag.x / RENDERSIZE.x;
        } else if (pulseLineDirection < 2.5) { // Diagonal A (bottom-left to top-right)
            pulse_coord = (uv_frag.x + uv_frag.y) / (RENDERSIZE.x + RENDERSIZE.y); 
        } else { // Diagonal B (top-left to bottom-right)
            pulse_coord = (uv_frag.x - uv_frag.y) / (RENDERSIZE.x + RENDERSIZE.y);
        }

        float pulse_wave = sin(pulse_coord * 20.0 + time_i * pulseLineSpeed * 5.0);
        float pulse_alpha = smoothstep(0.5 - pulseLineThickness, 0.5 + pulseLineThickness, fract(pulse_wave * 0.5 + 0.5));
        
        vec3 pulse_color = getPaletteColor(pulse_coord * 5.0 + time_i * 0.8, time_i); // Use palette for pulse color
        final_color.rgb = mix(final_color.rgb, pulse_color, pulse_alpha * pulseLineIntensity);
    }
    
    // Final color adjustments
    final_color.rgb = applyColorAdjustments(final_color.rgb);
    final_color.rgb *= globalBrightness;

    gl_FragColor = final_color;
}