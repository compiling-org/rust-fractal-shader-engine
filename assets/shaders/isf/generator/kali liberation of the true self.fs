/*
{
    "CATEGORIES": [
        "Psychedelic",
        "Fractal"
    ],
    "DESCRIPTION": "Enhanced shader with tunable parameters for psychedelic effects.",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 2.0 },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "morphing", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "geometry", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
        { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "transparency", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "cameraX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.0, "MAX": 3.0 },
        { "NAME": "cameraY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.0, "MAX": 3.0 },
        { "NAME": "colorPalette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, 
          "LABELS": ["Palette 1", "Palette 2", "Palette 3", "Palette 4"] }
    ]
}
*/

#define resolution RENDERSIZE
#define time TIME

vec3 objcol;

// Hash function for randomization
float hash12(vec2 p) {
    vec3 p3  = fract(vec3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// Rotation matrix
mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

// Distance estimation function
float de(vec3 pos) {
    float t = mod(time, 17.0);
    float a = smoothstep(13.0, 15.0, t) * 8.0 - smoothstep(4.0, 0.0, t) * 4.0;
    float f = sin(time * 5.0 + sin(time * 20.0) * 0.2);
    pos.xz *= rot(time + 0.5);
    pos.yz *= rot(time);
    vec3 p = pos;
    float s = 1.0;
    
    for (int i = 0; i < 4; i++) {
        p = abs(p) * 1.3 - 0.5 - f * 0.1 - a;
        p.xy *= rot(radians(45.0));
        p.xz *= rot(radians(45.0));
        s *= 1.3;
    }
    
    float fra = length(p) / s - 0.5;
    pos.xy *= rot(time);
    p = abs(pos) - 2.0 - a;
    float d = length(p) - 0.7;
    d = min(d, max(length(p.xz) - 0.1, p.y));
    d = min(d, max(length(p.yz) - 0.1, p.x));
    d = min(d, max(length(p.xy) - 0.1, p.z));
    p = abs(pos);
    p.x -= 4.0 + a + f * 0.5;
    d = min(d, length(p) - 0.7);
    d = min(d, length(p.yz - abs(sin(p.x * 0.5 - time * 10.0) * 0.3)));
    p = abs(pos);
    p.y -= 4.0 + a + f * 0.5;
    d = min(d, length(p) - 0.7);
    d = min(d, max(length(p.xz) - 0.1, p.y));
    d = min(d, fra);
    objcol = abs(p);
    if (d == fra) objcol = vec3(2.0, 0.0, 0.0);
    return d;
}

// Normal calculation
vec3 normal(vec3 p) {
    vec2 d = vec2(0.0, 0.01);
    return normalize(vec3(de(p + d.yxx), de(p + d.xyx), de(p + d.xxy)) - de(p));
}

// Marching function
vec3 march(vec3 from, vec3 dir) {
    float d = 0.0, td = 0.0, maxdist = 30.0;
    vec3 p = from, col = vec3(0.0);
    
    for (int i = 0; i < 100; i++) {
        float d2 = de(p) * (1.0 - hash12(gl_FragCoord.xy + time) * 0.2);
        if (d2 < 0.0) {
            vec3 n = normal(p);
            dir = reflect(dir, n);
            d2 = 0.1;
        }
        d = max(0.01, abs(d2));
        p += d * dir;
        td += d;
        if (td > maxdist) break;
        col += 0.01 * objcol;
    }
    return pow(col, vec3(2.0));
}

// Color palette function
vec3 getColor(float t) {
    t = fract(t);
    if (colorPalette < 1.0) {
        // Palette 1: Rainbow
        return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
    } else if (colorPalette < 2.0) {
        // Palette 2: Fire
        return vec3(
            min(1.0, t * 1.5),
            min(1.0, max(0.0, t - 0.3)),
            0.0
        );
    } else if (colorPalette < 3.0) {
        // Palette 3: Acid
        return fract(vec3(t * 0.61, t * 0.83, t * 0.47));
    } else {
        // Palette 4: Neon
        return abs(fract(vec3(t * 3.0, t * 5.0, t * 7.0)) * 2.0 - 1.0);
    }
}

// Function to adjust brightness and contrast
vec3 adjustBrightnessContrast(vec3 color, float brightness, float contrast) {
    color *= brightness; // Adjust brightness
    color = (color - 0.5) * contrast + 0.5; // Adjust contrast
    return clamp(color, 0.0, 1.0); // Ensure color is within valid range
}

void main() {
    vec2 uv = gl_FragCoord.xy / resolution.xy - 0.5;
    uv.x *= resolution.x / resolution.y;
    
    // Camera movement
    vec3 from = vec3(cameraX, cameraY, -10.0 + zoom); // Adjusted for zoom
    vec3 dir = normalize(vec3(uv, 1.0));
    
    // Apply morphing effect
    dir.x += sin(time * morphing) * 0.1;
    dir.y += cos(time * morphing) * 0.1;

    vec3 col = march(from, dir);
    
    // Apply color pulse effect
    col *= getColor(time * colorPulse);
    
    // Adjust brightness and contrast
    col = adjustBrightnessContrast(col, brightness, contrast);
    
    // Set background transparency
    gl_FragColor = vec4(col, transparency);
}