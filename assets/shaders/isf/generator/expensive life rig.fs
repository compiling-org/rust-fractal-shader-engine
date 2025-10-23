/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Tunable",
        "Procedural",
        "Organic",
        "Abstract"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/7sc3W4 by evvvvil. Expensive life rig - Result of an improvised live coding session on Twitch. Now with tunable parameters for animation, camera, colors, and geometry.",
    "IMPORTED": {},
    "INPUTS": [
        { "NAME": "AnimationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "HeartBeatIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Heartbeat Intensity" },
        { "NAME": "CameraOrbitSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.05, "MAX": 0.5, "LABEL": "Camera Orbit Speed" },
        { "NAME": "CameraOrbitAmplitude", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 15.0, "LABEL": "Camera Orbit Amplitude" },
        { "NAME": "CameraAltitude", "TYPE": "float", "DEFAULT": -4.0, "MIN": -10.0, "MAX": 0.0, "LABEL": "Camera Altitude" },
        { "NAME": "CameraZOffset", "TYPE": "float", "DEFAULT": 10.0, "MIN": 5.0, "MAX": 20.0, "LABEL": "Camera Z Offset" },
        { "NAME": "BackgroundScanlineIntensity", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.0, "MAX": 0.02, "LABEL": "Background Scanlines" },
        { "NAME": "BaseColorHue", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Hue" },
        { "NAME": "BaseColorSaturation", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Color Saturation" },
        { "NAME": "RedGlowHue", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0, "LABEL": "Red Glow Hue" },
        { "NAME": "RedGlowSaturation", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "LABEL": "Red Glow Saturation" },
        { "NAME": "BlueGlowHue", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Blue Glow Hue" },
        { "NAME": "BlueGlowSaturation", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0, "LABEL": "Blue Glow Saturation" },
        { "NAME": "BoneSmoothness", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5, "LABEL": "Bone Smoothness" },
        { "NAME": "TorsoShapeFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "LABEL": "Torso Shape Factor" },
        { "NAME": "NoiseScale", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.01, "MAX": 0.2, "LABEL": "Noise Scale" },
        { "NAME": "OutputGamma", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 2.0, "LABEL": "Output Gamma" }
    ]
}

*/

precision highp float;

// Tunable parameters (Uniforms are implicitly defined by the INPUTS block)
// float AnimationSpeed;
// float HeartBeatIntensity;
// float CameraOrbitSpeed;
// float CameraOrbitAmplitude;
// float CameraAltitude;
// float CameraZOffset;
// float BackgroundScanlineIntensity;
// float BaseColorHue;
// float BaseColorSaturation;
// float RedGlowHue;
// float RedGlowSaturation;
// float BlueGlowHue;
// float BlueGlowSaturation;
// float BoneSmoothness;
// float TorsoShapeFactor;
// float NoiseScale;
// float OutputGamma;

// Global variables, renamed for clarity and to avoid conflicts
vec2 z_hit_info, v_temp_vec, e_epsilon = vec2(0.0035,-0.0035);
float t_distance, tt_animated_time, tn_noise_val, nails_dist, b_bend_factor, g_red_glow_accum, gg_blue_glow_accum, heartBeat_val;
vec3 np_normal_calc_pos, bp_bone_pos, pp_transformed_pos, po_ray_hit_pos, no_hit_normal, al_albedo_color, ld_light_direction, lp_light_position, op_original_pos, rd_ray_dir; // rd_ray_dir added to globals

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// CHEAP BOX FUNCTION
float bo(vec3 p, vec3 r){p=abs(p)-r;return max(max(p.x,p.y),p.z);} 
// ROTATE FUNCTION
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));} 

// Procedural noise functions (replacing iChannel0 texture)
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

    f = f * f * (3.0 - 2.0 * f); // Smoothstep interpolation for smoother transitions

    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x),
               f.y);
}

// SMOOTH MIN FUNCTION
float smin(float a,float b,float k){  float h=max(0.,k-abs(a-b));return min(a,b)-h*h*.25/k;}
// SMOOTH MAX FUNCTION
float smax(float a,float b,float k){  float h=max(0.,k-abs(-a-b));return max(-a,b)+h*h*.25/k;}
// VECTOR2 SMOOTH MIN FUNCTION
vec2 smin( vec2 a, vec2 b,float k ){ float h=clamp(.5+.5*(b.x-a.x)/k,.0,1.);return mix(b,a,h)-k*h*(1.0-h);}             
// CAPSULE (Capped Cylinder) FUNCTION
float cap( vec3 p, float h, float r ){ p.y -= clamp( p.y, 0.0, h );return length( p ) - r;}

