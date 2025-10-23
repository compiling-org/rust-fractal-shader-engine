/*{
  "CATEGORIES": ["Fractal", "Raymarching"],
  "CREDIT": "Mandelmaze by dr2 - Full ISF Port with Pulse, Palettes, Glitch, Shake, Texture",
  "INPUTS": [
    { "NAME": "Zoom",         "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.1,  "MAX": 4.0 },
    { "NAME": "Speed",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 4.0 },
    { "NAME": "Size",         "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 10.0 },
    { "NAME": "RingOuter",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.1,  "MAX": 2.0 },
    { "NAME": "RingInner",    "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.1,  "MAX": 2.0 },
    { "NAME": "ChannelSize",  "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.01, "MAX": 1.0 },
    { "NAME": "Velocity",     "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.01, "MAX": 1.0 },
    { "NAME": "LightPulse",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 50.0 },
    { "NAME": "Brightness",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "Saturation",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "Contrast",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.1,  "MAX": 3.0 },
    { "NAME": "Shake",        "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "Glitch",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "Palette",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 6.0 },
    { "NAME": "TexScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
{ "NAME": "TexEnable", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "TexInput",     "TYPE": "image" }
  ]
}*/

const float pi = 3.14159;
const vec4 cHashA4 = vec4(0., 1., 57., 58.);
const vec3 cHashA3 = vec3(1., 57., 113.);
const float cHashM = 43758.54;

vec4 Hashv4f(float p) {
  return fract(sin(p + cHashA4) * cHashM);
}

float Noisefv2(vec2 p) {
  vec2 ip = floor(p);
  vec2 fp = fract(p);
  fp = fp * fp * (3. - 2. * fp);
  vec4 t = Hashv4f(dot(ip, cHashA3.xy));
  return mix(mix(t.x, t.y, fp.x), mix(t.z, t.w, fp.x), fp.y);
}

float Fbm2(vec2 p) {
  float f = 0., a = 1.;
  for (int i = 0; i < 5; i++) {
    f += a * Noisefv2(p);
    a *= 0.5;
    p *= 2.;
  }
  return f;
}

