/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Psychedelic",
        "Procedural"
    ],
    "DESCRIPTION": "A psychedelic shader with extensive tunable parameters for vibrant colors, pulses, shake, fractal, and geometry control.",
    "IMPORTED": {
        "Texture0": {
            "NAME": "Texture0",
            "PATH": "f735bee5b64ef98879dc618b016ecf7939a5756040c2cde21ccb15e69a6e1cfb.png"
        }
    },
    "INPUTS": [
        {
            "NAME": "Mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5]
        },
        {
            "NAME": "GLOW_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.006
        },
        {
            "NAME": "COLOR_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "DETAIL_FREQ",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "ABERRATION_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 4.1
        },
        {
            "NAME": "BLUR_AMOUNT",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.7
        },
        {
            "NAME": "ROTATION_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.3
        },
        {
            "NAME": "ZOOM_EFFECT",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.5
        },
        {
            "NAME": "COLOR_PALETTE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 6.0,
            "DEFAULT": 0.0,
            "STEP": 1.0
        },
        {
            "NAME": "PULSE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "SHAKE_STRENGTH",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.0
        },
        {
            "NAME": "FRACTAL_ITERATIONS",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 200.0, 
            "DEFAULT": 140.0,
            "STEP": 1.0
        },
        {
            "NAME": "GEOMETRY_DISTORTION",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "GEOMETRY_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
            "INPUTS": [
                {
                    "NAME": "BufferA",
                    "TYPE": "image"
                }
            ]
        }
    ]
}
*/

vec3 glow = vec3(0.0); // Initialize with floats
#define dmin(a, b) a.x < b.x ? a : b
#define PI acos(-1.)
#define tau (2.*PI)
#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))
#define TIME (TIME + 3.6 + 5.)

vec3 att = vec3(1.0); // Initialize with floats

// --- Psychedelic Color Palettes ---
vec3 getPaletteColor(float t) {
    t = fract(t); // Ensure t is between 0 and 1
    if (COLOR_PALETTE < 0.5) { // Palette 0: Rainbow Spectrum
        return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
    } else if (COLOR_PALETTE < 1.5) { // Palette 1: Fiery Sunset
        return mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 0.5, 0.0), t);
    } else if (COLOR_PALETTE < 2.5) { // Palette 2: Ocean Depths
        return mix(vec3(0.0, 0.0, 0.5), vec3(0.0, 0.8, 1.0), t);
    } else if (COLOR_PALETTE < 3.5) { // Palette 3: Alien Forest
        return mix(vec3(0.1, 0.5, 0.1), vec3(0.8, 1.0, 0.0), t);
    } else if (COLOR_PALETTE < 4.5) { // Palette 4: Cyberpunk Neon
        return mix(vec3(0.7, 0.0, 1.0), vec3(0.0, 1.0, 0.7), t);
    } else if (COLOR_PALETTE < 5.5) { // Palette 5: Vintage Pop
        return mix(vec3(0.9, 0.2, 0.5), vec3(0.2, 0.9, 0.8), t);
    } else { // Palette 6: Grayscale to Color Burst
        vec3 gray = vec3(t);
        return mix(gray, 0.5 + 0.5 * sin(6.28318 * (t * 2.0 + vec3(0.0, 0.33, 0.67))), smoothstep(0.0, 0.5, t));
    }
}

float pModPolar(inout vec2 p, float repetitions) {
    float angle = 2.*PI/repetitions;
    float a = atan(p.y, p.x) + angle/2.;
    float r = length(p);
    float c = floor(a/angle);
    a = mod(a,angle) - angle/2.;
    p = vec2(cos(a), sin(a))*r;
    if (abs(c) >= (repetitions/2.)) c = abs(c);
    return c;
}
#define pmod(p,x) mod(p,x) - 0.5*x

vec4 valueNoise(float t){
    return mix(texture(Texture0,mod(vec2(floor(t)/256.0),1.0)),texture(Texture0,mod(vec2(floor(t) + 1.0)/256.0,1.0)), smoothstep(0.0,1.0,fract(t))); // Added .0 to integers
}

float fOpUnionStairs(float a, float b, float r, float n) {
    float s = r/n;
    float u = b-r;
    return min(min(a,b), 0.5 * (u + a + abs ((mod (u - a + s, 2.0 * s)) - s))); // Added .0
}

float iii;
float sdRhombus(vec3 p, vec3 s){
    p = abs(p) - s;
    float d = max(p.z, max(p.x, p.y));
    d = max(d, dot(p.yx + s.yx*0.5, normalize(vec2(1.0)))); // Added .0
    d = max(d, dot(p.yz + s.yz*0.5, normalize(vec2(1.0)))); // Added .0
    return d;
}

