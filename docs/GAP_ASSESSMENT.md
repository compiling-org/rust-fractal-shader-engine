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
 - Post-process tone mapping & vignette: ⚠️ Partial
   - Ref: `src/fractal/shaders/fractal_render.wgsl`

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
- Export formats (PNG/JPEG, sequences, MP4, OBJ/STL/PLY, VOX): ⚠️ Partial
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
- Out-of-scope in this repository; see `docs/PLATFORM_SPLIT.md` for web locations.

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
# Gap Assessment

This document tracks the gaps between current implementation and requested capabilities.

## Status Update — 2025-11-11

- Advanced 3D features: no lighting, no physically based materials, no shadowing.
- Camera system: lacks multi-camera rigs, depth-of-field, cinematic controls.
- Fractal formulas: limited; comprehensive catalog missing (e.g., mandelbulb variants, hybrids).
- Shaders: minimal WGSL; advanced shading, SDF composition, and ray-march optimizations missing.
- Views: Rendering and Modeling share similar viewport; Modeling should become Render View; a dedicated Modeling View for 3D creation/export controls is not yet implemented.
- Export system: OBJ supported; basic ASCII STL and PLY exports added.
- Mesh generation: marching cubes/normal generation still basic; reliability and quality need work.

## Recent Progress

- Fixed `MeshFormat` equality for UI radio controls by deriving `PartialEq`.
- Corrected mesh generation indexing (`usize` vs `u32`) to resolve compile errors.
- Implemented basic ASCII STL exporter (triangulated facets with computed face normals).
 - Implemented basic ASCII PLY exporter (vertices, optional normals/UVs, triangle faces).
 - Shader post-process: fixed vignette application; added minimal Reinhard tone mapping in color grading.

## Next Steps (Short Term)

- Separate Modeling vs Rendering views: wire dedicated modeling controls for mesh generation and export.
- Add lighting controls to render pipeline (directional light, ambient term).
- Expand camera controls: focal length, aperture, focus distance, motion paths.
- Catalog fractal formulas and shader modules; scaffold loaders and presets.
- Improve mesh generation quality: robust normals, manifold handling, and decimation options.

## Risks / Blockers

- Missing shader architecture for modular fractal formula injection.
- Lack of render feature toggles (lighting, tone mapping) hampers visual parity with target tools.
- Export reliability dependent on mesh generation correctness and post-processing.