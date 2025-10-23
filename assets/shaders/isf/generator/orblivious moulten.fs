/*
{
    "DESCRIPTION": "Moulten Core without audio reactivity: Generates a kaleidoscopic fractal raymarched scene with dynamic lighting, depth of field. Designed for a single pass rendering.",
    "ISFVSN": "2.0",
    "CATEGORIES": [
        "Fractal",
        "Kaleidoscope",
        "Psychedelic",
        "Raymarching",
        "3D",
        "Post Processing"
    ],
    "INPUTS": [
        { "NAME": "TimeMultiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
        { "NAME": "SceneRotationSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": -1.0, "MAX": 1.0, "LABEL": "Scene Rotation Speed" },
        
        { "NAME": "CameraDistance", "TYPE": "float", "DEFAULT": 3.5, "MIN": 1.0, "MAX": 10.0, "LABEL": "Camera Distance" },
        { "NAME": "CameraPulseStrength", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Camera Pulse Strength" },
        { "NAME": "CameraHeight", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0, "LABEL": "Camera Height" },

        { "NAME": "FocalLength", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 200.0, "LABEL": "Focal Length" },
        { "NAME": "Aperture", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Aperture Size" },
        { "NAME": "FieldHalfWidth", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0, "LABEL": "DoF Field Width" },
        { "NAME": "JitterStrength", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Ray Jitter" },
        { "NAME": "MaxDoFSamples", "TYPE": "float", "DEFAULT": 32.0, "MIN": 1.0, "MAX": 64.0, "LABEL": "DoF Samples (Max 64)" },
        
        { "NAME": "RefractionIndex", "TYPE": "float", "DEFAULT": 1.5, "MIN": 1.0, "MAX": 2.5, "LABEL": "Refraction Index" },
        { "NAME": "SunColorHue", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0, "LABEL": "Sun Hue" },
        { "NAME": "SunColorSaturation", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "LABEL": "Sun Saturation" },
        { "NAME": "SunColorBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Sun Brightness" },
        { "NAME": "MaterialColorHue", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Material Hue" },
        { "NAME": "MaterialColorSaturation", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Material Saturation" },
        { "NAME": "MaterialColorBrightness", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Material Brightness" },
        { "NAME": "SpecularShininess", "TYPE": "float", "DEFAULT": 64.0, "MIN": 1.0, "MAX": 256.0, "LABEL": "Specular Shininess" },
        { "NAME": "BaseReflectivity", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "LABEL": "Base Reflectivity" },
        { "NAME": "EnvFresnelFactor", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Environment Fresnel Factor" },
        
        { "NAME": "SphereBaseRadius", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "LABEL": "Sphere Base Radius" },
        { "NAME": "SphereRadiusVar", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5, "LABEL": "Sphere Radius Var" },
        { "NAME": "SmoothMinK", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 1.0, "LABEL": "Smooth Min Factor" },

        { "NAME": "HexTileMinDist", "TYPE": "float", "DEFAULT": 0.0005, "MIN": 0.00001, "MAX": 0.01, "LABEL": "Hex Pattern Min Dist" },
        { "NAME": "HexTileBrightness", "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0001, "MAX": 0.01, "LABEL": "Hex Pattern Brightness" },
        { "NAME": "HexagonBrightness", "TYPE": "float", "DEFAULT": 0.0005, "MIN": 0.00001, "MAX": 0.005, "LABEL": "Hexagon Brightness" },
        { "NAME": "PatternTimeFactor", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.1, "MAX": 10.0, "LABEL": "Pattern Time Factor" },
        { "NAME": "PatternScaleFactor", "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.1, "MAX": 2.0, "LABEL": "Pattern Scale" },
        { "NAME": "EnvironmentNoiseSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Env Noise Speed" },
        { "NAME": "EnvironmentNoiseDetail", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Env Noise Detail" },
        
        { "NAME": "GlobalBrightness", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0, "LABEL": "Global Brightness" },
        { "NAME": "GlobalSaturation", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 3.0, "LABEL": "Global Saturation" },
        { "NAME": "EnvironmentOverallBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Environment Overall Brightness" },
        { "NAME": "NoiseIntensity", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0, "LABEL": "Overall Noise Intensity" }
    ]
}
*/

precision highp float;

