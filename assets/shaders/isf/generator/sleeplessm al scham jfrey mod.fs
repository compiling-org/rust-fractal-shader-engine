/*
{
  "ISFVSN": "2.0",
  "CATEGORIES": [ "Psychedelic", "Fractal", "Geometric" ],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.1 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.009, "MIN": 0.001, "MAX": 0.02 },
    { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "psyPulse", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "artifactStrength", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 2.5 }
  ]
}
*/





vec2 f_o_rotd(vec2 o_trn, float n_ang_nor){
    float n_tau = radians(360.);
    float n_sin = sin(n_ang_nor * n_tau);
    float n_cos = cos(n_ang_nor * n_tau);
    mat2 o_matr = mat2(
        n_cos, -n_sin, 
        n_sin, n_cos
    );
    return o_trn * o_matr;
}
float fns(float n, float b){
    return sin(n + b) * 0.5 + 0.5;
}
void main() {
    float n_scl_min = min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 o_trn = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / n_scl_min;
    float n_dcntr = length(o_trn);
    o_trn *= zoom;
    float n_tau = radians(360.);
    float n_circles = 1.0;
    float n_ang_nor = fract(atan(o_trn.x, o_trn.y) / n_tau + (1.0 / n_circles / 2.0 / 2.0));
    float n = length(o_trn);
    float norg = n;
    n_ang_nor = n_ang_nor + sin(n * n_tau + TIME) * morph + TIME * colorShift;
    float n1 = pow(n, 1.0 / 2.0);
    float n2 = pow(n, 2.0);
    float t = fns((1.0 / (n + 0.1) * 3.0), TIME);
    float n3 = t * n1 + ((1.0 - t) * n2);
    n = n3;
    float ntmp = 1.0 - pow(fns(norg * 3.0 - TIME * 2.0, 0.0), 1.0 / 3.0);
    
    float s = n_tau * n * 1.0 * sin(n_ang_nor * n_tau * psyPulse) * sin(1.0 / n);
    vec3 of = vec3(1.0, 2.0, 3.0) * ntmp * 2.0;
    vec3 on = vec3(
        fns(n * s, of[0]),
        fns(n * s, of[1]),
        fns(n * s, of[2])
    );
    vec3 odc2 = vec3(on);
    
    gl_FragColor = vec4(odc2, 1.0);
}
