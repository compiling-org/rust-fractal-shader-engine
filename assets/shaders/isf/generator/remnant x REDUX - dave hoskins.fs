/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Fractal",
        "Raymarching",
        "3D"
    ],
    "DESCRIPTION": "A raymarched fractal scene with two light sources, shadows, near-surface glows, and tunable parameters. Based on 'Remnant X' by Dave_Hoskins.",
    "CREDIT": "Original Shadertoy by Dave_Hoskins, adapted for ISF by Gemini",
    "INPUTS": [
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "FractalScale",
            "TYPE": "float",
            "DEFAULT": 2.8,
            "MIN": 1.0,
            "MAX": 5.0,
            "LABEL": "Fractal Scale"
        },
        {
            "NAME": "MinRadiusSquared",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0.01,
            "MAX": 1.0,
            "LABEL": "Min Radius Squared"
        },
        {
            "NAME": "SurfaceColor1",
            "TYPE": "color",
            "DEFAULT": [0.8, 0.0, 0.0, 1.0],
            "LABEL": "Surface Color 1"
        },
        {
            "NAME": "SurfaceColor2",
            "TYPE": "color",
            "DEFAULT": [0.4, 0.4, 0.5, 1.0],
            "LABEL": "Surface Color 2"
        },
        {
            "NAME": "SurfaceColor3",
            "TYPE": "color",
            "DEFAULT": [0.5, 0.3, 0.0, 1.0],
            "LABEL": "Surface Color 3"
        },
        {
            "NAME": "FogColor",
            "TYPE": "color",
            "DEFAULT": [0.4, 0.4, 0.4, 1.0],
            "LABEL": "Fog Color"
        },
        {
            "NAME": "SunDirectionX",
            "TYPE": "float",
            "DEFAULT": 0.35,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Sun Dir X"
        },
        {
            "NAME": "SunDirectionY",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Sun Dir Y"
        },
        {
            "NAME": "SunDirectionZ",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Sun Dir Z"
        },
        {
            "NAME": "SunIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Sun Intensity"
        },
        {
            "NAME": "SpotlightIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Spotlight Intensity"
        },
        {
            "NAME": "RaymarchMaxSteps",
            "TYPE": "float",
            "DEFAULT": 100.0,
            "MIN": 50.0,
            "MAX": 200.0,
            "LABEL": "Raymarch Max Steps"
        },
        {
            "NAME": "RaymarchMinHitDistance",
            "TYPE": "float",
            "DEFAULT": 0.0005,
            "MIN": 0.00001,
            "MAX": 0.005,
            "LABEL": "Raymarch Min Hit Dist"
        },
        {
            "NAME": "ShadowSteps",
            "TYPE": "float",
            "DEFAULT": 8.0,
            "MIN": 1.0,
            "MAX": 20.0,
            "LABEL": "Shadow Steps"
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "Contrast",
            "TYPE": "float",
            "DEFAULT": 1.08,
            "MIN": 0.5,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
            "NAME": "Saturation",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": 0.0,
            "MAX": 3.0,
            "LABEL": "Saturation"
        },
        {
            "NAME": "Brightness",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": 0.0,
            "MAX": 3.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "VignetteStrength",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Vignette Strength"
        },
        {
            "NAME": "FinalGamma",
            "TYPE": "float",
            "DEFAULT": 0.47,
            "MIN": 0.1,
            "MAX": 2.0,
            "LABEL": "Final Gamma"
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

// Shader precision
#ifdef GL_ES
precision highp float;
#endif

// ISF Uniforms: RENDERSIZE, TIME are automatically provided.

#define TAU 6.28318530718

// Global time variable, influenced by MOUSE.x for "time warp"
float gTime;

//----------------------------------------------------------------------------------------
// Simple pseudo-random hash for noise, without textures
float hash11(float p) {
    p = fract(p * .1031);
    p *= p + 33.33;
    p = fract((p + p) * p);
    return fract(p);
}

vec3 hash31(float p) {
    vec3 p3 = fract(vec3(p) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}

// Basic noise function (replaces texture-based noise)
float Noise( in vec3 x )
{
    vec3 p = floor(x);
    vec3 f = fract(x);
	f = f*f*(3.0-2.0*f);
	
    // Using hash function instead of iChannel0
    float n000 = hash11(dot(p, vec3(1.0, 57.0, 113.0)));
    float n100 = hash11(dot(p + vec3(1.0, 0.0, 0.0), vec3(1.0, 57.0, 113.0)));
    float n010 = hash11(dot(p + vec3(0.0, 1.0, 0.0), vec3(1.0, 57.0, 113.0)));
    float n110 = hash11(dot(p + vec3(1.0, 1.0, 0.0), vec3(1.0, 57.0, 113.0)));
    float n001 = hash11(dot(p + vec3(0.0, 0.0, 1.0), vec3(1.0, 57.0, 113.0)));
    float n101 = hash11(dot(p + vec3(1.0, 0.0, 1.0), vec3(1.0, 57.0, 113.0)));
    float n011 = hash11(dot(p + vec3(0.0, 1.0, 1.0), vec3(1.0, 57.0, 113.0)));
    float n111 = hash11(dot(p + vec3(1.0, 1.0, 1.0), vec3(1.0, 57.0, 113.0)));

    float res = mix(mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y),
                    mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y), f.z);
    return res;
}


