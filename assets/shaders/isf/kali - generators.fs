/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Psychedelic",
        "Raymarching"
    ],

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
            "NAME": "cameraRotZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera Z Rotation"
        },
        {
            "NAME": "fractalParamX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Mod X"
        },
        {
            "NAME": "fractalParamZ",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Fractal Mod Z"
        },
        {
            "NAME": "sphereRadius",
            "TYPE": "float",
            "DEFAULT": 0.015,
            "MIN": 0.001,
            "MAX": 0.1,
            "LABEL": "Sphere Radius"
        },
        {
            "NAME": "sphereGlowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Sphere Glow"
        },
        {
            "NAME": "lightPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Light Pulse Speed"
        },
        {
            "NAME": "lightPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Light Pulse Intensity"
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
        }
    ],
    "PASSES": [
        {}
    ]
}
*/

#define RAY_STEPS 70
#define SHADOW_STEPS 50
#define LIGHT_COLOR vec3(.85,.9,1.)
#define AMBIENT_COLOR vec3(.8,.83,1.)
#define FLOOR_COLOR vec3(1.,.7,.9)
#define ENERGY_COLOR vec3(1.,.7,.4)
#define DETAIL .00005

// ISF Built-in uniforms: TIME, RENDERSIZE
const float pi = acos(-1.0);
const float pi2 = pi * 2.0;

// Custom uniforms (from JSON INPUTS) are automatically available.



vec3 lightdir=normalize(vec3(0.5,-0.3,-1.));
vec3 ambdir=normalize(vec3(0.,0.,1.));
const vec3 origin=vec3(0.,3.11,0.);
vec3 energy=vec3(0.01);

float vibration=0.;
float det=0.0;
vec3 pth1;


mat2 rot(float a) {
	return mat2(cos(a),sin(a),-sin(a),cos(a));	
}

vec3 path(float ti) {
    return vec3(sin(ti),.3-sin(ti*.632)*.3,cos(ti*.5))*.5;
}

float Sphere(vec3 p, vec3 rd, float r){
	float b = dot( -p, rd );
	float inner = b * b - dot( p, p ) + r * r;
	if( inner < 0.0 ) return -1.0;
	return b - sqrt( inner );
}

