/*
{
  "DESCRIPTION": "Twisting morphing spheres with reflections and tunable psychedelic color palettes.",
  "CATEGORIES": ["Fractal", "Psychedelic", "3D"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom" },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Morph Strength" },
    { "NAME": "geometryTwist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Geometry Twist" },
    { "NAME": "reflectionWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Reflection Warp" },
    { "NAME": "pulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Color Pulse Speed" },
    { "NAME": "cameraX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Camera X Offset" },
    { "NAME": "cameraY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Camera Y Offset" },
    { "NAME": "colorPalette", "TYPE": "float", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette", "VALUES": ["Classic", "Acid", "Sunset", "Ice"] }
  ]
}
*/

#define PI 3.14159265359
float det = 0.01, sph;
vec3 pos;

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

vec3 palette(float t, int mode) {
    if (mode == 1) return vec3(0.5 + 0.5 * sin(PI * t + vec3(0.0, 2.0, 4.0)));             // Acid
    else if (mode == 2) return vec3(1.0, 0.5, 0.2) * abs(sin(t * 3.1415));                 // Sunset
    else if (mode == 3) return vec3(0.3, 0.5, 1.2) * abs(cos(t * 3.0));                    // Ice
    return vec3(0.5 + 0.5 * cos(2.0 * PI * t + vec3(0.0, 0.5, 1.0)));                      // Classic
}

float de(vec3 p, float time) {
    float s = sin(time * 2.0);
    p.xz *= rot(time - p.y * 0.4 * geometryTwist);
    p.xy *= rot(time - p.z * 0.2 * geometryTwist);

    sph = length(p) - 1.0 - length(sin(p * 3.0)) * 0.2 * morph;
    sph -= s * s * 0.5;

    pos = p;
    float d = length(p) - 2.0;
    d = max(d, -length(p.xy) + 3.0);
    d = max(d, -length(p.xz) + 3.0);
    d = max(d, -length(p.yz) + 3.0);
    d -= length(sin(p * 3.0)) * 0.9 * morph;
    return min(d, sph) * 0.25;
}

vec3 normal(vec3 p, float time) {
    vec2 e = vec2(0.0, det);
    return normalize(vec3(
        de(p + e.yxx, time),
        de(p + e.xyx, time),
        de(p + e.xxy, time)) - de(p, time));
}

vec3 march(vec3 from, vec3 dir, float time) {
    float td = 0.0, d = 0.0, maxdist = 30.0, g = 0.0, refCount = 0.0;
    vec3 p = from, col = vec3(0.0);

    for (int i = 0; i < 100; i++) {
        p += dir * d;
        d = de(p, time);
        if (td > maxdist) break;

        if (d < det && refCount < 1.0) {
            refCount += 1.0;
            vec3 n = normal(p, time);
            dir = reflect(dir, n);
            p += det * dir;
        }
        td += d;
        g = max(g, 0.15 / (0.1 + sph * 0.5));
    }

    if (d < 0.1) {
        vec3 ldir = normalize(-p);
        vec3 n = normal(p, time);
        float amb = 0.3;
        float dif = max(0.0, dot(ldir, n)) * 0.5;
        vec3 ref = reflect(dir, n);
        float spe = pow(max(0.0, dot(ldir, ref)), 10.0) * 0.5;
        float tint = length(fract(pos) - 0.5);
        col = palette(tint + time * pulseSpeed, int(colorPalette)) * (amb + dif) + spe;
    } else {
        p = (refCount == 1.0 ? dir : maxdist * dir);
        p *= length(p.xy) * 0.015 * reflectionWarp;
        p += vec3(0.3, 0.2, 0.1);
        for (int i = 0; i < 10; i++) {
            p = abs(p) / dot(p, p) - 0.78;
        }
        col += dot(p, p) * 0.005 * p;
    }

    return col + g * vec3(1.5, 0.7, 0.5);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
    vec3 dir = normalize(vec3(uv, 1.0));

    float t = TIME;
    dir.xy *= rot(sin(t * 0.5) * 0.2);

    vec3 from = vec3(sin(t) * 2.0 + cameraX, cameraY, -10.0 + cos(t * 0.5) * 3.0) / zoom;
    vec3 col = march(from, dir, t);

    gl_FragColor = vec4(col, 1.0);
}
