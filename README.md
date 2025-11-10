# Modular Fractal Shader - Professional Fractal Generator

A comprehensive, modular fractal generator built with Rust and Bevy. Features advanced GPU-accelerated fractal rendering, real-time audio/MIDI control, and a visual node-based composition interface.

## 🚀 **Latest Update - November 2025**

### **Architecture Migration Complete**
- ✅ **GUI Framework**: Migrated from eframe to Bevy 0.17 + bevy_egui
- ✅ **Rendering Engine**: Integrated WGPU renderer with Bevy's render pipeline
- ✅ **Performance**: Optimized GPU resource management and rendering pipeline
- ✅ **Compatibility**: Resolved dependency conflicts and compilation issues

### **Current Status & Recent Updates**

#### ✅ **Completed Features (v1.0.0)**
- **GPU Acceleration**: WebGPU/Vulkan/Metal/DX12 rendering with real-time performance
- **Node-Based Editor**: Visual composition interface with drag-and-drop functionality
- **Shader Generation**: Advanced fractal algorithms with distance estimation
- **3D Scene Environment**: Full 3D environment with fractal objects
- **Animation System**: Keyframe animation for camera, lighting, and parameters
- **Export System**: 3D mesh export (OBJ, STL) and image formats
- **Professional UI**: Modern dark theme with glassmorphism design
- **Cross-Platform**: Windows, macOS, Linux support
- **Web Deployment**: WASM/WebGPU support for browser-based editing

#### 🔄 **In Development**
- **Advanced Features**: Global illumination, volumetric effects, VR/AR support
- **Animation Enhancements**: Timeline editor, audio reactivity, MIDI control
- **Export Improvements**: Animation sequences, voxel formats, NFT integration
- **Node Editor**: Advanced node composition and preset management

#### 🚧 **Known Issues**
- **Performance**: Memory optimization for complex scenes
- **Feature Completeness**: Some advanced animation features pending

#### 📈 **Next Development Phase**
1. **Animation System**: Complete timeline editor and keyframe animation
2. **Audio Integration**: Real-time audio analysis and MIDI control
3. **Advanced Rendering**: Global illumination and volumetric effects
4. **Export Features**: Animation sequences and additional formats

