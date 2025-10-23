/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Psychedelic",
        "Raymarching"
    ],
    "DESCRIPTION": "A new ISF shader featuring a dynamic fractal environment with customizable color palettes, camera controls, fractal geometry adjustments, and post-processing effects. Converted from a two-pass ShaderToy to a single-pass ISF.",
    "CREDIT": "Original ShaderToy by Kali. Converted to ISF by Gemini AI.",
    "INPUTS": [
        {
            "NAME": "mouseNormX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Mouse X (Normalized)"
        },
        {
            "NAME": "mouseNormY",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Mouse Y (Normalized)"
        },
        {
        	"NAME": "mouseClickF",
        	"TYPE": "float",
        	"DEFAULT": 0.0,
        	"MIN": 0.0,
        	"MAX": 1.0,
        	"STEP": 1.0,
        	"LABEL": "Mouse Clicked (0/1)"
        },
        {
            "NAME": "inputTexture",
            "TYPE": "image",
            "DEFAULT": "null",
            "LABEL": "Background Texture (Used for noise and post-processing)"
        },
        {
            "NAME": "colorPaletteIndex",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.0,
            "STEP": 1.0,
            "LABEL": "Color Palette"
        },
        {
            "NAME": "colorPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Color Pulse Strength"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Color Pulse Speed"
        },
        {
            "NAME": "morphStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Morph"
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 5.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "cameraPosX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera X Offset"
        },
        {
            "NAME": "cameraPosY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Y Offset"
        },
        {
            "NAME": "cameraRotX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera X Rotation"
        },
        {
            "NAME": "cameraRotY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera Y Rotation"
        },
        {
            "NAME": "fractalModX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod X"
        },
        {
            "NAME": "fractalModY",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod Y"
        },
        {
            "NAME": "fractalModZ",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod Z"
        },
        {
            "NAME": "fractalClampMin",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MIN": 0.01,
            "MAX": 0.5,
            "LABEL": "Fractal Clamp Min"
        },
        {
            "NAME": "fractalClampMax",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Fractal Clamp Max"
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Saturation"
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
        	"NAME": "enableVibrationF",
        	"TYPE": "float",
        	"DEFAULT": 0.0,
        	"MIN": 0.0,
        	"MAX": 1.0,
        	"STEP": 1.0,
        	"LABEL": "Enable Vibration (0/1)"
        },
        {
            "NAME": "vibrationStrength",
            "TYPE": "float",
            "DEFAULT": 0.0013,
            "MIN": 0.0,
            "MAX": 0.01,
            "LABEL": "Vibration Strength"
        },
        {
        	"NAME": "enablePostProcessF",
        	"TYPE": "float",
        	"DEFAULT": 1.0,
        	"MIN": 0.0,
        	"MAX": 1.0,
        	"STEP": 1.0,
        	"LABEL": "Enable Post Process (0/1)"
        }
    ],
    "PASSES": [
        {}
    ]
}
*/

#define RAY_STEPS 100
#define SHADOW_STEPS 50
#define LIGHT_COLOR vec3(1.,.97,.93)
#define AMBIENT_COLOR vec3(.75,.65,.6)

#define SPECULAR 0.65
#define DIFFUSE  1.0
#define AMBIENT  0.35

#define DETAIL .00004

const float PI = acos(-1.0);
const float tau = (2.*PI);

// Global variables (per fragment)
vec3 glow = vec3(0);
vec3 att = vec3(1);
float id = 0.0;
vec3 col_de_output; // Renamed to avoid conflict with `col` in main
float vibration = 0.0;
float dith;
float side = 1.;
int it; // Iteration count for march loop

// Global constants
const vec3 lightdir = normalize(vec3(0.1,-0.15,-1.));
const vec3 origin = vec3(-1.,0.2,0.);

// dmin macro definition
#define dmin(a, b) (a.x < b.x ? a : b)


