/*
{
    "CATEGORIES": [
        "Animated",
        "Fractal",
        "Tunable",
        "Psychedelic",
        "Smooth"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/WsSBWh by Kali. Made for Newtro's Onchain Summer collective drop. Now with extensive tunable psychedelic parameters and smoothed transitions.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "iMouse", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "LABEL": "Mouse Input (Unused)" },

        
        { "NAME": "GlobalTimeScale", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.01, "MAX": 1.0, "LABEL": "Global Time Scale" },
        { "NAME": "OverallUVRotationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Overall UV Rotation Speed" },
        { "NAME": "DeformBaseSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Deform Base Speed" },
        { "NAME": "DeformTimeInfluence", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Deform Time Influence" },
        { "NAME": "DeformWaveFrequency", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5, "LABEL": "Deform Wave Frequency" },
        { "NAME": "DeformWaveTimeSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Deform Wave Time Speed" },
        { "NAME": "DeformWaveAmplitude", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Deform Wave Amplitude" },
        
        
        { "NAME": "CameraXOrbitAmp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera X Orbit Amp" },
        { "NAME": "CameraXOrbitFreq", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera X Orbit Freq" },
        { "NAME": "CameraYOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": -2.0, "MAX": 2.0, "LABEL": "Camera Y Offset" },
        { "NAME": "CameraZSpeed", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Camera Z Speed" },
        { "NAME": "CameraPerspectiveDepth", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Camera Perspective Depth" },
        { "NAME": "CameraPerspectiveWaveAmp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Cam Persp Wave Amp" },
        { "NAME": "CameraPerspectiveWaveFreq", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Cam Persp Wave Freq" },

        
        { "NAME": "FoldingModulusXY", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Folding Modulus XY" },
        { "NAME": "FoldingModulusZ", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Folding Modulus Z" },
        { "NAME": "FoldingOffsetZ", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Folding Offset Z" },
        { "NAME": "IterationScaleFactor", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Iteration Scale Factor" },
        { "NAME": "IterationClampMin", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "LABEL": "Iteration Clamp Min" },
        { "NAME": "IterationClampMax", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 5.0, "LABEL": "Iteration Clamp Max" },
        { "NAME": "IterationOffset", "TYPE": "float", "DEFAULT": 11.0, "MIN": 0.0, "MAX": 20.0, "LABEL": "Iteration Offset" },
        { "NAME": "PsychedelicWaveFreq", "TYPE": "float", "DEFAULT": 2.1, "MIN": 0.0, "MAX": 5.0, "LABEL": "Psychedelic Wave Freq" },
        { "NAME": "PsychedelicWaveAmp", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Psychedelic Wave Amp" },

      
        { "NAME": "BlendFactorF", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Blend Factor F" },
        { "NAME": "BlendFactorO", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Blend Factor O" },
        { "NAME": "BlendFactorL", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Blend Factor L" },
        { "NAME": "FinalDistanceOffset", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "LABEL": "Final Distance Offset" },
        { "NAME": "FinalDistanceScale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Final Distance Scale" },

       
        { "NAME": "BaseColorHue", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Hue" },
        { "NAME": "BaseColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Saturation" },
        { "NAME": "BaseColorValue", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Value" },

        { "NAME": "PulseColorHue", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Color Hue" },
        { "NAME": "PulseColorSaturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Color Saturation" },
        { "NAME": "PulseColorValue", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Color Value" },
        { "NAME": "PulseWidth", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.1, "LABEL": "Pulse Width" },
        { "NAME": "PulseTimeOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Time Offset" },
        { "NAME": "PulseZFrequency", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "LABEL": "Pulse Z Frequency" },

        { "NAME": "IdColorHue", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "ID Color Hue" },
        { "NAME": "IdColorSaturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "ID Color Saturation" },
        { "NAME": "IdColorValue", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "ID Color Value" },
        { "NAME": "IdMixFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "ID Color Mix Factor" },
        { "NAME": "IdBlendThreshold", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.001, "MAX": 0.1, "LABEL": "ID Blend Threshold" },

        { "NAME": "OrangeColorHue", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0, "LABEL": "Orange Color Hue" },
        { "NAME": "OrangeColorSaturation", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0, "LABEL": "Orange Color Saturation" },
        { "NAME": "OrangeColorValue", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Orange Color Value" },
        { "NAME": "OrangeMixFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Orange Color Mix Factor" },
        { "NAME": "OrangeBlendThreshold", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.001, "MAX": 0.1, "LABEL": "Orange Blend Threshold" },

        { "NAME": "ZChromaticFreq", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 50.0, "LABEL": "Z Chromatic Freq" },
        { "NAME": "ChromaticAberrationStrength", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.2, "LABEL": "Chromatic Aberration Str" },

       
        { "NAME": "MaxRaymarchIterations", "TYPE": "float", "DEFAULT": 200.0, "MIN": 50.0, "MAX": 400.0, "LABEL": "Max Raymarch Iterations" },
        { "NAME": "MaxRaymarchDistance", "TYPE": "float", "DEFAULT": 50.0, "MIN": 10.0, "MAX": 100.0, "LABEL": "Max Raymarch Distance" },
        { "NAME": "HitThreshold", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Hit Threshold" },
        { "NAME": "SurfaceOffset", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05, "LABEL": "Surface Offset" },
        { "NAME": "NormalEpsilon", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.05, "LABEL": "Normal Epsilon" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Glow Intensity" },
        { "NAME": "GlowDecay", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Glow Decay" },
        { "NAME": "AmbientBaseLight", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Ambient Base Light" },
        { "NAME": "SpecularPower", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Specular Power" },
        { "NAME": "FogColorR", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fog Color R" },
        { "NAME": "FogColorG", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fog Color G" },
        { "NAME": "FogColorB", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fog Color B" },
        { "NAME": "GlowMixFactor", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Mix Factor" },
        { "NAME": "DepthFadeFactor", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "Depth Fade Factor" },

   
        { "NAME": "OutputColorLumaMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Output Luma Mix Factor" },
        { "NAME": "OutputColorLumaMixStrength", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Output Luma Mix Strength" },
        { "NAME": "VignetteColorR", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Color R" },
        { "NAME": "VignetteColorG", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Color G" },
        { "NAME": "VignetteColorB", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Color B" },
        { "NAME": "VignetteStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Vignette Strength" },
        { "NAME": "OverallBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Overall Brightness" },
        { "NAME": "FadeInSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fade In Speed" }
    ]
}
*/

