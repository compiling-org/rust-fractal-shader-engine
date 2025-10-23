/*{
    "CATEGORIES": [
        "Psychedelic",
        "Fractal",
        "Geometric"
    ],
    "INPUTS": [
        {
            "DEFAULT": 0.01,
            "MAX": 1,
            "MIN": 0.01,
            "NAME": "speed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.5,
            "MAX": 3,
            "MIN": 0.5,
            "NAME": "zoom",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.62,
            "MAX": 2,
            "MIN": 0.1,
            "NAME": "morph",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.17,
            "MAX": 1,
            "MIN": 0,
            "NAME": "colorShift",
            "TYPE": "float"
        },
        {
            "DEFAULT": 12,
            "MAX": 24,
            "MIN": 1,
            "NAME": "psyPulse",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.2,
            "MAX": 2.5,
            "MIN": 0.5,
            "NAME": "artifactStrength",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/



vec3 palette(float t) {
    vec3 a = vec3(0.395, 0.353, 0.291);
    vec3 b = vec3(0.392, 0.242, 0.474);
    vec3 c = vec3(0.978, 1.446, 1.322);
    vec3 d = vec3(5.995, 5.848, 1.777);
    return a + b * cos(6.28318 * (c * t + d));
}

vec2 rotateUV(vec2 uv, float rot) {
    float cs = cos(rot), si = sin(rot);
    return vec2(uv.x * cs + uv.y * si, uv.x * si - uv.y * cs);
}

float rect(vec2 p) {
    return abs(p.x) + abs(p.y);
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv;
    vec3 finalColor = vec3(0.0);
    
    uv = rotateUV(uv, TIME * speed);
    
    float d;
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        uv = fract(uv * zoom) - 0.5;

        if (i % 2 == 0) {
            d = length(uv) * exp(-length(uv0));
        } else {
            d = rect(uv) * exp(-rect(uv0));
        }

        vec3 col = palette(length(uv0) + fi * morph + TIME * colorShift);

        d *= 1.0 - length(uv);
        d = sin(d * psyPulse + TIME * fi * 0.15) / 6.0;
        d = abs(d);
        d = pow(0.0025 * fi / d, artifactStrength);

        finalColor += col * d;
    }
    
    gl_FragColor = vec4(finalColor, 1.0);
}