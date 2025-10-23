/*{
  "CATEGORIES": [ "ISF", "Psychedelic", "Tunnel", "Raymarch" ],
  "DESCRIPTION": "Trippy tunnel morphing with animated colors, palette blending, twisting, and glow.",
  "INPUTS": [
    {
      "NAME": "pulseSpeed",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": 0.0,
      "MAX": 10.0
    },
    {
      "NAME": "twistAmount",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "geoMorphAmount",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "tunnelWidth",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.05,
      "MAX": 1.0
    },
    {
      "NAME": "paletteMix",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "glowIntensity",
      "TYPE": "float",
      "DEFAULT": 0.001,
      "MIN": 0.0,
      "MAX": 0.01
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 1.3,
      "MIN": 0.5,
      "MAX": 3.0
    },
    {
      "NAME": "flickerAmount",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ],
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    },
    {
    }
  ]
}*/

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define PI 3.14159265
#define TAU (2.0 * PI)

vec3 glowAccum = vec3(0.0);
vec3 reflAtt = vec3(1.0);

vec3 palette1(float t) {
    return vec3(0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.67))));
}
vec3 palette2(float t) {
    return vec3(sin(t * 2.0), sin(t * 4.0), sin(t * 6.0));
}

vec3 path(float z) {
    return vec3(
        sin(z + cos(z * 0.7)) * 0.7,
        cos(z + cos(z * 1.2)) * 0.6,
        0.0
    ) * 2.0;
}

#define pmod(p,x) mod(p,x) - x*0.5

float map(vec3 p, float time) {
    vec3 w = abs(p);
    p -= path(p.z);
    
    // Twisting + morph
    p.xy *= rot(time * twistAmount + sin(p.z * 0.5) * 0.5);
    p.x += geoMorphAmount * sin(p.y * 4.0 + time);
    p.y += geoMorphAmount * cos(p.x * 3.0 + time * 1.5);

    float floorTop = (-p.y + 0.3) * 0.3;
    float floorBot = (p.y + 0.3) * 0.3;
    float dFloor = min(floorTop, floorBot);

    float sep = tunnelWidth + 0.05 * sin(time + p.z * 0.3);
    w.y = pmod(w.y, sep);

    float atten = pow(abs(sin(p.z * 0.2 + time * 0.1)), 40.0);
    float atten2 = pow(abs(sin(p.x * 2.0 + time)), 20.0);

    float colorT = time * pulseSpeed + p.z * 0.05 + sin(p.x * 0.5);
    vec3 col1 = palette1(colorT);
    vec3 col2 = palette2(colorT * 0.5 + 1.0);
    vec3 col = mix(col1, col2, paletteMix);

    col *= 0.8 + 0.2 * sin(time * pulseSpeed + p.z); // pulsating brightness

    float dLines = max(dFloor, -abs(w.y) + sep * 0.5) - 0.02;
    glowAccum += exp(-dLines * 60.0) * reflAtt * col * 20.0;

    return dFloor * 0.7;
}

float march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit, float time) {
    float d = 10e6;
    p = ro; t = 0.0; hit = false;
    for (int i = 0; i < 180; i++) {
        d = map(p, time);
        if (d < 0.001) {
            hit = true;
            break;
        }
        t += d;
        p = ro + rd * t;
    }
    return d;
}

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv) {
    vec3 dir = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0,1,0), dir));
    vec3 up = normalize(cross(dir, right));
    return normalize(dir + right * uv.x + up * uv.y);
}

vec3 getNormal(vec3 p, float time) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p - e.xyy, time) - map(p + e.xyy, time),
        map(p - e.yxy, time) - map(p + e.yxy, time),
        map(p - e.yyx, time) - map(p + e.yyx, time)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME;

    if (PASSINDEX == 0) {
        uv *= 1.0 - dot(uv, uv) * -0.2;
        vec3 col = vec3(0);
        vec3 ro = vec3(0);
        ro.z += time * 0.7;
        ro += path(ro.z);

        vec3 lookAt = vec3(0, 0, ro.z + 1.0) + path(ro.z + 1.0);
        vec3 rd = getRd(ro, lookAt, uv);
        rd.xy *= rot(sin(time) * 0.05);

        bool hit; float t; vec3 p;
        for (int i = 0; i < 2; i++) {
            march(ro, rd, p, t, hit, time);
            vec3 n = getNormal(p, time);
            if (i == 0) reflAtt *= vec3(0.2);
            rd = reflect(rd, n);
            ro = p + rd * 0.2;
        }

        col += glowAccum * glowIntensity;

        // Flicker
        col *= 1.0 + flickerAmount * step(fract(time * 2.0), 0.03);

        col = clamp(col, 0.0, 1.0);
        col = pow(col, vec3(contrast));
        gl_FragColor = vec4(col, 1.0);
    }

    else if (PASSINDEX == 1) {
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 uvn = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.xy;

        const int STEPS = 40;
        float scale = 0.00 + pow(length(uv - 0.5), 3.4) * 0.1;
        float chromAb = pow(length(uv - 0.5), 1.0) * 1.5;
        vec2 offs = vec2(0);
        vec4 radial = vec4(0);

        for (int i = 0; i < STEPS; i++) {
            scale *= 0.98;
            vec2 target = uv + offs;
            offs -= normalize(uvn) * scale / float(STEPS);

            vec2 R = mod(target + chromAb / RENDERSIZE.xy, 1.0);
            vec2 G = mod(target, 1.0);
            vec2 B = mod(target - chromAb / RENDERSIZE.xy, 1.0);

            radial.r += IMG_NORM_PIXEL(BufferA, R).r;
            radial.g += IMG_NORM_PIXEL(BufferA, G).g;
            radial.b += IMG_NORM_PIXEL(BufferA, B).b;
        }

        radial.rgb /= float(STEPS);
        gl_FragColor = radial;
    }
}
