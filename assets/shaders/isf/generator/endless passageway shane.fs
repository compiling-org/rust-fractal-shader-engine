/*
{
    "CATEGORIES": [
        "Procedural",
        "3D",
        "Tunnel",
        "Abstract",
        "Animated",
        "Psychedelic"
    ],
    "DESCRIPTION": "An endless raymarched passageway with detailed procedural geometry, now with extensive tunable parameters for diverse psychedelic effects, camera motion, fractal complexity, and color pulse variations. Color palettes now affect only the 3D structure and have been refined to avoid foggy effects.",
    "CREDIT": "Original Shadertoy by Shane (https://www.shadertoy.com/view/4ttSRj). ISF conversion and enhancements by your AI assistant.",
    "IMPORTED": {
        "iChannel0": {
            "NAME": "Texture1",
            "PATH": "cd4c518bc6ef165c39d4405b347b51ba40f8d7a065ab0e8d2e4f422cbc1e8a43.jpg"
        },
        "iChannel1": {
            "NAME": "Texture2",
            "PATH": "e6e5631ce1237ae4c05b3563eda686400a401df4548d0f9fad40ecac1659c46c.jpg"
        }
    },
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall animation speed multiplier for all time-based effects."
        },
        {
            "NAME": "controlX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls horizontal camera offset or motion."
        },
        {
            "NAME": "controlY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls vertical camera offset or motion."
        },
        {
            "NAME": "cameraFOV",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 3.0,
            "STEP": 0.01,
            "DESCRIPTION": "Camera field of view. Higher values zoom out, lower values zoom in."
        },
        {
            "NAME": "cameraTwistSpeed",
            "TYPE": "float",
            "DEFAULT": 0.03,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Speed of the camera's rotational twist effect when turning."
        },
        {
            "NAME": "cameraPathAmplitude",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude of the camera's weaving path."
        },
        {
            "NAME": "cameraPathFrequency",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 3.0,
            "STEP": 0.01,
            "DESCRIPTION": "Frequency (density) of the camera's weaving path."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image brightness."
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall color saturation."
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image contrast."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float", 
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 9.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 10 distinct psychedelic color palettes (0-9).",
            "PRAGMA": "COLOR_PALETTE_ENUM"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Base speed for all color pulsing effects."
        },
        {
            "NAME": "colorPulseIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall intensity multiplier for all color pulsing effects."
        },
        {
            "NAME": "colorPulseHueStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of hue shifting in the color pulse."
        },
        {
            "NAME": "colorPulseSatStrength",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of saturation pulsing."
        },
        {
            "NAME": "colorPulseValStrength",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of value (brightness) pulsing."
        },
        {
            "NAME": "columnGap",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 1.0,
            "MAX": 8.0,
            "STEP": 0.1,
            "DESCRIPTION": "Controls the spacing between columns."
        },
        {
            "NAME": "columnRadius",
            "TYPE": "float",
            "DEFAULT": 0.32,
            "MIN": 0.1,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the effective radius/thickness of columns."
        },
        {
            "NAME": "carveFactor",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.2,
            "STEP": 0.005,
            "DESCRIPTION": "Adjusts the carving effect on columns, making them more or less hollowed."
        },
        {
            "NAME": "ceilingFloorThickness",
            "TYPE": "float",
            "DEFAULT": 2.5,
            "MIN": 1.0,
            "MAX": 5.0,
            "STEP": 0.05,
            "DESCRIPTION": "Adjusts the thickness of the ceiling and floor."
        },
        {
            "NAME": "detailScale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Scales the procedural details on surfaces (bump map density)."
        },
        {
            "NAME": "lightPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of the pulsating spotlight effect."
        },
        {
            "NAME": "surfaceDistortion",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.2,
            "STEP": 0.001,
            "DESCRIPTION": "Applies a 'liquid' or 'breathing' distortion to surfaces."
        },
        {
            "NAME": "vignetteIntensity",
            "TYPE": "float",
            "DEFAULT": 0.35,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the darkening effect at the edges of the screen."
        }
    ]
}
*/

