/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Raymarching",
        "3D",
        "Abstract"
    ],
    "DESCRIPTION": "A kaleidoscopic fractal explorer with tunable psychedelic effects, converted from Shadertoy. Not designed for clean code, but for visual impact.",
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
            "LABEL": "Zoom"
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
            "NAME": "FractalDetail",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Fractal Detail"
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
            "NAME": "PaletteFrequencyColor",
            "TYPE": "color",
            "DEFAULT": [0.8, 0.4, 0.7, 1.0],
            "LABEL": "Palette Freq RGB"
        },
        {
            "NAME": "PalettePhase",
            "TYPE": "float",
            "DEFAULT": 2.4,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Palette Phase"
        }
    ]
}
*/

precision highp float;

#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
	vec3 dir = normalize(lookAt - ro);
	vec3 right = normalize(cross(vec3(0,1,0), dir));
	vec3 up = normalize(cross(dir, right));
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
    vec4 c = vec4(0.9,0.77,0.4,0.2) * FractalDetail;
    vec4 u = vec4(0.4,0.54,0.7,0.4) * FractalDetail;
    
    for(int i = 0; i < ITERS; i++){
        p.xyz = abs(p.xyz) - c.xyz;
        float dpp = dot(p.xyz,p.xyz);
        p=p*(1.5 + u)/clamp(dpp,.4,1.)- mix(p,c,0.9)*0.9;
        
        if(i==1)
            p.xy += 0.4;
        
        p.xy *= rot(-0.1 + sin(id)*0.9 * TimeMultiplier);
        
        p.xz *= rot(0.9);
        
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

vec3 colorPalette(float a, float b, vec3 c, float d, float e) {
    return (a + b*sin(c*d + e));
}

void main() {

    // Corrected image sampling using texture2D with the named inputImage uniform
    // S will now be a vec4 directly from the texture sample
    vec4 S = texture2D(inputImage, vec2(0.01,0.0)); 

    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    uv *= 1. + dot(uv,uv)*UV_Distortion;
    
    vec3 col = vec3(0);
    
    // Corrected image sampling for D, now accessing the .x component for float value
    D = mix(0.8,1.,texture2D(inputImage, mod(20.0*uv*256.0 + TIME*2.0,1.0)).x); 
    
    vec3 ro = vec3(0);
    ro.x += sin(TIME * 0.5) * CameraShake;
    ro.y += cos(TIME * 0.7) * CameraShake;

    float m_time = TIME * TimeMultiplier;
    
    // Removed all mouse-related controls from camera position
    ro.x += sin(m_time * 0.2) * 0.25 * ZoomLevel; 
    ro.z += m_time + 14.0 * ZoomLevel;
    
    // The previous MouseZoomY was tied to MOUSE.y, so it also needs to be removed
    // or re-purposed for a different non-mouse control.
    // For now, removing it to completely eliminate mouse interaction.


    vec3 lookAt = vec3(0);
    lookAt.z = ro.z + 1.0;
    
    vec3 rd = getRd(ro, lookAt, uv);
    
    vec3 p;float t; bool hit;
    
    vec2 d_march = march(ro, rd, p, t, hit);
    
    float currentHueOffset = PaletteHueOffset + TIME * ColorShiftSpeed;
    vec3 pal_freq = PaletteFrequencyColor.rgb; // Access .rgb from the color input
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
        
        // S.x is now correctly taken from the texture sample S (which is vec4)
        vec3 CD = 0.05 * WS_values.z * colorPalette(0.2, 0.6, pal_freq, 5.9 + currentHueOffset, pal_phase + S.x);
        
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
    G += pow(glowD*0.004,vec3(1.0))*colorPalette(0.2, 0.6, pal_freq, 0.6 + currentHueOffset, pal_phase);
    G -= pow(glowC*0.005, vec3(1.1))*colorPalette(0.2, 0.4, pal_freq, 9.6 + currentHueOffset, pal_phase);
    
    G += glowB*0.002 *colorPalette(0.2, 0.6, pal_freq, 5.99 - sin(TIME), pal_phase);
    G *= 1. + pow(S.x,5.0)*0.2;
    
    col += G;
    uv.y *= 1.5;
    col *= 1.0 - dot(uv,uv)*0.1;
    col = mix(col, vec3(0.5,0.4,0.35)*0.3, smoothstep(0.0,1.0,t*0.1 - 0.1));
    
    col = (col - 0.5) * Contrast + 0.5;
    col *= OverallBrightness;

    col = clamp(col, 0.0, 1.0);
    col = pow(col, vec3(0.7));
    col = smoothstep(0.0,0.94, col);

    gl_FragColor = vec4(col,1.0);
}