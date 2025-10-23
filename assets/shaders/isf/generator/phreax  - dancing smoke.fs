/*{
  "CATEGORIES": ["Raymarching", "Psychedelic", "Feedback"],
  "DESCRIPTION": "Dancing smoke fractal with morphing and feedback.",
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    },
    {
    }
  ],
  "INPUTS": [
    {
      "NAME": "Speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 5.0
    },
    {
      "NAME": "Zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.5,
      "MAX": 2.0
    },
    {
      "NAME": "Morph",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.01,
      "MAX": 1.3
    },
    {
      "NAME": "ColorPulse",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 5.0
    },
    {
      "NAME": "Palette",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 0.0,
      "MAX": 6.0
    }
  ]
}*/

#define PI 3.141592
#define SIN(x) (sin(x)*0.5+0.5)

mat2 rot2(float a) {
  return mat2(cos(a), sin(a), -sin(a), cos(a));
}

vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
  return a + b * cos(6.28318 * (c * t + d));
}

vec3 getPal(int id, float t) {
  int i = int(mod(float(id), 7.0));
  if (i == 0) return pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.0, -0.33, 0.33));
  if (i == 1) return pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.0, 0.10, 0.20));
  if (i == 2) return pal(t, vec3(.5), vec3(.5), vec3(1.0), vec3(0.3, 0.20, 0.20));
  if (i == 3) return pal(t, vec3(.5), vec3(.5), vec3(1.0, 1.0, 0.5), vec3(0.8, 0.90, 0.30));
  if (i == 4) return pal(t, vec3(.5), vec3(.5), vec3(1.0, 0.7, 0.4), vec3(0.0, 0.15, 0.20));
  if (i == 5) return pal(t, vec3(.5), vec3(.5), vec3(2.0, 1.0, 0.0), vec3(0.5, 0.20, 0.25));
  return pal(t, vec3(0.8, 0.5, 0.4), vec3(0.2, 0.4, 0.2), vec3(2.0), vec3(0.0, 0.25, 0.25));
}

float field(vec3 p, float time, float morph) {
  for (int i = 0; i < 4; i++) {
    float s = float(i) * 0.1 + morph;
    p = abs(p) / dot(p, p) - s;
    p.xy *= rot2(time * 0.1 + float(i));
    p.zy *= rot2(time * 0.15 + float(i));
  }
  return length(p.xy) - 0.5;
}

void main() {
  if (PASSINDEX == 0) {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 p = uv * Zoom;

    float t = TIME * Speed;
    vec3 ray = normalize(vec3(p, 1.0));
    vec3 cam = vec3(0.0, 0.0, -2.0);
    vec3 pos = cam;
    float d = 0.0;
    vec3 col = vec3(0.0);

    for (int i = 0; i < 64; i++) {
      vec3 current = pos + ray * d;
      float f = field(current, t, Morph);
      d += max(abs(f), 0.01);
      float fade = exp(-float(i) * 0.05);
      float brightness = 0.005 / (0.01 + f * f);
      vec3 shade = getPal(int(Palette), current.z + t * 0.2);
      col += brightness * fade * shade;
      if (d > 4.0) break;
    }

    // Color pulse effect
    col *= 1.0 + 0.5 * sin(TIME * ColorPulse);

    vec3 old = IMG_NORM_PIXEL(BufferA, fract(gl_FragCoord.xy / RENDERSIZE.xy)).rgb;
    col = mix(col, old, 0.6);
    gl_FragColor = vec4(col, 1.0);
  } else {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = IMG_NORM_PIXEL(BufferA, fract(uv)).rgb;
    gl_FragColor = vec4(col, 1.0);
  }
}
