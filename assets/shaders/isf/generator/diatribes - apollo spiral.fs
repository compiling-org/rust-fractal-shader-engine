/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Apollonian fractal w/ psychedelic palettes + advanced controls",
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
    { "NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "fractalScale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": -2.0, "MAX": 2.0 }
  ]
}*/

float fractal(vec3 p, float iter, float scale, float offset) {
    float w = 4.0;
    for (float l, i = 0.0; i < iter; i++, p *= l, w *= l) {
        p = cos(p - offset);
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

    // global shake offset
    u += shake * 0.01 * vec2(
        sin(t * 20.0 + 1.0),
        cos(t * 17.0 + 3.5)
    );

    float d = 0.0, s = 0.0, n = 0.0;
    vec4 o = vec4(0.0);
    mat2 rotZ = mat2(cos(camZRot), -sin(camZRot), sin(camZRot), cos(camZRot));

    for (float i = 0.0; i < 100.0; i++) {
        vec3 p = vec3(u * d, d + t + t);

        p.xy *= rotZ;

        float angle = 0.05 * t + p.z * 0.2;
        p.xy *= mat2(cos(angle), -sin(angle), sin(angle), cos(angle));

        s = sin(4.0 + p.y + p.x);

        for (n = 5.0; n < 16.0; n += n)
            s -= abs(dot(cos(p * n), vec3(1.0))) / n;

        float f = fractal(p, fractalIterations, fractalScale, fractalOffset);
        s = 0.002 + abs(min(f, s)) * 0.5;

        vec2 g = glitch * 0.002 * sin(vec2(p.y, p.x) * 30.0 + t * 10.0);
        u += g;

        o += vec4(1.0 / s);
        d += s;
    }

    o /= 2e5 * length(u);
    o = palette(o, length(u) + colorPulse * t, paletteSelect);
    o = tanh(mix(o, o.yzxw, length(u)));
    o.rgb = applyPost(o.rgb, brightness, saturation, contrast);
    gl_FragColor = vec4(o.rgb, 1.0);
}
