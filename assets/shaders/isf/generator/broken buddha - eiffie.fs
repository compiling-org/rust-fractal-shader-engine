/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Procedural",
        "Raymarching"
    ],
    "DESCRIPTION": "A psychedelic fractal shader with extensive tunable parameters for vibrant colors, pulses, shake, fractal, and geometry control, optimized for older GLSL environments like isf.video.",
    "IMPORTED": {
        "Texture0": {
            "NAME": "Texture0",
            "PATH": "file://./textures/noise.png" 
        }
    },
    "INPUTS": [
        {
            "NAME": "Mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5]
        },
        {
            "NAME": "USE_TEXTURE_TOGGLE",
            "TYPE": "bool",
            "DEFAULT": true,
            "DESCRIPTION": "Use an external texture for noise or procedural noise."
        },
        {
            "NAME": "FUDGE_FACTOR",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 1.5,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Adjusts raymarching step size for quality/performance."
        },
        {
            "NAME": "MARCH_STEPS",
            "TYPE": "float",
            "MIN": 16.0,
            "MAX": 128.0,
            "DEFAULT": 64.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of raymarching steps for main scene."
        },
        {
            "NAME": "SHADOW_STEPS",
            "TYPE": "float",
            "MIN": 5.0,
            "MAX": 20.0,
            "DEFAULT": 10.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of steps for shadow raymarching."
        },
        {
            "NAME": "AO_STEPS",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 8.0,
            "DEFAULT": 4.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of steps for ambient occlusion calculation."
        },
        {
            "NAME": "LIGHT_COLOR_RGB",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0],
            "DESCRIPTION": "Color of the main light source."
        },
        {
            "NAME": "SKY_COLOR_RGB",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.16, 0.27, 1.0],
            "DESCRIPTION": "Color of the background sky."
        },
        {
            "NAME": "FRACTAL_MR_VALUE",
            "TYPE": "float",
            "MIN": 0.01,
            "MAX": 0.5,
            "DEFAULT": 0.13,
            "DESCRIPTION": "Minimum radius for fractal inversion."
        },
        {
            "NAME": "FRACTAL_SCALE_VAL",
            "TYPE": "float",
            "MIN": -3.0,
            "MAX": -1.0,
            "DEFAULT": -1.7,
            "DESCRIPTION": "Scaling factor for the fractal iterations."
        },
        {
            "NAME": "FRACTAL_P0_X",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 2.0,
            "DESCRIPTION": "X component of the fractal offset vector P0."
        },
        {
            "NAME": "FRACTAL_P0_Y",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": -0.32,
            "DESCRIPTION": "Y component of the fractal offset vector P0."
        },
        {
            "NAME": "FRACTAL_P0_Z",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 2.48,
            "DESCRIPTION": "Z component of the fractal offset vector P0."
        },
        {
            "NAME": "FRACTAL_DE_ITERATIONS",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the Distance Estimator (DE)."
        },
        {
            "NAME": "FRACTAL_CE_ITERATIONS",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for the Color Estimator (CE)."
        },
        {
            "NAME": "FRACTAL_DE_Z_G_ITERATION",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 9.0,
            "DEFAULT": 3.0,
            "STEP": 1.0,
            "DESCRIPTION": "Iteration number at which zG is captured in DE/CE."
        },
        {
            "NAME": "MAT_ROUGHNESS",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Material roughness (prp.x)."
        },
        {
            "NAME": "MAT_SPECULAR",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "DESCRIPTION": "Material specular strength (prp.y)."
        },
        {
            "NAME": "MAT_METALLIC",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Material metallicness (prp.z)."
        },
        {
            "NAME": "MAT_ALPHA",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Material alpha (prp.w)."
        },
        {
            "NAME": "STAR_GLOW_EXPONENT",
            "TYPE": "float",
            "MIN": 10.0,
            "MAX": 200.0,
            "DEFAULT": 110.0,
            "DESCRIPTION": "Exponent for the star glow effect in the background."
        },
        {
            "NAME": "BACKGROUND_ANIM_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.25,
            "DESCRIPTION": "Speed of background texture animation."
        },
        {
            "NAME": "NOISE_FREQ",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 512.0,
            "DEFAULT": 256.0,
            "DESCRIPTION": "Frequency of procedural noise if texture is off."
        },
        {
            "NAME": "NOISE_ANIM_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 3.0,
            "DESCRIPTION": "Speed of procedural noise animation."
        },
        {
            "NAME": "NOISE_DISTORTION",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 2.4,
            "DESCRIPTION": "Distortion amount for procedural noise."
        },
        {
            "NAME": "LIGHT_ANIM_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.02,
            "DESCRIPTION": "Speed of light source animation."
        },
        {
            "NAME": "CAMERA_ANIM_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.1,
            "DESCRIPTION": "Speed of camera animation."
        },
        {
            "NAME": "COLOR_PALETTE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 6.0,
            "DEFAULT": 0.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 7 psychedelic color palettes."
        },
        {
            "NAME": "PULSE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "DESCRIPTION": "Intensity of color pulsing effect."
        },
        {
            "NAME": "SHAKE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.0,
            "DESCRIPTION": "Intensity of screen shake effect."
        },
        {
            "NAME": "BACKGROUND_BRIGHTNESS",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Brightness multiplier for the background."
        },
        {
            "NAME": "SHADOW_INTENSITY",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Controls the intensity of shadows."
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
            "INPUTS": [
                {
                    "NAME": "BufferA",
                    "TYPE": "image"
                }
            ]
        }
    ]
}
*/

