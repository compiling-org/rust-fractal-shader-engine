

/*
{
    "CATEGORIES": [
        "Hexagons",
        "Dance Party"
    ],
    "DESCRIPTION": "A hexagonal dance party shader with tunable parameters for customization.",
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 10.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "HexScale",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "ColorSeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "MouseX",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "MouseY",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        }
    ],
    "IMPORTED": {},
    "PASSES": [
        {
            "TARGET": "BufferA",
            "FLOAT": true,
            "PERSISTENT": true
        }
    ]
}
*/

#define R3 1.732051

vec4 HexCoords(vec2 uv) {
    vec2 s = vec2(1, R3);
    vec2 h = .5 * s;
    vec2 gv = s * uv;

    vec2 a = mod(gv, s) - h;
    vec2 b = mod(gv + h, s) - h;

    vec2 ab = dot(a, a) < dot(b, b) ? a : b;
    vec2 st = ab;
    vec2 id = gv - ab;

    return vec4(st, id);
}

float GetSize(vec2 id, float seed) {
    float d = length(id);
    float t = TIME * Speed; // Use tunable Speed parameter
    float a = sin(d * seed + t) + sin(d * seed * seed * 10.0 + t * 2.0);
    return a / 2.0 + 0.5;
}

mat2 Rot(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(c, -s, s, c);
}

float Hexagon(vec2 uv, float r, vec2 offs) {
    uv *= Rot(mix(0.0, 3.1415, r));

    r /= 1.0 / sqrt(2.0);
    uv = vec2(-uv.y, uv.x);
    uv.x *= R3;
    uv = abs(uv);

    vec2 n = normalize(vec2(1, 1));
    float d = dot(uv, n) - r;
    d = max(d, uv.y - r * 0.707);

    d = smoothstep(0.06, 0.02, abs(d));

    d += smoothstep(0.1, 0.09, abs(r - 0.5)) * sin(TIME);
    return d;
}

float Xor(float a, float b) {
    return a * (1.0 - b) + b * (1.0 - a);
}

float Layer(vec2 uv, float s) {
    vec4 hu = HexCoords(uv * 2.0);
    float d = Hexagon(hu.xy, GetSize(hu.zw, s), vec2(0));
    vec2 offs = vec2(1, 0);
    d = Xor(d, Hexagon(hu.xy - offs, GetSize(hu.zw + offs, s), offs));
    d = Xor(d, Hexagon(hu.xy + offs, GetSize(hu.zw - offs, s), -offs));

    offs = vec2(0.5, 0.8725);
    d = Xor(d, Hexagon(hu.xy - offs, GetSize(hu.zw + offs, s), offs));
    d = Xor(d, Hexagon(hu.xy + offs, GetSize(hu.zw - offs, s), -offs));

    offs = vec2(-0.5, 0.8725);
    d = Xor(d, Hexagon(hu.xy - offs, GetSize(hu.zw + offs, s), offs));
    d = Xor(d, Hexagon(hu.xy + offs, GetSize(hu.zw - offs, s), -offs));

    return d;
}

float N(float p) {
    return fract(sin(p * 123.34) * cos(345.456));
}

vec3 Col(float p, float offs) {
    float n = N(cos(p)) * 1.34;
    return cos(n * vec3(12.23, 45.23, 56.2) + offs * 3.0) * 0.5 + 0.5;
}

vec3 GetRayDir(vec2 uv, vec3 p, vec3 lookat, float zoom) {
    vec3 f = normalize(lookat - p),
         r = normalize(cross(vec3(0, 1, 0), f)),
         u = cross(f, r),
         c = p + f * zoom,
         i = c + uv.x * r + uv.y * u,
         d = normalize(i - p);
    return d;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 UV = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
    float duv = dot(UV, UV);

    // Simulate mouse input using tunable MouseX and MouseY
    vec2 m = vec2(MouseX, MouseY);

    float t = TIME * 0.2 + m.x * 10.0 + 5.0;

    float y = sin(t * 0.5);
    vec3 ro = vec3(0, 20.0 * y, -5);
    vec3 lookat = vec3(0, 0, -10);
    vec3 rd = GetRayDir(uv, ro, lookat, Zoom); // Use tunable Zoom parameter

    vec3 col = vec3(0.5);

    vec3 p = ro + rd * (ro.y / rd.y);
    float dp = length(p.xz);

    if ((ro.y / rd.y) > 0.0)
        col *= 0.05;
    else {
        uv = p.xz * 0.1;
        uv *= mix(1.0, 5.0, sin(t * 0.5) * 0.5 + 0.5);
        uv *= Rot(t);
        m *= Rot(t);
        uv.x *= R3;

        for (float i = 0.0; i < 1.0; i += 1.0 / 3.0) {
            float id = floor(i + t);
            float tt = fract(i + t);
            float z = mix(5.0, 0.1, tt);
            float fade = smoothstep(0.0, 0.3, tt) * smoothstep(1.0, 0.7, tt);
            col += fade * tt * Layer(uv * z, N(i + id)) / Col(id, duv);
        }
    }

    col *= 2.0;

    if (ro.y < 0.0)
        col = 1.0 - cos(col);

    col *= smoothstep(18.0, 5.0, dp);
    col *= 1.0 - duv * 2.0;

    gl_FragColor = vec4(col, 1.0);
}