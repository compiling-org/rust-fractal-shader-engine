# Modular Fractal Shader — Development Plan (Living Document)

Purpose: Provide a comprehensive, evolving plan to deliver a stable, performant, and feature-complete fractal authoring environment across desktop and web.

## Vision & Scope
- Create a node-powered fractal authoring tool supporting 3D fractals, shaders, and procedural art.
- Deliver crisp 3D rendering with robust controls, lighting, and shading.
- Support animation timelines, presets, and high-quality exports for artists and researchers.
- Integrate ISF/GLSL→WGSL conversion, OSC/Leap/MediaPipe/MIDI inputs, and a WebGPU build.
- See: `REALTIME_3D_ENGINE_PLAN.md` for the real-time rendering loop, WGSL interface, camera, lighting, and quality scaling specifics.
 - See: `ADVANCED_SUITE_REQUIREMENTS.md` for end-to-end module requirements and acceptance criteria.
 - See: `GAP_ASSESSMENT.md` for a living checklist of implemented vs missing features.

## Goals Structure
- Each major task lists multiple goals to ensure breadth and depth.
- Goals are measurable and traceable via status fields: Planned, In Progress, Blocked, Done.
- Every release updates this plan; link PRs/issues to goal IDs.

## Phases & Tasks (A–O)

### Task A — Core Runtime & Windowing
- Ensure Bevy/WGPU app initialization opens window reliably across platforms.
- Graceful shutdown and resource cleanup; fix exit/close lifecycle.
- Structured logging for renderer, input, and UI subsystems.
- Configurable window options: size, vsync, MSAA, HDR if supported.
- Headless/offscreen render mode for testing and batch export.
- Deterministic startup seed for reproducible rendering.
- Health checks: GPU compatibility, feature flags.
 - Enforce GPU-only startup: hard fail (panic) if `RenderDevice` is not available after a short window.
 - Reference: Real-time renderer responsibilities and error handling in `REALTIME_3D_ENGINE_PLAN.md`.

### Task B — Fractal Renderer Correctness
Linkage: This task’s acceptance criteria align with the WGSL parameter packing and ray marching practices defined in `REALTIME_3D_ENGINE_PLAN.md`.
- Implement robust ray marching (sphere tracing) with safe step logic.
- Distance estimators: Mandelbulb, Mandelbox, Julia, custom formulas.
- Normal estimation via finite differences; eliminate artifacts.
- Phong/Blinn shading; adjustable specular power and materials.
- Ambient occlusion and soft shadow ray-marching options.
- Color ramps and tone mapping; gamma/exposure controls.
- Parameter buffer mapping audited and aligned with WGSL indices.

### Task C — Camera & Controls
- Orbit/FPS camera modes with smooth damping.
- FOV, near/far planes, dolly/pan/roll.
- Keyboard mappings (WASD), mouse drag, touch gestures.
- Save/restore cameras; named camera presets.
- Auto-focus on structure via DE probing; reset framing.
- Camera collision avoidance with DE checks.
- Optional look-at target and spline camera paths.

### Task D — Lighting & Shading
- Directional light with intensity, color, soft shadows.
- Ambient light and environment shading presets.
- Material params: diffuse/specular/metallic/roughness-like controls.
- Rim lighting and stylized modifiers.
- AO samples and quality presets; performance-aware toggles.
- Backface shading configuration; prevent dark flicker.
- Light gizmo visualization in editor space.

### Task E — Formula Library & Presets
- Curated formula database (Mandelbulb/box/julia/etc.) with metadata.
- Parameter ranges and defaults tuned for good outputs.
- Artist presets for color ramps, lighting, camera setups.
- Import/export of presets (JSON/TOML).
- Tagging, search, favorites; quick-load in UI.
- Versioned formulas; migration strategy.
- Community formula pack loader (optional).

### Task F — Node Editor
- Node graph for formulas, DE ops, color maps, post-process.
- Connectors with type checking; live parameter updates.
- Group nodes, comments, frames, subgraphs.
- Undo/redo, copy/paste, snap and align.
- Serialization of graphs; import/export networks.
- Execution order and caching; profiling.
- Quick search palette and shortcuts.

### Task G — UI/UX Workspaces & Panels
- Panels for Camera, Lighting, Formula, Render, Color, Output.
- Workspaces (Beginner/Advanced/Animation) with tailored layouts.
- Consistent typography, spacing, and color from design guide.
- Shortcuts, tooltips, contextual help.
- Touch support and accessibility.
- Settings persistence and reset-to-default.
- Performance indicator and frame time display.

