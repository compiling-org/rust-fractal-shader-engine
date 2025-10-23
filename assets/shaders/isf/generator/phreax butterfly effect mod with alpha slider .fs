/*
{
  "CATEGORIES": [
    "Fractal",
    "Kaleidoscope",
    "Raymarching"
  ],
  "DESCRIPTION": "Final corrected fractal kaleidoscope with full parameters",
  "CREDIT": "Based on original by phreax",
  "INPUTS": [
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.5,
      "MAX": 3.0,
      "LABEL": "Zoom"
    },
    {
      "NAME": "fractal_detail",
      "TYPE": "float",
      "DEFAULT": 10.0,
      "MIN": 5.0,
      "MAX": 20.0,
      "LABEL": "Detail Level"
    },
    {
      "NAME": "morph_speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Morph Speed"
    },
    {
      "NAME": "morph_amount",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Morph Intensity"
    },
    {
      "NAME": "bg_color",
      "TYPE": "color",
      "DEFAULT": [0.1,0.1,0.2,1.0],
      "LABEL": "BG Color"
    },
    {
      "NAME": "palette_index",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 6.0,
      "LABEL": "Color Palette"
    },
    {
      "NAME": "color_pulse",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Pulse Speed"
    },
    {
      "NAME": "animation_speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 3.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "glow_intensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 5.0,
      "LABEL": "Glow"
    },
    {
      "NAME": "symmetrical",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Symmetrical"
    }
  ]
}
*/

#define PI 3.1415926535
#define TAU 6.283185307
#define MAX_STEPS 100
const int MAX_ITERATIONS = 10; // Fixed constant for GLSL ES 1.0 compatibility

precision highp float;

mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

vec3 palette(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b*cos(TAU*(c*t + d));
}

vec3 getPalette(float idx, float t) {
    if (idx < 1.0) return palette(t, vec3(0.5), vec3(0.5), vec3(1.0), vec3(0.0, -0.33, 0.33));
    else if (idx < 2.0) return palette(t, vec3(0.5), vec3(0.5), vec3(1.0), vec3(0.0, 0.10, 0.20));
    else if (idx < 3.0) return palette(t, vec3(0.5), vec3(0.5), vec3(1.0), vec3(0.3, 0.20, 0.20));
    else if (idx < 4.0) return palette(t, vec3(0.5), vec3(0.5), vec3(1.0, 1.0, 0.5), vec3(0.8, 0.90, 0.30));
    else if (idx < 5.0) return palette(t, vec3(0.5), vec3(0.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
    else if (idx < 6.0) return palette(t, vec3(0.5), vec3(0.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
    else return palette(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0, 1.0, 1.0), vec3(0.0, 0.25, 0.25));
}

float map(vec3 p, float time) {
    // Base transformations
    float ang = PI * 0.25 * time;
    p.xz *= rot2(ang);
    p.xy *= rot2(ang * morph_amount);
    
    if (symmetrical) p.y = -(abs(p.y) - 0.3);
    
    // Morphing transformations
    float morph = sin(time * morph_speed);
    p.x = abs(p.x) - (0.5 + 0.3 * morph * morph_amount);
    p.y = abs(p.y) - (0.9 + 0.2 * sin(time * 0.8));
    p.y = abs(p.y) - 0.1;
    p.x = abs(p.x - 0.2) - 0.9;
    p.z = abs(p.z) - 0.5;

    p.zy -= 0.5;
    p.xy *= rot2(0.1 * time);
    p.zy *= rot2(-0.04 * time);
    
    // Torus with KIFS
    vec2 cp = vec2(length(p.xz) - 1.0, p.y);
    float angle = atan(p.z, p.x);
    cp *= rot2(3.0 * angle + time);
    cp = abs(cp) - 0.3;
    cp *= rot2(2.0 * angle);
    
    // Fixed iteration count for GLSL ES compatibility
    for (int i = 0; i < MAX_ITERATIONS; i++) {
        cp.y = abs(cp.y) - (0.05 * (0.5 * sin(time) + 0.9));
        cp *= rot2(0.1 * angle * sin(0.1 * time));
    }

    return length(cp) - mix(0.03, 0.3, sin(time * 0.3));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y / zoom;
    float time = TIME * animation_speed;
    
    vec3 ro = vec3(0.0, 0.0, -7.0 + sin(time));
    vec3 rd = normalize(vec3(uv, 0.7));
    
    vec3 p = ro;
    vec3 col = bg_color.rgb;
    float t = 0.0;
    float glow = 0.0;
    
    // Raymarching
    for (int i = 0; i < MAX_STEPS; i++) {
        float dist = map(p, time);
        if (dist < 0.001 || t > 100.0) break;
        t += dist;
        p += rd * dist;
        glow += 0.02 / (0.1 + dist * dist * fractal_detail);
    }
    
    // Apply coloring
    if (t < 100.0) {
        vec2 eps = vec2(0.003, 0.0);
        vec3 n = normalize(vec3(
            map(p + eps.xyy, time) - map(p - eps.xyy, time),
            map(p + eps.yxy, time) - map(p - eps.yxy, time),
            map(p + eps.yyx, time) - map(p - eps.yyx, time)
        ));
        
        vec3 l = normalize(vec3(0.4, 0.7, -1.0));
        float diff = max(0.0, dot(n, l));
        vec3 pal = getPalette(palette_index, fract(t * 0.02 + time * color_pulse));
        col = mix(pal * (diff + 0.3), bg_color.rgb, 1.0 - bg_color.a);
    }
    
    // Add glow and final output
    col += getPalette(palette_index, time * 0.5) * glow * glow_intensity;
    gl_FragColor = vec4(pow(col, vec3(0.4545)), 1.0);
}