#define ROT(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define OFF6(n) (vec2(1.0, 0.0)*ROT(n*TAU/6.0)) // Use 6.0 for float literal

const float PI = 3.14159265359;
const float TAU = 2.0 * PI;
const float EPS = 0.0001; 
const int ITR = 20; 

const float FUDGE_FACTOR = 1.0; 

const vec3 sunDir = normalize(vec3(0.0, 1.0, 5.0)); 

// --- HSV to RGB Conversion ---
const vec4 hsv2rgb_K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
vec3 hsv2rgb(vec3 c) {
  vec3 p = abs(fract(c.xxx + hsv2rgb_K.xyz) * 6.0 - hsv2rgb_K.www);
  return c.z * mix(hsv2rgb_K.xxx, clamp(p - hsv2rgb_K.xxx, 0.0, 1.0), c.y);
}

// --- Utility Functions ---
float rand2D(vec2 co) {
    return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

float rnd3D(vec3 p) {
    return fract(sin(dot(p, vec3(12.9898, 78.233, 37.719))) * 43758.5453123);
}

float noise3D(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = f * f * (3.0 - 2.0 * f);
    float a = rnd3D(i + vec3(0.0,0.0,0.0));
    float b = rnd3D(i + vec3(1.0,0.0,0.0));
    float c = rnd3D(i + vec3(0.0,1.0,0.0));
    float d = rnd3D(i + vec3(1.0,1.0,0.0));
    float e = rnd3D(i + vec3(0.0,0.0,1.0));
    float f_ = rnd3D(i + vec3(1.0,0.0,1.0));
    float g = rnd3D(i + vec3(0.0,1.0,1.0));
    float h = rnd3D(i + vec3(1.0,1.0,1.0));
    return mix(mix(mix(a,b,u.x),mix(c,d,u.x),u.y),mix(mix(e,f_,u.x),mix(g,h,u.x),u.y),u.z);
}

// --- Hextile Environment Functions (GLSL ES 1.00 array initialization workaround) ---
const vec2 off6_0 = OFF6(0.0);
const vec2 off6_1 = OFF6(1.0);
const vec2 off6_2 = OFF6(2.0);
const vec2 off6_3 = OFF6(3.0);
const vec2 off6_4 = OFF6(4.0);
const vec2 off6_5 = OFF6(5.0);

const vec2 noff6_0 = vec2(-1.0, 0.0);
const vec2 noff6_1 = vec2(-0.5, 0.5);
const vec2 noff6_2 = vec2( 0.5, 0.5);
const vec2 noff6_3 = vec2( 1.0, 0.0);
const vec2 noff6_4 = vec2( 0.5,-0.5);
const vec2 noff6_5 = vec2(-0.5,-0.5);

vec2 get_off6(int i) {
    if (i == 0) return off6_0;
    if (i == 1) return off6_1;
    if (i == 2) return off6_2;
    if (i == 3) return off6_3;
    if (i == 4) return off6_4;
    return off6_5; 
}

vec2 get_noff6(int i) {
    if (i == 0) return noff6_0;
    if (i == 1) return noff6_1;
    if (i == 2) return noff6_2;
    if (i == 3) return noff6_3;
    if (i == 4) return noff6_4;
    return noff6_5; 
}

vec2 round_compat_vec2(vec2 x) {
    return floor(x + 0.5);
}

vec2 hextile(inout vec2 p) {
    const vec2 sz = vec2(1.0, sqrt(3.0));
    const vec2 hsz = 0.5*sz;
    vec2 p1 = mod(p, sz)-hsz;
    vec2 p2 = mod(p - hsz, sz)-hsz;
    vec2 p3 = dot(p1, p1) < dot(p2, p2) ? p1 : p2;
    vec2 n = ((p3 - p + hsz)/sz);
    p = p3;
    n -= vec2(0.5);
    return round_compat_vec2(n*2.0)*0.5; 
}

float hexagon(vec2 p, float r) {
    p = p.yx;
    const vec3 k = vec3(-0.86602540378, 0.5, 1.154700538); 
    p = abs(p);
    p -= 2.0*min(dot(k.xy,p),0.0)*k.xy;
    p -= vec2(clamp(p.x, -k.z*r, k.z*r), r);
    return length(p)*sign(p.y);
}

float hash_vec2(vec2 co) { 
    co += 1.234;
    return fract(sin(dot(co.xy ,vec2(12.9898,58.233))) * 13758.5453);
}

float hash_float(float co) { 
    return fract(sin(co*12.9898) * 13758.5453);
}

float dot2(vec2 p) {
    return dot(p, p);
}

float bezier(vec2 pos, vec2 A, vec2 B, vec2 C) {
    vec2 a = B - A;
    vec2 b = A - 2.0*B + C;
    vec2 c = a * 2.0;
    vec2 d = A - pos;
    float kk = 1.0/dot(b,b);
    float kx = kk * dot(a,b);
    float ky = kk * (2.0*dot(a,a)+dot(d,b)) / 3.0;
    float kz = kk * dot(d,a);
    float res = 0.0;
    float p = ky - kx*kx;
    float p3 = p*p*p;
    float q = kx*(2.0*kx*kx-3.0*ky) + kz;
    float h = q*q + 4.0*p3;
    if( h >= 0.0) {
        h = sqrt(h);
        vec2 x = (vec2(h,-h)-q)/2.0;
        vec2 uv_bezier = sign(x)*pow(abs(x), vec2(1.0/3.0));
        float t = clamp( uv_bezier.x+uv_bezier.y-kx, 0.0, 1.0 );
        res = dot2(d + (c + b*t)*t);
    } else {
        float z = sqrt(-p);
        float v = acos( q/(p*z*2.0) ) / 3.0;
        float m = cos(v);
        float n = sin(v)*1.732050808;
        vec3  t = clamp(vec3(m+m,-n-m,n-m)*z-kx,0.0,1.0);
        res = min( dot2(d+(c+b*t.x)*t.x), dot2(d+(c+b*t.y)*t.y) );
    }
    return sqrt( res );
}

vec2 coff(float h) {
    float h0 = h;
    float h1 = fract(h0*9677.0);
    float h2 = fract(h0*8677.0);
    float t = mix(0.5, 1.0, h2*h2)*TIME*PatternTimeFactor+1234.5*h0;
    return mix(0.1, 0.2, h1*h1)*sin(t*vec2(1.0, sqrt(0.5)));
}

// Keeping aces_approx but we'll control its application more carefully
vec3 aces_approx(vec3 v) {
    v = max(v, 0.0);
    v *= 0.6; // Slightly less aggressive by default
    float a = 2.51;
    float b = 0.03;
    float c = 2.43;
    float d = 0.59;
    float e = 0.14;
    return clamp((v*(a*v+b))/(v*(c*v+d)+e), 0.0, 1.0);
}

// Function to adjust saturation
vec3 adjustSaturation(vec3 color, float saturation) {
    vec3 lum = vec3(dot(vec3(0.2126, 0.7152, 0.0722), color)); // ITU-R BT.709
    return mix(lum, color, saturation);
}


vec3 getPatternColor(vec2 hp) {
    vec3 bcol = 0.5*(1.0+cos(vec3(0.0, 1.0, 2.0) + dot(hp, hp)*0.1-0.5*TIME));
    vec3 col = vec3(0.0);
    
    vec2 hn = hextile(hp);
    float h0 = hash_vec2(hn); 
    vec2 p0 = coff(h0);

    float mx = HexTileMinDist; 

    for (int i = 0; i < 6; ++i) {
        vec2 current_noff6 = get_noff6(i); 
        float h1 = hash_vec2(hn + current_noff6); 
        vec2 current_off6 = get_off6(i); 
        vec2 p1 = current_off6 + coff(h1);
        float fade = smoothstep(1.05, 0.85, distance(p0, p1));
        if (fade < 0.025) continue;
        float h2 = h0+h1;
        vec2 p2 = 0.5*(p1+p0)+coff(h2);
        float dd = bezier(hp, p0, p2, p1);
        float gd = abs(dd);
        gd *= sqrt(gd);
        gd = max(gd, mx);
        col += fade*HexTileBrightness*bcol/(gd); 
    }

    float cd = length(hp-p0);
    float gd = abs(cd);
    gd *= (gd);
    gd = max(gd, mx);
    col += (HexTileBrightness*1.25)*sqrt(bcol)/(gd); 

    float hd = hexagon(hp, 0.485);
    gd = abs(hd);
    gd = max(gd, mx*10.0);
    col += HexagonBrightness*bcol*bcol/(gd); 
    
    return col;
}

// --- Depth of Field Functions ---
float pixelSize;
float focalDistance;

float circleOfConfusion(float t) {
    return max(abs(focalDistance - t) - FieldHalfWidth, 0.0) * (FocalLength / (2.0 * Aperture)) * 0.01 + pixelSize * t;
}

// --- SDF Functions ---
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * h * k * (1.0/6.0);
}

