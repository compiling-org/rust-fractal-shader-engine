/*{
    "DESCRIPTION": "Kaleidoscopic Waves",
    "CATEGORIES": ["Kaleidoscope", "Morphing"],
    "INPUTS": [
        {"NAME": "warp_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "twist_speed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0}
    ]
}*/

#define PI 3.1415926538

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 finalColor = vec3(0.0);
    
    float time = TIME * twist_speed;
    
    uv *= mat2(cos(time), -sin(time), sin(time), cos(time));
    uv = fract(uv * warp_intensity) - 0.5;
    
    float dist = length(uv);
    float wave = sin(PI * 4.0 * dist + time * 2.0);
    vec3 color = vec3(0.5 * wave + 0.5, 0.3 * wave + 0.7, 0.8 * wave + 0.2);
    
    gl_FragColor = vec4(color, 1.0);
}
