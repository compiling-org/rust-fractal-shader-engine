/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic"
    ],
    "DESCRIPTION": "A mesmerizing organic fractal tunnel, based on Kali's shader, dedicated to Amy. Enhanced with extensive psychedelic tunable parameters for speed, zoom, morphing, fractal control, geometry, tunnel size and warping, and multiple psychedelic color palettes.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "ZoomFactor", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Zoom Factor" },
        { "NAME": "WarpScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Warp Scale" },
        { "NAME": "WarpIntensity", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "Warp Intensity" },
        { "NAME": "WarpOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Warp Offset" },
        { "NAME": "FractalAngleSpeed", "TYPE": "float", "DEFAULT": 0.51, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Angle Speed" },
        { "NAME": "FractalOffsetSpeed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Fractal Offset Speed" },
        { "NAME": "FractalClampMin", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 1.0, "LABEL": "Fractal Clamp Min" },
        { "NAME": "FractalClampMax", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.0, "LABEL": "Fractal Clamp Max" },
        { "NAME": "FractalSubtract", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Subtract" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 12.0, "MIN": 1.0, "MAX": 15.0, "LABEL": "Fractal Iterations" },
        { "NAME": "FractalMultiply", "TYPE": "float", "DEFAULT": 4.5, "MIN": 0.0, "MAX": 50.0, "LABEL": "Fractal Multiply" },
        { "NAME": "FractalGlowStrength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 20.0, "LABEL": "Fractal Glow Str" },
        { "NAME": "FractalGlowDecay", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.01, "MAX": 5.0, "LABEL": "Fractal Glow Decay" },
        { "NAME": "DistanceGlowStrength", "TYPE": "float", "DEFAULT": 1.9, "MIN": 0.0, "MAX": 5.0, "LABEL": "Distance Glow Str" },
        { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0, "LABEL": "Chaos Intensity" },
        { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0, "LABEL": "Chaos Speed" },
        { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.48, "MIN": 0.0, "MAX": 1.0, "LABEL": "Chaos Mix" },
        { "NAME": "ColorPulseEnabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Color Pulse On/Off", "ANNOTATIONS": { "0.0": "Off", "1.0": "On" } },
        { "NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool", "3.0": "Vibrant Neon", "4.0": "Deep Space", "5.0": "Dreamy Pastel" } },
        { "NAME": "PaletteMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Mix Factor" },
        { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0, "LABEL": "Contrast" },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 3.0, "LABEL": "Brightness" },
        { "NAME": "Glow", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0, "LABEL": "Glow" },
        { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 0.70, "MIN": 0.0, "MAX": 2.0, "LABEL": "Transform Mode", "ANNOTATIONS": { "0.0": "Normal", "1.0": "Abs", "2.0": "Swirl" } },
        { "NAME": "Texture", "TYPE": "image" },
        { "NAME": "TextureWarp", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Texture Warp" },
        { "NAME": "TextureScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Texture Scale" }
    ]
}
*/

#ifdef GL_ES
precision highp float;
#endif

#ifndef PI
#define PI 3.14159265359
#endif

#ifndef TAU
#define TAU (2.0 * PI)
#endif

const int MAX_FRACTAL_ITERATIONS = 15;

mat2 rot2D(float a) {
	float s = sin(a);
	float c = cos(a);
	return mat2(c, s, -s, c);
}

vec3 hsv2rgb_smooth( in vec3 c )
{
	vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
	rgb = rgb*rgb*(3.0-2.0*rgb);
	return c.z * mix( vec3(1.0), rgb, c.y);
}

vec3 getPaletteColor(float h_val, float palette_type) {
	vec3 color_base;
	h_val = mod(h_val, 1.0);

	if (palette_type < 0.5) { // Palette 0: Classic Psychedelic (sin waves)
		color_base = clamp(abs(sin(h_val * TAU + vec3(0.0, 2.0, 4.0))), 0.0, 1.0);
	} else if (palette_type < 1.5) { // Palette 1: Warm Hues (Reds, Oranges, Yellows)
		h_val = mod(h_val, 1.0) * 0.3 + 0.0;
		color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
	} else if (palette_type < 2.5) { // Palette 2: Cool Tones (Blues, Purples, Greens)
		h_val = mod(h_val, 1.0) * 0.3 + 0.6;
		color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
	} else if (palette_type < 3.5) { // Palette 3: Vibrant Neon
		color_base = hsv2rgb_smooth(vec3(h_val, 0.9, 0.8));
	} else if (palette_type < 4.5) { // Palette 4: Deep Space (Darker blues/purples with bright accents)
		color_base = hsv2rgb_smooth(vec3(mod(h_val * 0.5 + 0.6, 1.0), 0.7, 0.3));
		color_base = mix(color_base, hsv2rgb_smooth(vec3(mod(h_val * 1.5 + 0.1, 1.0), 0.8, 0.9)), 0.2);
	} else { // Palette 5: Dreamy Pastel
		color_base = hsv2rgb_smooth(vec3(h_val, 0.3, 0.7));
	}
	return color_base;
}

