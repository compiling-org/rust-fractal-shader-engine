/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Apollonian fractal w/ tunnel, shimmer, psychedelic controls",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZRot", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "fractalScale", "TYPE": "float", "DEFAULT": 3.1, "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 1.5, "MIN": -3.0, "MAX": 3.0 }
  ]
}*/

float fractal(vec3 p, float iter, float scale, float offset) {
    float w = 1.5;
    p -= offset;
    for (float l, i = 0.0; i < iter; i++, p *= l, w *= l) {
        p = sin(p);
        l = scale / dot(p, p);
    }
    return length(p) / w;
}

vec4 palette(vec4 col, float depth, float idx) {
    if (idx < 0.5)
        col = pow(col, vec4(1, 2, 12, 0)) * 6.0;
    else if (idx < 1.5)
        col.rgb = sin(depth * 2.0 + vec3(0.0, 2.0, 4.0)) * col.rgb;
    else if (idx < 2.5)
        col.rgb = cos(depth * 3.0 + vec3(4.0, 2.0, 0.0)) * col.rgb;
    else if (idx < 3.5)
        col.rgb = pow(abs(sin(col.rgb + depth)), vec3(2.0));
    else if (idx < 4.5)
        col.rgb *= vec3(fract(depth * 3.3), fract(depth * 2.7), fract(depth * 4.1));
    else if (idx < 5.5)
        col.rgb = normalize(col.rgb + sin(depth + vec3(1.0, 2.0, 3.0)));
    else
        col.rgb = mix(col.rgb, col.bgr, sin(depth * 3.0));
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
    float t = TIME * speed;

    u = (u - r * 0.5) / r.y;
    u *= zoom;
    u += vec2(camX, camY);

    // global shake
    u += shake * 0.01 * vec2(sin(t * 17.0), cos(t * 13.0));

    float d = 0.2 * fract(sin(dot(u, vec2(12.9898, 78.233))) * 43758.5453);
    float i = 0.0, f = 0.0, s = 0.0, n = 0.0;
    vec4 o = vec4(0.0);
    bool hit = false;

    mat2 camRot = mat2(cos(camZRot), -sin(camZRot), sin(camZRot), cos(camZRot));

    for (i = 0.0; i < 100.0; i++) {
        vec3 p = vec3(u * d, d + t);

        p.xy *= camRot;

        float angle = 0.02 * t + p.z * 0.2;
        p.xy *= mat2(cos(angle), -sin(angle), sin(angle), cos(angle));

        s = max(1.5 - length(p.xy), sin(p.x + p.y));

        for (n = 16.0; n < 64.0; n *= 1.41)
            s += abs(dot(cos(p * n), vec3(1.0))) / n;

        f = fractal(p, fractalIterations, fractalScale, fractalOffset);
        float stepDist = 0.001 + abs(min(f, s)) * 0.8;

        hit = (f <= s);
        o += vec4(1.0 / stepDist);
        d += stepDist;

        // shimmer
        u += glitch * 0.0015 * sin(vec2(p.y, p.x) * 30.0 + t * 10.0);
    }

    float falloff = pow((0.5 + 0.1 * cos(t)) * dot(u, u), 0.6);
    o = tanh(o / 2e5 / falloff);
    o = palette(o, length(u) + colorPulse * t, paletteSelect);
    o.rgb = applyPost(o.rgb, brightness, saturation, contrast);
    gl_FragColor = vec4(o.rgb, 1.0);
}
