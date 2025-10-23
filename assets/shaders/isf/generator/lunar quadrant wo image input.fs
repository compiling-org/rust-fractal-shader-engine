/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Glitch"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/3tXcW8 by evvvvil. Lunar Quadrant Glitch - Influenced by the work of flopine and yx aka Luna. Now with tunable parameters for color, pulse, speed, morphing, and camera.",
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
            "NAME": "CameraZOffset",
            "TYPE": "float",
            "DEFAULT": 11.0,
            "MIN": 5.0,
            "MAX": 20.0,
            "LABEL": "Camera Z Offset"
        },
        {
            "NAME": "CameraSineAmplitude",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "LABEL": "Camera Sine Amplitude"
        },
        {
            "NAME": "CameraSineSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Camera Sine Speed"
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
        { "NAME": "BaseColorHueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Hue" },
        { "NAME": "BaseColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Sat" },
        { "NAME": "GlowColorHueShift", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Hue" },
        { "NAME": "GlowColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Sat" },
        {
            "NAME": "GlitchEffectStrength",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Glitch Effect Strength"
        },
        {
            "NAME": "MainGlowOverallIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Main Glow Intensity"
        }
    ]
}

*/


// Lunar Quadrant Glitch - Result of an improvised live code session on Twitch

// Influenced by the work of flopine and yx aka Luna, you ladies rock!

// LIVE SHADER CODING, SHADER SHOWDOWN STYLE, EVERY TUESDAYS 21:00 Uk time: 
// https://www.twitch.tv/evvvvil_

// "I've had more prime ministers than Madonna had number one hits." - Queen Elizabeth II

precision highp float; // Set high precision for floats
precision mediump sampler2D; // Set medium precision for sampler2D

vec2 z,v,e=vec2(.0035,-.0035),gpo=vec2(0.0),gl=vec2(0.0); // Explicit float initializers
float t,tt,b,bb,pi,g,gg,ti;
vec3 op,np,bp,cp,pp,po,no,al,ld;

// Declare uniforms explicitly for the ISF host
uniform float DarkeningFactorFranken;


// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w); // Corrected K.www to K.w
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float cx(vec3 p,vec3 r){return max(abs(length(p.yz)-r.x)-r.y,abs(p.x)-r.z);}
float cz(vec3 p,vec3 r){return max(abs(length(p.xy)-r.x)-r.y,abs(p.z)-r.z);}
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}

// texNoise function removed as there is no image input
/*
vec4 texNoise(vec2 uv_param){ // Renamed parameter to avoid conflict
    float f = 0.0; // Explicit float
    f+=texture2D(TEXTURE_0,mod(uv_param*.125,1.0)).r*.5;
    f+=texture2D(TEXTURE_0,mod(uv_param*.25,1.0)).r*.25;
    f+=texture2D(TEXTURE_0,mod(uv_param*.5,1.0)).r*.125;
    f+=texture2D(TEXTURE_0,mod(uv_param*1.0,1.0)).r*.125; // Explicit float
    f=pow(f,1.2);
    return vec4(f*.45+.05);
}
*/

float tick(float t_in){ // Renamed parameter to avoid conflict
  t_in=fract(t_in/40.0); // Explicit float
  t_in=smoothstep(0.0,1.0,t_in); // Explicit float
  t_in=pow(sin(t_in*2.0*pi),2.0); // Explicit float
  return t_in*2.0*pi; // Explicit float
}

