/*
{
    "CATEGORIES": [
        "Animated",
        "Fractal",
        "Tunable",
        "Procedural",
        "Psychedelic"
    ],
    "DESCRIPTION": "A fractal scene with tunable parameters for animation, camera, folding effects, glow, and post-processing, now including psychedelic color palettes and a moving color pulse. Loop error fixed.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "iMouse", "TYPE": "point2D", "DEFAULT": [0.0, 0.0], "LABEL": "Mouse Input" },
        { "NAME": "GlobalAnimationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Overall Animation Speed" },
        { "NAME": "FoldWaveStrength", "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0, "MAX": 0.01, "LABEL": "Fold Z Wave Strength" },
        { "NAME": "FoldScalingFactor", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "Fold Repetition Scale" },
        { "NAME": "FoldTranslateZ", "TYPE": "float", "DEFAULT": 1.275, "MIN": 0.5, "MAX": 3.0, "LABEL": "Fold Translation Z" },
        { "NAME": "BaseFOV", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0, "LABEL": "Base Field of View" },
        { "NAME": "FovAnimationStrength", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.2, "LABEL": "FOV Oscillation Strength" },
        { "NAME": "MapWaveFrequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0, "LABEL": "Map Surface Wave Freq" },
        { "NAME": "MapWaveStrength", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0, "MAX": 0.2, "LABEL": "Map Surface Wave Strength" },
        { "NAME": "RaymarchSteps", "TYPE": "float", "DEFAULT": 200.0, "MIN": 50.0, "MAX": 500.0, "LABEL": "Raymarch Iterations" },
        { "NAME": "CameraZoomBase", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 15.0, "LABEL": "Camera Base Zoom" },
        { "NAME": "CameraZoomWaveStrength", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Zoom Wave Strength" },
        { "NAME": "CameraRotationSpeedBase", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Base Rot Speed" },
        { "NAME": "CameraRotationWaveStrength1", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Rot Wave 1 Strength" },
        { "NAME": "CameraRotationWaveStrength2", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0, "LABEL": "Camera Rot Wave 2 Strength" },
        { "NAME": "GlowFalloff", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 50.0, "LABEL": "Glow Falloff Sharpness" },
        { "NAME": "GlowIntensityMain", "TYPE": "float", "DEFAULT": 0.095, "MIN": 0.0, "MAX": 0.5, "LABEL": "Main Glow Intensity" },
        { "NAME": "RefractionBaseIndex", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0, "LABEL": "Refraction Base Index" },
        { "NAME": "RefractionYInfluence", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.5, "LABEL": "Refraction Y Influence" },
        { "NAME": "RefractionXInfluence", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 0.5, "LABEL": "Refraction X Influence" },
        { "NAME": "PostExposureOffset", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 15.0, "LABEL": "Post-Exposure Offset" },
        { "NAME": "PostExposureScale", "TYPE": "float", "DEFAULT": 0.14, "MIN": 0.0, "MAX": 0.5, "LABEL": "Post-Exposure Scale" },
        { "NAME": "OutputGamma", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 2.0, "LABEL": "Output Gamma Correction" },
        { "NAME": "VignettePower", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Vignette Power" },
        { "NAME": "VignetteInnerFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "LABEL": "Vignette Inner Factor" },
        { "NAME": "VignetteOuterFactor", "TYPE": "float", "DEFAULT": 0.37, "MIN": 0.0, "MAX": 1.0, "LABEL": "Vignette Outer Factor" },
        { "NAME": "DiffuseMaterialColorFactor", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Material 1 Diffuse Mix" },
        { "NAME": "SpecularMaterialColorFactor", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Material 1 Specular Mix" },
        { "NAME": "DiffuseSecondMaterialFactor", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0, "MAX": 0.01, "LABEL": "Material 2 Diffuse Factor" },
        { "NAME": "SpecularSecondMaterialFactor", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.0, "MAX": 0.01, "LABEL": "Material 2 Specular Factor" },
        { "NAME": "AttenuationBaseR", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Attenuation Red" },
        { "NAME": "AttenuationBaseG", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Attenuation Green" },
        { "NAME": "AttenuationBaseB", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.0, "LABEL": "Attenuation Blue" },
        { "NAME": "AttenuationMultiply", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Attenuation Multiplier" },
        { "NAME": "SecondMaterialMixFactor", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Material 2 Color Mix" },
        { "NAME": "PostMultiply1", "TYPE": "float", "DEFAULT": 1.9, "MIN": 0.5, "MAX": 5.0, "LABEL": "Post-Process Multiply 1" },
        { "NAME": "PostMultiply2", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 5.0, "LABEL": "Post-Process Multiply 2" },

        
        { "NAME": "PaletteHueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Hue Shift" },
        { "NAME": "PaletteSaturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Saturation" },
        { "NAME": "PaletteValue", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Value" },
        { "NAME": "ColorPulseFrequency", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 20.0, "LABEL": "Color Pulse Freq" },
        { "NAME": "ColorPulseAmplitude", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse Amp" },
        { "NAME": "ColorSpatialPulseFactor", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Spatial Pulse" }
    ]
}
*/

