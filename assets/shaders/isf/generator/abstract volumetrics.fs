/*{
    "CATEGORIES": ["Fractal", "Psychedelic", "Volumetric", "Swirl", "Portal"],
    "DESCRIPTION": "An abstract volumetric portal that swirls and distorts, with layers of fractal detail and dynamic color palettes.",
    "ISFVSN": "2.0",
    "INPUTS": [
        { "NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 3.0 },
        { "NAME": "PaletteSelect", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.3, "MAX": 5.0 },
        { "NAME": "Speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
        { "NAME": "RotationRate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "SwirlDensity", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0 },
        { "NAME": "SwirlSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "SwirlFrequency", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
        { "NAME": "SwirlAmplitude", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5 },
        { "NAME": "PortalThickness", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.5 },
        { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 1.0 },
        { "NAME": "ZoomLevel", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Camera Zoom" },
        { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 1.8, "MIN": 1.0, "MAX": 10.0, "LABEL": "Fractal Iterations" },
        { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "FractalDetailScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Fractal Detail Scale" }
    ]
}
*/

#define MAX_MORPH_ITERATIONS_CONST 10.0 // Define constant for inner loop limit

mat2 rot(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

vec3 palette(float t, float type) {
    if (type < 1.0) {
        return 0.5 + 0.5 * cos(6.28318 * t + vec3(0.0, 0.5, 1.0));
    } else if (type < 2.0) {
        return vec3(sin(t), sin(t + 1.5), cos(t * 0.5));
    } else {
        return 0.5 + 0.5 * cos(6.28318 * t + vec3(1.0, 0.0, 0.25));
    }
}

// Manual implementation of tanh for vec3
vec3 tanh(vec3 x) {
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 col = vec3(0.0);
    // ZoomLevel now controls the ray direction directly, removing inherent Z-movement
    vec3 rd = normalize(vec3(uv, ZoomLevel)); 
    float time = TIME * Speed;
    float t = 0.0; // Distance along the ray

    for (float i = 0.0; i < 800.0; i++) { // Increased loop for raymarching/accumulation for less noise
        vec3 p = t * rd; // Current point in 3D space along the ray
        
        // Removed p.z += time; to stop the inherent shifting zoom
        p.xy *= rot(time * RotationRate); // Overall rotation

        // Apply dynamic swirling/twisting based on depth and time
        float swirl_angle = p.z * SwirlDensity + time * SwirlSpeed;
        p.xy *= rot(swirl_angle); // Apply rotation to XY plane

        // Add radial distortions
        p.x += sin(p.y * SwirlFrequency + time) * SwirlAmplitude;
        p.y += cos(p.x * SwirlFrequency + time) * SwirlAmplitude;

        // Apply fractal-like morphing (from reference, adjusted)
        for (float j = 0.0; j < MAX_MORPH_ITERATIONS_CONST; j++) { 
            if (j >= FractalIterations) break; 
            float a = exp(j) / exp2(j); // Scaling factor for each morph iteration
            // Applied FractalDetailScale to the position before cosine for fractal control
            p += cos(4.0 * (p.yzx * FractalDetailScale) * a + time - length(p.xy) * RotationRate) * MorphAmount / a;
        }

        // Apply absolute folding for fractal symmetry
        p = abs(p) - PortalThickness; // Creates a portal-like shape

        // Calculate a "distance" for brightness accumulation
        // Using length(p) for a spherical/volumetric glow
        float d = length(p); 
        
        // Accumulate color: Brighter when closer to the "surface" (d is small)
        // Adjusted accumulation multiplier and constant for better color balance and less noise
        vec3 palCol = palette(t * ColorPulse + length(p) * 0.1, PaletteSelect); 
        col += palCol * (10.0 / (d * d * 5.0 + 0.01)); // Increased multiplier, adjusted denominator

        // Advance ray: Move forward by a step related to the "distance"
        t += d * 0.02; // Adjusted step size for better accumulation (smaller steps)

        // Break condition: If ray goes too far, stop
        if (t > 50.0) break; 
    }

    // FIX: Apply Saturation control - Changed luma to float
    float luma = dot(col, vec3(0.2126, 0.7152, 0.0722));
    col = mix(vec3(luma), col, Saturation);

    // Apply Contrast control
    col = (col - 0.5) * Contrast + 0.5;

    // Apply tone mapping and final brightness
    col *= tanh(col * 1.5); // Adjusted tanh multiplier
    col = pow(col, vec3(1.2)); // Adjusted gamma for better color output
    col *= Brightness; // Apply overall brightness control

    gl_FragColor = vec4(col, 1.0);
}
