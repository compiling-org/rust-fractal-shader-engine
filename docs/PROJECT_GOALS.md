# Modular Fractal Shader - Project Goals & Progress Tracking

## Vision Statement
Build a powerful and approachable fractal studio focused on desktop, with clear evolution from CPU-based fractal math to a GPU‑accelerated pipeline, and an honest, incremental roadmap.

## Core Objectives

### 1. Professional 3D Fractal Generation
**Goal**: Enable artists and mathematicians to create, explore, and render complex 3D fractals in real-time with professional quality.

**Progress**: ⚠️ 40% Complete
- ✅ 2D fractal rendering (Mandelbrot, Julia)
- ✅ 3D fractal math (Mandelbulb, Mandelbox) on CPU
- 🚧 GPU integration (WGSL/WGPU) in progress
- 🚧 Basic 3D camera setup; minimal controls
- ❌ Lighting and materials not implemented
 
## Cross-References
- See `docs/ADVANCED_SUITE_REQUIREMENTS.md` for module-by-module acceptance criteria.
- See `docs/GAP_ASSESSMENT.md` for Present/Partial/Missing status across the suite.
- See `docs/REALTIME_3D_ENGINE_PLAN.md` for the real-time renderer loop, WGSL interface, and performance targets.

### 2. Node-Based Visual Programming
**Goal**: Provide an intuitive node-based interface for composing complex fractal systems, similar to Houdini or TouchDesigner.

**Progress**: ⚠️ 20% Complete
- ✅ Node data model and execution stubs
- ❌ Visual editor UI (drag/connect/group) not implemented
- ⏳ Generator/effect/transform nodes planned

### 3. Real-time Performance & Output
**Goal**: Deliver butter-smooth real-time rendering with professional output options for live visuals, installations, and broadcast.

**Progress**: ⚠️ 30% Complete
- ✅ Basic UI responsiveness optimizations
- 🚧 GPU wiring in progress
- ❌ Multi-display output and Syphon/Spout/NDI not implemented

### 4. Advanced Animation & Timeline
**Goal**: Offer sophisticated animation tools with keyframe editing, procedural generators, and audio-reactive capabilities.

**Progress**: ⚠️ 15% Complete
- 🚧 Timeline/keyframe structures exist
- ❌ Timeline UI and playback not implemented
- ⏳ Audio/MIDI modulation planned

### 5. Professional Export & Sharing
**Goal**: Support industry-standard export formats and blockchain integration for NFT creation and marketplace sharing.

**Progress**: ⚠️ 25% Complete
- 🚧 Basic image export (snapshot alpha)
- 🚧 Mesh export placeholders (OBJ/STL/PLY; cube)
- ❌ Animation sequences, NFT integration not implemented

#### 5.a 3D Fractal File Export (Explicit Goal)
**Goal**: Export fractals as 3D files suitable for DCC pipelines and 3D printing.

**Supported/Planned Formats**
- ✅ OBJ, ✅ STL (mesh)
- ⏳ FBX, ⏳ glTF (mesh + animation)
- ⏳ VOX, ⏳ QUB (voxel)

**Acceptance Criteria**
- Mesh exports load in Blender/Maya with correct scale and orientation.
- Vertex normals are coherent; no flipped/shaded artifacts on typical scenes.
- Topology is manifold for STL; non-manifold triangles flagged and optionally repaired.
- Bounds and density reflect export settings; reasonable vertex count at default quality.
- Voxel exports open in MagicaVoxel; resolution and palette preserved where applicable.

**Cross-References**
- Code: `src/export/mod.rs`, `src/export/mesh.rs`, `src/export/formats.rs`, `src/export/voxel.rs`
- UI: `src/ui/fractal_ui.rs::FractalExportDialog`
- Roadmap: `docs/FEATURE_DEVELOPMENT_ROADMAP.md` (Phase 6: Export System Enhancement)

## Feature Areas - Detailed Tracking

### Rendering Engine
- [x] 2D Fractal Algorithms (Mandelbrot, Julia)
- [x] 3D Fractal Math (Mandelbulb, Mandelbox)
- [ ] GPU Compute Shaders (wiring in progress)
- [ ] Real-time Ray Marching on GPU
- [ ] Global Illumination
- [ ] Volumetric Effects
- [ ] Adaptive Quality Scaling