// No explicit uniform declarations here.
// The environment automatically creates uniforms from the INPUTS JSON.
// Use TIME directly (provided by the environment)
// Apply GlobalAnimationSpeed where TIME is used in calculations.
#define rot(x) mat2(cos(x), -sin(x), sin(x), cos(x))
#define PI acos(-1.)
#define pi acos(-1.)
#define dmin(a, b) a.x < b.x ? a : b
#define fov (BaseFOV + sin(TIME * GlobalAnimationSpeed)*FovAnimationStrength)

// Helper to convert HSV to RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Function to generate psychedelic color based on time, distance, and palette parameters
vec3 getPsychedelicColor(float t_val, float palette_hue, float palette_sat, float palette_val) {
    // Basic sinusoidal color generation
    vec3 base_color = 0.5 + 0.5 * sin(t_val * vec3(2.9, 5.0, 1.2) + 2.6);
    
    // Convert to HSV, shift hue, apply saturation and value, then convert back to RGB
    // This allows the base color to be modified by the palette controls
    vec3 hsv = vec3(
        mod(palette_hue + base_color.r * 0.2, 1.0), // Apply palette hue shift
        palette_sat * base_color.g, // Apply palette saturation
        palette_val * base_color.b // Apply palette value
    );
    return hsv2rgb(hsv);
}


// thx nusan for refractions
// and knighty/lsdlive/tdhooper and others for folding

vec3 fold(vec3 p) {
	// Tunable Z offset and wave strength
	vec3 nc = vec3(-.5, -.809017, .309017 + sin(TIME * GlobalAnimationSpeed)*FoldWaveStrength);
	for (int i = 0; i < 5; i++) {
		p.xy = abs(p.xy);
		// Tunable scaling factor
		p -= FoldScalingFactor * min(0., dot(p, nc))*nc;
	}
	// Tunable Z translation
	return p - vec3(0, 0, FoldTranslateZ);
}


vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
	vec3 dir = normalize(lookAt - ro);
 	vec3 right = normalize(cross(vec3(0,1,0), dir));
 	vec3 up = normalize(cross(dir, right));
 	return dir + right*uv.x*fov + up*uv.y*fov;
}

vec2 map(vec3 p){
	vec2 d = vec2(10e5);
 	
 	
 	vec3 q = p;
 	p = fold(p);
 	vec3 z = p;
 	float b = pow((1. + sin(p.x*3.)), 0.7) - 1.;
 	vec3 l = p;
 	l.xz *= rot(sin(TIME * GlobalAnimationSpeed * 0.7)*0.4 - 0.2);
 	b = min(b, sin(l.x*3.)*3.);
 	
	// Tunable map offsets
 	z.z += 0.2;
 	z.x += 0.82;
	
	// Tunable mix factor for the first material
 	d = dmin(d, vec2(mix(dot(z - 0.6, vec3(0.1,0.1,0.2)), b, 0.1),1.));
	
 	q = fold(q) - vec3(0.5,0.6,-0.2);
 	q = abs(q);
	// Tunable Z offset for second material
 	q.z -= 0.2;
	// Second material properties - tunable factor
 	d = dmin(d, vec2( dot(q, normalize(vec3(1))),2.));
 	
	// Tunable offsets for fractal iteration
 	p.x -= 0.2;
 	p.y += 0.7;
 	
 	p = fold(p) - vec3(0.4,0.4,20.);
 	p = fold(p) - vec3(0.,0.4,20.);
 	p = fold(p) - vec3(0.,0.,1.);
 	
 	for (int i = 0;i < 2; i++){
 		p = abs(p);
 		p.x -= 0.2;
 	}
 	
	// Tunable offsets
 	p -= 7.;
 	
 	p = fold(p);
 	
 	p -= 0.6;
 	d = dmin(d, vec2( max(p.x, p.y), 1. ));
 	p.xy *= rot(2.5);
 	
	// Tunable map distortion
 	d.x *= 1.;
 	d.x += sin(p.x * MapWaveFrequency) * MapWaveStrength;
 	return d;
}
 	
 	
vec3 getNormal(vec3 p){
 	vec2 t = vec2(0.001,0.);
 	return normalize(map(p).x - vec3(
 		map(p - t.xyy).x,
 		map(p - t.yxy).x,
 		map(p - t.yyx).x
 	));
}


// Tunable camera zoom and rotation speed
#define zoom (CameraZoomBase + sin(TIME * GlobalAnimationSpeed)*CameraZoomWaveStrength)
#define rotSpeed (TIME * GlobalAnimationSpeed * CameraRotationSpeedBase + cos(TIME * GlobalAnimationSpeed * 0.7)*CameraRotationWaveStrength1 - sin(TIME * GlobalAnimationSpeed * 0.5)*CameraRotationWaveStrength2)

