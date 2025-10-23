/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Distortion",
        "Effect"
    ],
    "DESCRIPTION": "A complex 2-pass Kelenian fractal shader featuring dynamic camera, intricate geometry, psychedelic colors, text overlay, and optional Depth of Field. Optimized for ISF with numerous tunable parameters for animation, color, and post-processing effects.",
    "CREDIT": "Original shader by Sebastien Durand (https://www.shadertoy.com/view/ldSyRd), Text by Andre (https://www.shadertoy.com/view/lddXzM), converted and enhanced for ISF by Gemini."
    ,
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall animation speed."
        },
        {
            "NAME": "cameraZoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the camera's initial depth/zoom level (multiplies base Z position)."
        },
        {
            "NAME": "cameraPanX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "Horizontal camera pan/offset."
        },
        {
            "NAME": "cameraPanY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "Vertical camera pan/offset."
        },
        {
            "NAME": "cameraRoll",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -3.14159,
            "MAX": 3.14159,
            "STEP": 0.01,
            "DESCRIPTION": "Rotates the camera view around the Z-axis."
        },
        {
            "NAME": "fractalMorph",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Morphs between different fractal variations (replaces min/max interpolation for custom morph)."
        },
        {
            "NAME": "fractalFoldFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the folding intensity in the fractal's distance function (affects the 'k' factor, 1.0 is default)."
        },
        {
            "NAME": "fractalOffsetStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the strength of a subtle, time-varying positional offset in the fractal iterations."
        },
        {
            "NAME": "paletteSelection",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 7.0,
            "STEP": 0.01,
            "DESCRIPTION": "Continuously blend between different psychedelic color themes (0-7)."
        },
        {
            "NAME": "colorPulseFactor",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of time-based color pulsing within the selected palette."
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of time-based color pulsing."
        },
        {
            "NAME": "colorPulseOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Offset for the color pulse animation start point."
        },
        {
            "NAME": "colorPulseExponent",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Exponent for the color pulse interpolation, changes its curve."
        },
        {
            "NAME": "psychedelicShimmerIntensity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the psychedelic shimmering effect on colors."
        },
        {
            "NAME": "reflectiveness",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the mix of procedural environment reflection for the fractal material (0.0 = no reflection, 1.0 = full reflection)."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image brightness."
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image color saturation."
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image contrast."
        },
        {
            "NAME": "shakeIntensity",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Intensity of the camera shake effect."
        },
        {
            "NAME": "shakeSpeed",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 0.0,
            "MAX": 20.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of the camera shake effect."
        },
        {
            "NAME": "glitchIntensity",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the color channel glitch effect."
        },
        {
            "NAME": "glitchSpeed",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 0.0,
            "MAX": 50.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of the glitch animation."
        },
        {
            "NAME": "glitchFrequency",
            "TYPE": "float",
            "DEFAULT": 5.0,
            "MIN": 0.0,
            "MAX": 20.0,
            "STEP": 0.1,
            "DESCRIPTION": "Frequency of glitch occurrences."
        },
        {
            "NAME": "dofEnabled",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 1.0,
            "DESCRIPTION": "Enable or disable Depth of Field effect (0 = off, 1 = on)."
        },
        {
            "NAME": "dofAperture",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "Aperture size for Depth of Field effect. Higher values mean more blur."
        },
        {
            "NAME": "dofFocusOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Offset for the Depth of Field focus distance. Adjusts where the sharpest point is."
        },
        {
            "NAME": "textEnabled",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 1.0,
            "DESCRIPTION": "Enable or disable 'KELENIAN' text overlay (0 = off, 1 = on)."
        }
    ]
}
*/

// ISF standard uniforms (TIME, RENDERSIZE, and all inputs from JSON) are automatically declared by the ISF host.

#define BACK_COLOR vec3(.08, .16, .34)
#define PRECISION_FACTOR 5e-4
#define MIN_DIST_RAYMARCHING .01
#define MAX_DIST_RAYMARCHING 4.
#define MAX_RAYMACING_ITERATION 132

#define MIN_DIST_SHADOW 10.*PRECISION_FACTOR
#define MAX_DIST_SHADOW .25
#define PRECISION_FACTOR_SHADOW 3.*PRECISION_FACTOR

#define MIN_DIST_AO .5*PRECISION_FACTOR
#define MAX_DIST_AO .02
#define PRECISION_FACTOR_AO PRECISION_FACTOR

#define LIGHT_VEC normalize(vec3(.2,.7, 1.6) )

#define NB_ITERATION 7
#define BPM 127.0 // From original shader

// Constants for DOF
const float fov_dof = 3.;
const float GA =2.399;  // golden angle = 2pi/(1+phi)
const mat2 rot = mat2(cos(GA),sin(GA),-sin(GA),cos(GA));


