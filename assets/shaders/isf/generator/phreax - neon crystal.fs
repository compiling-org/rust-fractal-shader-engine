/*{
  "CATEGORIES": ["Generator", "Fractal", "Psychedelic"],
  "CREDIT": "Based on gaz fractal + extended by OpenAI Unity GPT",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "foldCount", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "fractalPower", "TYPE": "float", "DEFAULT": 2.2, "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "spacing", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.05, "MAX": 1.0 },
    { "NAME": "orbit", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "xyControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] }
  ]
}*/

#define PI 3.141592
#define PHI 1.61803
#define SIN(x) (.5+.5*sin(x))
#define R(p,a,r) mix(a*dot(p,a),p,cos(r))+sin(r)*cross(p,a)

mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

vec3 bump3y(vec3 x, vec3 yoffset) {
    vec3 y = 1. - x * x;
    return clamp(y - yoffset, vec3(0), vec3(1));
}

// Multiple palettes interpolated with float "palette"
vec3 getPalette(float t, float id) {
    vec3 p0 = vec3(sin(t*2.1), sin(t*3.7+0.5), sin(t*5.3+1.0));
    vec3 p1 = vec3(0.5+0.5*sin(t*PI*2.0), 0.5+0.5*cos(t*PI*1.5), 0.5+0.5*sin(t*PI*0.5));
    vec3 p2 = vec3(sin(t*8.0), cos(t*6.0), sin(t*4.0+1.5));
    vec3 p3 = vec3(sin(t*2.0), sin(t*5.0 + 0.5), cos(t*3.5 + 1.2));
    vec3 p4 = vec3(0.8, 0.6, 1.0)*vec3(sin(t*2.0), sin(t*3.0), cos(t*4.0));
    vec3 p5 = bump3y(vec3(3.5,2.9,2.4)*(t-vec3(0.7,0.5,0.3)), vec3(0.1));
    vec3 p6 = bump3y(vec3(3.9,3.2,3.9)*(t-vec3(0.1,0.8,0.6)), vec3(0.7));
    vec3 a = mix(p0, p1, smoothstep(0.,1.,fract(id)));
    vec3 b = mix(p2, p3, smoothstep(0.,1.,fract(id-2.)));
    vec3 c = mix(p4, mix(p5, p6, 0.5), smoothstep(0.,1.,fract(id-4.)));
    if(id < 2.0) return a;
    if(id < 4.0) return b;
    return c;
}

vec2 foldSym(vec2 p, float N, float spacing) {
    float t = atan(p.x, -p.y);
    t = mod(t + PI / N, 2.0 * PI / N) - PI / N;
    p = length(p) * vec2(cos(t), sin(t));
    p = abs(p) - spacing;
    p = abs(p) - spacing;
    return p;
}

vec3 applyFX(vec3 col, vec2 uv) {
    col *= mix(0.3, 1.0, (1.3 - pow(dot(uv, uv), 0.5))); // vignette
    col = mix(vec3(0.5), col, contrast);
    col = mix(vec3(dot(col, vec3(0.333))), col, saturation);
    col *= brightness;
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - xyControl) * zoom;

    // Shake as subtle screen warp
    if (shake > 0.0) {
        uv += vec2(sin(uv.y * 40. + TIME*5.), cos(uv.x * 40. - TIME*3.)) * 0.003 * shake;
    }

    vec3 col = vec3(0.08, 0.1, 0.12);
    float dm = SIN(-0.5 * PI + 0.21 * TIME * speed);
    float lw = mix(0.006, 0.002, dm); // line width

    vec3 p;
    float g = 0., e, l, s;

    for (float t = 0.0; t < 150.0; t++) {
        // Morph via nonlinear z-shift
        float morphShift = mix(6.0, 1.5, morph);
        p = vec3(g * uv, g - morphShift + morphShift * dm);

        // Rotate orbit
        float orbitSpeed = orbit * 2.0;
        p = R(p, normalize(vec3(1, 3, 5)), TIME * orbitSpeed);
        p = abs(p) + 0.1;

        if (p.y > p.x) p = p.yxz;
        if (p.z > p.x) p = p.zyx;
        if (p.y > p.z) p = p.xzy;

        p.xy = foldSym(p.xy, foldCount, spacing);
        s = 2.0;

        for (int j = 0; j < 3; j++) {
            s *= l = fractalPower / min(dot(p, p), 0.8);
            p.x = abs(p.x) - 0.01;
            p = abs(p) * l - vec3(
                2.0 + 0.4 * sin(TIME * 0.5),
                0.1 + 0.9 * SIN(0.1 * TIME),
                5.0
            );
        }

        p.xy = foldSym(p.xy, PHI, spacing * 0.5);
        p.yz *= rot(0.38 * 2.0 * PI);

        g += e = length(p.xz) / s;

        if (e < lw) {
            vec3 c = getPalette(g * 0.8 + 0.15 * TIME, palette);
            col += mix(vec3(1.0), c, 1.0) * 0.9 / t;
        }
    }

    // Glitch = spectral moiré interference
    if (glitch > 0.0) {
        vec2 moire = sin(uv * 50.0 + TIME * 5.0);
        vec3 overlay = getPalette(dot(moire, vec2(1.0, 1.5)) * 2.0, palette + 2.0);
        col = mix(col, col + overlay * 0.4, glitch);
    }

    col = pow(col * 1.2, vec3(1.1));
    col = applyFX(col, uv);
    gl_FragColor = vec4(col, 1.0);
}
