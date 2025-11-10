#!/usr/bin/env bash
set -euo pipefail

# 3D Development Plan Validation Script
# - Builds binaries
# - Renders multiple 3D fractal snapshots with tuned parameters
# - Captures device/state JSON and logs
# - Summarizes errors/warnings to guide next actions

PROJECT_ROOT=$(cd "$(dirname "${BASH_SOURCE[0]}")"/.. && pwd)
cd "$PROJECT_ROOT"

TS=$(date +%Y%m%d-%H%M%S)
OUT_DIR="snapshots/$TS"
mkdir -p "$OUT_DIR"

echo "[Phase] Build all binaries"
cargo build --bins 2>&1 | tee "$OUT_DIR/build_log.txt"

render() {
  local name="$1"; shift
  local json_payload="$1"; shift
  echo "[Render] $name"
  cargo run --bin fractal-snapshot -- "$json_payload" 2>&1 | tee "$OUT_DIR/${name}.log"
  local png="$OUT_DIR/${name}.png"
  if [[ ! -s "$png" ]]; then
    echo "[Error] Snapshot not produced or empty: $png" | tee -a "$OUT_DIR/errors.txt"
  fi
}

echo "[Phase] Render 3D snapshots with tuned parameters"

# Mandelbulb
json_mandelbulb=$(cat <<EOF
{
  "width":1280,
  "height":720,
  "formula":"mandelbulb",
  "max_iterations":900,
  "bailout":8.0,
  "power":8.0,
  "scale":1.0,
  "position":[0.25,-0.1,-1.1],
  "rotation":[0.25,0.2,0.0],
  "color_saturation":1.1,
  "output":"$OUT_DIR/mandelbulb.png",
  "state_out":"$OUT_DIR/mandelbulb_state.json"
}
EOF
)
render "mandelbulb" "$json_mandelbulb"

# Mandelbox
json_mandelbox=$(cat <<EOF
{
  "width":1280,
  "height":720,
  "formula":"mandelbox",
  "max_iterations":800,
  "bailout":8.0,
  "scale":1.8,
  "position":[0.1,0.2,-1.0],
  "rotation":[0.2,0.15,0.0],
  "color_saturation":1.05,
  "output":"$OUT_DIR/mandelbox.png",
  "state_out":"$OUT_DIR/mandelbox_state.json"
}
EOF
)
render "mandelbox" "$json_mandelbox"

# Quaternion Julia
json_quatjulia=$(cat <<EOF
{
  "width":1280,
  "height":720,
  "formula":"quaternion_julia",
  "max_iterations":700,
  "bailout":8.0,
  "scale":1.0,
  "position":[0.0,0.0,-1.2],
  "rotation":[0.0,0.0,0.0],
  "color_saturation":1.0,
  "output":"$OUT_DIR/quaternion_julia.png",
  "state_out":"$OUT_DIR/quaternion_julia_state.json"
}
EOF
)
render "quaternion_julia" "$json_quatjulia"

echo "[Phase] Summarize errors/warnings"
{
  echo "--- Build warnings (top 20) ---"
  grep -Ei 'warning' "$OUT_DIR/build_log.txt" | head -n 20 || true
  echo "--- Render errors ---"
  grep -Ei 'error|panic|nan|overflow' "$OUT_DIR"/*.log || true
} | tee "$OUT_DIR/summary.txt"

echo "[Done] Snapshots in $OUT_DIR"