// Global-like variables for fractal state (mimicking the two-pass approach)
// These will be calculated in renderFractalScene and used in trace/de/ce
vec2 kColor_global;
vec4 mins_global;
vec4 maxs_global;

// --- Helper Functions ---

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// Function to generate psychedelic rainbow colors with shimmer
vec3 getSelectedPaletteColor(float paletteIdx, float t_mix_base, float current_time_param, vec2 frag_uv) {
    float t_mix = pow(t_mix_base, colorPulseExponent);

    float shimmer_val_time = sin(current_time_param * 15.0 + t_mix * 25.0);
    float shimmer_val_spatial = sin(frag_uv.x * 50.0 + current_time_param * 18.0) * 0.5;
    shimmer_val_spatial += cos(frag_uv.y * 60.0 + current_time_param * 22.0) * 0.5;
    
    float total_shimmer = (shimmer_val_time + shimmer_val_spatial) * 0.5; 
    total_shimmer = (total_shimmer * 0.5 + 0.5) * psychedelicShimmerIntensity; 

    float hue_base = fract(t_mix + total_shimmer * 0.5); 
    
    float sat_base = 0.8 + 0.2 * sin(t_mix * 7.0 + current_time_param * 0.5);
    float val_base = 0.8 + 0.2 * cos(t_mix * 9.0 + current_time_param * 0.7);

    // Dynamic hue shift based on palette selection
    float hue_shift = paletteIdx * 0.125; 
    float sat_mod = 1.0 + sin(paletteIdx * 0.5) * 0.3; 
    float val_mod = 1.0 + cos(paletteIdx * 0.7) * 0.2; 

    float final_hue = fract(hue_base + hue_shift);
    float final_sat = clamp(sat_base * sat_mod, 0.0, 1.0);
    float final_val = clamp(val_base * val_mod, 0.0, 1.0);
    
    vec3 final_color = hsv2rgb(vec3(final_hue, final_sat, final_val));

    if (psychedelicShimmerIntensity > 0.001) {
        float extreme_shimmer_pulse = sin(current_time_param * 25.0 + t_mix * 60.0 + frag_uv.x * 10.0 + frag_uv.y * 10.0);
        float shimmer_threshold = 0.7 + (1.0 - clamp(psychedelicShimmerIntensity, 0.0, 1.0)) * 0.1; 

        if (extreme_shimmer_pulse > shimmer_threshold) { 
            final_color = mix(final_color, vec3(1.0), (extreme_shimmer_pulse - shimmer_threshold) / (1.0 - shimmer_threshold) * psychedelicShimmerIntensity);
        } else if (extreme_shimmer_pulse < -shimmer_threshold) { 
            final_color = mix(final_color, vec3(0.0), (abs(extreme_shimmer_pulse) - shimmer_threshold) / (1.0 - shimmer_threshold) * psychedelicShimmerIntensity);
        } else if (abs(extreme_shimmer_pulse) > 0.4 + (1.0 - clamp(psychedelicShimmerIntensity, 0.0, 1.0)) * 0.2) { 
             final_color = mix(final_color, 1.0 - final_color, (abs(extreme_shimmer_pulse) - (0.4 + (1.0 - clamp(psychedelicShimmerIntensity, 0.0, 1.0)) * 0.2)) * 2.0 * psychedelicShimmerIntensity * 0.5);
        }
    }

    return final_color;
}


// --- Original Fractal Geometry and Raymarching Functions ---

float de(vec3 p) {
    float k, scale=1.;
    // Apply fractalOffsetStrength as a subtle positional offset to 'p'
    p += fractalOffsetStrength * vec3(
        sin(TIME * 0.7 + p.x), 
        cos(TIME * 0.9 + p.y), 
        sin(TIME * 1.1 + p.z)
    ) * 0.05; // Small multiplier to keep it subtle and stable

    for(int i=0;i<NB_ITERATION;i++) {
        p = 2.*clamp(p, mins_global.xyz,maxs_global.xyz)-p;
        // Apply fractalFoldFactor to the 'k' calculation
        // A factor > 1 makes the folding stronger (k becomes larger faster)
        // A factor < 1 makes the folding weaker (k becomes smaller or grows slower)
        k = max(mins_global.w / dot(p,p), 1.) * fractalFoldFactor; 
        p *= k;
        scale *= k;
    }
    float rxy = length(p.xy);
    return .7*max(rxy-maxs_global.w, (rxy*p.z) / length(p))/scale;
}

