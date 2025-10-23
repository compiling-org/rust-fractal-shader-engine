/*
{
    "CATEGORIES": [
        "Raymarching",
        "Psychedelic",
        "Abstract",
        "Procedural"
    ],
    "DESCRIPTION": "Adapting tdhooper's 'Echeveria' as a blue lotus flower, now with tunable parameters for colors, animation, and shape.",
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "BufferA",
            "PERSISTENT": true
        },
        {
        }
    ],
    "INPUTS": [
        { "NAME": "iMouse", "TYPE": "point2D" },

        { "NAME": "OverallAnimationSpeed", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Animation Speed" },
        { "NAME": "GlobalTimeOffset", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.0, "LABEL": "Global Time Offset" },
        { "NAME": "LoopDuration", "TYPE": "float", "MIN": 1.0, "MAX": 60.0, "DEFAULT": 3.0, "LABEL": "Loop Duration (Seconds)" },

        { "NAME": "LeafBaseColor1", "TYPE": "color", "DEFAULT": [0.227,0.000,0.380,1.0], "LABEL": "Leaf Base Color A" },
        { "NAME": "LeafBaseColor2", "TYPE": "color", "DEFAULT": [0.027,0.184,0.812,1.0], "LABEL": "Leaf Base Color B" },
        { "NAME": "LeafBaseColorMixPoint", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "LABEL": "Leaf Base Color Mix" },
        { "NAME": "LeafBaseColorBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.2, "LABEL": "Leaf Base Brightness" },
        
        { "NAME": "LeafOverlayColor", "TYPE": "color", "DEFAULT": [0.06,0.045,0.001,1.0], "LABEL": "Leaf Overlay Color" },
        { "NAME": "LeafOverlayBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.5, "LABEL": "Leaf Overlay Brightness" },
        
        { "NAME": "LeafMidColor", "TYPE": "color", "DEFAULT": [0.957,0.616,0.145,1.0], "LABEL": "Leaf Mid Color" },
        { "NAME": "LeafMidColorMixStart", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Leaf Mid Mix Start" },
        { "NAME": "LeafMidColorMixEnd", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "LABEL": "Leaf Mid Mix End" },
        
        { "NAME": "LeafHighlightColor", "TYPE": "color", "DEFAULT": [0.949,0.788,0.212,1.0], "LABEL": "Leaf Highlight Color" },
        { "NAME": "LeafHighlightBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.4, "LABEL": "Leaf Highlight Brightness" },
        { "NAME": "LeafHighlightMixStart", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "LABEL": "Leaf Highlight Mix Start" },
        { "NAME": "LeafHighlightMixEnd", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.9, "LABEL": "Leaf Highlight Mix End" },
        { "NAME": "LeafHighlightPower", "TYPE": "float", "MIN": 0.5, "MAX": 5.0, "DEFAULT": 1.6, "LABEL": "Leaf Highlight Power" },

        { "NAME": "MinLeafThickness", "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 0.2, "LABEL": "Min Leaf Thickness" },
        { "NAME": "MaxLeafThickness", "TYPE": "float", "MIN": 0.5, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Max Leaf Thickness" },
        { "NAME": "LeafThicknessFalloff", "TYPE": "float", "MIN": 0.001, "MAX": 0.1, "DEFAULT": 0.01, "LABEL": "Leaf Thickness Falloff" },

        { "NAME": "LeafBaseWidth", "TYPE": "float", "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.4, "LABEL": "Leaf Base Width" },
        { "NAME": "LeafTipWidth", "TYPE": "float", "MIN": 0.01, "MAX": 0.5, "DEFAULT": 0.2, "LABEL": "Leaf Tip Width" },
        { "NAME": "LeafWidthMixPointY", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "LABEL": "Leaf Width Y Mix Point" },

        { "NAME": "LeafLengthBaseFactor", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 0.8333, "LABEL": "Leaf Length Base Factor" },
        { "NAME": "LeafLengthTipFactor", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Leaf Length Tip Factor" },
        { "NAME": "LeafLengthYScale", "TYPE": "float", "MIN": 1.0, "MAX": 5.0, "DEFAULT": 2.9, "LABEL": "Leaf Length Y Scale" },
        { "NAME": "LeafLengthYPower", "TYPE": "float", "MIN": 1.0, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Leaf Length Y Power" },

        { "NAME": "BloomRadiusOffset", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": -1.2, "LABEL": "Bloom Radius Offset" },
        { "NAME": "BloomMinRadius", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 3.3, "LABEL": "Bloom Min Radius" },
        { "NAME": "CellDistX", "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 7.0, "LABEL": "Cell Distribution X" },
        { "NAME": "CellDistY", "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 8.0, "LABEL": "Cell Distribution Y" },
        { "NAME": "BloomOffsetDirX", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Bloom Offset Dir X" },
        { "NAME": "BloomOffsetDirY", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Bloom Offset Dir Y" },
        { "NAME": "BloomTimeScale", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0, "LABEL": "Bloom Time Scale" },

        { "NAME": "CameraBaseX", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 0.1, "LABEL": "Camera Base X" },
        { "NAME": "CameraBaseY", "TYPE": "float", "MIN": 5.0, "MAX": 15.0, "DEFAULT": 8.4, "LABEL": "Camera Base Y" },
        { "NAME": "CameraBaseZ", "TYPE": "float", "MIN": -15.0, "MAX": 0.0, "DEFAULT": -8.0, "LABEL": "Camera Base Z" },
        { "NAME": "CameraZOscillationAmplitude", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.0, "LABEL": "Camera Z Oscillation Amplitude" },
        { "NAME": "CameraZOscillationSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "LABEL": "Camera Z Oscillation Speed" },
        { "NAME": "CameraGlobalPositionScale", "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 0.9, "LABEL": "Camera Global Position Scale" },
        { "NAME": "CameraLookAtTargetX", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 0.0, "LABEL": "Camera Look At X" },
        { "NAME": "CameraLookAtTargetY", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": -1.5, "LABEL": "Camera Look At Y" },
        { "NAME": "CameraLookAtTargetZ", "TYPE": "float", "MIN": -5.0, "MAX": 5.0, "DEFAULT": 0.0, "LABEL": "Camera Look At Z" },
        { "NAME": "RaymarchingDepthFactor", "TYPE": "float", "MIN": 1.0, "MAX": 5.0, "DEFAULT": 2.8, "LABEL": "Raymarching Depth Factor" },
        { "NAME": "MaxRayMarchDistance", "TYPE": "float", "MIN": 10.0, "MAX": 30.0, "DEFAULT": 16.0, "LABEL": "Max Ray March Distance" },

        { "NAME": "BGColor1", "TYPE": "color", "DEFAULT": [0.19,0.19,0.22,1.0], "LABEL": "Background Color 1" },
        { "NAME": "BGColor1_Mult", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.9, "LABEL": "BG Color 1 Multiplier" },
        { "NAME": "BGColor2", "TYPE": "color", "DEFAULT": [0.35,0.24,0.0,1.0], "LABEL": "Background Color 2" },
        { "NAME": "BGColor2_Mult", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.8, "LABEL": "BG Color 2 Multiplier" },
        { "NAME": "BGColor3", "TYPE": "color", "DEFAULT": [0.5,1.0,0.7,1.0], "LABEL": "Background Color 3" },
        { "NAME": "BGColor3_Mult", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.0, "LABEL": "BG Color 3 Multiplier" },
        
        { "NAME": "LightDiffuseIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.80, "LABEL": "Light Diffuse Intensity" },
        { "NAME": "LightDiffuseColor", "TYPE": "color", "DEFAULT": [1.30,1.00,0.70,1.0], "LABEL": "Light Diffuse Color" },
        { "NAME": "LightAmbientIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.55, "LABEL": "Light Ambient Intensity" },
        { "NAME": "LightAmbientColor", "TYPE": "color", "DEFAULT": [0.40,0.60,1.15,1.0], "LABEL": "Light Ambient Color" },
        { "NAME": "LightBacklightIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.55, "LABEL": "Light Backlight Intensity" },
        { "NAME": "LightBacklightColor", "TYPE": "color", "DEFAULT": [0.25,0.25,0.25,1.0], "LABEL": "Light Backlight Color" },
        { "NAME": "LightBacklightTint", "TYPE": "color", "DEFAULT": [2.0,0.0,1.0,1.0], "LABEL": "Light Backlight Tint" },
        { "NAME": "LightFresnelIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.25, "LABEL": "Light Fresnel Intensity" },
        { "NAME": "LightFresnelColor", "TYPE": "color", "DEFAULT": [1.0,1.0,1.0,1.0], "LABEL": "Light Fresnel Color" },
        { "NAME": "LightSpecularIntensity", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 5.00, "LABEL": "Light Specular Intensity" },
        { "NAME": "LightSpecularColor", "TYPE": "color", "DEFAULT": [1.10,0.90,0.70,1.0], "LABEL": "Light Specular Color" },

        { "NAME": "OverallOutputBrightness", "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.3, "LABEL": "Overall Output Brightness" },
        { "NAME": "OutputGamma", "TYPE": "float", "MIN": 0.1, "MAX": 5.0, "DEFAULT": 0.4545, "LABEL": "Output Gamma" }
    ]
}

*/


