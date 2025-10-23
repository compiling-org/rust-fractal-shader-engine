/*
{
    "DESCRIPTION": "Recursive volumetric portal with palette, rotation, morph, geometry, and advanced camera controls. This version has been corrected to ensure all sliders function as intended, including chaos, geometry, and texture warping. Chaos effects are now properly blended using the ChaosMix slider.",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Psychedelic", "Volumetric"],
    "INPUTS": [
        { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.1, "MAX": 2.0, "LABEL": "Animation Speed" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0, "LABEL": "Glow" },
        { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Zoom" },
        { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 2.8, "MIN": 0.0, "MAX": 5.0, "LABEL": "Transform Mode" },
        { "NAME": "GeometryType", "TYPE": "float", "DEFAULT": 1.80, "MIN": 0.0, "MAX": 6.0, "LABEL": "Geometry Mode" },
        { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0, "LABEL": "Chaos Intensity" },
        { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0, "LABEL": "Chaos Speed" },
        { "NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 13.0, "MIN": 0.0, "MAX": 19.0, "LABEL": "Color Palette" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 2.1, "MIN": 0.0, "MAX": 3.0, "LABEL": "Brightness" },
        { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0, "LABEL": "Contrast" },
        { "NAME": "Symmetry", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0, "LABEL": "Symmetry" },
        { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.48, "MIN": 0.0, "MAX": 1.0, "LABEL": "Chaos Mix" },
        { "NAME": "Sharpness", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Sharpness" },
        { "NAME": "FalloffCurve", "TYPE": "float", "DEFAULT": 2.21, "MIN": 0.1, "MAX": 3.0, "LABEL": "Falloff Curve" },
        { "NAME": "CameraOrbit", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "LABEL": "Camera Orbit" },
        { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "LABEL": "Camera Pitch" },
        { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "LABEL": "Camera Roll" },
        { "NAME": "FocusNear", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Focus Near" },
        { "NAME": "FocusFar", "TYPE": "float", "DEFAULT": 2.6, "MIN": 0.1, "MAX": 10.0, "LABEL": "Focus Far" },
        { "NAME": "FOV", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.2, "MAX": 3.0, "LABEL": "Field of View" },
        { "NAME": "StepCount", "TYPE": "float", "DEFAULT": 15.0, "MIN": 1.0, "MAX": 128.0, "LABEL": "Fractal Steps" },
        { "NAME": "Texture", "TYPE": "image", "LABEL": "Texture" },
        { "NAME": "TextureEnabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Texture On/Off" },
        { "NAME": "TextureWarp", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Warp" },
        { "NAME": "TextureScale", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.1, "MAX": 10.0, "LABEL": "Texture Scale" },
        { "NAME": "RotationRate", "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.1, "MAX": 5.0, "LABEL": "Portal Rotation Rate" },
        { "NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 1.10, "MIN": 0.0, "MAX": 3.0, "LABEL": "Morph Amount" },
        { "NAME": "GeometryScale", "TYPE": "float", "DEFAULT": 3.50, "MIN": 1.0, "MAX": 20.0, "LABEL": "Geometry Scale" }
    ]
}
*/

precision highp float;

#define MAX_STEPS 128.0
#define BAILOUT 16.0
#define PI 3.14159265359

// Camera matrix for 3D camera controls
mat3 cameraMatrix(float orbit, float pitch, float roll) {
    float co = cos(orbit), so = sin(orbit);
    float cp = cos(pitch), sp = sin(pitch);
    float cr = cos(roll), sr = sin(roll);
    return mat3(
        co * cr + so * sp * sr, sr * cp, -so * cr + co * sp * sr,
        -co * sr + so * sp * cr, cr * cp, sr * so + co * sp * cr,
        so * cp, -sp, co * cp
    );
}

// Function to generate a wide range of palettes
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.2831 * (c * t + d));
}

vec3 getColorPalette(int mode, float t) {
    t = fract(t);
    // This allows for a massive number of palettes based on a single float input
    return pal(t,
        vec3(0.5 + 0.4*sin(float(mode)*0.5), 0.6 + 0.3*cos(float(mode)*1.2), 0.4 + 0.5*sin(float(mode)*0.9)),
        vec3(0.4),
        vec3(1.0,1.3,0.7),
        vec3(0.1,0.2,0.3)
    );
}

