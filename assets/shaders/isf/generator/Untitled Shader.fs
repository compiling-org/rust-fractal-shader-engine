/*{
  "DESCRIPTION": "Self-animating 3D fractal with surface+volumetric blend, light & palette motion",
  "CATEGORIES": ["Fractal", "3D", "Volumetric", "Psychedelic"],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "iterations", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "formuparam", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.5 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "morphSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "orbitSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "lightSpeed", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "colorSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "volumetricGlow", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.0, "MAX": 3.0 }
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
  return 0.5 + 0.5 * cos(TAU * (vec3(0.0, 0.33, 0.67) + t));
}

float fractalDE(vec3 p, float morphVal, float fractalType) {
  vec3 z = p;
  float scale = 1.0;

  for (int i = 0; i < MAX_ITERATIONS; ++i) {
    if (i >= int(iterations)) break;

    if (fractalType < 1.0) {
      z = abs(z) / clamp(dot(z, z), 0.4, 1.8) - formuparam;
    } else if (fractalType < 2.0) {
      z = abs(z) - vec3(1.0 + 0.5 * morphVal, 1.3, 0.7 + morphVal);
    } else {
      z = fract(z * 1.5 + morphVal) - 0.5;
    }

    z = rotateY(z, morphVal + float(i) * 0.02);
    scale *= 1.35;
  }

  return length(z) / scale;
}

vec3 estimateNormal(vec3 p, float morphVal, float fractalType) {
  float eps = 0.001;
  return normalize(vec3(
    fractalDE(p + vec3(eps, 0, 0), morphVal, fractalType) - fractalDE(p - vec3(eps, 0, 0), morphVal, fractalType),
    fractalDE(p + vec3(0, eps, 0), morphVal, fractalType) - fractalDE(p - vec3(0, eps, 0), morphVal, fractalType),
    fractalDE(p + vec3(0, 0, eps), morphVal, fractalType) - fractalDE(p - vec3(0, 0, eps), morphVal, fractalType)
  ));
}

vec3 colorGrade(vec3 col) {
  float avg = (col.r + col.g + col.b) / 3.0;
  col = mix(vec3(avg), col, saturation);
  return pow(col, vec3(contrast)) * brightness;
}

vec3 shade(vec3 pos, vec3 normal, vec3 ro, vec3 rd, float t, float hueOffset) {
  vec3 lightDir = normalize(vec3(sin(t), 0.7, cos(t)));
  float diff = clamp(dot(normal, lightDir), 0.0, 1.0);
  float spec = pow(clamp(dot(reflect(-lightDir, normal), -rd), 0.0, 1.0), 24.0);
  vec3 hue = palette(hueOffset);
  return colorGrade(hue * diff + vec3(spec));
}

vec3 getCameraRay(vec2 uv, out vec3 ro, float tOrbit) {
  uv = uv * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  float a = tOrbit;
  ro = vec3(sin(a) * 3.5, 0.6, cos(a) * 3.5);
  vec3 lookAt = vec3(0.0);
  vec3 f = normalize(lookAt - ro);
  vec3 r = normalize(cross(vec3(0, 1, 0), f));
  vec3 u = cross(f, r);
  return normalize(uv.x * r + uv.y * u + zoom * f);
}

vec3 raymarch(vec3 ro, vec3 rd, float morphVal, float tColor, float tLight, float fractalType) {
  float t = 0.0;
  vec3 col = vec3(0.0);

  for (int i = 0; i < 120; ++i) {
    vec3 p = ro + rd * t;
    float d = fractalDE(p, morphVal, fractalType);

    float glow = exp(-d * 20.0) * volumetricGlow;
    vec3 hue = palette(t * 0.05 + tColor);
    col += hue * glow * 0.05;

    if (d < 0.002) {
      vec3 n = estimateNormal(p, morphVal, fractalType);
      return mix(col, shade(p, n, ro, rd, tLight, tColor), 0.7);
    }

    t += d;
    if (t > 15.0) break;
  }

  return colorGrade(col);
}

void main() {
  float tMorph = TIME * morphSpeed;
  float tOrbit = TIME * orbitSpeed;
  float tLight = TIME * lightSpeed;
  float tColor = TIME * colorSpeed;

  float morphVal = morph + sin(tMorph) * 0.5;

  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec3 ro;
  vec3 rd = getCameraRay(uv, ro, tOrbit);
  vec3 color = raymarch(ro, rd, morphVal, tColor, tLight, fractalType);
  gl_FragColor = vec4(color, 1.0);
}
