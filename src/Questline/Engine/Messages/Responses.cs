using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Responses
{
    public record UseItemResponse : IResponse
    {
        private UseItemResponse(string message) { Message = message; }

        public string Message { get; }

        public static UseItemResponse Success(string message) => new(message);
        public static UseItemResponse Error(string errorMessage) => new(errorMessage);
    }

    public record ExamineResponse : IResponse
    {
        private ExamineResponse(string message) { Message = message; }

        public string Message { get; }

        public static ExamineResponse Success(string description) => new(description);
        public static ExamineResponse Error(string errorMessage) => new(errorMessage);
    }

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
            IReadOnlyList<string> items,
            IReadOnlyList<string>? lockedBarriers = null) =>
            new(FormatRoomDescription(roomName, description, exits, items, lockedBarriers));

        public static PlayerMovedResponse Error(string errorMessage) => new(errorMessage);

        private static string FormatRoomDescription(
            string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items,
            IReadOnlyList<string>? lockedBarriers = null)
        {
            var parts = new List<string> { roomName, description };

            if (items.Count > 0)
            {
                parts.Add($"You can see: {string.Join(", ", items)}");
            }

            if (lockedBarriers is { Count: > 0 })
            {
                parts.AddRange(lockedBarriers);
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

    public record RoomDetailsResponse : IResponse
    {
        private RoomDetailsResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }

        private static string FormatRoomDescription(
            string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items,
            IReadOnlyList<string>? lockedBarriers = null)
        {
            var parts = new List<string> { roomName, description };

            if (items.Count > 0)
            {
                parts.Add($"You can see: {string.Join(", ", items)}");
            }

            if (lockedBarriers is { Count: > 0 })
            {
                parts.AddRange(lockedBarriers);
            }

            parts.Add($"Exits: {string.Join(", ", exits)}");

            return string.Join("\n", parts);
        }

        public static RoomDetailsResponse Success(string name, string description, List<string> exits,
            List<string> items, List<string>? lockedBarriers = null) =>
            new(FormatRoomDescription(name, description, exits, items, lockedBarriers));

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
    public record VersionResponse(string Version) : IResponse
    {
        public string Message => $"Questline v{Version}";
    }

    public record GameQuited : IResponse
    {
        public string Message => "Goodbye!";
    }
}
