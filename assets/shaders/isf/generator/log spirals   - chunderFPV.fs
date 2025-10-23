/*{
  "DESCRIPTION": "Converted spiral shader with palette, morph, and fractal geometry controls",
  "CATEGORIES": [ "Psychedelic", "Fractal", "VJ" ],
  "INPUTS": [
    { "NAME": "controlXY", "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "spiralStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "shapeWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "gridSharpness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.1415926
#define TAU 6.2831853

vec3 getPalette(float t, float i) {
  t = fract(t);
  if (i < 1.0) return sin(vec3(0.0, 1.047, 2.094) + t * TAU) * 0.5 + 0.5;
  if (i < 2.0) return cos(vec3(t, t * 2.0, t * 1.5)) * 0.5 + 0.5;
  if (i < 3.0) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0));
  if (i < 4.0) return abs(sin(vec3(t * 4.0 + 1.0, t * 2.0, t * 3.0)));
  if (i < 5.0) return vec3(t * t, sin(t * 2.1), abs(cos(t * 3.2))) * 0.6 + 0.2;
  if (i < 6.0) return vec3(0.5 + 0.5 * sin(t * 8.0), 0.5 + 0.5 * cos(t * 5.0), sin(t * 3.0) * 0.5 + 0.5);
  return vec3(sin(t * 1.5), sin(t * 2.1 + 1.0), cos(t * 2.8)) * 0.5 + 0.5;
}

vec3 adjustColor(vec3 c, float bri, float sat, float con) {
  c *= bri;
  float luma = dot(c, vec3(0.299, 0.587, 0.114));
  c = mix(vec3(luma), c, sat);
  c = mix(vec3(0.5), c, con);
  return clamp(c, 0.0, 1.0);
}

mat2 rotMatrix(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

void main() {
  vec2 R = RENDERSIZE.xy;
  vec2 fragCoord = gl_FragCoord.xy;
  vec2 normCoord = fragCoord / R;

  float t = (TIME * speed - 10.0) / 5.0;
  vec2 m = (controlXY * 4.0) - 2.0;
  if (length(controlXY - 0.5) < 0.01) {
    m = vec2(sin(t / 2.0) * 0.2, sin(t) * 0.1);
  }

  mat2 pitch = rotMatrix(m.y * PI * 0.5);
  mat2 yaw = rotMatrix(m.x * PI * 0.5);

  vec3 c = vec3(0.0);
  float aaPasses = 2.0;
  for (float k = 0.0; k < 4.0; k++) {
    if (k >= aaPasses * aaPasses) break;
    vec2 o = vec2(mod(k, aaPasses), floor(k / aaPasses)) / aaPasses;
    vec2 offsetCoord = (fragCoord - 0.5 * R + o) / R.y * zoom;
    vec3 u = normalize(vec3(offsetCoord, 1.0));
    u.yz = pitch * u.yz;
    u.xz = yaw * u.xz;

    float a = atan(u.y, u.x) * 0.5 * morph;
    u.xy = tan(log(length(u.xy) + 1e-3) * spiralStrength + vec2(a * 2.0, -a * 5.0) * shapeWarp);

    vec3 v = min(1.0 - abs(sin((u - t) * PI * pulse)), 1.0 / abs(u + 1e-3));
    float gval = min(v.x, v.y) / max(max(v.x, v.y), 1.0 - v.x);
    vec3 col = getPalette(u.x - t, palette);
    c += col * pow(gval, gridSharpness) * 0.6;
  }

  c /= aaPasses * aaPasses;
  c += c * c;
  c = adjustColor(c, brightness, saturation, contrast);
  gl_FragColor = vec4(c, 1.0);
}
