/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Fractal",
        "Visualizer"
    ],
    "DESCRIPTION": "Mandelbox fractal distorting distance field with volumetric rendering. Enhanced with extensive psychedelic tunable parameters by Gemini.",
    "CREDIT": "Original Shader by Kali, adapted for ISF by Gemini",
    "INPUTS": [
        {
            "NAME": "Texture",
            "TYPE": "image",
            "LABEL": "Audio Input Texture (Spectrum)",
            "DEFAULT": "null"
        },
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0, "LABEL": "Zoom Factor" },
        { "NAME": "CameraOrbitSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Cam Orbit Speed" },
        { "NAME": "CameraOrbitRadius", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Cam Orbit Radius" },
        { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159, "LABEL": "Cam Roll Angle" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations" },
        { "NAME": "BoxFoldScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Box Fold Scale" },
        { "NAME": "BoxFoldOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Box Fold Offset" },
        { "NAME": "MengerSphereRadius", "TYPE": "float", "DEFAULT": 3.5, "MIN": 0.5, "MAX": 5.0, "LABEL": "Menger Sphere Rad" },
        { "NAME": "TunnelShapeInfluence", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Tunnel Shape Inf" },
        { "NAME": "SquareTunnelStrength", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 0.5, "LABEL": "Square Tunnel Str" },
        { "NAME": "OctagonTunnelStrength", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 0.5, "LABEL": "Octagon Tunnel Str" },
        { "NAME": "SpaceWarpStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Space Warp Str" },
        { "NAME": "TimeWarpFrequency", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Time Warp Freq" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "ColorPulseAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse Amp." },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool" } },
        { "NAME": "FractalColorMix", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Color Mix" },
        { "NAME": "AudioColorInfluence", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio Color Inf" },
        { "NAME": "MarchSteps", "TYPE": "float", "DEFAULT": 150.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "March Steps" },
        { "NAME": "DetailThreshold", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Detail Threshold" }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

// Define PI for trigonometric functions
#ifndef PI
#define PI 3.14159265359
#endif

// Define TAU immediately after PI, as it depends on PI
#ifndef TAU
#define TAU (2.0 * PI)
#endif

// Define MAX iteration counts as constants for loops
// These should match or exceed the MAX value specified in your ISF JSON for the corresponding input
#define MAX_FRACTAL_ITERATIONS_CONST 20 // Corresponds to FractalIterations MAX
#define MAX_MARCH_STEPS_CONST 300       // Corresponds to MarchSteps MAX


float det = 0.0; // Changed to 0.0 as it's modified later, but good initial state
vec3 objcol = vec3(0.0);
float snd = 0.0; // Initialize audio sample


mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Custom hue function for color palettes
vec3 hue(float h, float palette_type) {
    if (palette_type < 0.5) { // Classic Psychedelic Palette (similar to previous)
        h = mod(h, 1.0);
        return clamp(abs(sin(h * PI * 2.0 + vec3(0.0, 2.0, 4.0))), 0.0, 1.0);
    } else if (palette_type < 1.5) { // Warm Hues (Reds, Oranges, Yellows)
        h = mod(h, 1.0) * 0.3 + 0.0; // Restrict hue to warm range
        return 0.5 + 0.5 * cos(h * TAU + vec3(0.0, 2.0, 4.0));
    } else { // Cool Tones (Blues, Purples, Greens)
        h = mod(h, 1.0) * 0.3 + 0.6; // Restrict hue to cool range
        return 0.5 + 0.5 * cos(h * TAU + vec3(0.0, 2.0, 4.0));
    }
}


float fractal(vec3 p) {
    vec3 c = p;
    float m = 100.0; // Use float literal
    
    // Use MAX_FRACTAL_ITERATIONS_CONST as the loop upper bound (constant)
    for(int i = 0; i < MAX_FRACTAL_ITERATIONS_CONST; i++) {
        // Only execute the fractal logic if 'i' is less than the actual desired FractalIterations uniform value
        if (float(i) < FractalIterations) {
            // Mandelbox folding (original logic)
            // p=abs(p + 1.) - abs( p - 1.) - p; // Original box fold
            // Tunable BoxFoldScale and BoxFoldOffset
            p = clamp(p, -BoxFoldOffset, BoxFoldOffset) * BoxFoldScale * 2.0 - p; // Box fold based on tunable offset and scale
            
            p = p / clamp(dot(p,p), 0.1, 1.0) - c; // Sphere inversion, then offset by original point
            m = min(m, abs(length(p) - MengerSphereRadius)); // Tunable Menger sphere radius
        } else {
            // Break from loop if desired iterations are met
            break;
        }
    }
    m = max(0.0, 1.0 - m); // Use float literals
    
    // objcol is the color derived from the fractal
    objcol = abs(p) * 0.6; // This creates the "stripes"
    
    // The density/influence of the fractal on the DE, modified by audio and new tunable
    return m * m * (0.2 + snd * AudioColorInfluence); // Tunable AudioColorInfluence
}

float de(vec3 p) {
    // Add space warping based on position and time
    p.yz *= rot(TIME * TimeWarpFrequency * SpaceWarpStrength);
    p.xz *= rot(TIME * TimeWarpFrequency * 0.2 * SpaceWarpStrength);
    
    // Original rotations
    p.yz *= rot(TIME);
    p.xz *= rot(0.2);

    float f = fractal(p); // Evaluate the fractal at this point
    
    // Combined distance field
    float d = length(p) - MengerSphereRadius + f * 1.5 - snd * 3.0; // Use tunable MengerSphereRadius

    // Tunnel geometry controls
    // Mix between original sphere tunnel and box/octagonal tunnels
    float tunnel_d = length(p.xy) - SquareTunnelStrength; // Square-like tunnel
    tunnel_d = min(tunnel_d, length(p.yz) - OctagonTunnelStrength); // Octagon-like tunnel
    tunnel_d = min(tunnel_d, length(p.xz) - OctagonTunnelStrength); // Octagon-like tunnel
    
    d = mix(d, min(d, tunnel_d), TunnelShapeInfluence); // Mix based on tunable influence

    return (d - f * 0.5) * 0.5; // Final distance, influenced by fractal
}

vec3 march(vec3 from, vec3 dir) {
    vec3 col = vec3(0.0); // Explicitly initialize
    vec3 p;
    float td = 0.0;
    float d;
    
    // Use MAX_MARCH_STEPS_CONST as the loop upper bound (constant)
    for (int i = 0; i < MAX_MARCH_STEPS_CONST; i++) {
        // Only execute march logic if 'i' is less than the actual desired MarchSteps uniform value
        if (float(i) < MarchSteps) {
            p = from + td * dir;    
            d = de(p);
            // Use tunable DetailThreshold
            if (d < DetailThreshold) break;
            td += max(DetailThreshold, abs(d)); // Step size based on distance and threshold
            if (td > 20.0) break; // Max march distance

            // Accumulate color
            col += objcol * exp(-0.005 * td * td); // Volume rendering, fading with distance
        } else {
            // Break from loop if desired steps are met
            break;
        }
    }
    
    vec3 rescol = col * 0.01 * (0.3 + snd * 1.5); // Base color, audio reactive
    return rescol;
}

// Custom psychedelic color palette blending
vec3 get_psychedelic_color(float value, float palette_type, float time_scaled) {
    vec3 base_color = hue(value + time_scaled * 0.05, palette_type);
    
    // Add pulsing effect
    float pulse_factor = 1.0 + sin(time_scaled * ColorPulseSpeed + value * 5.0) * ColorPulseAmplitude;
    
    return base_color * pulse_factor;
}


void main() {
    // Get audio sample from the 'Texture' input
    // Assuming 'Texture' is a 1D audio spectrum texture, we sample at a specific normalized coordinate.
    // The original uses a fixed .15 which might correspond to a specific frequency band.
    snd = texture2D(Texture, vec2(0.15, 0.0)).r; 

    // Prepare UVs with zoom
    vec2 p_uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
    vec2 uv_norm = gl_FragCoord.xy / RENDERSIZE.xy; // This is not used but kept from original structure

    vec3 dir = normalize(vec3(p_uv, ZoomFactor)); // Tunable ZoomFactor for perspective

    float t = TIME * GlobalSpeed; // Global speed control

    // Camera movement (original logic + new orbit/roll)
    vec3 from = vec3(sin(t * CameraOrbitSpeed) * CameraOrbitRadius, // Orbit X
                     cos(t * CameraOrbitSpeed) * CameraOrbitRadius, // Orbit Y (or Z if desired)
                     -10.0 + sin(t * 0.3) * CameraOrbitRadius * 0.5); // Z offset with some movement
    
    from.xz *= rot(t); // Original camera rotation

    // Apply Camera Roll
    dir.xy *= rot(CameraRoll);
    
    dir.xz *= rot(t); // Original direction rotation

    // det is used for initial step size in march, ensure it's not too small or too large
    // We'll tie det directly to DetailThreshold for consistency.
    det = DetailThreshold;

    vec3 col = march(from, dir);

    // Add post-processing effects (grid lines, specularity)
    // These are simplified for GLSL ES 1.0 and to integrate with new params
    
    // Simplified grid lines (more robust for GLSL ES 1.0)
    float grid_alpha_x = pow(abs(sin(dir.x * 20.0 * PI)), 10.0);
    float grid_alpha_y = pow(abs(sin(dir.y * 20.0 * PI)), 10.0);
    col += vec3(grid_alpha_x + grid_alpha_y) * 0.2; // Blend grid lines

    // Simplified Specular/Highlighting based on audio from Texture input
    // The original `floor(dir.x*20.)/20. * .5 + .5` is sampling a specific part of the audio texture
    // which corresponds to the x-direction of the ray. We'll replicate that.
    float audio_sample_for_spec = texture2D(Texture, vec2(floor(dir.x * 20.0) / 20.0 * 0.5 + 0.5, 0.1)).r;
    float spec_val = pow(audio_sample_for_spec, 2.0);
    
    // Add color based on audio sample and palette
    vec3 audio_color_influence = get_psychedelic_color(spec_val, PaletteChoice, t) * AudioColorInfluence;
    col += audio_color_influence * 0.4; // Multiplier for visual strength

    // Final color mixing
    // Mix the resulting marched color with the psychedelic palette based on FractalColorMix
    col = mix(col, get_psychedelic_color(length(col) * 0.5, PaletteChoice, t), FractalColorMix);

    // Apply some final subtle highlight (from original)
    vec2 sq = abs(0.5 - fract(dir.xy * 20.0)); // Square wave pattern
    float highlight = abs(1.0 - length(max(vec2(0.0), abs(sq) - 0.1))) * 0.4;
    col += highlight * step(dir.y + 0.4, spec_val) * length(fract(dir.xy * 10.0));
    
    // Orange/red glow (from original)
    col = max(col, vec3(0.5, 0.2, 0.0) - smoothstep(0.0, 0.03, abs(dir.y - spec_val + 0.35)));

    gl_FragColor = vec4(col, 1.0);
}