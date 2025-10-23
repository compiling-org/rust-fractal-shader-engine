/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Fractal",
        "Raymarching",
        "Abstract",
        "Psychedelic"
    ],
    "DESCRIPTION": "Corrected conversion based on direct Shadertoy comparison, addressing original 'rot' function, 'p.t' typo, and mod/chromatic aberration in Pass 1. Enhanced with tunable psychedelic parameters for color and light control, using individual float inputs for compatibility.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "MainIterations",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 1.0,
            "MAX": 5.0,
            "LABEL": "Reflect/Refract Iterations"
        },
        {
            "NAME": "CameraSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Camera Movement Speed"
        },
        {
            "NAME": "FractalModulationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Fractal Morph Speed"
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "BloomStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Bloom Strength"
        },
        {
            "NAME": "ChromaticAberration",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Chromatic Aberration"
        },
        {
            "NAME": "FogDensity",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Fog Density"
        },
        {
            "NAME": "OverallHueShift",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Overall Hue Shift"
        },
        {
            "NAME": "OverallSaturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Overall Saturation"
        },
        {
            "NAME": "LightTintR",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Light Tint Red"
        },
        {
            "NAME": "LightTintG",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Light Tint Green"
        },
        {
            "NAME": "LightTintB",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Light Tint Blue"
        },
        {
            "NAME": "LightTintA",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Light Tint Alpha"
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ]
}
*/

#define dmin(a,b) a.x<b.x ? a : b

#define rot_original_shadertoy_broken(x) mat2(cos(x),sin(x),sin(x),cos(x))

#define rotgood(x) mat2(cos(x),-sin(x),sin(x),cos(x))

#define pmod(p,x) mod(p,x) - 0.5*x


vec3 glow = vec3(0);
vec3 glowb = vec3(0);
vec3 att = vec3(1);
float side = 1.;

float sdBox(vec3 p, vec3 s){
	p = abs(p) - s;
    return length(max(p,0.0)) + min(max(p.x, max(p.y,p.z)),0.);
}
#define pi (acos(-1.))

vec3 saturate(vec3 color, float saturation) {
    vec3 lum = vec3(0.299, 0.587, 0.114);
    vec3 gray = dot(color, lum) * vec3(1.0);
    return mix(gray, color, saturation);
}

mat3 hueRotationMatrix(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    vec3 k = normalize(vec3(1.0, 1.0, 1.0));
    return mat3(
        k.x*k.x*(1.0-c) + c,      k.y*k.x*(1.0-c) - k.z*s,  k.z*k.x*(1.0-c) + k.y*s,
        k.x*k.y*(1.0-c) + k.z*s,  k.y*k.y*(1.0-c) + c,      k.z*k.y*(1.0-c) - k.x*s,
        k.x*k.z*(1.0-c) - k.y*s,  k.y*k.z*(1.0-c) + k.x*s,  k.z*k.z*(1.0-c) + c
    );
}

vec3 applyHueShift(vec3 color, float shift) {
    return color * hueRotationMatrix(shift * pi * 2.0);
}


vec2 o(vec3 p){
    vec2 d = vec2(10e7);
    
    p = abs(p);
    float dBox = sdBox(p, vec3(0.1,0.5,0.7));
    p.x -= .45;
    float dBoxb = sdBox(p, vec3(0.08,0.8,0.5));
    
    p = abs(p);
    
    p.y -= 0.45;
    float dBoxc = sdBox(p, vec3(0.16));
    
    p.y -= 0.4;
    p.z -= 0.7;
    p = abs(p);
    
    p.x -= 0.05;
    p.z -= 0.15;
    float dBoxg = max(p.x,p.z);
	
    d = dmin(d, vec2(dBox, 5.));
    d = dmin(d, vec2(dBoxb, 21.));
    d = dmin(d, vec2(dBoxc, 9.));
    d = dmin(d, vec2(dBoxg, 21.));
    
    return d;
}

float mmmm;

vec2 map(vec3 p){
	vec2 d = vec2(10e6);
    
    vec3 q = p;
    float modD = 9.;
    
    vec2 id = floor(p.xz/modD);
    p.xz = pmod(p.xz,modD); 
    
    float mt = sin(TIME*FractalModulationSpeed*2. + id.x + sin(id.y)*0.4 + id.y*0.4);
    mmmm = pow(abs(mt), 0.5)*sign(mt)*0.2;
    
    vec3 cc = vec3(0.7 + mmmm*2.,.9,0.9);
    glowb += exp(-length(p)*(5. - mmmm))*2.*cc*att;
    
    const int MAX_FRACTAL_LOOPS = 4;
    for(int i = 0; i < MAX_FRACTAL_LOOPS; i++){
    	p = abs(p);
        p.xy *= rot_original_shadertoy_broken((0.5)*pi);
        p.x -= 0.8 + mmmm;
        p.y -= 1.5;
        p.zy *= rot_original_shadertoy_broken(0.25*pi);
        p.z -= 1.;
    }
    
    vec2 dO = o(p);
    
    d = dmin(d, dO);
    
    q = abs(q);
    q.y -=6.;
    
    d = dmin(d, vec2(abs(q.y) - 0.1, 21.));
    d.x *= 0.6;
    
    glow += exp(-max(d.x, 0.)*10.)*att*2.;
    
    return d;
}


vec2 march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit){
	vec2 d = map(ro);
    
    if(d.x < 0.2)
        ro += rd*0.08;
    
    hit = false; t = 0.; p = ro;
    
    const int MAX_MARCH_LOOPS_CONST = 180;
    // Converted while loop to for loop
    for(int i = 0; i < MAX_MARCH_LOOPS_CONST; i++){
    	d = map(p);
        d.x *= side;
        if(d.x < 0.00007){
        	hit = true;
        	break;
        }
    	t += d.x;
        p = ro + rd*t;
    }
    return d;
}