const float PI  = 3.14159265359;

void pR(inout vec2 p, float a) {
    p = cos(a)*p + sin(a)*vec2(p.y, -p.x);
}

float smin(float a, float b, float k){
    float f = clamp(0.5 + 0.5 * ((a - b) / k), 0., 1.);
    return (1. - f) * a + f  * b - f * (1. - f) * k;
}

float smax(float a, float b, float k) {
    return -smin(-a, -b, k);
}

// Global time variable, now controlled by ISF inputs for animation speed, offset, and looping
#define time (mod(TIME * OverallAnimationSpeed + GlobalTimeOffset, LoopDuration))

// Declare built-in ISF uniform (MOUSE)
uniform vec4 MOUSE; // Declared MOUSE uniform for mouse position and button states

// All other custom uniform variables are automatically declared by the ISF host
// based on the "INPUTS" section in the JSON header. Therefore, explicit declarations
// for them are removed to avoid "redefinition" errors.


vec4 leaf(vec3 p, vec2 uv) {
    // Tunable leaf thickness
    float thick = clamp(uv.y, MinLeafThickness, MaxLeafThickness);
    float th = thick * LeafThicknessFalloff; // Tunable thickness falloff
    pR(p.xz, -uv.x);
    // Tunable leaf width
    float width = mix(LeafBaseWidth, LeafTipWidth, min(uv.y, LeafWidthMixPointY));
   
    vec3 n = normalize(vec3(1,0 ,width));
    float d = -dot(p, n);
    d = max(d, dot(p, n * vec3(1,1,-1)));
    
    // Tunable leaf length
    float len = mix(PI * LeafLengthBaseFactor, PI * LeafLengthTipFactor, pow(uv.y/LeafLengthYScale, LeafLengthYPower));
    len = max(len, 0.);
    pR(p.yz, PI / 2. - len);
    d = smax(d, p.y, thick);
    d = smax(d, abs(length(p) - uv.y) - thick * th, th);
    vec2 uuv = vec2(
        atan(p.y, p.z) / -len,
        p.x
   	);
    // Tunable leaf colors
    vec3 col = mix(LeafBaseColor1.rgb * LeafBaseColorBrightness, LeafBaseColor2.rgb * LeafBaseColorBrightness, smoothstep(.0, LeafBaseColorMixPoint, uuv.x));
    col += LeafOverlayColor.rgb * LeafOverlayBrightness * max(1. - uv.y / 2., 0.);
    
    col = mix(col, LeafMidColor.rgb * LeafMidColor.a, smoothstep(LeafMidColorMixStart, LeafMidColorMixEnd, uuv.x));
    col += vec3(.3,.05,.001) * max(.5 - uv.y / 2., 0.)*1.5; // This line still uses hardcoded colors, could be parameterized too if desired
    
    col = mix(col, col * .2, 1.-smoothstep(.0, .2, uuv.x));
    col += mix(vec3(0), LeafHighlightColor.rgb * LeafHighlightBrightness, smoothstep(LeafHighlightMixStart, LeafHighlightMixEnd, pow(uuv.x, LeafHighlightPower)));
    return vec4(d, col);
}

