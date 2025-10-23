/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Fractal",
        "Psychedelic"
    ],
    "DESCRIPTION": "Fractalscape effect from Rhodium 4k Intro with added tunable parameters for zoom, speed, color pulse, psychedelic color schemes, fractal control, morphing, and geometry.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "colorPulseFrequency",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Pulse Freq."
        },
        {
            "NAME": "paletteSelect",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 11.0,
            "DEFAULT": 0.0,
            "LABEL": "Color Palette",
            "VALUES": ["Rainbow 1","Rainbow 2","Psychedelic 1","Psychedelic 2","Warm Tones","Cool Tones","Monochrome","Electric","Forest","Ocean","Sunset","Custom"]
        },
        {
            "NAME": "customColor1",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 1.0, 1.0],
            "LABEL": "Custom Color 1"
        },
        {
            "NAME": "customColor2",
            "TYPE": "color",
            "DEFAULT": [1.0, 0.0, 0.0, 1.0],
            "LABEL": "Custom Color 2"
        },
        {
            "NAME": "fractalScaleFactor",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "LABEL": "Fractal Scale"
        },
        {
            "NAME": "fractalOffsetX",
            "TYPE": "float",
            "MIN": -2.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Fractal Offset X"
        },
        {
            "NAME": "fractalOffsetY",
            "TYPE": "float",
            "MIN": -2.0,
            "MAX": 2.0,
            "DEFAULT": 0.75,
            "LABEL": "Fractal Offset Y"
        },
        {
            "NAME": "fractalOffsetZ",
            "TYPE": "float",
            "MIN": -2.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Fractal Offset Z"
        },
        {
            "NAME": "morphIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Morph Intensity"
        },
        {
            "NAME": "geometryDistortion",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.0,
            "LABEL": "Geometry Distortion"
        },
        {
            "NAME": "cameraWobbleIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.1,
            "LABEL": "Camera Wobble Intensity"
        },
        {
            "NAME": "cameraRotationSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.125,
            "LABEL": "Camera Rotation Speed"
        },
        {
            "NAME": "refractionAngleAdjust",
            "TYPE": "float",
            "MIN": -0.1,
            "MAX": 0.1,
            "DEFAULT": 0.0,
            "LABEL": "Refraction Angle Adjust"
        },
        {
            "NAME": "glowEffectIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.1,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "mistEffectIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.1,
            "LABEL": "Mist Intensity"
        },
        {
            "NAME": "blurEffectRadius",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Blur Radius"
        },
        {
            "NAME": "openCloseEffectSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.1,
            "LABEL": "Open/Close Speed"
        }
    ],
    "PASSES": [
        {
            
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ]
}
*/

// Set medium precision for floats, generally good for compatibility
precision mediump float;


// ***********************************************************
// Alcatraz / Rhodium 4k Intro Fractalscape
// by Jochen "Virgill" Feldkötter
//
// 4kb executable: http://www.pouet.net/prod.php?which=68239
// Youtube: https://www.youtube.com/watch?v=YK7fbtQw3ZU
// ***********************************************************

int meep =0;

// Rotation (not directly used as tunable param, but part of fractal logic)
void pR(inout vec2 p,float a) 
{
	p=cos(a)*p+sin(a)*vec2(p.y,-p.x);
}

// 3D noise function (IQ)
float noise(vec3 p)
{
	vec3 ip=floor(p);
    p-=ip; 
    vec3 s=vec3(7,157,113);
    vec4 h=vec4(0.,s.yz,s.y+s.z)+dot(ip,s);
    p=p*p*(3.-2.*p); 
    h=mix(fract(sin(h)*43758.5),fract(sin(h+s.x)*43758.5),p.x);
    h.xy=mix(h.xz,h.yw,p.y);
    return mix(h.x,h.y,p.z); 
}

