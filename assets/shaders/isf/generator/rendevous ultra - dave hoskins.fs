/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic"
    ],
    "DESCRIPTION": "A breathing Kleinian fractal with pumping arteries, featuring extensive customizable parameters for zoom, speed, camera, multiple color palettes, color pulsing, and advanced color adjustments, now with fractal iterations, fractal morphing, and enhanced light controls.",
    "INPUTS": [
        {
            "NAME": "zoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01
        },
        {
            "NAME": "cameraX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0,
            "STEP": 0.1
        },
        {
            "NAME": "paletteSelect",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 4.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects a color palette (0: Default, 1: Warm, 2: Cool, 3: Cyberpunk, 4: Forest)"
        },
        {
            "NAME": "colorPulseFrequency",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Frequency (speed) of color pulsing."
        },
        {
            "NAME": "colorPulseAmplitude",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude (intensity) of color pulsing."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01
        },
        {
            "NAME": "kleinianCX",
            "TYPE": "float",
            "DEFAULT": 0.808,
            "MIN": 0.1,
            "MAX": 2.0,
            "STEP": 0.001,
            "DESCRIPTION": "X-component of the Kleinian folding constant."
        },
        {
            "NAME": "kleinianCY",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.1,
            "MAX": 2.0,
            "STEP": 0.001,
            "DESCRIPTION": "Y-component of the Kleinian folding constant."
        },
        {
            "NAME": "kleinianCZ",
            "TYPE": "float",
            "DEFAULT": 1.137,
            "MIN": 0.1,
            "MAX": 2.0,
            "STEP": 0.001,
            "DESCRIPTION": "Z-component of the Kleinian folding constant."
        },
        {
            "NAME": "fractalIterations",
            "TYPE": "float",
            "DEFAULT": 9.0,
            "MIN": 1.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for fractal calculation. Higher values increase detail but impact performance."
        },
        {
            "NAME": "fractalMorphFactor",
            "TYPE": "float",
            "DEFAULT": 1.15,
            "MIN": 0.5,
            "MAX": 2.0,
            "STEP": 0.001,
            "DESCRIPTION": "Controls a key morphing parameter within the fractal calculation."
        },
        {
            "NAME": "enableReflections",
            "TYPE": "bool",
            "DEFAULT": true,
            "DESCRIPTION": "Enable or disable secondary reflections."
        },
        {
            "NAME": "reflectionStrength",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of reflections if enabled."
        },
        {
            "NAME": "lightOffsetX",
            "TYPE": "float",
            "DEFAULT": -0.56,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "X offset for the light source from camera origin."
        },
        {
            "NAME": "lightOffsetY",
            "TYPE": "float",
            "DEFAULT": -1.4,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Y offset for the light source from camera origin."
        },
        {
            "NAME": "lightOffsetZ",
            "TYPE": "float",
            "DEFAULT": 0.24,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Z offset for the light source from camera origin."
        },
        {
            "NAME": "lightMotionSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed of the light source's inherent motion."
        },
        {
            "NAME": "lightMotionAmplitude",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude of the light source's inherent motion."
        }
    ]
}
*/

// Rendezvous. By David Hoskins. Jan 2014.
// A Kleinian thingy, breathing, and with pumping arteries!
// https://www.shadertoy.com/view/ldjGDw

// Modified for ISF with new parameters by Google Gemini

vec3  lightPos;
float intensity;

//----------------------------------------------------------------------------------------
float Hash( float n )
{
    return fract(sin(n)*43758.5453123);
}

//----------------------------------------------------------------------------------------
float Noise( in float x )
{
    float p = floor(x);
    float f = fract(x);
    f = f*f*(3.0-2.0*f);
    return mix(Hash(p), Hash(p+1.0), f);
}

