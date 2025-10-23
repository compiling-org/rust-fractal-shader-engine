/*
{
  "CATEGORIES": [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION": "Winning shader from REVISION 2020 demoparty Shader Showdown with tunable parameters.",
  "IMPORTED": {
    "iChannel0": {
      "NAME": "iChannel0",
      "PATH": "0a40562379b63dfb89227e6d172f39fdce9022cba76623f1054a2c83d6c0ba5d.png"
    }
  },
  "INPUTS": [
    {
      "NAME": "ColorIntensity",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "NoiseScale",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "LightStrength",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "VignetteStrength",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "GlowIntensity",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    }
  ]
}

*/


// Winning shader made at REVISION 2020 demoparty Shader Showdown. Round 1 against Nusan / Cookies
// Video of the battle is here: https://youtu.be/4GRD1gCX7fk?t=6058
vec2 z, v, e = vec2(.0035, -.0035);
float t, tt, g, g2;
vec3 np, bp, pp, po, no, al, ld;

float bo(vec3 p, vec3 r) {
    p = abs(p) - r;
    return max(max(p.x, p.y), p.z);
}

mat2 r2(float r) {
    return mat2(cos(r), sin(r), -sin(r), cos(r));
}

vec2 fb(vec3 p) {
    pp = p;
    pp.xz *= r2(.785);
    vec2 h, t = vec2(bo(pp, vec3(4)), 6);
    t.x = max(t.x, -(length(p) - 1.));
    t.x = max(abs(abs(t.x) - .8) - .3, abs(p.y) - 1.);
    t.x = max(t.x, abs(p.z) - 3.5);
    h = vec2(bo(pp, vec3(4)), 3);
    h.x = max(h.x, -(length(p) - 1.));
    h.x = max(abs(abs(h.x) - .8) - .15, abs(p.y) - 1.3);
    h.x = max(h.x, abs(p.z) - 3.3);
    t = t.x < h.x ? t : h;
    h = vec2(bo(pp, vec3(4)), 5);
    h.x = max(h.x, -(length(p) - 1.));
    h.x = max(abs(abs(h.x) - .8) - .4, abs(p.y) - .7);
    h.x = max(h.x, abs(p.z) - 3.7);
    t = t.x < h.x ? t : h;
    h = vec2(bo(pp, vec3(4)), 6);
    h.x = max(h.x, -(length(p) - 1.));
    h.x = max(abs(h.x), abs(p.y));
    h.x = max(h.x, abs(p.z) - 3.);
    g += 0.1 / (0.1 + h.x * h.x * (10. - sin(bp.y * bp.z * .005 + tt * 5.) * 9.));
    t = t.x < h.x ? t : h;
    t.x *= 0.7;
    return t;
}

vec4 texNoise(vec2 uv) {
    float f = 0.;
    f += IMG_NORM_PIXEL(iChannel0, mod(uv * NoiseScale * .125, 1.0)).r * .5;
    f += IMG_NORM_PIXEL(iChannel0, mod(uv * NoiseScale * .25, 1.0)).r * .25;
    f += IMG_NORM_PIXEL(iChannel0, mod(uv * NoiseScale * .5, 1.0)).r * .125;
    f += IMG_NORM_PIXEL(iChannel0, mod(uv * NoiseScale * 1., 1.0)).r * .125;
    f = pow(f, 1.2);
    return vec4(f * .45 + .05);
}

