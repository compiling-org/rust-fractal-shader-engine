/*{
  "CATEGORIES": ["Fractal", "Raymarching", "Alien"],
  "DESCRIPTION": "Alien Ruins PseudoKleinian fractal with full camera + geometry control + palette & pulse",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "flySpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "modFreq", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "kleinianBend", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "mirrorModAngle", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "structureScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "pulseSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "pulseIntensity", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "cameraOrbitSpeed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "cameraPitch", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "cameraDistance", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "cameraHeight", "TYPE": "float", "DEFAULT": -3.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.1415926

vec2 rot(vec2 p, float a){
    return vec2(p.x*cos(a)-p.y*sin(a),p.x*sin(a)+p.y*cos(a));
}
vec2 moda(vec2 p, float m){
    float a=atan(p.y,p.x)+PI/m;
    a=mod(a-m/2.,m)-m/2.;
    return vec2(cos(a),sin(a))*length(p);
}
vec2 pmod(vec2 p, float m){
    float a=PI/m-atan(p.y,p.x);
    float r=2.*PI/m;
    a=floor(a/r)*r;
    return rot(p,a);
}

float PseudoKleinian(vec3 p) {
    vec3 CSize = vec3(0.92436, 0.90756, 0.92436);
    float size = 1.0;
    vec3 c = vec3(0.0);
    float de = 1.0;
    vec3 offset = vec3(0.0);
    vec3 ap = p + vec3(1.0);
    for (int i = 0; i < 10; i++) {
        ap = p;
        p -= 2.0 * clamp(p, -CSize, CSize);
        float r2 = dot(p, p);
        float k = max(size / r2, kleinianBend);
        p *= k;
        de *= k;
        p += c;
    }
    float r = abs(0.5 * abs(p.z - offset.z) / de);
    return r;
}

float map(vec3 p){
    vec3 q = p.yxz;
    for(int i = 0; i < 3; ++i){
        q.xy = pmod(q.xy, PI / 0.2);
        q.xz = moda(q.xz, mirrorModAngle + 0.1 * sin(modFreq * TIME));
        q *= structureScale;
    }
    return PseudoKleinian(q) - 0.0001;
}

vec3 norm(vec3 p){
    vec2 e = vec2(.01, 0.0);
    return normalize(vec3(
        map(p+e.xyy) - map(p-e.xyy),
        map(p+e.yxy) - map(p-e.yxy),
        map(p+e.yyx) - map(p-e.yyx)
    ));
}

float ao(vec3 p, vec3 q, float d){
    return clamp((map(p+q*d)) / d, 0.0, 1.0);
}
float sss(vec3 p, vec3 q, float d){
    return clamp(3.0 * (map(p+q*d)), 0.0, 1.0);
}
float rnd(float x){return fract(sin(x+23.45)*87.65);}
float dots(vec3 p, float j){
    p *= 4.0 + sin(j);
    p.x += rnd(floor(p.y));
    p *= PI*PI*PI;
    return clamp(0.1 - length(vec2(sin(p.x), cos(p.y))), 0.0, 1.0) * 10.3;
}

vec3 palette(float t, float i) {
    if (i < 0.5) return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67)));
    if (i < 1.5) return vec3(sin(t*6.0), cos(t*4.0), sin(t*3.0+2.0));
    if (i < 2.5) return mix(vec3(1,0.2,0.1), vec3(1.0,1.0,0.5), sin(t*10.+TIME)*0.5+0.5);
    if (i < 3.5) return vec3(t, t*t, 1.0 - t);
    if (i < 4.5) return vec3(cos(t*5.0 + TIME), sin(t*3.0 + TIME), cos(t*7.0));
    if (i < 5.5) return 0.5 + 0.5*cos(6.28318*(t + vec3(0.1, 0.2, 0.3) + TIME*0.1));
    return vec3(sin(t*8.0 + TIME*0.5), cos(t*3.0 + TIME), sin(t*1.0));
}

void main() {
    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y) * zoom;
    vec3 col = vec3(0.0);

    vec3 ro = vec3(
      sin(TIME * cameraOrbitSpeed) * cameraDistance,
      cameraHeight,
      cos(TIME * cameraOrbitSpeed) * cameraDistance
    );
    vec3 ta = vec3(0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 up = normalize(vec3(0.0, 1.0, cameraPitch));
    vec3 side = normalize(cross(fwd, up));
    up = normalize(cross(side, fwd));
    vec3 rd = normalize(p.x * side + p.y * up + fwd * 0.6);

    vec3 ray = ro;
    vec3 N;
    int j = 0;
    for (int i = 0; i < 128; ++i) {
        float d = max(map(ray), 0.0007);
        if (d < 0.001) {
            N = norm(ray);
            j = i;
            break;
        }
        ray += d * rd;
    }

    float pulse = sin(dot(ray.xy, vec2(4.0)) + TIME * pulseSpeed) * 0.5 + 0.5;
    float glow = smoothstep(0.3, 0.7, pulse) * pulseIntensity;

    col = palette(length(ray.xy) + glow, paletteIndex);
    col += pow(1.0 - float(j) / 128.0, 3.0);
    col += length(ray - ro) / 8.0;
    col *= max(dot(N, normalize(ro - ray)), 0.0) + 0.125;
    col *= ao(ray, rd, 0.2) * 0.2 + ao(ray, rd, 0.4) * 0.4;
    col += sss(ray, rd, 0.2) + sss(ray, rd, 0.3) * 0.4 + sss(ray, rd, 0.6) * 0.6;

    vec3 col2 = vec3(0);
    for (int j = 1; j < 16; ++j) {
        float dist = float(j) * 0.2 / length(ray.xz);
        if (dist > 256.0) break;
        vec3 vp = vec3(ro.x, 0.5, 0.5) + rd * dist;
        vp.xy = rot(vp.xy, sin(vp.z * 2.0 + TIME * 0.02));
        col2 += dots(0.05 * vp, float(j)) * clamp(1.0 - dist / 32.0, 0.0, 1.0);
    }
    col.rg += col2.rg;
    col = mix(col, col.brg, vec3(sin(TIME)));

    vec3 gray = vec3(dot(col, vec3(0.299, 0.587, 0.114)));
    col = mix(gray, col, saturation);
    col = (col - 0.5) * contrast + 0.5;
    col *= brightness;

    gl_FragColor = vec4(col, 1.0);
}
