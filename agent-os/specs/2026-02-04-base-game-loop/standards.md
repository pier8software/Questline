# Base Game Loop — Standards

## Applicable Standards

### C# Coding Standards
- Use records for immutable data structures (GameState, Command, CommandResult)
- Use init-only properties for record fields
- Enable nullable reference types
- Use file-scoped namespaces
- Use primary constructors where appropriate

### Testing Standards
- Use xUnit for test framework
- Use Shouldly for assertions
- One test class per production class
- Test method naming: `MethodName_Scenario_ExpectedBehavior`
- Arrange-Act-Assert pattern

### Project Standards
- Solution file at repo root
- Source code in `src/` directory
- Tests in `tests/` directory
- Test project mirrors source project structure
