/*
{
    "DESCRIPTION": "Psychedelic morphing pattern with tunable pulse, palette, morph, and ghost trail.",
    "CATEGORIES": ["Psychedelic", "Color", "ISF"],
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 4.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "morph",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Morph Intensity"
        },
        {
            "NAME": "colorPulse",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Color Pulse Speed"
        },
        {
            "NAME": "ghostTrail",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MIN": 0.5,
            "MAX": 4.0,
            "LABEL": "Ghost Trail Strength"
        },
        {
            "NAME": "paletteSelect",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Palette",
            "VALUES": ["Classic", "Acid", "Chill"]
        }
    ]
}
*/

#define TAU 6.28318530718
#define rotation(angle) mat2(cos(angle), -sin(angle), sin(angle), cos(angle))

float eqTri(vec2 p, float r) {
    const float k = sqrt(3.0);
    p.x = abs(p.x) - r;
    p.x = p.x + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

vec3 palette(float t, float mode) {
    if (mode == 1.0) return vec3(0.6 + 0.4 * sin(TAU * t + vec3(0.0, 0.33, 0.67))); // Acid
    if (mode == 2.0) return vec3(0.4 + 0.4 * cos(TAU * t + vec3(0.6, 0.1, 0.3)));   // Chill
    return vec3(0.5 + 0.5 * cos(TAU * t + vec3(0.263, 0.416, 0.557)));           // Classic
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv;
    uv = normalize(vec3(uv, 1.2)).xy;

    float t = TIME * speed;
    float rep = 3.0;
    float r3 = mod(length(uv), 22.0);
    float a3 = sqrt(r3) - t * 0.12;

    float theta = atan(uv.y, uv.x);
    vec2 polarUV = vec2(-theta, a3);
    uv = fract(polarUV);

    vec3 finalColor = vec3(0.0);
    vec2 cPos = -1.0 + 2.0 * gl_FragCoord.xy / RENDERSIZE.xy;
    float cLength = length(cPos);

    for (float j = 0.0; j < 4.0; j++) {
        vec3 col2 = vec3(0.0);
        float tShift = fract(0.1 * t * 0.51);
        uv *= rotation(3.0 * TAU * (0.3 - clamp(length(uv), 0.0, 0.3)));

        // Optional ripple warp
        uv += (cPos / cLength) * cos(cLength * 4.0 - t * 4.0) * 0.53 * morph;

        float s = -1.0;
        for (float i = 0.0; i < 5.0; i++) {
            float rad = 1.4 / pow(2.0, i) * (0.9 - 0.2 * i);
            uv *= rotation(-2.0 * s * (i + 1.0) * TAU * tShift);
            float tri = eqTri(uv, rad);
            s *= -1.0;
            col2 += 1.004 / abs(tri);
        }

        uv = fract(uv * 1.5) - 0.5;
        float d = length(uv) * exp(-length(uv0));
        vec3 col = palette(length(uv0) + j * 0.4 + t * colorPulse, paletteSelect);
        d = sin(d * 8.0 + t) / 8.0;
        d = abs(d);
        d = pow(0.01 / d, ghostTrail);
        finalColor += col * d;
    }

    gl_FragColor = vec4(finalColor, 1.0);
}