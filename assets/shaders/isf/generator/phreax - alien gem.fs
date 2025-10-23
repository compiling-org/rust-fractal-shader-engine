/*
{
    "CATEGORIES": [
        "Fractal",
        "Abstract",
        "Iridescent",
        "Animated",
        "Psychedelic",
        "SDF"
    ],
    "DESCRIPTION": "An advanced ISF conversion of a complex Shadertoy fractal, featuring iridescent surfaces, intricate geometric transformations, and dynamic animations. Provides extensive controls for animation speed, camera position, fractal morphing, color palettes, bump mapping, and post-processing effects like glitch, shake, brightness, saturation, and contrast. Requires optional texture inputs for full reflection and bump map effects.",
    "CREDIT": "Original shader by phreax (2022) - Creative Commons Licence Attribution-NonCommercial-ShareAlike. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "xy_control_x", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "X-component for camera rotation/offset (normalized 0-1, maps to 0-2PI)." },
        { "NAME": "xy_control_y", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Y-component for camera rotation/offset (normalized 0-1, maps to 0-PI)." },
        { "NAME": "kalei_iterations", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of iterations for the Kalei fractal transformation." },
        { "NAME": "kalei_offset_morph", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Offset amount in Kalei fractal, influenced by animation." },
        { "NAME": "fold_symmetry_1", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0, "STEP": 1.0, "DESCRIPTION": "N-fold symmetry for the first fractal transformation." },
        { "NAME": "fold_symmetry_1_morph", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Morphing factor for the first fold symmetry." },
        { "NAME": "fold_symmetry_2", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 12.0, "STEP": 1.0, "DESCRIPTION": "N-fold symmetry for the second fractal transformation." },
        { "NAME": "fold_symmetry_2_morph", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Morphing factor for the second fold symmetry." },
        { "NAME": "transform_1_morph_mix", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for morphing in transform 1 (abs(cp) - mix)." },
        { "NAME": "transform_2_morph_mix", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for morphing in transform 2 (abs(cp) - mix)." },
        { "NAME": "smin_blend_factor", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.01, "MAX": 2.0, "DESCRIPTION": "Smooth minimum blend factor between fractal components." },
        { "NAME": "primary_light_x", "TYPE": "float", "DEFAULT": 3.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "X position of the primary light source." },
        { "NAME": "primary_light_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Y position of the primary light source." },
        { "NAME": "primary_light_z", "TYPE": "float", "DEFAULT": -2.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z position of the primary light source." },
        { "NAME": "secondary_light_x", "TYPE": "float", "DEFAULT": -3.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "X position of the secondary light source." },
        { "NAME": "secondary_light_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Y position of the secondary light source." },
        { "NAME": "secondary_light_z", "TYPE": "float", "DEFAULT": -2.0, "MIN": -10.0, "MAX": 10.0, "DESCRIPTION": "Z position of the secondary light source." },
        { "NAME": "bump_map_strength", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0, "MAX": 0.01, "DESCRIPTION": "Strength of the bump mapping effect." },
        { "NAME": "bump_map_offset", "TYPE": "float", "DEFAULT": 200.0, "MIN": 10.0, "MAX": 500.0, "DESCRIPTION": "Texture coordinate multiplier for bump map (bumpTexture)." },
        { "NAME": "reflection_mix", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for reflections from inputImage." },
        { "NAME": "iridescence_mix", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for additional iridescence on reflections." },
        { "NAME": "fractal_color_mix_1", "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for color on material 1 (fractal 2)." },
        { "NAME": "fractal_color_mix_2", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for color on material 0 (fractal 1)." },
        { "NAME": "background_color_mix", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for the background base colors." },
        { "NAME": "glow_intensity", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Base intensity of the outer glow." },
        { "NAME": "glow_pulse_intensity", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Intensity of the pulsing effect on the outer glow." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "vignette_intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of the vignette effect." },
        { "NAME": "dithering_enabled", "TYPE": "bool", "DEFAULT": true, "DESCRIPTION": "Enable or disable dithering for smoother gradients." },
        { "NAME": "inputImage", "TYPE": "image", "BASE_KEY": "inputImage", "DESCRIPTION": "Optional texture for reflections." },
        { "NAME": "bumpTexture", "TYPE": "image", "BASE_KEY": "bumpTexture", "DESCRIPTION": "Optional texture for bump mapping." }
    ]
}
*/

