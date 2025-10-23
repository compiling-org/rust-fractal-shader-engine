/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals", "Psychedelic", "Raymarching", "3D", "Tunnel"],
    "DESCRIPTION": "A dynamic raymarched fractal tunnel with complex movements and psychedelic effects. Features advanced camera control, 7 distinct color palettes, and extensive post-processing.",
    "INPUTS": [
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalGlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Glow", "DESCRIPTION": "Controls the overall intensity of glowing effects."},

        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Palette Brightness", "DESCRIPTION": "Overall brightness multiplier for palette colors."},
        {"NAME": "fractalGlowStrength", "TYPE": "float", "DEFAULT": 0.075, "MIN": 0.0, "MAX": 0.5, "LABEL": "Fractal Glow Strength", "DESCRIPTION": "Intensity of the fractal's self-illuminating glow."},

        {"NAME": "pulseLineIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Line Intensity", "DESCRIPTION": "Strength of the moving line of light effect (overlay)."},
        {"NAME": "pulseLineSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Pulse Line Speed", "DESCRIPTION": "Speed of the moving line of light."},
        {"NAME": "pulseLineThickness", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Pulse Line Thickness", "DESCRIPTION": "Thickness of the moving line of light."},
        {"NAME": "pulseLineDirection", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Pulse Line Direction", "DESCRIPTION": "0=Horizontal, 1=Vertical, 2=Diagonal A, 3=Diagonal B."},

        {"NAME": "enableAutoCameraMovement", "TYPE": "bool", "DEFAULT": true, "LABEL": "Auto Camera Movement", "DESCRIPTION": "Enable the original time-based camera animation."},
        {"NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position X", "DESCRIPTION": "X-coordinate of the camera position (if auto disabled)."},
        {"NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position Y", "DESCRIPTION": "Y-coordinate of the camera position (if auto disabled)."},
        {"NAME": "camZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position Z", "DESCRIPTION": "Z-coordinate of the camera position (if auto disabled)."},
        {"NAME": "lookAtX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At X", "DESCRIPTION": "X-coordinate of the point the camera is looking at (if auto disabled)."},
        {"NAME": "lookAtY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Y", "DESCRIPTION": "Y-coordinate of the point the camera is looking at (if auto disabled)."},
        {"NAME": "lookAtZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Z", "DESCRIPTION": "Z-coordinate of the point the camera is looking at (if auto disabled)."},
        {"NAME": "camFOV", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Camera FOV (Zoom)", "DESCRIPTION": "Field of View for the camera (smaller value means wider FOV/more zoom out)."},
        {"NAME": "autoCamSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Auto Cam Speed", "DESCRIPTION": "Speed multiplier for automatic camera movement functions."},
        {"NAME": "P_X_CosFactor", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.1, "MAX": 1.0, "LABEL": "P_X_Cos Factor", "DESCRIPTION": "Cosine frequency for P function X component."},
        {"NAME": "P_X_Amp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "P_X_Amplitude", "DESCRIPTION": "Amplitude multiplier for P function X component."},
        {"NAME": "P_X_Range", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "P_X_Range", "DESCRIPTION": "Overall range for P function X component."},
        {"NAME": "P_Y_CosFactor", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0, "LABEL": "P_Y_Cos Factor", "DESCRIPTION": "Cosine frequency for P function Y component."},
        {"NAME": "P_Y_Amp", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.1, "MAX": 2.0, "LABEL": "P_Y_Amplitude", "DESCRIPTION": "Amplitude multiplier for P function Y component."},
        {"NAME": "P_Y_Range", "TYPE": "float", "DEFAULT": 16.0, "MIN": 1.0, "MAX": 30.0, "LABEL": "P_Y_Range", "DESCRIPTION": "Overall range for P function Y component."},
        {"NAME": "cameraRotSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Speed", "DESCRIPTION": "Speed of camera's self-rotation (D vector)."},
        {"NAME": "cameraRotAmount", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Amount", "DESCRIPTION": "Amount of camera's self-rotation (D vector)."},
        {"NAME": "cameraRotZOffsetSpeed", "TYPE": "float", "DEFAULT": 22.0, "MIN": 0.0, "MAX": 50.0, "LABEL": "Camera Rot Z Offset Speed", "DESCRIPTION": "Speed of Z-based rotation offset."},
        {"NAME": "cameraRotZOffsetAmount", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Camera Rot Z Offset Amount", "DESCRIPTION": "Amount of Z-based rotation offset."},

        {"NAME": "orbAmplitudeX", "TYPE": "float", "DEFAULT": 2.6, "MIN": 0.1, "MAX": 5.0, "LABEL": "Orb Amp X", "DESCRIPTION": "Amplitude of X displacement in orb function."},
        {"NAME": "orbAmplitudeY", "TYPE": "float", "DEFAULT": 2.125, "MIN": 0.1, "MAX": 5.0, "LABEL": "Orb Amp Y", "DESCRIPTION": "Amplitude of Y displacement in orb function."},
        {"NAME": "orbSpeedX", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Orb Speed X", "DESCRIPTION": "Speed of X displacement in orb function."},
        {"NAME": "orbSpeedY", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Orb Speed Y", "DESCRIPTION": "Speed of Y displacement in orb function."},
        {"NAME": "orbTanCosAmplitude", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Orb Tan Cos Amp", "DESCRIPTION": "Amplitude of tan/cos displacement in orb function."},
        {"NAME": "orbTanCosSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.1, "MAX": 1.0, "LABEL": "Orb Tan Cos Speed", "DESCRIPTION": "Speed of tan/cos displacement in orb function."},

        {"NAME": "tunnelRaymarchSteps", "TYPE": "float", "DEFAULT": 60.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Tunnel Steps", "DESCRIPTION": "Max steps for tunnel raymarching."},
        {"NAME": "fractalRaymarchSteps", "TYPE": "float", "DEFAULT": 60.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Fractal Steps", "DESCRIPTION": "Max steps for fractal raymarching."},
        {"NAME": "raymarchMinDist", "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Raymarch Min Dist", "DESCRIPTION": "Minimum distance for raymarching precision."},
        {"NAME": "raymarchMaxDist", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 500.0, "LABEL": "Raymarch Max Dist", "DESCRIPTION": "Maximum distance the ray will travel."},
        {"NAME": "tunnelDensityOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tunnel Density Offset", "DESCRIPTION": "Offset for tunnel density."},
        {"NAME": "tunnelDensityFactor", "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tunnel Density Factor", "DESCRIPTION": "Factor for tunnel density."},
        {"NAME": "tunnelDensitySpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Tunnel Density Speed", "DESCRIPTION": "Speed for tunnel density oscillation."},
        
        {"NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 9.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations", "DESCRIPTION": "Number of iterations for the fractal pattern."},
        {"NAME": "fractalAmp", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Amp", "DESCRIPTION": "Amplitude of the fractal's recursive scaling."},
        {"NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Offset", "DESCRIPTION": "Offset applied to abs(sin(p)) in fractal iteration."},
        {"NAME": "flashingLightIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Flashing Light Intensity", "DESCRIPTION": "Intensity of the psychedelic flashing light effect."},
        {"NAME": "flashingLightSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Flashing Light Speed", "DESCRIPTION": "Speed of the primary flashing light component."},
        {"NAME": "flashingLightMixAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Flashing Light Mix", "DESCRIPTION": "Mix amount for the flashing light effect."},
        {"NAME": "flashingLightDotVec", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Flashing Light Dot Vec", "DESCRIPTION": "Vector dot product scale for flashing light."},

        {"NAME": "shimmerStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Shimmer Strength", "DESCRIPTION": "Intensity of the psychedelic shimmer overlay effect."},
        {"NAME": "shimmerSpeed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "LABEL": "Shimmer Speed", "DESCRIPTION": "Speed of the psychedelic shimmer effect."},

        {"NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of the screen shake effect."},
        {"NAME": "shakeSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 30.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of the screen shake effect."},

        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."},
        {"NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Strength", "DESCRIPTION": "Strength of the dark vignette effect at the edges."},
        {"NAME": "expFactor", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Exp Falloff Factor", "DESCRIPTION": "Exponential falloff for distance-based darkening."}
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
    return col * paletteBrightness;
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


// P function defines the path/motion of the tunnel and camera
vec3 P(float z) {
    return vec3(tanh(cos(z * P_X_CosFactor) * P_X_Amp) * P_X_Range,
                tanh(cos(z * P_Y_CosFactor) * P_Y_Amp) * P_Y_Range, 
                z);
}

// Orb function (distance to a moving sphere/point)
float orb(vec3 p_in) {
    float t_orb = time_i * 2.0; // Orb's internal time scale
    return length(p_in - vec3(
                P(p_in.z).x + tanh(sin(p_in.z * 0.1 + t_orb * orbSpeedX) * 0.75) * orbAmplitudeX,
                P(p_in.z).y + sin(sin(p_in.z * 0.5) + t_orb * orbSpeedY) * orbAmplitudeY,
                time_i + tan(cos(t_orb * orbTanCosSpeed) * orbTanCosAmplitude) * orbTanCosAmplitude));
}

// --- Post-processing functions ---
vec3 applyShimmer(vec3 col, vec2 fragCoordUV, float time_val) {
    if (shimmerStrength > 0.001) {
        float shimmer_noise_x = sin(fragCoordUV.x * 50.0 + time_val * shimmerSpeed * 0.7);
        float shimmer_noise_y = cos(fragCoordUV.y * 70.0 + time_val * shimmerSpeed * 1.1);
        float shimmer_val = fract(shimmer_noise_x + shimmer_noise_y) * 0.5 + 0.5; 
        
        vec3 shimmer_color = generatePsychedelicColor(shimmer_val, time_val, paletteSelect + 0.5); 
        col += shimmer_color * shimmerStrength * 0.2; 
    }
    return col;
}

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


// Main rendering function
void main() {
    // Populate the dynamic palette array
    time_i = TIME * masterSpeed * 2.5; // Original T was iTime * 2.5
    for (int j = 0; j < 7; j++) {
        dynamicPalette[j] = generatePsychedelicColor(float(j) / 7.0, time_i, paletteSelect);
    }

    vec2 uv_frag = gl_FragCoord.xy;

    // --- Screen Shake Effect ---
    if (shakeAmount > 0.001) {
        float shake_x = sin(time_i * shakeSpeed * 15.0) * cos(time_i * shakeSpeed * 10.0) * shakeAmount * RENDERSIZE.y;
        float shake_y = cos(time_i * shakeSpeed * 12.0) * sin(time_i * shakeSpeed * 18.0) * shakeAmount * RENDERSIZE.y;
        uv_frag += vec2(shake_x, shake_y);
    }

    // Normalized UV coordinates
    vec2 uv = (uv_frag - RENDERSIZE.xy / 2.0) / RENDERSIZE.y;
    
    float s_dist = raymarchMinDist; // Current step distance
    float d_total = 0.0;             // Total distance raymarched
    
    // Camera Setup
    vec3 ro; // Ray Origin
    vec3 ta; // Target (Look At)

    if (enableAutoCameraMovement) {
        // Original time-based camera movement P(T)
        ro = P(time_i * autoCamSpeed);
        // Look ahead along the path
        ta = P(time_i * autoCamSpeed + 1.0); // P(T+1)
    } else {
        // User-controlled camera
        ro = vec3(camX, camY, camZ);
        ta = vec3(lookAtX, lookAtY, lookAtZ);
    }

    // Camera Basis Vectors
    vec3 Z_axis = normalize(ta - ro); // Z-axis (forward)
    // Avoid cross product with collinear vectors if ta-ro is nearly vertical
    vec3 up_vec = vec3(0.0, 1.0, 0.0);
    if (abs(dot(Z_axis, up_vec)) > 0.999) { // If Z_axis is almost vertical
        up_vec = vec3(0.0, 0.0, 1.0); // Use Z-axis as up for cross product
    }
    vec3 X_axis = normalize(cross(up_vec, Z_axis)); // X-axis (right)
    vec3 Y_axis = normalize(cross(Z_axis, X_axis)); // Y-axis (up)
    
    // Ray Direction (D) calculation
    // Original: `D = vec3(rot(sin(T*.3)*.6)*rot(tanh(sin(p.z*.1)*22.)*3.)*(u-r.xy/2.)/r.y, 1) * .7 * mat3(-X, cross(X, Z), Z);`
    // Expanded:
    float rot_angle1 = sin(time_i * cameraRotSpeed) * cameraRotAmount;
    // p.z in original D calculation is ro.z here
    float rot_angle2 = tanh(sin(ro.z * 0.1) * cameraRotZOffsetSpeed) * cameraRotZOffsetAmount;
    
    mat2 combined_rot_mat = rot2D(rot_angle1) * rot2D(rot_angle2);
    
    vec3 D_ray = normalize(vec3(combined_rot_mat * uv, camFOV)) * 0.7 * mat3(-X_axis, cross(X_axis, Z_axis), Z_axis);

    vec4 final_color = vec4(0.0);
    vec3 current_pos = ro; 

    // --- First Raymarch Loop (Tunnel like shape) ---
    // Original: `s = (.5+tanh(sin(T*.3)*.5)*.75)*1.5 - length(p.xy - P(p.z).xy), d += s;`
    for(int i = 0; i < int(tunnelRaymarchSteps); i++) {
        current_pos = ro + D_ray * d_total;
        
        // Calculate dynamic density for the tunnel
        float dynamic_density = (tunnelDensityOffset + tanh(sin(time_i * tunnelDensitySpeed) * 0.5) * tunnelDensityFactor) * 1.5;
        s_dist = dynamic_density - length(current_pos.xy - P(current_pos.z).xy);
        
        if (s_dist < raymarchMinDist || d_total > raymarchMaxDist) break;
        d_total += s_dist;
    }
    // `d_total` now holds the distance to the "tunnel" like shape or max distance.
    // The original shader then assigns `s=.002, ro=p=ro+D*d, d=0;` for the second pass.
    
    // --- Second Raymarch Loop (Fractal Glow Accumulation) ---
    float d_fractal = 0.0;
    // Reset ray origin to the hit point from the first loop
    // If the first loop didn't hit, d_total will be max distance, so current_pos will be at max distance.
    vec3 ro_fractal = ro + D_ray * d_total; 
    
    for(int i = 0; i < int(fractalRaymarchSteps); i++) {
        vec3 p_fractal = ro_fractal + D_ray * d_fractal;
        
        // Fractal deformation: p.xy -= P(p.z).xy;
        p_fractal.xy -= P(p_fractal.z).xy;
        
        float w_fractal = 0.1; // Original hardcoded w = .1
        float l_fractal;
        
        // Fractal iteration loop (9 iterations originally)
        for (int j = 0; j < int(fractalIterations); j++) {
            p_fractal = abs(sin(p_fractal)) - fractalOffset; // Tunable fractalOffset
            l_fractal = fractalAmp / dot(p_fractal, p_fractal); // Tunable fractalAmp
            p_fractal *= l_fractal; 
            w_fractal *= l_fractal;
        }
        
        // Accumulate glow based on fractal properties and time
        // `o.rgb += tanh(T+p*4.)*.075+.075;`
        final_color.rgb += tanh(time_i + p_fractal * 4.0) * fractalGlowStrength + fractalGlowStrength;
        
        s_dist = length(p_fractal) / w_fractal; // Distance to fractal
        
        if (s_dist < raymarchMinDist || d_fractal > raymarchMaxDist) break;
        d_fractal += s_dist;
    }
    // `p` in the original shader, after loops, corresponds to `ro_fractal + D_ray * d_fractal`
    vec3 final_fractal_pos = ro_fractal + D_ray * d_fractal;

    // --- Post-processing and Coloring ---
    final_color.rgb = 1.0 - final_color.rgb; // Invert the accumulated glow
    
    // Base color mix
    // Original: `o.rgb *= vec3(.5,.1,.75)+sin(p);`
    vec3 base_color_palette = getPaletteColor(final_fractal_pos.z * 0.1 + time_i * 0.01, time_i);
    final_color.rgb *= (base_color_palette + sin(final_fractal_pos)); // Blend with palette and sin wave
    
    // Flashing light effect (optional, original was commented out with "if you don't like the flashing light")
    // Original: `o.rgb = mix(o.rgb,abs(sin(p) / dot(sin(T+p*2.)*f,vec3(4.))), .5);`
    if (flashingLightIntensity > 0.001) {
        float f_light = abs(tanh(cos(time_i * flashingLightSpeed * 1.0) * 23.0)) +
                        sin(time_i * flashingLightSpeed * 2.0) + cos(time_i * flashingLightSpeed * 1.0) + sin(time_i * flashingLightSpeed * 0.5) *
                        sin(time_i * flashingLightSpeed * 0.001);
        
        vec3 flashing_color = abs(sin(final_fractal_pos) / dot(sin(time_i + final_fractal_pos * 2.0) * f_light, vec3(flashingLightDotVec))) * flashingLightIntensity;
        final_color.rgb = mix(final_color.rgb, flashing_color, flashingLightMixAmount);
    }
    
    // Original `pow(o.rgb, vec3(2.));`
    final_color.rgb = pow(final_color.rgb, vec3(2.0));
    
    // Apply orb inverse square falloff
    float orb_dist = orb(final_fractal_pos);
    if (orb_dist < 0.001) orb_dist = 0.001; // Avoid division by zero
    final_color.rgb /= pow(orb_dist, 2.0);
    
    // Apply exponential distance falloff
    final_color.rgb *= exp(-d_fractal / expFactor);
    
    // Final tone mapping / gamma correction and vignette
    // Original: `o.rgb = pow(o.rgb, vec3(.45));` (gamma correction)
    final_color.rgb = pow(final_color.rgb, vec3(0.45));
    final_color.rgb -= dot(uv, uv) * vignetteStrength; // Apply vignette at the end
    final_color.a = 1.0;

    // --- Color Pulse (Moving Line of Light - independent overlay) ---
    if (pulseLineIntensity > 0.001) {
        float pulse_coord;
        if (pulseLineDirection < 0.5) { // Horizontal
            pulse_coord = uv.y;
        } else if (pulseLineDirection < 1.5) { // Vertical
            pulse_coord = uv.x;
        } else if (pulseLineDirection < 2.5) { // Diagonal A (bottom-left to top-right)
            pulse_coord = (uv.x + uv.y) * 0.707; 
        } else { // Diagonal B (top-left to bottom-right)
            pulse_coord = (uv.x - uv.y) * 0.707;
        }

        float pulse_wave = sin(pulse_coord * 20.0 + time_i * pulseLineSpeed * 5.0);
        float pulse_alpha = smoothstep(0.5 - pulseLineThickness, 0.5 + pulseLineThickness, fract(pulse_wave * 0.5 + 0.5));
        
        vec3 pulse_color = getPaletteColor(pulse_coord + time_i * 0.02, time_i); // Use palette for pulse color
        final_color.rgb = mix(final_color.rgb, pulse_color, pulse_alpha * pulseLineIntensity);
    }

    // --- Post-processing effects ---
    final_color.rgb = applyShimmer(final_color.rgb, uv_frag / RENDERSIZE.xy, time_i);
    final_color.rgb = applyColorAdjustments(final_color.rgb); // Brightness, Saturation, Contrast

    gl_FragColor = final_color;
}