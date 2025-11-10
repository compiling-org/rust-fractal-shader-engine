# Release Plan

Purpose: Define release cadence, criteria, and artifacts to consistently deliver stable versions across desktop and web.

## Versioning
- Semantic Versioning: MAJOR.MINOR.PATCH
- Desktop and Web releases share version numbers when features align.

## Release Cadence
- Target: Minor releases every 4–6 weeks; patches as needed.
- Hotfixes for critical regressions.

## Release Criteria
- All blocking issues resolved.
- Snapshot tests pass; MSE < threshold against baselines.
- Performance targets met for default quality (e.g., <=33ms @ 512×512).
- Docs updated: DEVELOPMENT_PLAN.md, EVOLUTION_TRACKER.md, FEATURES_STATUS.md, CHANGES.md.
- Demo presets load cleanly; UI actions smoke-tested.

## Channels
- Desktop: Windows/macOS/Linux binaries.
- Web: WebGPU build; fallback warning for non-supported browsers.

## Release Checklist (Template)
- [ ] Finalize scope and version bump.
- [ ] Run full test suite and performance benchmarks.
- [ ] Update docs and changelog.
- [ ] Build artifacts; verify signatures/hashes.
- [ ] Publish and tag release.
- [ ] Announce with highlights and screenshots.

## Release Notes Template
- Title: vX.Y.Z — Highlights
- Features:
  - ...
- Improvements:
  - ...
- Fixes:
  - ...
- Performance:
  - ...
- Known Issues:
  - ...
- Docs Updated:
  - DEVELOPMENT_PLAN.md, EVOLUTION_TRACKER.md, FEATURES_STATUS.md, CHANGES.md