float ce(vec3 p) {
    float k,r2, orb = 1.;
    // Apply fractalOffsetStrength as a subtle positional offset to 'p'
    p += fractalOffsetStrength * vec3(
        sin(TIME * 0.7 + p.x), 
        cos(TIME * 0.9 + p.y), 
        sin(TIME * 1.1 + p.z)
    ) * 0.05; // Small multiplier to keep it subtle and stable

    for(int i=0;i<NB_ITERATION;i++) {
        p = 2.*clamp(p, mins_global.xyz, maxs_global.xyz)-p;
        r2 = dot(p,p);
        orb = min(orb, r2);
        // Apply fractalFoldFactor to the 'k' calculation
        k = max(mins_global.w / r2, 1.) * fractalFoldFactor;
        p *= k;
    }
    return kColor_global.x + kColor_global.y*sqrt(orb);
}

float rayIntersect(const vec3 ro, const vec3 rd, const float prec, const float mind, const float maxd) {
    float h, t = mind;
    for(int i=0; i<MAX_RAYMACING_ITERATION; i++ ) {
        h = de(ro+rd*t);
        if (h<prec*t||t>maxd)break;
        t += h;
    }
    return t;
}

vec2 trace(const vec3 ro, const vec3 rd ) {
    float d = rayIntersect(ro, rd, PRECISION_FACTOR, MIN_DIST_RAYMARCHING, MAX_DIST_RAYMARCHING);
    return (d>0.) ? vec2(d, ce(ro+rd*d)) : vec2(-1., 1.);
}

#define WITH_SHADOWS // Enable shadows as per original
#define WITH_AO // Enable AO as per original

#ifdef WITH_SHADOWS
float shadow(vec3 ro, vec3 rd) {
    float d = rayIntersect(ro, rd, PRECISION_FACTOR_SHADOW, MIN_DIST_SHADOW, MAX_DIST_SHADOW);
    return (d>0.) ? smoothstep(0., MAX_DIST_SHADOW, d) : 1.;
}
#endif

#ifdef WITH_AO
float calcAO4(const vec3 pos, const vec3 nor ) {
    float hr, occ = 0., sca = 1.;
    for(int i=0; i<8; i++ ) {
        hr = MIN_DIST_AO + MAX_DIST_AO*float(i)/4.;
        occ += -(de( nor * hr + pos)-hr)*sca;
        sca *= .95;
    }
    return clamp(1. - 8.*occ, 0., 1.);    
}
#endif

vec3 calcNormal(const vec3 pos, const float t ){
    vec3 e = (PRECISION_FACTOR * t * .57) * vec3(1, -1, 0);
    return normalize(e.xyy*de(pos + e.xyy) + 
		 e.yyx*de(pos + e.yyx) + 
		e.yxy*de(pos + e.yxy) + 
	e.xxx*de(pos + e.xxx) );
}

// Helper function to get array values in GLSL ES 2.0
float get_camx(int index) {
    if (index == 0) return .2351;
    else if (index == 1) return 1.2351;
    else if (index == 2) return 1.2351;
    else if (index == 3) return 1.;
    else if (index == 4) return .2;
    else if (index == 5) return .41;
    else if (index == 6) return .5;
    else if (index == 7) return .084;
    else if (index == 8) return .145;
    else if (index == 9) return 3.04;
    else if (index == 10) return .12;
    else if (index == 11) return .44;
    else if (index == 12) return .44;
    else if (index == 13) return .416;
    else if (index == 14) return -1.404;
    else if (index == 15) return .21;
    else if (index == 16) return .2351;
    return 0.0; // Should not happen with correct indexing
}

float get_camy(int index) {
    if (index == 0) return -.094;
    else if (index == 1) return .35;
    else if (index == 2) return .28;
    else if (index == 3) return .38;
    else if (index == 4) return .04;
    else if (index == 5) return .11;
    else if (index == 6) return .35;
    else if (index == 7) return .0614;
    else if (index == 8) return .418;
    else if (index == 9) return 1.;
    else if (index == 10) return -.96;
    else if (index == 11) return .67;
    else if (index == 12) return .8;
    else if (index == 13) return .0;
    else if (index == 14) return -1.;
    else if (index == 15) return -.06;
    else if (index == 16) return -.094;
    return 0.0;
}

float get_camz(int index) {
    if (index == 0) return .608;
    else if (index == 1) return .608;
    else if (index == 2) return .35;
    else if (index == 3) return .3608;
    else if (index == 4) return -.03;
    else if (index == 5) return .48;
    else if (index == 6) return .47;
    else if (index == 7) return 0.201;
    else if (index == 8) return .05;
    else if (index == 9) return .28;
    else if (index == 10) return .3;
    else if (index == 11) return 1.445;
    else if (index == 12) return 1.;
    else if (index == 13) return 1.4;
    else if (index == 14) return 2.019;
    else if (index == 15) return .508;
    else if (index == 16) return .608;
    return 0.0;
}