### User Interface
- [x] bevy_egui integration and basic panels
- [x] Viewport texture binding to Camera3d
- [ ] Node Editor Canvas (UI)
- [ ] Animation Timeline
- [ ] Material Editor

### File I/O & Export
- [x] Image Export (PNG, JPEG)
- [x] 3D Mesh Export (OBJ, STL)
- [ ] Voxel Export (VOX, QUB)
- [ ] Animation Export (MP4, GIF)
- [x] Project Save/Load
- [ ] Preset Management

### Animation & Control
- [ ] Keyframe Timeline
- [x] Procedural Generators
- [ ] Audio Reactivity
- [ ] MIDI Control
- [ ] OSC Integration
- [ ] Gesture Control

### Web & Distribution
- ℹ️ Web/WASM is maintained externally (NUWE). Desktop scope only here.
- [ ] Collaborative Editing (future)
- [ ] Plugin Architecture (future)

### Blockchain & NFT
- Out-of-scope for this repository; may be tracked externally.

## Success Metrics

### Technical Performance
- Target: 60 FPS at 720p once GPU path lands
- Parameter updates responsive in GUI (basic)
- Headless export pipeline under development

### User Experience
- Intuitive interface for non-programmers ⚠️
- Comprehensive documentation and tutorials ⚠️
- Professional preset library ⚠️
- Community sharing platform ❌

### Market Position
- Adoption by VJ artists and digital creators ⚠️
- Integration with professional broadcast workflows ⚠️
- Recognition in creative coding communities ⚠️
- Commercial licensing opportunities ❌

## Quarterly Targets

### Q4 2025: Core Foundations
- Wire GPU renderer into viewport
- Minimal node editor UI
- Basic export reliability

### Q1 2026: Core Features
- Audio/MIDI integration
- Timeline editor and playback
- Mesh/voxel export improvements

### Q2 2026: Ecosystem
- Plugin architecture exploration
- Collaboration roadmap definition

### Q3 2026: Innovation
- Add AI-assisted fractal generation
- Implement VR/AR viewing modes
- Add collaborative editing features
- Release enterprise version

## Risk Mitigation

### Technical Risks
- **GPU Compatibility**: Vulkan/DX12/GL fallback diagnostics and selection
- **Performance Scaling**: Quality presets and adaptive iteration budgets
- **Desktop Scope**: Web deployment tracked externally; avoid cross‑scope drift

### Market Risks
- **Competition**: Differentiate through unique node-based approach
- **Adoption**: Provide extensive tutorials and example projects
- **Monetization**: Offer tiered licensing with clear value propositions

### Resource Risks
- **Team Scaling**: Document architecture for new contributors
- **Funding**: Demonstrate progress with milestone-based funding
- **Time Management**: Use agile methodology with 2-week sprints

## Stakeholder Communication

### Development Team
- Weekly standups
- Bi-weekly sprint reviews
- Monthly roadmap alignment

### Community & Users
- Monthly progress updates
- Beta testing program
- Feature request prioritization

### Investors & Partners
- Quarterly business reviews
- Technical milestone demonstrations
- Market positioning updates

## Recent Architecture Updates (November 2025)

### Migration from eframe to Bevy + bevy_egui
- ✅ Completed migration to Bevy 0.17 with bevy_egui for UI
- ✅ Removed all eframe dependencies
- ✅ Integrated WGPU renderer with Bevy's render pipeline
- ✅ Fixed type compatibility issues between Bevy and WGPU

### Technical Improvements
- ✅ Resolved dependency version conflicts
- ✅ Improved GPU resource management
- ✅ Enhanced fractal rendering performance
- ✅ Fixed compilation warnings and errors
## GPU Policy

- Prefer discrete GPU; continue with fallbacks when needed and log diagnostics.
- Configure `WGPU_BACKEND`, `WGPU_POWER_PREF`, `WGPU_DX12_COMPILER` on Windows.
- See startup logs (`gpu_startup.log`) for adapter/backend details.