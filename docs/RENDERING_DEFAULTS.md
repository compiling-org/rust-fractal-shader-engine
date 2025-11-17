# Rendering Defaults and Performance Guide

This project now ships with sensible rendering defaults that balance visual quality and GPU load, ensuring a clean 3D fractal view at startup without overwhelming your hardware.

## Overview of Current Defaults

- Workspace: `Rendering`
- Default fractal: `Mandelbulb (Power 8)`
- Quality preset: `Medium` (applied on renderer initialization)
- Render scale: `0.75` of viewport size
- FXAA: `enabled` for smoother edges
- Max iterations: `100` for crisper 3D detail
- Pseudo‑3D fragment pipeline: `disabled on init` to force the true raymarcher path
- Combiner presets: available in UI; viewport defaults to single formula unless enabled

## Verifying True 3D Rendering

- Ensure the “Fragment Pseudo‑3D” toggle is OFF in the UI.
- Select `Mandelbulb (Power 8)` and orbit/zoom the camera; you should see solid 3D structure with proper shading.

## Formula Selection Mapping

The viewport formula selection aligns with `fractal_types` indices:

- `0 | 1 | 2` → Mandelbulb
- `3 | 4 | 5` → Mandelbox
- `6 | 7`     → Quaternion Julia
- Others default to Mandelbulb unless you explicitly enable a combiner preset.

## Performance Tips

- If FPS is low, reduce `render_scale` to `0.5` or switch to `Low` quality.
- Increase `max_iterations` gradually (e.g., `150–200`) for more detail, and lower `render_scale` to compensate.
- Keep FXAA enabled; it offers a good quality boost at minimal cost.

## Troubleshooting

- Flat or “2D” looking output: make sure pseudo‑3D mode is disabled; the raymarcher is enforced at init.
- Wrong or bland fractal shape: confirm the selected preset matches the intended formula (see mapping above).
- Very high GPU usage: check the quality preset and render scale; Medium at 0.75 is the new default balance.

## Change Summary

- Default workspace set to `Rendering`.
- Default fractal set to Mandelbulb; combiner off by default in viewport.
- Renderer initialization applies `Medium` quality and disables pseudo‑3D.
- Viewport formula mapping fixed to match preset indices.
- Default `max_iterations` increased from 50 → 100.

## DE→Color Mapping Defaults

- Param indices: `params[50] = de_color_mode`, `params[51] = de_color_scale`.
- Modes: `0` off, `1` grayscale by depth, `2` multiply base color.
- Default on init: `de_color_mode = 0`, `de_color_scale = 0.0`.
- Quick Controls provide one-click toggles and an inline scale slider.

## Tonemapping Defaults

- Param indices: `params[52] = tonemap_mode`, `params[53] = tonemap_exposure`.
- Modes: `0` off, `1` Reinhard (applied before gamma).
- Defaults on init: `tonemap_mode = 0` (disabled), `tonemap_exposure = 1.0`.
- Recommended: enable `tonemap_mode = 1` and tune exposure (`0.8–1.5`).