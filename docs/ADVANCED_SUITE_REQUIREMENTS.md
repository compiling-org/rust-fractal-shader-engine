# Advanced Fractal Research Suite Requirements

Vision
- Professional real-time 3D fractal generation studio for VFX/3D pipelines and immersive VJ systems.
- Node-based authoring, real-time GPU renderer, animation, gesture/VR interaction, and advanced export.

Core Modules
- Renderer & Real-time: WGSL/WGPU renderer, ray marching, lighting/materials, quality scaling, metrics overlay.
- UI/UX: panels, workspace management, property editors, timeline/animation.
- Node System: formulas/operators graph, parameter ports, evaluation, presets.
- File I/O & Export: project save/load, images/sequences, GLTF/USD; later NDI/Spout/Syphon.
- Formula Library: curated 3D formulas with metadata and examples.
- Shader Conversion: ISF/GLSL → WGSL subset converter with validation.
- Controls & Modulation: MIDI/OSC, automation, mappings to parameters.
- Integration: VFX/3D pipeline and VJ system interop; multi-display.
- Web Deployment: WebGPU preview mode.
- Testing & Quality: QA suites, performance budgets, acceptance criteria.

VR/XR & Gesture Addendum
- VR/XR support (OpenXR):
  - Session creation and per-eye rendering via `wgpu` integration.
  - Stereoscopic per-eye projection; desktop stereo (SBS/OU) preview.
  - 180° dual-fisheye and 360° equirectangular export modes.
  - Acceptance: maintains HMD refresh ≥72 Hz with stable head pose; correct parallax and view transforms.
- Gesture controls:
  - Backends: MediaPipe (webcam), Leap Motion (`leaprs`), OpenXR hand tracking extension.
  - Unified gesture schema (pinch/grab/point/open) with smoothing and calibration.
  - Acceptance: gestures modulate parameters/UI consistently across backends at 30–90 FPS.

Milestones
- M1: Real-time renderer hits 60 FPS @ 720p; metrics overlay; 3 core formulas.
- M2: Node editor baseline; snapshot/export parity; soft shadows.
- M3: ISF/GLSL conversion subset; expanded formula library; VJ module API.
- M4: Multi-display/NDI/Spout/Syphon; advanced materials; 1080p perf targets.
- M5: VR/XR integration baseline (OpenXR per-eye); desktop stereo preview.
- M6: Gesture integration (MediaPipe, Leap Motion, OpenXR); unified mapping.
- M7: 180° dual-fisheye and 360° equirectangular export modes.

Operating Procedure
- Doc-first: update requirements and acceptance criteria before code changes.
- Validation: verify in live GUI; record performance metrics per change.

References
- VR/XR integration plan: ./VR_XR_INTEGRATION_PLAN.md
- Gesture input plan: ./GESTURE_INPUT_PLAN.md
- OpenXR Rust crate docs: https://docs.rs/openxr [Accessed]
- OpenXR Hand Tracking spec: https://registry.khronos.org/OpenXR/specs/1.1/man/html/XR_EXT_hand_tracking.html [Accessed]
- Leap Motion Rust crate: https://lib.rs/crates/leaprs [Accessed]
- MediaPipe Rust efforts: https://github.com/WasmEdge/mediapipe-rs [Accessed], https://github.com/julesyoungberg/mediapipe-rs [Accessed]