/*{
    "CATEGORIES": ["Fractal", "Psychedelic", "Volumetric", "Glitch", "Abstract"],
    "DESCRIPTION": "An abstract, fluid glitch effect with amorphous volumetric distortions, emphasizing dynamic color shifts and pulsing, controlled by tunable parameters.",
    "ISFVSN": "2.0",
    "INPUTS": [
        { "NAME": "AnimationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "MorphingIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Morphing Intensity" },
        { "NAME": "ZoomLevel", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Camera Zoom" },
        { "NAME": "ColorPulseIntensity", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse Intensity" },
        { "NAME": "ColorPulseFrequency", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Color Pulse Frequency" },
        { "NAME": "BaseColorHueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Hue" },
        { "NAME": "BaseColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Sat" },
        { "NAME": "GlowColorHueShift", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Hue" },
        { "NAME": "GlowColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Sat" },
        { "NAME": "DistortionStrength", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Distortion Strength" },
        { "NAME": "MainGlowOverallIntensity", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 5.0, "LABEL": "Main Glow Intensity" },
        { "NAME": "RaymarchSteps", "TYPE": "float", "DEFAULT": 128.0, "MIN": 50.0, "MAX": 256.0, "LABEL": "Raymarch Steps" },
        { "NAME": "RaymarchBailout", "TYPE": "float", "DEFAULT": 30.0, "MIN": 10.0, "MAX": 100.0, "LABEL": "Raymarch Bailout" }
    ]
}
*/

precision highp float;
precision mediump sampler2D;

vec2 z,v,e=vec2(0.0035,-0.0035),gpo=vec2(0.0),gl=vec2(0.0);
float t,tt,b,bb,pi,g,gg,ti;
vec3 op,np,bp,cp,pp,po,no,al,ld;

// Define a constant for the maximum raymarch steps to satisfy GLSL ES 1.0 loop requirements
#define MAX_RAYMARCH_STEPS_CONST 256 // Max value from RaymarchSteps input

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}

float tick(float t_in){
  t_in=fract(t_in/40.0);
  t_in=smoothstep(0.0,1.0,t_in);
  t_in=pow(sin(t_in*2.0*pi),2.0);
  return t_in*2.0*pi;
}

// Modified fb function for a more fluid, amorphous shape
vec2 fb( vec3 p, float mat, float tw) 
{
  vec2 h,t_val=vec2(1000.0, 6.0); // Initialize with a large distance, renamed 't' to 't_val' to avoid conflict

  // Primary fluid shape: length(p) with time and distortion
  float blob_dist = length(p) - sin(p.x * 3.0 + TIME * 2.0) * 0.5 - sin(p.y * 4.0 + TIME * 1.5) * 0.5;
  h = vec2(blob_dist, 6.0);
  t_val = t_val.x < h.x ? t_val : h;

  // Add more dynamic distortions to the point p
  p.x += sin(p.y * 7.0 + TIME * 3.0) * DistortionStrength;
  p.y += cos(p.x * 8.0 + TIME * 2.5) * DistortionStrength;
  p.z += sin(p.x * p.y * 10.0 + TIME * 4.0) * DistortionStrength;

  // Secondary fractal-like detail
  float fractal_detail = length(p.xy) - sin(p.z * 10.0 + TIME * 5.0) * 0.1;
  h = vec2(fractal_detail, 3.0);
  t_val = t_val.x < h.x ? t_val : h;

  // Accumulate glow based on distance
  gl.x += 0.1 / (0.1 + t_val.x * t_val.x * 20.0); // General glow
  gl.y += 0.1 / (0.1 + h.x * h.x * 10.0); // Detail glow

  return t_val;
}

// Modified mp function for amorphous iterations
vec2 mp( vec3 p ){ 
  op=p;
  vec4 np_local=vec4(p,1.0); 
  float mat,bro;
  vec2 h,t_val=vec2(1000.0, 0.0); // Initialize with a large distance, renamed 't' to 't_val'

  for(int i=0;i<5;i++){     
    mat=mod(float(i),2.0); 
    bro=cos(mat*pi);
    
    // Apply rotation and scaling
    np_local.xy*=r2(ti*bro * 0.5); // Reduced rotation intensity
    np_local.yz*=r2(-ti*bro * 0.5); // Reduced rotation intensity
    
    h=fb(np_local.xyz,mat,np_local.w); // Pass np_local.w as tw
    h.x/=np_local.w; 
    t_val=t_val.x<h.x?t_val:h;
    
    np_local.xyz*=2.0; // Scale up for next iteration
    np_local.w*=2.0; // Scale w as well
  }
  return t_val;
}

