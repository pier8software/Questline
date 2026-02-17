using Questline.Framework.Mediator;

namespace Questline.Domain.Players.Messages;

public static class Responses
{
    public record PlayerMovedResponse
        : IResponse
    {
        private PlayerMovedResponse(string message)
        {
            Message = message;
        }

        public static PlayerMovedResponse Success(
            string roomName,
            string description,
            IReadOnlyList<string> exits,
            IReadOnlyList<string> items) => new(FormatRoomDescription(roomName, description, exits, items));

        public static PlayerMovedResponse Error(string errorMessage) => new(errorMessage);

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

        public string Message { get; }
    }

    public record ItemDroppedResponse(string Item)
        : IResponse
    {
        public string Message => $"You drop the {Item}.";
    }

    public record PlayerInventoryResponse(IReadOnlyList<string> Items)
        : IResponse
    {
        public string Message => Items.Count == 0
            ? "You are not carrying anything."
            : $"You are carrying: {string.Join(", ", Items)}";
    }
}
