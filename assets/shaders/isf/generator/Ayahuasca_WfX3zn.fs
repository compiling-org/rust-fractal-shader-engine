/*
{
    "CATEGORIES": [
        "Shadertoy Conversion",
        "Morphing Geometry"
    ],
    "DESCRIPTION": "Morphing Sacred Geometry with tunable parameters",
    "IMPORTED": {},
    "INPUTS": [
        {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0},
        {"NAME": "MorphSpeed", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0},
        {"NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "PulseIntensity", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0}
    ]
}
*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv /= Zoom;
    float time = TIME * MorphSpeed;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 10.0; i++) {
        uv = abs(uv) / dot(uv, uv) - 0.6 + 0.3 * sin(uv.yx * 4.0 + time);
        float d = length(uv);
        color += vec3(sin(time + i * ColorShift), cos(time + i * 0.5), sin(time * 0.9)) * exp(-d * 6.0);
    }
    
    color *= 1.8 + PulseIntensity * sin(TIME * 0.4);
    gl_FragColor = vec4(color, 1.0);
}
