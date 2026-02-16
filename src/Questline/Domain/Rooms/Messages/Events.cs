using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Messages;

public static class Events
{
    public record RoomViewed(
        string RoomName,
        string Description,
        IReadOnlyList<string> Exits,
        IReadOnlyList<string> Items)
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


    public record RoomItemTaken(string Item)
        : CommandResult($"You pick up the {Item}.");
}
