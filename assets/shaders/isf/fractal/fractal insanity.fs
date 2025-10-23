/*{
  "CATEGORIES": ["Fractal","Optimized","Volumetric"],
  "DESCRIPTION": "Shadertoy core animation + full ISF reference controls. Preserves every slider (sharpness, falloff, chaos, etc.), distinct GeometryType engines, TransformMode families, texture on/off + warp, and GLSL-ES-safe loops.",
  "ISFVSN": "2.0",
  "INPUTS": [
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.1, "MAX": 50.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 1.8, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "GeometryType", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 19.0, "MIN": 0.0, "MAX": 19.0 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "Glow", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Symmetry", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Sharpness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "FalloffCurve", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "CameraOrbit", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "FocusNear", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FocusFar", "TYPE": "float", "DEFAULT": 2.6, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "FOV", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "StepCount", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 60.0 },
    { "NAME": "Texture", "TYPE": "image" },
    { "NAME": "TextureWarp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "TextureScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "TextureEnabled", "TYPE": "bool", "DEFAULT": true }
  ]
}*/

#define PI 3.141592
#define TAU (2.0 * PI)
#define MAX_STEPS 128
#define BAILOUT 16.0

// ---------------- camera ----------------
mat3 cameraMatrix(float orbit, float pitch, float roll) {
    float co = cos(orbit), so = sin(orbit);
    float cp = cos(pitch), sp = sin(pitch);
    float cr = cos(roll), sr = sin(roll);
    return mat3(
        co * cr + so * sp * sr,  sr * cp, -so * cr + co * sp * sr,
       -co * sr + so * sp * cr,  cr * cp,  sr * so + co * sp * cr,
        so * cp,                -sp,       co * cp
    );
}

// ---------------- palettes ----------------
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(TAU * (c * t + d));
}

vec3 getColorPalette(int mode, float t) {
    float T = fract(t * 0.18 + 0.02);
    if (mode == 0) return pal(T, vec3(0.5,0.5,0.5), vec3(0.5), vec3(1.0), vec3(0.0));
    if (mode == 1) return pal(T, vec3(0.9,0.55,0.2), vec3(0.6,0.45,0.3), vec3(1.0,0.9,0.6), vec3(0.02,0.18,0.30));
    if (mode == 2) return pal(T, vec3(0.1,0.6,1.0), vec3(0.7,0.25,0.6), vec3(1.0,1.2,0.8), vec3(0.2,0.1,0.05));
    if (mode == 3) return pal(T, vec3(1.0,0.15,0.05), vec3(0.9,0.6,0.4), vec3(0.5,0.2,1.0), vec3(0.05,0.18,0.28));
    if (mode == 4) return pal(T, vec3(0.05,1.0,0.5), vec3(0.9,0.45,0.25), vec3(0.8,1.1,1.4), vec3(0.18,0.35,0.10));
    if (mode == 5) return pal(T, vec3(0.7,0.2,0.9), vec3(0.45,0.6,0.7), vec3(1.0,1.0,0.6), vec3(0.22,0.08,0.38));
    if (mode == 6) return pal(T, vec3(0.95,0.4,0.05), vec3(0.6,0.5,0.2), vec3(1.2,0.8,0.5), vec3(0.0,0.12,0.22));
    if (mode == 7) return pal(T, vec3(0.05,0.28,0.9), vec3(0.8,0.4,0.5), vec3(1.0,0.9,1.2), vec3(0.12,0.05,0.18));
    if (mode == 8) return pal(T, vec3(0.2,0.95,0.45), vec3(0.7,0.4,0.6), vec3(0.8,1.1,0.9), vec3(0.3,0.18,0.08));
    if (mode == 9) return pal(T, vec3(0.9,0.12,0.7), vec3(0.6,0.3,0.8), vec3(1.0,0.6,1.3), vec3(0.15,0.05,0.28));
    if (mode == 10) return pal(T, vec3(0.3,0.8,1.0), vec3(0.5,0.5,0.6), vec3(1.5,0.9,0.6), vec3(0.1,0.25,0.35));
    if (mode == 11) return pal(T, vec3(0.95,0.65,0.25), vec3(0.9,0.4,0.2), vec3(0.8,1.0,0.5), vec3(0.05,0.2,0.12));
    if (mode == 12) return pal(T, vec3(0.1,0.25,0.6), vec3(0.6,0.8,0.3), vec3(1.0,1.5,0.7), vec3(0.18,0.28,0.05));
    if (mode == 13) return pal(T, vec3(0.8,0.05,0.3), vec3(0.7,0.3,0.6), vec3(1.0,0.7,1.2), vec3(0.1,0.02,0.22));
    if (mode == 14) return pal(T, vec3(0.35,0.95,0.5), vec3(0.9,0.6,0.4), vec3(0.9,1.2,1.0), vec3(0.25,0.18,0.1));
    if (mode == 15) return pal(T, vec3(0.6,0.15,0.95), vec3(0.4,0.6,0.7), vec3(1.1,0.9,0.6), vec3(0.22,0.06,0.3));
    if (mode == 16) return pal(T, vec3(0.85,0.35,0.05), vec3(0.6,0.6,0.3), vec3(1.3,0.85,0.55), vec3(0.02,0.14,0.18));
    if (mode == 17) return pal(T, vec3(0.05,0.9,0.9), vec3(0.8,0.5,0.55), vec3(0.9,1.0,1.2), vec3(0.12,0.25,0.22));
    if (mode == 18) return pal(T, vec3(0.9,0.3,0.85), vec3(0.5,0.6,0.7), vec3(1.0,1.0,0.7), vec3(0.18,0.06,0.33));
    return pal(T, vec3(0.5,0.2,1.0), vec3(0.6,0.5,0.8), vec3(1.2,0.9,0.6), vec3(0.15,0.05,0.4));
}

