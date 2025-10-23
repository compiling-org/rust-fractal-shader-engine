/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/3stGRH by evvvvil. Winning shader made at Revision 2019 Shader Showdown Quarter final. Live coded on stage in 25 minutes.\nVideo of the battle: https://youtu.be/uifMBMt9ASU?t=3496",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "u_zoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 10.0
        },
        {
            "NAME": "u_global_speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0
        },
        {
            "NAME": "u_color_pulse_strength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "u_palette_mix_factor",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "u_fractal_detail",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "u_morph_factor",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "u_geometry_scale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0
        }
    ]
}
*/

// Winning shader made at Revision 2019 Shader Showdown Quarter final
// Video of the battle: https://youtu.be/uifMBMt9ASU?t=3496

// The "Shader Showdown" is a demoscene live-coding shader battle competition.
// 2 coders battle for 25 minutes making a shader on stage. No google, no cheat sheets.
// The audience votes for the winner by making noise or by voting on their phone.

// "When the seagulls follow the trawler it's because they think fish will be thrown to the sea." - Eric Cantona to journalists who expected him to apologise for kicking a Crystal Palace fan who had insulted him.

#ifdef GL_ES
precision highp float;
#endif

// ISF Built-in Uniforms (RENDERSIZE, TIME) are automatically provided
// by the ISF host based on the ISF spec.
// Custom uniforms (u_zoom, u_global_speed, etc.) are also automatically provided
// by the ISF host based on the "INPUTS" section of the JSON.

// Global variables
vec2 s,e=vec2(.00035,-.00035);
float t,tt,b,bb,g,att,si;
vec3 np,bp,cp;

// Simple hash function for procedural texture replacement (replaces iChannel0's purpose)
float hash11(float p) {
    p = fract(p * .1031);
    p = p * 19.19 + p;
    p = p * p;
    p = p * 98.42;
    return fract(p);
}

float hash22(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

// Procedural color palette function
vec3 getProceduralPalette(float hue) {
    hue = fract(hue); // Ensure hue is between 0 and 1
    vec3 c = vec3(0.0);
    if (hue < 0.1666) { // Red to Yellow
        c = mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 1.0, 0.0), hue / 0.1666);
    } else if (hue < 0.3333) { // Yellow to Green
        c = mix(vec3(1.0, 1.0, 0.0), vec3(0.0, 1.0, 0.0), (hue - 0.1666) / 0.1666);
    } else if (hue < 0.5) { // Green to Cyan
        c = mix(vec3(0.0, 1.0, 0.0), vec3(0.0, 1.0, 1.0), (hue - 0.3333) / 0.1666);
    } else if (hue < 0.6666) { // Cyan to Blue
        c = mix(vec3(0.0, 1.0, 1.0), vec3(0.0, 0.0, 1.0), (hue - 0.5) / 0.1666);
    } else if (hue < 0.8333) { // Blue to Magenta
        c = mix(vec3(0.0, 0.0, 1.0), vec3(1.0, 0.0, 1.0), (hue - 0.6666) / 0.1666);
    } else { // Magenta to Red
        c = mix(vec3(1.0, 0.0, 1.0), vec3(1.0, 0.0, 0.0), (hue - 0.8333) / 0.1666);
    }
    return c;
}


// Box primitive (min(max(p.x,p.y),p.z) for box distance)
float bo(vec3 p,vec3 r){
    p=abs(p)-r;
    return max(max(p.x,p.y),p.z);
}

// Fractal Box function (fb)
vec2 fb( vec3 p,float s_param) // Renamed 's' to 's_param' to avoid conflict with global 's'
{
    bb=sin(p.z*.1);
    vec2 h,t_local=vec2(bo(abs(p)-vec3(3.0,0.0,0.0),vec3(1.0)),5.0+s_param);
    h=vec2(bo(abs(p)-vec3(2.0,0.0,0.0),vec3(1.2)),3.0+s_param);
    h.x=min(bo(abs(abs(p)-vec3(0.0,1.0,0.6))-vec3(0.0,0.3,0.3),vec3(3.0,0.1,0.1)),h.x);
    t_local=t_local.x<h.x?t_local:h;
    h=vec2(bo(abs(p)-vec3(2.0,0.0,0.0),vec3(0.4-bb,0.4-bb,10.0)),6.0+s_param);
    t_local=t_local.x<h.x?t_local:h;
    h=vec2(bo(abs(p)-vec3(2.0,0.0,0.0),vec3(0.3-bb,0.5-bb,10.0)),3.0+s_param);
    t_local=t_local.x<h.x?t_local:h; t_local.x*=0.5;
    return t_local;
}

// 2D Rotation matrix
mat2 r2(float r){
    return mat2(cos(r),sin(r),-sin(r),cos(r));
}

