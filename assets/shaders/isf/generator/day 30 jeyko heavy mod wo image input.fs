/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Fractal",
        "Raymarching",
        "Abstract",
        "Psychedelic"
    ],
    "DESCRIPTION": "Automatically converted from Shadertoy Day 30 (by jeyko). Now ISF compliant with no image inputs, float-based color controls, multiple palette choices, color pulsing, and extensive geometry morphing controls.",
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
            "NAME": "GlobalRotSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Global Rotation Speed"
        },
        {
            "NAME": "BoxXOffset",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Box X Offset"
        },
        {
            "NAME": "DistortionIntensity",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Atan Distortion Intensity"
        },
        {
            "NAME": "YOffsetWaveSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Y Offset Wave Speed"
        },
        {
            "NAME": "YOffsetWaveAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Y Offset Wave Amplitude"
        },
        {
            "NAME": "ModulusSize",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Pattern Modulus Size"
        },
        {
            "NAME": "FractalYOffsetBase",
            "TYPE": "float",
            "DEFAULT": 0.18,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Y Offset Base"
        },
        {
            "NAME": "FractalYMorphSpeed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Y Morph Speed"
        },
        {
            "NAME": "FractalYMorphAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.08,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Fractal Y Morph Amplitude"
        },
        {
            "NAME": "InnerRotationSpeed",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Inner Rotation Speed"
        },
        {
            "NAME": "BoxScaleWaveSpeed",
            "TYPE": "float",
            "DEFAULT": 0.26,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Box Scale Wave Speed"
        },
        {
            "NAME": "BoxScaleWaveAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Box Scale Wave Amplitude"
        },
        {
            "NAME": "PaletteChoice",
            "TYPE": "long",
            "DEFAULT": 0,
            "VALUES": [
                0,
                1,
                2,
                3
            ],
            "LABELS": [
                "Default Psychedelic",
                "Warm Glow",
                "Cool Cyber",
                "Monochrome Green"
            ]
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
            "NAME": "ColorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Color Pulse Speed"
        },
        {
            "NAME": "ColorPulseAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Color Pulse Amplitude"
        },
        {
            "NAME": "PulseUpperLimit",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MIN": 0.5,
            "MAX": 3.0,
            "LABEL": "Color Pulse Upper Limit"
        },
        {
            "NAME": "GlowBaseIntensity",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 0.1,
            "LABEL": "Base Glow Intensity"
        },
        {
            "NAME": "TimePulseInfluence",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Time Pulse Influence"
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


// ISF Conversion - Day 30 Shader (Restored Baseline + New Features)

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

// Pal function - now a proper function for more flexibility
vec3 pal(vec3 q, float w, float e_mult, vec3 r_vec, float t_offset) {
    return (q + w * cos(tau * (e_mult * r_vec + t_offset)));
}

vec2 id; // Global variable for floor division ID

vec2 map(vec3 p){
	vec2 d = (vec2(10e7));

    // Geometry Morphing Control: Global Rotation Speed
    p.xy *= rot(0. + p.z*GlobalRotSpeed + GlobalRotSpeed*AdjustedTime);
    
    for (float i = 0.; i < 4.; i++){
    	p = abs(p);
    	p.xy *= rot(0.4*pi );
        // Geometry Morphing Control: Box X Offset
        p.x -= BoxXOffset;
        // Geometry Morphing Control: Atan Distortion Intensity
        p.x *= 1. + DistortionIntensity*atan(p.x, p.y)/pi;
    }    

    p.xy -= 2.0;
    
    p.y = abs(p.y);
    
    // Geometry Morphing Control: Y Offset Wave Speed & Amplitude
    p.y -= 1. + sin(AdjustedTime*YOffsetWaveSpeed)*YOffsetWaveAmplitude;
    
    // Geometry Morphing Control: Pattern Modulus Size
    id = floor(p.xz/ModulusSize);
    
    p.xy -= 0.8;
    p.xz = pmod(p.xz, ModulusSize);
    
    for (float i = 0.; i < 5.; i++){
    	p = abs(p);
        // Geometry Morphing Control: Fractal Y Offset Base, Speed & Amplitude
        // Note: Original had 0.28 - sin(...) - 0.1; now base is 0.18 for cleaner parameterization
        p.y -= FractalYOffsetBase - sin(AdjustedTime*FractalYMorphSpeed)*FractalYMorphAmplitude;
        p.x += 0.04;
    	p.xy *= rot(0.6*pi + id.y*6.  + 0.9);
        if (i == 3.){
            // Geometry Morphing Control: Inner Rotation Speed
        	p.xz *= rot(AdjustedTime*InnerRotationSpeed + id.y);
        }
    }     

    // Geometry Morphing Control: Box Scale Wave Speed & Amplitude
    d = dmin(d, vec2(sdBox(p, vec3(ModulusSize*0.25 + sin(AdjustedTime*BoxScaleWaveSpeed)*BoxScaleWaveAmplitude)), 1.)); 
    
    d.x *= 0.25;
    return d;
} 
    
vec3 glow = vec3(0);

void main() {

    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 col = vec3(0);
    vec3 ro = vec3(0.,0,0);
    ro.z += AdjustedTime*3.; 
    
    float rate = ro.z*0.1 + GlobalRotSpeed*AdjustedTime; 
    
    ro.xy += vec2(sin(rate), cos(rate))*2.;
    
    vec3 lookAt = ro + vec3(0,0,4);
    float rotRate = AdjustedTime*0.3 + sin(AdjustedTime*0.3)*0.0; 
    lookAt.xz += vec2(
    	sin(rotRate),
    	cos(rotRate)
    );
    
    vec3 rd = getRd(ro, lookAt, uv);
    
    vec3 p = ro; float t = 0.;

    // --- Palette Selection and Color Pulse Setup ---
    vec3 baseQ = vec3(0.0);
    vec3 rVec = vec3(0.0);

    // Apply Color Pulse to the palette's amplitude (w)
    float timePulse = pow(sin(AdjustedTime * ColorPulseSpeed)*0.5 + 0.5, 4.0); // Simple 0-1 pulse
    float pulseFactor = 1.0 + timePulse * ColorPulseAmplitude;
    // Apply upper limit to control peak brightness/saturation of the pulse factor
    pulseFactor = min(pulseFactor, PulseUpperLimit);

    if (PaletteChoice == 0) { // Default Psychedelic (Original Look)
        baseQ = vec3(1.0, 0.95, 0.9); // From MainPaletteColorR/G/B
        rVec = vec3(0.4, 0.97, 0.9);
    } else if (PaletteChoice == 1) { // Warm Glow
        baseQ = vec3(1.0, 0.8, 0.6);
        rVec = vec3(1.0, 0.5, 0.2);
    } else if (PaletteChoice == 2) { // Cool Cyber
        baseQ = vec3(0.6, 0.8, 1.0);
        rVec = vec3(0.2, 0.7, 1.0);
    } else if (PaletteChoice == 3) { // Monochrome Green
        baseQ = vec3(0.5, 1.0, 0.5);
        rVec = vec3(0.1, 0.8, 0.1);
    }
    // --- End Palette Setup ---


    for (int i = 0; i < 250; i++){
    	vec2 d = map(p);
        
        // Original glow calculation (note: d.x is not abs(d.x) here)
        glow += exp(-d.x*60.) * pal(baseQ, 0.95 * pulseFactor, id.y*0.004 , rVec, 0.9 + p.z*0.02 + AdjustedTime*PaletteWaveSpeed);
        
        if(d.x < 0.0005){
        	break;
        }
        if (t > 100.){
        	break;
        }
        // Original ray advancement (note: no max(0.0005, d.x) here)
        t += d.x;
        p = ro + rd*t;
    }	
    
    col += glow*(GlowBaseIntensity + timePulse*TimePulseInfluence); 
    
    col = mix(col, vec3(0), pow(clamp(t*FadeOutStart - FadeOutStrength, 0., 1.), 2.));
    col = smoothstep(0.,PostContrast, col); 
    
    col = pow(col , vec3(PostPowerR,PostPowerG,PostPowerB)); 
    
    col.g = pow(col.g, GreenChannelPower - GreenChannelRatioR*col.r - GreenChannelRatioB*col.b); 
    
    gl_FragColor = vec4(col,1.0);
}