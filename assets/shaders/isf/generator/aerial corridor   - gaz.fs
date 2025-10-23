/*{
  "CATEGORIES": ["Fractal", "Psychedelic", "Raymarching"],
  "INPUTS": [
    { "NAME": "xyControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "ColorMode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "FractalMorph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "FractalDetail", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "FractalOffsetZ", "TYPE": "float", "DEFAULT": 3.5, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CameraPan", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "CameraTilt", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.14159265359

float Scale;

vec3 getPalette(float t, float mode) {
    if (mode < 0.5) return 0.5 + 0.5 * cos(t + vec3(0, 2, 4));
    if (mode < 1.5) return abs(sin(t * vec3(1.5, 0.9, 1.7)));
    if (mode < 2.5) return 0.5 + 0.5 * sin(vec3(t, t * 1.3, t * 1.7));
    if (mode < 3.5) return normalize(vec3(sin(t * 0.9), cos(t * 1.2), sin(t * 1.1)));
    if (mode < 4.5) return vec3(sin(t * 1.5), sin(t * 1.0), cos(t * 2.0));
    if (mode < 5.5) return 0.5 + 0.5 * cos(t + vec3(3, 2, 1));
    return vec3(fract(sin(t * vec3(12.9898, 78.233, 37.719)) * 43758.5453));
}

float map(vec3 p, float morph, float detail, float offsetZ) {
    p = mod(p, 2.0) - 1.0;
    p = abs(p) - 1.0;
    if (p.x < p.z) p.xz = p.zx;
    if (p.y < p.z) p.yz = p.zy;
    if (p.x < p.y) p.xy = p.yx;
    float s = 1.0;
    for (int i = 0; i < 20; i++) {
        if (float(i) > detail) break;
        float r2 = morph / clamp(dot(p, p), 0.1, 1.0);
        p = abs(p) * r2 - vec3(0.6, 0.6, offsetZ);
        s *= r2;
    }
    Scale = log2(s);
    return length(p) / s;
}

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

void main() {
    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 m = xyControl;

    vec3 ro = vec3(CameraPan + 0.5 * sin(TIME * 0.03), CameraTilt + 0.05 * cos(TIME * 0.03), -0.1 * TIME * Speed);
    vec3 target = vec3(0.0, 0.0, ro.z - 1.0);
    vec3 forward = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0, 1, 0), forward));
    vec3 up = cross(forward, right);

    // Apply camera roll
    mat2 rollRot = rot(CameraRoll);
    right = vec3(rollRot * right.xy, right.z);
    up = vec3(rollRot * up.xy, up.z);

    vec3 rd = normalize(uv.x * right + uv.y * up + Zoom * forward);

    float t = 0.4;
    float d;
    float i;
    vec3 p;

    for (i = 1.0; i < 100.0; i++) {
        p = ro + rd * t;
        d = map(p, FractalMorph, FractalDetail, FractalOffsetZ);
        if (d < 0.001 || t > 10.0) break;
        t += d;
    }

    vec3 color = getPalette(Scale * ColorPulse + dot(p, vec3(1.8)), ColorMode);
    vec3 finalColor = mix(vec3(1.0), color, 0.5) * 10.0 / i;

    if (i < 5.0) finalColor = vec3(0.5, 0.2, 0.1) * (5.0 - i);

    finalColor = pow(finalColor, vec3(0.4545));
    finalColor = mix(vec3(0.5), finalColor, Contrast);
    finalColor = mix(vec3(dot(finalColor, vec3(0.299, 0.587, 0.114))), finalColor, Saturation);
    finalColor *= Brightness;

    gl_FragColor = vec4(finalColor, 1.0);
}
