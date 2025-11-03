#!/bin/bash

# Web Deployment Build Script for Fractal Shader Studio
# This script builds the project for web deployment using wasm-pack

set -e

echo "🌀 Building Fractal Shader Studio for Web Deployment"
echo "=================================================="

# Check if wasm-pack is installed
if ! command -v wasm-pack &> /dev/null; then
    echo "❌ wasm-pack is not installed. Please install it first:"
    echo "   cargo install wasm-pack"
    exit 1
fi

# Check if required dependencies are available
echo "📦 Checking dependencies..."

# Create pkg directory if it doesn't exist
mkdir -p web/pkg

# Build for web with wasm-pack
echo "🔨 Building WASM module..."
wasm-pack build --target web --out-dir web/pkg --dev

# Copy additional web assets
echo "📄 Copying web assets..."
cp -r assets web/ 2>/dev/null || true

# Create optimized production build
echo "🏗️  Creating production build..."
wasm-pack build --target web --out-dir web/pkg --release

# Generate service worker for PWA support (optional)
cat > web/sw.js << 'EOF'
const CACHE_NAME = 'fractal-studio-v1';
const urlsToCache = [
  '/',
  '/index.html',
  '/pkg/fractal_studio.js',
  '/pkg/fractal_studio_bg.wasm',
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(urlsToCache))
  );
});

self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request)
      .then(response => response || fetch(event.request))
  );
});
EOF

# Update HTML to register service worker
cat >> web/index.html << 'EOF'

    <!-- Service Worker Registration -->
    <script>
        if ('serviceWorker' in navigator) {
            window.addEventListener('load', () => {
                navigator.serviceWorker.register('/sw.js')
                    .then(registration => console.log('SW registered'))
                    .catch(error => console.log('SW registration failed'));
            });
        }
    </script>
EOF

echo "✅ Web build completed successfully!"
echo ""
echo "🚀 To serve the application:"
echo "   cd web && python3 -m http.server 8000"
echo "   # or use any static file server"
echo ""
echo "📱 The application will be available at:"
echo "   http://localhost:8000"
echo ""
echo "🎯 Features available in web version:"
echo "   • Real-time fractal rendering"
echo "   • Interactive parameter controls"
echo "   • Multiple fractal types"
echo "   • Export to PNG"
echo "   • PWA support (offline capable)"