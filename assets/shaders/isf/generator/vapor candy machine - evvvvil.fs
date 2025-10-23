/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Vaporwave"
    ],
    "DESCRIPTION": "Vappour Candy Machine - Act 1, converted for ISF. Now features dynamic color palettes and zoom control, with no image input.",
    "INPUTS": [
        { "NAME": "TimeScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "BassIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Bass Effect" },
        { "NAME": "HighIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "High Freq Effect" },
        { "NAME": "OverallFreq", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Overall Freq Effect" },

        { "NAME": "CameraSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Camera Move Speed" },
        { "NAME": "CameraOrbitAmp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Orbit Amp" },
        { "NAME": "CameraTargetOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Camera Target Y Offset" },
        { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom" },

        { "NAME": "MidSphereSize", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Mid Sphere Size" },
        { "NAME": "MiddleSplineBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Middle Spline Brightness" },
        { "NAME": "ParticleGlow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Particle Glow" },
        { "NAME": "CogScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Cog Scale" },
        { "NAME": "InnerCogsBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Inner Cogs Brightness" },
        { "NAME": "CageDistortion", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Cage Distortion" },
        { "NAME": "YellowRingGlow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Yellow Ring Glow" },
        
        { "NAME": "ColorPaletteIndex", "TYPE": "float", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette", "VALUES": ["Vaporwave Sunset", "Cyberpunk Neon", "Dreamy Pastel", "Classic Retro"] }
    ]
}
*/

// Vappour Candy Machine - Trilogy of audio responsive eye candies - ACT 1
// Converted for ISF by Gemini

// --- Helper Functions ---
vec2 z,v,e=vec2(.00035,-.00035),cageSP;int s; float t,tt,b,pn,bb,g,gg,ggg; // 'tn' renamed to 'pn' for procedural noise
vec3 ro,bp,pp,op,sp,po,no,al,lp,ld,cageP;

const float PI = 3.14159265359;

// Distance functions
float bo(vec3 p,vec3 r){p=abs(p)-r; return max(max(p.x,p.y),p.z);}
float cx(vec3 p,vec3 r){return max(abs(length(p.yz)-r.x)-r.y,abs(p.x)-r.z/2.);}
float cx2(vec3 p,vec3 r){return max(abs(abs(length(p.yz)-r.x)-r.y*8.)-r.y,abs(p.x)-r.z/2.);}
float cz(vec3 p,vec3 r){return max(abs(length(p.xy)-r.x)-r.y,abs(p.z)-r.z/2.);}

// 2D Rotation matrix
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}

// Smooth min/max functions
float smin( float a, float b, float k ){float h=max(k-abs(a-b),0.0);return min(a, b) - h*h*0.25/k;}
float smax( float d1, float d2, float k ){float h=max(k-abs(-d1-d2),0.0);return max(-d1,d2)+h*h*0.25/k;}

// Procedural Noise Function (replaces texture input)
float hash12(vec2 p) {
    vec3 p3  = fract(p.xyx * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float proceduralNoise(vec2 uv){
    float f = 0.;
    f += hash12(uv * .125) * .5;
    f += hash12(uv * .25) * .25;
    f += hash12(uv * .5) * .125;
    f += hash12(uv * 1.) * .125;
    f = pow(f, 1.2);
    return f * .45 + .05;
}

// Color Palette Function
vec3 getPaletteColor(float v, int paletteIndex) {
    vec3 color;
    // Normalize v to [0, 1] for palette lookup, can adjust range
    v = clamp(v, 0.0, 1.0); 

    if (paletteIndex == 0) { // Vaporwave Sunset
        color = mix(vec3(0.0, 0.0, 0.5), vec3(1.0, 0.5, 0.0), v); // Dark Blue to Orange
        color = mix(color, vec3(0.8, 0.0, 0.8), sin(v * PI)); // Add some purple
    } else if (paletteIndex == 1) { // Cyberpunk Neon
        color = mix(vec3(0.0, 0.8, 0.8), vec3(0.0, 0.2, 1.0), v); // Cyan to Blue
        color = mix(color, vec3(1.0, 0.0, 0.5), cos(v * PI * 2.0)); // Add Magenta
    } else if (paletteIndex == 2) { // Dreamy Pastel
        color = mix(vec3(0.8, 0.9, 1.0), vec3(0.5, 0.7, 1.0), v); // Light blue to medium blue
        color = mix(color, vec3(0.9, 0.7, 0.9), sin(v * PI * 0.5)); // Soft pink/purple
    } else { // Classic Retro (default/index 3)
        color = mix(vec3(0.0, 0.0, 0.0), vec3(1.0, 0.0, 0.0), v); // Black to Red
        color = mix(color, vec3(1.0, 1.0, 0.0), cos(v * PI)); // Add Yellow
    }
    return clamp(color, 0.0, 1.0);
}


// --- Main Distance Field (Marching Primitive) ---
vec2 mp( vec3 p, float ga ){
    op=p;

    // Adjusted for dynamic Z based on overall effect and MidSphereSize
    p.y = abs(p.y) - 2. - cos(p.z * .1) * 10. * MidSphereSize; 
    
    // Replaced iChannel1 audio with ISF tunable parameters
    float ffBass = BassIntensity * 1.5;
    float ffHigh = HighIntensity * 2.0;
    float ffWhole = OverallFreq * 8.50; // Use overall freq uniform

    float spr = 9. + ffBass * 3.;
    vec2 h,t = vec2(.7 * cz(p,vec3(1,.2,spr*2.)),6);  
    
    pp=p; pp.z=abs(pp.z)-spr; // Outter bits
    t.x=smin(t.x,.8*cz(pp,vec3(1.5,.2,1)),0.5);
    t.x=smin(t.x,.8*cz(pp,vec3(.7,.1,1.)),2.);  
    
    pn=proceduralNoise(p.xz*.1); // Use procedural noise here

    float organic=length(p)-1.5-pn-ffWhole*.25; // Middle sphere  
    vec3 rp=p;rp.z=abs(abs(rp.z)-1.)-.4;
    organic=smax(rp.z,.5*organic,.5);
    t.x=smin(t.x,organic,.4);                        
    
    bp=p;bp=abs(bp)-2.-cos(p.z*.5)*2.;
    t.x=smin(t.x,max(0.5*(length(bp.xy)-.1-ffWhole*.1+pn*MiddleSplineBrightness),abs(p.z)-14.),1.); //Middle splines
    bp=p;bp=abs(bp)-4.;
    float glo=max(length(bp.xy),abs(p.z)-12.);
    bp.xy*=r2(cos(bp.z*.1)-tt);  // Use bp.z here, not p.z as in original
    glo=min(glo,max(max(length(cos(bp-vec3(0,0,tt*2.))),length(p.xy+vec2(0,9))-5.),abs(p.z)-18.)); //PARTICLES
    gg+=0.1/(0.2*glo*glo*(400.-390.*sin(op.x*.5+ffWhole)))*ga*ParticleGlow;    // Apply ParticleGlow
    t.x=min(t.x,glo);  
    
    vec3 outCylP=pp;   // OUTTER BITS
    outCylP.z=abs(outCylP.z)-.25;
    outCylP.xy*=r2(tt);  
    
    float cogDis=clamp(sin(abs(atan(outCylP.x,outCylP.y))*15.),-.5,.5);  
    h=vec2(cz(outCylP,vec3(2.*CogScale,.5-.1*cogDis,.3)),3); //Cogs // Apply CogScale
    pp=p;pp.xz=abs(pp.xz)-vec2(3.8,3);
    h.x=min(h.x,cx(pp,vec3(10,1,1.5)));
    rp=p;rp.z=abs(rp.z)-1.;
    float sz=min(4.,1.+ffHigh*1.5);
    float inner=cz(rp,vec3(sz,.4+cogDis*.1,.5)); //INNER BITS
    inner=max(inner,-(abs(rp.z)-.1));
    glo=cz(rp,vec3(sz,.1,.05));
    glo=min(glo,length(abs(rp.xz)-vec2(ffHigh,0)));
    h.x=min(h.x,glo);
    g+=0.1/(0.1*glo*glo*200.)*ga*InnerCogsBrightness; // Apply InnerCogsBrightness
    h.x=.7*min(h.x,inner); //Inner COGS    
    t=t.x<h.x?t:h;    
    
    cageP=p; cageP.xy*=r2(ffHigh+tt*CageDistortion); // Apply CageDistortion  
    cageP=abs(cageP)-vec3(1,1,6); //CAGE 
    cageP.xy*=r2(-.6);    
    cageSP=vec2(abs(atan(cageP.x,cageP.y)),cageP.z);//sphercial cage pos
    b=clamp(sin(cageSP.x*20.),-.5,.5);
    bb=clamp(sin(cageP.y*3.1),-.25,.25);
    h=vec2(bo(cageP,vec3(2,2.-b*.1,1.+b*.1)),5);  
    glo=h.x+.5;  
    glo=min(glo,cx2(pp,vec3(10,.02-clamp(sin(p.y*5.),-.25,.25)*.015,1.7)));//yellow ring
    g+=0.1/(0.1*glo*glo*(200.-190.*sin(p.y*.2-1.5+ffBass*4.)))*ga*YellowRingGlow; // Apply YellowRingGlow
    h.x=abs(h.x)-.1;    
    h.x=max(h.x,-(abs(abs(cageP.z)-.3)-max(.01,.1+b*.06+bb*.2)));
    h.x=.6*min(h.x,glo);    
    t=t.x<h.x?t:h;    
    cageP=op;
    return t;
}

// --- Raymarching ---
vec2 tr( vec3 ro, vec3 rd )
{
    vec2 h,t=vec2(.1);
    for(int i=0;i<128;i++){
        h=mp(ro+rd*t.x,1.); if(h.x<.0001||t.x>60.) break;
        t.x+=h.x;t.y=h.y;
    } if(t.x>60.) t.y=0.;
    return t;
}

#define a(d) clamp(mp(po+no*d,0.).x/d,0.,1.)  
#define s(d) smoothstep(0.,1.,mp(po+ld*d,0.).x/d)

// --- Main Shader Logic ---
void main() {
    // Apply Zoom directly to UVs to simulate camera zoom
    vec2 uv = (gl_FragCoord.xy/RENDERSIZE.xy-0.5)/vec2(RENDERSIZE.y/RENDERSIZE.x,1) * Zoom; 
    
    // Global time and camera animation based on CameraSpeed
    tt = 0.5 * mod(TIME * CameraSpeed, 125.6637);
    // Removed 's' as camera path index is gone.
    
    float ss = sin(tt/2.10743);
    float cc = cos(tt/2.10743);
    
    // Single, continuous camera path
    ro = vec3(cos(tt) * 15.0 * CameraOrbitAmp, sin(tt * 0.5) * 5.0 * CameraOrbitAmp, sin(tt) * 15.0 * CameraOrbitAmp);
    
    // Camera Target Y (now solely from tunable offset)
    float camTargetY_val = CameraTargetOffset; 
    vec3 cw=normalize(vec3(0,camTargetY_val,0)-ro);
    vec3 cu=normalize(cross(cw,vec3(0,1,0)));
    vec3 cv=normalize(cross(cu,cw));
    vec3 rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)); // UVs already scaled by Zoom
    
    vec3 co,fo;

    // ffMain derived from OverallFreq
    float ffMain = OverallFreq * 2.50; // Simplified as OverallFreq is used

    // Base color from palette
    // 'length(uv)' provides a radial gradient for palette mapping
    fo = getPaletteColor(length(uv) * 0.5 + ffMain * 0.1, int(ColorPaletteIndex));
    
    lp=ro+vec3(0,2,0);
    z=tr(ro,rd);t=z.x;
    
    if(z.y>0.){
        po=ro+rd*t;
        no=normalize(e.xyy*mp(po+e.xyy,0.).x+e.yyx*mp(po+e.yyx,0.).x+e.yxy*mp(po+e.yxy,0.).x+e.xxx*mp(po+e.xxx,0.).x);
        
        // Albedo based on palette, 's' is now obsolete, replacing with constant value
        // The original 'al' color was hardcoded; now it's mixed with the palette
        vec3 baseAlbedo = getPaletteColor(fract(po.z * 0.1), int(ColorPaletteIndex)); // Example using Z-coord for variation
        al=baseAlbedo * (1.+.5*ceil(abs(sin(cageP.z*15.)-b)-.2));
        
        if(z.y<5.)al=vec3(0);
        // Original line was: if(z.y>5.)al=vec3(.7,.8,.9)*(1.+tn*2.)-tn*OverallFreq*.75;
        // Now using palette color for 'al' (object color)
        if(z.y>5.)al=getPaletteColor(pn * 0.5 + OverallFreq * 0.1, int(ColorPaletteIndex)) * (1.+pn*2.) - pn*OverallFreq*.75; 
        
        ld=normalize(lp-po);
        float attn=1.0-pow(min(1.0,length(lp-po)/15.),4.0),
        dif=max(0.,dot(no,ld)),
        fr=pow(1.+dot(no,rd),4.),
        sp=pow(max(dot(reflect(-ld,no),-rd),0.),30.);
        co=mix((sp*.5+al*(a(.1)*a(.3)+.2)*(dif+s(1.)*.3)),fo,min(fr,1.));
        co=mix(fo,co,exp(-.00002*t*t*t));
    }
    
    // Apply glows from distance functions
    // Adjust glow colors to be more neutral or derived from palette if desired
    co = co + g*.2*vec3(.7,.1,.0)+gg*.2;
    co = mix(co,co.zyx,length(uv)*.7);
    gl_FragColor = vec4(pow(co,vec3(.45)),1);
}