float get_lookx(int index) {
    if (index == 0) return -.73;
    else if (index == 1) return -.627;
    else if (index == 2) return -1.;
    else if (index == 3) return -.3;
    else if (index == 4) return -1.;
    else if (index == 5) return -.72;
    else if (index == 6) return -.67;
    else if (index == 7) return -.5;
    else if (index == 8) return -.07;
    else if (index == 9) return -.67;
    else if (index == 10) return -.27;
    else if (index == 11) return -.35;
    else if (index == 12) return -.35;
    else if (index == 13) return -.775;
    else if (index == 14) return .08;
    else if (index == 15) return -.727;
    else if (index == 16) return -.727;
    return 0.0;
}

float get_looky(int index) {
    if (index == 0) return -.364;
    else if (index == 1) return -.2;
    else if (index == 2) return -.2;
    else if (index == 3) return -.2;
    else if (index == 4) return 0.;
    else if (index == 5) return -.39;
    else if (index == 6) return -.56;
    else if (index == 7) return -.37;
    else if (index == 8) return -.96;
    else if (index == 9) return -.74;
    else if (index == 10) return -.94;
    else if (index == 11) return -.35;
    else if (index == 12) return -.35;
    else if (index == 13) return -.1;
    else if (index == 14) return .83;
    else if (index == 15) return -.364;
    else if (index == 16) return -.364;
    return 0.0;
}

float get_lookz(int index) {
    if (index == 0) return -.582;
    else if (index == 1) return -.582;
    else if (index == 2) return -.5;
    else if (index == 3) return -.35;
    else if (index == 4) return -.0;
    else if (index == 5) return -.58;
    else if (index == 6) return -.48;
    else if (index == 7) return -.79;
    else if (index == 8) return -.25;
    else if (index == 9) return .06;
    else if (index == 10) return -.18;
    else if (index == 11) return -.87;
    else if (index == 12) return -.87;
    else if (index == 13) return .23;
    else if (index == 14) return .55;
    else if (index == 15) return -.582;
    else if (index == 16) return -.582;
    return 0.0;
}


float get_minsx(int index) {
    if (index == 0) return -.3252;
    else if (index == 1) return -.3252;
    else if (index == 2) return -.3252;
    else if (index == 3) return -.3252;
    else if (index == 4) return -.3252;
    else if (index == 5) return -.3252;
    else if (index == 6) return -1.05;
    else if (index == 7) return -1.05;
    else if (index == 8) return -1.21;
    else if (index == 9) return -1.22;
    else if (index == 10) return -1.04;
    else if (index == 11) return -0.737;
    else if (index == 12) return -.62;
    else if (index == 13) return -10.;
    else if (index == 14) return -.653;
    else if (index == 15) return -.653;
    else if (index == 16) return -.3252;
    return 0.0;
}

float get_minsy(int index) {
    if (index == 0) return -.7862;
    else if (index == 1) return -.7862;
    else if (index == 2) return -.7862;
    else if (index == 3) return -.7862;
    else if (index == 4) return -.7862;
    else if (index == 5) return -.7862;
    else if (index == 6) return -1.05;
    else if (index == 7) return -1.05;
    else if (index == 8) return -.954;
    else if (index == 9) return -1.17;
    else if (index == 10) return -.79;
    else if (index == 11) return -0.73;
    else if (index == 12) return -.71;
    else if (index == 13) return -.75;
    else if (index == 14) return -2.;
    else if (index == 15) return -2.;
    else if (index == 16) return -.7862;
    return 0.0;
}

float get_minsz(int index) {
    if (index == 0) return -.0948;
    else if (index == 1) return -.0948;
    else if (index == 2) return -.0948;
    else if (index == 3) return -.0948;
    else if (index == 4) return -.0948;
    else if (index == 5) return -.0948;
    else if (index == 6) return -0.0001;
    else if (index == 7) return -0.0001;
    else if (index == 8) return -.0001;
    else if (index == 9) return -.032;
    else if (index == 10) return -.126;
    else if (index == 11) return -1.23;
    else if (index == 12) return -.85;
    else if (index == 13) return -.787;
    else if (index == 14) return -.822;
    else if (index == 15) return -1.073;
    else if (index == 16) return -.0948;
    return 0.0;
}

float get_minsw(int index) {
    if (index == 0) return .69;
    else if (index == 1) return .69;
    else if (index == 2) return .69;
    else if (index == 3) return .69;
    else if (index == 4) return .69;
    else if (index == 5) return .678;
    else if (index == 6) return .7;
    else if (index == 7) return .73;
    else if (index == 8) return 1.684;
    else if (index == 9) return 1.49;
    else if (index == 10) return .833;
    else if (index == 11) return .627;
    else if (index == 12) return .77;
    else if (index == 13) return .826;
    else if (index == 14) return 1.8976;
    else if (index == 15) return 1.8899;
    else if (index == 16) return .69;
    return 0.0;
}

