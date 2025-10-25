# Rust Fractal Shader Engine

A comprehensive, modular fractal shader system built with Rust and Bevy. Features advanced GPU-accelerated fractal rendering, ISF shader support, real-time audio/MIDI control, and a visual node-based composition interface.

## ⚠️ **WORK IN PROGRESS - Active Development & Integration**

### **Current Status & Recent Updates**

#### ✅ **Completed Features**
- **ISF Shader Support**: ISF shaders with multi-format conversion (ISF ↔ GLSL ↔ WGSL ↔ HLSL)
- **GPU Acceleration**: WebGPU/Vulkan/Metal/DX12 rendering with real-time performance
- **Node-Based Editor**: Visual composition interface with drag-and-drop functionality
- **Audio/MIDI Integration**: Real-time spectrum analysis and MIDI control mapping
- **Shader Conversion**: Automatic format translation between shader languages

#### 🔄 **In Development**
- **Web Deployment**: WASM/WebGPU support for browser-based editing
- **NFT Integration**: Filecoin + NEAR blockchain minting capabilities
- **Video Recording**: Export capabilities for fractal animations

#### 🚧 **Known Issues**
- **Web Deployment**: WASM compilation and WebGPU integration incomplete
- **NFT Minting**: Blockchain integration needs completion
- **Performance Optimization**: Memory usage with large shader collections
- **Cross-Platform Testing**: Comprehensive testing across all target platforms

#### 📈 **Next Development Phase**
1. **Complete Web Deployment**: Finish WASM and WebGPU implementation
2. **NFT Integration**: Implement Filecoin/NEAR minting functionality
3. **Video Recording**: Add export capabilities for fractal animations
4. **Advanced Editor**: Improve fractal editor interface
5. **Performance Benchmarking**: Optimize for large shader collections

