# Gesture Input Plan (MediaPipe, Leap Motion, OpenXR)

Purpose
- Provide unified gesture input across camera-based tracking (MediaPipe), device-based tracking (Leap Motion/Ultraleap), and VR hand tracking (OpenXR extensions).
- Normalize skeleton/joints and gestures into a common schema for modulation and UI interaction.

Backends
- OpenXR Hand Tracking: use `XR_EXT_hand_tracking` when the runtime supports it to obtain 26/ joints, positions, and velocities. Map to gestures (pinch, grab, point, open hand) for in-VR interaction.
- Leap Motion (`leaprs`): connect to Ultraleap tracking service via LeapC; poll tracking events and extract hand/finger joints; optional `glam`/`nalgebra` integration for transforms.
- MediaPipe (camera): use MediaPipe Tasks hand landmarks (GPU preferred) to obtain 21-keypoint skeleton. Consider `mediapipe-rs` approaches: WasmEdge `mediapipe-rs` for Tasks, or C++ bindings (e.g., `julesyoungberg/mediapipe-rs`).

Data Model
- Coordinate frames: define app-local frame aligned to camera/HMD; provide transforms for each backend to unify into world-space.
- Joints: standardize a subset of joints (palm, wrist, metacarpals, proximal/intermediate/distal phalanges) and per-finger tips.
- Gesture primitives: pinch (thumb-index proximity), grab (multi-finger curl), point (index extension), open hand (low curl, spread).
- Confidence: backend reports per-joint confidence; pipeline computes gesture confidence and smoothing.

API & Services
- `GestureService`: multiplex backends, emit events: `GestureEvent { source, hand, gesture, value, pose }`.
- Mapping layer: bind `GestureEvent`s to parameter modulations (e.g., pinch → scale, grab → orbit/translate, point → pick/select).
- Calibration: per-user scaling, handedness, pose reference capture.
- Smoothing: temporal filtering, deadzones, hysteresis to prevent jitter.

Performance & Reliability
- Target <4 ms processing per frame for gesture pipeline; prefer GPU-accelerated tasks for MediaPipe where possible.
- Fallbacks: if hand tracking is unavailable, degrade to controllers; if webcam unavailable, disable MediaPipe.

Implementation Notes
- Leap Motion: requires Ultraleap Tracking Software; set `LEAPSDK_LIB_PATH`, ensure `LeapC.dll` on PATH (Windows). Use `leaprs` quick start pattern to poll tracking events.
- MediaPipe: prefer Tasks API via WasmEdge `mediapipe-rs` for Rust-side inference, or bind to C++ MediaPipe if needed; ensure model assets (TFLite) are available.
- OpenXR: enable `XR_EXT_hand_tracking`; detect support per runtime and switch to controller emulation when unavailable.

Security & Privacy
- Explicit opt-in for camera usage; show status indicator. No persistence of raw frames unless recording is enabled.

Acceptance Criteria
- MediaPipe backend: detect hands and emit pinch/grab/point/open with stable confidence at 30–60 FPS using a standard webcam.
- Leap Motion backend: track skeleton with stable joint positions; gestures mapped with low latency.
- OpenXR backend: hand tracking works on supported runtimes; controller fallback works where hand tracking is unsupported.
- Unified mapping: gestures modulate fractal parameters and interact with UI consistently across backends.

References
- Leap Motion Rust crate: https://lib.rs/crates/leaprs [Accessed], https://docs.rs/leaprs/latest/leaprs/ [Accessed]
- MediaPipe Rust efforts: https://github.com/WasmEdge/mediapipe-rs [Accessed], https://github.com/julesyoungberg/mediapipe-rs [Accessed]
- OpenXR Hand Tracking spec: https://registry.khronos.org/OpenXR/specs/1.1/man/html/XR_EXT_hand_tracking.html [Accessed]