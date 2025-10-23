/*{
  "CATEGORIES": ["Psychedelic", "Fractal", "Mandelbox", "Kaleidoscope", "Transform"],
  "DESCRIPTION": "An immersive Mandelbox fractal kaleidoscope. Explores intricate recursive geometry with tunable folding, symmetry, and a wide array of dynamic color palettes.",
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    }
  ],
  "INPUTS": [
    {"NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5},
    {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3},
    {"NAME": "MandelboxIterations", "TYPE": "float", "DEFAULT": 20.0, "MIN": 10.0, "MAX": 40.0},
    {"NAME": "MandelboxScale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0},
    {"NAME": "FoldingAmount", "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "KaleidoscopeSymmetry", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "KaleidoscopeRotationSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0, "MAX": 2},
    {"NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.20, "MIN": 0.1, "MAX": 4},
    {"NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 1, "MIN": 0, "MAX": 19, "VALUES": [
      {"NAME":"Vibrant", "VALUE":0},{"NAME":"Psycho", "VALUE":1},{"NAME":"Pastel", "VALUE":2},{"NAME":"Neon", "VALUE":3},{"NAME":"Deep", "VALUE":4},
      {"NAME":"Sunset", "VALUE":5},{"NAME":"Ocean", "VALUE":6},{"NAME":"Night", "VALUE":7},{"NAME":"Retro", "VALUE":8},
      {"NAME":"Fire", "VALUE":9},{"NAME":"Ice", "VALUE":10},{"NAME":"Galaxy", "VALUE":11},{"NAME":"Aurora", "VALUE":12},
      {"NAME":"Rainforest", "VALUE":13},{"NAME":"Desert", "VALUE":14},{"NAME":"Vintage", "VALUE":15},{"NAME":"Electric", "VALUE":16},
      {"NAME":"Frost", "VALUE":17},{"NAME":"Sunrise", "VALUE":18},{"NAME":"Dusk", "VALUE":19}
    ]},
    {"NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0, "MAX": 3},
    {"NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3},
    {"NAME": "FeedbackAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0, "MAX": 1},
    {"NAME": "Glow", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0, "MAX": 2}
  ]
}*/

// Define constants
#define PI 3.141592
#define MAX_STEPS 256 // Increased max steps for better quality
#define BAILOUT 16.0
// Define max iterations for loops with tunable limits (GLSL ES 1.0 workaround)
#define MAX_MANDELBOX_ITERATIONS_CONST 20 // Max from input range for MandelboxIterations
#define MAX_KALEIDOSCOPE_SYMMETRY_CONST 4 // Max from input range for KaleidoscopeSymmetry

// Rotation matrix for 2D
mat2 rot2(float a) {
    float c= cos(a);
    float s= sin(a);
    return mat2(c,s,-s,c);
}

// Palette function (from user's working shader)
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

