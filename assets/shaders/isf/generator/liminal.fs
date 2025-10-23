/*
{
  "DESCRIPTION": "Liminal Grid Splicer - Volumetric fractal glitch mesh with morphing pulse and camera distortion.",
  "CATEGORIES": ["Psychedelic", "Fractal", "Glitch"],
  "INPUTS": [
    { "NAME": "PulseSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "MorphRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "GridFrequency", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "DistortionStrength", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "GlowBoost", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "HueShift", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 }
  ]
}
*/

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float fractal(vec3 p) {
    float m = 100.0;
    for (int i = 0; i < 6; i++) {
        p = abs(p) / dot(p, p) - MorphRate;
        m = min(m, length(p));
    }
    return m;
}

float grid(vec3 p, float freq) {
    return sin(p.x * freq) * sin(p.y * freq) * sin(p.z * freq);
}

vec3 colorize(float val, float t) {
    return hsv2rgb(vec3(mod(val * 0.2 + HueShift + 0.1 * sin(t), 1.0), 0.9, val));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * PulseSpeed;
    
    vec3 ro = vec3(0.0, 0.0, -5.0);
    vec3 rd = normalize(vec3(uv, 1.5));
    
    float totalDist = 0.0;
    vec3 col = vec3(0.0);
    
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * totalDist;
        float d = fractal(p + sin(p.yzx + t) * DistortionStrength) * 0.8;
        float g = grid(p + t, GridFrequency);
        float glow = exp(-abs(d - 0.1) * 20.0) + 0.2 * abs(g);
        
        vec3 c = colorize(glow, t) * glow * GlowBoost;
        col += c * 0.03;
        totalDist += d * 0.5;
        if (d < 0.001 || totalDist > 20.0) break;
    }

    col = pow(col, vec3(0.6));
    gl_FragColor = vec4(col, 1.0);
}
