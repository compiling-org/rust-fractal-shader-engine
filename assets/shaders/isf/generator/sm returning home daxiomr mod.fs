/*
{
  "ISFVSN": "2.0",
  "CATEGORIES": [ "Psychedelic", "Fractal", "Geometric" ],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0 }
  ]
}
*/





const float TAU = 6.28318;

vec3 palette(in float t)
{
    vec3 a = vec3(0.000, 0.500, 0.500);
    vec3 b = vec3(2.000, 0.500, 0.490);
    vec3 c = vec3(2.000, 2.000, 0.500);
    vec3 d = vec3(0.000, 0.667, 0.500);

    return a + b * cos( TAU * (c * t + d) );
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2. - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv;
    vec3 finalColor = vec3(0.);
    
    for (float i = 0.; i < 3.; i++) {
        uv = (fract(zoom * uv * pow(0.125, i)) - .5);

        float d = length(uv) * exp(-length(uv0));

        vec3 col = palette(length(uv0) + i * .4 + TIME * pow(colorShift, i));

        d = sin(d * 8. + TIME * speed) / 8.;
        d = abs(d);
        d = pow(0.01 / d, 3.);

        finalColor += col * d;
    }

    gl_FragColor = vec4(finalColor, 1.0);
}
