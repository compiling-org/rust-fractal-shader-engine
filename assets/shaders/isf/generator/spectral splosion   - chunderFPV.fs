/*{
  "DESCRIPTION": "Exact visual match of Shadertoy md2XDz using feedbackImage for blur trails, 3D rotating grid, and trippy color cycling.",
  "CATEGORIES": [ "Psychedelic", "Fractal", "Grid", "VJ" ],
  "INPUTS": [
    { "NAME": "feedbackImage", "TYPE": "image" },
    { "NAME": "controlXY", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "MIN": [-1.0, -1.0], "MAX": [1.0, 1.0] },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 100.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.01, "MAX": 5.0 },
    { "NAME": "dotSize", "TYPE": "float", "DEFAULT": 350.0, "MIN": 100.0, "MAX": 1000.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.2831853

mat2 rot(float a) {
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

vec3 hue(float t) {
  return sin(vec3(0.0, 2.094, 4.188) + t * TAU) * 0.5 + 0.5;
}

vec3 colorAdjust(vec3 col, float bri, float sat, float con) {
  col *= bri;
  float luma = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(luma), col, sat);
  col = mix(vec3(0.5), col, con);
  return clamp(col, 0.0, 1.0);
}

float shape(vec2 p, float r) {
  vec2 fp = fract(p + 0.5) - 0.5;
  float len = length(fp);
  return max(0.0, 1.0 - len / (r + 1e-5));
}

void main() {
  vec2 R = RENDERSIZE.xy;
  vec2 U = gl_FragCoord.xy;
  vec2 uv = (U - R * 0.5) / R.y;

  float t = TIME * speed / 30.0;
  vec2 m = (controlXY * R - R * 0.5) / R.y;

  if (length(controlXY) < 0.01) {
    m = vec2(cos(t * PI), sin(t * PI * 2.0)) * 2.0;
  }

  vec3 rd = normalize(vec3(uv, abs(sin(t * 0.6)) * 0.075)) * zoom;
  rd.yz *= rot(-m.y * 1.57);
  rd.xz *= rot(-m.x * 1.57);

  float r = length(uv);
  float pr = R.y / dotSize;
  float b = clamp(r, 0.05, 1.0) * 0.25;

  // feedbackImage used for persistent blur (like BufferA)
  vec3 col = texture2D(feedbackImage, U / R).rgb * 0.97;
  col += vec3(0.2, 0.5, 0.9) * r * 0.013;

  vec3 mt = t * vec3(floor(rd.x + 0.5), floor(rd.y + 0.5), floor(rd.z + 0.5));
  vec3 l = vec3(length(rd.xy), length(rd.yz), length(rd.xz)) / TAU;

  col += shape(rd.xy - vec2(mt.y, 0.0), pr) * hue(l.x - t) * b;
  col += shape(rd.yx - vec2(mt.x, 0.0), pr) * hue(l.x - t + 1. / 6.) * b;
  col += shape(rd.zy - vec2(mt.y, 0.0), pr) * hue(l.y - t + 2. / 6.) * b;
  col += shape(rd.yz - vec2(mt.z, 0.0), pr) * hue(l.y - t + 3. / 6.) * b;
  col += shape(rd.xz - vec2(mt.z, 0.0), pr) * hue(l.z - t + 4. / 6.) * b;
  col += shape(rd.zx - vec2(mt.x, 0.0), pr) * hue(l.z - t + 5. / 6.) * b;

  col = colorAdjust(col, brightness, saturation, contrast);
  gl_FragColor = vec4(col, 1.0);
}
