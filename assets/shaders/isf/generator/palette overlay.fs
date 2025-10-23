/*
{
    "CATEGORIES": [
        "Post-Processing",
        "Color Correction"
    ],
    "DESCRIPTION": "Applies a selected color palette as an overlay to the input image, allowing for trippy or depth-oriented color grading.",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image",
            "DEFAULT": "null"
        },
        {
            "NAME": "palette_selection",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 6.0,
            "DEFAULT": 0.0,
            "LABELS": ["Depth Gray", "Depth Blue/Green", "Rainbow Swirl", "Neon Trip", "Space Nebula", "Fiery Cauldron", "Alien Flora"],
            "COMMENT": "Selects a color palette to apply to the scene (7 options)."
        },
        {
            "NAME": "palette_mix_factor",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Controls how much of the palette color is blended onto the original image (0.0=original, 1.0=full palette blend)."
        },
        {
            "NAME": "palette_strength",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "COMMENT": "Adjusts the intensity of the palette color when mixing. Increase this to make the palette more visible."
        },
        {
            "NAME": "palette_hue_offset",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Shifts the starting hue of the procedural palette for more variation."
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0,
            "COMMENT": "Adjusts the animation speed for time-based palettes."
        }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

// Procedural color palette functions (expanded to 7, with trippy and depth options)
vec3 getProceduralPalette(float hue, float type) {
    vec3 c = vec3(0.0);
    hue = fract(hue); // Ensure hue is between 0 and 1

    if (type < 0.5) { // Palette 0: Depth Gray (less colored, 3D depth oriented)
        float val = sin(hue * 3.14159 * 2.0) * 0.5 + 0.5; // Smooth sine wave
        c = vec3(val * 0.8); // Mostly grayscale
    } else if (type < 1.5) { // Palette 1: Depth Blue/Green (less colored, 3D depth oriented)
        float val = cos(hue * 3.14159 * 2.0) * 0.5 + 0.5;
        c = mix(vec3(0.1, 0.2, 0.4), vec3(0.2, 0.5, 0.5), val); // Deep blue to greenish-blue
    } else if (type < 2.5) { // Palette 2: Trippy Rainbow Swirl
        // HSV to RGB conversion
        vec3 hsv = vec3(hue, 1.0, 1.0);
        float r = abs(hsv.x * 6.0 - 3.0) - 1.0;
        float g = abs(hsv.x * 6.0 - 2.0) - 1.0;
        float b = abs(hsv.x * 6.0 - 4.0) - 1.0;
        c = clamp(vec3(r, g, b), 0.0, 1.0);
        c = mix(c, vec3(1.0) - c, sin(hue * 10.0 + TIME * 0.5) * 0.2 + 0.5); // Added trippy mix
    } else if (type < 3.5) { // Palette 3: Psychedelic Neon
        float freq = 6.0;
        c.r = abs(sin(hue * freq + 0.0));
        c.g = abs(sin(hue * freq + 2.0));
        c.b = abs(sin(hue * freq + 4.0));
        c = pow(c, vec3(2.0)) * 1.5; // Enhance neon glow
    } else if (type < 4.5) { // Palette 4: Deep Space Nebula
        c = mix(vec3(0.05, 0.0, 0.1), vec3(0.2, 0.1, 0.3), hue); // Dark purple base
        c = mix(c, vec3(0.1, 0.4, 0.6), sin(hue * 7.0) * 0.5 + 0.5); // Add blues
        c = mix(c, vec3(0.9, 0.8, 0.5), pow(sin(hue * 15.0 + TIME * 0.5), 4.0)); // Add distant yellows/stars
    } else if (type < 5.5) { // Palette 5: Fiery Cauldron
        c = mix(vec3(0.1, 0.0, 0.0), vec3(0.8, 0.1, 0.0), hue); // Dark red to orange
        c = mix(c, vec3(1.0, 0.6, 0.0), pow(hue, 2.0)); // Orange to bright yellow
        c = mix(c, vec3(0.5, 0.0, 0.0), sin(hue * 12.0) * 0.5 + 0.5); // Pulsing reds
    } else { // Palette 6: Alien Flora
        c = mix(vec3(0.1, 0.5, 0.2), vec3(0.8, 0.2, 0.9), hue); // Green to pink/purple
        c = mix(c, vec3(0.9, 1.0, 0.2), sin(hue * 9.0 - TIME * 0.5) * 0.5 + 0.5); // Yellowish highlights
    }
    return c;
}


void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Get the original color from the input image (which is OriginalShader.fs's output)
    vec4 original_color = IMG_NORM_PIXEL(inputImage, uv);

    float current_time_for_palette = TIME * speed; 
    
    // Calculate the procedural color for this pixel, including hue offset
    vec3 procedural_color = getProceduralPalette(fract(current_time_for_palette * 0.05 + length(uv) * 0.1 + palette_hue_offset), palette_selection);
    
    // Apply palette_strength to the procedural color
    procedural_color *= palette_strength;

    // A common way to apply a color tint/overlay is to multiply, then mix
    // This creates a more noticeable effect than simple linear mix for a 'palette'
    vec3 blended_color = original_color.rgb * procedural_color; 
    
    // Mix the original scene color with the newly blended color
    // palette_mix_factor of 0.0 keeps original image
    // palette_mix_factor of 1.0 applies the full blended palette
    vec3 final_color = mix(original_color.rgb, blended_color, palette_mix_factor); 

    gl_FragColor = vec4(final_color, original_color.a); 
}