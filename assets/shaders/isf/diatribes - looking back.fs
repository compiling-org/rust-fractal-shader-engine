/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals", "Psychedelic", "Raymarching", "3D"],
    "DESCRIPTION": "An intricate 3D raymarching fractal. Features a highly customizable and dynamic fractal structure, full 3D camera control, 7 distinct psychedelic color palettes, and a suite of post-processing effects for a truly immersive and trippy visual experience.",
    "INPUTS": [
        { "NAME": "inputImage", "TYPE": "image", "LABEL": "Texture Input", "DESCRIPTION": "Optional texture used for surface coloring. Connect a noise or pattern generator here." },
        
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalGlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Glow", "DESCRIPTION": "Controls the overall intensity of glowing effects."},

        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Palette Brightness", "DESCRIPTION": "Overall brightness multiplier for palette colors."},
        {"NAME": "fractalColorPulse", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Color Pulse", "DESCRIPTION": "Strength of the pulsing color modulation within the fractal."},

        {"NAME": "pulseLineIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Line Intensity", "DESCRIPTION": "Strength of the moving line of light effect (overlay)."},
        {"NAME": "pulseLineSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Pulse Line Speed", "DESCRIPTION": "Speed of the moving line of light."},
        {"NAME": "pulseLineThickness", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Pulse Line Thickness", "DESCRIPTION": "Thickness of the moving line of light."},
        {"NAME": "pulseLineDirection", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Pulse Line Direction", "DESCRIPTION": "0=Horizontal, 1=Vertical, 2=Diagonal A, 3=Diagonal B."},

        {"NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position X", "DESCRIPTION": "X-coordinate of the camera position."},
        {"NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position Y", "DESCRIPTION": "Y-coordinate of the camera position."},
        {"NAME": "camZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Position Z", "DESCRIPTION": "Z-coordinate of the camera position."},
        {"NAME": "lookAtX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At X", "DESCRIPTION": "X-coordinate of the point the camera is looking at."},
        {"NAME": "lookAtY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Y", "DESCRIPTION": "Y-coordinate of the point the camera is looking at."},
        {"NAME": "lookAtZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Look At Z", "DESCRIPTION": "Z-coordinate of the point the camera is looking at."},
        {"NAME": "camFOV", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Camera FOV", "DESCRIPTION": "Field of View for the camera (smaller value means wider FOV)."},
        {"NAME": "enableAutoCameraMovement", "TYPE": "bool", "DEFAULT": true, "LABEL": "Auto Camera Movement", "DESCRIPTION": "Enable the original time-based camera animation."},
        {"NAME": "autoCamSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Auto Cam Speed", "DESCRIPTION": "Speed of automatic camera movement."},
        {"NAME": "autoCamRange", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Auto Cam Range", "DESCRIPTION": "Range/intensity of automatic camera movement."},
        {"NAME": "fractalMorphStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Morph Strength", "DESCRIPTION": "Controls the strength of the morphing effect on the fractal's shape."},


        {"NAME": "fractalMode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.99, "LABEL": "Fractal Mode", "DESCRIPTION": "Switches the fractal's primary deformation: 0=Horizontal Shift, 1=Vertical Shift."},
        {"NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations", "DESCRIPTION": "Number of iterations for the fractal pattern."},
        {"NAME": "fractalScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Scale", "DESCRIPTION": "Overall scaling of the fractal."},
        {"NAME": "fractalAmp", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Amp", "DESCRIPTION": "Amplitude of the fractal's recursive scaling."},
        {"NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Offset", "DESCRIPTION": "Offset applied to abs(sin(p)) in fractal iteration."},
        {"NAME": "fractalSinFactor", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Sin Factor", "DESCRIPTION": "Factor for sine modulation in P function's X component."},
        {"NAME": "fractalLookFreqEffect", "TYPE": "float", "DEFAULT": 9.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Look Freq Effect", "DESCRIPTION": "Impact of LOOK_FREQ on fractal position."},
        {"NAME": "textureStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Strength", "DESCRIPTION": "Strength of the texture mapping on the fractal surface."},
        {"NAME": "textureScale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Texture Scale", "DESCRIPTION": "Scaling factor for the texture coordinates."},
        {"NAME": "fractalFloorCeiling", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Floor/Ceiling Dist", "DESCRIPTION": "Distance of the floor and ceiling planes from origin."},

        {"NAME": "raymarchMaxSteps", "TYPE": "float", "DEFAULT": 200.0, "MIN": 50.0, "MAX": 500.0, "LABEL": "Raymarch Steps", "DESCRIPTION": "Maximum raymarching steps for quality."},
        {"NAME": "raymarchMinDist", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Raymarch Min Dist", "DESCRIPTION": "Minimum distance for raymarching precision."},
        {"NAME": "raymarchMaxDist", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 500.0, "LABEL": "Raymarch Max Dist", "DESCRIPTION": "Maximum distance the ray will travel."},

        {"NAME": "aoStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "AO Strength", "DESCRIPTION": "Strength of Ambient Occlusion effect."},
        {"NAME": "aoSteps", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "AO Steps", "DESCRIPTION": "Number of steps for AO calculation."},
        {"NAME": "aoScale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "AO Scale", "DESCRIPTION": "Scale factor for AO sampling."},

        {"NAME": "shimmerStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Shimmer Strength", "DESCRIPTION": "Intensity of the psychedelic shimmer overlay effect."},
        {"NAME": "shimmerSpeed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "LABEL": "Shimmer Speed", "DESCRIPTION": "Speed of the psychedelic shimmer effect."},

        {"NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of the screen shake effect."},
        {"NAME": "shakeSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 30.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of the screen shake effect."},

        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."},
        {"NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 0.13, "MIN": 0.0, "MAX": 0.5, "LABEL": "Vignette Strength", "DESCRIPTION": "Strength of the dark vignette effect at the edges."},
        {"NAME": "enableTonemapping", "TYPE": "bool", "DEFAULT": true, "LABEL": "Enable Tonemapping", "DESCRIPTION": "Apply filmic tonemapping for better dynamic range."}
    ]
}
*/

#define PI 3.14159265359
#define TAU 6.28318530718 // 2 * PI

// Global animation time, derived from ISF's TIME and masterSpeed
float time_i;

// Global variable for fractal color
vec3 fractal_rgb;

// Global dynamic palette storage
// ISF doesn't directly support dynamic array sizing, so we'll fix size to 7
vec3 dynamicPalette[7]; 

// --- Utility Functions ---
vec3 rotX(vec3 p, float rad) {
    float c = cos(rad), s = sin(rad);
    return vec3(p.x, p.y * c - p.z * s, p.y * s + p.z * c);
}
vec3 rotY(vec3 p, float rad) {
    float c = cos(rad), s = sin(rad);
    return vec3(p.x * c + p.z * s, p.y, -p.x * s + p.z * c);
}
vec3 rotZ(vec3 p, float rad) {
    float c = cos(rad), s = sin(rad);
    return vec3(p.x * c - p.y * s, p.x * s + p.y * c, p.z);
}
mat2 rot2D(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

// Filmic Tonemapping (Unreal Engine 3 style)
vec3 unrealTonemap(vec3 x) {
    return x / (x + 0.155) * 1.019;
}

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

vec3 getsmcolor(float c_val, float s) {
    s *= 0.5;
    float scaled_c_val = mod(c_val, 7.0); 
    
    int idx1 = int(floor(scaled_c_val));
    int idx2 = (idx1 + 1) % 7; 

    vec3 color1 = dynamicPalette[idx1];
    vec3 color2 = dynamicPalette[idx2];

    float blend_factor = smoothstep(0.5 - s, 0.5 + s, fract(scaled_c_val));
    
    return mix(color1, color2, blend_factor);
}

// P function defines the path/motion
vec3 P(float z) {
    return vec3(tanh(sin(z * fractalSinFactor) * 0.15) * 4.0 * autoCamRange,
                sin(z * 6.0) * 0.03 * autoCamRange, z);
}

// Orb function for camera target displacement
vec3 orb(vec3 p_in) {
    float t_orb = time_i * 3.0;
    float look_freq = tanh(cos((time_i * 3.0) * 0.125) * 9.0); // Original LOOK_FREQ logic
    
    return (p_in - vec3(
                P(p_in.z).x + tanh(cos(t_orb * 0.5) * 3.0) * 2.5,
                P(p_in.z).y + tanh(cos(t_orb * 0.7) * 2.0) * 2.5,
                1.3 + time_i + look_freq * fractalLookFreqEffect));
}

// 3D Texture Lookup (Simulated)
// This shader uses inputImage, which implies a texture input.
// Assuming inputImage is a 2D texture, this function simulates 3D texture mapping.
vec3 tex3D(sampler2D tex, in vec3 p, in vec3 n ){ // Fixed sampler22D to sampler2D and added 'in' for n
    n = max((abs(n) - 0.2) * 7.0, 0.001);
    n /= (n.x + n.y + n.z);
    
    // Using fract for tiling if coordinates get too large
    return (texture(tex, fract(p.yz) * textureScale).rgb * n.x + 
            texture(tex, fract(p.zx) * textureScale).rgb * n.y + 
            texture(tex, fract(p.xy) * textureScale).rgb * n.z).rgb * textureStrength;
}

// Fractal Signed Distance Function
float fractal(vec3 p) {
    float s, w = 0.6, l;
    
    p *= fractalScale; // Apply global fractal scale
    
    // Fractal mode based on LOOK_FREQ, now controlled by input `fractalMode`
    if (fractalMode > 0.5) { // Original LOOK_FREQ > 0.5 branch
        p.y -= 1.5 * fractalOffset;
    } else { // Original LOOK_FREQ <= 0.5 branch
        p.x -= 1.5 * fractalOffset;
    }
        
    p.xy -= P(p.z).xy * fractalMorphStrength; // Apply morphing - fractalMorphStrength is now a direct uniform input
    
    for (int i = 0; i < int(fractalIterations); i++) {
        p = abs(sin(p)) - fractalOffset; // Use fractalOffset input
        l = fractalAmp / dot(p, p); // Use fractalAmp input
        p *= l; 
        w *= l;
    }

    // Color based on fractal position and palette
    vec3 base_fractal_color = getsmcolor(length(p) * fractalColorPulse + time_i * 0.03, 0.5);
    fractal_rgb = abs(base_fractal_color / (dot(cos(fractalColorPulse * time_i + p) + 1.25, vec3(0.03)))) * globalGlowIntensity;

    return length(p) / w;
}

// Main Scene SDF
float map(vec3 p) {
    float s_val; // Renamed to avoid conflict with global s
    s_val = fractal(p);
    
    // Add floor and ceiling planes
    s_val = min(s_val, fractalFloorCeiling - p.y); // Floor
    s_val = min(s_val, fractalFloorCeiling + p.y); // Ceiling (inverted Y)
    
    return s_val;
}

// Ambient Occlusion
float AO(in vec3 pos, in vec3 nor) {
    float sca = aoScale, occ = 0.0;
    for( int i=0; i < int(aoSteps); i++ ){
        float hr = 0.01 + float(i) * 0.5 / max(1.0, float(aoSteps - 1)); // Prevent division by zero if aoSteps is 1
        float dd = map(nor * hr + pos);
        occ += (hr - dd) * sca;
        sca *= 0.7; // Decay factor
    }
    return clamp(1.0 - occ * aoStrength, 0.0, 1.0);
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


// Main rendering function (equivalent to Shadertoy's mainImage)
void main() {
    // Populate the dynamic palette array
    time_i = TIME * masterSpeed;
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
    
    float s_dist = 0.002, d_hit = 0.0; // s_dist is step size, d_hit is total distance
    
    // Camera Setup
    vec3 ro; // Ray Origin
    vec3 ta; // Target (Look At)

    if (enableAutoCameraMovement) {
        // Original time-based camera movement
        vec3 p_cam_path = P(time_i * autoCamSpeed);
        ro = p_cam_path;
        // The original `orb` displacement was very strong.
        // Adjusting it slightly or making it optional for user camera control.
        // For auto camera, keeping original logic.
        ta = p_cam_path - orb(p_cam_path) - p_cam_path; // Target is calculated relative to path
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
    
    // Ray Direction (D) based on UV and camera basis
    vec3 D_ray = normalize(uv.x * X_axis + uv.y * Y_axis + camFOV * Z_axis); // Use camFOV input

    vec3 current_pos = ro; // Current position along ray
    // int steps = 0; // Not strictly needed for loop condition outside

    // Raymarching Loop
    for (int steps = 0; steps < int(raymarchMaxSteps); steps++) { // Explicitly cast to int
        current_pos = ro + D_ray * d_hit;
        s_dist = map(current_pos); // Get distance to scene
        s_dist *= 0.6; // Step multiplier from original shader
        if (s_dist < raymarchMinDist || d_hit > raymarchMaxDist) break;
        d_hit += s_dist;
    }
    
    vec4 final_color = vec4(0.0);

    if (d_hit < raymarchMaxDist) { // If a surface was hit
        // Calculate normal at hit position
        // Epsilon for normal calculation
        float eps = raymarchMinDist * 2.0; // Use a multiple of min_dist for epsilon
        vec3 normal_vec = normalize(vec3(map(current_pos + vec3(eps,0,0)) - map(current_pos - vec3(eps,0,0)),
                                         map(current_pos + vec3(0,eps,0)) - map(current_pos - vec3(0,eps,0)),
                                         map(current_pos + vec3(0,0,eps)) - map(current_pos - vec3(0,0,eps))));
        
        // Base color from fractal's `rgb` output and texture
        vec3 base_render_color = fractal_rgb * pow(tex3D(inputImage, current_pos * textureScale, normal_vec), vec3(2.2)); // Use inputImage
        
        // Apply secondary color modulation (from original shader, iterated)
        // This creates a shimmering/pulsing effect on the fractal's surface
        float iteration_color_factor = 0.0;
        for (float iter_i = 0.2; iter_i < 1.2; iter_i *= 1.4142) {
            iteration_color_factor += abs(dot(sin(base_render_color * iter_i * 32.0), vec3(0.04))) / iter_i;
        }
        base_render_color += iteration_color_factor;
        base_render_color *= base_render_color * 0.35; // Brightness adjustment

        // Apply Ambient Occlusion
        base_render_color *= AO(current_pos, normal_vec);
        
        // Final color based on distance and `orb` function
        float look_freq_final = tanh(cos((time_i * 3.0) * 0.125) * 9.0);
        float orb_len_pow = pow(length(orb(current_pos)), 2.5 + (look_freq_final * 1.1) * 0.5 - 0.5);
        
        // Prevent division by zero or very small numbers
        if (orb_len_pow < 0.001) orb_len_pow = 0.001; 
        
        final_color.rgb = base_render_color / orb_len_pow;

        // Conditional multipliers from original shader, based on `LOOK_FREQ` (now `fractalMode`)
        if (fractalMode > 0.5) {
            final_color.rgb *= 128.0;
        } else {
            final_color.rgb *= vec3(1.0, 0.1, 0.5);
            final_color.rgb *= 96.0;
        }

        // Apply Tonemapping (conditional based on input)
        if (enableTonemapping) {
            final_color.rgb = unrealTonemap(final_color.rgb * exp(-d_hit) - dot(uv, uv) * vignetteStrength); // Uses vignetteStrength
        } else {
            final_color.rgb = (final_color.rgb * exp(-d_hit) - dot(uv, uv) * vignetteStrength);
        }
        final_color.rgb = pow(final_color.rgb, vec3(0.45)); // Gamma correction
        final_color.a = 1.0;
        
    } else {
        // If no surface was hit, render a subtle background from the palette
        final_color.rgb = getsmcolor(length(uv) * 0.5 + time_i * 0.01, 0.5);
        final_color.rgb *= globalGlowIntensity * 0.1; // Dim background
        final_color.a = 1.0;
    }


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
        // Fixed the typo here: pulsaceLineThickness -> pulseLineThickness
        float pulse_alpha = smoothstep(0.5 - pulseLineThickness, 0.5 + pulseLineThickness, fract(pulse_wave * 0.5 + 0.5));
        
        vec3 pulse_color = generatePsychedelicColor(pulse_coord + time_i * 0.02, time_i, paletteSelect + 0.25);
        final_color.rgb = mix(final_color.rgb, pulse_color, pulse_alpha * pulseLineIntensity);
    }

    // --- Post-processing effects ---
    final_color.rgb = applyShimmer(final_color.rgb, uv_frag / RENDERSIZE.xy, time_i);
    final_color.rgb = applyColorAdjustments(final_color.rgb); // Brightness, Saturation, Contrast

    gl_FragColor = final_color;
}