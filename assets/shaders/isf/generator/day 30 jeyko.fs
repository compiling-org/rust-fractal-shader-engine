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
    "DESCRIPTION": "Automatically converted from Shadertoy Day 30 (by jeyko), made ISF compliant with no while loops, float-based color inputs, and added tunable psychedelic parameters.",
    "IMPORTED": {
        "BufferA": {
            "NAME": "BufferA",
            "PATH": "f735bee5b64ef98879dc618b016ecf7939a5756040c2cde21ccb15e69a6e1cfb.png"
        },
        "BufferB": {
            "NAME": "BufferB",
            "TYPE": "audio"
        }
    },
    "INPUTS": [
        {
            "NAME": "MOUSE",
            "TYPE": "point2D"
        },
        {
            "NAME": "TimeOffset",
            "TYPE": "float",
            "DEFAULT": -25.0,
            "MIN": -50.0,
            "MAX": 50.0,
            "LABEL": "Overall Time Offset"
        },
        {
            "NAME": "MouseXInfluence",
            "TYPE": "float",
            "DEFAULT": 10.0,
            "MIN": 0.0,
            "MAX": 20.0,
            "LABEL": "Mouse X Influence"
        },
        {
            "NAME": "MainPaletteColorR",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Main Palette Red"
        },
        {
            "NAME": "MainPaletteColorG",
            "TYPE": "float",
            "DEFAULT": 0.95,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Main Palette Green"
        },
        {
            "NAME": "MainPaletteColorB",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Main Palette Blue"
        },
        {
            "NAME": "PaletteWaveSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Palette Wave Speed"
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Base Glow Intensity"
        },
        {
            "NAME": "BassInfluence",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Audio Bass Influence"
        },
        {
            "NAME": "FadeOutStart",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Fade Out Start Depth"
        },
        {
            "NAME": "FadeOutStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Fade Out Strength"
        },
        {
            "NAME": "PostContrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Post Contrast"
        },
        {
            "NAME": "PostPowerR",
            "TYPE": "float",
            "DEFAULT": 1.8,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Post Power Red"
        },
        {
            "NAME": "PostPowerG",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Post Power Green"
        },
        {
            "NAME": "PostPowerB",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Post Power Blue"
        },
        {
            "NAME": "GreenChannelPower",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 0.5,
            "MAX": 4.0,
            "LABEL": "Green Channel Power"
        },
        {
            "NAME": "GreenChannelRatioR",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Green Ratio Red"
        },
        {
            "NAME": "GreenChannelRatioB",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Green Ratio Blue"
        }
    ]
}

*/


// ISF Conversion - Day 30 Shader

// Renamed and exposed original 'mx' calculation as an adjustable parameter
#define AdjustedTime (TIME + TimeOffset + MouseXInfluence * MOUSE.x / RENDERSIZE.y)

vec3 getRd(vec3 ro,vec3 lookAt,vec2 uv){
	vec3 dir = normalize(lookAt - ro);
	vec3 right = normalize(cross(vec3(0,1,0), dir));
	vec3 up = normalize(cross(dir, right));
	return dir + right*uv.x + up*uv.y;
}
#define dmin(a,b) a.x < b.x ? a : b
#define pmod(p,x) mod(p, x) - x*0.5

float sdBox(vec3 p, vec3 s){
	p = abs(p) - s;    
    return max(p.x, max(p.y, p.z));
}
#define pi acos(-1.)
#define tau (2.*pi)
#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))

// Fix the pal macro - it was missing a parenthesis and expecting a vec3 for 'q' in its commented uses.
// 'q' is now the base color. 'w' is amplitude, 'e' is frequency multiplier, 'r' is factor, 't' is offset.
vec3 pal(vec3 q, float w, float e_mult, vec3 r_vec, float t_offset) {
    return (q + w * cos(tau * (e_mult * r_vec + t_offset)));
}

vec2 id; // Global variable for floor division ID

vec2 map(vec3 p){
	vec2 d = (vec2(10e7));

    p.xy *= rot(0. + p.z*0.1 + 0.1*AdjustedTime);
    
    
    for (float i = 0.; i < 4.; i++){
    	p = abs(p);
    	p.xy *= rot(0.4*pi );
        p.x -= 0.2;
        p.x *= 1. + 0.4*atan(p.x, p.y)/pi;
    }    

    p.xy -= 2.0;
    
    
    p.y = abs(p.y);
    
    
    p.y -= 1. + sin(AdjustedTime*0.1)*0.2;
    
    #define modSz 0.5
    id = floor(p.xz/modSz);
    
    p.xy -= 0.8;
    p.xz = pmod(p.xz, modSz);
    
    for (float i = 0.; i < 5.; i++){
    	p = abs(p);
        p.y -= 0.28 - sin(AdjustedTime*0.2)*0.08 - 0.1;
        p.x += 0.04;
    	p.xy *= rot(0.6*pi + id.y*6.  + 0.9);
        if (i == 3.){
        	p.xz *= rot(AdjustedTime*2. + id.y);
        }
    }     

    d = dmin(d, vec2(sdBox(p, vec3(modSz*0.25 + sin(AdjustedTime*0.26)*0.1)), 1.)); 
    
    d.x *= 0.25;
    return d;
} 
    
vec3 glow = vec3(0);

void main() {

    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 col = vec3(0);
    vec3 ro = vec3(0.,0,0);
    ro.z += AdjustedTime*3.; // Use AdjustedTime
    
    float rate = ro.z*0.1 + 0.1*AdjustedTime; // Use AdjustedTime
    
    ro.xy += vec2(sin(rate), cos(rate))*2.;
    
    vec3 lookAt = ro + vec3(0,0,4);
    float rotRate = AdjustedTime*0.3 + sin(AdjustedTime*0.3)*0.0; // Use AdjustedTime
    lookAt.xz += vec2(
    	sin(rotRate),
    	cos(rotRate)
    );
    
    vec3 rd = getRd(ro, lookAt, uv);
    
    vec3 p = ro; float t = 0.;
    for (int i = 0; i < 250; i++){
    	vec2 d = map(p);
        // Fixed pal macro usage based on new definition
        glow += exp(-d.x*60.) * pal(vec3(MainPaletteColorR, MainPaletteColorG, MainPaletteColorB), 0.95, id.y*0.004 , vec3(0.4, 0.97,0.9), 0.9 + p.z*0.02 + AdjustedTime*PaletteWaveSpeed) ;
        
        if(d.x < 0.0005){
        	break;
        }
        if (t > 100.){
        	break;
        }
        t += d.x;
        p = ro + rd*t;
    }	
    
    // Convert iChannel1 to BufferB and extract bass value
    float bass = pow(IMG_NORM_PIXEL(BufferB,vec2(0.,0.14)).x, 4.);
    
    col += glow*(GlowIntensity + bass*BassInfluence); // Use tunable inputs
    
    // Use tunable fade parameters
    col = mix(col, vec3(0), pow(clamp(t*FadeOutStart - FadeOutStrength, 0., 1.), 2.));
    col = smoothstep(0.,PostContrast, col); // Use tunable contrast
    
    col = pow(col , vec3(PostPowerR,PostPowerG,PostPowerB)); // Use tunable power values
    
    col.g = pow(col.g, GreenChannelPower - GreenChannelRatioR*col.r - GreenChannelRatioB*col.b); // Use tunable green channel parameters
    
    gl_FragColor = vec4(col,1.0);
}