// Manual implementation of tanh for vec3
vec3 tanh(vec3 x) {
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

// 2D rotation matrix
mat2 rot(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

// Tri-planar texture mapping with warping
vec3 triplanarTexture(vec3 p, float scale, float warp, float time) {
    // Apply warping to coordinates
    vec3 warped_p = p + sin(p * 5.0 + time * 2.0) * warp * 0.2;
    
    vec3 blend = normalize(abs(warped_p));
    blend = pow(blend, vec3(4.0));
    blend /= dot(blend, vec3(1.0));
    
    vec2 xz = fract(warped_p.zy * scale);
    vec2 yz = fract(warped_p.xz * scale);
    vec2 xy = fract(warped_p.xy * scale);
    
    vec3 tx = texture2D(Texture, xz).rgb;
    vec3 ty = texture2D(Texture, yz).rgb;
    vec3 tz = texture2D(Texture, xy).rgb;
    
    return tx * blend.x + ty * blend.y + tz * blend.z;
}

// Applies transformations to the space
vec3 applyTransform(vec3 p, float mode, float sym) {
    p *= max(sym, 0.001);
    int intMode = int(floor(mode));
    
    if (intMode == 1) p = abs(p);
    else if (intMode == 2) p.y = abs(p.y);
    else if (intMode == 3) p = abs(p);
    
    return p;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= FOV;
    float time = TIME * TimeMultiplier;

    // Camera setup with new controls
    vec3 ro = vec3(0.0, 0.0, -3.0);
    vec3 rd = normalize(vec3(uv * Zoom, 1.0));
    rd = cameraMatrix(CameraOrbit, CameraPitch, CameraRoll) * rd;

    vec3 col = vec3(0.0);
    float dist = 0.0;
    
    for (float i = 0.0; i < MAX_STEPS; i++) {
        if (i >= StepCount) break;

        // Position along the ray
        vec3 p = ro + dist * rd;
        
        // --- Core fractal logic ---
        // Apply transformations and geometry type modifications
        p = applyTransform(p, TransformMode, Symmetry);
        
        int intGeo = int(floor(GeometryType));
        if (intGeo == 1) p = abs(p) - 0.2;
        else if (intGeo == 2) p.y = abs(p.y);
        else if (intGeo == 3) p = abs(p) - 0.3;
        
        p.z += time;
        
        // XY rotation based on z and RotationRate
        p.xy *= rot(p.z * RotationRate);
        
        // **NEW** - Apply chaos directly within the loop
        p += sin(p * 5.0 + time * ChaosSpeed) * ChaosIntensity * ChaosMix;
        
        // Morphing loop
        for (float j = 0.0; j < 3.0; j++) {
            float a = exp(j) / exp2(j);
            p += cos(4.0 * p.yzx * a + time - length(p.xy) * GeometryScale) * MorphAmount / a;
        }
        // --- End core fractal logic ---
        
        // Density calculation
        float d = 0.01 + abs((p - vec3(0, 1, 0)).y - 1.0) / 10.0;
        
        // Color accumulation
        float fade = exp(-i * 0.03 * Sharpness);
        float focus = smoothstep(FocusNear, FocusFar, dist);
        
        vec3 palCol = getColorPalette(int(ColorPaletteMode), dist * 0.1);
        vec3 texCol = triplanarTexture(p * TextureScale, 1.0, TextureWarp, time);
        
        // Mix color with texture based on TextureEnabled
        vec3 final_color = mix(palCol, texCol, TextureEnabled);
        
        // Use FalloffCurve for a more customizable effect
        float b = GlowIntensity / pow(0.01 + d * FalloffCurve, 2.0);
        col += final_color * b * fade * focus;
        
        dist += d / 4.0;
        if (dist > BAILOUT) break;
    }
    
    // Final color correction
    col = tanh(col * 0.1);
    col = (col - 0.5) * Contrast + 0.5;
    col *= Brightness;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
