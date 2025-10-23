/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Psychedelic",
        "Raymarching"
    ],
    "DESCRIPTION": "A new ISF shader featuring a dynamic fractal environment with customizable color palettes, camera controls, fractal geometry adjustments, and post-processing effects.",
    "CREDIT": "Original ShaderToy by Kali. Converted to ISF by Gemini AI.",
    "INPUTS": [
        {
            "NAME": "mouseNormX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Mouse X (Normalized)"
        },
        {
            "NAME": "mouseNormY",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Mouse Y (Normalized)"
        },
        {
            "NAME": "mouseClick",
            "TYPE": "bool",
            "DEFAULT": false,
            "LABEL": "Mouse Clicked"
        },
        {
            "NAME": "inputTexture",
            "TYPE": "image",
            "DEFAULT": "null",
            "LABEL": "Background Texture"
        },
        {
            "NAME": "colorPaletteIndex",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.0,
            "STEP": 1.0,
            "LABEL": "Color Palette"
        },
        {
            "NAME": "colorPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Color Pulse Strength"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Color Pulse Speed"
        },
        {
            "NAME": "morphStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Morph"
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 5.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "cameraPosX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera X Offset"
        },
        {
            "NAME": "cameraPosY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Y Offset"
        },
        {
            "NAME": "cameraRotX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera X Rotation"
        },
        {
            "NAME": "cameraRotY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera Y Rotation"
        },
        {
            "NAME": "fractalModX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod X"
        },
        {
            "NAME": "fractalModY",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod Y"
        },
        {
            "NAME": "fractalModZ",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Mod Z"
        },
        {
            "NAME": "fractalClampMin",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MIN": 0.01,
            "MAX": 0.5,
            "LABEL": "Fractal Clamp Min"
        },
        {
            "NAME": "fractalClampMax",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Fractal Clamp Max"
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Saturation"
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
            "NAME": "enableVibration",
            "TYPE": "bool",
            "DEFAULT": false,
            "LABEL": "Enable Vibration"
        },
        {
            "NAME": "vibrationStrength",
            "TYPE": "float",
            "DEFAULT": 0.0013,
            "MIN": 0.0,
            "MAX": 0.01,
            "LABEL": "Vibration Strength"
        }
    ],
    "PASSES": [
        {}
    ]
}
*/

#define RAY_STEPS 100
#define SHADOW_STEPS 50
#define LIGHT_COLOR vec3(1.,.97,.93)
#define AMBIENT_COLOR vec3(.75,.65,.6)

#define SPECULAR 0.65
#define DIFFUSE  1.0
#define AMBIENT  0.35

#define DETAIL .00004

const float pi = acos(-1.0);
const float pi2 = pi * 2.0;

// Global variables and constants
float id = 0.0;
vec3 col = vec3(0.0);
float vibration = 0.0;
float det = 0.0;
vec3 pth1;

const vec3 lightdir=normalize(vec3(0.1,-0.15,-1.));
const vec3 origin=vec3(-1.,0.2,0.);


