/*{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Biopunk"
    ],
    "DESCRIPTION": "Biopunk Fractal Mysticism with tunable parameters for color and distortions.",
    "INPUTS": [
        {
            "NAME": "ColorShift",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 3.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Shift"
        },
        {
            "NAME": "ColorPulse",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Pulse"
        },
        {
            "NAME": "ArtifactStrength",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Artifact Intensity"
        }
    ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * 1.3;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 8.0; i++) {
        uv = mat2(sin(time), cos(time), -cos(time), sin(time)) * uv;
        uv = abs(uv) / dot(uv, uv) - 0.6;
        float d = length(uv) + tan(uv.y * 18.0 + time * 2.1) * 0.09;
        
        vec3 colorMod = vec3(sin(time + i * 0.8 * ColorShift),
                             tan(time + i * 0.7 * ColorShift),
                             cos(time * 1.2 * ColorShift));
                             
        color += colorMod * exp(-d * 6.5);
    }
    
    // **Color Pulse Effect**
    color *= 1.0 + ColorPulse * 0.5 * sin(TIME * 0.5);

    // **Artifact Intensity Effect**
    color += ArtifactStrength * vec3(sin(uv.x * 10.0), cos(uv.y * 10.0), sin(uv.x * uv.y * 5.0));

    gl_FragColor = vec4(color, 1.0);
}
