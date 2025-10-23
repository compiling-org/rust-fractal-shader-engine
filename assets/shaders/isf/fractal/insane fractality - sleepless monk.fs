/*{
  "CATEGORIES": ["Fractal", "Psychedelic", "Volumetric"],
  "DESCRIPTION": "Merged psychedelic raymarcher with full control stack",
  "ISFVSN": "2.0",
  "INPUTS": [
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 8.0, "MIN": 0.1, "MAX": 50.0 },
    { "NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "TransformMode", "TYPE": "float", "DEFAULT": 1.8, "MIN": 0, "MAX": 5 },
    { "NAME": "GeometryType", "TYPE": "float", "DEFAULT": 3, "MIN": 0, "MAX": 6 },
    { "NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.43, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.66, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 19, "MIN": 0, "MAX": 19 },
    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0, "MAX": 3.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "Glow", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Symmetry", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "Sharpness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "FalloffCurve", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "CameraOrbit", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "CameraPitch", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.57, "MAX": 1.57 },
    { "NAME": "CameraRoll", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "FocusNear", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0 },
    { "NAME": "FocusFar", "TYPE": "float", "DEFAULT": 2.6, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "FOV", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "StepCount", "TYPE": "float", "DEFAULT": 6, "MIN": 1, "MAX": 60 },
    { "NAME": "Texture", "TYPE": "image" },
    { "NAME": "TextureWarp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "TextureScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "ColorPulseSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
{ "NAME": "HueShiftSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
{ "NAME": "GeometryDensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }

  ]
}*/

#define MAX_STEPS 150
#define BAILOUT 300.0
#define PI 3.14159

// --------------------------------------
// Rotation matrix for camera
mat3 cameraMatrix(float orbit, float pitch, float roll) {
  float co = cos(orbit), so = sin(orbit);
  float cp = cos(pitch), sp = sin(pitch);
  float cr = cos(roll), sr = sin(roll);
  return mat3(
    co * cr + so * sp * sr, sr * cp, -so * cr + co * sp * sr,
    -co * sr + so * sp * cr, cr * cp, sr * so + co * sp * cr,
    so * cp, -sp, co * cp
  );
}

// --------------------------------------
// Triplanar texture sampler
vec3 triplanarTexture(vec3 p, float scale) {
  vec3 blend = normalize(abs(p));
  blend = pow(blend, vec3(4.0));
  blend /= dot(blend, vec3(1.0));

  vec2 xz = fract(p.zy * scale);
  vec2 yz = fract(p.xz * scale);
  vec2 xy = fract(p.xy * scale);

  vec3 tx = texture2D(Texture, xz).rgb;
  vec3 ty = texture2D(Texture, yz).rgb;
  vec3 tz = texture2D(Texture, xy).rgb;

  return tx * blend.x + ty * blend.y + tz * blend.z;
}

// --------------------------------------
// HSV to RGB
vec3 hsv2rgb(vec3 c) {
  vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
  vec3 p = abs(fract(c.xxx + K.xyz)*6.0 - K.w);
  return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// --------------------------------------
// Palette function
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
  return a + b * cos(6.2831 * (c * t + d));
}

vec3 getColorPalette(int mode, float t) {
  return pal(t,
    vec3(0.5 + 0.4*sin(float(mode)*0.5), 0.6 + 0.3*cos(float(mode)*1.2), 0.4 + 0.5*sin(float(mode)*0.9)),
    vec3(0.4),
    vec3(1.0,1.3,0.7),
    vec3(0.1,0.2,0.3)
  );
}
// --------------------------------------
// Transformation logic from Shader 1
vec3 applyTransform(vec3 p, int mode, float chaos, float sym, float chspd) {
  p *= max(sym, 0.001);

  if (mode == 1) p = abs(p);
  else if (mode == 2) p += sin(p * 3.0 + TIME * chspd) * chaos * 0.3;
  else if (mode == 3) {
    p += sin(p * (1.0 + chaos * 2.0) + TIME * chspd) * chaos * 0.5;
    p = fract(p * 1.5) - 0.75;
  }
  else if (mode == 4 || mode == 5) {
    float a = atan(p.z, p.x);
    float r = length(p.xz);
    float spin = TIME * chspd * (mode == 4 ? 0.2 : 0.3);
    a += spin;
    p.x = cos(a) * r;
    p.z = sin(a) * r;
  }

  return p;
}

