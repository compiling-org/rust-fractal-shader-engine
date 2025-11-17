Theme Controls (Desktop UI)

Overview
- Adds a Theme submenu under `View` with live controls for brightness, contrast, font size, and colors.
- Applies changes every frame to `egui` visuals; no restart required.
- Works in the desktop app; no web server or preview URL is involved.

Where to Find
- Open the app and go to `View > Theme`.
- Adjust sliders and pick colors; click `Reset Theme` to restore defaults.

Controls
- Brightness: `0.5–1.5` range; scales overall luminance.
- Contrast: `0.5–1.5` range; adjusts contrast around 0.5 pivot.
- Font Size: `0.5–1.8` range; scales UI via `pixels_per_point`.
- Colors: Background, Primary (accent), Text via color pickers.

Defaults
- Brightness: `1.0`
- Contrast: `1.0`
- Font Size: `1.0`
- Background: `rgb(18,18,18)`
- Primary: `rgb(0,136,255)`
- Text: `rgb(220,220,220)`

Implementation Notes
- State fields added to `FractalStudioApp`: `ui_brightness`, `ui_contrast`, `ui_font_scale`, `ui_bg_color`, `ui_primary_color`, `ui_text_color`.
- Per-frame application via `apply_ui_theme(ctx)` at the start of `update()`.
- Visuals updated using `egui::Visuals::dark()` and overridden fills and strokes.

Run & Validate (Desktop)
- Ensure Rust toolchain is installed.
- From project root: `cargo run` to start the desktop app.
- Navigate to `View > Theme` and adjust settings.

Troubleshooting
- If `cargo run` fails due to unrelated compile errors, fix or temporarily disable failing modules, then retry.
- No web server is used; ignore any web preview steps.