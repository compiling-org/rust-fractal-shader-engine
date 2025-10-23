/*
{
  "DESCRIPTION": "An advanced ISF shader for 'Compleat Mandelbox' by dr2, featuring restored time-based animations, expanded color palettes, and new optional controls for fractal geometry animation, automated camera pan, camera shake, and post-processing effects like brightness, contrast, and saturation.",
  "CATEGORIES": [ "Fractal", "3D", "Raymarching", "Procedural", "Abstract" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "forward_speed", "TYPE": "float", "LABEL": "Forward Speed", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "zoom_factor", "TYPE": "float", "LABEL": "Zoom Factor", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "vertical_position", "TYPE": "float", "LABEL": "Vertical Position", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fractal_scale", "TYPE": "float", "LABEL": "Fractal Scale", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "camera_elevation", "TYPE": "float", "LABEL": "Camera Elevation", "DEFAULT": 0.5, "MIN": -1.5, "MAX": 1.5 },
    { "NAME": "camera_azimuth", "TYPE": "float", "LABEL": "Camera Azimuth", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159 },

    { "NAME": "fractal_animation_speed", "TYPE": "float", "LABEL": "Fractal Animation Speed", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "camera_pan_speed", "TYPE": "float", "LABEL": "Camera Pan Speed", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "camera_elevation_speed", "TYPE": "float", "LABEL": "Camera Elevation Speed", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "shake_intensity", "TYPE": "float", "LABEL": "Shake Intensity", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1 },

    { "NAME": "color_scheme_select", "TYPE": "float", "LABEL": "Color Palette", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0 },
    { "NAME": "brightness", "TYPE": "float", "LABEL": "Brightness", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "LABEL": "Contrast", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "saturation", "TYPE": "float", "LABEL": "Saturation", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}
*/

// optional antialiasing - can be toggled in ISF editor
#define AA 1

// Utility functions
vec3 HsvToRgb (vec3 c);
vec3 RgbToHsv (vec3 c);
float Maxv3 (vec3 p);
vec2 Rot2D (vec2 q, float a);

// Global variables for the ISF environment
vec3 ltPos[2], ltAx;
float dstFar, mScale, vuHt;
const float pi = 3.14159;
const float itMax = 12.;

// The Mandelbox Distance Field function with optional animation
float ObjDf (vec3 p)
{
  vec4 p4;
  vec3 q;
  float d;
  q = p;
  // Apply the optional fractal animation offset based on fractal_animation_speed
  p += vec3(sin(TIME * 0.5), cos(TIME * 0.8), sin(TIME * 1.1)) * fractal_animation_speed;

  p = mod(p + 3., 6.) - 3.;
  p4 = vec4 (p, 1.);
  for (float j = 0.; j < itMax; j ++) {
    p4.xyz = 2. * clamp(p4.xyz, -1., 1.) - p4.xyz;
    p4 = mScale * p4 / clamp(dot(p4.xyz, p4.xyz), 0.25, 1.) + vec4 (p, 1.);
  }
  d = length(p4.xyz) / p4.w;
  q.y -= vuHt;
  d = max(d, 0.02 - length(q.xy));
  return d;
}

// Function to determine the color of the object, with new palettes
vec4 ObjCol (vec3 p, float chCol, float tCur)
{
  vec4 col4;
  vec3 p3;
  float pp, ppMin, cn, cr;
  p = mod(p + 3., 6.) - 3.;
  p3 = p;
  cn = 0.;
  cr = 0.;
  ppMin = 1.;
  for (float j = 0.; j < itMax; j ++) {
    if (any(greaterThan(p3, vec3 (1.)))) ++cr;
    p3 = 2. * clamp(p3, -1., 1.) - p3;
    pp = dot(p3, p3);
    if (pp < ppMin) {
      cn = j;
      ppMin = pp;
    }
    if (abs(pp - 0.625) > 0.375) ++cr;
    p3 = mScale * p3 / clamp(pp, 0.25, 1.) + p;
  }

  // Color palette selection logic based on the ISF input
  if      (chCol < 0.5) col4 = vec4(0.04 * tCur + 0.06 * cn, 0.8, 1., 0.4); // Original animated
  else if (chCol < 1.5) col4 = vec4(0.03 * cr, mix(0.15, 0.05, mod(cn, 2.)), 1., 0.3); // Original
  else if (chCol < 2.5) col4 = vec4(0.1 * cn, 0.8, 1., 0.2); // Original
  else if (chCol < 3.5) col4 = vec4(0.3 + 0.2 * cn, 0.7, 1., 0.3); // Original
  else if (chCol < 4.5) col4 = vec4(0.13 + 0.004 * cn, 0.8, 1., 0.4); // Original
  else if (chCol < 5.5) col4 = vec4(0.05 + 0.05 * cn, 0.7 + 0.3 * sin(tCur), 1., 0.5); // New animated
  else if (chCol < 6.5) col4 = vec4(0.01 * cr + sin(tCur*0.5)*0.2, 0.9, 0.8, 0.6); // New animated with cr
  else                  col4 = vec4(0.2 + 0.008 * cn, 0.6, 1., 0.3 + 0.2*cos(tCur)); // New animated

  col4.r = mod(col4.r, 1.);
  col4.rgb = HsvToRgb(col4.rgb);
  return col4;
}

