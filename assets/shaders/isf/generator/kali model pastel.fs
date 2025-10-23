/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Fractal",
        "Tunnel"
    ],
    "DESCRIPTION": "A mesmerizing organic tunnel based on Kali's shader. Enhanced with extensive psychedelic tunable parameters, including dynamic space warping, fractal shape controls, and multiple color palettes.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Zoom Factor" },
        { "NAME": "CameraZStart", "TYPE": "float", "DEFAULT": -8.0, "MIN": -20.0, "MAX": 0.0, "LABEL": "Cam Z Start" },
        { "NAME": "CameraRotationSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Cam Rot Speed" },
        { "NAME": "CameraRotationOffset", "TYPE": "float", "DEFAULT": -2.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Cam Rot Offset" },
        { "NAME": "WarpScale", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 3.0, "LABEL": "Warp Scale" },
        { "NAME": "WarpIntensity", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "LABEL": "Warp Intensity" },
        { "NAME": "RotationFrequencyY", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Rot Freq Y" },
        { "NAME": "RotationFrequencyX", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Rot Freq X" },
        { "NAME": "InnerSphereRadius", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Inner Sphere Rad" },
        { "NAME": "OuterSphereRadius", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Outer Sphere Rad" },
        { "NAME": "SinPatternFrequency", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 10.0, "LABEL": "Sin Pattern Freq" },
        { "NAME": "SinPatternStrength", "TYPE": "float", "DEFAULT": 0.27, "MIN": 0.0, "MAX": 1.0, "LABEL": "Sin Pattern Str" },
        { "NAME": "SecondSphereRadius", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 3.0, "LABEL": "Second Sphere Rad" },
        { "NAME": "DEOutputScale", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0, "LABEL": "DE Output Scale" },
        { "NAME": "DetailThreshold", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.00001, "MAX": 0.01, "LABEL": "Detail Threshold" },
        { "NAME": "MaxMarchDistance", "TYPE": "float", "DEFAULT": 45.0, "MIN": 10.0, "MAX": 100.0, "LABEL": "Max March Dist" },
        { "NAME": "MarchSteps", "TYPE": "float", "DEFAULT": 150.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "March Steps" },
        { "NAME": "NormalDetailScale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Normal Detail Scale" },
        { "NAME": "AmbientLight", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Ambient Light" },
        { "NAME": "DiffuseIntensity", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0, "LABEL": "Diffuse Intensity" },
        { "NAME": "ColorMixFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Mix Factor" },
        { "NAME": "ReflectionMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Reflection Mix Factor" },
        { "NAME": "ReflectionOffset", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "Reflection Offset" },
        { "NAME": "ReflectionPatternInner", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Reflect Pattern Inner" },
        { "NAME": "ReflectionPatternOuter", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.5, "MAX": 3.0, "LABEL": "Reflect Pattern Outer" },
        { "NAME": "BackgroundStripeFrequency", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5, "LABEL": "BG Stripe Freq" },
        { "NAME": "BackgroundStripeBrightness", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "BG Stripe Bright" },
        { "NAME": "BackgroundSmoothstepMin", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5, "LABEL": "BG Smoothstep Min" },
        { "NAME": "BackgroundSmoothstepMax", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.5, "LABEL": "BG Smoothstep Max" },
        { "NAME": "DistanceGlowIntensity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Distance Glow Int" },
        { "NAME": "DistanceGlowColorHue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Distance Glow Hue" },
        { "NAME": "DistanceGlowColorSat", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Distance Glow Sat" },
        { "NAME": "DistanceGlowColorVal", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Distance Glow Val" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "VignetteOuter", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Vignette Outer" },
        { "NAME": "VignetteInner", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Vignette Inner" },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool", "3.0": "Vibrant Neon", "4.0": "Deep Space", "5.0": "Dreamy Pastel" } },
        { "NAME": "PaletteMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Mix Factor" }
    ]
}
*/


#ifdef GL_ES
precision highp float;
#endif

#ifndef PI
#define PI 3.14159265359
#endif

#ifndef TAU
#define TAU (2.0 * PI)
#endif

// Define MAX iteration counts as constants for loops
#define MAX_MARCH_STEPS_CONST 300 // Corresponds to MarchSteps MAX

float det_val = 0.0; // Renamed from 'det' to avoid conflict with uniform 'DetailThreshold'
vec3 pos; // Stores transformed position from de() for use in shade()

mat2 rot(float a){
    float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

// --- Color Conversion Functions ---

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Custom hue function for multiple color palettes
vec3 getPaletteColor(float h_val, float palette_type) {
    vec3 color_base;

    h_val = mod(h_val, 1.0); // Ensure hue is within [0, 1) range

    if (palette_type < 0.5) { // Palette 0: Classic Psychedelic (sin waves)
        color_base = clamp(abs(sin(h_val * TAU + vec3(0.0, 2.0, 4.0))), 0.0, 1.0);
    } else if (palette_type < 1.5) { // Palette 1: Warm Hues (Reds, Oranges, Yellows)
        h_val = mod(h_val, 1.0) * 0.3 + 0.0; // Restrict hue to warm range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 2.5) { // Palette 2: Cool Tones (Blues, Purples, Greens)
        h_val = mod(h_val, 1.0) * 0.3 + 0.6; // Restrict hue to cool range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 3.5) { // Palette 3: Vibrant Neon
        color_base = hsv2rgb(vec3(h_val, 0.9, 0.8)); // Static saturation/value for vibrant
    } else if (palette_type < 4.5) { // Palette 4: Deep Space (Darker blues/purples with bright accents)
        color_base = hsv2rgb(vec3(mod(h_val * 0.5 + 0.6, 1.0), 0.7, 0.3)); // Base deep color
        color_base = mix(color_base, hsv2rgb(vec3(mod(h_val * 1.5 + 0.1, 1.0), 0.8, 0.9)), 0.2); // Static vibrant accents
    } else { // Palette 5: Dreamy Pastel
        color_base = hsv2rgb(vec3(h_val, 0.3, 0.7)); // Static saturation/value for pastel
    }

    return color_base;
}


float de(vec3 p) {
    // Space Warp & Morphing Control
    p *= WarpScale - length(sin(p + TIME * GlobalSpeed)) * WarpIntensity;
    p.xz *= rot(TIME * GlobalSpeed + p.y * RotationFrequencyY);
    p.yz *= rot(TIME * GlobalSpeed + p.x * RotationFrequencyX);
    
    pos = p; // Pass transformed position for shading
    
    float d = length(p) - InnerSphereRadius; // Inner sphere
    d = max(d, -length(p) + OuterSphereRadius); // Outer sphere (shell)
    
    // Fractal/Pattern effect
    vec3 s = sin(p * SinPatternFrequency);
    d += length(s * s) * SinPatternStrength;
    
    d = min(d, length(p) - SecondSphereRadius); // Another sphere subtraction
    
    return d * DEOutputScale; // Final scale for the DE
}

vec3 normal(vec3 p) {
    vec2 e = vec2(0., DetailThreshold * NormalDetailScale); // Use uniforms
    return normalize(vec3(de(p + e.yxx), de(p + e.xyx), de(p + e.xxy)) - de(p));
}

vec3 shade(vec3 p, vec3 dir, vec3 n) {
    vec3 ldir=normalize(vec3(1.,1.,-1.)); // Fixed light direction
    float amb = AmbientLight; // Tunable ambient light
    float dif = max(0., dot(ldir,n)) * DiffuseIntensity; // Tunable diffuse intensity
    
    // Original psychedelic color calculation from position
    vec3 base_col_pattern = abs(mix(sin(pos), cos(pos), pos.x * ColorMixFactor)); // Tunable mix factor

    // Derive a hue value for the palette from the base pattern
    // Using sum of components for a dynamic hue source
    float hue_source = fract(dot(base_col_pattern, vec3(0.333))); 
    vec3 palette_col = getPaletteColor(hue_source, PaletteChoice);

    // Mix the original pattern color with the palette color
    vec3 final_object_color = mix(base_col_pattern, palette_col, PaletteMixFactor);
    
    return (amb + dif) * final_object_color;
}

vec3 march(vec3 from, vec3 dir) {
    float td = 0.,d=0.,ref=0.; // Renamed from 'det'
    vec3 col=vec3(0.),colref=col,p=from;
    
    for(int i=0; i<MAX_MARCH_STEPS_CONST; i++){ // Loop with constant max and uniform check
        if (float(i) >= MarchSteps) break; // Check against tunable MarchSteps

        p += d * dir;
        d = de(p);
        
        // Use DetailThreshold and MaxMarchDistance uniforms
        if ((d < DetailThreshold && ref > 0.) || td > MaxMarchDistance) break;
        
        // Reflection condition
        if (d < DetailThreshold && ref < 1.0 && (length(fract(pos) - 0.5) < ReflectionPatternInner || length(pos) > ReflectionPatternOuter)) { // Tunable pattern thresholds
            ref = 1.0;
            vec3 n = normal(p);
            colref += shade(p,dir,n);
            dir = reflect(dir,n);
            p += dir * ReflectionOffset; // Tunable reflection offset
        }
        td += d;
    }
    
    // Background / Sky color
    if (d > DetailThreshold) { // If ray didn't hit anything
        td = MaxMarchDistance; // Set total distance to max for background
        p = dir * MaxMarchDistance; // Project point to background
        
        // Striped background (tunable frequency, brightness, smoothstep ranges)
        vec3 stripe_color_y = getPaletteColor(fract(p.y * BackgroundStripeFrequency), PaletteChoice);
        vec3 stripe_color_x = getPaletteColor(fract(p.x * BackgroundStripeFrequency + 0.5), PaletteChoice); // Offset X stripe hue
        
        col += stripe_color_y * smoothstep(BackgroundSmoothstepMin, BackgroundSmoothstepMax, abs(fract(p.y * BackgroundStripeFrequency) - 0.5)) * BackgroundStripeBrightness;
        col += stripe_color_x * smoothstep(BackgroundSmoothstepMin, BackgroundSmoothstepMax, abs(fract(p.x * BackgroundStripeFrequency) - 0.5)) * BackgroundStripeBrightness;
    } else { // If ray hit something
        vec3 n = normal(p);
        col = shade(p,dir,n); // Shade the hit surface
    }
    
    // Mix with reflections
    col = mix(col, colref, ref * ReflectionMixFactor); // Tunable reflection mix factor
    
    // Distance Fog/Glow
    vec3 glow_color = hsv2rgb(vec3(DistanceGlowColorHue, DistanceGlowColorSat, DistanceGlowColorVal));
    col = mix(col, glow_color, td / MaxMarchDistance * DistanceGlowIntensity); // Tunable intensity and color
    
    return col;
}

void main() {
    float t_scaled = TIME * GlobalSpeed; // Use GlobalSpeed for time scaling

    vec2 uv = (gl_FragCoord.xy-RENDERSIZE.xy*.5)/RENDERSIZE.y;
    vec2 uv2 = gl_FragCoord.xy/RENDERSIZE.xy-0.5; // For vignette

    vec3 from = vec3(0.,0.,CameraZStart); // Tunable camera Z start
    vec3 dir = normalize(vec3(uv,ZoomFactor)); // Tunable zoom factor

    // Camera movement/rotation
    from.xz *= rot(t_scaled * CameraRotationSpeed + CameraRotationOffset); // Tunable speed and offset
    dir.xz *= rot(t_scaled * CameraRotationSpeed + CameraRotationOffset); // Same for direction
    
    vec3 col = march(from,dir);
    
    // Vignette/Edge Fade
    col *= smoothstep(VignetteOuter, VignetteInner, abs(uv2.x)); // Tunable vignette parameters
    
    // Final Global Brightness control
    gl_FragColor = vec4(col * GlobalBrightness,1.0);
}