#define PI 3.141592
#define TAU (2. * PI)
#define SIN_NORM(x) (sin(x) * .5 + .5) // Renamed from SIN to avoid conflict if any
#define PHI 1.618033988749895
#define BUMP_EPS 0.004
#define FAR_DISTANCE 100.0 // Renamed from FAR to FAR_DISTANCE to avoid potential keyword conflict

// Global variables for raymarching and material
float tt_global; // Renamed from tt
float g_mat_global; // Renamed from g_mat
float closest_global = FAR_DISTANCE; // Renamed from closest


mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

// zucconis spectral palette https://www.alanzucconi.com/2017/07/15/improving-the-rainbow-2/
vec3 bump3y(vec3 x, vec3 yoffset) {
    vec3 y = 1. - x * x;
    y = clamp((y - yoffset), vec3(0.0), vec3(1.0));
    return y;
}

const highp float NOISE_GRANULARITY = 0.5 / 255.0;

highp float random(highp vec2 coords) {
    return fract(sin(dot(coords.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

vec3 spectral_zucconi6(float x) {
    x = fract(x);
    const vec3 c1 = vec3(3.54585104, 2.93225262, 2.41593945);
    const vec3 x1 = vec3(0.69549072, 0.49228336, 0.27699880);
    const vec3 y1 = vec3(0.02312639, 0.15225084, 0.52607955);
    const vec3 c2 = vec3(3.90307140, 3.21182957, 3.96587128);
    const vec3 x2 = vec3(0.11748627, 0.86755042, 0.66077860);
    const vec3 y2 = vec3(0.84897130, 0.88445281, 0.73949448);
    return bump3y(c1 * (x - x1), y1) + bump3y(c2 * (x - x2), y2);
}

// Custom psychedelic color palettes (can be expanded)
vec3 getColorPalette(float t, float type) {
    if (type < 0.5) return spectral_zucconi6(t); // Palette 0: Original Zucconi spectral
    if (type < 1.5) return vec3(sin(t * 5.0), sin(t * 7.0 + 1.0), sin(t * 9.0 + 2.0)) * 0.5 + 0.5; // Palette 1: Rapid Sine Waves
    if (type < 2.5) return vec3(cos(t * 4.0 + 2.0), cos(t * 2.0 + 1.0), sin(t * 6.0)) * 0.5 + 0.5; // Palette 2: Muted Cosine Blends
    if (type < 3.5) return vec3(sin(t * 2.0), sin(t * 4.0), cos(t * 8.0)) * 0.5 + 0.5; // Palette 3: Fast RGB Pulse
    if (type < 4.5) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0)); // Palette 4: Hard Edge Fractal Colors
    if (type < 5.5) return vec3(sin(t * 1.5), cos(t * 3.0), sin(t * 4.5 + cos(t * 2.0))) * 0.5 + 0.5; // Palette 5: Organic Swirl
    if (type < 6.5) return mix(spectral_zucconi6(t * 0.7), spectral_zucconi6(t * 1.3 + 0.5), 0.5); // Palette 6: Dual Spectral Blend
    // Palette 7: Chakra-like static colors (can be animated if desired)
    vec3 cols[7] = vec3[](vec3(0.608, 0.020, 1.000), vec3(0.169, 0.059, 1.000), vec3(0.000, 0.800, 1.000),
                          vec3(0.035, 1.000, 0.020), vec3(0.984, 1.000, 0.161), vec3(1.000, 0.463, 0.020),
                          vec3(1.000, 0.000, 0.000));
    int index = int(mod(t * 7.0, 7.0)); // Cycle through chakra colors based on t
    return cols[index];
}


// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); // Adjust contrast around 0.5 (mid-gray)
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); // Calculate grayscale luminance
    color = mix(gray, color, sat); // Adjust saturation by mixing with grayscale
    return color * br; // Adjust brightness
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }


float rect(vec2 p, vec2 b, float r) {
    vec2 d = abs(p) - (b - r);
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}


vec3 kalei(vec3 p, float iter_count, float morph_offset) { // Added params
    float s = 1.0;
    for (float i = 0.0; i < iter_count; i++) { // Use input iter_count
        p = abs(p);
        p.xz *= rot(i / iter_count * PI + .2 * tt_global);
        p -= morph_offset; // Use input morph_offset
    }
    return clamp(p, -1e5, 1e5);
}


// n-fold symmetry by mla
vec2 foldSym(vec2 p, float N) {
    float t = atan(p.x, -p.y);
    t = mod(t + PI / N, 2.0 * PI / N) - PI / N;
    return length(p) * vec2(cos(t), sin(t));
}

