/*{
  "DESCRIPTION": "Converted from Shadertoy X3jyzW by nayk. Tunnel preserved, ISF controls added.",
  "CATEGORIES": ["Fractal", "Tunnel", "Converted", "ISF"],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 1.5 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "camYaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "camPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "camRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "camShake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "xy", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] }
  ]
}*/

#define PI 3.1415926535
#define TAU 6.2831853

vec3 rotate(vec3 p, float pitch, float yaw, float roll) {
  mat3 rx = mat3(1,0,0, 0,cos(pitch),-sin(pitch), 0,sin(pitch),cos(pitch));
  mat3 ry = mat3(cos(yaw),0,sin(yaw), 0,1,0, -sin(yaw),0,cos(yaw));
  mat3 rz = mat3(cos(roll),-sin(roll),0, sin(roll),cos(roll),0, 0,0,1);
  return rz * ry * rx * p;
}

vec3 getPalette(float t, int i) {
  if (i == 0) return vec3(0.5) + 0.5*cos(TAU*(vec3(0.0, 0.33, 0.67)+t));
  if (i == 1) return vec3(sin(t*6.28), sin(t*3.14), cos(t*4.71)) * 0.5 + 0.5;
  if (i == 2) return vec3(1.0 - abs(sin(t + vec3(0.0,1.0,2.0))));
  if (i == 3) return fract(t * vec3(3.14, 1.61, 0.99));
  if (i == 4) return mix(vec3(0.9,0.2,0.4), vec3(0.2,0.8,1.0), sin(t*2.));
  if (i == 5) return 0.5 + 0.5*sin(t*vec3(2.0,3.0,5.0));
  return vec3(0.6 + 0.4*cos(t*vec3(0.5,1.0,1.5)));
}

vec3 adjustBSC(vec3 color, float brightness, float saturation, float contrast) {
  vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114)));
  color = mix(gray, color, saturation);
  color = (color - 0.5) * contrast + 0.5;
  return color + brightness;
}

float happy_star(vec2 uv, float anim) {
  uv = abs(uv);
  vec2 pos = min(uv.xy / uv.yx, anim);
  float p = (2.0 - pos.x - pos.y);
  return (2.0 + p*(p*p - 1.5)) / (uv.x + uv.y);      
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
  uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1.0);

  vec3 col = vec3(0.0);
  vec2 r = RENDERSIZE.xy;
  vec3 ro = vec3(0.0, 0.0, 0.0);
  vec3 rd = normalize(vec3(uv, zoom));

  ro.xy += (xy - 0.5) * 2.0;
  rd = rotate(rd, camPitch, camYaw, camRoll);

  float g = 0.0;
  float anim = sin(TIME * 12.0 * speed) * 0.1 + morph;

  for (float i = 0.0; i < 110.0; i++) {
    vec3 p = g * rd;
    p.z += TIME * speed;
    p = atan(cos(p * PI / 3.0)) / PI * 3.0;
    p = rotate(p, 0.0, TIME, 0.0);

    float s = 5.0;
    float e = 1.0;

    for (int j = 0; j < 6; j++) {
      p = abs(p - vec3(1.0)) - vec3(1.1, 1.3, 1.0);
      s *= e = 2.2 / clamp(dot(p, p), 0.5, 2.3);
      p = abs(p) * e;
    }

    g += e = length(p.xz) / s;

    float pulse = 0.5 + 0.5 * sin(TIME * colorPulse);
    vec3 base = getPalette(log(s) * 0.3 + pulse, int(paletteIndex));
    col += mix(vec3(0.1, 0.2, 1.0), base, 0.6) * 0.02 * exp(-0.3 * i * i * e);
  }

  uv *= 2.0 * (cos(TIME * 2.0 * speed) - 2.5);
  vec3 starCol = happy_star(uv, anim) * getPalette(anim * 0.5 + TIME * 0.2, int(paletteIndex)) * 1.15;

  col *= starCol;
  col = adjustBSC(col, brightness, saturation, contrast);
  gl_FragColor = vec4(col, 1.0);
}