vec3 fractal(vec2 p, float t_scaled) {
	// The original shader's warping
	p *= (ZoomFactor * 0.35) + sin(t_scaled * WarpScale) * WarpIntensity * 0.15;
	p += WarpOffset;

	// Add texture warp to the initial position
	vec2 tex_warp = texture2D(Texture, p * TextureScale).rg * 2.0 - 1.0;
	p += tex_warp * TextureWarp;

	// Use FractalOffsetSpeed to add a subtle, continuous drift
	p += vec2(t_scaled * FractalOffsetSpeed, t_scaled * FractalOffsetSpeed * 0.5);

	// Implement the new TransformMode
	if (TransformMode > 0.5 && TransformMode < 1.5) { // Mode 1: Abs
		p = abs(p);
	} else if (TransformMode > 1.5) { // Mode 2: Swirl
		float a = atan(p.y, p.x);
		float r = length(p);
		a += t_scaled * 0.2;
		p.x = cos(a) * r;
		p.y = sin(a) * r;
	}

    // The chaos effect that was already present
	vec2 chaos_offset = sin(p * 5.0 + t_scaled * ChaosSpeed) * ChaosIntensity;
	p = mix(p, p + chaos_offset, ChaosMix);

	vec2 m2 = vec2(1000.);
	float m1 = 1000.;

	for (int i = 0; i < MAX_FRACTAL_ITERATIONS; i++) {
		if (i >= int(FractalIterations)) {
			break;
		}

		float p_len_sq = dot(p,p);
		if (p_len_sq < 0.000001) p_len_sq = 0.000001;
		p = abs(p) / p_len_sq - FractalSubtract;

		p *= rot2D(-t_scaled * FractalAngleSpeed);
		
		m1 = min(m1, dot(p,p));
		m2 = min(m2, abs(p));
	}

	float glow_m1 = exp(-FractalGlowDecay * m1);
	vec2 glow_m2 = exp(-FractalGlowDecay * m2);

	// Clamp the glow values directly, this is a more subtle way to control the effect.
    glow_m1 = clamp(glow_m1, FractalClampMin, FractalClampMax);
    glow_m2 = clamp(glow_m2, vec2(FractalClampMin), vec2(FractalClampMax));

	glow_m1 *= FractalGlowStrength;
	glow_m2 *= FractalGlowStrength;

	vec2 m2_rotated = glow_m2 * rot2D(t_scaled * 2.);
	vec3 c = pow(vec3(glow_m1, m2_rotated.x * 0.8, m2_rotated.y * 0.8) * FractalMultiply, vec3(3.0));
	
	c += exp(-DistanceGlowStrength * length(p));
	
	return c;
}


void main() {
	float t_scaled = TIME * GlobalSpeed;
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 p = uv - 0.5;
	p.x *= RENDERSIZE.x / RENDERSIZE.y;

	vec3 col = fractal(p, t_scaled);

	// Apply color palette
	float palette_hue_source = fract(length(uv) * 0.1 + t_scaled * 0.05);
	vec3 palette_color = getPaletteColor(palette_hue_source, ColorPaletteMode);
	col = mix(col, col * palette_color, PaletteMixFactor);

    // Apply color pulse (if enabled)
    if (ColorPulseEnabled > 0.5) {
        float pulse = sin(t_scaled * 0.5) * 0.5 + 0.5;
        col *= 1.0 + 0.3 * pulse;
    }

    // Apply Contrast
    col = (col - 0.5) * Contrast + 0.5;

	// Mix with grayscale version for desaturation/mood effect
	col = mix(vec3(length(col)), col, 0.6);

	uv = (uv - 0.5) * 1.9;
	// Vignette effect
	col *= max(0.0, 1.0 - length(uv * uv * uv));
	
    // Apply Brightness and Glow from reference shader
    col *= Brightness * Glow;

	gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
