/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Raymarching",
        "3D",
        "Abstract"
    ],
    "DESCRIPTION": "A kaleidoscopic fractal explorer with tunable psychedelic effects, converted from Shadertoy. Enhanced with more morphing, camera controls, and ultra-diverse colored palettes.",
    "IMPORTED": {
        "inputImage": {
            "NAME": "inputImage",
            "PATH": "f735bee5b64ef98879dc618b016ecf7939a5756040c2cde21ccb15e69a6e1cfb.png"
        }
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
            "NAME": "ZoomLevel",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Base Zoom"
        },
        {
            "NAME": "ZoomPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Zoom Pulse Strength"
        },
        {
            "NAME": "CamOrbitX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Orbit X"
        },
        {
            "NAME": "CamOrbitY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Orbit Y"
        },
        {
            "NAME": "CamRoll",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Camera Roll"
        },
        {
            "NAME": "LookAtOffsetZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "LookAt Z Offset"
        },
        {
            "NAME": "ColorShiftSpeed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Color Shift Speed"
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
            "NAME": "GlowATint",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0],
            "LABEL": "Glow A Tint"
        },
        {
            "NAME": "GlowBTint",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0],
            "LABEL": "Glow B Tint"
        },
        {
            "NAME": "GlowCTint",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0],
            "LABEL": "Glow C Tint"
        },
        {
            "NAME": "GlowDTint",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0],
            "LABEL": "Glow D Tint"
        },
        {
            "NAME": "FractalDetail",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Fractal Detail"
        },
        {
            "NAME": "FractalMorphSpeed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Morph Speed"
        },
        {
            "NAME": "SpecularPower",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 1.0,
            "MAX": 100.0,
            "LABEL": "Specular Power"
        },
        {
            "NAME": "FresnelPower",
            "TYPE": "float",
            "DEFAULT": 5.0,
            "MIN": 1.0,
            "MAX": 10.0,
            "LABEL": "Fresnel Power"
        },
        {
            "NAME": "OverallBrightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Overall Brightness"
        },
        {
            "NAME": "Contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
            "NAME": "UV_Distortion",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "UV Distortion"
        },
        {
            "NAME": "CameraShake",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Camera Shake"
        },
        {
            "NAME": "PaletteHueOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Palette Hue Offset"
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
            "NAME": "PaletteFreqColor",
            "TYPE": "color",
            "DEFAULT": [0.8, 0.4, 0.7, 1.0],
            "LABEL": "Custom Palette Freq"
        },
        {
            "NAME": "PalettePhase",
            "TYPE": "float",
            "DEFAULT": 2.4,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Palette Phase"
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
            "NAME": "FinalHueRotate",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Final Hue Rotate"
        }
    ]
}
*/

precision highp float;

#define PI 3.14159265359 // Define PI explicitly

#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv, float roll_angle){
	vec3 dir = normalize(lookAt - ro);
	vec3 right = normalize(cross(vec3(0,1,0), dir));
	vec3 up = normalize(cross(dir, right));
    
    // Apply camera roll
    mat2 rollRot = rot(roll_angle);
    right.xy = rollRot * right.xy;
    up.xy = rollRot * up.xy;

	return normalize(dir + uv.x*right + uv.y*up);
}

vec3 glowA = vec3(0);
vec3 glowB = vec3(0);
vec3 glowC = vec3(0);
vec3 glowD = vec3(0);


#define ITERS 4
#define ZOOM_BASE 1.0 
vec4 WS_values = vec4(1.0, 1.0, 1.0, 1.0);

