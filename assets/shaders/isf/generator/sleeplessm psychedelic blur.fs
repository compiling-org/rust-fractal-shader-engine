/*
{
    "CATEGORIES": ["Fractal", "Trippy"],
    "DESCRIPTION": "Converted shader with tunable parameters for color shift, pulse, and artifact intensity.",
    "INPUTS": [
        { "NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.1, "MAX": 1.0 },
        { "NAME": "ArtifactIntensity", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 }
    ]
}
*/

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * 0.5;
    vec3 color = vec3(0.0);
    
    for (float i = 0.0; i < 7.0; i++) {
        uv = abs(uv) / dot(uv, uv) - 0.5;
        float d = length(uv);
        color += vec3(sin(time + i * ColorShift) * 0.5 + 0.5, 
                      cos(time + i * 0.3) * 0.5 + 0.5, 
                      sin(time * 0.7) * 0.5 + 0.5) * exp(-d * ArtifactIntensity);
    }
    
    color *= 1.5 + 0.5 * sin(TIME * ColorPulse);
    gl_FragColor = vec4(color, 1.0);
}