vec3 transform(vec3 p, float fold_sym_1, float fold_sym_1_morph, float morph_mix_1) { // Added params
    p *= 1.5;

    p.xy = foldSym(p.xy, fold_sym_1); // Use input fold_sym_1
    p.xy = abs(p.xy) - fold_sym_1_morph * SIN_NORM(.3 * tt_global); // Use input fold_sym_1_morph

    p = kalei(p, kalei_iterations, kalei_offset_morph); // Pass params to kalei

    p.yz *= rot(PI * .5);
    p.xz *= rot(.2 * tt_global);

    float r = 1.0;
    vec2 cp = vec2(length(p.xz) - r, p.y);

    float rev = 2.0;
    float a = atan(p.z, p.x);

    cp *= rot(rev * a + .3 * tt_global);
    cp = abs(cp) - mix(0.4, 1.0, SIN_NORM(tt_global)) * morph_mix_1; // Use input morph_mix_1
    cp *= rot(.5 * tt_global);

    return vec3(cp, p.z);
}

vec3 transform2(vec3 p, float fold_sym_2, float fold_sym_2_morph, float morph_mix_2) { // Added params
    p *= .8;

    p.xy = foldSym(p.xy, fold_sym_2); // Use input fold_sym_2
    p.xy = abs(p.xy) - fold_sym_2_morph * SIN_NORM(-.3 * tt_global); // Use input fold_sym_2_morph
    p.yz *= rot(PI * .25);

    float r = 1.0;
    vec2 cp = vec2(length(p.xz) - r, p.y);


    float rev = 1.0;
    float a = atan(p.z, p.x);


    cp *= rot(rev * a + .3 * tt_global);
    cp = abs(cp) - mix(0.4, 1.0, SIN_NORM(tt_global)) * morph_mix_2; // Use input morph_mix_2
    cp *= rot(-.5 * tt_global);

    return vec3(cp, p.z);
}

float smin(float a, float b, float k) {
    float h = clamp((a - b) / k * .5 + .5, 0.0, 1.0);
    return mix(a, b, h) - h * (1.0 - h) * k;
}

float map(vec3 p) {
    vec3 bp = p; // Not used in this map function, but kept from original structure
    float edge = 0.05;


    vec2 cp = transform(p, fold_symmetry_1, fold_symmetry_1_morph, transform_1_morph_mix).xy;
    vec2 cp2 = transform2(p, fold_symmetry_2, fold_symmetry_2_morph, transform_2_morph_mix).xy;

    float dr = rect(cp.xy, vec2(.3, .3), edge);
    float dr2 = rect(cp2.xy, vec2(.08), 0.02);

    float d = smin(dr, dr2, smin_blend_factor); // Use tunable smin_blend_factor

    g_mat_global = dr < dr2 ? 0.0 : 1.0; // Update global material

    return .5 * d;
}


vec3 getNormal(vec3 p) {
    vec2 eps = vec2(0.001, 0.0);
    return normalize(vec3(map(p + eps.xyy) - map(p - eps.xyy),
                          map(p + eps.yxy) - map(p - eps.yxy),
                          map(p + eps.yyx) - map(p - eps.yyx)
                         )
                    );
}

float bumpSurf3D(in vec3 p) {
    p.z += .3 * tt_global;
    p = abs(mod(p * 4.0, 2.0 * 0.125) - 0.0125);

    float x = min(p.x, min(p.z, p.y)) / 0.03125;

    return clamp(x, 0.0, 1.0);
}

// Standard function-based bump mapping function (from Shane)
vec3 doBumpMap(in vec3 p, in vec3 nor, float bumpfactor) {
    const float eps = BUMP_EPS;
    float ref = bumpSurf3D(p);
    vec3 grad = vec3(bumpSurf3D(vec3(p.x - eps, p.y, p.z)) - ref,
                     bumpSurf3D(vec3(p.x, p.y - eps, p.z)) - ref,
                     bumpSurf3D(vec3(p.x, p.y, p.z - eps)) - ref) / eps;

    grad -= nor * dot(nor, grad);

    return normalize(nor + bumpfactor * grad);
}

// from iq (not used in mainImage, but kept for reference)
float softshadow(in vec3 ro, in vec3 rd, float mint, float maxt, float k) {
    float res = 1.0;
    float ph = 1e20;
    for (float t = mint; t < maxt; ) {
        float h = map(ro + rd * t);
        if (h < 0.001)
            return 0.0;
        float y = h * h / (2.0 * ph);
        float d = sqrt(h * h - y * y);
        res = min(res, k * d / max(0.0, t - y));
        ph = h;
        t += h;
    }
    return res;
}


