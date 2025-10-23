/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Raymarching",
        "3D",
        "Abstract"
    ],
    "DESCRIPTION": "A kaleidoscopic fractal explorer with tunable psychedelic effects, converted from https://www.shadertoy.com/view/ws2cWR. Enhanced with more morphing, camera controls, and ultra-diverse colored palettes. Fixed loop constant errors and explicit type casting for min. Addressed 'invalid init declaration' for loop.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "TimeMultiplier",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "FractalIterations",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 2.0,
            "MAX": 8.0,
            "LABEL": "Fractal Iterations"
        },
        {
            "NAME": "FractalMorphStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Morph Strength"
        },
        {
            "NAME": "FractalMorphSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Fractal Morph Speed"
        },
        {
            "NAME": "CameraZoomSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Camera Zoom Speed"
        },
        {
            "NAME": "CameraShakeAmount",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Camera Shake Amount"
        },
        {
            "NAME": "GlowIntensityMultiplier",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Glow Intensity Multiplier"
        },
        {
            "NAME": "PaletteSelector",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 4.99,
            "LABEL": "Palette Selection"
        },
        {
            "NAME": "PaletteCustomFreq",
            "TYPE": "color",
            "DEFAULT": [0.7, 0.3, 0.7, 1.0],
            "LABEL": "Custom Palette Freq"
        },
        {
            "NAME": "PalettePhaseOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Palette Phase Offset"
        },
        {
            "NAME": "PalettePulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Palette Pulse Strength"
        },
        {
            "NAME": "HueShift",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Global Hue Shift"
        },
        {
            "NAME": "BloomStrength",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Bloom Strength"
        },
        {
            "NAME": "ChromaticAberration",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Chromatic Aberration"
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

#define pi acos(-1.0)
#define tau (2.0*pi)

#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))
#define dmin(a,b) a.x < b.x ? a : b

#define pmod(p,z) mod(p,z) - 0.5*z

// Define max iterations for compile-time loops
const int MAX_FRACTAL_ITERATIONS_CONST = 8; // Max value for FractalIterations input
const int MAX_MARCH_ITERATIONS_CONST = 120; // Enough for 60 + max FRAMEINDEX (which is float)

// Define radial blur steps as a const float for strict compilers
const float RADIAL_BLUR_STEPS_CONST = 26.0;


// Custom color palette function for diversity
vec3 colorPalette(float t, vec3 freq, float phase_offset, float pulse_strength) {
    vec3 pulsed_freq = freq * (1.0 + sin(t * 7.0 + freq.r * 10.0) * pulse_strength * 0.2);
    pulsed_freq.y *= (1.0 + sin(t * 8.0 + freq.g * 10.0) * pulse_strength * 0.2);
    pulsed_freq.z *= (1.0 + sin(t * 9.0 + freq.b * 10.0) * pulse_strength * 0.2);

    return (0.5 + 0.5 * cos(pulsed_freq * t + phase_offset));
}


float sdBox(vec3 p, vec3 s){
	p = abs(p) - s;
 	return max(p.x,max(p.y,p.z));
}

vec3 glow = vec3(0);

vec2 map(vec3 p){
	vec2 d = vec2(10e7);
 	vec3 j = p;
 	p.z = pmod(p.z, 10.0);

    // Fractal morphing parameters
    float morph_time = TIME * FractalMorphSpeed;
    float morph_factor_main = sin(morph_time * 0.5) * FractalMorphStrength * 0.5 + (1.0 - FractalMorphStrength * 0.5);
    float morph_factor_rot = cos(morph_time * 0.7) * FractalMorphStrength * 0.2;

 	for(int i = 0; i < MAX_FRACTAL_ITERATIONS_CONST; i++){
        if (float(i) >= FractalIterations) break;
 		p = abs(p);
  		p.x -= 2.0 * morph_factor_main;
  		p.xy *= rot(0.25*pi + morph_factor_rot);
  		
  		p.y -= 1.0 + sin(morph_time * 1.2 + float(i) * 0.3) * FractalMorphStrength * 0.3;
  		p.z += 0.2 + cos(morph_time * 1.5 + float(i) * 0.4) * FractalMorphStrength * 0.2;
 	}
 	
 	p = abs(p);
 	p.xy -= 0.3 * morph_factor_main;
 	
 	p = abs(p);
 	p.y -= 0.2 * morph_factor_main;
 	p = abs(p);
 	p.x -= 0.5 * morph_factor_main;
 	p = abs(p);
 	p.x -= 0.5 * morph_factor_main;
 	
 	float attd = pow(abs(sin(j.z*0.5 + TIME*0.4)), 10.0);
 	float atte = pow(abs(sin(j.z*0.5 + TIME*10.4)), 100.0)*attd;
 	
 	vec3 q = p;
 	
 	q.x -= 0.4;
 	q = abs(q);
 	q.x -= 0.4;

 	q.y -= 0.4;
 	
 	vec3 sz = vec3(0.02,0.02,20.5);
 	float dB = sdBox(q, sz)*1.2;
 	
 	d = dmin(d, vec2(dB, 2.0));
 	float att = pow(abs(sin(j.z*0.1 + TIME*0.5 + sin(j.x*4.0)*0.2)), 400.0);
 	
 	glow += GlowIntensityMultiplier * 0.9/(0.0001 + dB*dB*10.0)*vec3(2.8 + attd*8.0,0.9,0.7)*att;
 	
 	p.z -= 1.5;
 	
 	p = abs(p);
 	p.z -= 0.2;
 	float dC = sdBox(p, vec3(0.02,2000.7,0.02));
 	d = dmin(d, vec2(dC, 2.0));
 	float attb = pow(abs(sin(p.x*0.4 + TIME - 1.0)), 10.0);
	glow += GlowIntensityMultiplier * 2.9/(0.001 - - atte*2.0 + dC*dC*400.0)*vec3(1.0,1.0,1.7)*attb;
 	
 	p.x -= 0.4;
 	p = abs(p);
 	p.x += 0.1;
 	p.y -= 0.2;
 	p.xy *= rot(-0.25*pi);
 	p.z -= 1.0;
 	p.x -= 0.3;
 	
 	float dD = sdBox(p, vec3(0.02,1.7,0.02));
 	d = dmin(d, vec2(dD, 2.0));
 	float attc = pow(abs(sin(p.y*0.4 + TIME + 4.0)), 10.0);
 	
 	glow += GlowIntensityMultiplier * 0.7/(0.001 + dD*dD*(60.0 - atte*59.0))*sin(vec3(0.1,0.4 - atte*0.6,1.1) + vec3(0,0,attd*0.0))*attc*vec3(1.0,1.0,1.0)*0.8;
 	
 	d.x = abs(d.x) + 0.002;
 	
 	d.x *= 0.4;
 	
 	return d;
}
int it = 0; // Declared globally.

vec2 march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit){
	vec2 d = vec2(10e7);
 	
 	t = 0.0; hit = false; p = ro;
 	
 	// Calculate dynamic target (float)
    float dynamic_march_limit = 60.0 + min(float(FRAMEINDEX), 60.0);

    // ***FIXED: Initialize 'it' before the for loop and remove init from loop declaration***
    it = 0;
 	for(; it < MAX_MARCH_ITERATIONS_CONST; it++){ // No 'int it = 0;' here, just a semicolon
        if (float(it) >= dynamic_march_limit) break;
 		d = map(p);
  		
  		if(d.x < 0.001){
  			hit = true;
  			break;
  		}
 		t += d.x;
  		p = ro + rd*t;
 	}
 	
 	return d;
}

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
 	vec3 dir = normalize(lookAt - ro);
 	vec3 right = normalize(cross(vec3(0,1,0), dir));
 	vec3 up = normalize(cross(dir, right));
 	return normalize(dir + (right*uv.x + up*uv.y)*0.6);
}