float fOpUnionSoft(float a, float b, float r) {
    float e = max(r - abs(a - b), 0.0); // Added .0
    return min(a, b) - e*e*0.25/r;
}

float fOpUnionRound(float a, float b, float r) {
    vec2 u = max(vec2(r - a,r - b), vec2(0.0)); // Added .0
    return max(r, min (a, b)) - length(u);
}

float fOpUnionChamfer(float a, float b, float r) {
    return min(min(a, b), (a - r + b)*sqrt(0.5));
}

void pR45(inout vec2 p) {
    p = (p + vec2(p.y, -p.x))*sqrt(0.5);
}

float pMod1(inout float p, float size) {
    float halfsize = size*0.5;
    float c = floor((p + halfsize)/size);
    p = mod(p + halfsize, size) - halfsize;
    return c;
}

float fOpUnionColumns(float a, float b, float r, float n) {
    if ((a < r) && (b < r)) {
        vec2 p = vec2(a, b);
        float columnradius = r*sqrt(2.0)/((n-1.0)*2.0+sqrt(2.0)); // Added .0
        pR45(p);
        p.x -= sqrt(2.0)/2.0*r; // Added .0
        p.x += columnradius*sqrt(2.0); // Added .0
        if (mod(n,2.0) == 1.0) { // Added .0
            p.y += columnradius;
        }
        pMod1(p.y, columnradius*2.0); // Added .0
        float result = length(p) - columnradius;
        result = min(result, p.x);
        result = min(result, b);
        return min(result, a);
    } else {
        return min(a, b);
    }
}

float sdVerticalCapsule( vec3 p, float h, float r ) {
    p.y -= clamp( p.y, 0.0, h );
    return length( p ) - r;
}

float sdBox( vec3 p, vec3 s ) {
    p = abs(p) - s;
    return max(p.x, max(p.y, p.z));
}

#define modD vec3(1.5,2.5,0.9)
float sdThangiPong(vec3 p){
    float mmm = sin(TIME * COLOR_SPEED);

    mmm = sign(mmm)*pow(abs(mmm), 5.0); // Added .0

    p.y -= TIME + iii + sin(TIME + iii);
    p.y = pmod(p.y, modD.y);

    p.xz *= rot(mmm*PI);
    p.xz = abs(p.xz);
    float d = sdRhombus(p , vec3(0.12));

    glow += 0.5/(0.08+ d*d*4.0)*getPaletteColor(TIME * 0.1 + d*0.5)*att; // Added .0

    p.y -= 0.4;
    p.xz *= rot(0.25*PI);

    float n = fOpUnionStairs(d, sdRhombus(p - vec3(0.0,0.0,0.0), vec3(0.1)), 0.2,4.0); // Added .0
    d = min(d, n);
    return d;
}

float speed = 0.5;

