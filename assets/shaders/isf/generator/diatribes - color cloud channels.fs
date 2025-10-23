/*{
  "CATEGORIES": ["Generator", "Fractal"],
  "DESCRIPTION": "Trippy Fractal Tunnel - No Whites, Balanced RGB",
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "zoom",       "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "shake",      "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch",     "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalType","TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "palette",    "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "camPan",     "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camTilt",    "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camRoll",    "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 }
  ]
}*/

#define P(z) vec3(cos(vec2(0.15,0.2)*(z))*5.0,z)

mat3 cameraMatrix(vec3 Z, float pan, float tilt, float roll) {
    vec3 X = normalize(vec3(Z.z, 0.0, -Z.x));
    vec3 Y = cross(Z, X);
    mat3 rot = mat3(X, Y, Z);

    float cp = cos(pan), sp = sin(pan);
    float ct = cos(tilt), st = sin(tilt);
    float cr = cos(roll), sr = sin(roll);

    mat3 panMat = mat3(1,0,0, 0,cp,-sp, 0,sp,cp);
    mat3 tiltMat = mat3(ct,0,st, 0,1,0, -st,0,ct);
    mat3 rollMat = mat3(cr,-sr,0, sr,cr,0, 0,0,1);

    return rot * rollMat * tiltMat * panMat;
}

vec3 paletteColor(float t, float p) {
    t = fract(t);
    vec3 c;
    if (p < 1.0)
        c = vec3(sin(t*6.0), sin(t*5.3 + 1.5), sin(t*4.7 + 2.3));
    else if (p < 2.0)
        c = vec3(cos(t*8.1 + 1.3), sin(t*6.2), cos(t*7.5));
    else if (p < 3.0)
        c = vec3(sin(t*10.0), cos(t*9.0 + 2.1), sin(t*8.0 + 0.7));
    else if (p < 4.0)
        c = vec3(sin(t*7.1 + 3.1), sin(t*6.7 + 1.2), cos(t*6.3 + 2.6));
    else if (p < 5.0)
        c = vec3(cos(t*5.5 + 0.2), sin(t*9.9 + 2.2), sin(t*8.8));
    else
        c = vec3(cos(t*13.0 + 1.0), cos(t*11.0), sin(t*12.0 + 3.0));

    // Clamp and bias to avoid white, oversaturation
    c = clamp(c * 0.5 + 0.5, 0.05, 0.95);
    return c;
}

void main() {
    vec2 u = gl_FragCoord.xy;
    vec2 res = RENDERSIZE;
    float t = TIME * speed;

    vec3 color = vec3(0.0);
    vec3 p = P(t);
    vec3 Z = normalize(P(t + 1.0) - p);
    mat3 cam = cameraMatrix(Z, camPan, camTilt, camRoll);

    vec2 uv = (u - res * 0.5) / res.y;
    uv *= zoom;
    uv += shake * vec2(sin(t*10.0), cos(t*11.0)) * 0.01;

    vec3 D = normalize(vec3(uv, 1.0)) * cam;

    float acc = 0.0, s = 0.0, n;

    for (int i = 0; i < 100; i++) {
        p += D * s;
        vec3 q = P(p.z) + cos(t + p.yzx) * 0.3 * morph;

        float type = floor(fractalType);
        float dist;
        if (type < 1.0)
            dist = 2.0 - min(length((p - q).xy), min(length(p.xy - q.y), length(p.xy - q.x)));
        else if (type < 2.0)
            dist = length(sin(p * 0.5 + t));
        else if (type < 3.0)
            dist = dot(cos(p * 0.3 + t), sin(q * 0.3 + t));
        else if (type < 4.0)
            dist = sin(length(p - q) + t);
        else
            dist = fract(length(p - q) + t);

        for (n = 0.1; n < 1.0; n += n)
            dist -= abs(dot(sin(p * n * 16.0), q - q + 0.03)) / n;

        s = 0.04 + abs(dist) * 0.2;
        acc += s;

        vec3 col = paletteColor(acc * colorPulse + t * 0.1, palette);
        color += col / s / acc;
    }

    color = tanh(color / 200.0);

    if (glitch > 0.0) {
        color.r += sin(t * 50.0 + color.g * 10.0) * 0.1 * glitch;
        color.g += cos(t * 45.0 + color.b * 10.0) * 0.1 * glitch;
        color.b += sin(t * 60.0 + color.r * 10.0) * 0.1 * glitch;
    }

    color = mix(vec3(dot(color, vec3(0.299, 0.587, 0.114))), color, saturation);
    color = (color - 0.5) * contrast + 0.5;
    color *= brightness;
    color = clamp(color, 0.0, 0.95); // no whites

    gl_FragColor = vec4(color, 1.0);
}
