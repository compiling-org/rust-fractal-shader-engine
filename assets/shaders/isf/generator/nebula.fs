/*
{
    "CATEGORIES": ["3D", "Volumetric", "Abstract"],
    "DESCRIPTION": "Explore a vibrant, ever-changing cosmic nebula with dynamic lights and procedural density.",
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME": "NebulaDensity",
            "TYPE": "float",
            "DEFAULT": 0.85,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Nebula Density"
        },
        {
            "NAME": "NebulaSpeed",
            "TYPE": "float",
            "DEFAULT": 0.12,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Nebula Speed"
        },
        {
            "NAME": "NebulaGlow",
            "TYPE": "float",
            "DEFAULT": 1.17,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Nebula Glow"
        },
        {
            "NAME": "ColorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Color Palette",
            "VALUES": ["Cosmic Dust", "Aurora Borealis", "Deep Space Glow"]
        },
        {
            "NAME": "ColorBlendSpeed",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Color Blend Speed"
        },
        {
            "NAME": "LightIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 3.0,
            "LABEL": "Light Intensity"
        },
        {
            "NAME": "CameraSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Camera Speed"
        },
        {
            "NAME": "NoiseDetail",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 3.0,
            "LABEL": "Noise Detail"
        }
    ]
}
*/

precision mediump float;

// --- Utility Functions ---

// Standard 3D Hash function (for noise)
vec3 hash33(vec3 p) {
    p = fract(p * vec3(0.1031, 0.11369, 0.13787));
    p += dot(p, p.yxz + 19.19);
    return fract(vec3((p.x + p.y)*p.z, (p.x+p.z)*p.y, (p.y+p.z)*p.x));
}

// Value Noise (simplified for ISF compatibility)
float noise(vec3 x) {
    vec3 p = floor(x);
    vec3 f = fract(x);
    f = f * f * (3.0 - 2.0 * f); // Smoothstep interpolation curve

    float n = p.x + p.y * 157.0 + p.z * 113.0;
    return mix(mix(mix(hash33(p + vec3(0,0,0)).x, hash33(p + vec3(1,0,0)).x, f.x),
                   mix(hash33(p + vec3(0,1,0)).x, hash33(p + vec3(1,1,0)).x, f.x), f.y),
               mix(mix(hash33(p + vec3(0,0,1)).x, hash33(p + vec3(1,0,1)).x, f.x),
                   mix(hash33(p + vec3(0,1,1)).x, hash33(p + vec3(1,1,1)).x, f.x), f.y), f.z);
}

// Fractal Brownian Motion (FBM) using the noise function
float fbm(vec3 p, float lacunarity, float gain) {
    const int FBM_OCTAVES = 6; // Increased octaves for more detail
    
    float sum = 0.0;
    float amp = 1.0;
    float freq = 1.0;
    for (int i = 0; i < FBM_OCTAVES; i++) {
        sum += amp * noise(p * freq);
        freq *= lacunarity;
        amp *= gain;
    }
    return sum;
}

// --- Color Palettes ---
vec3 getPaletteColor(float t, float paletteType) {
    t = fract(t); 
    vec3 col;

    if (paletteType < 0.5) { // Cosmic Dust: Deep purples, rich pinks, gold accents
        col = mix(vec3(0.1, 0.0, 0.25), vec3(0.85, 0.15, 0.6), t); // Dark base to vibrant pink-purple
        col = mix(col, vec3(1.0, 0.7, 0.2), pow(t, 4.0)); // Add golden highlights
        col = mix(col, vec3(0.5, 0.0, 0.8), sin(t * 5.0) * 0.5 + 0.5); // Deep pulsating purple
    } else if (paletteType < 1.5) { // Aurora Borealis: Shimmering greens, blues, and subtle pinks
        col = mix(vec3(0.0, 0.1, 0.3), vec3(0.2, 0.8, 0.4), t); // Deep blue base to bright green
        col = mix(col, vec3(0.9, 0.3, 0.7), sin(t * 3.0 + 1.57) * 0.5 + 0.5); // Warm pink shift
        col = mix(col, vec3(0.5, 0.9, 1.0), pow(t, 2.0)); // Bright cyan highlights
    } else { // Deep Space Glow: Intense blues, electric cyan, and stark whites
        col = mix(vec3(0.0, 0.0, 0.1), vec3(0.1, 0.5, 0.8), t); // Very dark base to deep blue
        col = mix(col, vec3(0.8, 1.0, 1.0), pow(t, 4.0)); // Intense white core glow
        col = mix(col, vec3(0.0, 0.8, 1.0), cos(t * 4.0) * 0.5 + 0.5); // Electric blue pulsing
    }
    return col;
}