#define PI 3.14159
#define ONE_OVER_PI 0.31831

// Define maximum constant iterations for loops due to older GLSL restrictions
const int MAX_DE_ITERATIONS_CONST = 10; // Max for DE/CE iterations
const int MAX_MARCH_STEPS_CONST = 128; // Max for MarchSteps
const int MAX_SHADOW_STEPS_CONST = 20; // Max for ShadowSteps
const int MAX_AO_STEPS_CONST = 8; // Max for AOSteps

// Explicit Uniform Declarations for ISF Inputs and Imported Textures


 // Explicitly declared

// Global variable for PixelSize (calculated in main, used elsewhere)
float PixelSize;

// Custom min function for floats to handle extremely strict GLSL compilers
float custom_min_f(float a, float b) {
    return (a < b) ? a : b;
}

// Function to get sign of a float (re-added)
float sgn(float x){return (x<0.0?-1.0:1.0);}

// --- Psychedelic Color Palettes ---
vec3 getPaletteColor(float t) {
    t = fract(t); // Ensure t is between 0 and 1
    if (COLOR_PALETTE < 0.5) { // Palette 0: Rainbow Spectrum
        return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
    } else if (COLOR_PALETTE < 1.5) { // Palette 1: Fiery Sunset
        return mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 0.5, 0.0), t);
    } else if (COLOR_PALETTE < 2.5) { // Palette 2: Ocean Depths
        return mix(vec3(0.0, 0.0, 0.5), vec3(0.0, 0.8, 1.0), t);
    } else if (COLOR_PALETTE < 3.5) { // Palette 3: Alien Forest
        return mix(vec3(0.1, 0.5, 0.1), vec3(0.8, 1.0, 0.0), t);
    } else if (COLOR_PALETTE < 4.5) { // Palette 4: Cyberpunk Neon
        return mix(vec3(0.7, 0.0, 1.0), vec3(0.0, 1.0, 0.7), t);
    } else if (COLOR_PALETTE < 5.5) { // Palette 5: Vintage Pop
        return mix(vec3(0.9, 0.2, 0.5), vec3(0.2, 0.9, 0.8), t);
    } else { // Palette 6: Grayscale to Color Burst
        vec3 gray = vec3(t);
        return mix(gray, 0.5 + 0.5 * sin(6.28318 * (t * 2.0 + vec3(0.0, 0.33, 0.67))), smoothstep(0.0, 0.5, t));
    }
}


