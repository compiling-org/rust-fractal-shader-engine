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

### Node Editor Demo
```bash
cargo run --example node_editor_demo
```

### Fractal Rendering Demo
```bash
cargo run --example fractal_demo
```

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