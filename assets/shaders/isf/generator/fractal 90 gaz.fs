/*{
  "CATEGORIES": ["Fractal", "Psychedelic", "3D"],
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "PulseRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "PulseStrength", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FractalType", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "ShakeAmount", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "CameraAngle", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] },
    { "NAME": "CameraPos", "TYPE": "point2D", "DEFAULT": [0.0, 0.0] }
  ]
}*/

#define R(p,a,r) mix(dot(p,a)*a,p,cos(r)) + sin(r)*cross(p,a)

vec3 palette(float t, float type) {
    if (type < 1.5) return vec3(sin(t*6.0), cos(t*3.0), sin(t*2.0)) * 0.5 + 0.5;
    if (type < 2.5) return vec3(fract(t*2.3), fract(t*5.7), fract(t*9.1));
    if (type < 3.5) return vec3(abs(sin(t*3.1)), abs(sin(t*2.1+1.0)), abs(sin(t*4.1+2.0)));
    if (type < 4.5) return vec3(cos(t*10.0), cos(t*5.0), sin(t*7.0)) * 0.5 + 0.5;
    if (type < 5.5) return vec3(sin(t*5.0), sin(t*7.0+1.0), sin(t*9.0+2.0)) * 0.5 + 0.5;
    return vec3(sin(t*4.0 + sin(t*3.0)), cos(t*5.0 + sin(t*2.0)), sin(t*6.0 + cos(t*1.0))) * 0.5 + 0.5;
}

vec3 adjust(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con);
    color = mix(vec3(dot(color, vec3(0.299, 0.587, 0.114))), color, sat);
    return color * br;
}

float glitchNoise(vec2 uv) {
    uv *= vec2(40.0, 4.0);
    return fract(sin(dot(uv, vec2(12.9898,78.233))) * 43758.5453);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= 2.0 / Zoom;

    float t = TIME * Speed;

    float shakeX = sin(t * 13.0) * ShakeAmount;
    float shakeY = cos(t * 17.0) * ShakeAmount;

    vec3 camPos = vec3(CameraPos.x * 5.0 + shakeX, CameraPos.y * 5.0 + shakeY, -4.0 + Morph * 4.0);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - camPos);
    vec3 right = normalize(cross(vec3(0.0,1.0,0.0), fwd));
    vec3 up = cross(fwd, right);

    vec3 d = normalize(fwd + uv.x * right + uv.y * up);

    vec3 col = vec3(0.0);
    float g = 0.0, s, e;

    for (int i = 0; i < 99; i++) {
        vec3 p = camPos + g * d;
        p = R(p, vec3(0.577), t * 0.1 + Morph * 6.28);
        p = sin(p * (1.0 + Morph * 2.0));

        s = 2.0;
        for (int j = 0; j < 5; j++) {
            s *= e = 2.0 / min(dot(p, p), 1.0);
            p = abs(p) * e - vec3(3.0, 20.0, 9.0 + Morph * 4.0);
        }

        g += e = abs(length(p.xy - clamp(p.xy, -0.5, 0.5)) / s) + 0.005;

        float pulse = sin(float(i) * PulseRate + t) * PulseStrength;
        col += mix(vec3(1.0), palette(log(s) * 0.5 + pulse, FractalType), 0.3)
             * 12e-5 * exp(sin(float(i))) / e;
    }

    col = pow(col, vec3(4.0));
    col = adjust(col, Brightness, Saturation, Contrast);

    if (Glitch > 0.0) {
        float n = glitchNoise(gl_FragCoord.xy) * Glitch;
        col += n * vec3(1.0, 0.2, 0.0);
    }

    gl_FragColor = vec4(col, 1.0);
}
