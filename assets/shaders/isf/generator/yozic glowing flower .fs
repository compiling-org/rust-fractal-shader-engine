/* {
    "CATEGORIES": [
        "Psychedelic",
        "Kaleidoscope",
        "Orbs"
    ],
    "DESCRIPTION": "Psychedelic kaleidoscope with tunable parameters",
    "IMPORTED": {},
    "INPUTS": [
        {
            "NAME": "RotationSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Rotation Speed"
        },
        {
            "NAME": "OrbCount",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 50.0,
            "DEFAULT": 20.0,
            "LABEL": "Orb Count"
        },
        {
            "NAME": "OrbSize",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.39,
            "LABEL": "Orb Size"
        },
        {
            "NAME": "Contrast",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.37,
            "LABEL": "Contrast"
        },
        {
            "NAME": "Brightness",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Brightness"
        },        
        {
            "NAME": "PulseSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Pulse Speed"
        },
        {
            "NAME": "KaleidoSides",
            "TYPE": "float",
            "MIN": 3.0,
            "MAX": 12.0,
            "DEFAULT": 6.0,
            "LABEL": "Kaleido Sides"
        },
        {
            "NAME": "WarpIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Warp Intensity"
        },
        {
            "NAME": "ColorScheme",
            "TYPE": "long",
            "VALUES": ["Rainbow", "Electric", "Fire", "Ocean", "Pastel"],
            "DEFAULT": "Rainbow",
            "LABEL": "Color Scheme"
        }
    ]
} */

#define PI 3.14159265359

vec2 kale(vec2 uv, vec2 offset, float sides) {
    float angle = atan(uv.y, uv.x);
    angle = ((angle / PI) + 1.0) * 0.5;
    angle = mod(angle, 1.0 / sides) * sides;
    angle = -abs(2.0 * angle - 1.0) + 1.0;
    float y = length(uv);
    angle = angle * y;
    return vec2(angle, y) - offset;
}

vec4 orb(vec2 uv, float size, vec2 position, vec3 color, float contrast) {
    float dist = clamp(size / length(uv + position), 0.0, 10.0);
    return pow(vec4(dist * color, 1.0), vec4(contrast));
}

mat2 rotate(float angle) {
    return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

vec3 getColor(int scheme, float index) {
    float t = index / float(OrbCount);
    
    if (scheme == 0) { // Rainbow
        return 0.5 + 0.5 * cos(2.0 * PI * (t + vec3(0.0, 0.33, 0.66)));
    }
    else if (scheme == 1) { // Electric
        return vec3(
            abs(sin(t * PI * 3.0)),
            abs(sin(t * PI * 3.0 + PI * 0.66)),
            abs(sin(t * PI * 3.0 + PI * 1.33))
        );
    }
    else if (scheme == 2) { // Fire
        return vec3(
            0.8 + 0.2 * sin(t * PI * 4.0),
            0.5 + 0.5 * sin(t * PI * 2.0),
            0.2 * sin(t * PI)
        );
    }
    else if (scheme == 3) { // Ocean
        return vec3(0.2, 0.5, 0.7) + 0.3 * cos(t * PI * 2.0 + vec3(0.0, 0.33, 0.66));
    }
    else { // Pastel
        return 0.6 + 0.4 * sin(t * PI * 2.0 + vec3(0.0, 0.33, 0.66));
    }
}

void main() {
    // Convert long inputs to usable numbers
    float orbCount = OrbCount; // Use float for orb count
    float sides = KaleidoSides; // Use float for kaleido sides
    
    // Base setup
    vec2 uv = 23.09 * (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    gl_FragColor = vec4(0.0);
    
    // Apply warping with intensity control
    uv.x += WarpIntensity * sin(0.3 * uv.y + TIME * PulseSpeed);
    uv.y -= WarpIntensity * cos(0.53 * uv.x + TIME * PulseSpeed);
    
    // Rotation controls
    uv *= rotate(TIME * RotationSpeed * 0.1);
    uv = kale(uv, vec2(6.97), sides);
    uv *= rotate(TIME * RotationSpeed * 0.05);

    // Orb rendering
    for (int i = 0; i < 50; i++) {
        if (float(i) >= orbCount) break; // Use float comparison
        
        float t = float(i) * PI * 2.0 / orbCount;
        float x = 4.02 * tan(t + TIME * PulseSpeed * 0.1);
        float y = 4.02 * cos(t - TIME * PulseSpeed * 0.03);
        
        vec3 color = getColor(int(ColorScheme), float(i)); // Convert ColorScheme to int
        gl_FragColor += orb(uv, OrbSize, vec2(x, y), color, Contrast) * Brightness;
    }
    
    // Apply pulse effect
    float pulse = 0.5 + 0.5 * sin(TIME * PulseSpeed);
    gl_FragColor.rgb *= (0.8 + 0.2 * pulse);
    
    // Final color correction
    gl_FragColor = clamp(gl_FragColor, 0.0, 1.0);
}