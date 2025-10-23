/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/3lGGzR by jeyko.  asdf",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "Speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "DEFAULT": 7.0,
            "MIN": 1.0,
            "MAX": 10.0
        }
    ]
}
*/


#define TIME (TIME * Speed + 9.)
#define rot(x) mat2(cos(x), -sin(x), sin(x), cos(x))
#define PI acos(-1.)
#define pi acos(-1.)
#define dmin(a, b) a.x < b.x ? a : b
#define fov (0.8 + sin(TIME)*0.05)

vec3 fold(vec3 p) {
	vec3 nc = vec3(-.5, -.809017, .309017 + sin(TIME)*0.002);
	for (int i = 0; i < 5; i++) {
		p.xy = abs(p.xy);
		p -= 2.*min(0., dot(p, nc))*nc;
	}
	return p - vec3(0, 0, 1.275);
}

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
	vec3 dir = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0,1,0), dir));
    vec3 up = normalize(cross(dir, right));
    return dir + right*uv.x*fov + up*uv.y*fov;
}

vec2 map(vec3 p){
	vec2 d = vec2(10e5);
    
    vec3 q = p;
    p = fold(p);
    vec3 z = p;
    float b = pow((1. + sin(p.x*3.)), 0.7) - 1.;
    vec3 l = p;
    l.xz *= rot(sin(TIME*0.7)*0.4 - 0.2);
    b = min(b, sin(l.x*3.)*3.);
    z.z += 0.2;
    z.x += 0.82;
    d = dmin(d, vec2(mix(dot(z - 0.6, vec3(0.1,0.1,0.2)), b, 0.1),1.));
    q = fold(q) - vec3(0.5,0.6,-0.2);
    q = abs(q);
    q.z -= 0.2;
    d = dmin(d, vec2( dot(q, normalize(vec3(1))),2.));
    p.x -= 0.2;
    p.y += 0.7;
    
    p = fold(p) - vec3(0.4,0.4,20.);
    p = fold(p) - vec3(0.,0.4,20.);
    p = fold(p) - vec3(0.,0.,1.);
    
    for (int i = 0;i < 2; i++){
    	p = abs(p);
        p.x -= 0.2;
    }
    
    p -= 7.;
    
    p = fold(p);
    
    p -= 0.6;
    d = dmin(d, vec2( max(p.x, p.y), 1. ));
    p.xy *= rot(2.5);
    
    d.x *= 1.;
    d.x += sin(p.x*10.)*0.06;
    return d;
}
    
vec3 getNormal(vec3 p){
    vec2 t = vec2(0.001,0.);
    return normalize(map(p).x - vec3(
        map(p - t.xyy).x,
        map(p - t.yxy).x,
        map(p - t.yyx).x
    ));
}

#define zoom (Zoom + sin(TIME)*0.4)
#define rotSpeed (TIME*0.2 + cos(TIME*0.7)*0.5 - sin(TIME*0.5)*0.9)
#define pal(x,t) (0.5 + sin(x*vec3(2.9,5.,1.2) + t + sin(TIME)))

vec3 glow = vec3(0);

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 col = vec3(0);
    vec3 ro = vec3(sin(rotSpeed)*zoom,0. + sin(TIME),cos(rotSpeed)*zoom);
    vec3 lookAt = vec3(0);
    vec3 rd = getRd(ro, lookAt, uv);
    
    vec3 p = ro; float t, tBeforeRefraction = 0.;
    float bounce = 0.;
    vec3 attenuation = vec3(1);
    float side = 1.;
    #define ITERS 200.
    for(float i = 0.; i <= ITERS ; i ++){
    	vec2 d = map(p);
        d.x *= side;
		glow += pal(t*0.2 + exp(-d.x*10.) + 2., 2.6)* .095 / (.01 + d.x*d.x);
        
        if (d.x < 0.002){
            vec3 lDir = normalize(vec3(1));
            vec3 n = getNormal(p)*side;
            vec3 h = normalize(lDir - rd);
            float diff = max(dot(lDir,n),0.);
            float fres = pow(1. - max(dot(-rd,n),0.), 20.);
            float spec = pow(max(dot(h,n),0.), 10.);
            if (d.y == 2.) {
            	col += mix((sin(vec3(2.9,2.9,2.6)*glow*0.005) + 7.)*0.1 *diff ,vec3(0.005*glow*min(fres*spec*diff, 1.)), 0.2)*attenuation;
                break;
            } else {
                col += mix(0.001*glow *diff ,vec3(0.005*glow*min(fres*spec, 1.)), 0.5)*attenuation;
                attenuation *= vec3(0.2,0.4,0.9)*0.6;
                ro = p;
                rd = refract(rd, n, 1. + n.y*0.07 + n.x*0.2);
                side = -side;
                bounce++;
                if(bounce == 1.) {
                    tBeforeRefraction = t;
                }
                t = 0.;
            }
        }
        if (t > 50. || i == ITERS) break;
        t += d.x;
        p = ro + rd*t;
    }
    col *= 1.5;
    gl_FragColor = vec4(col,tBeforeRefraction);
}
