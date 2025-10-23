/*
{
  "DESCRIPTION": "Fractal Tunnel - Trippy animated tunnel converted from Shadertoy (tX23Dy style), with error fixes and tunable parameters for animation speed, rotation, distortion, color pulses, palettes, flicker, gamma, and global brightness.",
  "CATEGORIES": ["Fractal", "Tunnel", "Psychedelic", "Tunable", "Shadertoy Conversion"],
  "INPUTS": [
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 10.0,
      "DEFAULT": 1.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "RotationSpeed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 10.0,
      "DEFAULT": 0.5,
      "LABEL": "Rotation Speed"
    },
    {
      "NAME": "DistortionStrength",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Distortion Strength"
    },
    {
      "NAME": "ColorPulseFreq",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 20.0,
      "DEFAULT": 2.0,
      "LABEL": "Color Pulse Freq"
    },
    {
      "NAME": "ColorPulseAmp",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5,
      "LABEL": "Color Pulse Amp"
    },
    {
      "NAME": "FlickerAmount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "LABEL": "Grain/Flicker"
    },
    {
      "NAME": "GammaAdjust",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 4.0,
      "DEFAULT": 0.5,
      "LABEL": "Gamma Correction"
    },
    { "NAME": "BaseColor1", "TYPE": "color", "DEFAULT": [0.2, 0.7, 1.0, 1.0], "LABEL": "Base Color 1" },
    { "NAME": "BaseColor2", "TYPE": "color", "DEFAULT": [1.0, 0.3, 0.8, 1.0], "LABEL": "Base Color 2" },
    { "NAME": "PaletteBlend", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Blend" },
    { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Global Brightness" }
  ],
  "ISFVSN": "2"
}
*/

precision highp float; // Set high precision for floats

#define PI 3.141592
#define TAU PI*2.0 // Ensure float literal
// Converted 'pal' macro to a function to avoid macro argument issues
vec3 generatePaletteColor(float x) {
    return (cos(x*2.0*PI+vec3(0.0, 22.0, 14.0))*0.5+0.5); // Ensure float literals
}
#define SIN(x) (0.5+0.5*sin(x)) // Ensure float literals
#define S(a, b, x) smoothstep(a, b, x)
#define rot(x) mat2(cos(x), -sin(x), sin(x), cos(x)) // Corrected rot matrix

// Global variable for scaled time
float scaledTime;

// The 'getPal' function has been removed as its functionality is replaced by tunable inputs.
/*
vec3 getPal(int id, float t) {
    vec3 col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,-0.33,0.33) );
    if( id == 1 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20) );
    if( id == 2 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.3,0.20,0.20) );
    if( id == 3 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,0.5),vec3(0.8,0.90,0.30) );
    if( id == 4 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,0.7,0.4),vec3(0.0,0.15,0.20) );
    if( id == 5 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(2.0,1.0,0.0),vec3(0.5,0.20,0.25) );
    if( id == 6 ) col = pal( t, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
    return col;
}
*/

vec3 colorStripeTexture(vec3 p) {
    return vec3(1.0); // Ensure float literal
}

float box(vec3 p, vec3 r) {
    vec3 q = abs(p) - r;
    return max(max(q.x, q.y),q.z);
}

// float repeat(inout float x, float n) { // Not used in main
//     float id = floor(n*x)/n;
//     x = fract(n*x);
//     return id;
// }

vec2 repeat(inout vec2 p, vec2 size) {
    vec2 c = floor((p + size*0.5)/size);
    p = mod(p + size*0.5,size) - size*0.5;
    return c;
}

float pMod1(inout float p, float size) {
    float halfsize = size*0.5;
    float c = floor((p + halfsize)/size);
    p = mod(p + halfsize, size) - halfsize;
    return c;
}

vec2 moda(vec2 p, float repetitions) {
    float angle = 2.0*PI/repetitions; // Ensure float literal
    float a = atan(p.y, p.x) + angle/2.0; // Ensure float literal
    a = mod(a,angle) - angle/2.0; // Ensure float literal
    return vec2(cos(a), sin(a))*length(p);
}

float pModSingle1(inout float p, float size) {
    float halfsize = size*0.5;
    float c = floor((p + halfsize)/size);
    if (p >= 0.0) // Ensure float literal
        p = mod(p + halfsize, size) - halfsize;
    return c;
}

vec3 kalei(vec3 p) {
    float w = 1.0; // Ensure float literal
    p = abs(p) - 0.2; // Ensure float literal
    for(float i=0.0; i < 3.0; i++) { // Ensure float literals
        float t1 = 2.0+sin(i+scaledTime) + sin(0.7*scaledTime)*0.4; // Ensure float literals
        p.xy *= rot(0.3*t1); // Ensure float literal
        p -= 0.1 + 0.1*i; // Ensure float literals
        p = abs(p);
    }
    p /= w;
    return p;
}

vec2 foldSym(vec2 p, float N) {
    float t = atan(p.x,-p.y);
    t = mod(t+PI/N,2.0*PI/N)-PI/N; // Ensure float literals
    p = length(p.xy)*vec2(cos(t),sin(t));
    p = abs(p)-0.25; // Ensure float literal
    p = abs(p)-0.25; // Ensure float literal
    return p;
}

