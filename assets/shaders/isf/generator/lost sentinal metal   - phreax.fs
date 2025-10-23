/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Tunnel",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Distortion",
        "Effect"
    ],
    "DESCRIPTION": "An intricate fractal tunnel with an animated 'creature', featuring tunable geometry, psychedelic color palettes, and post-processing controls. Enhanced with DMT-trip-like shimmering colors, shake, glitch, and tunnel shape morphing. Original shader by phreax, converted and enhanced for ISF.",
    "CREDIT": "Original Shader by phreax (https://www.shadertoy.com/view/WfyXDK), converted and enhanced for ISF.",
    "IMPORTED": {
        "TEX0": {
            "NAME": "TEX0",
            "PATH": [
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e93716d3ed0e86410784b919_1.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_2.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_3.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_4.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_5.jpg"
            ],
            "TYPE": "cube"
        }
    },
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
            "NAME": "octoMorphAlphaSpeed",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the 'octopus' fractal morphology animation."
        },
        {
            "NAME": "octoScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall scale of the 'octopus' fractal geometry."
        },
        {
            "NAME": "octoGlowIntensity",
            "TYPE": "float",
            "DEFAULT": 0.08,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.001,
            "DESCRIPTION": "Adjusts the glow intensity of the octomorph."
        },
        {
            "NAME": "octoLightMultiplier",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "Multiplies the overall light contribution to the octomorph."
        },
        {
            "NAME": "octoZSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed and direction of octomorph's movement along Z-axis."
        },
        {
            "NAME": "octoXYOscillationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of octomorph's side-to-side and up-down oscillation."
        },
        {
            "NAME": "tunnelBendAmplitude",
            "TYPE": "vec2",
            "DEFAULT": [4.0, 3.0],
            "MIN": [0.0, 0.0],
            "MAX": [10.0, 10.0],
            "STEP": [0.01, 0.01],
            "DESCRIPTION": "Amplitude of X and Y bending applied to the tunnel."
        },
        {
            "NAME": "tunnelRadius",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 1.0,
            "MAX": 20.0,
            "STEP": 0.1,
            "DESCRIPTION": "Radius of the main tunnel structure."
        },
        {
            "NAME": "columnRepeatX",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 1.0,
            "MAX": 10.0,
            "STEP": 0.1,
            "DESCRIPTION": "X-axis repetition frequency for patterns on tunnel walls."
        },
        {
            "NAME": "columnTwistSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of rotation/twist for column patterns."
        },
        {
            "NAME": "paletteSelection",
            "TYPE": "int",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 6,
            "STEP": 1,
            "DESCRIPTION": "Selects one of 7 predefined color palettes for the fractal."
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
            "DEFAULT": 0.98,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the mix of cubemap reflection for the tunnel material (0.0 = no reflection, 1.0 = full reflection)."
        },
        {
            "NAME": "octoReflectivity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls how much cubemap reflection affects the octomorph (0.0 = no reflection, 1.0 = full reflection). If no cubemap, set to 0.0."
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
            "NAME": "tunnelMorphIntensity",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the tunnel's shape morphing."
        },
        {
            "NAME": "tunnelMorphSpeed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the tunnel's shape morphing animation."
        },
        {
            "NAME": "tunnelMorphPattern",
            "TYPE": "int",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 3,
            "STEP": 1,
            "DESCRIPTION": "Selects different tunnel morphing patterns (0:None, 1:Twist/Ripple, 2:Pinch/Expand, 3:Complex Wave)."
        }
    ]
}
*/

#extension GL_OES_standard_derivatives : enable
// Required for textureCube on some GLSL ES implementations
#extension GL_NV_shadow_samplers_cube : enable
precision highp float;

// ISF standard uniforms (TIME, RENDERSIZE) are automatically declared by the ISF host.

// ONLY THESE ARE EXPLICITLY DECLARED AS PER YOUR DIRECT INSTRUCTION.
// ALL OTHER UNIFORMS ARE EXPECTED TO BE HANDLED AUTOMATICALLY BY ISF.
uniform samplerCube TEX0; 
uniform vec2 tunnelBendAmplitude; 
uniform int paletteSelection; 
uniform int tunnelMorphPattern; 