[![Rust](https://img.shields.io/badge/rust-1.70%2B-orange)](https://www.rust-lang.org/)
[![Bevy](https://img.shields.io/badge/bevy-0.17-blue)](https://bevyengine.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## ✨ Features

### 🎨 **Fractal Generation**
- **2D Fractals**: Mandelbrot, Julia, Burning Ship, Tricorn, Phoenix
- **3D Fractals**: Mandelbulb, Mandelbox, Menger Sponge, Quaternion Julia
- **Hybrid Fractals**: BulbBox, Amazing Box, Kaleidoscopic IFS
- **Procedural**: Generic spirals, torus, helix, vortex patterns
- **Distance Estimation**: GPU-accelerated real-time rendering

### 🎛️ **Interactive Control System**
- **Real-time Parameters**: Zoom, iterations, power, bailout, rotation
- **Color Control**: Customizable palettes and gradient mapping
- **Transform Controls**: Position, scale, rotation, folding parameters
- **Fractal-specific**: Mandelbulb power, Mandelbox folding, IFS transforms

### 🎯 **Node-Based Composition**
- **Visual Node Editor**: Drag-and-drop fractal composition
- **Generator Nodes**: 2D/3D fractals, noise, mathematical functions
- **Transform Nodes**: Position, scale, rotate, warp operations
- **Effect Nodes**: Color correction, geometry transforms, filters
- **Animation Nodes**: Timeline control, LFO oscillators, noise generators

### 🚀 **Performance & Compatibility**
- **GPU Acceleration**: WebGPU/Vulkan/Metal/DX12 support
- **Real-time Rendering**: 60+ FPS on modern GPUs
- **Cross-Platform**: Windows, macOS, Linux, Web (WASM)
- **Memory Efficient**: Optimized resource management

### 🎬 **Animation & Motion**
- **Keyframe Animation**: Professional timeline with interpolation
- **Procedural Animation**: L-systems, noise, attractors
- **Camera Animation**: Cinematic camera movement
- **Parameter Automation**: Dynamic fractal parameter changes

### 🎨 **Rendering Pipeline**
- **Ray Marching**: Real-time distance field rendering
- **Adaptive Quality**: Automatic LOD based on performance
- **Professional Viewport**: 3D navigation and camera controls
- **Material System**: PBR materials with metallic/roughness workflow

### 📺 **Real-time Output**
- **Multi-Display**: Support for complex display setups
- **Viewport Controls**: Professional camera and navigation tools
- **Interactive Controls**: Real-time parameter adjustment
- **Export Preview**: WYSIWYG export preparation

### 🎮 **Professional UI/UX**
- **Dark Theme**: Modern dark interface with glassmorphism
- **Customizable Workspaces**: Multiple layout configurations
- **Context Menus**: Right-click context-sensitive actions
- **Keyboard Shortcuts**: Efficient workflow optimization

## 📦 Installation

### Prerequisites
- Rust 1.70+
- Vulkan/Metal/DX12 compatible GPU
- Audio device (optional, for audio features)
- MIDI device (optional, for MIDI control)

### From Source
```bash
git clone https://github.com/compiling-org/modular-fractal-shader
cd modular-fractal-shader
cargo build --release
```

## Documentation Suite
- `docs/DEVELOPMENT_PLAN.md` — Living product plan with phases and multi-goal tasks.
- `docs/EVOLUTION_TRACKER.md` — Chronicle of growth across sprints/releases.
- `docs/RELEASE_PLAN.md` — Versioning, criteria, and release checklist.
- `docs/TESTING_QUALITY_PLAN.md` — Snapshot testing, performance, and quality gates.
- `docs/DOCS_MAINTENANCE.md` — How and when to update documents.
- See also: `docs/PROJECT_GOALS.md`, `docs/FEATURES_STATUS.md`, `docs/DEVELOPMENT_ROADMAP.md`, `docs/UI_UX_DESIGN_GUIDE.md`.

All documents are living and must be updated at each sprint and release.

## ⚠️ GPU-Only Policy (Non-Negotiable)

- This application must never run without a real GPU device. There is no CPU fallback for the GUI path.
- Startup enforces a fail-fast watchdog: if `RenderDevice` is not available shortly after launch, the app aborts with a clear error.
- Mandatory environment configuration before running:
  - Windows: set `WGPU_BACKEND=vulkan,dx12`, `WGPU_POWER_PREF=high`, `WGPU_DX12_COMPILER=fxc`
  - macOS: set `WGPU_BACKEND=metal`, `WGPU_POWER_PREF=high`
  - Linux: set `WGPU_BACKEND=vulkan`, `WGPU_POWER_PREF=high`
- Development and CI must use machines with a discrete GPU and up-to-date drivers. Do not attempt to run or validate GUI features on CPU-only hosts.

### Required Run Commands

- PowerShell (Windows):

```
$env:WGPU_BACKEND = 'vulkan,dx12'
$env:WGPU_POWER_PREF = 'high'
$env:WGPU_DX12_COMPILER = 'fxc'
cargo run --features gui
```

- Git Bash (Windows):

```
export WGPU_BACKEND='vulkan,dx12'
export WGPU_POWER_PREF='high'
export WGPU_DX12_COMPILER='fxc'
cargo run --features gui
```

- macOS/Linux:

```
export WGPU_BACKEND='metal'   # macOS
export WGPU_BACKEND='vulkan'  # Linux
export WGPU_POWER_PREF='high'
cargo run --features gui
```

If the app exits with a message about “GPU device not available — GPU-only policy enforced,” verify drivers and backend selection.

### Docs-First Workflow (Required)

- Every code change must update relevant docs in the same PR.
- CI blocks PRs if required docs are missing or outdated.
- Use the PR template to link to updated docs and explain rationale.
- Install local Git hooks (Windows) from `scripts/` to preflight commit messages and basic checks.

Required documentation updates per change:
- `docs/CHANGELOG.md` — summarize changes with links
- `docs/DEVELOPMENT_PLAN.md` — update scope/acceptance if features shift
- `docs/ARCHITECTURE.md` — reflect any design changes
- `docs/USAGE_GUIDE.md` — update behavior/UX if user flows change
- `docs/ISSUES_TRACKER.md` — add/update tracked issues when discovering or fixing problems

Track Current Issues:
- Known problems and planned fixes live in `docs/ISSUES_TRACKER.md`.
- When you discover a new issue or change an issue’s status, update the tracker in the same PR.
- Keep repro steps and acceptance criteria precise; this makes triage and validation fast.

## 🎮 Usage

### Basic Usage
```bash
# Start GUI application
cargo run

# Run performance benchmarks
cargo run -- benchmark

# Run compatibility tests
cargo run -- test

# Build for web deployment
cargo run --features web
```

### Fragment Pseudo‑3D Mode (Experimental)
The fragment pipeline enables ShaderToy/ShadPlay‑style iteration using a fullscreen fragment shader that raymarches a Mandelbox‑style SDF. It’s great for fast iteration and pseudo‑3D decorative geometries.

- Switch to the `Rendering` workspace.
- In the left `Render Settings` panel, enable `Fragment Pseudo‑3D Mode`.
- The viewport will render via the fragment raymarcher. Toggle off to return to the compute + post‑process path.

Key parameters affecting fragment visuals:
- `scale` (Mandelbox scale)
- `bailout` (ray exit distance)
- `max_iterations` (distance estimator iterations)
- `FOV` (camera field of view; default 60°)

Notes:
- The fragment renderer prioritizes interactive iteration; for more structured multi‑stage pipelines, use the default compute path.
- Future updates will add orbit/pitch camera controls and shader hot‑reload.

### Node Editor Demo
```bash
cargo run --example node_editor_demo
```

### Fractal Rendering Demo
```bash
cargo run --example fractal_demo
```

### CLI Mesh Export (alpha)
```bash
# Export a placeholder OBJ mesh (cube) to exports/mesh.obj
cargo run --bin mesh_export -- --output exports/mesh.obj --width 32 --height 32 --depth 32
```
- Current behavior exports a placeholder cube via the simplified marching cubes path.
- Intended roadmap: load a `.fract` project, evaluate scene objects, and export meshes per object parameters.
- Flags: `--output <path>`, `--width <w>`, `--height <h>`, `--depth <d>` (defaults: `exports/mesh.obj`, `32`, `32`, `32`).

### Web Deployment
```javascript
import init, { WebFractalStudio } from './pkg/modular_fractal_shader.js';

async function run() {
    await init();
    const studio = WebFractalStudio.new('canvas');
    // Start rendering loop
    function render() {
        studio.render_frame(performance.now());
        requestAnimationFrame(render);
    }
    render();
}
```

## 🏗️ Architecture

### Core Components
- **`FractalEngine`** - Main fractal computation engine
- **`FractalRenderer`** - GPU-accelerated rendering system
- **`NodeEditor`** - Visual node composition system
- **`AnimationSystem`** - Timeline and keyframe animation
- **`SceneSystem`** - 3D scene management
- **`ExportSystem`** - Mesh and image export
- **`UISystem`** - Bevy + bevy_egui interface

### Rendering Pipeline
1. **Fractal Computation** - Distance estimation on GPU
2. **Ray Marching** - Real-time rendering of distance fields
3. **Lighting** - Physically-based lighting calculations
4. **Post-Processing** - Color grading and effects
5. **Viewport Display** - Interactive 3D viewport

## 🎨 Node Types

### Generators
- **Fractals**: Mandelbrot, Julia, Burning Ship, Mandelbulb, Mandelbox, IFS
- **Noise**: Perlin, Simplex, Voronoi, Flow noise
- **Mathematical**: L-Systems, Cellular Automata, Strange Attractors
- **Geometric**: Spheres, Boxes, Torus, Custom shapes

### Transforms
- **Spatial**: Translate, Rotate, Scale
- **Deformations**: Warp, Twist, Bend, Taper
- **Combinations**: Union, Intersect, Subtract, Smooth operations
- **Replication**: Array, Mirror, Circular patterns

### Effects
- **Color**: Brightness, Contrast, Saturation, Hue, Curves
- **Geometry**: Displace, Noise, Fractal perturbation
- **Filter**: Blur, Sharpen, Edge Detect, Posterize
- **Distortion**: Wave, Ripple, Fisheye, Swirl

### Animation
- **Timeline**: Keyframe animation with interpolation
- **Procedural**: LFO, Noise, Attractors
- **Logic**: If/Then/Else, Switches, Math operations
- **Control**: Parameter drivers and expressions

### Rendering
- **Materials**: PBR properties, textures, normal maps
- **Lighting**: Point, directional, spot lights
- **Camera**: Position, rotation, field of view
- **Post-Processing**: Bloom, DOF, color grading

## 🔧 Development

### Building
```bash
# Debug build
cargo build

# Release build (optimized)
cargo build --release

# Run tests
cargo test

# Run examples
cargo run --example node_editor_demo
cargo run --example fractal_demo
```

### Project Structure
```
src/
├── main.rs              # Application entry point
├── lib.rs               # Library interface
├── gui.rs               # Bevy GUI implementation
├── fractal/             # Fractal engine
│   ├── mod.rs
│   ├── engine.rs
│   ├── formulas.rs
│   ├── renderer.rs
│   └── types.rs
├── ui/                  # User interface
│   ├── mod.rs
│   ├── main.rs
│   ├── node_editor.rs
│   ├── theme.rs
│   └── fractal_ui.rs
├── scene/               # 3D scene management
│   └── mod.rs
├── animation/           # Animation system
│   ├── mod.rs
│   ├── timeline.rs
│   └── keyframe.rs
├── export/              # Export functionality
│   └── mod.rs
├── benchmark.rs         # Performance benchmarking
├── project.rs           # Project management
└── nodes.rs             # Node definitions

docs/                    # Documentation
examples/                # Example applications
assets/                  # Shaders and resources
```

## 🎯 Roadmap

### ✅ Completed
- [x] Fractal engine with distance estimation
- [x] GPU-accelerated rendering with WGPU
- [x] Node-based visual composition
- [x] Professional UI with Bevy + bevy_egui
- [x] 3D scene management
- [x] Basic animation system
- [x] Mesh and image export
- [x] Cross-platform support
- [x] Web deployment (WASM/WebGPU)
- [x] Migration from eframe to Bevy

### 🚧 In Progress
- [x] Advanced animation timeline
- [ ] Audio/MIDI integration
- [ ] Global illumination
- [ ] Volumetric effects

### 🔮 Future
- [ ] Plugin system for custom nodes
- [ ] Networked multi-user collaboration
- [ ] VR/AR fractal environments
- [ ] AI-assisted fractal generation

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Guidelines
1. Follow Rust coding standards
2. Add tests for new features
3. Update documentation
4. Ensure cross-platform compatibility

### Docs-First Workflow (Required)
- All code changes must be paired with documentation updates.
- CI includes a Docs Gate that fails PRs with code changes unless `docs/` files are updated.
- PRs must link updated docs and acceptance criteria (see `.github/PULL_REQUEST_TEMPLATE.md`).
- Commit messages should include a `Docs:` section or links to updated docs. Use `Doc-Exempt` only for trivial infra/typo commits.

#### Install Git Hooks (Windows)
- Install local hooks to enforce docs-first commits:
  - PowerShell: `powershell -ExecutionPolicy Bypass -File scripts/install_hooks.ps1`
  - Or: `pwsh -File scripts/install_hooks.ps1`
- Hooks installed: `pre-commit` (blocks code-only commits), `commit-msg` (nudges to include docs context).

#### Required Docs to Update
- At least one of:
  - `docs/DEVELOPMENT_PLAN.md`
  - `docs/DEVELOPMENT_ROADMAP.md`
  - `docs/GAP_ASSESSMENT.md`
  - Relevant module plans (e.g., `docs/VR_XR_INTEGRATION_PLAN.md`, `docs/GESTURE_INPUT_PLAN.md`)
- Reference: `docs/DOCS_MAINTENANCE.md` — Living Docs Protocol and automation.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Bevy Engine** - For the amazing Rust game engine
- **WebGPU/WGSL** - For modern GPU compute capabilities
- **Rust Community** - For the excellent ecosystem
- **Fractal Community** - For the inspiration and algorithms

## 📞 Contact

- **Repository**: [GitHub](https://github.com/compiling-org/modular-fractal-shader)
- **Issues**: [GitHub Issues](https://github.com/compiling-org/modular-fractal-shader/issues)
- **Discussions**: [GitHub Discussions](https://github.com/compiling-org/modular-fractal-shader/discussions)

---

**Made with ❤️ and lots of fractals**