/*
{
    "DESCRIPTION": "Vortex Nova: A completely re-engineered shader with distinct and highly psychedelic color palettes, new fractal controls for depth and scale, and an on/off switch for color pulsing to prevent monochromatic effects. This version includes new geometry and transform modes, as well as 3D camera controls.",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Vortex", "Wormhole", "Psychedelic", "Dynamic", "Raymarch"],
    "INPUTS": [
        { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.02, "MAX": 3.0, "LABEL": "Animation Speed" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.05, "LABEL": "Light Emission" },
        { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Zoom" },
        { "NAME": "VortexTwist", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vortex Twist" },
        { "NAME": "RotationSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0, "LABEL": "Rotation Speed" },
        { "NAME": "KaleidoSymmetry", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0, "LABEL": "Kaleidoscope Symmetry" },
        { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 6.0, "LABEL": "Color Palette" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "ColorPulseEnabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse On/Off" },
        { "NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Global Color Shift" },
        { "NAME": "FractalDepth", "TYPE": "float", "DEFAULT": 14.0, "MIN": 4.0, "MAX": 140.0, "LABEL": "Fractal Detail" },
        { "NAME": "VortexScale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Vortex Scale" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness" },
        { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 2.0, "LABEL": "Contrast" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation" },
        { "NAME": "ShakeStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Shake Strength" },
        { "NAME": "GeometryType", "TYPE": "float", "DEFAULT": 3, "MIN": 0, "MAX": 6, "LABEL": "Geometry Mode" },
        { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 1.8, "MIN": 0, "MAX": 5, "LABEL": "Transform Mode" },
        { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0, "LABEL": "Chaos Intensity" },
        { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0, "LABEL": "Chaos Speed" },
        { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0, "LABEL": "Chaos Mix" },
        { "NAME": "Symmetry", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0, "LABEL": "Symmetry" },
        { "NAME": "Sharpness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Sharpness" },
        { "NAME": "FalloffCurve", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 3.0, "LABEL": "Falloff Curve" },
        { "NAME": "CameraOrbit", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "LABEL": "Camera Orbit" },
        { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57, "LABEL": "Camera Pitch" },
        { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "LABEL": "Camera Roll" },
        { "NAME": "FOV", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.2, "MAX": 3.0, "LABEL": "Field of View" },
        { "NAME": "Texture", "TYPE": "image", "LABEL": "Texture" },
        { "NAME": "TextureMode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Mode" },
        { "NAME": "TextureEnabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Texture Enabled" },
        { "NAME": "TextureWarpIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Warp" }
    ]
}
*/

precision highp float;

#define PI 3.14159265359

// More vibrant, psychedelic palettes
vec3 palette(float t, float type) {
    t = fract(t);
    vec3 c;

    // Palette 1: Psychedelic Sunrise (Oranges, pinks, purples)
    if (type < 1.0) {
        c = 0.5 + 0.5 * cos(2.0 * PI * (t + vec3(0.0, 0.2, 0.4)));
        c = c + 0.5 * sin(2.0 * PI * (t*1.5 + vec3(0.1, 0.3, 0.5)));
    }
    // Palette 2: Acid Trip (High-contrast, electric colors)
    else if (type < 2.0) {
        c = 0.5 + 0.5 * cos(2.0 * PI * (t + vec3(0.67, 0.33, 0.0)));
        c = mix(c, vec3(0.0, 1.0, 0.5), abs(sin(t * PI * 4.0)));
    }
    // Palette 3: Hyperspace (Deep, cosmic purples and blues)
    else if (type < 3.0) {
        vec3 dark_blue = vec3(0.1, 0.1, 0.3);
        vec3 purple = vec3(0.8, 0.2, 0.9);
        vec3 cyan = vec3(0.2, 0.9, 0.8);
        c = mix(dark_blue, purple, sin(t*PI*2.0)*0.5+0.5);
        c = mix(c, cyan, cos(t*PI*3.0)*0.5+0.5);
    }
    // Palette 4: Rainbow Rave (Pure psychedelic energy)
    else if (type < 4.0) {
        c = 0.5 + 0.5 * cos(t * 6.2831853 + vec3(0.0, 2.0, 4.0));
    }
    // Palette 5: Trippy Dream (Soft but shifting pastels)
    else if (type < 5.0) {
        c = 0.5 + 0.5 * sin(t*1.5 + vec3(0.8, 0.6, 0.2));
        c = mix(c, vec3(0.9, 0.5, 0.9), abs(cos(t * 2.5)));
    }
    // Palette 6: Cyberpunk Glitch (Neon pink, blue, and black)
    else {
        vec3 pink = vec3(1.0, 0.0, 0.8);
        vec3 blue = vec3(0.0, 0.8, 1.0);
        c = mix(pink, blue, t);
        c = mix(c, vec3(0.0), sin(t*10.0) * 0.5 + 0.5);
    }
    return c;
}

// Helper function to convert RGB to HSV
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + 1e-5)), d / (q.x + 1e-5), q.x);
}

// Helper function to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.x, 0.0, 1.0), c.y);
}

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

// Geometry shapes for the fractal
float shapeSierpinski(vec3 p) {
    p = abs(p);
    p = p * 3.0 - vec3(1.5);
    if (p.x + p.y + p.z > 4.5) p = vec3(4.5) - p;
    return length(p);
}
float shapeOctahedron(vec3 p) {
    p = abs(p);
    return p.x + p.y + p.z;
}

