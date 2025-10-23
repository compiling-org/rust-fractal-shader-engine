/*{
  "DESCRIPTION": "Psychedelic fractal pulse with warping, distortion, rotation, centering, and color palettes.",
  "CATEGORIES": ["Fractal", "Psychedelic", "Generative"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.061, "MIN": 0.01, "MAX": 2.0, "LABEL": "Animation Speed" },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 5.5, "MIN": 0.5, "MAX": 100.0, "LABEL": "Zoom Level" },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fractal Morph Strength" },
    { "NAME": "warp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Base Warp" },
    { "NAME": "motionWarp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Motion Warp" },
    { "NAME": "motionDistort", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Motion Distort" },
    { "NAME": "rotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14, "LABEL": "Rotation Offset" },
    { "NAME": "centerShiftX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "LABEL": "Center X Offset" },
    { "NAME": "centerShiftY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "LABEL": "Center Y Offset" },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0, "LABEL": "Brightness" },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
    {
      "NAME": "colorPalette",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Color Palette",
      "VALUES": ["Classic", "Acid", "Lava", "Ice"]
    }
  ]
}*/

#define PI 3.14159265359

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

vec3 getPalette(float t, int style) {
    if (style == 1) return vec3(0.5 + 0.5 * sin(PI * t + vec3(0.0, 2.0, 4.0)));         // Acid
    else if (style == 2) return vec3(1.2, 0.3, 0.1) * sin(2.0 * PI * t);                // Lava
    else if (style == 3) return vec3(0.2, 0.4, 1.0) * cos(2.5 * PI * t);                // Ice
    return vec3(0.5 + 0.5 * cos(2.0 * PI * t + vec3(0.0, 0.5, 1.0)));                   // Classic
}

vec3 fractal(vec2 p, float t) {
    // Apply center shift and rotation
    p -= vec2(centerShiftX, centerShiftY);
    p *= rot(rotation);

    // Apply warping
    float radial = length(p);
    float theta = atan(p.y, p.x);
    radial += sin(theta * 8.0 + t * 2.0) * warp * 0.05;
    theta += sin(radial * 4.0 + t) * motionWarp * 0.1;
    p = vec2(cos(theta), sin(theta)) * radial;

    // Apply motion distortion
    p += motionDistort * vec2(sin(p.y * 3.0 + t), cos(p.x * 3.0 - t)) * 0.1;

    p += vec2(sin(t), cos(t)) * 0.15;
    p *= zoom;
    p *= rot(sin(t * 0.5) * 45.0);
    p *= -sin(t * 0.1) * morph;
    p += vec2(0.5 + cos(t * 0.575));

    float d = length(p) * 0.0015;

    float minLine = 100.0, m = 100.0;
    vec2 mc = vec2(100.0);

    for (int i = 0; i < 5; i++) {
        p = abs(5.0 - mod(p * 2.0, 10.0)) - 0.5;
        p *= 4.0 / dot(p, p + 0.001);
        p *= rot(-PI);
        minLine = min(minLine, min(abs(p.x), abs(p.y)));
        mc = min(mc, abs(p));
        m = min(m, abs(p.x - 1.0));
    }

    float pulse = smoothstep(0.0, 0.5, abs(0.5 - fract(m + t * speed)));
    minLine = exp(-20.0 * minLine);
    m = exp(-20.0 * m);
    mc = exp(-10.0 * mc);

    vec3 color = vec3(mc.x, minLine * 0.6, mc.y * 1.5) * minLine * pulse * pulse + m + d * (1.0 + warp);
    return color;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;

    vec2 offset = vec2(0.5) / RENDERSIZE.xy;
    vec3 col = vec3(0.0);
    col += fractal(uv, t);
    col += fractal(uv + offset.xy, t);
    col += fractal(uv - offset.xy, t);
    col += fractal(uv + offset.yx, t);
    col += fractal(uv - offset.yx, t);
    col *= 0.2;

    vec3 pulseCol = getPalette(length(uv) + t * colorPulse, int(colorPalette));
    col *= pulseCol;

    col *= brightness;
    gl_FragColor = vec4(col, 1.0);
}
