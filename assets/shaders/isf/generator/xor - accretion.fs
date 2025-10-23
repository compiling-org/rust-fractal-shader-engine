/*{
  "CATEGORIES": ["Raymarching", "Fractal", "Psychedelic", "Camera"],
  "CREDIT": "XorDev + OpenAI Unity GPT",
  "DESCRIPTION": "Accretion fractal with alpha bg, DMT palettes, transform/camera control, glitch shimmer, and fixed step error",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom",           "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed",          "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "morph",          "TYPE": "float",   "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse",     "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "palette",        "TYPE": "float",   "DEFAULT": 2.0,  "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "glitch",         "TYPE": "float",   "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shake",          "TYPE": "float",   "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast",       "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "brightness",     "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation",     "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "alpha",          "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalTransform","TYPE": "float",  "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "xyControl",      "TYPE": "point2D", "DEFAULT": [0.5,0.5], "MIN": [0.0,0.0], "MAX": [1.0,1.0] },
    { "NAME": "camYaw",         "TYPE": "float",   "DEFAULT": 0.0,  "MIN": -6.28, "MAX": 6.28 },
    { "NAME": "camPitch",       "TYPE": "float",   "DEFAULT": 0.0,  "MIN": -1.5, "MAX": 1.5 },
    { "NAME": "camRoll",        "TYPE": "float",   "DEFAULT": 0.0,  "MIN": -6.28, "MAX": 6.28 },
    { "NAME": "camDist",        "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "turbulence",     "TYPE": "float",   "DEFAULT": 7.0,  "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "steps",          "TYPE": "float",   "DEFAULT": 20.0, "MIN": 1.0, "MAX": 100.0 }
  ]
}*/

mat3 rotation(float yaw, float pitch, float roll) {
    float sy = sin(yaw), cy = cos(yaw);
    float sp = sin(pitch), cp = cos(pitch);
    float sr = sin(roll), cr = cos(roll);
    return mat3(
        cy*cr + sy*sp*sr, sr*cp, -sy*cr + cy*sp*sr,
       -cy*sr + sy*sp*cr, cr*cp,  sr*sy + cy*sp*cr,
        sy*cp,            -sp,    cy*cp
    );
}

vec3 getPalette(float t, float id) {
    vec3 a, b, c, d;
    if(id < 1.0)      { a=vec3(.5); b=vec3(.5); c=vec3(1); d=vec3(0.0,-0.33,0.33); }
    else if(id < 2.0) { a=vec3(.5); b=vec3(.5); c=vec3(1); d=vec3(0.2,0.1,0.4); }
    else if(id < 3.0) { a=vec3(.6); b=vec3(.5); c=vec3(1); d=vec3(0.8,0.5,0.2); }
    else if(id < 4.0) { a=vec3(.4); b=vec3(.6); c=vec3(1); d=vec3(0.1,0.9,0.4); }
    else if(id < 5.0) { a=vec3(.4); b=vec3(.6); c=vec3(1); d=vec3(0.9,0.4,0.8); }
    else if(id < 6.0) { a=vec3(.7); b=vec3(.3); c=vec3(1); d=vec3(0.5,0.3,0.9); }
    else              { a=vec3(.5); b=vec3(.5); c=vec3(1); d=vec3(0.3,0.7,0.6); }
    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 I = gl_FragCoord.xy;
    vec2 res = RENDERSIZE.xy;
    vec2 uv = (I / res - xyControl) * zoom;

    if (shake > 0.0) {
        uv += vec2(
            sin(uv.y * 60. + TIME * 6.),
            cos(uv.x * 60. - TIME * 5.)
        ) * shake * 0.008;
    }

    mat3 camRot = rotation(camYaw, camPitch, camRoll);
    vec3 ro = vec3(0, 0, camDist);
    vec3 rd = normalize(camRot * vec3(uv * vec2(res.x/res.y, 1.0), -1.0));

    float z = 0.0, d = 0.0;
    vec4 O = vec4(0.0);
    float stepCount = floor(steps);

    for (float i = 1.0; i <= 100.0; i++) {
        if (i > stepCount) break;

        vec3 p = z * rd + ro;

        float f = fractalTransform;
        p = mix(p, vec3(
            atan(p.y * (1. + morph), p.x) * 2.0,
            p.z / 3.0,
            length(p.xy) - 5.0 - z * 0.2
        ), f);

        for (float j = 1.0; j <= turbulence; j++) {
            p += sin(p.yzx * j + TIME * speed + 0.3 * i) / j;
        }

        z += d = length(vec4(0.4 * cos(p) - 0.4, p.z));
        vec3 col = getPalette(p.x + i * 0.4 + z, palette) / d;
        O.rgb += col * colorPulse;
        O.a += 0.05;
    }

    vec3 c = tanh(O.rgb * O.rgb / 400.0);

    if (glitch > 0.0) {
        vec2 g = sin(uv * 90.0 + TIME * 5.0);
        vec3 shimmer = getPalette(dot(g, vec2(1.0, 1.5)), palette + 2.5);
        c = mix(c, c + shimmer * 0.4, glitch);
    }

    c = mix(vec3(0.5), c, contrast);
    c = mix(vec3(dot(c, vec3(0.333))), c, saturation);
    c *= brightness;

    float sceneAlpha = length(O.rgb) > 0.0001 ? 1.0 : alpha;
    gl_FragColor = vec4(c, sceneAlpha);
}
