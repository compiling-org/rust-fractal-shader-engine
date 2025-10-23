/*{
  "DESCRIPTION": "Psychedelic hex fractal shader with morphing, palette control, camera XY, and full color tuning.",
  "CATEGORIES": [ "Psychedelic", "VJ", "Fractal" ],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "deform", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "pattern", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "tileShape", "TYPE": "long", "DEFAULT": 0, "MIN": 0, "MAX": 2 },
    { "NAME": "palette", "TYPE": "long", "DEFAULT": 2, "MIN": 0, "MAX": 6 },
    { "NAME": "controlXY", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define PI 3.1415926
#define TAU 6.2831853
#define HUE_OFFSET radians(vec3(0.0, 60.0, 120.0))

vec3 hue(float a) {
    return cos(HUE_OFFSET + a * TAU) * 0.5 + 0.5;
}

vec3 getPalette(float t, int mode) {
    t = fract(t);
    if (mode == 0) return hue(t);
    if (mode == 1) return vec3(sin(t * TAU), sin(t * TAU * 1.5), sin(t * TAU * 2.0));
    if (mode == 2) return vec3(cos(t * PI), cos(t * TAU), cos(t * TAU * 1.5));
    if (mode == 3) return vec3(1.0 - t, t, sin(t * PI));
    if (mode == 4) return vec3(fract(t * 5.0), fract(t * 3.0), fract(t * 7.0));
    if (mode == 5) return vec3(0.5 + 0.5 * sin(t * 20.0), 0.5 + 0.5 * cos(t * 15.0), sin(t * 5.0));
    return hue(t + sin(t * 3.14159));
}

vec3 adjustColor(vec3 col, float bri, float sat, float con) {
    col *= bri;
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(lum), col, sat);
    col = mix(vec3(0.5), col, con);
    return clamp(col, 0.0, 1.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float t = TIME * speed * 0.0033;
    float s = zoom;

    vec2 cam = controlXY * vec2(2.0, 1.5) * s;
    if (length(controlXY) < 0.01) {
        cam = s * vec2(sin(t * TAU), sin(t * TAU * 0.5)) * 2.0;
    }

    float r = length(uv);
    float rPulse = sin(TIME * pulse * 2.5) * 0.5 + 0.5;
    vec3 color = vec3(0.0);
    vec3 extra = getPalette(t * 10.0 + r * 0.6, int(palette)) * 0.07;

    for (float i = 0.2; i < 1.01; i += 0.2) {
        vec2 pos = i * uv * s / dot(uv, uv) - cam;

        vec2 tileOffset = vec2(0.5 + deform * 0.3, 0.866 + deform * 0.3);

        vec2 a = mod(pos, tileOffset * 2.0) - tileOffset;
        vec2 b = mod(pos - tileOffset, tileOffset * 2.0) - tileOffset;
        vec2 h = dot(a, a) < dot(b, b) ? a : b;
        vec2 k = abs(h);

        float uLen = length(pos - h);
        float patternMod = mix(uLen, pow(uLen, 2.0), morph);
        float p = patternMod * t * mix(2.0, 8.0, pattern);

        float brightness = 1.0;
        if (tileShape == 0) {
            brightness = sin(p * TAU) * 0.5 + 1.0;
        } else if (tileShape == 1) {
            brightness = fract(p * 2.0);
        } else if (tileShape == 2) {
            brightness = pow(abs(cos(p * TAU)), 2.0);
        }

        float fade = (1.0 - max(k.x, dot(k, vec2(0.5, 0.866))) * 3.0);
        vec3 baseColor = getPalette(p + morph, int(palette)) * (0.5 + rPulse * 0.5);
        float vignette = min(r * sqrt(r), 1.0 / r);
        float falloff = pow(i, 0.1) * (1.2 - r);

        color = max(color, fade * brightness * baseColor * vignette * falloff + extra);
    }

    color += color * color * 6.0;
    color = adjustColor(color, brightness, saturation, contrast);
    gl_FragColor = vec4(color, 1.0);
}