float rnd(vec2 co){return fract(sin(dot(co,vec2(123.42,117.853)))*412.453);}
float noyz(vec2 p){
	vec2 c=floor(p),f=fract(p),v=vec2(1.0,0.0);
	return mix(mix(rnd(c),rnd(c+v.xy),f.x),mix(rnd(c+v.yx),rnd(c+v.xx),f.x),f.y);
}

// texturef now uses USE_TEXTURE_TOGGLE to switch
vec4 texturef(vec2 p){
    if (USE_TEXTURE_TOGGLE) {
        return texture2D(Texture0, p);
    } else {
        float n=noyz(p*NOISE_FREQ); // Use tunable NOISE_FREQ
        p=sin(p*NOISE_ANIM_SPEED+NOISE_DISTORTION*sin(p.yx*NOISE_ANIM_SPEED)); // Use tunable NOISE_ANIM_SPEED, NOISE_DISTORTION
        n+=0.5+0.5*(p.x*p.y);
        return vec4(0.5)+vec4(0.25*n,0.0,0.25*n,0.0);
    }
}

// Variables for fractal constants, now directly from uniforms
// No longer const, as they are derived from uniforms
float current_mr;
vec4 current_scale_vec;
vec4 current_p0;
// These are not needed globally, they are calculated and used within DE/CE
// float current_psni_de, current_psni2_de;
// float current_psni_ce, current_psni2_ce;

float DE(in vec3 z0){
    // Update fractal constants from uniforms
    current_mr = FRACTAL_MR_VALUE;
    current_scale_vec = vec4(FRACTAL_SCALE_VAL, FRACTAL_SCALE_VAL, FRACTAL_SCALE_VAL, abs(FRACTAL_SCALE_VAL));
    current_p0 = vec4(FRACTAL_P0_X, FRACTAL_P0_Y, FRACTAL_P0_Z, 1.0);

    int de_iters = int(FRACTAL_DE_ITERATIONS);
    int de_zG_iter = int(FRACTAL_DE_Z_G_ITERATION);

    vec4 z = vec4(z0,1.0),zG = vec4(0.0); // Initialize zG
    for (int n = 0; n < MAX_DE_ITERATIONS_CONST; n++) { // Use constant loop limit
        if (n >= de_iters) break; // Break if desired iterations reached

		z.xyz=clamp(z.xyz, -1.0, 1.0) *2.0-z.xyz;
		z*=current_scale_vec/clamp(dot(z.xyz,z.xyz),current_mr,1.0);
		if(n==de_zG_iter)zG=z; // Use tunable iteration for zG
		z+=current_p0;
	}
    // These psni calculations are local to DE if not used outside it
    // float current_psni_de = pow(abs(current_scale_vec.x),-float(de_iters));
    // float current_psni2_de = pow(abs(current_scale_vec.x),-float(de_zG_iter));

	float dG=(length(max(abs(zG.xyz)-vec3(0.8,4.2,0.0),0.0))-0.01)/zG.w;
	return custom_min_f(dG,(length(max(abs(z.xyz)-vec3(4.4,0.9,1.5),0.0))-0.01)/z.w);
}

struct matl{vec4 col;vec4 prp;}m;// prp=rough, spec, metal

