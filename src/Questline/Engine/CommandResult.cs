namespace Questline.Engine;

public record CommandResult(string Message, bool Success = true);

public record LookResult(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
    : CommandResult(FormatRoomDescription(RoomName, Description, Exits, Items))
{
    private static string FormatRoomDescription(
        string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items)
    {
        var parts = new List<string> { roomName, description };

        if (items.Count > 0)
        {
            parts.Add($"You can see: {string.Join(", ", items)}");
        }

        parts.Add($"Exits: {string.Join(", ", exits)}");

        return string.Join("\n", parts);
    }
}

public record MovedResult(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
    : CommandResult(FormatRoomDescription(RoomName, Description, Exits, Items))
{
    private static string FormatRoomDescription(
        string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items)
    {
        var parts = new List<string> { roomName, description };

        if (items.Count > 0)
        {
            parts.Add($"You can see: {string.Join(", ", items)}");
        }

        parts.Add($"Exits: {string.Join(", ", exits)}");

        return string.Join("\n", parts);
    }
}

public record ItemPickedUpResult(string ItemName)
    : CommandResult($"You pick up the {ItemName}.");

public record ItemDroppedResult(string ItemName)
    : CommandResult($"You drop the {ItemName}.");

public record InventoryResult(IReadOnlyList<string> Items)
    : CommandResult(Items.Count == 0
        ? "You are not carrying anything."
        : $"You are carrying: {string.Join(", ", Items)}");

public record ErrorResult(string ErrorMessage) : CommandResult(ErrorMessage, false);

public record QuitResult() : CommandResult("Goodbye!", true);
