/*{
  "DESCRIPTION": "Fractal Tunnel with Advanced Color Pulse & Shake Controls (No Vec3 Color Inputs)",
  "CATEGORIES": [ "Generator", "Fractal", "Tunnel" ],
  "INPUTS": [
    { "NAME": "inputTex1", "TYPE": "image" },
    { "NAME": "inputTex2", "TYPE": "image" },

    { "NAME": "speed",        "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph",        "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "shakeAmount",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "shakeSpeed",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "shakePhase",   "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "glitchAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "colorPulse",     "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Amplitude of the 'crackling light' pulse." },
    { "NAME": "colorPulseFreq", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Frequency of the 'crackling light' pulse." },
    { "NAME": "colorPulseShape","TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.9, "DESCRIPTION": "Shape of the 'crackling light' pulse (0: Sin, 1: Abs Sin, 2: Smoothstep, 3: Step)." },
    { "NAME": "flashIntensity", "TYPE": "float", "DEFAULT": 0.0002, "MIN": 0.0, "MAX": 0.0003, "DESCRIPTION": "Controls the overall intensity/brightness of the prominent light flashes. Lower values make them darker." },
    { "NAME": "flashColorR",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the flash color." },
    { "NAME": "flashColorG",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the flash color." },
    { "NAME": "flashColorB",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the flash color." },

    { "NAME": "paletteType",  "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 4.9 },

    { "NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },

    { "NAME": "fractal1", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "fractal2", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "fractal3", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 9.0 },

    { "NAME": "brightness",   "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 100.0 },
    { "NAME": "saturation",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast",     "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Strength of the vignette effect." }
  ]
}
*/

#define inf 9e9

// No explicit uniform declarations needed for float types, as they are
// implicitly handled by the ISF framework from the JSON.
// sampler2D uniforms (inputTex1, inputTex2) are always implicitly available.

// --- Color Palette Functions ---
// Now designed to provide vibrant and distinct color ranges.
vec3 applyPalette(float value, float t) {
    vec3 color = vec3(0.0);
    float p = floor(paletteType + 0.5);

    if (p == 0.0) { // Psychedelic Rainbow
        color = 0.5 + 0.5 * cos(6.28318 * (vec3(value, value + 0.166, value + 0.333) * 2.0 + t * 0.5));
    } else if (p == 1.0) { // Fiery Sunset
        color = mix(vec3(0.8, 0.0, 0.0), vec3(1.0, 0.6, 0.0), value);
        color = mix(color, vec3(1.0, 0.9, 0.5), smoothstep(0.7, 1.0, value));
        color = mix(color, vec3(0.2, 0.0, 0.2), 0.5 - abs(value - 0.5)); // Adds a deeper tone in the middle
    } else if (p == 2.0) { // Deep Ocean Blues
        color = mix(vec3(0.0, 0.1, 0.3), vec3(0.0, 0.6, 1.0), value);
        color = mix(color, vec3(0.5, 1.0, 1.0), smoothstep(0.7, 1.0, value));
    } else if (p == 3.0) { // Electric Greens & Purples
        color = mix(vec3(0.1, 0.8, 0.1), vec3(0.8, 0.2, 0.8), value);
        color = mix(color, vec3(0.3, 1.0, 0.3), smoothstep(0.8, 1.0, value));
        color = mix(color, vec3(0.6, 0.1, 0.6), 0.5 - abs(value - 0.5));
    } else { // Golden Tones
        color = mix(vec3(0.5, 0.4, 0.1), vec3(1.0, 0.8, 0.2), value);
        color = mix(color, vec3(1.0, 1.0, 0.8), smoothstep(0.7, 1.0, value));
    }
    return clamp(color, 0.0, 1.0);
}

float colorPulseValue(float t) {
    float shape = floor(colorPulseShape + 0.5);
    float v = sin(t * colorPulseFreq);
    // This value is a multiplier for the *dynamic light elements* (like the tex3D and light).
    // It controls the amplitude and shape of the pulse.
    if (shape == 0.0) return 1.0 + colorPulse * v * 0.3;
    if (shape == 1.0) return 1.0 + colorPulse * abs(v) * 0.4;
    if (shape == 2.0) return 1.0 + colorPulse * smoothstep(-1., 1., v) * 0.5;
    if (shape == 3.0) return 1.0 + colorPulse * step(0., v) * 0.6;
    return 1.0;
}

vec3 P(float z) {
    return vec3(cos(z * 0.1) * 4. + tanh(cos(z * 0.1) * 1.8) * 4.,
                tanh(cos(z * 0.15) * 0.4) * 8.,
                z);
}

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float length2(vec2 p){
    float k = (sin(TIME * speed * 0.1) * 8. + 4.) * morph;
    p = pow(abs(p), vec2(k));
    return pow(p.x + p.y, 1. / k);
}

// tex3D_grayscale now explicitly returns a single float (grayscale value)
float tex3D_grayscale(sampler2D tex, vec3 p, vec3 n) {
    vec3 nn = max((abs(n) - 0.2) * 7., 0.001);
    nn /= (nn.x + nn.y + nn.z);
    vec2 uv1 = fract(vec2(p.y, p.z));
    vec2 uv2 = fract(vec2(p.z, p.x));
    vec2 uv3 = fract(vec2(p.x, p.y));
    
    // Convert sampled texture to grayscale using luminance, then blend with normals
    float g1 = dot(texture2D(tex, uv1).rgb, vec3(0.2126, 0.7152, 0.0722));
    float g2 = dot(texture2D(tex, uv2).rgb, vec3(0.2126, 0.7152, 0.0722));
    float g3 = dot(texture2D(tex, uv3).rgb, vec3(0.2126, 0.7152, 0.0722));

    return g1 * nn.x + g2 * nn.y + g3 * nn.z;
}

