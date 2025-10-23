/*
{
    "CATEGORIES": [
        "Visual", "Generative"
    ],
    "DESCRIPTION": "Flower of Life visualization with tunable parameters.",
    "INPUTS": [
        {
            "NAME": "param1",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "param2",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        }
    ]
}
*/

#define TWO_PI 6.2831853072
#define PI 3.14159265359
const float timeScale = .05;
const float displace = 0.04;
const float gridSize = 36.0;
const float wave = 5.0;
const float brightness = 1.5;

vec2 rotate(vec2 v, float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return v * mat2(c, -s, s, c);
}

vec3 coordToHex(vec2 coord, float scale, float angle) {
    vec2 c = rotate(coord, angle);
    float q = (1.0 / 3.0 * sqrt(3.0) * c.x - 1.0 / 3.0 * c.y) * scale;
    float r = 2.0 / 3.0 * c.y * scale;
    return vec3(q, r, -q - r);
}

vec3 hexToCell(vec3 hex, float m) {
    return fract(hex / m) * 2.0 - 1.0;
}

float absMax(vec3 v) {
    return max(max(abs(v.x), abs(v.y)), abs(v.z));
}

float nsin(float value) {
    return sin(value * TWO_PI) * 0.5 + 0.5;
}

float hexToFloat(vec3 hex, float amt) {
    return mix(absMax(hex), 1.0 - length(hex) / sqrt(3.0), amt);
}

float calc(vec2 tx, float time, float modParam) {
    float angle = PI * nsin(time * 0.1) + PI / 6.0;
    float len = 2.0 / 122.0 * modParam + 1.0;
    float value = time * 0.005 + modParam * 0.0752;
    vec3 hex = coordToHex(tx, gridSize * nsin(time * 0.01), angle);
    
    for (int i = 0; i < 3; i++) {
        float offset = float(i) / 3.0;
        vec3 cell = hexToCell(hex, 1.0 + float(i));
        value += nsin(hexToFloat(cell, nsin(len + time + offset)) * 
                  wave * nsin(time * 0.5 + offset) + len + time);
    }
    return value / 3.0;
}

void main() {
    vec2 tx = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    tx.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * timeScale;
    
    vec3 rgb = vec3(0., 0., 0.);
    for (int i = 0; i < 3; i++) {
        float time2 = time + float(i) * displace;
        time2 += param1 * 1.1;
        rgb[i] += pow(calc(tx, time2, param2), 5.0);
    }
    
    vec3 finalColor = vec3(
        abs(sin(rgb[0] * 1.1)),
        abs(sin(rgb[1] * 1.0)),
        abs(sin(rgb[2] * 1.0))
    );
    
    finalColor *= brightness;
    gl_FragColor = vec4(finalColor, 1.0);
}