bool lightingPass;

vec4 opU(vec4 a, vec4 b) {
    return a.x < b.x ? a : b;
}

vec4 bloom(vec3 p) {

    // Tunable bloom radius and offset
    float bound = length(p - vec3(0,BloomRadiusOffset,0)) - BloomMinRadius;
    bound = max(bound, p.y - 1.1); // Y clamp for bloom, could be param
    if (bound > .01 && ! lightingPass) {
        return vec4(bound, 0, 0, 0);
        
    }

    // Tunable cell distribution
    vec2 cc = vec2(CellDistX, CellDistY);
  
    // Use MOUSE.z for click detection and MOUSE.xy for position
    if (MOUSE.z > 0.0) {
    	cc = floor(MOUSE.xy / RENDERSIZE.xy * 10.);
    }
    float aa = atan(cc.x / cc.y);
    float r = (PI*2.) / sqrt(cc.x*cc.x + cc.y*cc.y);
    mat2 rot = mat2(cos(aa), -sin(aa), sin(aa), cos(aa));
    
    // Tunable bloom offset direction and speed
    vec2 offset = vec2(BloomOffsetDirX, BloomOffsetDirY) * time * r * rot * BloomTimeScale;
    
    vec2 uv = vec2(
        atan(p.x, p.z),
        length(p)
    );

    uv -= offset;

    uv = rot * uv;
    // Corrected round() for vec2
    vec2 cell = floor(uv / r + 0.5); 

    vec4 d = vec4(1e12, vec3(0));

    d = opU(d, leaf(p, ((cell + vec2(-1, 0)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(0, -1)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(0, 0)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(1, -1)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(1, 0)) * rot * r) + offset));

    d = opU(d, leaf(p, ((cell + vec2(-1, -1)) * rot * r) + offset));
   	d = opU(d, leaf(p, ((cell + vec2(-1, 1)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(0, 1)) * rot * r) + offset));
    d = opU(d, leaf(p, ((cell + vec2(1, 1)) * rot * r) + offset));

    return d;
}

