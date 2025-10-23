/*
{
  "DESCRIPTION": "Psychedelic fractal shader with customizable parameters",
  "CATEGORIES": ["Psychedelic", "Fractal"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 2.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morphing", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "fractalControl", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "warping", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorPalette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0, 
      "LABELS": ["Default", "Palette 1", "Palette 2", "Palette 3"] },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 }
  ],
  "PASSES": [
    {
      "TARGET": "feedbackBuffer",
      "PERSISTENT": true
    }
  ]
}
*/

const int aa = 3;
float k;

mat2 rot(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(c, s, -s, c);
}

vec3 fractal(vec2 p) {
    p += vec2(sin(TIME * speed), cos(TIME * speed)) * 0.15 * zoom;
    p /= dot(p, p); 
    float d = length(p) * 0.005 * k;
    p *= rot(sin(TIME * speed) * 45.0);    
    p *= sin(TIME * 0.2 * morphing);
    p += TIME * 0.5;
    float ml = 100.0;
    float m = 100.0;
    vec2 mc = vec2(100.0);
    
    for (int i = 0; i < 7; i++) {
        p = abs(5.0 - mod(p * 2.0, 10.0)) - 1.0;
        p *= rot(TIME * 2.0 * warping);
        ml = min(ml, min(abs(p.x), abs(p.y)));
        mc = min(mc, abs(p));
        m = min(m, abs(p.x - 1.0));
    }
    
    float l = smoothstep(0.0, 0.5, abs(0.5 - fract(m * 2.0 + TIME)));
    ml = exp(-10.0 * ml);
    m = exp(-20.0 * m);
    mc = exp(-10.0 * mc);
    
    // Apply vibrant color palette selection
    vec3 color;
    if (colorPalette < 1.0) {
        color = vec3(0.5 + 0.5 * sin(TIME + mc.x * 10.0), 
                      0.5 + 0.5 * sin(TIME + mc.y * 10.0), 
                      0.5 + 0.5 * sin(TIME + ml * 10.0)); // Default vibrant colors
    } else if (colorPalette < 2.0) {
        color = vec3(0.5 + 0.5 * sin(TIME + mc.x * 5.0 + 1.0), 
                      0.5 + 0.5 * sin(TIME + ml * 5.0 + 3.0), 
                      0.5 + 0.5 * sin(TIME + mc.y * 5.0 + 5.0)); // Palette 1
    } else if (colorPalette < 3.0) {
        color = vec3(0.5 + 0.5 * sin(TIME + ml * 3.0 + 2.0), 
                      0.5 + 0.5 * sin(TIME + mc.x * 3.0 + 4.0), 
                      0.5 + 0.5 * sin(TIME + mc.y * 3.0 + 6.0)); // Palette 2
    } else {
        color = vec3(0.5 + 0.5 * sin(TIME + mc.x * 7.0 + 3.0), 
                      0.5 + 0.5 * sin(TIME + ml * 7.0 + 5.0), 
                      0.5 + 0.5 * sin(TIME + mc.y * 7.0 + 7.0)); // Palette 3
    }
    
    return color * ml * l * brightness * (1.0 + 0.2 * sin(TIME * colorPulse)) + m + d * (1.0 + k);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    if (PASSINDEX == 0) {
        // For feedback buffer pass
        k = IMG_NORM_PIXEL(feedbackBuffer, vec2(0.6, 0.5)).r;
        k = smoothstep(0.3, 0.7, k) * 4.0 * fractalControl;
        
        vec2 p = (gl_FragCoord.xy - RENDERSIZE * 0.5) / RENDERSIZE.y;
        vec2 pix = 1.0 / RENDERSIZE / float(aa * 2);
        vec3 col = vec3(0.0);
        
        for (int i = -aa; i <= aa; i++) {
            for (int j = -aa; j <= aa; j++) {
                vec2 d = vec2(i, j) * pix;
                col += fractal(p + d);
            }
        }
        
        col /= float((aa*2+1)*(aa*2+1));
        gl_FragColor = vec4(col, 1.0);
    } else {
        // Final output pass
        vec3 feedbackColor = IMG_NORM_PIXEL(feedbackBuffer, uv).rgb;
        gl_FragColor = vec4(feedbackColor, 1.0);
    }
}