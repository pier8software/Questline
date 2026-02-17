using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Messages;

public static class Responses
{
    public record RoomDetailsResponse(string RoomName, string RoomDescription, List<string> Exits, List<string> Items)
        : IResponse
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

        public string Message => FormatRoomDescription(RoomName, RoomDescription, Exits, Items);
    }

    public record ItemTakenResponse : IResponse
    {
        private ItemTakenResponse(string message)
        {
            Message = message;
        }

        public static ItemTakenResponse Success(string item) => new($"You pick up the {item}.");
        public static ItemTakenResponse Error(string errorMessage) => new(errorMessage);

        public string Message { get; }
    }
}