vec2 map(vec3 p){
    // Apply geometry distortion
    vec3 original_p = p;
    p += sin(p * DETAIL_FREQ + TIME * GEOMETRY_SPEED) * GEOMETRY_DISTORTION;

    vec2 d = vec2(10e7);
    vec3 f = p;

    vec3 n = p;
    vec3 q = p;

    iii = pModPolar(q.xz, 3.0); // Added .0

    vec3 u = q;
    q.x-= 2.2;

    float dd = -q.x ;

    vec3 i = q;

    i.y = pmod(i.y, modD.y);
    vec3 k = i;
    i = abs(i);

    i = abs(i) - vec3(0.1,1.0,0.4); // Added .0
    float dm = max(i.z,max(i.y,i.x)) ;

    q = abs(q);
    q.z -= 1.8;

    float ddC = max(abs(q.x),abs(q.z)) - 0.2;

    dd = fOpUnionStairs(ddC, dd, 0.7,5.0); // Added .0

    q = pmod(q, modD);

    u = abs(q);
    float dW = min( min(
                        max(u.z, u.y),
                        max(u.x, u.y)
                    ),                          max(u.x, u.z)) - 0.04 ;

    dd = fOpUnionStairs(dd, dW, 0.4,4.0); // Added .0

    d = dmin(d, vec2(dd, 1.0)); // Added .0
    d = dmin(d, vec2(dm, 11.0)); // Added .0
    d = dmin(d, vec2(ddC, 1.0)); // Added .0

    if(d.x == dW){
        d.y = 6.0; // Added .0
    }

    n.y -= speed*TIME;

    float ddB = length(n) - 0.2;

    float mmm = pow(abs(sin(f.y*0.5 + TIME*0.5 * COLOR_SPEED + cos(f.z*DETAIL_FREQ + TIME*0.25) + cos(f.y*DETAIL_FREQ + TIME*0.125) )), 5.0); // Added .0
    float mmb = pow(abs(sin(f.y*DETAIL_FREQ + TIME * COLOR_SPEED + cos(f.z*0.4*DETAIL_FREQ + TIME*0.45) + cos(f.y*DETAIL_FREQ + TIME*0.5) )), 2.0); // Added .0

    float a= 0.0 + mmm*0.6; // Added .0

    n.xz *= rot(mmm*4.0 * ROTATION_SPEED); // Added .0
    n.yz *= rot(mmb*1.0 * ROTATION_SPEED); // Added .0

    vec3 j = n;
    j = pmod(j, 0.1);
    j = abs(j) - 0.06;
    float ddD = max(j.x, max(j.y, j.z));

    n = abs(n);
    n.yz *= rot(0.9 * ROTATION_SPEED);
    n.xz -= 0.1 + mmb*0.1;

    float dRr = sdRhombus(n , -vec3(0.04));
    float dRb = sdRhombus(n , -vec3(0.0,0.1,0.0)); // Added .0

    ddB = fOpUnionStairs(ddB,dRr,0.2 + mmb*0.1, 5.0 + sin(TIME) ); // Added .0
    ddB = fOpUnionStairs(ddB,dRb,0.25 + sin(TIME)*0.2, 5.0 ); // Added .0

    ddB = mix(ddB,ddD,a );
    d = dmin(d, vec2(ddB, 10.0)); // Added .0

    k.x += 1.2;

    float dT = sdThangiPong(k);

    float dPp = length(k.xz) - 0.1;
    k.x -= 0.5;
    k = abs(k);
    k.z -= 0.9;

    k = abs(k);
    float dPpb = max(k.x,k.z) - 0.06;
    glow += 0.2/(0.06 + dPpb*dPpb*dPpb*44.0)*getPaletteColor(TIME * 0.2 + dPpb * 2.0)*att; // Added .0

    d = dmin(d, vec2(dT, 4.0)); // Added .0
    d = dmin(d, vec2(abs(dPp) + 0.01, 4.0)); // Added .0
    d = dmin(d, vec2(abs(dPpb) + 0.01, 4.0)); // Added .0

    d.x *= 0.6;
    return d;
}

float dith;
float side;

vec2 march(vec3 ro, vec3 rd, inout vec3 p, inout float t, inout bool hit){
    vec2 d = map(p);
    if(d.x < 0.3) ro += rd*0.3;
    p = ro; t = 0.0; hit = false; // Added .0
    for(int i = 0; i < int(FRACTAL_ITERATIONS); i++){ // Cast FRACTAL_ITERATIONS to int
        d = map(p);
        d.x *= dith * side;
        if(d.x < 0.001){
            hit = true;
            break;
        }
        t += d.x;
        p = ro + rd*t;
    }
    return d;
}

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
    vec3 dir = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0.0,1.0,0.0),dir )); // Added .0
    vec3 up = normalize(cross(dir, right));
    return normalize(dir + right*uv.x + up*uv.y);
}

vec3 getNormal(vec3 p){
    vec2 t= vec2(0.0004,0.0); // Added .0
    // Apply geometry distortion for normal calculation as well, to match the map
    vec3 p_distorted = p + sin(p * DETAIL_FREQ + TIME * GEOMETRY_SPEED) * GEOMETRY_DISTORTION;

    // We need to calculate the normal based on the distorted space
    // A simplified approach for perturbed SDFs
    float eps = 0.0001;
    vec3 n_distorted;
    n_distorted.x = map(p_distorted + vec3(eps, 0.0, 0.0)).x - map(p_distorted - vec3(eps, 0.0, 0.0)).x;
    n_distorted.y = map(p_distorted + vec3(0.0, eps, 0.0)).x - map(p_distorted - vec3(0.0, eps, 0.0)).x;
    n_distorted.z = map(p_distorted + vec3(0.0, 0.0, eps)).x - map(p_distorted - vec3(0.0, 0.0, eps)).x;
    return normalize(n_distorted);
}

#define mx (2.0*Mouse.x/RENDERSIZE.x) // Added .0
#define my (0.6*Mouse.y/RENDERSIZE.x) // Added .0

