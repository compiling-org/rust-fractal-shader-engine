/*{
    "CATEGORIES": [
        "Procedural"
    ],
    "DESCRIPTION": "Trippy fractal shader with color shift, pulse, and artifacts.",
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
            "LABEL": "Imaginal Artifacts"
        }
    ]
}*/

#define PI 3.14159265358979
#define N 12

mat2 rot(float a) {
    return mat2(cos(a), sin(a), -sin(a), cos(a));
}

vec2 pmod(vec2 p, float r) {
    float a = atan(p.x, p.y) + PI / r;
    float n = 2.0 * PI / r;
    a = floor(a / n) * n;
    return p * rot(-a);
}

float box(vec2 p, vec2 b) {
    vec2 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
}

void main() {
    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 uv = p;

    float t = TIME * 0.1 + ((0.25 + 0.05 * sin(TIME * 0.1)) / (length(uv.xy) + 0.07)) * 5.2;
    float si = sin(t);
    float co = cos(t);
    mat2 ma = mat2(co, si, -si, co);

    float size = 0.2;
    float dist = 0.0;
    float ang = 0.0;
    vec2 pos = vec2(0.0);
    vec3 color = vec3(0.1);

    for (int i = 0; i < N; i++) {
        float r = 0.4;
        ang += PI / (float(N) * 0.5);
        pos = vec2(cos(ang), sin(ang)) * r * cos(TIME + ang / 0.18);
        dist += size / distance(pos, p);
        vec3 c = vec3(0.05);
        color = c * dist;
    }

    vec3 col = vec3(0.0);
    uv = pmod(uv, (sin(TIME) * 1.0));

    for (int i = 0; i < 8; i++) {
        uv = abs(uv) - 0.05;
        uv *= rot(TIME);
        float box = box(uv * ma, vec2(0.5 - uv));

        float w = 0.8;
        vec3 x = vec3(0.0, 0.1, 1.0) * (0.05 * w) / length(box);
        vec3 xc = vec3(1.0, 0.5, 0.1) * (0.001) / length(uv.x)
                + vec3(1.0, 0.1, 0.1) * (0.001) / length(uv.y)
                + vec3(1.0, 1.0, 0.1) * (0.0015) / length(uv);

        col += x + xc * color;
    }

    // **Color Shift Effect**
    col = col * vec3(sin(ColorShift * TIME), cos(ColorShift * TIME), sin(ColorShift * TIME * 1.1));

    // **Color Pulse Effect**
    col *= 1.0 + ColorPulse * 0.5 * sin(TIME * 0.5);

    // **Imaginal Artifacts Effect**
    col += ArtifactStrength * vec3(sin(uv.x * 10.0), cos(uv.y * 10.0), sin(uv.x * uv.y * 5.0));

    gl_FragColor = vec4(col, 1.0);
}