// Maximum ray distance.
#define FAR 50.0

// Comment this out to omit the detailing. Basically, the bump mapping won't be included.
#define SHOW_DETAILS

// The edges give it subtle detail. Not entirely necessary, but adds just a little more depth.
#define SHOW_EDGES

float objID; // Structure object ID.
float bObjID; // Bump map detail ID.

// colorPalette is already declared as a float uniform from the JSON.


// --- Utility Functions for Palettes and Post-Processing ---
// These functions are moved here to be defined before main()
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.x, 0.0, 1.0), c.y);
}

vec3 applyColorPalette(vec3 color_in, float iTime_scaled) {
    vec3 newColor = color_in;
    
    vec3 hsv = rgb2hsv(color_in);
    
    // Apply granular color pulse controls, influenced by overall colorPulseIntensity
    float huePulse = iTime_scaled * colorPulseHueStrength * colorPulseIntensity;
    float satPulseFactor = (1.0 + colorPulseSatStrength * sin(iTime_scaled * 2.0) * colorPulseIntensity);
    float valPulseFactor = (1.0 + colorPulseValStrength * cos(iTime_scaled * 3.0) * colorPulseIntensity);

    hsv.x = mod(hsv.x + huePulse, 1.0);
    hsv.y = hsv.y * satPulseFactor;
    hsv.z = hsv.z * valPulseFactor;
    hsv.y = clamp(hsv.y, 0.0, 1.0); // Clamp saturation to valid range
    hsv.z = clamp(hsv.z, 0.0, 1.0); // Clamp value to valid range

    // Cast colorPalette to int here for direct comparison
    int palette = int(colorPalette);

    // Apply palette-specific modifications directly to HSV components
    if (palette == 1) { // Electric Blue/Orange Dream
        hsv.x = mix(hsv.x, mod(hsv.x + iTime_scaled * 0.03 + 0.6, 1.0), 0.4); // Bias towards blue/purple
        hsv.y = clamp(hsv.y * 1.3 + 0.1, 0.0, 1.0); // Boost saturation
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0); // Gentle value boost
    } else if (palette == 2) { // Neon Green/Yellow Slime
        hsv.x = mix(hsv.x, mod(hsv.x + iTime_scaled * 0.04 + 0.25, 1.0), 0.5); // Bias towards greens/yellows
        hsv.y = clamp(hsv.y * 1.4, 0.0, 1.0); // Higher saturation
        hsv.z = clamp(hsv.z * 1.15, 0.0, 1.0);
    } else if (palette == 3) { // Deep Purple/Magenta Abyss
        hsv.x = mix(hsv.x, mod(hsv.x + iTime_scaled * 0.02 + 0.75, 1.0), 0.5); // Bias towards deep purples/magentas
        hsv.y = clamp(hsv.y * 1.1 + 0.2, 0.0, 1.0); // Moderate saturation boost
        hsv.z = clamp(hsv.z * 0.9, 0.0, 1.0); // Slightly darker overall
    } else if (palette == 4) { // Rainbow Vortex
        hsv.x = mod(hsv.x + iTime_scaled * 0.08, 1.0); // Strong, continuous hue shift
        hsv.y = clamp(hsv.y * 1.5, 0.0, 1.0); // Very high saturation
        hsv.z = clamp(hsv.z * 1.05, 0.0, 1.0);
    } else if (palette == 5) { // Retro Grid Glow (Red/Cyan)
        // Alternate between red and cyan base hues
        float targetHue = (sin(iTime_scaled * 1.5) > 0.0) ? 0.98 : 0.55; // Reddish vs Cyan
        hsv.x = mix(hsv.x, targetHue, 0.6); 
        hsv.y = clamp(hsv.y * 1.3, 0.0, 1.0);
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 6) { // Forest Mystique (Greens/Browns with subtle glow)
        hsv.x = mix(hsv.x, mod(hsv.x + iTime_scaled * 0.01 + 0.3, 1.0), 0.3); // Gentle shift towards forest greens
        hsv.y = clamp(hsv.y * 0.9 + 0.05, 0.0, 1.0); // Slightly desaturated, earthy
        hsv.z = clamp(hsv.z * 0.95, 0.0, 1.0); 
    } else if (palette == 7) { // Fire & Ice (Vibrant Red/Blue)
        float targetHue = (sin(iTime_scaled * 1.8) > 0.0) ? 0.0 : 0.6; // Red vs Blue
        hsv.x = mix(hsv.x, targetHue, 0.7);
        hsv.y = clamp(hsv.y * 1.4, 0.0, 1.0);
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 8) { // Golden Galaxy (Warm Gold/Purple)
        hsv.x = mix(hsv.x, mod(hsv.x + iTime_scaled * 0.02 + 0.1, 1.0), 0.4); // Shift towards golds/warm tones
        hsv.y = clamp(hsv.y * 1.2 + 0.1, 0.0, 1.0);
        hsv.z = clamp(hsv.z * 1.2, 0.0, 1.0);
    } else if (palette == 9) { // Monochrome Glitch (Grayscale with subtle shifts)
        hsv.y *= 0.1; // Drastically reduce saturation
        hsv.x = mod(hsv.x + iTime_scaled * 0.005, 1.0); // Very slow hue shift
        hsv.z = mix(hsv.z, 0.5 + 0.1 * sin(iTime_scaled * 8.0), 0.2); // Subtle value flicker
    }
    // Default palette (palette == 0) uses base input color with only general HSV pulsing applied

    newColor = hsv2rgb(hsv);
    return newColor;
}