// --- Main Shader Logic ---
void main() {
    // Moved const declarations inside main() for compatibility
    const int MAX_VOLUMETRIC_STEPS = 100; 
    const float MAX_RAY_DISTANCE = 100.0; 

    // Explicitly cast TIME to mediump float for all calculations
    float current_time = TIME;

    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    // Camera setup with more complex oscillation for dynamic movement
    float time_wobble = current_time * 0.8;
    vec3 cameraOscillation = vec3(
        sin(time_wobble * 0.7) * 2.5, // Side-to-side
        cos(time_wobble * 0.9) * 2.0, // Up-and-down
        sin(time_wobble * 0.5) * 1.5  // Subtle forward/backward bob
    );
    vec3 cameraPos = vec3(0.0, 0.0, current_time * CameraSpeed * 15.0) + cameraOscillation; // Fly faster, add oscillation
    
    vec3 cameraLookAt = cameraPos + vec3(0.0, 0.0, 1.0); // Look forward
    vec3 cameraUp = vec3(0.0, 1.0, 0.0);

    // Construct camera basis vectors
    vec3 forward = normalize(cameraLookAt - cameraPos);
    vec3 right = normalize(cross(forward, cameraUp));
    vec3 up = normalize(cross(right, forward));

    // Ray direction for current pixel
    vec3 rayDir = normalize(uv.x * right + uv.y * up + forward);

    vec3 finalColor = vec3(0.0);
    float totalOpacity = 0.0;
    
    float stepSize = MAX_RAY_DISTANCE / float(MAX_VOLUMETRIC_STEPS);

    // Light source (dynamic, orbiting camera with more complex motion)
    vec3 lightOffset = vec3(sin(current_time * 0.7) * 8.0, cos(current_time * 0.9) * 7.0, sin(current_time * 0.6) * 6.0);
    vec3 lightPos = cameraPos + lightOffset; 

    for (int i = 0; i < MAX_VOLUMETRIC_STEPS; i++) {
        float t = float(i) * stepSize;
        vec3 p = cameraPos + rayDir * t; // Current point in 3D space

        // Add turbulence to the sample point for swirling effect
        // CORRECTED: Wrapped fbm result in vec3() to match dimension
        vec3 turbulence_offset = vec3(fbm(p * 0.5 + current_time * 0.1, 2.0, 0.5)) * 0.8; 
        p += turbulence_offset;

        // Apply nebula speed and noise detail to point
        vec3 noise_p = p * NoiseDetail + current_time * NebulaSpeed;

        // Get density from FBM noise, applying a "ridged" transformation for sharper clouds
        float density = fbm(noise_p, 2.0, 0.5); 
        density = 1.0 - abs(density * 2.0 - 1.0); // Ridged effect
        density = max(0.0, density - 0.1); // Threshold and clamp
        density *= NebulaDensity; // Apply user density control

        if (density > 0.0) {
            // Calculate light contribution at this point
            vec3 lightDir = normalize(lightPos - p);
            float lightDist = length(lightPos - p);
            
            // Simple light attenuation (inverse square + absorption)
            float lightAttenuation = 1.0 / (1.0 + lightDist * lightDist * 0.1);

            // Get color from palette based on position and time
            float color_t = (p.z * 0.05 + density * 0.7 + current_time * ColorBlendSpeed); // More influence from density
            vec3 sampleColor = getPaletteColor(color_t, ColorPalette);

            // Add volumetric glow/emission
            vec3 emission = sampleColor * pow(density, 2.0) * NebulaGlow; // Glow stronger with higher density
            finalColor += emission * (1.0 - totalOpacity); // Emission contributes regardless of external light

            // Accumulate color from external light
            // (1.0 - totalOpacity) ensures light is absorbed as it passes through the volume
            finalColor += sampleColor * density * LightIntensity * lightAttenuation * (1.0 - totalOpacity);
            totalOpacity += density;
        }

        if (totalOpacity > 0.99) break; // Stop if nearly opaque
    }

    // Add a very subtle ambient background color that changes with time/palette
    vec3 backgroundColor = getPaletteColor(current_time * 0.05, ColorPalette) * 0.05; // Very subtle
    gl_FragColor = vec4(mix(backgroundColor, finalColor, totalOpacity), 1.0); // Blend background with nebula based on its opacity
}