// Apply different transformations to the geometry
vec3 applyTransform(vec3 p, float mode, float chaos, float sym, float chspd, float time) {
    p *= max(sym, 0.001);
    if (mode < 1.0) p = p; // No extra geometry
    else if (mode < 2.0) p = abs(p);
    else if (mode < 3.0) p += sin(p * 3.0 + time * chspd) * chaos * 0.3;
    else if (mode < 4.0) {
        p += sin(p * (1.0 + chaos * 2.0) + time * chspd) * chaos * 0.5;
        p = fract(p * 1.5) - 0.75;
    }
    if (mode >= 4.0 && mode < 6.0) {
        float a = atan(p.z, p.x);
        float r = length(p.xz);
        float spin = time * chspd * (mode < 5.0 ? 0.2 : 0.3);
        a += spin;
        p.x = cos(a) * r;
        p.z = sin(a) * r;
    }
    return p;
}

// Kaleidescope folding
vec3 kaleidoFold(vec3 p, float symmetry) {
    float angle = atan(p.y, p.x);
    float r = length(p.xy);
    float folded_angle = mod(angle, 2.0 * PI / symmetry);
    if (folded_angle > PI / symmetry) {
        folded_angle = 2.0 * PI / symmetry - folded_angle;
    }
    p.x = cos(folded_angle) * r;
    p.y = sin(folded_angle) * r;
    return p;
}

// Function to warp texture coordinates
vec3 texture_coord_warp(vec3 p, float warp_intensity, float time) {
    if (warp_intensity > 0.0) {
        p += sin(p * 5.0 + time * 2.0) * warp_intensity * 0.2;
    }
    return p;
}

vec3 get_texture_color(vec3 p, float mode, float warp_intensity, float time) {
    // Apply warping to the texture coordinates
    p = texture_coord_warp(p, warp_intensity, time);
    
    vec3 texColor = vec3(0.0);
    if (mode == 1.0) {
        // Tri-planar mapping
        vec3 n = abs(normalize(p));
        n = pow(n, vec3(4.0));
        n /= (n.x + n.y + n.z);
        vec3 x_p = texture2D(Texture, p.yz).rgb;
        vec3 y_p = texture2D(Texture, p.xz).rgb;
        vec3 z_p = texture2D(Texture, p.xy).rgb;
        texColor = x_p * n.x + y_p * n.y + z_p * n.z;
    } else if (mode == 2.0) {
        // Spherical mapping
        float u = atan(p.y, p.x) / (2.0 * PI) + 0.5;
        float v = asin(p.z) / PI + 0.5;
        texColor = texture2D(Texture, vec2(u, v)).rgb;
    }
    return texColor;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * TimeMultiplier;

    // --- Shake Effect ---
    uv += vec2(sin(time * 15.0), cos(time * 170.0)) * 0.25 * ShakeStrength;

    // Camera setup with new controls
    vec3 rd = normalize(vec3(uv * FOV, 1.0));
    rd = cameraMatrix(CameraOrbit, CameraPitch, CameraRoll) * rd;
    
    vec3 p = rd * Zoom;
    vec3 color_accum = vec3(0.0);

    // Apply kaleidoscope folding
    p = kaleidoFold(p, KaleidoSymmetry);

    // Fractal loop
    for (int i = 0; i < 256; i++) {
        if (float(i) >= FractalDepth) break;

        // Apply transformations and geometry, ensuring time is applied for animation
        p = applyTransform(p, TransformMode, ChaosIntensity, Symmetry, ChaosSpeed, time);
        
        float geo = GeometryType;
        if (geo < 1.0) p = p; // No extra geometry
        else if (geo < 2.0) p = vec3(shapeSierpinski(p));
        else if (geo < 3.0) p = vec3(shapeOctahedron(p));

        // Vortex Fractal Logic
        float twist = p.z * VortexTwist + time * RotationSpeed;
        p.xy = p.xy * mat2(cos(twist), -sin(twist), sin(twist), cos(twist));
        p = abs(p) / dot(p,p) - vec3(VortexScale);

        // Apply Chaos Mix to the position
        p = mix(p, sin(p * 3.0 + time * ChaosSpeed) * ChaosIntensity, ChaosMix);
        
        float density = length(p.xy);
        
        float color_t = (float(i) / FractalDepth) * (1.0 - ColorPulseEnabled) + (sin(p.z * 0.2 + time * ColorPulseSpeed) * ColorPulseEnabled);
        vec3 current_palette_color = palette(mod(color_t + ColorShift, 1.0), PaletteSelect);

        vec3 texture_color = get_texture_color(p, TextureMode, TextureWarpIntensity, time);
        vec3 final_iter_color = mix(current_palette_color, texture_color, TextureEnabled);

        // Use FalloffCurve and Sharpness
        color_accum += final_iter_color * GlowIntensity / pow(density * density + 0.01, FalloffCurve) * exp(-float(i) * 0.03 * Sharpness);
        
        if (dot(p,p) > 20.0) break;
    }

    // --- Final Output ---
    vec4 final_color = vec4(1.0 - exp(-color_accum), 1.0);

    // --- Apply Brightness, Contrast, and Saturation ---
    final_color.rgb = (final_color.rgb - 0.5) * Contrast + 0.5;
    final_color.rgb *= Brightness;
    vec3 hsv = rgb2hsv(final_color.rgb);
    hsv.y *= Saturation;
    final_color.rgb = hsv2rgb(hsv);

    gl_FragColor = final_color;
}
