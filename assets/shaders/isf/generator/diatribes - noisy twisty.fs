/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Sound reactive tunnel w/ mirrored noise and psychedelic palette",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "noiseTex",    "TYPE": "image" },
    { "NAME": "zoom",        "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed",       "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "colorPulse",  "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "paletteSelect","TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "shake",       "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch",      "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brightness",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "camX",        "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY",        "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZRot",     "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 }
  ]
}*/

#define N(x,s) abs(dot(cos(x * n), vec3(s))) / n
#define R(a) mat2(cos(a+vec4(0,33,11,0)))

vec4 palette(vec4 col, float depth, float idx) {
    if (idx < 0.5) col = pow(col, vec4(1, 2, 12, 0)) * 6.0;
    else if (idx < 1.5) col.rgb = sin(depth * 2.0 + vec3(0.0, 2.0, 4.0)) * col.rgb;
    else if (idx < 2.5) col.rgb = cos(depth * 3.0 + vec3(4.0, 2.0, 0.0)) * col.rgb;
    else if (idx < 3.5) col.rgb = pow(abs(sin(col.rgb + depth)), vec3(2.0));
    else if (idx < 4.5) col.rgb *= vec3(fract(depth * 3.3), fract(depth * 2.7), fract(depth * 4.1));
    else if (idx < 5.5) col.rgb = normalize(col.rgb + sin(depth + vec3(1.0, 2.0, 3.0)));
    else col.rgb = mix(col.rgb, col.bgr, sin(depth * 3.0));
    return col;
}

vec3 applyPost(vec3 color, float b, float s, float c) {
    color = mix(vec3(dot(color, vec3(0.2126, 0.7152, 0.0722))), color, s);
    color = (color - 0.5) * c + 0.5;
    return color * b;
}

void main() {
    vec2 u = isf_FragNormCoord * RENDERSIZE;
    vec2 r = RENDERSIZE;
    vec2 uv = (u - r * 0.5) / r.y;

    uv *= zoom;
    uv += vec2(camX, camY);
    uv += shake * 0.01 * vec2(sin(TIME * 15.0), cos(TIME * 19.0));

    float d = 0.2 * texture(noiseTex, mod(u / 1024.0, 1.0)).a;

    float tbase = TIME * 1.9;
    float ft = fract(tbase);
    float t = floor(tbase) + sqrt(ft);

    vec4 o = vec4(0.0);
    float s = 0.002, n, i = 0.0;

    mat2 camRot = mat2(cos(camZRot), -sin(camZRot), sin(camZRot), cos(camZRot));
    uv *= camRot;

    vec3 w, p;

    for (; i++ < 64.0 && s > 0.001;
         d += s = 0.05 + 0.8 * abs(min(s, 4.0 - abs(w.x))),
         o += (1.0 + cos(fract(t * 0.5) + 0.1 * w.z + p.z * 0.1 + vec4(3, 1, 0, 0))) / s / d)
    {
        p = w = vec3(uv * d, d);
        w.xy *= R(t + w.z * 0.2);
        p.xy *= R(t + p.z * 0.5);

        s = 5.0 - length(p.xy);
        for (n = 1.5; n < 16.0;
             s += N(3.0 * t + p, 0.3),
             w += N(6.0 * t + w, 0.2),
             n += n) {}

        uv += glitch * 0.002 * sin(vec2(p.y, p.x) * 30.0 + t * 10.0);
    }

    float chaos = length(uv);
    float tone = 60.0 * chaos * pow(ft, 0.125) * length(cos(4.0 / ft * t * uv / log((1.0 + cos(t) * 16.0 + 32.0) / d)));
    vec4 raw = o / tone;

    raw = (exp(2.0 * raw) - 1.0) / (exp(2.0 * raw) + 1.0); // tanh

    raw = palette(raw, chaos + colorPulse * t, paletteSelect);
    raw.rgb = applyPost(raw.rgb, brightness, saturation, contrast);

    gl_FragColor = vec4(raw.rgb, 1.0);
}