vec3 applyPostProcessing(vec3 rgb, vec2 uv_pixel_norm)
{
    rgb = rgb * brightness;
    vec3 luma = vec3(dot(vec3(0.2125, 0.7154, 0.0721), rgb));
    rgb = mix(luma, rgb, saturation);
    rgb = mix(vec3(0.5), rgb, contrast);
    
    // Apply controllable vignette intensity
    rgb *= (1.0 - vignetteIntensity) + vignetteIntensity*pow( 16.0*uv_pixel_norm.x*uv_pixel_norm.y*(1.0-uv_pixel_norm.x)*(1.0-uv_pixel_norm.y) , 0.125);
    return sqrt(clamp(rgb, 0.0, 1.0)); // Approximate gamma correction and clamp
}


// 2D rotation. Always handy. Angle vector, courtesy of Fabrice.
mat2 rot( float th ){ vec2 a = sin(vec2(1.5707963, 0.0) - th); return mat2(a, -a.y, a.x); }

// Camera path. Arranged to coincide with the frequency of the lattice.
vec3 camPath(float t, float cX, float cY){
    // Curvy path. Weaving around the columns.
    // Apply cameraPathAmplitude and cameraPathFrequency
    float a = sin(t * 3.14159265/24.0 * cameraPathFrequency + 1.5707963) * cameraPathAmplitude;
    float b = cos(t * 3.14159265/32.0 * cameraPathFrequency) * cameraPathAmplitude;
    
    return vec3(a*4.35 + cX * 2.0, b*a + cY * 2.0, t); // Apply controlX/Y
}


// Smooth minimum. Courtesy of IQ.
float sminP( float a, float b, float smoothing ){
    float h = clamp((b-a)/smoothing*0.5 + 0.5, 0.0, 1.0);
    return mix(b, a, h) - smoothing*h*(1.0 - h);
}

// Smooth tiles. There are better ways, but it works.
float tiles(vec2 p){
    p = fract(p);
    
    float s = pow( 16.0*p.x*p.y*(1.0-p.x)*(1.0-p.y), 0.125);
    
    return smoothstep(0.0, 1.0, s);
}