#define SIN(x)  (.5+.5*sin(x)) 
#define PI 3.14159265359
#define PHI 1.618033988749895


// from shadertoy user tdhopper for Icosahedron SDF
// Renamed GDF variables to avoid potential conflicts with local variables
#define GDFVector3 normalize(vec3(1, 1, 1 ))
#define GDFVector4 normalize(vec3(-1, 1, 1))
#define GDFVector5 normalize(vec3(1, -1, 1))
#define GDFVector6 normalize(vec3(1, 1, -1))

#define GDFVector7 normalize(vec3(0, 1, PHI+1.))
#define GDFVector8 normalize(vec3(0, -1, PHI+1.))
#define GDFVector9 normalize(vec3(PHI+1., 0, 1))
#define GDFVector10 normalize(vec3(-PHI-1., 0, 1))
#define GDFVector11 normalize(vec3(1, PHI+1., 0))
#define GDFVector12 normalize(vec3(-1, PHI+1., 0))

#define GDFVector13 normalize(vec3(0, PHI, 1))
#define GDFVector14 normalize(vec3(0, -PHI, 1))
#define GDFVector15 normalize(vec3(1, 0, PHI))
#define GDFVector16 normalize(vec3(-1, 0, PHI))
#define GDFVector17 normalize(vec3(PHI, 1, 0))
#define GDFVector18 normalize(vec3(-PHI, 1, 0))

#define fGDFBegin float d_gdf = 0.;
#define fGDF(v) d_gdf = max(d_gdf, abs(dot(p_gdf, v)));
#define fGDFEnd return d_gdf - r_gdf;


float fIcosahedron(vec3 p_gdf, float r_gdf) {
    fGDFBegin
    fGDF(GDFVector3) fGDF(GDFVector4) fGDF(GDFVector5) fGDF(GDFVector6)
    fGDF(GDFVector7) fGDF(GDFVector8) fGDF(GDFVector9) fGDF(GDFVector10)
    fGDF(GDFVector11) fGDF(GDFVector12)
    fGDFEnd
}

// Function to select a palette based on index and a time/spatial factor 't_mix' for interpolation
vec3 getSelectedPaletteColor(int paletteIdx, float t_mix) {
    // Apply colorPulseExponent to t_mix
    t_mix = pow(t_mix, colorPulseExponent);

    // Apply psychedelic shimmer based on current_time and t_mix
    float shimmer_val = sin(TIME * 2.5 + t_mix * 5.0) * 0.5 + 0.5; // Base shimmer wave
    t_mix = mix(t_mix, shimmer_val, psychedelicShimmerIntensity); // Blend original t_mix with shimmer
    t_mix = clamp(t_mix, 0.0, 1.0); // Ensure t_mix stays in valid range

    vec3 colA, colB, colC;
    switch(paletteIdx) {
        case 0: // Original-ish Palette (Greys, Blues, Purples, Greens, Pinks)
            colA = vec3(0.510, 0.510, 0.510);
            colB = vec3(0.102, 0.675, 0.918);
            colC = vec3(0.427, 0.220, 1.000);
            return mix(mix(colA, colB, t_mix), colC, t_mix*t_mix);
        case 1: // Fiery Sunset
            colA = vec3(0.9, 0.1, 0.1); // Red
            colB = vec3(0.9, 0.5, 0.1); // Orange
            colC = vec3(0.9, 0.9, 0.1); // Yellow
            return mix(mix(colA, colB, t_mix), colC, t_mix);
        case 2: // Deep Space
            colA = vec3(0.05, 0.05, 0.1); // Deep Blue
            colB = vec3(0.2, 0.0, 0.4); // Dark Purple
            colC = vec3(0.0, 0.5, 0.5); // Cyan
            return mix(mix(colA, colB, t_mix), colC, t_mix*t_mix*t_mix);
        case 3: // Neon Cyber
            colA = vec3(0.0, 1.0, 0.0); // Neon Green
            colB = vec3(0.0, 0.8, 1.0); // Bright Cyan
            colC = vec3(1.0, 0.0, 1.0); // Magenta
            return mix(mix(colA, colB, t_mix), colC, t_mix);
        case 4: // Psychedelic Dream
            colA = vec3(0.8, 0.2, 0.9); // Pink-Purple
            colB = vec3(0.2, 0.9, 0.8); // Green-Cyan
            colC = vec3(0.9, 0.8, 0.2); // Yellow-Orange
            return mix(mix(colA, colB, t_mix), colC, sin(t_mix * PI) * 0.5 + 0.5); // Smooth blend
        case 5: // Aquatic Myst
            colA = vec3(0.0, 0.3, 0.5); // Dark Blue
            colB = vec3(0.0, 0.7, 0.9); // Light Blue
            colC = vec3(0.2, 0.9, 0.7); // Sea Green
            return mix(mix(colA, colB, t_mix), colC, t_mix);
        case 6: // Lava Flow
            colA = vec3(0.1, 0.0, 0.0); // Dark Red
            colB = vec3(0.8, 0.1, 0.0); // Orange-Red
            colC = vec3(1.0, 0.5, 0.0); // Bright Orange
            return mix(mix(colA, colB, t_mix), colC, t_mix*t_mix);
        default: return vec3(1.0); // White fallback
    }
}

