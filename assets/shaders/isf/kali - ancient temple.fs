/*{
  "CATEGORIES": ["Fractal", "Raymarching", "Psychedelic"],
  "DESCRIPTION": "Ancient Temple fractal with float-based palettes and orb/pattern controls",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "morph", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0 },
    { "NAME": "speed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "cameraPitch", "TYPE": "float", "MIN": -0.01, "MAX": 0.01, "DEFAULT": 0.0 },
    { "NAME": "cameraYaw", "TYPE": "float", "MIN": -0.01, "MAX": 0.01, "DEFAULT": 0.0 },
    { "NAME": "cameraRoll", "TYPE": "float", "MIN": -0.01, "MAX": 0.01, "DEFAULT": 0.0 },
    { "NAME": "fractalScale", "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.0 },
    { "NAME": "fractalIterations", "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 14.0 },
    { "NAME": "surfaceShake", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "floorPattern", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0 },
    { "NAME": "orbSize", "TYPE": "float", "MIN": 0.01, "MAX": 0.2, "DEFAULT": 0.017 },
    { "NAME": "orbPattern", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "colorPalette", "TYPE": "float", "MIN": 0.0, "MAX": 6.999, "DEFAULT": 0.0 },
    { "NAME": "contrast", "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

const float detail = 0.00002;
float hitfloor;
float hitrock;
float det;
float tt;

vec3 lightdir = normalize(vec3(0.0, -0.3, -1.0));

mat2 rot;

vec3 rotate(vec3 v, float pitch, float yaw, float roll) {
  float cx = cos(pitch), sx = sin(pitch);
  float cy = cos(yaw), sy = sin(yaw);
  float cz = cos(roll), sz = sin(roll);
  mat3 rx = mat3(1,0,0, 0,cx,-sx, 0,sx,cx);
  mat3 ry = mat3(cy,0,sy, 0,1,0, -sy,0,cy);
  mat3 rz = mat3(cz,-sz,0, sz,cz,0, 0,0,1);
  return rz * ry * rx * v;
}

vec3 getPalette(float index, float t) {
  float i = floor(index);
  if (i == 0.0) return vec3(sin(t * 6.0), cos(t * 3.5), sin(t * 2.2));
  if (i == 1.0) return vec3(0.7 + 0.3 * cos(t * 2.0), 0.5 + 0.5 * sin(t), 1.0);
  if (i == 2.0) return vec3(t * 0.8, sin(t * 3.0), cos(t * 4.0));
  if (i == 3.0) return vec3(fract(sin(t * 20.0)), fract(cos(t * 5.0)), fract(sin(t * 3.0)));
  if (i == 4.0) return vec3(abs(sin(t * 10.0)), abs(sin(t * 2.0)), 0.8 + 0.2 * sin(t));
  if (i == 5.0) return vec3(0.5 + 0.5 * sin(t * 4.0), 0.5 + 0.5 * sin(t * 2.0 + 2.0), 0.5 + 0.5 * sin(t * 3.0 + 4.0));
  if (i == 6.0) return vec3(smoothstep(0.2, 0.8, sin(t * 5.0)), fract(t), sin(t));
  return vec3(t); // fallback
}

float de(vec3 pos) {
  hitfloor = 0.0;
  hitrock = 0.0;
  vec3 p = pos + sin(pos * 10.0 + TIME) * surfaceShake * 0.01;
  p.xz = abs(0.5 - mod(p.xz, 1.0)) + 0.01;
  float DEfactor = 1.0;
  int iters = int(fractalIterations);
  for (int i = 0; i < 20; i++) {
    if (i >= iters) break;
    p = abs(p) - vec3(0.0, 2.0 + morph, 0.0);
    float r2 = dot(p, p);
    float sc = fractalScale / clamp(r2, 0.4, 1.0);
    p *= sc;
    DEfactor *= sc;
    p -= vec3(0.5, 1.0 + morph, 0.5);
  }
  float pattern = sin(pos.x * 5.0 + TIME * 0.5) * floorPattern * 0.2;
  float rr = length(pos + vec3(0.0, -3.03, 1.85 - tt)) - orbSize;
  rr += sin(pos.x * orbPattern + TIME * 0.5) * 0.005;
  float fl = pos.y - 3.013 + pattern;
  float d = min(fl, length(p) / DEfactor - 0.0005);
  d = min(d, -pos.y + 3.9);
  d = min(d, rr);
  if (abs(d - fl) < 0.0001) hitfloor = 1.0;
  if (abs(d - rr) < 0.0001) hitrock = 1.0;
  return d;
}

vec3 normal(vec3 p) {
  vec3 e = vec3(0.0, det, 0.0);
  return normalize(vec3(
    de(p + e.yxx) - de(p - e.yxx),
    de(p + e.xyx) - de(p - e.xyx),
    de(p + e.xxy) - de(p - e.xxy)
  ));
}

float shadow(vec3 pos, vec3 sdir) {
  float totalDist = 2.0 * det, sh = 1.0;
  for (int steps = 0; steps < 30; steps++) {
    if (totalDist < 1.0) {
      vec3 p = pos - totalDist * sdir;
      float dist = de(p) * 1.5;
      if (dist < detail) sh = 0.0;
      totalDist += max(0.05, dist);
    }
  }
  return max(0.0, sh);
}

float calcAO(const vec3 pos, const vec3 nor) {
  float aodet = detail * 80.0;
  float totao = 0.0;
  float sca = 10.0;
  for (int aoi = 0; aoi < 5; aoi++) {
    float hr = aodet + aodet * float(aoi * aoi);
    vec3 aopos = nor * hr + pos;
    float dd = de(aopos);
    totao += -(dd - hr) * sca;
    sca *= 0.75;
  }
  return clamp(1.0 - 5.0 * totao, 0.0, 1.0);
}

float kset(vec3 p) {
  p = abs(0.5 - fract(p * 20.0));
  float es = 0.0, l = 0.0;
  for (int i = 0; i < 13; i++) {
    float pl = l;
    l = length(p);
    p = abs(p) / dot(p, p) - 0.5;
    es += exp(-1.0 / abs(l - pl));
  }
  return es;
}

vec3 light(in vec3 p, in vec3 dir) {
  vec3 n = normal(p);
  float sh = clamp(shadow(p, lightdir) + hitfloor + hitrock, 0.4, 1.0);
  float ao = calcAO(p, n);
  float diff = max(0.0, dot(lightdir, -n)) * sh * 1.3;
  float amb = max(0.2, dot(dir, -n)) * 0.4;
  vec3 r = reflect(lightdir, n);
  float spec = pow(max(0.0, dot(dir, -r)) * sh, 10.0) * (0.5 + ao * 0.5);
  float k = kset(p) * 0.18;
  vec3 base = getPalette(colorPalette, k);
  if (hitrock > 0.0) base = vec3(0.9, 0.8, 0.7) * (1.0 + kset(p * 2.0) * 0.3);
  return base * ao * (amb * vec3(0.9, 0.85, 1.0) + diff * vec3(1.0, 0.9, 0.9)) + spec * vec3(1, 0.9, 0.5) * 0.7;
}

vec3 raymarch(in vec3 from, in vec3 dir) {
  float t = TIME * speed;
  float cc = cos(t * 0.03), ss = sin(t * 0.03);
  rot = mat2(cc, ss, -ss, cc);
  float glow = 0.0, d = 1.0, totdist = 0.0;
  vec3 p, col = vec3(0.0);
  for (int i = 0; i < 130; i++) {
    if (d > det && totdist < 3.5 * zoom) {
      p = from + totdist * dir;
      d = de(p);
      det = detail * (1.0 + totdist * 55.0);
      totdist += d;
      glow += max(0.0, 0.02 - d) * exp(-totdist);
    }
  }
  float l = pow(max(0.0, dot(normalize(-dir), normalize(lightdir))), 10.0);
  vec3 backg = getPalette(colorPalette, mod(t * 0.1, 1.0)) * (2.0 - l) + vec3(0.8, 0.6, 0.4) * l * 0.4;
  if (d < det) {
    col = light(p - det * dir * 1.5, dir);
    col *= min(1.2, 0.5 + totdist * totdist * 1.5);
    col = mix(col, backg, 1.0 - exp(-1.3 * pow(totdist, 1.3)));
  } else {
    col = backg;
  }
  col += glow * vec3(1.0, 0.9, 0.8) * 0.34;
  col += vec3(1.0, 0.8, 0.6) * pow(l, 3.0) * 0.5;
  return col;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2.0 - 1.0;
  uv.y *= RENDERSIZE.y / RENDERSIZE.x;
  tt = TIME * 0.05 * speed;
  vec3 from = vec3(0.0, 3.04, -2.0 + TIME * 0.05 * speed);
  vec3 dir = normalize(vec3(uv * 0.85 * zoom, 1.0));
  dir = rotate(dir, cameraPitch, cameraYaw, cameraRoll);
  from = rotate(from, cameraPitch, cameraYaw, cameraRoll);
  vec3 color = raymarch(from, dir);
  color = mix(vec3(length(color)), color, saturation);
  color = mix(vec3(0.5), color, contrast);
  color *= brightness;
  gl_FragColor = vec4(color, 1.0);
}
