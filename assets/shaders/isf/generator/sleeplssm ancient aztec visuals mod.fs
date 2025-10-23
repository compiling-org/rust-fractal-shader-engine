/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Visual", "Generative", "Psychedelic"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0}
    ]
}
*/

vec3 pal(float t) {
    // Psychedelic color palette
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.3, 0.6, 0.8) * sin(t * 5.0) + vec3(0.2, 0.4, 0.6) * cos(t * 3.0);
    
    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv_0 = uv;
    
    float it = 1.35 * time * speed;
    
    vec3 outColor = vec3(0.0);    

    for (float i = 0.0; i < 2.0; i++) {
        vec3 color = pal(length(0.333 * uv_0 + 0.01 * it) + 0.001 * i + 0.16 * it) * colorPulse;
        
        uv = fract(1.618 * uv * zoom) - 0.5;
        float d = 0.933 * length(uv) * 1.33 * exp(-length(uv_0));
        
        d = sin(26.0 * d + it) / 28.0;
        d = abs(d);
        d = pow(0.005 / d, 1.2) * morphing;

        outColor += color * d;
    }
    
    // Apply color shift
    outColor.rgb = mix(outColor.rgb, outColor.bgr, colorShift);
    
    gl_FragColor = vec4(outColor, 1.0);
}
