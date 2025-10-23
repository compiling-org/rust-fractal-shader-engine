/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/mtKyD3 by MV10. Three-way mix of phreax's butterfly effect, the related version 2, and Andre's fire effect. This one needs audio reactivity...",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "colorVariation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
        { "NAME": "intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0 }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferB"
        },
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferC"
        },
        {
        }
    ]
}
*/

#define PI 3.141592
#define SIN(x) (sin(x)*.5+.5)
#define hue(v) ( .6 + .6 * cos( 6.3*(v) + vec3(0,23,21) ) )

float tt;

mat2 rot2(float a) { return mat2(cos(a), sin(a), -sin(a), cos(a)); }

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

vec3 getPal(int id, float t) 
{
    id = id % 7;
    vec3 col = pal( t, vec3(.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,-0.33,0.33) );
    if( id == 1 ) col *= colorVariation;
    if( id == 2 ) col *= colorVariation * 0.8;
    if( id == 3 ) col *= colorVariation * 1.2;
    if( id == 4 ) col *= colorVariation * 1.5;
    if( id == 5 ) col *= colorVariation * 0.9;
    if( id == 6 ) col *= colorVariation * 1.3;
    return col;
}

float curve(float t, float d) {
  t/=d;
  return mix(floor(t), floor(t)+1., pow(smoothstep(0.,1.,fract(t)), 10.));
}

vec3 transform(vec3 p) {
    float a = PI*.5*curve(TIME * speed, 4.);
    p.xz *= rot2(a);
    p.xy *= rot2(a);
    return p;
}

float map(vec3 p) {
    vec3 bp = p;
    p = transform(p);
    p.x = abs(p.x) -.5*SIN(tt*.5);
    p.y = abs(p.y) -.9*SIN(tt*.8);
    p.y -= 0.1;
    p.y = abs(p.y) -.1;
    p.x -= 0.2;
    p.x = abs(p.x) -.9; 
    p.z = abs(p.z) -.5;
    p.zy -= 0.5;
    p.xy *= rot2(0.1*tt);
    p.zy *= rot2(-.04*tt);
    float r1 = 1.0;
    float r2 = mix(0.03, 0.3, SIN(TIME * speed));
    vec2 cp = vec2(length(p.xz) - r1, p.y);
    float a = atan(p.z, p.x);
    cp *= rot2(3.*a+tt);
    cp.x = abs(cp.x) - .3;
    cp *= rot2(3.*a);
    float n = 10.;
    for(float i = 0.; i< n; i++) {
        cp.y = abs(cp.y) -.05*(.5*sin(tt)+.9);
        cp *= rot2(0.1*a*sin(0.1*TIME * speed));
        cp -= i*0.01/n;
    }
    float d = length(cp) - r2;
    return .4*d * intensity;
}

void main() {
    if (PASSINDEX == 0) {
        vec2 uv = (gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y;
        float cz = -5.+1.5*sin(curve(TIME * speed, 4.));
        vec3 ro = vec3(0, .0, cz), rd = normalize(vec3(uv, .7));
        vec3 p = ro, col;
        float t, d = 0.1;
        tt = TIME * speed;  
        tt = tt+2.*curve(tt, 2.);
        float acc = 0.0;
        for(float i=.0; i<200.; i++) {
            d = map(p);
            if(d < 0.0001 || t > 100.) break;
            d = max(abs(d), 0.009);
            acc += 0.07;
            t += d;
            p += rd*d;
        }
        if(d < 0.001) {
            col += acc*clamp(1., 0., 1.2*abs(cz)/(t*t));
            float sl = dot(p,p);
            col *= 0.5*getPal(4, 1.-0.1*sqrt(sl)+0.05*p.z+.25*TIME+curve(TIME * speed, 8.));
        }
        col = pow(col, vec3(1.2))*1.4 * intensity;
        gl_FragColor = vec4(col, 1.0 - t * 0.3);
    }
}