vec2 de(vec3 pos) {
	float hid=0.;
	vec3 tpos=pos;
	tpos.xz=abs(fractalParamX - mod(tpos.xz, fractalParamZ));
	vec4 p=vec4(tpos,1.);
	float y=max(0.,.35-abs(pos.y-3.35))/.35;
	for (int i=0; i<7; i++) {
		p.xyz = abs(p.xyz) - vec3(-0.02,1.98,-0.02) * (1.0 - morphStrength) - morphStrength * 0.5;
		p=p*(2.0+vibration*y)/clamp(dot(p.xyz,p.xyz),.4,1.)-vec4(0.5,1.,0.4,0.);
		p.xz*=mat2(-0.416,-0.91,0.91,-0.416);
	}
	float fl=pos.y-3.013;
	float fr=(length(max(abs(p.xyz)-vec3(0.1,5.0,0.1),vec3(0.0)))-0.05)/p.w;
	float d=min(fl,fr);
	d=min(d,-pos.y+3.95);
	if (abs(d-fl)<.001) hid=1.;
	return vec2(d,hid);
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
    // Use sphereRadius for the sphere in shadow calculation
	float t1=Sphere((pos-.005*sdir)-pth1,-sdir,sphereRadius);
	if (t1>0. && t1<.5) {
		vec3 sphglowNorm=normalize(pos-t1*sdir-pth1);
		sh=1.-pow(max(.0,dot(sphglowNorm,sdir))*1.2,3.);
	}
	for (int steps=0; steps<SHADOW_STEPS; steps++) {
		if (totdist<.6 && dist>DETAIL) {
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
    float sca = 14.0;
    for( int aoi=0; aoi<5; aoi++ ) {
        float hr = aodet*float(aoi*aoi);
        vec3 aopos = nor * hr + pos;
        float dd = de( aopos ).x;
        totao += -(dd-hr)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - 5.0*totao, 0., 1.0 );
}

float _texture(vec3 p) {
	p=abs(.5-fract(p*10.));
	vec3 c=vec3(3.);
	float es, l=es=0.;
	for (int i = 0; i < 10; i++) {
			p = abs(p + c) - abs(p - c) - p;
			p/= clamp(dot(p, p), .0, 1.);
			p = p* -1.5 + c;
			if ( mod(float(i), 2.) < 1. ) {
				float pl = l;
				l = length(p);
				es+= exp(-1. / abs(l - pl));
			}
	}
	return es;
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
	float diff=max(0.,dot(lightdir,-n))*sh;
	float y=3.35-p.y;
	vec3 amb=max(.5,dot(dir,-n))*.5*getPsychedelicPalette(0.5, int(colorPaletteIndex));
	if (hid<.5) {
		amb+=max(0.2,dot(vec3(0.,1.,0.),-n))*FLOOR_COLOR*pow(max(0.,.2-abs(3.-p.y))/.2,1.5)*2.;
		amb+=energy*pow(max(0.,.4-abs(y))/.4,2.)*max(0.2,dot(vec3(0.,-sign(y),0.),-n))*2.;
	}
	vec3 r = reflect(lightdir,n);
	float spec=pow(max(0.,dot(dir,-r))*sh,10.);
	vec3 col;
	float energysource=pow(max(0.,.04-abs(y))/.04,4.)*2.;
	if (hid>1.5) {col=vec3(1.); spec=spec*spec;} // Sphere color
	else{
		float k=_texture(p)*.23+.2;
		k=min(k,1.5-energysource);
		col=mix(vec3(k,k*k,k*k*k),getPsychedelicPalette(k, int(colorPaletteIndex)),.3);
		if (abs(hid-1.)<.001) col*=FLOOR_COLOR*1.3;
	}
	col=col*(amb+diff*LIGHT_COLOR)+spec*LIGHT_COLOR;	
	if (hid<.5) {
		col=max(col,energy*2.*energysource);
	}
	col*=min(1.,ao+length(energy)*.5*max(0.,.1-abs(y))/.1);
	return col;
}

vec3 raymarch(in vec3 from, in vec3 dir) {
	float localTime = TIME * animationSpeed * 0.25;
	float ey=mod(localTime*.5,1.);
	float glow,eglow,ref,sphdist,totdist=glow=eglow=ref=sphdist=0.;
	vec2 d=vec2(1.,0.);
	vec3 p, col=vec3(0.);
	vec3 origdir=dir,origfrom=from,sphNorm;
	
    if (enableVibration) {
        vibration = sin(TIME * animationSpeed * 60.) * .0013;
    } else {
        vibration = 0.;
    }

    // Use sphereRadius for the sphere
	vec3 wob=cos(dir*500.0*length(from-pth1)+(from-pth1)*250.+TIME*animationSpeed*10.)*0.0005;
	float t1=Sphere(from-pth1+wob,dir,sphereRadius);
	float tg=Sphere(from-pth1+wob,dir,sphereRadius * 1.3); // Glow sphere slightly larger
	if(t1>0.){
		ref=1.0;from+=t1*dir;sphdist=t1;
		sphNorm=normalize(from-pth1+wob);
		dir=reflect(dir,sphNorm);
	}
	else if (tg>0.) {
		vec3 sphglowNorm=normalize(from+tg*dir-pth1+wob);
		glow+=pow(max(0.,dot(sphglowNorm,-dir)),5.) * sphereGlowIntensity; // Apply glow intensity
	}
	
	for (int i=0; i<RAY_STEPS; i++) {
		if (d.x>det && totdist<3.0) {
			p=from+totdist*dir;
			d=de(p);
			det=DETAIL*(1.+totdist*60.)*(1.+ref*5.);
			totdist+=d.x;
            // Light pulse controls
			energy=ENERGY_COLOR*(1.5+sin(TIME*animationSpeed*lightPulseSpeed+p.z*10.))*.25 * lightPulseIntensity;
			if(d.x<0.015)glow+=max(0.,.015-d.x)*exp(-totdist);
			if (d.y<.5 && d.x<0.03){
				float glw=min(abs(3.35-p.y-ey),abs(3.35-p.y+ey));
				eglow+=max(0.,.03-d.x)/.03*
				(pow(max(0.,.05-glw)/.05,5.)
				+pow(max(0.,.15-abs(3.35-p.y))/.15,8.))*1.5 * sphereGlowIntensity; // Apply glow intensity
			}
		}
	}
	float l=pow(max(0.,dot(normalize(-dir.xz),normalize(lightdir.xz))),2.);
	l*=max(0.2,dot(-dir,lightdir));
	vec3 backg=getPsychedelicPalette(0.2, int(colorPaletteIndex)) * (1.2-l) + LIGHT_COLOR*l*.7;
	backg*=getPsychedelicPalette(0.8, int(colorPaletteIndex));
	if (d.x<=det) {
		vec3 norm=normal(p-abs(d.x-det)*dir);
		col=light(p-abs(d.x-det)*dir, dir, norm, d.y)*exp(-.2*totdist*totdist);
		col = mix(col, backg, 1.0-exp(-1.*pow(totdist,1.5)));
	} else {
		col=backg;
	}
	vec3 lglow=LIGHT_COLOR*pow(l,30.)*.5;
	col+=glow*(backg+lglow)*1.3;
	col+=pow(eglow,2.)*energy*.015;
	col+=lglow*min(1.,totdist*totdist*.3);
	if (ref>0.5) {
		vec3 sphlight=light(origfrom+sphdist*origdir,origdir,sphNorm,2.);
		col=mix(col*.3+sphlight*.7,backg,1.0-exp(-1.*pow(sphdist,1.5)));
	}
	return col;
}

vec3 move(inout mat2 rotview1,inout mat2 rotview2) {
	float localTime = TIME * animationSpeed;
	vec3 go=path(localTime);
	vec3 adv=path(localTime+.7);
	vec3 advec=normalize(adv-go);
	float an=atan(advec.x,advec.z);
	rotview1=mat2(cos(an),sin(an),-sin(an),cos(an));
	an=advec.y*1.7;
	rotview2=mat2(cos(an),sin(an),-sin(an),cos(an));
	return go;
}


void main() {
    float localTime = TIME * animationSpeed * 0.25;
    pth1 = path(localTime+.3)+origin+vec3(0.,.01,0.);

    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy*2.-1.;
    vec2 uv2=uv;

    if (enablePostProcess) {
        uv*=1.+pow(length(uv2*uv2*uv2*uv2),4.)*.07;
    }

    uv.y*=RENDERSIZE.y/RENDERSIZE.x;

    vec2 mouse = (vec2(mouseNormX, mouseNormY)-.5)*3.;
    if (!mouseClick) mouse=vec2(0.);

    mat2 rotview1, rotview2;
    vec3 from=origin+move(rotview1,rotview2) + vec3(cameraPosX, cameraPosY, 0.0);
    vec3 dir=normalize(vec3(uv*.8 * zoomFactor,1.));

    dir.yz*=rot(mouse.y);
    dir.xz*=rot(mouse.x + cameraRotZ);
    dir.yz*=rotview2;
    dir.xz*=rotview1;

    vec3 color=raymarch(from,dir);
    color=clamp(color,vec3(.0),vec3(1.));

    // Original ShaderToy constants: GAMMA 1.3, BRIGHTNESS .9, SATURATION .85
    color=pow(color,vec3(1.0 / 1.3)) * brightness;
    color=mix(vec3(length(color)),color,saturation);

    // Color Pulse removed as requested

    if (enablePostProcess) {
        vec3 rain=pow(texture2D(inputTexture,uv2+TIME*7.25468).rgb,vec3(1.5));
        color=mix(rain,color,clamp(TIME*.5-.5,0.,1.));
        color*=1.-pow(length(uv2*uv2*uv2*uv2)*1.1,6.);
        uv2.y *= RENDERSIZE.y / 360.0;
        color.r*=(.5+abs(.5-mod(uv2.y     ,.021)/.021)*.5)*1.5;
        color.g*=(.5+abs(.5-mod(uv2.y+.007,.021)/.021)*.5)*1.5;
        color.b*=(.5+abs(.5-mod(uv2.y+.014,.021)/.021)*.5)*1.5;
        color*=.9+rain*.35;
    }

    color = (color - 0.5) * contrast + 0.5;

    gl_FragColor = vec4(color,1.);
}