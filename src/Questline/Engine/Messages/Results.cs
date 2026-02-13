using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Results
{
    public record RoomViewed(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
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

    public record PlayerMoved(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
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

    public record ItemTaken(string Item)
        : CommandResult($"You pick up the {Item}.");

    public record ItemDropped(string Item)
        : CommandResult($"You drop the {Item}.");

    public record InventoryLoaded(IReadOnlyList<string> Items)
        : CommandResult(Items.Count == 0
            ? "You are not carrying anything."
            : $"You are carrying: {string.Join(", ", Items)}");

    public record CommandError(string Message) : CommandResult(Message, false);

    public record GameQuited() : CommandResult("Goodbye!");
}
