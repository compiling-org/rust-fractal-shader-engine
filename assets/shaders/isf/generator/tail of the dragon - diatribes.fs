/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Generative"
    ],
    "DESCRIPTION": "A psychedelic 'Tail of the Dragon' effect, with colorful, tunable parameters for a vibrant and morphing visual experience.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "ZoomLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Zoom Level" },
        { "NAME": "TwistStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Twist Strength" },
        { "NAME": "RotationStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Global Rotation" },
        { "NAME": "TurbulenceStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Turbulence Strength" },
        { "NAME": "GeometryDensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Geometry Density" },
        { "NAME": "ColorPaletteMode", "TYPE": "long", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette", "LABELS": ["Rainbow Flow", "Acid Trip", "Monochromatic Pulse", "Vaporwave"] },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "HueShiftSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Global Hue Shift" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation" },
        { "NAME": "Value", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Glow Intensity" }
    ]
}
*/

// 2D rotation matrix
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// Custom tanh approximation (for tonemapping)
float custom_tanh(float x) {
    return x / (1.0 + abs(x));
}

// Converts HSV (Hue, Saturation, Value) color to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    float time = TIME * Speed;

    // Normalized fragment coordinates (-1 to 1, aspect ratio corrected)
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    vec4 finalColor = vec4(0.0);
    float currentRayDistance = 0.0; // Accumulated raymarch distance
    
    // **DEBUG BACKGROUND**
    // If you see this bright magenta, the shader is running, but the raymarch
    // isn't producing visible results. If you still see nothing, the issue
    // is with the ISF host or how the shader is being rendered externally.
    finalColor = vec4(1.0, 0.0, 1.0, 1.0); // Bright Magenta

    // Raymarch loop - increased steps for more detail and reach
    for (int i = 0; i < 150; i++) { // Increased iterations
        // Current ray position in 3D space
        // ZoomLevel moves the camera along the Z-axis
        // Default `30.0` adjusted to `20.0` to bring geometry closer
        vec3 p_ray = vec3(uv * currentRayDistance, currentRayDistance + 20.0 + ZoomLevel * 10.0); 

        // Apply Z-axis twist
        p_ray.xy *= rot(p_ray.z * 0.1 * TwistStrength);
        // Apply global rotation over time
        p_ray.xy *= rot(time * RotationStrength);

        // Apply turbulence/perturbation to the ray position
        p_ray += cos(time + p_ray.yzx * (0.7 + sin(time) * 0.7)) * TurbulenceStrength;

        // Calculate the distance field value 's'
        // FIX: The `dot` function expects two vectors. `vec3(0.1)` creates a vector for the dot product.
        float s = cos(cos(p_ray.x * GeometryDensity) - cos(p_ray.y * GeometryDensity));
        s += abs(dot(sin(p_ray * 8.0 * GeometryDensity), vec3(0.1))); 

        // Step distance for the next iteration
        float stepSize = 0.6 * abs(s) + 0.03;
        stepSize = clamp(stepSize, 0.01, 5.0); 

        currentRayDistance += stepSize;

        // Accumulate color only if we are relatively close to a 'surface'
        if (stepSize < 0.5) { // Threshold for color accumulation, can be tweaked
            float hitBrightness = 1.0 / (stepSize * 5.0 + 0.01); // Stronger brightness from smaller stepSize
            hitBrightness = clamp(hitBrightness, 0.0, 50.0); // Clamp to avoid extreme values

            // Apply color palette
            vec3 paletteColor;
            float pulseFactor = 0.5 + 0.5 * sin(time * ColorPulseSpeed + length(p_ray) * 0.1);
            float currentHue = fract(HueShiftSpeed * time + pulseFactor * 0.2);

            if (int(ColorPaletteMode) == 0) { // Rainbow Flow
                paletteColor = hsv2rgb(vec3(currentHue, Saturation, Value));
            } else if (int(ColorPaletteMode) == 1) { // Acid Trip
                float trippyHue = fract(currentHue + sin(length(p_ray) * 0.5 + time * 3.0) * 0.3);
                paletteColor = hsv2rgb(vec3(trippyHue, 1.0, 0.5 + pulseFactor * 0.5));
                paletteColor = mix(paletteColor, vec3(sin(time * 7.0 + length(p_ray) * 0.2)), 0.3);
            } else if (int(ColorPaletteMode) == 2) { // Monochromatic Pulse
                paletteColor = hsv2rgb(vec3(currentHue, 0.3 + pulseFactor * 0.7 * Saturation, 0.8 + pulseFactor * 0.2 * Value));
            } else if (int(ColorPaletteMode) == 3) { // Vaporwave
                vec3 vaporwaveHues = vec3(0.85, 0.6, 0.75);
                float hueMix = fract(time * 0.5 + length(p_ray) * 0.05);
                paletteColor = hsv2rgb(vec3(mix(vaporwaveHues.x, vaporwaveHues.y, sin(hueMix * 3.14159) * 0.5 + 0.5), Saturation, Value));
                paletteColor = mix(paletteColor, hsv2rgb(vec3(vaporwaveHues.z, Saturation, Value)), abs(sin(time * 2.5)));
            } else {
                paletteColor = hsv2rgb(vec3(currentHue, Saturation, Value));
            }

            // Accumulate color with higher intensity
            finalColor.rgb += paletteColor * hitBrightness * GlowIntensity * 0.05;

            // Optional: If you want to stop raymarching once a significant hit is found
            // if (stepSize < 0.05) break; 
        }

        // Break if we've raymarched too far
        if (currentRayDistance > 300.0) break; // Increased max distance
    }

    // Apply custom tonemap component-wise
    finalColor.r = custom_tanh(finalColor.r / 5.0);
    finalColor.g = custom_tanh(finalColor.g / 5.0);
    finalColor.b = custom_tanh(finalColor.b / 5.0);

    gl_FragColor = finalColor;
}