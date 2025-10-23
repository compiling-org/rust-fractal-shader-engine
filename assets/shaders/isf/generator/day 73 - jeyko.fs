/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Raymarching",
        "3D",
        "Psychedelic"
    ],
    "DESCRIPTION": "A raymarched scene with reflections and a post-processing blur, now featuring DMT-like color palettes and pulsing fog/lighting.",
    "CREDIT": "Converted from Shadertoy by jeyko, enhanced by Gemini",
    "ISFVSN": "2.0",
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ],
    "INPUTS": [
        {
            "NAME": "MOUSE",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5],
            "COMMENT": "Mouse position for camera rotation."
        },
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "CameraRotationStrength",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Camera Rot Strength"
        },
        {
            "NAME": "RaymarchIterations",
            "TYPE": "float",
            "DEFAULT": 150.0,
            "MIN": 50.0,
            "MAX": 300.0,
            "LABEL": "Raymarch Iterations"
        },
        {
            "NAME": "MaxRayDistance",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 1.0,
            "MAX": 50.0,
            "LABEL": "Max Ray Distance"
        },
        {
            "NAME": "MinHitDistance",
            "TYPE": "float",
            "DEFAULT": 0.001,
            "MIN": 0.0001,
            "MAX": 0.01,
            "LABEL": "Min Hit Distance"
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "BlurSteps",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 1.0,
            "MAX": 50.0,
            "LABEL": "Blur Steps"
        },
        {
            "NAME": "BlurScale",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.2,
            "LABEL": "Blur Scale"
        },
        {
            "NAME": "ChromaticAberration",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "LABEL": "Chromatic Aberration"
        },
        {
            "NAME": "PaletteMode",
            "TYPE": "long",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 2,
            "LABEL": "Color Palette Mode",
            "DESCRIPTION": "0: Original, 1: Psychedelic (Vibrant), 2: Psychedelic (Warm)"
        },
        {
            "NAME": "PaletteSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Palette Speed",
            "DESCRIPTION": "Controls the speed of color shifts."
        },
        {
            "NAME": "FogPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fog Pulse Strength",
            "DESCRIPTION": "Intensity of color pulsing for the 'fog' attenuation."
        },
        {
            "NAME": "LightingPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Lighting Pulse Strength",
            "DESCRIPTION": "Intensity of color pulsing for materials and glow."
        }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

// ISF Uniforms: RENDERSIZE, TIME, MOUSE are automatically provided.

vec3 attenuation = vec3(1.0);

#define mx (TIME * AnimationSpeed + 20.0 * MOUSE.x / RENDERSIZE.x)

// pmod: modulo function that centers the result around zero
#define pmod(p,x) mod(p,x) - x*0.5

// dmin: returns the vec2 with the smaller x component
#define dmin(a,b) a.x < b.x ? a : b

// rot: 2D rotation matrix
#define rot(x) mat2(cos(x), -sin(x), sin(x), cos(x))


#define modDist 1.0
#define HEIGHT 0.5

const float pi = 3.14159265359; // Using a direct float value for pi
const float tau = (2.0 * pi);

// pal: color palette function, adjusted for ISF TIME
vec3 pal(float a, float b, vec3 c, float d, float e) {
    return ( (a) + (b) * sin(tau * ((c) * (d) + (e)) ) );
}

// fOpUnionStairs: A "stairs" flavored union SDF operation
// much less stupid version by paniq
float fOpUnionStairs(float a, float b, float r, float n) {
	float s = r / n;
	float u = b - r;
	return min(min(a, b), 0.5 * (u + a + abs(mod(u - a + s, 2.0 * s) - s)));
}


float side = 1.0;
vec3 glow = vec3(0.0);
vec3 glowB = vec3(0.0);