vec2 mp(vec3 p) {
    np = bp = p;
    for (int i = 0; i < 4; i++) {
        np = abs(np) - vec3(7, 1.5, 5);
        np.xz *= r2(.3925);
    }
    vec2 h, t = fb(np);
    h = fb(p * .085);
    h.x *= 10.;
    h.x = max(h.x, -(length(p.xz) - 17.));
    t = t.x < h.x ? t : h;
    h = vec2(.5 * (abs(p.y) - 4. + 6. * texNoise(p.xz * .05).r), 7);
    h.x = max(h.x, -(length(p.xz) - 17.));
    t = t.x < h.x ? t : h;
    h = vec2(length(abs(p.xz) - vec2(5., 0.)) - .5 + (np.y * .06), 6);
    g2 += 1. / (0.1 + h.x * h.x * (10. - cos(np.y * .2 - tt * 5.) * 9.));
    t = t.x < h.x ? t : h;
    h = vec2(length(abs(p.xz) - vec2(11., 29.)) - .5 + (np.y * .06), 6);
    g += 1. / (0.1 + h.x * h.x * (10. - cos(np.y * .2 - tt * 5.) * 9.));
    t = t.x < h.x ? t : h;
    pp = p + vec3(0, sin(p.x * p.z * .01) * 3., 0);
    pp.xz *= r2(sin(p.y * .1) * .7 + tt);
    h = vec2(length(sin(pp * .5 - vec3(0, tt * 5., 0))), 6);
    h.x = max(h.x, (length(p.xz) - 17.));
    g += 0.1 / (0.1 + h.x * h.x * (100. - sin(bp.y * bp.z * .005 + tt * 5.) * 99.));
    t = t.x < h.x ? t : h;
    return t;
}

vec2 tr(vec3 ro, vec3 rd) {
    vec2 h, t = vec2(.1);
    for (int i = 0; i < 128; i++) {
        h = mp(ro + rd * t.x);
        if (h.x < .0001 || t.x > 120.)
            break;
        t.x += h.x;
        t.y = h.y;
    }
    if (t.x > 120.)
        t.y = 0.;
    return t;
}

#define a(d) clamp(mp(po + no * d).x / d, 0., 1.)
#define s(d) smoothstep(0., 1., mp(po + ld * d).x / d)

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5) / vec2(RENDERSIZE.y / RENDERSIZE.x, 1);
    tt = mod(TIME * AnimationSpeed, 62.82);
    vec3 ro = mix(vec3(sin(tt * .5) * 5., -cos(tt * .5) * 50., 5.), vec3(cos(tt * .5 - .5) * 5., 35., sin(tt * .5 - .5) * 45.), ceil(sin(tt * .5))),
         cw = normalize(vec3(0) - ro),
         cu = normalize(cross(cw, normalize(vec3(0, 1, 0)))),
         cv = normalize(cross(cu, cw)),
         rd = mat3(cu, cv, cw) * normalize(vec3(uv, .5)),
         co, fo;
    ld = normalize(vec3(.2, .5, .0));
    v = vec2(abs(atan(rd.x, rd.z)), rd.y - tt * .2);
    co = fo = (vec3(.1) - length(uv) * .1 - rd.y * .1) * 3. * texNoise(v * .4).r;
    z = tr(ro, rd);
    t = z.x;
    if (z.y > 0.) {
        po = ro + rd * t;
        no = normalize(e.xyy * mp(po + e.xyy).x + e.yyx * mp(po + e.yyx).x + e.yxy * mp(po + e.yxy).x + e.xxx * mp(po + e.xxx).x);
        al = mix(vec3(.7, .05, 0), vec3(.5, .1, 0), .5 + .5 * sin(np.x * .5));
        if (z.y < 5.)
            al = vec3(0);
        if (z.y > 5.)
            al = vec3(1);
        if (z.y > 6.)
            al = vec3(.7, .2, .1);
        float dif = max(0., dot(no, ld)),
              fr = pow(1. + dot(no, rd), 4.);
        co = mix(mix(vec3(.8), vec3(1), abs(rd)) * al * (a(.1) * a(.3) + .2) * (dif + s(25.)), fo, min(fr, .2));
        co = mix(fo, co, exp(-.000005 * t * t * t));
    }
    pp = co + g * .2 * mix(vec3(.7, .1, 0), vec3(.5, .2, .1), .5 + .5 * sin(np.z * .2));
    pp += g2 * .2 * vec3(.1, .2, .5) * GlowIntensity;
    pp *= VignetteStrength;
    gl_FragColor = vec4(pow(pp * ColorIntensity, vec3(0.55)), 1);
}