// Standard lattice variation, of which there are infinitely many. This is only called by the
// bump mapping function to add some detail to the structure. You could certainly incorporate it
// into the distance function, but it would slow it down considerably.
float lattice(vec3 p){
    // Apply detailScale to the lattice for bump mapping
    p *= detailScale; 
    // Repeat field entity one, which is just some square tubes repeated in all directions every
    // two units, then combined with a minimum function. Otherwise known as a lattice.
    p = abs(mod(p, 2.0) - 1.0);
    float x1 = min(max(p.x, p.y), min(max(p.y, p.z), max(p.x, p.z))) - 0.32;
    
    // Repeat field entity two, which is just an abstract object repeated every half unit.
    p = abs(mod(p,  0.5) - 0.25);
    float x2 = min(p.x, min(p.y, p.z));
    
    bObjID = step(x2, x1);
    
    // Combining the two entities above.
    return max(x1, x2) - 0.08;
}

// Standard lattice variation, of which there are infinitely many.
float columns(vec3 p){
    // Repeat field entity one, which is just some square tubes repeated in all directions every
    // four units.
    p = abs(mod(p, columnGap) - columnGap/2.0); // Use columnGap input
    
    float x1 = max(p.x, p.z) - columnRadius; // Columns. Use columnRadius input
    
    float bl = max(max(p.x - 0.5, p.z - 0.5), p.y + 0.1); // Column header and footer. Boxes.
    
    x1 = min(x1, bl); // Column with header and footer.
    
    // Repeat field entity two, which is just an abstract object repeated every half unit.
    p = abs(mod(p,  0.5) - 0.25);
    float x2 = min(p.x, min(p.y, p.z)); // Carving out the columns with a repeat object.
    
    objID = step(x1, x2 - carveFactor); // ID, to give the column two different materials. Use carveFactor
    // Combining the two entities above.
    return max(x1, x2) - 0.05;
}

// Nothing more than some columns enclosed with a floor, ceiling and walls. Pretty simple.
float map(vec3 p){
    float d =  columns(p); // Repeat columns.
    
    float fl = p.y + ceilingFloorThickness; // Floor. Use ceilingFloorThickness

    p = abs(p);
    
    d = sminP(d, -(p.y - ceilingFloorThickness - d*0.75), 1.5); // Add a smooth ceiling. Use ceilingFloorThickness
    
    d = min(d, -(p.x - 5.85)); // Add the Walls.
    
    d = sminP(d, fl, 0.25); // Smoothly combine the floor.
    
    return d*0.75;
}

// Raymarching. Pretty standard. Nothing fancy.
float trace(vec3 ro, vec3 rd){
    float t = 0.0, d;
    for (int i=0; i<80; i++){
        d = map(ro + rd*t);
        if(abs(d) < 0.001*(t*0.125 + 1.0) || t > FAR) break;
        t += d;
    }
    
    return min(t, FAR);
}

// Tri-Planar blending function.
// Now directly accepts a sampler2D uniform.
vec3 getTriPlanarTexture(sampler2D tex_sampler, vec3 p, vec3 n){
    n = max(abs(n) - 0.2, 0.001);
    n /= dot(n, vec3(1.0));
    
    // Use fract() to tile the texture coordinates, as 'p' are world coordinates
    // and texture() expects 0-1 UVs.
    vec3 tx_yz = texture(tex_sampler, fract(p.yz)).xyz;
    vec3 tx_xz = texture(tex_sampler, fract(p.xz)).xyz;
    vec3 tx_xy = texture(tex_sampler, fract(p.xy)).xyz;
    
    // Textures are stored in sRGB (I think), so you have to convert them to linear space
    // (squaring is a rough approximation) prior to working with them... or something like that. :)
    // Once the final color value is gamma corrected, you should see correct looking colors.
    return tx_yz*tx_yz*n.x + tx_xz*tx_xz*n.y + tx_xy*tx_xy*n.z;
}

// The bump mapping function.
float bumpFunction(in vec3 p){
    float c = 0.0;
    
    // The logic is simple, but a little messy.
    if(p.y > 2.15) c = min(abs(lattice(p*3.0))*1.6, 1.0); // Ceiling.
    else if(p.y > -2.15) c = min(abs(lattice(p*4.0))*1.6, 1.0); // Columns.
    else c = max(tiles(p.xz + 0.5) - min(abs(lattice(p*2.0))*1.6, 1.0), 0.0)*0.5; // Floor.
    
    return c;
}

