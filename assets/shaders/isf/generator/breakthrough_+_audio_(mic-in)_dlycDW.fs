 /*
 {
    "CATEGORIES": [
        "Converted", "Shadertoy"
    ],
    "DESCRIPTION": "Converted from Shadertoy. Added proper ISF buffer handling and tunable parameters.",
    "INPUTS": [
        { "NAME": "iChannel1", "TYPE": "audio" },
        { "NAME": "iChannel1_2", "TYPE": "audio" }
    ],
    "PASSES": [
        { "TARGET": "BufferA" },
        { }
    ]
}
*/
 
#define PI 3.141592
#define TAU 2.*PI
#define BEND .8     
#define COUNT 120.
#define DISTORT 1.5
#define COLOR 1.
#define SPIRAL 3.

float tt;

vec3 spectral_zucconi6(float x) {
    x = fract(x);
    const vec3 c1 = vec3(3.54585104, 2.93225262, 2.41593945);
    const vec3 x1 = vec3(0.69549072, 0.49228336, 0.27699880);
    const vec3 y1 = vec3(0.02312639, 0.15225084, 0.52607955);
    const vec3 c2 = vec3(3.90307140, 3.21182957, 3.96587128);
    const vec3 x2 = vec3(0.11748627, 0.86755042, 0.66077860);
    const vec3 y2 = vec3(0.84897130, 0.88445281, 0.73949448);
    return clamp((1. - c1 * (x - x1)) * y1 + (1. - c2 * (x - x2)) * y2, vec3(0), vec3(1));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = vec3(0);

    if (PASSINDEX == 0) {
        vec4 tex = IMG_NORM_PIXEL(BufferA, mod(uv, 1.0));
        tt = TIME * 0.2;
        uv *= .9;
        float tz = log(tt / 2.) * 2.;
        uv *= tz;
        float s = 1. / COUNT;
        for (float i = 0.; i < 1.; i += s) {   
            float z = fract(i - .1 * tt);
            col += spectral_zucconi6(z) * smoothstep(1., .9, z);
        }
        col = mix(col, tex.rgb, 0.7);
        gl_FragColor = vec4(col, 1.0);
    }
    else if (PASSINDEX == 1) {
        vec3 tex = IMG_NORM_PIXEL(BufferA, mod(uv, 1.0)).rgb;
        vec3 audio = IMG_NORM_PIXEL(iChannel1_2, mod(vec2(0.1, 0.25), 1.0)).rgb;
        gl_FragColor = vec4(tex * (audio.x * 3.0), 1);
    }
}
