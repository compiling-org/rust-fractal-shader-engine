/*{
  "CATEGORIES": [
    "Procedural",
    "Generative",
    "Fractal",
    "Raymarching",
    "3D",
    "Psychedelic",
    "DMT"
  ],
  "DESCRIPTION": "A highly customizable raymarched fractal scene with 6 distinct multi-cored psychedelic color palettes, advanced color pulsing, extensive camera controls, screen shake, and more fractal parameters for deep tuning of DMT-like visuals.",
  "CREDIT": "Original Shadertoy by Dave_Hoskins, adapted for ISF by Gemini",
  "INPUTS": [
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 5,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "FractalScale",
      "TYPE": "float",
      "DEFAULT": 2.8,
      "MIN": 1,
      "MAX": 5,
      "LABEL": "Fractal Scale"
    },
    {
      "NAME": "FractalOffset",
      "TYPE": "point3D",
      "DEFAULT": [
        0,
        0,
        0
      ],
      "LABEL": "Fractal Offset",
      "COMMENT": "Global offset for the fractal position"
    },
    {
      "NAME": "MinRadiusSquared",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.001,
      "MAX": 1,
      "LABEL": "Min Radius Squared"
    },
    {
      "NAME": "MandelboxIterations",
      "TYPE": "float",
      "DEFAULT": 9,
      "MIN": 5,
      "MAX": 15,
      "LABEL": "Mandelbox Iterations",
      "COMMENT": "Number of iterations for fractal detail"
    },
    {
      "NAME": "FractalFoldStrength",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.5,
      "MAX": 2,
      "LABEL": "Fractal Fold Strength",
      "COMMENT": "Adjusts the folding behavior within the fractal"
    },
    {
      "NAME": "PaletteSelector",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 5,
      "SCALE": 1,
      "LABEL": "Palette Selector",
      "COMMENT": "Selects a base psychedelic color palette (0-5)"
    },
    {
      "NAME": "PaletteMix",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Palette Mix",
      "COMMENT": "Blends between the selected palette and the next one"
    },
    {
      "NAME": "PaletteBlend",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Palette Blend",
      "COMMENT": "Blends original surface colors with psychedelic palettes"
    },
    {
      "NAME": "HuePulseSpeed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 10,
      "LABEL": "Hue Pulse Speed"
    },
    {
      "NAME": "HuePulseAmount",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Hue Pulse Amount"
    },
    {
      "NAME": "SatPulseSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 10,
      "LABEL": "Sat Pulse Speed"
    },
    {
      "NAME": "SatPulseAmount",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Sat Pulse Amount"
    },
    {
      "NAME": "BriPulseSpeed",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": 0,
      "MAX": 10,
      "LABEL": "Bri Pulse Speed"
    },
    {
      "NAME": "BriPulseAmount",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Bri Pulse Amount"
    },
    {
      "NAME": "SurfaceColorA",
      "TYPE": "color",
      "DEFAULT": [
        0.8,
        0,
        0,
        1
      ],
      "LABEL": "Surface Color A (Base)"
    },
    {
      "NAME": "SurfaceColorB",
      "TYPE": "color",
      "DEFAULT": [
        0.4,
        0.4,
        0.5,
        1
      ],
      "LABEL": "Surface Color B (Base)"
    },
    {
      "NAME": "SurfaceColorC",
      "TYPE": "color",
      "DEFAULT": [
        0.5,
        0.3,
        0,
        1
      ],
      "LABEL": "Surface Color C (Base)"
    },
    {
      "NAME": "GlowColor",
      "TYPE": "color",
      "DEFAULT": [
        0.02,
        0.04,
        0.1,
        1
      ],
      "LABEL": "Glow Color"
    },
    {
      "NAME": "FogColor",
      "TYPE": "color",
      "DEFAULT": [
        0.4,
        0.4,
        0.4,
        1
      ],
      "LABEL": "Fog Color"
    },
    {
      "NAME": "LightingAmbient",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Ambient Light"
    },
    {
      "NAME": "Light1DirectionX",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": -1,
      "MAX": 1,
      "LABEL": "Light 1 Dir X"
    },
    {
      "NAME": "Light1DirectionY",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": -1,
      "MAX": 1,
      "LABEL": "Light 1 Dir Y"
    },
    {
      "NAME": "Light1DirectionZ",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": -1,
      "MAX": 1,
      "LABEL": "Light 1 Dir Z"
    },
    {
      "NAME": "Light1Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        1,
        1,
        1
      ],
      "LABEL": "Light 1 Color"
    },
    {
      "NAME": "Light1Intensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 5,
      "LABEL": "Light 1 Intensity"
    },
    {
      "NAME": "Light2Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0.7,
        0.3,
        1
      ],
      "LABEL": "Light 2 Color"
    },
    {
      "NAME": "Light2Intensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 5,
      "LABEL": "Light 2 Intensity"
    },
    {
      "NAME": "RaymarchMaxSteps",
      "TYPE": "float",
      "DEFAULT": 100,
      "MIN": 50,
      "MAX": 200,
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
      "DEFAULT": 8,
      "MIN": 1,
      "MAX": 20,
      "LABEL": "Shadow Steps"
    },
    {
      "NAME": "GlowIntensity",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Glow Intensity"
    },
    {
      "NAME": "CameraOrbitSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Cam Orbit Speed"
    },
    {
      "NAME": "CameraZoom",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.1,
      "MAX": 5,
      "LABEL": "Camera Zoom"
    },
    {
      "NAME": "CameraFOV",
      "TYPE": "float",
      "DEFAULT": 1.3,
      "MIN": 0.1,
      "MAX": 3,
      "LABEL": "Camera FOV",
      "COMMENT": "Field of View (lower is zoomed in, higher is wide angle)"
    },
    {
      "NAME": "CameraPanX",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -5,
      "MAX": 5,
      "LABEL": "Cam Pan X"
    },
    {
      "NAME": "CameraPanY",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -5,
      "MAX": 5,
      "LABEL": "Cam Pan Y"
    },
    {
      "NAME": "CameraPanZ",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -5,
      "MAX": 5,
      "LABEL": "Cam Pan Z"
    },
    {
      "NAME": "CameraLookAtSpeed",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": 0,
      "MAX": 0.1,
      "LABEL": "Cam LookAt Speed"
    },
    {
      "NAME": "CameraTilt",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1,
      "LABEL": "Camera Tilt"
    },
    {
      "NAME": "CameraRollSpeed",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -5,
      "MAX": 5,
      "LABEL": "Cam Roll Speed"
    },
    {
      "NAME": "ShakeIntensity",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 0.1,
      "LABEL": "Shake Intensity"
    },
    {
      "NAME": "ShakeFrequency",
      "TYPE": "float",
      "DEFAULT": 10,
      "MIN": 1,
      "MAX": 50,
      "LABEL": "Shake Frequency"
    },
    {
      "NAME": "Contrast",
      "TYPE": "float",
      "DEFAULT": 1.08,
      "MIN": 0.5,
      "MAX": 2,
      "LABEL": "Contrast"
    },
    {
      "NAME": "Saturation",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Saturation"
    },
    {
      "NAME": "Brightness",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Brightness"
    },
    {
      "NAME": "HueShift",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1,
      "LABEL": "Hue Shift"
    },
    {
      "NAME": "SaturationBoost",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Saturation Boost"
    },
    {
      "NAME": "VignetteStrength",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Vignette Strength"
    },
    {
      "NAME": "FinalGamma",
      "TYPE": "float",
      "DEFAULT": 0.47,
      "MIN": 0.1,
      "MAX": 2,
      "LABEL": "Final Gamma"
    }
  ],
  "PASSES": [
    {
      "FLOAT": true,
      "PERSISTENT": true,
      "TARGET": "BufferA"
    },
    {}
  ]
}*/

