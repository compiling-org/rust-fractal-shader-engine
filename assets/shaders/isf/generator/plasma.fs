/*{
  "CATEGORIES": ["Raymarching", "Psychedelic", "Feedback", "Enhanced"],
  "DESCRIPTION": "Highly customizable psychedelic fractal with feedback, multiple palettes, advanced controls.",
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    }
  ],
  "INPUTS": [
    {"NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
    {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
    {"NAME": "Morph", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.01, "MAX": 2.0},
    {"NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
    {"NAME": "PaletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0, "MAX": 10},
    {"NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3},
    {"NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3},
    {"NAME": "GlowAmount", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2},
    {"NAME": "RotationSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 2},
    {"NAME": "Distortion", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2},
    {"NAME": "FeedbackIntensity", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1},
    {"NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1}
  ]
}*/

#define PI 3.141592

// Rotation matrix
mat2 rot2(float a) {
    return mat2(cos(a), sin(a), -sin(a), cos(a));
}

// The core palette function
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

// Palette functions, each calling 'pal' with different parameters
vec3 palette0(float t) { return pal(t, vec3(0.8,0.2,1.0), vec3(1.0,0.5,0.2), vec3(1.0,1.0,0.2), vec3(0.1,0.2,0.5)); }
vec3 palette1(float t) { return pal(t, vec3(0.9,0.1,0.3), vec3(0.3,0.9,0.2), vec3(0.2,0.4,0.9), vec3(1.0,0.2,0.9)); }
vec3 palette2(float t) { return pal(t, vec3(0.2,0.8,0.9), vec3(0.9,0.4,0.1), vec3(0.5,0.1,0.7), vec3(1.0,1.0,0.5)); }
vec3 palette3(float t) { return pal(t, vec3(1.0,0.0,0.5), vec3(0.0,1.0,0.5), vec3(0.5,0.0,1.0), vec3(1.0,1.0,1.0)); }
vec3 palette4(float t) { return pal(t, vec3(0.9,0.6,0.2), vec3(0.2,0.9,0.4), vec3(0.4,0.2,0.9), vec3(0.9,0.2,0.4)); }
vec3 palette5(float t) { return pal(t, vec3(0.1,0.9,0.7), vec3(0.9,0.1,0.7), vec3(0.7,0.9,0.1), vec3(0.3,0.3,0.3)); }
vec3 palette6(float t) { return pal(t, vec3(1.0,0.5,0.0), vec3(0.0,1.0,0.5), vec3(0.5,0.0,1.0), vec3(0.9,0.9,0.3)); }
vec3 palette7(float t) { return pal(t, vec3(0.5,0.2,0.8), vec3(0.8,0.5,0.2), vec3(0.2,0.8,0.5), vec3(1.0,0.5,0.5)); }
vec3 palette8(float t) { return pal(t, vec3(0.0,0.9,0.9), vec3(0.9,0.0,0.9), vec3(0.9,0.9,0.0), vec3(0.3,0.3,0.3)); }
vec3 palette9(float t) { return pal(t, vec3(0.3,0.6,0.9), vec3(0.9,0.3,0.6), vec3(0.6,0.9,0.3), vec3(1.0)); }

// Function to select palette based on index
vec3 getPaletteColor(int idx, float t) {
    if (idx == 0) return palette0(t);
    if (idx == 1) return palette1(t);
    if (idx == 2) return palette2(t);
    if (idx == 3) return palette3(t);
    if (idx == 4) return palette4(t);
    if (idx == 5) return palette5(t);
    if (idx == 6) return palette6(t);
    if (idx == 7) return palette7(t);
    if (idx == 8) return palette8(t);
    if (idx == 9) return palette9(t);
    // fallback
    return pal(t, vec3(1.0), vec3(0.5), vec3(0.0), vec3(0.0));
}

// Fractal field with transformations
float field(vec3 p, float time, float morph, float distortion, float rotationSpeed) {
    // Apply rotation over time
    float angle = time * rotationSpeed;
    p.xy *= rot2(angle);
    p.xz *= rot2(angle * 1.1);
    p.yz *= rot2(angle * 1.2);
    // Apply distortion
    p += distortion * vec3(
        sin(p.y * 3.0 + time),
        sin(p.z * 2.0 + time * 0.8),
        sin(p.x * 4.0 + time * 1.2)
    );
    // Recursive fractal-like pattern
    for (int i=0; i<4; i++) {
        p = abs(p)/dot(p,p) - vec3(0.6 + 0.4 * sin(time + float(i)));
    }
    return length(p.xy) - 0.5;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // Inputs
    float speed = Speed;
    float zoom = Zoom;
    float morph = Morph;
    float colorPulseSpeed = ColorPulseSpeed;
    int paletteIdx = int(PaletteIndex);
    float brightness = Brightness;
    float contrast = Contrast;
    float glow = GlowAmount;
    float rotSpeed = RotationSpeed;
    float distortion = Distortion;
    float feedbackAmt = FeedbackIntensity;
    float colorShift = ColorShift;

    float t = TIME * speed;

    // Camera setup
    vec3 camPos = vec3(0.0, 0.0, -2.0);
    vec3 rayDir = normalize(vec3(uv * zoom, 1.0));

    vec3 origin = camPos;
    float dist = 0.0;
    vec3 colorAccum = vec3(0.0);

    // Feedback buffer
    vec3 feedback = IMG_NORM_PIXEL(BufferA, fract(gl_FragCoord.xy / RENDERSIZE.xy)).rgb;

    for (int i=0; i<64; i++) {
        vec3 p = origin + rayDir * dist;
        float d = field(p, t, morph, distortion, rotSpeed);
        dist += max(abs(d), 0.01);
        float fade = exp(-float(i)*0.05);
        float brightnessVal = 0.005 / (0.01 + d*d);
        vec3 col = getPaletteColor(paletteIdx, p.z + t*0.2 + colorShift);
        colorAccum += brightnessVal * fade * col;
        if (dist > 4.0) break;
    }

    // Apply color pulse
    float pulse = sin(TIME * colorPulseSpeed) * 0.5 + 0.5;
    colorAccum *= 1.0 + pulse;

    // Apply brightness and contrast
    colorAccum = (colorAccum - 0.5) * contrast + 0.5;
    colorAccum *= brightness;

    // Add glow / feedback
    colorAccum = mix(colorAccum, feedback, feedbackAmt);
    colorAccum *= glow;

    // Final color
    gl_FragColor = vec4(colorAccum, 1.0);
}