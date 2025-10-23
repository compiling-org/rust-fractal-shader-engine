/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Fractal",
        "Abstract",
        "Procedural"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/7lsSz8 by evvvvil. Dr Greeble's spawn filter - Result of an improvised live coding session on Twitch. Now with tunable parameters for color palette, color pulse, morphing, geometry, camera movement, and speed.",
    "IMPORTED": {},
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
            "NAME": "CameraOrbitFreqX",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Camera Orbit Freq X"
        },
        {
            "NAME": "CameraOrbitFreqY",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Camera Orbit Freq Y"
        },
        {
            "NAME": "CameraOrbitAmp",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 1.0,
            "MAX": 20.0,
            "LABEL": "Camera Orbit Amplitude"
        },
        {
            "NAME": "CameraPitchFreq",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Camera Pitch Freq"
        },
        {
            "NAME": "CameraStartDist",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 5.0,
            "MAX": 20.0,
            "LABEL": "Camera Start Distance"
        },
        {
            "NAME": "CameraLookAtY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Look At Y"
        },
        { "NAME": "BaseColorHue", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Hue" },
        { "NAME": "BaseColorSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Saturation" },
        { "NAME": "GlowColorHue", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Hue" },
        { "NAME": "GlowColorSaturation", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0, "LABEL": "Glow Color Saturation" },
        {
            "NAME": "BgDarkenFactorUV",
            "TYPE": "float",
            "DEFAULT": 0.12,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Background Darken UV"
        },
        {
            "NAME": "BgDarkenFactorRayY",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Background Darken Ray Y"
        },
        { "NAME": "NoiseDensity", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.01, "MAX": 0.1, "LABEL": "Noise Density" },
        {
            "NAME": "GreebleScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Greeble Scale"
        },
        {
            "NAME": "SynapseScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Synapse Scale"
        },
        {
            "NAME": "ParticleGlowFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Particle Glow Factor"
        },
        {
            "NAME": "VerticalGlowLineIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Vertical Glow Intensity"
        },
        {
            "NAME": "PostProcessGamma",
            "TYPE": "float",
            "DEFAULT": 0.55,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Output Gamma"
        }
    ]
}

*/

precision highp float;

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0); // Explicit float literals
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w); // Explicit float literals, K.www to K.w
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y); // Explicit float literals
}

// Global variables, now with better control and initialization
vec2 z_res,v_res,e_normal=vec2(0.00035,-0.00035); // Renamed `z, v` to `z_res, v_res` to avoid conflict with `z` used as float, renamed `e` to `e_normal`
float t_global,tt_animated,bb_val,g_accum,gg_accum,r_greeble=0.0; // Renamed `t,tt,bb,g,gg,r` for clarity and to indicate global scope, `r_greeble` for specific use
vec3 np_temp,bp_temp,pp_local_transformed,po_hit,no_normal,al_color,ld_light_dir,lp_light_pos,op_orig_pos,cp_cam_pos,rd_ray_dir;

// Math functions
float smin(float a,float b,float k){  float h=max(0.0,k-abs(a-b));return min(a,b)-h*h*0.25/k;} // Explicit float literals
float smax(float a,float b,float k){  float h=max(0.0,k-abs(-a-b));return max(-a,b)+h*h*0.25/k;} // Explicit float literals
vec2 smin( vec2 a, vec2 b,float k ){ float h=clamp(0.5+0.5*(b.x-a.x)/k,0.0,1.0);return mix(b,a,h)-k*h*(1.0-h);} // Explicit float literals
float bo(vec3 p,vec3 r_box){p=abs(p)-r_box;return max(max(p.x,p.y),p.z);} // Renamed r to r_box to avoid conflict
float cz(vec3 p,vec3 r_cyl){return max(abs(length(p.xy)-r_cyl.x)-r_cyl.y,abs(p.z)-r_cyl.z);} // Renamed r to r_cyl to avoid conflict
mat2 r2(float r_angle){return mat2(cos(r_angle),sin(r_angle),-sin(r_angle),cos(r_angle));} // Renamed r to r_angle

// Procedural noise functions (replacing iChannel0)
// A simple hash function by Inigo Quilez
float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p = fract((p + p) * p);
    return fract(p);
}