// HAND SHAPE FUNCTION
float hand(vec3 p){  
  float t_hand=length(p+vec3(0.,.25,0.))-0.25;
  pp_transformed_pos=p;
  pp_transformed_pos.x=abs(abs(pp_transformed_pos.x)-.22)-.11;
  pp_transformed_pos.z-=sin(pp_transformed_pos.y*.6+2.5)*max(pp_transformed_pos.y*.4,0.);
  float fingers =max(length(pp_transformed_pos.xz)-.1,abs(p.y-.5)-.5);
  t_hand=smin(t_hand,fingers,.5);
  nails_dist=max(length(pp_transformed_pos.xz)-.01,abs(pp_transformed_pos.y)-2.5);  
  t_hand=min(t_hand,0.7*nails_dist);
  return t_hand;  
}

// BONE FUNCTION (modified from original for better tunable control)
// This function warps the input point 'p' and returns the distance to a bone segment.
// 'rot' applies a base rotation, 'r' defines height, and 'z' affects secondary rotation.
float honk(inout vec3 p,mat2 rot,vec3 r_bone){ 
  p.y-=r_bone.y*r_bone.z; // Apply offset based on bone height and z-component
  p.xy*=rot; // Apply base rotation
  
  // Secondary YZ rotation, influenced by TIME and AnimationSpeed
  p.yz*=r2(mix(0.,-.48,cos(tt_animated_time*.4 * AnimationSpeed)*.5+.5)); // Fixed tt_animated_time
  // Secondary XZ rotation, influenced by TIME and AnimationSpeed, and 'r_bone.z' for variation
  p.xz*=r2(sin(tt_animated_time * AnimationSpeed)*.5*(1.-r_bone.z)); // Fixed tt_animated_time
  
  // Greeble-like displacement on the bone surface
  float gr=clamp(sin(p.y*5.),-.25,.25)*.1;
  pp_transformed_pos=p;
  pp_transformed_pos.xy*=r2(sin(p.y));
  gr=min(gr,clamp(sin(pp_transformed_pos.y*20.),-.25,.25)*.1);
  
  // Distance to the bone capsule, with tunable BoneSmoothness and greeble displacement
  float t_bone=0.6*cap(p,1.5,0.3-sin(p.y*2.5+1.5)*.1 - gr * BoneSmoothness); // Apply BoneSmoothness
  p.y-=r_bone.y; // Restore Y position for next segment in chain (if any)
  return t_bone;
}

