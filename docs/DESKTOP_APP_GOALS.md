# Desktop App Goals

This document defines comprehensive goals for the desktop application and a phased roadmap to deliver them. The web/WASM edition is external-only and tracked in NUWE.

## Cross-References (Authoritative Sources)
- `DEVELOPMENT_ROADMAP.md` — milestones and phased delivery plan
- `FEATURE_DEVELOPMENT_ROADMAP.md` — feature-level breakdowns and completeness
- `FRONTEND_DEVELOPMENT_GUIDE.md` — UI architecture, components, and workflow
- `UI_UX_DESIGN_GUIDE.md` — design system, workspaces, HUD, interactions
- `RENDERING_DEFAULTS.md` — renderer defaults and recommended settings
- `REALTIME_3D_ENGINE_PLAN.md` — rendering architecture and pipelines
- `TESTING_QUALITY_PLAN.md` — quality gates, snapshot tests, coverage goals
- `DOCS_MAINTENANCE.md` — docs gate, update cadence, verification protocol
- `VR_XR_INTEGRATION_PLAN.md` — XR targets and interaction foundation
- `GESTURE_INPUT_PLAN.md` — gesture/MIDI/OSC input integration

The sections below summarize and consolidate goals from these documents so the desktop roadmap remains consistent without duplicating the authoritative details.

## WIP Features (Implemented/Scaffolded)
- Shader Viewer & Editor in UI with load/save and live preview.
  - Supports WGSL files and ISF shaders with on-load conversion.
  - Fallback GLSL→WGSL path when advanced ISF conversion fails.
- ISF → WGSL Converter (batch and inline) for shader ingestion.
- Shader Renderer utilities for previewing WGSL fragment shaders.
- 3D Modeling & Scene scaffolding: scene objects and mesh export.
- Fractal Formula Library and Parameter Builder for 2D/3D sets.
- Asset bundle of shaders (2D, 3D, pseudo‑3D) for testing and demos.

### Key Source References (code & examples)
- Shader viewer/editor: `src/ui/main.rs` (Shader Loader panel)
- ISF→WGSL conversion (advanced): `src/shader_converter.rs`
- ISF→WGSL/HLSL conversion (inline): `src/lib.rs::ShaderConverter`
- Shader renderer helpers: `src/shader_renderer.rs`
- ISF shader assets: `assets/shaders/isf/`
- Example loading ISF shaders: `examples/load_isf_shaders.rs`
- Fractal formulas and engine: `src/fractal/formulas.rs`, `src/fractal/engine.rs`
- Parameter builder and types: `src/fractal/param_builder.rs`, `src/fractal/types.rs`
- 3D scene objects and modeling scaffolding: `src/scene/objects.rs`, `src/scene/mod.rs`
- Mesh export (OBJ/PLY): `src/export/mesh.rs` (sample: `exports/mesh.obj`)
- PBR/lighting scaffolding: `src/pbr/mod.rs`
- Renderer pipelines: `src/fractal/renderer.rs`, `assets/shaders/*`

## Rendering Engine
- Deliver real-time GPU fractal rendering with stable 60+ FPS.
- Support 2D and 3D fractal formulas with unified parameter model.
- Implement high-quality anti-aliasing (FXAA/TAA) and optional post-processing.
- Provide pseudo-3D and true 3D raymarch pipelines with shared uniforms.
- Add physically based rendering hooks for lighting experiments (optional).

### UI/UX Highlights (from `UI_UX_DESIGN_GUIDE.md` and `FRONTEND_DEVELOPMENT_GUIDE.md`)
- Glassmorphism panels, dark theme, technical typography; responsive workspace layouts.
- Viewport HUD: FPS, resolution, camera info, performance metrics.
- Pie menus, context-aware panels, keyboard shortcuts, drag-and-drop interactions.
- Camera controls: FOV slider and target XYZ; updates pseudo‑3D direction.

## UI/UX & Workspaces
- Clean, responsive GUI using `bevy_egui` with clear panels.
- Workspaces: Fractal Edit, Node Editor, Render Preview, Export.
- Per-workspace toggles: FXAA, pseudo-3D, tone mapping, exposure.
- Preset management: save/load formula presets and parameter sets.
  - See `docs/PRESETS.md` for JSON/RON save/load and apply-to-renderer.
