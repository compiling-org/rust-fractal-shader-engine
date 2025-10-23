/*
{
  "CATEGORIES": [
    "Generative",
    "Psychedelic",
    "Pattern"
  ],
  "DESCRIPTION": "Enhanced psychedelic visualizer with full parameter control",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.5,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0,
      "LABEL": "Zoom Level"
    },
    {
      "NAME": "color_speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.3,
      "LABEL": "Color Pulse Speed"
    },
    {
      "NAME": "morph_amount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5,
      "LABEL": "Morph Intensity"
    },
    {
      "NAME": "glow_intensity",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0,
      "LABEL": "Glow Strength"
    },
    {
      "NAME": "palette_shift",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "LABEL": "Palette Shift"
    },
    {
      "NAME": "base_hue",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5,
      "LABEL": "Base Hue"
    },
    {
      "NAME": "saturation",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0,
      "LABEL": "Color Saturation"
    },
    {
      "NAME": "iterations",
      "TYPE": "float",
      "MIN": 1.0,
      "MAX": 10.0,
      "DEFAULT": 4.0,
      "LABEL": "Fractal Iterations"
    },
    {
      "NAME": "twist",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.2,
      "LABEL": "Twist Amount"
    }
  ]
}
*/

#define PI 3.14159265359

// Enhanced rotation with easing
mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Dynamic color palette with pulse effect
vec3 palette(float t, float pulse) {
    vec3 base = hsv2rgb(vec3(
        fract(base_hue + t * 0.1 + palette_shift),
        saturation,
        0.8 + 0.2 * sin(pulse * color_speed)
    ));
    
    // Add complementary color accents
    vec3 accent = hsv2rgb(vec3(
        fract(base_hue + t * 0.1 + 0.5 + palette_shift),
        saturation,
        0.6
    ));
    
    return mix(base, accent, 0.3 + 0.3 * sin(pulse * 2.0));
}

// Signed distance function with morphing
float sdf(vec2 p, float time) {
    p *= rot(time * 0.1 * twist);
    
    // Base shape with morphing
    float d = length(p) - 0.5;
    d = mix(d, length(p * rot(PI/4.0)) - 0.3, morph_amount * sin(time));
    
    // Add pulsing effect
    d += 0.1 * sin(time * 3.0) * morph_amount;
    return d;
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= zoom;
    
    float time = TIME * speed;
    vec3 col = vec3(0.0);
    
    // Fixed number of iterations
    for (int i = 0; i < 10; i++) {
        if (float(i) >= iterations) break; // Limit iterations based on user input
        
        uv = abs(uv) - 0.5;
        uv *= rot(time * 0.2 + float(i) * 0.3);
        
        float d = sdf(uv, time + float(i) * 0.5);
        float pulse = time + float(i) * 0.5 + length(uv);
        
        // Glow effect with parameter control
        float glow = glow_intensity * 0.01 / (0.01 + abs(d));
        col += palette(d + float(i) * 0.3, pulse) * glow;
    }
    
    // Color correction
    col = pow(col, vec3(1.0/2.2));
    gl_FragColor = vec4(col, 1.0);
}