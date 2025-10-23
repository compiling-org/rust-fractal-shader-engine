/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Enhanced version with tunable parameters for psychedelic effects",
    "IMPORTED": {
    },
    "INPUTS": [
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "morphIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "artifactStrength", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "patternScale", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0}
    ]
}
*/

#define PI 3.141592
#define TAU 2.*PI
#define hue(v) ( .6 + .6 * cos( colorShift * 6.3*(v) + vec3(0,23,21) ) )
#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define COUNT 30.
#define N 5.
#define SQR(x) ((x)*(x))

float tt;

vec2 kalei(vec2 uv) { 
    float n = N;
    float r = TAU/n;
    
    for(float i=0.; i<n; i++) {     
    	uv = abs(uv);
        uv.x -= .2*i+.2 * morphIntensity;
    	uv *= rot(r*i-.09*tt);
    }
    
    uv = abs(uv) - (sin(.15*tt) + 1.2 + artifactStrength * sin(tt*5.0));

    return uv;
}

float flower(vec2 uv) {
    float n = 3. + morphIntensity * 2.0;
    float a = atan(uv.y,uv.x);

    float d = length( uv) - cos(a*n);
    return smoothstep(fwidth(d), 0., abs(d));    
}

vec3 spiral(vec2 uv, float i) {  
    uv *= rot(i*PI+tt*.3);
    uv += 0.7*sin(5.*uv.yx * colorPulseSpeed);
	return flower(uv) * SQR(hue(i+tt*0.2));
}

void main() {
    vec2 uv = (gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 col = vec3(0);
    tt = TIME;
    
    uv = kalei(uv * patternScale);
    float s = 1./COUNT;
	
    for(float i=0.; i<1.; i+=s) {   
        float z = fract(i - 0.1 * tt * colorPulseSpeed);
        float fade = smoothstep(0., .1, 1.-abs(2.*z-1.));
        col += spiral(uv*z, i) * fade;
    }
    col = sqrt(col);
    gl_FragColor = vec4(col,1.);
}
