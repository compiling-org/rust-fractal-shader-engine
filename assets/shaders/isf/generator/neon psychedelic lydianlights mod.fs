/*
{
    "CATEGORIES": [
        "Generator",
        "Hexagons",
        "Color"
    ],
    "DESCRIPTION": "Hexagon pattern generator with enhanced tunable parameters",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 10.0,
            "DEFAULT": 1.0,
            "LABEL": "Zoom Level"
        },
        {
            "NAME": "ColorSeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Seed"
        },
        {
            "NAME": "LayerCount",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "LABEL": "Number of Layers"
        },
        {
            "NAME": "Frequency",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Hexagon Frequency"
        },
        {
            "NAME": "Brightness",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Brightness"
        }
    ],
    "PASSES": [
        {},
        {}
    ]
}
*/

const float PI = 3.1415927;
const float ROOT_3 = 1.7320508;

// Color palette function
vec3 palette(in float t, in float seed) {
    vec3 a = vec3(0.5, 0.5, 0.7);
    vec3 b = vec3(0.8, 0.2, 0.7);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.0, 0.2, 0.6);
    return a + b * cos(6.28318 * (c * (t + seed) + d));
}

// Hash function for randomness
vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// Hexagon coordinate system
vec4 getHex(vec2 p) {
    const vec2 s = vec2(1, ROOT_3);
    vec4 hC = floor(vec4(p, p - vec2(.5, 1)) / s.xyxy) + .5;
    vec4 h = vec4(p - hC.xy * s, p - (hC.zw + .5) * s);
    return dot(h.xy, h.xy) < dot(h.zw, h.zw)
        ? vec4(h.xy, hC.xy)
        : vec4(h.zw, hC.zw + .5);
}

// Distance to hexagon center
float hexDist(in vec2 p) {
    const vec2 s = vec2(1, ROOT_3);
    p = abs(p);
    return max(dot(p, s * .5), p.x);
}

// Main rendering pass
vec3 mainPass(in vec2 fragCoord) {
    const float r = PI / 6.0;
    const mat2 rot = mat2(cos(r), sin(r), -sin(r), cos(r));

    // Adjust UV coordinates for aspect ratio and zoom
    vec2 uv0 = 2.0 * (fragCoord / RENDERSIZE.xy) - 1.0;
    uv0.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv0 *= Zoom;

    vec2 uv = uv0;
    
    // Get initial hexagon coordinates
    vec2 h0 = getHex(0.5 * uv).xy;
    vec2 h = h0;

    float d0 = length(uv0);
    vec3 color = vec3(0.0);

    // Fixed loop count with LayerCount influence
    const int MAX_LAYERS = 10;
    int layers = int(min(LayerCount, float(MAX_LAYERS)));
    for (int i = 0; i < MAX_LAYERS; i++) {
        if (i >= layers) break;
        
        h = getHex(1.1 * ROOT_3 * h * rot).xy;

        float d = hexDist(h);
        d = 2.0 * d * pow(0.2, d0);
        d = 0.5 * sin(Frequency * d - 0.5 * TIME * Speed + float(i) * 2.0 * PI / 7.0);
        d = 0.04 / d;
        d = pow(d, 2.0);

        vec3 c = d * Brightness * palette(length(h0) + 0.3 * float(i), ColorSeed);
        color += c;
    }

    return color;
}

// Gamma correction
vec3 gamma(in vec3 color) {
    return pow(color, vec3(1.0 / 2.2));
}

void main() {
    if (PASSINDEX == 0) {
        // First pass
    } else if (PASSINDEX == 1) {
        // Second pass: Render the scene
        vec3 color = mainPass(gl_FragCoord.xy);
        color = gamma(color);
        gl_FragColor = vec4(color, 1.0);
    }
}