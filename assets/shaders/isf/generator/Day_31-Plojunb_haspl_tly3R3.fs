/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Updated shader with functional color palette, color pulse, and color shift.",
    "INPUTS": [
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0
        },
        {
            "NAME": "zoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0
        },
        {
            "NAME": "colorShift",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "colorPulse",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 3.0
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ]
}
*/

#define dmin(a, b) a.x < b.x  ? a : b
#define dmax(a, b) a.x > b.x  ? a : b
#define pi acos(-1.)
#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))
#define mx (20.*speed/RENDERSIZE.y)

// Hex code from BigWings! He has a tutorial on them.
float HexDist(vec2 p) {
    p = abs(p);
    float c = dot(p, normalize(vec2(1,1.73)));
    c = max(c, p.x);
    return c;
}

vec4 HexCoords(vec2 uv) {
    vec2 r = vec2(1, 1.73);
    vec2 h = r*.5;
    vec2 a = mod(uv, r)-h;
    vec2 b = mod(uv-h, r)-h;
    vec2 gv = dot(a, a) < dot(b,b) ? a : b;
    float x = atan(gv.x, gv.y);
    float y = .5-HexDist(gv);
    vec2 id = uv-gv;
    return vec4(x, y, id.x,id.y);
}

#define pmod(p, x) mod(p, x) - x*0.5
#define modDist 3.
float id;

vec2 map(vec3 p){
    vec2 d = vec2(10e9);
    id = floor(p.z/modDist);
    
    vec3 z = p;
    p.xy *= rot(id*1.2 - TIME*0.5 * speed);
    p.z = pmod(p.z, modDist);
    for (float i = 0.; i < 9.; i++){
        p = abs(p);
        p.xy *= rot(0.9 + i*1.8 + id*0. );
        p.zy *= rot(0.025 + sin(id)*0.03);
        p.xz *= rot(0.02 + exp(-length(z.xy)*1.)*0.57 + sin(id*0.6)*0.01);
    }
    
    vec4 hc = HexCoords(p.xy);
    d = dmin(d, vec2((hc.y - 0.12), 0.));
    d.x = max(d.x, (abs(p.z) - 0.1));
    d = dmin(d, vec2(max((hc.y - 0.19), (abs(p.z) - 0.06)), 1.));
    d.x *= 0.8;
    return d;
}

#define fov 2.
vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
    vec3 dir = normalize(lookAt - ro);
    vec3 right = normalize(cross(vec3(0,1,0), dir));
    vec3 up = normalize(cross(dir, right));
    return normalize(dir + right*uv.x*fov + up*uv.y*fov);
}

vec3 glow = vec3(0);

// Helper function for smooth color transitions
vec3 hue(float v) {
    return vec3(0.6 + 0.6 * cos(6.3*(v) + vec3(0,23,21)));
}

// Function to apply color palette
vec3 getColorFromPalette(float t, float paletteIndex) {
    if (paletteIndex < 1.0) {
        // Palette 0: Smooth gradient
        return hue(t);
    } else if (paletteIndex < 2.0) {
        // Palette 1: Triadic colors
        return mix(hue(t), hue(t + 0.33), 0.5);
    } else if (paletteIndex < 3.0) {
        // Palette 2: Complementary colors
        return mix(hue(t), hue(t + 0.5), 0.5);
    } else {
        // Palette 3: Randomized colors
        return vec3(fract(sin(t * 123.456) * 43758.5453),
                    fract(sin(t * 234.567) * 43758.5453),
                    fract(sin(t * 345.678) * 43758.5453));
    }
}

void main() {
    if (PASSINDEX == 0) {
        vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    
        vec3 col = vec3(0);
        vec3 ro = vec3(0,0,-2. + (TIME + mx)*5.);
        vec3 lookAt = ro + vec3(0,0,1);
        
        lookAt.x += sin(TIME)*(0.25 + sin(TIME*0.5)*0.25)*0.3;
        lookAt.y += cos(TIME)*(0.25 + sin(TIME*0.5)*0.25)*0.3;
        vec3 rd = getRd(ro, lookAt, uv);
        
        // Apply zoom effect
        rd.xy *= zoom;
        
        // Time-based variables
        float tt = TIME * speed;
        float pulse = sin(tt * colorPulse) * 0.5 + 0.5; // Oscillating pulse effect
        float shift = sin(tt * colorShift) * 0.5 + 0.5; // Color shift effect
        
        // Generate base color using the selected palette
        vec3 baseColor = getColorFromPalette(shift, colorPalette);
        
        // Apply pulse effect
        baseColor *= pulse;
        
        rd.xy *= 1. - dot(uv,uv)*0.2;
        rd = normalize(rd);
        float t= 0.; vec3 p = ro;
        #define iters 260.
        for (float i = 0.; i < iters; i++){
            vec2 d = map(p);
            float glowSc = exp(-d.x*20.);
            vec3 glowCol = vec3(0);
            if (d.y == 0.){
                glowCol = baseColor; // Use the dynamic base color
            } else {
                glowCol = baseColor * 0.5; // Dimmed version of the base color
            }
            glow += glowSc * glowCol;
            if (d.x < 0.001 || t > 100.) break;
            t += d.x;
            p = ro + rd*t;
        }
        
        col += glow*0.04;
        gl_FragColor = vec4(col,1.0);
    }
    else if (PASSINDEX == 1) {
        vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
        vec2 uvn = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.xy;
        
        // Radial blur
        float steps = 20.;
        float scale = 0.00 + pow(length(uv - 0.5)*1.2,2.7)*0.7;
        float chromAb = pow(length(uv - 0.5),1.4)*9.1;
        vec2 offs = vec2(0);
        vec4 radial = vec4(0);
        for(float i = 0.; i < steps; i++){
            scale *= 0.91;
            vec2 target = uv + offs;
            offs -= normalize(uvn)*scale/steps;
            radial.r += IMG_NORM_PIXEL(BufferA,mod(target + chromAb*1./RENDERSIZE.xy,1.0)).x;
            radial.g += IMG_NORM_PIXEL(BufferA,mod(target,1.0)).y;
            radial.b += IMG_NORM_PIXEL(BufferA,mod(target - chromAb*1./RENDERSIZE.xy,1.0)).z;
        }
        radial /= steps;
        gl_FragColor = radial*1.; 
        gl_FragColor = mix(gl_FragColor,smoothstep(0.,1.,gl_FragColor), 0.9);
        gl_FragColor *= 1. - dot(uvn,uvn)*1.8;
    }
}