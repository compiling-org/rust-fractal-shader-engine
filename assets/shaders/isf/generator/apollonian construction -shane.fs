/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "3D",
        "Raymarching",
        "Abstract",
        "Animated",
        "Psychedelic",
        "Glitch"
    ],
    "DESCRIPTION": "A single-pass ISF conversion of an Apollonian fractal raymarched scene, featuring intricate geometry, dynamic camera, glowing cables, and customizable visual effects. Now includes multiple psychedelic color palettes, color pulsing, and glitch/shake effects. Note: Full Depth of Field (DOF) is a multi-pass effect; this shader implements a simplified depth-based softening approximation for single-pass compatibility.",
    "CREDIT": "Original shader by mrlab (https://www.shadertoy.com/view/lsKyDW), converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Overall animation speed."
        },
        {
            "NAME": "cameraZoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts camera's distance to the scene."
        },
        {
            "NAME": "cameraPanX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Horizontal camera pan."
        },
        {
            "NAME": "cameraPanY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Vertical camera pan."
        },
        {
            "NAME": "cameraPanZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Depth camera pan."
        },
        {
            "NAME": "cameraFOV",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": 0.5,
            "MAX": 3.0,
            "STEP": 0.01,
            "DESCRIPTION": "Field of View (higher value means wider angle)."
        },
        {
            "NAME": "apollonianRadius",
            "TYPE": "float",
            "DEFAULT": 1.545,
            "MIN": 1.0,
            "MAX": 2.5,
            "STEP": 0.001,
            "DESCRIPTION": "Base radius for the Apollonian fractal inversion."
        },
        {
            "NAME": "apollonianIterations",
            "TYPE": "float",
            "DEFAULT": 8.0,
            "MIN": 3.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Number of iterations for fractal detail. Higher values increase complexity and render time."
        },
        {
            "NAME": "cableThickness",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.001,
            "MAX": 0.05,
            "STEP": 0.001,
            "DESCRIPTION": "Thickness of the glowing cables."
        },
        {
            "NAME": "lightColorSelection",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 9.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 10 predefined light color palettes."
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Speed at which the colors pulse and shift (0 for no pulse)."
        },
        {
            "NAME": "glowIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Multiplier for the overall glow effect from the cables."
        },
        {
            "NAME": "truchetWarpStrength",
            "TYPE": "float",
            "DEFAULT": 0.035,
            "MIN": 0.0,
            "MAX": 0.1,
            "STEP": 0.001,
            "DESCRIPTION": "Strength of the distortion applied to Truchet cables."
        },
        {
            "NAME": "randomSeedOffset",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 100.0,
            "STEP": 1.0,
            "DESCRIPTION": "Offset for random seeds to change cable patterns."
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
            "DESCRIPTION": "Adjusts the overall image color saturation."
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
            "NAME": "dofEnabled",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 1.0,
            "DESCRIPTION": "Enable or disable Depth of Field effect (0 = off, 1 = on)."
        },
        {
            "NAME": "dofFocusDistance",
            "TYPE": "float",
            "DEFAULT": 2.5,
            "MIN": 0.1,
            "MAX": 10.0,
            "STEP": 0.01,
            "DESCRIPTION": "The distance at which the scene is in sharpest focus."
        },
        {
            "NAME": "dofAperture",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the 'blur' amount for out-of-focus areas. Higher values mean more softening."
        },
        {
            "NAME": "vignetteIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Intensity of the darkening vignette effect around the edges."
        },
        {
            "NAME": "glitchStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of the digital glitch effect (0 for no glitch)."
        },
        {
            "NAME": "shakeStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Strength of the camera shake effect (0 for no shake)."
        }
    ]
}
*/

// Maximum ray distance.
#define MAXDIST 15.0

