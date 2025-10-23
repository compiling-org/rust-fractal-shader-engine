/*{
  "CATEGORIES": ["Fractal", "Tunnel", "Visuals"],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "feedbackImage",
      "TYPE": "image"
    },
    {
      "NAME": "xyControl",
      "TYPE": "point2D",
      "DEFAULT": [0.5, 0.5]
    },
    {
      "NAME": "ColorPulse",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 5.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "ColorMode",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 6.0,
      "DEFAULT": 0.0
    },
    {
      "NAME": "Morph",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5
    },
    {
      "NAME": "Zoom",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "Speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 4.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "FractalType",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.0
    },
    {
      "NAME": "Brightness",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "Saturation",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "Contrast",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    }
  ]
}*/

#define PI 3.141592
#define TAU 6.283185
#define MAX_DIST 8.
#define FRACTAL_ITER 4
#define AA 2

float time;
vec3 l1,l2;

mat3 palette(int index, float t) {
    if(index == 0) return mat3(0.5 + 0.5*cos(t + vec3(0,2,4)),
                               0.5 + 0.5*cos(t + vec3(2,4,0)),
                               0.5 + 0.5*cos(t + vec3(4,0,2)));
    if(index == 1) return mat3(sin(t*vec3(1.2, 0.8, 1.5)),
                               cos(t*vec3(0.7, 1.1, 1.3)),
                               sin(t*vec3(1.7, 0.6, 1.2)));
    if(index == 2) return mat3(cos(t + vec3(2, 1, 0)),
                               cos(t + vec3(3, 2, 1)),
                               cos(t + vec3(4, 3, 2)));
    if(index == 3) return mat3(vec3(sin(t)), vec3(cos(t)), vec3(sin(t*2.0)));
    if(index == 4) return mat3(cos(t*vec3(1,2,3)),
                               sin(t*vec3(3,2,1)),
                               cos(t*vec3(2,1,3)));
    if(index == 5) return mat3(vec3(0.5 + 0.5*sin(t + vec3(1, 2, 3))),
                               vec3(0.5 + 0.5*cos(t + vec3(4, 5, 6))),
                               vec3(0.5 + 0.5*sin(t + vec3(7, 8, 9))));
    return mat3(vec3(1),vec3(1),vec3(1));
}

vec2 path(float z){ return vec2(sin(z*.5),0); }

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, -s, s, c);
}

vec3 getRayDir(vec2 uv, vec3 c, vec3 t, float z) {
    vec3 f = normalize(t - c);
    vec3 s = normalize(cross(vec3(0,1,0), f));
    vec3 u = cross(f, s);
    return normalize(f*z + uv.x*s + uv.y*u);
}

float map(vec3 p) {
    p.xy -= path(p.z);
    float d = -1e10;
    float s = 1.;
    for (int m=0; m<FRACTAL_ITER; m++) {
        vec3 a = mod(p*s, 2.) - 1.;
        s *= 3.;
        vec3 r = 1. - 3.*abs(a);
        r.yz *= rot(.25*PI*Morph);
        r = abs(r);

        float da, db, dc;
        if (FractalType < 0.5) {
            da = max(r.x,r.y);
            db = max(r.y,r.z);
            dc = max(r.z,r.x);
        } else if (FractalType < 1.5) {
            da = min(r.x,r.y);
            db = min(r.y,r.z);
            dc = min(r.z,r.x);
        } else {
            da = dot(r, vec3(0.577));
            db = da;
            dc = da;
        }

        float c = min(da,min(db,dc)) - 1.;
        d = max(d, c/s);
    }
    return d;
}

float intersect(vec3 ro, vec3 rd) {
    float t = 0.;
    for (int i = 0; i < 512; i++) {
        vec3 p = ro + rd * t;
        float h = map(p);
        t += h;
        if (h < .0001 || t > MAX_DIST) break;
    }
    return t;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(.0001,0);
    float h = map(p);
    return normalize(h - vec3(map(p - e.xyy), map(p - e.yxy), map(p - e.yyx)));
}