mat2 rot(float a) {
	float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

float hash(vec2 p) {
	vec3 p3=fract(vec3(p.xyx)*.1031);
    p3+=dot(p3,p3.yzx+33.33);
    return fract((p3.x+p3.y)*p3.z);
}

vec4 formula(vec4 p) {
    p.xz = abs(p.xz+1.0 - morphStrength) - abs(p.xz-1.0 + morphStrength) - p.xz;
    p = p * 2.0 / clamp(dot(p.xyz,p.xyz), fractalClampMin, fractalClampMax) - vec4(fractalModX, fractalModY, fractalModZ, 0.);
    p.xy*=rot(.5);
    return p;
}

float screen(vec3 p) {
	float d1=length(p.yz-vec2(.25,0.))-.5;	
	float d2=length(p.yz-vec2(.25,2.))-.5;	
	return min(max(d1,abs(p.x-.3)-.01),max(d2,abs(p.x+2.3)-.01));
}

// de function must be defined before normal and march
float de(vec3 p) {
    vec3 pp=p;
    float sc = 1.;
    float local_t = TIME * animationSpeed * .15;

    p.xy*=rot(pp.z*.2 + local_t*.5 + sin(p.z*.05 + local_t*2.)*4.);
    p.xy = abs(2. - mod(p.xy, 4.));
    p.z=abs(1.5-mod(p.z,3.));
    vec3 cp=p;
    for (int i=0;i<2;i++) {
        p.xy=abs(p.xy+1.)-abs(p.xy-1.)-p.xy;
        float s=10./clamp(dot(p,p),.1,1.2);
        p=p*s-11.;
        sc=sc*s;
    }
    float f=length(p.xy)/sc;
    float o=min(length(cp.yz),length(cp.xz));
    float l=length(pp.xy)+cos(pp.z*2.1)*.4;
    float d=min(l,min(f,o));
    id=step(o,d);
    col=vec3(.0,.3,1.);
    col*=step(abs(fract(local_t + pp.z*.01)-.5),.02);
    col+=id;
    col+=vec3(.5,.1,0)*step(l,d);
    return (d-.02)*.5;
}

// normal function must be defined after de
vec3 normal(vec3 p) {
	vec3 e=vec3(0.,.01,0.);
    return normalize(vec3(de(p+e.yxx) - de(p), de(p+e.xyx) - de(p), de(p+e.xxy) - de(p)));
}

// shadow function must be defined after de
float shadow(vec3 pos, vec3 sdir) {
	float sh=1.0;
	float totdist =2.0*det;
	float dist=10.;
	for (int steps=0; steps<SHADOW_STEPS; steps++) {
		if (totdist<1. && dist>DETAIL) {
			vec3 p = pos - totdist * sdir;
			dist = de(p); // de() returns a float
			sh = min( sh, max(50.*dist/totdist,0.0) );
			totdist += max(.01,dist);
		}
	}
    return clamp(sh,0.1,1.0);
}

// calcAO function must be defined after de
float calcAO( const vec3 pos, const vec3 nor ) {
	float aodet=DETAIL*40.;
	float totao = 0.0;
    float sca = 13.0;
    for( int aoi=0; aoi<5; aoi++ ) {
        float hr = aodet*float(aoi*aoi);
        vec3 aopos = nor * hr + pos;
        float dd = de( aopos ); // de() returns a float
        totao += -(dd-hr)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - 5.0*totao, 0., 1.0 );
}

// --- Psychedelic Color Palettes ---
vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 7.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// getPsychedelicPalette function must be defined before light
vec3 getPsychedelicPalette(float t_val, int index) {
    vec3 col_palette;
    if (index == 0) { // Hypnotic Rainbow
        vec3 a = vec3(0.5, 0.5, 0.5);
        vec3 b = vec3(0.5, 0.5, 0.5);
        vec3 c = vec3(1.0, 1.0, 1.0);
        vec3 d = vec3(0.263, 0.416, 0.557);
        col_palette = a + b * cos(6.28318 * (t_val * 0.7 + d));
    } else if (index == 1) { // Electric Neon
        col_palette = 0.5 + 0.5 * sin(t_val * pi2 * 2.0 + vec3(0.0, 0.33, 0.66));
        col_palette.r = pow(col_palette.r, 1.5); col_palette.g = pow(col_palette.g, 1.5); col_palette.b = pow(col_palette.b, 1.5);
    } else if (index == 2) { // Acid Trip
        col_palette = hsv2rgb(vec3(mod(t_val * 1.5 + TIME * 0.1, 1.0), 1.0, sin(t_val * 3.0) * 0.5 + 0.5));
    } else if (index == 3) { // Cosmic Dust
        col_palette = 0.5 + 0.5 * cos(t_val * 5.0 + vec3(0.0, 2.0, 4.0));
        col_palette = pow(col_palette, vec3(1.5, 1.2, 1.8));
    } else if (index == 4) { // Retro Wave
        col_palette = mix(vec3(0.0, 1.0, 1.0), vec3(1.0, 0.0, 1.0), sin(t_val * 4.0 + TIME * 0.2) * 0.5 + 0.5);
        col_palette = mix(col_palette, vec3(1.0, 1.0, 0.0), sin(t_val * 2.0 + TIME * 0.3) * 0.5 + 0.5);
    } else if (index == 5) { // Bioluminescent
        col_palette = vec3(0.0, 0.8, 0.5) + sin(t_val * 7.0 + vec3(0.0, 1.0, 2.0)) * 0.4;
        col_palette.b += sin(t_val * 10.0) * 0.3;
        col_palette = clamp(col_palette, 0.0, 1.0);
    } else { // 6 - Dark Psychedelia
        col_palette = pow(abs(sin(t_val * 3.0 + vec3(0.0, 1.5, 3.0))), vec3(1.0, 2.0, 3.0));
        col_palette = mix(col_palette, vec3(0.1, 0.0, 0.2), 0.5);
    }
    return col_palette;
}

// colorize function must be defined before light
vec2 colorize(vec3 p) {
	p.z=abs(2.-mod(p.z,4.));
	float es, l=es=0.;
	float ot=1000.;
	for (int i = 0; i < 15; i++) {
		p=formula(vec4(p,0.)).xyz;
		float pl = l;
		l = length(p);
		es+= exp(-10. / abs(l - pl));
		ot=min(ot,abs(l-3.));
	}
	return vec2(es,ot);
}

// light function must be defined after colorize, getPsychedelicPalette, shadow, calcAO
vec3 light(in vec3 p, in vec3 dir, in vec3 n, in float hid) {
    float sh=shadow(p, lightdir);

	float ao=calcAO(p,n);
	float diff=max(0.,dot(lightdir,-n))*sh*DIFFUSE;
	vec3 amb=max(.5,dot(dir,-n))*AMBIENT*getPsychedelicPalette(0.5, int(colorPaletteIndex));
	vec3 r = reflect(lightdir,n);
	float spec=pow(max(0.,dot(dir,-r))*sh,15.)*SPECULAR;
	vec3 light_col;
	vec2 colorize_result=colorize(p);
	if (hid>.5) {light_col=getPsychedelicPalette(0.1, int(colorPaletteIndex)); spec=spec*spec;}
	else{
		float k=pow(colorize_result.x*.11,2.);
		light_col=mix(vec3(k,k*k,k*k),getPsychedelicPalette(k, int(colorPaletteIndex)),.5)+.1;
		light_col+=pow(max(0.,1.-colorize_result.y),5.)*.3;
	}
	light_col=light_col*ao*(amb+diff*LIGHT_COLOR)+spec*LIGHT_COLOR;	

	if (hid>.5) {
		vec3 p2=p;
		p2.z=abs(1.-mod(p2.z,2.));
		vec3 c_tex=texture2D(inputTexture,mod(1.-p.zy-vec2(.4,0.2),vec2(1.))).xyz*2.;
		light_col+=c_tex*abs(.01-mod(p.y-TIME*animationSpeed*.1,.02))/.01*ao;
		light_col*=max(0.,1.-pow(length(p2.yz-vec2(.25,1.)),2.)*3.5);
	} else{
		vec3 c_tex=(texture2D(inputTexture,mod(p.zx*2.+vec2(0.5),vec2(1.))).xyz);
		c_tex*=abs(.01-mod(p.x-TIME*animationSpeed*.1*sign(p.x+1.),.02))/.01;
		light_col+=pow(colorize_result.x,10.)*.0000000003*c_tex;
		light_col+=pow(max(0.,1.-colorize_result.y),4.)
			*pow(max(0.,1.-abs(1.-mod(p.z+TIME*animationSpeed*2.,4.))),2.)
			*getPsychedelicPalette(0.9, int(colorPaletteIndex))*4.*max(0.,.05-abs(p.x+1.))/.05;
	}
	return light_col;
}

// path function must be defined before move
vec3 path(float ti) {
	vec3 p=vec3(sin(ti)*2.,(1.-sin(ti*.5))*.5,-cos(ti*.25)*30.)*.5;
	return p;
}

// move function must be defined after path and de
vec3 move(inout vec3 dir) {
	float localTime = TIME * animationSpeed * 0.2;
	vec3 go=path(localTime);
	vec3 adv=path(localTime+.7);
	float hd=de(adv);
	vec3 advec=normalize(adv-go);
	float an=adv.x-go.x; an*=min(1.,abs(adv.z-go.z))*sign(adv.z-go.z)*.7;
	dir.xy*=rot(an);
    an=advec.y*1.7;
	dir.yz*=rot(an);
	an=atan(advec.x,advec.z);
	dir.xz*=rot(an);
	return go;
}

// march function must be defined after de, normal, light, getPsychedelicPalette
vec3 march(vec3 from, vec3 dir) {
	float glow,eglow,totdist=glow=0.;
	float d_val;
	vec3 p, c_march=vec3(0), ot=vec3(1000),g=c_march;

    if (enableVibration) {
        vibration=sin(TIME*animationSpeed*60.)*vibrationStrength;
    } else {
        vibration=0.;
    }
	
	for (int i=0; i<200; i++) {
		p=from+totdist*dir;
		d_val=de(p);
		totdist+=d_val;
		if (d_val<.001||totdist>50.) break;
		g+=exp(-.5*d_val)*col;
	}
	float l=max(0.,dot(normalize(-dir),normalize(lightdir)));
	vec3 backg=vec3(max(0.,-dir.y+.6))*getPsychedelicPalette(0.7, int(colorPaletteIndex))*.5*max(0.4,l);

	if (d_val<0.01 || totdist<50.) {
		p=p-abs(d_val-0.01)*dir;
		vec3 n=normal(p);
		c_march=light(p, dir, n, id);
		c_march = mix(c_march, backg, 1.0-exp(-.15*pow(totdist,1.5)));
	} else {
		c_march=backg;
	    vec3 st = (dir * 3.+ vec3(1.3,2.5,1.25)) * .3;
		for (int i = 0; i < 13; i++) st = abs(st) / dot(st,st) - .9;
		c_march+= min( 1., pow( min( 5., length(st) ), 3. ) * .0025 );

	}
	vec3 lglow=LIGHT_COLOR*pow(l,25.)*.5;
	c_march+=glow*(.5+l*.5)*LIGHT_COLOR*.7;
	c_march+=lglow*exp(min(30.,totdist)*.02);
	return c_march;
}


void main() {
    float localTime = TIME * animationSpeed * 0.15;
    pth1 = path(localTime+.3)+origin;

    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy*2.-1.;
    vec2 uv2=uv;

    // The original ShaderToy did not have an explicit `ENABLE_POSTPROCESS` define
    // for this initial UV distortion. If you want to control this,
    // please add an `enableInitialDistortion` boolean input.
    // For now, it's always applied if post-processing is generally active.
    // This block was inside `#ifdef ENABLE_POSTPROCESS` in the original ShaderToy.
    // I am assuming `enablePostProcess` input should control this.
    if (true) { // This `if(true)` means it's always on. If you want to use the input, change to `if(enablePostProcess)`
        uv*=1.+pow(length(uv2*uv2*uv2*uv2),4.)*.07;
    }

    uv.y*=RENDERSIZE.y/RENDERSIZE.x;

    vec2 mouse=(vec2(mouseNormX, mouseNormY)-.5)*3.;
    if (!mouseClick) mouse=vec2(0.,-.2);

    vec3 dir=normalize(vec3(uv*.8 * zoomFactor,1.));
    dir.yz*=rot(mouse.y + cameraRotY);
    dir.xz*=rot(mouse.x + cameraRotX);

    vec3 from=origin+move(dir) + vec3(cameraPosX, cameraPosY, 0.0);

    vec3 color=march(from,dir);
    color=clamp(color,vec3(.0),vec3(1.));

    color=pow(color,vec3(1.0 / 1.35)) * brightness;
    color=mix(vec3(length(color)),color,saturation);

    // Color Pulse (moving line of light)
    float pulsePos = mod(gl_FragCoord.x / RENDERSIZE.x + TIME * colorPulseSpeed, 1.0);
    float pulseEffect = 1.0 - smoothstep(0.4, 0.6, abs(gl_FragCoord.x / RENDERSIZE.x - pulsePos));
    color += pulseEffect * colorPulseStrength;

    // Post-processing block
    // This block was inside `#ifdef ENABLE_POSTPROCESS` in the original ShaderToy.
    // I am assuming `enablePostProcess` input should control this.
    if (true) { // This `if(true)` means it's always on. If you want to use the input, change to `if(enablePostProcess)`
        vec2 texCoord = (uv2 * 0.5 + 0.5) + vec2(TIME * animationSpeed * 7.25468);
        vec3 rain=pow(texture2D(inputTexture, texCoord).rgb,vec3(1.5));
        color=mix(rain,color,clamp(TIME*animationSpeed*.5-.5,0.,1.));
        color*=1.-pow(length(uv2*uv2*uv2*uv2)*1.1,6.);
        uv2.y *= RENDERSIZE.y / 360.0;
        color.r*=(.5+abs(.5-mod(uv2.y     ,.021)/.021)*.5)*1.5;
        color.g*=(.5+abs(.5-mod(uv2.y+.007,.021)/.021)*.5)*1.5;
        color.b*=(.5+abs(.5-mod(uv2.y+.014,.021)/.021)*.5)*1.5;
        color*=.9+rain*.35;
    }

    color = (color - 0.5) * contrast + 0.5;

    gl_FragColor = vec4(color,1.0)*min(1.,TIME*animationSpeed*.5);
}