vec2 map(vec3 p){
	vec2 d_map = vec2(1000000.0); // Renamed to avoid confusion with 'd' in main

    p.y += sin(p.z + p.x * 4.0 + TIME * AnimationSpeed) * 0.02;
    
    vec3 q = p;
    
    vec2 id = floor(p.xz);
    p.xz = pmod(p.xz, modDist);
    vec3 j = p;
    
    float dBall = length(p) - 0.14;
    
    d_map = dmin(d_map, vec2(dBall, 0.0));
    
    const float colW = 0.1; // Changed to const
    
    j.xz = abs(j.xz) - colW;
    vec3 k = j;
    j = abs(j);
    vec3 i = j;
    j.y -= HEIGHT * 0.95;
    
    for (int idx = 0; idx < 3; idx++){ // Changed 'i' to 'idx' to avoid conflicts
        j = abs(j);
        
        j.x -= 0.05;
        j.xy *= rot(-0.25 * pi + sin(TIME * AnimationSpeed + id.y * 0.1) - 0.34 );
        j.xz *= rot(-0.25 * pi);
    }
    
    float dCol = min(max(j.x, j.z), max(k.x, k.z) );
    
    d_map = dmin(d_map, vec2(dCol, 1.0));
    
    float dGoldA = min(length(j.xz), length(k.xz) ) - 0.01;
    
    d_map = dmin(d_map, vec2(dGoldA, 2.0));
    
    i = abs(i);
    i.xy *= rot(0.4);
    i.xz *= rot(0.2);
    i = abs(i);
    i.xz *= rot(0.6 * pi);
    
    dGoldA = length(i.xy) - 0.03;
    d_map = dmin(d_map, vec2(dGoldA, 2.0));
    
    p.xz = pmod(p.xz, 0.2);
    
    p.y -= 0.3;
    p = abs(p) - 0.001;
    
    float dGlow = min(
    	length(p.xy) - 0.1,
    	length(p.zy) - 0.1
    );
    
    float attenC = pow(abs( sin(q.x + TIME * AnimationSpeed + sin(q.z * 0.1 + TIME * AnimationSpeed) + q.z) ), 40.0);

    // --- DMT-like Color Palettes for glowB (main scene background glow) ---
    float dynamic_time_pulse = TIME * AnimationSpeed * PaletteSpeed;
    vec3 col_palette;

    if (PaletteMode == 0) { // Original Palette
        col_palette = pal(0.2, 0.8, vec3(0.1 + pow(abs(sin(TIME * 1.0)), 40.0) * 0.005, 2.2, 0.3), 0.5 + sin(TIME) * 0.005, 0.5 - attenC * 0.6);
    } else if (PaletteMode == 1) { // Psychedelic Palette 1 (Vibrant Blues/Pinks/Greens)
        vec3 p_c = vec3(0.1 + sin(dynamic_time_pulse * 0.5) * 0.1, 0.2 + cos(dynamic_time_pulse * 0.7) * 0.1, 0.3 + sin(dynamic_time_pulse * 0.9) * 0.1);
        col_palette = pal(0.5, 0.5, p_c, 0.8 + sin(dynamic_time_pulse * 0.3) * 0.2, 0.5 - attenC * 0.6);
        col_palette = pow(col_palette, vec3(1.5, 1.2, 1.8)); // Intensify colors
    } else if (PaletteMode == 2) { // Psychedelic Palette 2 (Fiery/Warm Tones)
        vec3 p_c = vec3(0.5 + sin(dynamic_time_pulse * 0.4) * 0.2, 0.1 + cos(dynamic_time_pulse * 0.6) * 0.1, 0.05 + sin(dynamic_time_pulse * 0.8) * 0.05);
        col_palette = pal(0.7, 0.3, p_c, 0.6 + cos(dynamic_time_pulse * 0.2) * 0.1, 0.5 - attenC * 0.6);
        col_palette = pow(col_palette, vec3(1.8, 1.5, 1.2)); // Intensify colors
    }

    glowB += exp(-dGlow * 20.0) * col_palette * attenuation * 4.0 * attenC;    
    
    q.xz = pmod(q.xz, 0.2);
    
    float dCeilings = q.y + HEIGHT;
    dCeilings = min(dCeilings, -q.y + HEIGHT);
    
    dCeilings += max(exp(-q.x * 100.0), exp(-q.z * 100.0)) * 0.0000004;
    
    d_map = dmin(d_map, vec2(dCeilings, 0.0));
    
    d_map.x *= 0.5;
    return d_map;
}


