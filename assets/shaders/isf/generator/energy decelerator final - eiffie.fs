/*{
  "DESCRIPTION": "Energy Decelerator - Full ISF Extended",
  "CREDIT": "Original by eiffie, ISF tunables by Shader Genius",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "pixelSizeBoost", "TYPE": "float", "DEFAULT": 2.25, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "glowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "animationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "fractalMorph", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "paletteShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -6.283, "MAX": 6.283 },
    { "NAME": "paletteSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorMode", "TYPE": "long", "DEFAULT": 0, "MIN": 0, "MAX": 3 },
    { "NAME": "camShake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "lightSaturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "contrastBoost", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0 }
  ]
}*/

bool bColoring = false;
vec3 mcol;
vec2 fragCoord;

const vec4 scale = vec4(-3.12, -3.12, -3.12, 3.12);
const vec3 light_col = vec3(1.0, 0.7, 0.4);

float hash(float n) { return fract(sin(n) * 43758.5453); }
float hash(vec2 n) { return fract(sin(dot(n * 0.123, vec2(78.233, 113.16))) * 43758.351); }

float noise(float p) {
  float c = floor(p), h1 = hash(c);
  return h1 + (hash(c + 1.0) - h1) * fract(p);
}
float noise(vec2 p) {
  vec2 c = floor(p), f = fract(p), v = vec2(1.0, 0.0);
  float h1 = hash(c), h2 = hash(c + v), h3 = hash(c + v.yx), h4 = hash(c + v.xx);
  h1 += (h2 - h1) * f.x;
  h3 += (h4 - h3) * f.x;
  return h1 + (h3 - h1) * f.y;
}

float rndStart(vec2 co) {
  return 0.5 + 0.5 * fract(sin(dot(co, vec2(123.42, 117.853))) * 412.453);
}

mat3 lookat(vec3 fw, vec3 up) {
  fw = normalize(fw);
  vec3 rt = normalize(cross(fw, up));
  return mat3(rt, cross(rt, fw), fw);
}

vec3 applyPalette(vec3 col, int mode) {
  if (mode == 1) return col * vec3(0.5, 1.2, 2.0);
  else if (mode == 2) return col * vec3(1.3, 1.0, 0.8);
  else if (mode == 3) return col.bgr * vec3(1.4, 0.8, 2.0);
  return col;
}

vec2 DE(in vec3 z0) {
  vec4 z = vec4(z0, 1.0);
  float morph = 1.19 + sin(TIME * animationSpeed * 3.0 + sign(z0.x + 0.54) + 2.0 * sign(z0.z - 0.47)) * 0.25 + fractalMorph;
  vec4 p0 = vec4(1.0, morph, -1.0, 0.0);
  float dL;
  for (int n = 0; n < 3; n++) {
    z.xyz = clamp(z.xyz, -0.94, 0.94) * 2.0 - z.xyz;
    z *= scale / clamp(dot(z.xyz, z.xyz), 0.25, 1.0);
    if (n == 0) dL = max(0.0, (length(z.xyz + vec3(0.0, 5.8, 2.2)) - 0.6) / z.w);
    z += p0;
  }
  if (bColoring) mcol += z.xyz;
  z.y += 3.0;
  float dS = (length(max(abs(z.xyz) - vec3(1.2, 49.0, 1.4), 0.0)) - 0.06) / z.w;
  return vec2(dS, dL);
}

float ShadAO(vec3 ro, vec3 rd, float px, float dist) {
  float res = 1.0, d, t = 4.0 * px * rndStart(fragCoord.xy);
  for (int i = 0; i < 12; i++) {
    d = max(0.0, DE(ro + rd * t).x) + 0.01;
    if (t + d > dist) break;
    res = min(res, 2.0 * d / t);
    t += d;
  }
  return res;
}

vec3 Light(vec3 so, vec3 rd, float px, float dist) {
  so += rd * (dist - px);
  bColoring = true;
  mcol = vec3(0.0);
  vec2 d = DE(so);
  vec2 v = vec2(px, 0.0);
  vec3 dn = vec3(DE(so - v.xyy).x, DE(so - v.yxy).x, DE(so - v.yyx).x);
  vec3 dp = vec3(DE(so + v.xyy).x, DE(so + v.yxy).x, DE(so + v.yyx).x);
  vec3 norm = (dp - dn) / (length(dp - vec3(d.x)) + length(vec3(d.x) - dn));
  bColoring = false;
  mcol = vec3(0.9) + sin(mcol + paletteShift + TIME * paletteSpeed) * 0.1;
  v = vec2(d.y, 0.0);
  vec3 light_dir = -normalize(vec3(-d.y) + vec3(DE(so + v.xyy).y, DE(so + v.yxy).y - d.y, DE(so + v.yyx).y));
  float shad = ShadAO(so, light_dir, px, d.y * 0.5);
  float dif = dot(norm, light_dir) * 0.5 + 0.5;
  float spec = dot(light_dir, reflect(rd, norm));
  vec3 diffuse_col = mcol + vec3(0.12, 0.05, -0.125) * spec;
  dif = min(dif, shad);
  spec = min(max(0.0, spec), shad);
  vec3 col = diffuse_col * dif + light_col * spec * lightSaturation;
  col *= exp(-d.y);
  return col * clamp(abs(so.y - 1.0) * 5.0, 0.0, 1.0);
}

