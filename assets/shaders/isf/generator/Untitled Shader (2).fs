/*{
  "DESCRIPTION": "Psychedelic eye morph with color ripple and fractal noise",
  "CATEGORIES": ["Psychedelic", "Eye", "Noise"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Speed Multiplier" },
    { "NAME": "eyeRadius", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0, "LABEL": "Eye Radius" },
    { "NAME": "zoomAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0, "LABEL": "Zoom" },
    { "NAME": "triangleIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Triangle Ripple" },
    { "NAME": "paletteMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Blend" }
  ]
}*/

#define DEG2RAD 0.0174532925199

float random(vec2 st) {
  return fract(sin(dot(st, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(vec2 st) {
  vec2 i = floor(st);
  vec2 f = fract(st);
  float a = random(i);
  float b = random(i + vec2(1.0, 0.0));
  float c = random(i + vec2(0.0, 1.0));
  float d = random(i + vec2(1.0, 1.0));
  vec2 u = f * f * (3.0 - 2.0 * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

#define NUM_OCTAVES 6
float fbm(vec2 st) {
  float v = 0.0;
  float a = 0.5;
  vec2 shift = vec2(100.0);
  mat2 rot = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
  for (int i = 0; i < NUM_OCTAVES; ++i) {
    v += a * noise(st + 0.2 + fract(time));
    st = rot * st * 2.0 + shift;
    a *= 0.5;
  }
  return v;
}

vec3 eye(vec3 bg, vec2 p, float radius) {
  p.x *= RENDERSIZE.x / RENDERSIZE.y;
  vec3 col = vec3(0.0);
  float r2 = length(p);
  float a = atan(p.y, p.x);
  if (r2 < radius) {
    col = vec3(0.0, 0.3, 0.4 * 0.2 + fract(time));
    float f = fbm(9.0 * p * cos(time));
    col = mix(col, vec3(0.2, 0.5, 0.4), f);
    f = 1.0 - smoothstep(0.2, 0.5, r2);
    col = mix(col, vec3(0.9, 0.6, 0.2), f);
    a += 0.05 * fbm(20.0 * p);
    f = smoothstep(0.3, 1.0, fbm(vec2(6.0 * r2, 20.0 * a)));
    col = mix(col, vec3(1.0), f);
    f = smoothstep(0.4, 0.9, fbm(vec2(8.0 * r2, 10.0 * a)));
    col *= 1.0 - 0.5 * f;
    f = smoothstep(0.6, 0.8, r2);
    col *= 1.0 - 0.5 * f;
    f = smoothstep(0.2, 0.25, r2);
    col *= f;
    f = smoothstep(0.0, 0.15, r2);
    col = mix(col, vec3(0.8, 1.0, 0.0), 1.0 - f);
    f = smoothstep(0.75, 0.8, r2);
    col = mix(col, bg, f);
  } else {
    col = bg;
  }
  return col;
}

float tri(vec2 pos) {
  float a = dot(pos + sin(time), vec2(sin(90.0 * DEG2RAD), cos(60.0 * DEG2RAD)));
  float b = dot(pos + cos(time), vec2(sin(-60.0 * DEG2RAD), cos(-60.0 * DEG2RAD)));
  float c = -pos.y + cos(time);
  return triangleIntensity * 0.002 / abs(min(min(0.01511 - a, 0.105 - b), 0.1505 - c));
}

float tri2(vec2 pos) {
  float a = dot(pos, vec2(sin(60.0 * DEG2RAD), cos(60.0 * DEG2RAD)));
  float b = dot(pos, vec2(sin(-60.0 * DEG2RAD), cos(-60.0 * DEG2RAD)));
  float c = -pos.y;
  float d = 0.2 * cos(60.0 * DEG2RAD);
  float u1 = a - d * 0.25;
  float v1 = b - d * 0.25;
  float w1 = c - d * 0.25;
  float u2 = c + d * 1.25;
  float v2 = b + d * 1.25;
  float w2 = a + d * 1.25;
  float t1 = min(u2, min(u1, v1));
  float t2 = min(v2, min(w1, u1));
  float t3 = min(w2, min(v1, w1));
  float res = max(max(t1, t2), t3);
  return triangleIntensity * 0.002 / abs(res);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 pos = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / max(RENDERSIZE.x, RENDERSIZE.y) / 16.0;
  vec2 st = gl_FragCoord.xy / RENDERSIZE.xy * 3.0;

  float t = time * speed;
  mat2 ma = mat2(cos(t), sin(t), -sin(t), cos(t));
  pos *= ma;

  vec2 q = vec2(fbm(st + 0.01 * time), fbm(st + vec2(1.0)));
  vec2 r = vec2(fbm(st + q + vec2(1.7, 9.2) + 0.15 * time), fbm(st + q + vec2(8.3, 2.8) + 0.126 * time));
  float f = fbm(st + r);

  vec3 base = mix(vec3(0.101, 0.619, 0.666), vec3(0.666, 0.666, 0.498), clamp(f * f * 4.0, 0.0, 1.0));
  base = mix(base, vec3(0, 0, 0.164), clamp(length(q), 0.0, 1.0));
  base = mix(base, vec3(0.666, 1.0, 1.0), clamp(abs(r.x), 0.0, 1.0));

  vec2 eyeUV = vec2(-1.0, -1.0) + 2.0 * uv;
  base *= f * f * f * f + 0.6 * f * f * f + 0.5 * f * f;
  vec3 col = eye(base, eyeUV, eyeRadius);

  float zoom = zoomAmount;
  pos *= zoom;

  vec3 ripple = vec3(0.0);
  for (int i = 0; i < 6; ++i) {
    ripple += vec3(tri(pos), tri(-pos * 0.5), tri2(pos));
    pos /= 0.25;
  }

  vec3 finalColor =
      mix(vec3(0.0), vec3(0.5, 0.0, 1.0), ripple.x) +
      mix(vec3(0.0), vec3(0.5, 0.25, 0.0), ripple.y) +
      mix(vec3(0.0), vec3(0.0, 0.5, 0.1), ripple.z) +
      mix(col, vec3(1.0), paletteMix * 0.3);

  gl_FragColor = vec4(finalColor, 1.0);
}
