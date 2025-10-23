/*
{
  "DESCRIPTION": "Fixed version of 4D Beats without audio input. Beat-synced recursive grid with color inversion. Now with tunable parameters for palette, color pulse, morphing, and separate background transparency.",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Abstract", "Experimental"],
  "INPUTS": [
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "BaseColor1",
      "TYPE": "color",
      "DEFAULT": [0.2, 0.7, 1.0, 1.0],
      "LABEL": "Base Color 1"
    },
    {
      "NAME": "BaseColor2",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.3, 0.8, 1.0],
      "LABEL": "Base Color 2"
    },
    {
      "NAME": "PaletteBlend",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Palette Blend"
    },
    {
      "NAME": "ColorPulseIntensity",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 0.5,
      "LABEL": "Color Pulse Intensity"
    },
    {
      "NAME": "ColorPulseFrequency",
      "TYPE": "float",
      "DEFAULT": 10.0,
      "MIN": 1.0,
      "MAX": 20.0,
      "LABEL": "Color Pulse Frequency"
    },
    {
      "NAME": "MorphingStrength",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Morphing Strength"
    },
    {
      "NAME": "BackgroundAlpha",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Background Alpha"
    },
    {
      "NAME": "ForegroundThreshold",
      "TYPE": "float",
      "DEFAULT": 500.0,
      "MIN": 0.0,
      "MAX": 5000.0,
      "LABEL": "Foreground Threshold"
    }
  ]
}
*/

precision highp float; // Set high precision for floats

// Custom tanh approximation for GLSL ES 1.00
vec4 tanh_approx(vec4 x) {
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv = gl_FragCoord.xy / res;
    vec3 rayDir = normalize(vec3(gl_FragCoord.xy - res * 0.5, res.y));
    
    // Time variable influenced by AnimationSpeed
    float T = TIME * 1.9 * AnimationSpeed;
    float F = fract(T);
    float t = floor(T) + sqrt(F);

    // Rotation matrix component, influenced by MorphingStrength
    float angle = t * (0.1 + MorphingStrength * 0.05); // Modulate rotation speed
    float ca = cos(angle);
    float sa = sin(angle);
    mat2 rot = mat2(ca, -sa, sa, ca);

    vec4 color = vec4(0.0); // Accumulated color along the ray
    vec4 U = vec4(1.0, 2.0, 3.0, 0.0); // Base color components or factors
    
    float z = 0.0;
    float d = 0.0;
    
    for (float i = 1.0; i < 77.0; i++) {
        // Z step, influenced by MorphingStrength
        z += (0.8 + MorphingStrength * 0.2) * d + 0.001;

        vec4 p = vec4(z * rayDir, 0.2);
        p.z -= 3.0;

        // Simulated 4D rotation (XY, XW, YW, ZW)
        float w = p.w;
        p.xz = rot * p.xz;
        p.yz = rot * p.yz;
        float tempX = p.x, tempW = p.w;
        p.x = ca * tempX - sa * tempW;
        p.w = sa * tempX + ca * tempW;

        float k = 9.0 / dot(p, p);
        vec4 P = p *= k;

        p -= 0.5 * t;

        // Unit lattice tiling
        p = abs(p - floor(p + 0.5));

        // Distance function
        float d1 = min(length(p.xz), min(length(p.yz), length(p.xy)));
        float d2 = length(p) - 0.2;
        float d3 = min(p.w, min(p.x, min(p.y, p.z))) + 0.05;

        d = abs(min(min(d1, d2), d3)) / k;

        // Original color calculation (vec4 `col`), now blended with tunable palette
        vec4 dynamic_color_texture = 1.0 + sin(P.z + log2(k) + U.wxyw);
        vec4 blended_color = mix(BaseColor1, BaseColor2, PaletteBlend);
        vec4 final_pixel_color = dynamic_color_texture * blended_color; // Apply tunable palette

        // Accumulate color
        color += U * exp(0.7 * k - 6.0 * F) + final_pixel_color.w * final_pixel_color / max(d, 1e-3);
    }

    // Apply color pulse effect
    float pulse = 1.0 + ColorPulseIntensity * sin(TIME * ColorPulseFrequency);
    vec4 final_rgb_output = tanh_approx(color / 10000.0) / 0.9 * pulse;
    
    // Determine alpha: 1.0 for foreground (fractal), BackgroundAlpha for true background
    // If length(color.rgb) is very small, it means the ray accumulated little color, so it's background.
    float foreground_mask = smoothstep(0.0, ForegroundThreshold, length(color.rgb));
    
    // Mix BackgroundAlpha (for background) with 1.0 (for foreground) using the mask
    gl_FragColor = vec4(final_rgb_output.rgb, mix(BackgroundAlpha, 1.0, foreground_mask));
}
