/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Abstract",
        "Psychedelic"
    ],
    "DESCRIPTION": "Twisted tunnel with metallic and glow materials, now with comprehensive tunable parameters for animation, camera, lighting, fractal details, and post-processing.",
    "IMPORTED": {
        "TEXTURE_0": {
            "NAME": "TEXTURE_0",
            "PATH": [
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_1.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_2.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_3.jpg",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_4.png",
                "488bd40303a2e2b9a71987e48c66ef41f5e937174bf316d3ed0e86410784b919_5.png"
            ],
            "TYPE": "cube"
        }
    },
    "INPUTS": [
        { "NAME": "AnimationSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 0.5, "LABEL": "Animation Speed" },
        { "NAME": "TwistSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.0, "LABEL": "Tunnel Twist Speed" },
        { "NAME": "PathAnimationSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 5.0, "LABEL": "Path Animation Speed" },
        { "NAME": "RotationSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Rotation Speed" },

        { "NAME": "CameraZOffset", "TYPE": "float", "MIN": -50.0, "MAX": 0.0, "DEFAULT": -8.0, "LABEL": "Camera Z Offset" },
        { "NAME": "CameraYOffset", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.0, "LABEL": "Camera Y Offset" },
        { "NAME": "CameraFocalLength", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Camera Focal Length" },
        { "NAME": "CameraPitchSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "LABEL": "Camera Pitch Speed" },

        { "NAME": "Light1PosX", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": 4.0, "LABEL": "Light 1 Pos X" },
        { "NAME": "Light1PosY", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": 3.0, "LABEL": "Light 1 Pos Y" },
        { "NAME": "Light1PosZ", "TYPE": "float", "MIN": -20.0, "MAX": 0.0, "DEFAULT": -10.0, "LABEL": "Light 1 Pos Z" },
        { "NAME": "Light2PosX", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": -4.0, "LABEL": "Light 2 Pos X" },
        { "NAME": "Light2PosY", "TYPE": "float", "MIN": -10.0, "MAX": 10.0, "DEFAULT": -3.0, "LABEL": "Light 2 Pos Y" },
        { "NAME": "Light2PosZ", "TYPE": "float", "MIN": -20.0, "MAX": 0.0, "DEFAULT": -10.0, "LABEL": "Light 2 Pos Z" },
        { "NAME": "LightColor1", "TYPE": "color", "DEFAULT": [0.894,0.843,0.957,1.0], "LABEL": "Light Color 1" },
        { "NAME": "LightColor2", "TYPE": "color", "DEFAULT": [0.314,0.984,0.984,1.0], "LABEL": "Light Color 2" },
        { "NAME": "SpecularPower", "TYPE": "float", "MIN": 1.0, "MAX": 100.0, "DEFAULT": 40.0, "LABEL": "Specular Power" },

        { "NAME": "RaymarchInitialSteps", "TYPE": "float", "MIN": 50.0, "MAX": 500.0, "DEFAULT": 250.0, "LABEL": "Raymarch Init Steps" },
        { "NAME": "RaymarchReflectSteps", "TYPE": "float", "MIN": 10.0, "MAX": 200.0, "DEFAULT": 50.0, "LABEL": "Raymarch Reflect Steps" },
        { "NAME": "MaxRaymarchDistance", "TYPE": "float", "MIN": 10.0, "MAX": 200.0, "DEFAULT": 90.0, "LABEL": "Max Raymarch Dist" },
        { "NAME": "HitThreshold", "TYPE": "float", "MIN": 0.0001, "MAX": 0.01, "DEFAULT": 0.001, "LABEL": "Hit Threshold" },

        { "NAME": "InnerTwistCount", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 4.0, "LABEL": "Inner Twist Count" },
        { "NAME": "OuterTwistCount", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 6.0, "LABEL": "Outer Twist Count" },
        { "NAME": "CylinderHeight", "TYPE": "float", "MIN": 0.01, "MAX": 0.5, "DEFAULT": 0.04, "LABEL": "Cylinder Height" },
        { "NAME": "CylinderRadiusMixStart", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.02, "LABEL": "Cylinder Radius Mix Start" },
        { "NAME": "CylinderRadiusMixEnd", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.08, "LABEL": "Cylinder Radius Mix End" },
        { "NAME": "CylinderRadiusSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 4.0, "LABEL": "Cylinder Radius Speed" },
        { "NAME": "MainTwistFactor", "TYPE": "float", "MIN": 0.0, "MAX": 50.0, "DEFAULT": 20.0, "LABEL": "Main Twist Factor" },
        { "NAME": "SecondaryTwistFactor", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Secondary Twist Factor" },
        { "NAME": "RotationAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Rotation Amplitude" },

        { "NAME": "BackgroundColor1", "TYPE": "color", "DEFAULT": [0.106,0.255,0.275,1.0], "LABEL": "Background Color 1" },
        { "NAME": "BackgroundColor2", "TYPE": "color", "DEFAULT": [0.165,0.051,0.286,1.0], "LABEL": "Background Color 2" },
        { "NAME": "BackgroundMixFactor", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Background Mix Factor" },
        { "NAME": "BackgroundBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.1, "LABEL": "Background Brightness" },
        { "NAME": "GlowIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "LABEL": "Glow Intensity" },
        { "NAME": "GlowFadeFactor", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.09, "LABEL": "Glow Fade Factor" },
        { "NAME": "FlashPaletteIndex", "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 5.0, "LABEL": "Flash Palette Index" },
        { "NAME": "FlashTimeScale", "TYPE": "float", "MIN": 0.0, "MAX": 50.0, "DEFAULT": 0.5, "LABEL": "Flash Time Scale" },
        { "NAME": "FlashSpatialScale", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 10.0, "LABEL": "Flash Spatial Scale" },
        { "NAME": "FlashMixFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "LABEL": "Flash Mix Factor" },
        { "NAME": "FlashDarknessFactor", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Flash Darkness Factor" },


        { "NAME": "ReflectionDesaturationFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.33, "LABEL": "Reflection Desaturation" },
        { "NAME": "ReflectionColorMixFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "LABEL": "Reflection Color Mix" },
        { "NAME": "ReflectionBlendFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.98, "LABEL": "Reflection Blend Factor" },
        { "NAME": "BumpFactor1", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.005, "LABEL": "Bump Factor 1" },
        { "NAME": "BumpFactor2", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.01, "LABEL": "Bump Factor 2" },
        { "NAME": "MetallicColor", "TYPE": "color", "DEFAULT": [0.055, 0.545, 0.400, 1.0], "LABEL": "Metallic Color" },
        { "NAME": "MetallicBlendFactor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Metallic Blend Factor" },


        { "NAME": "ShadowSoftness", "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 2.0, "LABEL": "Shadow Softness" },
        { "NAME": "ShadowMaxDistance", "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 5.0, "LABEL": "Shadow Max Distance" },
        { "NAME": "SSSDensity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "LABEL": "SSS Density" },
        { "NAME": "SSSAmp", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "LABEL": "SSS Amplitude" },
        { "NAME": "AmbientOcclusionMix", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "LABEL": "Ambient Occlusion Mix" },


        { "NAME": "DitheringAmount", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Dithering Amount" },
        { "NAME": "VignetteInnerStrength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "LABEL": "Vignette Inner" },
        { "NAME": "VignetteOuterStrength", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "LABEL": "Vignette Outer" },
        { "NAME": "VignettePower", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Vignette Power" },
        { "NAME": "FinalMultiplyFactor", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.8, "LABEL": "Final Color Multiply" },
        { "NAME": "FinalPowerFactor", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.4, "LABEL": "Final Color Power" },
        { "NAME": "FinalBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 9.0, "LABEL": "Final Brightness" },
        { "NAME": "FinalAlphaDecay", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.03, "LABEL": "Final Alpha Decay" }
    ],
    "PASSES": [
        {
        },
        {
        }
    ]
}

*/


/*
  Awesome geomtry tricks by sylvain69780 , I'm still trying to comprehend
  
  
  from https://www.shadertoy.com/view/Nsd3Wl
  
  the idea get vectors for the edges and vertices, 
  to create rounded edges, holes in the faces, etc.
*/


// Normal of a plan having a dihedral angle of PI/3 with the YZ plan and PI/5 with the XZ plane
const float CP = cos(3.1415/5.), SP=sqrt(0.75-CP*CP);
const vec3  P35 = vec3(-0.5, -CP, SP);

// Dihedral angles of the Dode. and Ico.
// This probably can be obtained using linera algebra calculations
// https://en.wikipedia.org/wiki/Table_of_polyhedron_dihedral_angles

const float ICODIHEDRAL  = acos(sqrt(5.)/3.);  
const float DODEDIHEDRAL = acos(sqrt(5.)/5.);

// below are the directions from the origin limiting the coordniate's domain after folding space
// trivial, this is the Z axis
const vec3 ICOMIDEDGE = vec3(0,0,1); 
// direction in the XZ plan, the ICO vertex on this line
// I think this is also the normal of a DODE face
const vec3 ICOVERTEX  = normalize(vec3(SP,0.0,0.5)); 
// direction in the YZ plan, you will find the DODE vertex on this line
// I think this is also the normal of an ICO face
const vec3 ICOMIDFACE = normalize(vec3(0.0,SP,CP));  
/*

    This represents the up view of a Rhombic face at z = 1 
    This can help to draw some figures on the faces

                Y_TO_DODE_VERTEX
                Y_TO_ICO_CENTER (after ICODIHEDRAL rotation on X axis)
                
                         ** ********
                   *** ** ***
                *** ** ***
             *** ** ***
          *** ** ***
       *** ** ***
    *** ** (0,0)             ***
 *************************************************** X_TO_ICO_VERTEX
    *** ** *** X_TO_DODE_CENTER (after DODEDIHEDRAL rotation on Y axis)
       *** ** ***
          *** ** ***
             *** ** ***
                *** ** ***
                   *** ** ***
                      ********
                         ** */

const float X_TO_ICO_VERTEX  = length(cross(ICOMIDEDGE,ICOVERTEX))/dot(ICOMIDEDGE,ICOVERTEX);
const float Y_TO_DODE_VERTEX = length(cross(ICOMIDEDGE,ICOMIDFACE))/dot(ICOMIDEDGE,ICOMIDFACE);
const float X_TO_DODE_CENTER = X_TO_ICO_VERTEX*cos(DODEDIHEDRAL*.5);
const float Y_TO_ICO_CENTER  = Y_TO_DODE_VERTEX*cos(ICODIHEDRAL*.5);
/* Creative Commons Licence Attribution-NonCommercial-ShareAlike 
   phreax 2024

*/

#define PI 3.141592
#define TAU (2.*PI)
#define SIN(x) (sin(x)*.5+.5)
#define BUMP_EPS 0.004
const float PHI = 1.618033988749895; // Declared PHI as a const float
// SQR2, ISQR2 are not used
// #define SQR2 1.4152135
// #define ISQR2 1./SQR2

#define sabsk(x, k) sqrt(x * x + k * k)
#define sabs(x) (sabsk(x, .5)) // This is a magic number .1

const highp float NOISE_GRANULARITY = 0.5/255.0; // Renamed to avoid collision and clarify use


float tt, g_mat, g_glow;
vec3 ro;
float intro; // Declared fadeIn as a global float
float fadeIn; // Declared fadeIn as a global float

// Set explicit precision qualifiers for floats and samplers
precision highp float;
// precision highp samplerCube; // Commented out due to persistent errors
precision mediump sampler2D;

// All TEXTURE_X uniforms are declared implicitly by the ISF host from the JSON.
// Removed explicit 'uniform' declarations from GLSL code to avoid 'redefinition' errors.


mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }


float saturate(float x) {
    return clamp(x, 0., 1.);
}

// zucconis spectral palette https://www.alanzucconi.com/2017/07/15/improving-the-rainbow-2/
vec3 bump3y (vec3 x, vec3 yoffset)
{
    vec3 y = 1. - x * x;
    y = clamp((y-yoffset), vec3(0), vec3(1));
    return y;
}

//  some 2d noise for dithering
highp float random(highp vec2 coords) {
   return fract(sin(dot(coords.xy, vec2(12.9898,78.233))) * 43758.5453);
}


float rect( vec2 p, vec2 b, float r ) {
    vec2 d = abs(p) - (b - r);
    return length(max(d, 0.)) + min(max(d.x, d.y), 0.) - r;
}


vec3 invGamma(vec3 col) {
    return pow(col, vec3(2.2));
}

vec3 gamma(vec3 col) {
    return pow(col, vec3(1./2.2));
}

// Zucconi's spectral palette
vec3 spectral_zucconi6(float x) {
    x = fract(x);
    const vec3 c1 = vec3(3.54585104, 2.93225262, 2.41593945);
    const vec3 x1 = vec3(0.69549072, 0.49228336, 0.27699880);
    const vec3 y1 = vec3(0.02312639, 0.15225084, 0.52607955);
    const vec3 c2 = vec3(3.90307140, 3.21182957, 3.96587128);
    const vec3 x2 = vec3(0.11748627, 0.86755042, 0.66077860);
    const vec3 y2 = vec3(0.84897130, 0.88445281, 0.73949448);
    return bump3y(c1 * (x - x1), y1) + bump3y(c2 * (x - x2), y2) ;
}


float cyl( vec3 p, float h, float r )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - vec2(r,h);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

// from https://mercury.sexy/hg_sdf/
float pModPolar(inout vec2 p, float repetitions) {
	float angle = 2.*PI/repetitions;
	float a = atan(p.y, p.x) + angle/2.;
	float r = length(p);
	float c = floor(a/angle);
	a = mod(a,angle) - angle/2.;
	p = vec2(cos(a), sin(a))*r;
	// For an odd number of repetitions, fix cell index of the cell in -x direction
	// (cell index would be e.g. -5 and 5 in the two halves of the cell):
	if (abs(c) >= (repetitions/2.)) c = abs(c);
	return c;
}



float box(vec3 p, vec3 r) {
  vec3 d = abs(p) - r;
  return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}


float smin(float a, float b, float k) {
  float h = clamp((a-b)/k * .5 + .5, 0.0, 1.0);
  return mix(a, b, h) - h*(1.-h)*k;
}



float pMod(inout float p, float size) {
	float halfsize = size*0.5;
	float c = floor((p + halfsize)/size);
	p = mod(p + halfsize, size) - halfsize;
	return c;
}

// Global variables for glow accumulation (initially outside map for consistency)
float g_glow_accum;
float g_glow2_accum;
float g_glow3_accum;

float n21(vec2 p) {
      return fract(sin(dot(p, vec2(524.423,123.34))) * 3228324.345);
}

// smooth noise
float noise(vec2 n) {
    const vec2 d = vec2(0., 1.0);
    vec2 b = floor(n);
    vec2 f = mix(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(n21(b), n21(b + d.yx), f.x), mix(n21(b + d.xy), n21(b + d.yy), f.x), f.y);
}


// Repeat only a few times: from indices <start> to <stop> (similar to above, but more flexible)
float pModInterval1(inout float p, float size, float start, float stop) {
	float halfsize = size*0.5;
	float c = floor((p + halfsize)/size);
	p = mod(p+halfsize, size) - halfsize;
	if (c > stop) { //yes, this might not be the best thing numerically.
		p += size*(c - stop);
		c = stop;
	}
	if (c <start) {
		p += size*(c - start);
		c = start;
	}
	return c;
}


float smax( float a, float b, float k )
{
    float h = max(k-abs(a-b),0.0);
    return max(a, b) + h*h*0.25/k;
}




float sdCapsule( vec3 p, float h, float r )
{
  p.y -= clamp( p.y, 0.0, h );
  return length( p ) - r;
}



float g_id = 0.;

vec2 pMod2(inout vec2 p, vec2 size) {
	vec2 c = floor((p + size*0.5)/size);
	p = mod(p + size*0.5,size) - size*0.5;
	return c;
}


vec3 transform(vec3 p) {
    p.x += sin(p.z*.5)*.1; // Hardcoded .5 and .1
    p.xy *= rot(PI*SIN(p.z*RotationSpeed+sin(2.*tt*RotationSpeed))); // Use RotationSpeed
    
    p.x += sin(p.z*CylinderRadiusSpeed+PathAnimationSpeed*tt)*.1; // Use CylinderRadiusSpeed, PathAnimationSpeed
    p.y += cos(p.z*CylinderRadiusSpeed+PathAnimationSpeed*tt)*.1; // Use CylinderRadiusSpeed, PathAnimationSpeed
    return p;
}

float g_id2 = 0.;

vec2 path(float z) {
    vec2 p = vec2(0);
    p.x += sin(z*.5 + PathAnimationSpeed*tt)*.9*cos(z*.5); // Use PathAnimationSpeed
    p.y += cos(z*.9-PathAnimationSpeed*tt)*.4*sin(z*.3); // Use PathAnimationSpeed
    return p;
}

float sdTwisted(vec3 p, float N) {
    vec3 bp = p;
    p = transform(p);
    
    if(N < 6.) {p.z += TwistSpeed*tt;} // Use TwistSpeed
    
    g_id2 = pModPolar(p.xy, N);
    
    
    p.x -= .5; // Hardcoded .5

    g_id = pMod(p.z, CylinderHeight/3.0); // Use CylinderHeight
    

    float r = mix(CylinderRadiusMixStart, CylinderRadiusMixEnd, SIN(bp.z*CylinderRadiusSpeed+CylinderRadiusSpeed*tt)); // Use CylinderRadius parameters
    

    p.yz = p.zy;
    float d = cyl(p, CylinderHeight, r); // Use CylinderHeight, CylinderRadius

   // g_glow_accum += .04/(3.+pow(abs(d), 10.)); // Hardcoded .04, 3., 10.
   return d;
}


float map(vec3 p) {   
    // Reset glow accumulators for each raymarch call
    g_glow_accum = 0.; 
    g_glow2_accum = 0.;
    g_glow3_accum = 0.;

    p.xy -= path(p.z);
    //p.y += 1.; // Commented out in original, keeping it that way
    vec3 bp = p;
    float id, id2;
    float inner = sdTwisted(p*2., InnerTwistCount)/2.; // Use InnerTwistCount
    id = g_id;
    id2 = g_id2;

    p.xy = sabsk(p.xy, 0.02); // Hardcoded 0.02

    p.x += sin(p.z*.9+tt)*.2; // Hardcoded .9, .2
    pModPolar(p.xy, OuterTwistCount); // Use OuterTwistCount
            
    p.xy = sabsk(p.xy, 0.02); // Hardcoded 0.02
    
    float outer = sdTwisted(p*1., OuterTwistCount)/1.; // Use OuterTwistCount
    float d = smin(inner, outer, .01); // Hardcoded .01
   // d = outer; // Commented out in original, keeping it that way
   
    
    g_mat = inner < outer ? 1. : 0.;
    if(inner < outer) {
        g_id = id;
        g_id2 = id2;
    }
    
    // Original glow logic, adapted to use tunable parameters
    if(inner < outer && mod(id, 5.) < 1.)  {
        g_glow_accum += GlowFadeFactor/(5.+pow(abs(inner), MainTwistFactor)); // Use GlowFadeFactor, MainTwistFactor
        
    } else if(inner >= outer && mod(id2, 5.) < 1.) {
        g_glow_accum += GlowFadeFactor/(10.+pow(abs(outer), MainTwistFactor)); // Use GlowFadeFactor, MainTwistFactor
    }

    return d*.5; // Hardcoded .5

}


vec3 getNormal(vec3 p) {

    vec2 eps = vec2(BUMP_EPS, 0.0); // Use BUMP_EPS
    return normalize(vec3(map(p + eps.xyy) - map(p - eps.xyy),
                          map(p + eps.yxy) - map(p - eps.yxy),
                          map(p + eps.yyx) - map(p - eps.yyx)
                         )
                     );
}

// Unrolled softshadow function (fixed 50 steps)
float softshadow( in vec3 ro, in vec3 rd, float mint, float maxt, float k ) {
    float res = 1.0;
    float ph = 1e20;
    float t_loop = mint;

    // Unroll the loop for a fixed number of iterations (e.g., 50)
    // The tunable ShadowMaxDistance still controls the effective max distance
    // but the *number* of steps is fixed for compilation.
    const int MAX_SHADOW_STEPS = 50; // Fixed number of iterations

    for(int i = 0; i < MAX_SHADOW_STEPS; i++) {
        if(t_loop >= maxt) break; // Check boundary condition

        float h = map(ro + rd*t_loop);
        if( h<HitThreshold ) return 0.0;

        float y = h*h/(2.0*ph);
        float d_shadow = sqrt(h*h-y*y);
        res = min( res, k*d_shadow/max(0.0,t_loop-y) );
        ph = h;
        t_loop += h;
    }
    return res;
}


// Unrolled raymarch function (fixed 200 steps)
vec3 raymarch(vec3 ro, vec3 rd, float steps_placeholder) { // steps_placeholder is now unused
    float mat = 0.,
          t   = 0.,
          d   = 0.;
    vec3 p = ro;

    const int MAX_RAYMARCH_STEPS = 200; // Fixed number of iterations

    for(int i = 0; i < MAX_RAYMARCH_STEPS; i++) {
        d = map(p);
        mat = g_mat; // save global material
        
        t += d;
        p += rd*d;
        if(abs(d) < HitThreshold || t > MaxRaymarchDistance) break;
    }
    return vec3(t, mat, d);
}


// Removed softshadow_duplicate as it was redundant and also needed unrolling.

float calcAO(vec3 p, vec3 n)
{
	float sca = 2.0, occ = 0.0; // Hardcoded 2.0
    for( int i=0; i<5; i++ ){ // This loop uses a constant integer, so it's fine
    
        float hr = 0.01 + float(i)*0.5/4.0; // Hardcoded 0.01, 0.5, 4.0        
        float dd = map(n * hr + p);
        occ += (hr - dd)*sca;
        sca *= 0.7; // Hardcoded 0.7
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}


vec3 getRayDir(vec2 uv, vec3 p, vec3 l, float z) { // z is now CameraFocalLength
    
    // camera system
    vec3 f = normalize(l - p),  // forward vector
         r = normalize(cross(vec3(0, 1, 0), f)), // right vector
         u = cross(f, r), // up vector
         c = p + f * z, // center of virtual screen
         i = c + uv.x * r + uv.y * u, // intersection with screen
         rd = normalize(i - p);  // ray direction
         
    return rd;
    
}


void cam(inout vec3 p) {

   p.xz *= rot(PI); // Hardcoded PI
   //p.z -= mod(4.*tt -1.*sin(4.*tt), 1000.); // Hardcoded and commented out
   p.z -= mod(TwistSpeed*tt, 1000.); // Use TwistSpeed
   p.xy += path(p.z);

}


// Palette data
// Explicitly declare array elements as constants
const vec3 COLOR1_0 = vec3(84, 13, 110) / 255.0; // Indigo
const vec3 COLOR1_1 = vec3(238, 66, 102) / 255.0; // Red (Crayola)
const vec3 COLOR1_2 = vec3(255, 210, 63) / 255.0; // Sunglow
const vec3 COLOR1_3 = vec3(59, 206, 172) / 255.0; // Turquoise
const vec3 COLOR1_4 = vec3(14, 173, 105) / 255.0; // Jade

const vec3 COLOR2_0 = vec3(27, 231, 255) / 255.0; // Electric blue
const vec3 COLOR2_1 = vec3(110, 235, 131) / 255.0; // Light green
const vec3 COLOR2_2 = vec3(228, 255, 26) / 255.0; // Lemon Lime
const vec3 COLOR2_3 = vec3(255, 184, 0) / 255.0; // Selective yellow
const vec3 COLOR2_4 = vec3(255, 87, 20) / 255.0; // Giants orange

const vec3 COLOR3_0 = vec3(155, 93, 229) / 255.0; // Amethyst
const vec3 COLOR3_1 = vec3(241, 91, 181) / 255.0; // Brilliant rose
const vec3 COLOR3_2 = vec3(254, 228, 64) / 255.0; // Maize
const vec3 COLOR3_3 = vec3(0, 187, 249) / 255.0; // Deep Sky Blue
const vec3 COLOR3_4 = vec3(0, 245, 212) / 255.0; // Aquamarine

const vec3 COLOR4_0 = vec3(0.169, 0.761, 0.718);
const vec3 COLOR4_1 = vec3(0.357, 0.518, 0.008);
const vec3 COLOR4_2 = vec3(0.604, 0.851, 0.259);
const vec3 COLOR4_3 = vec3(0.820, 0.235, 0.196);
const vec3 COLOR4_4 = vec3(0.522, 0.075, 0.020);

const vec3 COLOR5_0 = vec3(237, 174, 73) / 255.0; // Hunyadi yellow
const vec3 COLOR5_1 = vec3(209, 73, 91) / 255.0; // Amaranth
const vec3 COLOR5_2 = vec3(0, 121, 140) / 255.0; // Caribbean Current
const vec3 COLOR5_3 = vec3(48, 99, 142) / 255.0; // Lapis Lazuli
const vec3 COLOR5_4 = vec3(0, 61, 91) / 255.0; // Indigo dye

const vec3 COLOR6_0 = vec3(0, 204, 255) / 255.0; // Vivid sky blue
const vec3 COLOR6_1 = vec3(0, 255, 204) / 255.0; // Aquamarine
const vec3 COLOR6_2 = vec3(255, 255, 0) / 255.0; // Yellow
const vec3 COLOR6_3 = vec3(255, 0, 204) / 255.0; // Hot magenta
const vec3 COLOR6_4 = vec3(204, 0, 255) / 255.0; // Electric purple

const vec3 COLOR7_0 = vec3(176.,29.,30.) / 255.;
const vec3 COLOR7_1 = vec3(241.,104.,38.) / 255.;
const vec3 COLOR7_2 = vec3(234.,211.,95.) / 255.;
const vec3 COLOR7_3 = vec3(0.,187.,173.) / 255.;
const vec3 COLOR7_4 = vec3(0.,107.,228.) / 255.;
const vec3 COLOR7_5 = vec3(126.,99.,180.) / 255.;


vec3 getColorRamp(int palette, float x) {
    int len = 5; // Default length for most palettes
    vec3 col1_val, col2_val;

    if (palette == 6) { // Special case for colors7_arr
        len = 6;
    }
    // No need for colors77_arr or colors78_arr since they are not explicitly used in main,
    // and would require similar individual constant declarations if needed.

    // Scale x according to the adjusted length and apply modulo for wrapping
    // Use a small epsilon to avoid exactly hitting integer boundary for mod.
    float scaledX = mod(x * (float(len) - 0.001), float(len));
    
    // Calculate indices. Ensure index2 wraps around to the start if necessary
    int index1 = int(scaledX);
    int index2 = int(mod(float(index1 + 1), float(len)));
    
    // Calculate the fraction between the two indices for smooth interpolation
    float frac = fract(scaledX);
    
    // Select the correct constant palette and get the two colors for interpolation
    if(palette == 0) {
        if (index1 == 0) col1_val = COLOR1_0; else if (index1 == 1) col1_val = COLOR1_1; else if (index1 == 2) col1_val = COLOR1_2; else if (index1 == 3) col1_val = COLOR1_3; else col1_val = COLOR1_4;
        if (index2 == 0) col2_val = COLOR1_0; else if (index2 == 1) col2_val = COLOR1_1; else if (index2 == 2) col2_val = COLOR1_2; else if (index2 == 3) col2_val = COLOR1_3; else col2_val = COLOR1_4;
    } else if(palette == 1) {
        if (index1 == 0) col1_val = COLOR2_0; else if (index1 == 1) col1_val = COLOR2_1; else if (index1 == 2) col1_val = COLOR2_2; else if (index1 == 3) col1_val = COLOR2_3; else col1_val = COLOR2_4;
        if (index2 == 0) col2_val = COLOR2_0; else if (index2 == 1) col2_val = COLOR2_1; else if (index2 == 2) col2_val = COLOR2_2; else if (index2 == 3) col2_val = COLOR2_3; else col2_val = COLOR2_4;
    } else if(palette == 2) {
        if (index1 == 0) col1_val = COLOR3_0; else if (index1 == 1) col1_val = COLOR3_1; else if (index1 == 2) col1_val = COLOR3_2; else if (index1 == 3) col1_val = COLOR3_3; else col1_val = COLOR3_4;
        if (index2 == 0) col2_val = COLOR3_0; else if (index2 == 1) col2_val = COLOR3_1; else if (index2 == 2) col2_val = COLOR3_2; else if (index2 == 3) col2_val = COLOR3_3; else col2_val = COLOR3_4;
    } else if(palette == 3) {
        if (index1 == 0) col1_val = COLOR4_0; else if (index1 == 1) col1_val = COLOR4_1; else if (index1 == 2) col1_val = COLOR4_2; else if (index1 == 3) col1_val = COLOR4_3; else col1_val = COLOR4_4;
        if (index2 == 0) col2_val = COLOR4_0; else if (index2 == 1) col2_val = COLOR4_1; else if (index2 == 2) col2_val = COLOR4_2; else if (index2 == 3) col2_val = COLOR4_3; else col2_val = COLOR4_4;
    } else if(palette == 4) {
        if (index1 == 0) col1_val = COLOR5_0; else if (index1 == 1) col1_val = COLOR5_1; else if (index1 == 2) col1_val = COLOR5_2; else if (index1 == 3) col1_val = COLOR5_3; else col1_val = COLOR5_4;
        if (index2 == 0) col2_val = COLOR5_0; else if (index2 == 1) col2_val = COLOR5_1; else if (index2 == 2) col2_val = COLOR5_2; else if (index2 == 3) col2_val = COLOR5_3; else col2_val = COLOR5_4;
    } else if(palette == 5) {
        if (index1 == 0) col1_val = COLOR6_0; else if (index1 == 1) col1_val = COLOR6_1; else if (index1 == 2) col1_val = COLOR6_2; else if (index1 == 3) col1_val = COLOR6_3; else col1_val = COLOR6_4;
        if (index2 == 0) col2_val = COLOR6_0; else if (index2 == 1) col2_val = COLOR6_1; else if (index2 == 2) col2_val = COLOR6_2; else if (index2 == 3) col2_val = COLOR6_3; else col2_val = COLOR6_4;
    } else if(palette == 6) { // Special case for 6 elements
        if (index1 == 0) col1_val = COLOR7_0; else if (index1 == 1) col1_val = COLOR7_1; else if (index1 == 2) col1_val = COLOR7_2; else if (index1 == 3) col1_val = COLOR7_3; else if (index1 == 4) col1_val = COLOR7_4; else col1_val = COLOR7_5;
        if (index2 == 0) col2_val = COLOR7_0; else if (index2 == 1) col2_val = COLOR7_1; else if (index2 == 2) col2_val = COLOR7_2; else if (index2 == 3) col2_val = COLOR7_3; else if (index2 == 4) col2_val = COLOR7_4; else col2_val = COLOR7_5;
    } else {
        // Fallback to a default palette if index is out of range
        col1_val = COLOR1_0;
        col2_val = COLOR1_1;
    }
    
    // Interpolate between the two selected colors using smoothstep
    return mix(col1_val, col2_val, smoothstep(0.0, .9, frac));
}


void main() {
	if (PASSINDEX == 0)	{
	}
	else if (PASSINDEX == 1)	{

	   	vec2 uv = (gl_FragCoord.xy - .5*RENDERSIZE.xy)/RENDERSIZE.y; // Standardized UV calculation
	
	    tt = AnimationSpeed*TIME; // Use AnimationSpeed
	
	    vec3  lp = vec3(Light1PosX, Light1PosY, Light1PosZ); // Use Light positions from uniforms
	    vec3  lp2 = vec3(Light2PosX, Light2PosY, Light2PosZ); // Use Light positions from uniforms
	
	
	    fadeIn = smoothstep(1., 7., TIME);   // Hardcoded 1., 7.
	    //uv = uv.yx; // Commented out in original, keeping it that way
	    vec3 col = vec3(0);
	    float id = 0., id2 = 0., flash = 0.;
	    
	    ro = vec3(0.0, CameraYOffset, CameraZOffset); // Use CameraYOffset, CameraZOffset
	    vec3 lookat = vec3(0, 0, 0), p_unused; // p is unused
	    
	   
	      //  lookat = -tunnel(lookat); // Commented out in original, keeping it that way
	    cam(ro);
	    cam(lp2);
	    cam(lookat);
	   
	    
	    intro = smoothstep(0., 1., TIME); // Hardcoded 0., 1.
	   
	    float focal = CameraFocalLength; // Use CameraFocalLength
	    vec3 rd = getRayDir(uv, ro, lookat, focal);
	
	           
	    
	    float mat = 0., matId = 0.,
	          t   = 0.,
	          d   = 0.;
	     
	
	    vec2 e_norm = vec2(0.0035, -0.0035); // Hardcoded 0.0035 for normal calculation
	     
	    // background color
	    vec3 c1 = BackgroundColor1.rgb; // Use BackgroundColor1
	    vec3 c2 = BackgroundColor2.rgb; // Use BackgroundColor2
	    
	    // light color
	    vec3 lc1 = LightColor1.rgb; // Use LightColor1
	    vec3 lc2 = LightColor2.rgb; // Use LightColor2
	    
	    vec3 bg_color = vec3(0.090,0.078,0.102)*BackgroundBrightness; // Use BackgroundBrightness
	    // currently only one pass
	    // The loop here seems to be a single iteration loop for a fixed number of steps
	    // `i > 0` condition won't be true, so it will always use RaymarchInitialSteps
	    // Simplified to a single call since 'i' is only 0.
	    // for(float i = 0.; i < 1.; i++) { 
	        // The steps parameter is now unused as the raymarch loop is unrolled
	        vec3 rm = raymarch(ro, rd, RaymarchInitialSteps); 
	        id = g_id;
	        id2 = g_id2;
	        float glow = g_glow_accum; // Use accumulated glow
	        mat = rm.y;
	        
	        
	        vec3 p = ro + rm.x*rd;
	      
	        vec3 p2 = p; // p2 is unused
	        
	        //p = g_p; // g_p is not a global for this shader
	        vec3 n = normalize( e_norm.xyy*map(p+e_norm.xyy) + e_norm.yyx*map(p+e_norm.yyx) +
	                            e_norm.yxy*map(p+e_norm.yxy) + e_norm.xxx*map(p+e_norm.xxx)); // Corrected e.xxx to e_norm.xxx
	    
	
	        vec3 texCol = vec3(0); // texCol is unused
	 
	        flash = float(mod(id-FlashTimeScale*tt, 5.) < 1.); // Use FlashTimeScale
	   
	        vec3 flashCol = getColorRamp(int(FlashPaletteIndex), id*0.1 + FlashTimeScale*tt); // Use FlashPaletteIndex, FlashTimeScale, FlashSpatialScale
	
	        if(rm.z < HitThreshold) { // Use HitThreshold
	        
	            vec3 l = normalize(lp-p);
	            vec3 l2 = normalize(lp2-p);
	            float dif = max(dot(n, l), .0);
	            float dif2 = max(dot(n, l2), .0);
	            float spe = pow(max(dot(reflect(-rd, n), -l), .0),SpecularPower); // Use SpecularPower
	            
	            float shd=softshadow( p, l2+l, ShadowSoftness, ShadowMaxDistance, 1. ); // Use ShadowSoftness, ShadowMaxDistance, k=1.
	       
	            float sss = smoothstep(0., 1., map(p + l * SSSDensity)) / SSSAmp; // Use SSSDensity, SSSAmp
	            float sss2 = smoothstep(0., 1., map(p + l2 * SSSDensity)) / SSSAmp; // Use SSSDensity, SSSAmp
	
	
	            vec3 n2 = n;
	            n2.xy += noise(p.xy) * .5 - .025; // Hardcoded .5, .025
	            n2 = normalize(n2);
	            float height = atan(n2.y, n2.x);
	
	            vec3 iri = spectral_zucconi6(height*1.11)*smoothstep(.8, .2, abs(n2.z))-.02; // Hardcoded 1.11, .8, .2, .02
	
	            col = vec3(sss + dif*lc1 + dif2*lc2 + sss2) + iri; // Using tuned light colors
	      
	            float ao = calcAO(p, n);
	
	            
	            col *= mix(col, col*ao, AmbientOcclusionMix); // Use AmbientOcclusionMix
	            if(mat < 3.) { // Hardcoded 3.
	                rd = reflect(rd, n);
	       
	                // Removed textureCube sampling due to persistent compilation errors.
	                // Original lines:
	                // highp vec3 reflected_dir = rd;
	                // mediump vec4 sampled_refl = textureCube(TEXTURE_0, reflected_dir);
	                // mediump vec3 refl = sampled_refl.rgb;
	                
	                // Simplified reflection / metallic color for compilation:
	                vec3 refl = mix(vec3(0.0), MetallicColor.rgb, MetallicBlendFactor); // Use MetallicColor and MetallicBlendFactor
	                refl = vec3(dot(refl, vec3(ReflectionDesaturationFactor))); // Use ReflectionDesaturationFactor
	
	             
	                refl = invGamma(refl);
	       
	               
	                refl *= mix(vec3(1), spectral_zucconi6(n.x*n.y*3.), ReflectionColorMixFactor); // Use ReflectionColorMixFactor
	                col = mix(col, refl.rgb, ReflectionBlendFactor); // Use ReflectionBlendFactor
	
	          
	                t = rm.x;
	              
	                float fog = 1.-exp(-t*t*0.01); // Hardcoded 0.01 for fog density, could be a uniform
	                
	                
	                p = transform(p);
	                if(mat > 0.) { // Hardcoded 0.
	                    p.z += SecondaryTwistFactor*tt; // Use SecondaryTwistFactor
	                    p *= MainTwistFactor; // Use MainTwistFactor
	                } else p *= 2.; // Hardcoded 2.
	                
	                p *= MainTwistFactor; // Use MainTwistFactor
	                
	        
	                
	      
	 
	                
	                float dark = SIN(p.z*FlashSpatialScale+PI/2. + MainTwistFactor*tt); // Use FlashSpatialScale, MainTwistFactor
	                matId =  mod(id, 2.); // Hardcoded 2.
	                
	                
	                vec3 rainbow = getColorRamp(int(mix(2., 5., matId)), SIN(p.x*4.5+p.x*FlashSpatialScale + MainTwistFactor*p.z + id*FlashSpatialScale) + SIN(p.x*5.4+p.y*5.4 + id/10.)*.1 ); // Use FlashSpatialScale
	                
	                vec3 col1 = 1.0*mix(col, mix(col*rainbow, rainbow, .5), 1.*.5); // Hardcoded .5, 1.0, .5
	                vec3 col2 = 1.0*mix(col, mix(col*rainbow, rainbow, .5), 0.); // Hardcoded 1.0, .5, 0.
	            
	               col *= .9; // Hardcoded .9
	               
	              
	                vec3 col3 = mix(col1, col2, .5); // Hardcoded .5
	              //  col = mix(col+.3*col3, col, .6); // Commented out in original, keeping it that way
	               
	                if(mat < 1. ) { // Hardcoded 1.
	                    col *= .3; // Hardcoded .3
	                    flashCol *= .4; // Hardcoded .4
	                }
	                
	                col = mix(col, flashCol*FlashMixFactor, FlashMixFactor*clamp(flash, 0., 1.)*dark*FlashDarknessFactor); // Use FlashMixFactor, FlashDarknessFactor
	                col = mix(col, bg_color, fog); // Use bg_color
	     
	                
	            } 
	            
	            
	            col = mix(col*shd, col, .5); // Hardcoded .5 for shadow mix
	            
	            if(mat == 2.) col *= .8; // Hardcoded .8
	            if(mat == 1.) col *= .4; // Hardcoded .4
	
	        } else {
	           col = bg_color; // Use bg_color
	
	        } 
	        
	
	         col += GlowIntensity*glow*flashCol; // Use GlowIntensity, flashCol
	
	    // } // End of the single-iteration loop
	    
	    // dithering to avoid banding in the background
	    col += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random(uv)) * DitheringAmount; // Use DitheringAmount
	    col *= mix(VignetteInnerStrength, VignetteOuterStrength, (1.5-pow(dot(uv, uv), VignettePower))); // Use Vignette parameters
	    
	    // color control
	    col = pow(col*FinalMultiplyFactor, vec3(FinalPowerFactor))*FinalBrightness; // Use Final color controls
	    
	    col = gamma(col); // gamma
	    
	    
	    gl_FragColor = vec4(col, 1.0 - t * FinalAlphaDecay); // Use FinalAlphaDecay
	}

}
