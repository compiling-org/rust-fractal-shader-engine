# Real-Time 3D Engine Plan (WGSL/WGPU)

Purpose: Align implementation with the project’s advanced, real-time 3D fractal rendering goals. This document defines the runtime loop, renderer responsibilities, shader interface, camera/navigation, lighting/material model, quality scaling, performance targets, and validation.

## Scope & Principles
- Real-time first: maintain consistent frame times while rendering complex 3D fractals.
- GPU-centric: compute shaders (WGSL) drive ray marching, normals, lighting, and materials.
- Modular: renderer and UI are decoupled; parameters flow via a well-defined interface.
- Deterministic: parameters and seeds produce reproducible results for export.
- Measurable: every feature has performance and quality targets.

## Runtime Architecture
- App loop: Bevy-based update runs at vsync or target FPS. Each frame:
  - Gather input (camera, gestures, OSC/MIDI/audio-reactive signals).
  - Update `FractalParameters` (time, camera, formula, quality).
  - Push parameters to GPU buffers; dispatch compute workgroups.
  - Present output texture in viewport; update overlays/HUD.
  - If `Fragment Pseudo‑3D Mode` enabled, render via fragment pipeline instead of compute.
- Headless mode: offscreen rendering for snapshots and animation exports uses same pipeline.

## Renderer Responsibilities
- Initialize WGPU resources: device, queue, pipelines, bind groups, sampler.
- Maintain double-buffered textures for viewport to minimize stalls.
- Parameter packing: stable layout mapping Rust -> WGSL indices.
- Dispatch strategy: compute workgroup sizing derived from resolution; use tiles if needed.
- Readback policy: avoid per-frame readback in realtime; only map for exports/snapshots.
- Error handling: robust fallbacks for buffer mapping and shader failures.

## WGSL Shader Interface
- Params buffer: packed floats matching indices used in `fractal_compute.wgsl`.
  - Examples: `param(26u)` time, `param(32u)` formula_id, camera position/rotation, scale, bailout, iteration caps, normal epsilon, and color controls.
- Output: RGBA8 texture bound as storage in compute pipeline; viewport samples directly.
- Lighting: directional + ambient baseline; extendable to soft shadows and reflections.
- Materials: base color + roughness/metallic placeholders; future PBR via textures.

### Fragment Pipeline (Pseudo‑3D)
- Uniform struct: `CommonUniforms` bound to a uniform buffer for fragment shader.
  - Fields: `resolution`, `time`, `camera_pos`, `camera_dir`, `fov`, `mandelbox_scale`,
    `mandelbox_folding_limit`, `bailout`, `max_iterations`.
- Shaders: `pseudo3d_raymarch.wgsl` with `vs_main` and `fs_main` entries.
- Mode switch: UI toggles fragment mode; renderer updates the uniform buffer each frame.
- Use cases: rapid iteration and decorative pseudo‑3D visuals; compute path preferred for multi‑stage pipelines.

## Camera & Navigation
- Controls: orbit, pan, dolly with inertia; keyboard/mouse/gamepad bindings.
- Camera model: position + rotation (quaternion or Euler) packed to params.
- Gesture integration: `gesture.rs` feeds smooth deltas; respects UI focus.
- Framing presets: quick-save/load camera states; deterministic animation paths.

## Lighting & Materials
- Baseline: directional light with intensity, color, and ambient term.
- Normals: estimate via gradient of distance field (epsilon tunable).
- Extensions (staged):
  - Soft shadows using ray marching occlusion.
  - Specular highlights (Blinn-Phong) and simple reflections.
  - PBR placeholders: albedo, metallic, roughness with texture transform.

## Quality Scaling
- Adaptive resolution: scale viewport resolution at runtime to hit target frame time.
- Iteration budget: adjust max steps and epsilon based on performance.
- Anti-aliasing: temporal AA option leveraging motion vectors (future).
- Presets: Draft/Preview/High/Ultra with explicit parameter caps.

## Performance Targets
- Desktop (modern GPU): 60 FPS at 1280x720 for Mandelbulb/Mandelbox/Quaternion Julia.
- High-end GPUs: 60 FPS at 1920x1080 with soft shadows enabled.
- Headless export: >2x real-time throughput for image sequences.

## Testing & Validation
- Smoke tests: launch GUI, switch fractal formulas, verify stable FPS and responsiveness.
- Snapshot parity: rendered frame equals headless snapshot with same params.
- Metrics overlay: display frame time, iterations, epsilon, workgroup count.
- Crash guards: shader compile errors and buffer mapping failures show UI diagnostics.

## Integration Points
- UI Panels: parameters, materials, lighting, quality presets.
- Node Editor: parameters driven by node graph; renderer subscribes to outputs.
- Animation: keyframe timeline updates `FractalParameters` deterministically.
- Audio/OSC: modulation pipeline applies bounded changes to avoid hitches.

## Milestones & Deliverables
- M1: Stable 60 FPS at 720p across three 3D formulas; metrics overlay.
- M2: Soft shadows and specular highlights with quality presets.
- M3: Node-driven parameter modulation; headless export parity.
- M4: PBR placeholders and basic reflection; temporal AA prototype.

## References
- `src/fractal/renderer.rs` — parameter packing, compute dispatch, viewport updates.
- `src/fractal/shaders/fractal_compute.wgsl` — ray marching, normals, lighting.
- `src/ui/main.rs` and `src/gui.rs` — app loop, time progression, control flow.