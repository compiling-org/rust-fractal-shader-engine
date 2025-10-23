/*{
  "CATEGORIES": ["Fractal", "Psychedelic", "Abstract"],
  "DESCRIPTION": "Enhanced orbit trap fractal with full control over color, pulse, morph, and geometry.",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Animation Speed" },
    { "NAME": "Pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Color Pulse Speed" },
    { "NAME": "FractalScale", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5, "LABEL": "Fractal Scale" },
    { "NAME": "MorphAmount", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 1.0, "LABEL": "Morph Amount" },
    { "NAME": "PaletteMode", "TYPE": "float", "DEFAULT": 0, "MIN": 0, "MAX": 2, "LABEL": "Color Palette", "VALUES": ["Classic", "Acid", "Chill"] },
    { "NAME": "RedShiftSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Red Pulse Speed" },
    { "NAME": "BlueShiftSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0, "LABEL": "Blue Pulse Speed" },
    { "NAME": "GeometryDeform", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Geometry Deform Scale" },
    { "NAME": "TrapBias", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0, "LABEL": "Orbit Trap Bias" }
  ]
}*/

#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define TAU 6.28318

vec3 palette(float t, int mode) {
  if (mode == 1) return vec3(0.5 + 0.5 * sin(TAU * t + vec3(0.0, 0.5, 1.0))); // Acid
  if (mode == 2) return vec3(0.4 + 0.6 * cos(TAU * t + vec3(0.2, 0.4, 0.6))); // Chill
  return vec3(0.6 + 0.4 * sin(TAU * t + vec3(0.1, 0.3, 0.5)));                // Classic
}

vec3 render(vec2 p, float time, float scale, float morph, float pulseSpeed, int mode, float redShift, float blueShift, float deform, float trapBias) {
  p *= rot(time * 0.1) * scale;
  p.y -= 0.2266;
  p.x += 0.2082;

  vec2 ot = vec2(100.0);
  float m = 100.0;

  for (int i = 0; i < 150; i++) {
    vec2 cp = vec2(p.x, -p.y);
    float denom = dot(p, p);
    p = p + cp / denom - vec2(0.0, 0.25 + morph);
    p *= 0.1 + morph * deform;
    p *= rot(1.5 + morph * 0.5);
    ot = min(ot, abs(p) + trapBias * fract(max(abs(p.x), abs(p.y)) * 0.25 + time * 0.1 + float(i) * 0.15));
    m = min(m, abs(p.y));
  }

  ot = exp(-200.0 * ot) * 2.0;
  m = exp(-200.0 * m);

  float pulse = sin(time * pulseSpeed + length(p)) * 0.5 + 0.5;
  vec3 palColor = palette(pulse, mode);

  // Color shifting using sine time offsets for red/blue
  palColor.r *= sin(time * redShift + length(p)) * 0.5 + 0.5;
  palColor.b *= cos(time * blueShift + length(p)) * 0.5 + 0.5;

  vec3 base = vec3(ot.x, ot.y * 0.5 + ot.x * 0.3, ot.y) + m * 0.2;
  return base * palColor;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
  vec2 d = vec2(0.0, 0.5) / RENDERSIZE.xy;

  float t = TIME * Speed;

  vec3 col = render(uv, t, FractalScale, MorphAmount, Pulse, int(PaletteMode), RedShiftSpeed, BlueShiftSpeed, GeometryDeform, TrapBias);
  col += render(uv + d.xy, t, FractalScale, MorphAmount, Pulse, int(PaletteMode), RedShiftSpeed, BlueShiftSpeed, GeometryDeform, TrapBias);
  col += render(uv - d.xy, t, FractalScale, MorphAmount, Pulse, int(PaletteMode), RedShiftSpeed, BlueShiftSpeed, GeometryDeform, TrapBias);
  col += render(uv + d.yx, t, FractalScale, MorphAmount, Pulse, int(PaletteMode), RedShiftSpeed, BlueShiftSpeed, GeometryDeform, TrapBias);
  col += render(uv - d.yx, t, FractalScale, MorphAmount, Pulse, int(PaletteMode), RedShiftSpeed, BlueShiftSpeed, GeometryDeform, TrapBias);

  gl_FragColor = vec4(col * 0.2, 1.0);
}
