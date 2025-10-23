/*{
  "DESCRIPTION": "Hybrid tunnel + fractal with orb + AO + psychedelic controls",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "Zoom",          "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "Speed",         "TYPE": "float", "MIN": 0.01, "MAX": 4.0, "DEFAULT": 1.0 },
    { "NAME": "Morph",         "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "FractalFreq",   "TYPE": "float", "MIN": 0.5, "MAX": 5.0, "DEFAULT": 1.5 },
    { "NAME": "Shake",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2 },
    { "NAME": "Shimmer",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Brightness",    "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 1.2 },
    { "NAME": "Saturation",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.2 },
    { "NAME": "Contrast",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.1 },
    { "NAME": "Gain",          "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.5 },
    { "NAME": "ColorPulse",    "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.0 },
    { "NAME": "Palette",       "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 0.0 },
    { "NAME": "CamTheta",      "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.0 },
    { "NAME": "CamPhi",        "TYPE": "float", "MIN": -1.57, "MAX": 1.57, "DEFAULT": 0.0 },
    { "NAME": "CamDist",       "TYPE": "float", "MIN": 0.5, "MAX": 10.0, "DEFAULT": 4.0 },
    { "NAME": "Tex",           "TYPE": "image" }
  ]
}*/

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define N normalize

float length2(vec2 p){
    float k = 10.;
    p = pow(abs(p), vec2(k));
    return pow(p.x + p.y, 1.0/k);
}

float sdBox(vec3 p, vec3 b) {
  vec3 q = abs(p) - b;
  return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdCross(vec3 p) {
  float da = sdBox(p.xyz, vec3(9999.0, 1.0, 1.0));
  float db = sdBox(p.yzx, vec3(1.0, 9999.0, 1.0));
  float dc = sdBox(p.zxy, vec3(1.0, 1.0, 9999.0));
  return min(da, min(db, dc));
}

float menger(vec3 p) {
  float d = 0.0, scale = 0.75;
  for (int i = 0; i < 4; i++) {
    vec3 a = mod(p * scale, 3.0) - 1.0;
    vec3 r = 3.0 - 2.0 * abs(a);
    scale *= 4.0;
    float c = sdCross(r) / scale;
    d = max(d, c);
  }
  return d;
}

vec3 P(float z) {
  return vec3(cos(z * 0.23) * 3.2, 3.5 + cos(z * 0.32) * 3.2, z);
}

float orb(vec3 p, float T) {
  vec3 q = P(p.z);
  return length(p - vec3(q.x + 0.5, q.y + 0.5, 2.75 + T + tanh(cos(T * 0.25)) * 1.0));
}

float tunnel(vec3 p) {
  vec2 tun = p.xy - P(p.z).xy;
  return 1.0 - length2(tun);
}

float fractal(vec3 p) {
  float w = 0.8, l;
  float md = menger(p * 3.0);
  p *= FractalFreq;
  for (int i = 0; i < 6; i++) {
    p = abs(sin(p * Morph)) - 1.0;
    l = 1.6 / dot(p, p);
    p *= l;
    w *= l;
  }
  return min(md, length(p) / w);
}

vec3 getPalette(int i, float t) {
  if (i == 0) return vec3(sin(t*6.0), sin(t*4.0+1.0), sin(t*2.0+2.0));
  if (i == 1) return vec3(abs(cos(t*3.0)), abs(sin(t*5.0+1.0)), sin(t*7.0+2.0));
  if (i == 2) return vec3(0.5+0.5*sin(t), 0.5+0.5*cos(t*1.5), 0.5+0.5*sin(t*0.5));
  if (i == 3) return vec3(fract(t), fract(t*2.0), fract(t*3.0));
  if (i == 4) return vec3(sin(t*2.1)*0.7+0.3, cos(t*1.5)*0.7+0.3, sin(t*3.7)*0.7+0.3);
  if (i == 5) return vec3(abs(sin(t*4.3)), abs(cos(t*3.1)), abs(sin(t*2.7)));
  if (i == 6) return vec3(0.9, 0.3 + 0.7*sin(t*2.0), 0.6 + 0.4*cos(t*3.0));
  return vec3(1.0);
}

void main() {
  vec2 u = isf_FragNormCoord * RENDERSIZE;
  vec2 r = RENDERSIZE;
  float T = TIME * Speed;
  float d = 0.0, s = 0.002, i = 0.0;

  vec3 cam = vec3(
    CamDist * cos(CamPhi) * sin(CamTheta),
    CamDist * sin(CamPhi),
    CamDist * cos(CamPhi) * cos(CamTheta)
  );
  vec3 p = P(T), ro = p;
  vec3 Z = normalize(P(T + 1.0) - p);
  vec3 X = normalize(vec3(Z.z, 0.0, -Z.x));
  vec2 nuv = (u - r * 0.5) / r.y;

  vec3 D = vec3(
    rot(sin(T * 0.175) * 0.25) *
    rot(tanh(sin(p.z * 0.2) * 2.0) * 2.0) *
    nuv, 1.0 / Zoom
  ) * 0.75 * mat3(-X, cross(X, Z), Z);

  // Shake
  D += vec3(sin(T*10.0 + u.y*10.0), cos(T*7.0 + u.x*10.0), sin(T*3.5)) * Shake * 0.2;

  vec4 o = vec4(0.0);
  bool orbHit = false;

  for (i = 0.0; i++ < 99.0 && s > 0.001;) {
    p = ro + D * d;
    float orbDist = orb(p, T) - 0.1;
    float geoDist = max(fractal(p), tunnel(p));
    s = min(orbDist, geoDist) * 0.7;
    d += s;
    orbHit = s == orbDist;
    o.rgb += s * 0.01 + 0.01;
  }

  o.rgb = 2.0 - o.rgb;
  o.rgb *= texture(Tex, sin(p.xy)).rgb;

  float f = mod(0.01 * T + p.z * 0.1, 4.0);
  if (orbHit) {
    o.rgb = 0.5 * abs(sin(p * 0.1) / dot(sin(p * 64.0), vec3(16.0)));
  } else {
    if (f > 3.0)
      o.rgb = mix(o.rgb, vec3(1.0, 0.0, 0.0) * abs(sin(5.0*T + p * 0.2) / dot(sin(p * 16.0), vec3(2.0))), 0.9);
    else if (f > 1.0)
      o.rgb = mix(o.rgb, vec3(2.0, 0.5, 3.0) * abs(sin(1.0*T + p * 0.2) / dot(sin(p * 8.0), vec3(0.1))), 0.5);
  }

  vec3 e = vec3(0.01, 0.0, 0.0);
  vec3 rgrad = normalize(vec3(
    fractal(p - e.xyy) - fractal(p + e.xyy),
    fractal(p - e.yxy) - fractal(p + e.yxy),
    fractal(p - e.yyx) - fractal(p + e.yyx)
  ));

  o.rgb *= max(dot(rgrad, normalize(ro - p)), 0.1);
  o.rgb /= pow(orb(p, T), 2.5);
  o.rgb *= exp(-d / 4.0);
  o.rgb *= Gain;
  o.rgb *= getPalette(int(Palette), d * 0.1 + sin(p.y + T + ColorPulse) * 0.5 * Shimmer);

  float l = dot(o.rgb, vec3(0.2126, 0.7152, 0.0722));
  o.rgb = mix(vec3(l), o.rgb, Saturation);
  o.rgb = (o.rgb - 0.5) * Contrast + 0.5;
  o.rgb *= Brightness;
  o.rgb = pow(o.rgb, vec3(0.45));

  gl_FragColor = vec4(o.rgb, 1.0);
}
