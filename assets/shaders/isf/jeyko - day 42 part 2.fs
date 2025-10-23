/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",        "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "colorPulse",   "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "zoom",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "shake",        "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "brightness",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast",     "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "morph",        "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "palette",      "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "camX",         "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "camY",         "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "camZ",         "TYPE": "float", "DEFAULT": 3.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "lookX",        "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "lookY",        "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "lookZ",        "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "repSpacing",   "TYPE": "float", "DEFAULT": 3.7, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "fractDetail",  "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "distort",      "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

#define pi 3.14159265359
#define rot(a) mat2(cos(a),-sin(a),sin(a),cos(a))
#define fov 0.9
#define ITERS 4

float w;
float id;
float WS[ITERS];
vec3 glow = vec3(0.);

vec3 pal(float i, float t){
  if (i < 1.) return 0.5 + 0.5 * sin(vec3(6, 2, 1) * t + vec3(0, 2, 4));
  else if (i < 2.) return vec3(sin(t * 5.1), cos(t * 3.3), sin(t * 2.2));
  else if (i < 3.) return vec3(0.3, 0.8, 0.6) + vec3(sin(t * 2.0), sin(t * 1.1), sin(t * 0.6));
  else if (i < 4.) return vec3(sin(t * 8.0), sin(t * 4.0), sin(t * 2.0));
  else if (i < 5.) return vec3(0.7 + 0.3 * sin(t), 0.4 + 0.2 * cos(t * 1.5), 0.8);
  else if (i < 6.) return vec3(0.9, 0.3 + 0.2 * sin(t), 0.2 + 0.1 * cos(t));
  return vec3(0.1 + 0.5 * sin(t * 3.), 0.8, 0.5 + 0.3 * cos(t * 1.2));
}

vec2 map(vec3 q){
  vec2 d = vec2(1e5);
  id = floor(q.z / repSpacing);
  q.z = mod(q.z, repSpacing) - 0.5 * repSpacing;
  q.xy -= 2.;
  q.xy = mod(q.xy, 4.) - 2.;

  vec4 p = vec4(q, 1.0);
  vec4 c = vec4(0.5,1.,0.4,0.);
  vec4 u = vec4(0.4,0.44,0.7,0.4);

  for(int i = 0; i < ITERS; i++){
    p.xyz = abs(p.xyz) - vec3(0.78, 0.9, 0.4 + morph * 0.6);
    float dpp = dot(p.xyz, p.xyz);
    p = p * (fractDetail + u) / clamp(dpp, 0.4, 1.0) - c;
    p.xy *= rot(-0.8 + morph * 1.2);
    if (i < 2) p.z -= 0.5;
    WS[i] = p.w;
  }

  w = p.z;
  p.xyz = abs(p.xyz);
  p.xz *= rot(0.2 + distort * 1.5);
  float fr = max(p.x, max(p.y - 2.4, p.z - 2.9)) / p.w;
  d.x = fr * 0.5;
  return d;
}

vec2 march(vec3 ro, vec3 rd, out vec3 p, out float t, out bool hit){
  t = 0.;
  hit = false;
  for(int i = 0; i < 180; i++){
    p = ro + rd * t;
    vec2 d = map(p);
    glow += exp(-d.x * 20.);
    if (d.x < 0.001) { hit = true; break; }
    t += d.x;
  }
  return vec2(t);
}

vec3 getNormal(vec3 p){
  vec2 e = vec2(0.001, 0);
  return normalize(vec3(
    map(p).x - map(p - e.xyy).x,
    map(p).x - map(p - e.yxy).x,
    map(p).x - map(p - e.yyx).x
  ));
}

vec3 getRayDir(vec3 ro, vec3 lookAt, vec2 uv){
  vec3 dir = normalize(lookAt - ro);
  vec3 right = normalize(cross(vec3(0,1,0), dir));
  vec3 up = normalize(cross(dir, right));
  return normalize(dir + right * uv.x * fov + up * uv.y * fov);
}

vec3 applyVFX(vec3 col){
  col = (col - 0.5) * contrast + 0.5;
  float g = dot(col, vec3(0.333));
  col = mix(vec3(g), col, saturation);
  col *= brightness;
  return clamp(col, 0.0, 1.0);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  float time = TIME * speed;
  uv += sin(vec2(time * 12.3, time * 8.4)) * shake;
  uv *= zoom + dot(uv, uv) * 0.8;

  // animated base camera
  vec3 baseRo = vec3(sin(time)*2.0, 0.0, cos(time)*2.0 + time);
  vec3 baseLook = vec3(0.0, 0.0, time + 2.0);

  // blended with user camera inputs
  vec3 ro = mix(baseRo, vec3(camX, camY, camZ), morph);
  vec3 lookAt = mix(baseLook, vec3(lookX, lookY, lookZ), morph);

  vec3 rd = getRayDir(ro, lookAt, uv);

  vec3 p; float t;
  bool hit;
  march(ro, rd, p, t, hit);

  vec3 col = vec3(0);
  if(hit){
    vec3 lD = normalize(vec3(1));
    vec3 n = getNormal(p);
    float diff = max(dot(n, lD), 0.);
    vec3 h = normalize(lD - rd);
    float spec = pow(max(dot(n,h), 0.), 50.0);
    vec3 C = vec3(0);
    C += vec3(0.2,0.0,0.2)*pow(1. - WS[0]*(0.15), 7.0);
    C += vec3(1.,1.,0.4)*pow(WS[1]*0.05, 4.0);
    C += vec3(0.,0.4,0.2)*pow(WS[2]*0.02, 2.0);
    C += vec3(0.9,0.4,0.2)*pow(WS[3]*0.01, 2.0);
    col += mix(C, vec3(1.0,.7,.1) * spec, 0.3);
    col += diff * 0.1;
  }

  vec3 palCol = pal(palette, time);
  col += glow * palCol * colorPulse;
  col = applyVFX(col);
  col = pow(col, vec3(0.45));
  gl_FragColor = vec4(col, 1.0);
}
