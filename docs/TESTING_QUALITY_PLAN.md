# Testing & Quality Plan

Purpose: Ensure rendering correctness, performance stability, and UI reliability through layered tests and quality gates.

## Test Types
- Snapshot Rendering:
  - Offscreen renders to PNG baselines; compare MSE and edge-density.
- Parameter Sweeps:
  - Vary DE params (power, steps, epsilon) to validate convergence.
- UI Smoke Tests:
  - Panels open/close; sliders/buttons update parameters.
- Performance Benchmarks:
  - Measure frame times at standard resolutions and quality presets.
- Integration Tests:
  - OSC/Leap/MediaPipe/MIDI mapping to parameters.

## Tooling & CI
- WGSL validation (Tint/Naga) for shader changes.
- Headless `wgpu` renders for snapshots (Windows/macOS/Linux).
- CI gates: snapshot deltas within thresholds; perf not regressed.
- Artifacts: baseline images, logs, benchmark CSVs.

## GPU Availability Smoke Tests (Mandatory)
- The GUI path must fail-fast when no GPU device is available.
- Pre-run environment requirements:
  - Windows: `WGPU_BACKEND=vulkan,dx12`, `WGPU_POWER_PREF=high`, `WGPU_DX12_COMPILER=fxc`
  - macOS: `WGPU_BACKEND=metal`, `WGPU_POWER_PREF=high`
  - Linux: `WGPU_BACKEND=vulkan`, `WGPU_POWER_PREF=high`
- CI/Local smoke step:
  - Run `cargo run --bin fractal_snapshot` and assert a WGPU device is created; capture adapter/backend in logs.
  - Any failure to acquire device or adapter is a hard error for GUI features.

### Complex Fractal Validation (headless)
- Binary: `cargo run --bin complex-fractal-validation`
- Output: PNGs per formula to `validation_outputs/`, plus `report.json` with metrics.
- Metrics: `nonblack_ratio`, `luminance_stddev`, `edge_mean`, `elapsed_ms`.
- Thresholds:
  - 3D formulas (`mandelbulb`, `mandelbox`, `quaternion_julia`): `nonblack_ratio ≥ 0.05`, `edge_mean ≥ 1.0`, `luminance_stddev ≥ 5.0`.
  - 2D formulas (`mandelbrot`, `julia`): `nonblack_ratio ≥ 0.01`, `edge_mean ≥ 1.0`, `luminance_stddev ≥ 5.0`.
- Use as a smoke test for correctness after shader or parameter changes.

## Quality Gates
- Rendering: No NaNs; hit epsilon respected; clean normals.
- Shading: AO/shadows within step budgets; no flicker.
- UI: No panics; consistent parameter updates; accessibility checks.
- Performance: Targets met for default preset; heavy preset acceptable.
 - GPU: Adapter/device acquired; backend selection logged; fail otherwise.

## Metrics
- Frame time distribution (p50/p90/p99).
- GPU dispatch counts and occupancy (where available).
- Error rates and exceptions per session.

## Maintenance
- Update baselines when visual changes are intentional; document rationale.
- Keep thresholds tight; avoid masking regressions.
- Link tests to tasks in DEVELOPMENT_PLAN.md.