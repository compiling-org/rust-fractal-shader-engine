/*
{
    "CATEGORIES": [
        "Procedural",
        "Fractal",
        "Psychedelic"
    ],

    "INPUTS": [
        {
            "NAME": "colorPaletteIndex",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.0,
            "STEP": 1.0,
            "LABEL": "Color Palette"
        },
        {
            "NAME": "colorPulseStrength",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Pulse Strength"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Pulse Speed"
        },
        {
            "NAME": "morphStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Fractal Morph"
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.01,
            "MAX": 5.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "animationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "cameraPosX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera X"
        },
        {
            "NAME": "cameraPosY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Camera Y"
        },
        {
            "NAME": "cameraRotZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -6.28,
            "MAX": 6.28,
            "LABEL": "Camera Z Rotation"
        },
        {
            "NAME": "fractalOffset",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Offset"
        },
        {
            "NAME": "fractalRotationSpeedX",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Rot Speed X"
        },
        {
            "NAME": "fractalRotationSpeedY",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Fractal Rot Speed Y"
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Saturation"
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
            "NAME": "edgeDetectionThreshold",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 0.5,
            "LABEL": "Edge Threshold"
        },
        {
            "NAME": "distortionFrequency",
            "TYPE": "float",
            "DEFAULT": 8.0,
            "MIN": 0.1,
            "MAX": 20.0,
            "LABEL": "Distort Freq"
        },
        {
            "NAME": "distortionSpeed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Distort Speed"
        }
    ],
    "PASSES": [
        {
            "TARGET": "BufferA",
            "FLOAT": true,
            "PERSISTENT": true
        },
        {
            "TARGET": "BufferB",
            "FLOAT": true,
            "PERSISTENT": true
        },
        {
            "TARGET": "BufferC",
            "FLOAT": true,
            "PERSISTENT": true
        },
        {}
    ]
}
*/

#define NUM_ITER 60

const float pi = acos(-1.0);
const float pi2 = pi * 2.0;

// Declare sampler2D uniforms for the previous pass buffers.
// ISF hosts typically expose these target buffers as samplers.
// The naming convention is often "ISF_BufferA", "ISF_BufferB", etc.
// Or sometimes just the name itself, "BufferA".
// Let's use the standard `VAR_BufferName` prefix as it's common in ISF environments.



mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

vec2 pmod(vec2 p, float r) {
    float a = atan(p.x, p.y) + pi / r;
    float n = pi2 / r;
    a = floor(a / n) * n;
    return p * rot(-a);
}

float box(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
}

float ifsBox(vec3 p) {
    for (int i = 0; i < 5; i++) {
        p = abs(p) - fractalOffset - morphStrength * 0.5 * sin(TIME * animationSpeed);
        p.xy *= rot(TIME * animationSpeed * fractalRotationSpeedX);
        p.xz *= rot(TIME * animationSpeed * fractalRotationSpeedY);
    }
    p.xz *= rot(TIME * animationSpeed);
    return box(p, vec3(0.4, 0.8, 0.3));
}

float map(vec3 p, vec3 cPos) {
    vec3 p1 = p;
    p1.x = mod(p1.x - 5., 10.) - 5.;
    p1.y = mod(p1.y - 5., 10.) - 5.;
    p1.z = mod(p1.z, 16.) - 8.;
    p1.xy = pmod(p1.xy, 5.0 + morphStrength * 2.0);
    return ifsBox(p1);
}