// 2D to 1D hash (for 2D noise)
float hash21(vec2 p) {
    vec3 p3 = fract(p.xyx * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// Simple 2D procedural noise (value noise)
float procedural_noise(vec2 uv) {
    vec2 i = floor(uv);
    vec2 f = fract(uv);

    f = f * f * (3.0 - 2.0 * f); // Smoothstep interpolation

    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x),
               f.y);
}


const mat2 deg45=mat2(0.70738827,0.706825,-0.706825,0.70738827); // Explicit float literals

// mp function - Main distance function
vec2 mp( vec3 p,float ga ) // `ga` now controls glow intensity factor
{ 
  op_orig_pos=pp_local_transformed=p; // Store original position, and initialize local transformed position
  
  // Splines position and transformation, influenced by GreebleScale
  pp_local_transformed=abs(pp_local_transformed)-vec3(0.0,0.0,(3.5-cos(p.y*0.2)*2.0) * GreebleScale); // Explicit float literals, tunable GreebleScale
  pp_local_transformed.xy*=r2(-0.785); // Explicit float literal
  
  // Procedural noise sample (replaces texNoise from iChannel0)
  float tn=procedural_noise(op_orig_pos.xz*NoiseDensity); // Tunable NoiseDensity
  
  r_greeble=1.0; vec4 np_kifs=vec4(pp_local_transformed,1.0); // Explicit float literals, `r_greeble` used as temp var
  for(int i=0;i<4;i++){ //Greeble Displacement KIFS
    np_kifs.x=abs(np_kifs.x)-1.4; // Explicit float literal
    np_kifs.xy*=deg45;
    np_kifs*=2.0; // Explicit float literal
    r_greeble=min(r_greeble,clamp(sin(np_kifs.y)*sin(np_kifs.x*3.0),-0.3,0.3)/np_kifs.w); // Explicit float literals
  }
  r_greeble/=2.0; // Explicit float literal
  
  // Synapse Splines and Sphere, influenced by SynapseScale
  vec2 h_dist,t_dist=vec2(0.8*(length(pp_local_transformed.xz)-0.2+r_greeble),0.0); // Explicit float literals, tunable SynapseScale
  vec3 sp_synapse=pp_local_transformed;
  
  // sp_synapse.xz*=r2(cos(sp_synapse.y)*.3+tt_animated); // Original commented out, keep commented for now
  
  sp_synapse.xz=abs(sp_synapse.xz)-0.4-cos(sp_synapse.y*0.5)*0.5-sp_synapse.y*0.05; // Explicit float literals
  vec3 pinch_val=cos(p*2.5)*0.2; // Explicit float literals
  
  t_dist.x=smin(t_dist.x,0.6*(length(p)-3.0+pinch_val.x+pinch_val.y+r_greeble) * SynapseScale,0.5); // SYNAPSE SPHERE, tunable SynapseScale
  t_dist.x=smin(t_dist.x,0.7*(cz(p,vec3(4.0+(pinch_val.x+pinch_val.y)*0.5,0.15,0.2))) * SynapseScale,1.3); // SYNAPSE CYL, tunable SynapseScale
  t_dist.x=smax((length(p.xy)-1.0-r_greeble)*0.8,t_dist.x,0.5); // Explicit float literals
  
  float synapse_val=t_dist.x-0.2; // BLACK SYNAPSE, explicit float literal
  h_dist=vec2(synapse_val,1.0); // Explicit float literal
  
  h_dist.x=max(h_dist.x,0.7*abs(abs(abs(pp_local_transformed.y-4.9)-1.5-r_greeble)-1.5)-0.5); //CUT BLACK OUTTER, explicit float literals
  h_dist.x=smin(h_dist.x,max(0.7*(cz(p,vec3(3.2,1.5-r_greeble,0.2-r_greeble))),-(abs(p.z)-0.2)),0.5); //BLACK CYL AROUND, explicit float literals
  h_dist.x=smin(h_dist.x,max(0.7*(length(sp_synapse.xz)-0.15-r_greeble),-(abs(sp_synapse.y)-1.7)),0.2); //SMALLER SPLINES, explicit float literals
  h_dist.x=min(h_dist.x,bo(abs(op_orig_pos)-11.0-r_greeble,vec3(1.0,100.0,2.0))); //OUTTER BOXES, explicit float literals
  
  vec3 rp_temp=op_orig_pos+vec3(0.0,5.0,0.0);rp_temp.x=abs(rp_temp.x)-17.0; // Explicit float literals
  h_dist.x=min(h_dist.x,bo(rp_temp-r_greeble,vec3(1.0,7.0,2.0))); //OUTTER BOXES, explicit float literals
  
  h_dist.x=smin(h_dist.x,(op_orig_pos.y+7.0+tn*2.0+r_greeble)*0.6,0.5); //TERRAIN, explicit float literals
  
  pp_local_transformed=p*0.5; pp_local_transformed.y-=tt_animated; // PARTICLES, explicit float literals
  bb_val=cos(clamp(op_orig_pos.y*0.9-0.9+tn*3.0,-10.0,1.0)); // Explicit float literals
  pp_local_transformed.xz*=r2(bb_val*0.7); // Explicit float literal
  
  float bro_val=max((0.7-abs(bb_val)*0.5)*(length(sin(pp_local_transformed))+clamp(p.y-1.0,0.0,2.0)*0.05),length(p.xz)-10.0); // Explicit float literals
  gg_accum+=0.1/(0.1*bro_val*bro_val*(2000.0-1940.0*bb_val))*ga*ParticleGlowFactor; // PARTICLE GLOW, tunable ParticleGlowFactor
  
  t_dist=smin(t_dist,h_dist,0.1); // Explicit float literal
  
  float cylglo_val=max(length(p.xy)-4.7,abs(p.z)-0.05); //GLOW CYL AROUND, explicit float literals
  v_res=sin((p.xy-tt_animated*vec2(-1.0,0.5))*15.0)*0.03; // Explicit float literals
  vec3 ep_val=abs(p)-vec3(1.7-cos(p.y*0.5-2.5)-v_res.x+v_res.y,0.0,0.0); //VERTICAL GLOW LINES POS, explicit float literals
  ep_val.x=abs(ep_val.x)-0.3; // Explicit float literal
  cylglo_val=min(cylglo_val,0.7*length(ep_val.xz)); //VERTICAL GLOW LINES, explicit float literal
  gg_accum+=0.1/(0.1*cylglo_val*cylglo_val*200.0)*ga*max(0.0,1.0-p.y*0.1)*VerticalGlowLineIntensity;  //GLOW VERTICAL LINES + CYL GLO, tunable VerticalGlowLineIntensity
  h_dist=vec2(cylglo_val,6.0); // Explicit float literal
  h_dist.x=smin(h_dist.x,bro_val,0.4); //PARTICLES ADDED AT END, explicit float literal
  t_dist=t_dist.x<h_dist.x?t_dist:h_dist; 
  return t_dist;
}

