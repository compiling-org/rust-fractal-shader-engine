# Fractal Studio – Tracked Issues (Living Document)

This is the single source of truth for known problems in the app. Each issue includes repro steps, suspected cause, impact, and an implementation-ready fix plan with acceptance criteria.

Maintainers: update this file when discovering, fixing, or re-scoping issues. Follow the docs-first workflow: a PR must update this document when it changes issue state.

## How To Use
- Add a new issue entry when you find a reproducible problem.
- Keep statuses accurate: `open`, `in_progress`, `blocked`, `fixed`.
- Link to code (file paths) and reference menu/screens to make triage quick.
- Close an issue only after meeting acceptance criteria and verifying on Windows.

## Priorities
- P0: Blocks core workflows (startup, viewport render, save/load integrity)
- P1: Major feature gaps (render outputs, animation, shader selection)
- P2: Usability and DX (docs accuracy, deprecations, menu polish)
- P3: Performance and long-tail improvements

---

## UI-001 – Render menu actions are stubs
- Status: open
- Area: `src/ui/main.rs` (Render menu), app menus
- Description: Buttons like `Render Image`, `Render Animation`, `Render Settings...`, `Render Viewport`, `Preview`, `Production`, `Custom` log messages but do not execute rendering/export or change renderer state.
- Repro:
  1) Launch app with GUI.
  2) Open `Render` menu.
  3) Click any of the actions listed above.
  4) Observe no functional change beyond a log entry.
- Suspected cause: UI wiring is placeholder; renderer/export APIs are not connected.
- Impact: Users cannot render images/animations or switch quality modes from UI.
- Fix plan:
  - Wire `Preview`/`Production` to call `FractalEngine.apply_quality_preset(...)`.
  - Implement `Render Image` to render current viewport to PNG (temp path via `rfd::FileDialog`).
  - Implement `Render Animation` to drive timeline and export frames.
- Acceptance criteria:
  - Clicking `Preview`/`Production` visibly changes render resolution/iteration counts.
  - `Render Image` prompts for a location and writes a PNG of current viewport.
  - `Render Animation` prompts for an output folder and writes a sequence for the active timeline.

## UI-002 – Viewport rendering not reliably visible
- Status: open
- Area: `src/ui/main.rs::show_fractal_viewport`, `src/fractal/renderer.rs`
- Description: The viewport often shows a placeholder or fails to display a fractal frame; GPU renderer may not be initialized in time, or readback-to-egui path is fragile.
- Repro:
  1) Launch GUI and navigate to main workspace.
  2) Observe viewport shows “Renderer not initialized” or stays blank.
  3) Interact with menus; rendering does not recover.
- Suspected cause: WGPU context initialization timing and egui texture allocation/update flow.
- Impact: App appears non-functional; users cannot see fractal output.
- Fix plan:
  - Ensure `FractalRenderer::new_with_wgpu_context` is created before first `show_fractal_viewport` call.
  - Replace CPU readback for egui preview with direct texture sampling path (avoid full readback unless exporting).
  - Add robust error reporting in UI when renderer init fails.
- Acceptance criteria:
  - Viewport consistently shows fractal within 1 second of app start.
  - No blank frames after first render; recover gracefully on device errors.

## UI-003 – Quality presets not exposed in UI
- Status: open
- Area: `src/fractal/types.rs` (QualityPreset), `src/fractal/engine.rs` (apply_quality_preset), `src/ui/main.rs` (Render menu)
- Description: Preset system exists in core, but UI does not call it; quality buttons are inert.
- Repro:
  1) Open `Render` → `Preview` or `Production`.
  2) No visible change in renderer resolution/iterations.
- Suspected cause: Missing call from menu handlers to `apply_quality_preset`.
- Impact: Users cannot switch performance/quality modes.
- Fix plan: Wire `Preview`, `Production`, `Custom` to engine preset API and show current preset label.
- Acceptance criteria: Toggling preset updates resolution/iterations immediately in viewport and status bar.