float get_maxsx(int index) {
    if (index == 0) return .35;
    else if (index == 1) return .3457;
    else if (index == 2) return .3457;
    else if (index == 3) return .3457;
    else if (index == 4) return .3457;
    else if (index == 5) return .3457;
    else if (index == 6) return 1.05;
    else if (index == 7) return 1.05;
    else if (index == 8) return .39;
    else if (index == 9) return .85;
    else if (index == 10) return .3457;
    else if (index == 11) return .73;
    else if (index == 12) return .72;
    else if (index == 13) return 5.;
    else if (index == 14) return .888;
    else if (index == 15) return .735;
    else if (index == 16) return .35;
    return 0.0;
}

float get_maxsy(int index) {
    if (index == 0) return 1.;
    else if (index == 1) return 1.0218;
    else if (index == 2) return 1.0218;
    else if (index == 3) return 1.0218;
    else if (index == 4) return 1.0218;
    else if (index == 5) return 1.0218;
    else if (index == 6) return 1.0218;
    else if (index == 7) return 1.05;
    else if (index == 8) return 1.05;
    else if (index == 9) return .65;
    else if (index == 10) return .65;
    else if (index == 11) return 1.0218;
    else if (index == 12) return 0.73;
    else if (index == 13) return .74;
    else if (index == 14) return 1.67;
    else if (index == 15) return .1665;
    else if (index == 16) return 1.;
    return 0.0;
}

float get_maxsz(int index) {
    if (index == 0) return 1.22;
    else if (index == 1) return 1.2215;
    else if (index == 2) return 1.2215;
    else if (index == 3) return 1.2215;
    else if (index == 4) return 1.2215;
    else if (index == 5) return 1.2215;
    else if (index == 6) return 1.2215;
    else if (index == 7) return 1.27;
    else if (index == 8) return 1.4;
    else if (index == 9) return 1.27;
    else if (index == 10) return 1.27;
    else if (index == 11) return 1.2215;
    else if (index == 12) return .73;
    else if (index == 13) return .74;
    else if (index == 14) return .775;
    else if (index == 15) return 1.2676;
    else if (index == 16) return 1.22;
    return 0.0;
}

float get_maxsw(int index) {
    if (index == 0) return .84;
    else if (index == 1) return .84;
    else if (index == 2) return .84;
    else if (index == 3) return .84;
    else if (index == 4) return .84;
    else if (index == 5) return .9834;
    else if (index == 6) return .9834;
    else if (index == 7) return .95;
    else if (index == 8) return .93;
    else if (index == 9) return 2.74;
    else if (index == 10) return 1.23;
    else if (index == 11) return .9834;
    else if (index == 12) return .8335;
    else if (index == 13) return .14;
    else if (index == 14) return 1.172;
    else if (index == 15) return .7798;
    else if (index == 16) return .84;
    return 0.0;
}

float get_deph_arr(int index) {
    if (index == 0) return 1.;
    else if (index == 1) return .65;
    else if (index == 2) return .6;
    else if (index == 3) return .4;
    else if (index == 4) return .2;
    else if (index == 5) return .4;
    else if (index == 6) return .65;
    else if (index == 7) return .11;
    else if (index == 8) return .13;
    else if (index == 9) return 1.3;
    else if (index == 10) return .49;
    else if (index == 11) return 1.2;
    else if (index == 12) return 1.2;
    else if (index == 13) return .5;
    else if (index == 14) return .65;
    else if (index == 15) return .45;
    else if (index == 16) return 1.;
    return 0.0;
}


