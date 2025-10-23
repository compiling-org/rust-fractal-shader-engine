/*{
    "DESCRIPTION": "Polyhedral Grid Fractal with morphing structured symmetry.",
    "CATEGORIES": ["Sacred Geometry", "Fractal", "Visual"],
    "ISFVSN": "2.0",
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0
        },
        {
            "NAME": "MorphStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "ColorPulse",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "GridTiling",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 1.0,
            "MAX": 4.0
        }
    ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * Speed;
    vec3 color = vec3(0.0);

    for (float i = 0.0; i < 5.0; i++) {
        uv = fract(uv * GridTiling) - 0.5;
        float d = length(uv) * (1.5 + MorphStrength * sin(time * 0.8));

        color += vec3(
            sin(time + d * ColorPulse),
            cos(time + d * 1.5),
            sin(time + d * 2.5)
        ) * exp(-d * 2.0);
    }

    gl_FragColor = vec4(color, 1.0);
}
