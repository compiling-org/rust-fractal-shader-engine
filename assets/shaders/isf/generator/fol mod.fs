/*
{
  "CATEGORIES": [
    "Generative",
    "Hexagonal Grid",
    "Psychedelic"
  ],
  "DESCRIPTION": "A generative shader based on a hexagonal grid with oscillating patterns. Converted from Shadertoy to ISF.",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "pseudoAudioLevel",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "timeScale",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0.01,
      "MAX": 2.2
    },
    {
      "NAME": "displace",
      "TYPE": "float",
      "DEFAULT": 0.04,
      "MIN": 0.01,
      "MAX": 0.1
    },
    {
      "NAME": "gridSize",
      "TYPE": "float",
      "DEFAULT": 36.0,
      "MIN": 10.0,
      "MAX": 100.0
    },
    {
      "NAME": "wave",
      "TYPE": "float",
      "DEFAULT": 5.0,
      "MIN": 1.0,
      "MAX": 10.0
    },
    {
      "NAME": "brightness",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.5,
      "MAX": 3.0
    }
  ],
  "PASSES": [
    {
      "TARGET": "buffer",
      "PERSISTENT": true
    }
  ],
  "ISFVSN": "2"
}

*/


// Define constants
#define TWO_PI 6.2831853072
#define PI 3.14159265359

// Helper functions
vec2 rotate(vec2 v, float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return v * mat2(c, -s, s, c);
}

vec3 coordToHex(vec2 coord, float scale, float angle) {
    vec2 c = rotate(coord, angle);
    float q = (1.0 / 3.0 * sqrt(3.0) * c.x - 1.0 / 3.0 * c.y) * scale;
    float r = 2.0 / 3.0 * c.y * scale;
    return vec3(q, r, -q - r);
}

vec3 hexToCell(vec3 hex, float m) {
    return fract(hex / m) * 2.0 - 1.0;
}

float absMax(vec3 v) {
    return max(max(abs(v.x), abs(v.y)), abs(v.z));
}

float nsin(float value) {
    return sin(value * TWO_PI) * 0.5 + 0.5;
}

float hexToFloat(vec3 hex, float amt) {
    return mix(absMax(hex), 1.0 - length(hex) / sqrt(3.0), amt);
}

// Main calculation function
float calc(vec2 tx, float time, float pseudoAudioLevel) {
    // Smoothly oscillate the angle
    float angle = PI * nsin(time * 0.1) + PI / 6.0;

    // Replace audio input with pseudoAudioLevel
    float len = 2.0 / 122.0 * pseudoAudioLevel + 1.0;
    float value = TIME * 0.005 + pseudoAudioLevel * 0.000752;

    // Convert coordinates to hexagonal grid
    vec3 hex = coordToHex(tx, gridSize * nsin(time * 0.01), angle);
    for (int i = 0; i < 3; i++) {
        float offset = float(i) / 3.0;
        vec3 cell = hexToCell(hex, 1.0 + float(i));

        // Wrap time to prevent unbounded growth
        float wrappedTime = mod(time, 10.0); // Wrap time every 10 seconds

        // Add oscillating components with smooth transitions
        value += nsin(hexToFloat(cell, nsin(len + wrappedTime + offset)) *
                      wave * nsin(wrappedTime * 0.5 + offset) + len + wrappedTime);
    }

    // Normalize value to [0, 1] using sine oscillation
    value = 0.5 + 0.5 * sin(value);

    // Clamp to valid range
    return clamp(value, 0.0, 1.0);
}

// Main function
void main() {
    // Normalize screen coordinates
    vec2 tx = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    tx.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Scale time
    float time = TIME * timeScale;

    // Use pseudoAudioLevel instead of audio input
    float pseudoAudioLevel = pseudoAudioLevel; // Already defined as an input

    // Initialize RGB channels
    vec3 rgb = vec3(0.0);
    for (int i = 0; i < 3; i++) {
        float time2 = time + float(i) * displace;

        // Incorporate pseudoAudioLevel into the time offset with smoothing
        time2 += smoothstep(-1.0, 1.0, pseudoAudioLevel * 2.0 - 1.0) * 1.1;

        // Calculate RGB values
        rgb[i] += pow(calc(tx, time2, pseudoAudioLevel), 5.0);
    }

    // Apply neon psychedelic color palette
    vec3 finalColor = vec3(
        abs(sin(rgb[0] * 1.1)),
        abs(sin(rgb[1] * 1.0)),
        abs(sin(rgb[2] * 1.0))
    );

    // Apply brightness and saturation with clamping
    finalColor = clamp(finalColor * brightness, 0.0, 10.0);

    // Output final color
    gl_FragColor = vec4(finalColor, 1.0);
}