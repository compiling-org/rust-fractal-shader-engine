/*{
  "DESCRIPTION": "Advanced KIFS fractal with full morphing, trap shaping, symmetry, and psychedelic rendering.",
  "CATEGORIES": ["Fractal", "Raymarch", "Morph", "KIFS", "Psychedelic"],
  "INPUTS": [
    { "NAME": "PulseFreq", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "PulseAmp", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "PulseOffset", "TYPE": "float", "DEFAULT": 0.0, "MIN": -10.0, "MAX": 10.0 },
    { "NAME": "Shake", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "Palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.0 },

    { "NAME": "FractalIterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 1.0, "MAX": 20.0 },
    { "NAME": "TrapType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "TrapMix", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "SymmetryAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "TwistAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "TilingAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "MorphType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

#define MaxSteps 30
#define MinimumDistance 0.001
#define normalDistance 0.0002
#define PI 3.141592
#define Scale 2.0
#define FieldOfView 1.0
#define Jitter 0.05
#define FudgeFactor 1.0
#define Ambient 0.28452
#define Diffuse 0.57378
#define Specular 0.07272

#define LightDir vec3(1.0,1.0,-0.65048)
#define LightColor vec3(1.0,0.666667,0.0)
#define LightDir2 vec3(1.0,-0.62886,1.0)
#define LightColor2 vec3(0.596078,0.635294,1.0)

vec2 rotate(vec2 v, float a) {
	return vec2(cos(a)*v.x + sin(a)*v.y, -sin(a)*v.x + cos(a)*v.y);
}

float rand(vec2 co){ return fract(cos(dot(co,vec2(4.898,7.23))) * 23421.631); }

vec3 getPaletteColor(float t, float p) {
	t = fract(t);
	if (p < 1.0) return vec3(sin(2.0*PI*t), cos(3.0*PI*t), sin(5.0*PI*t));
	if (p < 2.0) return vec3(pow(t, 0.5), t, t*t);
	if (p < 3.0) return vec3(0.5 + 0.5*sin(PI*6.0*t + vec3(0,2,4)));
	if (p < 4.0) return vec3(1.0 - t, sin(t*PI*2.0), cos(t*PI*2.0));
	if (p < 5.0) return vec3(sin(t*10.0), sin(t*20.0), sin(t*30.0));
	if (p < 6.0) return vec3(0.5 + 0.5*cos(PI*t + vec3(1,2,3)));
	return vec3(t*t, t*0.5, t*0.25);
}

float trap(vec3 p, float time) {
	float cube = length(p.x - 0.5 - 0.5*sin(time / 10.0));
	float tube = abs(length(p.xz - vec2(1.0,1.0)) - 0.05);
	float sphere = length(p);
	return mix(mix(cube, tube, step(1.0, TrapType)), sphere, step(2.0, TrapType)) * TrapMix + sphere * (1.0 - TrapMix);
}

float DE_kaleido(vec3 z, float time, vec3 offset) {
	z = abs(TilingAmount - mod(z, TilingAmount*2.0));
	float d = 1000.0;
	for (int i = 0; i < int(FractalIterations); i++) {
		z.xz = rotate(z.xz, time/18.0 * TwistAmount);
		if (SymmetryAmount > 0.0) {
			if (z.x + z.y < 0.0) z.xy = -z.yx;
			if (z.x + z.z < 0.0) z.xz = -z.zx;
			if (z.x - z.y < 0.0) z.xy = z.yx;
			if (z.x - z.z < 0.0) z.xz = z.zx;
		}
		z = abs(z);
		z = z * Scale - offset * (Scale - 1.0);
		z.yz = rotate(z.yz, -time/18.0 * TwistAmount);
		d = min(d, trap(z, time) * pow(Scale, float(-i)-1.0));
	}
	return d;
}

float DE_mandel(vec3 z, float time) {
	vec3 p = z;
	float dr = 1.0;
	float r = 0.0;
	for (int i = 0; i < int(FractalIterations); i++) {
		r = length(z);
		if (r > 2.0) break;

		float theta = acos(z.z/r);
		float phi = atan(z.y,z.x);
		float power = 8.0;

		dr =  pow( r, power-1.0)*power*dr + 1.0;
		float zr = pow(r, power);
		theta = theta*power;
		phi = phi*power;

		z = zr*vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));
		z += p;
	}
	return 0.5*log(r)*r/dr;
}

float DE(vec3 z, float time, vec3 offset) {
	return mix(DE_kaleido(z, time, offset), DE_mandel(z, time), MorphType);
}

vec3 getNormal(vec3 pos, float time, vec3 offset) {
	vec3 e = vec3(0.0, normalDistance, 0.0);
	return normalize(vec3(
		DE(pos + e.yxx, time, offset) - DE(pos - e.yxx, time, offset),
		DE(pos + e.xyx, time, offset) - DE(pos - e.xyx, time, offset),
		DE(pos + e.xxy, time, offset) - DE(pos - e.xxy, time, offset)
	));
}

vec3 getColor(vec3 normal, vec3 pos, float time) {
	float t = sin(length(pos) * PulseAmp + time * PulseFreq + PulseOffset);
	return getPaletteColor(t, Palette);
}

vec3 getLight(vec3 color, vec3 normal, vec3 dir) {
	vec3 l1 = normalize(LightDir);
	vec3 l2 = normalize(LightDir2);
	float s1 = pow(max(0.0, dot(l1, -reflect(l1, normal))), 20.0);
	float s2 = pow(max(0.0, dot(l2, -reflect(l2, normal))), 20.0);
	float d1 = max(0.0, dot(-normal, l1));
	float d2 = max(0.0, dot(-normal, l2));
	return
		(Specular * s1) * LightColor +
		(d1 * Diffuse) * (LightColor * color) +
		(Specular * s2) * LightColor2 +
		(d2 * Diffuse) * (LightColor2 * color);
}

vec3 toneMap(vec3 c) {
	c = pow(c, vec3(2.0));
	vec3 x = max(vec3(0.), c - vec3(0.004));
	return (x * (6.2 * x + 0.5)) / (x * (6.2 * x + 1.7) + 0.06);
}

vec4 rayMarch(vec3 from, vec3 dir, vec2 fragCoord, float time, vec3 offset) {
	float totalDistance = Jitter * rand(fragCoord + vec2(time));
	vec3 pos;
	float distance;
	int steps = 0;
	for (int i = 0; i < MaxSteps; i++) {
		pos = from + totalDistance * dir;
		distance = DE(pos, time, offset) * FudgeFactor;
		totalDistance += distance;
		if (distance < MinimumDistance) break;
		steps = i;
	}
	float smoothStep = float(steps) + distance / MinimumDistance;
	float ao = 1.0 - smoothStep / float(MaxSteps);
	vec3 normal = getNormal(pos - dir * normalDistance * 3.0, time, offset);
	vec3 color = getColor(normal, pos, time);
	vec3 light = getLight(color, normal, dir);
	vec3 final = toneMap((color * Ambient + light) * ao);
	final = mix(vec3(dot(final, vec3(0.333))), final, Saturation);
	final = (final - 0.5) * Contrast + 0.5;
	final *= Brightness;
	return vec4(final, 1.0);
}

void main() {
	vec2 fragCoord = gl_FragCoord.xy;
	vec2 res = RENDERSIZE;

	float time = TIME * Speed + 38.0;
	vec3 offset = vec3(1.0 + 0.2 * cos(time / 5.7), 0.3 + 0.1 * cos(time / 1.7), 1.0).xzy;

	float shakeX = (rand(vec2(TIME, 2.0)) - 0.5) * Shake;
	float shakeY = (rand(vec2(TIME, 5.0)) - 0.5) * Shake;

	vec3 camPos = Zoom * 0.5 * time * vec3(1.0, 0.0, 0.0) + vec3(shakeX, shakeY, 0.0);
	vec3 target = camPos + vec3(1.0, 0.5 * cos(time), 0.5 * sin(0.4 * time));
	vec3 camUp = vec3(0.0, cos(time / 5.0), sin(time / 5.0));

	vec3 camDir = normalize(target - camPos);
	camUp = normalize(camUp - dot(camDir, camUp) * camDir);
	vec3 camRight = normalize(cross(camDir, camUp));

	vec2 coord = -1.0 + 2.0 * fragCoord / res;
	coord.x *= res.x / res.y;

	vec3 rayDir = normalize(camDir + (coord.x * camRight + coord.y * camUp) * FieldOfView);
	gl_FragColor = rayMarch(camPos, rayDir, fragCoord, time, offset);
}