// Standard function-based bump mapping function with some edging thrown into the mix.
vec3 doBumpMap(in vec3 p, in vec3 n, float bumpfactor, inout float edge){
    // Resolution independent sample distance... Basically, I want the lines to be about
    // the same pixel with, regardless of resolution... Coding is annoying sometimes. :)
    vec2 e = vec2(3.0/RENDERSIZE.y, 0.0);
    
    float f = bumpFunction(p); // Hit point function sample.
    
    float fx = bumpFunction(p - e.xyy); // Nearby sample in the X-direction.
    float fy = bumpFunction(p - e.yxy); // Nearby sample in the Y-direction.
    float fz = bumpFunction(p - e.yyx); // Nearby sample in the Y-direction.
    
    float fx2 = bumpFunction(p + e.xyy); // Sample in the opposite X-direction.
    float fy2 = bumpFunction(p + e.yxy); // Sample in the opposite Y-direction.
    float fz2 = bumpFunction(p + e.yyx); // Sample in the opposite Z-direction.
    
    // The gradient vector. Making use of the extra samples to obtain a more locally
    // accurate value. It has a bit of a smoothing effect, which is a bonus.
    vec3 grad = vec3(fx - fx2, fy - fy2, fz - fz2)/(e.x*2.0);
    
    // Using the above samples to obtain an edge value. In essence, you're taking some
    // surrounding samples and determining how much they differ from the hit point
    // sample. It's really no different in concept to 2D edging.
    edge = abs(fx + fy + fz + fx2 + fy2 + fz2 - 6.0*f);
    edge = smoothstep(0.0, 1.0, edge/e.x);
    
    // Some kind of gradient correction. I'm getting so old that I've forgotten why you
    // do this. It's a simple reason, and a necessary one. I remember that much. :)
    grad -= n*dot(n, grad);
    
    return normalize(n + grad*bumpfactor); // Bump the normal with the gradient vector.
}

// The normal function with some edge detection rolled into it. Sometimes, it's possible to get away
// with six taps, but we need a bit of epsilon value variance here, so there's an extra six.
vec3 nr(vec3 p, inout float edge, float t){
    vec2 e = vec2(3.0/RENDERSIZE.y, 0.0); // Larger epsilon for greater sample spread, thus thicker edges.

    // Take some distance function measurements from either side of the hit point on all three axes.
    float d1 = map(p + e.xyy), d2 = map(p - e.xyy);
    float d3 = map(p + e.yxy), d4 = map(p - e.yxy);
    float d5 = map(p + e.yyx), d6 = map(p - e.yyx);
    float d = map(p)*2.0;    // The hit point itself - Doubled to cut down on calculations. See below.
    
    // Edges - Take a geometry measurement from either side of the hit point. Average them, then see how
    // much the value differs from the hit point itself. Do this for X, Y and Z directions. Here, the sum
    // is used for the overall difference, but there are other ways. Note that it's mainly sharp surface
    // curves that register a discernible difference.
    edge = abs(d1 + d2 - d) + abs(d3 + d4 - d) + abs(d5 + d6 - d);
    
    // Once you have an edge value, it needs to normalized, and smoothed if possible. How you
    // do that is up to you. This is what I came up with for now, but I might tweak it later.
    edge = smoothstep(0.0, 1.0, sqrt(edge/e.x*2.0));
    
    // Redoing the calculations for the normal with a more precise epsilon value.
    e = vec2(0.005*min(1.0 + t, 5.0), 0.0);
    d1 = map(p + e.xyy), d2 = map(p - e.xyy);
    d3 = map(p + e.yxy), d4 = map(p - e.yxy);
    d5 = map(p + e.yyx), d6 = map(p - e.yyx);
    
    // Return the normal.
    // Standard, normalized gradient mearsurement.
    return normalize(vec3(d1 - d2, d3 - d4, d5 - d6));
}

