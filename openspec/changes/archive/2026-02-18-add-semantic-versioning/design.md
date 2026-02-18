## Context

GitHub releases currently tag with `v<run_number>` (e.g. v42), providing no information about the scope or compatibility of changes. The project is at Phase 0.4 with an incremental roadmap. Establishing SemVer now — while the audience is small — avoids a disruptive migration later.

The app is a self-contained CLI binary distributed via GitHub Releases. There is no NuGet package, no API contract, and no library consumers. The "public API" for SemVer purposes is the user-facing game commands and content format.

## Goals / Non-Goals

**Goals:**

- Single source of truth for the version number (`Directory.Build.props`)
- CI release workflow reads the version and uses it for the GitHub tag and release name
- A `version` command in-game that prints the current version
- A `CHANGELOG.md` at the repo root documenting notable changes per version
- Start at `0.4.0` to align with the Phase 0.4 milestone

**Non-Goals:**

- Automated version bumping from commit messages (manual bumps for now)
- Pre-release suffixes (`-alpha`, `-rc.1`) — deferred
- NuGet versioning or package publishing
- Retroactive changelog entries for v1–v42

## Decisions

### Decision 1: Version lives in Directory.Build.props

The version is defined once in `Directory.Build.props` at the repo root using the `<Version>` MSBuild property. This automatically sets `AssemblyVersion`, `FileVersion`, and `InformationalVersion` for all projects in the solution.

```xml
<Project>
  <PropertyGroup>
    <Version>0.4.0</Version>
  </PropertyGroup>
</Project>
```

**Why not a separate `version.json` or `.version` file?** `Directory.Build.props` is the idiomatic .NET approach. The version is automatically embedded in the assembly at build time, making it accessible at runtime via reflection without extra build steps or file-reading logic.

### Decision 2: CI reads version from Directory.Build.props

The release job in `ci.yml` extracts the version using a shell command:

```bash
VERSION=$(grep -oP '(?<=<Version>)[^<]+' Directory.Build.props)
```

This value replaces `github.run_number` in the `tag_name` and `name` fields of the GitHub Release step.

**Why not use a GitHub Action for XML parsing?** A simple `grep` is sufficient for a single well-known property in a small file. No additional dependencies needed.

### Decision 3: Runtime version via assembly metadata

The `version` command handler reads the version from `Assembly.GetExecutingAssembly().GetName().Version` or the `InformationalVersion` attribute. This avoids duplicating the version string in code — it comes directly from the build.

**Why `InformationalVersion`?** It preserves the full SemVer string (e.g. `0.4.0`) whereas `AssemblyVersion` truncates to major.minor.build.revision format.

### Decision 4: Start at 0.4.0

Per SemVer, versions below 1.0.0 signal "initial development — anything may change". Starting at `0.4.0` aligns with the Phase 0.4 milestone just completed. Future Phase 0 milestones bump the minor version (0.5.0, 0.6.0, 0.7.0). The 1.0.0 release coincides with the end of Phase 0 when the single-player experience is complete.

### Decision 5: CHANGELOG.md follows Keep a Changelog

The changelog uses the [Keep a Changelog](https://keepachangelog.com/) format with sections: Added, Changed, Removed, Fixed. Each release gets a `## [version] - date` heading. An `## [Unreleased]` section at the top collects in-progress changes.

## Risks / Trade-offs

- **Manual version bumps can be forgotten** → Mitigated by including the version bump as a PR checklist item. Automated bumping can be added later.
- **Existing releases use `v<run_number>` tags** → We leave them as-is. The new `v0.4.0+` tags are clearly different in format, avoiding confusion.
- **`grep` for version extraction is fragile** → Acceptable because the file format is simple and controlled. If `Directory.Build.props` becomes complex, switch to `dotnet msbuild -getProperty:Version`.