// mp function - Main distance function for the entire rig
vec2 mp( vec3 p ,float ga){  // `ga` is a glow intensity accumulation factor
  op_original_pos=p; // Store original position for terrain

  // Pulsating body effect, influenced by Time
  p.xz*=1.+(smoothstep(0.,1.,cos(length(p*2.)-tt_animated_time*3.*AnimationSpeed)*.5+.5)*2.)*0.2*max(0.,(1.-abs(p.x*.4))); // Fixed tt_animated_time
  
  b_bend_factor=sin(p.y*1.5)*.2; // Bending factor based on Y position

  // Procedural noise sample for various distortions
  tn_noise_val=procedural_noise(p.xy*NoiseScale); // Tunable NoiseScale
  
  np_normal_calc_pos=p; // Position for normal calculation
  pp_transformed_pos=p-vec3(-1.,0.,0.); // Offset for first arm

  // First arm calculation
  vec2 h_material_mix,t_combined_dist=vec2(honk(pp_transformed_pos,r2(1.6+sin(tt_animated_time*.2*AnimationSpeed)*.75),vec3(0.,1.,0.)),0.); // Fixed tt_animated_time

  bp_bone_pos=p;
  bp_bone_pos.xz=abs(bp_bone_pos.xz)-.5-sin(p.y*.5)*.4;   
  vec3 rp_temp=np_normal_calc_pos; rp_temp.y-=2.3;  
  rp_temp=abs(rp_temp-vec3(0.,-1.,0.))-vec3(.2+b_bend_factor,1.,.2+b_bend_factor);
  h_material_mix=vec2(.7*(length(rp_temp)-.1-tn_noise_val*.5),1.); // SMALL BLACK BALLS
  
  // Second arm calculation
  float arm2_dist=honk(pp_transformed_pos,r2(sin(tt_animated_time*.1*AnimationSpeed)),vec3(0.,1.,1.)); // Fixed tt_animated_time
  t_combined_dist.x=smin(t_combined_dist.x,arm2_dist,.2); 
  h_material_mix.x=smin(h_material_mix.x,max((arm2_dist-.1),abs(abs(pp_transformed_pos.y)-.15)-.05),.1);
  t_combined_dist.x=smin(t_combined_dist.x,hand(pp_transformed_pos-vec3(0.,1.,0.)),.1); // Hand attached to arm
  gg_blue_glow_accum+=0.1/(0.1+nails_dist*nails_dist*40.); // Accumulate blue glow for nails

  pp_transformed_pos=p-vec3(1.,0.,0.); // Offset for the other arm
  t_combined_dist.x=min(t_combined_dist.x,honk(pp_transformed_pos,r2(sin(tt_animated_time*.2*AnimationSpeed)*.5-1.85),vec3(0.,1.,0.))); // FIRST ARM, Fixed tt_animated_time
  arm2_dist=honk(pp_transformed_pos,r2(sin(tt_animated_time*.1*AnimationSpeed)),vec3(0.,1.,1.)); //SECOND ARM ADDED TO WHITE MATERIAL, Fixed tt_animated_time
  t_combined_dist.x=smin(t_combined_dist.x,arm2_dist,0.1); //SECOND ARM ADDED TO WHITE MATERIAL
  h_material_mix.x=smin(h_material_mix.x,max((arm2_dist-.1),abs(abs(pp_transformed_pos.y)-.15)-.05),.2); //SECOND ARM REUSED IN BLACK MATERIAL AND CUT
  t_combined_dist.x=smin(t_combined_dist.x,hand(pp_transformed_pos-vec3(0.,1.,0.)),.1);   //HAND
  gg_blue_glow_accum+=0.1/(0.1+nails_dist*nails_dist*40.); // Accumulate blue glow for nails
  
  // Torso shape, influenced by TorsoShapeFactor
  float torso_dist= bo(p-vec3(0.,-1.2,0.),vec3(.6)); 
  torso_dist=smax(abs(abs(abs(p.y)-1.)-0.3)-0.2 * TorsoShapeFactor,torso_dist,0.5); // Apply TorsoShapeFactor
  t_combined_dist.x=smin(t_combined_dist.x,0.7*torso_dist,0.5); //BOX / TORSO
  t_combined_dist.x=smax(length(p.xy)-1.2,t_combined_dist.x,.5);

  t_combined_dist.x=smin(t_combined_dist.x,0.7*(length(bp_bone_pos.xz)-.1),1.); //SPLINES VERTICAL  
  pp_transformed_pos.xz*=r2(sin(p.y*5.)*.3);
  bp_bone_pos.xz=abs(bp_bone_pos.xz)-.2-b_bend_factor;  
  float splines_dist=length(bp_bone_pos.xz)-sin(p.y*50.)*.005-tn_noise_val*.2;
  h_material_mix.x=smin(h_material_mix.x,0.5*splines_dist,.1); //SPLINES VERTICAL AGAIN
  h_material_mix.x=smin(h_material_mix.x,0.5*(length(abs(np_normal_calc_pos.xz)-.6+sin(p.y*1.5-.2)*.5)-.01-tn_noise_val*.2),.1); //SPLINES VERTICAL AGAIN AGAIN!
  
  // Terrain
  float ter_dist=op_original_pos.y+5.+tn_noise_val-sin(length(op_original_pos))*.7;  //TERRAIN
  ter_dist=smax((length(op_original_pos.xz)-3.-tn_noise_val),ter_dist,1.5); //CUT HOLE IN TERRAIN
  ter_dist=smin(ter_dist,length(abs(op_original_pos.xz)-7.+tn_noise_val*.5-cos(op_original_pos.y*.5)*1.5)-1.,2.); //ADD SIDE FEATURES
  h_material_mix.x=min(h_material_mix.x,0.6*ter_dist);

  // Core glow
  float glo_dist=0.8*(length(p.xz)-.1); //CORE  
  np_normal_calc_pos=p-vec3(0.,-0.5-heartBeat_val*0.5*HeartBeatIntensity,0.); // Tunable HeartBeatIntensity
  np_normal_calc_pos.z=abs(np_normal_calc_pos.z)-mix(0.4,3.,heartBeat_val);
  float heart_dist=length(np_normal_calc_pos)-.8+heartBeat_val*.3*HeartBeatIntensity+cos(np_normal_calc_pos.y*4.+1.)*.2*abs(sin(np_normal_calc_pos.x*4.)); //HEART, Tunable HeartBeatIntensity
  glo_dist=smin(glo_dist,0.4*heart_dist,0.1);        //HEART ADED TO GLOW 
  glo_dist=smin(glo_dist,0.55*(length(rp_temp+vec3(-.0,.1,-.36))-.05),.1);  
  g_red_glow_accum+=0.1/(0.1+glo_dist*glo_dist*(60.-50.*heartBeat_val*HeartBeatIntensity*sin(p.y*.4-tt_animated_time*6.)))*ga; // Fixed tt_animated_time, Tunable HeartBeatIntensity
  t_combined_dist.x=smin(t_combined_dist.x,glo_dist,.1);
  t_combined_dist=smin(t_combined_dist,h_material_mix,.2);  

  // Blue core glow
  glo_dist=length(bp_bone_pos.xz+tn_noise_val*.5); //BLUE CORE
  glo_dist=min(glo_dist,0.5*(length(cos((pp_transformed_pos-vec3(0.,4.,0.))*.1))+.05)); //BLUE SIN LINES AT BACK
  gg_blue_glow_accum+=0.1/(0.1+glo_dist*glo_dist*50.)*ga;
  h_material_mix=vec2(glo_dist,2.);
  t_combined_dist=t_combined_dist.x<h_material_mix.x?t_combined_dist:h_material_mix;
	return t_combined_dist;
}

