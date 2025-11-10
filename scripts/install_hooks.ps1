Param(
    [string]$RepoRoot = "$(Resolve-Path .)"
)

$hooksDir = Join-Path $RepoRoot ".git/hooks"
$sourceDir = Join-Path $RepoRoot "scripts/git-hooks"

if (!(Test-Path $hooksDir)) {
    Write-Error ".git/hooks directory not found. Initialize git before installing hooks."
    exit 1
}

$preCommit = Join-Path $sourceDir "pre-commit"
$commitMsg = Join-Path $sourceDir "commit-msg"

Copy-Item $preCommit -Destination (Join-Path $hooksDir "pre-commit") -Force
Copy-Item $commitMsg -Destination (Join-Path $hooksDir "commit-msg") -Force

# Try to set executable bit for Git Bash environments
try {
    & git update-index --chmod=+x ".git/hooks/pre-commit"
    & git update-index --chmod=+x ".git/hooks/commit-msg"
} catch {
    Write-Host "Note: Could not set executable bit; Git Bash should still run these hooks on Windows."
}

Write-Host "Git hooks installed: pre-commit and commit-msg"