//----------------------------------------------------------------------------------------
// Function to get palette color (from previous shader, adapted)
vec3 getPaletteColor(float value, float paletteIndex) {
    vec3 color;
    if (paletteIndex < 0.5) { // 0: Default palette (original)
        color = 0.5 + 0.5 * sin(value * vec3(1.647, -1.0, 4.9));
    } else if (paletteIndex < 1.5) { // 1: Warm palette
        color = 0.5 + 0.5 * sin(value * vec3(2.5, 1.5, 0.5) + vec3(0.0, 2.0, 4.0));
    } else if (paletteIndex < 2.5) { // 2: Cool palette
        color = 0.5 + 0.5 * sin(value * vec3(0.5, 1.0, 2.0) + vec3(4.0, 0.0, 2.0));
    } else if (paletteIndex < 3.5) { // 3: Cyberpunk palette (greens, purples, blues)
        color = 0.5 + 0.5 * sin(value * vec3(1.0, 3.0, 5.0) + vec3(0.0, 1.5, 3.0));
    } else { // 4: Forest palette (greens, browns, earthy tones)
        color = 0.5 + 0.5 * sin(value * vec3(0.8, 1.2, 0.4) + vec3(1.0, 0.5, 2.0));
    }
    return color;
}

//----------------------------------------------------------------------------------------
float Map( vec3 p, vec3 kleinianC, float timeAdjusted, float iterations, float morphFactor)
{
	float scale = 1.0;
	// float add = sin(timeAdjusted)*.2+.1; // Original variable, not strictly needed in this context for Map

	for( int i=0; i < int(iterations);i++ ) // Use fractalIterations parameter
	{
		p = 2.0*clamp(p, -kleinianC, kleinianC) - p; // Use tunable kleinianC
		float r2 = dot(p,p);
		float k = max((morphFactor)/r2, 1.); // Use fractalMorphFactor
		p     *= k;
		scale *= k;
	}
	float l = length(p.xy);
	float rxy = l - 4.0;
	float n = l * p.z;
	rxy = max(rxy, -(n) / (length(p))-.07+sin(timeAdjusted*2.0+p.x+p.y+23.5*p.z)*.02);
    float x = (1.+sin(timeAdjusted*2.));x =x*x*x*x*.5;
    float h = dot(sin(p*.013),(cos(p.zxy*.191)))*x;
	return ((rxy+h) / abs(scale));
}

//----------------------------------------------------------------------------------------
// Modified Colour function to use new palette and pulsing
vec3 Colour( vec3 p, float paletteIndex, float pulseFrequency, float pulseAmplitude, vec3 kleinianC)
{
	float col	= 0.0;
	// float r2	= dot(p,p); // Original variable, not strictly needed for final return

	for( int i=0; i < 10;i++ ) // This loop is fixed at 10 iterations for color calculation
	{
		// Pass kleinianC to clamp in Colour function as well for consistency
		vec3 p1= 2.0 * clamp(p, -kleinianC, kleinianC)-p;
		col += abs(p.z-p1.z);
		p = p1;
		float r2_inner = dot(p,p); // Local r2 to avoid re-declaring outer r2
		float k = max((1.15)/r2_inner, 1.0);
		p *= k;
	}
    // Apply color pulsing and selected palette
    float pulse = 0.5 + pulseAmplitude * 0.5 * sin(TIME * pulseFrequency); // Use pulseFrequency and pulseAmplitude
    return getPaletteColor(col, paletteIndex) * pulse;
}

//----------------------------------------------------------------------------------------
float RayMarch( in vec3 ro, in vec3 rd, vec3 kleinianC, float timeAdjusted, float iterations, float morphFactor)
{
	float precis = 0.001;
    float h		 = 0.0;
    float t		 = .0;
	float res	 = 200.0;
	bool hit	 = false;
    for( int i = 0; i < 120; i++ )
    {
		if (!hit && t < 12.0)
		{
			h = Map(ro + rd * t, kleinianC, timeAdjusted, iterations, morphFactor); // Pass new params
			if (h < precis)
			{
				res = t;
				hit = true;
			}
			t += h * .83;
		}
    }
	
    return res;
}