mat2 rot(float a) {
	return mat2(cos(a), sin(a), -sin(a), cos(a));
}

// iq's impulse function
float impulse2( float x, float k) {
    float h = k*x;
    return h*exp(1.0-h);
}

float impulse( float x, float k, float e) {
    float h = k*pow(x, e);
    return h*exp(1.0-h);
}

// repetitive, continues pulse function for animation
float continuesPulse(float x, float k, float e, float period) {
	return impulse(mod(x, period), k, e);
}

// repetitive, continues pulse function for animation
float continuesPulse2(float x, float k, float period) {
	return impulse2(mod(x, period), k);
}

// remap [0,1] -> [a, b])
float remap(float x, float a, float b) {
    return a*x+b;
}


float cyl(vec2 p, float r) {
	return length(p) - r;
}

float sph(vec3 p, float r) {
	return length(p) - r;
}

float cylcap( vec3 p, float r, float h ) {
  vec2 d = abs(vec2(length(p.xz),p.y)) - vec2(h,r);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float ring(vec3 p, float h, float rout, float rin) {
    return max(cylcap(p, h, rout), -cylcap(p, 2.*h, rin));
}

float box(vec3 p, vec3 r) {
    vec3 d = abs(p) - r;
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}

vec2 repeat_vec2(vec2 p, vec2 s) { 
   	return (fract(p/s-.5)-.5)*s;
}

float repeat_float(float p, float s) { 
   	return (fract(p/s-.5)-.5)*s;
}

vec3 tunnel_offset_func(vec3 p, float current_time_param) {
	vec3 off = vec3(0);
    // Original bending
    off.x += sin(p.z*0.1)*tunnelBendAmplitude.x;
    off.y += sin(p.z*0.12)*tunnelBendAmplitude.y;

    // Add morphing based on new uniforms
    float morph_strength = tunnelMorphIntensity;
    if (morph_strength > 0.001) {
        float morph_time = current_time_param * tunnelMorphSpeed;
        
        if (tunnelMorphPattern == 1) { // Twist/Ripple
            off.x += sin(morph_time + p.z * 0.7) * morph_strength;
            off.y += cos(morph_time * 0.8 + p.z * 0.6) * morph_strength * 0.7;
        } else if (tunnelMorphPattern == 2) { // Pinch/Expand
            float pinch_factor = sin(morph_time * 1.5 + p.z * 0.3) * 0.5 + 0.5;
            off.x += (pinch_factor - 0.5) * 2.0 * morph_strength;
            off.y += (pinch_factor - 0.5) * 2.0 * morph_strength;
        } else if (tunnelMorphPattern == 3) { // Complex Wave
            off.x += sin(morph_time * 1.1 + p.z * 1.2) * cos(morph_time * 0.7 + p.z * 0.8) * morph_strength;
            off.y += cos(morph_time * 0.9 + p.z * 1.0) * sin(morph_time * 1.3 + p.z * 1.1) * morph_strength;
        }
    }
    return off;
}

float sdOctopus(vec3 p, float current_time_param) { // Pass current_time_param
    float s_scaled = 0.4 * octoScale; // Uses octoScale
    vec3 q = p;
    
    q.z = abs(q.z);
    q.y = -abs(q.y);
    q.yz *= rot(PI*0.25);
    q.z = abs(q.z);
    q.y = -abs(q.y);
    q.yz *= rot(PI*0.125);
    q.z = abs(q.z);
   
    q.xz *= rot(-PI*0.25);

    q.xz -= 0.5;
 
    int maxIter = 20;
    float d = 1e6;
    // Use current_time_param instead of tt_global
    float alpha = remap(continuesPulse(current_time_param, octoMorphAlphaSpeed, 4.0, 5.0), -0.45, 0.15); // Uses octoMorphAlphaSpeed

    for(int i=1; i < maxIter; i++) { 
        q.xz *= rot(-alpha);
        q.z-=10./float(maxIter);
        s_scaled -= 0.8/float(maxIter); 
        float b = box(q, vec3(s_scaled))-.01;
        d = min(d, b);              
    }
    
    float head = fIcosahedron(p, .7 * octoScale); // Uses octoScale
    
    d = min(d, head);
  
    return d;
}

float repeat2(inout float p, float size) {
  float c = floor((p + size*0.5)/size);
  p = mod(p + size*0.5,size) - size*0.5;
  return c;
}


void cam(inout vec3 p_cam, float current_time_param) { // Pass current_time_param
    p_cam.z += current_time_param*5.;
    p_cam -= tunnel_offset_func(p_cam, current_time_param);
}

// map function now returns a vec4: (distance, material_id, cell_id, glow_contribution)
vec4 map(vec3 p, vec3 ray_origin_for_fade, float current_time_param) {

    vec3 np = p; // Original position for fade calc

    float tunr = tunnelRadius; // Uses tunnelRadius
    float vrep = 10.; 
    
    p += tunnel_offset_func(p, current_time_param);
   
    float cell_id_local; // Local variable for cell ID
    vec3 vvp = p;
    vvp.z += vrep/2.;
    cell_id_local = 1.+(repeat2(vvp.z, vrep)/vrep)*10.;
    
    float octo_dist = 0.;
    {
        vec3 q = p;
        // Use current_time_param and new octoZSpeed
        float pulse = continuesPulse(current_time_param, 0.7, 3.0, 5.0);
        float pulse2 = continuesPulse2(current_time_param-1., 3., 5.0);
  
        q.z += 10.;
        
        q.z -= remap(pulse, 1.5, 10.0);
        // Integrate octoZSpeed for Z-axis movement
        q.z -= octoZSpeed * current_time_param + current_time_param *remap(pulse2, 0., 3.5);
                
        q.xz *= rot(-PI*0.5 * columnTwistSpeed); // Uses columnTwistSpeed
        vec3 o1 = q;
        // Integrate octoXYOscillationSpeed for XY oscillation
        o1.xy += vec2(-4.5*sin(current_time_param * octoXYOscillationSpeed), 3.3*cos(.4*current_time_param * octoXYOscillationSpeed));
        o1.xz *= rot(.4*sin(current_time_param*.4));
        float s_octo = 1.8 * octoScale; // Uses octoScale
        octo_dist = 1./s_octo*sdOctopus(o1/s_octo, current_time_param); // Pass current_time_param
    }

    vec3 bp = p;
    vec3 vp = p;
    vec3 cp = p;
    
	
    bp.x = atan(p.y, p.x)*25./3.1415;
    bp.y = length(p.xy)-tunr;

    
    bp.xz = repeat_vec2(bp.xz, vec2(columnRepeatX)); // Uses columnRepeatX
    bp.xz = abs(bp.xz) - 1.; 
    bp.x -= bp.z*.4;
     
    for(float i = 0.; i < 3.; i += 1.) {
        // Use current_time_param and cell_id_local
        bp.xz *= rot(current_time_param*columnTwistSpeed+cell_id_local); // Uses columnTwistSpeed
        // Use current_time_param
        bp.xz = abs(bp.xz) - 1. - .1*SIN(i*current_time_param*.3);
        bp.x += .1;
    }
      
    float tunnel_dist = .9*box(bp, vec3(.8));

    // Glow calculation (local to map, returned as part of vec4)
    float current_glow = 0.0;
    cp.xy *= rot(.08*p.z);
    cp.x = abs(cp.x) - 3.;
    // Use current_time_param
    cp.x += sin(0.25*cp.y+sin(current_time_param))*2.; 
    cp.z = repeat_float(cp.z, 5.); 
    
    float vid = repeat2(vp.z, vrep);
    vp.yz *= rot(PI*.5);
    float veil = ring(vp, max(0., .01), tunr, tunr-.1);
    
    // Use current_time_param
    current_glow += SIN(vid*2.+2.*current_time_param); 
    current_glow += .018/(.1+pow(abs(veil), 8.));
    
    float final_dist = min(tunnel_dist, octo_dist);
    float material_id_local = tunnel_dist < octo_dist ? 0. : 1.+cell_id_local;

    return vec4(final_dist, material_id_local, cell_id_local, current_glow);
}


float calcAO(vec3 p, vec3 n, vec3 ray_origin_for_fade, float current_time_param) // Pass current_time_param
{
	float sca = 2.0, occ = 0.0;
    for( int i=0; i<5; i++ ){
    
        float hr = 0.01 + float(i)*0.5/4.0;        
        vec4 map_result = map(n * hr + p, ray_origin_for_fade, current_time_param); // Pass ray_origin_for_fade and current_time_param
        float dd = map_result.x; // Only need distance for AO
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - occ, 0.0, 1.0 );    
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

// Main rendering function (now encapsulated for post-processing)
vec3 renderScene(vec2 uv, float current_time) {
    vec3 ls = vec3(0, 0, -20.0); // light source
	vec3 col = vec3(0);
    
    // Initialize ray origin based on new controls
    vec3 ray_origin = vec3(cameraPanX, cameraPanY, -20.0 * cameraZoom); // Uses cameraPanX, cameraPanY, cameraZoom
    
    // Apply camera shake to ray origin
    if (shakeIntensity > 0.001) {
        float shake_offset_x = sin(current_time * shakeSpeed * 1.3) * cos(current_time * shakeSpeed * 0.7) * shakeIntensity;
        float shake_offset_y = cos(current_time * shakeSpeed * 1.1) * sin(current_time * shakeSpeed * 0.9) * shakeIntensity;
        ray_origin.xy += vec2(shake_offset_x, shake_offset_y);
    }

    // Apply camera roll to the ray direction
    mat2 camera_roll_rot = rot(cameraRoll); // Uses cameraRoll
    vec3 rd_initial = normalize(vec3(-uv, .7));
    rd_initial.xy = camera_roll_rot * rd_initial.xy;
    vec3 rd = rd_initial; 
    
    vec3 gcol = vec3(0.467,0.706,0.933); // A base blue-ish color (from original)

    // Adjust light source position with camera movement for consistent lighting
    // Note: 'ray_origin' is an 'inout' parameter in cam, so it gets modified directly.
    vec3 light_source_pos = ls; // Keep original LS and move a copy
    cam(ray_origin, current_time); // Pass current_time
    cam(light_source_pos, current_time); // Pass current_time

    vec3 p = ray_origin;
    float total_glow_accumulator = 0.0; // Use descriptive name, initialized locally
    
    vec4 map_result_hit; // To store properties of the hit surface
    
    float i, t = .1;
    for(i=0.; i<150.; i++) {
        vec4 current_map_result = map(p, ray_origin, current_time); // Call map, get all properties
        float d = current_map_result.x;
        
        // Accumulate glow during raymarch
        // Only accumulate glow if it's the octomorph material and scale by octoGlowIntensity
        if (current_map_result.y >= 1.0) { // Assuming material ID 1+ is octomorph
            total_glow_accumulator += current_map_result.w * octoGlowIntensity;
        }


        if(d < 0.001 || d > 20.) {
            map_result_hit = current_map_result; // Store the last map result if hit or went too far
            break;
        }
        
        p += d*rd;
        t += d;
        map_result_hit = current_map_result; // Store the latest properties
    }
    
    if(map_result_hit.x < 0.001) { // If a surface was hit
        float l_mat = map_result_hit.y;
        float l_cid = map_result_hit.z;

        vec2 e = vec2(0.0035, -0.0035);
        
        // Calculate palette color with pulse and shimmer
        vec3 al = getSelectedPaletteColor(paletteSelection, fract(l_cid * colorPulseFactor + current_time * colorPulseSpeed + colorPulseOffset)); 

        // Normal calculation: map() now correctly takes all necessary parameters
        vec3 n = normalize( e.xyy*map(p+e.xyy, ray_origin, current_time).x + 
                            e.yyx*map(p+e.yyx, ray_origin, current_time).x +
                            e.yxy*map(p+e.yxy, ray_origin, current_time).x + 
                            e.xxx*map(p+e.xxx, ray_origin, current_time).x);
        
        vec3 l = normalize(light_source_pos-p);
        float dif = max(dot(n, l), .0);
        float spe = pow(max(dot(reflect(-rd, n), -l), .0), 40.);
        // map() call for SSS now correctly takes parameters
        float sss = smoothstep(0., 1., map(p+l*.4, ray_origin, current_time).x)/.4;
        float ao = calcAO(p, n, ray_origin, current_time); // Pass ray_origin and current_time
        rd = reflect(rd, n);
        
        vec3 refl = textureCube(TEX0,rd).rgb; // Uses TEX0
        
        if(l_mat < 1.) { // Material ID 0 (tunnel)
            refl = pow(refl, vec3(1.0/1.5)); 
            // Control reflection intensity with 'reflectiveness'
            col += mix(gcol, vec3(1), .5)*refl.rgb * reflectiveness; 
            col += pow(i/100., 1.2)*.4*gcol; 
            col *= exp(-t*t*0.0002); 
            col = mix(col, col*(1.0), 1.); 
            col += al * (0.1 + dif * 0.5 + spe * 0.2); // Add direct light/color to tunnel
        } else { // Material ID 1+ (octopus creature)
            // Control octomorph reflection with 'octoReflectivity'
            vec3 octo_reflection_color = mix(al, refl, octoReflectivity);
            col +=  .2*spe + octoLightMultiplier * al * octo_reflection_color * (.3 + dif + 1.5*sss); // Apply octoLightMultiplier
            col = mix(col, col*(1.0-ao), 1.);
        }
    }
    
    col += total_glow_accumulator * gcol; // Apply total accumulated glow (octoGlowIntensity already applied in loop)
           
    col = pow(col, vec3(2.2)); 
    col = tanh(col*1.4); // Tone mapping / contrast

    // Apply post-processing controls
    col = adjustBrightness(col, brightness); 
    col = adjustSaturation(col, saturation); 
    col = adjustContrast(col, contrast); 
    
    return col;
}


void main() {
    float current_time = animationSpeed * TIME; 
    vec2 uv = (gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y; 

    vec3 final_color = renderScene(uv, current_time);

    // Apply glitch effect as post-processing (color channel mixing)
    if (glitchIntensity > 0.001) {
        float g_time = current_time * glitchSpeed;
        float g_strength = glitchIntensity * (sin(g_time * glitchFrequency) * 0.5 + 0.5); // Oscillating strength

        // Simple color channel mixing/shifting
        float r_val = final_color.r;
        float g_val = final_color.g;
        float b_val = final_color.b;

        // Shuffle channels based on glitch_strength
        final_color.r = mix(r_val, g_val, g_strength);
        final_color.g = mix(g_val, b_val, g_strength * 0.8);
        final_color.b = mix(b_val, r_val, g_strength * 1.2);
        
        // Add subtle pixel displacement (using uv for some pseudo-randomness)
        float displacement_x = sin(g_time + uv.x * 10.0) * g_strength * 0.1;
        float displacement_y = cos(g_time * 0.7 + uv.y * 12.0) * g_strength * 0.1;
        final_color.r += displacement_x;
        final_color.g += displacement_y;
        final_color.b -= displacement_x; // Counter-displace blue for effect
    }


    gl_FragColor = vec4(final_color,1.0);
}