/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Fractal"
    ],
    "DESCRIPTION": "Mandelbox fractal distorting distance field with volumetric rendering. Enhanced with extensive psychedelic tunable parameters by Gemini. Replaced audio input with beautiful inbuilt procedural textures. Added detailed brightness and fractal visibility controls.",
    "CREDIT": "Original Shader by Kali, adapted for ISF by Gemini",
    "INPUTS": [
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
        { "NAME": "FractalColorMix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Color Mix" },
        { "NAME": "ProceduralEnergyInfluence", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Proc Energy Inf" },
        { "NAME": "MarchSteps", "TYPE": "float", "DEFAULT": 150.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "March Steps" },
        { "NAME": "DetailThreshold", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Detail Threshold" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "FractalPatternBrightness", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Fractal Pattern Brightness" },
        { "NAME": "FractalDensity", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Density" },
        { "NAME": "FractalShapeInfluence", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Shape Influence" },
        { "NAME": "TunnelGlowIntensity", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.001, "MAX": 0.1, "LABEL": "Tunnel Glow Intensity" }
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


float det = 0.0; 
vec3 objcol = vec3(0.0);
float snd = 0.0; 
float spec_val_proc = 0.0; 


mat2 rot(float a) {
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

// --- Procedural Texture Functions ---

// Simple pseudo-random hash (from Shadertoy common)
float hash12(vec2 p) {
    p  = fract(p * vec2(5.3983, 5.4427));
    p += dot(p, p.yx);
    return fract(p.x * p.y * 93.0);
}

// Smooth value noise (2D)
float noise(vec2 p) {
    vec2 ip = floor(p);
    vec2 fp = fract(p);
    
    // Smoothstep for interpolation
    vec2 u = fp * fp * (3.0 - 2.0 * fp);

    float res = mix(mix(hash12(ip),
                        hash12(ip + vec2(1.0, 0.0)), u.x),
                    mix(hash12(ip + vec2(0.0, 1.0)),
                        hash12(ip + vec2(1.0, 1.0)), u.x), u.y);
    return res;
}

// Fractal Brownian Motion (FBM) for complex noise
float fbm(vec2 p) {
    float f = 0.0;
    float amp = 0.5;
    float freq = 2.0;
    for (int i = 0; i < 4; i++) { // 4 octaves
        f += amp * noise(p * freq);
        amp *= 0.5;
        freq *= 2.0;
    }
    return f;
}

// Get procedural "audio energy" (0.0-1.0)
float get_procedural_energy(float time_scaled) {
    float energy = fbm(vec2(time_scaled * 0.5, time_scaled * 0.3) + gl_FragCoord.xy * 0.001);
    energy = pow(energy, 1.5); 
    energy = clamp(energy, 0.0, 1.0); 
    return energy;
}

// Get procedural "specular value" (0.0-1.0) based on direction and time
float get_procedural_spec(vec3 dir, float time_scaled) {
    float spec_val_raw = sin(dir.x * 20.0 + time_scaled * 5.0) * 0.5 + 0.5; 
    spec_val_raw += fbm(vec2(dir.y * 5.0, time_scaled * 0.8)) * 0.2; 
    spec_val_raw = clamp(spec_val_raw, 0.0, 1.0);
    return pow(spec_val_raw, 2.0); 
}

// --- End Procedural Texture Functions ---


float fractal(vec3 p) {
    vec3 c = p;
    float m = 100.0; 
    
    for(int i = 0; i < MAX_FRACTAL_ITERATIONS_CONST; i++) {
        if (float(i) < FractalIterations) {
            p = clamp(p, -BoxFoldOffset, BoxFoldOffset) * BoxFoldScale * 2.0 - p; 
            
            p = p / clamp(dot(p,p), 0.1, 1.0) - c; 
            m = min(m, abs(length(p) - MengerSphereRadius)); 
        } else {
            break;
        }
    }
    m = max(0.0, 1.0 - m); 
    
    // Apply FractalPatternBrightness to the fractal's color
    objcol = abs(p) * 0.6 * FractalPatternBrightness; 
    
    // Apply FractalDensity to the fractal's influence on the DE
    return m * m * FractalDensity * (0.2 + snd * ProceduralEnergyInfluence); 
}

float de(vec3 p) {
    p.yz *= rot(TIME * TimeWarpFrequency * SpaceWarpStrength);
    p.xz *= rot(TIME * TimeWarpFrequency * 0.2 * SpaceWarpStrength);
    
    p.yz *= rot(TIME);
    p.xz *= rot(0.2);

    float f = fractal(p); // Evaluate the fractal at this point
    
    // Apply FractalShapeInfluence to how much the fractal distorts the geometry
    float d = length(p) - MengerSphereRadius + f * FractalShapeInfluence - snd * 3.0; 

    float tunnel_d = length(p.xy) - SquareTunnelStrength; 
    tunnel_d = min(tunnel_d, length(p.yz) - OctagonTunnelStrength); 
    tunnel_d = min(tunnel_d, length(p.xz) - OctagonTunnelStrength); 
    
    d = mix(d, min(d, tunnel_d), TunnelShapeInfluence); 

    // Adjust the second f multiplier proportionally to FractalShapeInfluence
    return (d - f * (FractalShapeInfluence / 3.0)) * 0.5; 
}

vec3 march(vec3 from, vec3 dir) {
    vec3 col = vec3(0.0); 
    vec3 p;
    float td = 0.0;
    float d;
    
    for (int i = 0; i < MAX_MARCH_STEPS_CONST; i++) {
        if (float(i) < MarchSteps) {
            p = from + td * dir;    
            d = de(p);
            if (d < DetailThreshold) break;
            td += max(DetailThreshold, abs(d)); 
            if (td > 20.0) break; 

            col += objcol * exp(-0.005 * td * td); 
        } else {
            break;
        }
    }
    
    // Apply TunnelGlowIntensity
    vec3 rescol = col * TunnelGlowIntensity * (1.0 + snd * 1.5); 
    return rescol;
}

// Custom psychedelic color palette blending
vec3 get_psychedelic_color(float value, float palette_type, float time_scaled) {
    vec3 base_color = hue(value + time_scaled * 0.05, palette_type);
    
    float pulse_factor = 1.0 + sin(time_scaled * ColorPulseSpeed + value * 5.0) * ColorPulseAmplitude;
    
    return base_color * pulse_factor;
}


void main() {
    float t_scaled_for_proc = TIME * GlobalSpeed;
    snd = get_procedural_energy(t_scaled_for_proc); 

    vec2 p_uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    vec3 dir = normalize(vec3(p_uv, ZoomFactor)); 

    float t = TIME * GlobalSpeed; 

    vec3 from = vec3(sin(t * CameraOrbitSpeed) * CameraOrbitRadius, 
                     cos(t * CameraOrbitSpeed) * CameraOrbitRadius, 
                     -10.0 + sin(t * 0.3) * CameraOrbitRadius * 0.5); 
    
    from.xz *= rot(t); 

    dir.xy *= rot(CameraRoll);
    
    dir.xz *= rot(t); 

    det = DetailThreshold;

    vec3 col = march(from, dir);

    float grid_alpha_x = pow(abs(sin(dir.x * 20.0 * PI)), 10.0);
    float grid_alpha_y = pow(abs(sin(dir.y * 20.0 * PI)), 10.0);
    col += vec3(grid_alpha_x + grid_alpha_y) * 0.2; 

    spec_val_proc = get_procedural_spec(dir, t_scaled_for_proc);
    
    vec3 procedural_color_influence = get_psychedelic_color(spec_val_proc, PaletteChoice, t) * ProceduralEnergyInfluence; 
    col += procedural_color_influence * 0.4; 

    col = mix(col, get_psychedelic_color(length(col) * 0.5, PaletteChoice, t), FractalColorMix);

    vec2 sq = abs(0.5 - fract(dir.xy * 20.0)); 
    float highlight = abs(1.0 - length(max(vec2(0.0), abs(sq) - 0.1))) * 0.4;
    col += highlight * step(dir.y + 0.4, spec_val_proc) * length(fract(dir.xy * 10.0));
    
    col = max(col, vec3(0.5, 0.2, 0.0) - smoothstep(0.0, 0.03, abs(dir.y - spec_val_proc + 0.35)));

    // Apply GlobalBrightness as the final step
    gl_FragColor = vec4(col * GlobalBrightness, 1.0);
}