float sdSphere(vec3 p, float s) {
    return length(p) - s;
}

mat3 rotY(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(c, 0.0, s, 0.0, 1.0, 0.0, -s, 0.0, c);
}

mat3 rotX(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat3(1.0, 0.0, 0.0, 0.0, c, -s, 0.0, s, c);
}

// --- Scene Definition ---
float map(vec3 p) {
    float time1 = TIME * TimeMultiplier * 0.8;
    float time2 = TIME * TimeMultiplier * 1.2;
    float time3 = TIME * TimeMultiplier * 0.6;
    
    float baseRadius = SphereBaseRadius;
    float radiusVariation = SphereRadiusVar;
    float k = SmoothMinK;
    
    vec3 pos1 = vec3(cos(time1) * 0.8, sin(time1) * 0.8, sin(time1 * 1.5) * 0.5);
    float radius1 = baseRadius + sin(time1 * 2.0) * radiusVariation;
    
    vec3 pos2 = vec3(cos(time2 + PI * 2.0/3.0) * 0.7, cos(time2 * 0.7) * 0.6, sin(time2 + PI * 2.0/3.0) * 0.9);
    float radius2 = baseRadius + cos(time2 * 1.8) * radiusVariation;
    
    vec3 pos3 = vec3(sin(time3 + PI * 4.0/3.0) * 0.6, cos(time3 * 1.3 + PI * 4.0/3.0) * 0.5, cos(time3 * 0.9) * 0.7);
    float radius3 = baseRadius + sin(time3 * 2.5) * radiusVariation;
    
    float globalRotY = TIME * SceneRotationSpeed;
    float globalRotX = sin(TIME * 0.15) * 0.4;
    mat3 globalRot = rotY(globalRotY) * rotX(globalRotX);
    
    pos1 = globalRot * pos1;
    pos2 = globalRot * pos2;
    pos3 = globalRot * pos3;
    
    float sphere1 = sdSphere(p - pos1, radius1);
    float sphere2 = sdSphere(p - pos2, radius2);
    float sphere3 = sdSphere(p - pos3, radius3);
    
    float d = smin(sphere1, sphere2, k);
    d = smin(d, sphere3, k * 0.95);
    
    return d;
}

