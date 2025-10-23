/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/mlSSz3 by sleeplessmonk. Audio reactivity added.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "param1",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Audio Amplitude"
        },
        {
            "NAME": "param2",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "param3",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Flap Modulation"
        },
        {
            "NAME": "param4",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "LABEL": "Fractal Detail"
        }
    ]
}
*/

vec3 NeonPsychedelicColor(float t) {
    return vec3(
        0.5 + 0.5 * sin(6.0 * t + 2.0),
        0.5 + 0.5 * sin(6.0 * t + 1.0),
        0.5 + 0.5 * sin(6.0 * t + 3.0)
    );
}

vec3 JuliaFractal(vec2 c, vec2 c2, float animparam, float anim2) {
    vec2 z = c;

    float ci = 0.0;
    float mean = 0.0;

    for (int i = 0; i < 48; i++) {
        vec2 a = vec2(z.x, abs(z.y));

        float b = atan(a.y * (0.99 + animparam * 9.0), a.x + 0.110765432 + animparam);

        if (b > 0.0) b -= 6.303431307 + (animparam * 3.1513);

        z = vec2(log(length(a * (0.98899 - (animparam * 2.70 * anim2)))), b) + c2;

        if (i > 0) mean += length(z / a * b);

        mean += a.x - (b * 70.0 / length(a * b));

        mean = clamp(mean, 111.0, 99999.0);
    }

    mean /= 131.21;
    ci = 1.0 - fract(log2(.5 * log2(mean / (0.57891895 - abs(animparam * 141.0)))));

    return NeonPsychedelicColor(ci);
}

void main() {
    // Use tunable parameters instead of audio input
    float animWings = 0.01 * param1 * cos(TIME * param2);  // Audio amplitude controlled by param1
    float animFlap = 0.005 * param1 * sin(TIME * param2);  // Flap modulation controlled by param1 and param2
    
    float timeVal = 56.48 - 20.1601;
    vec2 uv = gl_FragCoord.xy - RENDERSIZE.xy * 0.5;
    uv /= RENDERSIZE.x * 1.5113 * abs(sin(timeVal));
    uv.y -= animWings * 5.0;
    vec2 tuv = uv * 125.0;
    float rot = 3.141592654 * 0.5;

    uv.x = tuv.x * cos(rot) - tuv.y * sin(rot);
    uv.y = 1.05 * tuv.x * sin(rot) + tuv.y * cos(rot);
    
    float juliax = tan(timeVal) * 0.011 + 0.02 / (gl_FragCoord.y * 0.19531 * (1.0 - animFlap));
    float juliay = cos(timeVal * 0.213) * (0.022 + animFlap + 0.05 * param1) + 5.66752 - juliax * 1.5101;

    float tapU = (1.0 / float(RENDERSIZE.x)) * 25.5;
    float tapV = (1.0 / float(RENDERSIZE.y)) * 25.5;

    // Adjust fractal detail using param4
    gl_FragColor = vec4(JuliaFractal(uv + vec2(0.0, 0.0), vec2(juliax, juliay), animWings, animFlap), 1.0);
    gl_FragColor += vec4(JuliaFractal(uv + vec2(tapU, tapV), vec2(juliax, juliay), animWings, animFlap), 1.0);
    gl_FragColor += vec4(JuliaFractal(uv + vec2(-tapU, -tapV), vec2(juliax, juliay), animWings, animFlap), 1.0);
    gl_FragColor *= 0.3333;
    gl_FragColor.xyz = gl_FragColor.zyx;
    gl_FragColor.xyz = vec3(1) - gl_FragColor.xyz;
}