vec2 tr( vec3 ro, vec3 rd)
{
  vec2 h,t_val= vec2(0.1); // Renamed 't' to 't_val'
  // FIX: Use MAX_RAYMARCH_STEPS_CONST for loop limit and break early with RaymarchSteps
  for(int i=0;i<MAX_RAYMARCH_STEPS_CONST;i++){ 
    if (float(i) >= RaymarchSteps) break; // Break if current iteration exceeds tunable steps
    h=mp(ro+rd*t_val.x);
    if(h.x<0.0001||t_val.x>RaymarchBailout) break; // Use RaymarchBailout input
    t_val.x+=h.x;t_val.y=h.y;
  }
  if(t_val.x>RaymarchBailout) t_val.y=0.0; // Use RaymarchBailout input
  return t_val;
}

#define a(d) clamp(mp(po+no*d).x/d,0.0,1.0)
#define s(d) smoothstep(0.0,1.0,mp(po+ld*d).x/d)

void main() {
  vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.0);
  pi=acos(-1.0);
  tt=mod(TIME * AnimationSpeed,62.83)+3.0;
  ti=tick(tt);
  
  // Reset gl accumulators per frame
  gl = vec2(0.0, 0.0);

  // Simplified gpo for a more fluid glitch pulse
  gpo.x = abs(sin(tt * 5.0)) * 0.2;
  gpo.y = abs(cos(tt * 7.0)) * 0.2;

  float tnoi_dynamic = sin(TIME * 3.0) * 0.1 + cos(TIME * 5.0) * 0.05;
  vec2 tnoi_local=vec2(tnoi_dynamic); 

  tnoi_local+=(ceil(sin(abs(uv.x)*500.0*gpo.x-tt*10.0))+ceil(sin(abs(uv.y)*500.0*gpo.y-tt*10.0)))*(gpo.x-gpo.y);
  v=(gpo-0.2)*0.2*(tnoi_local); uv*=1.0-(v.y+v.x)*4.0;
  
  // Fixed ray origin for a central view, controlled by ZoomLevel
  vec3 ro=vec3(0.0,0.0,-5.0 * ZoomLevel); 
  vec3 cw=normalize(vec3(0.0)-ro),cu=normalize(cross(cw,vec3(0.0,1.0,0.0))),cv=normalize(cross(cu,cw)),
  rd=mat3(cu,cv,cw)*normalize(vec3(uv,0.7)),co,fo; // Adjusted uv.z to 0.7 for initial perspective
  ld=normalize(vec3(0.1,0.5,0.3));
  
  // Base color from tunable parameters using HSV
  vec3 baseColor = hsv2rgb(vec3(BaseColorHueShift, BaseColorSaturation, 0.1));
  co=fo=baseColor-length(uv)*0.01; // Simplified darkening factor

  z=tr(ro,rd);t=z.x;
  if(z.y>0.0){
    po=ro+rd*t;
    no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x); 
    al=vec3(0.5); 
    if(z.y<5.0) al=vec3(0.0); 
    if(z.y>5.0) al=vec3(1.0); 
    float dif=max(0.0,dot(no,ld)),
    fr=pow(1.0+dot(no,rd),4.0),
    sp=pow(max(dot(reflect(-ld,no),-rd),0.0),40.0);
    co=mix(sp+mix(vec3(0.7),vec3(1.0),abs(rd))*al*(a(0.1)*a(0.3)+0.2)*(dif+s(0.5)),fo,min(fr,0.5));
    co=mix(fo,co,exp(-0.0003*t*t*t));
  }
  
  // Glow colors from tunable parameters using HSV
  vec3 glowColor1 = hsv2rgb(vec3(GlowColorHueShift, GlowColorSaturation, 1.0));
  vec3 glowColor2 = hsv2rgb(vec3(mod(GlowColorHueShift + 0.3, 1.0), GlowColorSaturation, 1.0)); // A slightly shifted hue

  gl_FragColor = vec4(pow(co + MainGlowOverallIntensity * (gl.x*gpo.x*glowColor1 + gl.y*glowColor2*gpo.y),
                            vec3(0.45))*(1.0 + ColorPulseIntensity * sin(tt * ColorPulseFrequency)),1.0);
}
