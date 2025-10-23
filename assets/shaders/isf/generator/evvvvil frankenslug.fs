/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/3dtcDB by evvvvil. Frankenslug - Result of improvised live code session on Twitch. Now with tunable parameters for color, glow, and detail.",
    "IMPORTED": {
        "TEXTURE_0": {
            "NAME": "TEXTURE_0",
            "PATH": "79520a3d3a0f4d3caa440802ef4362e99d54e12b1392973e4ea321840970a88a.jpg"
        }
    },
    "INPUTS": [
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "MorphingStrength",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Morphing Strength"
        },
        {
            "NAME": "CameraOrbitSpeed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Camera Orbit Speed"
        },
        {
            "NAME": "ColorPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Color Pulse Intensity"
        },
        {
            "NAME": "ColorPulseFrequency",
            "TYPE": "float",
            "DEFAULT": 5.0,
            "MIN": 0.1,
            "MAX": 10.0,
            "LABEL": "Color Pulse Frequency"
        },
        { "NAME": "BaseColorFrankenR", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color R" },
        { "NAME": "BaseColorFrankenG", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color G" },
        { "NAME": "BaseColorFrankenB", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color B" },
        {
            "NAME": "DarkeningFactorFranken",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Darkening Factor"
        },
        { "NAME": "AmbientHueShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Ambient Hue Shift" },
        { "NAME": "AmbientSaturation", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Ambient Saturation" },
        {
            "NAME": "BlobDetailScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Blob Detail Scale"
        },
        {
            "NAME": "MainGlowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Main Glow Intensity"
        },
        { "NAME": "GlowHueShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Hue Shift" },
        { "NAME": "GlowSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Saturation" }
    ]
}

*/


// Frankenslug - Result of an improvised live coding session on Twitch
// LIVE SHADER CODING, SHADER SHOWDOWN STYLE, EVERY TUESDAYS 20:00 Uk time: 
// https://www.twitch.tv/evvvvil_

precision highp float; // Set high precision for floats
precision mediump sampler2D; // Set medium precision for sampler2D

vec2 z,v,e=vec2(.0035,-.0035);
float t,tt,tnoi=1.;
vec3 blobP,op,po,no,al,ld,colorP;

// Global accumulators for glow, passed as inout to mp
float g, ggg; // Declared global
float gg; // Declared global

float lngSp( vec3 p, vec3 h,float r ) {p = p - clamp( p, -h, h );return length( p )-r;}
const float PI=acos(-1.);
float smin(float a,float b,float k){float h=max(k-abs(a-b),0.);return min(a,b)-h*h*.25/k;}
float smax(float a,float b, float k){float h=max(k-abs(-a-b),0.);return max(-a,b)+h*h*.25/k;} // Fixed: added 'float' to 'k'
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Using texture2D for TEXTURE_0
vec4 texNoise(vec2 uv){
    float f = 0.;
    f+=texture2D(TEXTURE_0,mod(uv*.125,1.0)).r*.5;
    f+=texture2D(TEXTURE_0,mod(uv*.25,1.0)).r*.25;
    f+=texture2D(TEXTURE_0,mod(uv*.5,1.0)).r*.125;
    f+=texture2D(TEXTURE_0,mod(uv*1.,1.0)).r*.125;
    f=pow(f,1.2);
    return vec4(f*.45+.05);
}

// Tunable Parameters are implicitly declared by the ISF host from the JSON header.
// No explicit 'uniform' declarations are needed here to avoid redefinition errors.

// Modified bo function to accept float for 'r' and convert internally
float bo(vec3 p, float r_val){
    vec3 r = vec3(r_val); // Convert float to vec3 internally
    p=abs(p)-r;
    return max(max(p.x,p.y),p.z);
}

vec2 fb( vec3 p,float size )
{
  vec3 pp_local_fb; // Local declaration for pp in fb
  vec2 h,t;
  
  // Call bo with float literals directly
  t=vec2(bo(p,4.0),5.0);  
  t.x=abs(t.x)-.55;
  t.x=max(t.x,abs(p.y)-1.0);  // Explicit float
  t.x=smin(t.x,0.4*length(abs(p.yz+tnoi*1.8)-vec2(0.0,4.0*size))-.6*size,1.0); // Explicit floats
  
  h=vec2(bo(p,4.0),3.0);  
  h.x=abs(h.x)-.3;
  h.x=max(h.x,abs(p.y)-1.5);    
  t=t.x<h.x?t:h;  
  
  h=vec2(bo(p,3.5),6.0);  
  h.x=abs(h.x)-.7;
  h.x=max(h.x,abs(p.y)-.5);  
  h.x=max(h.x,abs(abs(abs(p.z)-2.0)-1.0)-.5);  // Explicit floats
  h.x=max(h.x,abs(abs(abs(p.x)-2.0)-1.0)-.5);  // Explicit floats
  t=t.x<h.x?t:h;  
  h=vec2(.6*length(p.yz)-.1+.0*sin(p.x)*.1,6.0); // Explicit float
  pp_local_fb=op; // op is global, assigned earlier in mp
  pp_local_fb.z=mod(pp_local_fb.z-tt*20.0+35.0,100.0)-50.0; // Explicit floats
  g+=0.1/(0.1+h.x*h.x*(40.0-39.9*sin(pp_local_fb.z*.05+.2*sin(pp_local_fb.x*.3)))); // Explicit floats
  t=t.x<h.x?t:h;
  return t;
}

vec2 mp( vec3 p, inout float g_accum, inout float gg_accum ) // g_accum and gg_accum are inout
{
    vec3 np_local = p; // Local declaration
    vec3 bp_local = p; // Local declaration
    float anim_local; // Local declaration
    float animInv_local; // Local declaration
    float b_local; // Local declaration

    op=p; // Global op gets the current p

    np_local.xy*=r2(sin(p.z*.15)*.3);  
    np_local.z=mod(np_local.z+tt*5.0,24.0)-12.0; // Explicit floats
    anim_local = sin(tt*.5+op.z*.03) * MorphingStrength; // Apply MorphingStrength
    animInv_local = cos(tt*.5+op.z*.03) * MorphingStrength; // Apply MorphingStrength
    vec2 h,t=vec2(1000.0); // Explicit float for vec2 constructor
    for(int i=0;i<6;i++){
        np_local.xz=abs(np_local.xz)-vec2(4.0,6.0) * BlobDetailScale; // Applied BlobDetailScale, explicit vec2
        np_local.xy*=r2(.17);
    } 
    tnoi=texNoise(vec2(np_local.x,dot(np_local.xz,vec2(.5,.5)))*vec2(.15,.3)).r; // Explicit vec2 for dot product
    t=fb(np_local,1.0 * BlobDetailScale); // Applied DnaDetailScale
    np_local.xz*=r2(.785*sin(np_local.z*.2));  
    h=fb(np_local.xzy,0.9 * BlobDetailScale); // Applied DnaDetailScale
    h=fb((np_local+vec3(4.0,-4.5,2.0))*3.0,0.3 * BlobDetailScale);h.x/=3.0; // Applied DnaDetailScale, explicit vec3, explicit float
    t=t.x<h.x?t:h;
    np_local.xz*=r2(-.785); 
    h=fb((np_local+vec3(0.0,-5.0,1.0))*5.0,0.0 * BlobDetailScale);h.x/=5.0; // Applied DnaDetailScale, explicit vec3, explicit float
    t=t.x<h.x?t:h;

    bp_local=p-vec3(0.0,20.0,0.0); // Explicit vec3
    bp_local.xy*=r2(-animInv_local*2.5+tt);
    b_local=sin(bp_local.z*15.0)*0.03 * MorphingStrength; // Apply MorphingStrength, explicit float
    h=vec2(length(abs(bp_local.xy)-vec2(4.0,0.0))-.2-b_local,3.0);h.x*=0.8; // Explicit vec2 and float
    t=t.x<h.x?t:h;
    bp_local.z=mod(bp_local.z-tt*20.0,100.0)-50.0; // Explicit floats
    h=vec2(length(bp_local.yz)-.1+abs(bp_local.x)*.03,6.0); // Explicit float
    gg_accum+=0.1/(0.1+h.x*h.x*40.0); // Use gg_accum, explicit float
    t=t.x<h.x?t:h;
    
    vec3 pp_local_inner = bp_local; // Local variable for this block
    pp_local_inner.z=mod(pp_local_inner.z+tt*30.0,10.0)-5.0; // Explicit floats
    h=vec2(length(abs(abs(pp_local_inner.xy)-vec2(1.0,0.0))-vec2(.5,.0))-.1+abs(pp_local_inner.z)*.02,6.0); // Explicit vec2 and float
    h.x=max(h.x,pp_local_inner.z);
    g_accum+=0.1/(0.1+h.x*h.x*(4.0-3.9*animInv_local)); // Use g_accum, explicit float
    t=t.x<h.x?t:h;
    t.x*=0.8 * BlobDetailScale; // Applied DnaDetailScale to the final distance
    return t;
}

vec2 tr( vec3 ro,vec3 rd)
{
    vec2 h,t=vec2(.2);
    for(int i=0;i<128;i++){
        h=mp(ro+rd*t.x, g, gg); // Pass g and gg as inout
        if(h.x<.0001||t.x>35.0) break; // Explicit float
        t.x+=h.x;t.y=h.y;
    }
    if(t.x>35.0) t.y=0.0; // Explicit float
    return t;
}
#define a(d) clamp(mp(po+no*d, g, gg).x/d,0.,1.) // Pass g and gg
#define s(d) smoothstep(0.,1.,mp(po+ld*d, g, gg).x/d) // Pass g and gg
void main() {
    // Reset global glow accumulators per frame
    g = 0.0; 
    gg = 0.0; // Ensure gg is also reset
    ggg = 0.0;

    vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.0); // Explicit float
    tt=mod(TIME * AnimationSpeed + 9.3, 62.83); // Apply AnimationSpeed
    vec3 ro=mix(vec3(18.0+cos(tt*.2)*9.0,4.0,sin(tt*.4)*4.0), // Explicit floats
                vec3(5.0-sin(tt*CameraOrbitSpeed)*20.0,cos(tt*CameraOrbitSpeed)*5.0,-2.0),ceil(cos(tt*0.2))); // Apply CameraOrbitSpeed, explicit floats
    vec3 cw=normalize(vec3(0.0)-ro),cu=normalize(cross(cw,vec3(0.0,1.0,0.0))), // Explicit vec3
        cv=normalize(cross(cu,cw)),rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo;
    
    // Base background color and darkening factor from tunable parameters
    co=fo=vec3(BaseColorFrankenR, BaseColorFrankenG, BaseColorFrankenB) - length(uv)*DarkeningFactorFranken-rd.y*.2;
    
    ld=normalize(vec3(.3,.3,-.0));
    z=tr(ro,rd);t=z.x;
    if(z.y>0.){
        po=ro+rd*t;
        no=normalize(e.xyy*mp(po+e.xyy, g, gg).x+e.yyx*mp(po+e.yyx, g, gg).x+e.yxy*mp(po+e.yxy, g, gg).x+e.xxx*mp(po+e.xxx, g, gg).x); // Pass g and gg
        
        float spa_local=1.0,sspa_local=0.0; // Local declarations, explicit floats
        
        // Ambient color mix from tunable parameters using HSV and spatial variation based on po
        vec3 ambientColor1 = hsv2rgb(vec3(AmbientHueShift, AmbientSaturation, 0.4));
        vec3 ambientColor2 = hsv2rgb(vec3(mod(AmbientHueShift + 0.5, 1.0), AmbientSaturation, 0.4)); // A complementary hue

        // Use po for spatial variation in ambient mix
        al = mix(ambientColor1, ambientColor2, sin(dot(po.xy, vec2(0.5, 0.5)) + TIME * ColorPulseFrequency * 0.7) * 0.5 + 0.5) + tnoi; 

        if(z.y<5.0)al=vec3(0.0); // Explicit float for vec3
        if(z.y>5.0)al=vec3(1.0),spa_local=0.0; // Explicit float for vec3 and assignment
        if(z.y>6.0)al=vec3(.1,.2,.4)*.5,spa_local=sspa_local=1.0; // Explicit float for assignment
        float dif=max(0.0,dot(no,ld)), // Explicit float
            fr=pow(1.0+dot(no,rd),4.0), // Explicit float
            sp=pow(max(dot(reflect(-ld,no),-rd),0.0),40.0); // Explicit float
        co=mix(sp*spa_local+al*(a(.05)*a(.2)+.2)*(dif*vec3(.6,.7,.9)+s(.5)*sspa_local*2.0),fo,min(fr,.5)); // Explicit float
        co=mix(fo,co,exp(-.00008*t*t*t));
    }

    // Glow colors from tunable parameters using HSV
    vec3 lazerGlow = hsv2rgb(vec3(GlowHueShift, GlowSaturation, 1.0));
    vec3 reddishGlow = hsv2rgb(vec3(mod(GlowHueShift + 0.3, 1.0), GlowSaturation, 1.0)); // A slightly shifted hue

    // Apply main glow intensity and color pulse
    co += MainGlowIntensity * (g * 0.2 * lazerGlow + ggg * 0.2 * reddishGlow);
    co *= (1.0 + ColorPulseIntensity * sin(tt * ColorPulseFrequency)); // Apply color pulse effect

    gl_FragColor = vec4(pow(co,vec3(.65)),1.0); // Explicit float for vec4
}
