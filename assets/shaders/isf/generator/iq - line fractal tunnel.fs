/*{
  "DESCRIPTION": "Converted from Inigo Quilez - Animated Line Fractal Tunnel",
  "CATEGORIES": ["Fractal", "Tunnel", "Audio Reactive"],
  "INPUTS": [
    {
      "NAME": "time",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "LABEL": "Time"
    },
    {
      "NAME": "paletteType",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0,
      "LABEL": "Color Palette"
    },
    {
      "NAME": "colorPulse",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Color Pulse"
    },
    {
      "NAME": "speedMultiplier",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 5.0,
      "DEFAULT": 1.0,
      "LABEL": "Speed"
    },
    {
      "NAME": "distortionScale",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Distortion Scale"
    },
    {
      "NAME": "rotationAmount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "LABEL": "Rotation"
    },
    {
      "NAME": "glowIntensity",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3.0,
      "DEFAULT": 1.0,
      "LABEL": "Glow Intensity"
    }
  ],
  "ISFVSN": "2.0"
}*/
vec3 hash3(float n) {
    return fract(sin(vec3(n, n+1.0, n+2.0)) * vec3(43758.5453123, 22578.1459123, 19642.3490423));
}

vec3 snoise3(in float x) {
    float p = floor(x);
    float f = fract(x);

    f = f*f*(3.0 - 2.0*f);

    return -1.0 + 2.0 * mix(hash3(p+0.0), hash3(p+1.0), f);
}

float freqs[16];

float dot2(vec3 v) { return dot(v,v); }

vec2 usqdLineSegment(vec3 ro, vec3 rd, vec3 v0, vec3 v1) {
    vec3 ba = v1 - v0;
    vec3 oa = ro - v0;

    float a = dot(ba, ba);
    float b = dot(rd, ba);
    float c = dot(oa, ba);
    float e = dot(oa, rd);

    vec2 st = vec2(c - b*e, b*c - a*e) / (a - b*b);

    st.x = min(max(st.x, 0.0), 1.0);
    st.y = max(st.y, 0.0);

    return vec2(dot2((v0 + st.x*ba) - (ro + st.y*rd)), st.x);
}

vec3 palette(float t, float type) {
    t = fract(t);
    if (type < 1.0)
        return vec3(0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))));
    else if (type < 2.0)
        return vec3(sin(t*3.14159), cos(t*6.28318), sin(t*1.5));
    else
        return vec3(t, 0.8*t, 1.0 - t);
}

vec3 castRay(vec3 ro, vec3 rd, float linesSpeed, float time, float pulse, float distort, float glow) {
    vec3 col = vec3(0.0);
    float h = 0.0;
    float rad = 0.04 + 0.15 * 0.5; // fake freqs

    for (int i = 0; i < 128; i++) {
        vec3 op = 1.25 * normalize(snoise3(64.0*h + linesSpeed*0.015*time));
        vec3 np = 1.25 * normalize(snoise3(64.0*(h + 1.0/128.0) + linesSpeed*0.015*time));

        vec2 dis = usqdLineSegment(ro, rd, op, np);
        float m = pow(0.5, 2.0) * (1.0 + 2.0*h); // fake audio input
        float f = 1.0 - 4.0*dis.y*(1.0 - dis.y);
        float width = 1240.0 - 1000.0*f;
        width *= distort;
        float ff = exp(-0.06*dis.x*dis.x*dis.x) * m;

        vec3 lcol = palette(fract(h*10.0 + pulse*time), paletteType);
        col += lcol * exp(-width*dis.x) * ff * glow;
        h += 1.0/128.0;
    }
    return col;
}

void main() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = -1.0 + 2.0*q;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME * speedMultiplier;

    // Fake freqs
    for (int i = 0; i < 16; i++)
        freqs[i] = 0.5; // replace with real audio input if available

    vec3 ta = vec3(0.0);
    float camSpeed = 1.0;

    // Camera target
    ta = 0.2 * vec3(cos(0.1*t), 0.0, sin(0.07*t));
    vec3 ro = vec3(1.0*cos(camSpeed*0.05*t), 0.0, 1.0*sin(camSpeed*0.05*t));
    float roll = rotationAmount * 0.25 * sin(camSpeed*0.01*t);

    // Camera transformation
    vec3 cw = normalize(ta - ro);
    vec3 cp = vec3(sin(roll), cos(roll), 0.0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    vec3 rd = normalize(p.x*cu + p.y*cv + 1.2*cw);

    // Add morphing distortion
    float curve = smoothstep(0.0, 1.0, sin(t*0.1));
    rd.xy += curve * 0.025 * vec2(sin(34.0*q.y), cos(34.0*q.x));
    rd = normalize(rd);

    // Raymarch
    vec3 col = castRay(ro, rd, 1.0, t, colorPulse, distortionScale, glowIntensity);

    // Post effects
    col = col * col * 2.4;
    col *= 0.15 + 0.85 * pow(16.0*q.x*q.y*(1.0 - q.x)*(1.0 - q.y), 0.15);

    gl_FragColor = vec4(col, 1.0);
}