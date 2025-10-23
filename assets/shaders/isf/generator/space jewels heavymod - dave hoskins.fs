/*{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic"
    ],
    "DESCRIPTION": "An advanced psychedelic fractal explorer with extensive customizable parameters for zoom, speed, camera, multiple color palettes, color pulsing, and detailed fractal morphing including Julia set variations and other transformations.",
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
            "MIN": -20.0,
            "MAX": 20.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -20.0,
            "MAX": 20.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -20.0,
            "MAX": 20.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetX",
            "TYPE": "float",
            "DEFAULT": -12.0,
            "MIN": -30.0,
            "MAX": 30.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -30.0,
            "MAX": 30.0,
            "STEP": 0.1
        },
        {
            "NAME": "cameraTargetZ",
            "TYPE": "float",
            "DEFAULT": -2.0,
            "MIN": -30.0,
            "MAX": 30.0,
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
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of color pulsing"
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
            "NAME": "fractalType",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects fractal type (0: Default, 1: Julia, 2: Menger)"
        },
        {
            "NAME": "juliaCX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Julia constant X (if fractalType is Julia)"
        },
        {
            "NAME": "juliaCY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Julia constant Y (if fractalType is Julia)"
        },
        {
            "NAME": "juliaCZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Julia constant Z (if fractalType is Julia)"
        },
        {
            "NAME": "mengerScale",
            "TYPE": "float",
            "DEFAULT": 3.0,
            "MIN": 1.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Menger sponge scale (if fractalType is Menger)"
        },
        {
            "NAME": "morphFactor",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "General fractal morph factor (blends between original and modified/secondary fractal)"
        }
    ]
}
*/


// Space Jewels. December 2014
// by David Hoskins
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// https://www.shadertoy.com/view/llX3zr

// Modified for ISF with new parameters by Google Gemini

//--------------------------------------------------------------------------
#define SUN_COLOUR vec3(1., .9, .85)
#define FOG_COLOUR vec3(0.07, 0.05, 0.05)
#define MOD2 vec2(443.8975,397.2973)

vec3 CSize;
vec4 aStack[2];
vec4 dStack[2];
vec2 fcoord;

//----------------------------------------------------------------------------------------
// From https://www.shadertoy.com/view/4djSRW
float Hash(vec2 p)
{
	p  = fract(p * MOD2);
    p += dot(p.xy, p.yx+19.19);
    return fract(p.x * p.y);
}

//----------------------------------------------------------------------------------------
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
vec3 Colour( vec3 p, float paletteIndex, float pulseSpeed)
{
	float col	= 0.0;
    float r2	= dot(p,p);
		
	for( int i=0; i < 12;i++ )
	{
		vec3 p1= 2.0 * clamp(p, -CSize, CSize)-p;
		col += abs(p.z-p1.z);
		p = p1;
		r2 = dot(p,p);
		float k = max((1.1)/(r2), .03);
		p *= k;
	}
    // Apply color pulsing
    float pulse = 0.5 + 0.5 * sin(TIME * pulseSpeed);
    return getPaletteColor(col, paletteIndex) * pulse;
}

//--------------------------------------------------------------------------
// Distance function for a Menger Sponge (simplified)
// Source: https://www.shadertoy.com/view/MdSyzW by iq
float sdMengerSponge( vec3 p, float scale )
{
    float d = abs(p.x);
    d = max( d, abs(p.y) );
    d = max( d, abs(p.z) );

    float s = 1.0;
    for( int i=0; i<4; i++ ) // Fewer iterations for performance
    {
        p = abs(p);
        if( p.x < p.y ) p.xy = p.yx;
        if( p.x < p.z ) p.xz = p.zx;
        if( p.y < p.z ) p.yz = p.zy;

        p = scale*p - (scale-1.0);
        
        s *= scale;
        d = scale * d - (scale-1.0);
    }
    return d/s;
}

//--------------------------------------------------------------------------
// Function for Julia iteration. Moved before Map.
// This implements a general 3D Julia iteration: z = z^power + C.
// Here, we use z = abs(z)*k + C as a simplified approach.
vec3 juliaIterate(vec3 z, vec3 c, float k) {
    // This is a simplified iteration for 3D Julia-like fractals.
    // For true quaternion Julia, complex math operations would be needed.
    // Here, we'll use a reflection/folding operation combined with a scale.
    z = abs(z) * k; // Apply a scaling factor to the absolute value
    return z + c; // Add the constant
}


//--------------------------------------------------------------------------

