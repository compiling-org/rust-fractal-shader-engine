/*
{
  "DESCRIPTION": "Psychedelic 3D tunnel fractal with pulse, palette, morph, and geometry controls.",
  "CATEGORIES": ["Psychedelic", "Fractal", "3D", "Tunnel"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Animation Speed" },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0, "LABEL": "Zoom Level" },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Morph Strength" },
    { "NAME": "twist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Twist Intensity" },
    { "NAME": "pulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Color Pulse Speed" },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Brightness" },
    {
      "NAME": "colorPalette",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Color Palette",
      "VALUES": ["Classic", "Acid", "Sunset", "Aurora"]
    }
  ]
}
*/

#define PI 3.14159265359
vec3 pos;
float it;
float sph;

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

vec3 palette(float t, int mode) {
    if (mode == 1) return 0.5 + 0.5 * sin(PI * t + vec3(0.0, 2.0, 4.0));               // Acid
    else if (mode == 2) return vec3(1.2, 0.6, 0.1) * sin(2.0 * PI * t);                // Sunset
    else if (mode == 3) return vec3(0.2, 0.7, 1.0) * cos(3.0 * PI * t);                // Aurora
    return 0.5 + 0.5 * cos(2.0 * PI * t + vec3(0.0, 0.5, 1.0));                        // Classic
}

vec3 path(float t) {
    vec3 p = vec3(sin(t * 0.5 + cos(t * 0.2)) * 3.0, cos(t), t);
    p.y += smoothstep(-2.0, 2.0, sin(t * 0.5)) * 2.0;
    return p;
}

float de(vec3 p, float t) {
    vec3 p2 = p - pos;
    p.xy -= path(p.z).xy;
    p.xy *= rot(t * twist);
    float d = 1000.0;
    float tun = length(p.xy) - 1.0;
    sph = (length(p2) - 0.5) - length(sin(p * 5.0)) * morph * 0.3;

    float sc = 1.3;
    p *= 0.3;
    for (int i = 0; i < 6; i++) {
        p = sin(p * 2.0);
        p.xz *= rot(1.0);
        p.xy *= rot(1.5);
        float l = length(p.xy) - 0.1;
        d = min(d, l);
        if (d == l) it = float(i);
    }

    d = max(d, -tun + 2.0);
    d = min(d, tun);
    d = max(d, -sph);
    return d * 0.3;
}

vec3 normal(vec3 p, float t) {
    vec2 e = vec2(0.0, 0.01);
    return normalize(vec3(
        de(p + e.yxx, t),
        de(p + e.xyx, t),
        de(p + e.xxy, t)) - de(p, t));
}

vec3 march(vec3 from, vec3 dir, float t) {
    float td = 0.0, d;
    vec3 p, col = vec3(0.0);
    vec3 ldir = vec3(0.0, 1.0, 0.0);
    bool inside = false;

    for (int i = 0; i < 200; i++) {
        p = from + dir * td;
        d = de(p, t);
        if (d < 0.01 && !inside) {
            inside = true;
            vec3 n = normal(p, t);
            vec3 ref = reflect(ldir, n);
            col += pow(max(0.0, dot(ref, dir)), 20.0) * 0.05;
        } else inside = false;

        d = max(0.003, abs(d));
        if (td > 100.0) break;
        td += d;

        vec3 pulseColor = palette(length(p.xy) + t * pulseSpeed + it * 0.2, int(colorPalette));
        vec3 c = 0.1 / (0.1 + d * 50.0) * pow(fract(-p.z * 0.2 + length(p.xy) * 0.2 - t * 0.5), 1.5) * 0.1 * pulseColor;
        c *= exp(-0.25 * td) * 3.0;
        c = abs(c);

        if (sph > 0.02) col += c;
        else col += 0.007;
    }

    col = mix(length(col) * vec3(0.5), col, 0.5);
    return col * brightness;
}

mat3 lookat(vec3 dir) {
    vec3 up = vec3(0.0, 1.0, 0.0);
    vec3 rt = normalize(cross(dir, up));
    return mat3(rt, cross(dir, rt), dir);
}

void main() {
    float t = TIME * speed;
    vec3 from = path(t);
    from.x += smoothstep(0.0, 0.8, sin(TIME * 0.5)) * 3.0;
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    vec3 adv = path(t + 1.0);
    pos = path(t + 2.0);
    vec3 dir = normalize(vec3(uv, 0.5));
    dir = lookat(normalize(adv - from)) * dir;

    vec3 col = march(from / zoom, dir, t);
    gl_FragColor = vec4(col, 1.0);
}
