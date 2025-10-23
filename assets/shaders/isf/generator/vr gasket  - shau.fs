/*{
  "CATEGORIES": [ "Fractal", "Psychedelic", "Raymarching" ],
  "DESCRIPTION": "Fully controllable Apollonian fractal with psychedelic color palettes, animation, and camera control.",
  "INPUTS": [
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "FractalMorph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FractalWarp", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "FractalIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "PulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "PaletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "CameraYaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "CameraDistance", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "AnimateTunnel", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "AnimateOrbit", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "AnimateWarp", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "XYControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] }
  ]
}*/

#define FAR 20.0
#define EPS 0.001

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 palette(float t, float i) {
  t *= ColorPulse * PulseSpeed;
  if (i < 1.0) return 0.6 + 0.4 * cos(6.2831 * (t + vec3(0.0, 0.15, 0.25))) * vec3(1.0, 1.8, 3.0);
  if (i < 2.0) return vec3(sin(t * 7.0), sin(t * 2.0 + 1.0), sin(t * 3.0 + 2.0)) * 0.5 + 0.5;
  if (i < 3.0) return abs(vec3(sin(t * 4.0), cos(t * 6.0), sin(t * 9.0)));
  if (i < 4.0) return vec3(sin(t * 5.0 + sin(t)), cos(t * 2.1), sin(t * 1.7 + cos(t)));
  if (i < 5.0) return vec3(sin(t * 9.0), sin(t * 0.5 + 2.0), cos(t * 2.0 + 1.0)) * 0.5 + 0.5;
  if (i < 6.0) return 0.5 + 0.5 * sin(t * vec3(2.0, 4.0, 6.0) + vec3(0.0, 0.3, 0.7));
  return vec3(sin(t * 10.0 + sin(t * 0.5)), cos(t * 3.5 + cos(t)), sin(t * 5.0));
}

vec3 tile(vec3 p) {
  return abs(mod(p, 2.0) - 1.0);
}

float sdSphere(vec3 p, float r) {
  return length(p) - r;
}

vec3 map(vec3 p, float t) {
  float scale = 1.0;
  vec3 q = p;
  for (int i = 0; i < 8; i++) {
    q = mod(q - 1.0, 2.0) - 1.0;
    q -= sign(q) * (0.05 + sin(t * 0.14) * 0.02 * Morph);
    float k = (1.1 + sin(t * 0.1) * -0.1) / dot(q, q);
    q *= k;
    scale *= k;
  }
  float d1 = 0.25 * length(q) / scale;
  float d2 = sdSphere(tile(p) - vec3(1.0), 0.46);
  return vec3(min(d1, d2), d1 < d2 ? 1.0 : 2.0, d2);
}

vec3 normal(vec3 p, float t) {
  vec2 e = vec2(EPS, 0.0);
  return normalize(
    e.yxx * map(p + e.yxx, t).x + e.xxy * map(p + e.xxy, t).x +
    e.xyx * map(p + e.xyx, t).x + e.yyy * map(p + e.yyy, t).x
  );
}

float AO(vec3 p, vec3 n, float t) {
  float r = 0.0, w = 1.0;
  for (int i = 1; i < 5; i++) {
    float d = float(i) / 5.0;
    r += w * (d - map(p + n * d, t).x);
    w *= 0.5;
  }
  return 1.0 - clamp(r, 0.0, 1.0);
}

vec2 csqr(vec2 z) {
  return vec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y);
}

float fractal(vec3 p, float t) {
  p = tile(p);
  if (AnimateWarp) p.yz *= rot(t * FractalWarp);
  vec3 c = p;
  float acc = 0.0;
  float warp = 0.7 + 0.2 * sin(t * 0.3) * FractalMorph;
  for (int i = 0; i < int(FractalIterations); i++) {
    p = warp * abs(p) / dot(p, p) - warp;
    p.yz = csqr(p.yz);
    p = p.zxy;
    acc += exp(-19.0 * abs(dot(p, c)));
  }
  return acc * FractalIntensity * 0.5;
}

float marchFractal(vec3 ro, vec3 rd, float t) {
  float sum = 0.0, d = EPS;
  for (int i = 0; i < 50; i++) {
    vec3 p = ro + d * rd;
    if (sdSphere(tile(p) - vec3(1.0), 0.46) > EPS) break;
    float b = 1.0 / (1.0 + pow(sdSphere(tile(p) - vec3(1.0), 0.01), 2.0) * 20.0);
    float f = fractal(p, t);
    d += 0.02 * exp(-2.0 * f);
    sum += 0.04 * b;
  }
  return sum;
}

vec3 render(vec3 ro, vec3 rd, float t) {
  vec3 col = vec3(0.0), bg = vec3(0.0), gc = palette(t, PaletteIndex), p;
  float dist = 0.0, id = 0.0;

  for (int i = 0; i < 100; i++) {
    p = ro + dist * rd;
    vec3 res = map(p, t);
    if (res.x < EPS || dist > FAR) {
      id = res.y;
      break;
    }
    float glow = 1.0 / (1.0 + res.z * res.z * 140.0);
    bg += gc * glow * 0.02;
    dist += res.x;
  }

  if (id > 0.0) {
    vec3 n = normal(p, t);
    float ao = AO(p, n, t);
    float diff = max(dot(n, normalize(vec3(3.0, 4.0, -1.0))), 0.1);
    float spec = pow(max(dot(reflect(-normalize(vec3(3.0, 4.0, -1.0)), n), -rd), 0.0), 32.0);
    float fres = pow(clamp(dot(n, rd) + 1.0, 0.0, 1.0), 2.0);

    if (id == 1.0) {
      col = vec3(0.1) * diff + vec3(0.1, 0.2, 0.4) * max(n.y, 0.0) + gc * 0.6 * spec;
    } else if (id == 2.0) {
      col = gc * diff * 0.4 + gc * marchFractal(p, rd, t) * (1.0 - fres) * 0.6 + vec3(1.0) * spec;
      col += gc * fres * 0.04 * diff;
    }

    col *= ao;
  }

  col += bg;
  col *= exp(-0.2 * dist);
  return col;
}

vec3 applyPost(vec3 c) {
  c = mix(vec3(0.5), c, Contrast);
  float l = dot(c, vec3(0.2126, 0.7152, 0.0722));
  c = mix(vec3(l), c, Saturation);
  return clamp(c * Brightness, 0.0, 1.0);
}

void main() {
  float t = TIME * Speed;
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 normUV = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

  vec3 la = vec3(0.0);
  vec3 ro = vec3(0.0, 0.0, -CameraDistance / Zoom);

  // user XYControl affects camera angle
  float camX = (XYControl.x - 0.5) * 3.14;
  float camY = (XYControl.y - 0.5) * 1.57;
  ro.xz *= rot(CameraYaw + camX);
  ro.yz *= rot(CameraPitch + camY);

  if (AnimateTunnel) ro.z += t;
  if (AnimateOrbit) {
    float r = CameraDistance / Zoom;
    float a = t * 0.3;
    ro.x = r * sin(a);
    ro.z = r * cos(a);
  }

  vec3 fwd = normalize(la - ro);
  vec3 rgt = normalize(vec3(fwd.z, 0.0, -fwd.x));
  vec3 up = cross(rgt, fwd);
  vec3 rd = normalize(fwd + rgt * normUV.x + up * normUV.y);

  vec3 col = render(ro, rd, t);
  col = applyPost(col);
  gl_FragColor = vec4(col, 1.0);
}
