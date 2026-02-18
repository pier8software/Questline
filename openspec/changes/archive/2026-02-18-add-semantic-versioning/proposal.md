## Why

GitHub releases currently use `v<run_number>` (e.g. v42, v43) which conveys no information about what changed or how significant it was. Adopting Semantic Versioning (SemVer) gives users and contributors a clear signal about compatibility and scope of each release. The project is pre-1.0 (Phase 0 roadmap), making now the right time to establish the convention before a wider audience arrives.

## What Changes

- Add a `Directory.Build.props` at the repo root with a `<Version>` property set to the current SemVer (starting at `0.4.0` to align with the Phase 0.4 milestone just completed)
- Add a `CHANGELOG.md` following the Keep a Changelog format to document notable changes per version
- Update the CI release workflow (`ci.yml`) to read the version from `Directory.Build.props` and use it for the GitHub release tag (`v0.4.0`) instead of `github.run_number`
- Add a version command (`version` / `ver`) to the game that prints the current version at runtime

## Non-goals

- Automated version bumping (e.g. conventional commits tooling) — version is manually maintained for now
- NuGet packaging or publishing — the app is distributed as self-contained binaries
- Pre-release suffix logic (e.g. `-alpha`, `-beta`) — can be added later if needed

## Capabilities

### New Capabilities

- `versioning`: Defines how the application version is tracked, displayed, and used in releases

### Modified Capabilities

_(none — no existing spec requirements are changing)_

## Impact

- **Build**: New `Directory.Build.props` sets `<Version>` for all projects in the solution
- **CI**: `ci.yml` release job reads version from props file and tags releases accordingly
- **CLI**: New `version` command added to the game engine (request, handler, registration)
- **Docs**: New `CHANGELOG.md` at repo root