// Psychedelic Color Palettes
vec3 getPsychedelicColor(float index, float time) {
    // Custom palette based on user inputs
    if (index >= 11.0) { // "Custom" palette
        return mix(customColor1.rgb, customColor2.rgb, 0.5 + 0.5 * sin(time * colorPulseFrequency));
    }
    
    // Standard palettes
    vec3 color;
    float t = time * colorPulseFrequency;

    if (index < 1.0) { // Rainbow 1
        color = 0.5 + 0.5 * cos(t + vec3(0, 2, 4));
    } else if (index < 2.0) { // Rainbow 2
        color = 0.5 + 0.5 * sin(t + vec3(0, 2, 4));
    } else if (index < 3.0) { // Psychedelic 1
        color = vec3(sin(t*0.8), cos(t*1.2), sin(t*1.5)) * 0.5 + 0.5;
    } else if (index < 4.0) { // Psychedelic 2
        color = vec3(fract(sin(t*0.7)*43758.5), fract(sin(t*1.1)*43758.5), fract(sin(t*1.4)*43758.5));
    } else if (index < 5.0) { // Warm Tones
        color = mix(vec3(1.0, 0.5, 0.0), vec3(1.0, 0.0, 0.5), 0.5 + 0.5 * sin(t));
    } else if (index < 6.0) { // Cool Tones
        color = mix(vec3(0.0, 0.5, 1.0), vec3(0.0, 1.0, 0.5), 0.5 + 0.5 * sin(t));
    } else if (index < 7.0) { // Monochrome (Blue-White)
        color = mix(vec3(0.0, 0.0, 0.5), vec3(1.0, 1.0, 1.0), 0.5 + 0.5 * sin(t));
    } else if (index < 8.0) { // Electric (Cyan-Magenta)
        color = mix(vec3(0.0, 1.0, 1.0), vec3(1.0, 0.0, 1.0), 0.5 + 0.5 * sin(t));
    } else if (index < 9.0) { // Forest (Green-Brown)
        color = mix(vec3(0.1, 0.4, 0.1), vec3(0.5, 0.3, 0.1), 0.5 + 0.5 * sin(t));
    } else if (index < 10.0) { // Ocean (Blue-Green)
        color = mix(vec3(0.0, 0.2, 0.6), vec3(0.2, 0.8, 0.6), 0.5 + 0.5 * sin(t));
    } else { // Sunset (Orange-Purple)
        color = mix(vec3(1.0, 0.4, 0.0), vec3(0.6, 0.0, 0.6), 0.5 + 0.5 * sin(t));
    }
    return color;
}

// kifs fractal (shane) - now with tunable fractalScaleFactor, fractalOffsetX/Y/Z, and morphIntensity
float kifs(vec3 p)
{
    vec3 offs = vec3(fractalOffsetX, fractalOffsetY, fractalOffsetZ); // Tunable fractal offset
    vec2 a = sin(vec2(0, 1.57) + 1.57/2.);
    vec2 b = sin(vec2(0, 1.57) + 1.57/4.);
    float s = fractalScaleFactor; // Tunable fractal scale
    float d = 1e5; // distance
    p  = abs(fract(p*.5)*2. - 1.);
    float amp = 1./s; 
    for(int i=0; i<5; i++)
    {
        // rotation
       	p.xy=mat2(a.y,-a.x,a)*p.xy;
       	p.yz=mat2(b.y,-b.x,b)*p.yz;
        p=abs(p);
    	if (p.x<p.y)p.xy=p.yx;
        if (p.x<p.z)p.xz=p.zx;
        if (p.y<p.z)p.yz=p.zy;
		p = p*s + offs*(1. - s);
        p.z -= step(p.z, offs.z*(1. - s)*.5)*offs.z*(1. - s);
        p=abs(p);
        d = min(d, max(max(p.x, p.y), p.z)*amp);
        amp /= s; 
        // animation
        if(i==1&&p.x>(9.+1.*sin(0.209*TIME*animationSpeed+1.))) meep = 1;
    }
    // Apply morphing based on morphIntensity
    d = mix(d, length(p) - 0.5, morphIntensity);
	return d - 0.29;
}

// Geometry distortion applied to the position before mapping
float map(vec3 p)
{	
    // Apply geometry distortion
    p += geometryDistortion * sin(p * 10.0 + TIME * animationSpeed);

// fractalscape 
    float f = -0.05-kifs(.4*p);
	if(meep==0) f+=0.002*noise(p*70.);
	return f;
}