float Map( vec3 p, float fractalType, vec3 juliaC, float mengerS, float morphFactor )
{
    float d = 1e10; // Initialize with a very large distance

    // --- Original Fractal (Space Jewels) ---
    float originalFractalScale = 1.0;
    vec3 p_original = p;
	for( int i=0; i < 12;i++ )
	{
		p_original = 2.0*clamp(p_original, -CSize, CSize) - p_original;
		float r2 = dot(p_original,p_original);
		float k = max((1.1)/(r2), .03);
		p_original     *= k;
		originalFractalScale *= k;
	}
	float l_original = length(p_original.xy);
	float rxy_original = l_original - 4.0;
	float n_original = l_original * p_original.z;
	rxy_original = max(rxy_original, -(n_original) / (length(p_original))-.1);
	float distOriginal = (rxy_original) / abs(originalFractalScale);
    d = distOriginal; // Default distance is the original fractal


    // --- Julia Set (simplified 3D iteration) ---
    if (fractalType > 0.5 && fractalType < 1.5) { // If fractalType is Julia (1.0)
        vec3 p_julia = p;
        float r2_julia;
        float distJulia = 0.0;
        int maxIter = 10;
        float bailout = 4.0; 
        float fold_k = 1.1; // A constant for the Julia iteration's folding/scaling

        for(int i = 0; i < maxIter; i++) {
            p_julia = juliaIterate(p_julia, juliaC, fold_k); // Use the new juliaIterate function
            r2_julia = dot(p_julia, p_julia);
            if (r2_julia > bailout) {
                distJulia = 0.5 * length(p_julia) * log(r2_julia) / length(p_julia); // Simple distance estimate
                break;
            }
        }
        d = mix(d, distJulia, morphFactor); // Blend with Julia
    }
    
    // --- Menger Sponge ---
    if (fractalType > 1.5) { // If fractalType is Menger (2.0)
        float distMenger = sdMengerSponge(p, mengerS);
        d = mix(d, distMenger, morphFactor); // Blend with Menger
    }

    return d;
}


//--------------------------------------------------------------------------
float Shadow( in vec3 ro, in vec3 rd, float fractalType, vec3 juliaC, float mengerS, float morphFactor)
{
	float res = 1.0;
    float t = 0.05;
	float h;
	
    for (int i = 0; i < 6; i++)
	{
		h = Map( ro + rd*t, fractalType, juliaC, mengerS, morphFactor );
		res = min(7.0*h / t, res);
		t += h+.01;
	}
    return max(res, 0.0);
}

//--------------------------------------------------------------------------
vec3 DoLighting(in vec3 mat, in vec3 pos, in vec3 normal, in vec3 eyeDir, in float d, float fractalType, vec3 juliaC, float mengerS, float morphFactor)
{
    vec3 sunLight  = normalize( vec3(  0.5, 0.2,  0.3 ) );
	float sh = Shadow(pos,  sunLight, fractalType, juliaC, mengerS, morphFactor);
    // Light surface with 'sun'...
	vec3 col = mat * SUN_COLOUR*(max(dot(sunLight,normal), 0.0)) *sh;
    //col += mat * vec3(0.1, .0, .0)*(max(dot(-sunLight,normal), 0.0));
    
    normal = reflect(eyeDir, normal); // Specular...
    col += pow(max(dot(sunLight, normal), 0.0), 25.0)  * SUN_COLOUR * 1.5 *sh;
    // Abmient..
    col += mat * .2 * max(normal.z, 0.0);
    col = mix(FOG_COLOUR,col, min(exp(-d*d*.05), 1.0));
    
	return col;
}


//--------------------------------------------------------------------------
vec3 GetNormal(vec3 p, float sphereR, float fractalType, vec3 juliaC, float mengerS, float morphFactor)
{
	vec2 eps = vec2(sphereR*.5, 0.0);
	return normalize( vec3(
           Map(p+eps.xyy, fractalType, juliaC, mengerS, morphFactor) - Map(p-eps.xyy, fractalType, juliaC, mengerS, morphFactor),
           Map(p+eps.yxy, fractalType, juliaC, mengerS, morphFactor) - Map(p-eps.yxy, fractalType, juliaC, mengerS, morphFactor),
           Map(p+eps.yyx, fractalType, juliaC, mengerS, morphFactor) - Map(p-eps.yyx, fractalType, juliaC, mengerS, morphFactor) ) );
}

//--------------------------------------------------------------------------
float SphereRadius(in float t)
{
	if (t< 1.4) t= (1.4-t) * 4.5;
	t = t*0.04;
	return max(t*t, 16.0/RENDERSIZE.x);
}

