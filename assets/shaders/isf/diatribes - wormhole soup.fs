/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals", "Psychedelic", "Raymarching", "3D", "Tunnel"],
    "DESCRIPTION": "A psychedelic raymarched tunnel with fractal elements. Features dynamic color palettes, customizable fractal geometry, advanced 3D camera control, and various post-processing effects for trippy visuals.",
    "INPUTS": [
        { "NAME": "inputImage", "TYPE": "image", "LABEL": "Texture Input", "DESCRIPTION": "Optional texture used for surface coloring. Connect a noise or pattern generator here." },
        
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalGlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Glow", "DESCRIPTION": "Controls the overall intensity of glowing effects."},
        {"NAME": "baseRaymarchSteps", "TYPE": "float", "DEFAULT": 60.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Tunnel Steps", "DESCRIPTION": "Steps for tunnel raymarching (L)."},
        {"NAME": "fractalRaymarchSteps", "TYPE": "float", "DEFAULT": 60.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Fractal Steps", "DESCRIPTION": "Steps for fractal raymarching."},

        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Palette Brightness", "DESCRIPTION": "Overall brightness multiplier for palette colors."},
        {"NAME": "tunnelColorBlend", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Tunnel Color Blend", "DESCRIPTION": "Blend factor for tunnel-specific colors (f > 3.0/f > 1.0 logic)."},
        {"NAME": "bgGlowStrength", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "LABEL": "Background Glow", "DESCRIPTION": "Intensity of the background glow/trail effect."},

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
        {"NAME": "camFOV", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.01, "MAX": 2.0, "LABEL": "Camera FOV (Zoom)", "DESCRIPTION": "Field of View for the camera (smaller value means wider FOV/more zoom out)."},
        {"NAME": "autoCamSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Auto Cam Speed", "DESCRIPTION": "Speed of automatic camera movement."},
        {"NAME": "autoCamRangeXY", "TYPE": "float", "DEFAULT": 6.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Auto Cam XY Range", "DESCRIPTION": "Range of automatic camera movement on XY axis."},
        {"NAME": "autoCamRangeZ", "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.1, "MAX": 5.0, "LABEL": "Auto Cam Z Range", "DESCRIPTION": "Range of automatic camera movement on Z axis."},
        {"NAME": "cameraRotSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Speed", "DESCRIPTION": "Speed of camera's self-rotation (D vector)."},
        {"NAME": "cameraRotAmount", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Amount", "DESCRIPTION": "Amount of camera's self-rotation (D vector)."},
        
        {"NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations", "DESCRIPTION": "Number of iterations for the fractal pattern."},
        {"NAME": "fractalAmp", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Amp", "DESCRIPTION": "Amplitude of the fractal's recursive scaling."},
        {"NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Offset", "DESCRIPTION": "Offset applied to abs(sin(p)) in fractal iteration."},
        {"NAME": "fractalScaleMain", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Main Fractal Scale", "DESCRIPTION": "Scale factor for the first fractal instance."},
        {"NAME": "fractalScaleSecond", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Second Fractal Scale", "DESCRIPTION": "Scale factor for the second fractal instance."},
        {"NAME": "fractalTimeOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Time Offset", "DESCRIPTION": "Time offset for the main fractal's movement."},
        {"NAME": "textureStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Strength", "DESCRIPTION": "Strength of the texture mapping on the fractal surface."},
        {"NAME": "textureScale", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.01, "MAX": 1.0, "LABEL": "Texture Scale", "DESCRIPTION": "Scaling factor for the texture coordinates."},

        {"NAME": "raymarchMinDist", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Raymarch Min Dist", "DESCRIPTION": "Minimum distance for raymarching precision."},
        {"NAME": "raymarchMaxDist", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 500.0, "LABEL": "Raymarch Max Dist", "DESCRIPTION": "Maximum distance the ray will travel."},
        {"NAME": "expFactorTunnel", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Tunnel Exp Factor", "DESCRIPTION": "Exponential falloff for tunnel distance."},
        {"NAME": "expFactorFractal", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Exp Factor", "DESCRIPTION": "Exponential falloff for fractal distance."},

        {"NAME": "shimmerStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Shimmer Strength", "DESCRIPTION": "Intensity of the psychedelic shimmer overlay effect."},
        {"NAME": "shimmerSpeed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "LABEL": "Shimmer Speed", "DESCRIPTION": "Speed of the psychedelic shimmer effect."},

        {"NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of the screen shake effect."},
        {"NAME": "shakeSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 30.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of the screen shake effect."},

        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."},
        {"NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Strength", "DESCRIPTION": "Strength of the dark vignette effect at the edges."}
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

// P function defines the path/motion of the tunnel
vec3 P(float z) {
    return vec3(tanh(cos(z * 0.4 * autoCamSpeed) * 0.5) * autoCamRangeXY,
                tanh(cos(z * 0.3 * autoCamSpeed) * 0.75) * autoCamRangeXY, 
                z);
}

// 3D Texture Lookup (Simulated)
vec3 tex3D(sampler2D tex, in vec3 p, in vec3 n ){
    n = max((abs(n) - 0.2) * 7.0, 0.001);
    n /= (n.x + n.y + n.z);
    
    // Using fract for tiling if coordinates get too large
    return (texture(tex, fract(p.yz) * textureScale).rgb * n.x + 
            texture(tex, fract(p.zx) * textureScale).rgb * n.y + 
            texture(tex, fract(p.xy) * textureScale).rgb * n.z).rgb * textureStrength;
}

// distance to tunnel
float tunnel(vec3 p) {
    return 1.0 - length(p.xy - P(p.z).xy);
}

// distance to fractal
float fractal(vec3 p, float s) {
    float w = 1.0, l;
    
    p.xy *= s;
    
    // distance to fractal
    for (int i = 0; i < int(fractalIterations); i++) {
        p = abs(sin(p)) - fractalOffset; // Tunable fractalOffset
        l = fractalAmp / dot(p, p);      // Tunable fractalAmp
        p *= l; 
        w *= l;
    }
    return length(p) / w; 
}

// Main Signed Distance Function for the scene
float map(vec3 p) {
    return min(fractal(fractalTimeOffset * time_i + p, fractalScaleMain),
               fractal(p, fractalScaleSecond));
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
    
    float s_dist = raymarchMinDist, d_hit_tunnel = 0.0; // s_dist is step size, d_hit is total distance
    
    // Camera Setup
    vec3 ro; // Ray Origin
    vec3 ta; // Target (Look At)

    if (enableAutoCameraMovement) {
        // Original time-based camera movement P(T)
        ro = P(time_i * autoCamSpeed);
        // Look ahead along the path
        ta = P(time_i * autoCamSpeed + autoCamRangeZ); 
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
    
    // Ray Direction (D) based on UV and camera basis, with added rotation
    // Original: `vec3(rot(sin(T*.2)*.3)*u, 1) * .3 * mat3(-X, cross(X, Z), Z);`
    // Expanded:
    mat2 camera_rot_mat = rot2D(sin(time_i * cameraRotSpeed) * cameraRotAmount);
    vec3 D_ray = normalize(vec3(camera_rot_mat * uv, camFOV)) * mat3(-X_axis, cross(X_axis, Z_axis), Z_axis);

    vec4 final_color = vec4(0.0);
    vec3 current_pos = ro; 

    // --- Raymarch Tunnel ---
    for(int i = 0; i < int(baseRaymarchSteps); i++) {
        current_pos = ro + D_ray * d_hit_tunnel;
        s_dist = tunnel(current_pos);
        if (s_dist < raymarchMinDist || d_hit_tunnel > raymarchMaxDist) break;
        d_hit_tunnel += s_dist;
    }
    float tunnelDist = d_hit_tunnel;
    // vec3 tun_hit_pos = current_pos; // Not used currently, but could be for effects on tunnel wall
    
    // --- Reset for Fractal Raymarch, starting from tunnel hit point ---
    // If tunnel wasn't hit, d_hit_tunnel will be raymarchMaxDist. If it was, it's the hit distance.
    ro = ro + D_ray * d_hit_tunnel; // New ray origin is where tunnel ray stopped
    float d_hit_fractal = 0.0;
    vec3 fractal_glow_acc = vec3(0.0); // Accumulate glow

    for(int i = 0; i < int(fractalRaymarchSteps); i++) {
        current_pos = ro + D_ray * d_hit_fractal;
        s_dist = map(current_pos); // Distance to fractal
        if (s_dist < raymarchMinDist || d_hit_fractal > raymarchMaxDist) break;
        d_hit_fractal += s_dist;
        
        // Accumulate glow/trail effect
        fractal_glow_acc += sin(3.0 * time_i + current_pos * d_hit_fractal) * bgGlowStrength + 0.001;
    }
    final_color.rgb = 1.0 - fractal_glow_acc; // Invert glow for effect

    // --- Surface Shading for Fractal ---
    if (d_hit_fractal < raymarchMaxDist) { // If a fractal surface was hit
        vec3 hit_pos = ro + D_ray * d_hit_fractal;
        
        // Calculate normal at hit position
        float eps_normal = raymarchMinDist * 2.0;
        vec3 normal_vec = normalize(vec3(map(hit_pos + vec3(eps_normal,0,0)) - map(hit_pos - vec3(eps_normal,0,0)),
                                         map(hit_pos + vec3(0,eps_normal,0)) - map(hit_pos - vec3(0,eps_normal,0)),
                                         map(hit_pos + vec3(0,0,eps_normal)) - map(hit_pos + vec3(0,0,eps_normal))));
        
        // Apply color mixing based on the original f logic, now using tunable palette colors
        float f_val = mod((0.05 * time_i + hit_pos.z * 0.1), 4.0);
        
        vec3 surface_color = getPaletteColor(hit_pos.z * 0.1 + time_i * 0.05, time_i);

        if (f_val > 3.0) {
            vec3 specific_color = getPaletteColor(3.5 + time_i * 0.02, time_i); // Use a distinct palette offset
            surface_color = mix(surface_color, abs(sin(3.5 * time_i + hit_pos * 0.4) / dot(sin(time_i + hit_pos * 3.0), vec3(1.6))), tunnelColorBlend);
             surface_color = mix(surface_color, specific_color * abs(sin(3.5 * time_i + hit_pos * 0.4)), tunnelColorBlend);
        } else if (f_val > 1.0) {
            vec3 specific_color = getPaletteColor(1.5 + time_i * 0.02, time_i); // Use another distinct palette offset
            surface_color = mix(surface_color, vec3(1.0,0.0,1.0) * abs(sin(1.0 * time_i + hit_pos * 0.2) / dot(sin(hit_pos * 1.0), vec3(1.9))), tunnelColorBlend);
            surface_color = mix(surface_color, specific_color * abs(sin(1.0 * time_i + hit_pos * 0.2)), tunnelColorBlend);
        }

        final_color.rgb *= surface_color; // Blend with background accumulation
        final_color.rgb *= tex3D(inputImage, hit_pos, normal_vec); // Apply texture
    }
    
    // Apply exponential falloff based on distances
    final_color.rgb *= exp(-d_hit_fractal / expFactorFractal);
    final_color.rgb *= exp(-tunnelDist / expFactorTunnel);
    final_color.rgb *= globalGlowIntensity; // Overall glow
    
    // Final tone mapping and vignette
    final_color.rgb = pow(final_color.rgb - dot(uv, uv) * vignetteStrength, vec3(0.45));
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