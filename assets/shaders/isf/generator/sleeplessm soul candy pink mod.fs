/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Audio Reactive", "Fractal", "Live Visuals"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0},
        {"NAME": "ringMultiplier", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 5.0},
        {"NAME": "ringWidth", "TYPE": "float", "DEFAULT": 64.0, "MIN": 10.0, "MAX": 100.0},
        {"NAME": "smoothing", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0},
        {"NAME": "iter", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0},
        {"NAME": "audioInput", "TYPE": "image"}
    ]
}
*/

#define getLevel(x) (IMG_PIXEL(audioInput, vec2(x, 0.0)).r)

float getVol(float samples) {
    float avg = 0.0;
    for (float i = 0.0; i < samples; i++) avg += getLevel(i / samples);
    return avg / samples;
}

float PHI = 1.61803398874989484820459;

float gold_noise(in vec2 xy, in float seed) {
    return fract(tan(distance(xy * PHI, xy) * seed) * xy.x);
}

vec3 palette(float dist) {
    vec3 a = vec3(0.5, 0.0, 0.5);
    vec3 b = vec3(0.25, 0.25, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.263, 0.416, 0.557);
    return a + b * cos(6.28318 * (c * dist * d));
}

float sdHexagon(in vec2 p, in float r) {
    const vec3 k = vec3(-0.866025404, 0.5, 0.577350269);
    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= vec2(clamp(p.x, -k.z * r, k.z * r), r);
    return length(p) * sign(p.y);
}

void main() {
    float vol = getVol(8.0);
    float myTime = (time * 0.5) + vol;
    
    vec2 uv = ((gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y);
    vec2 uv0 = uv;
    
    float noise = gold_noise(gl_FragCoord.xy, 6502.0) - ((sin(myTime * 0.5) * 0.5) + 0.5);
    uv *= (zoom * sin(myTime * 0.05));
    
    vec3 outCol = vec3(0.0);

    for (float i = 0.0; i < iter; i++) {
        uv = fract(uv * (1.25 + (sin((myTime * 0.1) * i)))) - 0.5;
        
        float dist = sdHexagon(uv, 0.2 * (vol + i));
        vec3 col = palette(length(uv0) + myTime + (vol * colorPulse));
        
        dist = sin((dist - (myTime * speed)) * ringMultiplier * 2.0) / ringWidth;
        dist = abs(dist);
        dist = pow(0.00015 / dist, 1.05);
        
        col = col * (smoothstep(0.0, 0.5 - smoothing, dist) * noise) * 2.0;
        outCol += col + dist;
    }

    outCol.rgb = mix(outCol.rgb, outCol.bgr, colorShift);
    
    gl_FragColor = vec4(outCol, 1.0);
}
