# Fractal Shader Studio — Status Report (2025-11-07)

## Summary
- Project builds successfully under `dev-fast` profile; app starts without immediate exit.
- Egui viewport now renders predictably with a visible window and debug overlay.
- Compute shader compiles cleanly after fixing WGSL function scoping.
- GPU renderer still not displaying fractal output in the UI; temporary CPU gradient fallback added.

## Changes Implemented
- Corrected egui 0.32 image API usage: use `egui::load::SizedTexture` path compatible with current egui version.
- Fixed WGSL parse failure: moved helper functions (`eval_formula`, `smin`, `smax`, `combine2`) to module scope in `fractal_compute.wgsl`.
- Added dedicated egui "Viewport" window with a small debug overlay (texture size, renderer availability, readback cadence).
- Throttled GPU→CPU readbacks in `src/gui.rs` to every N frames (default 10) to improve UI responsiveness.
- Aligned Bevy viewport `Image` texture format to `Rgba8Unorm` to match renderer output and avoid SRGB gamma mismatch.
- Added one-time CPU gradient population when `FractalRenderer` is missing so the viewport is never blank.
- Preserved WGPU preflight checks and device watchdog, but deferred initialization to when Bevy’s `RenderDevice` and `RenderQueue` are available.

## Accomplishments
- Build green (exit code 0) with known non-critical warnings.
- App window remains open; no auto-close due to missing windows.
- Compute shader now compiles (no nested function errors), unblocking renderer creation logic.
- UI viewport is consistently visible, aiding diagnosis.
- Logged size mismatches and renderer absence for targeted debugging.

## Current Behavior
- On startup, if the GPU renderer is not available, the viewport shows a gradient placeholder.
- When the renderer is present, CPU readback occurs at most once every 10 frames; UI should be more responsive.
- Egui viewport and overlay show resolution and whether a renderer is attached.

## Known Issues
- Fractal output not visible in UI: `FractalRenderer` may not fully initialize, or its output isn’t copied into the Bevy texture.
- No GPU-side copy path into the Bevy `Image`: current approach relies on CPU readback (slow, stutters).
- Backend selection fragility on Windows: Vulkan may fail on some drivers; need auto-fallback to DX12.
- Heavy warnings across bins and modules (unused fields/variants/imports) add noise during development.
- Limited runtime diagnostics: renderer init errors are logged but not surfaced in the UI overlay.
- Potential mismatch between render target format and shader assumptions in post-process pass.
- Performance bottleneck: per-frame UI work and occasional readbacks can block the main thread.

## Suggested Next Steps
1. Implement GPU copy path into `Image`:
   - Use Bevy render graph or `wgpu::Queue::write_texture` with staging buffer to update the Bevy texture without CPU readback.
   - Prefer `TextureFormat::Rgba8Unorm` end-to-end.
2. Harden backend selection:
   - Switch env default to `WGPU_BACKEND=Auto` and attempt DX12 if Vulkan fails.
   - Log adapter selection (name, vendor, backend) in UI.
3. Surface renderer init errors in UI:
   - Capture `FractalRenderer::new_with_wgpu_context` failures and display error text in the viewport overlay.
   - Add a "Retry GPU init" button.
4. Reduce warning noise:
   - Run `cargo fix` across bins; disable or gate unused code behind feature flags.
5. Add shader validation step:
   - Compile WGSL modules at startup and list errors in a collapsible UI panel.
6. Performance improvements:
   - Avoid per-frame heavy operations in `update`; cache and only refresh on camera/param changes.
   - Consider double-buffering for texture updates.

## How to Verify
- Build: `cargo build --profile dev-fast`
- Run: `cargo run --profile dev-fast`
- Expected: App window opens; "Viewport" window shows resolution and either a gradient (no renderer) or fractal output.
- Logs: Check `gpu_startup.log` and runtime logs for renderer availability and any size mismatches.

## Commit Summary
- Files touched:
  - `src/fractal/shaders/fractal_compute.wgsl` — helper functions lifted to module scope.
  - `src/gui.rs` — viewport window, debug overlay, readback throttling, texture format alignment, gradient fallback.
  - `docs/STATUS_REPORT.md` — this status report.