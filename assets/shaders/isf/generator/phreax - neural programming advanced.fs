/*{
  "DESCRIPTION": "Fractal neuron spiral with palettes, morph, glitch, shake, and background separation.",
  "CATEGORIES": ["Fractal", "Generative", "VFX"],
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 6.999 },
    { "NAME": "colorPulseStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "fractalType", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "fractalTwist", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "fractalScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 }
  ],
  "PASSES": [
    { "TARGET": "BufferA", "PERSISTENT": true },
    {}
  ]
}*/

#define TAU 6.28318530718
#define R1 0.15
#define R2 1.0
#define PI 3.14

vec3 pal(float t, float id) {
    int pid = int(floor(id));
    if (pid == 0) return vec3(0.6,0.6,0.6) + vec3(1.0)*cos(TAU*(vec3(0.5)*t+vec3(0.1,0.4,0.7)));
    if (pid == 1) return vec3(0.5)*cos(TAU*(t+vec3(0.1,0.3,0.6)))+0.5;
    if (pid == 2) return vec3(sin(t*3.1), sin(t*2.1+1.0), sin(t*1.1+2.0));
    if (pid == 3) return vec3(cos(t*2.5+1.5), cos(t*1.5+2.5), cos(t*3.5+0.5))*0.5+0.5;
    if (pid == 4) return vec3(t, t*t, sin(t*PI))*0.6+0.4;
    if (pid == 5) return vec3(sin(6.3*t), sin(6.3*t+2.1), sin(6.3*t+4.1))*0.5+0.5;
    if (pid == 6) return vec3(0.3+0.7*sin(t*6.0),0.3+0.7*sin(t*4.0),0.3+0.7*sin(t*2.0));
    return vec3(t);
}

mat2 rot(float a) {
    return mat2(cos(a), sin(a), -sin(a), cos(a));
}

float d2(vec2 v, float k) {
    return pow(pow(abs(v.x), k) + pow(abs(v.y), k), 1. / k);
}

vec2 drosteSpiral(vec2 uv, out vec2 zOut) {
    vec2 z = vec2(length(uv), atan(uv.y, uv.x));
    z.x = log(z.x / R1);
    float ratio = log(R2 / R1);
    float alpha = atan(ratio, TAU);
    mat2 digamma = mat2(cos(alpha), sin(alpha), -sin(alpha), cos(alpha));
    vec2 beta = digamma * z;
    beta /= cos(alpha);
    beta.x = mod(beta.x, log(R2 / R1));
    beta = R1 * exp(beta.x) * vec2(cos(beta.y), sin(beta.y));
    zOut = z;
    return beta;
}

float neurons(vec2 uv, float time) {
    vec2 p = uv + sin(time*0.1)/10.;
    vec2 n = vec2(0), q, N = vec2(0);
    float S = 5.;
    mat2 m = rot(1. - 0.0001);

    for (float j = 0.; j++ < 30.;) {
        p *= m;
        n *= m;
        q = p * S + j + n + time;
        n += sin(q * 0.01);
        N += cos(q * 0.01) / S;
        S *= 1.15;
    }

    return pow(dot(N, vec2(1.)) + 0.1 + 0.0001 / length(N), 2.1);
}

vec3 fractalNeuron(vec2 uv, float N, float n, float l, float time, float palId, float pulse) {
    vec2 uv0 = uv, uv2 = uv;
    vec3 col = vec3(0);

    for (float i = 0.; i < N; i++) {
        uv = fract(uv * 1.9) - 0.5;
        uv2 = fract(uv2 * (1. + 0.5 * sin(time * 0.1))) - 0.5;

        float t = length(uv0) + time * 0.4 + i * 0.9 + pulse;
        vec3 c = pal(t, palId);

        float d = (d2(uv, 1.2) + 0.05 / n + length(uv2) * exp(-length(uv0)));
        d = abs(0.1 * d + sin(d * (8. + 2. * n)) / 10.) + 0.001 / n;
        d = l * 0.1 * pow((0.02) / d, 1.2) + smoothstep(0.05, 0.0, d);
        col += max(0., d) * c;
    }

    return col;
}

vec3 applyColor(vec3 col) {
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(col, vec3(lum), 1.0 - saturation);
    col = (col - 0.5) * contrast + 0.5;
    col *= brightness;
    return clamp(col, 0.0, 1.0);
}

void main() {
    float time = TIME * speed;
    float pulse = sin(time * colorPulseSpeed) * colorPulseStrength;

    if (PASSINDEX == 0) {
        vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
        float zoomFactor = exp(-mod(time * 0.5, 10.0)) * zoom;

        vec2 zSpiral;
        uv = drosteSpiral(uv * zoomFactor, zSpiral);
        uv += vec2(0.1, -0.2) * sin(time * 0.2);
        uv *= rot(time * 0.3 + shake);

        float n1 = neurons(uv, time);
        float n2 = neurons(uv * fractalScale, time + morph);

        vec3 fg = mix(
            fractalNeuron(uv, 6., n1, 1., time, palette, pulse),
            fractalNeuron(uv, 6., n2, 1., time, palette, pulse),
            0.5
        );

        vec3 bg = fractalNeuron(uv, 6., 1.0, 0.0, time, palette, 0.0);

        vec3 outCol = mix(fg, bg, mix(0.2, 0.4 * smoothstep(0.4, 0.5, length(uv)), 0.4));

        // Glitch
        if (glitchStrength > 0.0) {
            float g = sin(gl_FragCoord.y * 40.0 + time * glitchSpeed) * glitchStrength;
            outCol += vec3(g);
        }

        gl_FragColor = vec4(applyColor(outCol), 1.0);
    }
    else {
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        gl_FragColor = vec4(IMG_NORM_PIXEL(BufferA, uv).rgb, 1.0);
    }
}
