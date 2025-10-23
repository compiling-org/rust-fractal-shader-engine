/*
{
  "CATEGORIES": [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION": "Final corrected version with all parameters working",
  "CREDIT": "Original by bal-khan, fixed by AI",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.125,
      "MIN": 0.01,
      "MAX": 1.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "torus_scale",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 0.5,
      "MAX": 10.0,
      "LABEL": "Torus Scale"
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Zoom Level"
    },
    {
      "NAME": "color1",
      "TYPE": "color",
      "DEFAULT": [0.3,0.7,0.9,1.0],
      "LABEL": "Primary Color"
    },
    {
      "NAME": "color2",
      "TYPE": "color",
      "DEFAULT": [0.95,0.5,0.1,1.0],
      "LABEL": "Secondary Color"
    },
    {
      "NAME": "glow_strength",
      "TYPE": "float",
      "DEFAULT": 0.036,
      "MIN": 0.001,
      "MAX": 0.1,
      "LABEL": "Glow Strength"
    },
    {
      "NAME": "pulse_speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Color Pulse Speed"
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 40.0,
      "MIN": 10.0,
      "MAX": 100.0,
      "LABEL": "Fractal Detail"
    },
    {
      "NAME": "dodeca_scale",
      "TYPE": "float",
      "DEFAULT": 6.0,
      "MIN": 1.0,
      "MAX": 15.0,
      "LABEL": "Dodecahedron Scale"
    }
  ],
  "PASSES": [
    {
      "FLOAT": true,
      "PERSISTENT": true,
      "TARGET": "BufferA"
    },
    {
    }
  ]
}
*/

#define PI 3.14159265359
#define TAU 6.28318530718
#define I_MAX 100.
#define E 0.0001
#define FAR 30.

// Global variables
float t;
vec3 ret_col;
vec3 h; 
float mind;
float ming;
float mint;
float minl;

// Rotation matrix helper
mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

// Polar coordinate modding
vec2 modA(vec2 p, float count) {
    float an = TAU/count;
    float a = atan(p.y,p.x)+an*.5;
    a = mod(a,an)-an*.5;
    return vec2(cos(a),sin(a))*length(p);
}

// Special distance function
float mylength(vec2 p) {
    p = p*p*p*p;
    p = p*p;
    return pow(p.x+p.y,1./8.);
}

// Rotation helper
void rotate(inout vec2 v, float angle) {
    v = rot2(angle) * v;
}

// Camera function
vec3 camera(vec2 uv) {
    vec3 forw  = vec3(0.0,0.0,-1.0);
    vec3 right = vec3(1.0,0.0,0.0);
    vec3 up    = vec3(0.0,1.0,0.0);
    return normalize(uv.x*right + uv.y*up + 1.0*forw);
}

