/* {
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Fully functional psychedelic fractal with adjustable color palettes and pulses",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "AudioReactivity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "ColorIntensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "Zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "DistortionAmount",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "ColorPalette",
            "TYPE": "long",
            "VALUES": ["Rainbow", "Fiery", "Electric", "Pastel", "Custom"],
            "DEFAULT": "Rainbow"
        },
        {
            "NAME": "PulseSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "PulseIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "CustomColor1",
            "TYPE": "color",
            "DEFAULT": [1.0, 0.5, 0.0, 1.0]
        },
        {
            "NAME": "CustomColor2",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.5, 1.0, 1.0]
        }
    ],
    "PASSES": [
        {
            "TARGET": "BufferA",
            "PERSISTENT": true
        },
        {}
    ]
} */

#define PI 3.14159265359

// Audio reactive pulse
float audioPulse() {
    return 0.5 + 0.5 * sin(TIME * PulseSpeed * 5.0) * PulseIntensity;
}

// Get color from selected palette
vec3 getPaletteColor(float t) {
    t = fract(t);
    if (ColorPalette == 0) { // Rainbow
        return 0.5 + 0.5 * cos(2.0 * PI * (t + vec3(0.0, 0.33, 0.66)));
    }
    else if (ColorPalette == 1) { // Fiery
        return vec3(
            0.8 + 0.2 * sin(t * 5.0),
            0.3 * t,
            0.1
        );
    }
    else if (ColorPalette == 2) { // Electric
        return vec3(
            abs(sin(t * 7.0)),
            abs(sin(t * 7.0 + 2.0)),
            abs(sin(t * 7.0 + 4.0))
        );
    }
    else if (ColorPalette == 3) { // Pastel
        return 0.7 + 0.3 * cos(2.0 * PI * (t + vec3(0.1, 0.3, 0.5)));
    }
    else { // Custom
        return mix(CustomColor1.rgb, CustomColor2.rgb, t);
    }
}

// Get reactive fractal color with all parameters connected
vec3 getFractalColor(vec2 p) {
    float t = TIME * AnimationSpeed;
    float audio = sin(t * AudioReactivity * 10.0) * 0.5 + 0.5;
    
    p *= Zoom * (1.0 + audio * DistortionAmount * 0.5);
    p = vec2(atan(p.x, p.y)/PI, length(p) * (1.0 + sin(t * 0.4) * 0.2));
    
    vec3 c = vec3(0);
    for (int i = 0; i < 5; i++) {
        p = abs(p) / dot(p,p) - 0.5;
        c += getPaletteColor(p.x * 0.2 + p.y + t * 0.2) * 0.2;
    }
    
    return c * ColorIntensity * (1.0 + audioPulse() * 0.5);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    if (PASSINDEX == 0) {
        vec3 col = getFractalColor(uv * 5.0);
        col *= 1.0 + 0.2 * sin(uv.x * 100.0 + TIME * 5.0); // Sparkle effect
        gl_FragColor = vec4(col, 1.0);
    }
    else {
        // Add feedback effect
        vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
        vec3 col = IMG_NORM_PIXEL(BufferA, p).rgb * 0.9;
        gl_FragColor = vec4(col, 1.0);
    }
}
