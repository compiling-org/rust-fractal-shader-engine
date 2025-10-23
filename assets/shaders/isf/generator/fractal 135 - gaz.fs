/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "3D",
        "Abstract",
        "Animated",
        "Psychedelic"
    ],
    "DESCRIPTION": "An ISF conversion of a captivating fractal shader, featuring dynamic rotations and an intricate mandelbox-like structure. Enhanced with multiple psychedelic color palettes, color pulsing, and extensive tunable parameters for animation, zoom, fractal geometry, and image post-processing.",
    "CREDIT": "Original shader by Dave_Van_Dorn (https://www.shadertoy.com/view/wsGyzc), converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall speed of the animation."
        },
        {
            "NAME": "cameraZoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the camera's zoom level."
        },
        {
            "NAME": "cameraOffsetX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Horizontal camera offset."
        },
        {
            "NAME": "cameraOffsetY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Vertical camera offset."
        },
        {
            "NAME": "fractalOffsetStrength",
            "TYPE": "float",
            "DEFAULT": 0.0001,
            "MIN": 0.0,
            "MAX": 0.001,
            "STEP": 0.00001,
            "DESCRIPTION": "Strength of the iterative fractal offset (p -= i/5e4 in original)."
        },
        {
            "NAME": "boxFoldIterations",
            "TYPE": "float",
            "DEFAULT": 6.0,
            "MIN": 1.0,
            "MAX": 12.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the inner box folding fractal. Higher values increase complexity."
        },
        {
            "NAME": "boxFoldScale",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 1.0,
            "MAX": 4.0,
            "STEP": 0.01,
            "DESCRIPTION": "Scaling factor applied during box folding."
        },
        {
            "NAME": "boxFoldOffset",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Offset applied to each box fold iteration."
        },
        {
            "NAME": "detailScalingFactor",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.001,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Factor for calculating distance in the fractal (e = 0.01/dot(p,p))."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 7 predefined psychedelic color palettes."
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed at which the colors pulse and shift (0 for no pulse)."
        },
        {
            "NAME": "glowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall intensity of the glow/light."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image brightness."
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image color saturation."
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image contrast."
        },
        {
            "NAME": "vignetteIntensity",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the darkening vignette effect around the edges."
        }
    ]
}
*/

// Rodrigues' rotation formula
// R(p, a, t) = p * cos(t) + cross(a, p) * sin(t) + a * dot(a, p) * (1.0 - cos(t))
// The original macro was a simplified version that works for unit vector 'a'
// Let's ensure it's correct for a general unit vector 'a'.
// The original was: mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a)
// This is essentially: (p * cos(t)) + (a * dot(p,a) * (1-cos(t))) + (cross(p,a) * sin(t))
// This matches Rodrigues' formula if 'a' is a unit vector.
#define R(p,a,t) (mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a))

// Helper to generate a psychedelic color based on a hue input
// This function will be integrated with our defined palettes
vec3 H(float h) {
    return (cos(h * 6.3 + vec3(0, 23, 21)) * .5 + .5);
}

// Function to adjust brightness
vec3 adjustBrightness(vec3 color, float val) {
    return color * val;
}

// Function to adjust saturation
vec3 adjustSaturation(vec3 color, float val) {
    vec3 gray = vec3(dot(color, vec3(0.2126, 0.7152, 0.0722)));
    return mix(gray, color, val);
}

// Function to adjust contrast
vec3 adjustContrast(vec3 color, float val) {
    return (color - 0.5) * val + 0.5;
}


