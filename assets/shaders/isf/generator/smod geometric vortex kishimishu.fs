/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Psychedelic", "Abstract"],
    "DESCRIPTION": "Converted Shadertoy effect with tunable parameters for morphing visuals.",
    "INPUTS": [
        {"NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14},
        {"NAME": "Pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0},
        {"NAME": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0}
    ]
}

*/


#define PI 3.141592
#define S(a,b,d) mix(a, b, sin(d + n * PI * 2.0) * 0.5 + 0.5)
#define rot(a) mat2(cos(a + vec4(0, 33, 11, 0)))
#define n (-TIME * 0.04 * Speed + 0.03)

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / (RENDERSIZE.y * Zoom);
    vec4 color = vec4(0.0);
    
    for (float s = 0.0; s < 3.0; s++) {
        float p = 9.0;
        for (float i = 0.0; i < 25.0; i++) {
            vec2 a = fract(rot(i * sin(n * PI * 2.0) * 0.25) * uv * (i + S(1.0, 4.0, PI / 2.0)) + 0.5) - 0.5;
            float r = mix(length(a), abs(a.x) + abs(a.y), S(0.0, 1.0, Morph));
            float t = abs(r + 0.1 - s * 0.02 - i * S(0.005, 0.05, Pulse));
            p = min(p, smoothstep(0.0, 0.1 + s * i * S(0.0, 0.015, PI), t * S(s * 0.1 + 0.14, 0.2, Morph)) +
                smoothstep(0.0, 20.0, i * S(0.45, 1.0, Pulse)) + smoothstep(0.0, 1.0, length(uv) * i * 0.08));
        }
        color[int(s)] = 0.1 / p;
    }
    
    color.rgb = mix(color.rgb, vec3(sin(ColorShift), cos(ColorShift), sin(ColorShift + PI * 0.5)), 0.5);
    gl_FragColor = vec4(color.rgb, 1.0);
}