// --- Normal Generation ---
vec3 generateNormal(vec3 p) {
    // Incorporate NoiseIntensity here, so it affects the 'roughness' of normals
    return normalize(vec3(
        map(p + vec3(EPS, 0.0, 0.0)) - map(p - vec3(EPS, 0.0, 0.0)),
        map(p + vec3(0.0, EPS, 0.0)) - map(p - vec3(0.0, EPS, 0.0)),
        map(p + vec3(0.0, 0.0, EPS)) - map(p - vec3(0.0, 0.0, EPS))
    ) + (noise3D(p * 0.25) - 0.5) * 2.0 * NoiseIntensity); // Added NoiseIntensity
}

// --- Environment/Skybox Rendering ---
vec3 render_environment(vec3 ro, vec3 rd, vec2 u) {
    float reactiveOffset = 0.0; // No audio reactivity

    float n, s_val_local; 
    float T = TIME*2.0, ft = fract(T);
    float t = floor(T)+sqrt(ft);
    float d = reactiveOffset * 0.5 + rand2D(u * 10.0 + TIME); 
    
    vec3 w,p_env = vec3(0.0); 
    vec2 u_temp = rd.xy; 
    vec4 o=vec4(0.0,0.0,0.0,1.0); 

    s_val_local = 0.002; 
    
    const int ENV_ITR = 40; 
    
    for (float i = 0.0; i < float(ENV_ITR); ++i) 
    {
        if (s_val_local <= 0.0002) {
            break; 
        }

        float new_s_val_local = 0.05 + 0.8 * abs(min(s_val_local, 4.0 - abs(w.x)));
        d += s_val_local; 
        s_val_local = new_s_val_local; 
        
        // Modified accumulation and added EnvironmentOverallBrightness
        o += (1.0+cos(fract(t*0.1)+0.1*w.z+p_env.z*0.1+ vec4(2.0,1.0,0.0,0.0)))/s_val_local/d + EnvironmentNoiseSpeed * vec4(1.0, 1.0, 1.0, 0.0);
        
        p_env = w = vec3(u_temp * d, d); 
        w.xy *= ROT(t+w.z*0.2);
        p_env.xy *= ROT(t+p_env.z*0.5);
        n = 1.5;
        s_val_local = 5.0 - length(p_env.xy); 
        
        for (int j = 0; j < 16; ++j) { 
            if (n >= 16.0) break; 
            // Adjusted noise contribution with NoiseIntensity
            s_val_local += noise3D(3.0*t+p_env) * EnvironmentNoiseDetail * NoiseIntensity; 
            w += noise3D(6.0*t+w) * EnvironmentNoiseDetail * NoiseIntensity; 
            n += n; 
        }
    }
    
    // Adjusted division for brighter environment
    // Increased base brightness and reduced the impact of the final divisors
    o = (o * EnvironmentOverallBrightness) / (50.0 + length(u_temp) * 5.0 + pow(ft, 0.225) * 10.0 + length(cos(4.0/ft*T*u_temp/log((1.0+cos(t)*16.0+32.0)/d))));
    
    // Applying aces_approx at the very end of environment render, but can be skipped for more raw look
    return aces_approx(o.rgb);
}

