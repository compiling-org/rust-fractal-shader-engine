arr# Changelog

All notable changes to the Rust Fractal Shader Engine will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial release of the Rust Fractal Shader Engine
- Complete ISF shader support with 369 shaders from Magic directory
- Multi-format shader conversion (ISF ↔ WGSL ↔ HLSL)
- GPU-accelerated rendering using Bevy engine
- Real-time parameter control system (16 controllable parameters)
- Audio analysis and MIDI control integration
- Visual node-based composition interface
- Modular shader system for fractal generation
- WebGPU/WebAssembly support preparation

### Changed
- **Bevy 0.17 Migration**: Updated all UI components, rendering systems, and API calls for Bevy 0.17 compatibility
- **UI System Updates**: Migrated from NodeBundle/TextBundle to Node/Text components with separate styling
- **Color API**: Updated Color::rgb/rgba usage to Color::srgb/srgba functions
- **Window Query**: Fixed Window query handling to unwrap Result types
- **Rendering**: Updated PbrBundle to Mesh3d/MeshMaterial3d components
- **Camera**: Migrated Camera2dBundle to Camera2d component
- **Math**: Replaced deprecated angle_between with angle_to method

### Features
- **Shader Loading**: Automatic ISF shader discovery and loading
- **Format Conversion**: Real-time shader format translation
- **Interactive Controls**: Mouse, keyboard, audio, and MIDI parameter mapping
- **Node Editor**: Visual drag-and-drop shader composition
- **GPU Rendering**: High-performance real-time fractal display
- **Cross-Platform**: Windows, macOS, Linux support

### Technical
- Bevy 0.17 game engine integration (migrated from 0.13)
- WebGPU/Vulkan/Metal/DX12 rendering backends
- Real-time audio spectrum analysis
- MIDI CC parameter mapping
- Node-based shader composition system
- ISF metadata parsing and validation

## [0.1.0] - 2025-10-23

### Added
- Core fractal shader engine architecture
- ISF shader parsing and metadata extraction
- Basic shader format conversion utilities
- Bevy application setup with GPU rendering
- Initial project structure and dependencies
- Basic fractal parameter system
- Audio/MIDI integration framework
- Node editor foundation

### Changed
- Initial implementation of all core systems

### Technical Details
- Rust 1.70+ compatibility
- Bevy 0.13 integration
- GPU-accelerated rendering pipeline
- Modular architecture for extensibility
- Real-time parameter updates
- Cross-platform compatibility

---

**Legend:**
- `Added` for new features
- `Changed` for changes in existing functionality
- `Deprecated` for soon-to-be removed features
- `Removed` for now removed features
- `Fixed` for any bug fixes
- `Security` in case of vulnerabilities