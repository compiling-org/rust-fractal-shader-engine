# Presets

Workspace-level presets serialize fractal parameters, quality settings, and render toggles for consistent re-use across sessions.

## What’s Included
- `fractal`: `crate::fractal::types::FractalParameters`
- `quality`: `crate::fractal::types::QualitySettings`
- `pseudo3d_enabled`: per-workspace pseudo‑3D toggle
- `fxaa_enabled`: anti‑aliasing toggle (renderer wiring pending)
- `created_at`: timestamp

## Save/Load Formats
- JSON (`.json`) via `serde_json` – human-friendly and interoperable
- RON (`.ron`) via `ron` – Rusty readable format with comments support

## Usage (Library)
- Module: `crate::preset`
- Type: `preset::WorkspacePreset`

### Create and Save
```rust
use fractal_generator_lib::preset::WorkspacePreset;
use fractal_generator_lib::fractal::types::{FractalParameters, QualitySettings};

let fractal = FractalParameters::default();
let quality = QualitySettings::default();
let preset = WorkspacePreset::new("My Preset", fractal, quality, true, false);

preset.save_json("presets/my_preset.json").unwrap();
preset.save_ron("presets/my_preset.ron").unwrap();
```

### Load and Apply
```rust
use fractal_generator_lib::preset::WorkspacePreset;
use fractal_generator_lib::fractal::renderer::FractalRenderer;

let mut renderer: FractalRenderer = /* obtain from UI init */ unimplemented!();
let preset = WorkspacePreset::load_json("presets/my_preset.json").unwrap();
preset.apply_to_renderer(&mut renderer);
```

## Notes
- FXAA toggle is captured for forward compatibility; integration in the renderer post-process is pending.
- The pseudo‑3D toggle is applied via `FractalRenderer::set_fragment_pseudo3d`.
- Presets are intended to be non-UI; UI wiring can bind buttons to `save_*`/`load_*` and `apply_to_renderer`.

## UI Usage

In the desktop app, use the top bar Presets menu for quick management:

- Open `File` → `Presets`.
- Enter a name and click `Save JSON` or `Save RON` to write into `presets/`.
- Use `Save As...` to choose a custom path via a file dialog (JSON or RON).
- Use `Open...` to pick an existing preset file and apply it.
- The menu lists existing presets found under `presets/`; click `Load` next to an item.
- Status messages appear as a dismissible banner in the top panel.