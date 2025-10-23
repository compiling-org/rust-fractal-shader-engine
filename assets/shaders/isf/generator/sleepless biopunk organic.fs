
// ISF Shader with tunable parameters

/*
{
    "DESCRIPTION": "Converted from GLSL, with tunable zoom, morphing, color shift, and pulses.",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
        { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
        { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0 }
    ]
}
*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * 0.7 * morph;
    uv *= zoom;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 5.0; i++) {
        uv = cos(uv * 2.8 + time * 0.9) / dot(uv, uv) - 0.3;
        float d = length(uv) + tan(uv.y * 8.0 + time * 1.1 * pulse) * 0.06;
        color += vec3(
            tan(time + i * 0.4 + colorShift), 
            sin(time + i * 0.3 + colorShift), 
            cos(time * 0.5 + colorShift)
        ) * exp(-d * 3.8);
    }
    
    gl_FragColor = vec4(color, 1.0);
}