// ---------------- triplanar texture ----------------
vec3 triplanarTexture(vec3 p, float scale) {
    vec3 blend = normalize(abs(p) + 1e-6);
    blend = pow(blend, vec3(4.0));
    blend /= (blend.x + blend.y + blend.z);
    vec2 xz = fract(p.zy * scale);
    vec2 yz = fract(p.xz * scale);
    vec2 xy = fract(p.xy * scale);
    vec3 tx = texture2D(Texture, xz).rgb;
    vec3 ty = texture2D(Texture, yz).rgb;
    vec3 tz = texture2D(Texture, xy).rgb;
    return tx * blend.x + ty * blend.y + tz * blend.z;
}

// ---------------- Shadertoy core DE (kept as primary engine) ----------------
// This is the exact Shadertoy recursion ported into a function, with no audio.
// It is used as the core animation engine; other GeometryType engines will wrap/augment it.
float shadertoyCoreDE(vec3 p, float tt) {
    // Replicate main inner recursive behavior used in the Shadertoy sample.
    // We'll do a safe fixed-loop implementation (no variable loop conditions).
    float s = 2.7;
    float d = 1.0;
    vec3 q = p;
    // apply initial offsets similar to shadertoy before inner folding
    q = abs(q) - vec3(0.1 + 0.02 * smoothstep(3.0, 7.0, tt), 0.4 - 0.02 * smoothstep(3.0, 7.0, tt), 1.9);
    // fixed 5 iterations (mirrors example), but we won't depend on StepCount here
    for (int j = 0; j < 5; j++) {
        // sinusoidal mod per iteration similar to original
        q.xy *= mix(1.05, 0.95, 0.5 + 0.5 * sin(length(q.xy) * 0.5 + 0.1 * tt));
        q = abs(q - vec3(1.2, 1.3, 1.1)) - vec3(1.0, 1.2, 1.08 + 0.04 * sin(tt));
        float denom = clamp(dot(q, q), 0.5, 1.7);
        float local = 2.5 / denom;
        s *= local;
        q = abs(q) * local;
        d = local;
    }
    // rotation step
    float rrot = 0.5 * PI * (0.5 + 0.5 * sin(tt));
    mat2 rotm = mat2(cos(rrot), -sin(rrot), sin(rrot), cos(rrot));
    q.xy = rotm * q.xy;
    // produce distance-like estimate
    float value = (max(length(q.xz), -(length(p) - 0.3))) / s;
    return max(0.0001, value);
}

