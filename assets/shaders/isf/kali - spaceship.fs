/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Psychedelic",
        "Raymarching"
    ],
    "DESCRIPTION": "An ISF shader converted from 'Britney's Spaceship' by Kali. Features a complex fractal with improved shading, lighting effects, and a camera path following a liquid metal ball. Includes extensive tunable parameters for color, geometry, camera, and post-processing.",
    "CREDIT": "Original ShaderToy 'Britney's Spaceship' by Kali. Converted to ISF by Gemini AI.",

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
            "NAME": "enableHardShadows",
            "TYPE": "bool",
            "DEFAULT": true,
            "LABEL": "Enable Hard Shadows"
        },
        {
            "NAME": "enablePostProcess",
            "TYPE": "bool",
            "DEFAULT": true,
            "LABEL": "Enable Post Process"
        },
        {
            "NAME": "enableVibration",
            "TYPE": "bool",
            "DEFAULT": false,
            "LABEL": "Enable Vibration"
        },
        {
            "NAME": "lessDetail",
            "TYPE": "bool",
            "DEFAULT": false,
            "LABEL": "Less Detail (Faster)"
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

// All variables defined in JSON INPUTS are automatically available as uniforms.
// No need to redeclare them here.

vec3 lightdir=normalize(vec3(0.1,-0.15,-1.));
const vec3 origin=vec3(-1.,0.2,0.);
float det=0.0;
vec3 pth1;

// Declare vibration as a global mutable variable
float vibration = 0.0;


mat2 rot(float a) {
	return mat2(cos(a),sin(a),-sin(a),cos(a));	
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

vec2 de(vec3 pos) {
	float hid=0.;
	vec3 tpos=pos;
	tpos.z=abs(2.-mod(tpos.z,4.));
	vec4 p=vec4(tpos,1.5);
	float y=max(0.,.35-abs(pos.y-3.35))/.35;

    int iterations = lessDetail ? 6 : 8;
	for (int i=0; i<iterations; i++) {p=formula(p);}

    float fr;
    if (lessDetail) {
        fr=max(-tpos.x-4.,(length(max(vec2(0.),p.yz-2.))-.5)/p.w);
    } else {
        fr=max(-tpos.x-4.,(length(max(vec2(0.),p.yz-3.)))/p.w);
    }

	float sc=screen(tpos);
	float d=min(sc,fr);
	if (abs(d-sc)<.001) hid=1.;
	return vec2(d,hid);
}

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

vec3 path(float ti) {
	vec3 p=vec3(sin(ti)*2.,(1.-sin(ti*.5))*.5,-cos(ti*.25)*30.)*.5;
	return p;
}

vec3 normal(vec3 p) {
	vec3 e = vec3(0.0,det,0.0);
	
	return normalize(vec3(
			de(p+e.yxx).x-de(p-e.yxx).x,
			de(p+e.xyx).x-de(p-e.xyx).x,
			de(p+e.xxy).x-de(p-e.xxy).x
			)
		);	
}

float shadow(vec3 pos, vec3 sdir) {
	float sh=1.0;
	float totdist =2.0*det;
	float dist=10.;
	for (int steps=0; steps<SHADOW_STEPS; steps++) {
		if (totdist<1. && dist>DETAIL) {
			vec3 p = pos - totdist * sdir;
			dist = de(p).x;
			sh = min( sh, max(50.*dist/totdist,0.0) );
			totdist += max(.01,dist);
		}
	}
    return clamp(sh,0.1,1.0);
}

float calcAO( const vec3 pos, const vec3 nor ) {
	float aodet=DETAIL*40.;
	float totao = 0.0;
    float sca = 13.0;
    for( int aoi=0; aoi<5; aoi++ ) {
        float hr = aodet*float(aoi*aoi);
        vec3 aopos = nor * hr + pos;
        float dd = de( aopos ).x;
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

vec3 getPsychedelicPalette(float t_val, int index) {
    vec3 col;
    if (index == 0) { // Hypnotic Rainbow
        vec3 a = vec3(0.5, 0.5, 0.5);
        vec3 b = vec3(0.5, 0.5, 0.5);
        vec3 c = vec3(1.0, 1.0, 1.0);
        vec3 d = vec3(0.263, 0.416, 0.557);
        col = a + b * cos(6.28318 * (t_val * 0.7 + d));
    } else if (index == 1) { // Electric Neon
        col = 0.5 + 0.5 * sin(t_val * pi2 * 2.0 + vec3(0.0, 0.33, 0.66));
        col.r = pow(col.r, 1.5); col.g = pow(col.g, 1.5); col.b = pow(col.b, 1.5);
    } else if (index == 2) { // Acid Trip
        col = hsv2rgb(vec3(mod(t_val * 1.5 + TIME * 0.1, 1.0), 1.0, sin(t_val * 3.0) * 0.5 + 0.5));
    } else if (index == 3) { // Cosmic Dust
        col = 0.5 + 0.5 * cos(t_val * 5.0 + vec3(0.0, 2.0, 4.0));
        col = pow(col, vec3(1.5, 1.2, 1.8));
    } else if (index == 4) { // Retro Wave
        col = mix(vec3(0.0, 1.0, 1.0), vec3(1.0, 0.0, 1.0), sin(t_val * 4.0 + TIME * 0.2) * 0.5 + 0.5);
        col = mix(col, vec3(1.0, 1.0, 0.0), sin(t_val * 2.0 + TIME * 0.3) * 0.5 + 0.5);
    } else if (index == 5) { // Bioluminescent
        col = vec3(0.0, 0.8, 0.5) + sin(t_val * 7.0 + vec3(0.0, 1.0, 2.0)) * 0.4;
        col.b += sin(t_val * 10.0) * 0.3;
        col = clamp(col, 0.0, 1.0);
    } else { // 6 - Dark Psychedelia
        col = pow(abs(sin(t_val * 3.0 + vec3(0.0, 1.5, 3.0))), vec3(1.0, 2.0, 3.0));
        col = mix(col, vec3(0.1, 0.0, 0.2), 0.5);
    }
    return col;
}


vec3 light(in vec3 p, in vec3 dir, in vec3 n, in float hid) {
    float sh;
    if (enableHardShadows) {
        sh=shadow(p, lightdir);
    } else {
        sh=calcAO(p,-2.5*lightdir);
    }
	float ao=calcAO(p,n);
	float diff=max(0.,dot(lightdir,-n))*sh*DIFFUSE;
	vec3 amb=max(.5,dot(dir,-n))*AMBIENT*getPsychedelicPalette(0.5, int(colorPaletteIndex));
	vec3 r = reflect(lightdir,n);
	float spec=pow(max(0.,dot(dir,-r))*sh,15.)*SPECULAR;
	vec3 col;
	vec2 getcol=colorize(p);
	if (hid>.5) {col=getPsychedelicPalette(0.1, int(colorPaletteIndex)); spec=spec*spec;}
	else{
		float k=pow(getcol.x*.11,2.);
		col=mix(vec3(k,k*k,k*k),getPsychedelicPalette(k, int(colorPaletteIndex)),.5)+.1;
		col+=pow(max(0.,1.-getcol.y),5.)*.3;
	}
	col=col*ao*(amb+diff*LIGHT_COLOR)+spec*LIGHT_COLOR;	

	if (hid>.5) {
		vec3 p2=p;
		p2.z=abs(1.-mod(p2.z,2.));
		vec3 c=texture2D(inputTexture,mod(1.-p.zy-vec2(.4,0.2),vec2(1.))).xyz*2.;
		col+=c*abs(.01-mod(p.y-TIME*animationSpeed*.1,.02))/.01*ao;
		col*=max(0.,1.-pow(length(p2.yz-vec2(.25,1.)),2.)*3.5);
	} else{
		vec3 c=(texture2D(inputTexture,mod(p.zx*2.+vec2(0.5),vec2(1.))).xyz);
		c*=abs(.01-mod(p.x-TIME*animationSpeed*.1*sign(p.x+1.),.02))/.01;
		col+=pow(getcol.x,10.)*.0000000003*c;
		col+=pow(max(0.,1.-getcol.y),4.)
			*pow(max(0.,1.-abs(1.-mod(p.z+TIME*animationSpeed*2.,4.))),2.)
			*getPsychedelicPalette(0.9, int(colorPaletteIndex))*4.*max(0.,.05-abs(p.x+1.))/.05;
	}
	return col;
}

vec3 raymarch(in vec3 from, in vec3 dir) {
	float glow,eglow,totdist=glow=0.;
	vec2 d=vec2(1.,0.);
	vec3 p, col=vec3(0.);
	
    if (enableVibration) {
        vibration=sin(TIME*animationSpeed*60.)*.0013;
    } else {
        vibration=0.;
    }
	
	for (int i=0; i<RAY_STEPS; i++) {
		if (d.x>det && totdist<30.0) {
			p=from+totdist*dir;
			d=de(p);
			det=DETAIL*(1.+totdist*50.);
			totdist+=d.x;
			if(d.x<0.015)glow+=max(0.,.015-d.x)*exp(-totdist);
		}
	}
	float l=max(0.,dot(normalize(-dir),normalize(lightdir)));
	vec3 backg=vec3(max(0.,-dir.y+.6))*getPsychedelicPalette(0.7, int(colorPaletteIndex))*.5*max(0.4,l);

	if (d.x<det || totdist<30.) {
		p=p-abs(d.x-det)*dir;
		vec3 norm=normal(p);
		col=light(p, dir, norm, d.y);
		col = mix(col, backg, 1.0-exp(-.15*pow(totdist,1.5)));
	} else {
		col=backg;
	    vec3 st = (dir * 3.+ vec3(1.3,2.5,1.25)) * .3;
		for (int i = 0; i < 13; i++) st = abs(st) / dot(st,st) - .9;
		col+= min( 1., pow( min( 5., length(st) ), 3. ) * .0025 );

	}
	vec3 lglow=LIGHT_COLOR*pow(l,25.)*.5;
	col+=glow*(.5+l*.5)*LIGHT_COLOR*.7;
	col+=lglow*exp(min(30.,totdist)*.02);
	return col;
}

vec3 move(inout vec3 dir) {
	float localTime = TIME * animationSpeed * 0.2;
	vec3 go=path(localTime);
	vec3 adv=path(localTime+.7);
	float hd=de(adv).x;
	vec3 advec=normalize(adv-go);
	float an=adv.x-go.x; an*=min(1.,abs(adv.z-go.z))*sign(adv.z-go.z)*.7;
	dir.xy*=mat2(cos(an),sin(an),-sin(an),cos(an));
    an=advec.y*1.7;
	dir.yz*=mat2(cos(an),sin(an),-sin(an),cos(an));
	an=atan(advec.x,advec.z);
	dir.xz*=mat2(cos(an),sin(an),-sin(an),cos(an));
	return go;
}


void main() {
    float localTime = TIME * animationSpeed * 0.2;
    pth1 = path(localTime+.3)+origin;

    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy*2.-1.;
    vec2 uv2=uv; // Declared uv2 here to ensure it's in scope for post-processing

    if (enablePostProcess) {
        uv*=1.+pow(length(uv2*uv2*uv2*uv2),4.)*.07;
    }

    uv.y*=RENDERSIZE.y/RENDERSIZE.x;

    vec2 mouse=(vec2(mouseNormX, mouseNormY)-.5)*3.;
    if (!mouseClick) mouse=vec2(0.,-.2);

    mat2 rotview1, rotview2;
    vec3 dir=normalize(vec3(uv*.8 * zoomFactor,1.));
    dir.yz*=rot(mouse.y + cameraRotY);
    dir.xz*=rot(mouse.x + cameraRotX);

    vec3 from=origin+move(dir) + vec3(cameraPosX, cameraPosY, 0.0);

    vec3 color=raymarch(from,dir);
    color=clamp(color,vec3(.0),vec3(1.));

    color=pow(color,vec3(1.0 / 1.35)) * brightness;
    color=mix(vec3(length(color)),color,saturation);

    if (enablePostProcess) {
        // Normalize uv2 for texture2D sampling
        vec2 texCoord = (uv2 * 0.5 + 0.5) + vec2(TIME * animationSpeed * 7.25468);
        vec3 rain=pow(texture2D(inputTexture, texCoord).rgb,vec3(1.5));
        color=mix(rain,color,clamp(TIME*animationSpeed*.5-.5,0.,1.));
        color*=1.-pow(length(uv2*uv2*uv2*uv2)*1.1,6.);
        uv2.y *= RENDERSIZE.y / 360.0; // This uv2.y is for the color banding, not texture sampling
        color.r*=(.5+abs(.5-mod(uv2.y     ,.021)/.021)*.5)*1.5;
        color.g*=(.5+abs(.5-mod(uv2.y+.007,.021)/.021)*.5)*1.5;
        color.b*=(.5+abs(.5-mod(uv2.y+.014,.021)/.021)*.5)*1.5;
        color*=.9+rain*.35;
    }

    color = (color - 0.5) * contrast + 0.5;

    gl_FragColor = vec4(color,1.);
}