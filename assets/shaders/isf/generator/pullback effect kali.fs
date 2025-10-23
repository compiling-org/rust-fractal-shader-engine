/*
{
    "CATEGORIES": [
        "Raymarching",
        "Psychedelic",
        "Abstract",
        "4k Intro"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/7t2yDz by Kali.  4k intro presented at Revision 2022 demoparty by LIA (Latitude Independent Association), now with many tunable parameters.",
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "BufferA",
            "PERSISTENT": true
        },
        {
        }
    ],
    "INPUTS": [
        { "NAME": "OverallAnimationSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Animation Speed" },
        { "NAME": "GlobalTimeOffset", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 1.2, "LABEL": "Global Time Offset" },

        { "NAME": "MainColor1", "TYPE": "color", "DEFAULT": [0.6, 1.0, 0.5, 1.0], "LABEL": "Main Color A" },
        { "NAME": "MainColor2", "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.5, 1.0], "LABEL": "Main Color B" },
        { "NAME": "GlowColor1", "TYPE": "color", "DEFAULT": [0.5, 1.0, 0.5, 1.0], "LABEL": "Glow Color A" },
        { "NAME": "GlowColor2", "TYPE": "color", "DEFAULT": [1.5, 0.5, 0.5, 1.0], "LABEL": "Glow Color B" },
        { "NAME": "PaletteBlend", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "LABEL": "Palette Blend" },

        { "NAME": "GlobalColorPulseSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.7, "LABEL": "Color Pulse Speed" },
        { "NAME": "GlowPulseStrength", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "LABEL": "Glow Pulse Strength" },

        { "NAME": "FractalZoomFactor", "TYPE": "float", "MIN": 0.01, "MAX": 0.5, "DEFAULT": 0.05, "LABEL": "Fractal Zoom" },
        { "NAME": "FractalClampX", "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 0.15, "LABEL": "Fractal Clamp X" },
        { "NAME": "FractalClampY", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 5.0, "LABEL": "Fractal Clamp Y" },
        { "NAME": "FractalShiftX", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.5, "LABEL": "Fractal Shift X" },
        { "NAME": "FractalShiftY", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Fractal Shift Y" },
        { "NAME": "FractalIterationTimeScale", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "LABEL": "Fractal Iteration Time Scale" },
        { "NAME": "FractalOutputBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "LABEL": "Fractal Output Brightness" },
        { "NAME": "FractalTypeSwitchTime", "TYPE": "float", "MIN": 0.0, "MAX": 100.0, "DEFAULT": 75.0, "LABEL": "Fractal Type Switch Time" },

        { "NAME": "FractalOffsetAmount", "TYPE": "point2D", "DEFAULT": [1.5, 1.0], "LABEL": "Fractal Offset" },

        { "NAME": "ObjectRotationSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.5, "LABEL": "Object Rotation Speed" },
        { "NAME": "ObjectRotationAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "LABEL": "Object Rotation Amplitude" },

        { "NAME": "RoadPatternStrength", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 3.0, "LABEL": "Road Pattern Strength" },
        { "NAME": "RoadPatternScale", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.2, "LABEL": "Road Pattern Scale" },
        { "NAME": "CarBodySize", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 3.0, "LABEL": "Car Body Size" },
        { "NAME": "CarBodyDistortion", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "LABEL": "Car Body Distortion" },
        { "NAME": "Reflectivity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "LABEL": "Reflectivity" },
        { "NAME": "EmissionStrength", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 5.0, "LABEL": "Emission Strength" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Global Brightness" }
    ]
}

*/

#define time (TIME * OverallAnimationSpeed + GlobalTimeOffset) // Tunable time scaling and offset

#define resolution RENDERSIZE

#define rot(a) mat2(cos(a),sin(a),-sin(a),cos(a))
#define sm(a,b) smoothstep(a,b,time)
#define hash(p) fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453)

// Global variables (retained from original for structure, but some now influenced by inputs)
float st, det = .01, t_internal, sc = 0., on = 0., tr = 1., mat = 0., y;
vec3 col = vec3(0), carpos, cardir;
// 'pal' and 'glcol' will now be calculated based on new inputs
vec2 pf1, pf2, pf3, e = vec2(0, .001);


mat3 lookat(vec3 dir) {
    dir = normalize(dir);
    vec3 rt = normalize(vec3(-dir.z, 0, dir));
    return mat3(rt, cross(rt, dir), dir);
}


