/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator"],
    "DESCRIPTION": "Converted shader with tunable parameters.",
    "INPUTS": [
        {"NAME": "distort", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "count", "TYPE": "float", "DEFAULT": 40.0, "MIN": 1.0, "MAX": 100.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0},
        {"NAME": "paletteId", "TYPE": "float", "DEFAULT": 0, "MIN": 0, "MAX": 6}
    ]
}
*/

#define PI 3.141592
#define TAU 2.*PI
#define hue(v) ( .6 + .6 * cos( 6.3*(v) + vec3(0,23,21) ) )
#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define SQR(x) ((x)*(x))
#define SIN(x) sin(x)*.5+.5
#define T TIME
#define F(a, b, s) (smoothstep(a, a+s, T)-smoothstep(b, b+s, T))

float tt;

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d ) {
    return a + b*cos( 6.28318*(c*t+d) );
}

vec3 getPal(int id, float t) {
    id = id % 7;
    vec3 col = pal( t, vec3(.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,-0.33,0.33) );
    if( id == 1 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20) );
    if( id == 2 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.3,0.20,0.20) );
    if( id == 3 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,0.5),vec3(0.8,0.90,0.30) );
    if( id == 4 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,0.7,0.4),vec3(0.0,0.15,0.20) );
    if( id == 5 ) col = pal( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(2.0,1.0,0.0),vec3(0.5,0.20,0.25) );
    if( id == 6 ) col = pal( t, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 col = vec3(0.0);
    if(T < 20.) tt = 0.7 * T;
    else tt = (0.7 * 20.) + (T * speed - 20.) * 0.5;
    float s = 1.0 / count;
    for(float i = 0.0; i < 1.0; i += s) {
        float z = fract(i - 0.1 * tt);
        float fade = smoothstep(0.0, 0.2, 1.0 - abs(2.0 * z - 1.0));
        col += fade * getPal(int(paletteId), i + tt * 0.2);
    }
    col = sqrt(col);
    gl_FragColor = vec4(col, 1.0);
}
