/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/WtfBzX by evvvvil. DNA tracer - Result of improvised live code session on Twitch. Now with tunable parameters for color, glow, and detail.",
    "IMPORTED": {
        "TEXTURE_0": {
            "NAME": "TEXTURE_0",
            "PATH": "79520a3d3a0f4d3caa440802ef4362e99d54e12b1392973e4ea321840970a88a.jpg"
        }
    },
    "INPUTS": [
        {
            "NAME": "ColorPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Color Pulse Intensity"
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
precision mediump sampler2D; // Set medium precision for sampler2D

vec2 z,v,e=vec2(.0035,-.0035);float t,tnoi,tt,b,bb,anim,animInv,g,gg;vec3 np,bp,pp,op,po,no,al,ld;

// Tunable Parameters are implicitly declared by the ISF host from the JSON header.
// No explicit 'uniform' declarations are needed here to avoid redefinition errors.

float bo(vec3 p,vec3 r){p=abs(p)-r; return max(max(p.x,p.y),p.z);}
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}
float smin( float d1, float d2, float k ){  float h = max(k-abs(d1-d2),0.0);return min(d1,d2)-h*h*0.25/k; }

// Using texture2D for TEXTURE_0 as IMG_NORM_PIXEL is not recognized
vec4 texNoise(vec2 uv){
    float f = 0.;
    f+=texture2D(TEXTURE_0,mod(uv*.125,1.0)).r*.5;
    f+=texture2D(TEXTURE_0,mod(uv*.25,1.0)).r*.25;
    f+=texture2D(TEXTURE_0,mod(uv*.5,1.0)).r*.125;
    f+=texture2D(TEXTURE_0,mod(uv*1.,1.0)).r*.125;
    f=pow(f,1.2);
    return vec4(f*.45+.05);
}

vec2 fb( vec3 p,float size )
{
  vec2 h,t=vec2(bo(p,vec3(4)),5);  
  t.x=abs(t.x)-.55;
  t.x=max(t.x,abs(p.y)-1.);  
  t.x=smin(t.x,0.4*length(abs(p.yz+tnoi*1.8)-vec2(0,4.*size))-.6*size,1.);
  h=vec2(bo(p,vec3(4)),3);  
  h.x=abs(h.x)-.3;
  h.x=max(h.x,abs(p.y)-1.5);    
  t=t.x<h.x?t:h;  
  h=vec2(bo(p,vec3(3.5)),6);  
  h.x=abs(h.x)-.7;
  h.x=max(h.x,abs(p.y)-.5);  
  h.x=max(h.x,abs(abs(abs(p.z)-2.)-1.)-.5);  
  h.x=max(h.x,abs(abs(abs(p.x)-2.)-1.)-.5);  
  t=t.x<h.x?t:h;  
  h=vec2(.6*length(p.yz)-.1+.0*sin(p.x)*.1,6);
  pp=op; pp.z=mod(pp.z-tt*20.+35.,100.)-50.;
  g+=0.1/(0.1+h.x*h.x*(40.-39.9*sin(pp.z*.05+.2*sin(pp.x*.3))));
  t=t.x<h.x?t:h;
  return t;
}
vec2 mp( vec3 p )
{
    np=bp=p;
    op=p;
    np.xy*=r2(sin(p.z*.15)*.3);  
    np.z=mod(np.z+tt*5.,24.)-12.; 
    anim = sin(tt*.5+op.z*.03);
    animInv = cos(tt*.5+op.z*.03);
    vec2 h,t=vec2(1000);
    for(int i=0;i<6;i++){
        np.xz=abs(np.xz)-vec2(4,6);
        np.xy*=r2(.17);
    } 
    tnoi=texNoise(vec2(np.x,dot(np.xz,vec2(.5)))*vec2(.15,.3)).r;
    t=fb(np,1.0 * DnaDetailScale); // Applied DnaDetailScale
    np.xz*=r2(.785*sin(np.z*.2));  
    h=fb(np.xzy,0.9 * DnaDetailScale); // Applied DnaDetailScale
    h=fb((np+vec3(4,-4.5,2.))*3.,0.3 * DnaDetailScale);h.x/=3.; // Applied DnaDetailScale
    t=t.x<h.x?t:h;
    np.xz*=r2(-.785); 
    h=fb((np+vec3(0,-5,1))*5.,0.0 * DnaDetailScale);h.x/=5.; // Applied DnaDetailScale
    t=t.x<h.x?t:h;
    bp=p-vec3(0,20,0);
    bp.xy*=r2(-animInv*2.5+tt);
    b=sin(bp.z*15.)*0.03;
    h=vec2(length(abs(bp.xy)-vec2(4,0))-.2-b,3);h.x*=0.8;
    t=t.x<h.x?t:h;
    bp.z=mod(bp.z-tt*20.,100.)-50.;
    h=vec2(length(bp.yz)-.1+abs(bp.x)*.03,6);
    gg+=0.1/(0.1+h.x*h.x*40.);
    t=t.x<h.x?t:h;
    pp=bp; bp.z=mod(bp.z+tt*30.,10.)-5.;
    h=vec2(length(abs(abs(bp.xy)-vec2(1,0))-vec2(.5,0))-.1+abs(pp.z)*.02,6);
    h.x=max(h.x,pp.z);
    g+=0.1/(0.1+h.x*h.x*(4.-3.9*animInv));
    t=t.x<h.x?t:h;
    t.x*=0.8 * DnaDetailScale; // Applied DnaDetailScale to the final distance
    return t;
}
vec2 tr( vec3 ro,vec3 rd )
{
  vec2 h,t=vec2(0.1);
  for(int i=0;i<128;i++){
    h=mp(ro+rd*t.x);
    if(h.x<.0001||t.x>100.) break;
    t.x+=h.x;t.y=h.y;
  }  
  if(t.x>100.) t.y=0.;
  return t;
}
#define a(d) clamp(mp(po+no*d).x/d,0.,1.)
#define s(d) smoothstep(0.,1.,mp(po+ld*d).x/d)
void main() {
  vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1);
  tt=mod(TIME,62.82);
  vec3 ro=mix(vec3(1),vec3(-1,2,2),ceil(sin(tt)))*vec3(cos(tt*.4+1.)*10.,14.-cos(tt*.5)*5.,-10.),
  cw=normalize(vec3(0,10,0)-ro),
  cu=normalize(cross(cw,vec3(0,1,0))),
  cv=normalize(cross(cu,cw)),
  rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo;
  
  // Base background color and darkening factor from tunable parameters
  co=fo=vec3(BaseColorR, BaseColorG, BaseColorB) - length(uv)*vec3(DarkeningFactorR, DarkeningFactorG, DarkeningFactorB);
  
  ld=normalize(vec3(-.1,.5,.3));
  z=tr(ro,rd);t=z.x;
  if(z.y>0.){
    po=ro+rd*t;
    no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x);
    al=mix(vec3(.4,.1,.2),vec3(.1,.2,.4),sin(np*.5)*.5+.5)+tnoi;
    if(z.y<5.)al=vec3(0);
    if(z.y>5.)al=vec3(1);
    float dif=max(0.,dot(no,ld)),
    fr=pow(1.+dot(no,rd),4.);
    co=mix(al*(a(.05)*a(.1)+.2)*(dif+s(2.)),fo,min(fr,.5));
    co=mix(fo,co,exp(-.000005*t*t*t));
  }  

  // Apply main glow intensity and color pulse
  co += MainGlowIntensity * (g * 0.2 * vec3(0.1, 0.2, 0.7) + gg * 0.2);
  co *= (1.0 + ColorPulseIntensity * sin(tt * 5.0)); // Apply color pulse effect

  gl_FragColor = vec4(pow(co,vec3(.45)),1);
}
