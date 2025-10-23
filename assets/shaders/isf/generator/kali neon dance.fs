/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Raymarching",
        "Heightmap",
        "Fractal",
        "Animated",
        "Psychedelic"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/7sX3WN by Kali. Raymarching a heightmap with fixed steps and binary search. 2D Mandelbox fractal variation used for animated coloring. Now with ISF controls for zoom, speed, color pulse, geometry scaling, fractal detail, morphing, combined psychedelic motion, and selectable color palettes.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "COMMENT": "Controls the overall camera zoom/distance from the scene."
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0,
            "COMMENT": "Adjusts the overall animation speed."
        },
        {
            "NAME": "color_pulse",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Adds a pulsating effect to the colors."
        },
        {
            "NAME": "geometry_scale",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "COMMENT": "Adjusts the scale of the raymarched geometry features."
        },
        {
            "NAME": "fractal_detail",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "COMMENT": "Controls the detail/density of the fractal coloring."
        },
        {
            "NAME": "morph_factor",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Applies a morphing/warping effect to the heightmap geometry."
        },
        {
            "NAME": "psychedelic_motion",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Controls combined geometry warp and complex camera movement for psychedelic effects."
        },
        {
            "NAME": "palette_selection",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.0,
            "LABELS": ["Original", "Rainbow", "Cyberpunk"],
            "COMMENT": "Selects a color palette for the scene."
        },
        {
            "NAME": "palette_mix_factor",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "COMMENT": "Mixes between the original fractal color and the selected procedural palette."
        }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

float st=.025, maxdist=15.;
vec3 ldir=vec3(0.,-1.,-1.);
vec3 g_col=vec3(0.); 

mat2 rot(float a) {
	float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

// Procedural color palette functions (full version, no recursion)
vec3 getProceduralPalette(float hue, float type) {
    vec3 c = vec3(0.0);
    hue = fract(hue); // Ensure hue is between 0 and 1

    if (type < 0.5) { // Palette 0: Original smooth transition (similar to previous)
        if (hue < 0.1666) { c = mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 1.0, 0.0), hue / 0.1666); }
        else if (hue < 0.3333) { c = mix(vec3(1.0, 1.0, 0.0), vec3(0.0, 1.0, 0.0), (hue - 0.1666) / 0.1666); }
        else if (hue < 0.5) { c = mix(vec3(0.0, 1.0, 0.0), vec3(0.0, 1.0, 1.0), (hue - 0.3333) / 0.1666); }
        else if (hue < 0.6666) { c = mix(vec3(0.0, 1.0, 1.0), vec3(0.0, 0.0, 1.0), (hue - 0.5) / 0.1666); }
        else if (hue < 0.8333) { c = mix(vec3(0.0, 0.0, 1.0), vec3(1.0, 0.0, 1.0), (hue - 0.6666) / 0.1666); }
        else { c = mix(vec3(1.0, 0.0, 1.0), vec3(1.0, 0.0, 0.0), (hue - 0.8333) / 0.1666); }
    } else if (type < 1.5) { // Palette 1: Intense Rainbow (no recursion, direct colors)
        if (hue < 0.14) { c = vec3(1.0, 0.0, 0.0); } // Red
        else if (hue < 0.28) { c = vec3(1.0, 0.5, 0.0); } // Orange
        else if (hue < 0.42) { c = vec3(1.0, 1.0, 0.0); } // Yellow
        else if (hue < 0.56) { c = vec3(0.0, 1.0, 0.0); } // Green
        else if (hue < 0.70) { c = vec3(0.0, 0.5, 1.0); } // Blue
        else if (hue < 0.84) { c = vec3(0.5, 0.0, 1.0); } // Indigo
        else { c = vec3(1.0, 0.0, 1.0); } // Violet
        // Removed: c = mix(c, getProceduralPalette(hue, 0.0), 0.2);
    } else { // Palette 2: Cyberpunk
        if (hue < 0.25) { c = mix(vec3(0.0, 1.0, 1.0), vec3(0.0, 0.5, 1.0), hue / 0.25); } 
        else if (hue < 0.5) { c = mix(vec3(0.0, 0.5, 1.0), vec3(0.5, 0.0, 1.0), (hue - 0.25) / 0.25); } 
        else if (hue < 0.75) { c = mix(vec3(0.5, 0.0, 1.0), vec3(1.0, 0.0, 0.5), (hue - 0.5) / 0.25); } 
        else { c = mix(vec3(1.0, 0.0, 0.5), vec3(1.0, 0.5, 0.0), (hue - 0.75) / 0.25); } 
    }
    return c;
}


vec3 fractal(vec2 p) {
    vec2 pos=p;
    float d, ml=100.;
    vec2 mc=vec2(100.);
    // Apply fractal_detail here
    p=abs(fract(p*.1 * fractal_detail)-.5);
    vec2 c=p;
    for(int i=0;i<8;i++) {
        d=dot(p,p);
        p=abs(p+1.)-abs(p-1.)-p;
    	p=p*-1.5/clamp(d,.5,1.)-c;
        mc=min(mc,abs(p));
        if (i>2) ml=min(ml*(1.+float(i)*.1),abs(p.y-.5));
    }
    mc=max(vec2(0.),1.-mc);
    mc=normalize(mc)*.8;
    ml=pow(max(0.,1.-ml),6.);
    return vec3(mc,d*.4)*ml*(step(.7,fract(d*.1+TIME*.5+pos.x*.2)))-ml*.1;
}