void mainImage(out vec4 fragColor, in vec2 iFragCoord) {
  fragCoord = iFragCoord;
  float tim = TIME * animationSpeed;
  float px = pixelSizeBoost / (RENDERSIZE.y * zoom);

  vec2 uv = (2.0 * fragCoord - RENDERSIZE.xy) / RENDERSIZE.y;
  uv += glitchAmount * vec2(noise(uv * 10.0 + tim), noise(uv * 15.0 - tim)) * 0.05;

  float tm = abs(mod(tim, 60.0) - 30.0) / 30.0;
  vec3 ro = vec3(cos(tim * 0.17), 0.0, sin(tim * 0.05));
  ro.z = 1.0 + ro.z * abs(ro.z);
  ro.xz *= vec2(1.0 + tim * 0.0025, 1.2) - vec2(tm * tm * 3.5); // Less zoom-out
  ro.x = -0.64 + ro.x / (1.0 + ro.z * ro.z * 0.1);
  ro += vec3(sin(tim * 9.0), 0.0, cos(tim * 7.0)) * camShake * 0.02;

  vec3 rd = normalize(vec3(uv, zoom));
  rd = lookat(vec3(sin(tim * 0.6), sin(tim * 0.4), -0.5) - ro, vec3(0.01, 0.99, 0.02)) * rd;

  float t = DE(ro).x * rndStart(fragCoord.xy), tt = t, dm = 100.0, od = 1000.0, de = 0.0, te = 0.0;
  float ft = (sign(rd.y) - ro.y) / rd.y, ref = 1.0, dR = clamp(DE(ro + rd * ft).x * 15.0, 0.0, 1.0);
  float maxT = min((sign(rd.x) * 4.0 - ro.x) / rd.x, (sign(rd.z) * 4.0 - ro.z) / rd.z);
  float liteGlow = 0.0, mask = 1.0;
  vec2 d;

  for (int i = 0; i < 64; i++) {
    d = DE(ro + rd * t) * 0.95;
    liteGlow += mask / (1.0 + 1000.0 * d.y * d.y);
    t += d.x; tt += d.x;
    if (t > ft) {
      ro += rd * ft; t -= ft;
      if (tt - t < maxT) {
        vec2 p = mod(2.0 * vec2(ro.x + ro.z, ro.x - ro.z), 2.0) - 1.0;
        float tile = sign(p.x * p.y);
        p = abs(fract(p) - 0.5);
        mask = max(0.0, mask - pow(2.0 * max(p.x, p.y), 10.0));
        ref *= 0.75;
        if (tile > 0.0) {
          rd.y = -rd.y;
          rd.xz += fract(rd.zx * 1252.1123) * 0.006;
          ft = (sign(rd.y) - ro.y) / rd.y;
        } else { tt += 1000.0; break; }
      } else {
        t = maxT - tt + t; ro += rd * t; break;
      }
    } else if (d.x > od && te == 0.0) {
      if (od < px * tt) { de = od; te = tt - d.x - od; }
    }
    if (d.x < dm) { dm = d.x; }
    od = d.x;
    if (tt > maxT) { t -= tt - maxT; ro += rd * t; break; }
    if (d.x < 0.00001) break;
  }

  vec3 col = vec3(0.0);
  if (tt < 1000.0 && tt >= maxT) {
    vec3 r2 = ro;
    if (abs(r2.z) > abs(r2.x)) {
      r2.xz = r2.zx;
      od = max(abs(r2.z + 1.0) - 0.3, abs(r2.y * 8.0 + 1.9) - 5.8);
    } else {
      od = max(abs(r2.z - 1.0) - 0.5, abs(r2.y * 4.0) - 1.0);
    }
    float d1 = noise(r2.yz * 70.0);
    r2.y *= 4.0;
    float d2 = pow(1.0 - clamp(abs(sin(tim * 10.0 + r2.z * 150.0 * sin(tim)) + r2.y * 1.2), 0.0, 1.0), 10.0);
    r2.y += 0.5;
    r2.z += floor(mod(r2.y + 0.5, 2.0)) * 0.25;
    col = vec3(0.2, 0.15, 0.1) * (1.0 - 0.5 * exp(-200.0 * abs((fract(r2.z * 2.0) - 0.5) * (fract(r2.y) - 0.5))));
    col -= d1 * vec3(0.1, 0.05, 0.0);
    col = mix(vec3(0.5 + 0.5 * rd.x, d2, 1.0) * clamp(abs(od * 2.0), 0.0, 0.5), col, clamp(od * 10.0, 0.0, 1.0));
  } else if (tt > 1000.0) {
    tt -= 1000.0;
    col = vec3(0.3);
    dR = min(dR, 4.3 - max(abs(ro.x), abs(ro.z)));
  }

  od = noise(tim * 5.0 + rd.x * rd.z);
  float tL = clamp(od, 0.4, 0.5) * 2.0;
  if (dm < px * tt) col = mix(Light(ro + rd * tt, rd, px * tt, dm) * tL, col, clamp(dm / (px * tt), 0.0, 1.0));
  if (de < px * te && te < tt) col = mix(Light(ro + rd * te, rd, px * te, de) * tL, col, clamp(de / (px * te), 0.0, 1.0));
  if (ref < 1.0) {
    col = pow(col, vec3(ref));
    col = mix(vec3(0.4 - 0.2 * ref), col, mask);
    col *= dR;
  }

  col += light_col * liteGlow * clamp(od, 0.05, 0.5) * ref;
  tt = min(tt, maxT);
  col = 3.0 * col * exp(-tt * 0.22);
  col *= 1.0 + colorPulse * sin(tim * 5.0);
  col = pow(col, vec3(contrastBoost));
  col *= glowIntensity;
  col = applyPalette(col, colorMode);

  fragColor = vec4(col, 1.0);
}

void main() {
  vec4 fragColor;
  mainImage(fragColor, gl_FragCoord.xy);
  gl_FragColor = fragColor;
}