## RND-001 – Asset shader folder not in use
- Status: open
- Area: `assets/shaders/*` vs `src/fractal/shaders/*`
- Description: The active renderer includes WGSL via `include_str!` from `src/fractal/shaders/` (compute + render). Shaders under `assets/shaders/` are not loaded, causing confusion.
- Repro:
  1) Inspect `src/fractal/renderer.rs` shader loading.
  2) Note `fractal_compute.wgsl` and `fractal_render.wgsl` are embedded; `assets/shaders/*` are unused.
- Suspected cause: Asset-based shader system not yet integrated.
- Impact: Contributors expect shader swapping via assets; changes there have no effect.
- Fix plan: Document shader pipeline clearly; optionally add loader to use asset WGSL files for experimentation.
- Acceptance criteria: README clarifies shader source of truth; optional loader works behind a feature flag.

## EXP-001 – Export pipelines not connected to UI
- Status: open
- Area: `src/export/video.rs`, `src/export/animation.rs`, `src/ui/main.rs` (Export, Render menus)
- Description: Quality enums and export scaffolding exist, but UI actions do not trigger exports.
- Repro:
  1) Click `Export` or `Render Animation`.
  2) No file output generated.
- Suspected cause: Missing wiring and implementation of export triggers.
- Impact: Users cannot export images/animations.
- Fix plan: Implement `Export → PNG Sequence` and `Render Animation` to drive timeline and write frames.
- Acceptance criteria: Export generates files at chosen location with correct dimensions/quality.

## DEV-001 – Deprecated egui usage warnings
- Status: open
- Area: `build_log.txt`, `src/ui/*`
- Description: Build log references deprecated `egui` methods; migration is pending.
- Repro: Build the project and inspect warnings.
- Suspected cause: API changes in `egui/bevy_egui` versions.
- Impact: Future breakage risk and noisy builds.
- Fix plan: Update UI code to current `egui` API and bump dependencies if safe.
- Acceptance criteria: Clean build on Windows with zero deprecation warnings.

## SCN-001 – Node editor scaffold not executable
- Status: open
- Area: `src/nodes.rs`, `src/ui/workspaces.rs`
- Description: NodeGraph types exist but likely not executed or reflected in renderer.
- Repro: Attempt to add nodes and evaluate; results do not affect viewport.
- Suspected cause: Missing evaluation pipeline integration.
- Impact: Visual programming workflow unavailable.
- Fix plan: Implement execution graph → fractal parameter bindings; add minimal node palette.
- Acceptance criteria: Changes in node graph affect renderer parameters; saved to project.

## PRJ-001 – Project save/load surface-level only
- Status: open
- Area: `src/project/mod.rs`, `src/ui/main.rs` (File menu)
- Description: Save/load uses JSON with basic structures; not all runtime state is preserved or validated.
- Repro: Save a complex project (nodes, animations), load it; state mismatch.
- Suspected cause: Serialization gaps across modules.
- Impact: Data loss or incorrect state on reload.
- Fix plan: Expand serialization coverage; add versioned schema; validation on load.
- Acceptance criteria: Round-trip save/load preserves scene, animation, nodes, and renderer settings.

## DOCS-002 – README start instructions mislead; GUI feature required
- Status: open
- Area: `README.md`, `src/main.rs`, `scripts/dev_smoke.ps1`
- Description: `cargo run` without features prints that GUI is unavailable. README suggests running without enabling `gui`, leading to confusion.
- Repro:
  1) Run `cargo run`.
  2) Observe message that GUI is not available because the `gui` feature is disabled.
  3) Follow README instructions; app still doesn’t start the GUI.
- Suspected cause: GUI entry point is gated under `#[cfg(feature = "gui")]` and README lacks explicit feature guidance.
- Impact: Onboarding friction; users assume the app is broken.
- Fix plan:
  - Update README to use `cargo run --features gui` for Bevy GUI.
  - Document optional `eframe` standalone path and how to launch it (if supported).
  - Add guidance in `scripts/dev_smoke.ps1` to detect missing features and print help.
- Acceptance criteria:
  - README contains explicit, tested commands to start the GUI.
  - Smoke script prints a helpful message if run without `--features gui`.