// --- Random on Disk for DOF ---
vec2 randomOnDisk(vec2 seed) {
    float angle = rand2D(seed) * 2.0 * PI;
    float radius = sqrt(rand2D(seed + vec2(1.0, 0.0)));
    return vec2(cos(angle), sin(angle)) * radius;
}

// --- Custom lookAt for GLSL ES 1.00 ---
mat3 customLookAt(vec3 eye, vec3 center, vec3 up) {
    vec3 zaxis = normalize(center - eye);
    vec3 xaxis = normalize(cross(zaxis, up));
    vec3 yaxis = cross(xaxis, zaxis); 

    mat3 m;
    m[0][0] = xaxis.x; m[0][1] = xaxis.y; m[0][2] = xaxis.z;
    m[1][0] = yaxis.x; m[1][1] = yaxis.y; m[1][2] = yaxis.z;
    m[2][0] = zaxis.x; m[2][1] = zaxis.y; m[2][2] = zaxis.z;
    return m;
}

vec3 shade(vec3 pos, vec3 normal, vec3 rayDir, vec2 fragCoord); // Forward declaration

vec3 renderRay(vec3 rayPos, vec3 rayDir, vec2 fragCoord) {
    float t = 0.0;
    
    for (int i = 0; i < ITR; ++i) { 
        float d = map(rayPos + rayDir * t);
        
        if (d < EPS) {
            vec3 hitPos = rayPos + rayDir * t;
            vec3 normal = generateNormal(hitPos);
            return shade(hitPos, normal, rayDir, fragCoord); 
        }
        
        float stepSize = max(d * 0.9, EPS * 2.0);
        if (JitterStrength > 0.0) { 
            stepSize *= mix(1.0, 0.5 + 0.5 * rand2D(fragCoord * 0.01 + vec2(float(i)) * t), JitterStrength);
        }
        
        t += stepSize;
        if (t > 20.0) break; 
    }

    return render_environment(rayPos, rayDir, fragCoord);
}