//----------------------------------------------------------------------------------------
// Mandelbox fractal distance estimator
float Map(vec3 pos)
{
	vec4 p = vec4(pos,1);
	vec4 p0 = p;
    
    // Tunable FractalScale and MinRadiusSquared
    float currentMinRad2 = clamp(MinRadiusSquared, 1.0e-9, 1.0);
    vec4 scaleVec = vec4(FractalScale, FractalScale, FractalScale, abs(FractalScale)) / currentMinRad2;
    float absScalem1 = abs(FractalScale - 1.0);
    // Iterations for AbsScaleRaisedTo1mIters should match the loop count for consistency (9 iterations)
    float AbsScaleRaisedTo1mIters = pow(abs(FractalScale), float(1-9));

	for (int i = 0; i < 9; i++)
	{
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;

		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(currentMinRad2/r2, currentMinRad2), 0.0, 1.0);

		// scale, translate
		p = p*scaleVec + p0;
	}
	return ((length(p.xyz) - absScalem1) / p.w - AbsScaleRaisedTo1mIters);
}

//----------------------------------------------------------------------------------------
vec3 Colour(vec3 pos, float sphereR)
{
	vec3 p = pos;
	vec3 p0 = p;
	float trap = 1.0;
    
	for (int i = 0; i < 6; i++)
	{
        
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;
		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(MinRadiusSquared/r2, MinRadiusSquared), 0.0, 1.0);

		p = p*vec3(FractalScale) + p0.xyz;
		trap = min(trap, r2);
	}
	// |c.x|: log final distance (fractional iteration count)
	// |c.y|: spherical orbit trap at (0,0,0)
	vec2 c = clamp(vec2( 0.3333*log(dot(p,p))-1.0, sqrt(trap) ), 0.0, 1.0);

    // Dynamic color mixing based on position and time
    float t_color = mod(length(pos) - gTime*150., 16.0);
    vec3 currentSurfaceColor1 = mix( SurfaceColor1.rgb, vec3(.4, 3.0, 5.), pow(smoothstep(0.0, .3, t_color) * smoothstep(0.6, .3, t_color), 10.0));
	return mix(mix(currentSurfaceColor1, SurfaceColor2.rgb, c.y), SurfaceColor3.rgb, c.x);
}


//----------------------------------------------------------------------------------------
vec3 GetNormal(vec3 pos, float distance)
{
    // Adjusted epsilon for normal calculation
    distance *= 0.001 + 0.0001;
	vec2 eps = vec2(distance, 0.0);
	vec3 nor = vec3(
	    Map(pos+eps.xyy) - Map(pos-eps.xyy),
	    Map(pos+eps.yxy) - Map(pos-eps.yxy),
	    Map(pos+eps.yyx) - Map(pos-eps.yyx));
	return normalize(nor);
}