vec2 shake(float t) {
    float phase = sin(t * shakeSpeed + shakePhase * 6.28);
    return vec2(sin(t * shakeSpeed * 250.), cos(t * shakeSpeed * 570.)) * 7.0 * shakeAmount * phase;
}

float fractal(vec3 q){
    vec3 p;
    float d = inf, s = 2.;
    #define MENGER(scale, minmax, hole) \
        s /= scale; \
        p = abs(fract(q/s)*s - s*.5); \
        d = minmax(d, min(max(p.x, p.y), min(max(p.y, p.z), max(p.x, p.z))) - s / hole);
    MENGER(fractal1, min, 3.0);
    MENGER(fractal2, max, 3.0);
    MENGER(fractal3, max, 3.0);
    return d;
}

float tunnel(vec3 p) {
    return (sin(p.z * .6) + 2.) -
           min(length(p.xy - P(p.z).x + 4.),
           min(length(p.x - p.y - 10.),
           min(length2(p.xy - P(p.z).xy),
               length2(p.xy - vec2(P(p.z).y)))));
}

float map(vec3 p) {
    return max(tunnel(p), fractal(p));
}

float AO(vec3 pos, vec3 nor) {
    float sca = 2.0, occ = 0.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + float(i) * 0.5 / 4.0;
        float dd = map(nor * hr + pos);
        occ += (hr - dd) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

// Post-processing function applying BCS and color pulse
vec3 post(vec3 c, float pulse_multiplier) {
    c = (c - 0.5) * contrast + 0.5;
    float gray = dot(c, vec3(0.2126, 0.7152, 0.0722));
    c = mix(vec3(gray), c, saturation);
    c *= brightness;
    
    // Apply pulse_multiplier here to globally scale the output intensity/brightness with the pulse.
    c *= pulse_multiplier; 
    return c;
}

void main() {
    float t = TIME;
    vec2 u = isf_FragNormCoord * RENDERSIZE - RENDERSIZE * 0.5;
    
    // Apply shake to fragment coordinates only if within the specified range, affecting vignette directly
    if (sin(t * speed * 0.1) < -0.05 && sin(t * speed * 0.1) > -0.45) {
        u += shake(t);
    }
    u /= RENDERSIZE.y;

    vec3 e = vec3(.01, 0, 0);
    vec3 ro = vec3(camX, camY, t * camZ);
    vec3 lookAt = P(t + 1.);
    vec3 Z = normalize(lookAt - ro);
    vec3 X = normalize(vec3(Z.z, 0, -Z.x));
    vec3 D = vec3(rot(tanh(sin(ro.z * 0.03) * 8.) * 3.) * u, 1.0)
           * mat3(-X, cross(X, Z), Z);

    float s = 0.002, d = 0., i = 0.;
    vec3 p;
    while (i++ < 200. && s > 0.001) {
        p = ro + D * d;
        d += (s = map(p) * 0.35);
    }

    vec3 r = normalize(vec3(
        map(p - e.xyy) - map(p + e.xyy),
        map(p - e.yxy) - map(p + e.yxy),
        map(p - e.yyx) - map(p + e.yyx)
    ));

    // Get grayscale base texture value
    float base_texture_val = mod(p.z, 10.0) > 5.0 ?
        tex3D_grayscale(inputTex1, p * 2.5, r) :
        tex3D_grayscale(inputTex2, p * 1.5, r) * 2.0;

    // Apply palette to the grayscale base texture value to get desired vibrant color
    vec3 final_color = applyPalette(base_texture_val, t);

    // Apply basic lighting
    final_color *= max(dot(r, normalize(ro - p)), 0.1);
    final_color *= AO(p, r);

    // Reconstruct flashColor from individual R, G, B float inputs
    vec3 reconstructedFlashColor = vec3(flashColorR, flashColorG, flashColorB);

    // Calculate the 'light' component (the source of flashes)
    // Now controlled by flashIntensity and the reconstructed flashColor
    vec3 light_flashes = abs(vec3(3,2,1) / dot(cos(0.1 * t + p * 0.1), vec3(0.01)));
    light_flashes *= flashIntensity * reconstructedFlashColor; // Apply chosen flash color and intensity
    
    // Add the light flashes to the final color to enhance brightness
    final_color += light_flashes; 

    if (glitchAmount > 0.0)
        final_color += sin(p.z * 25. + t * 10.) * glitchAmount;

    // Apply fog
    final_color *= exp(-d / 3.0);
    
    // Apply vignette using the new uniform
    final_color -= dot(-u, u) * vignetteStrength; 
    
    // Apply post-processing (brightness, saturation, contrast, and color pulse value)
    final_color = post(final_color, colorPulseValue(t));
    
    // Final tone mapping to prevent over-exposure
    final_color = final_color / (final_color + 0.155) * 1.019;

    gl_FragColor = vec4(final_color, 1.0);
}