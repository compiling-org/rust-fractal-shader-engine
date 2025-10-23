/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Psychedelic", "Fractal", "Visuals"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "blur", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0}
    ]
}
*/

vec3 palette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 1.0);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.9, 0.2, 1.5);
    return a + b * cos(6.28318 * (c * t + d * colorShift));
}

void main() {
    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE) / RENDERSIZE.y * zoom;
    vec2 uv0 = uv;
    vec3 finalColor = vec3(0.0);
    
    for (float i = 0.0; i < 3.0; i++) {
        uv = fract((0.2 * i + 0.5 * sin(0.2 * time * speed) + 1.5) * uv) - 0.5;
        float d = length(uv) * exp(sin(0.3 * time * speed) * length(uv0));
        vec3 col = palette(d + i * 0.9 + length(uv0) + time * 0.1 * colorPulse);
        
        float factor = 8.0 * morphing;
        d = sin(d * factor + time) / factor;
        d = abs(d);
        d = pow(0.02 / d, 1.2 - blur);
        
        finalColor += col * d;
    }
    
    gl_FragColor = vec4(finalColor, 1.0);
}