//----------------------------------------------------------------------------------------
float Shadow(in vec3 ro, in vec3 rd, float dist, vec3 kleinianC, float timeAdjusted, float iterations, float morphFactor)
{
	float res = 1.0;
    float t = 0.02;
	float h = 0.0;
    
	for (int i = 0; i < 14; i++)
	{
		// Don't run past the point light source...
		if(t < dist)
		{
			h = Map(ro + rd * t, kleinianC, timeAdjusted, iterations, morphFactor); // Pass new params
			res = min(4.*h / t, res);
			t += 0.0 + h*.4;
		}
	}
    return clamp(res, 0.0, 1.0);
}

//----------------------------------------------------------------------------------------
vec3 Normal(in vec3 pos, in float t, vec3 kleinianC, float timeAdjusted, float iterations, float morphFactor)
{
	vec2  eps = vec2(t*t*.0075,0.0);
	vec3 nor = vec3(Map(pos+eps.xyy, kleinianC, timeAdjusted, iterations, morphFactor) - Map(pos-eps.xyy, kleinianC, timeAdjusted, iterations, morphFactor),
					Map(pos+eps.yxy, kleinianC, timeAdjusted, iterations, morphFactor) - Map(pos-eps.yxy, kleinianC, timeAdjusted, iterations, morphFactor),
					Map(pos+eps.yyx, kleinianC, timeAdjusted, iterations, morphFactor) - Map(pos-eps.yyx, kleinianC, timeAdjusted, iterations, morphFactor));
	return normalize(nor);
}

//----------------------------------------------------------------------------------------
float LightGlow(vec3 light, vec3 ray, float t)
{
	float ret = 0.0;
	if (length(light) < t)
	{
		light = normalize(light);
		ret = pow(max(dot(light, ray), 0.0), 2000.0)*.5;
		float a = atan(light.x - ray.x, light.z - ray.z);
		ret = (1.0+(sin(a*10.0-TIME*4.3)+sin(a*13.141+TIME*3.141)))*(sqrt(ret))*.05+ret;
		ret *= 3.0;
	}
		
	return ret;
}

//----------------------------------------------------------------------------------------
vec3 RenderPosition(vec3 pos, vec3 ray, vec3 nor, float t, vec3 kleinianC, float timeAdjusted, float paletteIndex, float pulseFrequency, float pulseAmplitude, float iterations, float morphFactor)
{
	vec3 col = vec3(0.0);				
	vec3 lPos  = lightPos-pos;
	float lightDist = length(lPos);
	vec3 lightDir  = normalize(lPos);

	float bri = max( dot( lightDir, nor ), 0.0) * intensity;
	float spe = max(dot(reflect(ray, nor), lightDir), 0.0);
	float amb = max(abs(nor.z)*.04, 0.025);
	float sha = Shadow(pos, lightDir, lightDist, kleinianC, timeAdjusted, iterations, morphFactor); // Pass new params
	col = Colour(pos, paletteIndex, pulseFrequency, pulseAmplitude, kleinianC);   // Pass new palette/pulse
	col = col * bri *sha+ pow(spe, 15.0) *sha*.7 +amb*col;
	
	return col;
}

//--------------------------------------------------------------------------
vec3 PostEffects(vec3 rgb, vec2 xy, float brightness, float contrast, float saturation)
{
	// Gamma first... (kept for original look)
	rgb = pow(rgb, vec3(0.45));

	// Then apply user controls for brightness, contrast, saturation
	rgb = rgb * brightness; // Apply brightness
    rgb = mix(vec3(0.5), rgb, contrast); // Apply contrast around 0.5 gray
    rgb = mix(vec3(dot(vec3(0.2125, 0.7154, 0.0721), rgb)), rgb, saturation); // Apply saturation

	// Vignette...
	rgb *= .5+0.5*pow(180.0*xy.x*xy.y*(1.0-xy.x)*(1.0-xy.y), 0.3 );	

	return clamp(rgb, 0.0, 1.0);
}


