/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator", "Psychedelic"],
    "DESCRIPTION": "Converted from Shadertoy: 'Voyager' by @kishimisu",
    "INPUTS": [
        { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0 },
        { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
    ],
    "PASSES": [{ "TARGET": "bufferA" }, { "TARGET": "image" }]
}
*/
void main() {
    vec2 u = gl_FragCoord.xy / RENDERSIZE.xy;
    u = (u - 0.5) * zoom + 0.5;
    vec3 p, q, f = vec3(.2, 2., .2);
    float i, t, d, n, T = TIME * speed;
    vec4 O = vec4(0.0);
    
    for (i = 0.0; i < 50.0; i++) {
        p = q = t * normalize(vec3(u * mat2(cos(T / 16. + vec4(0, 33, 11, 0))), 1));
        n = sin(p.z += T) * cos(p.x * 1.4 + T / 4.) * cos(p.z * 1.2 - T * 0.5) * 0.5 + 0.5;
        p.y += 1.0 + q.z * sin(T / 6.0) * 0.2 - n * morph;
        t += d = length(p = mod(p, f + f) - f) - 0.1;
        O += 0.07 * pow(n, 5.0) / ++d * (1.0 + cos(length(q) * 0.14 + length(u) * 3.0 - T + vec4(0,1,2,0) * colorShift));
    }
    
    O.rgb *= mix(vec3(1.0, 0.5, 1.5), vec3(0.5, 1.5, 1.0), sin(T * colorPulse));
    gl_FragColor = O;
}