// ---------------- additional engines for GeometryType ----------------
float sphereDE(vec3 p) { return length(p) - 1.0; }
float cylinderDE(vec3 p) { vec2 q = vec2(length(p.xz)-1.0, p.y); return length(q) - 0.3; }
float boxDE(vec3 p, vec3 b) { vec3 q = abs(p) - b; vec3 qm = max(q, vec3(0.0)); float outd = length(qm); float ind = min(max(q.x, max(q.y, q.z)), 0.0); return outd + ind; }
float torusDE(vec3 p, vec2 t) { vec2 q = vec2(length(p.xz)-t.x, p.y); return length(q) - t.y; }

// Spike fractal (fold heavy) - safe fixed loop
float spikeFractalDE(vec3 p) {
    float acc = 0.0;
    for (int i = 0; i < 64; i++) {
        p = abs(p) / (dot(p, p) + 1e-4) - 0.5;
        p *= 0.95;
        acc += length(p);
    }
    return acc / 20.0;
}

// Mandelbulb-like approximate (safe small iterations)
float mandelbulbDE(vec3 pos) {
    vec3 z = pos;
    float dr = 1.0;
    float r = 0.0;
    const int MAXMB = 12;
    for (int i = 0; i < MAXMB; i++) {
        r = length(z);
        if (r > 2.0) break;
        float theta = acos(clamp(z.z / r, -1.0, 1.0));
        float phi = atan(z.y, z.x);
        float power = 8.0;
        float zr = pow(max(r, 1e-6), power);
        float newTheta = theta * power;
        float newPhi = phi * power;
        vec3 newZ = zr * vec3(sin(newTheta) * cos(newPhi), sin(newTheta) * sin(newPhi), cos(newTheta));
        z = newZ + pos;
        dr = pow(max(r, 1e-6), power - 1.0) * power * dr + 1.0;
    }
    r = length(z);
    return 0.5 * log(max(r, 1e-6)) * r / max(dr, 1e-6);
}

// Menger-like roughness (IFS-style)
float mengerLikeDE(vec3 p) {
    float scale = 1.0;
    float acc = 0.0;
    for (int i = 0; i < 6; i++) {
        p = abs(p);
        if (p.x < p.y) p.xy = p.yx;
        if (p.x < p.z) p.xz = p.zx;
        p = p * 3.0 - (vec3(2.0) * floor(p * 0.3333333));
        scale *= 3.0;
        acc += length(p) / scale;
    }
    return acc * 0.2;
}

// ---------------- chaos / turbulence ----------------
float shapeChaos(vec3 p, float chaos, float chspd, float tt) {
    return (sin(p.x*3.0 + tt*chspd) + sin(p.y*4.0 + tt*chspd*1.2) + sin(p.z*5.0 + tt*chspd*0.8)) * chaos;
}