float is(float s) {
    return step(abs(sc - s), .1);
}

vec3 path(float tt) {
    // Path generation, influenced by animation speed implicitly through 'time'
    vec3 p = vec3(sin(tt * .5 + cos(tt * .2)) + sin(tt * .1), 5., tt);
    tt += 70. * step(time, 80.);
    p.y -= smoothstep(290., 280., tt) * 10. + smoothstep(270., 265., tt) * 5. + smoothstep(240., 235., tt) * 5.;
    p.x *= .5 + tr * .5;
    p.x *= sm(57., 55.) + sm(89., 91.);
    return p;
}

vec3 carpath(float t_cp) {
    // Car path generation, based on main path but with specific offsets and rotations
    vec3 p = path(t_cp);
    p.y += 1. - tr + sm(52., 55.) * 4. * sm(82., 80.);
    p.x *= sm(55., 52.) + sm(105., 107.);
    p.z -= 375.;
    p.xz *= rot(-sm(75., 80.) * 3.1416); // Time-based rotation for car path
    p.z += 375.;
    if (time > 89.) p = path(p.z);
    return p;
}

vec3 fractal(vec2 p)
{
    // Fractal generation function with new tunable parameters
    p = abs(fract(p * FractalZoomFactor) - .5); // FractalZoomFactor
    float ot1 = 1000., ot2 = ot1;
    for (float i = 0.; i < 6.; i++) {
        // FractalOffsetAmount (XY)
        p = abs(p) / clamp(abs(p.x * p.y), FractalClampX, FractalClampY) - FractalOffsetAmount;
        ot1 = min(ot1, abs(p.x) + step(fract(time * FractalIterationTimeScale + float(i) * .2), .5 * p.y)); // FractalIterationTimeScale
        ot2 = min(ot2, length(p));
    }
    ot1 = smoothstep(.1, .05, ot1);
    // FractalTypeSwitchTime
    return time < FractalTypeSwitchTime ? vec3(p, 0) * ot2 * ot1 * FractalOutputBrightness + ot1 * FractalOutputBrightness : vec3(p.x, -1, p.y * .5) * ot2 * ot1 * FractalOutputBrightness + ot1 * FractalOutputBrightness; // FractalOutputBrightness
}


float map(vec2 p) {
    // Ground mapping function
    if (y > 10.) return 0.;
    vec2 ppp = p.yx; ppp.x -= 311.;
    vec3 pa = path(p.y);
    float h = 0.;
    p.x -= pa.x * sm(24., 25.);
    float d = floor(p.y * 3.) / 3. - carpos.z - sm(52., 57.) * 20.;
    p.x *= 1. + smoothstep(0., 2., d) * 2. * is(1.);
    pf1 = p;
    if (time < 24.) {
        p -= carpos.xz;
        p *= rot(-.5 * time / max(.5, floor(length(p))));
        pf2 = vec2((atan(p.x, p.y) / 3.1416), length(p));
        return pa.y - .5 - floor(length(p)) * sm(18., 17.) * (sm(5., 8.) - .5) * .7;
    }
    float b = step(300. + step(75., time) * 46., p.y);
    p = floor(p * RoadPatternScale) / RoadPatternScale; // RoadPatternScale
    pf3 = p;
    h += hash(p) * RoadPatternStrength * sm(24., 26.) * (1. - b * .9) * sm(550., 500.); // RoadPatternStrength
    if (sc > 1.) p = floor(p), h += (clamp(hash(p + .1), .75, 1.) - .75) * 20.;
    if (time > 22. && b < .5) h *= smoothstep(0.5, 5. - d, abs(p.x) * 1.5) * (sc > 1. ? 2. : 1.); // barre
    h += pa.y - .5;
    return h;
}