## BUILD-001 – Fragmented feature flags for GUI (bevy_egui vs eframe)
- Status: open
- Area: `Cargo.toml`, `src/main.rs`, `src/main_standalone.rs`
- Description: Multiple entry points (`bevy_egui` vs `eframe`) behind different features cause confusion.
- Repro: Inspect code and features; unclear which path is canonical.
- Suspected cause: Historical split between Bevy and `eframe` UIs.
- Impact: Confusing build matrix; harder maintenance.
- Fix plan:
  - Declare Bevy + `bevy_egui` as canonical GUI.
  - Optionally keep `eframe` under `standalone_ui` feature; document it.
  - Update Cargo features and README with clear commands per path.
- Acceptance criteria:
  - Canonical GUI launches with `--features gui` and is documented.
  - Optional standalone path documented or removed if deprecated.

## TEST-001 – Complex fractal validation thresholds failing
- Status: open
- Area: `src/bin/complex_fractal_validation.rs`, `src/fractal/shaders/fractal_compute.wgsl`, `src/fractal/renderer.rs`
- Description: Headless validation snapshots produce low-contrast/flat outputs; metrics (`edge_mean`, `luminance_stddev`) below thresholds for all formulas. 3D formulas (`mandelbulb`, `quaternion_julia`) show near-zero nonblack ratios.
- Repro:
  1) Run `cargo run --bin complex-fractal-validation`.
  2) Inspect `validation_outputs/report.json` and PNGs.
  3) Observe all cases marked FAIL with low `edge_mean` and `stddev`; some 3D cases fully black.
- Suspected cause:
  - Compute shader writes unshaded DE values directly to `rgba8unorm`, causing narrow dynamic range (flat images).
  - Parameter mapping for 3D branch (camera/position/rotation/bailout/epsilon) may not align with current WGSL expectations, yielding empty hits.
  - Colorization/tone-mapping missing in compute path; storage texture likely needs post process.
- Impact: Automated correctness check flags failures; makes it difficult to detect regressions vs expected look.
- Fix plan:
  - Add simple tonemap/colorize pass in compute shader or post-processing in CPU to map DE to visually meaningful range.
  - Verify `params[0..32]` uniform mapping consistency with WGSL (`formula_id`, `max_iterations`, `bailout`, `power`, camera transforms).
  - Adjust default camera/position for 3D formulas; lower `normal_epsilon`/tune steps.
  - Update validation thresholds based on improved signal; add per-formula baselines.
- Acceptance criteria:
  - At least 3 of 5 formulas PASS with `edge_mean ≥ 1.0`, `luminance_stddev ≥ 5.0` at 1024×768.
  - 3D formulas produce visible nonblack ratios ≥ 0.05 with recognizable structure.
  - Report written with PASS/FAIL and stable metrics across two consecutive runs.

## PERF-001 – CPU readback path in preview is expensive
- Status: open
- Area: `src/fractal/renderer.rs::render_frame_to_texture`
- Description: GPU → CPU readback to build `egui` texture stalls pipeline and reduces FPS.
- Repro: Run viewport at higher resolutions; observe stutter/low FPS.
- Suspected cause: Synchronous readback and CPU-side `Color32` conversion for preview.
- Impact: Poor interactivity; heavy performance cost.
- Fix plan:
  - Use a zero-copy GPU path to present a texture via `bevy_egui` bindings.
  - Reserve CPU readback for export-only code paths.
- Acceptance criteria:
  - Viewport meets target FPS without synchronous readbacks in preview.
  - Profiling confirms no GPU→CPU round-trip in interactive rendering.

## EXP-002 – 3D mesh export not fully wired/validated
- Status: partially_addressed
- Area: `src/export/mod.rs`, `src/export/mesh.rs`, `src/export/formats.rs`, `src/ui/fractal_ui.rs::FractalExportDialog`
- Description: Mesh exporters and generators exist (OBJ/STL scaffolding), but the Export dialog does not consistently trigger mesh generation, and acceptance criteria (normals/topology/scale) are not validated against DCC tools.
  Update (2025-11-11): Basic ASCII STL export implemented; reliability still depends on mesh generation quality and UI wiring.
  Update (2025-11-11, later): ASCII PLY export implemented in `src/export/mesh.rs`. Supports vertices, optional normals/UVs, and triangle faces. UI wiring remains pending; mesh quality still depends on generator/topology.