vec4 renderFractalScene(vec2 fragCoord, float current_time_buffer_pass) {
    vec2 res = RENDERSIZE.xy;
    vec2 q = fragCoord / res;

    // --- Interpolate positions and fractal configuration ---
    const int NB_FRAMES = 17; // This constant is fine.

    float t_interp_base = .1*current_time_buffer_pass;
    float kt = smoothstep(0.,1.,fract(t_interp_base));
    
    // Use floor and mod for index
    int i0 = int(mod(floor(t_interp_base), float(NB_FRAMES)));
    int i1 = int(mod(float(i0 + 1), float(NB_FRAMES))); 

    // Base camera and look-at vectors from original keyframes
    vec3 ro_base = mix(vec3(get_camx(i0),get_camy(i0),get_camz(i0)), vec3(get_camx(i1),get_camy(i1),get_camz(i1)), kt);
    vec3 ww_base = mix(vec3(get_lookx(i0),get_looky(i0),get_lookz(i0)), vec3(get_lookx(i1),get_looky(i1),get_lookz(i1)), kt);
    
    // Apply ISF input overrides to camera
    ro_base.x += cameraPanX;
    ro_base.y += cameraPanY;
    ro_base.z *= cameraZoom; // Apply zoom to the base Z position

    vec3 vv = -normalize(cross(ww_base, vec3(0,1,0)));
    vec3 uu = -normalize(cross(vv,ww_base));
    
    vec3 er = vec3((2. * q.x - 1.) * res.x/res.y,  (2. * q.y - 1.), 3.);
    vec3 rd = normalize(er.x*uu + er.y*vv + er.z*ww_base );

    // Apply camera shake to the ray direction or camera origin
    if (shakeIntensity > 0.001) {
        float shake_offset_x = sin(current_time_buffer_pass * shakeSpeed * 1.3) * cos(current_time_buffer_pass * shakeSpeed * 0.7) * shakeIntensity;
        float shake_offset_y = cos(current_time_buffer_pass * shakeSpeed * 1.1) * sin(current_time_buffer_pass * shakeSpeed * 0.9) * shakeIntensity;
        ro_base.xy += vec2(shake_offset_x, shake_offset_y);
    }

    // Apply camera roll by rotating the ray
    float roll_angle = cameraRoll; // Use ISF input for camera roll
    mat2 roll_mat = mat2(cos(roll_angle), -sin(roll_angle), sin(roll_angle), cos(roll_angle));
    rd.xy = rd.xy * roll_mat;

    // --- Fractal Configuration (mins/maxs) with Morphing ---
    mins_global = mix(vec4(get_minsx(i0),get_minsy(i0),get_minsz(i0),get_minsw(i0)), vec4(get_minsx(i1),get_minsy(i1),get_minsz(i1),get_minsw(i1)), kt);
    maxs_global = mix(vec4(get_maxsx(i0),get_maxsy(i0),get_maxsz(i0),get_maxsw(i0)), vec4(get_maxsx(i1),get_maxsy(i1),get_maxsz(i1),get_maxsw(i1)), kt);

    // Apply fractalMorph: Let's slightly shift min/max values based on the morph param
    mins_global.xyz = mix(mins_global.xyz, mins_global.xyz * (1.0 - fractalMorph), fractalMorph * 0.5);
    maxs_global.xyz = mix(maxs_global.xyz, maxs_global.xyz * (1.0 + fractalMorph), fractalMorph * 0.5);
    mins_global.w = mix(mins_global.w, mins_global.w * (1.0 - fractalMorph * 0.7), fractalMorph);
    maxs_global.w = mix(maxs_global.w, maxs_global.w * (1.0 + fractalMorph * 0.7), fractalMorph);


    // --- Music light (BPM by iq) ---
    // Using TIME for audio sync
    float h = fract( .25 + .5 * TIME * BPM/60.0 );
    float f = (1.0-smoothstep( 0.0, 1.0, h )) * smoothstep( 4.5, 4.51, TIME); 
    float r =  exp(-4.0*f);
    
    // The original shader uses 't' from its internal `iTime` for kColor,
    // which is the current_time_buffer_pass here.
    kColor_global = mix(vec2(.25,1.),vec2(.01325,1.23), r*r*(.13+smoothstep(10.,12.,t_interp_base)));
 
    // --- Rendering ---    
    vec3 col = BACK_COLOR;
    vec2 res_trace = trace(ro_base, rd);
    float t = res_trace.x;

    if (t < MAX_DIST_RAYMARCHING) {
        vec3 pos = ro_base + t*rd;
        vec3 nor = calcNormal( pos, t);
        vec3 ref = reflect( rd, nor);
        vec3 lig = LIGHT_VEC;

        // Base color based on fractal properties (res_trace.y)
        vec3 base_fractal_color = .5 + .5*cos( 6.2831*res_trace.y + vec3(0,1,2) );
        vec3 psychedelic_color_applied = getSelectedPaletteColor(paletteSelection, fract(pos.z * colorPulseFactor + current_time_buffer_pass * colorPulseSpeed + colorPulseOffset), current_time_buffer_pass, fragCoord / RENDERSIZE.xy);
        base_fractal_color = mix(base_fractal_color, psychedelic_color_applied, 1.0); // Always apply custom palette

        // lighting          
        float occ = 1.0;
        #ifdef WITH_AO
        occ = calcAO4(pos, nor);
        #endif

        float sh = 1.0;
        #ifdef WITH_SHADOWS
        sh = .2+.8*shadow( pos, lig); //, 0.1, t );
        #endif

        float amb = .3;
        float dif = clamp( dot( nor, lig ), 0., 1.);
        float bac = clamp( dot( nor, normalize(vec3(-lig.x,0.,-lig.z))), 0., 1. )*clamp( 1.-pos.y,0.,1.);
        float dom = smoothstep( -.1, .1, ref.y );
        float fre = clamp(1.+dot(nor,rd),0.,1.);
        fre *= fre;
        float spe = pow(clamp( dot( ref, lig ), 0., 1. ), 16.);

        vec3 lin =  .3 +
             + 3.*sh*dif*vec3(1.,0.8,0.55)
             + 3.*spe*vec3(1.,0.9,0.7)*dif
             + (.3+.7*occ)*(.4*amb*vec3(0.4,0.6,1.) +
                          .5*sh*vec3(0.4,0.6,1.) +
                          .25*fre*vec3(1.,1.,1.));

        col = base_fractal_color * lin;

        // Environmental reflection (using a simplified procedural environment)
        vec3 reflected_env_color = getSelectedPaletteColor(paletteSelection + 4.0, fract(dot(ref, vec3(1.2, 3.4, 5.6)) + current_time_buffer_pass * 0.1), current_time_buffer_pass, fragCoord / RENDERSIZE.xy);
        col = mix(col, reflected_env_color, reflectiveness); // Blend reflection with base color

        // Shading.
        float atten = 1./(1. + t*.2 + t*.1); // + distlpsp*distlpsp*0.02
        col *= atten*col*occ;
        col = mix(col, BACK_COLOR, smoothstep(0.2, 1., t/MAX_DIST_RAYMARCHING));
    } else {
        // Background color
        col = BACK_COLOR;
        // Simple procedural background for non-hit rays, using existing palette logic
        vec3 background_psychedelic_color = getSelectedPaletteColor(paletteSelection + 2.0, fract(dot(rd, vec3(0.1, 0.2, 0.3)) + current_time_buffer_pass * 0.05), current_time_buffer_pass, fragCoord / RENDERSIZE.xy);
        col = mix(col, background_psychedelic_color, 0.5);
    }
    
    // Original returns vec4(sqrt(col),t) where t is depth
    return vec4(sqrt(clamp(col, 0.0, 1.0)), t);
}