- Command palette for quick actions and keyboard shortcuts.

### Shader Tools (Viewer & Conversion)
- Load/edit WGSL shaders directly in the Shader Loader workspace.
- Import ISF (`.fs`) shaders; convert to WGSL on load.
- If advanced conversion fails, apply GLSL→WGSL fallback to keep workflow moving.
- Save converted WGSL to disk; mark unsaved changes in the editor.

### Workspace Details (see `UI_UX_DESIGN_GUIDE.md`)
- Modeling: scene hierarchy + fractal library; parameter inspector.
- Animation: timeline editor; viewport with animation preview; curve editing.
- Node Editing: node library, graph canvas with mini-previews; properties panel.
- Rendering: render settings + asset browser; performance monitor; experimental pseudo‑3D toggle.

## Node Editor & Procedural Graph
- Node graph for composing fractal formulas and modifiers.
- Live preview nodes and inline parameter editing.
- Import/export node graphs to file for sharing.
- Basic undo/redo and selection tools.

### Implementation Notes (see `FRONTEND_DEVELOPMENT_GUIDE.md`)
- Visual nodes, drag-to-connect, context menus, zoom/pan; color-coded wires.
- Mini-previews on nodes; centralized state with undo/redo history.

## Performance & Stability
- GPU pipeline profiling and frame-time logging.
- Async resource loading; avoid blocking UI.
- Graceful fallback on non-supported features/hardware.
- Deterministic exports with pinned engine state.

## Export & Integration
- Image exports (PNG/JPEG/EXR) and image sequences.
- Video export path via frames + external encoder guidance.
- Mesh/voxel exports for 3D fractals (OBJ/PLY, VDB optional).
- Metadata embedding for presets and engine settings.

### References
- See `export/` modules and `FEATURE_DEVELOPMENT_ROADMAP.md` export milestones.
  - Mesh export entry points: `src/export/mesh.rs`; sample output: `exports/mesh.obj`.

## Input & Control
- Gesture/mouse controls for camera and parameter scrubbing.
- Optional MIDI/OSC control for live parameter modulation.
- Optional audio-reactive modifiers (FFT-based) for experiments.

### References
- `GESTURE_INPUT_PLAN.md` for backends and sensitivity/inversion options.

## Quality, Testing, and Tooling
- Targeted unit tests for formula math and param builders.
- Snapshot tests for visual regressions of key presets.
- Editor-only diagnostics workspace and GPU capability report.
- CI docs gate to keep developer guidance up-to-date.

### References
- `TESTING_QUALITY_PLAN.md` for acceptance criteria; `DOCS_MAINTENANCE.md` for docs gate.

## Roadmap (Phased)

### Phase 1 — Desktop-Only Hardening (now)
- Remove web/WASM deps and scripts; archive in `archive/blockchain_nft_web`.
- Enforce desktop build defaults; strip cdylib and web targets.
- Clean startup banners and docs to reflect desktop-only scope.

### Phase 2 — Core Rendering & UI
- Stabilize 2D/3D fractal pipelines; unify param model.
- Implement FXAA toggle and pseudo-3D toggle per workspace.
- Build Fractal Edit and Render Preview workspaces.

### Phase 3 — Node Editor & Presets
- Introduce node graph editing with live previews.
- Save/load presets and parameter sets with metadata.

### Phase 4 — Export Suite
- Reliable image/image-sequence export and mesh/voxel export.
- Add encoder integration guidance; keep core deterministic.

### Phase 5 — Performance & Diagnostics
- Profiling tools, GPU capability report, and snapshot tests.
- Optimize pipelines and resource lifecycles.

### Phase 6 — Optional Integrations
- MIDI/OSC controls and audio-reactive modifiers (optional).
- PBR lighting experiments for 3D fractal surfaces (optional).

### Alignment
- Cross-check milestones with `DEVELOPMENT_ROADMAP.md` and `FEATURE_DEVELOPMENT_ROADMAP.md`.

## Immediate Next Tasks
- Verify desktop manifest (done) and prune residual web references.
- Implement workspace-level toggles wiring for FXAA and pseudo-3D.
- Draft preset schema and export format.
  - Reference `RENDERING_DEFAULTS.md` and `REALTIME_3D_ENGINE_PLAN.md` for defaults and data flow.