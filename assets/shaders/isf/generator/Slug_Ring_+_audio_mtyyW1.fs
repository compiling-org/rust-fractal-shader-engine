/*
{
    "CATEGORIES": ["Automatically Converted", "Shadertoy"],
    "DESCRIPTION": "Enhanced version with tunable psychedelic effects",
    "INPUTS": [
        {"NAME": "iChannel0", "TYPE": "audio"},
        {"NAME": "ColorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "ColorPulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0},
        {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0},
        {"NAME": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0}
    ]
}
*/

#define N 120
#define PI 3.141593

float circle(vec2 p, float r) {
    return smoothstep(.1, .0, abs(length(p) - r));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (uv * 2. - 1.) * Zoom;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * Speed;
    float a = atan(p.y, p.x);
    vec3 col = vec3(0.0);
    
    for (int i = 0; i < N; i++) {
        float fi = float(i);
        float t = fi / float(N);
        float aa = (t + time / 12.) * 2. * PI;
        
        float beat = IMG_NORM_PIXEL(iChannel0, mod(vec2(0.05, 0.25), 1.0)).x * 0.25;
        float size = .3 + sin(t * 6. * PI) * .1 + beat;
        
        float a1 = -time * PI / 3. + aa;
        a1 += sin(time + beat) * Morph;
        a1 += sin(length(p) * 3. + time * PI / 2.) * 0.3 * Morph;
        vec2 c1 = vec2(cos(a1), sin(a1));
        
        float a2 = aa * (4.0 + beat * 0.05);            
        vec2 c2 = vec2(cos(a2), sin(a2)) * 0.3 + c1;
        col.r += .001 / abs(length(p - c2) - size) * (1.0 + sin(time * ColorPulse));      
        col.g += .0013 / abs(length(p - c2) - size * 1.05) * (1.0 + sin(time * ColorPulse + 2.0));      
        col.b += .0015 / abs(length(p - c2) - size * 1.09) * (1.0 + sin(time * ColorPulse + 4.0));      
    }
    
    col = mix(col, col.bgr, ColorShift * sin(time));
    gl_FragColor = vec4(col, 1.);
}