vec3 hsv2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 7.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// --- Psychedelic Color Palettes ---
vec3 getPsychedelicPalette(float t) {
    vec3 col;
    int index = int(colorPaletteIndex);

    if (index == 0) { // Hypnotic Rainbow
        vec3 a = vec3(0.5, 0.5, 0.5);
        vec3 b = vec3(0.5, 0.5, 0.5);
        vec3 c = vec3(1.0, 1.0, 1.0);
        vec3 d = vec3(0.263, 0.416, 0.557);
        col = a + b * cos(6.28318 * (t * 0.7 + d));
    } else if (index == 1) { // Electric Neon
        col = 0.5 + 0.5 * sin(t * pi2 * 2.0 + vec3(0.0, 0.33, 0.66));
        col.r = pow(col.r, 1.5);
        col.g = pow(col.g, 1.5);
        col.b = pow(col.b, 1.5);
    } else if (index == 2) { // Acid Trip
        col = hsv2rgb(vec3(mod(t * 1.5 + TIME * 0.1, 1.0), 1.0, sin(t * 3.0) * 0.5 + 0.5));
    } else if (index == 3) { // Cosmic Dust
        col = 0.5 + 0.5 * cos(t * 5.0 + vec3(0.0, 2.0, 4.0));
        col = pow(col, vec3(1.5, 1.2, 1.8));
    } else if (index == 4) { // Retro Wave
        col = mix(vec3(0.0, 1.0, 1.0), vec3(1.0, 0.0, 1.0), sin(t * 4.0 + TIME * 0.2) * 0.5 + 0.5);
        col = mix(col, vec3(1.0, 1.0, 0.0), sin(t * 2.0 + TIME * 0.3) * 0.5 + 0.5);
    } else if (index == 5) { // Bioluminescent
        col = vec3(0.0, 0.8, 0.5) + sin(t * 7.0 + vec3(0.0, 1.0, 2.0)) * 0.4;
        col.b += sin(t * 10.0) * 0.3;
        col = clamp(col, 0.0, 1.0);
    } else { // 6 - Dark Psychedelia (Default if out of bounds)
        col = pow(abs(sin(t * 3.0 + vec3(0.0, 1.5, 3.0))), vec3(1.0, 2.0, 3.0));
        col = mix(col, vec3(0.1, 0.0, 0.2), 0.5);
    }
    return col;
}

mat2 r2d(float a) {
    return mat2(cos(a), sin(a), -sin(a), cos(a));
}

vec3 hueShift(vec3 col, float shift) {
    vec3 m = vec3(cos(shift), -sin(shift) * .57735, 0);
    m = vec3(m.xy, -m.y) + (1. - m.x) * .33333;
    return mat3(m, m.zxy, m.yzx) * col;
}

vec2 customRotZoom(vec2 uv) {
    uv.xy *= r2d(cameraRotZ);
    uv *= (1.0 / zoomFactor);
    return uv;
}

vec3 aces_approx(vec3 v) {
  v = max(v, 0.0);
  v *= 0.6f;
  float a = 2.51f;
  float b = 0.03f;
  float c = 2.43f;
  float d = 0.59f;
  float e = 0.14f;
  return clamp((v*(a*v+b))/(v*(c*v+d)+e), 0.0f, 1.0f);
}

vec3 aces(vec3 x) {
    return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0);
}

