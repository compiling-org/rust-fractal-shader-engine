/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Generative"
    ],
    "DESCRIPTION": "A psychedelic firewall effect with tunable parameters for color, morphing, speed, camera motion, zoom, and geometry density.",
    "IMPORTED": {},
    "INPUTS": [
        { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "MorphStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Morphing Strength" },
        { "NAME": "CameraRotationSpeed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Orbit Speed" },
        { "NAME": "ZoomLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Zoom Level" },
        { "NAME": "GeometryScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Geometry Density" },
        { "NAME": "ColorPaletteMode", "TYPE": "long", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette", "LABELS": ["Rainbow Flow", "Acid Trip", "Monochromatic Pulse", "Vaporwave"] },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "HueShiftSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Global Hue Shift" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Saturation" },
        { "NAME": "Value", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Brightness" },
        { "NAME": "GlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Glow Intensity" }
    ]
}
*/

// Custom tanh approximation for tonemapping
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

    // Camera setup with rotation
    float camRotAngle = time * CameraRotationSpeed;
    
    // Zoom control applied to camera's Z position
    // Base Z is -10.0, then adjusted by ZoomLevel
    // Negative ZoomLevel moves camera closer, positive moves it further away
    vec3 ro = vec3(sin(camRotAngle) * 3.0, cos(camRotAngle) * 3.0, -10.0 - ZoomLevel * 5.0); // Camera origin

    vec3 rd = normalize(vec3(uv, 1.0)); // Ray direction (simple perspective)

    // Apply camera rotation to ray direction
    mat2 rot_mat = mat2(cos(camRotAngle), -sin(camRotAngle), sin(camRotAngle), cos(camRotAngle));
    rd.xy = rot_mat * rd.xy;

    vec4 finalColor = vec4(0.0);
    float z = 0.0; // Raymarch depth
    
    // Debug background color: if still black, the shader isn't running or outputting anything.
    // If you see this dark blue, the raymarcher isn't hitting or accumulating enough color.
    finalColor = vec4(0.0, 0.0, 0.1, 1.0); 

    // Raymarch loop - increased steps and max distance for better coverage
    for (int i = 0; i < 120; i++) { 
        vec3 P = z * rd + ro; // Sample point

        // Polar coordinates and additional transformations
        // Geometry control applied here: scales the '4.0' in length(P.xz) - 4.0
        // A smaller GeometryScale makes the 'hole' smaller, leading to denser geometry
        // A larger GeometryScale makes the 'hole' larger, leading to sparser geometry
        vec3 p = vec3(atan(P.z + 9.0, P.x + 0.1) * 2.0 - 0.3 * P.y, 0.6 * P.y - time, length(P.xz) - 4.0 * GeometryScale);

        // Apply turbulence and refraction effect with morphing control
        for (int j = 0; j < 9; j++) {
            p += sin(p.yzx * float(j) + time + 0.4 * float(i)) / (float(j) + 1.0) * MorphStrength;
        }

        // Distance to cylinder and waves with refraction
        float d = 0.2 * length(vec4(cos(p + P.y * 0.2) - 1.0, p.z));
        z += d;

        // Exit if max distance reached or a hit is registered (d is very small)
        if (z > 300.0) break; 
        
        // Accumulate color only when we're reasonably close to a surface (d is small)
        if (d < 1.5) { 
            // Base color components (original from Shadertoy, but these can be abstract)
            vec3 baseColorOriginal = vec3(4.0, z, 2.0); 
            
            // Simpler brightness calculation: stronger contribution the closer we are to the surface
            float brightness = 0.5 / (d + 0.01); 

            // Apply color palette
            vec3 paletteColor;
            float pulseFactor = 0.5 + 0.5 * sin(time * ColorPulseSpeed + length(P) * 0.1);
            float currentHue = fract(HueShiftSpeed * time + pulseFactor * 0.2);

            if (int(ColorPaletteMode) == 0) { // Rainbow Flow
                paletteColor = hsv2rgb(vec3(currentHue, Saturation, Value));
            } else if (int(ColorPaletteMode) == 1) { // Acid Trip
                float trippyHue = fract(currentHue + sin(length(P) * 0.5 + time * 3.0) * 0.3);
                paletteColor = hsv2rgb(vec3(trippyHue, 1.0, 0.5 + pulseFactor * 0.5));
                paletteColor = mix(paletteColor, vec3(sin(time * 7.0 + length(P) * 0.2)), 0.3);
            } else if (int(ColorPaletteMode) == 2) { // Monochromatic Pulse
                paletteColor = hsv2rgb(vec3(currentHue, 0.3 + pulseFactor * 0.7 * Saturation, 0.8 + pulseFactor * 0.2 * Value));
            } else if (int(ColorPaletteMode) == 3) { // Vaporwave
                vec3 vaporwaveHues = vec3(0.85, 0.6, 0.75);
                float hueMix = fract(time * 0.5 + length(P) * 0.05);
                paletteColor = hsv2rgb(vec3(mix(vaporwaveHues.x, vaporwaveHues.y, sin(hueMix * 3.14159) * 0.5 + 0.5), Saturation, Value));
                paletteColor = mix(paletteColor, hsv2rgb(vec3(vaporwaveHues.z, Saturation, Value)), abs(sin(time * 2.5)));
            } else {
                paletteColor = hsv2rgb(vec3(currentHue, Saturation, Value));
            }

            // Accumulate color. The '0.01' is a multiplier to control overall intensity before tonemapping.
            // You might need to tweak this and the tonemap divisor for desired look.
            finalColor.rgb += baseColorOriginal * paletteColor * brightness * GlowIntensity * 0.01; 
        }
    }

    // Apply custom tonemap to each color component
    finalColor.r = custom_tanh(finalColor.r / 10.0);
    finalColor.g = custom_tanh(finalColor.g / 10.0);
    finalColor.b = custom_tanh(finalColor.b / 10.0);

    gl_FragColor = finalColor;
}