// normal calculation
vec3 calcNormal(vec3 pos)
{
    float eps=0.0001;
	float d=map(pos);
	return normalize(vec3(map(pos+vec3(eps,0,0))-d,map(pos+vec3(0,eps,0))-d,map(pos+vec3(0,0,eps))-d));
}

// standard sphere tracing inside and outside
float castRayx(vec3 ro,vec3 rd) 
{
    float function_sign=(map(ro)<0.)?-1.:1.;
    float precis=.0001;
    float h=precis*2.;
    float t=0.;
	for(int i=0;i<120;i++) 
	{
        if(abs(h)<precis||t>12.)break;
		h=function_sign*map(ro+rd*t);
        t+=h;
	}
    return t;
}

// refraction - now with tunable refractionAngleAdjust
float refr(vec3 pos,vec3 lig,vec3 dir,vec3 nor,float angle,out float t2, out vec3 nor2)
{
    float h=0.;
    t2=2.;
	vec3 dir2=refract(dir,nor,angle + refractionAngleAdjust);  
	for(int i=0;i<50;i++) 
	{
		if(abs(h)>3.) break;
		h=map(pos+dir2*t2);
		t2-=h;
	}
    nor2=calcNormal(pos+dir2*t2);
    return(.5*clamp(dot(-lig,nor2),0.,1.)+pow(max(dot(reflect(dir2,nor2),lig),0.),8.));
}

// softshadow (IQ)
float softshadow(vec3 ro,vec3 rd) 
{
    float sh=1.;
    float t=.02;
    float h=.0;
    for(int i=0;i<22;i++)  
	{
        if(t>20.)continue;
        h=map(ro+rd*t);
        sh=min(sh,4.*h/t);
        t+=h;
    }
    return sh;
}

const float GA =2.399; 

mat2 rot = mat2(cos(GA),sin(GA),-sin(GA),cos(GA));

// simplified version of Dave Hoskins blur - now with tunable blurEffectRadius
// Using texture2D() for GLSL ES 1.0 compatibility
vec3 dof(sampler2D tex,vec2 uv_norm,float rad) 
{
	vec3 acc=vec3(0);
    vec2 pixel=vec2(.002*RENDERSIZE.y/RENDERSIZE.x,.002),angle=vec2(0,rad);;
    rad=blurEffectRadius; // Use tunable blurEffectRadius
	for (int j=0;j<80;j++)
    {  
        rad += 1./rad;
	    angle*=rot;
        // Changed to texture2D()
        vec4 col=texture2D(tex,uv_norm+pixel*(rad-1.)*angle);
		acc+=col.xyz;
	}
	return acc/80.;
}

