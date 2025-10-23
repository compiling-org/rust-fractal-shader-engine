/*{
  "DESCRIPTION": "True 3D fractal with surface + glow, animated by speed params (not fake time input)",
  "CATEGORIES": ["Fractal", "3D", "Psychedelic"],
  "INPUTS": [
    { "NAME": "zoom",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "iterations",   "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 40.0 },
    { "NAME": "formuparam",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "morph",        "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "morphSpeed",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "orbitSpeed",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "lightSpeed",   "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorSpeed",   "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "glowIntensity","TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "brightness",   "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast",     "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation",   "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define MAX_ITERATIONS 40
#define PI 3.14159265359
#define TAU 6.2831853

vec3 rotateY(vec3 p, float a) {
  float c = cos(a), s = sin(a);
  return vec3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}

vec3 palette(float t) {
  return 0.5 + 0.5 * cos(TAU * (vec3(0.0, 0.15, 0.33) + t));
}

float DE(vec3 p, float morphVal) {
  vec3 z = p;
  float scale = 1.0;
  for (int i = 0; i < MAX_ITERATIONS; ++i) {
    if (i >= int(iterations)) break;
    z = abs(z) / clamp(dot(z, z), 0.2, 1.5) - formuparam;
    z = rotateY(z, morphVal + float(i) * 0.015);
    scale *= 1.2;
  }
  return length(z) / scale;
}

vec3 estimateNormal(vec3 p, float morphVal) {
  float eps = 0.001;
  return normalize(vec3(
    DE(p + vec3(eps, 0, 0), morphVal) - DE(p - vec3(eps, 0, 0), morphVal),
    DE(p + vec3(0, eps, 0), morphVal) - DE(p - vec3(0, eps, 0), morphVal),
    DE(p + vec3(0, 0, eps), morphVal) - DE(p - vec3(0, 0, eps), morphVal)
  ));
}

vec3 colorGrade(vec3 col) {
  float avg = dot(col, vec3(0.333));
  col = mix(vec3(avg), col, saturation);
  return pow(col, vec3(contrast)) * brightness;
}

vec3 shade(vec3 pos, vec3 n, vec3 ro, vec3 rd, float hueOffset, float tLight) {
  vec3 lightDir = normalize(vec3(sin(tLight), 1.0, cos(tLight)));
  float diff = max(dot(n, lightDir), 0.0);
  float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 24.0);
  vec3 base = palette(hueOffset);
  return colorGrade(base * diff + spec);
}

vec3 getCameraRay(vec2 uv, out vec3 ro, float tOrbit) {
  uv = uv * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  ro = vec3(sin(tOrbit) * 3.5, 1.2, cos(tOrbit) * 3.5);
  vec3 f = normalize(-ro);
  vec3 r = normalize(cross(vec3(0, 1, 0), f));
  vec3 u = cross(f, r);
  return normalize(uv.x * r + uv.y * u + zoom * f);
}

vec3 raymarch(vec3 ro, vec3 rd, float morphVal, float hueT, float lightT) {
  float t = 0.0;
  vec3 col = vec3(0.0);

  for (int i = 0; i < 128; ++i) {
    vec3 p = ro + rd * t;
    float d = DE(p, morphVal);
    float glow = exp(-d * 20.0) * glowIntensity;
    col += palette(hueT + t * 0.03) * glow * 0.05;

    if (d < 0.002) {
      vec3 n = estimateNormal(p, morphVal);
      return mix(col, shade(p, n, ro, rd, hueT, lightT), 0.75);
    }

    t += d;
    if (t > 15.0) break;
  }

  return colorGrade(col);
}

void main() {
  float t = TIME;
  float morphVal = morph + sin(t * morphSpeed) * 0.5;
  float tOrbit = t * orbitSpeed;
  float tLight = t * lightSpeed;
  float tHue = t * colorSpeed;

  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec3 ro;
  vec3 rd = getCameraRay(uv, ro, tOrbit);
  vec3 col = raymarch(ro, rd, morphVal, tHue, tLight);
  gl_FragColor = vec4(col, 1.0);
}