// Color palettes (from user's working shader)
vec3 getColorPalette(int mode, float t) {
    if (mode==0) return pal(t, vec3(0.8,0.2,1.0), vec3(1.0,0.5,0.2), vec3(1.0,1.0,0.2), vec3(0.1,0.2,0.5)); // Vibrant
    if (mode==1) return pal(t, vec3(0.9,0.1,0.3), vec3(0.3,0.9,0.2), vec3(0.2,0.4,0.9), vec3(1.0,0.2,0.9)); // Psycho
    if (mode==2) return pal(t, vec3(0.2,0.8,0.9), vec3(0.9,0.4,0.1), vec3(0.5,0.1,0.7), vec3(1.0,1.0,0.5)); // Pastel
    if (mode==3) return pal(t, vec3(1.0,0.0,0.5), vec3(0.0,1.0,0.5), vec3(0.5,0.0,1.0), vec3(1.0,1.0,1.0)); // Neon
    if (mode==4) return pal(t, vec3(0.3,0.6,0.9), vec3(0.9,0.3,0.6), vec3(0.6,0.9,0.3), vec3(1.0,0.7,0.2)); // Deep
    if (mode==5) return pal(t, vec3(1.0,0.5,0.2), vec3(0.2,0.8,0.5), vec3(0.8,0.2,0.5), vec3(0.5,0.5,0.5)); // Sunset
    if (mode==6) return pal(t, vec3(0.0,0.5,1.0), vec3(1.0,0.0,0.5), vec3(0.5,1.0,0.0), vec3(0.2,0.4,0.6)); // Ocean
    if (mode==7) return pal(t, vec3(0.1,0.1,0.3), vec3(0.3,0.1,0.1), vec3(0.1,0.3,0.1), vec3(0.6,0.6,0.6)); // Night
    if (mode==8) return pal(t, vec3(1.0,0.8,0.2), vec3(0.2,1.0,0.8), vec3(0.8,0.2,1.0), vec3(0.4,0.2,0.6)); // Retro
    if (mode==9) return pal(t, vec3(1.0,0.3,0.0), vec3(0.0,1.0,0.3), vec3(0.3,0.0,1.0), vec3(0.2,0.4,0.6)); // Fire
    if (mode==10) return pal(t, vec3(0.0,0.5,1.0), vec3(1.0,1.0,0.0), vec3(0.5,0.0,0.0), vec3(0.2,0.4,0.6)); // Ice
    if (mode==11) return pal(t, vec3(0.6,0.1,0.7), vec3(0.2,0.3,0.8), vec3(0.9,0.4,0.2), vec3(0.7,0.7,0.7)); // Galaxy
    if (mode==12) return pal(t, vec3(0.0,1.0,0.5), vec3(1.0,0.0,0.5), vec3(0.5,1.0,0.0), vec3(0.2,0.4,0.6)); // Aurora
    if (mode==13) return pal(t, vec3(0.0,0.4,0.0), vec3(0.4,0.8,0.4), vec3(0.2,0.6,0.2), vec3(0.2,0.4,0.6)); // Rainforest
    if (mode==14) return pal(t, vec3(0.9,0.8,0.5), vec3(0.8,0.7,0.2), vec3(0.6,0.4,0.2), vec3(0.2,0.4,0.6)); // Desert
    if (mode==15) return pal(t, vec3(0.9,0.7,0.3), vec3(0.4,0.2,0.1), vec3(0.8,0.6,0.4), vec3(0.1,0.2,0.3)); // Vintage
    if (mode==16) return pal(t, vec3(1.0,0.0,1.0), vec3(0.0,1.0,1.0), vec3(1.0,1.0,0.0), vec3(0.2,0.4,0.6)); // Electric
    if (mode==17) return pal(t, vec3(0.8,0.9,1.0), vec3(0.2,0.3,0.4), vec3(0.5,0.6,0.7), vec3(0.2,0.4,0.6)); // Frost
    if (mode==18) return pal(t, vec3(1.0,0.5,0.0), vec3(0.6,0.2,0.1), vec3(0.9,0.4,0.2), vec3(0.7,0.3,0.2)); // Sunrise
    if (mode==19) return pal(t, vec3(0.2,0.2,0.2), vec3(0.4,0.4,0.4), vec3(0.6,0.6,0.6), vec3(0.8,0.8,0.8)); // Dusk
    return pal(t, vec3(0.5), vec3(0.5), vec3(0.5), vec3(0.0));
}

// Mandelbox distance function (adapted from original, with tunable parameters)
float shapeMandelboxExtended(vec3 p, float iterations, float scale, float foldingAmount) {
    vec3 z = p;
    // Loop runs up to a constant max, breaks early based on 'iterations' uniform
    for (int i = 0; i < MAX_MANDELBOX_ITERATIONS_CONST; i++) {
        if (float(i) >= iterations) break; // Early exit based on tunable input

        // Box fold (clamp and mirror)
        z = clamp(z, -1.0, 1.0) * 2.0 - z;
        
        // Sphere fold (if inside/outside specific radius)
        float r2 = dot(z, z);
        if (r2 < 0.25) { // Inside inner sphere
            z *= 4.0;
        } else if (r2 < 4.0) { // Outside inner, inside outer sphere
            float c = (scale - 1.0) / r2;
            z *= c;
        }
        
        z = z * scale + p * foldingAmount; // Apply scale and add original point back
    }
    return length(z) - 1.0; // FIX: Return length(z) - 1.0, like the working example
}