float de(vec3 p) {
    // Distance Estimator function for internal objects/car
    p -= carpos;
    st = .1;
    // CarBodySize, CarBodyDistortion
    float bound = length(p * vec3(1, CarBodyDistortion, 1)) - CarBodySize + tr;
    if (bound > 0.) return bound + 5.;
    p = lookat(cardir * vec3(.5, 0, 1)) * p;
    // ObjectRotationSpeed, ObjectRotationAmplitude
    p.xy *= rot(sin(time * ObjectRotationSpeed) * ObjectRotationAmplitude + cardir.x);
    p.yz *= rot(t_internal * 1.5 * step(.2, tr));
    p.xz *= rot(.5 * tr);
    float mat1 = exp(-.8 * length(sin(p * 6.))); // Could add DetailFrequency here
    float d1 = length(p) - .5; // Could add InnerShapeSize here
    float d = 100.;
    p.xy *= rot(smoothstep(.3, .5, abs(p.x)) * sign(p.x) * .2);
    p.y *= 1.2 + smoothstep(.3, .4, abs(p.x));
    p.x *= 1. - min(.7, abs(p.z - .4));
    p.z += smoothstep(0., .6, p.x * p.x);
    p.z -= smoothstep(.1, .0, abs(p.x)) * .5 * min(p.z, 0.);
    d = length(p) - .5;
    d += abs(p.y) * smoothstep(.6, .3, abs(p.x));
    p.y += 5.;
    d = mix(d, d1, sqrt(tr));
    mat = mix(exp(-.8 * length(sin(p * 6.3))), mat1, tr) + step(abs(p.x), .03) * .1; // Could add DetailFrequency
    mat *= min(1., on * 4.);
    if (d < 2.) st = .05;
    return d * .6;
}

vec4 hit(vec3 p) {
    float h = map(p.xz), d = de(p);
    return vec4(p.y < h, d < det * 2., h, d);
}

vec3 bsearch(vec3 p, vec3 dir) {
    float ste = st*-.5;
    float h2 = 1.0;
    // Corrected for loop: initialize 'i' directly in the loop statement with a constant
    for (float i_loop_bsearch = 1.0; i_loop_bsearch < 21.0; i_loop_bsearch++)
    {
        p += dir * ste;
        vec4 hi = hit(p);
        float h = max(hi.x, hi.y);
        if (abs(h - h2) > .001) {
            ste *= -.5;
            h2 = h;
        }
    }
    return p;
}

vec3 march(vec3 from, vec3 dir) {
    vec3 p, cl = vec3(0.), pr = p;
    float td = 2. + hash(dir.xy + time) * .1, g = 0., eg = 0., ref = 0.;
    p = from + td * dir;
    vec4 h;
    for (int i = 0; i < 300; i++) {
        p += dir * st;
        y = p.y;
        td += st;
        h = hit(p);
        if (h.y > .5 && ref == 0.) {
            pr = p;
            ref = 1.;
            p -= .2 * dir;
            for (int i = 0; i < 20; i++) {
                float d = de(p) * .5;
                p += d * dir;
                if (d < det) break;
            }
            dir = reflect(dir, normalize(vec3(de(p + e.yxx), de(p + e.xyx), de(p + e.xxy)) - de(p)));
            p += hash(dir.xy + time) * .1 * dir;
        }
        g = max(g, max(0., .2 - h.w) / .2) * mat;
        eg += .01 / (.1 + h * h * 20.).w * mat; // Could add GlowFalloff parameters
        if (h.x > .5 || td > 25. || (h.y > .5 && mat > .4)) break;
    }
    if (h.x > .9) {
        p -= dir * det;
        p = bsearch(p, dir);
        vec3 ldir = normalize(p - (carpos + vec3(0., 2., 0.)));
        vec3 n = normalize(vec3(map(p.xz + e.yx) - map(p.xz - e.yx), 2. * e.y, map(p.xz + e.xy) - map(p.xz - e.xy)));;
        n.y *= -1.;
        float cam = max(.2, dot(dir, n)) * step(on, .9 - is(3.)) * .8;
        // Use MainColor palette, controlled by MainColorBlend and ReflectedLight
        cl = (max(cam * .3, dot(ldir, n)) * on + cam) * .8 * mix(MainColor1.rgb * MainColor1.a, MainColor2.rgb * MainColor2.a, PaletteBlend);
        float dl = length(p.xz - carpos.xz) * 1.3 * (1. - sm(52., 55.) * .5);
        cl *= min(Reflectivity, exp(-.15 * dl * dl)); // Reflectivity
        cl += (fractal(pf1) * sm(20., 22.) + fractal(pf2 * 5.) * sm(25., 23.) + fractal(pf3 * .2) * 2. * float(1. < sc) * -n.y + fractal(p.xy).g * n.z * 2. * is(2.) + .7 * step(abs(pf1.x), .3) * step(.7, fract(pf1.y * 4.)) * step(pf1.y, 292.)
            * step(1.5, sc)) * exp(-.3 * dl) * .7;
        mat = 0.;
    }
    else {
        // Use MainColor palette for background reflection/glow
        cl = mix(MainColor1.rgb * MainColor1.a, MainColor2.rgb * MainColor2.a, PaletteBlend) * ref * .3 + smoothstep(7., 0., length(p.xz)) * .13;
    }
    if (td > 25.) cl = fractal(p.xz * .2) * max(0.,dir.y);
    cl = mix(cl, vec3(ref), Reflectivity * ref) + exp(-.3 * length(p + vec3(0, 17, -157))) * (mix(GlowColor1.rgb * GlowColor1.a, GlowColor2.rgb * GlowColor2.a, PaletteBlend) * GlowPulseStrength) * EmissionStrength * is(3.); // GlowColor, GlowPulseStrength, EmissionStrength
    p -= carpos;
    if (time > 80. && time < 89. && length(p) < 2.) cl += fractal(p.zx*2.);
    return cl + (g + eg) * (mix(GlowColor1.rgb * GlowColor1.a, GlowColor2.rgb * GlowColor2.a, PaletteBlend) * GlowPulseStrength); // GlowColor, GlowPulseStrength
}