// --------------------------------------
// Psychedelic chaos layer
float shapeChaos(vec3 p, float chaos) {
  return (sin(p.x*3. + TIME*ChaosSpeed) + sin(p.y*4. + TIME*ChaosSpeed*1.2) + sin(p.z*5. + TIME*ChaosSpeed*0.8)) * chaos;
}

// --------------------------------------
// Fractal-style geometry using shader 2 logic
float tailDragonShape(vec3 p, float density) {
  float s = cos(cos(p.x * density) - cos(p.y * density));
  s += abs(dot(sin(p * 8.0 * density), vec3(0.1)));
  return s;
}

// --------------------------------------
// Scene distance function
float scene(vec3 p, int geo, float chaos, float chaosMix, float density) {
  float base = 0.0;

  if (geo == 0) base = length(p) - 1.0;
  else if (geo == 1) {
    vec2 q = vec2(length(p.xz)-1.0, p.y);
    base = length(q) - 0.3;
  }
  else {
    base = tailDragonShape(p, density); // default to Tail of the Dragon
  }

  float chaosLayer = shapeChaos(p, chaos);
  return mix(base, chaosLayer, chaosMix);
}
void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  uv *= FOV;
  float t = TIME * Speed;

  vec3 ro = vec3(0.0, 0.0, -3.0);
  vec3 rd = normalize(vec3(uv * Zoom, 1.0));
  rd = cameraMatrix(CameraOrbit, CameraPitch, CameraRoll) * rd;

  // Texture warp from shader 1
  vec3 warp = triplanarTexture(ro * TextureScale, 1.0) - 0.5;
  vec3 roWarped = ro + warp * TextureWarp;

  vec3 col = vec3(0.0);
  float dist = 0.0;

  int mode = int(TransformMode);
  int geo = int(GeometryType);
  float chaos = ChaosIntensity;
  float chaosMix = ChaosMix;
  float sym = Symmetry;
  float chspd = ChaosSpeed;
  float br = Brightness;
  float ct = Contrast;
  float glow = Glow;
  int palMode = int(ColorPaletteMode);
  float sharp = Sharpness;
  float falloff = FalloffCurve;
  float density = GeometryDensity;

  for (int i = 0; i < MAX_STEPS; i++) {
    if (i >= int(StepCount)) break;

    vec3 p = roWarped + dist * rd;
    p = applyTransform(p, mode, chaos, sym, chspd);

    float d = scene(p, geo, chaos, chaosMix, density);
    d = max(abs(d), 0.01);

    float fade = exp(-float(i) * 0.03 * sharp);
    float focus = smoothstep(FocusNear, FocusFar, dist);

    // Palette and hue shifting
    float pulse = 0.5 + 0.5 * sin(TIME * ColorPulseSpeed + length(p) * 0.1);
    float hue = fract(HueShiftSpeed * t + pulse * 0.2);

    vec3 paletteColor;
    if (palMode == 0) paletteColor = hsv2rgb(vec3(hue, 1.0, 1.0));
    else if (palMode == 1) {
      float trippy = fract(hue + sin(length(p) * 0.5 + TIME * 3.0) * 0.3);
      paletteColor = hsv2rgb(vec3(trippy, 1.0, 0.5 + pulse * 0.5));
      paletteColor = mix(paletteColor, vec3(sin(TIME * 7.0 + length(p) * 0.2)), 0.3);
    } else if (palMode == 2) paletteColor = hsv2rgb(vec3(hue, 0.3 + pulse * 0.7, 0.8 + pulse * 0.2));
    else if (palMode == 3) {
      vec3 vapor = vec3(0.85, 0.6, 0.75);
      float hm = fract(TIME * 0.5 + length(p) * 0.05);
      paletteColor = hsv2rgb(vec3(mix(vapor.x, vapor.y, sin(hm * PI) * 0.5 + 0.5), 1.0, 1.0));
      paletteColor = mix(paletteColor, hsv2rgb(vec3(vapor.z, 1.0, 1.0)), abs(sin(TIME * 2.5)));
    } else {
      paletteColor = getColorPalette(palMode, p.z + t * 0.1);
    }

    vec3 texCol = triplanarTexture(p * TextureScale, 1.0);
    float b = 0.005 / (0.01 + d * falloff);

    col += mix(paletteColor, texCol, 0.5) * b * fade * focus * Glow;
    dist += d;
    if (dist > BAILOUT) break;
  }

  float pulse = sin(TIME * ChaosSpeed) * 0.5 + 0.5;
  col *= 1.0 + 0.3 * pulse;
  col = (col - 0.5) * ct + 0.5;
  col *= br;

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