float CE(in vec3 z0){
    // Update fractal constants from uniforms
    current_mr = FRACTAL_MR_VALUE;
    current_scale_vec = vec4(FRACTAL_SCALE_VAL, FRACTAL_SCALE_VAL, FRACTAL_SCALE_VAL, abs(FRACTAL_SCALE_VAL));
    current_p0 = vec4(FRACTAL_P0_X, FRACTAL_P0_Y, FRACTAL_P0_Z, 1.0);

    int ce_iters = int(FRACTAL_CE_ITERATIONS);
    int ce_zG_iter = int(FRACTAL_DE_Z_G_ITERATION); // Using the same zG iteration for CE

	vec4 z = vec4(z0,1.0),zG = vec4(0.0); // Initialize zG
	for (int n = 0; n < MAX_DE_ITERATIONS_CONST; n++) { // Use constant loop limit
        if (n >= ce_iters) break; // Break if desired iterations reached

		z.xyz=clamp(z.xyz, -1.0, 1.0) *2.0-z.xyz;
		z*=current_scale_vec/clamp(dot(z.xyz,z.xyz),current_mr,1.0);
		if(n==ce_zG_iter)zG=z; // Use tunable iteration for zG
		z+=current_p0;
	}
    // These psni calculations are local to CE if not used outside it
    // float current_psni_ce = pow(abs(current_scale_vec.x),-float(ce_iters));
    // float current_psni2_ce = pow(abs(current_scale_vec.x),-float(ce_zG_iter));

	float dG=length(max(abs(zG.xyz)-vec3(0.8,4.2,0.0),0.0))/zG.w;
	float dS=length(max(abs(z.xyz)-vec3(4.4,0.9,1.5),0.0))/z.w;

    // Use tunable material properties
	vec4 col_base=texturef(10.0*z0.xy+5.0*z0.zz)+vec4(sin(z.xyz)*0.1,0.0);
    vec4 prp_base=vec4(MAT_ROUGHNESS, MAT_SPECULAR, MAT_METALLIC, MAT_ALPHA);

	if(dS<dG){dS-=col_base.r*0.01/z.w;}
	else {col_base=col_base.brga;dG-=col_base.r*0.01/zG.w;col_base.r+=0.5;prp_base=vec4(0.14,0.22,1.0,0.1);}
	m.col+=col_base;m.prp+=prp_base;
	return custom_min_f(dS,dG);
}

vec3 getBackground( in vec3 rd ){
    vec3 current_sky_color = SKY_COLOR_RGB.rgb;
    vec3 current_light_color = LIGHT_COLOR_RGB.rgb;

	float d=max(0.0,dot(rd,vec3(0.0,1.0,0.0)));
	vec3 bcol=current_sky_color+rd*0.1+current_light_color*d*0.05;
	float y=1.0-abs(rd.y),a=0.44+atan(rd.x,rd.z);
    // Use tunable background animation speed
	vec2 pt=vec2(a+sin(7.0*y+a*10.0+TIME*BACKGROUND_ANIM_SPEED)*0.05*y,rd.y+TIME*BACKGROUND_ANIM_SPEED);
	bcol*=texturef(pt).rgb;
    // Use tunable star glow exponent
	bcol+=current_light_color*pow(d,STAR_GLOW_EXPONENT)*0.25;
	return bcol * BACKGROUND_BRIGHTNESS; // Apply background brightness
}

float shadao(vec3 ro, vec3 rd, float px, float max_dist){
	float res=1.0,d,t=2.0*px;
    int shadow_steps = int(SHADOW_STEPS); // Use tunable shadow steps
	for(int i=0;i<MAX_SHADOW_STEPS_CONST;i++){ // Use constant loop limit
        if (i >= shadow_steps) break; // Break if desired steps reached

		d=max(0.0,DE(ro+rd*t)*1.5);
		if(t+d>max_dist)break;
		t+=d;
		res=custom_min_f(res,3.0*d/t);
	}
	return res;
}

float fakeAO(vec3 ray, vec3 norm, float ao_eps) {
	float ao=1.0,w=0.1/ao_eps,dist=2.0*ao_eps,d;
    int ao_steps = int(AO_STEPS); // Use tunable AO steps
	for (int i=0; i<MAX_AO_STEPS_CONST; i++) { // Use constant loop limit
        if (i >= ao_steps) break; // Break if desired steps reached

		d = DE(ray + norm*dist);
		ao -= (dist-d) * w;
		w *= 0.5; dist = dist*2.0 - ao_eps;
	}
 	return clamp(ao, 0.0, 1.0);
}

