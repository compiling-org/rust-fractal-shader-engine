/*{
  "DESCRIPTION": "3D Hex Fractal Tunnel with Starfield, Hue Shift, AO, and Rotation",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Raymarch", "Tunnel", "Color", "Starfield"],
  "INPUTS": [
    { "NAME": "TimeSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "HueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "Rotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": -6.28, "MAX": 6.28 }
  ]
}*/

float sdHexPrism(vec3 p, vec2 h) {
    vec3 q = abs(p);
    return max(q.z - h.y, max((q.x * 0.866025 + q.y * 0.5), q.y) - h.x);
}

float opS(float d1, float d2) {
    return max(-d1, d2);
}

vec2 opU(vec2 d1, vec2 d2) {
    return (d1.x < d2.x) ? d1 : d2;
}

vec2 map(vec3 pos) {
    float height = 0.42;
    float depth = 0.75;
    float t = 0.001;
    pos.z = mod(pos.z, depth * 2.0) - depth;

    float cyl = sdHexPrism(pos, vec2(height - t, depth + t));
    float scyl = sdHexPrism(pos, vec2(height - t * 2.0, depth + t + 0.001));
    vec2 res = vec2(opS(scyl, cyl), 1.5);

    for (int i = 1; i < 3; i++) {
        height -= 0.1;
        depth -= 0.19;
        cyl = sdHexPrism(pos, vec2(height - t, depth + t));
        scyl = sdHexPrism(pos, vec2(height - t * 2.0, depth + t + 0.001));
        res = opU(res, vec2(opS(scyl, cyl), 2.5));
    }

    return res;
}

vec2 castRay(vec3 ro, vec3 rd) {
    float t = 0.0;
    float m = -1.0;
    for (int i = 0; i < 100; ++i) {
        vec2 res = map(ro + rd * t);
        if (t > 100.0) break;
        t += res.x;
        m = res.y;
    }
    if (t > 100.0) m = -1.0;
    return vec2(t, m);
}

vec3 calcNormal(vec3 pos, float time) {
    vec3 eps = vec3(0.01, 0.0, 0.0);
    vec3 nor = vec3(
        map(pos + eps.xyy).x - map(pos - eps.xyy).x,
        map(pos + eps.yxy).x - map(pos - eps.yxy).x,
        map(pos + eps.yyx).x - map(pos - eps.yyx).x
    );
    mat2 rotM = mat2(cos(time), sin(time), -sin(time), cos(time));
    nor.xz *= rotM;
    nor.yz *= rotM;
    return normalize(nor);
}

float calcAO(vec3 pos, vec3 nor) {
    float occ = 0.0;
    float sca = 1.0;
    for (int i = 0; i < 5; ++i) {
        float hr = 0.01 + 0.12 * float(i) / 4.0;
        vec3 aopos = pos + nor * hr;
        float dd = map(aopos).x;
        occ += -(dd - hr) * sca;
        sca *= 0.95;
    }
    return clamp(1.0 - 3.0 * occ, 0.0, 1.0);
}

vec3 hueShift(vec3 color, float shift) {
    const vec3 kRGBToYPrime = vec3(0.299, 0.587, 0.114);
    const vec3 kRGBToI = vec3(0.596, -0.275, -0.321);
    const vec3 kRGBToQ = vec3(0.212, -0.523, 0.311);
    const vec3 kYIQToR = vec3(1.0, 0.956, 0.621);
    const vec3 kYIQToG = vec3(1.0, -0.272, -0.647);
    const vec3 kYIQToB = vec3(1.0, -1.107, 1.704);

    float YPrime = dot(color, kRGBToYPrime);
    float I = dot(color, kRGBToI);
    float Q = dot(color, kRGBToQ);

    float hue = atan(Q, I);
    float chroma = sqrt(I * I + Q * Q);
    hue += shift;

    Q = chroma * sin(hue);
    I = chroma * cos(hue);
    vec3 yIQ = vec3(YPrime, I, Q);
    return vec3(dot(yIQ, kYIQToR), dot(yIQ, kYIQToG), dot(yIQ, kYIQToB));
}

mat3 setCamera(vec3 ro, vec3 ta, float cr) {
    vec3 cw = normalize(ta - ro);
    vec3 cp = vec3(sin(cr), cos(cr), 0.0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    return mat3(cu, cv, cw);
}

vec3 starfield(vec2 uv, float time) {
    vec3 col = vec3(0.0);
    vec2 p = fract(sin(vec2(dot(uv, vec2(12.9898,78.233)), 
                            dot(uv, vec2(39.3468,11.1352)))) * 43758.5453);
    float rnd = fract(p.x * p.y * 95.3 + time * 0.1);
    if (rnd > 0.998) {
        float brightness = smoothstep(0.997, 1.0, rnd);
        col += vec3(1.0, 0.9, 0.8) * brightness;
    }
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * TimeSpeed;

    vec3 ro = vec3(0.0, 0.0, t);
    vec3 ta = ro + vec3(0.0, 0.0, 1.0);
    mat3 cam = setCamera(ro, ta, Rotation);
    vec3 rd = cam * normalize(vec3(uv, Zoom * 1.5));

    vec3 col = vec3(1.0);
    vec2 ray = castRay(ro, rd);
    float dist = ray.x;
    float m = ray.y;

    if (m > -0.5) {
        vec3 pos = ro + rd * dist;
        vec3 nor = calcNormal(pos, t);
        float occ = calcAO(pos, nor);
        col = 1.0 - hueShift(vec3(0.0, 1.0, 1.0), HueShift + pos.z * 0.05) * occ;
    }

    vec3 stars = starfield(uv * 20.0, t);
    col = mix(stars, col, 0.9); // subtle blend
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
