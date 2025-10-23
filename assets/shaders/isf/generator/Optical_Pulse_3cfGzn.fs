/*
{
    "CATEGORIES": [
        "Fractals",
        "Shadertoy"
    ],
    "DESCRIPTION": "Waveform fractals with tunable parameters",
    "INPUTS": [
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
        { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "pulseSpeed", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0 }
    ]
}
*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= zoom;
    float time = TIME * pulseSpeed;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 6.0; i++) {
        uv = sin(uv * (3.14 + morph) + time) * 0.8;
        float d = length(uv);
        color += vec3(sin(time + i * 0.4 + colorShift), cos(time + i * 0.3 + colorShift), sin(time * 0.5 + colorShift)) * exp(-d * 3.0);
    }
    gl_FragColor = vec4(color, 1.0);
}