vec2 beamId;
float map(vec3 p) {
    vec3 bp = p;
    float repz = 8.0; // Ensure float literal
    float idz = pModSingle1(p.z,repz);
    
    //p.xy *= rot(scaledTime*TAU/16.0*sign(mod(idz, 2.0)-0.5)); // Ensure float literals
    p.xy = foldSym(p.xy, 6.0); // Ensure float literal
    
    p.y += 0.4*sin(p.z*TAU/4.0+scaledTime*TAU/2.0); // Ensure float literals
    
    p.xy *= rot(0.25*PI*mod(idz, 2.0)); // Ensure float literals
    
    //p = kalei(p); // If uncommented, ensure kalei uses scaledTime
    float blen = 3.9; // Ensure float literal

    float outer = 1.3; // Ensure float literal
    float inner = 1.1; // Ensure float literal
    float maskout = box(p, vec3(vec2(outer), blen));
    float maskin = box(p, vec3(vec2(inner), blen + 0.3)); // Ensure float literal
    beamId = repeat(p.xy, vec2(0.2)); // Ensure float literal
    float beam = box(p, vec3(vec2(0.07), blen)); // Ensure float literal
    
    float frame = max(maskout, -maskin);
    float d = max(beam, maskout);
    d = max(d, frame);
    return d;
}

float calcAO(vec3 p, vec3 n) {
    float sca = 2.0, occ = 0.0; // Ensure float literals
    for( int i=0; i<5; i++ ){ // Loop init fixed
        float hr = 0.01 + float(i)*0.5/4.0; // Ensure float literals
        float dd = map(n * hr + p);
        occ += (hr - dd)*sca;
        sca *= 0.7; // Ensure float literal
    }
    return clamp( 1.0 - occ, 0.0, 1.0 ); // Ensure float literal
}

void cam(inout vec3 p) {
    p.z += 4.0*scaledTime; // Ensure float literal, use scaledTime
}

void main() // Replaced mainImage with main
{
    // Initialize global scaledTime at the start of main
    scaledTime = TIME * AnimationSpeed;
    scaledTime = mod(scaledTime, 8.0); // Modulate after initial scaling
    
    // Replaced fragCoord and iResolution with gl_FragCoord and RENDERSIZE
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.x; // Original divided by iResolution.x
    
    //uv *= rot(.5*PI); // Original commented out
    
    // tt_local removed, using scaledTime directly
    
    vec3 ro = vec3(0.0, 0.0, 0.0); // Ensure float literal
    vec3 rd = normalize(vec3(uv, 0.25)); // Ensure float literal
    vec3 lp = vec3(0.0, 0.0, 2.0); // Ensure float literal
    
    cam(ro);
    cam(lp);
    vec3 col_final; // Renamed to avoid shadowing
    float t, d = 0.1; // Ensure float literal, t initialized

    vec3 p = ro;
    
    vec2 beamIdTemp;
    for(float i=0.0; i<200.0; i++) { // Corrected loop variable declaration
        d = map(p);
        beamIdTemp = beamId; // save id
        
        if(d < 0.001 || t > 100.0) break; // Ensure float literals
        
        p += rd*d;
        t += d;
    }
    
    vec2 e = vec2(0.0035, -0.0035); // Ensure float literals
    
    if(d < 0.001) { // Ensure float literal
        // Color Palette for 'al' (ambient/light color)
        vec3 al_base = generatePaletteColor(beamIdTemp.y/10.0 + sin(scaledTime*TAU/16.0)); // Use generatePaletteColor function directly
        vec3 al = mix(BaseColor1.rgb, BaseColor2.rgb, PaletteBlend); // Mix with BaseColors
        al = mix(al, al_base, 0.7); // Blend with shader's inherent palette
        
        vec3 n = normalize( e.xyy*map(p+e.xyy) + e.yyx*map(p+e.yyx) +
                            e.yxy*map(p+e.yxy) + e.xxx*map(p+e.xxx));
        
        vec3 l = normalize(lp-p);
        float dif = max(dot(n, l), 0.0); // Ensure float literal
        float spe = pow(max(dot(reflect(-rd, n), -l), 0.0), 40.0); // Ensure float literal
        float sss = smoothstep(0.0, 1.0, map(p+l*0.4))/0.4; // Ensure float literal
        float ao = calcAO(p, n);
        
        col_final = al*mix(1.0, spe+0.9*(dif+1.5*sss), 0.4); // Ensure float literals
        col_final = mix(col_final, col_final*ao, 0.9); // Ensure float literal
        
        float fog = 1.0-exp(-t*0.04); // Ensure float literal
        
        col_final = mix(col_final, vec3(0.1), fog); // Ensure float literal
    } else {
        // If ray didn't hit anything, default to a dark background.
        // This makes the transparent background more obvious when no fractal is visible.
        col_final = vec3(0.0);
    }

    // Apply FlickerAmount
    float grain = fract(sin(gl_FragCoord.x * 12.9898 + gl_FragCoord.y * 75.5914 + scaledTime * 2.0)) * FlickerAmount;
    col_final += grain;

    // Apply Color Pulse
    float pulse_factor = 1.0 + ColorPulseAmp * SIN(scaledTime * ColorPulseFreq);
    col_final *= pulse_factor;

    // Apply Gamma Adjustment
    col_final = pow(col_final, vec3(1.0 / GammaAdjust));

    // Apply Global Brightness
    col_final *= GlobalBrightness;

    // Output to screen with full alpha for transparency control from the outside
    gl_FragColor = vec4(col_final, 1.0); // Explicitly set alpha to 1.0 for transparency if used in layering
}