float luma(in vec4 color) {
    return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

float luma(vec3 color) {
  return dot(color, vec3(0.299, 0.587, 0.114));
}

vec2 distortPosition(vec2 uv) {
    float timeOffset = TIME * distortionSpeed;
    vec2 frequencyOffset = distortionFrequency * (uv + timeOffset);

    vec2 distortion = cos(vec2(
        cos(frequencyOffset.x - frequencyOffset.y) * cos(frequencyOffset.y + frequencyOffset.x),
        sin(frequencyOffset.x + frequencyOffset.y) * sin(frequencyOffset.y - frequencyOffset.x)));
    return distortion;
}

// --- Main rendering logic for passes ---
void main() {
    if (PASSINDEX == 0) {
        vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);

        p = customRotZoom(p);

        vec3 cPos = vec3(cameraPosX, cameraPosY, -3.0 * TIME * animationSpeed);
        vec3 cDir = normalize(vec3(0.0, 0.0, -1.0));
        vec3 cUp = vec3(sin(TIME * animationSpeed), 1.0, 0.0);
        vec3 cSide = cross(cDir, cUp);

        vec3 ray = normalize(cSide * p.x + cUp * p.y + cDir);

        float acc = 0.0;
        float acc2 = 0.0;
        float t = 0.0;

        for (int i = 0; i < NUM_ITER; i++) {
            float snd = 0.5; // Placeholder constant, as audio input is not used
            vec3 pos = cPos + ray * t;
            float dist = map(pos, cPos);
            dist = max(abs(dist), 0.02 + .1 * snd);
            float a = exp(-dist * 3.0);
            if (mod(length(pos) + 24.0 * TIME * animationSpeed, 30.0) < 5.0) {
                a *= 2. + 2.5 * snd;
                acc2 += a;
            }
            acc += a;
            t += dist * (.25 + .75 * snd);
        }

        float hue = mod(t * 0.15 + TIME * animationSpeed * 0.2, 1.0);
        float saturationVal = t;
        float value = clamp(acc * 0.25 + acc2 * 0.25, 0.1, 1.0);

        vec3 col = pow(getPsychedelicPalette(hue), vec3(2.));

        float pulsePos = mod(gl_FragCoord.x / RENDERSIZE.x + TIME * colorPulseSpeed, 1.0);
        float pulseEffect = 1.0 - smoothstep(0.4, 0.6, abs(gl_FragCoord.x / RENDERSIZE.x - pulsePos));
        col += pulseEffect * colorPulseStrength;

        col += max(luma(col) - 1.1, 0.0);
        gl_FragColor = vec4((col), 1.0);
    } else if (PASSINDEX == 1) {
        float h = 0.5;

        // Use texture2D for sampling. The buffers are available as sampler2D uniforms.
        // It's crucial that the 'uniform sampler2D BufferA;' etc. declarations match the JSON 'TARGET' names.
        vec4 o = texture2D(BufferA, gl_FragCoord.xy / RENDERSIZE.xy);
        vec4 n = texture2D(BufferA, (gl_FragCoord.xy + vec2(0, h)) / RENDERSIZE.xy);
        vec4 e = texture2D(BufferA, (gl_FragCoord.xy + vec2(h, 0)) / RENDERSIZE.xy);
        vec4 s = texture2D(BufferA, (gl_FragCoord.xy + vec2(0, -h)) / RENDERSIZE.xy);
        vec4 w = texture2D(BufferA, (gl_FragCoord.xy + vec2(-h, 0)) / RENDERSIZE.xy);

        vec4 dy = (n - s) * .75;
        vec4 dx = (e - w) * .75;

        vec4 edge = sqrt(dx * dx + dy * dy);

        if (edge.x + edge.y + edge.z > edgeDetectionThreshold) {
            gl_FragColor = vec4(edge.xyz * 5.0, 1.);
        } else {
            gl_FragColor = vec4(texture2D(BufferA, gl_FragCoord.xy / RENDERSIZE.xy).rgb, 1.);
        }

        vec4 laplacian = n + e + s + w - 4.0 * o;
        gl_FragColor.xyz += abs(laplacian.xyz / o.xyz) * 1.;

    } else if (PASSINDEX == 2) {
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 distorted1 = distortPosition(uv);
        vec2 distorted2 = distortPosition(uv + 1.0);

        float timeVaryingFactor = 1.5;
        float displacement = timeVaryingFactor * 50.0 / RENDERSIZE.x;
        vec2 shiftedUV = uv + displacement * (distorted1 - distorted2);

        // Sample BufferB
        gl_FragColor = texture2D(BufferB, shiftedUV);
    } else if (PASSINDEX == 3) {
        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 texel = 1.0 / RENDERSIZE.xy;
        vec4 total_color = vec4(0.0);

        float[] gk1s = float[](
            0.003765, 0.015019, 0.023792, 0.015019, 0.003765,
            0.015019, 0.059912, 0.094907, 0.059912, 0.015019,
            0.023792, 0.094907, 0.150342, 0.094907, 0.023792,
            0.015019, 0.059912, 0.094907, 0.059912, 0.015019,
            0.003765, 0.015019, 0.023792, 0.015019, 0.003765
        );

        for (int i = 0; i < 5; i++) {
            float fi = float(i) - 2.0;
            for (int j = 0; j < 5; j++) {
                float fj = float(j) - 2.0;
                // Sample BufferC
                vec4 color = texture2D(BufferC, uv + vec2(texel.x * fi, texel.y * fj));
                total_color += color * gk1s[i * 5 + j];
            }
        }

        vec3 finalColor = total_color.rgb;

        finalColor = (finalColor - 0.5) * contrast + 0.5;

        float l = luma(finalColor);
        finalColor = mix(vec3(l), finalColor, saturation);

        finalColor *= brightness;

        gl_FragColor = vec4(finalColor, 1.0);
    }
}