// Shader precision
#ifdef GL_ES
precision highp float;
#endif

#define TAU 6.28318530718

// Global time variable
float gTime;

// ISF Input Uniforms (automatically generated from JSON)
uniform float AnimationSpeed;
uniform float FractalScale;
uniform vec3 FractalOffset; // Correctly declared
uniform float MinRadiusSquared;
uniform float MandelboxIterations;
uniform float FractalFoldStrength;
uniform float PaletteSelector;
uniform float PaletteMix;
uniform float PaletteBlend; // Correctly declared
uniform float HuePulseSpeed;
uniform float HuePulseAmount;
uniform float SatPulseSpeed;
uniform float SatPulseAmount;
uniform float BriPulseSpeed;
uniform float BriPulseAmount;
uniform vec4 SurfaceColorA;
uniform vec4 SurfaceColorB;
uniform vec4 SurfaceColorC;
uniform vec4 GlowColor;
uniform vec4 FogColor;
uniform float LightingAmbient;
uniform float Light1DirectionX;
uniform float Light1DirectionY;
uniform float Light1DirectionZ;
uniform vec4 Light1Color;
uniform float Light1Intensity;
uniform vec4 Light2Color;
uniform float Light2Intensity;
uniform float RaymarchMaxSteps;
uniform float RaymarchMinHitDistance;
uniform float ShadowSteps;
uniform float GlowIntensity;
uniform float CameraOrbitSpeed;
uniform float CameraZoom;
uniform float CameraFOV; // Correctly declared
uniform float CameraPanX;
uniform float CameraPanY;
uniform float CameraPanZ;
uniform float CameraLookAtSpeed;
uniform float CameraTilt;
uniform float CameraRollSpeed;
uniform float ShakeIntensity;
uniform float ShakeFrequency;
uniform float Contrast;
uniform float Saturation;
uniform float Brightness;
uniform float HueShift;
uniform float SaturationBoost;
uniform float VignetteStrength;
uniform float FinalGamma;


