/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Tunnel",
        "Abstract",
        "Animated",
        "Psychedelic"
    ],
    "DESCRIPTION": "A dynamic fractal tunnel with an orb chase effect. Features extensive tunable parameters for animation speed, tunnel path generation, fractal geometry, orb behavior, trippy color palettes, color pulsing, and comprehensive post-processing controls. Converted and enhanced from a Shadertoy shader.",
    "CREDIT": "Original Shader by diatribes (https://www.shadertoy.com/view/XXGfDz), converted and enhanced by your AI assistant.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 10.0,
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
            "DESCRIPTION": "Controls horizontal offset/sway of the camera view within the tunnel."
        },
        {
            "NAME": "controlY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls vertical offset/sway of the camera view within the tunnel."
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall zoom level of the scene, changing field of view."
        },
        {
            "NAME": "fractalDepth",
            "TYPE": "float",
            "DEFAULT": 6.0,
            "MIN": 1.0,
            "MAX": 15.0,
            "STEP": 1.0,
            "DESCRIPTION": "Controls the number of iterations for fractal complexity. Higher values mean more detail but might impact performance."
        },
        {
            "NAME": "fractalMorphA",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.5,
            "STEP": 0.01,
            "DESCRIPTION": "Morphing parameter A for the fractal equation. Affects the inner boundary of clamping."
        },
        {
            "NAME": "fractalMorphB",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.1,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Morphing parameter B for the fractal equation. Affects the outer boundary of clamping."
        },
        {
            "NAME": "fractalMorphC",
            "TYPE": "float",
            "DEFAULT": 1.95,
            "MIN": 0.5,
            "MAX": 3.0,
            "STEP": 0.01,
            "DESCRIPTION": "Morphing parameter C for the fractal equation. Affects the subtraction value."
        },
        {
            "NAME": "pathFreqZ1",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.01,
            "MAX": 0.5,
            "STEP": 0.005,
            "DESCRIPTION": "Frequency of the first sine wave influencing the tunnel's X path."
        },
        {
            "NAME": "pathFreqZ2",
            "TYPE": "float",
            "DEFAULT": 0.18,
            "MIN": 0.01,
            "MAX": 0.5,
            "STEP": 0.005,
            "DESCRIPTION": "Frequency of the first cosine wave influencing the tunnel's X path."
        },
        {
            "NAME": "pathAmpX1",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 0.1,
            "MAX": 10.0,
            "STEP": 0.05,
            "DESCRIPTION": "Amplitude of the first X path component."
        },
        {
            "NAME": "pathAmpX2",
            "TYPE": "float",
            "DEFAULT": 1.15,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude of the second X path component."
        },
        {
            "NAME": "pathFreqZ3",
            "TYPE": "float",
            "DEFAULT": 0.14,
            "MIN": 0.01,
            "MAX": 0.5,
            "STEP": 0.005,
            "DESCRIPTION": "Frequency of the first sine wave influencing the tunnel's Y path."
        },
        {
            "NAME": "pathFreqZ4",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.01,
            "MAX": 0.5,
            "STEP": 0.005,
            "DESCRIPTION": "Frequency of the first cosine wave influencing the tunnel's Y path."
        },
        {
            "NAME": "pathAmpY1",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.01,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude of the first Y path component."
        },
        {
            "NAME": "pathAmpY2",
            "TYPE": "float",
            "DEFAULT": 3.2,
            "MIN": 0.1,
            "MAX": 10.0,
            "STEP": 0.05,
            "DESCRIPTION": "Amplitude of the second Y path component."
        },
        {
            "NAME": "orbWobbleAmp",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Amplitude of the orb's vertical wobble relative to the path."
        },
        {
            "NAME": "orbZSpeed",
            "TYPE": "float",
            "DEFAULT": 3.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of the orb's forward/backward motion."
        },
        {
            "NAME": "orbZOffset",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "STEP": 0.1,
            "DESCRIPTION": "Base Z offset of the orb relative to the camera."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 7.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of 8 distinct psychedelic color palettes (0-7).",
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

#define MAX_STEPS 80.
#define MAX_DIST 40.
#define SURFACE_DIST .001
#define inf (MAX_DIST+1.0) // Not directly used in current logic, but kept for context

#define ORB_RADIUS .5 // <--- This line is crucial and restored!

// --- Path and Light Helper Functions ---

// Calculates a point on the animated 3D path based on Z-coordinate.
vec3 path(float z, float time_val) {
    return vec3(
        sin(z * pathFreqZ1) * pathAmpX1 * cos(z * pathFreqZ2) * pathAmpX2,
        sin(z * pathFreqZ3) * pathAmpY1 * cos(z * pathFreqZ4) * pathAmpY2,
        time_val + 1.5 // Time for forward motion
    );
}

// Calculates the light orb's position relative to the camera.
vec3 lightPosition(vec3 p, float time_val) {
    // Current fragment position p, offset by the path
    p -= path(p.z, time_val); 
    return p - vec3(
        0.,
        sin(time_val) * orbWobbleAmp, // Orb wobbles vertically
        sin(time_val) * orbZSpeed + orbZOffset // Orb moves back and forth along Z
    );
}

// --- Fractal Function ---
// Modified from a common fractal algorithm, takes tunable morphing parameters.
float fractal(vec3 p, out vec3 rgb, float iterations_float, float morphA, float morphB, float morphC){
  int iterations = int(iterations_float); // Cast to int for loop
  for (int i = 0; i < iterations; i++) {
    float n = abs(p.x * p.y * p.z);
    p = abs(p) / clamp(n, morphA, morphB) - morphC; // Tunable morphing
  }
  rgb = vec3(p.x + p.y);
  return length(rgb * .1);
}

// --- Scene SDF (Signed Distance Function) ---
// Describes the geometry of the tunnel and the orb.
float scene(vec3 p, float time_val, out vec3 rgb) {
    // Calculate tunnel walls relative to the path
	vec3 tun = abs(p - path(p.z, time_val));
    
    // Calculate distance to the light orb
    float orb = length(lightPosition(p, time_val)) - ORB_RADIUS;
    
    // The main scene is the minimum distance to tunnel walls or orb
    float hit = min(orb, min(3. - tun.x, 2. - tun.y)); // 3. and 2. are tunnel dimensions
    
    if (orb == hit) { // If we hit the orb
        fractal(vec3(tun.xy, p.z), rgb, fractalDepth, fractalMorphA, fractalMorphB, fractalMorphC);
        rgb *= vec3(.05, 1.5, .05); // Orb color - green/yellowish
    } else { // If we hit the tunnel walls
        fractal(vec3(tun.xy, sin(p.z)), rgb, fractalDepth, fractalMorphA, fractalMorphB, fractalMorphC);
    }
    return hit;
}

// --- Ray Marching ---
// Steps along a ray to find the nearest object in the scene.
float raymarch(vec3 ro, vec3 rd, float time_val, out vec3 rgb) {
  float dist = 0.0;
  for(float i = 0.; i < MAX_STEPS; i++) {
    vec3 p = ro + rd * dist;
    float step = scene(p, time_val, rgb);
    dist += step;
    if(dist > MAX_DIST || step < SURFACE_DIST) {
        break;
    }
  }
  return dist;
}

// --- LookAt Matrix ---
// Creates a view matrix for the camera.
mat3 lookAt(vec3 origin, vec3 target, float roll) {
  vec3 rr = vec3(sin(roll), cos(roll), 0.0);
  vec3 ww = normalize(target - origin);
  vec3 uu = normalize(cross(ww, rr));
  vec3 vv = normalize(cross(uu, ww));
  return mat3(uu, vv, ww);
}

// --- Normal Calculation ---
// Approximates the surface normal for lighting.
vec3 normal(vec3 p, float time_val) {
  vec2 e = vec2(.01, 0); // Small epsilon for gradient
  vec3 temp_rgb; // Dummy RGB for scene calls in normal
  
  // Calculate gradient by sampling scene at slightly offset points
  vec3 n = scene(p, time_val, temp_rgb) - vec3(
    scene(p - e.xyy, time_val, temp_rgb),
    scene(p - e.yxy, time_val, temp_rgb),
    scene(p - e.yyx, time_val, temp_rgb));

  return normalize(n);
}

// --- Color Utility Functions ---

// HSV to RGB conversion
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.x, 0.0, 1.0), c.y);
}

