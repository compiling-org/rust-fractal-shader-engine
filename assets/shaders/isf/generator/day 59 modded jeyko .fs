/*{
  "DESCRIPTION": "Day 59 with zoom, symmetry, tilt, glitch, freeze, palette morphs, edge and shape morphing",
  "CREDIT": "Original by jeyko, extended by Shader Genius",
  "INPUTS": [
    { "NAME": "zoom",          "TYPE": "float", "DEFAULT": 4.10, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "gap",           "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "edgeWidth",     "TYPE": "float", "DEFAULT": 0.003, "MIN": 0.001, "MAX": 0.05 },
    { "NAME": "colorPulse",    "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "psyPalette",    "TYPE": "float",  "DEFAULT": 4, "MIN": 0, "MAX": 5 },
    { "NAME": "shapeMorph",    "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "squareTilt",    "TYPE": "float", "DEFAULT": 0.97, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed",         "TYPE": "float", "DEFAULT": 0.30, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "symmetry",      "TYPE": "float", "DEFAULT": 0.330, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "glitch",        "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "freezeTime",    "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "paletteOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0 }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.28318530718
#define ROT(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define PLANES 40.0
#define spacing 0.06

float rand2D(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898,78.233))) * 43758.5453);
}

float dotNoise2D(float x, float y, float maxDotSize, float dDensity) {
    float ix = x - fract(x);
    float fx = x - ix;
    float iy = y - fract(y);
    float fy = y - iy;
    if (rand2D(vec2(ix + 1.0, iy + 1.0)) > dDensity) return 0.0;
    float xo = rand2D(vec2(ix, iy)) - 0.5;
    float yo = rand2D(vec2(ix + 1.0, iy)) - 0.5;
    float dotSize = 0.5 * maxDotSize * max(0.25, rand2D(vec2(ix, iy + 1.0)));
    vec2 truePos = vec2(0.5 + xo * (1.0 - 2.0 * dotSize), 0.5 + yo * (1.0 - 2.0 * dotSize));
    float dist = length(truePos - vec2(fx, fy));
    return 1.0 - smoothstep(0.3 * dotSize, 1.0 * dotSize, dist);
}

vec3 palette(float t, int p) {
    if (p == 0) return 0.5 + 0.5 * sin(TAU * (vec3(0.1, 0.3, 0.6) + t));
    if (p == 1) return vec3(
        sin(t * 3.0) * 0.5 + 0.5,
        cos(t * 1.9 + 1.3) * 0.5 + 0.5,
        sin(t * 4.1 + 2.7) * 0.5 + 0.5
    );
    if (p == 2) return vec3(
        sin(t * 2.0 + 1.0) * 0.4 + 0.6,
        pow(cos(t * 3.2), 2.0),
        0.2 + 0.3 * sin(t * 1.5 + 0.7)
    );
    if (p == 3) return 0.5 + 0.5 * cos(vec3(1.2, 0.8, 2.3) * t + vec3(0.0, 2.0, 4.0));
    if (p == 4) return vec3(
        pow(abs(sin(t * 10.0 + float(p))), 3.0),
        cos(t * 5.0 + float(p)),
        sin(t * 3.0 + float(p))
    );
    if (p == 5) return vec3(
        sin(t * 7.2 + 1.0),
        sin(t * 6.7 + 0.5),
        sin(t * 8.5 + 2.3)
    ) * 0.5 + 0.5;
    return vec3(1.0);
}

vec3 sumonTheDemon(vec2 p, float id, float t) {
    vec3 col = vec3(0.0);
    vec2 k = p;

    p.x += gap;

    float tiltAngle = mix(0.0, 0.25 * PI, squareTilt);
    p *= ROT(tiltAngle);

    float circle = length(p) - 0.4;
    float square = max(abs(p.x), abs(p.y)) - 0.4;
    float fig = mix(square, circle, shapeMorph);
    fig = mod(fig * (20. + 5. * sin(t + id)), 1.0) - 0.5;

    float stroke = smoothstep(edgeWidth * 1.5, edgeWidth, abs(fig));
    vec3 pal = palette(t * colorPulse + float(id) * 0.03 + paletteOffset, int(psyPalette)) * stroke;
    col += pal;

    k += id;
    for (int i = 0; i < 2; i++) {
        k = abs(k);
        k.x -= 0.6;
        k *= ROT(2.0);
    }
    k *= 20.0;
    col += dotNoise2D(k.x, k.y, 0.05, 1.0) * 0.4;

    return col;
}

float iPlane(vec3 ro, vec3 rd, vec3 p0, vec3 n) {
    float denom = dot(rd, n);
    if (denom > 1e-6) {
        float t = -dot(ro - p0, n) / denom;
        return t > 0.0 ? t : 1e10;
    }
    return 1e10;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float t = TIME * speed;
    if (freezeTime) t = floor(t); // fixed: use boolean directly

    // Symmetry
    if (symmetry >= 1.0) uv.x = abs(uv.x);
    if (symmetry >= 2.0) uv.y = abs(uv.y);

    // Glitch FX
    uv += glitch * vec2(
        sin(uv.y * 200.0 + t * 30.0),
        cos(uv.x * 150.0 - t * 40.0)
    ) * 0.005;

    vec3 ro = vec3(0.0);
    vec3 rd = normalize(vec3(uv, 1.0));
    vec3 col = vec3(0.0);

    for (float i = 0.0; i < PLANES; i++) {
        float z = mod(-t + i, PLANES);
        float plA = iPlane(ro, rd, vec3(0.0, 0.0, z) * spacing, vec3(0.0, 0.0, 1.0));
        vec2 p = (ro + rd * plA).xy * zoom;
        col += sumonTheDemon(p, i, t) * smoothstep(1.0, 0.0, plA * 1.4);
    }

    gl_FragColor = vec4(col, 1.0);
}
