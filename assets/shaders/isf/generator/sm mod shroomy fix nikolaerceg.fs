/*
{
    "ISFVSN": "2.0",
    "CATEGORIES": ["Visual", "Generative", "Psychedelic"],
    "INPUTS": [
        {"NAME": "time", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1000.0},
        {"NAME": "colorShift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
        {"NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0},
        {"NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0},
        {"NAME": "morphing", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0}
    ]
}
*/

vec4 hue(vec4 color, float shift) {
    const vec4 kRGBToYPrime = vec4(0.299, 0.587, 0.114, 0.0);
    const vec4 kRGBToI = vec4(0.596, -0.275, -0.321, 0.0);
    const vec4 kRGBToQ = vec4(0.212, -0.523, 0.311, 0.0);
    const vec4 kYIQToR = vec4(1.0, 0.956, 0.621, 0.0);
    const vec4 kYIQToG = vec4(1.0, -1.272, -0.647, 0.0);
    const vec4 kYIQToB = vec4(1.0, -1.107, 1.704, 0.0);

    float YPrime = dot(color, kRGBToYPrime);
    float I = dot(color, kRGBToI);
    float Q = dot(color, kRGBToQ);

    float hue = atan(Q, I);
    float chroma = sqrt(I * I + Q * Q);
    hue += shift;
    Q = chroma * sin(hue);
    I = chroma * cos(hue);
    vec4 yIQ = vec4(YPrime, I, Q, 0.0);
    color.r = dot(yIQ, kYIQToR);
    color.g = dot(yIQ, kYIQToG);
    color.b = dot(yIQ, kYIQToB);
    return color;
}

vec2 kale(vec2 uv, float angle, float base, float spin) {
    float a = atan(uv.y, uv.x) + spin;
    float d = length(uv);
    a = mod(a, angle * 2.0);
    a = abs(a - angle);
    uv.x = sin(a + base) * d;
    uv.y = cos(a + base) * d;
    return uv;
}

vec2 rotate(float px, float py, float angle) {
    return vec2(cos(angle) * px - sin(angle) * py, sin(angle) * px + cos(angle) * py);
}

vec3 psychedelicPalette(float t) {
    return vec3(
        0.5 + 0.5 * sin(t * 3.0 + 0.0),
        0.5 + 0.5 * cos(t * 2.0 + 2.0),
        0.5 + 0.5 * sin(t * 1.5 + 4.0)
    );
}

void main() {
    float p = 3.14159265359;
    float i = time * speed * 1.618;
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 8.0 - 4.0;
    uv *= zoom;
    uv = kale(uv, p / 5.0, i, i * 23.40);
    vec4 c = vec4(1.618);
    mat2 m = mat2(
        sin(uv.y * cos(uv.x + i) + i * 1.618) * 1.618, -255.5,
        sin(uv.x + i * 5.0) * 1.618, -cos(uv.y - i) * 6.0
    );
    uv = rotate(uv.x, uv.y, length(uv) + i * 1.618);
    c.rg = cos(sin(uv.xx + uv.yy) * m - i);
    c.b = sin(rotate(uv.x, uv.x, length(uv.xx) * 1.618 + i).x - uv.y + i);
    c.rgb *= psychedelicPalette(time * morphing);
    c = hue(c, colorShift);
    gl_FragColor = vec4(0.001 - c.rgb, 0.8);
}
