/*
{
  "DESCRIPTION": "Psychedelic fractal shader with customizable parameters",
  "CATEGORIES": ["Psychedelic", "Fractal"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 2.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morphing", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "fractalControl", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "warping", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPalette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, 
      "LABELS": ["Rainbow", "Neon", "Fire", "Ice"] },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 }
  ],
  "PASSES": [
    {
      "TARGET": "bufferA",
      "PERSISTENT": true
    }
  ]
}
*/

#define MAX_ITERATIONS 7

mat2 rotate2d(float angle) {
    return mat2(cos(angle), -sin(angle),
                sin(angle), cos(angle));
}

vec3 paletteRainbow(float t) {
    return vec3(0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67))));
}

vec3 paletteNeon(float t) {
    return vec3(0.5 + 0.5 * sin(6.28318 * (t + vec3(0.0, 0.2, 0.4))));
}

vec3 paletteFire(float t) {
    return vec3(
        pow(t, 0.6),
        pow(t, 0.3),
        0.0
    );
}

vec3 paletteIce(float t) {
    return vec3(
        0.0,
        pow(t, 0.5) * 0.8,
        pow(t, 0.3) * 1.2
    );
}

vec3 getColor(float t) {
    if (colorPalette < 1.0) return paletteRainbow(t);
    if (colorPalette < 2.0) return paletteNeon(t);
    if (colorPalette < 3.0) return paletteFire(t);
    return paletteIce(t);
}

vec3 fractal(vec2 p) {
    p += vec2(sin(TIME * speed), cos(TIME * speed)) * 0.3 * zoom;
    p /= dot(p, p);
    p *= rotate2d(TIME * speed * 0.5);
    p *= sin(TIME * 0.2 * morphing);
    
    float m = 10.0;
    vec2 mc = vec2(10.0);
    
    for (int i = 0; i < MAX_ITERATIONS; i++) {
        p = abs(p) - 0.5;
        p *= rotate2d(TIME * warping * (float(i) / float(MAX_ITERATIONS)));
        p *= 1.2;
        m = min(m, length(p));
        mc = min(mc, abs(p));
    }
    
    float t = mod(m * 2.0 + TIME * colorPulse, 1.0);
    vec3 baseColor = getColor(t);
    vec3 mcColor = getColor(mod(length(mc) * 4.0 + TIME * 0.25, 1.0));
    
    return mix(baseColor, mcColor, 0.7) * saturation * brightness;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    
    if (PASSINDEX == 0) {
        vec2 pos = (gl_FragCoord.xy - RENDERSIZE * 0.5) / RENDERSIZE.y;
        vec3 col = fractal(pos);
        
        // Feedback mixing if buffer exists
        if (TIME > 0.0) {
            vec4 feedback = IMG_NORM_PIXEL(bufferA, isf_FragNormCoord);
            col = mix(col, feedback.rgb, 0.1);
        }
        
        gl_FragColor = vec4(col, 1.0);
    } else {
        // Final output pass
        gl_FragColor = IMG_NORM_PIXEL(bufferA, uv * 0.5 + 0.5);
    }
}