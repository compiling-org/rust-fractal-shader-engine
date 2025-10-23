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
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0}
    ]
}
*/

vec3 palette(float t) {
    vec3 a = vec3(0.2, 0.7, 0.4);
    vec3 b = vec3(0.5, 0.8, 0.5);
    vec3 c = vec3(1.0, 2.0, 1.0);
    vec3 d = vec3(0.0, 0.33333, 0.66666);
    
    return a + b * cos(6.28318 * (c * t + d * colorShift));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float dynamicZoom = -5.0 + abs(sin(time * 0.05 * speed)) * 4.0;
    p /= zoom * dynamicZoom;
    p += p * sin(dot(p, p) * 20.0 - time * speed) * 0.04 * morphing;

    vec4 c = vec4(0.0);
    
    for (float i = 0.5; i < 8.0; i++) {
        p = abs(2.0 * fract(p - 0.5) - 1.0) * 
            mat2(cos(0.01 * (time + colorPulse) * i * i + 0.78 * vec4(1, 7, 3, 1)));

        c += exp(-abs(p.y) * 5.0) * 
             (cos(vec4(0.0, 0.7, 1.5, 0.0) * i) * 0.5 + 0.2);
    }

    c = vec4(palette(c.x), 1.0);
    c = clamp(c, 0.0, 1.0);
    
    gl_FragColor = c;
}