// ---------------- sceneDE combining Shadertoy core + selectable engines ----------------
float sceneDE(vec3 p, int geo, float chaos, float mixAmt, float tt, int steps) {
    // Ensure we preserve Shadertoy base animation: use shadertoyCoreDE as primary driver for many modes,
    // but let GeometryType pick different base engines or heavy augmentations.
    float base = 0.0;
    if (geo == 0) {
        // sphere core with shadertoy ribbons overlay
        base = sphereDE(p);
        base += 0.35 * shadertoyCoreDE(p * 0.9, tt);
    } else if (geo == 1) {
        // cylinder with banded recursion
        base = cylinderDE(p);
        vec3 q = p;
        q.y += 0.25 * sin(tt * 0.45 + length(p.xz) * 1.8);
        base = min(base, 0.6 * shadertoyCoreDE(q * 1.05, tt));
    } else if (geo == 2) {
        // box + aggressive spike fractal
        base = boxDE(p, vec3(1.0));
        base = mix(base, spikeFractalDE(p * 1.2), 0.85);
    } else if (geo == 3) {
        // keep Shadertoy core as dominant fractal (preserve base animation exactly)
        base = shadertoyCoreDE(p, tt);
    } else if (geo == 4) {
        // menger-ish layered on a box
        base = boxDE(p, vec3(0.9));
        base += 0.55 * mengerLikeDE(p * 0.8);
    } else if (geo == 5) {
        // torus with recursive ribbons
        base = torusDE(p, vec2(1.0, 0.25));
        base += 0.35 * shadertoyCoreDE(p * vec3(1.0, 0.8, 1.1), tt);
    } else { // geo == 6
        // mandelbulb-like (very different)
        base = mandelbulbDE(p * 0.95);
    }

    // apply chaos blending carefully so it doesn't fully override base engine
    float chaosVal = shapeChaos(p, chaos * 0.6, ChaosSpeed, tt);
    float outDE = mix(base, chaosVal, mixAmt * 0.5);

    return outDE;
}

// ---------------- Transform families (distinct from Geometry engines) ----------------
vec3 applyTransform(vec3 p, int mode, float chaos, float sym, float chspd, float tt, int geoMode) {
    p *= max(sym, 0.001);

    // Geo pre-ops to help each geometry read differently under transforms
    if (geoMode == 1) p.xz *= (1.0 + 0.05 * sin(tt * 0.3));
    if (geoMode == 2) p *= (0.95 + 0.05 * cos(tt * 0.2));
    if (geoMode == 4) p = abs(p) - 0.08;

    // TransformMode families (distinct behaviors)
    if (mode == 0) {
        // base wobble (keeps original Shadertoy subtle wobble)
        p += 0.02 * sin(vec3(p.yzx * 3.0 + tt * chspd));
    } else if (mode == 1) {
        // absolute folding
        p = abs(p);
        p += 0.03 * vec3(sin(p.y * 3.0 + tt), sin(p.z * 2.0 + tt*0.9), sin(p.x * 4.0 + tt*1.1));
    } else if (mode == 2) {
        // parametric sinusoidal warp (stronger)
        p += sin(p * (1.0 + chaos * 2.0) + tt * chspd) * chaos * 0.45;
        p.xz += 0.1 * vec2(-p.z, p.x) * sin(tt * 0.2);
    } else if (mode == 3) {
        // aggressive repeated folding (IFS-like)
        for (int i = 0; i < 3; i++) {
            p = abs(p) - 0.6 + 0.02 * sin(tt * (0.3 + float(i) * 0.15));
            p *= 1.2;
        }
        p += 0.2 * fract(p * 1.7) - 0.1;
    } else if (mode == 4) {
        // polar swirl
        float a = atan(p.y, p.x) + 0.4 * sin(tt * chspd + length(p) * 2.0);
        float r = length(p.xy);
        p.xy = vec2(cos(a), sin(a)) * r;
        p.z += 0.05 * sin(r * 6.0 + tt);
    } else if (mode == 5) {
        // rotation + shear
        float s = sin(tt * 0.2 * chspd);
        float c = cos(tt * 0.2 * chspd);
        mat2 m = mat2(c, -s, s, c);
        p.xy = m * p.xy;
        p.x += 0.2 * p.y * sin(tt * 0.15);
    } else if (mode == 6) {
        // modular tiling repeat to create distinct repeated structures
        vec3 rp = p;
        rp = mod(rp * 2.0, vec3(2.0)) - vec3(1.0);
        p = mix(p, rp, 0.7);
    }

    // small extra fold pass to ensure sliders visibly affect results
    vec3 pf = abs(p);
    p += pf * chaos * 0.06;

    return p;
}