// Kaleidoscope transformation (new)
vec3 applyKaleidoscope(vec3 p, float symmetry, float rotationSpeed, float t) {
    // Apply symmetry folding based on KaleidoscopeSymmetry input
    // Loop runs up to a constant max, breaks early based on 'symmetry' uniform
    for(int i = 0; i < MAX_KALEIDOSCOPE_SYMMETRY_CONST; i++) {
        if (float(i) >= symmetry) break; // Early exit based on tunable input
        p = abs(p); // Fold all axes
        p = p * 2.0 - 1.0; // Scale and translate for repeating folds
    }

    // Apply rotation
    float angle = t * rotationSpeed;
    p.xy *= rot2(angle);
    p.xz *= rot2(angle * 0.7); // Different rotation speeds for more complexity
    p.yz *= rot2(angle * 0.5);
    
    return p;
}

// Main fragment shader
void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * Speed;

    vec3 origin = vec3(0.0, 0.0, -3.0);
    vec3 dir = normalize(vec3(uv * Zoom, 1.0));

    // Feedback buffer sampling (from user's working shader)
    vec4 feedbackColor = texture2D(BufferA, fract(gl_FragCoord.xy / RENDERSIZE.xy));
    vec3 feedback = feedbackColor.rgb;

    vec3 color = vec3(0.0);
    float dist = 0.0;

    // Parameters from INPUTS
    float mandelboxIterations = MandelboxIterations;
    float mandelboxScale = MandelboxScale;
    float foldingAmount = FoldingAmount;
    float kaleidoscopeSymmetry = KaleidoscopeSymmetry;
    float kaleidoscopeRotationSpeed = KaleidoscopeRotationSpeed;
    float chaos = ChaosIntensity;
    float chaosspeed = ChaosSpeed;
    float brightness = Brightness;
    float contrast = Contrast;
    float glow = Glow;
    float feedbackAmt = FeedbackAmount;

    for (int i = 0; i < MAX_STEPS; i++) { // This loop already uses a constant MAX_STEPS
        vec3 p = origin + dir * dist;
        
        // Apply kaleidoscope transformation
        p = applyKaleidoscope(p, kaleidoscopeSymmetry, kaleidoscopeRotationSpeed, t);

        // Calculate distance to Mandelbox
        float d = shapeMandelboxExtended(p, mandelboxIterations, mandelboxScale, foldingAmount);
        
        // Add some general chaotic distortion to the distance for more psychedelic feel
        d += sin(p.x * 3.0 + t * chaosspeed) * chaos * 0.05;
        d += sin(p.y * 4.0 + t * chaosspeed * 1.2) * chaos * 0.05;
        d += sin(p.z * 5.0 + t * chaosspeed * 0.8) * chaos * 0.05;

        dist += max(abs(d), 0.001); // FIX: Smaller minimum step for more precision

        float fade = exp(-float(i) * 0.05);
        vec3 col = getColorPalette(int(ColorPaletteMode), p.z * 0.1 + t * 0.2);
        float brightnessVal = 0.01 / (0.01 + d * d); // FIX: Increased constant for more brightness
        color += brightnessVal * fade * col;
        
        if (dist > BAILOUT) break;
    }

    // Psychedelic pulse (using ChaosSpeed for animation)
    float pulse = sin(t * ChaosSpeed) * 0.5 + 0.5;
    color *= 1.0 + 0.3 * pulse;

    // Apply contrast and brightness
    color = (color - 0.5) * contrast + 0.5;
    color *= brightness;

    // Mix feedback and glow
    color = mix(color, feedback, feedbackAmt);
    color *= glow;

    gl_FragColor = vec4(color, 1.0);
}
