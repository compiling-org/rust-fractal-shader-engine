/*
{
  "CATEGORIES": [
    "Generative",
    "Intersecting Circles",
    "Psychedelic"
  ],
  "DESCRIPTION": "Generates intersecting circles in a Flower of Life pattern.",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "pulseSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.01,
      "MAX": 2.0
    },
    {
      "NAME": "circleRadius",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.05,
      "MAX": 0.5
    },
    {
      "NAME": "brightness",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.5,
      "MAX": 3.0
    }
  ],
  "ISFVSN": "2"
}
*/

#define TWO_PI 6.2831853072

float circle(vec2 uv, vec2 center, float radius, float pulse) {
    float d = length(uv - center) - radius;
    return smoothstep(pulse, 0.0, abs(d));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float time = TIME * pulseSpeed;
    vec3 color = vec3(0.0);

    for (int i = 0; i < 6; i++) {
        float angle = float(i) * TWO_PI / 6.0;
        vec2 center = vec2(cos(angle), sin(angle)) * 0.3;
        float t = circle(uv, center, circleRadius, 0.05 + 0.02 * sin(time));
        color += vec3(t * abs(sin(TIME * 2.0)));
    }

    color *= brightness;
    gl_FragColor = vec4(color, 1.0);
}