/*{
  "DESCRIPTION": "Combined shader with tunable parameters and fixed GLSL loop behavior",
  "CATEGORIES": ["Raymarching", "Psychedelic"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "rings", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 50.0 }
  ]
}*/

#define PI 3.14159265359

float happy_star(vec2 uv, float anim) {
    uv = abs(uv);
    vec2 pos = min(uv.xy / uv.yx, anim);
    float p = (2.0 - pos.x - pos.y);
    return (2.0 + p * (p * p - 1.5)) / (uv.x + uv.y);
}

void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv = (gl_FragCoord.xy - 0.5 * res) / res.y;
    uv *= zoom;

    float t = time * speed;
    float t2 = t * 0.1 + ((0.25 + 0.05 * sin(t * 0.1)) / (length(uv.xy) + 0.07)) * 2.2;
    float si = sin(t2);
    float co = cos(t2);
    mat2 ma = mat2(co, si, -si, co);
    uv *= ma;

    vec2 o = gl_FragCoord.xy - res / 2.0;
    o *= ma;
    o = vec2(max(abs(o.x) * 0.8996 + o.y * 0.5, -o.y) * 2.0 / res.y - 0.25, atan(o.y, o.x));
    
    vec4 s = (sin(t) * 0.03 + 0.07) * sin(1.5 * vec4(sin(t)*4.1,32,33,14) + 42.0 * t / 90.0 + o.y + t * 1.75);
    vec4 e = s.yzwx;
    vec4 f = max(o.x - s - 0.1, e - o.x);
    
    vec2 p = (gl_FragCoord.xy * 2.0 - res) / min(res.x, res.y);
    vec3 color = vec3(0.0, 0.3, 0.5);
    
    float f3 = 0.0;
    int ringCount = int(clamp(rings, 1.0, 50.0));
    for (int i = 0; i < 50; i++) {
        if (i >= ringCount) break;
        float fi = float(i);
        float s = sin(t + fi * PI / 10.0) * 0.8;
        float c = cos(t + fi * PI / 10.0) * 0.8;
        f3 += 0.001 / (abs(p.x + c) * abs(p.y + s));
    }

    vec2 o2 = gl_FragCoord.xy - res / 2.0;
    o2 = vec2(length(o2) / res.y - 0.3, atan(o2.y, o2.x));
    vec4 s2 = 0.1 * cos(1.6 * vec4(0, 1, 2, 3) + t + o2.y + asin(sin(o2.x)) * cos(sin(t) * 2.0));
    vec4 e2 = s2.yzwx;
    vec4 f2 = max(o2.x - s2, e2 - o2.x);
    vec4 fc = dot(clamp(f2 * res.y, 0.0, 1.0), 40.0 * (s2 - e2)) * (s2 - 0.1) - f2;

    vec4 final = dot(clamp(f * res.y, 0.0, 1.0), 82.0 * (s - e)) * (s - 0.1) * 2.5 + fc * f3;

    uv *= 2.0 * (cos(t * pulse) - 2.5);
    float anim = sin(t * 12.0) * 0.1 + 1.0;
    final += vec4(happy_star(uv, anim) * vec3(0.35, 0.2, 0.55) * 0.5, 1.0);

    gl_FragColor = final;
}