//----------------------------------------------------------------------------------------
// HSV to RGB conversion
vec3 hsv2rgb(vec3 c)
{
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// RGB to HSV conversion
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Pseudo-random hash for noise
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
	vec4 p = vec4(pos - FractalOffset.xyz, 1); // Apply fractal offset
	vec4 p0 = p;
    
    float currentMinRad2 = clamp(MinRadiusSquared, 1.0e-9, 1.0);
    vec4 scaleVec = vec4(FractalScale, FractalScale, FractalScale, abs(FractalScale)) / currentMinRad2;
    float absScalem1 = abs(FractalScale - 1.0);
    
    int numIterations = int(MandelboxIterations);
    float AbsScaleRaisedTo1mIters = pow(abs(FractalScale), float(1-numIterations));

	for (int i = 0; i < numIterations; i++)
	{
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz; // Core fold
        p.xyz *= FractalFoldStrength; // Apply folding strength

		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(currentMinRad2/r2, currentMinRad2), 0.0, 1.0);

		p = p*scaleVec + p0;
	}
	return ((length(p.xyz) - absScalem1) / p.w - AbsScaleRaisedTo1mIters);
}

//----------------------------------------------------------------------------------------
// Psychedelic Color Palettes
vec3 getPalette(int type, float t) {
    if (type == 0) { // Classic DMT: vivid, high contrast, purples/greens
        return hsv2rgb(vec3(mod(t * 0.12, 1.0), 1.0, 1.0));
    } else if (type == 1) { // Tryptamine Flow: softer, pastels, fluid shifts
        return hsv2rgb(vec3(mod(t * 0.08 + 0.3, 1.0), 0.8, 0.9));
    } else if (type == 2) { // Electric Blue & Orange: sharp, energetic
        return hsv2rgb(vec3(mod(t * 0.15 + 0.6, 1.0), 0.95, 0.85));
    } else if (type == 3) { // Deep Cosmic: rich, dark, subtle shifts
        return hsv2rgb(vec3(mod(t * 0.06 + 0.1, 1.0), 0.9, 0.7));
    } else if (type == 4) { // Rainbow Warp: rapid, full spectrum
        return hsv2rgb(vec3(mod(t * 0.25, 1.0), 1.0, 1.0));
    } else { // type == 5, Golden Trails: warm, shimmering
        return hsv2rgb(vec3(mod(t * 0.09 + 0.8, 1.0), 0.7, 1.0));
    }
}