// Scene distance function
float scene(vec3 p) {
    mind = 1e5;
    ming = 1e5;
    mint = 1e5;
    minl = 1e5;

    rotate(p.xz, 1.57 - 0.15 * t);
    rotate(p.yz, 1.57 - 0.125 * t);
    vec3 op = p;
    
    float var = (atan(p.x, p.z) + PI) / TAU;
    var *= detail;
    p.xz = modA(p.xz, detail);
    p.xz -= vec2(8., 0.0);

    rotate(p.xy, t * 0.25 * (mod(var - 0.5, 2.0) <= 1.0 ? 1.0 : -1.0));
    vec2 q = vec2(length(p.xy) - torus_scale, p.z);
    mind = mylength(q) - 0.05;

    float vir = (atan(p.x, p.y) + PI) / TAU;
    var = vir * 30.0;
    p.xy = modA(p.xy, 30.0) - vec2(0.0, 0.0);
    p.xz -= vec2(4.0, 0.0);
    q = vec2(length(p.zx) - 0.25, p.y - 0.0);
    ming = mylength(q) - 0.05;
    mind = min(mind, ming);

    float as = (mind == ming ? 1.0 : 0.0);
    ret_col = step(as, 0.0) * color1.rgb + step(1.0, as) * color2.rgb;
    
    rotate(p.xz, t * 1.5 * (mod(var, 2.0) <= 1.0 ? 1.0 : -1.0) * (mod(vir, 2.0) <= 1.0 ? 1.0 : -1.0));
    p.xz = modA(p.xz, 20.0) - vec2(0.0, -0.0);
    p.xy -= vec2(0.25, 0.0);
    q = vec2(length(p.xy) - 0.1, p.z);
    mint = mylength(q) - 0.02;
    mind = min(mind, mint);
    as = (mind == mint ? 1.0 : 0.0);
    if (as == 1.0) ret_col = color1.rgb;
    
    // Dodecahedron
    rotate(op.zx, t * 0.5);
    op.xz = modA(op.xz, 25.0);
    op -= vec3(dodeca_scale, 0.0, 0.0);
    op /= 1.732; // sqrt(3.)
    vec3 b = vec3(0.075);
    minl = max(max(abs(op.x) + 0.5 * abs(op.y) - b.x, abs(op.y) + 0.5 * abs(op.z) - b.y), abs(op.z) + 0.5 * abs(op.x) - b.z);
    b *= 0.95;
    minl = max(minl, -max(max(abs(op.x) + 0.5 * abs(op.y) - b.x, abs(op.y) - 0.5 * abs(op.z) - b.y), abs(op.z) + 0.5 * abs(op.x) - b.z));
    minl = max(minl, -max(max(abs(op.x) - 0.5 * abs(op.y) - b.x, abs(op.y) + 0.5 * abs(op.z) - b.y), abs(op.z) + 0.5 * abs(op.x) - b.z));
    minl = max(minl, -max(max(abs(op.x) + 0.5 * abs(op.y) - b.x, abs(op.y) + 0.5 * abs(op.z) - b.y), abs(op.z) - 0.5 * abs(op.x) - b.z));
    
    mind = min(mind, minl);
    as = mind == minl ? 1.0 : 0.0;

    if (as == 1.0) ret_col = color2.rgb;
    
    h += glow_strength * color2.rgb / (pow(minl, 25.0) + 0.5);
    return mind;
}

// Raymarching function
vec2 march(vec3 pos, vec3 dir) {
    vec2 dist = vec2(0.0);
    vec3 p;
    vec2 s = vec2(0.0);
    float deps = E;
    for (float i = -1.0; i < I_MAX; i++) {
        p = pos + dir * dist.y;
        dist.x = scene(p);
        dist.y += dist.x;
        deps = -dist.x + (dist.y) / 1500.0;
        if (log(dist.y * dist.y / dist.x / 1e5) > 0.0 || dist.x < deps || dist.y > FAR) break;
        s.x++;
    }
    return vec2(s.x, dist.y);
}

void main() {
    t = abs(TIME * speed); // Use tunable animation speed
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv /= zoom; // Apply zoom control
    vec3 col = vec3(0.0);
    vec2 R = RENDERSIZE.xy;

    vec3 dir = camera(uv);
    vec3 pos = vec3(0.0 + cos(t * 5.0), 0.0 + sin(t * 5.0) * 0.5, 0.0);
    pos.z = 20.0 * exp(-t * 5.0) + 10.0 + 0.5 * sin(t * 10.0);
    h *= 0.0;
    vec2 inter = march(pos, dir);
    float id = (mind == ming ? 1.0 : 0.0) + (mind == mint ? 2.0 : 0.0) + (mind == minl ? 3.0 : 0.0);
    if (inter.y <= FAR)
        col.xyz = ret_col * (1.0 - inter.x * 0.025);
    else
        col *= 0.0;

    col += h;
    col *= clamp((1.5 - length(uv)), 0.5, 1.0);
    col *= clamp((1.1 - length(uv)), 0.0, 1.0);
    gl_FragColor = vec4(col, 1.0); // Output color with alpha
}