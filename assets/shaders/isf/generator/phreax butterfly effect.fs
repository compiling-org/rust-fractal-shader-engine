/*
{
  "CATEGORIES": [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION": "Modified version with tunable parameters - fixed loop index error",
  "IMPORTED": {},
  "INPUTS": [
    {
      "NAME": "BackgroundColor",
      "TYPE": "color",
      "DEFAULT": [0.0,0.0,0.0,1.0],
      "LABEL": "BG Color"
    },
    {
      "NAME": "BackgroundAlpha",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "BG Alpha"
    },
    {
      "NAME": "PaletteSelect",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 6.0,
      "LABEL": "Palette"
    },
    {
      "NAME": "ColorPulseSpeed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0,
      "LABEL": "Pulse Speed"
    },
    {
      "NAME": "FractalSpeed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 3.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "GlowIntensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 5.0,
      "LABEL": "Glow"
    },
    {
      "NAME": "SymmetricalMode",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Symmetrical"
    }
  ]
}
*/

#define PI 3.1415926535
#define SIN(x) (sin(x)*0.5+0.5)
#define hue(v) (0.6 + 0.6 * cos(6.3*(v) + vec3(0,23,21)))
#define MAX_STEPS 100

float tt;
vec3 ro; 

mat2 rot2(float a) { return mat2(cos(a), sin(a), -sin(a), cos(a)); }

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d ) {
    return a + b*cos( 6.28318*(c*t+d) );
}

vec3 getPal(float id, float t) {
    if (id < 1.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,-0.33,0.33));
    else if (id < 2.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20));
    else if (id < 3.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.3,0.20,0.20));
    else if (id < 4.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,0.5),vec3(0.8,0.90,0.30));
    else if (id < 5.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,0.7,0.4),vec3(0.0,0.15,0.20));
    else if (id < 6.0) return pal(t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(2.0,1.0,0.0),vec3(0.5,0.20,0.25));
    else return pal(t, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25));
}

float curve(float t, float d) {
    t /= d;
    return mix(floor(t), floor(t)+1.0, pow(smoothstep(0.0,1.0,fract(t)), 10.0));
}

vec3 transform(vec3 p) {
    float a = PI*0.5*curve(TIME*FractalSpeed, 4.0);
    p.xz *= rot2(a);
    p.xy *= rot2(a);
    return p;
}

float map(vec3 p) {
    vec3 bp = p;
    p = transform(p);
    
    if (SymmetricalMode)
        p.y = -(abs(p.y) - 0.3);  
    
    p.x = abs(p.x) - 0.5*SIN(tt*0.5);
    p.y = abs(p.y) - 0.9*SIN(tt*0.8);
    p.y -= 0.1;
    p.y = abs(p.y) - 0.1;
    p.x -= 0.2;
    p.x = abs(p.x) - 0.9; 
    p.z = abs(p.z) - 0.5;

    p.zy -= 0.5;
    p.xy *= rot2(0.1*tt);
    p.zy *= rot2(-0.04*tt);
                       
    float r1 = 1.0;
    float r2 = mix(0.03, 0.3, SIN(TIME));
    
    vec2 cp = vec2(length(p.xz)-r1, p.y);
    
    float a = atan(p.z, p.x);
    cp *= rot2(3.0*a+tt);
    cp *= vec2(3.0, 0.4);
    cp.x = abs(cp.x) - 0.3;
    cp *= rot2(2.0*a);
    
    const float n = 10.0;
    for (float i = 0.0; i < n; i++)
    {
        cp.y = abs(cp.y) - 0.05*(0.5*sin(tt)+0.9);
        cp *= rot2(0.1*a*sin(0.1*TIME));
        cp -= i*0.01/n;
    }

    float d = length(cp) - r2;
    d = max(0.09*d, -length(bp.xy-ro.xy) - 4.0);
    return d;
}

void main() {
    vec2 uv = (gl_FragCoord.xy-0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    float cz = -7.0+sin(curve(TIME*FractalSpeed, 4.0));
     
    ro = vec3(0.0, 0.0, cz);
    vec3 rd = normalize(vec3(uv, 0.7));
    vec3 p = ro;
    vec3 col = BackgroundColor.rgb;
    
    tt = TIME * FractalSpeed;  
    tt += 2.0*curve(tt, 2.0);
    
    int steps;
    float t = 0.0;
    float d, acc = 0.001;
    
    for (int i = 0; i < MAX_STEPS; i++)
    {
        d = map(p);
        t += d;
        p += rd*d;
        acc += 0.01+d*0.4;
        
        float sl = dot(p,p);
        col += getPal(PaletteSelect, 
                    1.0-0.1*sqrt(sl) + 0.2*p.z + 
                    0.25*TIME*ColorPulseSpeed + curve(TIME, 8.0)) * 
                    (GlowIntensity/(1.0+t*t));
        
        if (t > 150.0 || d < 0.001) break;
    }
    
    if (d < 0.001) {
        vec2 e = vec2(0.0035, -0.0035);
        vec3 n = normalize(e.xyy*map(p+e.xyy) + e.yyx*map(p+e.yyx) + e.yxy*map(p+e.yxy) + e.xxx*map(p+e.xxx));
        vec3 l = normalize(vec3(0.0,1.0,cz)-p);
        float dif = max(dot(n,l),0.0);
        float spe = pow(max(dot(reflect(-rd,n),-l),0.0),40.0);
        float sss = smoothstep(0.0,1.0,map(p+l*0.4))/0.4; 
        
        col *= mix(1.0, 0.4*spe+0.8*(dif+2.5*sss), 0.4);
    }
    
    col = mix(BackgroundColor.rgb, col, BackgroundAlpha);
    gl_FragColor = vec4(col, 1.0 - t*0.0005*BackgroundColor.a);
}