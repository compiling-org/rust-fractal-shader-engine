/*
{
    "CATEGORIES": [
        "Fractal",
        "Psychedelic",
        "Feedback"
    ],
    "DESCRIPTION": "A psychedelic feedback fractal with extensive controls for animation speed, geometry, color palettes, and post-processing effects. Explore an organic, evolving Mandelbrot/Julia set.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "GLOBAL_ANIM_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Overall speed multiplier for all animations."
        },
        {
            "NAME": "ZOOM",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 10.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Controls the zoom level of the fractal."
        },
        {
            "NAME": "PAN_X",
            "TYPE": "float",
            "MIN": -2.0,
            "MAX": 2.0,
            "DEFAULT": 0.0,
            "DESCRIPTION": "Pans the fractal view along the X-axis."
        },
        {
            "NAME": "PAN_Y",
            "TYPE": "float",
            "MIN": -2.0,
            "MAX": 2.0,
            "DEFAULT": 0.0,
            "DESCRIPTION": "Pans the fractal view along the Y-axis."
        },
        {
            "NAME": "MANDELBROT_ITERATIONS",
            "TYPE": "float",
            "MIN": 5.0,
            "MAX": 64.0,
            "DEFAULT": 10.0,
            "STEP": 1.0,
            "DESCRIPTION": "Iterations for the main Mandelbrot point tracking. Higher values increase detail but can slow down."
        },
        {
            "NAME": "JULIA_ITERATIONS",
            "TYPE": "float",
            "MIN": 5.0,
            "MAX": 60.0,
            "DEFAULT": 20.0,
            "STEP": 1.0,
            "DESCRIPTION": "Iterations for the Julia set drawing. Higher values increase complexity."
        },
        {
            "NAME": "INITIAL_ROT_SPEED_MAIN",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 20.0,
            "DEFAULT": 7.0,
            "DESCRIPTION": "Base rotation speed for the main fractal elements."
        },
        {
            "NAME": "INITIAL_ROT_SPEED_SUB",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 10.0,
            "DEFAULT": 5.0,
            "DESCRIPTION": "Base rotation speed for sub-elements within the fractal."
        },
        {
            "NAME": "ROTATION_AMOUNT_MULTIPLIER",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Multiplies the overall rotation amount of fractal elements."
        },
        {
            "NAME": "COLOR_ROT_SPEED",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.03,
            "DESCRIPTION": "Speed of color rotation/cycling effect."
        },
        {
            "NAME": "PERTURB_MAGNITUDE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "DESCRIPTION": "Magnitude of the subtle position perturbation applied to the fractal origin."
        },
        {
            "NAME": "PERTURB_FREQ",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.05,
            "DEFAULT": 0.007,
            "DESCRIPTION": "Frequency of the position perturbation."
        },
        {
            "NAME": "DECAY_RATE",
            "TYPE": "float",
            "MIN": 0.8,
            "MAX": 0.999,
            "DEFAULT": 0.995,
            "DESCRIPTION": "Rate at which the previous frame decays. Lower values mean faster fading trails."
        },
        {
            "NAME": "FRACTAL_DETAIL_MULTIPLIER",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.2,
            "DESCRIPTION": "Multiplier for the detail level of the fractal rendering."
        },
        {
            "NAME": "JULIA_COLOR_SCALE",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Scales the color contribution of the Julia set components."
        },
        {
            "NAME": "COLOR_MIX_ALPHA",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Alpha value for mixing new fractal colors onto the feedback."
        },
        {
            "NAME": "INITIAL_FRAME_OFFSET",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.001,
            "DESCRIPTION": "Initial offset multiplier for the fractal animation based on frame index."
        },
        {
            "NAME": "TRAIL_LENGTH_FACTOR",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.1,
            "DESCRIPTION": "Controls a factor in the fractal's trail generation."
        },
        {
            "NAME": "TRAIL_INTENSITY",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.1,
            "DESCRIPTION": "Controls the intensity/brightness of the generated trails."
        },
        {
            "NAME": "P_COORD_SCALE",
            "TYPE": "float",
            "MIN": 0.0001,
            "MAX": 0.01,
            "DEFAULT": 0.001,
            "DESCRIPTION": "Scale factor for 'p' coordinates in Julia set calculation."
        },
        {
            "NAME": "COLOR_PALETTE_SELECT",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 0.0,
            "STEP": 1.0,
            "DESCRIPTION": "Selects one of several psychedelic color palettes."
        },
        {
            "NAME": "BRIGHTNESS",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Adjusts the overall brightness of the output."
        },
        {
            "NAME": "CONTRAST",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Adjusts the contrast of the output."
        },
        {
            "NAME": "SATURATION",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "DESCRIPTION": "Adjusts the color saturation of the output."
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
            "INPUTS": [
                {
                    "NAME": "BufferA",
                    "TYPE": "image"
                }
            ]
        }
    ]
}
*/

