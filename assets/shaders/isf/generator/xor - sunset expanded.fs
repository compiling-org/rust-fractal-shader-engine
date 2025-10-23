/*{
  "DESCRIPTION": "Psychedelic Sunset - converted from ShaderToy by XorDev, extended with multiple controls",
  "CATEGORIES": [ "Psychedelic", "Fractal", "Raymarch", "Visuals" ],
  "INPUTS": [
    { "NAME": "pulse", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "palette", "TYPE": "float", "MIN": 0.0, "MAX": 11.0, "DEFAULT": 0.0 },
    { "NAME": "zoom", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "morph", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "speed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "shake", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "xy", "TYPE": "point2D", "DEFAULT": [0.5, 0.5] },
    { "NAME": "brightness", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 }
  ]
}*/

#define PI 3.14159265
#define STEPS 100.0
#define WAVE_STEPS 8.0
#define WAVE_FREQ 5.0
#define WAVE_AMP 0.6
#define WAVE_EXP 1.8
#define PASSTHROUGH 0.2
#define SOFTNESS 0.005
#define SKY 10.0
#define FOV 1.0

vec3 getPaletteColor(float t, float p) {
    if (p < 0.5)  return vec3(sin(t*2.0)*0.5+0.5, sin(t*3.0+1.0)*0.5+0.5, sin(t*1.5+2.0)*0.5+0.5);             // Fire Spiral
    if (p < 1.5)  return vec3(sin(t+0.0), sin(t+2.0), sin(t+4.0)) * 0.5 + 0.5;                                 // Rainbow Cycle
    if (p < 2.5)  return vec3(sin(t*5.0), cos(t*3.0), sin(t*7.0))*0.5+0.5;                                     // Cyber Chrome
    if (p < 3.5)  return vec3(cos(t*3.0+2.0), sin(t*2.0+1.0), cos(t*1.0));                                     // Fuchsia Flux
    if (p < 4.5)  return 1.0 - vec3(sin(t), sin(t*2.0), sin(t*3.0))*0.5+0.5;                                   // Inverted Pulse
    if (p < 5.5)  return vec3(sin(t*9.0), sin(t*4.0+1.0), sin(t*2.0+2.0))*0.5+0.5;                             // Lava Pulse
    if (p < 6.5)  return vec3(fract(t*1.1), fract(t*1.5), fract(t*2.3));                                       // Alien DNA
    if (p < 7.5)  return vec3(sin(t*10.0)*0.5+0.5, cos(t*3.0)*0.5+0.5, sin(t*5.0)*0.5+0.5);                    // Acid Bloom
    if (p < 8.5)  return vec3(fract(t*4.1), sin(t*2.2)*0.5+0.5, cos(t*1.3)*0.5+0.5);                           // Neon Flicker
    if (p < 9.5)  return vec3(abs(sin(t*6.0)), abs(cos(t*4.0)), sin(t*2.1 + cos(t)));                          // Jungle Spiral
    if (p < 10.5) return vec3(mod(t,1.0), mod(t*0.618,1.0), mod(t*1.618,1.0));                                 // Golden Flow
    return vec3(cos(t*8.0), sin(t*6.0), cos(t*4.0)) * 0.5 + 0.5;                                               // UV Blossom
}

vec3 applyColorMod(vec3 col, float brightness, float saturation, float contrast) {
    vec3 grey = vec3(dot(col, vec3(0.333)));
    col = mix(grey, col, saturation);
    col = (col - 0.5) * contrast + 0.5;
    return col * brightness;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 fragCoord = uv * RENDERSIZE.xy;
    vec2 xyControl = xy * 2.0 - 1.0;

    float time = TIME * speed;

    vec3 dir = normalize(vec3(2.0 * fragCoord - RENDERSIZE.xy, -FOV * RENDERSIZE.y / zoom));
    dir.xy += (sin(time * 20.0 + dir.xy * 40.0) * shake * 0.01);

    vec3 col = vec3(0.0);
    float z = 0.0, d = 0.0, s = 0.0;

    for (float i = 0.0; i < STEPS; i++) {
        vec3 p = z * dir;
        float f = WAVE_FREQ;

        for (float j = 0.0; j < WAVE_STEPS; j++, f *= WAVE_EXP) {
            p += WAVE_AMP * sin(p * f - vec3(0.2) * time * morph).yzx / f;
        }

        s = 0.3 - abs(p.y);
        d = SOFTNESS + max(s, -s * PASSTHROUGH) / 4.0;
        z += d;

        float phase = pulse * s + dot(p, vec3(1, -1, 0)) + time;

        if (glitch > 0.0) {
            phase += sin(p.x * 10.0 + time * 20.0) * 0.2 * glitch;
        }

        vec3 baseColor = getPaletteColor(phase, palette);
        baseColor += vec3(1.5);

        col += baseColor * exp(s * SKY) / d;
    }

    col *= SOFTNESS / STEPS;
    col = tanh(col * col);
    col = applyColorMod(col, brightness, saturation, contrast);

    gl_FragColor = vec4(col, 1.0);
}
