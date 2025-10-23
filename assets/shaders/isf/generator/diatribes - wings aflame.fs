/*{
  "CATEGORIES": ["Generator", "Fractal"],
  "DESCRIPTION": "True Psychedelic Tunnel - Color Fixed",
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "zoom",       "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast",   "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "shake",      "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch",     "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalType","TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "palette",    "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "camRoll",    "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 }
  ]
}*/

vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6. + vec3(0., 4., 2.), 6.) - 3.) - 1., 0., 1.);
    return c.z * mix(vec3(1.), rgb, c.y);
}

vec3 paletteColor(float t, float p) {
    float h = mod(t, 1.0);
    vec3 col;

    if (p < 1.0)
        col = vec3(0.1, 0.9, 0.3) + vec3(0.9, -0.5, 0.7) * sin(h * 6.28);
    else if (p < 2.0)
        col = vec3(0.7, 0.3, 0.8) + vec3(0.5, 0.4, -0.7) * cos(h * 7.0);
    else if (p < 3.0)
        col = mix(vec3(0.2, 0.8, 0.9), vec3(0.9, 0.2, 0.6), sin(h * 5.0) * 0.5 + 0.5);
    else if (p < 4.0)
        col = vec3(sin(h * 10.0 + 1.0), cos(h * 8.0 + 3.0), sin(h * 6.0));
    else if (p < 5.0)
        col = vec3(fract(sin(h * 100.0 + 1.2) * 43.45),
                   fract(sin(h * 170.0 + 3.0) * 73.44),
                   fract(sin(h * 210.0 + 5.7) * 91.23));
    else if (p < 6.0)
        col = hsv2rgb(vec3(h * 2.0, 1.0 - abs(sin(h * 3.0)), 0.8));
    else
        col = hsv2rgb(vec3(h, 0.7, 0.9));

    return clamp(col, 0.0, 0.95);
}

void main() {
    vec2 u = gl_FragCoord.xy;
    vec2 res = RENDERSIZE;
    float t = TIME * speed;
    float hueShift = mod(colorPulse + TIME * 0.05, 1.0);

    vec3 o = vec3(0.0);
    float d = 0.0, s = 0.0;

    vec2 uv = (u - res * 0.5) / res.y;
    uv *= zoom;
    uv += shake * vec2(sin(t * 18.0), cos(t * 21.0)) * 0.01;

    mat2 roll = mat2(cos(camRoll), -sin(camRoll), sin(camRoll), cos(camRoll));
    uv *= roll;

    for (int i = 0; i < 100; i++) {
        vec3 p = d * normalize(vec3(uv * 2.0, 0.0) - vec3(0.0, 0.0, -1.0)) * 0.8;
        p.z -= t;

        for (s = 0.1; s < 2.0; s *= 1.4) {
            if (fractalType < 1.0)
                p -= dot(cos(t + p * s * 16.0), vec3(0.02)) / s;
            else if (fractalType < 2.0)
                p -= normalize(sin(p * s * 12.0 + t)) * 0.03;
            else
                p -= sin(p.zyx * (s * 10.0) + t) * 0.01;

            p -= cos(p.yzx * 1.5) * 0.2 * morph;
            p.xy *= mat2(cos(t * 0.1 + vec4(0, 33, 11, 0)));
        }

        float f = abs(2.0 - min(length(p.xy),
                  min(length(p.xy - p.y + cos(t + p.zy * 6.0)),
                      length(p.xy - p.x + cos(t + p.yz * 5.0))))) * 0.14;

        s = 0.0175 + f;
        d += s;

        float tt = mod(d * 0.015 + hueShift, 1.0);
        vec3 col = paletteColor(tt, palette);
        o += (1.0 + col) / s;
    }

    o = tanh(o * vec3(1.4, 0.75, 0.6) / 2000.0);

    if (glitch > 0.0) {
        o.r += sin(t * 50.0 + o.g * 10.0) * 0.1 * glitch;
        o.g += cos(t * 45.0 + o.b * 10.0) * 0.1 * glitch;
        o.b += sin(t * 60.0 + o.r * 10.0) * 0.1 * glitch;
    }

    o = mix(vec3(dot(o, vec3(0.299, 0.587, 0.114))), o, saturation);
    o = (o - 0.5) * contrast + 0.5;
    o *= brightness;
    o = clamp(o, 0.0, 0.95);

    gl_FragColor = vec4(o, 1.0);
}
