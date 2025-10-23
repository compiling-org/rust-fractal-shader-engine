/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Psychedelic"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/wtBBzG by Kali. Overthinking is bad. Enhanced with tunable parameters for camera, zoom, morphing, fractal control, geometry, color pulse, and multiple psychedelic color palettes.",
    "IMPORTED": {
    },
    "INPUTS": [
        { "NAME": "GlobalSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 5.0, "LABEL": "Global Speed" },
        { "NAME": "CameraSpeed", "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Speed" },
        { "NAME": "CameraOrbitRadius", "TYPE": "float", "DEFAULT": 0.19, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Orbit Radius" },
        { "NAME": "CameraYPos", "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Camera Y Position" },
        { "NAME": "CameraZOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0, "LABEL": "Camera Z Offset" },
        { "NAME": "CameraLookAtZRatio", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0, "LABEL": "Camera LookAt Z Ratio" },
        { "NAME": "ZoomLevel", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Zoom Level" },
        { "NAME": "RotationIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Fractal Rotation Int." },
        { "NAME": "FoldIntensity", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Fractal Fold Intensity" },
        { "NAME": "ScaleFactor", "TYPE": "float", "DEFAULT": -2.5, "MIN": -5.0, "MAX": 0.0, "LABEL": "Fractal Scale Factor" },
        { "NAME": "IterationOffset", "TYPE": "float", "DEFAULT": -1.0, "MIN": -5.0, "MAX": 5.0, "LABEL": "Fractal Iteration Offset" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 5.3, "MIN": 1.0, "MAX": 20.0, "LABEL": "Fractal Iterations" },
        { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 2.4, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
        { "NAME": "ColorPulseAmplitude", "TYPE": "float", "DEFAULT": 2.4, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Pulse Amplitude" },
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "PaletteChoice", "TYPE": "float", "DEFAULT": 2.3, "MIN": 0.0, "MAX": 5.0, "LABEL": "Color Palette", "ANNOTATIONS": { "0.0": "Classic", "1.0": "Warm", "2.0": "Cool", "3.0": "Vibrant Neon", "4.0": "Deep Space", "5.0": "Dreamy Pastel" } },
        { "NAME": "PaletteMixFactor", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Palette Mix Factor" }
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

// Define t as a uniform controlled by GlobalSpeed and CameraSpeed
#define t (TIME * GlobalSpeed * CameraSpeed)

// Hard limit for fractal iterations to satisfy GLSL ES compiler constraints
const int MAX_FRACTAL_ITERATIONS_LIMIT = 25; 

vec3 col_internal; // Original shader used 'col', renamed to 'col_internal' for clarity

mat2 rot(float a)
{
    float s=sin(a),c=cos(a);
    return mat2(c,s,-s,c);
}

// HSV to RGB conversion with cubic smoothing
vec3 hsv2rgb_smooth( in vec3 c )
{
    vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
    rgb = rgb*rgb*(3.0-2.0*rgb); // cubic smoothing
    return c.z * mix( vec3(1.0), rgb, c.y);
}

// Custom hue function for multiple color palettes
vec3 getPaletteColor(float h_val, float palette_type) {
    vec3 color_base;
    h_val = mod(h_val, 1.0); // Ensure hue is within [0, 1) range

    if (palette_type < 0.5) { // Palette 0: Classic Psychedelic (sin waves)
        color_base = clamp(abs(sin(h_val * TAU + vec3(0.0, 2.0, 4.0))), 0.0, 1.0);
    } else if (palette_type < 1.5) { // Palette 1: Warm Hues (Reds, Oranges, Yellows)
        h_val = mod(h_val, 1.0) * 0.3 + 0.0; // Restrict hue to warm range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 2.5) { // Palette 2: Cool Tones (Blues, Purples, Greens)
        h_val = mod(h_val, 1.0) * 0.3 + 0.6; // Restrict hue to cool range
        color_base = 0.5 + 0.5 * cos(h_val * TAU + vec3(0.0, 2.0, 4.0));
    } else if (palette_type < 3.5) { // Palette 3: Vibrant Neon
        color_base = hsv2rgb_smooth(vec3(h_val, 0.9, 0.8)); // Static saturation/value for vibrant
    } else if (palette_type < 4.5) { // Palette 4: Deep Space (Darker blues/purples with bright accents)
        color_base = hsv2rgb_smooth(vec3(mod(h_val * 0.5 + 0.6, 1.0), 0.7, 0.3)); // Base deep color
        color_base = mix(color_base, hsv2rgb_smooth(vec3(mod(h_val * 1.5 + 0.1, 1.0), 0.8, 0.9)), 0.2); // Static vibrant accents
    } else { // Palette 5: Dreamy Pastel
        color_base = hsv2rgb_smooth(vec3(h_val, 0.3, 0.7)); // Static saturation/value for pastel
    }
    return color_base;
}


float de(vec3 p)
{
    float z=p.z,m=1000.,sc=1.;
    vec3 ot=vec3(1000);
    // vec3 cp=p; // Original shader had this, but it was unused. Removed for clean code.

    // Apply tunable rotation intensity
    p.xy*=rot(p.z * .05 + t * 3. * RotationIntensity); // Original was xz and then xy; sticking to original's xy with tunable intensity
    p.xy*=rot(t * RotationIntensity - p.z*.2); // Second rotation, applied to xy

    // Apply tunable fold intensity
    p=abs(FoldIntensity-mod(p,10.));
    
    // Use tunable FractalIterations with a const loop limit to avoid GLSL ES compiler errors
    for (int i=0;i < MAX_FRACTAL_ITERATIONS_LIMIT; i++) 
    {
        if (float(i) >= FractalIterations) { // Break if user's iteration count is reached
            break;
        }

        p.xy=abs(p.xy+.5)-abs(p.xy-.5)-p.xy-1.;
        p.yz*=rot(t);
        
        // Use tunable ScaleFactor and ensure no division by zero
        float p_dot_p = dot(p,p);
        float s_val = ScaleFactor / clamp(p_dot_p, 0.000001, 1.0); // Use 0.000001 to prevent div by zero
        
        // Use tunable IterationOffset
        p=p*s_val + IterationOffset; 
        sc*=s_val;
        ot=min(ot,abs(p.yzz));
        m=min(m,abs(p.z));
    }

    m=exp(-1.*m);
    
    // col_internal is still set here, but its influence will be mixed with a palette later in main()
    col_internal=exp(-2.*ot)+m*.5;
    
    // Apply color pulse parameters to internal color calculation
    float pulse_factor = fract(-t * ColorPulseSpeed + m * .3 + z * .05) * ColorPulseAmplitude;
    col_internal *= pulse_factor;

    return (p.x/sc-.5)*.8;
}

vec3 march(vec3 from, vec3 dir) 
{
	float d,td=0.;
    vec3 p, c_accumulated=vec3(0); 
    // vec3 ot=vec3(1000); // Original shader had this but it was unused within march, so removed for clarity.

    for (int i=0; i<70; i++) // This loop already uses a constant, so no change needed here.
    {
        p=from+dir*td;
        d=de(p); // de() sets the global col_internal variable
        td+=max(.002, abs(d));
        if (td>50.) break;
        c_accumulated+=col_internal*max(0.,1.-d)*exp(-.05*td); 
    }
   	return pow(c_accumulated*.02,vec3(1.5));
}

void main() {
    // Apply ZoomLevel to UV
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE.xy*.5)/RENDERSIZE.y / ZoomLevel;

    // Apply Camera Orbit, Position, and LookAt from tunable parameters
    vec3 dir = normalize(vec3(uv, CameraLookAtZRatio));
    vec3 from = vec3(cos(t*2.) * CameraOrbitRadius, CameraYPos, t*10. + CameraZOffset);
    
    // Original camera rotation logic
    dir.xz *= rot(smoothstep(-.3,.3,sin(t))*3.);

	vec3 final_color = march(from, dir);

    // Apply color palette based on time and UV (can be adjusted)
    float palette_hue_source = fract(TIME * 0.02 + length(uv) * 0.1); 
    vec3 palette_effect_color = getPaletteColor(palette_hue_source, PaletteChoice);
    
    // Mix the original shader's color with the chosen palette
    final_color = mix(final_color, final_color * palette_effect_color, PaletteMixFactor);

    // Apply global brightness
    gl_FragColor = vec4(final_color * GlobalBrightness, 1.0);
}