vec2 march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit){
	vec2 d_march = vec2(1000000.0); // Renamed to avoid confusion

    // Convert RaymarchIterations to a regular integer for the loop limit
    int maxRaymarchSteps = int(RaymarchIterations); 

    p = ro; t = 0.0; hit = false;
    
    for(int i = 0; i < maxRaymarchSteps ; i++){ // Use the int here
    	d_march = map(p);
        d_march.x *= side;
        glow += exp(-max(d_march.x, 0.0) * 80.0) * attenuation * 1.0;
        
        if(d_march.x < MinHitDistance || t > MaxRayDistance){ // Using tunable inputs
        	hit = true;
            break;
        }
        t += d_march.x;
        p += rd * d_march.x;
    }
    return d_march;
}

vec3 getNormal(vec3 p){
	vec2 t_eps = vec2(0.001, 0.0); 
    return normalize(vec3(
    	map(p - t_eps.xyy).x - map(p + t_eps.xyy).x ,
    	map(p - t_eps.yxy).x - map(p + t_eps.yxy).x ,
    	map(p - t_eps.yyx).x - map(p + t_eps.yyx).x 
    ));
}

void main() {
	if (PASSINDEX == 0)	{ // BufferA (Main Scene Render)

	    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	
	    uv *= 1.0 + length(uv) * 0.2;
	    
	    vec3 col = vec3(0.0);
	    vec3 ro = vec3(0.0);
	    vec3 rd = normalize(vec3(uv, 1.0));
	    vec3 p; // Position in raymarch
	
	    ro.y += sin(TIME * AnimationSpeed) * 0.04;    
	    ro.z += mx;
	    
	    rd.xy *= rot(sin(TIME * AnimationSpeed * 0.7) * CameraRotationStrength); // Using CameraRotationStrength input
	    
	    float t; // Raymarch distance
        bool hit;
	    vec2 d_result; // Declare d_result here to store the output of march
	    float tB = 0.0;
	    
        // --- Fog/Attenuation Pulsing Color ---
        float time_pulse_fog = TIME * AnimationSpeed * PaletteSpeed * 0.5; // Slightly different speed for fog
        vec3 fog_pulse_color = pal(0.5, 0.5, vec3(0.5 + sin(time_pulse_fog * 0.8), 0.5 + cos(time_pulse_fog * 1.0), 0.5 + sin(time_pulse_fog * 1.2)), 0.5, 0.5);
        fog_pulse_color = mix(vec3(1.0), fog_pulse_color, FogPulseStrength); // Blend with white based on input
        attenuation *= fog_pulse_color; // Apply overall fog pulsing at the start

	    // Reflection/refraction iterations. This loop always runs 2 times.
	    for(int i = 0; i < 2; i++){
	    	d_result = march(ro, rd, p, t, hit); // Assign march result to d_result
	    	
	        if(i == 0) {
	            tB = t;
            }
	        
	        vec3 n = getNormal(p) * side;
	        
	        if(d_result.y == 0.0){
	            // Material type 0.0, no specific effect currently
	        }
	        if(d_result.y == 2.0){ // Gold-like material
                float time_pulse_light = TIME * AnimationSpeed * PaletteSpeed * 1.5; // Faster pulse for lighting
	            vec3 c_gold_base = vec3(0.7, 0.6, 0.5) * 4.0;
	            // Add a pulse to the gold color
	            vec3 c_gold_pulse = pal(0.5, 0.5, vec3(0.1, 0.2, 0.05), 0.5 + sin(time_pulse_light * 1.5), 0.5);
	            vec3 c = mix(c_gold_base, c_gold_pulse * 2.0, LightingPulseStrength); // Blend based on input
	            
	            if(i == 0)
	                glow *= c * 1.0;
	            
	            glow *= c * 1.0;
	        	attenuation *= c * 0.6;
	        }
	        if(d_result.y == 1.0){ // Column-like material (refractive)
                float time_pulse_light = TIME * AnimationSpeed * PaletteSpeed * 1.7; // Faster pulse for lighting
	        	vec3 c_column_base = vec3(0.11, 0.1, 0.1);
	            // Add a pulse to the column color
	            vec3 c_column_pulse = pal(0.5, 0.5, vec3(0.1, 0.05, 0.2), 0.5 + cos(time_pulse_light * 1.7), 0.5);
	            vec3 c = mix(c_column_base, c_column_pulse * 1.5, LightingPulseStrength); // Blend based on input

	            if(i == 0)
	                glow *= c;
	            glow *= c;
	        	rd = refract(rd, n, 0.4 + sin(p.x * 40.0 + p.y * 40.0 + TIME * AnimationSpeed) * 0.2);
	        	attenuation *= c * 3.0; // Apply attenuation for this material
	        } else { // Default to reflection for other materials
	        	rd = reflect(rd, n);
	        }
	        
	        ro = p + rd * 0.05;
	        
	        attenuation *= 0.7; // General attenuation per step
	    }
	    
	    glow = pow(glow, vec3(1.1)) * GlowIntensity; // Using GlowIntensity input
	    
        // --- Final Glow Pulsing Color ---
        float time_pulse_final_glow = TIME * AnimationSpeed * PaletteSpeed * 2.0; // Even faster pulse
        vec3 final_glow_pulse = pal(0.5, 0.5, vec3(0.8 + sin(time_pulse_final_glow * 2.0), 0.9 + cos(time_pulse_final_glow * 2.2), 0.7 + sin(time_pulse_final_glow * 2.4)), 0.5, 0.5);
        
		col += glow * 0.02 * mix(vec3(1.0), final_glow_pulse, LightingPulseStrength); // Blend glow with pulsed color
		col += glowB * 0.01 * mix(vec3(1.0), final_glow_pulse, LightingPulseStrength); // Apply to glowB as well
	    
	    col = clamp(col, 0.0, 10.0);
	    
	    col = mix(col, vec3(0.7), smoothstep(0.0, 1.0, tB * 0.05));
	    
	    uv *= 0.8;
	    col *= 1.0 - dot(uv, uv);
	    
	    gl_FragColor = vec4(col,1.0);
	}
	else if (PASSINDEX == 1)	{ // Main Pass (Post-processing blur)

		vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
		vec2 uvn = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.xy;
	    
	    // Radial blur
        // Convert BlurSteps to a regular integer for the loop limit
	    int blurStepsCount = int(BlurSteps); 
        
	    float scale = 0.0 + dot(uvn, uvn) * BlurScale; // Using BlurScale input
	    float chromAb = dot(uvn, uvn) * ChromaticAberration; // Using ChromaticAberration input
	    
	    vec2 offs = vec2(0.0); // Removed iChannel1 texture lookup for offset
	    
	    vec4 radial = vec4(0.0);
	    for(int i = 0; i < blurStepsCount; i++){ // Use the int here
	        scale *= 0.99;
	        // The 'target' UV is based on the current iteration's offset
            // and the base fragment UV.
	        vec2 current_target_uv = uv + offs; 

	        // Pre-compute the 'mod' for UVs to ensure they are within [0,1]
            // before passing to IMG_NORM_PIXEL, as required by some GLSL ES implementations/ISF.
	        vec2 target_uv_x_offset = mod(current_target_uv + chromAb / RENDERSIZE.xy, 1.0);
            vec2 target_uv_y_offset = mod(current_target_uv, 1.0);
            vec2 target_uv_z_offset = mod(current_target_uv - chromAb / RENDERSIZE.xy, 1.0);

	        offs -= normalize(uvn) * scale / float(blurStepsCount); // Ensure division by float
	        
	        // Correct sampling of BufferA (previous pass)
	    	radial.x += IMG_NORM_PIXEL(BufferA, target_uv_x_offset).x;
	    	radial.y += IMG_NORM_PIXEL(BufferA, target_uv_y_offset).y;
	    	radial.z += IMG_NORM_PIXEL(BufferA, target_uv_z_offset).z;
	    }
	    
	    radial /= float(blurStepsCount); // Use blurStepsCount here
	    
	    // Apply mod to UV for the final display sample
	    vec2 final_uv = mod(uv, 1.0);
	    gl_FragColor = IMG_NORM_PIXEL(BufferA, final_uv) * 0.8 + radial * 1.5;
	    
	    gl_FragColor = mix(gl_FragColor, smoothstep(0.0, 1.0, gl_FragColor), 0.7);
	    gl_FragColor *= 0.6;
	    gl_FragColor = pow(gl_FragColor, vec4(0.45));
	}
}