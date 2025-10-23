/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals", "Psychedelic", "Raymarching", "3D", "Tunnel"],
    "DESCRIPTION": "A psychedelic raymarched fractal tunnel with complex movements, multi-stage color mixing, and procedural glimmers. Features advanced camera control, 7 distinct color palettes, and various post-processing effects. Glimmers and shake effects are applied primarily to the 3D surface.",
    "INPUTS": [
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalGlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Glow", "DESCRIPTION": "Controls the overall intensity of glowing effects."},

        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Palette Brightness", "DESCRIPTION": "Overall brightness multiplier for palette colors."},
        {"NAME": "fractalGlowStrengthA", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0, "MAX": 0.1, "LABEL": "Fractal Glow Base", "DESCRIPTION": "Base intensity for fractal self-illuminating glow."},
        {"NAME": "fractalGlowStrengthB", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.2, "LABEL": "Fractal Glow Sin Amp", "DESCRIPTION": "Amplitude for sin wave in fractal glow."},
        {"NAME": "fractalGlowStrengthC", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.1, "LABEL": "Fractal Glow Offset", "DESCRIPTION": "Offset for fractal self-illuminating glow."},

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
        {"NAME": "P_X_CosFactor", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 1.0, "LABEL": "P_X_Cos Factor", "DESCRIPTION": "Cosine frequency for P function X component."},
        {"NAME": "P_X_Amp", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1, "MAX": 2.0, "LABEL": "P_X_Amplitude", "DESCRIPTION": "Amplitude multiplier for P function X component."},
        {"NAME": "P_X_Range", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "P_X_Range", "DESCRIPTION": "Overall range for P function X component."},
        {"NAME": "P_Y_CosFactor", "TYPE": "float", "DEFAULT": 0.32, "MIN": 0.1, "MAX": 1.0, "LABEL": "P_Y_Cos Factor", "DESCRIPTION": "Cosine frequency for P function Y component."},
        {"NAME": "P_Y_Amp", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 2.0, "LABEL": "P_Y_Amplitude", "DESCRIPTION": "Amplitude multiplier for P function Y component."},
        {"NAME": "P_Y_Range", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 30.0, "LABEL": "P_Y_Range", "DESCRIPTION": "Overall range for P function Y component."},
        {"NAME": "cameraRotSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Speed", "DESCRIPTION": "Speed of camera's self-rotation (D vector)."},
        {"NAME": "cameraRotAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rotation Amount", "DESCRIPTION": "Amount of camera's self-rotation (D vector)."},
        {"NAME": "cameraRotZOffsetSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 50.0, "LABEL": "Camera Rot Z Offset Speed", "DESCRIPTION": "Speed of Z-based rotation offset."},
        {"NAME": "cameraRotZOffsetAmount", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Camera Rot Z Offset Amount", "DESCRIPTION": "Amount of Z-based rotation offset."},

        {"NAME": "tunnelRaymarchSteps", "TYPE": "float", "DEFAULT": 120.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Tunnel Steps", "DESCRIPTION": "Max steps for tunnel raymarching."},
        {"NAME": "tunnelDistMultiplier", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Tunnel Dist Multiplier", "DESCRIPTION": "Multiplier for tunnel distance in raymarch."},
        {"NAME": "fractalRaymarchSteps", "TYPE": "float", "DEFAULT": 120.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Fractal Steps", "DESCRIPTION": "Max steps for fractal raymarching."},
        {"NAME": "raymarchMinDist", "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Raymarch Min Dist", "DESCRIPTION": "Minimum distance for raymarching precision."},
        {"NAME": "raymarchMaxDist", "TYPE": "float", "DEFAULT": 100.0, "MIN": 10.0, "MAX": 500.0, "LABEL": "Raymarch Max Dist", "DESCRIPTION": "Maximum distance the ray will travel."},

        {"NAME": "fractalInitScale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Initial Scale", "DESCRIPTION": "Initial scaling factor for the fractal geometry."},
        {"NAME": "fractalInitWeight", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.0, "LABEL": "Fractal Initial Weight", "DESCRIPTION": "Initial weight for the fractal distance calculation."},
        {"NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations", "DESCRIPTION": "Number of iterations for the fractal pattern."},
        {"NAME": "fractalAmp", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Amp", "DESCRIPTION": "Amplitude of the fractal's recursive scaling."},
        {"NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Offset", "DESCRIPTION": "Offset applied to abs(sin(p)) in fractal iteration."},
        {"NAME": "fractalTranslateSpeed", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Translate Speed", "DESCRIPTION": "Speed of the fractal's XZ translation."},
        {"NAME": "fractalTranslateAmount", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Fractal Translate Amount", "DESCRIPTION": "Amount of the fractal's XZ translation."},

        {"NAME": "fTimeScale", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Mix Time Scale", "DESCRIPTION": "Time scale for the main 'f' color mixing variable."},
        {"NAME": "fPZScale", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "Color Mix PZ Scale", "DESCRIPTION": "P.z scale for the main 'f' color mixing variable."},
        {"NAME": "fThreshold1", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 4.0, "LABEL": "Mix Threshold 1", "DESCRIPTION": "Threshold for the first color mix (f > 3.0)."},
        {"NAME": "fThreshold2", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 4.0, "LABEL": "Mix Threshold 2", "DESCRIPTION": "Threshold for the second color mix (f > 2.0)."},
        {"NAME": "fThreshold3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0, "LABEL": "Mix Threshold 3", "DESCRIPTION": "Threshold for the third color mix (f > 1.0)."},
        {"NAME": "mixAmount1", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Mix Amount 1", "DESCRIPTION": "Mix factor for threshold 1."},
        {"NAME": "mixAmount2", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Mix Amount 2", "DESCRIPTION": "Mix factor for threshold 2."},
        {"NAME": "mixAmount3", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Mix Amount 3", "DESCRIPTION": "Mix factor for threshold 3."},
        {"NAME": "mixAmount4", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Mix Amount 4", "DESCRIPTION": "Mix factor for default (else) case."},
        {"NAME": "sinAmp1", "TYPE": "float", "DEFAULT": 3.5, "MIN": 0.1, "MAX": 10.0, "LABEL": "Sin Amp 1", "DESCRIPTION": "Sin amplitude for threshold 1."},
        {"NAME": "sinAmp2", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Sin Amp 2", "DESCRIPTION": "Sin amplitude for threshold 2."},
        {"NAME": "sinAmp3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Sin Amp 3", "DESCRIPTION": "Sin amplitude for threshold 3."},
        {"NAME": "sinAmp4", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 10.0, "LABEL": "Sin Amp 4", "DESCRIPTION": "Sin amplitude for default (else) case."},
        {"NAME": "dotVec1", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 5.0, "LABEL": "Dot Vec 1", "DESCRIPTION": "Dot product vector component for threshold 1."},
        {"NAME": "dotVec2", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.1, "MAX": 5.0, "LABEL": "Dot Vec 2", "DESCRIPTION": "Dot product vector component for threshold 2."},
        {"NAME": "dotVec3", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.1, "MAX": 5.0, "LABEL": "Dot Vec 3", "DESCRIPTION": "Dot product vector component for threshold 3."},
        {"NAME": "dotVec4", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.1, "MAX": 5.0, "LABEL": "Dot Vec 4", "DESCRIPTION": "Dot product vector component for default (else) case."},

        {"NAME": "glimmerIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Glimmer Intensity", "DESCRIPTION": "Intensity of the procedural noise glimmers on the surface."},
        {"NAME": "glimmerStartScale", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.01, "MAX": 1.0, "LABEL": "Glimmer Start Scale", "DESCRIPTION": "Starting scale for glimmer noise iterations."},
        {"NAME": "glimmerEndScale", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Glimmer End Scale", "DESCRIPTION": "Ending scale for glimmer noise iterations."},
        {"NAME": "glimmerIncrement", "TYPE": "float", "DEFAULT": 1.4142, "MIN": 1.1, "MAX": 2.0, "LABEL": "Glimmer Increment", "DESCRIPTION": "Multiplier for scale in each glimmer iteration."},
        {"NAME": "glimmerDotFactor", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.2, "LABEL": "Glimmer Dot Factor", "DESCRIPTION": "Factor for dot product in glimmer noise."},

        {"NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of the screen shake effect (only if surface hit)."},
        {"NAME": "shakeSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 30.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of the screen shake effect."},

        {"NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."},
        {"NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Strength", "DESCRIPTION": "Strength of the dark vignette effect at the edges."},
        {"NAME": "gammaCorrection", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 2.0, "LABEL": "Gamma Correction", "DESCRIPTION": "Gamma correction for final output (higher makes brighter)."},
        {"NAME": "expFactorFractal", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 10.0, "LABEL": "Fractal Exp Falloff", "DESCRIPTION": "Exponential falloff for fractal distance."},
        {"NAME": "expFactorTunnel", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Tunnel Exp Falloff", "DESCRIPTION": "Exponential falloff for tunnel distance."}
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

// distance to tunnel
float tunnel(vec3 p) {
    return 0.5 - length(p.xy - P(p.z).xy);
}

// distance to fractal
float fractal(vec3 p_in) {
    float w = fractalInitWeight, l;
    
    // scale
    p_in *= fractalInitScale;
    
    // translate to some place interesting
    p_in.xz += tanh(sin(time_i * fractalTranslateSpeed) * 5.0) * fractalTranslateAmount;

    // distance to fractal
    for (int i = 0; i < int(fractalIterations); i++) {
        p_in = abs(sin(p_in)) - fractalOffset;
        l = fractalAmp / dot(p_in, p_in);
        p_in *= l; 
        w *= l;
    }
    return length(p_in) / w; 
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
    time_i = TIME * masterSpeed * 0.85; // Original T was iTime * 0.85
    for (int j = 0; j < 7; j++) {
        dynamicPalette[j] = generatePsychedelicColor(float(j) / 7.0, time_i, paletteSelect);
    }

    vec2 uv_frag = gl_FragCoord.xy;
    bool surfaceHit = false; // Flag to check if a surface was hit for conditional effects

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
    // Original: `D = vec3(rot(sin(T*.3)*.5) * rot(tanh(sin(p.z*.2)*2.)*3.)*u, 1) * mat3(-X, cross(X, Z), Z);`
    // Expanded:
    float rot_angle1 = sin(time_i * cameraRotSpeed) * cameraRotAmount;
    // p.z in original D calculation is ro.z here
    float rot_angle2 = tanh(sin(ro.z * 0.2) * cameraRotZOffsetSpeed) * cameraRotZOffsetAmount;
    
    mat2 combined_rot_mat = rot2D(rot_angle1) * rot2D(rot_angle2);
    
    vec3 D_ray = normalize(vec3(combined_rot_mat * uv, 1.0)) * mat3(-X_axis, cross(X_axis, Z_axis), Z_axis);

    vec4 final_color = vec4(0.0);
    vec3 current_pos = ro; 
    vec3 tunnel_hit_pos_q; // This stores 'q' from the original shader

    // --- First Raymarch Loop (Tunnel) ---
    for(int i = 0; i < int(tunnelRaymarchSteps); i++) {
        current_pos = ro + D_ray * d_total;
        s_dist = tunnel(current_pos) * tunnelDistMultiplier; // Original had *.5
        
        if (s_dist < raymarchMinDist || d_total > raymarchMaxDist) break;
        d_total += s_dist;
    }
    float tunnelDist = d_total;
    tunnel_hit_pos_q = current_pos; // Store q

    // --- Reset for Fractal Raymarch, starting from tunnel hit point ---
    // If tunnel wasn't hit, d_total will be raymarchMaxDist. If it was, it's the hit distance.
    vec3 ro_fractal = ro + D_ray * d_total; 
    float d_fractal = 0.0;
    
    for(int i = 0; i < int(fractalRaymarchSteps); i++) {
        current_pos = ro_fractal + D_ray * d_fractal;
        s_dist = fractal(current_pos); // Distance to fractal
        
        if (s_dist < raymarchMinDist || d_fractal > raymarchMaxDist) {
            if (s_dist < raymarchMinDist) { // If we actually hit something
                surfaceHit = true;
            }
            break;
        }
        d_fractal += s_dist;
        
        // Accumulate glow/trail effect based on fractal position
        // `o.rgb += .001 - sin(p*.6)*.05+.03;`
        final_color.rgb += fractalGlowStrengthA - sin(current_pos * 0.6) * fractalGlowStrengthB + fractalGlowStrengthC;
    }
    
    // `p` in the original shader, after loops, corresponds to `current_pos` here
    vec3 final_fractal_pos = current_pos;

    // --- Post-processing and Coloring ---
    final_color.rgb = 1.0 - final_color.rgb; // Invert the accumulated glow
    
    // Conditional color mixing based on 'f'
    float f_val = mod((fTimeScale * time_i + final_fractal_pos.z * fPZScale), 4.0);
    
    vec3 mixed_color = vec3(0.0);
    vec3 palette_base_color = getPaletteColor(final_fractal_pos.z * 0.1 + time_i * 0.01, time_i); // For general palette influence

    if (f_val > fThreshold1) {
        vec3 specific_color = getPaletteColor(f_val * 0.5 + time_i * 0.02, time_i);
        mixed_color = mix(palette_base_color, abs(sin(sinAmp1 * time_i + final_fractal_pos * 0.4) / dot(sin(time_i + tunnel_hit_pos_q * 32.0), vec3(dotVec1))), mixAmount1);
        mixed_color = mix(mixed_color, specific_color, mixAmount1); // Blend palette with original logic
    } else if (f_val > fThreshold2) {
        vec3 specific_color = getPaletteColor(f_val * 0.5 + time_i * 0.02, time_i);
        mixed_color = mix(palette_base_color, abs(sin(sinAmp2 * time_i + final_fractal_pos * 0.3) / dot(sin(time_i + tunnel_hit_pos_q * 16.0), vec3(dotVec2))), mixAmount2);
        mixed_color = mix(mixed_color, specific_color, mixAmount2);
    } else if (f_val > fThreshold3) {
        vec3 specific_color = getPaletteColor(f_val * 0.5 + time_i * 0.02, time_i);
        mixed_color = mix(palette_base_color, abs(sin(sinAmp3 * time_i + tunnel_hit_pos_q * 0.2) / dot(sin(tunnel_hit_pos_q * 8.0), vec3(dotVec3))), mixAmount3);
        mixed_color = mix(mixed_color, specific_color, mixAmount3);
    } else {
        vec3 specific_color = getPaletteColor(f_val * 0.5 + time_i * 0.02, time_i);
        mixed_color = mix(palette_base_color, abs(sin(sinAmp4 * time_i + final_fractal_pos * 0.1) / dot(sin(time_i + final_fractal_pos * 32.0), vec3(dotVec4))), mixAmount4);
        mixed_color = mix(mixed_color, specific_color, mixAmount4);
    }
    
    final_color.rgb *= mixed_color;

    // Procedural Glimmer (Noise Texture Loop) - only applied if surface was hit
    if (surfaceHit && glimmerIntensity > 0.001) {
        for (float i_glim = glimmerStartScale; i_glim < glimmerEndScale; i_glim *= glimmerIncrement) {
            final_color.rgb += abs(dot(sin(final_fractal_pos * i_glim * 512.0), vec3(glimmerDotFactor))) / i_glim * glimmerIntensity;
        }
    }

    // Distance fade
    final_color.rgb *= exp(-d_fractal / expFactorFractal);
    final_color.rgb *= exp(-tunnelDist / expFactorTunnel);
    final_color.rgb *= globalGlowIntensity; // Overall glow
    
    // Vignette & Gamma (original order was vignette then gamma)
    final_color.rgb = pow(final_color.rgb - dot(uv, uv) * vignetteStrength, vec3(gammaCorrection));
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

    // --- Screen Shake Effect (only if surface was hit) ---
    if (shakeAmount > 0.001 && surfaceHit) {
        float shake_x = sin(time_i * shakeSpeed * 15.0) * cos(time_i * shakeSpeed * 10.0) * shakeAmount * RENDERSIZE.y;
        float shake_y = cos(time_i * shakeSpeed * 12.0) * sin(time_i * shakeSpeed * 18.0) * shakeAmount * RENDERSIZE.y;
        vec3 shake_color_shift = vec3(shake_x, shake_y, 0.0) * 0.005; 
        final_color.rgb += shake_color_shift;
    }

    // --- Brightness, Saturation, Contrast ---
    final_color.rgb = applyColorAdjustments(final_color.rgb); 
    
    gl_FragColor = final_color;
}