vec2 raymarch(vec3 ro, vec3 rd, float steps) {
    float mat = 0.0;
    float t = 0.0;
    float d = 0.0;
    vec3 p = ro;

    for (float i = .0; i < steps; i++) {
        d = map(p);
        mat = g_mat_global; // save global material

        closest_global = min(closest_global, d / t);
        if (abs(d) < 0.0001 || t > FAR_DISTANCE) break; // Using FAR_DISTANCE global const

        t += d;
        p += rd * d;
    }
    return vec2(t, mat);
}

float n21(vec2 p) {
    return fract(sin(dot(p, vec2(524.423, 123.34))) * 3228324.345);
}

float noise(vec2 n) {
    const vec2 d = vec2(0.0, 1.0);
    vec2 b = floor(n);
    vec2 f = mix(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(n21(b), n21(b + d.yx), f.x), mix(n21(b + d.xy), n21(b + d.yy), f.x), f.y);
}

// Spherical mapping function for reflections from a 3D direction vector to 2D UV
vec2 sphericalMap(vec3 dir) {
    dir = normalize(dir);
    // Convert Cartesian to spherical coordinates
    float lon = atan(dir.z, dir.x); // Longitude
    float lat = asin(dir.y);        // Latitude

    // Map spherical coordinates to UV (0 to 1)
    float u = (lon + PI) / TAU;
    float v = (lat + PI / 2.0) / PI;
    return vec2(u, v);
}