//----------------------------------------------------------------------------------------
float GetSky(vec3 pos)
{
    pos *= 2.3;
	float t = Noise(pos);
    t += Noise(pos * 2.1) * .5;
    t += Noise(pos * 4.3) * .25;
    t += Noise(pos * 7.9) * .125;
	return t;
}

//----------------------------------------------------------------------------------------
float BinarySubdivision(in vec3 rO, in vec3 rD, vec2 t_range)
{
    float halfwayT;
    // Iterations for binary subdivision
    for (int i = 0; i < 6; i++)
    {
        halfwayT = dot(t_range, vec2(.5));
        float d = Map(rO + halfwayT*rD);
        t_range = mix(vec2(t_range.x, halfwayT), vec2(halfwayT, t_range.y), step(RaymarchMinHitDistance, d)); // Use tunable input
    }
	return halfwayT;
}

//----------------------------------------------------------------------------------------
vec2 Scene(in vec3 rO, in vec3 rD) // Removed fragCoord parameter as iChannel0 is gone
{
    // Original code used iChannel0 with fragCoord here, now it's removed.
    // The initial 't' value is now a fixed constant or can be based on other uniforms.
    float t = .05; // Starting raymarch distance
	vec3 p = vec3(0.0);
    float oldT = 0.0;
    bool hit = false;
    float glow = 0.0;
    vec2 dist_range;

    int maxRaymarchSteps = int(RaymarchMaxSteps); // Use tunable input
	for( int j=0; j < maxRaymarchSteps; j++ )
	{
		if (t > 12.0) break;
        p = rO + t*rD;
        
		float h = Map(p);
        
		if(h < RaymarchMinHitDistance) // Use tunable input
		{
            dist_range = vec2(oldT, t);
            hit = true;
            break;
        }
        glow += clamp(.05-h, 0.0, .4);
        oldT = t;
        t +=  h + t*0.001;
	}
    if (!hit)
        t = 1000.0; // Indicate no hit
    else
        t = BinarySubdivision(rO, rD, dist_range); // Refine hit with binary subdivision
    return vec2(t, clamp(glow * GlowIntensity, 0.0, 1.0)); // Use tunable GlowIntensity
}

//----------------------------------------------------------------------------------------
// This Hash function is for post-effects (takes vec2)
float Hash(vec2 p)
{
	return fract(sin(dot(p, vec2(12.9898, 78.233))) * 33758.5453)-.5;
}

//----------------------------------------------------------------------------------------
vec3 PostEffects(vec3 rgb, vec2 xy)
{
	// Gamma first...
	
	// Then...
    // Tunable Contrast, Saturation, Brightness
	rgb = mix(vec3(.5), mix(vec3(dot(vec3(.2125, .7154, .0721), rgb*Brightness)), rgb*Brightness, Saturation), Contrast);
	
	// Vignette...
    // Tunable VignetteStrength
	rgb *= .5 + 0.5*pow(20.0*xy.x*xy.y*(1.0-xy.x)*(1.0-xy.y), VignetteStrength);	

    // Tunable FinalGamma
    rgb = pow(rgb, vec3(FinalGamma));
	return rgb;
}

//----------------------------------------------------------------------------------------
float Shadow( in vec3 ro, in vec3 rd)
{
	float res = 1.0;
    float t = 0.05;
	float h;
	
    int shadowStepsCount = int(ShadowSteps); // Use tunable input
    for (int i = 0; i < shadowStepsCount; i++)
	{
		h = Map( ro + rd*t );
		res = min(6.0*h / t, res);
		t += h;
	}
    return max(res, 0.0);
}

//----------------------------------------------------------------------------------------
mat3 RotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat3(oc * axis.x * axis.x + c,       oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,        oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}