vec3 Colour(vec3 pos, float sphereR)
{
	vec3 p = pos;
	vec3 p0 = p;
	float trap = 1.0;
    
    int numIterations = int(MandelboxIterations);
	for (int i = 0; i < numIterations; i++) // Use tunable iterations
	{
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;
        p.xyz *= FractalFoldStrength; // Apply folding strength
		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(MinRadiusSquared/r2, MinRadiusSquared), 0.0, 1.0);
		p = p*vec3(FractalScale) + p0.xyz;
		trap = min(trap, r2);
	}

	vec2 c = clamp(vec2( 0.3333*log(dot(p,p))-1.0, sqrt(trap) ), 0.0, 1.0);

    float base_time_warp = length(pos) * 0.05 + gTime * 150.0;
    
    // Apply color pulse controls
    float hue_pulse = sin(gTime * HuePulseSpeed * TAU) * HuePulseAmount;
    float sat_pulse = sin(gTime * SatPulseSpeed * TAU + 1.0) * SatPulseAmount;
    float bri_pulse = sin(gTime * BriPulseSpeed * TAU + 2.0) * BriPulseAmount;

    // Determine current and next palette
    int currentPalette = int(floor(PaletteSelector));
    int nextPalette = int(min(5.0, floor(PaletteSelector) + 1.0)); // Ensure we don't go out of bounds for 6 palettes

    float palette_lerp_factor = PaletteMix; // Use PaletteMix for blending

    vec3 col_val1_current = getPalette(currentPalette, base_time_warp);
    vec3 col_val2_current = getPalette(currentPalette, base_time_warp + 10.0);
    vec3 col_val3_current = getPalette(currentPalette, base_time_warp + 20.0);

    vec3 col_val1_next = getPalette(nextPalette, base_time_warp);
    vec3 col_val2_next = getPalette(nextPalette, base_time_warp + 10.0);
    vec3 col_val3_next = getPalette(nextPalette, base_time_warp + 20.0);

    vec3 final_color1 = mix(col_val1_current, col_val1_next, palette_lerp_factor);
    vec3 final_color2 = mix(col_val2_current, col_val2_next, palette_lerp_factor);
    vec3 final_color3 = mix(col_val3_current, col_val3_next, palette_lerp_factor);

    // Mix with base surface colors using PaletteBlend (now correctly defined)
    vec3 mixed_color1 = mix(SurfaceColorA.rgb, final_color1, PaletteBlend);
    vec3 mixed_color2 = mix(SurfaceColorB.rgb, final_color2, PaletteBlend);
    vec3 mixed_color3 = mix(SurfaceColorC.rgb, final_color3, PaletteBlend);

    // Apply color pulse to the final mixed colors (as an offset in HSV space)
    mixed_color1 = hsv2rgb(vec3(mod(rgb2hsv(mixed_color1).x + hue_pulse, 1.0), clamp(rgb2hsv(mixed_color1).y + sat_pulse, 0.0, 1.0), clamp(rgb2hsv(mixed_color1).z + bri_pulse, 0.0, 1.0)));
    mixed_color2 = hsv2rgb(vec3(mod(rgb2hsv(mixed_color2).x + hue_pulse, 1.0), clamp(rgb2hsv(mixed_color2).y + sat_pulse, 0.0, 1.0), clamp(rgb2hsv(mixed_color2).z + bri_pulse, 0.0, 1.0)));
    mixed_color3 = hsv2rgb(vec3(mod(rgb2hsv(mixed_color3).x + hue_pulse, 1.0), clamp(rgb2hsv(mixed_color3).y + sat_pulse, 0.0, 1.0), clamp(rgb2hsv(mixed_color3).z + bri_pulse, 0.0, 1.0)));

	return mix(mix(mixed_color1, mixed_color2, c.y), mixed_color3, c.x);
}


