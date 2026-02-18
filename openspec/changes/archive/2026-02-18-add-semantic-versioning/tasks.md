## 1. Version source of truth

- [x] 1.1 Create `Directory.Build.props` at the repo root with `<Version>0.4.0</Version>`. Verify `dotnet build` succeeds and the assembly `InformationalVersion` is `0.4.0`.

## 2. Version command

- [x] 2.1 Write tests for the version command handler: `version` returns `Questline v0.4.0`, `ver` alias parses to the same request. Add parser test for `version` and `ver`.
- [x] 2.2 Add `VersionQuery` request with `[Verbs("version", "ver")]` to `Engine/Messages/Requests.cs`. Add `VersionResponse` to `Engine/Messages/Responses.cs`.
- [x] 2.3 Implement `VersionQueryHandler` in `Engine/Handlers/` that reads the version from `Assembly.GetEntryAssembly()` or `Assembly.GetExecutingAssembly()` informational version. Register handler in `ServiceCollectionExtensions`.

## 3. CI release workflow

- [x] 3.1 Update `.github/workflows/ci.yml` release job: extract version from `Directory.Build.props`, replace `github.run_number` with the extracted version in `tag_name` and `name`.

## 4. Changelog

- [x] 4.1 Create `CHANGELOG.md` at the repo root with an `[Unreleased]` section and a `[0.4.0]` entry documenting Phase 0.4 (puzzles and barriers).

## 5. Verification

- [x] 5.1 `dotnet build` — compiles with no warnings.
- [x] 5.2 `dotnet test` — all existing and new tests pass.
