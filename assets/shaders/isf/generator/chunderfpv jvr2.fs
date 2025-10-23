/*{
  "DESCRIPTION": "Jeweled Vortex with full parameterization, color pulse, palette blending, and fractal controls",
  "CATEGORIES": [ "Fractal", "VJ", "Psychedelic" ],
  "INPUTS": [
    { "NAME": "controlXY", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.01, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "twist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "expFactor", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define PI 3.1415926
#define TAU 6.2831853

vec3 getPalette(float t, float mode) {
    t = fract(t);
    if (mode < 1.0) return vec3(sin(t*TAU), sin(t*TAU*0.5), sin(t*TAU*1.2));
    else if (mode < 2.0) return vec3(cos(t*PI), sin(t*TAU), sin(t*PI*2.));
    else if (mode < 3.0) return vec3(t, 1.0 - t, sin(t * PI));
    else if (mode < 4.0) return vec3(fract(t * 4.0), fract(t * 6.0), fract(t * 9.0));
    else if (mode < 5.0) return vec3(abs(sin(t*10.)), abs(cos(t*5.)), t*t);
    else if (mode < 6.0) return vec3(0.5 + 0.5*sin(t * 20.0), 0.5 + 0.5*cos(t * 15.0), sin(t * 5.0));
    else return vec3(t, t*t, sin(t*5.0));
}

vec3 colorAdjust(vec3 col, float bri, float sat, float con) {
    col *= bri;
    float luma = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(luma), col, sat);
    col = mix(vec3(0.5), col, con);
    return clamp(col, 0.0, 1.0);
}

float numMap(float t) {
    return -sign(t) * log(1.0 - abs(t));
}

float safePow(float b, float e) {
    return pow(max(b, 1e-5), e);
}

void main() {
    float sec = 900.0;
    float st = 150.0;
    float wr = 3.0;

    vec2 R = RENDERSIZE.xy;
    vec2 U = gl_FragCoord.xy;
    vec2 u = (U - R / 2.0) / R.y;
    u = -u.yx;

    float tParam;
    vec2 m = (controlXY * R - R / 2.0) / R.y;
    if (length(controlXY) < 0.01) {
        float a = TIME * speed;
        m = wr * vec2(sin(a * TAU), sin(a * TAU * 2.0));
        tParam = ((mod(TIME * speed + st, sec) / sec) - 0.5) * 2.0;
    } else {
        tParam = atan(m.x, -m.y) / PI;
    }

    float at = abs(tParam);
    float n = numMap(tParam);
    float e = n * (expFactor - at);
    float s = 5.0 + 25.0 * at;
    float ro = log(abs(n)) * s * PI * twist;

    float fr = pow(length(u) * wr, 1.0 / e);
    ro *= min(1.0, pow(at, 0.2));
    u *= clamp(pow(s, e) * wr, 1e-16, 1e18);

    float r = pow(length(u), 1.0 / e);
    float a = atan(u.y, u.x) + ro;
    float v = a / TAU;
    float rv = r - v;
    float cr = ceil(rv);
    float vd = cos((cr * TAU + a) / n);
    float sc = TIME * speed + PI * pow(cr + v, 2.0) / n;

    float q = sin(sc);
    q *= cos(sc * 2.0);
    q *= safePow(abs(sin(PI * rv)), abs(e) + 2.0);
    q *= 0.2 + abs(vd);

    vec3 base = getPalette(TIME * colorPulse + r, palette);
    vec3 c = q * base * vec3(1.4 - vd, abs(cos(tParam * PI / 2.0)) + 0.5, vd + 1.4);
    c += q * q;

    c = min(c, 1.0);
    c *= min(pow(fr, 0.3), 1.0 / fr);

    // Fix for round() GLSL ES compatibility
    vec2 fracU = fract(U / R);
    vec2 b = 0.005 / abs(fracU - vec2(0.5));
    c -= max(b.x, b.y);

    c = colorAdjust(c, brightness, saturation, contrast);
    gl_FragColor = vec4(c, 1.0);
}
