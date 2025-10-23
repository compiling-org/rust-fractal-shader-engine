/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Fractal",
        "Text"
    ],
    "DESCRIPTION": "A mesmerizing organic fractal tunnel with animated 16-segment display text overlay, based on Kali's shader. Enhanced with extensive psychedelic tunable parameters for speed, zoom, morphing, fractal control, geometry, tunnel size and warping, and multiple psychedelic color palettes.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Zoom Factor" },
        { "NAME": "WarpScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Warp Scale" },
        { "NAME": "WarpIntensity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Warp Intensity" },
        { "NAME": "WarpOffset", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Warp Offset" },
        { "NAME": "FractalAngleSpeed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Angle Speed" },
        { "NAME": "FractalOffsetSpeed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Offset Speed" },
        { "NAME": "FractalClampMin", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "LABEL": "Fractal Clamp Min" },
        { "NAME": "FractalClampMax", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.0, "LABEL": "Fractal Clamp Max" },
        { "NAME": "FractalSubtract", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Subtract" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Fractal Iterations" },
        { "NAME": "FractalMultiply", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Multiply" },
        { "NAME": "FractalGlowStrength", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Fractal Glow Str" },
        { "NAME": "FractalGlowDecay", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Fractal Glow Decay" },
        { "NAME": "BaseColorMixX", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 2.0, "LABEL": "Base Color Mix X" },
        { "NAME": "BaseColorMixY", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.5, "MAX": 2.0, "LABEL": "Base Color Mix Y" },
        { "NAME": "BaseColorMixZ", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.5, "MAX": 2.0, "LABEL": "Base Color Mix Z" },
        { "NAME": "DistanceGlowStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Distance Glow Str" },
        { "NAME": "TextScale", "TYPE": "float", "DEFAULT": 35.0, "MIN": 10.0, "MAX": 100.0, "LABEL": "Text Scale" },
        { "NAME": "TextOffsetX", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "LABEL": "Text Offset X" },
        { "NAME": "TextOffsetY", "TYPE": "float", "DEFAULT": 4.0, "MIN": -50.0, "MAX": 50.0, "LABEL": "Text Offset Y" },
        { "NAME": "TextWobbleFreq", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0, "LABEL": "Text Wobble Freq" },
        { "NAME": "TextWobbleAmp", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5, "LABEL": "Text Wobble Amp" },
        { "NAME": "TextHueSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Hue Speed" },
        { "NAME": "TextHueYInfluence", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "LABEL": "Text Hue Y Inf" },
        { "NAME": "TextSaturation", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Saturation" },
        { "NAME": "TextValue", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Value" },
        { "NAME": "TextSmoothstepMin", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Smoothstep Min" },
        { "NAME": "TextSmoothstepMax", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Smoothstep Max" },
        { "NAME": "TextAlphaBlend", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Text Alpha Blend" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool", "3.0": "Vibrant Neon", "4.0": "Deep Space", "5.0": "Dreamy Pastel" } },
        { "NAME": "PaletteMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Mix Factor" }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

#ifndef PI
#define PI 3.14159265359
#endif

#ifndef TAU
#define TAU (2.0 * PI)
#endif

// --- Global Variables ---
vec2 ch_pos = vec2(0.0, 0.0); // Character position for 16-segment display
float d_char_segment = 1e6; // Distance for 16-segment display segments

// --- Character Display Constants (from original shader) ---
const vec2 ch_size    = vec2(1.0, 2.0) * 0.6;   // character size (X,Y)
const vec2 ch_space   = ch_size + vec2(1.5, 1.0); // character distance Vector(X,Y)
const vec2 ch_start   = vec2 (ch_space.x * -5., 1.); // start position

// --- Helper Functions for 16-Segment Display ---

mat2 rotate(float a)
{
    float c = cos(a);
    float s = sin(a);
    return mat2(c, s, -s, c);
}

// dseg now takes 'pixel_uv' as a parameter to resolve scope issues
float dseg(vec2 p0, vec2 p1, vec2 pixel_uv)
{
    vec2 dir = normalize(p1 - p0);
    vec2 cp = (pixel_uv - ch_pos - p0) * mat2(dir.x, dir.y,-dir.y, dir.x);
    return distance(cp, clamp(cp, vec2(0), vec2(distance(p0, p1), 0)));
}

bool bit(int n)
{
    return (n/2)*2 != n;
}

// ddigit now takes 'pixel_uv' as a parameter to resolve scope issues
void ddigit(int n, int nn, vec2 pixel_uv)
{
    float v = 1e6;
    if (n == 0 && nn == 0)      v = min(v, dseg(vec2(-0.405, -1.000), vec2(-0.500, -1.000), pixel_uv));
    if (bit(nn/1)) v = min(v, dseg(vec2( 0.500,  0.063), vec2( 0.500,  0.937), pixel_uv));
    if (bit(nn/2)) v = min(v, dseg(vec2( 0.438,  1.000), vec2( 0.063,  1.000), pixel_uv));
    if (bit(nn/4)) v = min(v, dseg(vec2(-0.063,  1.000), vec2(-0.438,  1.000), pixel_uv));
    if (bit(nn/8)) v = min(v, dseg(vec2(-0.500,  0.937), vec2(-0.500,  0.062), pixel_uv));
    if (bit(nn/16)) v = min(v, dseg(vec2(-0.500, -0.063), vec2(-0.500, -0.938), pixel_uv));
    if (bit(nn/32)) v = min(v, dseg(vec2(-0.438, -1.000), vec2(-0.063, -1.000), pixel_uv));
    if (bit(nn/64)) v = min(v, dseg(vec2( 0.063, -1.000), vec2( 0.438, -1.000), pixel_uv));
    if (bit(nn/128)) v = min(v, dseg(vec2( 0.500, -0.938), vec2( 0.500, -0.063), pixel_uv));
    if (bit(n/1)) v = min(v, dseg(vec2( 0.063,  0.000), vec2( 0.438, -0.000), pixel_uv));
    if (bit(n/2)) v = min(v, dseg(vec2( 0.063,  0.063), vec2( 0.438,  0.938), pixel_uv));
    if (bit(n/4)) v = min(v, dseg(vec2( 0.000,  0.063), vec2( 0.000,  0.937), pixel_uv));
    if (bit(n/8)) v = min(v, dseg(vec2(-0.063,  0.063), vec2(-0.438,  0.938), pixel_uv));
    if (bit(n/16)) v = min(v, dseg(vec2(-0.438,  0.000), vec2(-0.063, -0.000), pixel_uv));
    if (bit(n/32)) v = min(v, dseg(vec2(-0.063, -0.063), vec2(-0.438, -0.938), pixel_uv));
    if (bit(n/64)) v = min(v, dseg(vec2( 0.000, -0.938), vec2( 0.000, -0.063), pixel_uv));
    if (bit(n/128)) v = min(v, dseg(vec2( 0.063, -0.063), vec2( 0.438, -0.938), pixel_uv));
    ch_pos.x += ch_space.x;
    d_char_segment = min(d_char_segment, v);
}

// Macros for 16-segment display characters (now correctly pass the pixel_uv)
#define n0 ddigit(0x22,0xFF, pixel_uv);
#define n1 ddigit(0x02,0x81, pixel_uv);
#define n2 ddigit(0x11,0x77, pixel_uv);
#define n3 ddigit(0x11,0xE7, pixel_uv);
#define n4 ddigit(0x55,0x08, pixel_uv);
#define n5 ddigit(0x11,0xEE, pixel_uv);
#define n6 ddigit(0x11,0xFE, pixel_uv);
#define n7 ddigit(0x22,0x06, pixel_uv);
#define n8 ddigit(0x11,0xFF, pixel_uv);
#define n9 ddigit(0x11,0xEF, pixel_uv);

#define A ddigit(0x11,0x9F, pixel_uv);
#define B ddigit(0x92,0x7E, pixel_uv);
#define C ddigit(0x00,0x7E, pixel_uv);
#define D ddigit(0x44,0xE7, pixel_uv);
#define E ddigit(0x10,0x7E, pixel_uv);
#define F ddigit(0x10,0x1E, pixel_uv);
#define G ddigit(0x80,0x7E, pixel_uv);
#define H ddigit(0x11,0x99, pixel_uv);
#define I ddigit(0x44,0x66, pixel_uv);
#define J ddigit(0x44,0x36, pixel_uv);
#define K ddigit(0x92,0x18, pixel_uv);
#define L ddigit(0x00,0x78, pixel_uv);
#define M ddigit(0x0A,0x99, pixel_uv);
#define N ddigit(0x88,0x99, pixel_uv);
#define O ddigit(0x00,0xFF, pixel_uv);
#define P ddigit(0x11,0x1F, pixel_uv);
#define Q ddigit(0x80,0xFF, pixel_uv);
#define R ddigit(0x91,0x1F, pixel_uv);
#define S ddigit(0x88,0x66, pixel_uv);
#define T ddigit(0x44,0x06, pixel_uv);
#define U ddigit(0x00,0xF9, pixel_uv);
#define u ddigit(0x00,0xF0, pixel_uv);
#define V ddigit(0x22,0x18, pixel_uv);
#define W ddigit(0xA0,0x99, pixel_uv);
#define w ddigit(0xA0,0x90, pixel_uv);
#define X ddigit(0xAA,0x00, pixel_uv);
#define Y ddigit(0x4A,0x00, pixel_uv);
#define Z ddigit(0x22,0x66, pixel_uv);
#define _ ch_pos.x += ch_space.x;
#define s_dot       ddigit(0,0, pixel_uv);
#define s_minus     ddigit(0x11,0x00, pixel_uv);
#define s_plus      ddigit(0x55,0x00, pixel_uv);
#define s_greater   ddigit(0x28,0x00, pixel_uv);
#define s_less      ddigit(0x82,0x00, pixel_uv);
#define s_sqrt      ddigit(0x0C,0x02, pixel_uv);
#define s_sw        ddigit(0x55,0xAA, pixel_uv);
#define s_pow       ddigit(0x02,0x01, pixel_uv);
#define upper_u     ddigit(0x11,0x09, pixel_uv);
#define s_bra      ddigit(0x00,0x3C, pixel_uv);
#define s_ket      ddigit(0x00,0xC3, pixel_uv);
#define s_quotl     ddigit(0x04,0x01, pixel_uv);
#define s_quotr     ddigit(0x04,0x08, pixel_uv);
#define s_degrees   ddigit(0x05,0x03, pixel_uv);
#define s_ast      ddigit(0xFF,0x00, pixel_uv);
#define s_question ch_pos-=vec2(-.45,.4); ddigit(0,0, pixel_uv); ch_pos+=vec2(-ch_space.x-.45,.4); ddigit(0x41,0x07, pixel_uv);
#define s_exclam   ch_pos-=vec2(-.45,.4); ddigit(0,0, pixel_uv); ch_pos+=vec2(-ch_space.x-.45,.4); ddigit(0x44,0x00, pixel_uv);
#define s_comma    ch_pos-=vec2(.45); ddigit(0x20,0x00, pixel_uv); ch_pos+=vec2(.45);
#define nl1 ch_pos = ch_start;  ch_pos.y -= 3.0;
#define nl2 ch_pos = ch_start;  ch_pos.y -= 6.0;
#define nl3 ch_pos = ch_start;  ch_pos.y -= 9.0;


// --- Color Conversion Functions ---

// HSV to RGB conversion with cubic smoothing
vec3 hsv2rgb_smooth( in vec3 c )
{
    vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
    rgb = rgb*rgb*(3.0-2.0*rgb); // cubic smoothing
    return c.z * mix( vec3(1.0), rgb, c.y);
}

// Custom hue function for multiple color palettes
vec3 getPaletteColor(float h_val, float palette_type) {
    vec3 color_base;
    h_val = mod(h_val, 1.0); // Ensure hue is within [0, 1) range

    if (palette_type < 0.5) { // Palette 0: Classic Psychedelic (sin waves)
        color_base = clamp(abs(sin(h_val * TAU + vec3(0.0, 2.0, 4.0))), 0.0, 1.0);
    } else if (palette_type < 1.5) { // Palette 1: Warm Hues (Reds, Oranges, Yellows)
        h_val = mod(h_val, 1.0) * 0.3 + 0.0; // Restrict hue to warm range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 2.5) { // Palette 2: Cool Tones (Blues, Purples, Greens)
        h_val = mod(h_val, 1.0) * 0.3 + 0.6; // Restrict hue to cool range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 3.5) { // Palette 3: Vibrant Neon
        color_base = hsv2rgb_smooth(vec3(h_val, 0.9, 0.8)); // Static saturation/value for vibrant
    } else if (palette_type < 4.5) { // Palette 4: Deep Space (Darker blues/purples with bright accents)
        color_base = hsv2rgb_smooth(vec3(mod(h_val * 0.5 + 0.6, 1.0), 0.7, 0.3)); // Base deep color
        color_base = mix(color_base, hsv2rgb_smooth(vec3(mod(h_val * 1.5 + 0.1, 1.0), 0.8, 0.9)), 0.2); // Static vibrant accents
    } else { // Palette 5: Dreamy Pastel
        color_base = hsv2rgb_smooth(vec3(h_val, 0.3, 0.7)); // Static saturation/value for pastel
    }
    return color_base;
}

// Define a constant for the maximum number of fractal iterations to satisfy GLSL ES loop requirements
const int MAX_FRACTAL_ITERATIONS = 10; 

// --- Fractal Function ---
vec3 fractal(vec2 p, float t_scaled) {
    // Warp and morphing
    p = vec2(abs(atan(p.x, p.y) / PI * sin(t_scaled * FractalAngleSpeed) * WarpScale), length(p));
    p.y -= tan(t_scaled * FractalOffsetSpeed);
    p.x = fract(p.x - t_scaled * WarpOffset) + 0.5;

    vec2 m = vec2(1000);
    float ml = 100.;
    
    // Corrected loop for GLSL ES compatibility
    for (int i = 0; i < MAX_FRACTAL_ITERATIONS; i++) {
        // Break early if the desired number of iterations is reached
        if (i >= int(FractalIterations)) { 
            break;
        }
        p = abs(p) / clamp(abs(p.x * p.y), FractalClampMin, FractalClampMax) - FractalSubtract;
        m = min(m, abs(p)) + fract(p.x * 0.2 + t_scaled * 0.5) + fract(p.y * 0.2 + t_scaled);
        ml = min(ml, length(p));
    }
    m = exp(-1.5 * m);
    vec3 c = vec3(m.x * 3., length(m), m.y * 2.) * FractalMultiply;

    ml = exp(-FractalGlowDecay * ml) * FractalGlowStrength;
    c += ml;
    return c;
}

void main() {
    float t_scaled = TIME * GlobalSpeed;
    vec2 uv_frag = gl_FragCoord.xy / RENDERSIZE.xy; // Normalized UV for screen
    vec2 uv_aspect = -1. + 2. * uv_frag; // UV from -1 to 1, aspect ratio corrected
    uv_aspect.x *= RENDERSIZE.x / RENDERSIZE.y;

    // --- Fractal Background ---
    vec2 fractal_uv = uv_aspect;
    fractal_uv.x *= 1. - fractal_uv.y * WarpIntensity; // Vertical warp
    fractal_uv *= ZoomFactor; // Global zoom
    
    // Apply smoothstep based on length of UV for additional pattern
    float t_fractal = t_scaled;
    t_fractal += smoothstep(0.0, 0.2, fract(length(fractal_uv * 0.5) - t_scaled * 0.5));

    vec3 fractal_color = fractal(fractal_uv, t_fractal);
    fractal_color += exp(-FractalGlowDecay * length(fractal_uv)) * DistanceGlowStrength; // Distance glow
    fractal_color *= vec3(BaseColorMixX, BaseColorMixY, BaseColorMixZ); // Base color mixing

    // --- Text Overlay ---
    vec2 text_uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y; // Text UV space
    text_uv *= TextScale;
    text_uv -= vec2(TextOffsetX, TextOffsetY); // Text position offset

    // Text wobble
    text_uv.x += 3. + sin(text_uv.y * TextWobbleFreq + t_scaled * 3.) * TextWobbleAmp;

    // FIX: Define 'pixel_uv' in this scope for the ddigit macros to use.
    vec2 pixel_uv = text_uv; 
    
    ch_pos = ch_start; // Reset character position
    d_char_segment = 1e6; // Reset segment distance for each pixel

    // Define the text to display
    nl2
    _ _ _ _ _ _ _ _ _ _ _ _ _ _ _

    // Calculate text color and alpha
    float text_alpha = smoothstep(TextSmoothstepMin, TextSmoothstepMax, d_char_segment);
    vec3 text_ch_color = hsv2rgb_smooth(vec3(t_scaled * TextHueSpeed + text_uv.y * TextHueYInfluence, TextSaturation, TextValue));
    vec3 text_col = text_ch_color * text_alpha;

    // Blend text with fractal background
    vec3 final_color = mix(fractal_color, text_col, text_alpha * TextAlphaBlend);

    // --- Apply Color Palette to entire scene ---
    // Use a value derived from the scene (e.g., length of fractal_uv or time) to drive hue
    float palette_hue_source = fract(length(fractal_uv) * 0.1 + t_scaled * 0.05);
    vec3 palette_color = getPaletteColor(palette_hue_source, PaletteChoice);
    final_color = mix(final_color, final_color * palette_color, PaletteMixFactor);

    // --- Final Brightness ---
    gl_FragColor = vec4(final_color * GlobalBrightness, 1.0);
}