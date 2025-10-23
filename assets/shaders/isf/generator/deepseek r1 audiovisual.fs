/* {
    "CATEGORIES": [
        "Automatically Converted", 
        "Shadertoy"
    ],
    "DESCRIPTION": "Psychedelic fractal kaleidoscope visualizer with tunable parameters.",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "AudioReactivity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "BackgroundColor",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.1, 0.1, 1.0]
        },
        {
            "NAME": "ColorIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "MorphingSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "DistortionAmount",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "SparkleFrequency",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1000.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "ColorPalette",
            "TYPE": "long",
            "VALUES": ["Rainbow", "Electric", "Pastel"],
            "DEFAULT": "Rainbow"
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {}
    ]
} */

#define PI 3.14159265359
#define TAU (PI * 2.0)

vec2 uv;
float d = 1e6;

const vec2 ch_size = vec2(1.0, 2.0) * 0.6;
const vec2 ch_space = ch_size + vec2(1.5, 1.0);
const vec2 ch_start = vec2(ch_space.x * -5.0, 1.0);
vec2 ch_pos = vec2(0.0, 0.0);

vec3 hsv2rgb_smooth(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

float getSound() {
    float s = 0.0;
    for (float i = 0.0; i < 20.0; i++) {
        s += sin(TIME * AudioReactivity + i) * AudioReactivity;
    }
    return clamp(s / 20.0, 0.0, 1.0);
}

vec3 fractal(vec2 p) {
    p = vec2(abs(atan(p.x, p.y) / PI * sin(TIME * 0.1) * 6.0), length(p));
    p.y -= tan(TIME * 0.1);
    p.x = fract(p.x - TIME) + 0.5;
    vec2 m = vec2(1000);
    float ml = 100.0;
    for (int i = 0; i < 4; i++) {
        p = abs(p) / clamp(abs(p.x * p.y), 0.1, 0.6) - 3.0;
        m = min(m, abs(p)) + fract(p.x * 0.2 + TIME * 0.5) + fract(p.y * 0.2 + TIME);
        ml = min(ml, length(p));
    }
    m = exp(-1.5 * m);
    vec3 c = vec3(m.x * 3.0, length(m), m.y * 2.0) * 1.0;
    ml = exp(-3.0 * ml) * 3.0 * getSound();
    c += ml;
    return c;
}

// New segment checking without bit operations
bool checkSegment(int value, int mask) {
    return (float(value) / float(mask) - floor(float(value) / float(mask))) >= 0.5;
}

void ddigit(int n, int nn) {
    float v = 1e6;
    vec2 cp = uv - ch_pos;

    if (n == 0 && nn == 0) v = min(v, distance(cp, vec2(-0.405, -1.000)));
    if (checkSegment(nn, 4)) v = min(v, distance(cp, vec2(-0.438, 1.000)));
    if (checkSegment(nn, 2)) v = min(v, distance(cp, vec2(0.063, 1.000)));
    if (checkSegment(nn, 32)) v = min(v, distance(cp, vec2(-0.438, -1.000)));
    if (checkSegment(nn, 64)) v = min(v, distance(cp, vec2(0.063, -1.000)));
    if (checkSegment(n, 1)) v = min(v, distance(cp, vec2(0.063, 0.000)));
    if (checkSegment(n, 16)) v = min(v, distance(cp, vec2(-0.438, 0.000)));

    ch_pos.x += ch_space.x;
    d = min(d, v);
}

void renderText() {
    ch_pos = ch_start;
    ddigit(0x11, 0x9F);
    ddigit(0x11, 0x77);
    ddigit(0x11, 0xEE);
    ddigit(0x11, 0x9F);
}

void main() {
    if (PASSINDEX == 0) {
        vec2 aspect = RENDERSIZE.xy / RENDERSIZE.y;
        uv = (gl_FragCoord.xy / RENDERSIZE.y) - aspect / 2.0;
        uv *= 35.0;
        uv -= vec2(-10.0, 4.0);
        uv.x += 3.0 + sin(uv.y * 1.5 + TIME * 3.0) * 0.15;

        renderText();

        float a = smoothstep(0.4, 0.2, d);
        float b = smoothstep(0.5, 0.4, d) * 0.8;

        vec3 ch_color;
        if (ColorPalette == 0) { // Rainbow
            ch_color = hsv2rgb_smooth(vec3(TIME * 0.4 + uv.y * 0.1, 0.5, 1.0));
        } else if (ColorPalette == 1) { // Electric
            ch_color = vec3(
                0.5 + 0.5 * sin(TIME * 0.7),
                0.5 + 0.5 * sin(TIME * 0.7 + 2.0),
                0.5 + 0.5 * sin(TIME * 0.7 + 4.0)
            );
        } else { // Pastel
            ch_color = vec3(
                0.7 + 0.3 * sin(TIME * 0.4),
                0.7 + 0.3 * sin(TIME * 0.4 + 1.0),
                0.7 + 0.3 * sin(TIME * 0.4 + 2.0)
            ) * 1.5;
        }
        
        vec3 col = ch_color * a * ColorIntensity;
        gl_FragColor = vec4(col, max(a, b));
    } else if (PASSINDEX == 1) {
        float s = getSound();
        uv = -1.0 + 2.0 * gl_FragCoord.xy / RENDERSIZE.xy;
        uv.x *= RENDERSIZE.x / RENDERSIZE.y;
        uv.x *= 1.0 - uv.y * 0.7;
        uv *= Zoom - s * DistortionAmount;

        vec3 c = fractal(uv);
        c += exp(-2.0 * length(uv)) * (1.0 + 4.0 * s * s);
        c *= vec3(1.2, 0.9, 0.8);
        
        // Sparkle effect
        float sparkle = sin(uv.x * SparkleFrequency) * sin(uv.y * SparkleFrequency * 1.3) * sin(TIME * 3.0);
        sparkle = pow(abs(sparkle), 10.0) * 2.0;
        c += vec3(sparkle);

        vec3 bg = BackgroundColor.rgb;
        c = mix(bg, c, smoothstep(0.0, 0.1, length(uv)));

        // Pulsing effect
        float pulse = 0.5 + 0.5 * sin(TIME * MorphingSpeed * 2.5);
        c *= 1.0 + pulse * 0.3;

        gl_FragColor = vec4(c, 1.0);
    }
}
