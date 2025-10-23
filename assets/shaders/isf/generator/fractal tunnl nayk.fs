/*{
  "DESCRIPTION": "Converted from Shadertoy X3jyzW by nayk, with ISF params and palette control",
  "CATEGORIES": [ "Fractal", "Converted", "Trippy" ],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "geometryType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "xy", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] }
  ]
}*/

#define PI 3.1415926535
#define TAU 6.283185307

// Palette system from Iñigo Quílez
vec3 getPalette(float t, int i) {
  if (i == 0) return vec3(0.5) + 0.5*cos(TAU*(vec3(0.0, 0.33, 0.67)+t));
  if (i == 1) return vec3(0.5) + 0.5*sin(TAU*(vec3(0.3, 0.5, 0.9)+t));
  if (i == 2) return fract(t * vec3(3.14, 1.61, 0.99));
  if (i == 3) return 1.0 - abs(sin(t + vec3(0.2, 1.0, 2.5)));
  if (i == 4) return sin(t * vec3(1.1, 2.3, 3.3)) * 0.5 + 0.5;
  if (i == 5) return vec3(0.25 + 0.75*fract(t*vec3(0.5, 0.7, 0.9)));
  return vec3(0.5) + 0.5*cos(TAU*(vec3(0.1, 0.3, 0.5)+t));
}

vec3 adjustBSC(vec3 color, float brightness, float saturation, float contrast) {
  vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114)));
  color = mix(gray, color, saturation);
  color = (color - 0.5) * contrast + 0.5;
  return color + brightness;
}

#define R(p,a,r) mix(dot(p,a)*a, p, cos(r)) + sin(r)*cross(p,a)

float happy_star(vec2 uv, float anim) {
  uv = abs(uv);
  vec2 pos = min(uv.xy / uv.yx, anim);
  float p = 2.0 - pos.x - pos.y;
  return (2.0 + p*(p*p - 1.5)) / (uv.x + uv.y);      
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
  uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1.0);
  vec2 fragUV = uv;

  vec3 col = vec3(0.0);
  vec3 p;
  vec2 r = RENDERSIZE.xy;

  vec3 d = normalize(vec3((gl_FragCoord.xy - 0.5 * r) / r.y, zoom));
  float anim = sin(TIME * 12.0 * speed) * 0.1 + 1.0;
  float g = 0.0;

  for (float i = 0.0; i < 110.0; i++) {
    p = g * d + vec3(0, 0, TIME * 0.0);
    p.z += TIME * speed;
    p = atan(cos(p * PI / 3.0)) / PI * 3.0;

    if (int(geometryType) == 1) {
      p.xy = sin(p.yx * 0.6);
    } else if (int(geometryType) == 2) {
      p *= cos(p.zxy * 0.4 + morph);
    } else if (int(geometryType) == 3) {
      p = normalize(p) * (length(p) + morph);
    }

    p = R(p, vec3(0.0, 1.0, 0.0), TIME * 0.5);

    float s = 5.0;
    float e = 1.0;
    for (int j = 0; j < 6; j++) {
      p = abs(p - vec3(1.0)) - vec3(1.1, 1.3, 1.0);
      s *= e = 2.2 / clamp(dot(p, p), 0.5, 2.3);
      p = abs(p) * e;
    }
    g += e = length(p.xz) / s;

    float pulse = 0.5 + 0.5 * sin(TIME * colorPulse);
    vec3 baseColor = getPalette(log(s) * 0.3 + pulse, int(paletteIndex));
    col += mix(vec3(0.1, 0.2, 1.0), baseColor, 0.6) * 0.02 * exp(-0.3 * i * i * e);
  }

  uv *= 2.0 * (cos(TIME * 2.0 * speed) - 2.5);
  vec3 starCol = happy_star(uv, anim) * getPalette(anim * 0.5 + TIME * 0.2, int(paletteIndex)) * 1.15;

  col *= starCol;
  col = adjustBSC(col, brightness, saturation, contrast);

  gl_FragColor = vec4(col, 1.0);
}
