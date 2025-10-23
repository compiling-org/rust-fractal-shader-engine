/*{
  "DESCRIPTION": "Fractal tunnel with spatial palettes, fog/bg color controls, and visible pulse overlay.",
  "CATEGORIES": ["Generator", "Fractal", "Psychedelic"],
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "pulseIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "zoom", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "speed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "morph", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.0 },
    { "NAME": "fractalType", "TYPE": "long", "VALUES": [0,1,2], "LABELS": ["Default","Warped","Bubble"], "DEFAULT": 0 },
    { "NAME": "iterations", "TYPE": "float", "MIN": 1.0, "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "scaleMult", "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "folding", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
    { "NAME": "detail", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "shake", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "brightness", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "saturation", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "camX", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "camY", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "camZ", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 5.0 },
    { "NAME": "paletteSelect", "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 0.0 },
    { "NAME": "backgroundColor", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "fogColor", "TYPE": "color", "DEFAULT": [0.3, 0.3, 0.3, 1.0] },
    { "NAME": "fogStrength", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

#define MARCH_ITERS 99
#define N normalize
#define TONEMAP(x) ((x) / ((x) + 0.155) * 1.019)

bool fractalHit = false;

vec3 palette(float z, float id) {
    float t = fract(z * 0.05);
    if (id < 1.0) return vec3(sin(6.0*t), sin(3.0*t), sin(1.0*t)) * 0.5 + 0.5;
    if (id < 2.0) return vec3(sin(10.0*t+0.5), sin(7.0*t+1.0), sin(13.0*t+1.5)) * 0.5 + 0.5;
    if (id < 3.0) return vec3(0.7+0.3*sin(2.0*t), 0.6+0.4*sin(2.0*t+1.0), 0.8+0.2*sin(2.0*t+2.0));
    if (id < 4.0) return vec3(step(0.5,sin(t*10.0)), step(0.5,sin(t*15.0+1.0)), step(0.5,sin(t*20.0+2.0)));
    if (id < 5.0) return vec3(abs(sin(t*5.0+3.1)), abs(sin(t*8.0+1.3)), abs(sin(t*12.0+4.7)));
    if (id < 6.0) return vec3(fract(sin(t*9.0)*43758.5453), fract(sin(t*7.0)*23421.3123), fract(sin(t*13.0)*34512.1234));
    return vec3(0.5 + 0.5*sin(16.0*t + vec3(2.7,1.3,6.1)));
}

float fractal(vec3 p) {
    float w = 1.0, l;
    p *= vec3(4., 3.5, 4.) + morph;
    p.xy += 0.5;
    for (int i = 0; i < int(iterations); i++) {
        if (fractalType == 0.0)
            p = cos(p - 0.5);
        else if (fractalType == 1.0)
            p = sin(p - folding);
        else
            p = abs(p - folding);
        l = scaleMult / dot(p, p);
        p *= l;
        w *= l;
    }
    return length(p)/w * detail;
}

float map(vec3 q){
    vec3 p = q;
    float t = 4. - length(p.xy);
    float f = fractal(p);
    fractalHit = f < t;
    return min(f, t);
}

float AO(vec3 pos, vec3 nor) {
    float sca = 2.0, occ = 0.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + float(i)*0.5/4.0;
        float dd = map(nor * hr + pos);
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

vec3 fogMix(vec3 col, float d) {
    float f = clamp((d - 0.5) * fogStrength, 0.0, 1.0);
    return mix(col, fogColor.rgb, f);
}

vec3 applyGlitch(vec3 col, vec3 p, float t) {
    float g = glitch * sin(50.0*p.x + t*10.0) * sin(50.0*p.y + t*5.0);
    return col + g * 0.1;
}

vec3 postProcess(vec3 col) {
    col = mix(vec3(dot(col, vec3(0.333))), col, saturation);
    col = (col - 0.5) * contrast + 0.5;
    col *= brightness;
    return col;
}

void main() {
    vec2 U = isf_FragNormCoord;
    vec2 u = (U - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    float t = TIME * speed;
    float zVal = t * zoom;
    vec3 ro = vec3(0, 0, zVal);
    vec3 lookAt = ro - vec3(camX, camY, camZ);
    vec3 Z = N(vec3(0, 0, zVal + 3.0) - lookAt);
    vec3 X = N(vec3(Z.z, 0, -Z.x));
    vec3 Y = cross(Z, X);
    vec3 D = normalize(vec3(u, 1.0)) * mat3(X, Y, Z);

    if (shake > 0.0)
        ro.xy += vec2(sin(t*20.0), cos(t*25.0)) * shake * 0.1;

    float d = 0.0;
    vec3 hitPos;
    for (int i = 0; i < MARCH_ITERS; i++) {
        hitPos = ro + D * d;
        float dist = map(hitPos) * 0.55;
        d += dist;
        if (dist < 0.001) break;
    }

    vec3 e = vec3(0.01, 0.0, 0.0);
    vec3 n = normalize(vec3(
        map(hitPos) - map(hitPos - e.xyy),
        map(hitPos) - map(hitPos - e.yxy),
        map(hitPos) - map(hitPos - e.yyx)
    ));

    vec3 col = backgroundColor.rgb;

    if (fractalHit) {
    vec3 base = palette(hitPos.z, paletteSelect);
    float pulse = sin(hitPos.z * 10.0 + t * colorPulse);
    pulse = 0.5 + 0.5 * sign(pulse); // sharp pulse bands
    col = mix(base, vec3(1.0, 1.0, 0.5), pulse * pulseIntensity);
}
    col *= AO(hitPos, n);
    col = applyGlitch(col, hitPos, t);
    col = fogMix(col, d);
    col = TONEMAP(col);
    col = postProcess(col);

    gl_FragColor = vec4(col, 1.0);
}
