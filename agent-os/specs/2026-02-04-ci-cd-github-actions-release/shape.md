# Shape: CI/CD Pipeline with GitHub Actions

## Scope

Set up a complete CI/CD pipeline for the Questline project using GitHub Actions, deploying to GitHub Releases instead of AWS infrastructure.

## Decisions

### Deployment Target: GitHub Releases
- Chose GitHub Releases over AWS ECS/Fargate for initial deployment
- Simpler setup with no cloud infrastructure dependencies
- Self-contained executables for all major platforms
- Users can download and run directly

### Build Targets
- Windows x64
- Linux x64
- Linux ARM64
- macOS x64
- macOS ARM64

### Versioning Strategy
- Using `github.run_number` for automatic incrementing version numbers
- Format: `v1`, `v2`, `v3`, etc.
- Simple and automatic - no manual version bumping required

### Archive Formats
- Windows: ZIP archive
- Linux/macOS: tar.gz archives
- Standard formats for each platform

### Workflow Triggers
- Push to `main`: Full pipeline (build, test, release)
- Pull requests to `main`: Build and test only (no release)

## Out of Scope

- AWS deployment (ECS, ECR, Fargate)
- Docker containerization
- Database migrations
- Environment-specific configurations
- Semantic versioning (future enhancement)
