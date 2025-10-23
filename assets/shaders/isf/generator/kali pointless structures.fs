/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic",
        "Fractal",
        "Tunnel"
    ],
    "DESCRIPTION": "A complex fractal tunnel based on Kali's private shader. Enhanced with extensive psychedelic tunable parameters, including 6 color palettes and granular control over flashing glow effects.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Zoom Factor" },
        { "NAME": "CameraLeadDistance", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Cam Lead Dist" },
        { "NAME": "CameraLookAheadInfluence", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Cam Lookahead Inf" },
        { "NAME": "CameraRollX", "TYPE": "float", "DEFAULT": 45.0, "MIN": -90.0, "MAX": 90.0, "LABEL": "Cam Roll X (Deg)" },
        { "NAME": "CameraRollY", "TYPE": "float", "DEFAULT": -20.0, "MIN": -90.0, "MAX": 90.0, "LABEL": "Cam Roll Y (Deg)" },
        { "NAME": "KSetIterations", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "KSet Iterations" },
        { "NAME": "KSetFoldFactor", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "KSet Fold Factor" },
        { "NAME": "KSetZoomFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "LABEL": "KSet Zoom Factor" },
        { "NAME": "MainFractalIterations", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 15.0, "LABEL": "Main Fractal Iter" },
        { "NAME": "FractalScaleFactor", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.5, "MAX": 3.0, "LABEL": "Fractal Scale Factor" },
        { "NAME": "FractalOffsetX", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Offset X" },
        { "NAME": "FractalOffsetY", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Offset Y" },
        { "NAME": "FractalOffsetZ", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Offset Z" },
        { "NAME": "FractalRotationXY", "TYPE": "float", "DEFAULT": 90.0, "MIN": 0.0, "MAX": 180.0, "LABEL": "Fractal Rotation XY (Deg)" },
        { "NAME": "FractalBaseOffset", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Fractal Base Offset" },
        { "NAME": "TunnelCoreSize", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5, "LABEL": "Tunnel Core Size" },
        { "NAME": "TunnelPathWobble", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tunnel Path Wobble" },
        { "NAME": "BoxShapeScaleX", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Box Shape Scale X" },
        { "NAME": "BoxShapeScaleY", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Box Shape Scale Y" },
        { "NAME": "BoxShapeScaleZ", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Box Shape Scale Z" },
        { "NAME": "BoxShapeEdgeRoundness", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.5, "LABEL": "Box Shape Edge Roundness" },
        { "NAME": "BoxShapeRotationSpeed", "TYPE": "float", "DEFAULT": 90.0, "MIN": 0.0, "MAX": 360.0, "LABEL": "Box Shape Rot Speed (Deg)" },
        { "NAME": "MainGridScale", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Main Grid Scale" },
        { "NAME": "MarchSteps", "TYPE": "float", "DEFAULT": 130.0, "MIN": 50.0, "MAX": 300.0, "LABEL": "March Steps" },
        { "NAME": "MaxDistance", "TYPE": "float", "DEFAULT": 15.0, "MIN": 5.0, "MAX": 50.0, "LABEL": "Max March Dist" },
        { "NAME": "DetailThreshold", "TYPE": "float", "DEFAULT": 0.0001, "MIN": 0.00001, "MAX": 0.01, "LABEL": "Detail Threshold" },
        { "NAME": "VolumetricGlowDecay", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.001, "MAX": 0.1, "LABEL": "Volumetric Glow Decay" },
        { "NAME": "RandomStepInfluence", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Random Step Inf" },
        { "NAME": "KSetGlowStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "KSet Glow Strength" },
        { "NAME": "GlowColorBlendStrength", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.2, "LABEL": "Glow Color Blend" },
        { "NAME": "TunnelEdgeBlend", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Tunnel Edge Blend" },
        { "NAME": "FinalColorPower", "TYPE": "float", "DEFAULT": 1.3, "MIN": 0.5, "MAX": 3.0, "LABEL": "Final Color Power" },
        { "NAME": "FinalColorMultiplier", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0, "LABEL": "Final Color Multiplier" },
        { "NAME": "GlowAccentHue", "TYPE": "float", "DEFAULT": 0.13, "MIN": 0.0, "MAX": 1.0, "LABEL": "Moving Light Hue" },
        { "NAME": "GlowAccentSaturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Moving Light Sat" },
        { "NAME": "GlowAccentBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Moving Light Bright" },
        { "NAME": "GlowAccentStrength", "TYPE": "float", "DEFAULT": 0.012, "MIN": 0.0, "MAX": 0.1, "LABEL": "Moving Light Str" },
        { "NAME": "GlowAccentPower", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Moving Light Power" },
        { "NAME": "BandBrightness", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 50.0, "LABEL": "Band Brightness" },
        { "NAME": "BaseGlowBrightness", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 10.0, "LABEL": "Base Glow Brightness" },
        { "NAME": "BandFrequencyZ", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Band Freq Z" },
        { "NAME": "BandFrequencyTime", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Band Freq Time" },
        { "NAME": "BandFrequencyPZ", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.0, "MAX": 0.05, "LABEL": "Band Freq PZ" },
        { "NAME": "BandOffsetIteration", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "LABEL": "Band Offset Iter" },
        { "NAME": "BandThreshold", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.1, "LABEL": "Band Threshold" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "GridPatternBrightness", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0, "LABEL": "Grid Pattern Brightness" },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool", "3.0": "Vibrant Neon", "4.0": "Deep Space", "5.0": "Dreamy Pastel" } }
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

// Define MAX iteration counts as constants for loops
#define MAX_KSET_ITERATIONS_CONST 10    // Corresponds to KSetIterations MAX
#define MAX_MAIN_FRACTAL_ITERATIONS_CONST 15 // Corresponds to MainFractalIterations MAX
#define MAX_MARCH_STEPS_CONST 300       // Corresponds to MarchSteps MAX


float det_val = 0.0; 
float maxdist_val = 0.0; 
vec3 ldir=vec3(0.5,1.,1.); 
vec3 pa;
float gcol; // Global color modulator (for banding)
float t_time, it_iter, k_val; 


// --- Color Conversion Functions ---

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Custom hue function for multiple color palettes
vec3 getPaletteColor(float h_val, float palette_type, float time_factor) {
    vec3 color_base;

    // No time-based hue shift here, as main color pulse removed per request
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
        color_base = hsv2rgb(vec3(h_val, 0.9, 0.8)); // Static saturation/value for vibrant
    } else if (palette_type < 4.5) { // Palette 4: Deep Space (Darker blues/purples with bright accents)
        color_base = hsv2rgb(vec3(mod(h_val * 0.5 + 0.6, 1.0), 0.7, 0.3)); // Base deep color
        color_base = mix(color_base, hsv2rgb(vec3(mod(h_val * 1.5 + 0.1, 1.0), 0.8, 0.9)), 0.2); // Static vibrant accents
    } else { // Palette 5: Dreamy Pastel
        color_base = hsv2rgb(vec3(h_val, 0.3, 0.7)); // Static saturation/value for pastel
    }

    return color_base;
}

// --- Noise and Rotation Functions ---

float hash12(vec2 p)
{
	vec3 p3  = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

mat2 rot(float a) {
    a=radians(a); 
	float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

// --- Distance Field Primitives ---

float dot2( in vec3 v ) { return dot(v,v); }
float sdBoundingBox( vec3 p, vec3 b, float e)
{
        p = abs(p )-b;
  vec3 q = abs(p+e)-e;

  return sqrt(min(min(dot2(max(vec3(p.x,q.y,q.z),0.0)),
                      dot2(max(vec3(q.x,p.y,q.z),0.0))),
                      dot2(max(vec3(q.x,q.y,p.z),0.0)))) 
          +min(0.0,min(min( max(p.x,max(q.y,q.z)),
                            max(p.y,max(q.z,q.x))),
                            max(p.z,max(q.x,q.y))));
}

// --- Fractal Definitions ---

float kset(vec3 p) {
    p=abs(fract(p * KSetZoomFactor)-0.5); 
    for (int i=0; i < MAX_KSET_ITERATIONS_CONST; i++) {
        if (float(i) < KSetIterations) { 
            p=abs(p)/dot(p,p)-KSetFoldFactor; 
        } else {
            break;
        }
    }
    return length(p.xy);
}

float shape (vec3 p, float z) {
    p.xz*=rot(smoothstep(0.28,0.3,abs(0.5-fract(t_time * 0.1 + floor(z*4.0)*0.005))) * BoxShapeRotationSpeed);
    float d=sdBoundingBox(p,vec3(BoxShapeScaleX, BoxShapeScaleY, BoxShapeScaleZ), BoxShapeEdgeRoundness);
    return d;
}

vec3 path(float t_val) {
	return vec3(sin(t_val*0.5),cos(t_val)*0.5,t_val);
}

vec3 pathcam(float t_val) {
	vec3 p=path(t_val);
    p.y+=smoothstep(0.0,0.5,abs(0.5-fract(t_val*0.05 * TunnelPathWobble)))*3.0 * CameraLookAheadInfluence;
    return p;
}

float de(vec3 pos) {
	float tu=length(pos.xy-pathcam(pos.z).xy)-TunnelCoreSize;
    pos.y+=-1.0;
    pos.x-=0.4;
	pos.xy-=path(pos.z).xy;
    float z=pos.z;
    pos=abs(MainGridScale - mod(pos, MainGridScale * 2.0)) - MainGridScale;
    pa=pos; 
	float sc=1.4, d=1000., der=1.;
    vec3 p=pos;
    float o=1000.;
    for (int i=0; i < MAX_MAIN_FRACTAL_ITERATIONS_CONST; i++) {
        if (float(i) < MainFractalIterations) { 
            p.xy*=rot(FractalRotationXY); 
            p.xz=abs(p.xz);
            p.y+=FractalBaseOffset; 
            sc=FractalScaleFactor/clamp(dot(p,p),0.1,1.0); 
            p=p*sc-vec3(FractalOffsetX,FractalOffsetY,FractalOffsetZ); 
            p.y-=FractalBaseOffset; 
            der*=sc;
            float shp=shape(p,z)/der;
            if (shp<d && i>1) {
                d=shp;
                it_iter=float(i); 
            }
            o=min(o,length(p));
        } else {
            break;
        }
    }
    d=min(d,length(p.xy)/der-0.005);

    gcol = step(fract(pos.z * BandFrequencyZ + TIME * BandFrequencyTime + p.z * BandFrequencyPZ + it_iter * BandOffsetIteration), BandThreshold) * BandBrightness + BaseGlowBrightness;

    d=max(d,-tu);
    return d*0.7;
}

// --- Raymarching ---

vec3 march(vec3 from, vec3 dir) {
	vec3 p, col=vec3(0.);
    float totdist=0.,d;
    float g=0.,gg=0.;
    for(int i=0; i < MAX_MARCH_STEPS_CONST; i++) {
        if (float(i) < MarchSteps) { 
            p=from+totdist*dir;
            d=de(p);
            float current_det = DetailThreshold * (1.0 + totdist * 0.03); 
            if (d < current_det || totdist > MaxDistance) break; 
            totdist+=d*(1.0+hash12(dir.xy*1000.0)*RandomStepInfluence); 
            g+=exp(-VolumetricGlowDecay * totdist)*kset(pa)*gcol * KSetGlowStrength; 
        } else {
            break;
        }
    }
    if (d<0.1) {
        col=vec3(1.0) * GlowColorBlendStrength; // Base white glow before palette is applied
    }
    col=mix(vec3(0.0),col,exp(-TunnelEdgeBlend*totdist)); 
    col=pow(col,vec3(FinalColorPower))*FinalColorMultiplier; 

    // Apply controllable flashing accent color (moving lights)
    vec3 accent_color_hsv = vec3(GlowAccentHue, GlowAccentSaturation, GlowAccentBrightness);
    vec3 accent_color_rgb = hsv2rgb(accent_color_hsv);

    return col + pow(g * GlowAccentStrength, GlowAccentPower) * accent_color_rgb;
}

// --- Camera Setup ---

mat3 lookat(vec3 dir_val, vec3 up_val) {
	dir_val=normalize(dir_val); vec3 r=normalize(cross(dir_val,up_val));
    return mat3(r,cross(dir_val,r),dir_val);
}

// --- Main Shader Function ---

void main() {

	t_time=TIME*GlobalSpeed; 
    vec2 uv = (gl_FragCoord.xy-RENDERSIZE.xy*0.5)/RENDERSIZE.y;
    vec3 dir = normalize(vec3(uv,ZoomFactor)); 

    vec3 from = pathcam(t_time);
    vec3 to = pathcam(t_time + CameraLeadDistance); 
    vec3 adv = normalize(to-from);
    
    dir*=lookat(adv,vec3(0.0,1.0,0.0));
    dir.xy*=rot(CameraRollX); 
    dir.yz*=rot(CameraRollY); 

    det_val = DetailThreshold; 
    maxdist_val = MaxDistance; 

    vec3 col = march(from, dir);
    
    // Apply the chosen color palette to the entire scene after marching
    // The 'gcol' provides a good value to drive the hue variation for the palette
    col = getPaletteColor(gcol * 0.1, PaletteChoice, t_time) * col;

    col = col * max(mod(gl_FragCoord.x,3.0),mod(gl_FragCoord.y,3.0)) * GridPatternBrightness;
    
    // Final Global Brightness control
    gl_FragColor = vec4(col * GlobalBrightness,1.0);
}