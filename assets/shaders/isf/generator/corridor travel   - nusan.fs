/*{
  "DESCRIPTION": "Past racer with full ISF parameter control (preserved logic)",
  "CATEGORIES": [ "Fractal", "Raymarch", "Converted" ],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "fractalType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "xy", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] },
    { "NAME": "camShake", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "camRoll",  "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "tunnelSize", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "surfaceWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 }
  ]
}*/



float steps = 30.0;
float time = 0.0;

mat2 rot(float a) {
  float ca = cos(a);
  float sa = sin(a);
  return mat2(ca, sa, -sa, ca);  
}

void cam(inout vec3 p, float t) {
  t *= 0.3 * speed;
  p.xz *= rot(sin(t) * 0.3 * camShake + camRoll);
  p.xy *= rot(sin(t * 0.7) * 0.4 * camShake);
}

float hash(float t) {
  return fract(sin(t * 788.874));
}

float curve(float t, float d) {
  t /= d;
  return mix(hash(floor(t)), hash(floor(t)+1.0), pow(smoothstep(0.0,1.0,fract(t)),10.0));
}

float tick(float t, float d) {
  t /= d;
  float m = fract(t);
  m = smoothstep(0.0,1.0,m);
  m = smoothstep(0.0,1.0,m);
  return (floor(t) + m) * d;
}

float hash2(vec2 uv) {
  return fract(dot(sin(uv * 425.215 + uv.yx * 714.388), vec2(522.877)));
}

vec2 hash22(vec2 uv) {
  return fract(sin(uv * 425.215 + uv.yx * 714.388) * vec2(522.877));
}

vec3 hash3(vec2 id) {
  return fract(sin(id.xyy * vec3(427.544,224.877,974.542) + id.yxx * vec3(947.544,547.847,652.454)) * 342.774);
}

float camtime(float t) {
  return t * 1.9 + tick(t, 1.9);
}

vec3 getPalette(float t, int index) {
  if (index == 0) return vec3(0.5 + 0.5 * sin(6.2831 * (vec3(0.0, 0.33, 0.67) + t)));
  if (index == 1) return vec3(0.5 + 0.5 * cos(6.2831 * (vec3(1.0, 0.5, 0.25) + t)));
  if (index == 2) return vec3(sin(t * 6.2831 + vec3(0, 2, 4)) * 0.5 + 0.5);
  if (index == 3) return vec3(cos(t * 6.2831 + vec3(2, 1, 3)) * 0.5 + 0.5);
  if (index == 4) return 1.0 - abs(sin(t + vec3(1.0, 2.0, 3.0)));
  if (index == 5) return fract(t * vec3(3.14, 1.61, 0.99));
  return vec3(0.5 + 0.5 * sin(t * vec3(3.0, 1.0, 2.0)));
}

vec3 adjustBSC(vec3 color, float brightness, float saturation, float contrast) {
  vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114)));
  color = mix(gray, color, saturation);
  color = (color - 0.5) * contrast + 0.5;
  return color + brightness;
}

void main() {
  time = mod(TIME, 300.0);

  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  uv -= 0.5;
  uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1);
  vec3 col = vec3(0.0);
  vec3 size = vec3(0.9, 0.9, 1000.0);
  float dof = 0.02;
  float dofdist = 0.2;

  for (float j = 0.0; j < steps; ++j) {
    vec2 off = hash22(uv + j * 74.542 + 35.877) * 2.0 - 1.0;
    float t2 = camtime(time + j * 0.05 / steps * speed);
    vec3 s = vec3(0, 0, -1);
    s.xy += off * dof;
    vec3 r = normalize(vec3(-uv - off * dof * dofdist, zoom));

    cam(s, t2);
    cam(r, t2);

    vec3 alpha = vec3(1.0);

    for (float i = 0.0; i < 3.0; ++i) {
      vec3 boxmin = (size - s) / r;
      vec3 boxmax = (-size - s) / r;
      vec3 box = max(boxmin, boxmax);
      float d = min(box.x, box.y);
      vec3 p = s + r * d;
      vec2 cuv = p.xz;
      vec3 n = vec3(0, sign(box.y), 0);

      if (box.x < box.y) {
        cuv = p.yz;
        cuv.x += 1.0;
        n = vec3(sign(box.x), 0.0, 0.0);
      }

      vec3 p2 = p;
      p2.z += t2 * 3.0;
      cuv.y += t2 * 3.0;
      cuv *= tunnelSize;
      cuv.y += sin(p2.x * 0.3 + time) * surfaceWarp * 0.1;
      vec2 id = floor(cuv);

      float rough = min(1.0, 0.85 + 0.2 * hash2(id + 100.5));
      float tcolor = cuv.y * 0.05 + sin(TIME * colorPulse);
      vec3 baseColor = getPalette(tcolor, int(paletteIndex));
      vec3 addcol = baseColor * 2.0;

      addcol *= smoothstep(0.5 * curve(time + id.y * 0.01 + id.x * 0.03, 0.3 + morph), 0.0, hash2(id));

      float pattern = 0.0;
      int ftype = int(fractalType);
      if (ftype == 0) pattern = sin(p2.x) * sin(p2.z * 0.4);
      else if (ftype == 1) pattern = sin(p2.x * 0.5 + sin(p2.z)) * cos(p2.z * 0.4);
      else if (ftype == 2) pattern = fract(sin(p2.x * p2.z));
      else pattern = step(0.5, fract(sin(p2.x + p2.z)));

      addcol *= step(0.5, pattern);
      addcol += vec3(0.7, 0.5, 1.2) * step(p2.y, -0.9) * max(0.0, curve(time, 0.2) * 2.0 - 1.0) * step(hash2(id + 0.7), 0.2);

      col += addcol * alpha;

      float fre = pow(1.0 - max(0.0, dot(n, r)), 3.0);
      alpha *= fre * 0.9;

      vec3 pure = reflect(r, n);
      r = normalize(hash3(uv + j * 74.524 + i * 35.712) - 0.5);
      float dr = dot(r, n);
      if (dr < 0.0) r = -r;
      r = normalize(mix(r, pure, rough));
      s = p;
    }
  }

  col /= steps;
  col *= 2.0;
  col = smoothstep(0.0, 1.0, col);
  col = pow(col, vec3(0.4545));
  col = adjustBSC(col, brightness, saturation, contrast);
  gl_FragColor = vec4(col, 1.0);
}