// Pal macro used for glow, now uses the general psychedelic color function
#define pal(x, t_unused) getPsychedelicColor(x + TIME * GlobalAnimationSpeed, PaletteHueShift, PaletteSaturation, PaletteValue)

vec3 glow = vec3(0);
#define mx (20.*iMouse.x/RENDERSIZE.x)

void main() {

 	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
 	vec3 col = vec3(0);
 	vec3 ro = vec3(sin(rotSpeed + mx)*zoom,0. + sin(TIME * GlobalAnimationSpeed),cos(rotSpeed + mx)*zoom);
 	vec3 lookAt = vec3(0);
 	vec3 rd = getRd(ro, lookAt, uv);
 	
 	vec3 p = ro; float t, tBeforeRefraction = 0.;
 	
 	float bounce = 0.;
 	vec3 attenuation = vec3(1);
 	float side = 1.;

    // Fix for "Loop index cannot be compared with non-constant expression":
    // Use a fixed integer for loop bounds, and use RaymarchSteps as a soft limit.
 	const int MAX_RAYMARCH_STEPS = 250; 
 	for(int i = 0; i < MAX_RAYMARCH_STEPS ; i ++){
        float current_i_float = float(i); // Convert loop index to float for calculations
        if (current_i_float > RaymarchSteps) break; // Use RaymarchSteps as a soft limit

 		vec2 d = map(p);
 	 	d.x *= side;
		// Tunable glow parameters
		glow += pal(t*0.2 + exp(-d.x*GlowFalloff) + 2., 2.6)* GlowIntensityMain / (.01 + d.x*d.x);
 	 	
 	 	if (d.x < 0.002){
 	 		vec3 lDir = normalize(vec3(1));
 	 		vec3 n = getNormal(p)*side;
 	 		vec3 h = normalize(lDir - rd);
 	 		
 	 		float diff = max(dot(lDir,n),0.);
 	 		float fres = pow(1. - max(dot(-rd,n),0.), 20.);
 	 		float spec = pow(max(dot(h,n),0.), 10.);

            // Get a psychedelic color based on iteration, distance, and time
            vec3 material_psy_color = getPsychedelicColor(
                current_i_float * 0.1 + t * 0.05, // Varies with iteration and ray distance
                PaletteHueShift, PaletteSaturation, PaletteValue
            );

 	 		if (d.y == 2.) {
				// Tunable material colors, now blended with psychedelic palette
 	 			col += mix( (sin(vec3(2.9,2.9,2.6)*glow*DiffuseSecondMaterialFactor) + 7.)*DiffuseMaterialColorFactor *diff ,vec3(SpecularSecondMaterialFactor*glow*min(fres*spec*diff, 1.)), SpecularMaterialColorFactor)*attenuation * material_psy_color;
 	 			break;
 	 		} else {
				// Tunable material colors, now blended with psychedelic palette
 	 			col += mix( DiffuseSecondMaterialFactor*glow *diff ,vec3(SpecularSecondMaterialFactor*glow*min(fres*spec, 1.)), SecondMaterialMixFactor)*attenuation * material_psy_color;
				// Tunable attenuation
 	 			attenuation *= vec3(AttenuationBaseR, AttenuationBaseG, AttenuationBaseB)*AttenuationMultiply;
 	 			ro = p;
				// Tunable refraction properties
 	 			rd = refract(rd, n, RefractionBaseIndex + n.y*RefractionYInfluence + n.x*RefractionXInfluence);
 	 			side = -side;
 	 			bounce++;
 	 			if(bounce == 1.) {
 	 				tBeforeRefraction = t;
 	 			}
 	 			t = 0.;
 	 			d.x = 0.2;
 	 		}
 	 		
 	 		
 	 		//break;
 	 	}
 	 	if (t > 50. || i == MAX_RAYMARCH_STEPS - 1) { // Break if max steps reached or distance too high
 	 		if (bounce == 0.) {
				tBeforeRefraction = t;
 	 		}
 	 		break;
 	 	}
 	 	t += d.x;
 	 	p = ro + rd*t;
 	}
 	
	// Tunable post-exposure
 	col -= exp(-PostExposureOffset + tBeforeRefraction*PostExposureScale);
 	
	// Tunable post-processing multipliers
 	col *= PostMultiply1;
 	col = clamp(col, 0., 1.);
 	
	// Tunable background color and smoothstep
 	col = mix(col, vec3(0.02,0.03,0.05),smoothstep(0.,1.,0.2 + tBeforeRefraction*0.01));
	// Tunable output gamma
 	col = pow(col, vec3(OutputGamma));
 	
	// Tunable post-processing multiplier
 	col *= PostMultiply2;
	// Tunable vignette
 	col *= 1. - pow(length(uv)*VignetteInnerFactor + VignetteOuterFactor, VignettePower);
    
    // Apply moving color pulse to the final output
    float pulse_value = (1.0 + ColorPulseAmplitude * sin(TIME * ColorPulseFrequency + p.x * ColorSpatialPulseFactor));
    col *= pulse_value;

 	gl_FragColor = vec4(col,tBeforeRefraction);
}
