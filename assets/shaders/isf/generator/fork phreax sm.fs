/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Fractal",
        "Distortion",
        "Abstract"
    ],
    "DESCRIPTION": "Modified for software like TouchDesigner, Synesthesia, HeavyM. Now with tunable parameters for animation, colors, and fractal properties.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "ReactivityFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Reactivity Factor" },
        { "NAME": "DistortionStrength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "LABEL": "Distortion Strength" },
        { "NAME": "GlobalSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Global Speed" },
        { "NAME": "BaseHue", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "LABEL": "Custom Base Color Hue (If Scheme 0)" },
        { "NAME": "BaseSaturation", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "LABEL": "Custom Base Color Saturation (If Scheme 0)" },
        { "NAME": "AccentHue", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Custom Accent Color Hue (If Scheme 0)" },
        { "NAME": "AccentSaturation", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.9, "LABEL": "Custom Accent Color Saturation (If Scheme 0)" },
        { "NAME": "FinalGamma", "TYPE": "float", "MIN": 1.0, "MAX": 3.0, "DEFAULT": 2.0, "LABEL": "Output Gamma" },
        { "NAME": "MaxRaymarchSteps", "TYPE": "float", "MIN": 50.0, "MAX": 500.0, "DEFAULT": 130.0, "LABEL": "Raymarch Steps" },
        { "NAME": "FractalOffsetScale", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Fractal Offset Base Scale" },
        { "NAME": "PulseFrequency", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 2.0, "LABEL": "Color Pulse Frequency" },
        { "NAME": "PulseIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Color Pulse Intensity" },
        { "NAME": "HuePulseFrequency", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 1.0, "LABEL": "Hue Shift Frequency" },
        { "NAME": "HuePulseIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.2, "LABEL": "Hue Shift Intensity" },
        { "NAME": "ColorScheme", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.0, "LABEL": "Color Scheme", "OPTIONS": ["Custom", "Cool Tones", "Warm Glow", "Vibrant Psychedelia"] },
        { "NAME": "FractalMorphFactorA", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3, "LABEL": "Fractal Core Offset" },
        { "NAME": "FractalMorphFactorB", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Fractal Recursive Offset Scale" },
        { "NAME": "FractalMorphFactorC", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.04, "LABEL": "Fractal Time Distortion" },
        { "NAME": "CameraZSpeed", "TYPE": "float", "MIN": -0.1, "MAX": 0.1, "DEFAULT": -0.01, "LABEL": "Camera Z Speed" }
    ]
}
*/

#define PI 3.14159265359
#define TAU (2.0 * PI)
#define SIN(x) (0.5 + 0.5 * sin(x))
#define S(a, b, x) smoothstep(a, b, x)

#define tt (TIME * GlobalSpeed)

mat2 rot(float x) {
    return mat2(cos(x), -sin(x), sin(x), cos(x));
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    vec3 rd = normalize(vec3(uv, 1.0 - DistortionStrength * SIN(PI + 0.25 * tt)));
    
    vec3 ro = vec3(0.0, 0.0, 0.0 + tt * CameraZSpeed); 
    
    vec3 col = vec3(0.0);

    float currentBaseHue, currentBaseSaturation;
    float currentAccentHue, currentAccentSaturation;

    // Use floor to convert float ColorScheme to integer for comparison
    if (floor(ColorScheme + 0.5) == 1.0) { // Cool Tones
        currentBaseHue = 0.6;
        currentBaseSaturation = 0.9;
        currentAccentHue = 0.4;
        currentAccentSaturation = 0.8;
    } else if (floor(ColorScheme + 0.5) == 2.0) { // Warm Glow
        currentBaseHue = 0.05;
        currentBaseSaturation = 0.9;
        currentAccentHue = 0.9;
        currentAccentSaturation = 0.85;
    } else if (floor(ColorScheme + 0.5) == 3.0) { // Vibrant Psychedelia
        currentBaseHue = 0.8;
        currentBaseSaturation = 1.0;
        currentAccentHue = 0.2;
        currentAccentSaturation = 1.0;
    } else { // Custom (ColorScheme == 0.0 or any other value)
        currentBaseHue = BaseHue;
        currentBaseSaturation = BaseSaturation;
        currentAccentHue = AccentHue;
        currentAccentSaturation = AccentSaturation;
    }

    vec3 active_base_color_rgb = hsv2rgb(vec3(currentBaseHue, currentBaseSaturation, 1.0));
    vec3 active_accent_color_rgb = hsv2rgb(vec3(currentAccentHue, currentAccentSaturation, 1.0));

    float t = 0.0;
    float d = 0.0;
    
    for (int i = 0; i < 500; i++) {
        if (i >= int(MaxRaymarchSteps)) break; 

        vec3 p = t * rd + ro;

        float len = mix(0.5 * (abs(p.x) + abs(p.y)), length(p.xz), 0.4);
        
        p.zy += vec2(0.2, 0.1) * sin(0.2 * tt + len * DistortionStrength + ReactivityFactor);
        p.xy *= mix(1.2, 0.5, SIN(len * 5.0 + 0.5 * tt + ReactivityFactor * 2.0));

        p = abs(p) - vec3(FractalMorphFactorA, FractalMorphFactorA, FractalMorphFactorA); 
        float s = 2.0; 
        
        for (int j = 0; j < 5; j++) {
            p.xy *= mix(1.05, 0.95, SIN(length(p.xy) * 0.5 + 0.1 * tt));
            
            p = abs(p - vec3(1.2, 1.3, 1.1) * FractalOffsetScale * FractalMorphFactorB) - vec3(1.0, 1.2, 1.08 + FractalMorphFactorC * sin(tt + ReactivityFactor));
            
            float d_iter = 2.5 / clamp(dot(p, p), 0.2, 2.0); 
            s *= d_iter;
            p = abs(p) * d_iter;
        }

        p.xy *= rot(0.5 * PI * SIN(tt + ReactivityFactor));

        d = (max(length(p.xz), -(length(p - ro) - 0.1))) / s; 
        
        d = max(0.0, d);
        
        t += d;

        if (d < 0.0001) { 
            float normalized_iter = float(i) / MaxRaymarchSteps;
            float current_time_offset = tt * 0.1;

            float hue_shift = sin(normalized_iter * 20.0 + current_time_offset * HuePulseFrequency * 1.5) * HuePulseIntensity +
                              sin(t * 0.05 + current_time_offset * HuePulseFrequency * 0.8) * HuePulseIntensity;
            float dynamic_hue = fract(currentBaseHue + hue_shift);

            float dynamic_saturation = mix(currentBaseSaturation, currentAccentSaturation, SIN(t * 0.03 + current_time_offset * PulseFrequency * 2.0) * PulseIntensity);
            dynamic_saturation = clamp(dynamic_saturation, 0.0, 1.0);

            float dynamic_value = mix(0.3, 1.0, SIN(float(i) * 0.08 + current_time_offset * PulseFrequency * 3.0) * PulseIntensity); 
            dynamic_value = clamp(dynamic_value, 0.0, 1.0);

            col = hsv2rgb(vec3(dynamic_hue, dynamic_saturation, dynamic_value));

            col += active_accent_color_rgb * (0.05 + 0.2 * SIN(tt * 4.0 + t * 0.2));
            col *= (0.7 + 0.3 * sin(t * 0.1 + tt * 10.0));

            break;
        }

        if (t > 200.0) break;
    }

    col = pow(col, vec3(FinalGamma)) + 0.05 * sin(TAU * (tt + ReactivityFactor));
    
    gl_FragColor.xyz = clamp(col, 0.0, 1.0);
    gl_FragColor.w = 1.0;
}