// Standard precision settings
precision highp float;

// Define PI for convenience
#define PI (acos(-1.0))

// Global variables for material ID and current color
float g_material_id = 0.0;
vec3 g_current_color;

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// 2D rotation matrix
mat2 rot(float a) { 
	float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Distance estimator function
float de(vec3 p) {
    vec3 pp = p; // Store original point for some calculations
    float sc = 1.0; // Scaling factor accumulator

    // Apply main deformation rotation with tunable parameters
    float deform_angle = pp.z * DeformBaseSpeed + TIME * DeformTimeInfluence + 
                         sin(p.z * DeformWaveFrequency + TIME * DeformWaveTimeSpeed) * DeformWaveAmplitude;
    p.xy *= rot(deform_angle);

    // Fractal folding with tunable moduli
    p.xy = abs(FoldingModulusXY - mod(p.xy, FoldingModulusXY * 2.0));
    p.z = abs(FoldingOffsetZ - mod(p.z, FoldingModulusZ * 2.0));

    vec3 cp = p; // Store a copy for different distance calculations

    // Fractal iteration loop (fixed at 2 iterations for GLSL ES 1.00 compatibility)
    for (int i = 0; i < 2; i++) {
        p.xy = abs(p.xy + 1.0) - abs(p.xy - 1.0) - p.xy;
        float s = IterationScaleFactor / clamp(dot(p, p), IterationClampMin, IterationClampMax);
        p = p * s - IterationOffset;
        sc = sc * s;
    }

    // Different distance calculations for blending
    float f = length(p.xy) / sc; // Fractal-based distance
    float o = min(length(cp.yz), length(cp.xz)); // Orthogonal distance
    float l = length(pp.xy) + cos(pp.z * PsychedelicWaveFreq) * PsychedelicWaveAmp; // Wave-like distance

    // Blend the distances using tunable factors (defaults to 1.0, so effectively min(l, min(f, o)) )
    float d = min(
        l * BlendFactorL,
        min(f * BlendFactorF, o * BlendFactorO)
    );

    // Assign material ID based on which distance is smallest, using smoothstep for smoother transitions
    g_material_id = smoothstep(0.0, IdBlendThreshold, o - d);

    // Calculate base color based on tunable HSV parameters
    g_current_color = hsv2rgb(vec3(BaseColorHue, BaseColorSaturation, BaseColorValue));

    // Add pulse effect to color, using smoothstep for smoothness
    float pulse_blend_factor = smoothstep(PulseWidth, PulseWidth + 0.01, abs(fract(TIME * GlobalTimeScale + PulseTimeOffset + pp.z * PulseZFrequency) - 0.5));
    vec3 pulse_color = hsv2rgb(vec3(PulseColorHue, PulseColorSaturation, PulseColorValue));
    g_current_color = mix(g_current_color, pulse_color, pulse_blend_factor);

    // Add ID-based color
    vec3 id_color = hsv2rgb(vec3(IdColorHue, IdColorSaturation, IdColorValue));
    g_current_color = mix(g_current_color, id_color, g_material_id * IdMixFactor);

    // Add orange material based on 'l' distance, using smoothstep
    float orange_blend_factor = smoothstep(0.0, OrangeBlendThreshold, l - d);
    vec3 orange_color = hsv2rgb(vec3(OrangeColorHue, OrangeColorSaturation, OrangeColorValue));
    g_current_color = mix(g_current_color, orange_color, orange_blend_factor * OrangeMixFactor);

    // Apply color chromatic shift based on Z position
    g_current_color.r += sin(pp.z * ZChromaticFreq * 0.1) * ChromaticAberrationStrength;
    g_current_color.b -= cos(pp.z * ZChromaticFreq * 0.1) * ChromaticAberrationStrength;
    g_current_color = clamp(g_current_color, 0.0, 1.0); // Clamp to valid color range

    return (d - FinalDistanceOffset) * FinalDistanceScale;
}


// Function to calculate normal vector using central differencing
vec3 normal(vec3 p) {
	vec3 e = vec3(0.0, NormalEpsilon, 0.0);
    return normalize(vec3(de(p + e.yxx), de(p + e.xyx), de(p + e.xxy)) - de(p));
}

// Raymarching function
vec3 march(vec3 from, vec3 dir) {
	float d_step, td = 0.0; // d_step for distance in current step, td for total distance
    vec3 p_current_pos;
    vec3 surface_color = vec3(0.0);
    vec3 accumulated_glow = vec3(0.0);

    // Raymarch loop with fixed iterations for GLSL ES 1.00 compatibility
    // MaxRaymarchIterations uniform acts as a soft limit
    const int MAX_RAYMARCH_ITERS_FIXED = 400; 

    for (int i = 0; i < MAX_RAYMARCH_ITERS_FIXED; i++) {
        // Break if we exceed the tunable max iterations
        if (float(i) >= MaxRaymarchIterations) break;

        p_current_pos = from + dir * td;
        d_step = de(p_current_pos); // Get distance from current position

        // Break if hit threshold reached or max raymarch distance exceeded
        if (d_step < HitThreshold || td > MaxRaymarchDistance) break;

        td += d_step;
        // Accumulate glow along the ray
        accumulated_glow += exp(-GlowDecay * d_step) * g_current_color * GlowIntensity; 
    }

    // If we hit a surface (distance is very small)
    if (d_step < HitThreshold) {
        p_current_pos -= dir * SurfaceOffset; // Small offset to avoid artifacts
        vec3 n = normal(p_current_pos); // Calculate normal at hit point
    	
        // Lighting: Ambient, Diffuse, Specular
        float diffuse_light = max(0.0, dot(dir, -n)); // Basic diffuse
        float specular_highlight = pow(diffuse_light, SpecularPower); // Specular highlight

        // Combine base color, diffuse, specular, and material properties
    	surface_color = (AmbientBaseLight + g_current_color + fract(p_current_pos.z * ZChromaticFreq)) * specular_highlight + g_material_id;
        
        // Chromatic aberration from original shader's 'c.rb*=rot(dir.y);'
        surface_color.r *= cos(dir.y * ChromaticAberrationStrength);
        surface_color.b *= sin(dir.y * ChromaticAberrationStrength);
    }
    
    // Mix hit color with accumulated glow and ambient fog based on depth
    vec3 ambient_fog_color = vec3(FogColorR, FogColorG, FogColorB);
   	vec3 final_marched_color = mix(ambient_fog_color, (surface_color + accumulated_glow * GlowMixFactor), exp(-DepthFadeFactor * td));

   	return final_marched_color;
}

void main() {
    // Current time scaled by global time scale
    float t_scaled_global = TIME * GlobalTimeScale;

    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    // Apply overall UV rotation based on global time
    uv *= rot(-t_scaled_global * OverallUVRotationSpeed);

    // Ray direction with tunable perspective depth and wave distortion
	vec3 dir = normalize(vec3(uv, CameraPerspectiveDepth + sin(t_scaled_global * CameraPerspectiveWaveFreq) * CameraPerspectiveWaveAmp));
    
    // Camera position with tunable orbit, offset, and advance speed
	vec3 from = vec3(
        sin(t_scaled_global * CameraXOrbitFreq) * CameraXOrbitAmp, // X-orbit
        CameraYOffset, // Y-offset
        t_scaled_global * CameraZSpeed // Z-advance
    );
    
	vec3 c = march(from, dir); // Perform raymarching

    // Post-processing: Mix with luminance
    float luma = length(c);
    c = mix(luma * vec3(OutputColorLumaMixFactor), c, OutputColorLumaMixStrength);
    
    // Post-processing: Vignette
 	c *= vec3(VignetteColorR, VignetteColorG, VignetteColorB) * exp(-VignetteStrength * length(uv));
    
    // Apply overall brightness
    c *= OverallBrightness;

    // Apply fade-in effect
    gl_FragColor = vec4(c, 1.0) * min(1.0, TIME * FadeInSpeed);
}
