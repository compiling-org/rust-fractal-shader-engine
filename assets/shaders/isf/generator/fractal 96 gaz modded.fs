/*{
  "CATEGORIES": ["Fractal", "Psychedelic", "Raymarch"],
  "DESCRIPTION": "Advanced fractal with full 3D camera, morph, palette, and color pulse controls.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "MorphStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "FractalScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "FoldAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FractalPower", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "FractalOffsetX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FractalOffsetY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FractalOffsetZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "CamYaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CamPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "CamRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CamDist", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "ColorPulseAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.99 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "ShakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define R(p,a,t) mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a)
#define H(h) (cos((h)*6.3+vec3(0,23,21))*.5+.5)

mat3 rot3(float yaw, float pitch, float roll) {
    float cy = cos(yaw), sy = sin(yaw);
    float cp = cos(pitch), sp = sin(pitch);
    float cr = cos(roll), sr = sin(roll);
    return mat3(
        cy*cr+sy*sp*sr, sr*cp, -sy*cr+cy*sp*sr,
        -cy*sr+sy*sp*cr, cr*cp, sr*sy+cy*sp*cr,
        sy*cp, -sp, cy*cp
    );
}

vec3 palette(float t, float id) {
  int i = int(floor(id));
  if (i == 0) return H(t);
  if (i == 1) return vec3(sin(t*3.14), cos(t*2.1), sin(t*5.1));
  if (i == 2) return vec3(.6+.4*cos(6.28*t+0.0), .6+.4*cos(6.28*t+2.1), .6+.4*cos(6.28*t+4.2));
  if (i == 3) return vec3(.5+.5*sin(3.*t), .5+.5*sin(4.*t+2.), .5+.5*sin(5.*t+4.));
  if (i == 4) return vec3(.5+.5*cos(2.*t), .5+.5*cos(3.*t+1.), .5+.5*cos(4.*t+3.));
  if (i == 5) return vec3(sin(t*9.0)*.5+.5, cos(t*7.0)*.5+.5, sin(t*5.0)*.5+.5);
  if (i == 6) return vec3(fract(sin(t*10.)*43758.5453));
  return H(t);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv += XYControl;

    if (ShakeAmount > 0.0)
        uv += vec2(sin(TIME * 40.0), cos(TIME * 50.0)) * ShakeAmount * 0.01;

    float t = TIME * Speed;
    float pulse = t * ColorPulseSpeed;
    vec3 c = vec3(0.0);
    vec3 d = normalize(vec3(uv * Zoom, 1.0));
    d = rot3(CamYaw, CamPitch, CamRoll) * d;

    vec3 cam = vec3(0.0, 0.0, -CamDist);
    vec3 p;

    float g = 0.0, s, e;

    for (float i = 0.0; i++ < 99.;) {
        p = cam + g * d;

        p.x += FractalOffsetX;
        p.y += FractalOffsetY;
        p.z += FractalOffsetZ + t;

        p = R(p, vec3(.577), clamp(sin(t * 0.5) * 3., -2., 0.5 * cos(t * 0.3)) * MorphStrength);
        p.z = asin(sin(p.z));

        p = 0.8 - abs(p);
        if (p.y < p.z) p = p.xzy;
        if (p.x < p.y) p = p.yxz;

        s = 3.0;
        float N = floor(FractalIterations);
        for (float j = 0.0; j++ < N;) {
            p = abs(p) - 0.9;
            e = dot(p, p);
            s *= e = FractalPower / min(e, 2.0 + cos(t * 2.0) * 1.5) + 5.0 / min(e, 0.8 + cos(t) * 0.1);
            s *= FractalScale;
            p = abs(p) * e - vec3(2.0, 7.0, 3.0) * (1.0 + FoldAmount);
        }

        g += e = length(p.yz) / s + 0.002;
        vec3 col = palette(log(s) * 0.5 + pulse * ColorPulseAmount, Palette);
        c += mix(vec3(1.0), col, 0.5) * 0.15 / exp(i * i * e);
    }

    c *= c * c;

    float luma = dot(c, vec3(0.299, 0.587, 0.114));
    c = mix(vec3(luma), c, Saturation);
    c = (c - 0.5) * Contrast + 0.5;
    c *= Brightness;
    c = clamp(c, 0.0, 1.0);

    gl_FragColor = vec4(c, 1.0);
}