- Repro:
  1) Open Export dialog; choose OBJ/STL.
  2) Attempt export; files may be missing, incomplete, or fail to load cleanly in Blender.
- Suspected cause: Missing UI wiring to `Exporter::export_fractal` mesh path and incomplete marching cubes/normal generation parameters.
- Impact: Users cannot reliably export usable 3D mesh files.
- Fix plan:
  - Wire Export dialog actions to mesh export pipeline with quality and iso-level controls.
  - Validate normal generation and implement manifold repair pass for STL.
  - Add orientation/scale options; document defaults.
- Acceptance criteria:
  - OBJ/STL exports open in Blender with correct scale/orientation; shading looks correct.
  - STL meshes are manifold or repaired; no critical non-manifold artifacts.
  - Export respects bounds/resolution settings; vertex count in expected range for defaults.

## EXP-003 – Voxel export (VOX/QUB) generation and format writer missing
- Status: open
- Area: `src/export/voxel.rs`, `src/export/formats.rs`, `src/ui/fractal_ui.rs::FractalExportDialog`
- Description: Voxel generator logic exists, but VOX/QUB format writing is incomplete or absent; UI exposes VOX but end-to-end export is not guaranteed.
- Repro:
  1) Select VOX in Export dialog.
  2) Attempt export; file may not be produced or fails to open in MagicaVoxel.
- Suspected cause: Missing VOX writer and palette handling; bounds/resolution mapping not fully implemented.
- Impact: Users cannot export voxel representations for voxel editors/3D printing workflows.
- Fix plan:
  - Implement VOX writer (MagicaVoxel format) with palette support.
  - Validate voxel density mapping against bounds and resolution; add threshold control.
  - Add sample presets and docs for typical voxel exports.
- Acceptance criteria:
  - VOX files open in MagicaVoxel with correct dimensions and palette.
  - Export reflects settings for bounds/resolution/threshold; surface voxels identifiable.
  - Example scenes export successfully with reasonable file sizes.

## STABILITY-001 – WGPU device lost/focus changes not handled
- Status: open
- Area: `src/gui.rs`, renderer init/config
- Description: Window resize/focus change can trigger device/surface errors without recovery.
- Repro: Resize window rapidly or switch focus; viewport may blank or freeze.
- Suspected cause: Missing handling for `SurfaceError::Lost/Outdated` and reconfigure.
- Impact: Frozen or blank viewport; perceived instability.
- Fix plan:
  - Add device/surface error handling and reconfigure logic; debounce resizes.
  - Provide fallback frame and clear UI messaging on recovery events.
- Acceptance criteria:
  - Resizing/focus changes recover gracefully; logs show handled reconfigure.

## SHD-001 – Vignette function incomplete in render shader
- Status: fixed
- Area: `src/fractal/shaders/fractal_render.wgsl`
- Description: `apply_vignette` had incomplete logic leading to negligible vignette effect.
- Repro:
  1) Render any scene and adjust vignette amount.
  2) Observe minimal or no vignette darkening at edges.
- Suspected cause: Missing final application/return of vignette factor.
- Impact: Post-process look lacked expected vignette shaping.
- Fix plan: Implement proper vignette factor computation and apply to color; add lightweight Reinhard tonemapping step in color grading.
- Acceptance criteria: Vignette amount parameter visibly darkens frame edges consistently; overall exposure is saner under bright highlights.
- Verification: Built successfully on Windows; visual validation pending in GUI preview.
---

## Triage Log
- 2025-11-06: Initialized tracker with core UI/render/export issues and misaligned shader assets.
- 2025-11-06: Added priorities and captured docs/startup, build flags, perf, stability issues.

## Acceptance Checklist (global)
- Reproducible steps documented for every issue.
- Fixes linked to specific commits/PRs.
- Verified on Windows; where applicable, smoke-tested with `scripts/dev_smoke.ps1`.
- Docs updated alongside fixes per docs-first workflow.