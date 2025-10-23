/*{
  "DESCRIPTION": "Trippy 3D raymarch with animated palettes, morphing fractals, and full ISF controls",
  "CATEGORIES": [ "Fractal", "3D", "ISF" ],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "geometryType", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "camYaw", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.1415927, "MAX": 3.1415927 },
    { "NAME": "camPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.5707963, "MAX": 1.5707963 },
    { "NAME": "camRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.1415927, "MAX": 3.1415927 },
    { "NAME": "camShake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchIntensity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "warpAmount", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "tunnelTwist", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0 },
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

float hash(vec3 p) {
  return fract(sin(dot(p ,vec3(127.1,311.7,74.7))) * 43758.5453);
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

vec3 warp(vec3 p, float t) {
  p += sin(p.zxy * 2.5 + t * 1.2) * warpAmount;
  return p;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
  uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1.0);
  vec3 col = vec3(0.0);

  vec3 ro = vec3(0.0, 0.0, -4.0 + sin(TIME * 0.5) * camShake);
  vec3 rd = normalize(vec3(uv, zoom));

  rd = rotate(rd, camPitch, camYaw, camRoll);
  ro.xy += (xy - 0.5) * 2.0;

  float t = 0.0;
  float glow = 0.0;
  vec3 finalColor = vec3(0.0);

  for (int i = 0; i < 64; i++) {
    vec3 p = ro + rd * t;
    p = warp(p, TIME);
    p += glitchIntensity * vec3(hash(p), hash(p.yzx), hash(p.zxy));

    vec3 pp = p;

    int gtype = int(geometryType);
    if (gtype == 0) pp = abs(pp) - morph;
    else if (gtype == 1) pp = sin(pp * morph + TIME);
    else if (gtype == 2) pp = normalize(pp) * (length(pp) + morph);
    else if (gtype == 3) pp += cos(pp.zxy * morph + TIME);
    else pp = pp * morph + tan(pp.zxy * 0.5);

    pp.xy = mat2(cos(tunnelTwist), -sin(tunnelTwist), sin(tunnelTwist), cos(tunnelTwist)) * pp.xy;

    float dist = length(pp) - 1.0;
    if (dist < 0.01) break;
    float glowAmt = exp(-dist * 10.0) * 0.02;
    float pulse = 0.5 + 0.5 * sin(TIME * colorPulse + t * 0.2);
    vec3 pal = getPalette(pulse + t * 0.1, int(paletteIndex));

    finalColor += pal * glowAmt;
    glow += glowAmt;
    t += dist * 0.5;
  }

  col = finalColor / (glow + 1e-5);
  col = adjustBSC(col, brightness, saturation, contrast);
  gl_FragColor = vec4(col, 1.0);
}