// tr function - Raymarching loop
vec2 tr( vec3 ro_ray, vec3 rd_ray ) //RAYMARCHING LOOP
{
  vec2 h_step_info,t_ray_state=vec2(.1); //NEAR PLANE
  for(int i=0;i<128;i++){ //LOOP MAX 128 STEPS
    h_step_info=mp(ro_ray+rd_ray*t_ray_state.x,1.); //GET DISTANCE TO GEOM
    if(h_step_info.x<.0001||t_ray_state.x>50.) break; //IF WE CLOSE ENOUGH OR IF WE TOO FAR, BREAK
    t_ray_state.x+=h_step_info.x; //BIG JUMP TO GEOMETRY IN NEXT ITERATION
    t_ray_state.y=h_step_info.y; //REMEMBER MATERIAL ID
  }  
  if(t_ray_state.x>50.) t_ray_state.y=-1.;//IF WE TOO FAR RETURN -1 MAT ID
  return t_ray_state;
}

// Helper macros for material properties and lighting
#define a_attenuation(d) clamp(mp(po_ray_hit_pos+no_hit_normal*d,0.).x,0.,1.)
#define s_smoothstep_factor(d) smoothstep(0.,1.,mp(po_ray_hit_pos+ld_light_direction*d,0.).x)

void main() {
  // Reset global glow accumulators per frame
  g_red_glow_accum = 0.0; 
  gg_blue_glow_accum = 0.0; 

  vec2 uv=(gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1.);   //GET UVS  
  tt_animated_time=mod(TIME * AnimationSpeed,62.83)+23.;  //MOD TIME TO AVOID ARTIFACT, tunable AnimationSpeed
  heartBeat_val=smoothstep(0.,1.,cos(-tt_animated_time*3.)*.5+.5); // Heartbeat animation

  // Camera position setup, influenced by tunable parameters
  vec3 ro_camera_pos=mix(vec3(cos(tt_animated_time*CameraOrbitSpeed)*CameraOrbitAmplitude,CameraAltitude,sin(tt_animated_time*CameraOrbitSpeed)*CameraZOffset), // Tunable CameraOrbit, CameraAltitude, CameraZOffset
                      vec3(cos(tt_animated_time*CameraOrbitSpeed)*4.,-sin(tt_animated_time*.4*AnimationSpeed)*5.,3.),sin(tt_animated_time*.2*AnimationSpeed)*.5+.5); // Tunable AnimationSpeed

  // Camera orientation
  vec3 cw_camera_world=normalize(vec3(0.)-ro_camera_pos); // Camera look at origin
  vec3 cu_camera_up=normalize(cross(cw_camera_world,vec3(0.,1.,0.))); // Camera "up" vector
  vec3 cv_camera_right=normalize(cross(cu_camera_up,cw_camera_world)); // Camera "right" vector

  // Ray direction from camera
  rd_ray_dir=mat3(cu_camera_up,cv_camera_right,cw_camera_world)*normalize(vec3(uv,0.5)); // rd_ray_dir is now globally declared
  
  // Background color, influenced by tunable parameters
  vec3 base_rgb = hsv2rgb(vec3(BaseColorHue, BaseColorSaturation, 0.15)); // Base color
  vec3 red_glow_rgb = hsv2rgb(vec3(RedGlowHue, RedGlowSaturation, 1.0)); // Red glow color
  vec3 blue_glow_rgb = hsv2rgb(vec3(BlueGlowHue, BlueGlowSaturation, 1.0)); // Blue glow color

  vec3 co_final_color = base_rgb-length(uv)*0.1-rd_ray_dir.y*0.2+sin(rd_ray_dir.x*150.*rd_ray_dir.y)*BackgroundScanlineIntensity; // Background with scanlines, tunable BackgroundScanlineIntensity
  vec3 fo_fallback_color = co_final_color; // Fallback color if no hit

  z_hit_info=tr(ro_camera_pos,rd_ray_dir); // Raymarch the scene. z_hit_info is vec2, tr returns vec2. This is correct.
  t_distance=z_hit_info.x;  

  if(z_hit_info.y>-1.){ //IF WE HIT SOMETHING THEN DO LIGHTING
    lp_light_position=ro_camera_pos+vec3(0.,2.,0.); // Light position
    po_ray_hit_pos=ro_camera_pos+rd_ray_dir*t_distance; // Ray hit position
    ld_light_direction=normalize(lp_light_position-po_ray_hit_pos); // Light direction
    
    // Calculate surface normal
    no_hit_normal=normalize(e_epsilon.xyy*mp(po_ray_hit_pos+e_epsilon.xyy,0.).x+e_epsilon.yyx*mp(po_ray_hit_pos+e_epsilon.yyx,0.).x+e_epsilon.yxy*mp(po_ray_hit_pos+e_epsilon.yxy,0.).x+e_epsilon.xxx*mp(po_ray_hit_pos+e_epsilon.xxx,0.).x);
    
    // Albedo color mixing based on material ID (z_hit_info.y) and noise
    al_albedo_color=mix(vec3(1.74),mix(vec3(.05),base_rgb,tn_noise_val*15.5),smoothstep(0.,1.,min(1.,z_hit_info.y*2.)));    
    if(z_hit_info.y>1.) al_albedo_color=vec3(1.); // Make certain parts white

    // Lighting calculations
    float dif=max(0.,dot(no_hit_normal,ld_light_direction)); // Diffuse
    float fr=pow(1.+dot(no_hit_normal,rd_ray_dir),4.); // Fresnel. rd_ray_dir is now correctly recognized.
    float attn=1.-pow(min(1.,length(lp_light_position-po_ray_hit_pos)/15.),4.0); // Attenuation
    float sp=pow(max(dot(reflect(-ld_light_direction,no_hit_normal),-rd_ray_dir),0.),40.); // Specular. rd_ray_dir is now correctly recognized.

    // Combine lighting components
    co_final_color=attn*mix(sp+al_albedo_color*(a_attenuation(0.1)+0.2)*(dif+s_smoothstep_factor(1.)),fo_fallback_color,min(fr,.5));
    co_final_color=mix(fo_fallback_color,co_final_color,exp(-0.0001*t_distance*t_distance*t_distance));
  }  
  
  // Apply accumulated glow contributions, using tunable glow colors
  co_final_color = pow(co_final_color + g_red_glow_accum * 0.2 * red_glow_rgb + gg_blue_glow_accum * 0.2 * blue_glow_rgb, vec3(OutputGamma)); // Tunable OutputGamma
  
  gl_FragColor = vec4(co_final_color,1.); // Final output
}
