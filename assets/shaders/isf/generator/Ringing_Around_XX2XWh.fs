/*
{
  "ISFVSN": "2.0",
  "CATEGORIES": [ "Psychedelic", "Fractal", "Geometric" ],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.05, "MAX": 1.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "morphing", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.1, "MAX": 1.0 }
  ]
}

*/



#ifdef GL_FRAGMENT_PRECISION_HIGH
precision highp float;
#else
precision mediump float;
#endif

vec2 rotateCW(vec2 p, float a)
{
  mat2 m = mat2(cos(a), -sin(a), sin(a), cos(a));
  return p * m;
}

vec3 palette(float t)
{
  vec3 a = vec3(0.731, 1.098, 0.192);
  vec3 b = vec3(0.358, 1.090, 0.657);
  vec3 c = vec3(1.077, 0.360, 0.328);
  vec3 d = vec3(0.965, 2.265, 0.837);

  return a + b * cos(6.28318 * (c * t * d * colorShift));
}

void mainImage_orig(out vec4 fragColor, in vec2 fragCoord) {
  vec2 uv = (fragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
  vec2 uv0 = uv;
  vec3 finalColor = vec3(0.0);

  for (float i = 0.0; i < 5.0; i++) {
    uv = fract(uv * zoom) - 0.5;
    uv = rotateCW(uv, i * cos(TIME * speed));

    float d = length(uv) * exp(-length(uv0));

    vec3 color = palette(d * colorPulse * cos(TIME));

    d = sin(d * colorPulse + cos(0.25 * TIME * (morphing - i))) / 4.0;
    d = abs(d);
    d = 0.01 / d;
    d = pow(d, 1.4);

    finalColor += color * d;
  }
  fragColor = vec4(finalColor, 1.0);
}

void main() {
    vec4 tcol = vec4(0.);
    const int AA = 4;
    for (int mx = 0; mx < AA; mx++)
    for (int nx = 0; nx < AA; nx++)
    {
        vec2 o = vec2(float(mx), float(nx)) / float(AA) - 0.5;
        mainImage_orig(tcol, gl_FragCoord.xy + o);
        tcol += clamp(tcol, 0., 1.);
    }
    gl_FragColor = tcol / float(AA * AA);
}