//----------------------------------------------------------------------------------------
vec3 GetNormal(vec3 pos, float distance)
{
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
    for (int i = 0; i < 6; i++)
    {
        halfwayT = dot(t_range, vec2(.5));
        float d = Map(rO + halfwayT*rD);
        t_range = mix(vec2(t_range.x, halfwayT), vec2(halfwayT, t_range.y), step(RaymarchMinHitDistance, d));
    }
	return halfwayT;
}

//----------------------------------------------------------------------------------------
vec2 Scene(in vec3 rO, in vec3 rD)
{
    float t = .05;
	vec3 p = vec3(0.0);
    float oldT = 0.0;
    bool hit = false;
    float glow = 0.0;
    vec2 dist_range;

    int maxRaymarchSteps = int(RaymarchMaxSteps);
	for( int j=0; j < maxRaymarchSteps; j++ )
	{
		if (t > 12.0) break;
        p = rO + t*rD;
        
		float h = Map(p);
        
		if(h < RaymarchMinHitDistance)
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
        t = 1000.0;
    else
        t = BinarySubdivision(rO, rD, dist_range);
    return vec2(t, clamp(glow * GlowIntensity, 0.0, 1.0));
}

//----------------------------------------------------------------------------------------
// Hash for post-effects
float Hash(vec2 p)
{
	return fract(sin(dot(p, vec2(12.9898, 78.233))) * 33758.5453)-.5;
}

//----------------------------------------------------------------------------------------
vec3 PostEffects(vec3 rgb, vec2 xy)
{
	// Gamma correction first
	
	// Then contrast, saturation, brightness
	rgb = mix(vec3(.5), mix(vec3(dot(vec3(.2125, .7154, .0721), rgb*Brightness)), rgb*Brightness, Saturation), Contrast);
	
    // Hue Shift and Saturation Boost
    vec3 hsv = rgb2hsv(rgb);
    hsv.x = mod(hsv.x + HueShift, 1.0);
    hsv.y = clamp(hsv.y * SaturationBoost, 0.0, 1.0);
    rgb = hsv2rgb(hsv);

	// Vignette
	rgb *= .5 + 0.5*pow(20.0*xy.x*xy.y*(1.0-xy.x)*(1.0-xy.y), VignetteStrength);	

    // Final Gamma
    rgb = pow(rgb, vec3(FinalGamma));
	return rgb;
}


//----------------------------------------------------------------------------------------
float Shadow( in vec3 ro, in vec3 rd)
{
	float res = 1.0;
    float t = 0.05;
	float h;
	
    int shadowStepsCount = int(ShadowSteps);
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
    // Original path, now affected by CameraLookAtSpeed
    vec3 p = vec3(
        -.78 + 3. * sin(2.14 * (t + CameraLookAtSpeed)),
        .05 + 2.5 * sin(.942 * (t + CameraLookAtSpeed) + 1.3),
        .05 + 3.5 * cos(3.594 * (t + CameraLookAtSpeed))
    );
    
    // Apply CameraOrbitSpeed and CameraZoom
    float orbit_angle = t * CameraOrbitSpeed;
    float cos_orbit = cos(orbit_angle);
    float sin_orbit = sin(orbit_angle);
    
    // Rotate around a central point, then zoom
    p.xz = p.xz * mat2(cos_orbit, -sin_orbit, sin_orbit, cos_orbit);
    p *= CameraZoom;

    // Apply global camera pan
    p += vec3(CameraPanX, CameraPanY, CameraPanZ);

	return p;
}
    
//----------------------------------------------------------------------------------------
void main() {
    // Pass 0: Render the main scene to BufferA
	if (PASSINDEX == 0)	{

        gTime = (TIME * AnimationSpeed)*.01 + 15.00;
        
        // Screen shake effect applied to gl_FragCoord
        vec2 shake_offset = vec2(
            Noise(vec3(gTime * ShakeFrequency * 1.3, 0.0, 0.0)) - 0.5,
            Noise(vec3(gTime * ShakeFrequency * 1.7, 1.0, 0.0)) - 0.5
        ) * ShakeIntensity * RENDERSIZE.xy;
        
        vec2 xy = (gl_FragCoord.xy + shake_offset) / RENDERSIZE.xy;
		vec2 uv = (-1.0 + 2.0 * xy) * vec2(RENDERSIZE.x/RENDERSIZE.y, 1.0);
		
		vec3 cameraPos	= CameraPath(gTime);
        vec3 camTar		= CameraPath(gTime + CameraLookAtSpeed) + vec3(0.0, CameraTilt, 0.0);
	
		float roll = 13.0*sin(gTime*.5+.4) + gTime * CameraRollSpeed;
		vec3 cw = normalize(camTar-cameraPos);
	
		vec3 cp = vec3(sin(roll), cos(roll),0.0);
		vec3 cu = normalize(cross(cw,cp));
	
		vec3 cv = normalize(cross(cu,cw));
        cw = RotationMatrix(cv, sin(-gTime*20.0)*.7) * cw;
        
        // Adjust FOV (field of view) - CameraFOV is now correctly defined
		vec3 dir = normalize(uv.x*cu + uv.y*cv + CameraFOV*cw);
	
        vec3 spotLight = CameraPath(gTime + .03) + vec3(sin(gTime*18.4), cos(gTime*17.98), sin(gTime * 22.53))*.2;
		vec3 col = vec3(0.0);
        
        vec3 light1Dir = normalize(vec3(Light1DirectionX, Light1DirectionY, Light1DirectionZ));
        vec3 sky = FogColor.rgb * GetSky(dir);
		vec2 ret = Scene(cameraPos, dir);
        
        if (ret.x < 900.0)
        {
			vec3 p = cameraPos + ret.x*dir;
			vec3 nor = GetNormal(p, ret.x);
            
            vec3 light2Pos = spotLight - p; // Light 2 is the 'spotlight'
			float light2Atten = length(light2Pos);
	
            light2Pos /= light2Atten;
            
            float shadowLight2 = Shadow(p, light2Pos);
            float shadowLight1 = Shadow(p, light1Dir);
            
            float brightnessLight2 = max(dot(light2Pos, nor), 0.0) / pow(light2Atten, 1.5) * .25;
            float brightnessLight1 = max(dot(light1Dir, nor), 0.0) * .2;
            
            col = Colour(p, ret.x);
            
            // Apply ambient lighting
            col += LightingAmbient * col;

            // Apply light contributions with their respective colors and intensities
            col += (col * brightnessLight2 * shadowLight2 * Light2Intensity * Light2Color.rgb);
            col += (col * brightnessLight1 * shadowLight1 * Light1Intensity * Light1Color.rgb);
            
            vec3 ref = reflect(dir, nor);
            col += pow(max(dot(light2Pos,  ref), 0.0), 10.0) * 2.0 * shadowLight2 * brightnessLight2 * Light2Intensity * Light2Color.rgb;
            col += pow(max(dot(light1Dir, ref), 0.0), 10.0) * 2.0 * shadowLight1 * brightnessLight1 * Light1Intensity * Light1Color.rgb;
        }
        
        col = mix(sky, col, min(exp(-ret.x+1.5), 1.0));
        col += vec3(pow(abs(ret.y), 2.)) * GlowColor.rgb;
	
        col += LightSource(spotLight-cameraPos, dir, ret.x);
		col = PostEffects(col, xy);	
		
		gl_FragColor=vec4(col,1.0);
	}
    // Pass 1: Display BufferA (final output)
	else if (PASSINDEX == 1)	{
        gl_FragColor = IMG_NORM_PIXEL(BufferA, gl_FragCoord.xy / RENDERSIZE.xy);
	}
}