float id;
float sd(vec3 q){
	#define repD 3.97
    id = floor(q.z/repD);
    q.z = mod(q.z, repD) - 0.5*repD;
    q.xy -= 2.;
    q.xy = mod(q.xy, 4.) - 0.5*4.;
    vec4 p = vec4(q.xyz, 1.);
    
    // Dynamic Fractal Parameters
    float morphFactor = sin(TIME * FractalMorphSpeed * 0.7) * 0.2 + 0.8;
    vec4 c = vec4(0.9,0.77,0.4,0.2) * FractalDetail * morphFactor; 
    vec4 u = vec4(0.4,0.54,0.7,0.4) * FractalDetail * morphFactor;
    
    for(int i = 0; i < ITERS; i++){
        p.xyz = abs(p.xyz) - c.xyz;
        float dpp = dot(p.xyz,p.xyz);
        
        // More complex transformation with time/iteration dependency
        float iterationMorph = sin(TIME * FractalMorphSpeed + float(i) * 0.5) * 0.1;
        p=p*(1.5 + u)/clamp(dpp,.4 + iterationMorph,.9 + iterationMorph)- mix(p,c,0.9)*0.9;
        
        if(i==1)
            p.xy += 0.4 + sin(TIME * FractalMorphSpeed * 1.2) * 0.1;
        
        p.xy *= rot(-0.1 + sin(id)*0.9 * TimeMultiplier + sin(TIME * FractalMorphSpeed * 0.5) * 0.1);
        
        p.xz *= rot(0.9 + cos(TIME * FractalMorphSpeed * 0.8) * 0.1);
        
        if (i == 0) WS_values.x = p.w;
        else if (i == 1) WS_values.y = p.w;
        else if (i == 2) WS_values.z = p.w;
        else if (i == 3) WS_values.w = p.w;
    }
    
    p.xyz = abs(p.xyz);
    
    p.xz *= rot(0.2);
    float fr = max(p.x - 0., max(p.y - 2.4 + sin(id)*0.7, p.z - 2.9))/p.w;
    float d = fr;
    
	return d*0.5;
}

float D;
vec2 map(vec3 p){
	vec2 d = vec2(10e5);
    
    d.x = min(d.x, sd(p));
    return d;
}

vec2 march(vec3 ro, vec3 rd,inout vec3 p,inout float t,inout bool hit){
	vec2 d = vec2(10e5);
    hit = false;
    p = ro;
    t = 0.;
    for(int i = 0; i < 90; i++){
    	d = map(p);
        d.x *= D;
        
        glowA += exp(-d.x*20.)*WS_values.x * GlowIntensity;
        glowB += exp(-d.x*05.)*WS_values.y * 0.5 * GlowIntensity;
        glowC += exp(-d.x*10.)*WS_values.z * GlowIntensity;
        glowD += exp(-d.x*50.)*WS_values.w * 0.7 * GlowIntensity;

        if (d.x < 0.001){
        	hit = true;
            break;
        }
        t += d.x;
        p = ro + rd*t;
    }
    
    return d;
}