float pow5(float v){float tmp = v*v;return tmp*tmp*v;}

vec3 shade(in vec3 ro, in vec3 rd, in float t, in vec3 color){
    vec3 current_light_color = LIGHT_COLOR_RGB.rgb;
    // vec3 current_sky_color = SKY_COLOR_RGB.rgb; // Not used directly here, but in getBackground

	float px=PixelSize*t;
	vec2 e=vec2(0.5*px,0.0);
	float ds=DE(ro+rd*t);
	ro+=rd*(t+ds-px);
	m.col=vec4(0.0);m.prp=vec4(0.0);//clear material before taking samples
	float d0=CE(ro);
	vec3 dn=vec3(CE(ro-e.xyy),CE(ro-e.yxy),CE(ro-e.yyx));
	vec3 dp=vec3(CE(ro+e.xyy),CE(ro+e.yxy),CE(ro+e.yyx));
	m.col*=0.143;m.prp*=0.143;
	vec3 N=(dp-dn)/(length(dp-vec3(d0))+length(vec3(d0)-dn));
	vec3 L=normalize(vec3(-0.11,0.74,0.19)),H=normalize(L-rd);

    // Apply color palette and pulse to material color
    vec3 pulsed_material_color = getPaletteColor(m.col.r + TIME * 0.1 * PULSE_STRENGTH) * m.col.rgb;
    vec3 col=pulsed_material_color*m.prp.a;

#define MF 0.001
 	float h=max(MF,dot(N,H)),d=max(MF,dot(L,H)),l=max(MF,dot(N,L)),v=max(MF,dot(N,-rd));
	float rf=max(MF,m.prp.x),frk=.5+2.*d*d*rf;
	vec3 diff=pulsed_material_color*ONE_OVER_PI*(1.+(frk-1.)*pow5(1.-l))*(1.+(frk-1.)*pow5(1.-v));
	float a=rf*rf,dv=h*h*(a-1.)+1.,D=a/(PI*dv*dv),k=rf/2.,G=l/(l*(1.-k)+k)*v/(v*(1.-k)+k);
	vec3 F0=m.prp.y*mix(vec3(1.0),pulsed_material_color,m.prp.z);
	vec3 F=F0+(1.0-F0)*pow5(1.0-d);
	vec3 spec=clamp(D*F*G/(4.0*l*v),0.0,1.0);
	float shad=0.0;
	if(l>MF)shad=shadao(ro,L,px,10.0);
	h*=0.05;
    // Apply color palette to light contribution
	col+=clamp(diff+spec+vec3(-0.05+h,0.0,0.05-h),0.0,1.0)*l*shad*current_light_color * SHADOW_INTENSITY;

    float ao=fakeAO(ro,N,px);
    // Background color for reflection, now uses tunable sky color and light color
    vec3 bcol_reflected=getBackground(reflect(rd,N));

	return mix(ao*col*(1.25-0.25*sin(TIME*2.5)),color,clamp(ds/px,0.0,1.0)); // st replaced with sin(TIME*2.5) for consistency
}