void main() {
    vec4 O = vec4(0); // Output color
    vec2 C = gl_FragCoord.xy;
    // FIX: RENDERSIZE is vec2, create vec3 with z=1.0 for ray direction calculation
    vec3 r = vec3(RENDERSIZE.xy, 1.0); // iResolution

    // FIX: Declare uv here
    vec2 uv = C / RENDERSIZE.xy;

    vec3 p;
    // Normalized ray direction from camera
    vec3 d = normalize(vec3((C - .5 * r.xy) / r.y + vec2(cameraOffsetX, cameraOffsetY), 1.0));

    float i = 0.0; // Outer loop counter for raymarching
    float g = 0.0; // Accumulated distance (similar to 't' in other raymarchers)
    float e;      // Temp variable
    float s;      // Scaling factor for fractal detail
    float t = TIME * animationSpeed; // iTime with animation speed control

    // Define color palettes
    vec3 palette_color_a, palette_color_b;
    float pulse_factor = (sin(t * colorPulseSpeed) * 0.5 + 0.5); // 0 to 1 for pulsing

    if (colorPalette < 0.5) { // Palette 0: Neon Dreams (Blue/Pink/Green)
        palette_color_a = vec3(0.1, 0.5, 1.0); // Electric Blue
        palette_color_b = vec3(1.0, 0.2, 0.8); // Hot Pink
    } else if (colorPalette < 1.5) { // Palette 1: Acid Trip (Yellow/Green/Purple)
        palette_color_a = vec3(0.8, 1.0, 0.1); // Acid Yellow-Green
        palette_color_b = vec3(0.6, 0.1, 0.9); // Deep Purple
    } else if (colorPalette < 2.5) { // Palette 2: Lava Lamp (Orange/Red/Teal)
        palette_color_a = vec3(1.0, 0.4, 0.0); // Fiery Orange
        palette_color_b = vec3(0.0, 0.8, 0.7); // Bright Teal
    } else if (colorPalette < 3.5) { // Palette 3: Galactic Swirl (Deep Blue/Magenta/Cyan)
        palette_color_a = vec3(0.1, 0.1, 0.6); // Deep Indigo
        palette_color_b = vec3(0.9, 0.1, 0.9); // Vibrant Magenta
    } else if (colorPalette < 4.5) { // Palette 4: Retro Grid (Primary Colors)
        palette_color_a = vec3(1.0, 0.0, 0.0); // Red
        palette_color_b = vec3(0.0, 0.0, 1.0); // Blue
    } else if (colorPalette < 5.5) { // Palette 5: Dark Cosmos (Subtle Greens/Blues)
        palette_color_a = vec3(0.1, 0.2, 0.15); // Dark Greenish
        palette_color_b = vec3(0.15, 0.1, 0.2); // Dark Bluish
    } else { // Palette 6: Rainbow Glitch (Rapid cycling hues)
        palette_color_a = H(t * 0.5 + 0.0);
        palette_color_b = H(t * 0.5 + 1.0);
    }

    // Main raymarching loop
    for (; i++ < 99.; ) {
        p = d * g; // Current point in 3D space along the ray
        p.z -= 0.1; // Offset Z, part of original fractal setup
        
        // Apply camera zoom to the point
        p *= cameraZoom;

        // Apply rotation (Rodrigues' formula)
        // Original: p=R(p,vec3(.577),t*.2);
        // .577 is approx 1/sqrt(3), so vec3(.577) is a diagonal unit vector.
        p = R(p, vec3(0.57735, 0.57735, 0.57735), t * 0.2);

        // Iterative fractal offset
        p -= i * fractalOffsetStrength; // p-=i/5e4 in original, now tunable

        s = boxFoldScale; // Start with tunable scaling factor (original s=2.)
        
        // Inner loop for Mandelbox-like folding
        for (int j = 0; j++ < int(boxFoldIterations); ) {
            // abs(p--) - .1
            // p-- means p.x = p.x - 1, p.y = p.y - 1, p.z = p.z - 1 (post-decrement)
            // This is a common Mandelbox fold, but with a specific offset and pre-value of p
            p = abs(p - boxFoldOffset) - boxFoldOffset; // Adjusted for control
            s *= (e = detailScalingFactor / dot(p, p)); // e = .01/dot(p,p), s*=e
            p *= e;
        }

        // Increment accumulated distance
        // e=min(.1,length(cross(p,normalize(mix(vec3(1),H(t*.2),.4))*2.-1.)))/s;
        // The H(t*.2) here creates a dynamic normal/direction for the cross product,
        // making the fractal appear to have internal motion or light interaction.
        vec3 cross_norm = normalize(mix(vec3(1), H(t * 0.2 + pulse_factor * colorPulseSpeed * 0.1), 0.4) * 2.0 - 1.0);
        e = min(0.1, length(cross(p, cross_norm))) / s;
        g += e;

        // Color accumulation
        // O.xyz+=mix(vec3(1),H(log(g)),.5)*.02/exp(i*i*e);
        // H(log(g)) uses the accumulated distance for hue, creating dynamic colors.
        // The exponential factor makes closer/less detailed parts brighter.
        
        // Mix with chosen palette and apply glow intensity
        vec3 base_color = mix(vec3(1), mix(palette_color_a, palette_color_b, H(log(g)).x), 0.5);
        O.xyz += base_color * glowIntensity * (0.02 / exp(i * i * e));

        // Break if distance is too small (hit surface) or ray is too long
        if (e < 0.0001 || g > 100.0) break; // Increased max ray distance for more detail
    }

    // Post-processing: Square and Cube the output for contrast (original shader style)
    // The original O*=O*O implies O.r = O.r * O.r * O.r etc.
    // Let's ensure it's a cube.
    O.xyz = O.xyz * O.xyz * O.xyz;

    // Apply brightness, saturation, contrast
    O.rgb = adjustBrightness(O.rgb, brightness);
    O.rgb = adjustSaturation(O.rgb, saturation);
    O.rgb = adjustContrast(O.rgb, contrast);

    // Apply vignette
    float vignette_val = pow(16.0 * uv.x * uv.y * (1.0 - uv.x) * (1.0 - uv.y), 0.3);
    O.rgb *= mix(1.0, vignette_val, vignetteIntensity);


    gl_FragColor = vec4(clamp(O.xyz, 0.0, 1.0), 1.0);
}