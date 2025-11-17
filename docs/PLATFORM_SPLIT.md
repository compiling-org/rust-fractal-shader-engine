# Platform Split: Desktop vs Web/WASM

IMPORTANT: The web/WASM edition is external-only. All web code has been archived under `archive/blockchain_nft_web/` in this repo and will be migrated into the NUWE blockchain/NFT project. Do not build or modify web code here.

## Scope
- Desktop app: native GUI, GPU rendering, node editor, export tools.
- Web/WASM app: browser UI, WASM bindings, WebGL/WebGPU pipeline, NFT integrations.

## Where the Web/WASM Code Lives
- NUWE blockchain/NFT monorepo provides the web edition and deployment pipeline.
- Archived sources in this repo: `archive/blockchain_nft_web/` (mirrors original paths for migration).
- Reference docs and links are maintained in NUWE. This repo points to NUWE rather than duplicating web code.

## Relocation Plan (Archived in `archive/blockchain_nft_web/`)
The following files were moved into `archive/blockchain_nft_web/` and are no longer active in the desktop app:
- `src/web/mod.rs` — WebGL/WebGPU bindings and helpers (wasm32 target gated).
- `src/web_canvas2d.rs` — Canvas2D fallback renderer and utilities.
- `src/web_interface.rs` — Web-facing API and demo harness.
- `web/index.html` — Demo shell for browser testing.
- `examples/web_demo.html` — Example page for WASM initialization.
- `scripts/export_web_bundle.ps1` — Web bundle export helper.
- `scripts/build_web.sh` — Web build script using `wasm-pack`.

## Build Flags and Gating
- Desktop builds ignore web modules by default.
- Web modules compile only under `wasm32` AND when feature `web` is enabled (`#[cfg(all(target_arch = "wasm32", feature = "web"))]`).
- The `web` feature exists in `Cargo.toml` but is disabled by default; enable it only in the NUWE repo.
- No web servers or browser previews are started from the desktop app.

## Developer Guidance
- To work on the web edition, use the NUWE repo and follow its instructions.
- Do not enable web features in this repository. Prefer NUWE for any web/WASM testing or builds.

## Next Steps
- Create a migration PR in NUWE to import modules listed above.
- Wire DE→Color params and shader uniforms consistently across desktop and web.
- Keep README’s Web Deployment note pointing to this document and NUWE.
- Use `scripts/export_web_bundle.ps1` to assemble files for copying into NUWE.