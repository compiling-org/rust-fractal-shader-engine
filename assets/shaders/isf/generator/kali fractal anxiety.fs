/*
{
    "DESCRIPTION": "Complete psychedelic fractal shader with full parameter controls",
    "CATEGORIES": ["Psychedelic", "Fractal"],
    "INPUTS": [
        {
            "NAME": "zoom",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "color_pulse",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 5.0,
            "DEFAULT": 1.0,
            "LABEL": "Color Pulse Frequency"
        },
        {
            "NAME": "palette_index",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 11.0,
            "DEFAULT": 0.0,
            "LABEL": "Color Palette",
            "VALUES": ["Purple","Indigo","Teal","Maroon","Yellow","Orange","Red","Magenta","Cyan","Green","Pink","White"]
        },
        {
            "NAME": "fractal_scale",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 3.0,
            "DEFAULT": 1.4,
            "LABEL": "Fractal Scale"
        },
        {
            "NAME": "glow_intensity",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.2,
            "LABEL": "Glow Intensity"
        }
    ],
    "ISFVSN": "2"
}
*/

// Full color palette definition
vec3 palette0 = vec3(0.56, 0.0, 1.0); // Purple
vec3 palette1 = vec3(0.3, 0.0, 0.5);  // Indigo
vec3 palette2 = vec3(0.0, 0.2, 0.2);  // Teal
vec3 palette3 = vec3(0.6, 0.2, 0.2);  // Maroon
vec3 palette4 = vec3(1.0, 1.0, 0.0);  // Yellow
vec3 palette5 = vec3(1.0, 0.5, 0.0);  // Orange
vec3 palette6 = vec3(1.0, 0.0, 0.0);  // Red
vec3 palette7 = vec3(1.0, 0.0, 1.0);  // Magenta
vec3 palette8 = vec3(0.0, 1.0, 1.0);  // Cyan
vec3 palette9 = vec3(0.0, 1.0, 0.0);  // Green
vec3 palette10 = vec3(1.0, 0.0, 0.5); // Pink
vec3 palette11 = vec3(1.0, 1.0, 1.0); // White

// Color selection with float-based index
vec3 getPaletteColor(float index) {
    int i = int(floor(index + 0.5)); // Add 0.5 for proper rounding when casting to int
    if (i == 0) return palette0;
    if (i == 1) return palette1;
    if (i == 2) return palette2;
    if (i == 3) return palette3;
    if (i == 4) return palette4;
    if (i == 5) return palette5;
    if (i == 6) return palette6;
    if (i == 7) return palette7;
    if (i == 8) return palette8;
    if (i == 9) return palette9;
    if (i == 10) return palette10;
    return palette11; // Default to white
}

// Smooth color gradient between palette colors
// 'c' is the base color value (e.g., from fractal distance, time)
// 's' is the smoothness factor
// 'basePaletteIndex' is the offset from the palette_index input
vec3 getsmcolor(float c, float s, float basePaletteIndex) {
    s *= 0.5;
    // Add basePaletteIndex to shift the starting point in the palette cycle
    // We use 12.0 for the total number of palettes for modulo operation
    c = c + basePaletteIndex; 
    c = c - 0.5 - 12.0 * floor((c - 0.5) / 12.0); // Use 12.0 for 12 palettes
    
    float i1 = floor(c);
    float i2 = i1 + 1.0;
    if (i2 > 11.0) i2 = 0.0; // Wrap around to the first palette color

    vec3 color1 = getPaletteColor(i1);
    vec3 color2 = getPaletteColor(i2);

    float mixVal = smoothstep(0.5 - s, 0.5 + s, fract(c));
    return mix(color1, color2, mixVal);
}

// Main rendering function
void main() {
    // Normalize coordinates
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (uv - 0.5);
    
    // Apply aspect ratio correction and zoom
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    p *= zoom;
    
    // Time-based animation
    float t = TIME * speed;
    float a = t * 0.075;   
    float b = t * 60.0;    
    
    // Fractal orbit trap
    float ot = 1000.0;
    mat2 rot = mat2(cos(a), sin(a), -sin(a), cos(a));
    p += sin(b) * 0.005;
    float l = length(p);
    
    // Fractal iteration loop
    for(int i = 0; i < 20; i++) {
        p *= rot;
        p = abs(p) * fractal_scale - 1.0;
        ot = min(ot, abs(dot(p,p) - sin(b + l * 20.0) * 0.015 - 0.15));
    }
    
    // Post-process orbit trap value
    ot = max(0.0, 0.1 - ot) / 0.1;
    
    // Color application with pulse control
    // --- CRITICAL FIX: Pass palette_index to getsmcolor ---
    vec3 color = getsmcolor(ot * 4.0 + l * 10.0 - t * 7.0 * color_pulse, 1.0, palette_index);
    color *= (1.0 - 0.4 * step(0.5, 1.0 - dot(p,p)));
    
    // Color processing
    color = mix(vec3(length(color)) * 0.5, color, 0.6);
    
    // Glow effects
    color *= 1.0 - pow(l * 1.1, 5.0); 
    color += pow(max(0.0, 0.2 - l)/0.2, 3.0) * glow_intensity;
    
    // Final output
    gl_FragColor = vec4(color, 1.0);
}