// RGB to HSV conversion
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Applies color pulsing and chosen palette to an input color.
vec3 applyColorPalette(vec3 color_in, float iTime_scaled) {
    vec3 hsv = rgb2hsv(color_in);

    // Apply granular color pulse controls, influenced by overall colorPulseIntensity
    float huePulse = iTime_scaled * colorPulseHueStrength * colorPulseIntensity * 0.5; 
    float satPulseFactor = (1.0 + colorPulseSatStrength * sin(iTime_scaled * 2.0) * colorPulseIntensity);
    float valPulseFactor = (1.0 + colorPulseValStrength * cos(iTime_scaled * 3.0) * colorPulseIntensity);

    hsv.x = mod(hsv.x + huePulse, 1.0);
    hsv.y = hsv.y * satPulseFactor;
    hsv.z = hsv.z * valPulseFactor;
    hsv.y = clamp(hsv.y, 0.0, 1.0); 
    hsv.z = clamp(hsv.z, 0.0, 1.0); 

    int palette = int(colorPalette);

    if (palette == 1) { // Psychedelic Dream (Purples, Cyans, Pinks)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.6 + sin(iTime_scaled * 0.05) * 0.1, 1.0), 0.9); 
        hsv.y = clamp(hsv.y * 1.8 + 0.1, 0.0, 1.0); 
        hsv.z = clamp(hsv.z * 1.2, 0.0, 1.0); 
    } else if (palette == 2) { // Electric Neon (Greens, Yellows, Bright Blues)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.2 + cos(iTime_scaled * 0.07) * 0.1, 1.0), 0.95); 
        hsv.y = clamp(hsv.y * 2.0 + 0.15, 0.0, 1.0); 
        hsv.z = clamp(hsv.z * 1.3, 0.0, 1.0);
    } else if (palette == 3) { // Fiery Abyss (Deep Reds, Oranges, Dark Purples)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.0 + sin(iTime_scaled * 0.03) * 0.05, 1.0), 0.9); 
        hsv.y = clamp(hsv.y * 1.6 + 0.2, 0.0, 1.0); 
        hsv.z = clamp(hsv.z * 0.9, 0.0, 1.0); 
    } else if (palette == 4) { // Oceanic Tranquility (Blues, Greens, Aquas)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.5 + 0.4 + cos(iTime_scaled * 0.06) * 0.1, 1.0), 0.9); 
        hsv.y = clamp(hsv.y * 1.7 + 0.1, 0.0, 1.0);
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 5) { // Galactic Glitch (Muted Pinks, Teals, Grays)
        hsv.x = mix(hsv.x, mod(hsv.x * 0.3 + 0.7 + sin(iTime_scaled * 0.04) * 0.08, 1.0), 0.8);
        hsv.y = clamp(hsv.y * 1.2 + 0.05, 0.0, 1.0); 
        hsv.z = clamp(hsv.z * 1.1, 0.0, 1.0);
    } else if (palette == 6) { // Rainbow Pulse
        hsv.x = mod(hsv.x + iTime_scaled * 0.2, 1.0); 
        hsv.y = clamp(hsv.y * 2.0 + 0.2, 0.0, 1.0); 
        hsv.z = clamp(hsv.z * 1.2, 0.0, 1.0);
    } else if (palette == 7) { // Monochrome Echo (Grayscale with subtle hue hints)
        hsv.y = hsv.y * 0.05; 
        hsv.x = mod(hsv.x + iTime_scaled * 0.01, 1.0); 
        hsv.z = hsv.z * 1.0;
    }
    // Default palette (palette == 0) uses base input color with only general HSV pulsing applied

    return hsv2rgb(hsv);
}