// Raymarching function
float ObjRay (vec3 ro, vec3 rd)
{
  float dHit, h, s, sLo, sHi, eps;
  eps = 0.0005;
  s = 0.;
  sLo = 0.;
  dHit = dstFar;
  for (int j = 0; j < 220; j ++) {
    h = ObjDf (ro + s * rd);
    if (h < eps || s > dstFar) {
      sHi = s;
      break;
    }
    sLo = s;
    s += h;
  }
  if (h < eps) {
    for (int j = 0; j < 6; j ++) {
      s = 0.5 * (sLo + sHi);
      if (ObjDf(ro + s * rd) > eps) sLo = s;
      else sHi = s;
    }
    dHit = sHi;
  }
  return dHit;
}

// Function to compute normal vector
vec3 ObjNf (vec3 p)
{
  vec4 v;
  vec2 e = vec2 (0.0001, -0.0001);
  v = vec4(ObjDf (p + e.xxx), ObjDf (p + e.xyy), ObjDf (p + e.yxy), ObjDf (p + e.yyx));
  return normalize (vec3 (v.x - v.y - v.z - v.w) + 2. * v.yzw);
}

// Soft shadow calculation
float ObjSShadow (vec3 ro, vec3 rd, float dMax)
{
  float sh, d, h;
  sh = 1.;
  d = 0.02;
  for (int j = 0; j < 30; j ++) {
    h = ObjDf (ro + rd * d);
    sh = min(sh, smoothstep (0., 0.05 * d, h));
    d += clamp(3. * h, 0.04, 0.3);
    if (sh < 0.05 || d > dMax) break;
  }
  return 0.6 + 0.4 * sh;
}

// Ambient occlusion calculation
float ObjAO (vec3 ro, vec3 rd)
{
  float ao, d;
  ao = 0.;
  for (float j = 1.; j < 4.; j ++) {
    d = 0.02 * j;
    ao += max(0., d - ObjDf(ro + d * rd));
  }
  return 0.5 + 0.5 * clamp(1. - 5. * ao, 0., 1.);
}

// Main rendering function
vec3 ShowScene (vec3 ro, vec3 rd)
{
  vec4 col4;
  vec3 col, vn, ltDir;
  vec2 q;
  float dstObj, atten, dfSum, spSum, sh;
  dstObj = ObjRay (ro, rd);
  if (dstObj < dstFar) {
    ro += dstObj * rd;
    vn = ObjNf (ro);
    dfSum = 0.;
    spSum = 0.;
    for (int k = 0; k < 2; k ++) {
      ltDir = ltPos[k] - ro;
      atten = 1. / (1. + 0.02 * dot(ltDir, ltDir));
      ltDir = normalize (ltDir);
      atten *= smoothstep (0.3, 0.5, dot(ltAx, - ltDir));
      dfSum += atten * max(dot(vn, ltDir), 0.);
      spSum += atten * pow(max(0., dot(ltDir, reflect(rd, vn))), 16.);
    }
    ltDir = normalize(0.5 * (ltPos[0] + ltPos[1]) - ro);
    sh = ObjSShadow (ro, ltDir, max(dstObj - 0.2, 0.));
    col4 = ObjCol(ro, color_scheme_select, TIME);
    col = (0.2 + 0.4 * sh * dfSum) * col4.rgb + 0.5 * col4.a * sh * spSum * vec3 (1., 1., 0.5);
    col *= ObjAO(ro, vn);
    col += vec3 (0.2, 0.2, 0.) * max(dot(-rd, vn), 0.) *
            (1. - smoothstep(0., 0.03, abs (dstObj - 0.5 * mod (TIME, 6.))));
    rd = reflect(rd, vn);
    q = smoothstep(0.15, 0.25, mod(512. * vec2(atan(rd.x, rd.y), asin(rd.z)) / pi, 1.));
    col *= 0.95 + 0.05 * q.x * q.y;
    col *= mix(1., smoothstep(0., 1., Maxv3(col)), 0.3);
  } else {
    // Fixed background color
    col = vec3(0.15, 0.14, 0.14);
  }
  return col;
}