//--------------------------------------------------------------------------
float Scene(in vec3 rO, in vec3 rD, float fractalType, vec3 juliaC, float mengerS, float morphFactor)
{
    //float t = 0.0;
	float t = .1 * Hash(fcoord*fract(TIME));
	float  alphaAcc = 0.0;
	vec3 p = vec3(0.0);
    int hits = 0;

	for( int j=0; j < 80; j++ )
	{
		if (hits == 8 || alphaAcc >= 1.0 || t > 10.0) break;
		p = rO + t*rD;
		float sphereR = SphereRadius(t);
		float h = Map(p, fractalType, juliaC, mengerS, morphFactor);
        // Is it within the sphere?...
		if( h < sphereR)
		{
			// Accumulate the alphas with the scoop of geometry from the sphere...
            // Think of it as an expanding ice-cream scoop flying out of the camera! 
			float alpha = (1.0 - alphaAcc) * min(((sphereR-h) / sphereR), 1.0);
			// put it on the 2 stacks, alpha and distance...
			aStack[1].yzw = aStack[1].xyz; aStack[1].x = aStack[0].w;
			aStack[0].yzw = aStack[0].xyz; aStack[0].x = alpha;
			dStack[1].yzw = dStack[1].xyz; dStack[1].x = dStack[0].w;
			dStack[0].yzw = dStack[0].xyz; dStack[0].x = t;
			alphaAcc += alpha;
			hits++;
		}
		t +=  h*.85+t*.001;
        
	}
	
	return clamp(alphaAcc, 0.0, 1.0);
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

//--------------------------------------------------------------------------
vec3 Albedo(vec3 pos, vec3 nor, float paletteIndex, float pulseSpeed)
{
    vec3 baseColor = vec3(0.5 + 0.5 * sin(pos.x * 0.1 + pos.y * 0.2 + pos.z * 0.3 + TIME * 0.5)); // More dynamic procedural color
    
    // Multiply by the Colour function which now handles palette and pulsing
    baseColor *= Colour(pos, paletteIndex, pulseSpeed);
    return baseColor;
}


//--------------------------------------------------------------------------
vec3 CameraPath( float t, float zoomFactor, float camX, float camY, float camZ )
{
    // Apply zoom to the animated path
    vec3 p = vec3(-13.0 +3.4 * sin(t),-0.+4.5 * cos(t),-1.1+.3 * sin(2.3*t+2.0) ) * zoomFactor;
    // Add manual camera control offsets
    p.x += camX;
    p.y += camY;
    p.z += camZ;
	return p;
} 
    

//--------------------------------------------------------------------------
void main() {

    fcoord = gl_FragCoord.xy;
    
	float gTime = (TIME * speed + 26.0) * 0.2;
    vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 uv = (-1.0 + 2.0 * xy) * vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    
    // Animate CSize with `morphFactor` (now more general)
    CSize = vec3(.808, mix(.99-sin((gTime+35.0)*.5)*.3, 0.5 + 0.2*sin(TIME*0.7), morphFactor), mix(1.151-sin((gTime+16.0)*.78)*.3, 0.5 + 0.2*cos(TIME*0.9), morphFactor));
	
	vec3 cameraPos 	= CameraPath(gTime + 0.0, zoom, cameraX, cameraY, cameraZ);
	vec3 camTarget 	= vec3 (cameraTargetX, cameraTargetY, cameraTargetZ);
	vec3 cw = normalize(camTarget-cameraPos);
	vec3 cp = vec3(0.0, 0.0,1.0);
	vec3 cu = normalize(cross(cw,cp));
	vec3 cv = cross(cu,cw);
	vec3 dir = normalize(uv.x*cu + uv.y*cv + 1.1*cw);
	vec3 col = vec3(.0);
	
    // Julia Constant
    vec3 juliaConst = vec3(juliaCX, juliaCY, juliaCZ);

    for (int i = 0; i <2; i++)
    {
		dStack[i] = vec4(-20.0);
    }
	float alpha = Scene(cameraPos, dir, fractalType, juliaConst, mengerScale, morphFactor);
	
    
    // Render both stacks...
    for (int s = 0; s < 2; s++)
    {
        for (int i = 0; i < 4; i++)
        {
            float d = dStack[s][i];
            if (d < 0.0) continue;
            float sphereR = SphereRadius(d);
            vec3 pos = cameraPos + dir * d;
            vec3 normal = GetNormal(pos, sphereR, fractalType, juliaConst, mengerScale, morphFactor);
            vec3 alb = Albedo(pos, normal, paletteSelect, colorPulseSpeed);
            col += DoLighting(alb, pos, normal, dir, d, fractalType, juliaConst, mengerScale, morphFactor)* aStack[s][i];
        }
    }
    // Fill in the rest with fog...
   col += FOG_COLOUR * (1.0-alpha);
   
   
	col = PostEffects(col, xy, brightness, contrast, saturation) * smoothstep(.0, 2.0, TIME);	
	
	gl_FragColor=vec4(col,1.0);
}

//--------------------------------------------------------------------------