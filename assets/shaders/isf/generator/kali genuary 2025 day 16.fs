/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Tunnel",
        "Fractal",
        "Psychedelic",
        "Abstract"
    ],
    "DESCRIPTION": "Generative palette. Trippy tunnel effect with tunable parameters for animation, color pulsing, tunnel shape, fractal details, and visual effects.",
    "IMPORTED": {},
    "INPUTS": [
        { "NAME": "AnimationSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Global Animation Speed" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 1.0, "LABEL": "Main Color Pulse Speed" },
        { "NAME": "ColorPulseRatio", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Secondary Color Pulse Ratio" },
        { "NAME": "Saturation", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.5, "LABEL": "Overall Saturation" },
        { "NAME": "Brightness", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Overall Brightness" },
        { "NAME": "CameraZOffset", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 0.0, "LABEL": "Camera Z Offset" },
        { "NAME": "PathXAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Tunnel Path X Wobble" },
        { "NAME": "PathYAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Tunnel Path Y Wobble" },
        { "NAME": "PathYFrequency", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 0.5, "LABEL": "Tunnel Path Y Frequency" },
        { "NAME": "TunnelRadius", "TYPE": "float", "MIN": 0.05, "MAX": 1.0, "DEFAULT": 0.25, "LABEL": "Tunnel Base Radius" },
        { "NAME": "TunnelWobbleFrequency", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 5.0, "LABEL": "Tunnel Surface Wobble Freq" },
        { "NAME": "TunnelWobbleAmount", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.1, "LABEL": "Tunnel Surface Wobble Amount" },
        { "NAME": "FractalIterations", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 4.0, "LABEL": "Fractal Iterations" },
        { "NAME": "FractalClampMin", "TYPE": "float", "MIN": 0.01, "MAX": 0.5, "DEFAULT": 0.2, "LABEL": "Fractal Clamp Min" },
        { "NAME": "FractalClampMax", "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Fractal Clamp Max" },
        { "NAME": "FractalOffset", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": -2.0, "LABEL": "Fractal Recursive Offset" },
        { "NAME": "FractalGlowExponent", "TYPE": "float", "MIN": -20.0, "MAX": 0.0, "DEFAULT": -10.0, "LABEL": "Fractal Glow Exponent" },
        { "NAME": "FractalGlowMultiplier", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 2.0, "LABEL": "Fractal Glow Multiplier" },
        { "NAME": "FogDensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "LABEL": "Tunnel Fog Density" },
        { "NAME": "MaxRaymarchSteps", "TYPE": "float", "MIN": 10.0, "MAX": 200.0, "DEFAULT": 80.0, "LABEL": "Max Raymarch Steps" },
        { "NAME": "HitThreshold", "TYPE": "float", "MIN": 0.0001, "MAX": 0.01, "DEFAULT": 0.001, "LABEL": "Raymarch Hit Threshold" },
        { "NAME": "AASamples", "TYPE": "float", "MIN": 1.0, "MAX": 16.0, "DEFAULT": 4.0, "LABEL": "Anti-Aliasing Samples (Squared)" },
        { "NAME": "LookAtOffset", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Camera LookAt Offset" }
    ]
}
*/

// Rotation matrix
mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Defines the path of the tunnel
vec3 path(float z, float time) {
    // x and y coordinates oscillate based on z and time, creating a winding path
    vec3 p = vec3(
        sin(z + cos(z) * time * 0.1) * PathXAmplitude, // X-coordinate with time influence
        cos(z * PathYFrequency + sin(z * time * 0.05)) * PathYAmplitude, // Y-coordinate with time influence
        z // Z-coordinate is simply z
    );
    return p;
}

// Generates the fractal pattern on the tunnel surface
vec3 fractal(vec3 p) {
    // Normalize z to create a repeating pattern along the tunnel axis
    float z_normalized = p.z * 2.0;
    // FractalZCenter is implicitly 0.5 from original logic. Keeping it fixed here for consistency.
    p.z = abs(0.5 - fract(z_normalized)); // Center the fractal in Z and repeat

    float m = 100.0; // Initialize minimum distance for glow calculation
    
    // Iterative fractal deformation with fixed loop limit and early exit
    // Max iterations set to 10 to cover the tunable range
    for (int i = 0; i < 10; i++) { // Fixed loop limit
        if (i >= int(FractalIterations)) break; // Early exit based on tunable parameter
        // Apply scaling and clamping based on product of coordinates
        p = abs(p) / clamp(abs(p.x * p.y * p.z), FractalClampMin, FractalClampMax) + FractalOffset;
        // Update minimum distance for glow (flicker effect)
        m = min(m, min(abs(p.z), min(abs(p.x), abs(p.y))));
    }
    
    // Calculate glow/brightness based on fractal structure and tunable parameters
    m = exp(FractalGlowExponent * m) * FractalGlowMultiplier;
    
    // Assign color based on fractal coordinates and glow
    vec3 col = vec3(p.xz, 1.0) * m;
    return col;
}

// Distance Estimator for the tunnel
float de(vec3 p, float time) {
    // Offset point by the current path position
    p.xy -= path(p.z, time).xy;
    
    // Calculate distance to the tunnel surface with wobbling effect
    float d = -length(p.xy) + TunnelRadius + sin(p.z * TunnelWobbleFrequency + time) * TunnelWobbleAmount;
    return d * 0.5; // Scale the distance for smoother steps
}

// Raymarching function
vec3 march(vec3 from, vec3 dir, float time) {
    float d, td = 0.0; // d: distance to surface, td: total distance traveled
    vec3 p, col = vec3(0.0); // p: current point, col: accumulated color
    
    // Raymarching loop with fixed limit and early exit
    // Max steps set to 200 to cover the tunable range
    for (int i = 0; i < 200; i++) { // Fixed loop limit
        if (i >= int(MaxRaymarchSteps)) break; // Early exit based on tunable parameter
        
        p = from + dir * td; // Calculate current 3D point
        d = de(p, time); // Get distance to tunnel surface
        
        if (d < HitThreshold) break; // Break if close enough to a surface
        td += d; // Advance ray if no hit
    }
    
    // If a hit occurred (or very close)
    if (d < 0.1) { // A slightly larger threshold for rendering the fractal
        p -= HitThreshold * dir; // Move back slightly to avoid artifacts
        
        // Render fractal at the hit point
        col = fractal(p) * exp(-FogDensity * td * td); // Apply fog based on distance
    }
    
    // Post-process color with normalization and scaling
    col = normalize(2.0 + col) * length(col) * 0.5;
    
    // Apply color pulsing rotation to Red/Blue and Green/Blue channels
    col.rb *= rot(time * ColorPulseSpeed);
    col.gb *= rot(time * ColorPulseSpeed * ColorPulseRatio);
    
    col = abs(col); // Ensure positive color values
    return col;
}

// LookAt matrix for camera orientation
mat3 lookat(vec3 dir, vec3 up) {
    dir = normalize(dir);
    vec3 rt = normalize(cross(dir, normalize(up)));
    return mat3(rt, cross(rt, dir), dir);
}

void main() {
    vec3 finalCol = vec3(0.0);
    // int numSamples = int(AASamples); // This can still be non-constant for the if condition below
    
    // Max Anti-aliasing samples set to 16 to cover the tunable range
    // The loop iterates up to a fixed maximum, and samples are only added if within AASamples
    const int MAX_AA_SAMPLES = 16; 
    float offset = 1.0 / float(MAX_AA_SAMPLES); // Use max for offset calculation for consistent grid

    // Anti-aliasing loop (supersampling) with fixed limit and early exit
    int actualAASamples = int(AASamples); // Get integer value for comparison
    
    for (int x = 0; x < MAX_AA_SAMPLES; x++) { // Fixed loop limit
        if (x >= actualAASamples) break; // Early exit based on tunable parameter

        for (int y = 0; y < MAX_AA_SAMPLES; y++) { // Fixed loop limit
            if (y >= actualAASamples) break; // Early exit based on tunable parameter

            // Calculate sample position with sub-pixel offsets
            vec2 samplePos = gl_FragCoord.xy + vec2(float(x) * offset, float(y) * offset);
            vec2 uv = (samplePos - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
            
            float t = TIME * AnimationSpeed; // Global animation time

            // Camera position and direction
            vec3 from = path(t + CameraZOffset, t); // Camera origin on the path
            vec3 fw = normalize(path(t + LookAtOffset + CameraZOffset, t) - from); // Forward direction (where camera looks)
            
            vec3 dir = normalize(vec3(uv, 1.0)); // Initial ray direction
            dir = lookat(fw, vec3(0, 1, 0)) * dir; // Apply camera orientation to ray direction
            
            vec3 col = march(from, dir, t); // Perform raymarch
            finalCol += col; // Accumulate color for anti-aliasing
        }
    }

    // Adjust division for actual number of samples used
    finalCol /= float(actualAASamples * actualAASamples); 
    finalCol *= Saturation; // Apply saturation
    finalCol *= Brightness; // Apply brightness
    gl_FragColor = vec4(finalCol, 1.0); // Final output color
}