vec2 fb( vec3 p, float mat, float tw) 
{
  pp=abs(p)-vec3(6.0,0.0,0.0); // Explicit floats
  vec2 h,t=vec2(length(pp)-1.9,6.0); // Explicit float

  // Replaced gl[int(mat)] with if/else if for constant array access
  if (int(mat) == 0) { 
      gl.x+=0.1/(0.1+t.x*t.x*20.0); // Access gl.x
  } else if (int(mat) == 1) {
      gl.y+=0.1/(0.1+t.x*t.x*20.0); // Access gl.y
  }

  h=vec2(cx(pp,vec3(2.0,0.5,0.5)),5.0); t=t.x<h.x?t:h; // Explicit floats
  h=vec2(length(p)-sin(tt*10.0*MorphingIntensity)*10.0*MorphingIntensity,6.0); // Apply MorphingIntensity, explicit float
  gl.y+=(0.1/(0.1+h.x*h.x*.1))*(gpo.x-.2)*-10.0; // Explicit float
  h=vec2(cx(pp,vec3(2.0,0.7,0.3)),6.0); t=t.x<h.x?t:h; // Explicit floats
  h=vec2(cx(pp,vec3(2.0,0.8,0.1)),3.0); t=t.x<h.x?t:h; // Explicit floats
  pp=abs(p)-vec3(0.0,2.0,0.0); // Explicit floats
  pp.xz*=r2(sin(pp.y*.12)*2.0 * MorphingIntensity); // Apply MorphingIntensity, explicit float
  h=vec2(cz(pp,vec3(6.0,0.5,0.5)),5.0);h.x*=0.6; t=t.x<h.x?t:h; // Explicit floats
  h=vec2(cz(pp,vec3(6.0,0.3,0.7)),6.0);h.x*=0.6; t=t.x<h.x?t:h; // Explicit floats
  h=vec2(cz(pp,vec3(6.0,0.1,0.8)),3.0);h.x*=0.6; t=t.x<h.x?t:h; // Explicit floats
  np=p;
  for(int i=0;i<8;i++){
    np=abs(np);
    np.xy*=r2(-.1);
  }
  h=vec2(length(np.xz)-.35,3.0); // Explicit float
  h.x=max(h.x,cz(pp,vec3(6.0,0.7,0.5))); // Explicit floats
  h.x=max(h.x,-(length(np.xz)-.2)); t=t.x<h.x?t:h;  
  h=vec2(length(np.xz),6.0); // Explicit float
  h.x=max(h.x,cz(pp+vec3(0.0,1.5,0.0),vec3(6.5,1.9,1.5)));  // Explicit floats
  
  // Replaced gl[int(mat)] with if/else if for constant array access
  if (int(mat) == 0) {
      gl.x+=0.1/(0.1+h.x/tw*h.x/tw*(40.0-39.0*abs(cos(pp.x*pp.y*.05-tt*4.0)))); // Access gl.x
  } else if (int(mat) == 1) {
      gl.y+=0.1/(0.1+h.x/tw*h.x/tw*(40.0-39.0*abs(cos(pp.x*pp.y*.05-tt*4.0)))); // Access gl.y
  }

  t=t.x<h.x?t:h; 
  return t;
}

vec2 mp( vec3 p ){ 
  op=p;
  vec4 np_local=vec4(p,1.0); // Explicit float, renamed to local
  float mat,bro;
  vec2 h,t=vec2(1000.0); // Explicit float
  for(int i=0;i<5;i++){     
    mat=mod(float(i),2.0); // Explicit float
    bro=cos(mat*pi);
    np_local.xy*=r2(ti*bro);
    np_local.yz*=r2(-ti*bro);
    h=fb(np_local.xyz,mat,np_local.w);
    h.x/=np_local.w; t=t.x<h.x?t:h;
    np_local*=2.0; // Explicit float
  }
  return t;
}

vec2 tr( vec3 ro, vec3 rd)
{
  vec2 h,t= vec2(.1);
  for(int i=0;i<128;i++){
    h=mp(ro+rd*t.x);
    if(h.x<.0001||t.x>30.0) break; // Explicit float
    t.x+=h.x;t.y=h.y;
  }
  if(t.x>30.0) t.y=0.0; // Explicit float
  return t;
}

