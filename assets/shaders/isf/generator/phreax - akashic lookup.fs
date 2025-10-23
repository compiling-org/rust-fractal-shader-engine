/*{
  "CATEGORIES": ["Generator", "Fractal", "Psychedelic"],
  "CREDIT": "Converted from ShaderToy by phreax (2023) + ISF by OpenAI Unity GPT",
  "DESCRIPTION": "Fractal morphing shader with palette morph and fractal geometry types",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.3 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "chebPower", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "fractalType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "shake", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "xyControl", "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] },
    { "NAME": "palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 }
  ]
}*/

#define SIN(x) (.5+.5*sin(x))
#define N 6.
#define PI 3.141592 

mat2 rot(float a) { float s=sin(a), c=cos(a); return mat2(c, s, -s, c); }

float curve(float t, float d) {
  t /= d;
  return mix(floor(t), floor(t)+1., pow(smoothstep(0., 1., fract(t)), 10.));
}

vec3 pal(float t, float index) {
    vec3 p0 = vec3(0.6) + vec3(1.0)*cos(6.283*(vec3(0.5)*t+vec3(0.149,0.875,0.729)));
    vec3 p1 = vec3(sin(t*3.1), sin(t*1.7+1.0), sin(t*2.3+2.0));
    vec3 p2 = vec3(1.0, 0.5+0.5*sin(t*3.0), 0.25+0.75*cos(t*2.0));
    vec3 p3 = vec3(cos(t*5.0), sin(t*4.0), sin(t*3.0));
    vec3 p4 = vec3(sin(t*8.0)*0.5+0.5, cos(t*6.0)*0.5+0.5, sin(t*4.0+1.5)*0.5+0.5);
    vec3 p5 = vec3(0.2+0.8*sin(t*2.0), 0.4+0.6*cos(t*3.0), 1.0-0.5*cos(t*4.0));
    vec3 p6 = vec3(1.0, 0.5+0.5*sin(t), 0.5+0.5*cos(t*1.5));

    vec3 palA = mix(p0, p1, smoothstep(0.0,1.0,fract(index)));
    vec3 palB = mix(p2, p3, smoothstep(0.0,1.0,fract(index-2.0)));
    vec3 palC = mix(p4, p5, smoothstep(0.0,1.0,fract(index-4.0)));

    if(index < 1.0) return palA;
    if(index < 3.0) return palB;
    if(index < 5.0) return palC;
    return p6;
}

float chebd(vec2 v, float k, float type) {
    if (type < 1.0) return pow(pow(abs(v.x), k)+pow(abs(v.y), k), 1./k);
    if (type < 2.0) return max(abs(v.x), abs(v.y));  // Chebyshev (infinite norm)
    if (type < 3.0) return dot(v, v);                // Euclidean squared
    if (type < 4.0) return length(v);                // Euclidean
    if (type < 5.0) return distance(v, vec2(0.25*sin(TIME), 0.25*cos(TIME))); // moving center
    return length(mod(v*8.0, 2.0)-1.0);              // Tiled domain distance
}

vec3 applyVignette(vec3 col, vec2 uv) {
    return col * mix(.2, 1., (1.8-pow(dot(uv, uv), .4)));
}

vec3 applyEffects(vec3 col) {
    col = mix(vec3(0.5), col, contrast);
    col = mix(vec3(dot(col, vec3(0.333))), col, saturation);
    col *= brightness;
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy - xyControl) * zoom;
    vec2 uv0 = uv;
    vec2 uv2 = uv;
    vec2 uvb = uv;

    float tt = TIME * speed;
    float morphAmt = morph;

    vec3 col = vec3(0.0);
    for(float j = 0.; j < N; j++) {
        float stepf = j/N;
        float z = fract(stepf - 0.05*(tt + 6.0*curve(tt+2., 4.)));
        z = z*z;

        float luma = smoothstep(0.2, .9, exp(-z*1.5));
        luma *= smoothstep(1., .8, exp(-z*1.));
        float blur = smoothstep(.1, 0.0, z);

        uv0 *= rot(PI*0.25*j);
        uv = uv0 * z;
        uv2 = uv0 * z;

        for(float i=0.; i<5.; i++) {
            uv = abs(abs(abs(abs(uv)-.1+i/2.)-.1*SIN(.1*tt+.8*curve(tt,2.)))-.05);
            uv = fract(uv*1.9 + uv*z + shake*sin(tt*10.0 + i + j))*morphAmt - 0.5;
            uv2 = fract(uv2*(1.0 + 0.5*sin(tt*0.1))) - 0.5;

            float d0 = chebd(uv0, chebPower, fractalType);
            vec3 c = pal(colorPulse*SIN(3.*d0 - 0.1*tt - 4.0*curve(tt,2.)) + tt*0.4 + i*0.4 + 0.4*j, palette);

            float d = fract(chebd(uv, chebPower, fractalType)-0.5) + chebd(uv2, chebPower, fractalType)*exp(-d0);
            d = abs(d - 0.4*i);
            d = abs(0.1*d + sin(d*8. + tt + j*0.3)/8.);
            d = smoothstep(0.015 + 0.01*blur, 0.0, d);

            col += d * luma * c;
        }
    }

    col = applyVignette(col, uvb);
    if(glitch > 0.0) col.rg += sin(uvb.yx * 30.0 + TIME*5.0) * glitch * 0.05;

    col = applyEffects(col);

    gl_FragColor = vec4(col, 1.0);
}
