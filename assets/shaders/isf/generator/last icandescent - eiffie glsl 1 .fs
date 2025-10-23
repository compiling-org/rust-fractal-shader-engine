/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Procedural"
    ],
    "DESCRIPTION": "A psychedelic fractal shader with extensive tunable parameters, optimized for older GLSL environments like isf.video.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "Mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5]
        },
        {
            "NAME": "FRACTAL_ITERATIONS_MAIN",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 15.0,
            "DEFAULT": 5.0,
            "STEP": 1.0
        },
        {
            "NAME": "FRACTAL_ITERATIONS_SUB",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 3.0,
            "STEP": 1.0
        },
        {
            "NAME": "FRACTAL_SCALE",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 5.0,
            "DEFAULT": 3.48
        },
        {
            "NAME": "FRACTAL_OFFSET_X",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 1.9
        },
        {
            "NAME": "FRACTAL_OFFSET_Y",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "FRACTAL_OFFSET_Z",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 2.56
        },
        {
            "NAME": "LIGHT_FALLOFF_PARAM",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "LIGHT_MOVE_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "LIGHT_AMPLITUDE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.4
        },
        {
            "NAME": "SHADOW_INTENSITY",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "BRIGHTNESS",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "TILE_SIZE",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 3.0
        },
        {
            "NAME": "COLOR_PALETTE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 6.0,
            "DEFAULT": 0.0,
            "STEP": 1.0
        },
        {
            "NAME": "PULSE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "SHAKE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.0
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
#define LIGHT_FALLOFF LIGHT_FALLOFF_PARAM // Use tunable parameter

// Define maximum constant iterations for loops due to older GLSL restrictions
const int MAX_FRACTAL_ITERATIONS_MAIN = 15;
const int MAX_FRACTAL_ITERATIONS_SUB = 10;
const int MAX_MARCH_ITERATIONS_PASS0 = 32; // Loop for ray march in pass 0
const int MAX_MARCH_ITERATIONS_PASS1_PRIMARY = 48; // Loop for ray march in pass 1
const int MAX_MARCH_ITERATIONS_PASS1_SHADOW = 32; // Loop for shadow accumulation in pass 1


// Custom min function for floats to handle extremely strict GLSL compilers
float custom_min_f(float a, float b) {
    return (a < b) ? a : b;
}

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

float rand(vec3 co){// implementation found at: lumina.sourceforge.net/Tutorials/Noise.html
	return fract(sin(dot(co*0.123,vec3(12.9898,78.233,112.166))) * 43758.5453);
}

// Tile function now uses a tunable parameter
vec3 Tile(vec3 p, float a){return abs(mod(p+a,a*4.0)-a*2.0)-a;}

int obj; // Declared globally as it's used to pass info out of DE
float DE(in vec3 z){
    // Declare and initialize these variables inside the function
    // as they depend on uniforms, which are not compile-time constants.
    int current_iters_main = int(FRACTAL_ITERATIONS_MAIN);
    int current_iters_sub = int(FRACTAL_ITERATIONS_SUB);
    float current_fractal_scale = FRACTAL_SCALE;
    vec3 current_fractal_offset = vec3(FRACTAL_OFFSET_X, FRACTAL_OFFSET_Y, FRACTAL_OFFSET_Z);

    // Calculate psni and psni2 using the local variables
    float current_psni = pow(current_fractal_scale,-float(current_iters_main));
    float current_psni2 = pow(current_fractal_scale,-float(current_iters_sub));

	z=Tile(z, TILE_SIZE); // Use tunable TILE_SIZE
	vec3 z2 = z; // Initialize z2
	for (int n = 0; n < MAX_FRACTAL_ITERATIONS_MAIN; n++) { // Loop condition is now a compile-time constant
        if (n >= current_iters_main) break; // Break if desired iterations are met

		if(n==current_iters_sub)z2=z; // This comparison is fine because 'n' is also an int
		z = abs(z);
		if(z.x<z.y)z.xy = z.yx;
		z.xz = z.zx;
		z = z*current_fractal_scale - current_fractal_offset*(current_fractal_scale-1.0); // Use current tunable parameters
		if(z.z<-0.5*current_fractal_offset.z*(current_fractal_scale-1.0))z.z+=current_fractal_offset.z*(current_fractal_scale-1.0); // Use current tunable parameters
	}
    float d1=(length(z.xy)-1.0)*current_psni; // Use current_psni
    float d2=length(max(abs(z2)-vec3(0.2,5.1,1.3),0.0))*current_psni2; // Use current_psni2
    obj=(d1<d2)?0:1;
	return custom_min_f(d1,d2); // Use custom_min_f
}

mat3 lookat(vec3 fw){
	fw=normalize(fw);vec3 rt=normalize(cross(fw,vec3(0.0,1.0,0.0)));return mat3(rt,cross(rt,fw),fw);
}

float sgn(float x){return (x<0.0?-1.0:1.0);} // Add .0

float isInShadow(vec3 p, vec3 posLight, float eps){
    vec3 L=(p-posLight);//light direction for shadow lookup
	float d=length(L);
	if(d<LIGHT_FALLOFF){//ignore if light is too far away
		L/=d;//normalize
		float phi=asin(L.y);//transform back to 2d
		vec2 pt=vec2(asin(L.z/cos(phi)),phi);
        if(L.x<0.0)pt.x=sgn(L.z)*PI-pt.x;
        pt/=vec2(PI*2.0,PI);
		pt+=0.5;//uncenter
		pt*=vec2(0.5,1.0);//left side of texture only
		// Use texture2D and explicitly access .r
		if(d-2.0*eps*d<texture2D(BufferA,mod(pt,1.0)).r*LIGHT_FALLOFF)return d;
	}
    return -1.0;
}

void main() {
	if (PASSINDEX == 0)	{
		vec2 uv=gl_FragCoord.xy/RENDERSIZE.xy;

        // Apply Screen Shake from previous shader
        vec2 shake_offset = vec2(sin(TIME * 20.0 + uv.x * 10.0), cos(TIME * 25.0 + uv.y * 12.0)) * SHAKE_STRENGTH;
        uv += shake_offset;

		uv*=vec2(2.0,1.0); // stretch to 2x1
		vec3 ro,rd;
        // Tunable light position and movement
        vec3 posLight=vec3(TIME * LIGHT_MOVE_SPEED, sin(TIME*0.4 * LIGHT_MOVE_SPEED) * LIGHT_AMPLITUDE, 1.25);

		float maxdepth=10.0,eps=1.0/RENDERSIZE.y;
		if(uv.x<1.0){ // left side is shadow map
			uv-=0.5; // center left side at 0
			uv*=vec2(PI*2.0,PI); // for spherical projection
			ro=posLight;
			rd=vec3(cos(uv.y)*cos(uv.x),sin(uv.y),cos(uv.y)*sin(uv.x));
			maxdepth=LIGHT_FALLOFF;
		}else{ // right side is quarter sized depth map
	        if(uv.y>0.5){gl_FragColor=vec4(0.0);return;} // a discard here caused garbage
			uv-=vec2(1.5,0.25); // center right side at 0
			uv*=vec2(RENDERSIZE.x/RENDERSIZE.y,2.0);
            // Tunable camera position and movement
			ro=vec3(TIME*LIGHT_MOVE_SPEED-1.0+sin(TIME*0.24*LIGHT_MOVE_SPEED)*0.25,sin(TIME*0.3*LIGHT_MOVE_SPEED),0.88+0.5*sin(TIME*0.34*LIGHT_MOVE_SPEED));
			rd=normalize(vec3(uv,1.0));
            // Tunable lookAt target movement
			rd=lookat(posLight-vec3(0.0,sin(TIME*0.3*LIGHT_MOVE_SPEED)*0.4,sin(TIME*0.2*LIGHT_MOVE_SPEED)*0.3)-ro)*rd;
			eps*=2.0;
		}

		float t=0.0,d; // march to surface
		for(int i=0;i<MAX_MARCH_ITERATIONS_PASS0;i++){ // Use constant MAX_MARCH_ITERATIONS_PASS0
			t+=d=DE(ro+rd*t);
			if(d<eps*t || t>maxdepth)break;
		}

		t=clamp(t/maxdepth,0.0,1.0); // this isn't needed for our unclamped float buffer
		gl_FragColor=vec4(vec3(t),1.0);

	}
	else if (PASSINDEX == 1)	{

		vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

        // Apply Screen Shake from previous shader
        vec2 shake_offset = vec2(sin(TIME * 20.0 + uv.x * 10.0), cos(TIME * 25.0 + uv.y * 12.0)) * SHAKE_STRENGTH;
        uv += shake_offset;

	    vec2 pt=uv*0.5+vec2(0.5,0.0);
		//gl_FragColor = texture2D(BufferA,mod(uv,1.0));return; // Uncomment for debugging BufferA
	    uv-=0.5;
	    uv*=vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
	    vec3 posLight=vec3(TIME * LIGHT_MOVE_SPEED, sin(TIME*0.4 * LIGHT_MOVE_SPEED) * LIGHT_AMPLITUDE, 1.25);
		vec3 ro=vec3(TIME*LIGHT_MOVE_SPEED-1.0+sin(TIME*0.24*LIGHT_MOVE_SPEED)*0.25,sin(TIME*0.3*LIGHT_MOVE_SPEED),0.88+0.5*sin(TIME*0.34*LIGHT_MOVE_SPEED));
		vec3 rd=normalize(vec3(uv,1.0));
		rd=lookat(posLight-vec3(0.0,sin(TIME*0.3*LIGHT_MOVE_SPEED)*0.4,sin(TIME*0.2*LIGHT_MOVE_SPEED)*0.3)-ro)*rd;

	    float maxdepth=10.0,eps=1.0/RENDERSIZE.y,d,t=texture2D(BufferA,mod(pt,1.0)).r*maxdepth;
	    vec4 ts=vec4(0.0),ds=vec4(0.0);
	    for(int i=0;i<MAX_MARCH_ITERATIONS_PASS1_PRIMARY;i++){ // Use constant MAX_MARCH_ITERATIONS_PASS1_PRIMARY
	        t+=d=DE(ro+rd*t);
	        if(d<eps*t){ts=vec4(t,ts.xyz);ds=vec4(d,ds.xyz);}//push
	        if(ts.w>0.0 || t>maxdepth)break;
	    }
	    t=clamp(t,0.0,maxdepth);
        // Apply palette, pulse, and brightness
	    vec3 col=getPaletteColor(t/maxdepth + TIME * 0.05 * PULSE_STRENGTH) * BRIGHTNESS;

	    for(int i=0;i<4;i++){ // This loop has a constant iteration count (4)
	        if(ts.x<0.001)break;
	    	vec3 scol=getPaletteColor(0.4999*ts.x/maxdepth + TIME * 0.05 * PULSE_STRENGTH) * BRIGHTNESS; // Apply palette, pulse, brightness
	   		vec3 p=ro+ts.x*rd;
	    	float d2=isInShadow(ro+rd*ts.x,posLight,eps);
	    	if(d2>=0.0){
				vec3 L=normalize(p-posLight);
	    		float d3=DE(ro+rd*ts.x-L*ds.x); // test in the direction of the light
				scol+=clamp((d3-ds.x*0.75)/ds.x,0.0,1.0)*getPaletteColor(TIME * 0.1 + L.x * 0.2) / (1.0+4.0*d2*d2) * SHADOW_INTENSITY; // Apply palette & shadow intensity
	            if(obj==1)scol+=vec3(0.0,0.12,0.1); // Original color, can be mixed with palette if desired
			}
	        col=mix(scol,col,clamp(ds.x/(eps*ts.x),0.0,1.0));
	        ts=ts.yzwx;ds=ds.yzwx;//pop
	    }
		float dt=2.0*LIGHT_FALLOFF/32.0;
	    maxdepth=t;
		t=max(0.0,length(ro-posLight)-LIGHT_FALLOFF)+dt*rand(rd);
		for(int i=0;i<MAX_MARCH_ITERATIONS_PASS1_SHADOW;i++){ // Use constant MAX_MARCH_ITERATIONS_PASS1_SHADOW
	        d=isInShadow(ro+rd*t,posLight,eps);
	        if(d>=0.0){
				col+=getPaletteColor(TIME * 0.15 + t * 0.01) / (1.0+300.0*d*d) * BRIGHTNESS; // Apply palette & brightness
			}
			t+=dt;
	        if(t>maxdepth)break;
	    }
		col = clamp(col,0.0,1.0); // Final clamp
	    gl_FragColor=vec4(col,maxdepth); // maxdepth is actually scene depth
	}
}