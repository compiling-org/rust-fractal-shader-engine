/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Tunnel"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/WlycDt by Kali. Yet another trippy tunnel. Modified with extensive psychedelic tunable parameters by Gemini.",
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom Factor" },
        { "NAME": "PathMorphStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Path Morph Strength" },
        { "NAME": "PathFrequency", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Path Frequency" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations" },
        { "NAME": "FractalScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 1.0, "MAX": 3.0, "LABEL": "Fractal Scale" },
        { "NAME": "FractalOffset", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Offset" },
        { "NAME": "FractalTimeInfluence", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Time Inf." },
        { "NAME": "TunnelSize", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.05, "MAX": 0.5, "LABEL": "Tunnel Size" },
        { "NAME": "TunnelWarpStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tunnel Warp Strength" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "ColorPulseAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse Amp." },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm Hues", "2.0": "Cool Tones" } },
        { "NAME": "Distortion", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "UV Distortion" }
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
#define MAX_FRACTAL_ITERATIONS 20 // Corresponds to FractalIterations MAX
#define MAX_MARCH_ITERATIONS 100  // Corresponds to the hardcoded march loop limit


mat2 rot(float a)
{
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Custom hue function for color palettes
vec3 hue(float h, float palette_type) {
    if (palette_type < 0.5) { // Classic Psychedelic Palette
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


vec3 path(float z, float time_scaled)
{
    // Original path with tunable frequency and morphing
    vec3 p = vec3(
        sin(z * PathFrequency) * 0.5,
        cos(z * 0.5 * PathFrequency),
        z
    );

    // Add morphing effect
    p.x += sin(z * 2.0 + time_scaled * 0.7) * PathMorphStrength * 0.3;
    p.y += cos(z * 3.0 + time_scaled * 0.5) * PathMorphStrength * 0.2;

    return p;
}

vec3 fractal(vec3 p, float time_scaled)
{
    float z = p.z * 0.1;
    p.z = abs(0.5 - fract(z));
    float m = 100.0; // Use float literal
    
    // Use MAX_FRACTAL_ITERATIONS as the loop upper bound (constant)
    for (int i = 0; i < MAX_FRACTAL_ITERATIONS; i++) 
    {
        // Only execute the fractal logic if 'i' is less than the actual desired FractalIterations uniform value
        if (float(i) < FractalIterations) { 
            p.xy *= rot(z + float(i) * 0.05 * FractalTimeInfluence); // Fractal rotation influenced by time
            p = abs(p * FractalScale) - FractalOffset; // Tunable scale and offset
            m = min(m, abs(p.y) + 0.5 * abs(0.5 - fract(p.x * 0.25 + time_scaled * FractalTimeInfluence + float(i) * 0.1)));
        } else {
            // Optional: break here for slight optimization, though compiler might optimize away
            // In GLSL ES 1.0, static unrolling means this 'break' might not yield much performance gain,
            // but it's good practice for clarity.
            break; 
        }
    }
    m = exp(-4.0 * m) * 2.0; // Use float literals
    return vec3(p.xz * 2.0, m) * m; // Use float literals
}

float de(vec3 p, float time_scaled)
{
    p.xy -= path(p.z, time_scaled).xy;
    // Tunable tunnel size
    float dist = length(p.xy);
    
    // Add warping effect to the tunnel
    float warp = sin(p.z * 5.0 + time_scaled * 2.0) * TunnelWarpStrength * 0.1;
    dist -= warp;

    return -dist + TunnelSize; // Tunable tunnel size
}

vec3 march(vec3 from, vec3 dir, float time_scaled)
{
    float d, td = 0.0; // Use float literal
    vec3 p, col = vec3(0.0); // Explicitly initialize
    
    // Use MAX_MARCH_ITERATIONS as the loop upper bound (constant)
    for (int i = 0; i < MAX_MARCH_ITERATIONS; i++) 
    {
        p = from + dir * td;
        d = de(p, time_scaled);
        if (d < 0.001) break; // Use float literal
        td += d;
        if (td > 50.0) break; // Max march distance to prevent infinite loops
    }

    if (d < 0.1) // Use float literal
    {
        p -= 0.001 * dir; // Use float literal
        // Combine fractal output with depth fading and color pulsing
        vec3 fractal_color = fractal(p, time_scaled);
        
        // Color pulsing based on time and distance
        float pulse = sin(time_scaled * ColorPulseSpeed + td * 0.1) * ColorPulseAmplitude;
        
        // Apply chosen color palette
        float h_val = mod(p.z * 0.1 + time_scaled * 0.05, 1.0); // Base hue value
        vec3 palette_color = hue(h_val + pulse, PaletteChoice);

        col = fractal_color * exp(-0.7 * td * td) * smoothstep(0.3, 1.0, td); // Use float literals
        col *= palette_color; // Apply palette color
    }
    return col;
}

mat3 lookat(vec3 dir, vec3 up) {
    dir = normalize(dir);
    vec3 rt = normalize(cross(dir, normalize(up)));
    return mat3(rt, cross(rt, dir), dir);
}

void main() {
    // Apply UV Distortion
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
    uv += sin(uv.yx * 10.0 + TIME * 5.0) * Distortion; // Time-based distortion

    float t = TIME * GlobalSpeed; // Global speed control

    vec3 from = path(t, t); // Camera position
    vec3 fw = normalize(path(t + 0.5, t) - from); // Forward direction

    vec3 dir = normalize(vec3(uv, ZoomFactor)); // Zoom factor influences perspective
    dir = lookat(fw, vec3(0.0, 1.0, 0.0)) * dir; // Use float literals

    vec3 col = march(from, dir, t);
    gl_FragColor = vec4(col,1.0);
}