void main() {
    if (PASSINDEX == 0)    {
        vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y; // Added .0
        // Apply Screen Shake
        vec2 shake_offset = vec2(sin(TIME * 20.0 + uv.x * 10.0), cos(TIME * 25.0 + uv.y * 12.0)) * SHAKE_STRENGTH; // Added .0
        uv += shake_offset;

        uv.xy *= rot(sin((TIME*0.7 - 3.6))*ROTATION_SPEED);
        uv *= 1.0 + dot(uv,uv)*ZOOM_EFFECT; // Added .0

        vec3 col = vec3(0.0); // Initialize with floats

        dith = mix(0.76,1.0, texture(Texture0,mod(20.0*uv*256.0,1.0)).x); // Added .0
        vec3 ro = vec3(0.0); // Initialize with floats

        ro.y += TIME*speed;
        ro.y -= 0.57 - my;

        float n = pow(valueNoise(TIME*1.0 * COLOR_SPEED).x, 2.0); // Added .0

        ro.y += sin(n);
        float nb = valueNoise(TIME*0.5 * COLOR_SPEED).x; // Added .0
        float zoom = 1.2 + n*1.0; // Added .0
        n *= 1.0; // Added .0
        ro.xz += vec2(sin( nb*6.14*1.5 + mx),cos( nb*6.14*1.5 + mx))*zoom;

        ro.y += 0.3;

        vec3 lookAt = vec3(0.0,ro.y + sin(TIME)*0.05,0.0); // Added .0
        lookAt.y += -0.5 + valueNoise(TIME*0.5 * COLOR_SPEED).x; // Added .0
        vec3 rd = getRd(ro, lookAt, uv);

        vec3 p; float t = 0.0; bool hit; // Added .0
        float tA = 0.0; side = 1.0; // Added .0

        // Apply a global color pulse effect to the lighting/material colors
        float pulse = 0.5 + 0.5 * sin(TIME * 5.0 * COLOR_SPEED) * PULSE_STRENGTH; // Added .0
        vec3 base_color = getPaletteColor(t * 0.1 + TIME * 0.05);
        base_color *= pulse;

        for(int i = 0; i < 2 + min(0, int(FRAMEINDEX)); i ++){ // Cast FRAMEINDEX to int
            vec2 d = march(ro, rd, p, t, hit);
            vec3 n = getNormal(p)*side;

            vec3 ld = normalize(vec3(1.0)); // Added .0
            vec3 j = normalize(ld - n);

            float diff = max(dot(n, ld), 0.0); // Added .0
            float ss = pow(max(dot(n, j), 0.0), 20.0); // Added .0
            float fres = pow(1.0 - max(dot(n, -rd), 0.0), 5.0); // Added .0

            col += ss*0.05*base_color*att;
            tA = max(tA,t);
            if (d.y == 10.0){ // Added .0
                col += fres*0.1*base_color*diff*att;
                ro = p + n*0.5;
                att *= vec3(0.2,0.7,1.0)*(0.9 ); // Added .0
                rd = reflect(rd, n);
            } else if (d.y == 11.0){ // Added .0
                col += fres*0.1*base_color*diff*att;
                ro = p + n*0.5;
                att *= vec3(0.2,0.6,1.0)*0.9; // Added .0
                rd = reflect(rd, n + sin(p*40.0)*0.01); // Added .0
            }else if (d.y == 20.0){ // Added .0
                // Explicitly perform component-wise multiplication here
                col += fres * 0.5 * (base_color * glow) * 0.05 * att;
                break;
            } else {
                #define aa(j) clamp(map(p + n/j).x*j, 0.0,1.0) // Added .0
                float aaa = aa(0.9)*aa(0.6)*10.0; // Added .0
                col += fres*0.5*base_color*1.0*att*aaa; // Added .0
                break;
            }
        }
        col += glow*GLOW_STRENGTH;
        gl_FragColor = vec4(col,1.0);
    }
    else if (PASSINDEX == 1)    {
        vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
        vec2 uvn = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.xy; // Added .0

        // Radial blur
        float steps = 20.0; // Added .0
        float scale = 0.00 + pow(length(uv - 0.5),4.0)*BLUR_AMOUNT; // Added .0
        float chromAb = pow(length(uv - 0.5),1.0)*ABERRATION_STRENGTH; // Added .0
        vec2 offs = vec2(0.0); // Added .0
        vec4 radial = vec4(0.0); // Added .0
        for(float i = 0.0; i < steps; i++){ // Added .0
            scale *= 0.97;
            vec2 target = uv + offs;
            offs -= normalize(uvn)*scale/steps;
            radial.r += texture(BufferA,mod(target + chromAb*1.0/RENDERSIZE.xy,1.0)).x; // Added .0
            radial.g += texture(BufferA,mod(target,1.0)).y; // Added .0
            radial.b += texture(BufferA,mod(target - chromAb*1.0/RENDERSIZE.xy,1.0)).z; // Added .0
        }
        radial /= steps;

        gl_FragColor = radial*3.0; // Added .0
        gl_FragColor = mix(gl_FragColor,smoothstep(0.0,1.0,gl_FragColor), 0.6); // Added .0
        gl_FragColor = max(gl_FragColor, 0.0); // Added .0
        gl_FragColor = pow(gl_FragColor, vec4(0.4545 + dot(uvn,uvn)*1.1)); // Added .0
        gl_FragColor *= 1.0 - dot(uvn,uvn)*0.6; // Added .0
    }
}