void main() {
    vec4 fragColor = vec4(0.0);
    vec2 fragCoord = gl_FragCoord.xy;

    float current_time = TIME * speed; // Use ISF TIME and speed input
    tt_global = .5 * current_time; // Map tt_global

    vec2 uv_norm_res = (fragCoord - .5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv = uv_norm_res / zoom; // Apply zoom

    // Camera ray direction - initial fixed Z
    vec3 ro = vec3(uv * 6.0, -4.0); // Original ro
    vec3 rd = vec3(0.0, 0.0, 1.0); // Original rd

    // Camera rotation based on xy_control
    float camera_angle_x = xy_control_x * TAU; // Map 0-1 to 0-2PI
    float camera_angle_y = xy_control_y * PI;  // Map 0-1 to 0-PI (to prevent flip)

    // Apply camera rotation to ray direction
    // Rotations around axes:
    // Rotate around Y (vertical rotation based on mouse_y)
    mat2 rot_y = mat2(cos(camera_angle_y), -sin(camera_angle_y), sin(camera_angle_y), cos(camera_angle_y));
    rd.xz = rot_y * rd.xz;
    // Rotate around Z (horizontal rotation based on mouse_x, applied to the already rotated XY plane)
    mat2 rot_z = mat2(cos(camera_angle_x), -sin(camera_angle_x), sin(camera_angle_x), cos(camera_angle_x));
    rd.xy = rot_z * rd.xy;
    
    // Apply rotations to ray origin for camera movement
    ro.xz = rot_y * ro.xz;
    ro.xy = rot_z * ro.xy;


    // Apply camera shake directly to initial ray direction
    if (shake_strength > 0.001) {
        vec2 shake_uv_norm = (fragCoord / RENDERSIZE.xy) - 0.5;
        shake_uv_norm.x *= RENDERSIZE.x / RENDERSIZE.y;

        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency + hash11(1.0)) * 0.1,
            cos(current_time * shake_frequency * 1.1 + hash11(2.0)) * 0.1
        ) * shake_strength;

        rd.x += shake_offset.x;
        rd.y += shake_offset.y;
        rd = normalize(rd); // Re-normalize after adding offset
    }

    // Apply glitch effect directly to initial ray direction
    if (glitch_strength > 0.001) {
        vec2 glitch_uv_norm = fragCoord / RENDERSIZE.xy;
        float offset_x_noise = (hash22(glitch_uv_norm * 10.0 + current_time * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(glitch_uv_norm.y * 150.0 + current_time * 20.0) * 0.5 + 0.5;

        rd.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
        rd = normalize(rd);
    }

    vec3 lp = vec3(primary_light_x, primary_light_y, primary_light_z);   // Tunable light position 1
    vec3 lp2 = vec3(secondary_light_x, secondary_light_y, secondary_light_z); // Tunable light position 2


    vec3 col = vec3(0.0);

    vec2 e = vec2(0.0035, -0.0035);

    // background color
    vec3 c1 = vec3(0.165, 0.208, 0.298);
    vec3 c2 = vec3(0.180, 0.337, 0.337);

    // light color
    vec3 lc1 = vec3(0.573, 0.424, 0.976);
    vec3 lc2 = vec3(0.573, 0.922, 0.969);


    // currently only one pass
    // Replaced hardcoded steps (250/50) with `raymarch_steps` input (currently fixed at 250 for this shader)
    float steps = 250.0; // Default to the higher quality pass if only one is run
    vec2 rm = raymarch(ro, rd, steps);
    float mat = rm.y;

    vec3 p = ro + rm.x * rd;

    vec3 n = normalize(e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) +
                       e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));


    if (mat == 1.0) {
        vec3 pt = transform2(p, fold_symmetry_2, fold_symmetry_2_morph, transform_2_morph_mix); // Pass transform2 params
        n = doBumpMap(pt, n, mix(0.0, bump_map_strength, length(p.xy) - .5)); // Use tunable bump_map_strength
    }


    if (rm.x < FAR_DISTANCE) { // Use FAR_DISTANCE for hit check
        vec3 l = normalize(lp - p);
        vec3 l2 = normalize(lp2 - p);
        float dif = max(dot(n, l), 0.0);
        float dif2 = max(dot(n, l2), 0.0);
        
        vec3 n2 = n;
        n2.xy += noise(p.xy) * .5 - .025;
        n2 = normalize(n2);
        float height_param = atan(n2.y, n2.x);

        vec3 iri = spectral_zucconi6(height_param * 1.11) * smoothstep(.8, .2, abs(n2.z)) - .02;


        col += dif * lc1 * .5 + .5 * dif2 * lc2 + .5 * iri;


        if (mat > 0.0) { // If mat is 1.0
            // Texture lookup for bump map
            n += .4 * texture(bumpTexture, n.xy * bump_map_offset).rgb; // Use bumpTexture and tunable offset
            n = normalize(n); // Re-normalize after adding texture influence
            rd = reflect(rd, n);

            // Reflection texture lookup
            vec3 refl = vec3(0.0);
            vec2 reflectionUV = sphericalMap(rd); // Use spherical mapping for 2D texture
            refl = texture(inputImage, reflectionUV).rgb;
            
            refl *= mix(vec3(1.0), spectral_zucconi6(n.x * n.y * 3.0), iridescence_mix); // Use tunable iridescence_mix
            col = mix(col, refl, reflection_mix); // Use tunable reflection_mix

        }
        if (mat == 1.0) // This corresponds to `dr2` material
            col = mix(col, col * getColorPalette(p.z * .2 + length(p.xy * .2) * .2 + .05 * tt_global, color_palette_type), fractal_color_mix_1); // Use tunable mix and color_palette_type
        if (mat == 0.0) // This corresponds to `dr` material
            col = mix(col, col * (mix(.6, .0, length(p * .5) - .3 + .3 * sin(tt_global)) + vec3(1.000, 0.506, 0.239) * getColorPalette(length(p.xy * .4) + .6 + .1 * tt_global, color_palette_type)), fractal_color_mix_2); // Use tunable mix and color_palette_type

    } else {
        col = mix(c1 - .2, c2, background_color_mix * (.3 - pow(dot(uv_norm_res, uv_norm_res), .8))) * .2 + .04; // Use tunable background_color_mix. Use uv_norm_res for fixed background vignette.
        // outer glow (inspired from https://www.shadertoy.com/view/ldB3Rz)
        float f = 1.0 - clamp(closest_global * 0.5, 0.0, 1.0);

        float glowAmount = 0.0;

        glowAmount += pow(f, 400.0) * (glow_intensity + glow_pulse_intensity * SIN_NORM(tt_global)); // Tunable glow intensity and pulse
        vec3 glowColor = getColorPalette(length(p.xy * .2) + .6, color_palette_type); // Use selected palette for glow
        col += glowColor * glowAmount;
    }


    if (dithering_enabled) { // Tunable dithering
        col += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random(uv_norm_res)); // dithering. Use uv_norm_res for consistent dithering pattern.
    }

    col *= 1.8;
    col *= mix(.2, 1.0, (1.3 - vignette_intensity * pow(dot(uv_norm_res, uv_norm_res), .5))); // Tunable vignette intensity. Use uv_norm_res.
    col = pow(col, vec3(gamma_correction)); // Tunable gamma

    // Brightness, Saturation, Contrast
    col = adjustColor(col, brightness, saturation, contrast);


    fragColor = vec4(col, 1.0);
    gl_FragColor = fragColor;
}