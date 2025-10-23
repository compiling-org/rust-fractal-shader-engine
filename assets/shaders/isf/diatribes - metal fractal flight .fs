/*{
  "DESCRIPTION": "Fractal + Orb AO Tunnel with working shake, true palettes, brightness gain",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "Zoom",         "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "Speed",        "TYPE": "float", "MIN": 0.01, "MAX": 4.0, "DEFAULT": 1.0 },
    { "NAME": "Morph",        "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "Shake",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2 },
    { "NAME": "Shimmer",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 1.0 },
    { "NAME": "Saturation",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Contrast",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "Gain",         "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.5 },
    { "NAME": "ColorPulse",   "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.0 },
    { "NAME": "Palette",      "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 0.0 },
    { "NAME": "FractalDetail","TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 8.0 },
    { "NAME": "FractalScale", "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.5 },
    { "NAME": "FractalOffset","TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": -1.0 },
    { "NAME": "OrbRadius",    "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 0.05 },
    { "NAME": "OrbFollow",    "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "CamTheta",     "TYPE": "float", "MIN": -3.14, "MAX": 3.14, "DEFAULT": 0.0 },
    { "NAME": "CamPhi",       "TYPE": "float", "MIN": -1.57, "MAX": 1.57, "DEFAULT": 0.0 },
    { "NAME": "CamDist",      "TYPE": "float", "MIN": 0.5, "MAX": 10.0, "DEFAULT": 4.0 },
    { "NAME": "Tex0",         "TYPE": "image" },
    { "NAME": "Tex1",         "TYPE": "image" },
    { "NAME": "Tex2",         "TYPE": "image" }
  ]
}*/

#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define N normalize

bool orbHit = false;

// REPLACED: palettes with distinct, vibrant psychedelic variants
vec3 getPalette(int i, float t) {
  if (i == 0) return vec3(1.0, 0.5 + 0.5*sin(t*5.0), sin(t*3.1));
  if (i == 1) return vec3(0.6 + 0.4*sin(t*7.0), sin(t*3.0+1.0), 1.0);
  if (i == 2) return vec3(0.5+0.5*sin(t*4.1), 0.5+0.5*cos(t*2.7), 0.5+0.5*sin(t*3.9));
  if (i == 3) return vec3(sin(t*6.0)*0.7+0.3, cos(t*4.0)*0.6+0.4, sin(t*8.0)*0.7+0.3);
  if (i == 4) return vec3(fract(t*2.5), fract(t*1.3+1.0), fract(t*3.7+2.0));
  if (i == 5) return vec3(abs(sin(t*4.0)), abs(sin(t*5.0+1.0)), abs(cos(t*6.0)));
  if (i == 6) return vec3(0.8, 0.3 + 0.7*sin(t*2.0), 0.5 + 0.5*cos(t*9.0));
  return vec3(1.0);
}

// unchanged from your last good version
vec3 P(float z) {
    return vec3(
        tanh(cos(z * 0.4) * 0.2) * 8.0,
        tanh(cos(z * 0.25) * 0.3) * 16.0,
        z
    );
}

vec3 tex3D(sampler2D tex, in vec3 p, in vec3 n) {
    n = max((abs(n) - 0.2)*7., 0.001);
    n /= (n.x + n.y + n.z);  
    return (texture(tex, p.yz)*n.x + texture(tex, p.zx)*n.y + texture(tex, p.xy)*n.z).rgb;
}

float orb(vec3 p, float t) {
    return length(p - vec3(
        P(p.z).x + tanh(sin(p.z + t) * 0.1),
        P(p.z).y + sin(sin(p.z * 1.5) + t * 1.75) * 0.1,
        0.7 + t + tan(cos(t * 0.5) * 0.9) * 0.2 * OrbFollow
    ));
}

float fractal(vec3 p) {
    float w = 1.0, l;
    p *= FractalScale;
    p.y += FractalOffset;
    for (int i = 0; i < int(FractalDetail); i++) {
        p = abs(sin(p * Morph)) - 1.0;
        l = 1.5 / dot(p, p);
        p *= l;
        w *= l;
    }
    return length(p) / w;
}

float map(vec3 p, float t) {
    vec3 q = p;
    p.xy -= P(p.z).xy;
    float s = max(0.3 - abs(p.x), 0.3 - abs(p.y));
    s = min(s, fractal(p));
    float orbDist = orb(q, t) - OrbRadius;
    s = min(s, orbDist);
    orbHit = s == orbDist;
    return s;
}

float AO(vec3 pos, vec3 nor, float t) {
    float sca = 2.0, occ = 0.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + float(i) * 0.5 / 4.0;
        float dd = map(nor * hr + pos, t);
        occ += (hr - dd) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

void main() {
    vec2 u = isf_FragNormCoord * RENDERSIZE;
    vec2 r = RENDERSIZE;
    float t = TIME * Speed;
    float s = 0.002, d = 0.0, i = 0.0;

    vec3 cam = vec3(
        CamDist * cos(CamPhi) * sin(CamTheta),
        CamDist * sin(CamPhi),
        CamDist * cos(CamPhi) * cos(CamTheta)
    );
    vec3 target = P(t);
    vec3 Z = normalize(P(t + 1.0) - target);
    vec3 X = normalize(vec3(Z.z, 0.0, -Z.x));
    vec2 rotUv = rot(sin(t * 0.6) * 0.7) * (u - r * 0.5) / r.y;
    vec3 D = vec3(rotUv, 1.0 / Zoom) * mat3(-X, cross(X, Z), Z);

    // APPLY true 3D shake to ray direction
    D += vec3(
        sin(t * 10.0 + u.y * 20.0),
        cos(t * 13.0 + u.x * 15.0),
        sin(t * 7.0 + u.x * u.y * 0.01)
    ) * Shake * 0.2;

    vec3 p, ro = target;
    while (i++ < 200.0 && s > (0.001 * i * 0.02)) {
        p = ro + D * d;
        d += (s = map(p, t) * 0.8);
    }

    vec3 e = vec3(0.01, 0.0, 0.0);
    vec3 norm = normalize(vec3(
        map(p - e.xyy, t) - map(p + e.xyy, t),
        map(p - e.yxy, t) - map(p + e.yxy, t),
        map(p - e.yyx, t) - map(p + e.yyx, t)
    ));

    vec3 col;
    if (orbHit) {
        col = pow(tex3D(Tex0, p, norm), vec3(1.2)) * 2.0;
    } else if (mod(p.z, 10.0) > 5.0) {
        col = pow(tex3D(Tex1, p * 2.5, norm), vec3(2.2)) / pow(orb(p, t), 2.5);
    } else {
        col = pow(tex3D(Tex2, p * 2.5, norm), vec3(2.2)) * pow(orb(p, t), 2.5);
    }

    float pulse = sin((p.y + t) * 10.0 + ColorPulse) * 0.5 + 0.5;
    col *= getPalette(int(Palette), d * 0.1 + pulse * Shimmer);

    col *= max(dot(norm, normalize(ro - p)), 0.05);
    col *= AO(p, norm, t) * 4.0;
    col *= exp(-d / 4.0);
    col = pow(vec3(1.5, 1.0, 0.7) * col * Gain, vec3(0.45));

    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    col = mix(vec3(lum), col, Saturation);
    col = (col - 0.5) * Contrast + 0.5;
    col *= Brightness;

    gl_FragColor = vec4(col, 1.0);
}