#define a(d) clamp(mp(po+no*d).x/d,0.,1.)
#define s(d) smoothstep(0.,1.,mp(po+ld*d).x/d)

void main() {
  vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.0); // uv declared here
  pi=acos(-1.0); // Explicit float
  tt=mod(TIME * AnimationSpeed,62.83)+3.0; // Apply AnimationSpeed, explicit float
  ti=tick(tt);
  
  // Reset gl accumulators per frame
  gl = vec2(0.0, 0.0);

  gpo=vec2(ceil(sin((sin(tt*5.0*GlitchEffectStrength+cos(tt*12.0*GlitchEffectStrength)))))); // Apply GlitchEffectStrength, explicit float
  gpo.y=1.0-gpo.y; // Explicit float
  gpo*=0.2; // Explicit float
  if(cos(ti)<.7) gpo=vec2(0.2); 
  
  // Replaced tnoi_local with a simple time-based dynamic noise since TextureNoiseScale and image input are removed
  float tnoi_dynamic = sin(TIME * 3.0) * 0.1 + cos(TIME * 5.0) * 0.05;
  vec2 tnoi_local=vec2(tnoi_dynamic); 

  tnoi_local+=(ceil(sin(abs(uv.x)*500.0*gpo.x-tt*10.0))+ceil(sin(abs(uv.y)*500.0*gpo.y-tt*10.0)))*(gpo.x-gpo.y); // Explicit floats
  v=(gpo-.2)*.2*(tnoi_local); uv*=1.0-(v.y+v.x)*4.0; // Explicit floats
  
  vec3 ro=vec3(0.0,0.0,CameraZOffset+CameraSineAmplitude*cos(tt*CameraSineSpeed)), // Apply camera parameters, explicit floats
  cw=normalize(vec3(0.0)-ro),cu=normalize(cross(cw,vec3(0.0,1.0,0.0))),cv=normalize(cross(cu,cw)), // Explicit floats
  rd=mat3(cu,cv,cw)*normalize(vec3(uv,.7)),co,fo;
  ld=normalize(vec3(.1,.5,.3));
  
  // Base color from tunable parameters using HSV
  vec3 baseColor = hsv2rgb(vec3(BaseColorHueShift, BaseColorSaturation, 0.1));
  co=fo=baseColor-length(uv)*DarkeningFactorFranken-rd.y*.15; // Using DarkeningFactorFranken as DarkeningFactor for this shader

  z=tr(ro,rd);t=z.x;
  if(z.y>0.){
    po=ro+rd*t;
    no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x); 
    al=vec3(.5); 
    if(z.y<5.0) al=vec3(0.0); // Explicit float
    if(z.y>5.0) al=vec3(1.0); // Explicit float
    float dif=max(0.0,dot(no,ld)), // Explicit float
    fr=pow(1.0+dot(no,rd),4.0), // Explicit float
    sp=pow(max(dot(reflect(-ld,no),-rd),0.0),40.0); // Explicit float
    co=mix(sp+mix(vec3(.7),vec3(1.0),abs(rd))*al*(a(.1)*a(.3)+.2)*(dif+s(.5)),fo,min(fr,.5)); // Explicit float
    co=mix(fo,co,exp(-.0003*t*t*t));
  }
  
  // Glow colors from tunable parameters using HSV
  vec3 glowColor1 = hsv2rgb(vec3(GlowColorHueShift, GlowColorSaturation, 1.0));
  vec3 glowColor2 = hsv2rgb(vec3(mod(GlowColorHueShift + 0.3, 1.0), GlowColorSaturation, 1.0)); // A slightly shifted hue

  gl_FragColor = vec4(pow(co + MainGlowOverallIntensity * (gl.x*gpo.x*glowColor1 + gl.y*glowColor2*gpo.y),
                            vec3(.45))*(1.0 + ColorPulseIntensity * sin(tt * ColorPulseFrequency)),1.0); // Explicit float, apply main glow and color pulse
}