vec3 shade(vec3 pos, vec3 normal, vec3 rayDir, vec2 fragCoord) {
    // Original line: vec3 reflectionNormal = normalize(normal + (noise3D(pos * 0.25) - 0.5) * 2.0);
    // Moved noise to generateNormal for consistency and to be controlled by NoiseIntensity input.
    vec3 reflectionNormal = normal; 
    
    vec3 reflectDir = reflect(rayDir, reflectionNormal);
    vec3 refractDir = refract(rayDir, normal, 1.0 / RefractionIndex); 
    
    vec3 sunCol = hsv2rgb(vec3(SunColorHue, SunColorSaturation, SunColorBrightness));
    vec3 materialCol = -hsv2rgb(vec3(MaterialColorHue, MaterialColorSaturation, MaterialColorBrightness)); // Renamed variable for clarity

    float fresnel = BaseReflectivity + (1.0 - BaseReflectivity) * pow(1.0 + dot(rayDir, normal), 5.0); 

    vec3 reflectionColor = render_environment(pos, reflectDir, fragCoord);

    vec3 refractionColor = vec3(0.0);
    if (dot(refractDir, refractDir) > 0.0001) { 
        // Use MaterialColor inputs for internal glow
        vec3 materialInnerColor = hsv2rgb(vec3(MaterialColorHue, MaterialColorSaturation, MaterialColorBrightness)); 
        vec3 internalGlow = materialInnerColor * 0.5; // Base glow

        float absorptionFactor = pow(1.0 - abs(dot(rayDir, normal)), 2.0) * EnvFresnelFactor; 
        vec3 absorption = exp(materialCol * absorptionFactor); // Use materialCol here (negative hsv for absorption)
        refractionColor = internalGlow * absorption;
    }
    
    vec3 finalColor = mix(refractionColor, reflectionColor, fresnel * EnvFresnelFactor * sqrt(fresnel)); 

    vec3 viewDir = -rayDir;
    vec3 halfwayDir = normalize(sunDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), SpecularShininess); 
    vec3 specularHighlight = sunCol * spec * 2.5; // Increased multiplier for brighter specular
    
    finalColor += specularHighlight;

    return finalColor;
}

void main() {
    float time = TIME * TimeMultiplier; 

    // --- Render Core Scene ---
    pixelSize = 2.0 / RENDERSIZE.y / FocalLength; 
    
    float camDistance = CameraDistance + CameraPulseStrength * sin(time * 0.5); 
    float camAngle = SceneRotationSpeed * TIME * 0.2; 
    float camHeight = CameraHeight; 
    
    vec3 camPos = vec3(
        cos(camAngle) * camDistance,
        camHeight,
        sin(camAngle) * camDistance
    );
    vec3 target = vec3(0.0, 0.0, 0.0);
    
    focalDistance = length(camPos - target);

    vec3 forward = normalize(target - camPos);
    vec3 up = vec3(0.0, 1.0, 0.0);
    
    mat3 camMatrix = customLookAt(camPos, target, up); 
    
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    float fov = 1.0; 
    
    vec3 baseRayDir = normalize(camMatrix * vec3(uv * fov, FocalLength * 0.02)); 
    
    float rCoC = circleOfConfusion(length(camPos - target));
    int numSamples = int(clamp(rCoC * 200.0 + 1.0, 1.0, MaxDoFSamples)); 

    vec3 finalRenderColor = vec3(0.0);
    
    if (numSamples <= 1) {
        finalRenderColor = renderRay(camPos, baseRayDir, gl_FragCoord.xy);
    } else {
        const int MAX_DOF_ITR = 64; 
        for (int i = 0; i < MAX_DOF_ITR; ++i) { 
            if (i >= numSamples) break; 
            
            vec2 apertureSeed = gl_FragCoord.xy + vec2(float(i) * 23.45, float(i) * 67.89);
            vec2 apertureOffset = randomOnDisk(apertureSeed) * rCoC * 0.5;
            
            vec3 right = normalize(cross(forward, up));
            vec3 apertureUp = normalize(cross(right, forward));
            vec3 apertureCamPos = camPos + right * apertureOffset.x + apertureUp * apertureOffset.y;
            
            vec3 focusPoint = camPos + baseRayDir * focalDistance;
            vec3 newRayDir = normalize(focusPoint - apertureCamPos);
            
            finalRenderColor += renderRay(apertureCamPos, newRayDir, gl_FragCoord.xy);
        }
        finalRenderColor /= float(numSamples);
    }
    
    // Post-processing for core rendering 
    // Apply GlobalBrightness and GlobalSaturation to overall render
    finalRenderColor = finalRenderColor * GlobalBrightness;
    finalRenderColor = adjustSaturation(finalRenderColor, GlobalSaturation);

    // Apply gamma correction (approx 2.2)
    finalRenderColor = pow(finalRenderColor, vec3(1.0/2.2)); 
    
    // Removed the problematic division here: finalRenderColor = finalRenderColor / (1.0 + finalRenderColor * 0.8);
    // This division can flatten bright colors significantly.
    
    gl_FragColor = vec4(finalRenderColor, 1.0); 
}