/*
{
    "DESCRIPTION": "Parallax Nova Reactor — psychedelic supernova field with dynamic glitch, parallax, and blooming fractal core",
    "CATEGORIES": ["Psychedelic", "Fractal", "Glitch", "Experimental"],
    "INPUTS": [
        { "NAME": "CorePulse", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Core Pulse Strength" },
        { "NAME": "GlitchWarp", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glitch Warp Intensity" },
        { "NAME": "FieldMorph", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0, "LABEL": "Outer Field Morphing" },
        { "NAME": "SaturationBoost", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 3.0, "LABEL": "Saturation Boost" },
        { "NAME": "BloomIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Core Bloom Intensity" },
        { "NAME": "ColorPhase", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Phase Rotation" },
        { "NAME": "CoreSpinSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0, "LABEL": "Core Spin Speed" }
    ],
    "ISFVSN": "2"
}
*/

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f*f*(3.0 - 2.0*f);
    return mix(mix(hash(i), hash(i + vec2(1, 0)), f.x),
               mix(hash(i + vec2(0,1)), hash(i + vec2(1,1)), f.x),
               f.y);
}

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, -s, s, c);
}

vec3 palette(float t, float phase) {
    return 0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.66)) + phase * 6.2831);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME;

    // Parallax distortion field
    float r = length(uv);
    vec2 uvShift = uv + 0.15 * vec2(sin(uv.y * 10.0 + time), cos(uv.x * 10.0 + time)) * FieldMorph;

    // Spin + bloom core
    vec2 spun = uvShift * rot(time * CoreSpinSpeed);
    float glow = exp(-10.0 * length(spun)) * BloomIntensity;
    
    // Radial rings modulated by noise
    float ring = sin(40.0 * r - time * 2.0 + noise(uvShift * 8.0)) * 0.5 + 0.5;
    ring *= exp(-4.0 * r);

    // Core pulsing
    float pulse = sin(time * 6.0) * 0.5 + 0.5;
    float pulseShape = pow(glow + pulse * CorePulse, 2.0);

    // Final composite
    float glitch = noise(uv * 20.0 + vec2(time * 5.0, time * 3.0)) * GlitchWarp;

    float combined = clamp(pulseShape + ring + glitch, 0.0, 1.0);

    // Dynamic hue rotation
    vec3 col = palette(r + time * 0.1, ColorPhase);
    col *= combined;
    col = pow(col, vec3(1.5)) * SaturationBoost;

    gl_FragColor = vec4(col, 1.0);
}