vec4 main2()
{
    // Corrected UV calculation for cross-platform compatibility
    // isf_FragNormCoord is normalized (0.0 to 1.0) and already accounts for resolution.
    // We center it (-0.5 to 0.5) and then apply aspect ratio correction (RENDERSIZE.x / RENDERSIZE.y).
    // The original shader's uv.x *= 1.8; is preserved for its unique visual effect.
    vec2 uv = (isf_FragNormCoord - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0); uv.x *= 1.8;

    tr = sm(50., 48.) + sm(86.4, 89.);
    on = sm(14., 15.) * abs(sin(time * GlobalColorPulseSpeed)) * GlowPulseStrength - fract(sin(time) * 10.) * step(20., time); // GlobalColorPulseSpeed, GlowPulseStrength
    if (time > 21.) on = 1.;
    if (time > 110.) on = step(time, 114.3) * abs(sin(time * 8.));
    
    // pal and glcol are now directly controlled by new color inputs
    // The original lines were commented out as their functionality is replaced by new inputs.
    // pal = mix(vec3(.6, 1, .5) * .75, vec3(1, .5, .5), sm(74., 75.));
    // glcol = mix(vec3(.5, 1, .5) * on * .8, vec3(1.5, .5, .5), sm(74., 75.));
    
    t_internal = (max(21.2, time) - (time - 114.5) * sm(90., 116.5))*5.;
    vec3 from = carpath(t_internal - 2.);
    vec3 cam = vec3(-4., 4., 2.); // Initial camera position
    // These time-based camera movements are hardcoded and complex to parameterize fully without altering core demo flow.
    // For now, retaining them as they define the "story" of the demo.
    if (time < 28.) cam.xz *= rot(-max(8., time) * .7 + 2.5);
    if (time > 28.) from = carpath(t_internal - 2.), cam = vec3(-3, 4, -2), sc = 1.;
    if (time > 35.) from = carpath(t_internal + 4.), cam = vec3(0, 3, 0);
    if (time > 41.5) from = carpath(t_internal - 3.), cam = vec3(1, 4. - tr * 2., 0);
    if (time > 52.) from = carpath(t_internal + 5.), cam = vec3(sin(time) * 3., 4, 0);
    if (time > 55.) from = carpath(t_internal + 5.), cam = mix(cam, vec3(-5, 6, -8), sm(55., 58.)), sc = 2.;
    if (abs(time - 67.) < 3.) from = path(68. * 5.), cam = vec3(4, 3, -0.5);
    if (time > 85.) sc = 3.;
    cam.z += sm(77., 78.) * 4.;
    cam.y *= .5 + .5 * sm(87., 85.);
    cam.x += sm(90., 92.) * 10.;
    cam = mix(cam, vec3(3, 2, 10), sm(105., 110.));
    from += cam;
    carpos = carpath(t_internal);
    vec3 carpos2 = carpath(t_internal + 1. * (1. - is(0.)));
    from = mix(from, carpos2 + cam * .8, sm(93., 95.));
    cardir = normalize(carpath(t_internal + 1.) - carpos);
    return vec4(march(from, lookat(normalize(carpos2 - from)) * normalize(vec3(uv, 1.2 + sm(85., 86.) - sm(90., 91.)))).rgb * sm(1., 3.) * sm(117.5, 115.5) * GlobalBrightness, 1.0); // Explicitly set alpha to 1.0
}


void main() {
    gl_FragColor = main2();
}