// Standard 2D rotation formula.
mat2 r2(in float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// IQ's vec2 to float hash.
float hash31(vec3 p){
    return fract(sin(dot(p, vec3(113.619, 57.583, 27.897)))*43758.5453);
}

float hash21(vec2 p){ return fract(sin(dot(p, vec2(27.619, 57.583)))*43758.5453); }

// The path is a 2D sinusoid that varies over time, depending upon the frequencies, and amplitudes.
vec2 path(in float z){
    // Windy weaved path.
    float c = cos(z*3.14159265/4.);
    float s = sin(z*3.14159265/4.);
    return vec2(1. + c*2.0, 1. + s*2.0);
}

// Commutative smooth maximum function. Provided by Tomkh, and taken
// from Alex Evans's (aka Statix) talk:
// http://media.lolrus.mediamolecule.com/AlexEvans_SIGGRAPH-2015.pdf
// Credited to Dave Smith @media molecule.
float smax(float a, float b, float k){
    float f = max(0., 1. - abs(b - a)/k);
    return max(a, b) + k*.25*f*f;
}


// Tri-Planar blending function. Based on an old Nvidia writeup:
// GPU Gems 3 - Ryan Geiss: http://http.developer.nvidia.com/GPUGems3/gpugems3_ch01.html
// Modified to use procedural texture since iChannel1 is not used for texture.
vec3 proceduralTex3D(vec3 p, vec3 n, float time_val){

    // A simple noise-based texture
    float nx = mix(sin(p.y * 5.0 + time_val * 0.1), cos(p.z * 5.0 + time_val * 0.1), 0.5);
    float ny = mix(sin(p.x * 5.0 + time_val * 0.1), cos(p.z * 5.0 + time_val * 0.1), 0.5);
    float nz = mix(sin(p.x * 5.0 + time_val * 0.1), cos(p.y * 5.0 + time_val * 0.1), 0.5);

    vec3 tex_color = vec3(
        0.5 + 0.5 * nx,
        0.5 + 0.5 * ny,
        0.5 + 0.5 * nz
    );

    n = max(abs(n) - .2, 0.001);
    n /= dot(n, vec3(1));

    // Simple blending based on normals, similar to tri-planar mapping
    return tex_color;
}


// Apollonian sphere packing of sorts, based on IQ's cool example here:
// https://www.shadertoy.com/view/4ds3zn
// `apollonianRadius` and `apollonianIterations` are ISF inputs
float apollonian(vec3 p){
    float scale = 1.;
    for( int i=0; i<int(apollonianIterations); i++ ){
        p = mod(p - 1., 2.) - 1.;
        float k = apollonianRadius/dot(p, p);
        p *= k;
        scale *= k;
    }
    return length(p)/scale/4. - .0025;
}

// Poloidal distance.
float lengthP(vec2 p){
    return length(p);
}


float gID; // Object ID.
vec3 glow; // Glow.

float map(vec3 p) {

    // The Truchet cabel object.
    // Warping the cables just a bit.
    vec3 q = p - sin(p*8. - cos(p.yzx*8.)) * truchetWarpStrength;

    // Local cell ID and coordinates.
    vec3 iq = floor(q/2.);
    q -= (iq + .5)*2.;

    // Random cell -- and as such, Truchet cable -- rotation.
    // Use randomSeedOffset to change the pattern
    float rnd = hash31(iq + randomSeedOffset + .12);
    if(rnd<.33) q = q.yxz;
    else if(rnd<.66) q = q.xzy;

    // Repeat light object along the cable, and angle and cell number.
    float aN = 8., a, n;

    // Truchet (cable) and light objects.
    float lat, light;


    // First arc edge point.
    vec3 rq = q - vec3(1, 1, 0);
    // Torus coordinates around the edge point.
    vec2 tor = vec2(length(rq.xy) - 1., rq.z);
    tor = abs(abs(tor) - .45); // Repeating the torus four more times.
    lat = lengthP(tor); // Torus (cable) object.
    // Repeat lights on this torus.
    a = atan(rq.y, rq.x)/6.2831;
    n = (floor(a*aN) + .5)/aN;
    rq.xy *= r2(-6.2831*n);
    rq.x -= 1.;
    rq.xz = abs(abs(rq.xz) - .45); // Repeating the lights four more times.
    //
    // Constructing the light object, if applicable.
    float bx = 1e5;
    // Use randomSeedOffset to change which lights are on
    if(hash31(iq - vec3(1, 1, 0) + n*.043 + randomSeedOffset*.1)<.25) bx = length(rq);
    light = bx;

    // Doing the same for the second arc edge point.
    rq = q - vec3(0, -1, -1);
    a = atan(rq.z, rq.y)/6.2831;
    tor = vec2(length(rq.yz) - 1., rq.x);
    tor = abs(abs(tor) - .45);
    lat = min(lat, lengthP(tor));
    n = (floor(a*aN) + .5)/aN;
    rq.yz *= r2(-6.2831*n);
    rq.y -= 1.;
    rq.xy = abs(abs(rq.xy) - .45);
    bx = 1e5;
    if(hash31(iq - vec3(0, -1, -1) + n*.043 + randomSeedOffset*.1)<.25) bx = length(rq);
    light = min(light, bx);

    // Doing the same for the third arc edge point.
    rq = q - vec3(-1, 0, 1);
    a = atan(rq.z, rq.x)/6.2831;
    tor = vec2(length(rq.xz) - 1., rq.y);
    tor = abs(abs(tor) - .45);
    lat = min(lat, lengthP(tor));
    n = (floor(a*aN) + .5)/aN;
    rq.xz *= r2(-6.2831*n);
    rq.x -= 1.;
    rq.xy = abs(abs(rq.xy) - .45);
    bx = 1e5;
    if(hash31(iq - vec3(-1, 0, 1) + n*.043 + randomSeedOffset*.1)<.25) bx = length(rq);
    light = min(light, bx);


    // Giving the lattice and lights some thickness using cableThickness ISF input.
    lat -= cableThickness;
    light -= cableThickness * 5.0; // Lights are slightly thicker than cables

    // Use the wire distance to create a diode looking object.
    light = max(light, abs(lat - cableThickness * 1.5) - cableThickness);


    // Construct the repeat apollonian object to thread the cables through.
    float ga = apollonian(p);


    // Repeat sphere space to match the repeat apollonian objects.
    q = mod(p, 2.) - 1.;
    // Smooth combine with the main chamber sphere to give a
    // slightly smoother surface.
    ga = smax(ga, -(length(q) - 1.203), .005);


    // Object ID.
    gID = lat<ga && lat<light? 2. : ga<light? 1. : 0.;

    // If we've hit the light object, add some glow.
    if(gID==0.){
        vec3 base_glow_color_A, base_glow_color_B;
        float pulse_phase = sin(TIME * colorPulseSpeed * 3.0) * 0.5 + 0.5; // Normalized 0 to 1

        // --- DEFINED PSYCHEDELIC COLOR PALETTES ---
        if (lightColorSelection < 0.5) { // Palette 0: Neon Cyberpunk (Green to Magenta)
            base_glow_color_A = vec3(0.0, 1.0, 0.5); // Neon Green
            base_glow_color_B = vec3(0.8, 0.0, 1.0); // Bright Magenta
        } else if (lightColorSelection < 1.5) { // Palette 1: Vaporwave Sunset (Pink/Purple to Cyan/Yellow)
            base_glow_color_A = vec3(1.0, 0.2, 0.8); // Hot Pink
            base_glow_color_B = vec3(0.0, 1.0, 1.0); // Cyan
        } else if (lightColorSelection < 2.5) { // Palette 2: Electric Forest (Deep Blue to Acid Green)
            base_glow_color_A = vec3(0.1, 0.1, 0.9); // Deep Electric Blue
            base_glow_color_B = vec3(0.7, 1.0, 0.0); // Acid Green
        } else if (lightColorSelection < 3.5) { // Palette 3: Warm Psychedelia (Orange/Red to Yellow/Lime)
            base_glow_color_A = vec3(1.0, 0.4, 0.0); // Bright Orange
            base_glow_color_B = vec3(0.9, 1.0, 0.1); // Lime Greenish-Yellow
        } else if (lightColorSelection < 4.5) { // Palette 4: Cosmic Dust (Purple/Blue to Gold)
            base_glow_color_A = vec3(0.6, 0.1, 0.9); // Dark Purple
            base_glow_color_B = vec3(1.0, 0.8, 0.2); // Golden Yellow
        } else if (lightColorSelection < 5.5) { // Palette 5: Glitched Rainbow (Fast Cycle)
            base_glow_color_A = abs(sin(TIME * 7.0 + vec3(0, 2, 4)));
            base_glow_color_B = abs(cos(TIME * 7.0 + vec3(1, 3, 5)));
        } else if (lightColorSelection < 6.5) { // Palette 6: Alien Bioluminescence (Teal to Orange)
            base_glow_color_A = vec3(0.0, 0.8, 0.7); // Teal
            base_glow_color_B = vec3(1.0, 0.5, 0.1); // Orange
        } else if (lightColorSelection < 7.5) { // Palette 7: Monochrome Glitch (Subtle blue/red shift)
            base_glow_color_A = vec3(0.1, 0.2, 0.3);
            base_glow_color_B = vec3(0.3, 0.1, 0.2);
        } else if (lightColorSelection < 8.5) { // Palette 8: Candy Pop (Bright Pink/Yellow to Blue/Green)
            base_glow_color_A = vec3(1.0, 0.4, 0.8);
            base_glow_color_B = vec3(0.2, 0.8, 0.9);
        } else { // Palette 9: Deep Space (Subtle desaturated dark tones)
            base_glow_color_A = vec3(0.15, 0.15, 0.25);
            base_glow_color_B = vec3(0.25, 0.20, 0.15);
        }

        // Apply pulse to the chosen palette
        glow += mix(base_glow_color_A, base_glow_color_B, pulse_phase) / (1.0 + light*light*256.0) * glowIntensity;
    }


    // Minimum scene object.
    return min(lat, min(ga, light));
}


float march(vec3 ro, vec3 rd, inout vec3 accumulated_glow) {

    // Closest and total distance.
    float d, t = hash31(ro + rd)*.15; // Jittering to alleviate glow artifacts.

    accumulated_glow = vec3(0);

    vec2 dt = vec2(1e5, 0); // IQ's clever desparkling trick.

    int i;
    const int iMax = 128; // Max iterations for raymarching
    for (i = 0; i<iMax; i++)
    {
        glow = vec3(0); // Reset glow for current step to get local glow
        d = map(ro + rd*t);
        accumulated_glow += glow; // Accumulate glow from hit light sources
        dt = d<dt.x? vec2(d, dt.x) : dt; // Shuffle things along.
        if (abs(d)<.001*(1. + t*.1) || t>MAXDIST) break;

        // Limit the marching step. It's a bit slower but produces better glow.
        t += min(d*.8, .3);
    }

    // If we've run through the entire loop and hit the far boundary,
    // check to see that we haven't clipped an edge point along the way.
    // Obvious... to IQ, but it never occurred to me. :)
    if(i>=iMax - 1) t = dt.y;

    return min(t, MAXDIST);
}



// Normal function. It's not as fast as the tetrahedral calculation, but more symmetrical.
vec3 nrm(in vec3 p) {
    float sgn = 1.;
    vec3 e = vec3(.001, 0, 0), mp = e.zzz; // Spalmer's clever zeroing.
    // Using a fixed loop count here instead of iFrame-dependent loop
    for(int i = 0; i<6; i++){
        mp.x += map(p + sgn*e)*sgn;
        sgn = -sgn;
        if((i&1)==1){ mp = mp.yzx; e = e.zxy; }
    }
    return normalize(mp);
}


float shadow(vec3 ro, vec3 lp, vec3 n, float k){

    const int maxIterationsShad = 48; // Max shadow iterations

    ro += n*.0015; // Coincides with the hit condition in the "trace" function.
    vec3 rd = lp - ro; // Unnormalized direction ray.

    float shade = 1.;
    float t = 0.;
    float end = max(length(rd), .0001);
    rd /= end;

    for (int i = 0; i<maxIterationsShad; i++){
        float d = map(ro + rd*t);
        shade = min(shade, k*d/t);
        t += clamp(d*.8, .005, .25);

        if (d<0. || t>end) break;
    }
    return max(shade, 0.);
}


// I keep a collection of occlusion routines... OK, that sounded really nerdy. :)
// Anyway, I like this one. I'm assuming it's based on IQ's original.
float cAO(in vec3 pos, in vec3 nor)
{
    float sca = 1.5, occ = 0.0;
    for( int i=0; i<5; i++ ){

        float hr = 0.01 + float(i)*0.35/4.0;
        float dd = map(nor * hr + pos);
        occ += (hr - dd)*sca;
        sca *= 0.7;
    }
    return clamp( 1.0 - occ, 0.0, 1.0 );
}


vec4 render_scene(vec3 ro, vec3 rd, vec3 lp, float current_time_total){

    vec3 accumulated_glow_val; // This will hold the accumulated glow
    float t = march(ro, rd, accumulated_glow_val);

    // Saving the object ID.
    float svID = gID;
    // Saving the glow.
    vec3 svGlow = accumulated_glow_val; // Use the accumulated glow

    // Initialize to the scene color to zero.
    vec3 col = vec3(0);

    // If we've hit the surface color it.
    if (t<MAXDIST){

        // Surface hit point.
        vec3 sp = ro + rd*t;

        // Normal.
        vec3 sn = nrm(sp);

        // Light direction vector and attenuation.
        vec3 ld = (lp - sp);
        float atten = max(length(ld), .001);
        ld /= atten;
        // Light attenuation.
        atten = 1./(1. + atten*.125);

        // Diffuse and specular.
        float diff = max(dot(ld, sn), 0.);
        float spec = pow(max(dot(reflect(rd, sn), ld), 0.), 8.);

        // Ambient occlusion and shadows.
        float ao = cAO(sp, sn);
        float sh = shadow(sp, lp, sn, 8.);


        // Manipulate the glow color based on current_time_total and lightColorSelection.
        float sinF = dot(sin(sp - cos(sp.yzx)*1.57), vec3(1./6.)) + .5;
        // Interpolate colors for trippy effect
        vec3 light_palette_1, light_palette_2;

        float pulse_val = sin(current_time_total * colorPulseSpeed * 5.0) * 0.5 + 0.5; // 0 to 1

        // --- These base colors are used for scene objects, not directly for glow in this section ---
        // Glow calculation for cables is done in map()
        // This part mainly affects the general scene lighting's color tint.
        if (lightColorSelection < 0.5) { // Palette 0: Neon Cyberpunk (Green to Magenta)
            light_palette_1 = vec3(0.0, 0.8, 0.4);
            light_palette_2 = vec3(0.6, 0.0, 0.8);
        } else if (lightColorSelection < 1.5) { // Palette 1: Vaporwave Sunset (Pink/Purple to Cyan/Yellow)
            light_palette_1 = vec3(0.8, 0.1, 0.6);
            light_palette_2 = vec3(0.1, 0.8, 0.8);
        } else if (lightColorSelection < 2.5) { // Palette 2: Electric Forest (Deep Blue to Acid Green)
            light_palette_1 = vec3(0.05, 0.05, 0.7);
            light_palette_2 = vec3(0.5, 0.8, 0.0);
        } else if (lightColorSelection < 3.5) { // Palette 3: Warm Psychedelia (Orange/Red to Yellow/Lime)
            light_palette_1 = vec3(0.8, 0.3, 0.0);
            light_palette_2 = vec3(0.7, 0.8, 0.0);
        } else if (lightColorSelection < 4.5) { // Palette 4: Cosmic Dust (Purple/Blue to Gold)
            light_palette_1 = vec3(0.4, 0.0, 0.7);
            light_palette_2 = vec3(0.8, 0.6, 0.1);
        } else if (lightColorSelection < 5.5) { // Palette 5: Glitched Rainbow (Fast Cycle)
            light_palette_1 = abs(sin(current_time_total * 4.0 + vec3(0, 2, 4)));
            light_palette_2 = abs(cos(current_time_total * 4.0 + vec3(1, 3, 5)));
        } else if (lightColorSelection < 6.5) { // Palette 6: Alien Bioluminescence (Teal to Orange)
            light_palette_1 = vec3(0.0, 0.6, 0.5);
            light_palette_2 = vec3(0.8, 0.4, 0.0);
        } else if (lightColorSelection < 7.5) { // Palette 7: Monochrome Glitch (Subtle blue/red shift)
            light_palette_1 = vec3(0.1, 0.2, 0.3) * 0.5;
            light_palette_2 = vec3(0.3, 0.1, 0.2) * 0.5;
        } else if (lightColorSelection < 8.5) { // Palette 8: Candy Pop (Bright Pink/Yellow to Blue/Green)
            light_palette_1 = vec3(0.8, 0.3, 0.6);
            light_palette_2 = vec3(0.1, 0.6, 0.7);
        } else { // Palette 9: Deep Space (Subtle desaturated dark tones)
            light_palette_1 = vec3(0.1, 0.1, 0.2);
            light_palette_2 = vec3(0.2, 0.15, 0.1);
        }

        // Scene objects generally influenced by this light palette, with a pulse.
        col = mix(light_palette_1, light_palette_2, pulse_val);


        if (svID==1.) col /= 8.; // Darken the apollonian structure.
        else if (svID==2.) col /= 4.; // Darken the wires a little less.


        // Applying the diffuse and specular.
        col = col*(diff*sh + .2) + spec*sh*.5;


        // Adding the glow.
        col += col * svGlow * glowIntensity * 32.0;


        /// Attenuation and ambient occlusion.
        col *= atten*ao;


    }


    // Fog out the background.
    col = mix(col, vec3(0), smoothstep(.0, .8, t/MAXDIST));


    // Color and distance.
    return vec4(col, t);
}

// The following is based on John Hable's Uncharted 2 tone mapping, which
// I feel does a really good job at toning down the high color frequencies
// whilst leaving the essence of the gamma corrected linear image intact.
vec4 uTone(vec4 x){
    return ((x*(x*.6 + .1) + .004)/(x*(x*.6 + 1.)  + .06) - .0667)*1.933526;
}

// Post-processing functions
vec3 adjustBrightness(vec3 color, float val) {
    return color * val;
}

vec3 adjustSaturation(vec3 color, float val) {
    vec3 gray = vec3(dot(color, vec3(0.2126, 0.7152, 0.0722)));
    return mix(gray, color, val);
}

vec3 adjustContrast(vec3 color, float val) {
    return (color - 0.5) * val + 0.5;
}

// Simplified Depth of Field Approximation for single pass
vec3 apply_dof_softening(vec3 color, float depth, float focus_dist, float aperture) {
    if (aperture < 0.001) return color;

    // Calculate circle of confusion (CoC)
    float coc_val = abs(depth - focus_dist) * aperture;

    // Use CoC to control the amount of "softening"
    // This isn't a true blur, but a falloff in crispness/detail
    float focus_factor = 1.0 - smoothstep(0.0, 1.0, coc_val * 0.5); // Adjust multiplier (0.5) to control falloff speed

    // Blend the original color with a slightly desaturated/darker version based on focus_factor
    vec3 softened_color = mix(color, color * 0.5 + vec3(0.1), 1.0 - focus_factor); // Darken and slightly desaturate
    softened_color = mix(color, softened_color, 1.0 - focus_factor); // Blend for a smoother transition

    return softened_color;
}

// Glitch effect (modifies UV for ray direction, and applies color shift post-render)
vec2 applyGlitchUV(vec2 uv, float strength, float time_val) {
    // Horizontal shifts
    float offset_x = (hash21(uv + time_val * 10.0) - 0.5) * 0.2; // Random horizontal shift
    float scanline_strength = sin(uv.y * 150.0 + time_val * 20.0) * 0.5 + 0.5; // Animated scanline pattern
    uv.x += offset_x * scanline_strength * strength;

    // Vertical jumps (less common, more abrupt)
    float jump_chance = 0.99; // Higher value means less frequent jumps
    if (hash21(uv.yx + time_val * 7.0 + 123.45) > jump_chance) {
        uv.y += (hash21(uv.xy * 2.0 + time_val * 3.0 + 54.321) * 2.0 - 1.0) * 0.05 * strength;
    }
    return uv;
}

// Shake effect (applied to camera origin and look-at for genuine scene shake)
vec3 applyShake(vec3 p, float strength, float time_val) {
    vec3 shake_offset = vec3(
        sin(time_val * 20.0 + 1.0) * 0.1,
        cos(time_val * 25.0 + 2.0) * 0.1,
        sin(time_val * 15.0 + 3.0) * 0.1
    ) * strength;
    return p + shake_offset;
}

void main()
{
    float current_time_total = animationSpeed * TIME;
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Apply glitch to UVs *before* raymarching for proper scene distortion
    vec2 glitched_uv = applyGlitchUV(uv, glitchStrength, current_time_total);

    // Camera Setup.
    // Apply camera shake to camera position and look-at point
    vec3 ro_base = vec3(0, 0, current_time_total*.25);
    vec3 lk_base = ro_base + vec3(0, -.05, .25);
    vec3 lp_base = ro_base + vec3(0, 0, .75);

    vec3 ro = applyShake(ro_base, shakeStrength, current_time_total);
    vec3 lk = applyShake(lk_base, shakeStrength, current_time_total);
    vec3 lp = applyShake(lp_base, shakeStrength, current_time_total);


    // Using the Z-value to perturb the XY-plane.
    // Sending the camera, "look at," and light vector down the path. The "path" function is
    // synchronized with the distance function.
    ro.xy += path(ro.z);
    lk.xy += path(lk.z);
    lp.xy += path(lp.z);


    // Apply ISF inputs to camera
    ro.x += cameraPanX;
    ro.y += cameraPanY;
    ro.z += cameraPanZ;
    ro.z *= cameraZoom; // Apply zoom to the base Z position

    // Using the above to produce the unit ray-direction vector.
    float FOV = 3.14159/cameraFOV; // FOV - Field of view from ISF input
    // Use glitched_uv for ray direction
    vec3 fwd = normalize(lk - ro);
    vec3 rgt = normalize(vec3(fwd.z, 0., -fwd.x ));
    vec3 up = cross(fwd, rgt);

    // rd - Ray direction using glitched_uv
    vec3 rd = normalize(glitched_uv.x*rgt + glitched_uv.y*up + fwd/FOV);

    // Camera swivel - based on path position.
    vec2 sw = path(lk.z);
    rd.xy *= r2(-sw.x/24.);
    rd.yz *= r2(-sw.y/16.);


    // Render the scene and get color + depth
    vec4 col4 = render_scene(ro, rd, lp, current_time_total);

    // Applying tone mapping.
    col4.xyz = uTone(col4).xyz; // Don't apply this to the alpha channel (depth).

    // Apply DOF softening if enabled
    if (dofEnabled > 0.5) {
        col4.rgb = apply_dof_softening(col4.rgb, col4.w, dofFocusDistance, dofAperture);
    }

    // Apply brightness, saturation, contrast post-processing
    col4.rgb = adjustBrightness(col4.rgb, brightness);
    col4.rgb = adjustSaturation(col4.rgb, saturation);
    col4.rgb = adjustContrast(col4.rgb, contrast);

    // Post-render glitch color channel shift & noise (alternative to RENDERPASS_PREV)
    vec3 final_color = col4.rgb;
    if (glitchStrength > 0.001) {
        // Simple color channel separation based on noise and glitch strength
        float noise_r = (hash21(uv.xy * 0.1 + current_time_total * 20.0) - 0.5) * 2.0;
        float noise_g = (hash21(uv.yx * 0.1 + current_time_total * 20.0 + 1.0) - 0.5) * 2.0;
        float noise_b = (hash21(uv.xy * 0.1 + current_time_total * 20.0 + 2.0) - 0.5) * 2.0;

        // Apply distinct color shifts for R, G, B channels
        final_color.r = final_color.r + noise_r * glitchStrength * 0.08; // Adjusted factor
        final_color.g = final_color.g + noise_g * glitchStrength * 0.08; // Adjusted factor
        final_color.b = final_color.b + noise_b * glitchStrength * 0.08; // Adjusted factor

        // Add some random noise/flicker specific to glitch
        final_color += (hash21(gl_FragCoord.xy * 0.1 + current_time_total * 50.0 + 0.5) - 0.5) * 0.2 * glitchStrength;

        // Introduce some scanline blocking (optional, can be intense)
        float scan_block_thresh = 0.98;
        float scan_block_noise = hash21(uv * 50.0 + current_time_total * 100.0);
        if (scan_block_noise > scan_block_thresh) {
             final_color *= (0.8 - (scan_block_noise - scan_block_thresh) / (1.0 - scan_block_thresh) * 0.5) * glitchStrength;
        }
    }


    // Vignette
    float vignette = pow(16.0 * uv.x * uv.y * (1.0 - uv.x) * (1.0 - uv.y), 0.3);
    final_color *= mix(1.0, vignette, vignetteIntensity);


    // Output to screen.
    gl_FragColor = vec4(clamp(final_color, 0., 1.), 1.0); // Ensure alpha is 1.0 for final output
}