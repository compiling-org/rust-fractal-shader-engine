/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Enhanced version of \"Neon Pulse Kraken\" with customizable parameters, improved audio reactivity, and smoother color transitions.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "ColorCycleDuration",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 20.0,
            "DEFAULT": 7.0
        },
        {
            "NAME": "AudioReactivity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "SimulatedAudioInput",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        }
    ]
}
*/

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * Speed;

    // Octopus body and arms with more variation
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);
    float armPattern = sin(12.0 * angle + time * 4.0 - radius * 10.0);

    // Add rhythmic wave-like movement to the arms
    float waveMovement = sin(time * 1.5 + radius * 5.0 + angle * 3.0) * cos(time * 2.0 - angle * 4.0);
    armPattern += 0.1 * waveMovement;

    // Add reverse movement for more dynamic variation
    float reverseWave = cos(time * 2.0 - radius * 3.0 - angle * 2.0);
    armPattern += 0.05 * reverseWave;

    // Create dots for tentacles (suckers) with additional variation
    float dotPattern = smoothstep(0.015, 0.035, cos(radius * 40.0 + armPattern * 6.0 + waveMovement));

    // Smooth color shifting over time
    float colorCycle = mod(TIME / ColorCycleDuration, 6.0);
    float blendFactor = fract(TIME / ColorCycleDuration);
    vec3 baseColor1, baseColor2;
    if (colorCycle < 1.0) {
        baseColor1 = vec3(1.0, 0.0, 0.5); // Neon Pink
        baseColor2 = vec3(0.0, 1.0, 0.5); // Neon Green
    } else if (colorCycle < 2.0) {
        baseColor1 = vec3(0.0, 1.0, 0.5); // Neon Green
        baseColor2 = vec3(0.0, 0.5, 1.0); // Neon Blue
    } else if (colorCycle < 3.0) {
        baseColor1 = vec3(0.0, 0.5, 1.0); // Neon Blue
        baseColor2 = vec3(1.0, 1.0, 0.0); // Neon Yellow
    } else if (colorCycle < 4.0) {
        baseColor1 = vec3(1.0, 1.0, 0.0); // Neon Yellow
        baseColor2 = vec3(1.0, 0.5, 0.0); // Neon Orange
    } else if (colorCycle < 5.0) {
        baseColor1 = vec3(1.0, 0.5, 0.0); // Neon Orange
        baseColor2 = vec3(0.5, 0.0, 1.0); // Neon Purple
    } else {
        baseColor1 = vec3(0.5, 0.0, 1.0); // Neon Purple
        baseColor2 = vec3(1.0, 0.0, 0.5); // Neon Pink
    }
    vec3 baseColor = mix(baseColor1, baseColor2, smoothstep(0.0, 1.0, blendFactor));

    // Simulate audio input using a tunable parameter
    float sound = SimulatedAudioInput * AudioReactivity;

    // Audio-reactive color modulation
    vec3 color = baseColor * (0.7 + 0.3 * sin(armPattern * 12.0 + time * 1.5 + sound * 5.0));

    // Elasticity and pulsing effects
    float elasticity = 0.05 * sin(time * 2.0 + radius * 8.0 + sound * 10.0);
    armPattern += elasticity;

    // Apply dot pattern to simulate tentacle suckers
    color *= dotPattern;

    // Glow effect with adjustable intensity
    float glow = smoothstep(0.1, 0.25, armPattern + waveMovement * 0.5) * GlowIntensity;
    color += glow * 0.5;

    // Beat-synced pulsing effect
    float beatPulse = 0.4 * sin(TIME * 3.0 + radius * 10.0 + sound * 15.0);
    color += beatPulse;

    // Final blending for enhanced brightness and contrast
    color = pow(color, vec3(1.4)); // Increase contrast slightly for more punch
    gl_FragColor = vec4(color, 1.0);
}