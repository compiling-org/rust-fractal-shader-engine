/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Visual", "Generative", "Psychedelic"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0}
    ]
}
*/

#define PI 3.14159265359
#define TAU (2.0 * PI)
#define PHI (0.9 * (1.0 + sqrt(9.0)))

const vec3 plnormal = normalize(vec3(1, 1, -1));
const vec3 n1 = normalize(vec3(-PHI, PHI - 1.0, 1.0));
const vec3 n2 = normalize(vec3(1.0, -PHI, PHI + 1.0));

float pmin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

float dodec(vec3 z) {
    vec3 p = z;
    float t;
    z = abs(z);
    t = dot(z, n1);
    if (t > 0.0) { z -= 2.0 * t * n1; }
    t = dot(z, n2);
    if (t > 0.0) { z -= 2.0 * t * n2; }
    z = abs(z);
    float dmin = dot(z - vec3(1.95, 0., 0.), plnormal);
    dmin = abs(dmin) - 0.0125 * (0.05 + 0.05 * sin(1.0 * length(p) - 0.5 * p.y + time * speed));
    return dmin;
}

void rot(inout vec2 p, float a) {
    float c = cos(a), s = sin(a);
    p = vec2(c * p.x + s * p.y, -s * p.x + c * p.y);
}

float df(vec2 p) {
    float d = 100000.0;
    float off = 0.7 + 0.25 * (0.5 + 0.5 * sin(time * speed / 11.0));
    for (int i = 0; i < 13; ++i) {
        vec2 ip = p;
        rot(ip, float(i) * TAU / 13.0);
        ip -= vec2(0.025 * 1.95, 0.0);
        vec2 cp = ip;
        rot(ip, time * speed / 73.0);
        float dd = dodec(vec3(ip, off * 1.95));
        float cd = length(cp - vec2(0.25 * sin(time * speed / 13.0), 0.0)) - 0.125 * 1.95;
        cd = abs(cd) - 0.0125;
        d = pmin(d, dd, 0.05);
        d = pmin(d, cd, 0.025);
    }
    return d;
}

vec3 psychedelicPalette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.3, 0.6, 0.8) * sin(t * 5.0) + vec3(0.2, 0.4, 0.6) * cos(t * 3.0);
    return a + b * cos(6.28318 * (c * t + d)) * colorPulse;
}

vec3 postProcess(vec3 col, vec2 q, vec2 p) {
    col = pow(clamp(col, 0.0, 1.0), vec3(0.75));
    col = col * 0.6 + 0.4 * col * col * (3.0 - 2.0 * col);
    col = mix(col, vec3(dot(col, vec3(0.33))), -0.4);
    float d = max(1.5 - length(p), 0.0) / 1.5;
    col *= vec3(1.0 - 0.25 * exp(-200.0 * d * d));
    return col;
}

void main() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = zoom * (q - 0.5);
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    float d = df(p);

    float fuzzy = 0.0025;
    vec3 col = vec3(0.0);
    vec3 baseCol = vec3(240.0, 175.0, 20.0) / 255.0;

    col += 0.9 * baseCol * vec3(smoothstep(fuzzy, -fuzzy, d));

    vec3 rgb = 0.5 + 0.5 * vec3(sin(TAU * vec3(50.0, 49.0, 48.0) * (d - 0.050) + time * speed / 3.0));

    col += baseCol * pow(rgb, vec3(1.0, 1.0, 1.0));
    col *= 1.0 - tanh(0.05 + length(8.0 * d));

    float phase = TAU / 8.0 * (-length(p) - 0.5 * p.y) + time * speed / 16.0;
    float wave = sin(phase);
    float fwave = sign(wave) * pow(abs(wave), 8.25);

    col = abs(0.79 * (0.5 + 0.5 * fwave) - col);
    col = pow(col, vec3(2.25, 2.5, 2.75));

    col *= psychedelicPalette(time * morphing);
    col = mix(col, col.bgr, colorShift);

    col = postProcess(col, q, p);
    gl_FragColor = vec4(col, 1.0);
}