#define PI 3.14159

mat2 rmat(float a){return mat2(cos(a),sin(a),-sin(a),cos(a));}
vec2 cmul(vec2 a, vec2 b){return vec2(a.x*b.x-a.y*b.y,dot(a,b.yx));}

// Helper function to get sign of a float
float sgn(float x){return (x<0.0?-1.0:1.0);}

// Psychedelic Color Palettes
vec3 getPaletteColor(float t, float palette_id) {
    t = fract(t); // Normalize t to 0-1 range

    if (palette_id < 0.5) { // Palette 0: Rainbow Spectrum
        return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
    } else if (palette_id < 1.5) { // Palette 1: Fiery Sunset
        return mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 0.5, 0.0), t);
    } else if (palette_id < 2.5) { // Palette 2: Electric Blue/Purple
        return mix(vec3(0.0, 0.5, 1.0), vec3(0.7, 0.0, 1.0), t);
    } else if (palette_id < 3.5) { // Palette 3: Acid Green/Yellow
        return mix(vec3(0.0, 0.8, 0.2), vec3(1.0, 1.0, 0.0), t);
    } else if (palette_id < 4.5) { // Palette 4: Retro Wave
        return mix(vec3(0.0, 0.0, 0.3), vec3(0.9, 0.0, 0.9), t);
    } else { // Palette 5: Desaturated to Vibrant burst
        vec3 grey = vec3(t);
        return mix(grey, 0.5 + 0.5 * sin(6.28318 * (t * 2.0 + vec3(0.0, 0.6, 0.3))), smoothstep(0.0, 1.0, t));
    }
}


