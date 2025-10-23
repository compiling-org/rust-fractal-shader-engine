/*{
  "DESCRIPTION": "Mandelbox Tunnel with palettes, full controls, and texture beautification",
  "CATEGORIES": ["Fractal", "Tunnel", "Raymarch", "Texture", "Color"],
  "INPUTS": [
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "TunnelDepth", "TYPE": "float", "DEFAULT": 5.5, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "ViewAngle", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "TunnelSpin", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "ZShift", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 20.0 },

    { "NAME": "MandelboxScale", "TYPE": "float", "DEFAULT": 2.8, "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 30.0, "MIN": 1.0, "MAX": 50.0 },
    { "NAME": "FractalModulation", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.01, "MAX": 2.0 },
    { "NAME": "FractalWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "StepCount", "TYPE": "float", "DEFAULT": 50.0, "MIN": 1.0, "MAX": 200.0 },

    { "NAME": "LightX", "TYPE": "float", "DEFAULT": 0.2, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "LightY", "TYPE": "float", "DEFAULT": 1.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "LightZ", "TYPE": "float", "DEFAULT": -0.2, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "SpecularPower", "TYPE": "float", "DEFAULT": 16.0, "MIN": 1.0, "MAX": 128.0 },
    { "NAME": "DiffusePower", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "AmbientPower", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "ColorMode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "HueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "Sat", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Val", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "GradientBoost", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "ColorOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "ColorIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },

    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Gamma", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "ToneMapBoost", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },

    { "NAME": "UseTexture", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "TexScale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "TexMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "TexRot", "TYPE": "float", "DEFAULT": 0.0, "MIN": -6.28, "MAX": 6.28 },

    { "NAME": "image", "TYPE": "image" }
  ]
}*/

mat2 rot2(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

vec3 palette(float t) {
  t = fract(t + HueShift);
  int p = int(floor(ColorMode + 0.5));
  if (p == 0) return vec3(t, t*t, 1.0 - t);                         // Electric
  if (p == 1) return vec3(sin(3.14*t), cos(3.14*t), sin(2.0*3.14*t));     // Rainbow
  if (p == 2) return vec3(t, 1.0 - t, sin(3.14*t));                   // Trippy fire
  if (p == 3) return vec3(pow(t, 0.5), t*t, sin(t*10.));            // UV nebula
  if (p == 4) return vec3(1.0 - t*t, t, t*t);                       // Pastel
  if (p == 5) return vec3(fract(3.*t), fract(5.*t), fract(7.*t));   // Random RGB
  return vec3(t);                                                  // fallback grayscale
}

float mandelbox(vec3 p) {
  vec4 q = vec4(p, 1.0);
  vec4 q0 = q;
  for (int i = 0; i < 100; i++) {
    if (float(i) >= FractalIterations) break;
    q.xyz = clamp(q.xyz, -1., 1.) * 2. - q.xyz;
    q *= MandelboxScale / clamp(dot(q.xyz, q.xyz), FractalModulation, 1.0);
    q += q0 * FractalWarp;
  }
  return length(q.xyz) / abs(q.w);
}

float capsule(vec3 p) {
  return length(p - vec3(0.0, 0.0, 0.5 * clamp(p.z / 0.5, -1.0, 1.0))) - 0.2;
}

float scene(vec3 p) {
  return max(mandelbox(p), -capsule(p - vec3(0, 0, -TunnelDepth + mod(ZShift * TIME * Speed + 1.0, 9.5))));
}

float trace(vec3 ro, vec3 rd) {
  float t = 0.0;
  for (int i = 0; i < 200; i++) {
    if (float(i) >= StepCount) break;
    float d = scene(ro + rd * t);
    if (d < 0.001 || t > 30.0) break;
    t += d;
  }
  return t;
}

vec3 getNormal(vec3 p) {
  float e = 0.001;
  return normalize(vec3(
    scene(p + vec3(e, 0, 0)) - scene(p - vec3(e, 0, 0)),
    scene(p + vec3(0, e, 0)) - scene(p - vec3(0, e, 0)),
    scene(p + vec3(0, 0, e)) - scene(p - vec3(0, 0, e))
  ));
}

void main() {
  vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  float t = TIME * Speed;
  vec3 ro = vec3(0.0, 0.0, -TunnelDepth + mod(ZShift * t + 1.0, 9.5));
  vec3 rd = normalize(vec3(uv * ViewAngle, Zoom));
  rd.xy = rot2(TunnelSpin) * rd.xy;

  float dst = trace(ro, rd);
  vec3 col = vec3(0.0);
  vec3 p = ro + rd * dst;

  if (dst < 30.0) {
    vec3 n = getNormal(p);
    vec3 l = normalize(vec3(LightX, LightY, LightZ));
    float diff = max(0.0, dot(n, l));
    float spec = pow(max(0.0, dot(l, reflect(rd, n))), SpecularPower);
    float hue = mod(length(p) * GradientBoost / 5.0 + ColorOffset, 1.0);
    col = palette(hue);
    col *= AmbientPower + diff * DiffusePower + spec;
    col = pow(col * ColorIntensity, vec3(Gamma)) * ToneMapBoost;

    if (UseTexture > 0.5) {
      vec2 texUV = TexScale * p.xy;
      texUV = rot2(TexRot) * texUV;
      vec3 texColor = IMG_NORM_PIXEL(image, fract(texUV)).rgb;
      col = mix(col, texColor, TexMix);
    }
  }

  col = mix(vec3(0.5), col, Saturation);
  col = (col - 0.5) * Contrast + 0.5;
  col *= Brightness;
  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
