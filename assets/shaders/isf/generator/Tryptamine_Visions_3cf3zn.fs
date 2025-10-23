/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/3cf3zn by sleeplessmonk. Rotating Fractal Mandala with tunable audio modulation",
    "INPUTS": [
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0
        },
        {
            "NAME": "MorphSpeed",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.1,
            "MAX": 2.0
        },
        {
            "NAME": "ColorShift",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 3.0
        },
        {
            "NAME": "PulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 3.0
        },
        {
            "NAME": "AudioModulation",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        }
    ]
}
*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * MorphSpeed;
    
    // Use tunable parameter instead of external audio input
    float audioInput = AudioModulation;
    audioInput = smoothstep(0.0, 1.0, audioInput);
    
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 12.0; i++) {
        uv = mat2(cos(time + audioInput), -sin(time + audioInput),
                  sin(time + audioInput), cos(time + audioInput)) * uv;
        uv = abs(uv) / dot(uv, uv) - 0.5;
        float d = length(uv) * Zoom;
        
        color += vec3(
            sin(time + i * 0.9 + audioInput * ColorShift),
            cos(time + i * 0.7 + audioInput * ColorShift),
            sin(time * 1.1 + audioInput * ColorShift)
        ) * exp(-d * 7.0);
    }
    
    float noise = fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
    color += vec3(noise * 0.1 * audioInput);
    
    color *= 2.0 + 0.8 * sin(time * PulseSpeed + audioInput * 3.0);
    color = clamp(color, 0.0, 1.0);
    
    gl_FragColor = vec4(color, 1.0);
}