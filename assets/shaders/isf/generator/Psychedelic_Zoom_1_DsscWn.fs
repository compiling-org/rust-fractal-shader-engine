/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Fractal", "Abstract", "Live Visuals"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "mouseX", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0}
    ]
}
*/

void main() {
    vec2 v = RENDERSIZE.xy;
    vec2 p = (gl_FragCoord.xy - v * 0.5) * 0.4 / v.y;

    float zoomFactor = -5.0 + abs(sin(time * 0.05 * speed)) * 4.0 * zoom;
    p /= zoomFactor;

    p += p * sin(dot(p, p) * 20.0 - time * speed) * 0.04 * morphing;

    vec4 c = vec4(0.0);
    
    for (float i = 0.5; i < 8.0; i++) {
        p = abs(2.0 * fract(p - 0.5) - 1.0) * mat2(cos(0.01 * (time + mouseX * 0.1) * i * i + 0.78 * vec4(1, 7, 3, 1)));

        c += exp(-abs(p.y) * 5.0) * (cos(vec4(1, 2, 3, 0) * i * colorPulse) * 0.3 + 0.2);
    }

    // Apply color shift for dynamic hues
    c.rgb = mix(c.rgb, c.bgr, colorShift);

    // Adjust brightness for better contrast
    c -= vec4(0.3, 0.3, 0.3, 0.0);
    c = clamp(c, 0.0, 1.0);

    gl_FragColor = c;
}