//------------------------------------------------------------------------
void main() {

    vec2 q = gl_FragCoord.xy/RENDERSIZE.xy;
    vec2 p = -1.0+2.0*q;
	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	
	float timeAdjusted = (TIME * speed + 26.0) * 0.2; // Use ISF TIME and new speed input for animation

    // Camera animation based on original shader, with new controls for offset and zoom
	float baseTime = sin(1.6 + timeAdjusted * 0.05) * 12.5; // Replaced iMouse.x with timeAdjusted
    float height = (smoothstep(9.4, 11.5, abs(baseTime)) * 0.5);
    
	vec3 origin = vec3( 1.2, baseTime + 1.0, 2.5 + height) * zoom; // Apply zoom
	vec3 target = vec3(0.0 + sin(baseTime), 0.0, 2.5 - height * 4.0) * zoom; // Apply zoom

    // Add manual camera controls
    origin += vec3(cameraX, cameraY, cameraZ);
    target += vec3(cameraTargetX, cameraTargetY, cameraTargetZ);

    // Calculate light position with new offsets and motion
    vec3 lightBaseOffset = vec3(lightOffsetX, lightOffsetY, lightOffsetZ);
    vec3 lightMotion = lightMotionAmplitude * vec3(
        cos(TIME * lightMotionSpeed * 2.0 + 2.8),
        sin(TIME * lightMotionSpeed * 2.5 + 1.5),
        cos(TIME * lightMotionSpeed * 3.0 + 0.5)
    );
	lightPos = origin + lightBaseOffset + lightMotion; // Combined
	intensity = .8+.3*Noise(TIME*5.0); // Use ISF TIME

    // Use tunable Kleinian constant
    vec3 kleinianConst = vec3(kleinianCX, kleinianCY, kleinianCZ);
	
	vec3 cw = normalize( target-origin);
	vec3 cp = normalize(vec3(0.0, sin(timeAdjusted * 0.25), 1.80)); // Replaced iMouse.x with timeAdjusted
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = cross(cu,cw);
	vec3 ray = normalize( p.x*cu + p.y*cv + 2.6*cw ); // 2.6 factor can be a new parameter if needed
	
	vec3 col = vec3(0.0);
	float t = 0.0;
	t = RayMarch(origin, ray, kleinianConst, timeAdjusted, fractalIterations, fractalMorphFactor); // Pass new params
	if(t < 199.0)
	{
		vec3 pos = origin + t * ray;
		vec3 nor = Normal(pos, t, kleinianConst, timeAdjusted, fractalIterations, fractalMorphFactor); // Pass new params
		col = RenderPosition(pos, ray, nor, t, kleinianConst, timeAdjusted, paletteSelect, colorPulseFrequency, colorPulseAmplitude, fractalIterations, fractalMorphFactor); // Pass all new params
		
		if (enableReflections) // Conditional reflections
		{
			vec3 ray2    = reflect(ray, nor);
			vec3 origin2 = pos + nor*.01;
			float d = RayMarch(origin2, ray2, kleinianConst, timeAdjusted, fractalIterations, fractalMorphFactor); // Pass new params
			if(d < 199.0)
			{
				pos = origin2 + d * ray2;
				nor = Normal(pos, d, kleinianConst, timeAdjusted, fractalIterations, fractalMorphFactor); // Pass new params
				col += RenderPosition(pos, ray, nor, d, kleinianConst, timeAdjusted, paletteSelect, colorPulseFrequency, colorPulseAmplitude, fractalIterations, fractalMorphFactor) * reflectionStrength; // Apply strength
			}
		}
	}
	
	// Effects...
	vec3 FogColour = vec3(.05, .05, .05); // Keep as local for now, could be tunable if desired
	col = mix(FogColour, col, exp(-.6*max(t-3.0, 0.0)));
    col = clamp(mix(col, vec3(.333), -.07), 0.0, 1.0);
	
    // Apply post-processing effects
	col = PostEffects(col, q, brightness, contrast, saturation); 

	col += LightGlow(lightPos-origin, ray, t) * intensity;
	
	gl_FragColor=vec4(clamp(col, 0.0, 1.0),1.0);
}