// tr function - Raymarching loop
vec2 tr( vec3 ro_ray,vec3 rd_ray ) // Renamed parameters
{
  vec2 h_ray,t_ray=vec2(0.1); // Explicit float literal
  for(int i=0;i<128;i++){ // Fixed loop iterations
    h_ray=mp(ro_ray+rd_ray*t_ray.x,1.0); // Explicit float literal for ga
    if(h_ray.x<0.0001||t_ray.x>30.0) break; // Explicit float literals
    t_ray.x+=h_ray.x;t_ray.y=h_ray.y;
  }
  if(t_ray.x>30.0) t_ray.y=-1.0; // Explicit float literal
	return t_ray;
}

// Helper macros for material properties and lighting
#define a_macro(d_macro) clamp(mp(po_hit+no_normal*d_macro,0.0).x/d_macro,0.0,1.0) // Explicit float literal
#define s_macro(d_macro) smoothstep(0.0,1.0,mp(po_hit+ld_light_dir*d_macro,0.0).x/d_macro) // Explicit float literals

void main() {
  // Reset global glow accumulators per frame
  g_accum = 0.0; 
  gg_accum = 0.0; 

  vec2 uv_coords=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.0); // Explicit float literal
  tt_animated=7.86+mod(TIME*AnimationSpeed,62.82); // Tunable AnimationSpeed, explicit float literal
  
  // Camera position (ro) and direction (rd), influenced by tunable parameters
  cp_cam_pos=mix(vec3(cos(tt_animated*CameraOrbitFreqX)*CameraOrbitAmp,cos(tt_animated*CameraOrbitFreqY)*5.0,sin(tt_animated*CameraOrbitFreqX)*CameraOrbitAmp), // Tunable CameraOrbit parameters, explicit float literals
          vec3(cos(tt_animated*CameraPitchFreq)*5.0,CameraLookAtY,cos(tt_animated*CameraOrbitFreqY)*10.0),ceil(sin(tt_animated*CameraPitchFreq*0.4))); // Tunable CameraPitchFreq, CameraLookAtY
  cp_cam_pos.z += CameraStartDist; // Tunable CameraStartDist

  vec3 cw_cam=normalize(vec3(CameraLookAtY)-cp_cam_pos); // Tunable CameraLookAtY
  vec3 cu_cam=normalize(cross(cw_cam,vec3(0.0,1.0,0.0))); // Explicit float literal
  vec3 cv_cam=normalize(cross(cu_cam,cw_cam));
  rd_ray_dir=mat3(cu_cam,cv_cam,cw_cam)*normalize(vec3(uv_coords,0.5)); // Explicit float literal for rd.z
  
  // Base and background colors from tunable parameters (HSV converted to RGB)
  vec3 base_rgb = hsv2rgb(vec3(BaseColorHue, BaseColorSaturation, 0.1));
  vec3 fo_final_base=clamp(base_rgb-length(uv_coords)*BgDarkenFactorUV-rd_ray_dir.y*BgDarkenFactorRayY,0.0,1.0); // Tunable BgDarkenFactorUV, BgDarkenFactorRayY

  // Raymarch the scene
  z_res=tr(cp_cam_pos,rd_ray_dir);t_global=z_res.x; // `z_res` is the result of tr (distance, material_id), `t_global` is ray distance
  
  vec3 final_color_accum = fo_final_base; // Initialize final color with background
  
  if(z_res.y>-1.0){ // Check if ray hit something (material ID > -1)
    po_hit=cp_cam_pos+rd_ray_dir*t_global; // Calculate hit position
    no_normal=normalize(e_normal.xyy*mp(e_normal.xyy+po_hit,0.0).x+e_normal.yyx*mp(e_normal.yyx+po_hit,0.0).x+e_normal.yxy*mp(e_normal.yxy+po_hit,0.0).x+e_normal.xxx*mp(e_normal.xxx+po_hit,0.0).x); // Calculate normal
    
    // Albedo color with color pulse from `r_greeble` and `z_res.y` (material ID)
    vec3 morph_glow_rgb = hsv2rgb(vec3(GlowColorHue, GlowColorSaturation, 1.0));
    al_color=clamp(mix(mix(vec3(0.4,0.6,0.8),morph_glow_rgb,abs(r_greeble*20.0)-1.5),vec3(-1.0)-sin(r_greeble*50.0)*2.0,z_res.y),0.0,1.0); // Explicit float literals, tunable GlowColor
    
    if(z_res.y>5.0) al_color=vec3(1.0); // Explicit float literal for vec3
    
    lp_light_pos=cp_cam_pos; // Light position same as camera for some effects
    ld_light_dir=normalize(lp_light_pos-po_hit); // Light direction
    
    float dif=max(0.0,dot(no_normal,ld_light_dir)); // Diffuse light
    float fr=pow(1.0+dot(no_normal,rd_ray_dir),4.0); // Fresnel factor
    float attn=1.0-pow(min(1.0,length(lp_light_pos-po_hit)/15.0),4.0); // Attenuation, explicit float literals
    float sp=pow(max(dot(reflect(-ld_light_dir,no_normal),-rd_ray_dir),0.0),30.0); // Specular, explicit float literal
    
    // Final color calculation for hit surface
    final_color_accum=attn*mix(0.5*sp+al_color*(a_macro(0.15)*a_macro(0.3)+0.2)*(dif+s_macro(1.0)*0.5),fo_final_base,fr); // Explicit float literals
    final_color_accum=mix(fo_final_base,final_color_accum,exp(-0.0001*t_global*t_global*t_global)); // Explicit float literal
  }	
  
  // Apply accumulated glow from mp function
  vec3 glow_rgb = hsv2rgb(vec3(GlowColorHue, GlowColorSaturation, 1.0)); // Tunable GlowColor
  final_color_accum = pow(final_color_accum+g_accum*0.2*glow_rgb+gg_accum*0.2*glow_rgb,vec3(PostProcessGamma)); // Tunable PostProcessGamma, explicit float literal, multiply glow by `glow_rgb`

  gl_FragColor = vec4(final_color_accum,1.0); // Final output
}
