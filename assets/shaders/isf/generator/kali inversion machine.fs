/*{
  "DESCRIPTION": "Inversion Machine fractal with dynamic geometry and vibrant palettes.",
  "CATEGORIES": ["Fractal", "Psychedelic"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "fractalWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "geoMorph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    {
      "NAME": "paletteIndex",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 6.0,
      "LABEL": "Color Palette",
      "VALUES": ["Classic", "Neon", "Rainbow", "Electric", "BW", "PulseWave", "UltraRGB"]
    }
  ]
}*/

#define DETAIL 0.002
#define SCALE 4.0
#define WIDTH 0.22

mat2 rot;
vec3 lightdir = -vec3(0.2, 0.5, 1.0);

float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 palette(float t, int mode) {
    if (mode == 1) return vec3(0.5 + 0.5 * sin(t * 6.28 + vec3(0.0, 1.0, 2.0))); // Neon
    if (mode == 2) return vec3(sin(t * 10.0 + vec3(1, 3, 5)) * 0.5 + 0.5); // Rainbow
    if (mode == 3) return vec3(0.7 + 0.3 * sin(t * 8.0 + vec3(0.5, 1.5, 3.0))); // Electric
    if (mode == 4) return vec3(t); // BW grayscale
    if (mode == 5) return vec3(0.7 + 0.3 * cos(t * 12.0 + vec3(2.0, 4.0, 6.0))); // PulseWave
    if (mode == 6) return vec3(0.5 + 0.5 * sin(t * vec3(9.0, 7.0, 5.0))); // UltraRGB
    return vec3(0.5 + 0.5 * cos(6.283 * t + vec3(0.0, 0.33, 0.67))); // Classic
}

float de(vec3 p, float t) {
    float dotp = dot(p, p);
    p.x += sin(t * 40.0) * 0.007;
    p = p / dotp * SCALE;

    // Apply fractalWarp and geoMorph
    vec3 offset = vec3(sin(1.0 + t * geoMorph) * 2.0, -t, -t * 2.0);
    p = sin(p + offset * fractalWarp);

    float d = length(p.yz) - WIDTH;
    d = min(d, length(p.xz) - WIDTH);
    d = min(d, length(p.xy) - WIDTH);
    d = min(d, length(p * p * p) - WIDTH * 0.3 * geoMorph);
    return d * dotp / SCALE;
}

vec3 normal(vec3 p, float t) {
    vec3 e = vec3(0.0, DETAIL, 0.0);
    return normalize(vec3(
        de(p + e.yxx, t) - de(p - e.yxx, t),
        de(p + e.xyx, t) - de(p - e.xyx, t),
        de(p + e.xxy, t) - de(p - e.xxy, t)
    ));
}

float lighting(vec3 p, vec3 dir, float t) {
    vec3 ldir = normalize(lightdir);
    vec3 n = normal(p, t);
    float diff = max(0.0, dot(ldir, -n)) + 0.1 * max(0.0, dot(normalize(dir), -n));
    vec3 r = reflect(ldir, n);
    float spec = pow(max(0.0, dot(dir, -r)), 20.0) * 0.7;
    return diff + spec;
}

float raymarch(vec3 from, vec3 dir, vec2 fragCoord, float t, int paletteMode) {
    vec2 uv = fragCoord.xy / RENDERSIZE.xy * 2.0 - 1.0;
    uv.y *= RENDERSIZE.y / RENDERSIZE.x;
    
    float st = 0.0, d, col = 0.0, totdist = 0.0;
    vec3 p;
    float ra = hash(uv.xy * t) - 0.5;

    for (int i = 0; i < 60; i++) {
        p = from + totdist * dir;
        d = de(p, t);
        if (d < DETAIL || totdist > 2.0) break;
        totdist += d;
        st += max(0.0, 0.04 - d);
    }

    float backg = 0.45 * pow(1.5 - min(1.0, length(uv + vec2(0.0, -0.6))), 1.5);
    if (d < DETAIL) {
        col = lighting(p - DETAIL * dir, dir, t);
    } else {
        col = backg;
    }

    col += smoothstep(0.0, 1.0, st) * 0.8;
    col += pow(max(0.0, 1.0 - length(p)), 8.0) * 0.5;
    col += pow(max(0.0, 1.0 - length(p)), 30.0) * 50.0;
    col = mix(col, backg, 1.0 - exp(-0.25 * pow(totdist, 3.0)));
    col += ra * 0.03;
    
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;
    vec3 from = vec3(0.0, 0.0, -1.5 / zoom);
    vec3 dir = normalize(vec3(uv, 1.0));

    float angle = t * 0.2;
    rot = mat2(cos(angle), sin(angle), -sin(angle), cos(angle));
    dir.xy = rot * dir.xy;

    float shade = raymarch(from, dir, gl_FragCoord.xy, t, int(paletteIndex));
    vec3 col = palette(length(uv) * colorPulse + t * 0.2, int(paletteIndex)) * shade;

    if (int(paletteIndex) == 4) {
        col = vec3(shade);
    }

    gl_FragColor = vec4(col, 1.0);
}
