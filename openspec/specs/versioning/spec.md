## Purpose

Defines how the application version is tracked, displayed at runtime, and used in CI releases.

## Requirements

### Requirement: Version defined in Directory.Build.props

The application version SHALL be defined as a single `<Version>` property in `Directory.Build.props` at the repository root, following Semantic Versioning (MAJOR.MINOR.PATCH).

#### Scenario: Version property exists

- **WHEN** the solution is built
- **THEN** `Directory.Build.props` SHALL contain a `<Version>` element with a valid SemVer string

#### Scenario: All projects inherit the version

- **WHEN** the solution is built
- **THEN** the main project assembly SHALL have its `InformationalVersion` set to the value from `Directory.Build.props`

### Requirement: GitHub releases use SemVer tags

The CI release workflow SHALL create GitHub Releases tagged with `v<MAJOR>.<MINOR>.<PATCH>` derived from the version in `Directory.Build.props`.

#### Scenario: Release tagged with version from props

- **WHEN** a push to `main` triggers the release workflow
- **THEN** the GitHub Release SHALL be tagged with the version from `Directory.Build.props` prefixed with `v` (e.g. `v0.4.0`)

#### Scenario: Release name matches version

- **WHEN** a GitHub Release is created
- **THEN** the release name SHALL be `Release v<version>` (e.g. `Release v0.4.0`)

### Requirement: Version command displays current version

The `version` command SHALL display the application's current version at runtime.

#### Scenario: Player types version command

- **WHEN** the player executes `version`
- **THEN** the response SHALL contain the current SemVer version string (e.g. `Questline v0.4.0`)

#### Scenario: Version alias

- **WHEN** the player executes `ver`
- **THEN** the response SHALL be identical to executing `version`

### Requirement: Changelog tracks notable changes

A `CHANGELOG.md` file SHALL exist at the repository root, following the Keep a Changelog format, documenting notable changes for each version.

#### Scenario: Changelog has current version entry

- **WHEN** a new version is released
- **THEN** `CHANGELOG.md` SHALL contain a heading for that version with the release date

#### Scenario: Changelog has Unreleased section

- **WHEN** `CHANGELOG.md` is read
- **THEN** it SHALL contain an `[Unreleased]` section at the top for in-progress changes
