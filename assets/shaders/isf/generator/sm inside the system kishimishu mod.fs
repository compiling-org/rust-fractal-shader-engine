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

#define spacing       7.0
#define light_spacing 2.0
#define attenuation  22.0

#define iterations 50.0
#define max_dist   80.0

#define epsilon 0.005
#define GLOBAL_SPEED 0.7
#define camera_speed 1.0
#define lights_speed 30.0
#define columns_speed 4.0

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define rep(p, r) (mod(p + r / 2.0, r) - r / 2.0)
#define torus(p) (length(vec2(length(p.xz) - 0.6, p.y)) - 0.06)

float hash12(vec2 p) {
	vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 getLight(vec3 p, vec3 color) {
    return max(vec3(0.0), color / (1.0 + pow(abs(torus(p) * attenuation), 1.3)) - 0.001);
}

vec3 geo(vec3 po, inout float d, inout vec2 f) {
    float r = hash12(floor(po.yz / spacing + vec2(0.5))) - 0.5;
    vec3 p = rep(po + vec3(time * speed * r * columns_speed, 0.0, 0.0), vec3(0.5, spacing, spacing));
    p.xy *= rot(1.57);
    d = min(d, torus(p));
    
    f = floor(po.yz / (spacing * light_spacing) - vec2(0.5));
    r = hash12(f) - 0.5;
    if (r > -0.45) p = rep(po + vec3(time * speed * lights_speed * r, 0.0, 0.0), spacing * light_spacing * vec3(r + 0.54, 1.0, 1.0));
    else p = rep(po + vec3(time * speed * lights_speed * 0.5 * (1.0 + r * 0.003 * hash12(floor(po.yz * spacing))), 0.0, 0.0), spacing * light_spacing);
    p.xy *= rot(1.57);
    f = (cos(f.xy) * 0.5 + 0.5) * 0.4;
    
    return p;
}

vec4 map(vec3 p) {
    float d = 1e6;
    vec3 po, col = vec3(0.0);
    vec2 f;
    
    po = geo(p, d, f);
    col += getLight(po, vec3(1.0, f));
    
    p.z += spacing / 2.0;
    p.xy *= rot(1.57);
    po = geo(p, d, f);
    col += getLight(po, vec3(f.x, 0.5, f.y));
    
    p.xy += spacing / 2.0;
    p.xz *= rot(1.57);
    po = geo(p, d, f);
    col += getLight(po, vec3(f, 1.0));
     
    return vec4(col, d);
}

vec3 getOrigin(float t) {
    t = (t + 35.0) * -0.05 * camera_speed;
    float rad = mix(50.0, 80.0, cos(t * 1.24) * 0.5 + 0.5);
    return vec3(rad * sin(t * 0.97), rad * cos(t * 1.11), rad * sin(t * 1.27));
}

void initRayOriginAndDirection(vec2 uv, inout vec3 ro, inout vec3 rd) {
    ro = getOrigin(time);
    
    vec3 f = normalize(getOrigin(time + 0.5) - ro);
    vec3 r = normalize(cross(normalize(ro), f));
    rd = normalize(f + uv.x * r + uv.y * cross(f, r));
}

vec3 psychedelicPalette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.3, 0.6, 0.8) * sin(t * 5.0) + vec3(0.2, 0.4, 0.6) * cos(t * 3.0);
    
    return a + b * cos(6.28318 * (c * t + d)) * colorPulse;
}

void main() {
    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 p, ro, rd, col;
    
    initRayOriginAndDirection(uv * zoom, ro, rd);
    
    float t = 2.0;
    for (float i = 0.0; i < iterations; i++) {
        p = ro + t * rd;
        
        vec4 res = map(p);
        col += res.rgb * psychedelicPalette(time * morphing);
        t += abs(res.w);

        if (abs(res.w) < epsilon) t += epsilon;
        
        if (col.r >= 1.0 && col.g >= 1.0 && col.b >= 1.0) break;
        if (t > max_dist) break;
    }
    
    col = pow(col, vec3(0.45));
    
    col = mix(col, col.bgr, colorShift);
    
    gl_FragColor = vec4(col, 1.0);
}
