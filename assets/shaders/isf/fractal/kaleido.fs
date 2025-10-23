/*
{
    "DESCRIPTION": "Vortex Nova: A completely re-engineered shader with distinct and highly psychedelic color palettes, new fractal controls for depth and scale, and an on/off switch for color pulsing to prevent monochromatic effects.",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Vortex", "Wormhole", "Psychedelic", "Dynamic"],
    "INPUTS": [
        { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Animation Speed" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.001, "MAX": 0.05, "LABEL": "Light Emission" },
        { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom" },
        { "NAME": "VortexTwist", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vortex Twist" },
        { "NAME": "RotationSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0, "LABEL": "Rotation Speed" },
        { "NAME": "KaleidoSymmetry", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0, "LABEL": "Kaleidoscope Symmetry" },
        { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 6.0, "LABEL": "Color Palette" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "ColorPulseEnabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse On/Off" },
        { "NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Global Color Shift" },
        { "NAME": "FractalDepth", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Fractal Detail" },
        { "NAME": "VortexScale", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0, "LABEL": "Vortex Scale" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness" },
        { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Contrast" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation" },
        { "NAME": "ShakeStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "LABEL": "Shake Strength" }
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



void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * TimeMultiplier;

    // --- Shake Effect ---
    uv += vec2(sin(time * 15.0), cos(time * 17.0)) * 0.05 * ShakeStrength;

    // --- Kaleidoscope Effect ---
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);
    angle = mod(angle, 2.0 * PI / KaleidoSymmetry);
    angle = abs(angle - PI / KaleidoSymmetry);
    uv = vec2(cos(angle), sin(angle)) * radius;

    // --- Global Rotation ---
    float rot = time * RotationSpeed;
    uv *= mat2(cos(rot), -sin(rot), sin(rot), cos(rot));

    vec3 p = vec3(uv * Zoom, time * 0.5);
    vec3 color_accum = vec3(0.0);

    for (int i = 0; i < int(FractalDepth); i++) {
        // --- Vortex Fractal Logic ---
        float twist = p.z * VortexTwist;
        p.xy = p.xy * mat2(cos(twist), -sin(twist), sin(twist), cos(twist));
        
        // Mandelbulb-like folding with new VortexScale
        float r = dot(p, p);
        p = abs(p) / r - vec3(VortexScale);

        float density = length(p.xy);
        
        // 'color_t' is now based on iteration count, not a global time value
        // The time and pulse controls are applied here for animation
        float color_t = (float(i) / FractalDepth) * (1.0 - ColorPulseEnabled) + (sin(p.z * 0.2 + time * ColorPulseSpeed) * ColorPulseEnabled);
        vec3 current_palette_color = palette(mod(color_t + ColorShift, 1.0), PaletteSelect);

        color_accum += current_palette_color * GlowIntensity / (density * density + 0.01);
        
        if (r > 20.0) break;
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