void main() {
    if (PASSINDEX == 0) { // Pass 0: Compute fractal and write to BufferA (feedback loop)
        // Read previous frame from BufferA for feedback effect
        gl_FragColor = texture2D(BufferA, mod(gl_FragCoord.xy / RENDERSIZE.xy, 1.0));

        // Use local variable for floored fragment coordinates (fixes C7565 error)
        vec2 current_frag_coord = floor(gl_FragCoord.xy);

        // Frame index and global time, scaled by GLOBAL_ANIM_SPEED
        float f = float(FRAMEINDEX);
        float anim_time = TIME * GLOBAL_ANIM_SPEED;
        float a = floor(f) * INITIAL_FRAME_OFFSET * GLOBAL_ANIM_SPEED; 

        if (current_frag_coord.x + current_frag_coord.y == 0.0) {
            // This block runs only for the pixel at (0,0) and calculates the fractal's center point
            int iters = int(MANDELBROT_ITERATIONS); 
            a *= PI;
            vec2 p = vec2(cos(a), sin(a / 1.5));
            vec2 rd = -p;
            // Apply perturbation to the Mandelbrot point, scaled by tunable magnitude and frequency
            p.x -= PERTURB_MAGNITUDE * cos(a * PERTURB_FREQ); 

            // Mandelbrot iteration to find a point on the border
            for (int i = 0; i < 64; i++) { // Max iterations to prevent infinite loop
                if (i >= iters) break; // Break if desired iterations reached

                float dr = 1.0;
                float r = length(p);
                vec2 C = p;
                vec2 Z = p;
                for (int n = 0; n < 32 && r < 2000.0; n++) { // Max inner iterations for safety
                    Z = cmul(Z, Z) + C;
                    dr = dr * r * 2.0 + 1.0;
                    r = length(Z);
                }
                // Apply trail length factor and intensity
                p += rd * (1.0 + cos(a * TRAIL_LENGTH_FACTOR)) * TRAIL_INTENSITY; 
            }
            gl_FragColor = vec4(p.x, p.y, p.x, p.y); // Store the computed Mandelbrot point in BufferA (0,0)
        } else {
            // This block runs for all other pixels, drawing the Julia set based on the (0,0) pixel's data
            vec4 L = texture2D(BufferA, vec2(0.0, 0.0)); // Load p0 (Julia constant) from (0,0) of BufferA
            vec2 p0 = L.xy; // The complex constant for the Julia set

            // Normalized fragment coordinate, with zoom, pan controls
            vec2 u = ((2.0 * current_frag_coord - RENDERSIZE.xy) / RENDERSIZE.y) / ZOOM + vec2(PAN_X, PAN_Y); 

            for (int j = 0; j < 5; j++) {
                vec2 p = p0 + vec2(P_COORD_SCALE) * float(j); 
                for (int n = 0; n < 60; n++) { // Max inner iterations for safety
                    if (n >= int(JULIA_ITERATIONS) || dot(p, p) >= 300000.0) break; 

                    p = cmul(p, p) + p0;
                    float d = length(abs(u) - abs(p));
                    // Apply fractal detail multiplier
                    d = smoothstep(2.0 / RENDERSIZE.y, 0.0, d) * FRACTAL_DETAIL_MULTIPLIER; 

                    // Combine position, iteration, and time for color palette input
                    float color_t = length(p) * 0.1 + float(n) * 0.05 + anim_time * COLOR_ROT_SPEED * 0.1; 
                    vec3 fractal_base_color = getPaletteColor(color_t, COLOR_PALETTE_SELECT);

                    // Apply main rotation speed and multiplier
                    vec2 rotated_p = p * rmat(a * INITIAL_ROT_SPEED_MAIN * ROTATION_AMOUNT_MULTIPLIER + float(j) * 0.3);
                    vec3 col = fractal_base_color * d * JULIA_COLOR_SCALE * vec3(abs(rotated_p), 1.0); // Use rotated_p magnitude for part of color contribution

                    // Apply sub-rotation speed and multiplier
                    col.yz *= rmat(a * INITIAL_ROT_SPEED_SUB * ROTATION_AMOUNT_MULTIPLIER + float(j + n) * COLOR_ROT_SPEED); 
                    
                    // Mix new color with existing gl_FragColor based on alpha
                    gl_FragColor.rgb += abs(col.grb) * COLOR_MIX_ALPHA; 
                }
            }
            gl_FragColor.rgb *= DECAY_RATE; // Apply feedback decay rate
        }
    } else if (PASSINDEX == 1) { // Pass 1: Final output pass (apply post-processing)
        // Load the computed fractal from BufferA
        vec3 final_color = texture2D(BufferA, mod(gl_FragCoord.xy / RENDERSIZE.xy, 1.0)).rgb;

        // Apply Brightness
        final_color *= BRIGHTNESS;

        // Apply Contrast
        final_color = mix(vec3(0.5), final_color, CONTRAST);

        // Apply Saturation
        float lum = dot(final_color, vec3(0.2126, 0.7152, 0.0722)); // Luminance
        final_color = mix(vec3(lum), final_color, SATURATION);

        gl_FragColor = vec4(final_color, 1.0);
    }
}