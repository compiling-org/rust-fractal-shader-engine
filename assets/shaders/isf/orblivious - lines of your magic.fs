/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": [
        "Fractal",
        "Abstract",
        "Live Visuals",
        "Psychedelic",
        "Raymarching",
        "3D",
        "Tunnel"
    ],
    "DESCRIPTION": "A psychedelic raymarched fractal combining intricate 3D structures with abstract 2D patterns and dynamic color palettes. Features extensive controls for camera, geometry, and post-processing effects like color pulse and screen shake.",
    "INPUTS": [
        {"NAME": "masterSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Master Speed", "DESCRIPTION": "Overall animation speed multiplier."},
        {"NAME": "globalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Global Brightness", "DESCRIPTION": "Adjusts overall image brightness."},
        {"NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99, "LABEL": "Color Palette", "DESCRIPTION": "Choose from 7 distinct psychedelic color palettes."},
        {"NAME": "paletteAnimSpeed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Palette Animation Speed", "DESCRIPTION": "Speed at which the selected palette's colors subtly evolve."},
        {"NAME": "paletteMixFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Palette Mix Factor", "DESCRIPTION": "Multiplier for palette influence on colors."},
        {"NAME": "pulseLineIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Line Intensity", "DESCRIPTION": "Strength of the moving line of light effect (overlay)."},
        {"NAME": "pulseLineSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Pulse Line Speed", "DESCRIPTION": "Speed of the moving line of light."},
        {"NAME": "pulseLineThickness", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Pulse Line Thickness", "DESCRIPTION": "Thickness of the moving line of light."},
        {"NAME": "pulseLineDirection", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Pulse Line Direction", "DESCRIPTION": "0=Horizontal, 1=Vertical, 2=Diagonal A, 3=Diagonal B."},
        {"NAME": "enableAutoCamera", "TYPE": "bool", "DEFAULT": true, "LABEL": "Auto Camera Movement", "DESCRIPTION": "Enable or disable automatic camera movement."},
        {"NAME": "camPosX", "TYPE": "float", "DEFAULT": 10.0, "MIN": -20.0, "MAX": 20.0, "LABEL": "Camera Pos X", "DESCRIPTION": "Manual camera X position."},
        {"NAME": "camPosY", "TYPE": "float", "DEFAULT": 10.0, "MIN": -20.0, "MAX": 20.0, "LABEL": "Camera Pos Y", "DESCRIPTION": "Manual camera Y position."},
        {"NAME": "camPosZ", "TYPE": "float", "DEFAULT": -20.0, "MIN": -50.0, "MAX": 50.0, "LABEL": "Camera Pos Z", "DESCRIPTION": "Manual camera Z position."},
        {"NAME": "lookAtX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Look At X", "DESCRIPTION": "Manual look-at X position."},
        {"NAME": "lookAtY", "TYPE": "float", "DEFAULT": 1.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Look At Y", "DESCRIPTION": "Manual look-at Y position."},
        {"NAME": "lookAtZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Look At Z", "DESCRIPTION": "Manual look-at Z position."},
        {"NAME": "camFwdDist", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0, "LABEL": "Camera Forward Dist", "DESCRIPTION": "Adjusts camera's look-at distance."},
        {"NAME": "cameraRotationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Rotate Speed", "DESCRIPTION": "Speed of camera rotation (auto)."},
        {"NAME": "cameraRotationAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Camera Rotate Amount", "DESCRIPTION": "Amount of camera rotation (auto)."},
        {"NAME": "raymarchSteps", "TYPE": "float", "DEFAULT": 150.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "Raymarch Steps", "DESCRIPTION": "Maximum raymarch iterations."},
        {"NAME": "maxDistance", "TYPE": "float", "DEFAULT": 150.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "Max Ray Distance", "DESCRIPTION": "Maximum distance the ray can travel."},
        {"NAME": "minHitDistance", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.001, "MAX": 0.05, "LABEL": "Min Hit Distance", "DESCRIPTION": "Minimum distance to consider a hit."},
        {"NAME": "stepMultiplier", "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.5, "MAX": 1.5, "LABEL": "Step Multiplier", "DESCRIPTION": "Multiplier for distance in each raymarch step."},
        {"NAME": "logPolarScale", "TYPE": "float", "DEFAULT": 12.0, "MIN": 1.0, "MAX": 30.0, "LABEL": "Log-Polar Scale", "DESCRIPTION": "Scale for log-polar transformation."},
        {"NAME": "logPolarYSpeed", "TYPE": "float", "DEFAULT": 0.065, "MIN": 0.0, "MAX": 0.2, "LABEL": "Log-Polar Y Speed", "DESCRIPTION": "Speed of Y-offset in log-polar."},
        {"NAME": "idGridScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0, "LABEL": "ID Grid Scale", "DESCRIPTION": "Scale for the ID grid in log-polar."},
        {"NAME": "warpSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Warp Speed", "DESCRIPTION": "Speed of upward/downward warping effect."},
        {"NAME": "mainBoxSize", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.0, "LABEL": "Main Box Size", "DESCRIPTION": "Uniform size for the primary box."},

        {"NAME": "boxSubtractParamsX", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Subtract Box X", "DESCRIPTION": "X dimension for the subtracted box (animated)."},
        {"NAME": "boxSubtractParamsY", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Subtract Box Y", "DESCRIPTION": "Y dimension for the subtracted box."},
        {"NAME": "boxSubtractParamsZ", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Subtract Box Z", "DESCRIPTION": "Z dimension for the subtracted box."},

        {"NAME": "glowBeamParamsR", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0, "LABEL": "Glow Beam Radius", "DESCRIPTION": "Radius for glow beam."},
        {"NAME": "glowBeamParamsS1", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Beam Sine1 Amp", "DESCRIPTION": "Amplitude of first sine wave for glow beam."},
        {"NAME": "glowBeamParamsS2", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Beam Sine2 Amp", "DESCRIPTION": "Amplitude of second sine wave for glow beam."},
        {"NAME": "glowBeamParamsY", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Beam Y Scale", "DESCRIPTION": "Y-scale for glow beam."},
        {"NAME": "glowBeamSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 40.0, "LABEL": "Glow Beam Speed", "DESCRIPTION": "Speed for glow beam sine waves."},

        {"NAME": "mapTransformParamsX", "TYPE": "float", "DEFAULT": 17.0, "MIN": 0.0, "MAX": 50.0, "LABEL": "Map Transform X", "DESCRIPTION": "X translation in map."},
        {"NAME": "mapTransformParamsY", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Map Transform Y", "DESCRIPTION": "Y translation in map."},
        {"NAME": "mapTransformParamsZ", "TYPE": "float", "DEFAULT": 24.0, "MIN": 0.0, "MAX": 50.0, "LABEL": "Map Transform Z", "DESCRIPTION": "Z translation in map."},
        {"NAME": "mapTransformParamsW", "TYPE": "float", "DEFAULT": 9.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Map Transform YScale", "DESCRIPTION": "Final Y adjustment scale in map."},
        {"NAME": "alphaEffectSpeed", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Alpha Effect Speed", "DESCRIPTION": "Speed for alpha-related sine effects in map."},
        {"NAME": "secondLoopIterations", "TYPE": "float", "DEFAULT": 20.0, "MIN": 5.0, "MAX": 50.0, "LABEL": "Secondary Ray Iterations", "DESCRIPTION": "Number of iterations for the secondary ray effect."},
        {"NAME": "secondaryRayOffset", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Secondary Ray Z Offset", "DESCRIPTION": "Z offset for secondary ray positions."},
        {"NAME": "secondaryRaySpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Secondary Ray Speed", "DESCRIPTION": "Overall rotation and sine wave speed for secondary ray."},
        {"NAME": "secondaryRayModXRange", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Secondary Ray Mod X Range", "DESCRIPTION": "Range for modulo X in secondary ray."},

        {"NAME": "secondaryAccumParamsG1", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "LABEL": "Secondary Accum Gain1", "DESCRIPTION": "Gain factor 1 for secondary accumulation."},
        {"NAME": "secondaryAccumParamsD1", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "LABEL": "Secondary Accum Div1", "DESCRIPTION": "Divisor 1 for secondary accumulation."},
        {"NAME": "secondaryAccumParamsO", "TYPE": "float", "DEFAULT": 0.025, "MIN": 0.0, "MAX": 0.1, "LABEL": "Secondary Accum Offset", "DESCRIPTION": "Offset for secondary accumulation."},
        {"NAME": "secondaryAccumParamsD2", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0, "LABEL": "Secondary Accum Div2", "DESCRIPTION": "Divisor 2 for secondary accumulation."},
        {"NAME": "secondaryPowFactor", "TYPE": "float", "DEFAULT": 1.8, "MIN": 1.0, "MAX": 3.0, "LABEL": "Secondary Pow Factor", "DESCRIPTION": "Power factor for secondary accumulation color."},
        {"NAME": "normalSmoothness", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Normal Smoothness", "DESCRIPTION": "Offset for normal calculation (smaller=sharper)."},
        {"NAME": "edgeIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Edge Intensity", "DESCRIPTION": "Overall intensity of edge glow."},
        {"NAME": "aoStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "AO Strength", "DESCRIPTION": "Strength of Ambient Occlusion effect."},

        {"NAME": "aoSampleOffsetsX", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "AO Sample Offset X", "DESCRIPTION": "X offset for AO samples."},
        {"NAME": "aoSampleOffsetsY", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "AO Sample Offset Y", "DESCRIPTION": "Y offset for AO samples."},
        {"NAME": "aoSampleOffsetsZ", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0, "LABEL": "AO Sample Offset Z", "DESCRIPTION": "Z offset for AO samples."},

        {"NAME": "lightingPowersDiff", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Lighting Diffuse", "DESCRIPTION": "Power for diffuse component."},
        {"NAME": "lightingPowersSpec", "TYPE": "float", "DEFAULT": 12.0, "MIN": 1.0, "MAX": 30.0, "LABEL": "Lighting Specular", "DESCRIPTION": "Power for specular component."},
        {"NAME": "lightingPowersFresnel", "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.0, "MAX": 1.0, "LABEL": "Lighting Fresnel", "DESCRIPTION": "Fresnel component."},
        {"NAME": "specularColorMix", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Specular Color Mix", "DESCRIPTION": "Mix factor for specular color (with palette)."},
        {"NAME": "edgeGlowMix", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Edge Glow Mix", "DESCRIPTION": "Overall mix factor for edge glow colors."},
        {"NAME": "patternSpeed", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Pattern Speed", "DESCRIPTION": "Speed for fan/repeat pattern animation and image changes."},
        {"NAME": "uvScaleFan", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "UV Scale Fan", "DESCRIPTION": "Scale for UV coordinates before fan/repeat."},
        {"NAME": "backgroundMixBase", "TYPE": "float", "DEFAULT": 0.075, "MIN": 0.0, "MAX": 0.2, "LABEL": "Background Mix Base", "DESCRIPTION": "Base color for background mix."},
        {"NAME": "backgroundMixAmount", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Background Mix Amount", "DESCRIPTION": "Amount background mixes with raymarched color."},

        {"NAME": "glowBlendFactorsSub", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Blend Subtract", "DESCRIPTION": "Factor for subtracting glow."},
        {"NAME": "glowBlendFactorsHit", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Glow Blend On Hit", "DESCRIPTION": "Factor for mixing glow when ray hits."},
        {"NAME": "glowBlendFactorsNoHit", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Blend No Hit", "DESCRIPTION": "Factor for mixing glow when ray does not hit."},
        
        {"NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation", "DESCRIPTION": "Adjusts overall color saturation."},
        {"NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Contrast", "DESCRIPTION": "Adjusts overall image contrast."},
        {"NAME": "screenShakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Screen Shake Amount", "DESCRIPTION": "Intensity of screen shake effect."},
        {"NAME": "screenShakeSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Screen Shake Speed", "DESCRIPTION": "Speed of screen shake effect."}
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
mat2 rot(float a){return mat2(cos(a),sin(a),-sin(a),cos(a));} // Standard 2D rotation

// Centered modulo function
float pmod(float p, float x) { return mod(p, x) - 0.5 * x; }
vec2 pmod(vec2 p, float x) { return mod(p, x) - 0.5 * x; }

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
    return col * paletteMixFactor;
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
    col *= globalBrightness; 
    return col;
}


// --- SDF Functions ---

// Standard box SDF
float box(vec3 p, vec3 b){
    vec3 d = abs(p)-b;
    return max(d.x,max(d.y,d.z));
}

// Main SDF (distance field) function - now takes individual float uniforms
vec2 map(vec3 p, float in_boxSubtractParamsX, float in_boxSubtractParamsY, float in_boxSubtractParamsZ,
         float in_glowBeamParamsR, float in_glowBeamParamsS1, float in_glowBeamParamsS2, float in_glowBeamParamsY,
         float in_glowBeamSpeed,
         float in_mapTransformParamsX, float in_mapTransformParamsY, float in_mapTransformParamsZ, float in_mapTransformParamsW,
         float in_alphaEffectSpeed){
    
    vec2 a = vec2(1.0); // distance, material ID
    vec2 b = vec2(22.0); // distance, material ID
    vec3 po = p; // Store original position for later checks

    // Log-polar mapping
    vec2 p2 = p.xz;
    float r = length(p2);
    p2 = vec2(log(r),atan(p2.y,p2.x));  
    float scl = logPolarScale / PI;
   
    float t_map = time_i * logPolarYSpeed;
    p2.y += t_map;
    float yoff = 0.5;

    p2 *= scl;
   
    vec2 id = vec2(floor((p2.x)/idGridScale) + 0.5, floor(p2.y/idGridScale) + 0.5);
   
    p2 = pmod(p2, idGridScale);
   
    // Apply up/down warp-in motion
    float ring =smoothstep(6.0,9.0,id.x - t_map * scl * (2.0/3.0)) * yoff * warpSpeed * floor(mod(id.x,2.0));
    ring -= smoothstep(6.0,9.0,id.x - t_map * scl * (2.0/3.0)) * yoff * warpSpeed * floor(mod(id.x+1.0,2.0));
    p.y -= ring;
   
    float mul = r / scl; // Scale for distance
   
    vec3 p3 = vec3(p2.x, p.y / mul, p2.y);
 
    //======BEGIN NON-DOMAIN WARPED SDF======  
    // Boxes
    a.x = box(p3,vec3(mainBoxSize, mainBoxSize + 0.15, mainBoxSize));
    a.x = max(a.x, -box(p3,vec3(in_boxSubtractParamsX * cos(time_i), in_boxSubtractParamsY, in_boxSubtractParamsZ)));
   
    //anti overstep artifact hidden stuff
    vec3 d_abs_p3 = abs(p3) - 1.0;
    float outerBox = max(d_abs_p3.x, d_abs_p3.z);    
    if(length(po.xz) > 0.1) a.x = min(-outerBox, a.x);
   
    //=======END NON-DOMAIN WARPED SDF=======
    a.x *= mul; // Apply scaling based on distance from origin
   
    // Additional clamping/max conditions
    a.x = max(a.x, (length(p.xz) - 20.0));
    a.x = max(a.x, -(length(p.xz) - 0.3));

    // GLOWY BEAM THING
    b.x = length(p.xz) - in_glowBeamParamsR - 
          sin(p.y - time_i * in_glowBeamSpeed) * in_glowBeamParamsS1 -
          sin(p.y * 0.3 - time_i * in_glowBeamSpeed * 2.0) * in_glowBeamParamsS2 -
          abs(p.y) * in_glowBeamParamsY;

    // Apply material IDs: 1 for main structure, 2 for beam (material ID 22.0 from original)
    a.y = 1.0; // Material ID for main structure
    b.y = 2.0; // Material ID for glowy beam
   
    // Blend a and b, keeping material ID
    a = (a.x < b.x) ? a : b;
   
    float alpha = atan(p.z, p.x); // Polar angle in XZ plane
    p.y = abs(p.y) - in_mapTransformParamsW * cos(time_i);
 
    p -= vec3(in_mapTransformParamsX, sin(alpha * in_alphaEffectSpeed - time_i) * cos(time_i) * 2.0, in_mapTransformParamsZ * cos(time_i));

    p.y += in_mapTransformParamsY;
     
    // This `b.y` was material ID for `b`, now re-used as value. The original `b.y = 3.0*cos(TIME);`
    // was for a distance, not material ID here. Let's make it a separate `c` vector with material ID 3.
    vec2 c = vec2(3.0 * cos(time_i * in_alphaEffectSpeed), 3.0); // Distance, Material ID 3
    a = (a.x < c.x) ? a : c; // Blend again
    
    return a; // Returns vec2(distance, material_id)
}


// Normal calculation using finite differences - now takes individual float uniforms
vec3 norm(vec3 p, float s,
          float in_boxSubtractParamsX, float in_boxSubtractParamsY, float in_boxSubtractParamsZ,
          float in_glowBeamParamsR, float in_glowBeamParamsS1, float in_glowBeamParamsS2, float in_glowBeamParamsY,
          float in_glowBeamSpeed,
          float in_mapTransformParamsX, float in_mapTransformParamsY, float in_mapTransformParamsZ, float in_mapTransformParamsW,
          float in_alphaEffectSpeed){
    vec2 e= vec2(s,0);
    return normalize(vec3(
        map(p+e.xyy, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x,
        map(p+e.yxy, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x,
        map(p+e.yyx, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x) -
        vec3(map(p-e.xyy, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x,
        map(p-e.yxy, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x,
        map(p-e.yyx, in_boxSubtractParamsX, in_boxSubtractParamsY, in_boxSubtractParamsZ, in_glowBeamParamsR, in_glowBeamParamsS1, in_glowBeamParamsS2, in_glowBeamParamsY, in_glowBeamSpeed, in_mapTransformParamsX, in_mapTransformParamsY, in_mapTransformParamsZ, in_mapTransformParamsW, in_alphaEffectSpeed).x)
    );
}

// fan and repeat functions from the included source (for background image)
vec2 fan(in vec2 pos, in float q) 
{
    q = q / 180. * PI;
    float ang = atan(pos.x, pos.y);
    float len = length(pos.xy);
    ang = mod(ang + q/2., q) - q/2.;
    pos.xy = len * vec2(sin(ang), cos(ang));
    return pos;
}

/** repeat the position in 'pos' every 'q' units */
vec2 repeat(in vec2 pos, in vec2 q) 
{
    pos.xy = mod(pos.xy + q/2., q) - q/2.;
    return pos;
}

/** combination of fan and repeat calls, controlled by the 'scene' parameter. 
    scene is range 0 to 3*5*6*7*11-1
*/
vec2 fan_repeat(in vec2 uv, in int scene) 
{
    // These arrays are constants, can be exposed as indices if needed, but keeping original for now.
    uv = fan(uv, float[5](120., 90., 60., 45., 30.)[scene % 5]);
    uv = repeat(uv, vec2(
        float[3](2., 3., 4.)[scene%3],
        float[7](1., 3., 5., 2., 4., 5., 2.)[scene%7]));
    uv = fan(uv, float[11](120., 90., 60., 30., 45., 12.5, 120., 180., 60., 30., 45.)[scene % 11]);
    uv = repeat(uv, vec2(
        float[6](1., 3., 1., 4., 3., 5.)[scene%6],
        float[5](1., 4., 1., 3., 2.)[scene%5]));
    
	return uv;
}

/** a dull image with some variation through 'scene' */
vec3 img(in vec2 uv, in int scene)
{
    vec3 col = vec3(0.);
    
    // Original colors, now mixed with palette colors
    vec3 c1_base = vec3[3](
        vec3(1., .7, .4), vec3(.4, .9, .3), vec3(.2, .5, 1.)
    )[scene%3];
    vec3 c2_base = vec3[4](
        vec3(.3, .5, .6), vec3(.4, .2, .3), vec3(.1, 1., .2), vec3(.2, .5, 1.)
    )[scene%4];
    vec3 c3_base = vec3[5](
        vec3(.9, .5, .1), vec3(.4, .9, .1), vec3(.1, .6, 1.), vec3(.9, .3, 1.), vec3(.5, .5, .5)
    )[scene%5];
    
    // Mix base colors with palette colors
    vec3 palette_base = getPaletteColor(float(scene)*0.1 + time_i*0.01, time_i);
    vec3 c1 = mix(c1_base, palette_base, 0.5);
    vec3 c2 = mix(c2_base, getPaletteColor(float(scene)*0.05 + time_i*0.02, time_i), 0.5);
    vec3 c3 = mix(c3_base, getPaletteColor(float(scene)*0.08 + time_i*0.03, time_i), 0.5);

    float step1 = float[3](.01, .02, .1)[scene%3];
    float step2 = float[4](.01, .02, 0.05, .1)[scene%4];
    float step3 = float[5](.01, .02, 0.2, 0.03, .1)[scene%5];
    float step4 = float[6](.01, .02, 0.03, 0.04, 0.05, .1)[scene%6];
    
    col += c1 * smoothstep(step2,0., length(uv.xy) - float[3](0., 0.1, .2)[scene%3]);
    col += c1 * smoothstep(step1,0., abs(uv.y-.6)-0.02);
    col += c2 * smoothstep(step4,0., abs(uv.x)-0.02);
    col += c3 * smoothstep(step3,0., abs(uv.y-.3)-0.02);
    
    return col;
}


void main() {
    time_i = TIME * masterSpeed; // Global time variable

    // Populate the dynamic palette array
    for (int j = 0; j < 7; j++) {
        dynamicPalette[j] = generatePsychedelicColor(float(j) / 7.0, time_i, paletteSelect);
    }

    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 col = vec3(0.0); // Main output color
    vec3 glow = vec3(0.0); // Reset glow per frame

    // --- Camera Setup ---
    vec3 ro; // Ray Origin
    vec3 lk; // Look At

    if (enableAutoCamera) {
        ro = vec3(10.0, 10.0, -20.0);
        ro.xz *= rot(time_i * cameraRotationSpeed * 0.1); // Apply rotation to camera position
        lk = vec3(0.0, 1.0, 0.0);
    } else {
        ro = vec3(camPosX, camPosY, camPosZ);
        lk = vec3(lookAtX, lookAtY, lookAtZ);
    }

    vec3 f = normalize(lk - ro); // Forward vector
    vec3 r_base = normalize(cross(vec3(0.0, 1.0, 0.0), f)); // Right vector (using Y-axis as up)
    // Adjust up vector if f is too close to vertical to prevent cross product issues
    if (abs(dot(f, vec3(0.0,1.0,0.0))) > 0.999) {
        r_base = normalize(cross(vec3(0.0, 0.0, 1.0), f)); // Use Z-axis as up for cross product
    }
    vec3 u_vec_base = cross(f, r_base); // Up vector

    // Apply additional camera rotation (original shader had this on 'r' after it was defined)
    // This rotates the entire camera basis
    float cam_rot_angle_offset = time_i * cameraRotationSpeed * cameraRotationAmount;
    mat2 cam_rot_mat = rot(cam_rot_angle_offset);
    vec3 r = r_base; 
    r.yz = cam_rot_mat * r_base.yz;
    vec3 u_vec = u_vec_base;
    u_vec.yz = cam_rot_mat * u_vec_base.yz;

    vec3 rd = normalize(f * camFwdDist + uv.x * r + uv.y * u_vec); // Ray Direction
    
    float dO = 0.0; // Total raymarch distance
    bool hit = false;
    vec2 d_map_result; // Stores vec2(distance, material_id) from map function
    vec3 p_raymarch_pos; // Current position in raymarch

    // --- First Raymarch Loop (Main SDF) ---
    for(float i = 0.0; i < raymarchSteps; i++){
        p_raymarch_pos = ro + rd * dO;
        // Pass all relevant uniform float parameters to the map function
        d_map_result = map(p_raymarch_pos, 
                           boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ,
                           glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY,
                           glowBeamSpeed,
                           mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW,
                           alphaEffectSpeed); 
        
        dO += d_map_result.x * stepMultiplier;
       
        if(abs(d_map_result.x) < minHitDistance){
            hit = true;
            break;
        }
        if(dO > maxDistance){
            dO = maxDistance; 
            break;
        }
    }

    // --- Second Loop (Accumulation/Volumetric Effect) ---
    float t_accum = time_i; // Use time_i for the secondary loop
    vec4 O_accum = vec4(0,0,0,1); // Accumulator for secondary effect
    
    vec2 frag_coords_scaled = gl_FragCoord.xy; // Using gl_FragCoord directly
    
    for (float i_second = 0.0; i_second < secondLoopIterations; i_second++) {
        vec3 pp_second = vec3(frag_coords_scaled + frag_coords_scaled - RENDERSIZE.xy, RENDERSIZE.y);
        pp_second = normalize(pp_second) * t_accum;
        pp_second.z -= secondaryRayOffset;
        
        float k_second = time_i / 8.0 * secondaryRaySpeed;

        // Applying the R macro's logic, assuming it's a simple rotation on the relevant plane
        // R(pp.xz, k)
        float angle_xz = round((atan(pp_second.z, pp_second.x) + k_second) * 1.91) / 1.91 - k_second;
        pp_second.xz = rot(angle_xz) * pp_second.xz;

        // R(pp.zy, k + k)
        float angle_zy = round((atan(pp_second.y, pp_second.z) + k_second * 2.0) * 1.91) / 1.91 - (k_second * 2.0);
        pp_second.zy = rot(angle_zy) * pp_second.zy;
        
        float dd_second = length(pp_second) - sin(k_second * 0.5) * 0.5 - 0.4; // Original value
        
        pp_second.y += sin(pp_second.x * secondaryRaySpeed + k_second * 4.0) * sin(k_second) * 0.3; // Original value

        pp_second.x = mod(pp_second.x + k_second * 8.0, secondaryRayModXRange) - secondaryRayModXRange / 2.0;
        
        t_accum += dd_second = min(dd_second, length(pp_second.yz) - 0.03) * 0.5;
        
        // Accumulate color
        vec4 cos_color_terms = cos(t_accum - k_second + vec4(0, 1, 3, 0));
        vec3 palette_effect = getPaletteColor(t_accum + k_second * 0.5, time_i);
        
        // Correctly use individual float secondaryAccumParams
        O_accum.rgb += secondaryAccumParamsG1 * cos_color_terms.rgb / (length(pp_second) - secondaryAccumParamsD1) * palette_effect;
        O_accum.rgb += (secondaryAccumParamsO + sin(k_second) * 0.01) / (secondaryAccumParamsD2 + dd_second * 24.0) * palette_effect; // Original value 24.0
    }

    // --- Shading if Hit ---
    if(hit){
        // Pass all relevant uniform float parameters to the norm function
        vec3 n = norm(p_raymarch_pos, normalSmoothness, 
                      boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ,
                      glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY,
                      glowBeamSpeed,
                      mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW,
                      alphaEffectSpeed);
        float edge = length(n - norm(p_raymarch_pos, normalSmoothness * 6.0, 
                                     boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ,
                                     glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY,
                                     glowBeamSpeed,
                                     mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW,
                                     alphaEffectSpeed)) * min(edgeIntensity, length(p_raymarch_pos) * 0.2); // Simplified second offset & scale
        
        // Base colors based on material ID from map function
        if(d_map_result.y == 1.0) col = getPaletteColor(0.1, time_i); // Material 1
        else if(d_map_result.y == 2.0) col = getPaletteColor(0.6, time_i); // Material 2 (glowy beam)
        else if(d_map_result.y == 3.0) col = getPaletteColor(0.3, time_i); // Material 3 (last box)
        else col = getPaletteColor(0.8, time_i); // Default/fallback

        // Ambient Occlusion
        // Correctly use individual float aoSampleOffsets
        float ao = smoothstep(-.1,.1,map(p_raymarch_pos+n*aoSampleOffsetsX, boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ, glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY, glowBeamSpeed, mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW, alphaEffectSpeed).x)*
                   smoothstep(-.3,.3,map(p_raymarch_pos+n*aoSampleOffsetsY, boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ, glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY, glowBeamSpeed, mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW, alphaEffectSpeed).x)*
                   smoothstep(-.7,.7,map(p_raymarch_pos+n*aoSampleOffsetsZ, boxSubtractParamsX, boxSubtractParamsY, boxSubtractParamsZ, glowBeamParamsR, glowBeamParamsS1, glowBeamParamsS2, glowBeamParamsY, glowBeamSpeed, mapTransformParamsX, mapTransformParamsY, mapTransformParamsZ, mapTransformParamsW, alphaEffectSpeed).x);
        ao *= aoStrength;
        
        vec3 r_reflect = reflect(rd, n); // Reflected ray for specular
        float diff = length(sin(n*3.0)*.7+.3)/sqrt(3.0); // Diffuse component
        float spec = length(sin(r_reflect*3.0)*.5+.5)/sqrt(3.0); // Specular component
        // Correctly use individual float lightingPowers
        float fres = 1.0 - abs(dot(rd,n)) * lightingPowersFresnel; // Fresnel component
        
        // Combine lighting
        vec3 specular_color = mix(vec3(0.463,0.490,0.898), getPaletteColor(0.4, time_i), specularColorMix);
        // Correctly use individual float lightingPowers
        col = col * pow(diff, lightingPowersDiff) + pow(spec, lightingPowersSpec) * fres * specular_color;
        col *= ao; // Apply AO

        // Edge Glow
        if(d_map_result.y == 1.0) col += smoothstep(0.0, 0.1, edge) * mix(vec3(0.188,0.804,0.910), getPaletteColor(0.2, time_i), edgeGlowMix);
        if(d_map_result.y == 3.0) col += smoothstep(0.0, 0.15, edge) * mix(vec3(0.659,0.188,0.910), getPaletteColor(0.7, time_i), edgeGlowMix) * 10.0;
    }

    O_accum.rgb *=  pow(O_accum.rgb, vec3(secondaryPowFactor));

    // --- Fan/Repeat Image (Background Pattern) ---
    vec2 uv_fan = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv_fan *= uvScaleFan;
    
    int fan_scene_idx = int(time_i * patternSpeed * 3.0);
    uv_fan = fan_repeat(uv_fan, fan_scene_idx);
    
    int img_scene_idx = int(time_i * patternSpeed * 6.0);
    vec3 col3_background = img(uv_fan, img_scene_idx);

    // --- Final Color Mixing ---
    vec3 background_base_color = mix(vec3(backgroundMixBase), vec3(0.0), length(uv));
    col = mix(col, background_base_color, dO / maxDistance);

    // Correctly use individual float glowBlendFactors
    col = col3_background * 0.5 * (hit ? 0.0 : 1.0) +
          col * (hit ? backgroundMixAmount : 0.0) +
          col * (-glow * glowBlendFactorsSub) * (1.0 - O_accum.a) + // Adjusted O.a influence
          (glow + col) * O_accum.rgb * (hit ? glowBlendFactorsHit : glowBlendFactorsNoHit);

    // --- Color Pulse Overlay ---
    if (pulseLineIntensity > 0.001) {
        float pulse_coord;
        if (pulseLineDirection < 0.5) { // Horizontal
            pulse_coord = gl_FragCoord.y / RENDERSIZE.y;
        } else if (pulseLineDirection < 1.5) { // Vertical
            pulse_coord = gl_FragCoord.x / RENDERSIZE.x;
        } else if (pulseLineDirection < 2.5) { // Diagonal A (bottom-left to top-right)
            pulse_coord = (gl_FragCoord.x + gl_FragCoord.y) / (RENDERSIZE.x + RENDERSIZE.y); 
        } else { // Diagonal B (top-left to bottom-right)
            pulse_coord = (gl_FragCoord.x - gl_FragCoord.y) / (RENDERSIZE.x + RENDERSIZE.y);
        }

        float pulse_wave = sin(pulse_coord * 20.0 + time_i * pulseLineSpeed * 5.0);
        float pulse_alpha = smoothstep(0.5 - pulseLineThickness, 0.5 + pulseLineThickness, fract(pulse_wave * 0.5 + 0.5));
        
        vec3 pulse_color = getPaletteColor(pulse_coord * 5.0 + time_i * 0.8, time_i); // Use palette for pulse color
        col = mix(col, pulse_color, pulse_alpha * pulseLineIntensity);
    }

    // --- Screen Shake (applied as a final offset) ---
    if (screenShakeAmount > 0.001) {
        float shake_x = sin(time_i * screenShakeSpeed * 15.0) * cos(time_i * screenShakeSpeed * 10.0) * screenShakeAmount;
        float shake_y = cos(time_i * screenShakeSpeed * 12.0) * sin(time_i * screenShakeSpeed * 18.0) * screenShakeAmount;
        col += vec3(shake_x, shake_y, 0.0) * 0.5; // Apply as a color shift/ghosting effect
    }

    // --- Final Color Adjustments ---
    col = applyColorAdjustments(col);
    
    gl_FragColor = vec4(col, 1.0);
}