//----------------------------------------------------------------------------------------
vec3 LightSource(vec3 spotLight, vec3 dir, float dis)
{
    float g = 0.0;
    if (length(spotLight) < dis)
    {
        float a = max(dot(normalize(spotLight), dir), 0.0);
		g = pow(a, 500.0);
        g +=  pow(a, 5000.0)*.2;
    }
    
    return vec3(.6) * g;
}

//----------------------------------------------------------------------------------------
vec3 CameraPath( float t )
{
    vec3 p = vec3(-.78 + 3. * sin(2.14*t),.05+2.5 * sin(.942*t+1.3),.05 + 3.5 * cos(3.594*t) );
	return p;
}
    
//----------------------------------------------------------------------------------------
void main() {
    // Pass 0: Render the main scene to BufferA
	if (PASSINDEX == 0)	{

        // gTime now only depends on TIME and AnimationSpeed
		gTime = (TIME * AnimationSpeed)*.01 + 15.00; // Tunable AnimationSpeed
        vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy;
		vec2 uv = (-1.0 + 2.0 * xy) * vec2(RENDERSIZE.x/RENDERSIZE.y, 1.0);
		
		vec3 cameraPos	= CameraPath(gTime);
        vec3 camTar		= CameraPath(gTime + .01);
	
		float roll = 13.0*sin(gTime*.5+.4);
		vec3 cw = normalize(camTar-cameraPos);
	
		vec3 cp = vec3(sin(roll), cos(roll),0.0);
		vec3 cu = normalize(cross(cw,cp));
	
		vec3 cv = normalize(cross(cu,cw));
        cw = RotationMatrix(cv, sin(-gTime*20.0)*.7) * cw;
		vec3 dir = normalize(uv.x*cu + uv.y*cv + 1.3*cw);
	
        vec3 spotLight = CameraPath(gTime + .03) + vec3(sin(gTime*18.4), cos(gTime*17.98), sin(gTime * 22.53))*.2;
		vec3 col = vec3(0.0);
        // Tunable SunDirection
        vec3 sunDir = normalize(vec3(SunDirectionX, SunDirectionY, SunDirectionZ));
        vec3 sky = FogColor.rgb * GetSky(dir); // Use tunable FogColor
		vec2 ret = Scene(cameraPos, dir); // Removed fragCoord parameter
        
        if (ret.x < 900.0)
        {
			vec3 p = cameraPos + ret.x*dir;
			vec3 nor = GetNormal(p, ret.x);
            
            vec3 spot = spotLight - p;
			float atten = length(spot);
	
            spot /= atten;
            
            float shaSpot = Shadow(p, spot);
            float shaSun = Shadow(p, sunDir);
            
            float bri = max(dot(spot, nor), 0.0) / pow(atten, 1.5) * .25;
            float briSun = max(dot(sunDir, nor), 0.0) * .2;
            
            col = Colour(p, ret.x);
            // Tunable SunIntensity and SpotlightIntensity
            col = (col * bri * shaSpot * SpotlightIntensity) + (col * briSun * shaSun * SunIntensity);
            
            vec3 ref = reflect(dir, nor);
            col += pow(max(dot(spot,  ref), 0.0), 10.0) * 2.0 * shaSpot * bri * SpotlightIntensity;
            col += pow(max(dot(sunDir, ref), 0.0), 10.0) * 2.0 * shaSun * briSun * SunIntensity;
        }
        
        col = mix(sky, col, min(exp(-ret.x+1.5), 1.0));
        col += vec3(pow(abs(ret.y), 2.)) * vec3(.02, .04, .1); // Glow from Scene function
	
        col += LightSource(spotLight-cameraPos, dir, ret.x);
		col = PostEffects(col, xy);	
		
		gl_FragColor=vec4(col,1.0);
	}
    // Pass 1: Display BufferA (final output)
	else if (PASSINDEX == 1)	{
        // This pass simply displays the content rendered to BufferA in Pass 0.
        gl_FragColor = IMG_NORM_PIXEL(BufferA, gl_FragCoord.xy / RENDERSIZE.xy);
	}
}