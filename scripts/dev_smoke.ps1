<#
  Dev Smoke Script (Windows PowerShell)
  - Runs quick checks to catch UI/code errors and minimize runtime
  - Captures outputs to build_log.txt for review

  Usage:
    pwsh -File scripts/dev_smoke.ps1

  Notes:
    - Requires Rust toolchain and cargo in PATH
    - Non-interactive; will not launch GUI windows
#>

$ErrorActionPreference = "Stop"
$Log = "build_log.txt"

function Write-Log {
  param([string]$msg)
  $timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
  "$timestamp $msg" | Tee-Object -FilePath $Log -Append
}

function Run-Step {
  param([string]$name, [string]$cmd)
  Write-Host "== $name ==" -ForegroundColor Cyan
  Write-Log "== $name =="
  try {
    $output = Invoke-Expression $cmd 2>&1
    $exit = $LASTEXITCODE
    if ($null -ne $output) { $output | Out-String | Tee-Object -FilePath $Log -Append | Out-Null }
    if ($exit -ne 0) {
      Write-Host "FAILED: $name (Exit $exit)" -ForegroundColor Red
      Write-Log "FAILED: $name (Exit $exit)"
      return $false
    } else {
      Write-Host "OK: $name" -ForegroundColor Green
      Write-Log "OK: $name"
      return $true
    }
  } catch {
    Write-Host "ERROR: $name $_" -ForegroundColor Red
    Write-Log "ERROR: $name $_"
    return $false
  }
}

# Init log
Remove-Item $Log -ErrorAction SilentlyContinue
Write-Log "Starting dev smoke at $(Get-Date)"

$success = $true

# 1) Cargo checks/builds (fast, no GUI)
$success = $success -and (Run-Step "Cargo check (workspace)" "cargo check")
$success = $success -and (Run-Step "Cargo build (bins)" "cargo build --bins")

# 2) Snapshot CLI (quick compute validation, no window)
$success = $success -and (Run-Step "Run fractal-snapshot (default)" "cargo run --bin fractal-snapshot")

# 3) Optional: examples if present (non-blocking quick test)
try {
  $examples = (cargo run --example list 2>$null) # harmless probe; ignore failures
} catch {}

Write-Log "Smoke complete: $success"
if (-not $success) { exit 1 } else { exit 0 }