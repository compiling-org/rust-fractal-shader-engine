/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/WtfBzX by evvvvil. DNA tracer - Result of improvised live code session on Twitch. Now with tunable parameters for color, glow, and detail.",
    "IMPORTED": {
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
            "NAME": "MorphingIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Morphing Intensity"
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
        { "NAME": "BaseColorR", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Red" },
        { "NAME": "BaseColorG", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Green" },
        { "NAME": "BaseColorB", "TYPE": "float", "DEFAULT": 0.13, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Blue" },
        { "NAME": "DarkeningFactorR", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Darkening Factor Red" },
        { "NAME": "DarkeningFactorG", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Darkening Factor Green" },
        { "NAME": "DarkeningFactorB", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Darkening Factor Blue" },
        {
            "NAME": "DnaDetailScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "DNA Detail Scale (Size)"
        },
        {
            "NAME": "MainGlowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Main Glow Intensity"
        }
    ]
}

*/


// DNA tracer - Result of an improvised live coding session on Twitch
// LIVE SHADER CODING, SHADER SHOWDOWN STYLE, EVERY TUESDAYS 20:00 Uk time: 
// https://www.twitch.tv/evvvvil_

precision highp float; // Set high precision for floats
// precision mediump sampler2D; // Removed: no image input

vec2 z,v,e=vec2(.0035,-.0035);
// tnoi is no longer a global default, it's calculated in mp
float t,tnoi,tt,b,bb,anim,animInv,g,gg;
vec3 np,bp,pp,op,po,no,al,ld;

// Tunable Parameters are implicitly declared by the ISF host from the JSON header.
// No explicit 'uniform' declarations are needed here to avoid redefinition errors.

float bo(vec3 p,vec3 r){p=abs(p)-r; return max(max(p.x,p.y),p.z);}
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}
float smin( float d1, float d2, float k ){  float h = max(k-abs(d1-d2),0.0);return min(d1,d2)-h*h*0.25/k; }

// Procedural noise function to replace texNoise - Adjusted for even smoother output
float custom_noise(vec3 p) {
    // Reduced multipliers for p.xy, p.yz, p.xz to make the noise more diffuse
    float n = fract(sin(dot(p.xy * 0.05, vec2(12.9898, 78.233))) * 43758.5453);
    n += fract(sin(dot(p.yz * 0.05, vec2(89.123, 45.678))) * 12345.6789);
    n += fract(sin(dot(p.xz * 0.05, vec2(23.456, 76.543))) * 98765.4321);
    // Further scale down the noise magnitude and adjust time for subtle animation
    return fract(n * 0.2 + TIME * 0.02); // Reduced overall scale of noise, slower time variation
}


vec2 fb( vec3 p,float size )
{
  vec2 h,t=vec2(bo(p,vec3(4.0, 4.0, 4.0)),5.0);  // Explicit float initializers
  t.x=abs(t.x)-.55;
  t.x=max(t.x,abs(p.y)-1.0);  // Explicit float
  t.x=smin(t.x,0.4*length(abs(p.yz+tnoi*1.8)-vec2(0.0,4.0*size))-.6*size,1.0); // Explicit float
  h=vec2(bo(p,vec3(4.0, 4.0, 4.0)),3.0);  // Explicit float initializers
  h.x=abs(h.x)-.3;
  h.x=max(h.x,abs(p.y)-1.5);    
  t=t.x<h.x?t:h;  
  h=vec2(bo(p,vec3(3.5, 3.5, 3.5)),6.0);  // Explicit float initializers
  h.x=abs(h.x)-.7;
  h.x=max(h.x,abs(p.y)-.5);  
  h.x=max(h.x,abs(abs(abs(p.z)-2.0)-1.0)-.5);  // Explicit float
  h.x=max(h.x,abs(abs(abs(p.x)-2.0)-1.0)-.5);  // Explicit float
  t=t.x<h.x?t:h;  
  h=vec2(.6*length(p.yz)-.1+.0*sin(p.x)*.1,6.0); // Explicit float
  pp=op; pp.z=mod(pp.z-tt*20.0+35.0,100.0)-50.0; // Explicit float
  g+=0.1/(0.1+h.x*h.x*(40.0-39.9*sin(pp.z*.05+.2*sin(pp.x*.3)))); // Explicit float
  t=t.x<h.x?t:h;
  return t;
}
vec2 mp( vec3 p )
{
    vec3 np_local=p; // Renamed to local
    vec3 bp_local=p; // Renamed to local
    float anim_local;
    float animInv_local;
    float b_local;

    op=p; // op is a global var
    
    np_local.xy*=r2(sin(p.z*.15)*.3);  
    np_local.z=mod(np_local.z+tt*5.0,24.0)-12.0; // Explicit float
    anim_local = sin(tt*.5+op.z*.03) * MorphingIntensity; // Apply MorphingIntensity
    animInv_local = cos(tt*.5+op.z*.03) * MorphingIntensity; // Apply MorphingIntensity
    vec2 h,t=vec2(1000.0); // Explicit float initializers
    for(int i=0;i<6;i++){
        np_local.xz=abs(np_local.xz)-vec2(4.0,6.0); // No DnaDetailScale here directly, controlled by fb
        np_local.xy*=r2(.17);
    }
    
    // tnoi now calculated procedurally
    tnoi=custom_noise(np_local * DnaDetailScale * 0.5 + TIME * 0.2); // Apply DnaDetailScale

    t=fb(np_local,1.0 * DnaDetailScale); // Applied DnaDetailScale
    np_local.xz*=r2(.785*sin(np_local.z*.2));  
    h=fb(np_local.xzy,0.9 * DnaDetailScale); // Applied DnaDetailScale
    h=fb((np_local+vec3(4.0,-4.5,2.0))*3.0,0.3 * DnaDetailScale);h.x/=3.0; // Applied DnaDetailScale, explicit float
    t=t.x<h.x?t:h;
    np_local.xz*=r2(-.785); 
    h=fb((np_local+vec3(0.0,-5.0,1.0))*5.0,0.0 * DnaDetailScale);h.x/=5.0; // Applied DnaDetailScale, explicit float
    t=t.x<h.x?t:h;
    bp_local=p-vec3(0.0,20.0,0.0); // Explicit float
    bp_local.xy*=r2(-animInv_local*2.5+tt);
    b_local=sin(bp_local.z*15.0)*0.03 * MorphingIntensity; // Apply MorphingIntensity, explicit float
    h=vec2(length(abs(bp_local.xy)-vec2(4.0,0.0))-.2-b_local,3.0);h.x*=0.8; // Explicit float
    t=t.x<h.x?t:h;
    bp_local.z=mod(bp_local.z-tt*20.0,100.0)-50.0; // Explicit float
    h=vec2(length(bp_local.yz)-.1+abs(bp_local.x)*.03,6.0); // Explicit float
    gg+=0.1/(0.1+h.x*h.x*40.0); // Explicit float
    t=t.x<h.x?t:h;
    pp=bp_local; bp_local.z=mod(bp_local.z+tt*30.0,10.0)-5.0; // Explicit float
    h=vec2(length(abs(abs(bp_local.xy)-vec2(1.0,0.0))-vec2(.5,.0))-.1+abs(pp.z)*.02,6.0); // Explicit float
    h.x=max(h.x,pp.z);
    g+=0.1/(0.1+h.x*h.x*(4.0-3.9*animInv_local)); // Explicit float
    t=t.x<h.x?t:h;
    t.x*=0.8 * DnaDetailScale; // Applied DnaDetailScale to the final distance
    return t;
}
vec2 tr( vec3 ro,vec3 rd )
{
  vec2 h,t=vec2(0.1); // Explicit float
  for(int i=0;i<128;i++){
    h=mp(ro+rd*t.x);
    if(h.x<.0001||t.x>100.0) break; // Explicit float
    t.x+=h.x;t.y=h.y;
  }  
  if(t.x>100.0) t.y=0.0; // Explicit float
  return t;
}
#define a(d) clamp(mp(po+no*d).x/d,0.0,1.0) // Explicit float
#define s(d) smoothstep(0.0,1.0,mp(po+ld*d).x/d) // Explicit float
void main() {
    tt=mod(TIME * AnimationSpeed,62.82); // Apply AnimationSpeed
    vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.0); // Explicit float
    vec3 ro=mix(vec3(1.0),vec3(-1.0,2.0,2.0),ceil(sin(tt)))*vec3(cos(tt*.4+1.0)*10.0,14.0-cos(tt*.5)*5.0,-10.0), // Explicit float
    cw=normalize(vec3(0.0,10.0,0.0)-ro), // Explicit float
    cu=normalize(cross(cw,vec3(0.0,1.0,0.0))), // Explicit float
    cv=normalize(cross(cu,cw)),
    rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo; // Explicit float
    
    // Base background color and darkening factor from tunable parameters
    co=fo=vec3(BaseColorR, BaseColorG, BaseColorB) - length(uv)*vec3(DarkeningFactorR, DarkeningFactorG, DarkeningFactorB);
    
    ld=normalize(vec3(-.1,.5,.3));
    z=tr(ro,rd);t=z.x;
    if(z.y>0.){
        po=ro+rd*t;
        no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x);
        al=mix(vec3(.4,.1,.2),vec3(.1,.2,.4),sin(po.x*.5)*.5+.5)+tnoi*0.2; // Using po for spatial mix, further reduced tnoi influence

        if(z.y<5.0)al=vec3(0.0); // Explicit float
        if(z.y>5.0)al=vec3(1.0); // Explicit float
        float dif=max(0.0,dot(no,ld)), // Explicit float
        fr=pow(1.0+dot(no,rd),4.0); // Explicit float
        co=mix(al*(a(.05)*a(.1)+.2)*(dif+s(2.0)),fo,min(fr,.5)); // Explicit float
        co=mix(fo,co,exp(-.000005*t*t*t)); // Explicit float
    }  

    // Apply main glow intensity and color pulse
    co += MainGlowIntensity * (g * 0.2 * vec3(0.1, 0.2, 0.7) + gg * 0.2); // Explicit float
    co *= (1.0 + ColorPulseIntensity * sin(tt * ColorPulseFrequency)); // Apply color pulse effect

    gl_FragColor = vec4(pow(co,vec3(.45)),1.0); // Explicit float
}