// --- TEXT by Andre ---
float line(vec2 p, vec2 a, vec2 b) {
	vec2 pa = p - a, ba = b - a;
    return length(pa - ba * clamp(dot(pa, ba)/dot(ba,ba), 0., 1.));
}
float _u(vec2 uv,float w,float v) {
    return length(vec2(abs(length(vec2(uv.x,max(0.,-(.4-v)-uv.y) ))-w),max(0.,uv.y-.4)));
}
float _i(vec2 uv) {
    return length(vec2(uv.x,max(0.,abs(uv.y)-.4)));
}
float _l(vec2 uv) {
    uv.y -= .2;
    return length(vec2(uv.x,max(0.,abs(uv.y)-.6)));
}
float _o(vec2 uv) {
    return abs(length(vec2(uv.x,max(0.,abs(uv.y)-.15)))-.25);
}
float aa_char(vec2 uv) { 
    uv = -uv;
    float x = abs(length(vec2(max(0.,abs(uv.x)-.05),uv.y-.2))-.2);
    x = min(x,length(vec2(uv.x+.25,max(0.,abs(uv.y-.2)-.2))));
    return min(x,(uv.x<0.?(uv.y<0.?_o(uv):length(vec2(uv.x-.22734,uv.y+.254))):atan(uv.x,uv.y+0.15)>2.?_o(uv):length(vec2(uv.x-.22734,uv.y+.254))));
}
float ee_char(vec2 uv) { 
    float x = _o(uv);
    return min(uv.x<0. || uv.y > .05 || atan(uv.x,uv.y+0.15) > 2. ? x : length(vec2(uv.x-.22734,uv.y+.254)),
              length(vec2(max(0.,abs(uv.x)-.25),uv.y-.05)));
}
float ii_char(vec2 uv) { 
    return min(_i(uv),length(vec2(uv.x,uv.y-.7)));
}
float kk_char(vec2 uv) { 
    float x = line(uv,vec2(-.25,-.1), vec2(0.25,0.4));
    x = min(x,line(uv,vec2(-.15,.0), vec2(0.25,-0.4)));
    uv.x+=.25;
    return min(x,_l(uv));
}
float nn_char(vec2 uv) { 
    uv.y *= -1.;
    float x = _u(uv,.25,.25);
    uv.x+=.25;
    return min(x,_i(uv));
}

#define ch(l) if (nr++==ofs) x=min(x,l(uv_char));

void drawText(float time_param, vec2 frag_coord_param, inout vec4 color) {
    if (textEnabled > 0.5) { // Control text with ISF input
        float text_time_start = 12.6; // Original timing
        float text_time_end = 14.1;

        // Use a repeating time for the animation phase within the text segment
        float anim_duration = text_time_end - text_time_start;
        float current_text_anim_time = mod(time_param, anim_duration);
        
        float anim = smoothstep(0.,1.,smoothstep(1.0, 1.5, current_text_anim_time)); 

        vec2 uv_char = frag_coord_param - .5*RENDERSIZE.xy;
        uv_char += (.5 + mix(vec2(-.1,.4), vec2(.2,.3), anim)) * RENDERSIZE.xy;
        uv_char = (uv_char-.5*RENDERSIZE.xy) / RENDERSIZE.x * 22.0 * mix(1.,.5,anim);
        
        float ofs = floor(uv_char.x); 
        float x = 1.0; 
        float nr = 0.; // Changed to float to match `ofs` in `ch` macro for modulo operator change
        uv_char.x = mod(uv_char.x,1.)-.5;
        
        // KELENIAN
		ch(kk_char);ch(ee_char);ch(_l);ch(ee_char);ch(nn_char);ch(ii_char);ch(aa_char);ch(nn_char);
        
        float px = 17./RENDERSIZE.x, clr = smoothstep(.06-px,.06+px, x);
        color = mix(color, mix(vec4(0,0,0,.5), color, clr), smoothstep(0., 0.4, current_text_anim_time)*(1.-anim));
    }
}


