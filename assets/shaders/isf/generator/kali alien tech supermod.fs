/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Fractal"
    ],
    "DESCRIPTION": "Enhanced psychedelic fractal shader with tunable parameters",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.01, "MAX": 2.0 },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.30, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "morphing", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "geometry", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
        { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
        { "NAME": "colorPalette", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 3.0, 
          "LABELS": ["Rainbow", "Fire", "Acid", "Neon"] }
    ]
}
*/

#define AA_SAMPLES 2

mat2 rotate(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(c, -s, s, c);
}

vec3 getPalette(float t) {
    t = fract(t);
    if (colorPalette < 1.0) {
        // Rainbow
        return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
    } else if (colorPalette < 2.0) {
        // Fire
        return vec3(
            min(1.0, t * 1.5),
            min(1.0, max(0.0, t - 0.3)),
            0.0
        );
    } else if (colorPalette < 3.0) {
        // Acid
        return fract(vec3(t * 0.61, t * 0.83, t * 0.47));
    } else {
        // Neon
        return abs(fract(vec3(t * 3.0, t * 5.0, t * 7.0)) * 2.0 - 1.0);
    }
}

vec3 fractal(vec2 p) {
    // Camera movement
    p += vec2(sin(TIME * speed * 0.5), cos(TIME * speed * 0.3)) * 0.3;
    p *= zoom;
    
    // Morphing effect
    p *= rotate(TIME * morphing * 0.2);
    
    // Fractal iteration
    float minDist = 100.0;
    vec2 minPoint = vec2(100.0);
    
    for (int i = 0; i < 8; i++) {
        p = abs(p) / max(0.2, dot(p, p)) - geometry;
        p *= rotate(TIME * 0.1);
        minDist = min(minDist, length(p));
        minPoint = min(minPoint, abs(p));
    }
    
    // Color calculation
    float colorTime = minDist * 2.0 + TIME * colorPulse;
    vec3 color = getPalette(colorTime);
    
    // Add glow effect
    float glow = pow(max(0.0, 1.0 - minDist), 2.0);
    return color * (1.0 + glow * 2.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
    
    // Anti-aliasing
    vec3 col = vec3(0.0);
    for (int i = -AA_SAMPLES; i <= AA_SAMPLES; i++) {
        for (int j = -AA_SAMPLES; j <= AA_SAMPLES; j++) {
            vec2 offset = vec2(i, j) / RENDERSIZE.xy;
            col += fractal(uv + offset * 0.5);
        }
    }
    col /= float((AA_SAMPLES * 2 + 1) * (AA_SAMPLES * 2 + 1));
    
    // Vignette
    col *= 1.0 - 0.5 * dot(uv, uv);
    
    gl_FragColor = vec4(col, 1.0);
}