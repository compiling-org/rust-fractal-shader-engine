/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "cameraShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "fractalMorph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractalType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0  },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 0,  "MIN": 0.0, "MAX": 6.0, "VALUES": [0,1,2,3,4,5,6], "LABELS": ["Psy1","Psy2","Psy3","Psy4","Psy5","Psy6","Psy7"] },
    { "NAME": "feedback", "TYPE": "image" }
  ]
}*/

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 getPalette(int id, float t) {
  if (id == 0) return vec3(sin(t*3.), cos(t*1.3), sin(t*2.1));
  if (id == 1) return vec3(cos(t*1.7), sin(t*1.2), cos(t*3.3));
  if (id == 2) return vec3(sin(t), sin(t*1.5), cos(t*2.));
  if (id == 3) return vec3(cos(t*2.3), cos(t*.7), sin(t*1.1));
  if (id == 4) return vec3(sin(t*2.7), sin(t*.9), cos(t*1.4));
  if (id == 5) return vec3(sin(t*0.9), cos(t*0.8), sin(t*2.4));
  return vec3(cos(t*1.1), sin(t*2.3), cos(t*1.8));
}

vec4 disk(vec2 p, float w, float time) {
  float t = time + w * 6.5;
  p *= zoom + dot(p, p) * 2.;

  float s = sin(t * 0.1 * 1.585 * 3.1416);
  p.x += s * s * s;
  p *= rot(tan(t * 3.1416 * 1.585 * .2) * .1);
  p *= 7. - atan(5. * cos(t * 2. * 3.1416 * 1.585 * .25) * 3.1416 * 1.585) * 1.5;

  float m = 1000., it = 0., cir = smoothstep(7.5, 7., length(p));

  for (int i = 0; i < 4; i++) {
    p *= rot(radians(45.));
    p = abs(p * .9) - .5;
  }

  for (float i = 0.; i < 7.; i++) {
    p.x = abs(p.x) - fract(floor(t * 1.585) * .1 + fractalMorph);
    p = p / dot(p, p);
    float l = abs(p.x) + abs(.5 - fract(p.y * .2 + .25 * t * 1.585 + i * .2)) * .5;
    if (l < m) {
      m = l;
      it = i;
    }
  }

  m = .2 / (.2 + m * m * 15.);
  vec3 col = vec3(m, m * m, m * m * m) * colorPulse;

  col *= getPalette(int(palette), it * .3 + t * .1);
  col.xz *= rot(it * .3);
  return vec4(abs(col) * cir, cir) + smoothstep(1., .5, cir) * cir;
}

vec3 applyVFX(vec3 col) {
  col = (col - 0.5) * contrast + 0.5;
  float avg = (col.r + col.g + col.b) / 3.;
  col = mix(vec3(avg), col, saturation);
  col *= brightness;
  return col;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 norm = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

  float t = TIME * speed;

  norm += sin(vec2(t * 13.1, t * 11.5)) * shakeAmount;

  vec4 f1 = disk(norm + cameraShift, 0.0, t);
  vec4 f2 = disk(norm + cameraShift, 1.0, t);

  vec3 col = mix(f2.rgb, f1.rgb, f1.a);
  col += smoothstep(.4, .5, abs(.5 - fract(norm.y * 50.))) * (1. - max(f1.a, f2.a)) * .3;

  vec3 feedbackCol = vec3(0.);
  float s = 1.;

  for (float i = 0.; i < 100.; i++) {
    s *= 0.99;
    vec2 tuv = (uv - 0.5) * s + 0.5;
    vec3 texCol = IMG_PIXEL(feedback, tuv).rgb;
    feedbackCol += texCol * exp(-.05 * i) * .07 * step(1., length(texCol));
  }

  feedbackCol += IMG_PIXEL(feedback, uv).rgb * .5;
  col += feedbackCol;

  col = applyVFX(col);
  gl_FragColor = vec4(col, 1.0);
}
