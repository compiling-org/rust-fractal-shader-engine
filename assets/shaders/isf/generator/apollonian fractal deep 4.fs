/*{
  "CATEGORIES": ["Fractal", "Raymarching"],
  "DESCRIPTION": "Faithful ISF port of Apollonian feedback flythrough with pulse, palette, morph, etc.",
  "INPUTS": [
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.01, "MAX": 2.0 },
    { "NAME": "flySpeed", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "warpStrength", "TYPE": "float", "DEFAULT": 1.7, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "kPower", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "paletteIndex", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "pulseSpeed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "pulseIntensity", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// distance estimator
float Sphere(vec3 p) {
    p.z += TIME * flySpeed;
    float s = 1.0;
    for (int i = 0; i < 4; ++i) {
        float k = warpStrength / dot(p = mod(p - 1.0, 2.0) - 1.0, p);
        k = pow(k, kPower);
        p *= k;
        s *= k;
    }
    return length(p) / s;
}

// palette logic
vec3 palette(float t, float index) {
    if (index < 0.5) return 0.5 + 0.5 * cos(6.2831 * (t + vec3(0.0, 0.33, 0.67)));
    if (index < 1.5) return mix(vec3(1,0.2,0.1), vec3(1.0,1.0,0.5), sin(t*10. + TIME)*0.5+0.5);
    if (index < 2.5) return vec3(sin(t*6.0), cos(t*4.0), sin(t*3.0 + 2.0));
    if (index < 3.5) return vec3(t, t*t, 1.0 - t);
    if (index < 4.5) return 0.5 + 0.5 * cos(6.2831 * (t + vec3(0.1, 0.2, 0.3) + TIME * 0.1));
    if (index < 5.5) return vec3(sin(t*9. + TIME), cos(t*3. + TIME * 0.5), sin(t*5.));
    return vec3(cos(t * 5. + TIME), sin(t * 3. + TIME), cos(t * 7.));
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 resolution = RENDERSIZE.xy;

    // identical to your logic
    vec3 uv = vec3(fragCoord / resolution.y - 0.5, 1.0) * zoom;
    vec3 r = vec3(1.0, 1.0, TIME * flySpeed);
    vec2 m = sin(vec2(0.0, 1.5) + TIME * 0.25);

    // rotate
    mat2 rot = mat2(m.y, -m.x, m.x, m.y);
    uv.xy = rot * uv.xy;
    uv.xz = rot * uv.xz;

    vec4 color = vec4(0.0);

    // recursive ray accumulation
    for (color.w; color.w < 40.0; color.w += 1.0) {
        r += Sphere(r) * uv;
    }

    // pulse bands on result
    float d1 = Sphere(r);
    float d2 = Sphere(r - uv);
    float pulse = sin(dot(r.xy, vec2(5.0, 3.0)) + TIME * pulseSpeed) * 0.5 + 0.5;
    float glow = smoothstep(0.3, 0.7, pulse) * pulseIntensity;

    vec3 col = palette(d1 * d2 * 40.0 + glow, paletteIndex);
    col += glow * 0.4;
    col *= col;

    // color grading
    vec3 gray = vec3(dot(col, vec3(0.299, 0.587, 0.114)));
    col = mix(gray, col, saturation);
    col = (col - 0.5) * contrast + 0.5;
    col *= brightness;

    gl_FragColor = vec4(col, 1.0);
}
