# Documentation Maintenance

Purpose: Keep all documents accurate and reflective of the app’s evolution.

## Update Cadence
- Update docs at each sprint and release.
- Minimum set to update: DEVELOPMENT_PLAN.md, EVOLUTION_TRACKER.md, FEATURES_STATUS.md, CHANGES.md.

## Roles & Workflow
- Owner: Assign a doc lead per sprint.
- Process:
  - Propose changes via PR with clear summary.
  - Link to issues/tasks; tag affected sections.
  - Request review from core maintainers.

## Style & Structure
- Use concise headings and lists.
- Keep templates at top of each doc where applicable.
- Cross-link related docs for context.

## Templates
- Evolution entry, release checklist, snapshot test baselines.

## Verification
- CI checks for broken links and required document changes per release.
- Manual review: Ensure goals and statuses are consistent across docs.

## Living Docs Protocol (Enforced)
- Docs-first changes: Any code changes must be paired with doc updates.
- Automation:
  - Docs Gate CI: PRs with code changes fail if `docs/` is not updated.
  - Git hooks: pre-commit blocks code-only commits; commit-msg suggests including a `Docs:` section.
- Required updates on code change:
  - At least one of: `DEVELOPMENT_PLAN.md`, `DEVELOPMENT_ROADMAP.md`, `GAP_ASSESSMENT.md`, relevant module plans.
  - Acceptance criteria and validation steps recorded in PR.
- Exemptions:
  - Typos/infra-only changes: add `Doc-Exempt` in commit message and PR notes.