float calcAO(vec3 p, vec3 n) {
    float occ = 0., sca = 1.;
    for(int i=0; i<4; i++) {
        float h = .01 + .12*float(i)/4.;
        float d = map(p + h * n);
        occ += (h-d)*sca;
        sca *= .95;
        if( occ>.35 ) break;
    }
    return clamp(1. - 3.*occ, 0., 1.);
}

vec3 render(vec3 ro, vec3 rd, float tval, float pulse, int palID) {
    vec3 col = vec3(0);
    float t = intersect(ro, rd);
    if (t < MAX_DIST) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);
        vec3 mat = vec3(.9,.85,1);
        vec3 xy = IMG_NORM_PIXEL(inputImage,mod(p.xy*4.,1.0)).rgb;
        vec3 xz = IMG_NORM_PIXEL(inputImage,mod(p.xz*4.,1.0)).rgb;
        vec3 yz = IMG_NORM_PIXEL(inputImage,mod(p.yz*4.,1.0)).rgb;
        vec3 m = abs(n);
        vec3 tex = yz*m.x + xz*m.y + xy*m.z;
        tex *= tex;

        vec3 lig = normalize(l1 - p);
        vec3 lig2 = normalize(l2 - p);
        float dif = clamp(dot(n, lig),0.,1.);
        float dif2 = clamp(dot(n, lig2),0.,1.);
        float occ = calcAO(p, n);
        float focc = smoothstep(sign(p.y),0.,p.y);
        occ *= .4+.6*focc;

        mat3 pal = palette(palID, tval * pulse);
        col += .2*mat*tex*occ;
        col += 3.*tex*(pal[0])*pow(dif,8.)*occ*occ;
        col += 2.*tex*(pal[1])*pow(dif2,6.)*occ*occ;
        col += 1.5*tex*(pal[0])*pow(dif,16.)*occ*occ;
        col += tex*(pal[2])*pow(dif2,12.)*occ*occ;

        float fog = 1.-exp(-.05*t*t*t);
        col = mix(col, vec3(0), fog);
    }
    return col;
}

vec3 ACES(vec3 x) {
    float a = 2.51, b = .03, c = 2.43, d = .59, e = .14;
    return (x*(a*x+b))/(x*(c*x+d)+e);
}

void main() {
    vec2 m = xyControl;
    vec3 tot = vec3(0);
    for (int i = 0; i < AA; i++) {
        for (int j = 0; j < AA; j++) {
            vec2 o = vec2(i, j) / float(AA) - .5;
            vec2 uv = (gl_FragCoord.xy+o - .5*RENDERSIZE.xy) / RENDERSIZE.y;

            float mb = float(i*AA+j) + IMG_PIXEL(feedbackImage, mod(gl_FragCoord.xy, RENDERSIZE.xy) / RENDERSIZE.xy).x;
            time = TIME + 3.*m.x - .02*mb/float(AA*AA);

            vec3 ro = vec3(0,0,1.5*time*Speed);
            vec3 ta = ro + vec3(0,0,.1);
            l1 = ro + vec3(0,0,.4);
            l2 = ro + vec3(0,0,.9);

            ro.xy += path(ro.z);
            ta.xy += path(ta.z);
            l1.xy += path(l1.z);
            l2.xy += path(l2.z);

            vec3 rd = getRayDir(uv, ro, ta, .8*Zoom);
            rd.xy *= rot(path(ta.z).x*.25);

            vec3 col = render(ro, rd, time, ColorPulse, int(ColorMode));
            tot += col;
        }
    }
    tot /= float(AA*AA);
    tot = pow(tot, vec3(.4545));
    tot = mix(vec3(0.5), tot, Contrast);
    tot = mix(vec3(dot(tot, vec3(0.299, 0.587, 0.114))), tot, Saturation);
    tot *= Brightness;
    vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
    tot *= clamp(pow(80. * p.x*p.y*(1.-p.x)*(1.-p.y), .1), 0., 1.);
    gl_FragColor = vec4(tot,1.0);
}
