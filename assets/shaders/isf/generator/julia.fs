/*{
    "DESCRIPTION": "Fractal Waves - Dynamic fractal with tunable inputs",
    "INPUTS": [
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0},
        {"NAME": "colorIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0}
    ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 color = vec3(0.0);
    float globalDist = length(uv);
    
    for (float i = 0.0; i < 4.0; i++) {
        uv = fract(uv * zoom) * 2.0 - 1.0;
        float dist = length(uv) * globalDist;
        color = 0.1 / abs(vec3(dist) - 0.5);
        color = abs(sin(color + globalDist + dist * 2.0 + TIME * speed));
        color = pow(color, vec3(colorIntensity));
    }

    gl_FragColor = vec4(color, 1.0);
}