[![Rust](https://img.shields.io/badge/rust-1.70%2B-orange)](https://www.rust-lang.org/)
[![Bevy](https://img.shields.io/badge/bevy-0.13-blue)](https://bevyengine.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## ✨ Features

### 🎨 **Shader Generation & Conversion**
- ** ISF Shaders** - Complete collection from Sleepless Monk
- **Multi-format Support** - ISF ↔ GLSL ↔ WGSL ↔ HLSL conversion
- **Real-time Conversion** - Automatic shader format translation
- **Custom Shader Support** - Load and convert user-created shaders

### 🎛️ **Interactive Control System**
- **16 Controllable Parameters** - Time, mouse, zoom, iterations, colors, audio, MIDI
- **Audio-Reactive** - Real-time spectrum analysis and beat detection
- **MIDI Control** - Full MIDI CC mapping and note triggers
- **Live Parameter Mapping** - Dynamic parameter assignment

### 🎯 **Node-Based Composition**
- **Visual Node Editor** - Drag-and-drop shader composition
- **Generator Nodes** - 2D/3D fractals, noise, mathematical functions
- **FX Nodes** - Color correction, geometry transforms, filters
- **Audio/MIDI Nodes** - Spectrum analysis, beat detection, MIDI mapping
- **Buffer Nodes** - Multi-pass rendering, feedback loops

### 🚀 **Performance & Compatibility**
- **GPU Acceleration** - WebGPU/Vulkan/Metal/DX12 support
- **Real-time Rendering** - 60+ FPS on modern GPUs
- **Cross-Platform** - Windows, macOS, Linux, Web (future)
- **Memory Efficient** - Optimized for large shader collections

## 📦 Installation

### From Crates.io
```bash
cargo add rust-fractal-shader-engine
```

### From Source
```bash
git clone https://github.com/compiling-org/rust-fractal-shader-engine
cd rust-fractal-shader-engine
cargo build --release
```

## 🎮 Usage

### Basic Usage
```rust
use rust_fractal_shader_engine::RustFractalShaderEngine;

let mut engine = RustFractalShaderEngine::new();

// Load ISF shaders from directory
let loaded = engine.load_isf_shaders_from_directory("assets/shaders/isf").unwrap();
println!("Loaded {} ISF shaders", loaded.len());

// Convert ISF to WGSL
let wgsl_code = engine.convert_isf_to_wgsl(isf_source).unwrap();
```

### Node Editor Demo
```bash
cargo run --example node_editor_demo
```

### ISF Shader Loader
```bash
cargo run --example load_isf_shaders
```

## 🏗️ Architecture

### Core Components
- **`RustFractalShaderEngine`** - Main engine with shader management
- **`ShaderConverter`** - Multi-format shader conversion
- **`NodeEditorPlugin`** - Visual node composition system
- **`AudioMidiSystem`** - Real-time audio and MIDI processing
- **`FractalUniforms`** - GPU parameter buffer structure

### Shader Pipeline
1. **Load** - Import ISF/GLSL shaders from files
2. **Parse** - Extract metadata and parameters
3. **Convert** - Transform to target format (WGSL/HLSL)
4. **Compose** - Combine via node-based interface
5. **Render** - GPU-accelerated real-time display

## 🎨 Node Types

### Generators
- **Fractals**: Mandelbrot, Julia, Burning Ship, Mandelbulb
- **Noise**: Perlin, Simplex, Voronoi, Flow noise
- **Mathematical**: L-Systems, Cellular Automata, Strange Attractors
- **Imported**: ISF shaders, Shadertoy conversions

### Effects
- **Color**: Brightness, Contrast, Saturation, Hue, Curves
- **Geometry**: Rotate, Scale, Warp, Kaleidoscope
- **Filter**: Blur, Sharpen, Edge Detect, Posterize
- **Distortion**: Wave, Ripple, Fisheye, Swirl

### Audio/MIDI
- **Analysis**: Spectrum, Beat Detection, Frequency Bands
- **Control**: MIDI CC Mapping, Note Triggers, Velocity

## 🔧 Development

### Prerequisites
- Rust 1.70+
- Vulkan/Metal/DX12 compatible GPU
- Audio device (optional, for audio features)
- MIDI device (optional, for MIDI control)

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
cargo run --example load_isf_shaders
```

### Project Structure
```
src/
├── lib.rs              # Main library interface
├── shader_converter.rs # Shader format conversion
├── shader_renderer.rs  # GPU rendering system
├── node_editor.rs      # Visual node editor
├── nodes.rs           # Node definitions and logic
├── audio.rs           # Audio/MIDI processing
├── ui.rs              # User interface components
└── main.rs            # Application entry point

examples/
├── node_editor_demo.rs    # Visual node editor demo
├── fractal_demo.rs         # Basic fractal rendering
└── load_isf_shaders.rs     # ISF shader loading demo

assets/shaders/isf/     # ISF shader collection (369 shaders)
```

## 🎯 Roadmap

### ✅ Completed
- [x] ISF shader loading and parsing
- [x] Shader format conversion (ISF ↔ WGSL/HLSL)
- [x] GPU uniform parameter system
- [x] Node-based visual composition
- [x] Audio analysis and MIDI control
- [x] Real-time fractal rendering

### 🚧 In Progress
- [x] Web deployment (WASM/WebGPU)
- [ ] NFT minting (Filecoin + NEAR)
- [ ] Video/audio recording
- [ ] Advanced fractal editor

### 🔮 Future
- [ ] Plugin system for custom nodes
- [ ] Networked multi-user collaboration
- [ ] VR/AR fractal environments
- [ ] Machine learning shader generation

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

- **ISF Community** - For the incredible shader collection
- **Bevy Engine** - For the amazing Rust game engine
- **WebGPU/WGSL** - For modern GPU compute capabilities
- **Rust Community** - For the excellent ecosystem

## 📞 Contact

- **Repository**: [GitHub](https://github.com/compiling-org/rust-fractal-shader-engine)
- **Issues**: [GitHub Issues](https://github.com/compiling-org/rust-fractal-shader-engine/issues)
- **Discussions**: [GitHub Discussions](https://github.com/compiling-org/rust-fractal-shader-engine/discussions)

---

**Made with ❤️ and lots of fractals**

## ISF Shader Integration

The engine can load ISF shaders from your Magic installation directory. ISF (Interactive Shader Format) is a JSON metadata format for GLSL shaders used in VJ software like VDMX and Resolume.

### Supported ISF Features

- Shader metadata parsing (categories, inputs, defaults, ranges)
- Parameter extraction and validation
- GLSL to WGSL/HLSL conversion
- Time-based animations
- Texture sampling
- Sleepless Monk fractal shader collection integration

## Shader Format Conversion

The engine provides utilities to convert between different shader formats:

- **ISF → WGSL**: For WebGPU and modern graphics APIs
- **ISF → HLSL**: For DirectX applications
- **GLSL → WGSL**: General GLSL to WebGPU conversion

## Future Development

- VS Code extension for live shader editing
- WebAssembly compilation for web-based editing
- Advanced fractal algorithms
- Real-time parameter interpolation
- Shader composition and chaining

## Contributing

This project is designed to be modular and extensible. Contributions for additional shader formats, fractal algorithms, or editor integrations are welcome.

## License

MIT License - see LICENSE file for details.