vec4 map(vec3 p) {
    return bloom(p);
}

vec3 calcNormal(vec3 pos){
    float eps = .0005;
    vec2 e = vec2(1.0,-1.0) * 0.5773;
    return normalize(
        e.xyy * map(pos + e.xyy * eps).x + 
		e.yyx * map(pos + e.yyx * eps).x + 
		e.yxy * map(pos + e.yxy * eps).x + 
		e.xxx * map(pos + e.xxx * eps).x
    );
}

// https://www.shadertoy.com/view/lsKcDD
float softshadow( in vec3 ro, in vec3 rd, in float mint, in float tmax )
{
    float res = 1.0;
    float t = mint;
    // float ph = 1e10; // Unused variable
    
    for( int i=0; i<64; i++ )
    {
        float h = map( ro + rd*t ).x;
        res = min( res, 10.0*h/t );
        t += h;
        if( res<0.0001 || t>tmax ) break;
        
    }
    return clamp( res, 0.0, 1.0 );
}

// https://www.shadertoy.com/view/Xds3zN
float calcAO( in vec3 pos, in vec3 nor )
{
    float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + 0.12*float(i)/4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = map( aopos ).x;
        occ += -(dd-hr)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}

mat3 calcLookAtMatrix( in vec3 ro, in vec3 ta, in float roll )
{
    vec3 ww = normalize( ta - ro );
    vec3 uu = normalize( cross(ww,vec3(sin(roll),cos(roll),0.0) ) );
    vec3 vv = normalize( cross(uu,ww));
    return mat3( uu, vv, ww );
}

//#define AA 3 // Removed as it was commented out and not used