mat2 rot(float a) {
	float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

float hash(vec2 p) {
	vec3 p3=fract(vec3(p.xyx)*.1031);
    p3+=dot(p3,p3.yzx+33.33);
    return fract((p3.x+p3.y)*p3.z);
}

vec4 formula(vec4 p) {
    p.xz = abs(p.xz+1.0 - morphStrength) - abs(p.xz-1.0 + morphStrength) - p.xz;
    p = p * 2.0 / clamp(dot(p.xyz,p.xyz), fractalClampMin, fractalClampMax) - vec4(fractalModX, fractalModY, fractalModZ, 0.);
    p.xy*=rot(.5);
    return p;
}

float screen(vec3 p) {
	float d1=length(p.yz-vec2(.25,0.))-.5;	
	float d2=length(p.yz-vec2(.25,2.))-.5;	
	return min(max(d1,abs(p.x-.3)-.01),max(d2,abs(p.x+2.3)-.01));
}

// pmod macro
#define pmod(p,x) mod(p,x) - 0.5*x

// valueNoise function for float
vec4 valueNoise_float(float t){
	return texture2D(inputTexture,vec2(floor(t)/256., 0.0)); // Only using x for 1D noise
}

// valueNoise function for vec2
vec4 valueNoise_vec2(vec2 t){
    vec2 fr = fract(t);
	return
        mix(
            mix(
                texture2D(inputTexture,vec2(floor(t.x), floor(t.y))/256.),
                texture2D(inputTexture,vec2(floor(t.x), floor(t.y) + 1.)/256.),
                smoothstep(0.,1.,fr.y)
            ),
            mix(
                texture2D(inputTexture,vec2(floor(t.x) + 1.,floor(t.y))/256.),
                texture2D(inputTexture,vec2(floor(t.x) + 1.,floor(t.y) + 1.)/256.),
                smoothstep(0.,1.,fr.y)
            ),
            smoothstep(0.,1.,fr.x));
}


float sdRhombus(vec3 p, vec3 s){
    p = abs(p) - s;
    float d = max(p.z, max(p.x, p.y));
    d = max(d, dot(p.yx + s.yx*0.5, normalize(vec2(1.))));
    d = max(d, dot(p.yz + s.yz*0.5, normalize(vec2(1.))));
    return d;
}

float fOpUnionSoft(float a, float b, float r) {
	float e = max(r - abs(a - b), 0.);
	return min(a, b) - e*e*0.25/r;
}

float sdBox( vec3 p, vec3 s ) {
    p = abs(p) - s;
    return max(p.x, max(p.y, p.z));
}

// de function - sets global `id` and `col_de_output`
float de(vec3 p) {
    vec3 pp=p;
    float sc = 1.;
    float local_t = TIME * animationSpeed * .15; // Using TIME and animationSpeed

    p.xy*=rot(pp.z*.2 + local_t*.5 + sin(p.z*.05 + local_t*2.)*4.);
    p.xy = abs(2. - mod(p.xy, 4.));
    p.z=abs(1.5-mod(p.z,3.));
    vec3 cp=p;
    for (int i=0;i<2;i++) {
        p.xy=abs(p.xy+1.)-abs(p.xy-1.)-p.xy;
        float s=10./clamp(dot(p,p),.1,1.2);
        p=p*s-11.;
        sc=sc*s;
    }
    float f=length(p.xy)/sc;
    float o=min(length(cp.yz),length(cp.xz));
    float l=length(pp.xy)+cos(pp.z*2.1)*.4;
    float d=min(l,min(f,o));
    id=step(o,d);
    col_de_output=vec3(.0,.3,1.);
    col_de_output*=step(abs(fract(local_t + pp.z*.01)-.5),.02);
    col_de_output+=id;
    col_de_output+=vec3(.5,.1,0)*step(l,d);
    return max(0.0001, (d-.02)*.5); // Ensure positive distance
}

// map function (needs to be before march, getNormal, shadow, calcAO)
vec2 map(vec3 p){
	vec2 d = vec2(10e7);
    
    p.x += sin(p.x*0.1 + p.y*4. + p.z)*0.2;
    
    vec3 q = p;
    
    vec3 modD = vec3(4.,4.,4.); // Declared locally as it's not a uniform
    q.yz = pmod(q.yz, modD.yz);
    
    vec3 u = q;
    
    vec3 j;
    vec3 m = vec3(0.0); // Initialized m
    
    for(int i = 0; i < 5;i ++){
        
        if(i == 3)
            j = q;
    	q = abs(q);
    	if ( q.x < q.y ) { q.xy = q.yx; }
	    if ( q.x < q.z ) { q.xz = q.zx; }
    	if ( q.y < q.z ) { q.yz = q.zy; }
        
        q.xz *= rot(-0.3);
        q.yz *= rot(0.25*PI);
        q.x -= 0.8 + sin(TIME * animationSpeed)*0.; // Using TIME and animationSpeed
    
    	q.x += 0.24;

    }
    
    q.x += 0.4;
    
    
    float dB = sdBox(q, vec3(0.6,0.2,0.1)*0.8);
    float dBb = length(u) - 0.3;
    
    dBb = abs(dBb) + 0.07;
    d = dmin(d, vec2(dBb,5.)); // dmin used here
    
    float dW = -q.x + 0.8;
    
    d = dmin(d, vec2(dW, 10.)); // dmin used here
    
    j.y -= 0.1;
    d = dmin(d, vec2(1. - sdBox(j, vec3(0.14)), 11.)); // dmin used here
    
    m = abs(m);
    
    m.z -= 0.;
    
    p.y -= 6.;
    
    
    float diam = 0.3;
    float dddd = length(p.xy) - diam + 3.*smoothstep(0.,1.,abs(p.z - TIME * animationSpeed + sin(TIME * animationSpeed)*2.)*0.03); // Using TIME and animationSpeed
    
    d = dmin(d, vec2(abs(dddd) + 0.01, 15.)); // dmin used here
    
    vec4 n_noise = valueNoise_float(TIME * animationSpeed * 1.); // Using TIME and animationSpeed, and float version
    
    glow += 0.14/(0.01 + dddd*dddd*(1. + n_noise.y*20.))*vec3(0.1,0.1,0.1 + n_noise.x)*att; // glow is global
    
    d.x *= 0.8;
    return d;
}

// getNormal function
vec3 getNormal(vec3 p){
	vec2 t_offset= vec2(0.001,0);
	return normalize(vec3(
        map(p - t_offset.xyy).x - map(p).x, // Calls map
        map(p - t_offset.yxy).x - map(p).x, // Calls map
        map(p - t_offset.yyx).x - map(p).x  // Calls map
    ));
}

// shadow function
float shadow(vec3 pos, vec3 sdir) {
	float sh=1.0;
	float totdist =2.0*DETAIL; // Use DETAIL constant
	float dist=10.;
	for (int steps=0; steps<SHADOW_STEPS; steps++) {
		if (totdist<1. && dist>DETAIL) {
			vec3 p = pos - totdist * sdir;
			dist = map(p).x; // Calls map
			sh = min( sh, max(50.*dist/totdist,0.0) );
			totdist += max(.01,dist);
		}
	}
    return clamp(sh,0.1,1.0);
}

// calcAO function
float calcAO( const vec3 pos, const vec3 nor ) {
	float aodet=DETAIL*40.;
	float totao = 0.0;
    float sca = 13.0;
    for( int aoi=0; aoi<5; aoi++ ) {
        float hr = aodet*float(aoi*aoi);
        vec3 aopos = nor * hr + pos;
        float dd = map( aopos ).x; // Calls map
        totao += -(dd-hr)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - 5.0*totao, 0., 1.0 );
}

// --- Psychedelic Color Palettes ---
vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 7.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// getPsychedelicPalette function (7 trippy palettes)
vec3 getPsychedelicPalette(float t_val, int index) {
    vec3 col_palette;
    if (index == 0) { // Hypnotic Rainbow
        vec3 a = vec3(0.5, 0.5, 0.5);
        vec3 b = vec3(0.5, 0.5, 0.5);
        vec3 c = vec3(1.0, 1.0, 1.0);
        vec3 d = vec3(0.263, 0.416, 0.557);
        col_palette = a + b * cos(6.28318 * (t_val * 0.7 + d));
    } else if (index == 1) { // Electric Neon
        col_palette = 0.5 + 0.5 * sin(t_val * tau * 2.0 + vec3(0.0, 0.33, 0.66));
        col_palette.r = pow(col_palette.r, 1.5); col_palette.g = pow(col_palette.g, 1.5); col_palette.b = pow(col_palette.b, 1.5);
    } else if (index == 2) { // Acid Trip
        col_palette = hsv2rgb(vec3(mod(t_val * 1.5 + TIME * 0.1, 1.0), 1.0, sin(t_val * 3.0) * 0.5 + 0.5));
    } else if (index == 3) { // Cosmic Dust
        col_palette = 0.5 + 0.5 * cos(t_val * 5.0 + vec3(0.0, 2.0, 4.0));
        col_palette = pow(col_palette, vec3(1.5, 1.2, 1.8));
    } else if (index == 4) { // Retro Wave
        col_palette = mix(vec3(0.0, 1.0, 1.0), vec3(1.0, 0.0, 1.0), sin(t_val * 4.0 + TIME * 0.2) * 0.5 + 0.5);
    } else if (index == 5) { // Bioluminescent
        col_palette = vec3(0.0, 0.8, 0.5) + sin(t_val * 7.0 + vec3(0.0, 1.0, 2.0)) * 0.4;
        col_palette.b += sin(t_val * 10.0) * 0.3;
        col_palette = clamp(col_palette, 0.0, 1.0);
    } else { // 6 - Dark Psychedelia
        col_palette = pow(abs(sin(t_val * 3.0 + vec3(0.0, 1.5, 3.0))), vec3(1.0, 2.0, 3.0));
        col_palette = mix(col_palette, vec3(0.1, 0.0, 0.2), 0.5);
    }
    return col_palette;
}


// light function
vec3 light(in vec3 p, in vec3 dir, in vec3 n, in float hid) {
    float sh=shadow(p, lightdir);

	float ao=calcAO(p,n);
	float diff=max(0.,dot(lightdir,-n))*sh*DIFFUSE;
	vec3 amb=max(.5,dot(dir,-n))*AMBIENT*getPsychedelicPalette(0.5, int(colorPaletteIndex));
	vec3 r = reflect(lightdir,n);
	float spec=pow(max(0.,dot(dir,-r))*sh,15.)*SPECULAR;
	vec3 light_col;
	
	if (hid>.5) {light_col=getPsychedelicPalette(0.1, int(colorPaletteIndex)); spec=spec*spec;}
	else{
		float k=pow(col_de_output.x*.11,2.); // Using col_de_output
		light_col=mix(vec3(k,k*k,k*k),getPsychedelicPalette(k, int(colorPaletteIndex)),.5)+.1;
		light_col+=pow(max(0.,1.-col_de_output.y),5.)*.3; // Using col_de_output
	}
	light_col=light_col*ao*(amb+diff*LIGHT_COLOR)+spec*LIGHT_COLOR;	

	if (hid>.5) {
		vec3 p2=p;
		p2.z=abs(1.-mod(p2.z,2.));
		vec3 c_tex=texture2D(inputTexture,mod(1.-p.zy-vec2(.4,0.2),vec2(1.))).rgb*2.; // Explicit .rgb
		light_col+=c_tex*abs(.01-mod(p.y-TIME*animationSpeed*.1,.02))/.01*ao;
		light_col*=max(0.,1.-pow(length(p2.yz-vec2(.25,1.)),2.)*3.5);
	} else{
		vec3 c_tex=(texture2D(inputTexture,mod(p.zx*2.+vec2(0.5),vec2(1.))).rgb); // Explicit .rgb
		c_tex*=abs(.01-mod(p.x-TIME*animationSpeed*.1*sign(p.x+1.),.02))/.01;
		light_col+=pow(col_de_output.x,10.)*.0000000003*c_tex; // Using col_de_output
		light_col+=pow(max(0.,1.-col_de_output.y),4.) // Using col_de_output
			*pow(max(0.,1.-abs(1.-mod(p.z+TIME*animationSpeed*2.,4.))),2.)
			*getPsychedelicPalette(0.9, int(colorPaletteIndex))*4.*max(0.,.05-abs(p.x+1.))/.05;
	}
	return light_col;
}

// march function
vec2 march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit){
    
	vec2 d = map(ro); // Calls map
	d.x *= side;
    if(d.x < 0.01)
        ro += rd*0.04;
    
    p = ro; t = 0.; hit = false;
    
    for(it = 0; it < 90 ; it++){ // Fixed FRAME_INDEX, it was effectively 90
    	d = map(p); // Calls map
        d.x *= side;
        d.x *= dith;
        if(d.x < 0.00001){
        	hit = true;
            break;
        }
        
        t += d.x;
        p = ro + rd*t;
    }
    return d;
}

