using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Messages;

public static class Responses
{
    public record RoomDetailsResponse : IResponse
    {
        private RoomDetailsResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }

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

        public static RoomDetailsResponse Success(string name, string description, List<string> exits,
            List<string> items) => new(FormatRoomDescription(name, description, exits, items));

        public static RoomDetailsResponse Error(string errorMessage) => new(errorMessage);
    }

    public record ItemTakenResponse : IResponse
    {
        private ItemTakenResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static ItemTakenResponse Success(string item) => new($"You pick up the {item}.");
        public static ItemTakenResponse Error(string errorMessage) => new(errorMessage);
    }
}