void main() {

    vec3 col;
    vec3 tot = vec3(0.0);
    // Removed mTime and direct assignment to 'time' as 'time' is now a #define
    // float mTime = mod(TIME / 3., 1.); 
    // time = mTime; 
    
    vec2 o = vec2(0);
    // Removed AA #ifdef block as it was commented out/not defined
    // #ifdef AA
    // for( int m=0; m<AA; m++ )
    // for( int n=0; n<AA; n++ )
    // {
    // // pixel coordinates
    // o = vec2(float(m),float(n)) / float(AA) - 0.5;
    // // time coordinate (motion blurred, shutter=0.5)
    // float d = 0.5*sin(gl_FragCoord.x*147.0)*sin(gl_FragCoord.y*131.0);
    // time = mTime - 0.1*(1.0/24.0)*(float(m*AA+n)+d)/float(AA*AA-1);
    // #endif
    
    lightingPass = false;
    vec2 p_frag = (isf_FragNormCoord - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0); // Use isf_FragNormCoord for portability
    
    // Tunable camera position and animation
    vec3 camPos = vec3(CameraBaseX, CameraBaseY, CameraBaseZ + CameraZOscillationAmplitude * sin(CameraZOscillationSpeed * TIME)) * CameraGlobalPositionScale;
    
    // Tunable camera look at target
    mat3 camMat = calcLookAtMatrix( camPos, vec3(CameraLookAtTargetX, CameraLookAtTargetY, CameraLookAtTargetZ), -0.5); // Roll is still hardcoded for now
    vec3 rd = normalize( camMat * vec3(p_frag.xy, RaymarchingDepthFactor) ); // Tunable Raymarching Depth Factor
    vec3 pos = camPos;
    float rayLength = 0.;
    float dist = 0.;
    bool bg = false;
    vec4 res;
    for (int i = 0; i < 100; i++) {
        rayLength += dist;
        pos = camPos + rd * rayLength;
        res = map(pos);
        dist = res.x;
        if (abs(dist) < .001) {
            break;
        }
        
        if (rayLength > MaxRayMarchDistance) { // Tunable Max Ray March Distance
            bg = true;
            break;
        }
    }
    
    // Tunable background colors and multipliers
    col = BGColor1.rgb * BGColor1_Mult;
    col += BGColor2.rgb * BGColor2_Mult;
    col += BGColor3.rgb * BGColor3_Mult;
    // Original mixes had a hardcoded `.00` for the last color, implying an intentional blend,
    // which has been replaced by the multiplier.
    
    if ( ! bg) {
        
        lightingPass = true;
        
        vec3 nor = calcNormal(pos);
        float occ = calcAO( pos, nor );
        vec3  lig = normalize( vec3(-.2, 1.5, .3) ); // Could be MainLightDirection uniform
        vec3  lba = normalize( vec3(.5, -1., -.5) ); // Could be BackLightDirection uniform
        vec3  hal = normalize( lig - rd );
        float amb = sqrt(clamp( 0.5+0.5*nor.y, 0.0, 1.0 ));
        float dif = clamp( dot( nor, lig ), 0.0, 1.0 );
        float bac = clamp( dot( nor, lba ), 0.0, 1.0 )*clamp( 1.0-pos.y,0.0,1.0);
        float fre = pow( clamp(1.0+dot(nor,rd),0.0,1.0), 2.0 );
        occ = mix(1., occ, 1.); // Redundant mix with 1.0, effectively just 'occ'
        
        dif *= softshadow( pos, lig, 0.001, .9 ); // Shadow mint and tmax could be params
        float spe = pow( clamp( dot( nor, hal ), 0.0, 1.0 ),16.0)*
                    dif *
                    (0.04 + 0.96*pow( clamp(1.0+dot(hal,rd),0.0,1.0), 5.0 )); // Specular components could be further parameterized
        vec3 lin = vec3(0.0);
        lin += LightDiffuseIntensity * dif * LightDiffuseColor.rgb; // Tunable diffuse light
        lin += LightAmbientIntensity * amb * LightAmbientColor.rgb * occ; // Tunable ambient light
        lin += LightBacklightIntensity * bac * LightBacklightColor.rgb * occ * LightBacklightTint.rgb; // Tunable backlight
        lin += LightFresnelIntensity * fre * LightFresnelColor.rgb * occ; // Tunable fresnel light
        col = res.yzw; // Leaf color from bloom pass
        col = col*lin;
        col += LightSpecularIntensity * spe * LightSpecularColor.rgb; // Tunable specular light
    }
    
    tot += col;
    // Removed AA #ifdef block for averaging
    // tot /= float(AA*AA); 
    
    col = tot;
    col *= OverallOutputBrightness; // Tunable overall brightness
    col = pow( col, vec3(OutputGamma) ); // Tunable output gamma
    gl_FragColor = vec4(col,1.0);
}