### Task H — Animation & Timeline
- Timeline with keyframes for camera, light, formula params.
- Easing functions, curves editor, snapping and markers.
- Preview playback with real-time rendering.
- Baking to cache for export; frame skipping controls.
- Import/export of animation clips.
- Parameter drivers (noise, OSC, MIDI, timecode).
- Scripting hooks (optional).

### Task I — Export Pipelines
- Image sequences (png/exr), video (mp4, prores where feasible).
- Mesh/point cloud/voxel export for DE-based surface capture.
- Render presets for resolution/quality/performance.
- Batch export with progress and cancellation.
- Naming templates; output folder management.
- Color management (gamma), optional LUT/post-process.
- CLI export mode for headless rendering.

### Task J — Web Build & Delivery
- WebGPU build; Canvas2D fallback for limited platforms.
- Reduced UI for web with essential panels.
- Performance mode defaults for mobile.
- WASM bindings for shader parameter updates.
- Asset loading strategy for web.
- Web demo page and minimal deploy pipeline.
- Cross-browser compatibility testing.

### Task K — Shader Converter (ISF/GLSL→WGSL)
- Parse ISF metadata; map uniforms and UI params.
- GLSL→SPIR-V and SPIR-V→WGSL toolchain (Tint/Naga).
- Validation and texture/sampler resolution.
- Fragment-path wrapper for quick adoption; compute-path port.
- Parameter mapping to UI panels.
- Import/export and preset integration.
- Test suite with known ISF shaders.

### Task L — Performance & Quality Modes
- Profile compute vs fragment path; tune step budgets and epsilons.
- Quality presets (Low/Medium/High/Ultra).
- Adaptive step size and early exit; bounding volumes.
- Multi-resolution rendering or checkerboarding (optional).
- GPU feature detection; toggle advanced features.
- Frame time target controls; dynamic scaling.
- Benchmarks and performance dashboards.

### Task M — Testing & Snapshot Baselines
- Offscreen render targets for deterministic snapshots.
- Baseline PNGs; MSE and edge-density heuristics.
- Parameter-sweep tests for convergence and stability.
- UI action smoke tests.
- CI integration for shader validation and renders.
- Cross-platform tests.
- Logging assertions and error detection.

### Task N — Integrations (OSC, Leap Motion, MediaPipe, MIDI)
- Stable input processing and threading; debounce and rate limits.
- Map gestures/OSC/MIDI to parameters and camera controls.
- Calibration flows and presets for devices.
- Recording input streams for animation use.
- Safety fallbacks when device unavailable.
- Visualization of input state in UI.
- Example profiles for live performance setups.

### Task O — Documentation & Onboarding
- Quickstart guide and workspace tour.
- Deep dives: formulas, node editor, animation, export.
- Troubleshooting and performance tips.
- Developer docs: architecture, renderer, converter pipeline.
- Example projects and presets included.
- Contribution guide and code style.
- Changelog and roadmap updates.

## Milestones & Indicative Timeline
- Weeks 1–3: A–C (Stability, Renderer correctness, Camera).
- Weeks 4–6: D–E (Lighting, Formula library, Presets).
- Weeks 7–9: F–G (Node editor, UI workspaces).
- Weeks 10–12: H–I (Animation, Export).
- Weeks 13–14: J (Web build).
- Weeks 15–16: K (Shader converter).
- Ongoing: L–M–N–O (Performance, Testing, Integrations, Docs).

## Dependencies
- `wgpu`, Bevy, WGSL shaders, Tint/Naga for conversion.
- Platform codecs/libraries for video export.
- WebGPU availability for browser targets.

## Risks & Mitigations
- Shader conversion corner cases → validate with Tint/Naga; fragment-path fallback.
- Performance variability across GPUs → quality modes and auto-scaling.
- UI/Node Editor complexity → stage features and maintain design consistency.
- Export codec availability → start with image-sequence and MP4 baseline.

## Update Cadence & Roles
- Update this plan every sprint (weekly/bi-weekly) and at releases.
- Use status tags: Planned, In Progress, Blocked, Done.
- Link issues/PRs to goal IDs; include date and owner for changes.
- Keep cross-links to other docs in `/docs` and `CHANGES.md`.
## VR/XR & Gesture Plans
- See `VR_XR_INTEGRATION_PLAN.md` for OpenXR session/render loop, stereoscopic modes, and 180°/360° projections.
- See `GESTURE_INPUT_PLAN.md` for MediaPipe, Leap Motion, and OpenXR hand tracking approaches and unified gesture schema.