vec3 getNormal(vec3 p){
	vec2 t = vec2(0.0002, 0);
	return normalize(
    	map(p).x - vec3(
    		map(p - t.xyy).x,
    		map(p - t.yxy).x,
    		map(p - t.yyx).x
    	)
    );
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

// Color Palette function with dynamic parameters and pulse
vec3 colorPalette(float a, float b, vec3 c, float d, float e, float pulse_strength) {
    // Add pulsing to a and b
    float pulsed_a = a + sin(TIME * 5.0 + e) * pulse_strength * 0.5;
    float pulsed_b = b + cos(TIME * 6.0 + e) * pulse_strength * 0.5;

    // Add pulsing to individual frequency components
    vec3 pulsed_c = c * (1.0 + sin(TIME * 7.0 + c.r * 10.0) * pulse_strength * 0.2);
    pulsed_c.y *= (1.0 + sin(TIME * 8.0 + c.g * 10.0) * pulse_strength * 0.2);
    pulsed_c.z *= (1.0 + sin(TIME * 9.0 + c.b * 10.0) * pulse_strength * 0.2);

    return (pulsed_a + pulsed_b*sin(pulsed_c*d + e));
}

void main() {

    vec4 S = texture2D(inputImage, vec2(0.01,0.0)); 

    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    uv *= 1. + dot(uv,uv)*UV_Distortion;
    
    vec3 col = vec3(0);
    
    D = mix(0.8,1.,texture2D(inputImage, mod(20.0*uv*256.0 + TIME*2.0,1.0)).x); 
    
    vec3 ro = vec3(0);
    ro.x += sin(TIME * 0.5) * CameraShake;
    ro.y += cos(TIME * 0.7) * CameraShake;

    float m_time = TIME * TimeMultiplier;
    
    // Base camera movement
    ro.x += sin(m_time * 0.2) * 0.25 * ZoomLevel; 
    ro.z += m_time + 14.0 * ZoomLevel;
    
    // New Camera Controls: Orbit and LookAt offset
    ro.x += CamOrbitX;
    ro.y += CamOrbitY;

    // Zoom Pulse
    float currentZoomLevel = ZoomLevel + sin(m_time * 3.0) * ZoomPulseStrength;
    ro.z += currentZoomLevel; 


    vec3 lookAt = vec3(0);
    lookAt.z = ro.z + 1.0;
    lookAt.y += sin(m_time * 0.8) * LookAtOffsetZ; // Oscillating LookAt

    // Pass CamRoll to getRd
    vec3 rd = getRd(ro, lookAt, uv, CamRoll * PI); // Convert turns to radians
    
    vec3 p;float t; bool hit;
    
    vec2 d_march = march(ro, rd, p, t, hit);
    
    float currentHueOffset = PaletteHueOffset + TIME * ColorShiftSpeed;
    
    // Multiple Palettes Logic: Use floor for float PaletteSelector
    vec3 pal_freq_base;
    int selector_int = int(floor(PaletteSelector)); // Correctly convert float to int for comparison
    
    if (selector_int == 0) { // Default/Original
        pal_freq_base = PaletteFreqColor.rgb; // Use custom color if selected
    } else if (selector_int == 1) { // Warm psychedelic
        pal_freq_base = vec3(2.0, 1.0, 0.5);
    } else if (selector_int == 2) { // Cool / Cyan-Magenta
        pal_freq_base = vec3(0.5, 2.5, 1.5);
    } else if (selector_int == 3) { // Rainbow / Fast cycle
        pal_freq_base = vec3(10.0, 5.0, 2.0);
    } else if (selector_int == 4) { // Grungy / Earthy
        pal_freq_base = vec3(0.2, 0.7, 0.9);
    } else { // Fallback for values outside 0-4 range
        pal_freq_base = PaletteFreqColor.rgb; 
    }
    
    float pal_phase = PalettePhase;

    if(hit){
		vec3 n = getNormal(p);
        vec3 lD = normalize(vec3(1));
        vec3 h = normalize(lD - rd);
        float diff = max(dot(n, lD),0.);
        float spec = pow(max(dot(n, h),0.), SpecularPower);
        float fres = pow(1. - max(dot(n, -rd),0.), FresnelPower);
        
        vec3 CA = WS_values.x * vec3(0.6,0.2,0.7) * 0.01;
        vec3 CB = WS_values.y * vec3(0.6,0.2,0.7) * 0.01;
        vec3 CC = WS_values.z * vec3(0.6,0.2,0.7) * 0.01;
        
        vec3 CD = 0.05 * WS_values.z * colorPalette(0.2, 0.6, pal_freq_base, 5.9 + currentHueOffset, pal_phase + S.x, PalettePulseStrength);
        
        vec3 C = CA + CB + CC + CD;
        
        col += diff * 0.2;
        col += pow(CD*0.9,vec3(2.0));
        col += spec * 0.5;
        col += fres * 0.5;
    } else {
        // This shader doesn't have a background/environment rendering function outside the march.
        // It will typically be black if no surface is hit, unless glow effects are strong enough.
    }
    
    vec3 G = vec3(0);
    // Apply Glow Tints
    G += pow(glowD*0.004,vec3(1.0))*colorPalette(0.2, 0.6, pal_freq_base, 0.6 + currentHueOffset, pal_phase, PalettePulseStrength) * GlowDTint.rgb;
    G -= pow(glowC*0.005, vec3(1.1))*colorPalette(0.2, 0.4, pal_freq_base, 9.6 + currentHueOffset, pal_phase, PalettePulseStrength) * GlowCTint.rgb;
    
    G += glowB*0.002 *colorPalette(0.2, 0.6, pal_freq_base, 5.99 - sin(TIME), pal_phase, PalettePulseStrength) * GlowBTint.rgb;
    G += glowA * 0.001 * GlowATint.rgb; // Add glowA contribution and tint
    G *= 1. + pow(S.x,5.0)*0.2;
    
    col += G;
    uv.y *= 1.5;
    col *= 1.0 - dot(uv,uv)*0.1;
    col = mix(col, vec3(0.5,0.4,0.35)*0.3, smoothstep(0.0,1.0,t*0.1 - 0.1));
    
    col = (col - 0.5) * Contrast + 0.5;
    col *= OverallBrightness;

    // Final Hue Rotation
    col = huerot(col, FinalHueRotate * PI * 2.0); // Convert 0-1 range to full circle

    col = clamp(col, 0.0, 1.0);
    col = pow(col, vec3(0.7));
    col = smoothstep(0.0,0.94, col);

    gl_FragColor = vec4(col,1.0);
}