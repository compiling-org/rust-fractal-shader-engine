/*{
  "CATEGORIES": [
    "Psychedelic",
    "Particles"
  ],
  "DESCRIPTION": "Corrected color palette switching for psychedelic particle shader.",
  "INPUTS": [
    {
      "NAME": "ParticleSpinSpeed",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.0,
      "MAX": 2.0,
      "LABEL": "Particle Spin Speed"
    },
    {
      "NAME": "ShapeWarpAmount",
      "TYPE": "float",
      "DEFAULT": 2.8,
      "MIN": 0.1,
      "MAX": 5.0,
      "LABEL": "Shape Warp Amount"
    },
    {
      "NAME": "GlowIntensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Glow Intensity"
    },
    {
      "NAME": "ColorRotationSpeed",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 2.0,
      "LABEL": "Color Spin Speed"
    },
    {
      "NAME": "ColorScheme",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Color Scheme",
      "VALUES": ["Firestorm", "Aurora", "Plasma", "Rainbow"]
    }
  ]
}
*/

float it = 0.0;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float hash(vec2 p) {
  vec3 p3 = fract(vec3(p.xyx) * 0.1031);
  p3 += dot(p3, p3.yzx + 33.33);
  return fract((p3.x + p3.y) * p3.z);
}

float de(vec3 p) {
  p.yz *= rot(-0.5);
  p.xz *= rot(TIME * ParticleSpinSpeed);
  float d = 100.0;
  p *= 0.2;
  for (float i = 0.0; i < 12.0; i++) {
    p.xy = sin(p.xy * ShapeWarpAmount);
    p.xy *= rot(1.0);
    p.xz *= rot(1.5);
    float l = length(p.xy) + 0.01;
    if (i > 1.0) d = min(d, l);
    if (d == l) it = i;
  }
  return d * 0.3;
}

vec3 getColor(vec3 c, float shift, int mode) {
  float t = TIME * ColorRotationSpeed + shift;
  if (mode == 1) return c.zyx * vec3(sin(t), cos(t), sin(t * 0.5));
  if (mode == 2) return c * vec3(sin(t * 0.3), sin(t), cos(t));
  if (mode == 3) return vec3(sin(t + c.x), sin(t + c.y), sin(t + c.z));
  return c; // mode 0 = original firestorm
}

vec3 march(vec3 from, vec3 dir) {
  float d, td = hash(gl_FragCoord.xy + TIME) * 0.2;
  vec3 p, col = vec3(0.0);
  for (int i = 0; i < 200; i++) {
    p = from + dir * td;
    d = max(0.005, abs(de(p)));
    td += d;
    if (td > 10.0) break;

    vec3 c = vec3(1.0, -0.5, 0.0);
    c.rb *= rot(-it * 0.15 + TIME * ColorRotationSpeed);
    c = normalize(1.0 + c);
    c = getColor(c, td, int(ColorScheme));

    c *= exp(-0.15 * td);
    c *= exp(-0.5 * length(p));
    c /= 1.0 + d * 1500.0;

    c *= (0.3 + abs(pow(abs(fract(length(p) * 0.15 - TIME * 0.2 + it * 0.05) - 0.5) * 2.0, 30.0))) * 4.0;

    col += c * GlowIntensity;
    col += exp(-5.0 * length(p)) * 0.15;
  }
  return col;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
  vec3 from = vec3(0.0, 0.0, -3.0 - cos(TIME * 0.5));
  vec3 dir = normalize(vec3(uv, 1.2));
  vec3 col = march(from, dir);
  gl_FragColor = vec4(col, 1.0);
}
