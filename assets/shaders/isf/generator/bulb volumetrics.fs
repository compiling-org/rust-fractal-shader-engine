/*{
  "DESCRIPTION": "Hybrid Psychedelic Mandelbulb with morph blending and palette shift",
  "CATEGORIES": ["Fractal", "Morphing", "Psychedelic"],
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME": "FractalPower", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 12.0},
    {"NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "SphereFoldScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "TorusTwist", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.5},
    {"NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0},
    {"NAME": "PulseRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
    {"NAME": "PalettePhase", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0},
    {"NAME": "RotateSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 4.0}
  ]
}*/

mat3 rotx(float a) {
  float c = cos(a), s = sin(a);
  return mat3(1,0,0, 0,c,-s, 0,s,c);
}
mat3 roty(float a) {
  float c = cos(a), s = sin(a);
  return mat3(c,0,s, 0,1,0, -s,0,c);
}

float hybridFractal(vec3 p, float time, float power, float morph, float fold, float twist) {
  vec3 z = p;
  float dr = 1.0;
  float r = 0.0;

  for (int i = 0; i < 8; i++) {
    r = length(z);
    if (r > 2.0) break;

    // Mandelbulb
    float theta = acos(z.z / r);
    float phi = atan(z.y, z.x);
    float zr = pow(r, power);
    theta *= power;
    phi *= power;
    vec3 bulb = zr * vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));

    // Sphere fold
    vec3 s = z;
    float m = dot(s, s);
    s *= clamp(1.0 / m, 0.0, 1.0) * fold;

    // Torus twist
    float angle = atan(z.y, z.x) + twist * sin(time + r);
    float radius = length(z.xy);
    vec3 t = vec3(cos(angle) * radius, sin(angle) * radius, z.z);

    // Blend all
    z = mix(bulb + p, mix(s, t, 0.5), morph);
    dr = pow(r, power - 1.0) * power * dr + 1.0;
  }

  return 0.5 * log(r) * r / dr;
}

vec3 getNormal(vec3 p, float time, float power, float morph, float fold, float twist) {
  float eps = 0.002;
  vec2 e = vec2(1.0, -1.0) * eps;
  return normalize(
    e.xyy * hybridFractal(p + e.xyy, time, power, morph, fold, twist) +
    e.yyx * hybridFractal(p + e.yyx, time, power, morph, fold, twist) +
    e.yxy * hybridFractal(p + e.yxy, time, power, morph, fold, twist) +
    e.xxx * hybridFractal(p + e.xxx, time, power, morph, fold, twist)
  );
}

vec3 palette(float t, float shift) {
  return 0.6 + 0.4 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.67) + shift));
}

void main() {
  vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  vec3 ro = vec3(0.0, 0.0, -Zoom);
  vec3 rd = normalize(vec3(uv, 1.2));
  mat3 rot = rotx(0.4) * roty(TIME * RotateSpeed);
  ro *= rot;
  rd *= rot;

  float t = 0.0;
  vec3 p;
  float d;
  for (int i = 0; i < 128; i++) {
    p = ro + t * rd;
    d = hybridFractal(p, TIME, FractalPower, MorphAmount, SphereFoldScale, TorusTwist);
    if (d < 0.001 || t > 10.0) break;
    t += d * 0.9;
  }

  vec3 col = vec3(0.0);
  if (d < 0.01) {
    vec3 n = getNormal(p, TIME, FractalPower, MorphAmount, SphereFoldScale, TorusTwist);
    vec3 lightDir = normalize(vec3(0.3, 0.6, -1.0));
    float diff = max(dot(n, lightDir), 0.0);

    float pulse = sin(TIME * PulseRate) * 0.5 + 0.5;
    float m = dot(p, p);
    vec3 baseColor = palette(m * 0.2 + p.z + TIME * ColorPulse, PalettePhase);
    col = baseColor * diff * (1.0 + pulse * 1.5);
  }

  col *= Brightness;
  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