//-------------------------------------------------------------------------------------------
void main() {
	if (PASSINDEX == 0)	{
	    float bounce=abs(fract(0.05*TIME*animationSpeed)-.5)*20.; // triangle function, influenced by animationSpeed
	    meep=0;
		vec2 uv=gl_FragCoord.xy/RENDERSIZE.xy; 
	    vec2 p=uv*2.-1.;
	    
	// bouncy cam every 10 seconds - now with tunable cameraWobbleIntensity
	    float wobble=(fract(.1*(TIME*animationSpeed-1.))>=0.9)?fract(-TIME*animationSpeed)*cameraWobbleIntensity*sin(30.*TIME*animationSpeed):0.;
	    
	// camera - now with tunable zoomFactor, cameraWobbleIntensity, cameraRotationSpeed
	    vec3 dir = normalize(vec3(2.*gl_FragCoord.xy -RENDERSIZE.xy, RENDERSIZE.y));
	    dir /= zoomFactor; // Apply zoom factor
	//	org (Left-Right,Down-Up,Near-Far)  
	    vec3 org = vec3(0,2.*wobble,-3.);  
	    
	// cam fractalscape - now with tunable cameraRotationSpeed
	   	vec2 m = sin(vec2(0, 1.57) + TIME*cameraRotationSpeed);
	   	dir.xy = mat2(m.y, -m.x, m)*dir.xy;
	   	dir.xz = mat2(m.y, -m.x, m)*dir.xz;
	   	org = vec3(0, 2.+wobble, 0.+8.*sin(bounce/3.));
	
	// standard sphere tracing:
	    vec3 color = vec3(0.);
	    vec3 color2 = vec3(0.);
	    float t=castRayx(org,dir);
		vec3 pos=org+dir*t;
		vec3 nor=calcNormal(pos);
	
	// lighting:
	    vec3 lig=normalize(-pos);
	
	// scene depth    
	    float depth=clamp((1.-0.09*t),0.,1.);
	    vec3 pos2,nor2 =  vec3(0.);
	    if(t<12.0)
	    {
	    	color2 = vec3(max(dot(lig,nor),0.)  +  pow(max(dot(reflect(dir,nor),lig),0.),16.));
	    	color2 *=clamp(softshadow(pos,lig),0.,1.);  // shadow                 	
	
	        if(meep==1) 								// refraction
	        {    
	        	float t2;
				color2.r +=refr(pos,lig,dir,nor,0.91, t2, nor2)*depth;
	       		color2.g +=refr(pos,lig,dir,nor,0.90, t2, nor2)*depth;
	       		color2.b +=refr(pos,lig,dir,nor,0.89, t2, nor2)*depth;
	   			color2-=clamp(.1*t2,0.,1.);				// inner intensity loss
	        }
		}        
	    float tmp = 0.;
	    float T = 1.;
	
	// animation of glow intensity - now with tunable glowEffectIntensity
	    float intensity = glowEffectIntensity*-sin(.209*TIME*animationSpeed+1.)+0.05; 
		for(int i=0; i<128; i++)
		{
	    	if (i<int(1.*(t+110.))) continue;// intensity damping
	        float density = 0.; float nebula = noise(org+bounce);
	        
	        density=(meep==1)?intensity-map(org+.5*nor2)*nebula:.7*intensity-map(org)*nebula;
			if(density>0.)
			{
				tmp = density / 128.;
	            T *= 1. -tmp * 100.;
				if( T <= 0.) break;
			}
			org += dir*0.078;
	    }    
	
	    // Base color and color pulse - now with tunable paletteSelect and colorPulseFrequency
		vec3 basecol = getPsychedelicColor(paletteSelect, TIME);				
	    T=clamp(T,0.,1.5); 
	    color += basecol* exp(4.*(0.5-T) - 0.8);
	    color2*=depth;
	    color2+= (1.-depth)*noise(6.*dir+0.3*TIME*animationSpeed)*mistEffectIntensity;	// subtle mist, influenced by mistEffectIntensity
	    
	// scene depth included in alpha channel
	    gl_FragColor = vec4(vec3(1.*color+0.8*color2)*1.3,abs(0.67-depth)*2.+4.*wobble);
        
        // DEBUG LINE REMOVED: gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0); // Output solid red
	}
	else if (PASSINDEX == 1)	{
		vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	
	// open and close effect - now with tunable openCloseEffectSpeed
	    float blend,blend2,multi1,multi2;
	    blend=min (3. *abs(sin((openCloseEffectSpeed*TIME)*3.1415/3.0)),1.); 
	    blend2=min(2.5*abs(sin((openCloseEffectSpeed*TIME)*3.1415/3.0)),1.); 
	    
	    multi1=((fract(uv.x*6.-4.*uv.y*(1.-blend2))< 0.5 || uv.y<blend) 
	    &&(fract(uv.x*6.-4.*uv.y*(1.-blend2))>=0.5 || uv.y>1.-blend))?1.:0.;
	 	multi2=(fract(uv.x*12.-0.05-8.*uv.y*(1.-blend2))>0.9)?blend2:1.;
	    
	    uv.y=(fract(uv.x*6.-4.*uv.y*(1.-blend2))<0.5)?uv.y-(1.-blend):uv.y+=(1.-blend);
        // Changed to texture2D() for BufferA sampling
		gl_FragColor=vec4(dof(BufferA, uv, texture2D(BufferA,mod(uv,vec2(1.0))).w),1.)*multi1*multi2*blend2;
	}
}