// ---------------- Main ----------------
void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= FOV;

    float tt = TIME * Speed;

    // camera setup (preserve original camera math)
    vec3 ro = vec3(0.0, 0.0, -3.0);
    vec3 rd = normalize(vec3(uv * Zoom, 1.0));
    rd = cameraMatrix(CameraOrbit, CameraPitch, CameraRoll) * rd;

    // apply texture warp to camera origin (preserve ability to warp origin)
    vec3 warp = TextureEnabled ? (triplanarTexture(ro * TextureScale, 1.0) - 0.5) : vec3(0.0);
    vec3 roWarped = ro + warp * TextureWarp;

    vec3 col = vec3(0.0);
    float dist = 0.0;

    int mode = int(clamp(TransformMode, 0.0, 6.0));
    int geo = int(clamp(GeometryType, 0.0, 6.0));
    float chaos = ChaosIntensity;
    float chaosMix = ChaosMix;
    float sym = Symmetry;
    float chspd = ChaosSpeed;
    float br = Brightness;
    float ct = Contrast;
    float glow = Glow;
    int pal = int(max(0.0, floor(ColorPaletteMode + 0.5)));
    float sharp = Sharpness;
    float falloff = FalloffCurve;
    int steps = int(clamp(StepCount, 1.0, float(MAX_STEPS)));

    // Raymarch loop: GLSL-ES safe (constant bound + break)
    for (int i = 0; i < MAX_STEPS; i++) {
        if (i >= steps) break;

        vec3 p = roWarped + dist * rd;

        // Apply transforms (distinct families)
        p = applyTransform(p, mode, chaos, sym, chspd, tt, geo);

        // Compute DE for chosen geometry (distinct engines)
        float d = sceneDE(p, geo, chaos, chaosMix, tt, steps);

        // Secondary shading/detail from shadertoy core (ribbon-like); blend depends on geo
        float shade = shadertoyCoreDE(p * 0.9, tt);
        float secondaryBlend = (geo == 3) ? 0.02 : 0.22;
        d = mix(d, shade, secondaryBlend);

        d = max(abs(d), 0.001);

        // falloff and focus
        float fade = exp(-float(i) * 0.03 * sharp);
        float focus = smoothstep(FocusNear, FocusFar, dist);

        // color: palette + texture (texture can color and warp surfaces)
        float tBias = dist * 0.08 + float(geo) * 0.12 + tt * 0.06;
        vec3 palCol = getColorPalette(pal, p.z + tBias);
        vec3 texCol = TextureEnabled ? triplanarTexture(p * TextureScale, 1.0) : vec3(0.0);

        // Geometry-specific bias for mixing texture vs palette
        float geoTexBias = 0.5;
        if (geo == 0) geoTexBias = 0.2;
        else if (geo == 1) geoTexBias = 0.6;
        else if (geo == 2) geoTexBias = 0.35;
        else if (geo == 3) geoTexBias = 0.15;
        else if (geo == 4) geoTexBias = 0.45;
        else if (geo == 5) geoTexBias = 0.55;
        else if (geo == 6) geoTexBias = 0.3;

        // final mix: if texture disabled, prefer palette entirely
        vec3 mixed = TextureEnabled ? mix(palCol, texCol, geoTexBias) : palCol;

        // brightness contribution scaled by DE + falloff
        float b = 0.005 / (0.01 + d * falloff);

        // accumulate
        col += mixed * b * fade * focus;

        // advance
        dist += d;

        if (dist > BAILOUT) break;
    }

    // final modulations (preserve original feel)
    float pulse = sin(TIME * ChaosSpeed) * 0.5 + 0.5;
    col *= 1.0 + 0.3 * pulse;

    // contrast/brightness/glow
    col = (col - 0.5) * ct + 0.5;
    col *= br * (1.0 + glow);

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
