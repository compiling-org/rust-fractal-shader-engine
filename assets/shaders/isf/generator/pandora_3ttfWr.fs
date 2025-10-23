/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Trippy kaleidoscopic effect with fully tunable parameters replacing texture inputs.",
    "INPUTS": [
        {
            "NAME": "PatternIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 2.0,
            "LABEL": "Pattern Intensity"
        },
        {
            "NAME": "RotationSpeed",
            "TYPE": "float",
            "MIN": 0.01,
            "MAX": 0.5,
            "DEFAULT": 0.05,
            "LABEL": "Rotation Speed"
        },
        {
            "NAME": "ColorMix",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Color Mix"
        },
        {
            "NAME": "ColorR",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Red Channel Influence"
        },
        {
            "NAME": "ColorG",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Green Channel Influence"
        },
        {
            "NAME": "ColorB",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Blue Channel Influence"
        }
    ]
}
*/

vec2 N(float angle) {
    return vec2(sin(angle), cos(angle));
}

mat2 rotate2d(float _angle) {
    return mat2(cos(_angle), -sin(_angle),
                sin(_angle), cos(_angle));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    vec3 col = vec3(0);
    uv.x = abs(uv.x);
    uv.y += tan((5.0 / 6.0) * 3.1415) * 0.5;
    
    vec2 n = N((5.0 / 6.0) * 3.1415);
    float d = dot(uv - vec2(0.5, 0.0), n);
    uv -= n * max(0.0, d) * 2.0;

    // Use parameters for custom animation control
    float adjustedTime = TIME * RotationSpeed;
    n = N(abs(sin(adjustedTime * 0.3) * 2.0) / abs(sin(adjustedTime * 0.3) * 8.0) * adjustedTime * 3.1415) * rotate2d(3.14 * adjustedTime);
    
    float scale = 1.0;
    uv.x += 0.5;

    for (int i = 0; i < 8; i++) {
        float val = PatternIntensity;
        uv *= val;
        scale *= val;
        uv.x -= val / 2.0;

        uv.x = abs(uv.x);
        uv.x -= 0.5;
        uv -= n * max(0.0, dot(uv, n)) * 2.0;
    }

    d = length(uv - vec2(clamp(uv.x, -1.0, 1.0), 0));
    col += smoothstep(1.0 / RENDERSIZE.y, 0.0, d / scale);
    uv /= scale;
    uv *= rotate2d(3.14 * adjustedTime);

    // Procedural color generation replacing iChannel textures
    float r = sin(uv.x * 10.0 + adjustedTime) * 0.5 + 0.5;
    float g = sin(uv.y * 10.0 + adjustedTime) * 0.5 + 0.5;
    float b = cos((uv.x + uv.y) * 10.0 + adjustedTime) * 0.5 + 0.5;

    vec3 colorEffect1 = vec3(r, g, b);
    vec3 colorEffect2 = vec3(b, r, g);
    vec3 colorEffect3 = vec3(g, b, r);

    col += mix(colorEffect1, colorEffect2, ColorMix);
    col /= mix(colorEffect3, colorEffect1, ColorMix);

    // Apply tunable color influences
    col.r *= ColorR;
    col.g *= ColorG;
    col.b *= ColorB;

    gl_FragColor = vec4(col, 1.0);
}