// Post-processing functions
vec3 adjustBrightness(vec3 color, float val) {
    return color * val;
}

vec3 adjustSaturation(vec3 color, float val) {
    vec3 gray = vec3(dot(color, vec3(0.2126, 0.7152, 0.0722)));
    return mix(gray, color, val);
}

vec3 adjustContrast(vec3 color, float val) {
    return (color - 0.5) * val + 0.5;
}

// --- Main ISF Entry Point ---
void main()
{
    float current_time_total = animationSpeed * TIME;
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // First pass: Render the fractal scene (Buffer A equivalent)
    vec4 fractal_scene_output = renderFractalScene(fragCoord, current_time_total);
    vec3 final_color_rgb = fractal_scene_output.rgb;
    float depth_val = fractal_scene_output.a; // Depth is stored in alpha channel

    // Simplified Depth-of-Field (just a blur based on depth difference, no bokeh):
    if (dofEnabled > 0.5) {
        const int NB_DOF_FRAMES = 17;
        
        float t_dof = .1 * current_time_total;
        float kt_dof = smoothstep(0.,1.,fract(t_dof));
        // Use floor and mod for index
        int i0_dof = int(mod(floor(t_dof), float(NB_DOF_FRAMES)));
        int i1_dof = int(mod(float(i0_dof + 1), float(NB_DOF_FRAMES)));

        float fdist = mix(get_deph_arr(i0_dof), get_deph_arr(i1_dof), kt_dof);
        fdist += dofFocusOffset;

        float coc_val = max(0.0, abs(depth_val - fdist) * dofAperture);
        float blur_effect_mix = clamp(coc_val * 2.0, 0.0, 1.0);
        
        vec3 gray = vec3(dot(final_color_rgb, vec3(0.2126, 0.7152, 0.0722)));
        final_color_rgb = mix(final_color_rgb, gray * 0.5, blur_effect_mix); // Darken and desaturate
    }
    
    // Apply glitch effect as post-processing (color channel mixing)
    if (glitchIntensity > 0.001) {
        float g_time = current_time_total * glitchSpeed;
        float g_strength = glitchIntensity * (sin(g_time * glitchFrequency) * 0.5 + 0.5); 

        float r_val = final_color_rgb.r;
        float g_val = final_color_rgb.g;
        float b_val = final_color_rgb.b;

        final_color_rgb.r = mix(r_val, g_val, g_strength);
        final_color_rgb.g = mix(g_val, b_val, g_strength * 0.8);
        final_color_rgb.b = mix(b_val, r_val, g_strength * 1.2);
        
        float displacement_x = sin(g_time + uv.x * 10.0) * g_strength * 0.1;
        float displacement_y = cos(g_time * 0.7 + uv.y * 12.0) * g_strength * 0.1;
        final_color_rgb.r += displacement_x;
        final_color_rgb.g += displacement_y;
        final_color_rgb.b -= displacement_x; 
    }

    // Vigneting from original shader
    #define WITH_VIGNETING
    #ifdef WITH_VIGNETING
    final_color_rgb *= pow(16.*uv.x*uv.y*(1.-uv.x)*(1.-uv.y), .3); 
    #endif

    // Apply brightness, saturation, contrast post-processing
    final_color_rgb = adjustBrightness(final_color_rgb, brightness); 
    final_color_rgb = adjustSaturation(final_color_rgb, saturation); 
    final_color_rgb = adjustContrast(final_color_rgb, contrast); 

    // Add Text - Pass a modifiable vec4 variable
    vec4 output_color_with_alpha = vec4(final_color_rgb, 1.0);
    drawText(current_time_total, fragCoord, output_color_with_alpha); 

    gl_FragColor = output_color_with_alpha;

    // --- DIAGNOSTIC: TEMPORARY OVERRIDE FOR DEBUGGING ---
    // If the screen is black, uncomment the line below to visualize the raymarch 't' value.
    // A blue screen means rays are hitting something, but maybe too far.
    // A black screen means rays are mostly missing, or 't' is always MAX_DIST_RAYMARCHING.
    // gl_FragColor = vec4(vec3(fractal_scene_output.a / MAX_DIST_RAYMARCHING), 1.0); 
}