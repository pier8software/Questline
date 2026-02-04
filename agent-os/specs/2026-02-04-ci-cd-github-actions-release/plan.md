# CI/CD Pipeline with GitHub Actions

## Summary

Set up CI/CD pipeline using GitHub Actions to build, test, and deploy Questline to GitHub Releases. Includes creating a starter .NET console project.

## Spec Folder

`agent-os/specs/2026-02-04-ci-cd-github-actions-release/`

---

## Task 1: Save Spec Documentation

Create spec folder with:
- `plan.md` — Copy of this plan
- `shape.md` — Shaping notes capturing scope and decisions
- `standards.md` — Full content of ci-cd and coding-style standards
- `references.md` — Note that no existing references apply

---

## Task 2: Create .NET Console Project

### Files to create:

**`src/Questline/Questline.csproj`**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

**`src/Questline/Program.cs`**
```csharp
Console.WriteLine("Welcome to Questline!");
```

**Update `Questline.slnx`** to include the project:
```xml
<Solution>
  <Project Path="src/Questline/Questline.csproj" />
</Solution>
```

---

## Task 3: Create EditorConfig

**`.editorconfig`** at repo root — use content from `agent-os/standards/global/coding-style.md`

---

## Task 4: Create GitHub Actions Workflow

**`.github/workflows/ci.yml`**

### Workflow structure:

```yaml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal

  release:
    needs: build-and-test
    if: github.ref == 'refs/heads/main' && github.event_name == 'push'
    runs-on: ubuntu-latest
    permissions:
      contents: write
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      # Publish for all platforms
      - name: Publish win-x64
        run: dotnet publish src/Questline -c Release -r win-x64 --self-contained -o publish/win-x64

      - name: Publish linux-x64
        run: dotnet publish src/Questline -c Release -r linux-x64 --self-contained -o publish/linux-x64

      - name: Publish linux-arm64
        run: dotnet publish src/Questline -c Release -r linux-arm64 --self-contained -o publish/linux-arm64

      - name: Publish osx-x64
        run: dotnet publish src/Questline -c Release -r osx-x64 --self-contained -o publish/osx-x64

      - name: Publish osx-arm64
        run: dotnet publish src/Questline -c Release -r osx-arm64 --self-contained -o publish/osx-arm64

      # Create zip archives
      - name: Create archives
        run: |
          cd publish
          zip -r questline-win-x64.zip win-x64
          tar -czf questline-linux-x64.tar.gz linux-x64
          tar -czf questline-linux-arm64.tar.gz linux-arm64
          tar -czf questline-osx-x64.tar.gz osx-x64
          tar -czf questline-osx-arm64.tar.gz osx-arm64

      # Create GitHub Release
      - name: Create Release
        uses: softprops/action-gh-release@v2
        with:
          tag_name: v${{ github.run_number }}
          name: Release v${{ github.run_number }}
          files: |
            publish/questline-win-x64.zip
            publish/questline-linux-x64.tar.gz
            publish/questline-linux-arm64.tar.gz
            publish/questline-osx-x64.tar.gz
            publish/questline-osx-arm64.tar.gz
          generate_release_notes: true
```

---

## Files Modified/Created

| File | Action |
|------|--------|
| `agent-os/specs/2026-02-04-ci-cd-github-actions-release/plan.md` | Create |
| `agent-os/specs/2026-02-04-ci-cd-github-actions-release/shape.md` | Create |
| `agent-os/specs/2026-02-04-ci-cd-github-actions-release/standards.md` | Create |
| `agent-os/specs/2026-02-04-ci-cd-github-actions-release/references.md` | Create |
| `src/Questline/Questline.csproj` | Create |
| `src/Questline/Program.cs` | Create |
| `Questline.slnx` | Update |
| `.editorconfig` | Create |
| `.github/workflows/ci.yml` | Create |

---

## Verification

1. **Local build**: Run `dotnet build` to verify project compiles
2. **Local test**: Run `dotnet test` (will pass with no tests)
3. **PR workflow**: Create a PR to trigger build-and-test job
4. **Release workflow**: Merge to main to trigger release job and verify GitHub Release is created with all platform binaries
