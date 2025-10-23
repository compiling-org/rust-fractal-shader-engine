/*
{
    "DESCRIPTION" : "ISF conversion of 'Energetic Overflow' by @kishimisu with tunable parameters and psychedelic color palette.",
    "CATEGORIES" : [ "Psychedelic", "Abstract" ],
    "INPUTS" : [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
        { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
        { "NAME": "pulse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "morphing", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 }
    ]
}
*/
#define e(p,r) mod(p, r) - r/2.

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * zoom - vec2(0.5);
    vec3 p, g = vec3(0.0);
    float i = 0.0, d = 0.0, t = TIME * speed, s, r;
    
    for (i = 0.0; i < 60.0; i++) {
        p = abs(d * normalize(vec3(uv, 1.0)));
        p.z += t * 12.73;
        p.xy *= mat2(cos(d * 0.03 + vec4(0,33,11,0)));
        r = 0.5 + sin(t) * (sin(p.z * 2.0) * 0.1 + cos(p.z * 4.0) * 0.05);
        d += min(s = min(
            length(e(p, vec3(5,5,20))) - r, 
            length(vec2(length(p.xy) - 8.0, e(p.z, 20.0))) - 0.25), 
            length(e(p.xy, 5.0)) - r);
        g += 1.0 / (1.0 + pow(abs(s) * 40.0, 1.3));
    }
    
    vec3 color = (1.5 * (cos(d + t + vec3(0,1,2) + colorShift) + 1.0) * g + g) / exp(d * 0.01);
    color = mix(color, vec3(sin(TIME + pulse), cos(TIME + pulse), sin(TIME * morphing)), morphing);
    
    gl_FragColor = vec4(color, 1.0);
}