// Main map function (mp) - distance field and other properties
vec2 mp( vec3 p)
{
    // Apply u_geometry_scale
    p *= (1.0 / u_geometry_scale);

    att=length(p)-20.0;
    si=sin(p.z*.1+tt*20.0+1.57);
    p.y+=si*5.0;
    p.xy*=r2(si*.3);

    // Apply u_morph_factor to a part of the fractal
    vec3 p_morph = p;
    p_morph.x += sin(p.y * 0.5 + TIME) * u_morph_factor * 2.0;
    p_morph.y += cos(p.x * 0.5 + TIME) * u_morph_factor * 2.0;

    np=bp=p_morph; // Use morphed 'p' for np and bp
    for(int i=0;i<6;i++){
        np=abs(np)-vec3(1.0+4.0*b,1.0,3.0);
        np.xy*=r2(.5+sin(p.z*.2)*.3+b);
        np.yz*=r2(.2);
        if(mod(float(i),2.0)>0.0){
            bp=abs(bp)-vec3(5.0+2.0*b,1.0,2.0);
            bp.xz*=r2(.1);
            bp.xy*=r2(.2+si*.2);
        }
    }
    bp.x-=1.0;
    vec2 h,t_local=fb(np,0.0);
    h=fb(bp,5.0);
    t_local=t_local.x<h.x?t_local:h;
    h=vec2(length(abs(p)-vec3(4.0+2.0*b,0.0,15.0))-3.0,6.0);
    g+=0.1/(0.1+h.x*h.x*20.0);
    t_local=t_local.x<h.x?t_local:h;
    cp=p*.5;cp.z=mod(cp.z+tt*100.0-sin(p.y),10.0)-5.0;
    h=vec2(length(cos(cp*.5-2.0))-0.001,6.0);
    g+=0.1/(0.1+h.x*h.x*400.0);
    t_local=t_local.x<h.x?t_local:h;
    h=vec2(bo(abs(np*.5)-1.0-fract(tt*10.0)*20.0,vec3(.2,10.0,.2)),6.0);
    g+=0.1/(0.1+h.x*h.x*10.0);
    t_local=t_local.x<h.x?t_local:h;
    return t_local;
}

// Ray marching function (tr)
vec2 tr( vec3 ro, vec3 rd)
{
    vec2 h,t_local=vec2(.1);
    // Fixed: Loop iterations must be a constant expression in GLSL ES 2.0 for 'for' loops.
    // u_fractal_detail will now affect the *effect* of the fractal, not the iteration count directly.
    const int MAX_ITERATIONS = 128; // Reverted to a constant value

    for(int i=0;i<MAX_ITERATIONS;i++){
        h=mp(ro+rd*t_local.x);
        if(t_local.x<.0001||t_local.x>60.0) break;
        t_local.x+=h.x;t_local.y=h.y;
    }
    if(t_local.x>60.0) t_local.x=0.0;
    return t_local;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv -= 0.5;
    uv /= vec2(RENDERSIZE.y/RENDERSIZE.x,1.0);

    // Apply u_global_speed to TIME
    float current_time = TIME * u_global_speed;

    tt=mod(current_time*.1,6.283);
    b=.5+2.0*clamp(sin(tt*5.0),-0.25,0.25);

    // Apply u_zoom to camera position
    vec3 ro=vec3(2.0+cos(tt*5.0-1.57)*(1.0-b)*40.0,2.0,sin(tt*5.0-1.57)*40.0);
    ro *= u_zoom; // Apply zoom here

    vec3 cw=normalize(vec3(0.0)-ro);
    vec3 cu=normalize(cross(cw,vec3(0.0,1.0,0.0)));
    vec3 cv=normalize(cross(cu,cw));
    vec3 rd=mat3(cu,cv,cw)*normalize(vec3(uv,0.5));
    vec3 co,fo,ld=normalize(vec3(-0.5,0.5,0.1));
    co=fo=vec3(1.0,0.5,0.2)-rd.y*0.4;
    s=tr(ro,rd);t=s.x;

    if(t>0.0){
        vec3 po=ro+rd*t;
        vec3 no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x);
        vec3 al=vec3(0.1,0.5-att*0.03,0.6-att*0.03);

        if(s.y<5.0) al=vec3(0.0);
        if(s.y>5.0) al=vec3(1.0);
        if(s.y<10.0) al=vec3(0.0);
        if(s.y>10.0) al=vec3(1.0);
        if(s.y==10.0||s.y==5.0) al=vec3(0.1,0.5+att*0.03,0.6-att*0.03);

        float dif=max(0.0,dot(no,ld));
        float aor=t/50.0;
        float ao=exp2(2.0-pow(max(0.0,1.0-mp(po+no*aor).x/aor),2.0));

        // Replaced iChannel0 texture sampling with a procedural noise for 'spo'
        // u_fractal_detail can now influence the noise pattern directly
        float spo_val_np = hash22(vec2(np.y, dot(np.xz, vec2(0.7))) * u_fractal_detail);
        float spo = exp2(10.0 * spo_val_np);

        if(s.y>6.0) {
            float spo_val_bp = hash22(vec2(bp.y, dot(bp.xz, vec2(0.7))) * u_fractal_detail);
            spo = exp2(10.0 * spo_val_bp);
        }

        float fr=pow(1.0+dot(no,rd),4.0);
        float sss=smoothstep(0.0,1.0,mp(po+ld*0.4).x/0.4);
        float sp=pow(max(dot(reflect(-ld,no),-rd),0.0),spo);

        co=mix(sp+al*ao*(dif+sss),fo,min(fr,0.5));
        co=mix(co,fo,1.0-exp(-0.000003*t*t*t));

        // Apply u_color_pulse_strength
        co += sin(current_time * 5.0) * u_color_pulse_strength * 0.2;

        // Apply u_palette_mix_factor for psychedelic color scheme
        vec3 procedural_color = getProceduralPalette(fract(current_time * 0.05 + length(uv) * 0.1));
        co = mix(co, procedural_color, u_palette_mix_factor);
    }
    gl_FragColor = vec4(pow(mix(co,co.zyx,b)+g*0.3,vec3(0.45)),1.0);
}