vec3 getRd(vec3 ro,vec3 lookAt,vec2 uv){
	vec3 dir = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0,1,0), dir));
    vec3 up = normalize(cross(dir, right));
	return normalize(dir + (right*uv.x + up*uv.y)*0.8);
}

vec3 getNormal(vec3 p){
	vec2 t_eps = vec2(0.0001,0.);
	return normalize(-vec3(
    	map(p - t_eps.xyy).x - map(p + t_eps.xyy).x ,
    	map(p - t_eps.yxy).x - map(p + t_eps.yxy).x ,
    	map(p - t_eps.yyx).x - map(p + t_eps.yyx).x 
    ));
}


void main() {
	if (PASSINDEX == 0)	{

	    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
		vec2 uvn = uv;
	    uv *= 1. + dot(uv,uv)*0.4;
	    
	    vec3 col = vec3(0);
	    
	    vec3 ro = vec3(0);
	    
	    ro.xy += vec2(cos(TIME*CameraSpeed*0.6), sin(TIME*CameraSpeed*0.5)*0.1)*3.;
	    
	    ro.z += TIME*CameraSpeed*10.;
	    
	    vec3 lookAt = vec3(0,0,ro.z + 1.5);
	    
	    vec3 rd = getRd(ro, lookAt, uv);
	    rd.xy *= rotgood(-cos(TIME)*0.1);
	    
	    vec3 p; float t; bool hit; float tA = 0.0;
		float tF = 0.0;
	    side = sign(map(ro).x);
	    vec2 d;

        const int MAX_MAIN_ITERATIONS = 5; 
	    for(int i = 0; i < MAX_MAIN_ITERATIONS; i++){
            if (float(i) >= MainIterations) break;

	        d = march(ro, rd, p, t, hit);
	        
	        if(i == 0) 
	            tF = t;
	        tA = max(t, tA);
	        if(hit){
				vec3 n = getNormal(p)*side;
				vec3 l = normalize(vec3(1));
	            
	            float diff = max(dot(n,l), 0.);
	            float spec = pow(max(dot(reflect(l, -rd),n),0.), 20.);
	            float fres = pow(1. - max(dot(n,-rd), 0.), 5.);
	            #define ao(j) clamp(map(p + n*j).x/j, 0., 1.)
	            #define sss(j) smoothstep(0., 1.,map(p + l*j).x/j)
	            float a = ao(0.4)*ao(0.1)*ao(0.1);
	 				 	 	 	 	 	 	 	 
	            vec3 lCol = vec3(0.3,0.7,1.) * vec3(LightTintR, LightTintG, LightTintB);
	            if(d.y > 10.){
	                col += 0.04*(pow(fres, 1.)*lCol + glow*0.0002 + lCol*spec*0.8)*att*a;
	                ro = p;
	                rd = reflect(rd, n);
	                att *= vec3(0.6,0.5,0.6)*0.4;
	            } else if(d.y > 5.){
	                col += vec3(0.7,0.7,0.4)*att*fres*4.*(glow*0.0004 );
	                ro = p;
	            	break;
	            } else {
	            	col += vec3(0.4,0.2,0.1)*((0.10 + diff*1.*fres)*att + spec*0.4*lCol)*a*att;
	            	break;
	            }
	            
	            #define FOG (vec3(0.15 + mmmm*0.2,0.10,0.28)*FogDensity)
	            if (float(i) >= (MainIterations - 1.0) || i == (MAX_MAIN_ITERATIONS - 1)){
	 				col = mix(col, FOG*att, smoothstep(0.,1.,tA*0.015));
	            }
	        }
	    }
	    
	    col += glowb*0.002;
	    
	    col = mix(col, FOG*0.06, pow(smoothstep(0.,1.,tF*0.015), 1.4));
	    
	    gl_FragColor = vec4(col,1.0) * GlowIntensity;
	}
	else if (PASSINDEX == 1)	{

		vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
		vec2 uvn = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.xy;
	    
	    const float STEPS_RADIAL_BLUR = 20.0;
	    float scale = 0.00 + pow(length(uv - 0.5),4.)*BloomStrength;
	    float chromAb_val = pow(length(uv - 0.5),1.)*ChromaticAberration;
	    vec2 offs = vec2(0);
	    vec4 radial = vec4(0);
	    vec2 dir = normalize(uvn);

        vec2 px_uv_offset = chromAb_val / RENDERSIZE.xy; 

	    for(float i = 0.; i < STEPS_RADIAL_BLUR; i++){
	        scale *= 0.97;
	        vec2 target_uv = uv + offs; 
	        offs -= dir*scale/STEPS_RADIAL_BLUR;
	    	
            radial.r += IMG_NORM_PIXEL(BufferA, target_uv + px_uv_offset).x;
            radial.g += IMG_NORM_PIXEL(BufferA, target_uv).y;
            radial.b += IMG_NORM_PIXEL(BufferA, target_uv - px_uv_offset).z;
	    }
	    radial /= STEPS_RADIAL_BLUR;
	    
	    gl_FragColor = radial*45.; 
	    gl_FragColor = mix(gl_FragColor,smoothstep(0.,1.,gl_FragColor), 0.4);
	    
	    gl_FragColor = max(gl_FragColor, 0.);
	    gl_FragColor = pow(gl_FragColor, vec4(0.4545 + dot(uvn,uvn)*1.));

        vec3 finalColor = gl_FragColor.rgb;
        finalColor = saturate(finalColor, OverallSaturation);
        finalColor = applyHueShift(finalColor, OverallHueShift);
        gl_FragColor = vec4(finalColor, gl_FragColor.a * LightTintA);
	}

}