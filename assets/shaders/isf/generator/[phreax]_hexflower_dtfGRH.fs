/*{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Enhanced version with tunable parameters for psychedelic effects",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "DEFAULT": 1,
            "MAX": 2,
            "MIN": 0.5,
            "NAME": "colorShift",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 0.1,
            "MIN": 0.1,
            "NAME": "colorPulseSpeed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 2.5,
            "MIN": 0.5,
            "NAME": "morphIntensity",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.2,
            "MAX": 1,
            "MIN": 0,
            "NAME": "artifactStrength",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 2,
            "MIN": 0.5,
            "NAME": "patternScale",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/

#define PI 3.141592
#define TAU (PI * 2.0)
#define SIN(x) (0.5 + 0.5 * sin(x))

float tt;

vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(TAU * (c * t + d));
}

vec3 getPal(int id, float t) {
    t *= colorShift; // Apply color shift variation
    return pal(t, vec3(0.5), vec3(0.5), vec3(1.0), vec3(0.0, 0.15, 0.25));
}

float box(vec3 p, vec3 r) {
    vec3 q = abs(p) - r;
    return max(max(q.x, q.y), q.z);
}

mat2 rot(float a) {
    return mat2(cos(a), sin(a), -sin(a), cos(a));
}

vec3 kalei(vec3 p) {
    float w = 1.0;
    p = abs(p) - 0.2;
    for (float i = 0.0; i < 3.0; i++) {
        float t1 = 2.0 + sin(i + tt * morphIntensity) + sin(0.7 * tt) * 0.4;
        p.xy *= rot(0.3 * t1);
        p -= 0.1 + 0.1 * i;
        p = abs(p);
    }
    p /= w;
    return p;
}

vec2 foldSym(vec2 p, float N) {
    float t = atan(p.x, -p.y);
    t = mod(t + PI / N, 2.0 * PI / N) - PI / N;
    p = length(p.xy) * vec2(cos(t), sin(t));
    p = abs(p) - 0.25;
    p = abs(p) - 0.25;
    return p;
}

float map(vec3 p) {
    p = kalei(p * patternScale);
    float blen = 3.9;
    float outer = 1.3;
    float inner = 1.1;
    float d = max(box(p, vec3(vec2(outer), blen)), -box(p, vec3(vec2(inner), blen + 0.3)));
    return d + artifactStrength * sin(p.z * 10.0); // Add imaginary artifacts
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.x;
    tt = mod(TIME * colorPulseSpeed, 8.0);
    
    vec3 col;
    float d = 0.1, t = 0.0;
    vec3 p = vec3(uv, 0.25);
    
    for (float i = 0.0; i < 200.0; i++) {
        d = map(p);
        if (d < 0.001 || t > 100.0) break;
        p += d * normalize(vec3(uv, 1.0));
        t += d;
    }

    vec3 al = getPal(5, uv.y + sin(tt * TAU / 16.0));
    col = mix(al, vec3(0.1), 1.0 - exp(-t * 0.04));

    gl_FragColor = vec4(col, 1.0);
}
