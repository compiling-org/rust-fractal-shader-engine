/*{
  "CATEGORIES": ["Fractal", "Raymarching", "Psychedelic"],
  "CREDIT": "phreax 2022 + OpenAI Unity GPT",
  "DESCRIPTION": "Raymarched fractal with morph, palette, camera, shimmer, shake, and background alpha",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalDetail", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "spacing", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "orbitAngle", "TYPE": "float", "DEFAULT": 0.5, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "pitch", "TYPE": "float", "DEFAULT": 0.2, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "cameraDist", "TYPE": "float", "DEFAULT": 7.0, "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "alpha", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "xyControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] }
  ]
}*/

#define PI 3.141592
#define SIN(x) (sin(x)*.5+.5)

mat2 rot2(float a) { return mat2(cos(a), sin(a), -sin(a), cos(a)); }

vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

vec3 getPal(float id, float t) {
    vec3 col = pal(t, vec3(.5), vec3(0.5), vec3(1.0), vec3(0.0, -0.33, 0.33));
    if(id < 1.0) return col;
    if(id < 2.0) return pal(t, vec3(.5), vec3(0.5), vec3(1.0), vec3(0.0, 0.10, 0.20));
    if(id < 3.0) return pal(t, vec3(.5), vec3(0.5), vec3(1.0), vec3(0.3, 0.20, 0.20));
    if(id < 4.0) return pal(t, vec3(.5), vec3(0.5), vec3(1.0,1.0,0.5), vec3(0.8, 0.90, 0.30));
    if(id < 5.0) return pal(t, vec3(.5), vec3(0.5), vec3(1.0,0.7,0.4), vec3(0.0, 0.15, 0.20));
    if(id < 6.0) return pal(t, vec3(.5), vec3(0.5), vec3(2.0,1.0,0.0), vec3(0.5, 0.20, 0.25));
    return pal(t, vec3(0.8,0.5,0.4), vec3(0.2,0.4,0.2), vec3(2.0,1.0,1.0), vec3(0.0, 0.25, 0.25));
}

float tt;
vec3 ro;

float curve(float t, float d) {
    t /= d;
    return mix(floor(t), floor(t)+1., pow(smoothstep(0.,1.,fract(t)), 10.));
}

vec3 transform(vec3 p, float a) {
    p.xz *= rot2(a);
    p.xy *= rot2(a);
    return p;
}

float map(vec3 p) {
    vec3 bp = p;
    float morphA = mix(0.4, 1.2, morph);
    float morphB = mix(0.1, 0.6, morph);

    p = transform(p, PI * 0.5 * curve(tt, 4.));

    p.x = abs(p.x) - 0.5 * SIN(tt * .5 * morphA);
    p.y = abs(p.y) - 0.9 * SIN(tt * .8 * morphB);
    p.y -= 0.1;
    p.y = abs(p.y) - 0.1;
    p.x -= 0.2;
    p.x = abs(p.x) - 0.9;
    p.z = abs(p.z) - 0.5;

    p.zy -= 0.5;
    p.xy *= rot2(0.1 * tt);
    p.zy *= rot2(-0.04 * tt);

    float r1 = 1.0;
    float r2 = mix(0.03, 0.3, SIN(tt));
    vec2 cp = vec2(length(p.xz) - r1, p.y);

    float a = atan(p.z, p.x);
    cp *= rot2(3.0 * a + tt);
    cp *= vec2(3.0, 0.4);
    cp.x = abs(cp.x) - 0.3;
    cp *= rot2(2.0 * a);

    float n = fractalDetail;
    for (float i = 0.; i < 20.; i++) {
        if(i >= n) break;
        cp.y = abs(cp.y) - spacing * (0.5 * sin(tt) + 0.9);
        cp *= rot2(0.1 * a * sin(0.1 * tt));
        cp -= i * 0.01 / n;
    }

    float d = length(cp) - r2;
    d = max(0.09 * d, -length(bp.xy - ro.xy) - 4.0);
    return d;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - xyControl) * zoom;

    if (shake > 0.0)
        uv += vec2(sin(uv.y * 30. + TIME * 3.), cos(uv.x * 25. - TIME * 2.)) * shake * 0.01;

    tt = TIME * speed + 2.0 * curve(TIME, 2.0);

    float ca = orbitAngle;
    float cp = pitch;
    float cz = -cameraDist;

    ro = vec3(sin(ca)*cos(cp), sin(cp), cos(ca)*cos(cp)) * cameraDist;
    vec3 target = vec3(0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0,1,0), fwd));
    vec3 up = cross(fwd, right);
    vec3 rd = normalize(uv.x * right + uv.y * up + 0.7 * fwd);

    vec3 p = ro;
    vec3 col = vec3(0.0);
    float t = 0.0, d = 0.1, acc = 0.001;

    for(float i = 0.; i < 200.; i++) {
        d = map(p);
        if(t > 150.) break;
        acc += 0.01 + d * 0.4;
        d = max(abs(d), 0.00002);
        t += d;
        p += rd * d;

        col += 2.0 * clamp(1.0, 0.0, 0.7 / (acc * acc));
        float sl = dot(p, p);
        float pt = 1.0 - 0.1 * sqrt(sl) + 0.2 * p.z + 0.25 * TIME + curve(TIME, 8.0);
        col *= 0.5 * getPal(palette, pt);
        col = clamp(vec3(0.4), vec3(0.0), col);
    }

    if (d < 0.001) {
        vec2 e = vec2(0.0035, -0.0035);
        vec3 n = normalize(
            e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) +
            e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));
        vec3 l = normalize(vec3(0,1,cz) - p);
        float dif = max(dot(n, l), 0.0);
        float spe = pow(max(dot(reflect(-rd, n), -l), 0.0), 40.0);
        float sss = smoothstep(0.0, 1.0, map(p + l * 0.4)) / 0.4;
        col *= mix(1.0, 0.4 * spe + 0.8 * (dif + 2.5 * sss), 0.4);
    }

    if (glitch > 0.0) {
        vec2 g = sin(uv * 80.0 + TIME * 6.0);
        vec3 shimmer = getPal(palette + 1.5, dot(g, vec2(1.0, 1.5)) * 2.0);
        col = mix(col, col + shimmer * 0.6, glitch);
    }

    col = mix(vec3(0.5), col, contrast);
    col = mix(vec3(dot(col, vec3(0.333))), col, saturation);
    col *= brightness;

    // Transparent background, solid ray object
    float shape = smoothstep(0.001, 0.1, d);
    gl_FragColor = vec4(col, mix(alpha, 1.0, 1.0 - shape));
}
