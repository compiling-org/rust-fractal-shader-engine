/*{
  "CATEGORIES": ["Fractal","Optimized","Volumetric","Ported"],
  "DESCRIPTION": "Complete ISF port combining the ISF fractal + Shadertoy loop. All features preserved: camera, geometry, transform modes, transform strength, chaos, palettes, texture/triplanar (enable/mix/scale/warp), focus, FOV, steps, sharpness, falloff, brightness/contrast/glow, symmetry, and the original 2D Shadertoy loop influence.",
  "ISFVSN": "2.0",
  "INPUTS": [
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.1, "MAX": 50.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 1.8, "MIN": 0, "MAX": 5 },
    { "NAME": "TransformStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "GeometryType", "TYPE": "float", "DEFAULT": 3, "MIN": 0, "MAX": 6 },
    { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Symmetry", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 19, "MIN": 0, "MAX": 19 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0, "MAX": 3.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "Glow", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Sharpness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "FalloffCurve", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "CameraOrbit", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "FocusNear", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FocusFar", "TYPE": "float", "DEFAULT": 2.6, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "FOV", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "StepCount", "TYPE": "float", "DEFAULT": 6, "MIN": 1, "MAX": 60 },
    { "NAME": "TextureEnable", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "TextureMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Texture", "TYPE": "image" },
    { "NAME": "TextureWarp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "TextureScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "OriginalMix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

precision highp float;

#define MAX_STEPS 48
#define MAX_SHAPE_ITERS 128
#define BAILOUT 16.0
#define PI 3.14159265359

// -----------------------------------------------------------------------------
// Camera matrix (orbit/pitch/roll)
mat3 cameraMatrix(float orbit, float pitch, float roll) {
    float co = cos(orbit), so = sin(orbit);
    float cp = cos(pitch), sp = sin(pitch);
    float cr = cos(roll), sr = sin(roll);
    return mat3(
        co * cr + so * sp * sr, sr * cp, -so * cr + co * sp * sr,
        -co * sr + so * sp * cr, cr * cp, sr * so + co * sp * cr,
        so * cp, -sp, co * cp
    );
}

// -----------------------------------------------------------------------------
// Palette helpers (classic HSB-like cosine palette)
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.2831853 * (c * t + d));
}
vec3 getColorPalette(int mode, float t) {
    vec3 a = vec3(0.5 + 0.4*sin(float(mode)*0.5),
                  0.6 + 0.3*cos(float(mode)*1.2),
                  0.4 + 0.5*sin(float(mode)*0.9));
    return pal(t, a, vec3(0.4), vec3(1.0,1.3,0.7), vec3(0.1,0.2,0.3));
}

// -----------------------------------------------------------------------------
// Triplanar texture sampling (uses IMG_NORM_PIXEL - ISF normalized sampling)
vec3 triplanarTexture(vec3 p, float scale) {
    // build blending weights from normal-like value
    vec3 blend = normalize(abs(p) + 1e-5);
    blend = pow(blend, vec3(4.0));
    blend /= dot(blend, vec3(1.0));

    // repeating UVs from object-space coordinates
    vec2 xz = fract(p.zy * scale);
    vec2 yz = fract(p.xz * scale);
    vec2 xy = fract(p.xy * scale);

    // IMG_NORM_PIXEL expects UV normalized in [0,1]
    vec3 tx = IMG_NORM_PIXEL(Texture, xz).rgb;
    vec3 ty = IMG_NORM_PIXEL(Texture, yz).rgb;
    vec3 tz = IMG_NORM_PIXEL(Texture, xy).rgb;

    return tx * blend.x + ty * blend.y + tz * blend.z;
}

// -----------------------------------------------------------------------------
// Fractal / shape building blocks
float shapeSpikeFractal(vec3 p, int maxIter) {
    float d = 0.0;
    for (int i = 0; i < MAX_SHAPE_ITERS; i++) {
        if (i >= maxIter) break;
        float denom = dot(p, p) + 0.001;
        p = abs(p) / denom - 0.5;
        p *= 0.95;
        d += length(p);
    }
    return d / 20.0;
}

float shapeChaos(vec3 p, float chaos, float t, float chspd) {
    return (sin(p.x*3.0 + t*chspd) + sin(p.y*4.0 + t*chspd*1.2) + sin(p.z*5.0 + t*chspd*0.8)) * chaos;
}

// Choose a base scene SDF-ish distance (not exact SDF, but marching-friendly)
float scene(vec3 p, int geo, float chaos, float mixAmt, float t) {
    float base;
    if (geo == 0) {
        base = length(p) - 1.0;
    } else if (geo == 1) {
        vec2 q = vec2(length(p.xz) - 1.0, p.y);
        base = length(q) - 0.3;
    } else if (geo == 2) {
        base = shapeSpikeFractal(p * 1.2, 20);
    } else if (geo == 3) {
        base = shapeSpikeFractal(p, 24);
    } else if (geo == 4) {
        base = (abs(p.x) + abs(p.y) + abs(p.z)) - 1.2;
    } else {
        vec3 pp = mod(p * 1.3, 2.0) - 1.0;
        base = shapeSpikeFractal(pp, 18);
    }
    // mix shape with chaos
    return mix(base, shapeChaos(p, chaos, t, ChaosSpeed), clamp(mixAmt, 0.0, 1.0));
}

// -----------------------------------------------------------------------------
// Transforms (several modes) - returns transformed point
vec3 applyTransformRaw(vec3 p, int mode, float chaos, float sym, float chspd, float t) {
    // symmetry scaling
    p *= max(sym, 0.001);
    if (mode == 1) {
        p = abs(p);
    } else if (mode == 2) {
        p += sin(p * 3.0 + t * chspd) * chaos * 0.3;
    } else if (mode == 3) {
        p += sin(p * (1.0 + chaos * 2.0) + t * chspd) * chaos * 0.5;
        p = fract(p * 1.5) - 0.75;
    } else if (mode == 4 || mode == 5) {
        float a = atan(p.z, p.x);
        float r = length(p.xz);
        float spin = t * chspd * (mode == 4 ? 0.2 : 0.3);
        a += spin;
        p.x = cos(a) * r;
        p.z = sin(a) * r;
    } else if (mode == 0) {
        p += vec3(
            sin(p.y * 2.0 + t * chspd),
            cos(p.z * 1.7 + t * chspd),
            sin(p.x * 1.3 + t * chspd)
        ) * chaos * 0.2;
    }
    return p;
}

// wrapper that respects TransformStrength (0..1)
vec3 applyTransform(vec3 p, int mode, float chaos, float sym, float chspd, float t, float strength) {
    vec3 p2 = applyTransformRaw(p, mode, chaos, sym, chspd, t);
    return mix(p, p2, clamp(strength, 0.0, 1.0));
}

// -----------------------------------------------------------------------------
// Original Shadertoy 2D loop ported exactly (keeps original look available).
// Returns vec3(f,g,0) where f,g are the original loop outputs used as detail.
vec3 shadertoy2DFractal(vec2 U, float T, vec2 res, vec2 mouse) {
    // Port of:
    // for( int i = 0; i < 20;i++)
    //   u = vec2( u.x, -u.y ) / dot(u,u) + p,
    //   u.x =  abs(u.x),
    //   f = max( f, dot(u-p,u-p) ),
    //   g = min( g, sin(dot(u+p,u+p))+1.);
    //
    vec2 r = res;
    vec2 u = (U + U - r) / r.y;
    vec2 m = mouse;
    if (m.x == 0.0 && m.y == 0.0) {
        m = (vec2(sin(T * .3) * sin(T * .17) + sin(T * .3),
                  (1.0 - cos(T * .632)) * sin(T * .131) * 1.0 + cos(T * .3)) + 1.0) * r;
    }
    vec2 p = (2.0 + m - r) / r.y;
    float f = 3.0;
    float g = f;
    for (int i = 0; i < 20; i++) {
        u = vec2(u.x, -u.y) / dot(u, u) + p;
        u.x = abs(u.x);
        f = max(f, dot(u - p, u - p));
        g = min(g, sin(dot(u + p, u + p)) + 1.0);
    }
    f = abs(-log(max(f, 1e-6)) / 3.5);
    g = abs(-log(max(g, 1e-6)) / 8.0);
    return vec3(f, g, 0.0);
}

// -----------------------------------------------------------------------------
// Utility: safe mix of palette/texture with an enable boolean
vec3 mixTexturePalette(vec3 palCol, vec3 texCol, bool texEnable, float mixAmt) {
    float ena = texEnable ? 1.0 : 0.0;
    return mix(palCol, texCol, mixAmt * ena);
}

// -----------------------------------------------------------------------------
// Main
void main() {
    vec2 U = gl_FragCoord.xy;
    vec2 R = RENDERSIZE.xy;
    float globalTime = TIME;
    float t = TIME * Speed;

    // Camera / Ray setup
    vec2 uv = (U - 0.5 * R) / R.y;
    uv *= FOV;
    vec3 ro = vec3(0.0, 0.0, -3.0);
    vec3 rd = normalize(vec3(uv * Zoom, 1.0));
    rd = cameraMatrix(CameraOrbit, CameraPitch, CameraRoll) * rd;

    // Camera-space texture warp (optional)
    vec3 camWarp = vec3(0.0);
    if (TextureEnable) {
        camWarp = triplanarTexture(ro * TextureScale, TextureScale) - 0.5;
    }
    vec3 roWarped = ro + camWarp * TextureWarp;

    // Collect original 2D shadertoy loop output (so the original animation can dominate)
    vec3 st2 = shadertoy2DFractal(U, globalTime, R, vec2(0.0));
    float st2_f = st2.x;
    float st2_g = st2.y;

    // parameters (cast/clamp)
    int mode = int(clamp(TransformMode, 0.0, 5.0));
    int geo = int(clamp(GeometryType, 0.0, 6.0));
    float chaos = ChaosIntensity;
    float chaosMix = ChaosMix;
    float sym = Symmetry;
    float chspd = ChaosSpeed;
    float brt = Brightness;
    float ctr = Contrast;
    float glow = Glow;
    int palMode = int(clamp(ColorPaletteMode, 0.0, 19.0));
    float sharp = Sharpness;
    float falloff = FalloffCurve;
    int steps = int(clamp(StepCount, 1.0, float(MAX_STEPS)));
    float transformStr = TransformStrength;

    // Accumulate color along ray
    vec3 col = vec3(0.0);
    float dist = 0.0;

    // Raymarch / volumetric accumulation loop (bounded by MAX_STEPS)
    for (int i = 0; i < MAX_STEPS; i++) {
        if (i >= steps) break;

        vec3 p = roWarped + dist * rd;

        // apply transforms (mix original vs transformed by TransformStrength)
        if (transformStr > 0.0) {
            p = applyTransform(p, mode, chaos, sym, chspd, globalTime, transformStr);
        }

        // compute "distance" / density
        float d = scene(p, geo, chaos, chaosMix, globalTime);
        d = max(abs(d), 0.01);

        // step-dependent fade (sharpness)
        float fade = exp(-float(i) * 0.03 * sharp);

        // depth-based focus
        float focus = smoothstep(FocusNear, FocusFar, dist);

        // coloring: palette + triplanar texture + shadertoy-2D detail
        vec3 palCol = getColorPalette(palMode, p.z * 0.25 + t * 0.1 + st2_f * 0.5);
        vec3 texCol = vec3(0.0);
        if (TextureEnable) {
            texCol = triplanarTexture(p * TextureScale, TextureScale);
        }

        // dynamic mix influenced by the 2D fractal (keeps original animation readable)
        float autoMix = clamp(0.5 + st2_g * 0.5, 0.0, 1.0);
        float texMixFinal = mix(TextureMix, autoMix, 0.25); // small adaptiveness
        vec3 baseColor = mixTexturePalette(palCol, texCol, TextureEnable, texMixFinal);

        // cheap volumetric contribution: stronger when d is smaller
        float weight = 0.005 / (0.01 + d * falloff);

        // accumulate
        col += baseColor * weight * fade * focus;

        // advance ray
        dist += d;
        if (dist > BAILOUT) break;
    }

    // Post processing & final mixes

    // subtle pulsing from chaos speed
    float pulse = sin(globalTime * ChaosSpeed) * 0.5 + 0.5;
    col *= 1.0 + 0.3 * pulse;

    // contrast + brightness mapping
    col = (col - 0.5) * ctr + 0.5;
    col *= brt * (1.0 + glow);

    // original shadertoy 2D color (reconstructed to be similar to original look)
    // Original used: o = min(vec4(g, g*f, f, 0), 1.);
    vec3 original2D = min(vec3(st2_g, st2_g * st2_f, st2_f), vec3(1.0));

    // Combine volumetric result with original 2D loop using OriginalMix.
    // Also, if transforms are turned off (TransformStrength == 0), the volumetric
    // result will be less warped; OriginalMix lets you bring back exact original.
    float origMix = clamp(OriginalMix, 0.0, 1.0);
    float computedMix = origMix + (1.0 - transformStr) * (1.0 - origMix); // if transforms low, bias to original
    computedMix = clamp(computedMix, 0.0, 1.0);

    vec3 finalCol = mix(col, original2D, computedMix);

    // tone mapping / final gamma-like tweak using sharpness
    float gamma = 1.0 / max(0.0001, 1.0 + 0.2 * (sharp - 1.0));
    finalCol = pow(clamp(finalCol, 0.0, 1.0), vec3(gamma));

    gl_FragColor = vec4(finalCol, 1.0);
}