// Applies post-processing effects like brightness, saturation, contrast, and vignette.
vec3 applyPostProcessing(vec3 rgb, vec2 uv_pixel_norm) {
    rgb = rgb * brightness;
    vec3 luma = vec3(dot(vec3(0.2125, 0.7154, 0.0721), rgb));
    rgb = mix(luma, rgb, saturation);
    rgb = mix(vec3(0.5), rgb, contrast);
    
    // Vignette effect - darkens edges
    rgb *= (1.0 - vignetteIntensity) + vignetteIntensity * pow(16.0 * uv_pixel_norm.x * uv_pixel_norm.y * (1.0 - uv_pixel_norm.x) * (1.0 - uv_pixel_norm.y), 0.125);
    
    return sqrt(clamp(rgb, 0.0, 1.0)); // Gamma correction and clamp
}


// --- Main Shader Function ---
void main() {
    float totalTime = TIME * animationSpeed; // Global animated time

    vec3 rgb = vec3(1.); // Initial color for raymarching output
	
    // Normalize UVs to -1.0 to 1.0 range, correct aspect ratio, apply zoom and XY controls
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 uv_modified = -1.0 + 2.0 * uv;
	uv_modified.x *= RENDERSIZE.x / RENDERSIZE.y; // Aspect ratio correction
    uv_modified /= zoomFactor; // Apply zoom
    uv_modified += vec2(controlX, controlY) * 0.5; // Apply XY control offsets
    
    // Ray origin (camera position)
	vec3 ro = vec3(0., 0., totalTime);
    // Ray look-at target (where the camera is looking)
	vec3 la = ro + vec3(0., 0., 1.); // Looking slightly forward
    
    // Adjust camera position to follow the path
	ro.xy = path(ro.z, totalTime).xy;
    
    // Adjust look-at target relative to the orb's position for "chase" effect
	la.xy = path(la.z, totalTime).xy - lightPosition(ro, totalTime).xy;
	
    // Calculate the ray direction based on modified UVs and look-at matrix
	vec3 rd = normalize(vec3(uv_modified, 1.) * lookAt(ro, la, 0.));
	
    // Perform ray marching
	float d = raymarch(ro, rd, totalTime, rgb); // rgb will be filled by scene/fractal
    
    // Calculate the hit point in 3D space
    vec3 p = ro + rd * d;
    
    // Apply diffuse lighting based on surface normal
    float diffuse = max(dot(normal(p, totalTime), normalize(lightPosition(p, totalTime) - p)), .0);
    rgb *= diffuse * 2.; // Brighten with diffuse light
    
    // Apply light orb influence (fades out color if far from orb)
    float len_orb_dist = length(lightPosition(p, totalTime));
    if (len_orb_dist > ORB_RADIUS + .1) { // If not very close to the orb
        // Dim the color based on distance to orb, with a color tint
        rgb /= pow(len_orb_dist, len_orb_dist * .75) * vec3(1., .1, 1.); 
    }
    
    // Apply color palette and pulsing
    rgb = applyColorPalette(rgb, totalTime * colorPulseSpeed);

    // Apply post-processing effects
    rgb = applyPostProcessing(rgb, uv); // Pass original uv for vignette

    // Final output color (gamma corrected)
    gl_FragColor = vec4(pow(rgb, vec3(.45)), 1.0);
}