vec3 getNormal(vec3 p){
 	vec2 t = vec2(0.001,0.0);
 	return -normalize(vec3(
 		map(p - t.xyy).x - map(p + t.xyy).x,
 		map(p - t.yxy).x - map(p + t.yxy).x,
 		map(p - t.yyx).x - map(p + t.yyx).x
 	));
}

// Helper for Hue Rotation
vec3 huerot(vec3 color, float hue) {
    vec3 k = vec3(0.57735); // 1/sqrt(3)
    float cosA = cos(hue);
    float sinA = sin(hue);
    mat3 rotMat = mat3(
        vec3(cosA + (1.0 - cosA) * k.x * k.x, (1.0 - cosA) * k.x * k.y - sinA * k.z, (1.0 - cosA) * k.x * k.z + sinA * k.y),
        vec3((1.0 - cosA) * k.y * k.x + sinA * k.z, cosA + (1.0 - cosA) * k.y * k.y, (1.0 - cosA) * k.y * k.z - sinA * k.x),
        vec3((1.0 - cosA) * k.z * k.x - sinA * k.y, (1.0 - cosA) * k.z * k.y + sinA * k.x, cosA + (1.0 - cosA) * k.z * k.z)
    );
    return rotMat * color;
}


void main() {
	if (PASSINDEX == 0)	{

	 	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
	
	 	vec3 col = vec3(0);
	 	
		uv *= 1.0 - dot(uv,uv)*0.06;
	 	
	 	vec3 ro = vec3(0);
	 	
        ro.x += sin(TIME * 0.3) * CameraShakeAmount;
        ro.y += cos(TIME * 0.5) * CameraShakeAmount;
        ro.z += TIME * CameraZoomSpeed;
	 	
	 	vec3 lookAt = vec3(0);
	 	
	 	lookAt.z = ro.z + 2.0;
	 	
	 	vec3 rd = getRd(ro, lookAt, uv);
	 	
	 	vec3 p; float t; bool hit;
	 	
	 	vec2 d = march(ro, rd, p, t, hit);
	 	
	 	if (d.x < 0.001){
	 		vec3 n = getNormal(p);
	  		vec3 l = normalize(vec3(1.0));
	  		float dnh = max(dot(n,normalize(l - rd)),0.0);
	  		float diff = max(dot(n,l),0.0);
	  		float spec = pow(max(dot(n,normalize(l - rd)),0.0), 20.0);
	  		float fres = pow(1.0 - dnh, 5.0);
	 		
	 		// Applying hit surface color based on palette
            vec3 surface_color;
            int selector_int = int(floor(PaletteSelector));
            if (selector_int == 0) { // Default/Original
                surface_color = colorPalette(p.x + p.y + p.z, PaletteCustomFreq.rgb, PalettePhaseOffset, PalettePulseStrength);
            } else if (selector_int == 1) { // Warm psychedelic
                surface_color = colorPalette(p.x + p.y + p.z, vec3(2.0, 1.0, 0.5), PalettePhaseOffset, PalettePulseStrength);
            } else if (selector_int == 2) { // Cool / Cyan-Magenta
                surface_color = colorPalette(p.x + p.y + p.z, vec3(0.5, 2.5, 1.5), PalettePhaseOffset, PalettePulseStrength);
            } else if (selector_int == 3) { // Rainbow / Fast cycle
                surface_color = colorPalette(p.x + p.y + p.z, vec3(10.0, 5.0, 2.0), PalettePhaseOffset, PalettePulseStrength);
            } else if (selector_int == 4) { // Grungy / Earthy
                surface_color = colorPalette(p.x + p.y + p.z, vec3(0.2, 0.7, 0.9), PalettePhaseOffset, PalettePulseStrength);
            } else {
                surface_color = colorPalette(p.x + p.y + p.z, PaletteCustomFreq.rgb, PalettePhaseOffset, PalettePulseStrength);
            }
            
            // Basic lighting
	  		col += surface_color * (diff * 0.5 + spec * 0.3 + fres * 0.2);
	 		
	 	}
	 	
	 	col += glow * 0.001;
	 	col = mix(col, vec3(0.1,0.1,0.16 + (uv.x + 0.5)*0.1)*0.02, smoothstep(0.0,1.0,t*0.04 ));
	 	
	 	col = max(col, 0.0);
	 	
        // Apply global hue shift
        col = huerot(col, HueShift * tau);

	 	gl_FragColor = vec4(col,1.0);
	}
	else if (PASSINDEX == 1)	{

		vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
		vec2 uvn = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.xy;
	 	
		gl_FragColor = vec4(0.0);
	 	// Radial blur
	 	float scale = 0.0 + pow(dot(uvn,uvn),1.1)*BloomStrength;
	 	float chromAb = pow(length(uv - 0.5),1.5)*ChromaticAberration;
	 	vec2 offs = vec2(0.0);
	 	vec4 radial = vec4(0.0);
	 	for(float i = 0.0; i < RADIAL_BLUR_STEPS_CONST; i++){
	 	 	scale *= 0.97;
	 	 	vec2 target = uv + offs;
	 	 	offs -= normalize(uvn)*scale/RADIAL_BLUR_STEPS_CONST;
	 	 	
            // Re-confirming pre-calculation for IMG_NORM_PIXEL robustness
            vec2 texCoordR = mod(target + vec2(chromAb*1.4)/RENDERSIZE.xy, 1.0);
            vec2 texCoordG = mod(target, 1.0);
            vec2 texCoordB = mod(target - vec2(chromAb*1.0)/RENDERSIZE.xy, 1.0);

	  		radial.r += IMG_NORM_PIXEL(BufferA, texCoordR).x;
	  		radial.g += IMG_NORM_PIXEL(BufferA, texCoordG).y;
	  		radial.b += IMG_NORM_PIXEL(BufferA, texCoordB).z;
	 	}
	 	radial /= RADIAL_BLUR_STEPS_CONST;
	 	
	 	gl_FragColor += radial;
	 	
	 	gl_FragColor.a *= 1.0 - smoothstep(0.0,1.0,dot(uvn,uvn))*0.0;
	 	
	 	gl_FragColor *= 1.0 - dot(uvn,uvn)*2.0;
	 	
	 	gl_FragColor = max(gl_FragColor, 0.0);
	 	gl_FragColor = pow(gl_FragColor, vec4(0.4545));
	}

}