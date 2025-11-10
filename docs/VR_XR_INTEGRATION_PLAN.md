# VR/XR Integration Plan (OpenXR + WGPU)

Purpose
- Define how the app supports VR/XR headsets via OpenXR, including stereoscopic rendering and 180°/360° projection modes.
- Establish interfaces for per-eye rendering, swapchain integration, tracking spaces, and hand/controller input.

Scope
- Platforms: Windows (SteamVR, Oculus), Linux (Monado, SteamVR). macOS is limited for OpenXR.
- Graphics: `wgpu` backends (Vulkan, D3D12) driven by existing renderer.
- Input: OpenXR action maps, controller poses, and `XR_EXT_hand_tracking` when available.

Runtime Architecture
- Entry: Initialize OpenXR `Entry` → `Instance` with required extensions, select `System` for `FormFactor::HeadMountedDisplay`.
- Session: Create `Session` with graphics binding (Vulkan/D3D12 via `wgpu`), set `ReferenceSpace` (`Local` for scene, `View` per-eye).
- Swapchain: Create color/depth swapchains compatible with HMD; acquire/release images each frame.
- Frame Loop: Wait/Begin frame → locate views → render per-eye → submit composition layers → End frame.

Renderer Responsibilities
- Per-eye views: obtain `Fov` and view/projection matrices; render eye textures using existing fractal ray marcher with correct camera.
- Timing: drive animation using OpenXR frame timing (`predictedDisplayTime`) to keep motion reprojection stable.
- Composition: submit `XR_COMPOSITION_LAYER_PROJECTION` with array of `ProjectionView`s for each eye.

Stereoscopic Rendering Modes
- HMD Stereo: standard per-eye rendering via OpenXR `ProjectionView`.
- Desktop Stereo: side-by-side and over-under render targets for non-HMD displays; optional anaglyph preview.
- Convergence/IPD: expose IPD and convergence adjustments; in HMD use runtime IPD, desktop uses app-defined values.

180°/360° Projection Modes
- 180° VR (VR180): dual-fisheye stereo rendering; project rays using fisheye mapping (e.g., equidistant or equisolid). Target equirectangular or dual-fisheye textures.
- 360° Mono/Stereo: equirectangular sphere mapping for panorama output. Stereo uses per-eye offset and appropriate spherical parallax.
- Lens/distortion: rely on HMD runtime distortion; for offline 180/360 exports apply projection without runtime distortion.

Tracking & Input
- Controllers: use OpenXR action maps for pose, buttons, and haptics.
- Hand tracking: when available, enable `XR_EXT_hand_tracking` to query joint locations/velocities and map gestures to app actions.
- Spaces: `Local` for world, `View` for per-eye, `Stage` when room-scale is required.

Data Interfaces
- `XrRuntime` service: start/stop runtime, enumerate devices, create session, expose frame timing and per-eye matrices.
- `XrInput` layer: action sets for locomotion, UI interaction, and parameter modulation.
- `HandTracking` layer: abstraction over OpenXR hand joints to emit high-level gestures (pinch, grab, point).

Performance Targets
- HMD: 90 Hz preferred (Quest/Index), 72/80 Hz acceptable fallback; maintain <11ms GPU at 90 Hz on mid-range GPU.
- Desktop stereo: 60 FPS at 1080p; scalable quality presets and foveation where available.

Testing & Validation
- Runtimes: SteamVR (Windows), Oculus (Windows), Monado (Linux). Verify session creation, views, and frame submission.
- Visual: confirm correct per-eye parallax, head pose stability, and no judder.
- Input: validate action map bindings across runtimes; verify hand tracking when supported.

Integration Points
- Renderer: per-eye camera feed and projection; quality/foveation hooks; ray marcher supports spherical mappings.
- Controls & Modulation: map XR inputs to parameters and node system events.
- Export: allow 180/360 renders as image/video targets.

References
- OpenXR Rust crate docs: https://docs.rs/openxr [Accessed]
- OpenXR Hand Tracking spec: https://registry.khronos.org/OpenXR/specs/1.1/man/html/XR_EXT_hand_tracking.html [Accessed]
- Bevy/OpenXR efforts (ecosystem context): https://github.com/blaind/xrbevy [Accessed], https://lib.rs/crates/bevy_mod_openxr [Accessed]

Milestones
- M1: Create OpenXR session, render per-eye projection at 60–90 Hz; desktop stereo preview.
- M2: Add hand tracking abstraction and basic gestures; implement VR180 dual-fisheye render path.
- M3: Performance tuning, foveation where supported; export 180/360 panoramas.