vec3 scene( vec3 ro, vec3 rd, vec2 fragCoord )
{// find color and distance of scene
	//march
    int march_steps = int(MARCH_STEPS); // Use tunable march steps
	float t=DE(ro)*rnd(fragCoord.xy)*0.75;
	float d,dm=100.0,tm=0.0,MIN_DIST=PixelSize*0.001,od=1000.0;//distances
	vec4 hit=vec4(-1.0);//we will grab up to 4 depths that are local mins
	for(int i=0;i<MAX_MARCH_STEPS_CONST;i++){ // Use constant loop limit
        if (i >= march_steps) break; // Break if desired steps reached

		d=DE(ro+rd*t)*FUDGE_FACTOR; // Use tunable FudgeFactor
		// #ifdef HI_QUAL logic removed, now controlled by MARCH_STEPS for quality
		if(d<od){
			if(d<0.5*PixelSize*t && hit.x<0.0){//we want to draw this edge, it occludes the pixel
				hit=vec4(hit.yzw,t);//push
			}
		}
		od=d;
		if(d<dm){tm=t;dm=d;}//and save the max occluder
		t+=d;
		if(t>10.0 || d<MIN_DIST)break;
	}
	//we have saved the edges but there is also a min or final surface
	if(tm>hit.w && dm<PixelSize*(tm-dm)){//if minimum has not been saved
		if(hit.x>0.0)hit=hit.wxyz;//write over the last entry, not the first
		hit.x=tm-dm;hit=hit.yzwx;
	}
	// #ifdef HI_QUAL logic removed
	if(od<PixelSize*(t-d) && hit.x<0.0){//save final distance if we have room
		hit.x=t-d;hit=hit.yzwx;
	}

	//color the background
	vec3 col=getBackground(rd);

	//add in the object(s)
	// #ifdef HI_QUAL logic removed
	for(int i=0;i<4;i++){//play back the hits and mix the color samples
		hit=hit.wxyz;//pop
		if(hit.x>0.0)col=shade(ro,rd,hit.x,col);
	}

	return clamp(col,0.0,1.0);
}

mat3 lookat(vec3 fw,vec3 up){
	fw=normalize(fw);vec3 rt=normalize(cross(fw,normalize(up)));return mat3(rt,cross(rt,fw),fw);
}

void main() { // Renamed from mainImage
    // Apply screen shake at the very beginning of the fragment shader
    vec2 fragCoord_shaken = gl_FragCoord.xy;
    vec2 shake_offset = vec2(sin(TIME * 20.0 + fragCoord_shaken.x * 10.0), cos(TIME * 25.0 + fragCoord_shaken.y * 12.0)) * SHAKE_STRENGTH;
    fragCoord_shaken += shake_offset;

	float current_st=sin(TIME*2.5); // st is now a local variable, derived from TIME
	vec3 ro=vec3(sin(TIME*LIGHT_ANIM_SPEED)*2.31,-0.00521,0.05113); // Use tunable LIGHT_ANIM_SPEED
	vec3 dr=vec3((2.0*fragCoord_shaken.xy-RENDERSIZE.xy)/RENDERSIZE.y,2.0); // Use shaken fragCoord
	vec3 rd=normalize(dr);
	PixelSize=2.5/(RENDERSIZE.y*dot(rd,dr)); // PixelSize is assigned here, now that it's globally declared
	vec3 fw=mix(vec3(sin(TIME*CAMERA_ANIM_SPEED),cos(TIME*0.27*CAMERA_ANIM_SPEED),sin(TIME*0.13*CAMERA_ANIM_SPEED)),-ro,clamp(dot(ro,ro)*0.12,0.0,1.0)); // Use tunable CAMERA_ANIM_SPEED
	float d=DE(ro);
	fw=mix(vec3(sgn(sin(TIME*LIGHT_ANIM_SPEED+1.57)),0.0,0.0),fw,smoothstep(-0.025,0.04,d*d)); // Use tunable LIGHT_ANIM_SPEED
	rd=lookat(fw,vec3(0.3+0.5*sin(TIME*0.23*CAMERA_ANIM_SPEED),1.0,0.4))*rd; // Use tunable CAMERA_ANIM_SPEED

	vec3 color=scene(ro,rd,fragCoord_shaken); // Pass shaken fragCoord to scene
	if(color!=color)color=vec3(0.0,1.0,0.0); // Check for NaNs
	gl_FragColor = vec4(color,1.0);
}