## Why

Every command handler in the Questline engine has a dedicated test file—except `QuitCommandHandler`. Adding tests closes this coverage gap and ensures the quit behaviour works as expected if the handler evolves.

## What Changes

- Add a new test file `QuitCommandHandlerTests.cs` with tests verifying:
  - Handler returns a `QuitResult` type
  - Result has `Success = true`
  - Result message is "Goodbye!"

## Capabilities

### New Capabilities
- `quit-handler-test-coverage`: Unit tests verifying QuitCommandHandler behaviour matches expected quit semantics

### Modified Capabilities
<!-- None — this change adds tests only, no requirement changes to existing capabilities. -->

## Impact

- `tests/Questline.Tests/Engine/Handlers/QuitCommandHandlerTests.cs` (new file)
- No production code changes
