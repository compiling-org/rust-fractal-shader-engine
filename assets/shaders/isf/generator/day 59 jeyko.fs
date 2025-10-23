/*{
  "DESCRIPTION": "Day 59 by jeyko extended with zoom, tilt, morph, edge control, and real palettes",
  "CREDIT": "Original by jeyko, extended by Shader Genius",
  "INPUTS": [
    { "NAME": "zoom",         "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "gap",          "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "edgeWidth",    "TYPE": "float", "DEFAULT": 0.003, "MIN": 0.001, "MAX": 0.05 },
    { "NAME": "colorPulse",   "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "psyPalette",   "TYPE": "float",  "DEFAULT": 0, "MIN": 0, "MAX": 3 },
    { "NAME": "shapeMorph",   "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "squareTilt",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed",        "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 5.0 }
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
    if (p == 0) {
        return 0.5 + 0.5 * sin(TAU * (vec3(0.1, 0.3, 0.6) + t));
    }
    if (p == 1) {
        return vec3(
            sin(t * 3.0) * 0.5 + 0.5,
            cos(t * 1.9 + 1.3) * 0.5 + 0.5,
            sin(t * 4.1 + 2.7) * 0.5 + 0.5
        );
    }
    if (p == 2) {
        return vec3(
            sin(t * 2.0 + 1.0) * 0.4 + 0.6,
            pow(cos(t * 3.2), 2.0),
            0.2 + 0.3 * sin(t * 1.5 + 0.7)
        );
    }
    if (p == 3) {
        return 0.5 + 0.5 * cos(vec3(1.2, 0.8, 2.3) * t + vec3(0.0, 2.0, 4.0));
    }
    return vec3(1.0);
}

vec3 sumonTheDemon(vec2 p, float id, float t) {
    vec3 col = vec3(0.0);
    vec2 k = p;

    // Apply gap offset
    p.x += gap;

    // Tilt (square rotation toward diagonal alignment)
    float tiltAngle = mix(0.0, 0.25 * PI, squareTilt);
    p *= ROT(tiltAngle);

    // Shape morph between square and circle
    float circle = length(p) - 0.4;
    float square = max(abs(p.x), abs(p.y)) - 0.4;
    float fig = mix(square, circle, shapeMorph);
    fig = mod(fig * (20. + 5. * sin(t + id)), 1.0) - 0.5;

    float stroke = smoothstep(edgeWidth * 1.5, edgeWidth, abs(fig));
    vec3 pal = palette(t + id * 0.03, int(psyPalette)) * stroke;
    col += pal;

    // Dot star noise
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
    float mx = t;
    vec3 ro = vec3(0.0);
    vec3 rd = normalize(vec3(uv, 1.0));

    vec3 col = vec3(0.0);
    for (float i = 0.0; i < PLANES; i++) {
        float z = mod(-mx + i, PLANES);
        float plA = iPlane(ro, rd, vec3(0.0, 0.0, z) * spacing, vec3(0.0, 0.0, 1.0));
        vec2 p = (ro + rd * plA).xy * zoom;
        col += sumonTheDemon(p, i, t * colorPulse) * smoothstep(1.0, 0.0, plA * 1.4);
    }

    gl_FragColor = vec4(col, 1.0);
}