float PrBox2Df(vec2 p, vec2 b) {
  vec2 d = abs(p) - b;
  return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float PrCylAnDf(vec3 p, float r, float w, float h) {
  return max(abs(length(p.xy) - r) - w, abs(p.z) - h);
}

mat3 vuMat;
vec3 vuPos;
float tCur, dstFar, chRingO, chRingI, vuVel, bxSize, chSize, qnStep;
int idObj;

float MBoxDf(vec3 p) {
  vec4 q = vec4(p, 1.);
  vec4 q0 = q;
  const float mScale = 2.62;
  const int nIter = 12;
  for (int n = 0; n < nIter; n++) {
    q.xyz = clamp(q.xyz, -1., 1.) * 2. - q.xyz;
    q = q * mScale / clamp(dot(q.xyz, q.xyz), 0.5, 1.) + q0;
  }
  return length(q.xyz) / abs(q.w);
}

float ObjDf(vec3 p) {
  vec3 q = p;
  float dMin = dstFar;
  float d = MBoxDf(p);
  q.y -= vuPos.y;
  float tWid = 0.7 * chSize;
  float dm = min(PrCylAnDf(q.xzy, chRingO, chSize, chSize),
                 PrCylAnDf(q.xzy, chRingI, tWid, chSize));
  dm = min(min(dm, PrBox2Df(q.xy, vec2(tWid, chSize))),
               PrBox2Df(q.zy, vec2(tWid, chSize)));
  d = max(d, -dm);
  if (d < dMin) { dMin = d; idObj = 1; }
  return dMin;
}

float ObjRay(vec3 ro, vec3 rd) {
  const int nStep = 200;
  float dHit = 0., d = 0., s = 0.;
  for (int j = 0; j < nStep; j++) {
    d = ObjDf(ro + dHit * rd);
    dHit += d;
    s += 1.;
    if (d < 0.0003 || dHit > dstFar) break;
  }
  qnStep = s / float(nStep);
  return dHit;
}

vec3 ObjNf(vec3 p) {
  const vec3 e = vec3(0.001, -0.001, 0.);
  vec4 v = vec4(ObjDf(p + e.xxx), ObjDf(p + e.xyy),
                ObjDf(p + e.yxy), ObjDf(p + e.yyx));
  return normalize(vec3(v.x - v.y - v.z - v.w) + 2. * v.yzw);
}

vec3 TrackPath(float t) {
  vec3 p;
  vec2 tr = vec2(0.);
  float ti[9];
  float tO = 0.5 * pi * chRingO / vuVel;
  float tI = 0.5 * pi * chRingI / vuVel;
  float rGap = chRingO - chRingI;
  float tR = rGap / vuVel;
  bool rotStep = false;
  ti[0] = 0.;
  ti[1] = ti[0] + tO; ti[2] = ti[1] + tR;
  ti[3] = ti[2] + tI; ti[4] = ti[3] + tR;
  ti[5] = ti[4] + tO; ti[6] = ti[5] + tR;
  ti[7] = ti[6] + tI; ti[8] = ti[7] + tR;
  float aDir = 2. * mod(floor(t / ti[8]), 2.) - 1.;
  p.y = 0.7 * bxSize * sin(2. * pi * floor(t / (2. * ti[8])) / 11.);
  t = mod(t, ti[8]);
  float a = 0., r = chRingO;
  if (t < ti[4]) {
    if (t < ti[1]) { rotStep = true; a = (t - ti[0]) / (ti[1] - ti[0]); }
    else if (t < ti[2]) { tr.y = chRingO - rGap * (t - ti[1]) / (ti[2] - ti[1]); }
    else if (t < ti[3]) { rotStep = true; a = 1. + (t - ti[2]) / (ti[3] - ti[2]); r = chRingI; }
    else { tr.x = -(chRingI + rGap * (t - ti[3]) / (ti[4] - ti[3])); }
  } else {
    if (t < ti[5]) { rotStep = true; a = 2. + (t - ti[4]) / (ti[5] - ti[4]); }
    else if (t < ti[6]) { tr.y = -chRingO + rGap * (t - ti[5]) / (ti[6] - ti[5]); }
    else if (t < ti[7]) { rotStep = true; a = 3. + (t - ti[6]) / (ti[7] - ti[6]); r = chRingI; }
    else { tr.x = chRingI + rGap * (t - ti[7]) / (ti[8] - ti[7]); }
  }
  if (rotStep) {
    a *= 0.5 * pi * aDir;
    p.xz = r * vec2(cos(a), sin(a));
  } else {
    if (aDir < 0.) tr.y *= -1.;
    p.xz = tr;
  }
  return p;
}

void VuPM(float t) {
  float dt = 0.5;
  vec3 fpF = TrackPath(t + dt);
  vec3 fpB = TrackPath(t - dt);
  vuPos = 0.5 * (fpF + fpB);
  vuPos.y = fpB.y;
  vec3 vel = (fpF - fpB) / (2. * dt);
  float a = atan(vel.z, vel.x) - 0.5 * pi;
  float ca = cos(a), sa = sin(a);
  vuMat = mat3(ca, 0., -sa, 0., 1., 0., sa, 0., ca);
}

vec3 GetTexColor(vec3 pos) {
  if (!TexEnable) return vec3(1.0);
  vec2 uv = fract(pos.xz * 0.2 * TexScale);
  return texture2D(TexInput, uv).rgb;
}


vec3 PaletteColor(float t) {
  t = fract(t);
  if (Palette < 0.5) return vec3(t, 0.5 * sin(t * 6.28), 1. - t);
  if (Palette < 1.5) return vec3(sin(t*10.0), cos(t*3.0), sin(t*5.0));
  if (Palette < 2.5) return vec3(sin(t*6.0), sin(t*2.5 + 2.0), cos(t*2.0));
  if (Palette < 3.5) return vec3(sin(t*2.0), 1.0 - t, t);
  if (Palette < 4.5) return vec3(t, t*t, 1.0 - t);
  if (Palette < 5.5) return vec3(sin(t*7.0), sin(t*13.0), cos(t*17.0));
  return vec3(1.0 - t, sin(t*9.0), cos(t*11.0));
}

float ObjSShadow(vec3 ro, vec3 rd) {
  float sh = 1., d = 0.05, h;
  for (int j = 0; j < 25; j++) {
    h = ObjDf(ro + rd * d);
    sh = min(sh, smoothstep(0., 1., 20. * h / d));
    d += min(0.1, 3. * h);
    if (h < 0.001) break;
  }
  return sh;
}

vec3 ShowScene(vec3 ro, vec3 rd) {
  vec3 col;
  vec3 ltDir[4];
  ltDir[0] = normalize(vec3(1., 1., 0.));
  ltDir[1] = normalize(vec3(0., 1., 1.));
  ltDir[2] = normalize(vec3(1., 0., 1.));
  idObj = -1;
  float dstHit = ObjRay(ro, rd);
  if (dstHit < dstFar) {
    vec3 hit = ro + dstHit * rd;
    vec3 vn = ObjNf(hit);
    float light = 0.;
    for (int j = 0; j < 3; j++) {
      float sh = 0.1 + ObjSShadow(hit, ltDir[j]);
      light += sh * (0.2 + max(dot(vn, ltDir[j]), 0.));
    }
    vec3 tex = GetTexColor(hit);
    float pulse = 0.5 * (1. + cos(LightPulse * tCur));
    vec3 pal = PaletteColor(length(hit) * 0.05 + pulse);
    col = mix(pal * light, tex, 0.55);
    col += pow(max(dot(normalize(-rd), reflect(ltDir[1], vn)), 0.0), 32.0);
  } else {
    float b = Fbm2(rd.xy * 4.0 + tCur);
    col = PaletteColor(b);
  }

  col = pow(clamp(col, 0., 1.), vec3(Contrast));
  col = mix(vec3(dot(col, vec3(0.333))), col, Saturation);
  col *= Brightness;

  return clamp(col, 0.0, 1.0);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  uv = 2.0 * uv - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  uv += vec2(sin(TIME * 60.0), cos(TIME * 70.0)) * 0.005 * Shake;
  uv.x += step(fract(uv.y * 50.0 + TIME), 0.05) * Glitch * 0.2;

  tCur = TIME * Speed;
  dstFar = 30.0;
  bxSize = Size / Zoom;
  chRingO = RingOuter * bxSize;
  chRingI = RingInner * bxSize;
  chSize = ChannelSize * bxSize;
  vuVel = Velocity * bxSize;

  VuPM(tCur);
  vec3 ro = vuPos;
  vec3 rd = normalize(vec3((1.0 / 0.5) * sin(0.5 * uv.x), uv.y, 2.0)) * vuMat;

  gl_FragColor = vec4(ShowScene(ro, rd), 1.0);
}
