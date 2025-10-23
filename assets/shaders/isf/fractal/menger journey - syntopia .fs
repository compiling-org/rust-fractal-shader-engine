/*{
  "DESCRIPTION": "Trippy animated 3D fractal with distinct palettes, real shake, and color pulse controls.",
  "CATEGORIES": ["Fractal", "Raymarch", "Psychedelic"],
  "INPUTS": [
    { "NAME": "PulseFreq", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "PulseAmp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "PulseOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },

    { "NAME": "Shake", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "CameraPan", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "CameraTilt", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0 },

    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },

    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },

    { "NAME": "FractalTwist", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "FractalScale", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "FractalOffset", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MaxSteps 30
#define MinimumDistance 0.0009
#define normalDistance 0.0002
#define Iterations 7
#define PI 3.141592
#define FieldOfView 1.0
#define FudgeFactor 0.7
#define NonLinearPerspective 2.0

#define Ambient 0.32184
#define Diffuse 0.5
#define LightDir vec3(1.0)
#define LightColor vec3(1.0,1.0,0.858824)
#define LightDir2 vec3(1.0,-1.0,1.0)
#define LightColor2 vec3(0.0,0.333333,1.0)

vec2 rotate(vec2 v, float a) {
	return vec2(cos(a)*v.x + sin(a)*v.y, -sin(a)*v.x + cos(a)*v.y);
}

float rand(vec2 co){ return fract(sin(dot(co, vec2(12.9898,78.233))) * 43758.5453); }

// 7 distinct trippy palettes
vec3 getPaletteColor(float t, float p) {
	t = fract(t);
	if (p < 1.0) return vec3(sin(2.0*PI*t), cos(3.0*PI*t), sin(5.0*PI*t));          // PsySinCos
	if (p < 2.0) return vec3(pow(t, 0.5), t, t*t);                                  // LavaMist
	if (p < 3.0) return vec3(0.5 + 0.5*sin(PI*6.0*t + vec3(0,2,4)));                // CandyLoop
	if (p < 4.0) return vec3(1.0 - t, sin(t*PI*2.0), cos(t*PI*2.0));                // RetroInverse
	if (p < 5.0) return vec3(sin(t*10.0), sin(t*20.0), sin(t*30.0));                // NoiseBands
	if (p < 6.0) return vec3(0.5 + 0.5*cos(PI*t + vec3(1,2,3)));                    // TrigTrip
	return vec3(t*t, t*0.5, t*0.25);                                                // RustyDust
}

float DE(in vec3 z) {
	z = abs(1.0 - mod(z, 2.0));
	vec3 offset = vec3(FractalOffset);
	float d = 1000.0;
	for (int n = 0; n < Iterations; n++) {
		z.xy = rotate(z.xy, FractalTwist + 2.0*cos(TIME * Speed / 8.0));
		z = abs(z);
		if (z.x < z.y) z.xy = z.yx;
		if (z.x < z.z) z.xz = z.zx;
		if (z.y < z.z) z.yz = z.zy;
		z = FractalScale * z - offset * (FractalScale - 1.0);
		if (z.z < -0.5 * offset.z * (FractalScale - 1.0))
			z.z += offset.z * (FractalScale - 1.0);
		d = min(d, length(z) * pow(FractalScale, float(-n)-1.0));
	}
	return d - 0.001;
}

vec3 getNormal(in vec3 pos) {
	vec3 e = vec3(0.0, normalDistance, 0.0);
	return normalize(vec3(
		DE(pos+e.yxx)-DE(pos-e.yxx),
		DE(pos+e.xyx)-DE(pos-e.xyx),
		DE(pos+e.xxy)-DE(pos-e.xxy)
	));
}

vec3 getLight(in vec3 color, in vec3 normal, in vec3 dir) {
	vec3 lightDir1 = normalize(LightDir);
	vec3 lightDir2 = normalize(LightDir2);
	float d1 = max(0.0, dot(-normal, lightDir1));
	float d2 = max(0.0, dot(-normal, lightDir2));
	return
	(d1 * Diffuse) * (LightColor * color) +
	(d2 * Diffuse) * (LightColor2 * color);
}

vec3 getColor(vec3 normal, vec3 pos) {
	float t = sin(length(pos) * PulseAmp + TIME * PulseFreq + PulseOffset);
	return getPaletteColor(t, Palette);
}

vec4 rayMarch(in vec3 from, in vec3 dir) {
	float totalDistance = 0.0;
	vec3 pos;
	float distance;
	int steps = 0;
	for (int i = 0; i < MaxSteps; i++) {
		pos = from + totalDistance * dir;
		distance = DE(pos) * FudgeFactor;
		totalDistance += distance;
		if (distance < MinimumDistance) break;
		steps = i;
	}
	float smoothStep = float(steps) + distance / MinimumDistance;
	float ao = 1.1 - smoothStep / float(MaxSteps);
	vec3 normal = getNormal(pos - dir * normalDistance * 3.0);
	vec3 color = getColor(normal, pos);
	vec3 light = getLight(color, normal, dir);
	color = (color * Ambient + light) * ao;
	color = mix(vec3(dot(color, vec3(0.333))), color, Saturation);
	color = (color - 0.5) * Contrast + 0.5;
	color *= Brightness;
	return vec4(color, 1.0);
}

void main() {
	vec2 fragCoord = gl_FragCoord.xy;
	vec2 res = RENDERSIZE;

	// Single frame jitter offset for whole camera
	float shakeX = (rand(vec2(TIME, 3.1)) - 0.5) * Shake;
	float shakeY = (rand(vec2(TIME, 7.9)) - 0.5) * Shake;

	vec3 camPos = Zoom * 0.5 * TIME * vec3(1.0, 0.0, 0.0) + vec3(shakeX, shakeY, 0.0);
	vec3 target = camPos + vec3(1.0, CameraPan * cos(TIME), CameraTilt * sin(TIME * 0.4));
	vec3 camUp = vec3(0.0, 1.0, 0.0);

	vec3 camDir = normalize(target - camPos);
	camUp = normalize(camUp - dot(camDir, camUp) * camDir);
	vec3 camRight = normalize(cross(camDir, camUp));

	vec2 coord = -1.0 + 2.0 * fragCoord / res;
	coord.x *= res.x / res.y;

	vec3 rayDir = normalize(camDir + (coord.x * camRight + coord.y * camUp) * FieldOfView);
	gl_FragColor = rayMarch(camPos, rayDir);
}
