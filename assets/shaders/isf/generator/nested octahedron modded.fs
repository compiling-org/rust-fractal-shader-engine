/*
{
  "CATEGORIES": ["Raymarching", "Psychedelic", "Feedback"],
  "DESCRIPTION": "Nested octahedrons with raymarched glow and feedback blur, now with tunable parameters.",
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    },
    {
    }
  ],
  "INPUTS": [
    {
      "NAME": "AnimationSpeed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "ColorPulseSpeed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Color Pulse Speed"
    },
    {
      "NAME": "DistortionIntensity",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 0.5,
      "DEFAULT": 0.08,
      "LABEL": "Fractal Distortion"
    },
    {
      "NAME": "FractalOffset",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 1.0,
      "DEFAULT": 0.79,
      "LABEL": "Fractal Offset"
    },
    {
      "NAME": "ShapeSize",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 2.0,
      "DEFAULT": 0.6,
      "LABEL": "Octahedron Size"
    },
    {
      "NAME": "RotationAxisFreq",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.25,
      "LABEL": "Rotation Axis Freq"
    },
    {
      "NAME": "PaletteMix",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.7,
      "LABEL": "Palette Mix"
    },
    {
      "NAME": "GammaCorrection",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 4.0,
      "DEFAULT": 2.0,
      "LABEL": "Gamma Correction"
    },
    {
      "NAME": "InitialBrightness",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 5.0,
      "DEFAULT": 1.0,
      "LABEL": "Initial Brightness"
    },
    {
      "NAME": "BlurStrength",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Blur Strength"
    }
  ]
}
*/

// Rotational matrix define (not directly used in this ROT, but good to keep if needed)
#define rot(x) mat2(cos(x+vec4(0,11,33,0)))

// Rotation macro for a 3D point 'p' around an 'axis' by angle 't'
#define ROT(p,axis,t) mix(axis*dot(p,axis),p,cos(t))+sin(t)*cross(p,axis)

// Hue to RGB conversion / color palette function
// 'h' is hue, 'id' is an identifier (not used in H, but kept for context if needed later)
#define H(h,id) (cos(h + vec3(10.,3.,2.)) * 0.7 + 0.2)

// M function - logarithmic mapping, typically used for scaling color contributions
#define M(c) log(c)

void main() {
    // Pass 0: Fractal Generation (renders to BufferA)
    if (PASSINDEX == 0) {
        vec3 c = vec3(0.0); // Initialize color accumulator
        // Calculate normalized UV coordinates for screen space
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        // Calculate ray direction 'rd' from camera to pixel, scaled for scene depth
        vec3 rd = normalize(vec3(gl_FragCoord.xy - 0.5 * RENDERSIZE.xy, RENDERSIZE.y)) * 32.0;

        float sc, dotp, totdist = 0.0; // Scaling factor, dot product, total distance raymarched
        float t = TIME * AnimationSpeed; // Time scaled by AnimationSpeed input

        // Raymarching loop for fractal generation
        for (int i = 0; i < 150; i++) {
            // Current raymarch position in 3D space
            vec4 p = vec4(rd * totdist, 0.0);
            p.xyz += vec3(0.0, 0.0, -100.0); // Offset the fractal's position
            sc = 1.0; // Reset scaling factor for each step

            // Rotate the point 'p' around a dynamic axis determined by Time and RotationAxisFreq
            p.xyz = ROT(p.xyz, normalize(vec3(sin(t * RotationAxisFreq), cos(t * RotationAxisFreq * 1.2), 0.0)), t);

            vec4 id = floor(p / 4.0); // Grid ID for potential variations (not directly used for color here)

            // Iterative fractal transformation
            for (int j = 0; j < 7; j++) {
                // 'blobs' controls a base value for the fractal transform, modulated by DistortionIntensity
                float blobs = FractalOffset + DistortionIntensity * abs(sin(t * 1.2));
                // Apply logarithmic fractal transformation
                p = log(blobs + abs(p));
                // 'dotp' is an inverse squared length, used for scaling and influencing the fractal
                dotp = max(1.0 / dot(p.xyz, p.xyz), 0.2);
                // Scale 'sc' based on 'dotp' and a time-varying term (influenced by DistortionIntensity)
                sc *= dotp * (0.7 + DistortionIntensity * abs(sin(t * 1.2 + 1.57)));
                // Apply transformation to 'p'
                p *= dotp - 0.7;
            }

            // Calculate distance to the fractal surface
            float dist = abs(length(p.xyz) - ShapeSize) / sc; // 'ShapeSize' now controls the size
            float stepsize = dist / 4.0 + 1e-4; // Calculate step size for raymarching
            totdist += stepsize; // Accumulate total distance

            // Calculate fade based on distance and iteration
            float fade = 0.015 * exp(-float(i) * float(i) * stepsize * stepsize);
            // Mix colors: white and palette color, weighted by PaletteMix
            c += mix(vec3(1.0), H(M(sc), id), PaletteMix) * fade;
        }

        // Clamp final color to [0, 1] range
        c = clamp(c, 0.0, 1.0);
        // Apply gamma correction using GammaCorrection input
        c = pow(c, vec3(GammaCorrection));
        // Apply overall brightness and color pulse effect using InitialBrightness and ColorPulseSpeed
        c *= InitialBrightness + 0.5 * sin(TIME * ColorPulseSpeed);
        // Final fragment color with full alpha
        gl_FragColor = vec4(c, 1.0);
    }
    // Pass 1: Gaussian Blur Feedback (renders to main output)
    else if (PASSINDEX == 1) {
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec4 sum = vec4(0.0); // Accumulator for blurred color

        // Manually unroll 5x5 Gaussian kernel for blur
        float kernel[5];
        kernel[0] = 0.06136;
        kernel[1] = 0.24477;
        kernel[2] = 0.38774;
        kernel[3] = 0.24477;
        kernel[4] = 0.06136;

        // Apply blur kernel
        for (int i = -2; i <= 2; i++) {
            for (int j = -2; j <= 2; j++) {
                // Offset calculation for blur, scaled by BlurStrength input
                vec2 offset = vec2(float(i), float(j)) / RENDERSIZE.xy * BlurStrength;
                float weight = kernel[i + 2] * kernel[j + 2]; // Kernel weight
                sum += weight * IMG_NORM_PIXEL(BufferA, uv + offset); // Sample from BufferA (previous frame)
            }
        }

        gl_FragColor = sum; // Output the blurred sum
    }
}
