/*{
  "DESCRIPTION": "Plasma Core Engine — radiant fusion sphere with layered plasma and field coils",
  "CATEGORIES": ["3D", "Fractal", "Psychedelic"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "PulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "GlowAmount", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "CoreDensity", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "RadiationFreq", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "FieldSpin", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 }
  ]
}*/

#define PI 3.14159
#define TAU 6.28318

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, -s, s, c);
}

// Soft distance field: pulsating plasma sphere with layered ridges
float de(vec3 p, float t) {
    float r = length(p);
    
    // Central plasma heart
    float core = r - 0.5 - 0.1 * sin(t * PulseSpeed + r * 5.0);
    
    // Toroidal field coils
    p.xy *= rot(p.z * FieldSpin + t * 0.5);
    float rings = abs(length(p.xy) - 1.0 + 0.2 * sin(p.z * RadiationFreq + t)) - 0.05;

    // Combine with min for inner plasma and shell field
    return min(core, rings);
}

// Color palette and glow
vec3 shade(vec3 p, float d, float t) {
    float glow = exp(-GlowAmount * d * d);
    vec3 base = vec3(0.3 + 0.7 * sin(t + p.x * 3.0), 0.4 + 0.6 * sin(t + p.y * 4.0), 0.5 + 0.5 * cos(t + p.z * 5.0));
    return base * glow;
}

vec3 march(vec3 ro, vec3 rd, float t) {
    float dist = 0.0;
    vec3 col = vec3(0.0);
    
    for (int i = 0; i < 128; i++) {
        vec3 p = ro + dist * rd;
        float d = de(p, t);
        if (d < 0.001) break;
        
        col += shade(p, d, t) * 0.03;
        dist += d * 0.6;
        if (dist > 6.0) break;
    }
    
    // Optional fog
    col *= exp(-0.05 * dist * dist * CoreDensity);
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;

    vec3 ro = vec3(0.0, 0.0, -3.5);
    vec3 rd = normalize(vec3(uv, 1.5));

    vec3 col = march(ro, rd, t);
    gl_FragColor = vec4(col, 1.0);
}