// getRd function
vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
    vec3 dir = normalize(lookAt - ro);
	vec3 right = normalize(cross(vec3(0,1,0),dir ));
	vec3 up = normalize(cross(dir, right));
    float fov = 0.7;
    return normalize(dir + (right*uv.x + up*uv.y)*fov);
}

// path function
vec3 path(float ti) {
	vec3 p=vec3(sin(ti)*2.,(1.-sin(ti*.5))*.5,-cos(ti*.25)*30.)*.5;
	return p;
}


void main() {
    // Reset global accumulators for each fragment
    glow = vec3(0);
    att = vec3(1);
    side = 1.;

    float localTime = TIME * animationSpeed * 0.15;

    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    vec2 uvn = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.xy; // For post-processing

    // Initial UV distortion (controlled by enablePostProcessF)
    if (enablePostProcessF > 0.5) {
        uv*=1.+pow(length(uvn*uvn*uvn*uvn),4.)*.07; // Use uvn for distortion
    }
    
    uv.y*=RENDERSIZE.y/RENDERSIZE.x;

    vec4 n_main = pow(valueNoise_float(TIME * animationSpeed * 1.), vec4(2.)); // Using TIME and animationSpeed
    
    uv.xy *= rot(TIME * animationSpeed * 0.1 + PI*0.78 + sin((TIME * animationSpeed - 3.6))*0.2 + n_main.y*0.25); // Using TIME and animationSpeed
    
    // Initialize fragment color
    vec3 final_col = vec3(0);

    dith = mix(0.95,1., texture2D(inputTexture, 20.*uv*256. + TIME * animationSpeed).x); // Using TIME and animationSpeed
    vec3 ro = vec3(0);
    
    float nb = valueNoise_float(TIME * animationSpeed * 1./2.).x; // Using TIME and animationSpeed
    float zoom = 5. + n_main.x*1.;
    
    ro.z += TIME * animationSpeed * 1.; // Using TIME and animationSpeed
    
    ro.y += 0.6 + sin(TIME * animationSpeed)*0.; // Using TIME and animationSpeed
    
    vec3 lookAt = vec3(0,ro.y + 1.8,ro.z + 3.);
    
    // Mouse controls for lookAt (using float input)
    if(mouseClickF > 0.5){
        lookAt.y += mouseNormY*0.05;
        lookAt.x -= mouseNormX*0.05 - 0.025;
    } else {
        lookAt.y += -0. + valueNoise_float(TIME * animationSpeed * 1./2.).x*0.5; // Using TIME and animationSpeed
    }

    vec3 rd = getRd(ro, lookAt, uv);
    
    rd.xz *= rot(0.1);
    
    vec3 p; float t; bool hit;
    float tA = 0.;
    float tF = 0.;
    
    for(int i = 0; i < 3; i ++){
    	vec2 d = march(ro, rd, p, t, hit); // Calls march
    	vec3 n = getNormal(p)*side;
        
        vec3 ld = normalize(vec3(1));
        vec3 h = normalize(ld - rd);
        
        float diff = max(dot(n, ld), 0.);
        float spec = pow(max(dot(n, -h), 0.), 20.);
        float fres = pow(1. - max(dot(n, -rd), 0.14), 5.);
        
        if(i == 0)
            tF = t;
        
        tA = max(tA, t);
        if(hit){
            if(d.y == 10.){ // Material ID 10
                fres = max(fres, 0.1);
                
                // ao macro
                #define ao(j) clamp(map(p + n*j).x/j, 0., 1.)
                // Corrected call to getPsychedelicPalette: only pass t_val and index
                float dynamic_t_val_1 = 0.5 + dot(n, -rd) *20. + TIME * animationSpeed;
                final_col += pow(fres, 3.3)*0.1*getPsychedelicPalette(dynamic_t_val_1, int(colorPaletteIndex))*att*ao(0.1);
                att *= vec3(0.2,0.3,0.4)*1.8;

                side *= -1.;
                ro = p;
                rd = reflect(rd,n);
                rd = refract(rd,n, 0.98);

            } else if(d.y == 11.){ // Material ID 11
                fres = max(fres, 0.1);
                
                // Corrected call to getPsychedelicPalette: only pass t_val and index
                float dynamic_t_val_2 = 0.5 + dot(n, -rd) *10. + TIME * animationSpeed;
                vec3 a_color = pow(fres, 1.3)*0.014*getPsychedelicPalette(dynamic_t_val_2, int(colorPaletteIndex))*att;
                
                #define ao(j) clamp(map(p + n*j).x/j, 0., 1.)
                a_color *= ao(0.1);
                final_col += a_color;
                
                att *= vec3(0.2,0.3,0.4)*0.1;

                break; // Break after hitting this material
                
            }
        }
    }

    final_col = mix(final_col, vec3(0.41,0.1,0.1)*0.0014, pow(smoothstep(0.,1.,tF*0.03), 1.));
    
    final_col -= glow*0.00002;
    
    final_col = min(final_col, 0.0032);
    
    // Apply overall Brightness, Saturation, Contrast
    final_col = pow(final_col, vec3(0.6545 + dot(uvn,uvn)*1.6)) * brightness;
    final_col = mix(vec3(length(final_col)),final_col,saturation);
    final_col = (final_col - 0.5) * contrast + 0.5;

    // Color Pulse (moving line of light)
    float pulsePos = mod(gl_FragCoord.x / RENDERSIZE.x + TIME * colorPulseSpeed, 1.0);
    float pulseEffect = 1.0 - smoothstep(0.4, 0.6, abs(gl_FragCoord.x / RENDERSIZE.x - pulsePos));
    final_col += pulseEffect * colorPulseStrength;

    // Post-processing effects (from Image pass)
    if (enablePostProcessF > 0.5) { // Controlled by float input
        // Radial blur and chromatic aberration simulation by direct color manipulation
        float radial_uv_len = length(uvn);
        final_col *= (1.0 - radial_uv_len * 0.7); // Vignette effect
        
        // Simulate chromatic aberration by shifting colors based on radial distance
        float chromAb_local = pow(length(uv - 0.5),1.)*1.1; // Defined chromAb_local
        
        // Applying the chromatic aberration effect directly to final_col
        final_col.r = texture2D(inputTexture, gl_FragCoord.xy / RENDERSIZE.xy + chromAb_local * 0.01).r;
        final_col.g = texture2D(inputTexture, gl_FragCoord.xy / RENDERSIZE.xy).g;
        final_col.b = texture2D(inputTexture, gl_FragCoord.xy / RENDERSIZE.xy - chromAb_local * 0.01).b;

        // Original Image pass final adjustments
        final_col *= 5.5;
        final_col *= 40.;
        final_col = mix(final_col,smoothstep(0.,1.,final_col), 0.34);
        final_col = max(final_col, 0.);
        final_col *= 1.7;
        final_col.g *= 1.1;
        final_col.r *= 0.95 + uvn.x*0.7;
        final_col.g *= 0.95 + uvn.y*0.3;
        final_col = pow(final_col, vec3(0.6545 + dot(uvn,uvn)*1.6));
    }

    gl_FragColor = vec4(final_col,1.0)*min(1.,TIME*animationSpeed*.5);
}