// I keep a collection of occlusion routines... OK, that sounded really nerdy. :)
// Anyway, I like this one. I'm assuming it's based on IQ's original.
float cao(in vec3 p, in vec3 n){
    float sca = 1.0, occ = 0.0;
    for(int i=0; i<5; i++){
        float hr = 0.01 + float(i)*0.5/4.0;
        float dd = map(n * hr + p);
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}


// Cheap shadows are hard. In fact, I'd almost say, shadowing particular scenes with limited
// iterations is impossible... However, I'd be very grateful if someone could prove me wrong. :)
float softShadow(vec3 ro, vec3 lp, float k){
    // More would be nicer. More is always nicer, but not really affordable... Not on my slow test machine, anyway.
    const int maxIterationsShad = 20;
    
    vec3 rd = (lp-ro); // Unnormalized direction ray.

    float shade = 1.0;
    float dist = 0.05;
    float end = max(length(rd), 0.001);
    
    rd /= end;

    // Max shadow iterations - More iterations make nicer shadows, but slow things down. Obviously, the lowest
    // number to give a decent shadow is the best one to choose.
    for (int i=0; i<maxIterationsShad; i++){
        float h = map(ro + rd*dist);
        shade = min(shade, smoothstep(0.0, 1.0, k*h/dist)); // Subtle difference. Thanks to IQ for this tidbit.
        dist += clamp(h, 0.01, 0.2);
        
        // Early exits from accumulative distance function calls tend to be a good thing.
        if (h < 0.001 || dist > end) break;
    }

    // I've added 0.5 to the final shade value, which lightens the shadow a bit. It's a preference thing.
    return min(max(shade, 0.0) + 0.2, 1.0);
}


void main() {
    float totalTime = TIME * animationSpeed;
    
    // Screen coordinates.
    vec2 u = (gl_FragCoord.xy - RENDERSIZE.xy*0.5)/RENDERSIZE.y;
    
    // Camera Setup.
    float speed = 3.0;
    vec3 ro = camPath(totalTime*speed, controlX, controlY); // Camera position, doubling as the ray origin.
    vec3 lk = camPath(totalTime*speed + 0.1, controlX, controlY);  // "Look At" position.
    vec3 lp = vec3(0.0, 0.0, totalTime*speed) + vec3(0.0, 0.5, 3.5); // Light position, somewhere near the moving camera.
    
    // Using the above to produce the unit ray-direction vector.
    // Use cameraFOV directly
    float FOV = 3.14159/3.0 / cameraFOV; 
    vec3 fwd = normalize(lk-ro);
    vec3 rgt = normalize(vec3(fwd.z, 0.0, -fwd.x ));
    vec3 up = cross(fwd, rgt);
    
    // Mild lens distortion. The sheer straight edged geometry is was getting to me. :)
    vec3 rd = fwd + FOV*(u.x*rgt + u.y*up);
    rd = normalize(vec3(rd.xy, (rd.z - length(rd.xy)*0.125)*0.75));
    
    // Swiveling the camera from left to right when turning corners.
    // Use cameraTwistSpeed
    rd.xy = rot(-camPath(lk.z, controlX, controlY).x * cameraTwistSpeed )*rd.xy; 
    
    // Raymarch.
    float t = trace(ro, rd);
    float svObjID = objID;
    
    // Surface hit point.
    vec3 sp = ro + rd*t;
    
    // Normal with edge component.
    float edge;
    vec3 sn = nr(sp, edge, t); // Calculate original normal first

    // Surface distortion applied to the normal for a visual 'liquid' effect.
    // This happens AFTER the raymarch and normal calculation, so it doesn't break the SDF.
    // It's a visual trick rather than a change to the underlying geometry.
    if (surfaceDistortion > 0.001) {
        sn = normalize(sn + normalize(vec3(sin(sp.x*5.0 + totalTime), cos(sp.y*5.0 + totalTime), sin(sp.z*5.0 + totalTime))) * surfaceDistortion);
    }
    
    // Shadows and ambient self shadowing.
    float sh = softShadow(sp, lp, 16.0); // Soft shadows.
    float ao = cao(sp, sn); // Ambient occlusion.
    
    // Light direction vector setup and light to surface distance.
    lp -= sp;
    float lDist = max(length(lp), 0.0001);
    lp /= lDist;
    
    // Attenuation.
    float atten = 1.0 / (1.0 + lDist*lDist*0.15);
    
    // More fake lighting. This was just a bit of trial-and-error to produce some repetitive,
    // slightly overhead, spotlights throughout the space. Cylinder in XY, sine repeat
    // in the Z direction over three rows... Something like that.
    vec3 spl = sp;
    spl.x = mod(spl.x, 2.0) - 1.0;
    // Apply lightPulseSpeed
    float spot = max(4.0 - length(spl.xy - vec2(0.0, 2.0)), 0.0)*(sin((spl.z + 1.0)*3.14159/2.0 + totalTime * lightPulseSpeed)*0.5+0.5);
    spot = smoothstep(0.25, 1.0, spot);
    
    // Heavy bump. We do this after texture lookup, so as not to disturb the normal too much.
    float edge2 = 0.0;
    float svBObjID = 0.0;
    #ifdef SHOW_DETAILS
    sn = doBumpMap(sp, sn, 0.15/(1.0 + t/FAR), edge2);
    svBObjID = bObjID;
    #endif
    
    // Diffuse, specular and Fresnel.
    float dif = max(dot(lp, sn), 0.0);
    float spe = pow(max(dot(reflect(rd, sn), lp), 0.0), 6.0);
    float fre = pow(clamp(dot(rd, sn) + 1.0, 0.0, 1.0), 4.0);
    
    // Texturing the object.
    vec3 tx;
    if(sp.y > ceilingFloorThickness + 0.25) { // Ceiling - adjusted threshold
        tx = getTriPlanarTexture(Texture1, sp/2.0, sn)*1.0;
    }
    else if(sp.y > ceilingFloorThickness - 0.12) {
        tx = getTriPlanarTexture(Texture2, sp/1.0, sn);
        tx = smoothstep(0.025, 0.7, tx);
    }
    else if(sp.y > -(ceilingFloorThickness - 0.12)) {
        if(svObjID > 0.5) {
            tx = getTriPlanarTexture(Texture2, sp/1.0, sn);
            tx = smoothstep(0.025, 0.7, tx);
        }
        else tx = getTriPlanarTexture(Texture1, sp/4.0 + 0.5, sn)*1.0;
    }
    else if(sp.y > -(ceilingFloorThickness + 0.25)) {
        tx = getTriPlanarTexture(Texture2, sp/1.0, sn);
        tx = smoothstep(0.025, 0.7, tx);
    }
    else {
        tx = getTriPlanarTexture(Texture1, sp/4.0 + 0.5, sn);
        tx = smoothstep(-0.15, 0.9, tx);
        if (svBObjID > 0.5) tx *= 2.0/1.25;
    }
    
    #ifdef SHOW_EDGES
    // Applying the normal-based and bump mapped edges.
    tx *= (1.0-edge*0.7)*(1.0-edge2*0.7);
    #endif
    
    // Combining the terms above to produce the base color of the 3D structure.
    vec3 fc = tx *(dif + 0.25 + vec3(0.5, 0.7, 1.0)*fre*4.0) + vec3(1.0, 0.7, 0.3)*spe*3.0 + spot*tx*3.0;
    fc *= atten*sh*ao;
    
    // *** IMPORTANT CHANGE: Apply color palette ONLY to the structure's color (fc) ***
    fc = applyColorPalette(fc, totalTime * colorPulseSpeed);

    // Mixing in a bright sunny... distant light (background). This is done *after* the palette.
    vec3 bg = mix(vec3(1.0, 0.5, 0.3), vec3(1.0, 0.9, 0.5), rd.y*0.5+0.5);
    fc = mix(fc, bg*1.25, smoothstep(0.0, 0.9, t/FAR)); // Mix the structured color with the background

    // Post processing & Vignette
    fc = applyPostProcessing(fc, gl_FragCoord.xy/RENDERSIZE.xy);
    
    gl_FragColor = vec4(fc, 1.0);
}