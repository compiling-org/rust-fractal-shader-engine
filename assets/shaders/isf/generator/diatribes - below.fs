/*{
  "DESCRIPTION": "Fractal with color palette + fractal structure style + background variations.",
  "CATEGORIES": [ "Fractal", "Psychedelic" ],
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "Speed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.5 },
    { "NAME": "Morph", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Pulse", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Shake", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2 },
    { "NAME": "Glitch", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "FractalType", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.0 },
    { "NAME": "StructureStyle", "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 0.0 },
    { "NAME": "CameraOffset", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-2.0, -2.0], "MAX": [2.0, 2.0] },
    { "NAME": "Brightness", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Saturation", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Contrast", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "PaletteIndex", "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 0.0 }
  ]
}*/

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define N normalize

vec3 palette(float t, float index) {
    vec3 p0 = 0.5 + 0.5 * cos(6.2831 * (vec3(0.8, 0.4, 0.2) + t));
    vec3 p1 = vec3(sin(t*3.1), sin(t*2.1+1.0), sin(t*4.1+2.0));
    vec3 p2 = pow(vec3(sin(t*6.0), sin(t*3.0+2.5), sin(t*4.5+1.5)), vec3(2.0));
    vec3 p3 = vec3(cos(t*2.5 + 1.0), cos(t*3.5), cos(t*1.5 + 3.0));
    vec3 p4 = vec3(sin(t*5.0 + 2.0), cos(t*7.0 + 3.0), sin(t*3.0 + 1.0));
    vec3 p5 = vec3(sin(t*9.0), sin(t*6.0 + 1.5), cos(t*4.5 + 2.2));
    vec3 p6 = vec3(sin(t*3.0)*1.2, pow(sin(t*2.0), 2.0), abs(sin(t*5.0)));

    vec3 col = p0;
    col = mix(col, p1, step(1.0, index));
    col = mix(col, p2, step(2.0, index));
    col = mix(col, p3, step(3.0, index));
    col = mix(col, p4, step(4.0, index));
    col = mix(col, p5, step(5.0, index));
    col = mix(col, p6, step(6.0, index));
    return col;
}

float fractal(vec3 p, float F, float morph, float type, float style) {
    float s, w = 2.5, l;
    p *= vec3(2.0 + sin(style), 2.5 + morph + style * 0.5, 2.0 + cos(style));
    p.xy += 0.5 * cos(style);
    for (s = 0.0, w = 2.0; s++ < 9.0; p *= l, w *= l) {
        if (style < 1.0) p = cos(p - 0.5 + type);
        else if (style < 2.0) p = sin(p + type);
        else if (style < 3.0) p = tan(p - type);
        else if (style < 4.0) p = normalize(p) * cos(p + type);
        else p = p * p - sin(p + type);

        l = 2.5 / dot(p, p) - (F)*.05 + 0.09;
    }
    return length(p) / w;
}

float map(vec3 p, float T, float F, float morph, float type, float style) {
    float s = fractal(p, F, morph, type, style);
    float n;
    for (n = 0.2; n < 1.8;
        p -= abs(dot(sin(T*2. + p.rgb * n * 32.), vec3(.004))) / n,
        n *= 1.42);
    float f = .35 - p.y;
    return min(s, f);
}

float AO(vec3 pos, vec3 nor, float T, float F, float morph, float type, float style) {
    float sca = 2.0, occ = 0.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + float(i)*0.5/4.0;
        float dd = map(nor * hr + pos, T, F, morph, type, style);
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

void main() {
    vec2 r = RENDERSIZE;
    vec2 U = gl_FragCoord.xy;
    float t = TIME * Speed;
    float F = tanh(cos(t) * 10.0) * Pulse;
    float style = StructureStyle;
    float paletteID = PaletteIndex;

    vec2 u = rot(sin(t * 0.6) * 0.6) * ((U - r * 0.5) / r.y) * Zoom;

    vec3 e = vec3(0.01, 0.0, 0.0);
    vec3 p = vec3(0.0, 0.0, t), ro = p;
    vec3 Z = N(vec3(0.0, 0.0, t+3.0) - (p - vec3(0,0,3.3 + t)));
    vec3 X = N(vec3(Z.z, 0.0, -Z.x));
    vec3 D = vec3(u, 1.0) * mat3(-X, cross(X, Z), Z);
    ro += vec3(CameraOffset, 0.0);

    float d = 0.0, s = 0.002, i = 0.0;
    while (i++ < 120.0 && s > 0.001) {
        p = ro + D * d;
        d += (s = map(p, t, F, Morph, FractalType, style) * 0.8);
    }

    vec3 n = N(vec3(
        map(p - e.xyy, t, F, Morph, FractalType, style),
        map(p - e.yxy, t, F, Morph, FractalType, style),
        map(p - e.yyx, t, F, Morph, FractalType, style)
    ) - map(p, t, F, Morph, FractalType, style));

    vec3 baseColor = palette(length(p)*0.3 + t*0.2, paletteID);
    vec3 bgColor = palette(t * 0.5 + dot(p.xy, vec2(0.3)), 6.0 - paletteID);

    float ao = AO(p, n, t, F, Morph, FractalType, style);
    float lighting = max(dot(n, normalize(ro - p)), 0.075);
    float flicker = abs(1.0 / dot(cos(2.0 * t + p * F * 2.5), vec3(0.5 + F)));

    vec3 col = baseColor;
    col *= lighting;
    col *= ao;
    col *= flicker;

    col += sin(vec3(12.3, 43.2, 21.1) * p * Glitch + t * 10.0) * Shake;

    float fogDist = 0.1 + sin(TIME * 0.1);
    float fogAmt = 0.35 + sin(TIME * 0.1) * 0.1;
    float fogCol = 0.6 + sin(TIME * 0.3) * 0.1;
    float f = d - fogDist;
    if (f > 0.0) {
        f = min(1.0, f * fogAmt);
        col = mix(col, bgColor * (0.2 + f * fogCol), f);
    }

    col = (col - 0.5) * Contrast + 0.5;
    col = mix(vec3(dot(col, vec3(0.299, 0.587, 0.114))), col, Saturation);
    col *= Brightness;

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
