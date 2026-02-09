namespace Questline.Engine;

public record CommandResult(string Message, bool Success = true);

public record LookResult(string RoomName, string Description, IReadOnlyList<string> Exits)
    : CommandResult($"{RoomName}\n{Description}\nExits: {string.Join(", ", Exits)}");

public record MovedResult(string RoomName, string Description, IReadOnlyList<string> Exits)
    : CommandResult($"{RoomName}\n{Description}\nExits: {string.Join(", ", Exits)}");

public record ErrorResult(string ErrorMessage) : CommandResult(ErrorMessage, false);

public record QuitResult() : CommandResult("Goodbye!", true);
