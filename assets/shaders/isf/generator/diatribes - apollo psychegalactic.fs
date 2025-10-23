/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Apollonian fractal with camera and advanced controls",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "paletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZRot", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "fractalIterations", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "fractalScale", "TYPE": "float", "DEFAULT": 2.1, "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "fractalOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "warpIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

float fractal(vec3 p, float iter, float scale, float offset) {
    float w = 2.0;
    for (float l, i = 0.0; i < iter; i++, p *= l, w *= l) {
        p = cos(p - offset);
        l = scale / dot(p, p);
    }
    return length(p) / w;
}

vec3 getPalette(float t, float idx) {
    if (idx < 0.5)
        return vec3(0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.67))));
    else if (idx < 1.5)
        return vec3(sin(t * 3.0 + vec3(0.0, 2.0, 4.0)) * 0.5 + 0.5);
    else if (idx < 2.5)
        return vec3(cos(t * 5.0 + vec3(2.0, 0.0, 1.0)) * 0.5 + 0.5);
    else if (idx < 3.5)
        return vec3(0.5 + 0.5 * sin(vec3(1.0, 2.0, 3.0) + t * 2.5));
    else if (idx < 4.5)
        return vec3(mod(t * vec3(3.0, 7.0, 13.0), 1.0));
    else if (idx < 5.5)
        return vec3(pow(sin(t + vec3(0.0, 1.0, 2.0)), vec3(2.0)));
    else
        return vec3(fract(t * vec3(1.3, 2.1, 3.7)));
}

vec3 applyPost(vec3 color, float b, float s, float c) {
    color = mix(vec3(dot(color, vec3(0.2126, 0.7152, 0.0722))), color, s);
    color = (color - 0.5) * c + 0.5;
    return color * b;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float t = TIME * speed;

    // camera + zoom
    uv *= zoom;
    uv += vec2(camX, camY);

    // apply global shake (temporal offset)
    vec2 globalShake = shake * 0.01 * vec2(
        sin(t * 20.0 + 1.0),
        cos(t * 17.0 + 3.5)
    );
    uv += globalShake;

    float d = 0.2 * fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
    vec4 o = vec4(0.0);
    float s = 0.0, n = 0.0;

    mat2 camRot = mat2(cos(camZRot), -sin(camZRot), sin(camZRot), cos(camZRot));

    for (float i = 0.0; i < 100.0; i++) {
        vec3 p = vec3(uv * d, d + t);

        p.xy *= camRot;

        float angle = 0.02 * t + p.z * 0.2;
        p.xy *= mat2(cos(angle), -sin(angle), sin(angle), cos(angle));

        s = sin(2.0 + p.y + p.x) * warpIntensity;

        for (n = 6.0; n < 32.0; n *= 1.41)
            s += abs(dot(cos(p * n), vec3(0.3))) / n;

        float f = fractal(p, fractalIterations, fractalScale, fractalOffset);
        s = abs(min(f, s)) * 0.7 + 0.001;

        vec3 col = getPalette(p.z * 0.3 + colorPulse * t, paletteSelect);
        o.rgb += col / s;

        d += s;

        uv += glitch * sin(vec2(p.y, p.x) * 30.0 + t * 10.0) * 0.002;
    }

    o.rgb = tanh(o.rgb * o.rgb / 6e8);
    o.rgb = applyPost(o.rgb, brightness, saturation, contrast);
    gl_FragColor = vec4(o.rgb, 1.0);
}