float map(vec2 p) {
    vec2 pos=p;
    float current_time = TIME * speed; // Use current_time based on speed
    
    // Apply geometry_scale here
    p *= geometry_scale;

    // Apply morph_factor here
    p.x += sin(p.y * 0.5 + current_time) * morph_factor * 2.0;
    p.y += cos(p.x * 0.5 + current_time) * morph_factor * 2.0;

    // Apply additional warp from psychedelic_motion
    p.x += sin(p.y * 0.8 + current_time * 1.5) * psychedelic_motion * 1.5;
    p.y += cos(p.x * 0.8 + current_time * 1.5) * psychedelic_motion * 1.5;


    float t_map=current_time; // Use current_time for map animations
    g_col+=fractal(p); 
    vec2 p2=abs(.5-fract(p*8.+4.));
	float h=0.;
    h+=sin(length(p)+t_map);
    p=floor(p*2.+1.);
    float l=length(p2*p2);
    h+=(cos(p.x+t_map)+sin(p.y+t_map))*.5;
    h+=max(0.,5.-length(p-vec2(18.,0.)))*1.5;
    h+=max(0.,5.-length(p+vec2(18.,0.)))*1.5;
    p=p*2.+.2345;
    t_map*=.5;
    h+=(cos(p.x+t_map)+sin(p.y+t_map))*.3;
    return h;
}

vec3 normal(vec2 p, float td) {
	vec2 eps=vec2(0.,.001);
    return normalize(vec3(map(p+eps.yx)-map(p-eps.yx),2.*eps.y,map(p+eps.xy)-map(p-eps.xy)));
}

vec2 hit(vec3 p) {
    float h=map(p.xz);
    return vec2(step(p.y,h),h);
}

vec3 bsearch(vec3 from,vec3 dir,float td) {
    vec3 p;
    st*=-.5;
    td+=st;
    float h2=1.;
    for (int i=0;i<20;i++) {
        p=from+td*dir;
        float h=hit(p).x;
        if (abs(h-h2)>.001) {
            st*=-.5;
        h2=h;
        }
        td+=st;
    }
	return p;
}

vec3 shade(vec3 p,vec3 dir,float h,float td) { 
    ldir=normalize(ldir);
	g_col=vec3(0.); 
    vec3 n=normal(p.xz,td);
	g_col*=.25; 
    float dif=max(0.,dot(ldir,-n));
    vec3 ref=reflect(ldir,dir);
    float spe=pow(max(0.,dot(ref,-n)),8.);
    return g_col+(dif*.5+.2+spe*vec3(1.,.8,.5))*.2; 
}


vec3 march(vec3 from,vec3 dir) {
	vec3 p, col=vec3(0.); // Changed back to 'col'
    float td=.5, k=0.;
    vec2 h; // Changed back to 'h'
    for (int i=0;i<600;i++) {
    	p=from+td*dir;
        h=hit(p);
        if (h.x>.5||td>maxdist) break;
        td+=st;
    }
    if (h.x>.5) {
        p=bsearch(from,dir,td);
    	col=shade(p,dir,h.y,td); // Changed back to 'h.y'
    } else {
    }
	col=mix(col,2.*vec3(mod(gl_FragCoord.y,4.)*.1),pow(td/maxdist,3.));
    return col*vec3(.9,.8,1.);
}

mat3 lookat(vec3 dir,vec3 up) {
	dir=normalize(dir);vec3 rt=normalize(cross(dir,normalize(up)));
    return mat3(rt,cross(rt,dir),dir);
}

vec3 path(float t) {
	return vec3(cos(t)*5.5,1.5-cos(t)*.0,sin(t*2.))*2.5;
}

void main() {
    vec2 uv = (gl_FragCoord.xy-RENDERSIZE.xy*.5)/RENDERSIZE.y;
	float current_time=TIME * speed; 
    float t_main=current_time*.2;

    vec3 from=path(t_main) * zoom; 
    vec3 dir=normalize(vec3(uv,.7));
    vec3 adv=path(t_main+.1)-from;

    // Apply 3D camera movement from psychedelic_motion
    dir=lookat(adv+vec3(sin(t_main*3.0)*psychedelic_motion*0.5, -.2-(1.+sin(t_main*2.)), cos(t_main*3.5)*psychedelic_motion*0.5),vec3(adv.x*.1,1.,0.))*dir;
    
    vec3 col=march(from, dir)*1.5; // Changed back to 'col'

    // Apply color pulse here
    col += sin(current_time * 5.0) * color_pulse * 0.2;

    // Apply procedural color palette using palette_selection and palette_mix_factor
    vec3 procedural_color = getProceduralPalette(fract(current_time * 0.05 + length(uv) * 0.1), palette_selection);
    col = mix(col, procedural_color, palette_mix_factor); // Changed back to 'col'

    gl_FragColor = vec4(col,1.);
}