#!/bin/bash

# Build script for web deployment
# This script builds the fractal generator for web deployment using WASM

set -e

echo "🌀 Building Fractal Shader Studio for Web Deployment"
echo "=================================================="

# Check if wasm-pack is installed
if ! command -v wasm-pack &> /dev/null; then
    echo "❌ wasm-pack is not installed. Please install it first:"
    echo "   curl https://rustwasm.github.io/wasm-pack/installer/init.sh -sSf | sh"
    exit 1
fi

# Check if we have the wasm32 target
if ! rustup target list --installed | grep -q wasm32-unknown-unknown; then
    echo "📦 Installing wasm32 target..."
    rustup target add wasm32-unknown-unknown
fi

# Create pkg directory if it doesn't exist
mkdir -p web/pkg

# Build for web with optimizations
echo "🔨 Building WASM module..."
wasm-pack build --target web --out-dir web/pkg --release --features web

# Optimize the WASM file
if command -v wasm-opt &> /dev/null; then
    echo "⚡ Optimizing WASM file..."
    wasm-opt -Oz web/pkg/modular_fractal_shader_bg.wasm -o web/pkg/modular_fractal_shader_bg.wasm
else
    echo "⚠️  wasm-opt not found. Install binaryen for better optimization."
fi

# Copy additional web assets
echo "📋 Copying web assets..."
cp -r assets web/ 2>/dev/null || true

echo "✅ Web build completed successfully!"
echo ""
echo "🚀 To serve the application:"
echo "   cd web && python3 -m http.server 8000"
echo "   Open http://localhost:8000 in your browser"
echo ""
echo "📁 Build output is in: web/pkg/"
echo "🌐 Web interface: web/index.html"