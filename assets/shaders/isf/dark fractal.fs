/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "inputTex",     "TYPE": "image" },
    { "NAME": "speed",        "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "zoom",         "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 5.0 },
    { "NAME": "shake",        "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1 },
    { "NAME": "colorPulse",   "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.50 },
    { "NAME": "brightness",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 20.0 },
    { "NAME": "contrast",     "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "saturation",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "morph",        "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "palette",      "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 6.999 },
    { "NAME": "foldingLimit", "TYPE": "float", "DEFAULT": 1.3, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "scale",        "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "fixedRadius",  "TYPE": "float", "DEFAULT": 5.7, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "minRadius",    "TYPE": "float", "DEFAULT": 2.9, "MIN": 0.01, "MAX": 3.0 },
    { "NAME": "glowBoost",    "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 10.0 }
  ]
}*/

#define pi acos(-1.)
#define tau (2.*pi)
#define pal(a,b,c,d,e) ((a) + (b)*sin((c)*(d) + (e)))
#define rot(x) mat2(cos(x),-sin(x),sin(x),cos(x))

vec3 glow = vec3(0.);

void sphereFold(inout vec3 z, inout float dz) {
	float r2 = dot(z,z);
	float minRadius2 = minRadius * minRadius;
	float fixedRadius2 = fixedRadius * fixedRadius;
	if (r2 < minRadius2) { 
		float temp = fixedRadius2 / minRadius2;
		z *= temp;
		dz *= temp;
	} else if (r2 < fixedRadius2) { 
		float temp = fixedRadius2 / r2;
		z *= temp;
		dz *= temp;
	}
}

void boxFold(inout vec3 z, inout float dz) {
	z = clamp(z, -foldingLimit, foldingLimit) * 2.0 - z;
}

#define pmod(p,j) mod(p,j) - 0.5*j

float map(vec3 z, float t){
	float d = 10e7;
	vec3 p = z;
	z.z = pmod(z.z, 10.);

	for(int i = 0; i < 4; i++){
		z = abs(z);
		z.xy *= rot(0.125 * pi);
	}

	vec3 q = vec3(z);
	vec3 j;
	float jdr;
	vec3 offset = z;
	float dr = 1.;

	for (int n = 0; n < 7; n++) {
		boxFold(z, dr);
		sphereFold(z, dr);
		if (n == 2) {
			j = z;
			jdr = dr;
		}
		z = scale * z + offset;
		dr = dr * abs(scale) + 1.0;
	}

	float r = length(z);
	d = r / abs(dr);
	d *= 0.7;
	d += smoothstep(1., 0., t * 0.75) * 0.15;

	float db = length(j) / abs(jdr) + 0.001;
	float att = pow(abs(sin(p.z + t + length(p.xy))), 50.);
	glow += glowBoost * 0.5 / (0.6 + pow((abs(d) + 0.001) * 0.7, 2.) * 800000.) * 0.9;
	glow -= glowBoost * 0.92 / (0.04 + pow((abs(db) + 0.001) * 0.7, 2.) * 16000.) * vec3(0.5, 0.9, 1.4) * att;

	d = min(d, db);
	d = abs(d) + 0.001;
	return d;
}

vec3 getRd(vec3 ro, vec3 lookAt, vec2 uv){
	vec3 dir = normalize(lookAt - ro);
	vec3 right = normalize(cross(vec3(0.,1.,0), dir));
	vec3 up = normalize(cross(dir, right));
	return normalize(dir + right * uv.x + up * uv.y);
}

vec3 getPalette(int i, float t){
	if (i == 0) return 0.5 + 0.5 * sin(vec3(6,2,1) * t + vec3(0,2,4));
	if (i == 1) return vec3(sin(t*5.1), cos(t*3.3), sin(t*2.2));
	if (i == 2) return vec3(0.3,0.8,0.6) + vec3(sin(t*2.0), sin(t*1.1), sin(t*0.6));
	if (i == 3) return vec3(sin(t*8.0), sin(t*4.0), sin(t*2.0));
	if (i == 4) return vec3(0.7 + 0.3*sin(t), 0.4 + 0.2*cos(t*1.5), 0.8);
	if (i == 5) return vec3(0.9, 0.3 + 0.2*sin(t), 0.2 + 0.1*cos(t));
	return vec3(0.1 + 0.5*sin(t*3.), 0.8, 0.5 + 0.3*cos(t*1.2));
}

vec3 palCol(float index, float t){
	float i = floor(index);
	float f = fract(index);
	vec3 a = getPalette(int(i), t);
	vec3 b = getPalette(int(i + 1), t);
	return mix(a, b, f);
}

vec3 applyVFX(vec3 col){
	col = (col - 0.5) * contrast + 0.5;
	float g = dot(col, vec3(0.333));
	col = mix(vec3(g), col, saturation);
	col *= brightness;
	return clamp(col, 0.0, 1.0);
}

void main() {
	vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	float t = TIME * speed;

	uv += sin(vec2(t * 13.1, t * 9.7)) * shake;
	uv *= zoom;

	vec3 ro = vec3(0);
	ro.z += t;

	float T = t / tau + pi * 0.25;
	ro.xy += vec2(cos(T), sin(T)) * 0.7;

	vec3 lookAt = vec3(0.001);
	lookAt.z = ro.z + 4.;

	vec3 rd = getRd(ro, lookAt, uv);

	float d;
	vec3 p = ro;
	float dist = 0.;
	for(int i = 0; i < 120; i++){
		d = map(p, t);
		if (d < 0.001) break;
		dist += d;
		p = ro + rd * dist;
	}

	vec3 textureColor = IMG_PIXEL(inputTex, gl_FragCoord.xy / RENDERSIZE.xy).rgb;
	vec3 pal = palCol(palette + morph, t);
	vec3 col = pal * textureColor * colorPulse;

	col -= glow * 0.07;
	col = max(col, 0.);
	col = pow(col, vec3(1. + dot(uv, uv)));
	col = applyVFX(col);
	col = pow(col, vec3(0.454545));

	gl_FragColor = vec4(col, 1.0);
}
