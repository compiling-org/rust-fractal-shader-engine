/* {
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Psychedelic grid with 5 TRIPPY color palettes & pulsing edges",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "LineWidth",
            "TYPE": "float",
            "MIN": 0.01,
            "MAX": 0.1,
            "DEFAULT": 0.03,
            "LABEL": "Line Width"
        },
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 0.4,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "ColorPalette",
            "TYPE": "long",
            "VALUES": ["Electric Dreams", "Acid Trip", "Deep Ocean", "Solar Flare", "Neon Noir"],
            "DEFAULT": "Electric Dreams",
            "LABEL": "Color Palette"
        },
        {
            "NAME": "PulseSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Pulse Speed"
        },
        {
            "NAME": "PulseIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Pulse Intensity"
        },
        {
            "NAME": "MorphAmount",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Morph Amount"
        },
        {
            "NAME": "Saturation",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 3.0,
            "DEFAULT": 1.5,
            "LABEL": "Color Saturation"
        }
    ]
} */

const float PI = 3.1415926;

#define cexp(z) (exp((z).x) * vec2(cos((z).y), sin((z).y)))
#define clog(z) vec2(0.5 * log(dot(z, z)), atan((z).y, (z).x))

// TRIPPY COLOR PALETTES ======================================================
vec3 getElectricDreams(float t) {
    // High-contrast electric colors
    vec3 c = vec3(
        abs(sin(t * PI * 2.7)) * 1.2,
        abs(cos(t * PI * 1.7)) * 1.1,
        abs(sin(t * PI * 3.3)) * 1.3
    );
    return clamp(c, 0.0, 1.0) * Saturation;
}

vec3 getAcidTrip(float t) {
    // Wildly shifting unnatural colors
    vec3 c = vec3(
        fract(t * 0.617),
        fract(t * 0.537),
        fract(t * 0.457)
    );
    return (0.8 + 0.6 * sin(c * PI * 2.0)) * Saturation;
}

vec3 getDeepOcean(float t) {
    // Cool blues with glowing teal accents
    vec3 c = vec3(
        0.3 + 0.3 * sin(t * 2.0),
        0.5 + 0.4 * sin(t * 1.3),
        0.7 + 0.3 * sin(t * 0.7)
    );
    return pow(c, vec3(1.5)) * Saturation;
}

vec3 getSolarFlare(float t) {
    // Hot plasma colors
    vec3 c = vec3(
        0.8 + 0.5 * sin(t * 1.3),
        0.5 + 0.5 * sin(t * 0.7),
        0.2 + 0.3 * sin(t * 0.3)
    );
    return c * c * Saturation;
}

vec3 getNeonNoir(float t) {
    // Blacklight-style neon palette
    vec3 c = vec3(
        abs(sin(t * 4.0)) * 0.8,
        0.1 + abs(cos(t * 3.0)) * 0.9,
        abs(sin(t * 5.0)) * 0.7
    );
    return pow(c, vec3(3.0)) * Saturation;
}

vec3 getPaletteColor(float t) {
    if (ColorPalette == 0) return getElectricDreams(t);
    else if (ColorPalette == 1) return getAcidTrip(t);
    else if (ColorPalette == 2) return getDeepOcean(t);
    else if (ColorPalette == 3) return getSolarFlare(t);
    else return getNeonNoir(t);
}
// ============================================================================

float sfract(float x) {
    x = fract(x);
    return x * smoothstep(1.0, 0.0, x);
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    
    // Transform coordinates with morphing
    uv = clog(uv);
    uv.x = sfract(uv.x - TIME * AnimationSpeed) * 0.5;
    float s = sqrt(sqrt(uv.x));
    uv.y += sin(s * s * PI * 1.5 - TIME * AnimationSpeed) * MorphAmount;
    uv = cexp(uv);
    
    uv *= 4.0;

    // Grid calculation
    float line_width = LineWidth;
    vec2 uv_deriv = vec2(0.0); // Initialize derivative vector

    // Approximate derivatives using finite differences
    vec2 uv_plus_x = clog(uv + vec2(0.001, 0.0));
    vec2 uv_plus_y = clog(uv + vec2(0.0, 0.001));
    uv_deriv.x = length(uv_plus_x - uv);
    uv_deriv.y = length(uv_plus_y - uv);

    vec2 draw_width = max(vec2(line_width), uv_deriv);
    vec2 grid_uv = abs(fract(uv) * 2.0 - 1.0);
    vec2 grid2 = smoothstep(uv_deriv * 1.5, -uv_deriv * 1.5, grid_uv - draw_width);
    grid2 *= min(line_width / draw_width, 1.0);
    float grid = mix(grid2.x, 1.0, grid2.y);

    // Select color from palette
    vec3 col = getPaletteColor(TIME * 0.2 + uv.x * 0.1 + uv.y * 0.1);
    
    // Edge detection for pulsing effect
    vec2 fuv = abs(fract(uv - 0.5) * 2.0 - 1.0);
    float edge = 1.0 - smoothstep(0.0, line_width * 2.0, min(fuv.x, fuv.y));
    
    // Add pulsing effect
    float pulse = 0.5 + 0.5 * sin(TIME * PulseSpeed * 5.0);
    edge *= pulse * PulseIntensity;
    
    // Combine colors with grid and pulse
    col = mix(col, vec3(1.0), grid);
    col += edge * getPaletteColor(TIME * 0.5) * 2.0;
    col *= s * 2.0;

    gl_FragColor = vec4(col, 1);
}
