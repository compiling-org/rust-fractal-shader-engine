/*{
    "DESCRIPTION": "Converted from GLSL to ISF with tunable parameters for color shift, pulse, and visual artifacts.",
    "CATEGORIES": ["Visual", "Psychedelic"],
    "ISFVSN": "2.0",
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
            "MIN": 0.5,
            "MAX": 2.5,
            "DEFAULT": 1.0,
            "LABEL": "Color Pulse"
        },
        {
            "NAME": "ImaginalArtifacts",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Imaginal Artifacts"
        }
    ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * 0.9;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 12.0; i++) {
        float tMod = time + i * ColorShift;
        
        // Rotation matrix
        uv = mat2(cos(tMod), -sin(tMod), sin(tMod), cos(tMod)) * uv;
        
        // Fractal warping based on "ImaginalArtifacts" parameter
        uv = abs(uv) / (dot(uv, uv) + 0.1 * ImaginalArtifacts) - 0.5;
        
        float d = length(uv);
        
        // Color modulation based on tunable parameters
        color += vec3(
            sin(tMod * ColorPulse),
            cos(tMod * ColorPulse),
            sin(tMod * 1.1)
        ) * exp(-d * (7.0 + ImaginalArtifacts * 5.0));
    }
    
    color *= 2.0 + 0.8 * sin(TIME * 0.5 * ColorPulse);
    
    gl_FragColor = vec4(color, 1.0);
}