// ISF main entry point
void main()
{
  vec2 uv = 2. * gl_FragCoord.xy / RENDERSIZE.xy - 1.;
  uv.x *= RENDERSIZE.x / RENDERSIZE.y;

  // Map ISF inputs to internal variables
  float zmFac = 1.2 + (13.2 - 1.2) * zoom_factor;
  vuHt = 6. * vertical_position;
  mScale = 2.5 + (3.5 - 2.5) * fractal_scale;
  float vuTr = 2. - 4. * forward_speed;

  vec3 ro = vec3(0., vuHt, vuTr);

  // Apply shake effect
  vec2 shake_offset = vec2(sin(TIME * 10.0), cos(TIME * 12.0)) * shake_intensity;
  ro.xy += shake_offset;

  // Use ISF inputs for camera angles
  float el = camera_elevation;
  float az = camera_azimuth;

  // Add optional camera pan animation
  az += TIME * camera_pan_speed;

  // Add optional camera elevation animation
  el += sin(TIME * camera_elevation_speed) * 0.5;

  vec2 ori = vec2(el, az);
  vec2 ca = cos (ori);
  vec2 sa = sin (ori);
  mat3 vuMat = mat3 (ca.y, 0., - sa.y, 0., 1., 0., sa.y, 0., ca.y) *
                mat3 (1., 0., 0., 0., ca.x, - sa.x, 0., sa.x, ca.x);

  ltPos[0] = ro + vuMat * vec3 (-0.3, 0.2, -0.05);
  ltPos[1] = ro + vuMat * vec3 (0.3, 0.2, -0.05);
  ltAx = vuMat * vec3 (0., 0., 1.);
  dstFar = 80.;

#if ! AA
  const float naa = 1.;
#else
  const float naa = 4.;
#endif

  vec3 col = vec3 (0.);
  for (float a = 0.; a < naa; a ++) {
    vec2 offset = step(1.5, naa) * Rot2D(vec2(0.71 / RENDERSIZE.y, 0.), 0.5 * pi * (a + 0.5));
    vec3 rd = normalize (vec3 (uv + offset, zmFac));
    rd = vuMat * rd;
    col += (1. / naa) * ShowScene (ro, rd);
  }

  // Post-processing: Brightness, Contrast, Saturation
  col *= brightness;
  col = mix(vec3(0.5), col, contrast);
  vec3 hsv = RgbToHsv(col);
  hsv.y *= saturation;
  col = HsvToRgb(hsv);

  gl_FragColor = vec4 (pow(clamp(col, 0., 1.), vec3(0.9)), 1.);
}


// Utility functions from the original shader
vec3 HsvToRgb (vec3 c)
{
  return c.z * mix (vec3(1.), clamp (abs(fract(c.xxx + vec3(1., 2./3., 1./3.)) * 6. - 3.) - 1., 0., 1.), c.y);
}

vec3 RgbToHsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.z, c.y, K.x, K.y), vec4(c.y, c.z, K.x, K.z), step(c.z, c.y));
    vec4 q = mix(vec4(p.x, p.y, p.w, c.x), vec4(c.x, p.y, p.z, p.x), step(p.x, c.x));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float Maxv3 (vec3 p)
{
  return max (p.x, max(p.y, p.z));
}

vec2 Rot2D (vec2 q, float a)
{
  vec2 cs;
  cs = sin (a + vec2 (0.5 * pi, 0.));
  return vec2(dot(q, vec2(cs.x, - cs.y)), dot(q.yx, cs));
}
