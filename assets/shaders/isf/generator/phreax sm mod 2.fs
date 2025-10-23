/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Fractal",
        "Distortion",
        "Abstract",
        "Psychedelic"
    ],
    "DESCRIPTION": "Modified for software like TouchDesigner, Synesthesia, HeavyM. Now with tunable parameters for animation, colors, fractal properties, background transparency, fractal-specific flicker, and central, interacting rainbow sunlight rays.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "ReactivityFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Reactivity Factor" },
        { "NAME": "DistortionStrength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "LABEL": "Distortion Strength" },
        { "NAME": "GlobalSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 0.5, "LABEL": "Global Speed" },
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
        { "NAME": "CameraZSpeed", "TYPE": "float", "MIN": -0.1, "MAX": 0.1, "DEFAULT": -0.01, "LABEL": "Camera Z Speed" },
        { "NAME": "Transparency", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "LABEL": "Background Transparency" },
        { "NAME": "FlickerIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "LABEL": "Fractal Flicker Intensity" },
        { "NAME": "FlickerFrequency", "TYPE": "float", "MIN": 0.1, "MAX": 100.0, "DEFAULT": 10.0, "LABEL": "Fractal Flicker Frequency" },
        { "NAME": "SunRayIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "LABEL": "Sun Ray Intensity" },
        { "NAME": "SunRayMovementSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 3.0, "LABEL": "Sun Ray Pulsing Speed" },
        { "NAME": "SunRayRainbowSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.2, "LABEL": "Sun Ray Rainbow Speed" },
        { "NAME": "RayConeAngle", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "LABEL": "Sun Ray Spread/Size" },
        { "NAME": "RayDiffusionFactor", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.5, "LABEL": "Sun Ray Fractal Diffusion" }
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
    
    // Initialize output with background color and transparency
    gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0 - Transparency); // Black background with desired transparency

    vec3 col_fractal = vec3(0.0); // This will hold the color of the fractal if hit

    float currentBaseHue, currentBaseSaturation;
    float currentAccentHue, currentAccentSaturation;

    // Determine color scheme based on input
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

    float t = 0.0; // Total distance ray has traveled
    float d = 0.0; // Distance to nearest surface

    // Raymarching loop
    for (int i = 0; i < 500; i++) {
        if (i >= int(MaxRaymarchSteps)) break; 

        vec3 p = t * rd + ro; // Current point in 3D space along the ray

        // Fractal deformation logic
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
        d = max(0.0, d); // Ensure distance is non-negative
        
        t += d; // Advance the ray

        // If we hit the fractal
        if (d < 0.0001) { 
            float normalized_iter = float(i) / MaxRaymarchSteps;
            float current_time_offset = tt * 0.1;

            // Psychedelic hue variation for fractal color
            float hue_shift = sin(normalized_iter * 20.0 + current_time_offset * HuePulseFrequency * 1.5) * HuePulseIntensity +
                              sin(t * 0.05 + current_time_offset * HuePulseFrequency * 0.8) * HuePulseIntensity;
            float dynamic_hue = fract(currentBaseHue + hue_shift);

            // Pulsing saturation for fractal color
            float dynamic_saturation = mix(currentBaseSaturation * 0.5, currentAccentSaturation, SIN(t * 0.03 + current_time_offset * PulseFrequency * 2.0) * PulseIntensity);
            dynamic_saturation = clamp(dynamic_saturation, 0.0, 1.0);

            // Pulsing value/brightness for fractal color
            float dynamic_value = mix(0.1, 1.0, SIN(float(i) * 0.08 + current_time_offset * PulseFrequency * 3.0) * PulseIntensity); 
            dynamic_value = clamp(dynamic_value, 0.0, 1.0);

            // --- Apply fractal-specific flicker to its value/brightness ---
            // Flicker frequency influenced by ReactivityFactor for "flicker on movement"
            float flicker_mod = (0.5 + 0.5 * sin(tt * (FlickerFrequency + ReactivityFactor * 50.0) + (uv.x + uv.y) * 50.0));
            // Invert flicker_mod for a "fade out" effect as intensity increases
            float flicker_effect = 1.0 - FlickerIntensity * flicker_mod; 
            dynamic_value *= flicker_effect;
            dynamic_value = clamp(dynamic_value, 0.0, 1.0); // Ensure value stays in range after flicker
            // --- End fractal flicker ---

            col_fractal = hsv2rgb(vec3(dynamic_hue, dynamic_saturation, dynamic_value));

            // Apply accent color and final fractal color adjustments
            col_fractal += active_accent_color_rgb * (0.05 + 0.2 * SIN(tt * 4.0 + t * 0.2));
            col_fractal *= (0.7 + 0.3 * sin(t * 0.1 + tt * 10.0));

            // --- Add Pulsing, Flashing, Traveling Rainbow Sunlight Rays ---
            vec3 central_ray_direction = normalize(vec3(0.0, 0.0, 1.0)); // Fixed central direction for rays
            
            // Calculate cone factor for ray spread/size
            float cone_factor = dot(normalize(rd), central_ray_direction); // Use normalized ray direction for accurate dot product
            cone_factor = S(1.0 - RayConeAngle, 1.0, cone_factor); // Control spread with RayConeAngle

            // Pulsing effect for rays, linked to SunRayMovementSpeed and UV coordinates for variation
            cone_factor *= (0.5 + 0.5 * sin(tt * SunRayMovementSpeed * 5.0 + dot(uv, uv) * 10.0)); 

            // Diffusion from fractal interaction: make rays stronger closer to the fractal surface
            // This creates the effect of rays interacting with the fractal's depth/volume
            float diffusion_intensity = S(t * 0.1, t * 0.5, 1.0); // Stronger closer to camera/fractal surface
            diffusion_intensity = pow(diffusion_intensity, RayDiffusionFactor * 5.0 + 1.0); // Control falloff/intensity
            
            cone_factor *= diffusion_intensity; // Apply diffusion to the cone factor

            // Generate a rainbow color that shifts over time for the rays
            float ray_color_hue = fract(tt * SunRayRainbowSpeed);
            vec3 rainbow_col = hsv2rgb(vec3(ray_color_hue, 1.0, 1.0));

            // Add the rainbow rays to the fractal color, making them interact
            col_fractal += rainbow_col * cone_factor * SunRayIntensity;
            // --- End Sun Rays ---

            // Final color for the pixel if fractal is hit
            gl_FragColor.xyz = clamp(pow(col_fractal, vec3(FinalGamma)) + 0.05 * sin(TAU * (tt + ReactivityFactor)), 0.0, 1.0);
            gl_FragColor.w = 1.0; // Set alpha to 1.0 (fully opaque) for the fractal itself

            break; // Exit the raymarching loop as we found a surface
        }

        if (t > 200.0) break; // Max distance reached, no hit
    }
    // If the loop finishes without a hit, gl_FragColor remains its initial transparent black value.
}
