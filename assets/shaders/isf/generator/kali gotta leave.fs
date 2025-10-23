/* {
    "CATEGORIES": [
        "Psychedelic",
        "3D",
        "Generative"
    ],
    "DESCRIPTION": "Generative 3D effect with tunable parameters.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "TimeScale",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Time Scale"
        },
        {
            "NAME": "RotationSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Rotation Speed"
        },
        {
            "NAME": "Scale",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Scale"
        },
        {
            "NAME": "Detail",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 4.0,
            "LABEL": "Detail Level"
        },
        {
            "NAME": "ColorIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Intensity"
        },
        {
            "NAME": "Morphing",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Morphing Factor"
        },
        {
            "NAME": "TunnelSize",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0,
            "LABEL": "Tunnel Size"
        },
        {
            "NAME": "ColorPulseSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Pulse Speed"
        },
        {
            "NAME": "ColorScheme",
            "TYPE": "long",
            "VALUES": ["0: Rainbow", "1: Electric", "2: Fire", "3: Ocean", "4: Pastel"],
            "DEFAULT": "0",
            "LABEL": "Color Scheme"
        }
    ]
} */

#define time TIME

float det = 0.001, it;
float cyl;
float so = 0.;
float id = 0.;

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

float hash(vec2 p) {
    p *= 1000.;
    vec3 p3 = fract(vec3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 path(float t) {
    float s = sin(t * 0.1 + cos(t * 0.05) * 2.);
    float c = cos(t * 0.3);
    return vec3(s * s * s, c * c, t);
}

float de(vec3 p) {
    id = 0.;
    p.xy -= path(p.z).xy;
    p.xy *= rot(p.z * 1. + time * RotationSpeed + so * 50.);
    vec3 p2 = p + sin(p.z) * .2;
    float m = 1000., sc = 1.;
    float s = sin(p.z * .7) * .7;
    float sph = length(p.xy) - (1.3 + TunnelSize * 0.5) - s; // Adjusted for tunnel size
    cyl = length(p2.xy + .5 * s + .1) - .05 - fract(-p.z + time * 3.) * .05;
    cyl *= .7;
    for (int i = 0; i < 8; i++) {
        float s = 2.;
        p.xy = sin(p.xy);
        p.xy *= rot(1.);
        p.xz *= rot(1.6);
        p = p * s;
        sc *= s;
        float l = length(p.xy) - .2;
        m = min(m, l);
        if (m == l) it = float(i);
    }
    float d = m / sc * 2.;
    d = max(d, -sph);
    d = min(d, cyl);
    if (d == cyl) id = 1.;
    return d;
}

vec3 normal(vec3 p) {
    vec2 e = vec2(0., det);
    return normalize(vec3(de(p + e.yxx), de(p + e.xyx), de(p + e.xxy)) - de(p));
}

vec3 march(vec3 from, vec3 dir) {
    vec3 p, col = vec3(0.);
    float d, td = 0., maxdist = 8.;
    float g = 0.;
    float r = hash(dir.xy + time);
    for (int i = 0; i < 200; i++) {
        p = from + dir * td;
        d = de(p) * (1. - r * .2);
        if (d < det || td > maxdist) break;
        td += d;
        g += .1 / (.1 + cyl * 5.);
    }
    if (d < det) {
        vec3 n = normal(p);
        col = normalize(1. + dir) * .3 * max(0., -n.z);
        if (mod(floor(-time * 4. + p.z * 1.5), 8.) == it) col += 1.;
        if (id == 1.) col = vec3(1., .5, .3) * max(0., n.x);
    } else td = maxdist;
    col.rb *= rot(dir.y * 1.5);
    col = mix(col, vec3(1., .7, .5) * exp((-1.5 + so * 150.) * length(p.xy - path(p.z).xy)) * (1.5 + so * 70.), pow(td / maxdist, 1.5));
    col += g * .05 * vec3(1., 0.5, .3);
    
    // Apply color pulse effect
    float pulse = 0.5 + 0.5 * sin(time * ColorPulseSpeed);
    col *= pulse;

    return col;
}

vec3 getColor(vec3 col, int scheme) {
    if (scheme == 0) { // Rainbow
        return col * vec3(1.0, 0.5 + 0.5 * sin(time * 2.0), 0.5 + 0.5 * cos(time * 2.0));
    } else if (scheme == 1) { // Electric
        return col * vec3(abs(sin(time * 3.0)), abs(sin(time * 3.0 + 1.0)), abs(sin(time * 3.0 + 2.0)));
    } else if (scheme == 2) { // Fire
        return col * vec3(1.0, 0.5 + 0.5 * sin(time * 2.0), 0.2);
    } else if (scheme == 3) { // Ocean
        return col * vec3(0.2, 0.5 + 0.5 * sin(time * 2.0), 0.7);
    } else { // Pastel
        return col * vec3(0.6 + 0.4 * sin(time * 2.0), 0.6 + 0.4 * sin(time * 2.0 + 1.0), 0.6 + 0.4 * sin(time * 2.0 + 2.0));
    }
}

mat3 lookat(vec3 dir) {
    vec3 up = vec3(0., 1., 0.);
    vec3 rt = cross(up, dir);
    return mat3(rt, cross(dir, rt), dir);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * .5) / RENDERSIZE.y;
    uv *= Scale + sin(length(uv) * 10. + time) * .1;
    float t = time * TimeScale;
    vec3 from = path(t);
    from.x += .5;
    vec3 adv = path(t + 1.);
    vec3 rdir = normalize(adv - from);
    vec3 dir = normalize(vec3(uv, 1.));
    dir = lookat(rdir) * dir;
    dir *= 1. + tan(t * .2) * .2;

    vec3 col = march(from, dir);
    col = getColor(col, int(ColorScheme)); // Apply color scheme
    gl_FragColor = vec4(col * ColorIntensity, 1.0);
}