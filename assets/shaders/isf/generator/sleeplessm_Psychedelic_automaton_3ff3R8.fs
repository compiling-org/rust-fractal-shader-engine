/*
{
  "CATEGORIES": [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION": "Psychedelic fractal shader with tunable parameters for color shifts, fractal depth, artifact intensity, and more.",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "ColorPulseSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "FractalDepth",
      "TYPE": "float",
      "MIN": 1.0,
      "MAX": 10.0,
      "DEFAULT": 7.0
    },
    {
      "NAME": "ArtifactIntensity",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5
    },
    {
      "NAME": "Brightness",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.8
    },
    {
      "NAME": "RotationSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "PolyhedronScale",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 10.0,
      "DEFAULT": 3.0
    }
  ]
}

*/


// Constants
#define PI 3.14159265359

// Helper function for rotation
vec2 rotate(vec2 p, float angle) {
    float c = cos(angle), s = sin(angle);
    return mat2(c, -s, s, c) * p;
}

void main() {
    // Normalize coordinates
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Time scaling
    float time = TIME * RotationSpeed;

    // Base color accumulation
    vec3 color = vec3(0.0);

    // Wave-based geometry with fractal recursion
    for (float i = 0.0; i < FractalDepth; i++) {
        // Apply wave-like transformations
        uv = rotate(uv, sin(time * 0.2 + i) * 0.5); // Rotate UV coordinates
        uv = sin(uv * PI + time * 0.5) * 0.7;       // Apply sine wave distortion

        // Calculate distance to center
        float d = length(uv);

        // Add psychedelic colors with fading and shifting
        color += vec3(
            sin(time + i * ColorPulseSpeed) * 0.5 + 0.5,
            cos(time + i * ColorPulseSpeed * 0.7) * 0.5 + 0.5,
            sin(time * ColorPulseSpeed * 0.5) * 0.5 + 0.5
        ) * exp(-d * 3.0);
    }

    // Polyhedron-like effect with fractal symmetry
    uv = fract(uv * PolyhedronScale) - 0.5;
    float poly = length(uv);
    color += vec3(0.3, 0.6, 1.0) * exp(-poly * 22.0);

    // Organic morphing effect with imaginal artifacts
    uv += 0.2 * sin(uv.yx * 44.0 + time);
    float organic = length(uv);
    color += vec3(1.0, 0.2, 0.4) * exp(-organic * 4.0);

    // Add noise-like artifacts for a dreamlike distortion
    float noise = fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
    color += vec3(noise * ArtifactIntensity);

    // Apply brightness modulation
    color *= Brightness + 0.7 * sin(time * 0.5);

    // Clamp colors to avoid overexposure
    color = clamp(color, 0.0, 1.0);

    // Output final color
    gl_FragColor = vec4(color, 1.0);
}