# Feature Gap Assessment — Real-time Fractal Suite

Purpose: Maintain a living checklist of implemented vs missing features with code references and acceptance criteria links.

## Status Legend
- ✅ Present (meets acceptance)
- ⚠️ Partial (needs work)
- ❌ Missing

## Checklist

### Renderer & Real-time
- Real-time viewport without CPU readback: ⚠️ Partial
  - Ref: `src/fractal/renderer.rs`, `docs/REALTIME_3D_ENGINE_PLAN.md`
- Stable 60 FPS @ 720p for 3D formulas: ⚠️ Partial
  - Ref: `src/benchmark.rs`, GUI metrics overlay (planned)
- Parameter packing documented & stable: ✅ Present
  - Ref: `src/fractal/renderer.rs`, `src/fractal/shaders/fractal_compute.wgsl`

### UI/UX
- Main menu and panels framework: ⚠️ Partial
  - Ref: `src/ui/*.rs`, `docs/UI_UX_DESIGN_GUIDE.md`
- Workspaces save/load: ⚠️ Partial
  - Ref: `src/ui/workspaces.rs`
- Metrics overlay: ❌ Missing
  - Ref: `docs/TESTING_QUALITY_PLAN.md` (to add)

### Node System
- Node editor canvas and connections: ⚠️ Partial
  - Ref: `src/node_editor.rs`, `src/ui/node_editor.rs`
- Core nodes library: ❌ Missing
  - Ref: `docs/FEATURE_DEVELOPMENT_ROADMAP.md`

### File I/O & Export
- Project save/load: ⚠️ Partial
  - Ref: `src/project/mod.rs`
- Export formats (PNG/JPEG, sequences, MP4, OBJ/STL, VOX): ⚠️ Partial
  - Ref: `src/export/*`
- Headless export parity: ⚠️ Partial
  - Ref: `src/bin/fractal_snapshot.rs`

### Formula Library (3D)
- Mandelbulb, Mandelbox, Quaternion Julia: ✅ Present
  - Ref: `src/fractal/engine.rs`, `src/fractal/formulas.rs`
- Extended formulas & hybrids: ⚠️ Partial
  - Ref: `docs/FEATURE_DEVELOPMENT_ROADMAP.md`

### Shader Conversion
- ISF/GLSL → WGSL converter baseline: ⚠️ Partial
  - Ref: `src/shader_converter.rs`, `assets/shaders/isf/`

### Controls & Modulation
- Camera controls & presets: ⚠️ Partial
  - Ref: `src/ui/main.rs`, `src/gesture.rs`
- Lighting/material controls: ⚠️ Partial
  - Ref: `src/pbr/mod.rs`
- Audio/OSC/MIDI modulation pipeline: ⚠️ Partial
  - Ref: `src/audio.rs`, `src/osc.rs`

### Integration (VFX, VJ)
- Export compatibility presets: ❌ Missing
  - Ref: `docs/DEVELOPMENT_ROADMAP.md`
- Live outputs (NDI/Syphon/Spout): ❌ Missing
  - Ref: planned modules
- VJ module API: ❌ Missing

### Web/WebGPU
- WASM/WebGPU demo build: ⚠️ Partial
  - Ref: `build_web.sh`, `src/web/*`, `web/index.html`

### Testing & Quality
- Metrics overlay & performance targets validated: ❌ Missing
- UI diagnostics for shader/renderer errors: ⚠️ Partial

## How to Use
- Update this file weekly with status changes.
- Link PRs/issues to items here and to `docs/ADVANCED_SUITE_REQUIREMENTS.md`.
- Treat ❌ Missing and ⚠️ Partial items as backlog targets in Roadmap.
## VR/XR & Stereo/180
- OpenXR session + per-eye render: Missing
- Desktop stereo preview (SBS/OU): Missing
- 180° dual-fisheye render path: Missing
- 360° equirectangular export: Missing

## Gesture Controls
- MediaPipe webcam hand tracking backend: Missing
- Leap Motion (`leaprs`) backend: Missing
- OpenXR hand tracking integration: Missing
- Unified gesture schema + modulation mapping: Missing