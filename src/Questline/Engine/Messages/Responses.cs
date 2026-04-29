using Questline.Domain.Characters.Data;
using Questline.Domain.Players.Entity;
using Questline.Domain.Playthroughs.Data;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Responses
{
    public enum CharacterCreationStep
    {
        SelectClass,
        SelectRace,
        RollHitPoints,
        EnterName
    }

    public record GameStartedResponse : IResponse;

    public record GameQuitedResponse : IResponse;

    public record LoggedInResponse(Player Player, Resources.AdventureSummary[] Adventures) : IResponse;

    public record StartMenuResponse : IResponse;

    public record SavedPlaythroughsResponse(IReadOnlyList<PlaythroughSummary> Playthroughs) : IResponse;

    public record NoSavedGamesResponse : IResponse;

    public record NewGameResponse(Resources.AdventureSummary[] Adventures) : IResponse;

    public record NewAdventureSelectedResponse : IResponse;

    public record CharacterCreationOption(string Value, string Label);

    public record CharacterCreationResponse(
        CharacterCreationStep                   Step,
        string                                  Prompt,
        IReadOnlyList<CharacterCreationOption>? Options = null) : IResponse;

    public record CharacterCreationCompleteResponse(CharacterSummary Summary) : IResponse;

    public record AdventureStartedResponse(
        CharacterSummary      Character,
        string                RoomName,
        string                Description,
        IReadOnlyList<string> Exits,
        IReadOnlyList<string> Items,
        IReadOnlyList<string> LockedBarriers) : IResponse;

    public record PlayerMovedResponse(
        string                RoomName,
        string                Description,
        IReadOnlyList<string> Exits,
        IReadOnlyList<string> Items,
        IReadOnlyList<string> LockedBarriers) : IResponse;

    public record RoomDetailsResponse(
        string                RoomName,
        string                Description,
        IReadOnlyList<string> Exits,
        IReadOnlyList<string> Items,
        IReadOnlyList<string> LockedBarriers) : IResponse;

    public record ItemTakenResponse(string ItemName, string? CharacterName = null) : IResponse
    {
        public string Message => CharacterName is null
            ? $"You pick up the {ItemName}."
            : $"{CharacterName} picks up the {ItemName}.";
    }

    public record ItemDroppedResponse(string ItemName, string? CharacterName = null) : IResponse
    {
        public string Message => CharacterName is null
            ? $"You drop the {ItemName}."
            : $"{CharacterName} drops the {ItemName}.";
    }

    public record InventoryResponse(
        IReadOnlyList<(string CharacterName, IReadOnlyList<string> ItemNames)> PartyInventory) : IResponse
    {
        public string Message
        {
            get
            {
                var lines = PartyInventory.Select(p =>
                {
                    var items = p.ItemNames.Count == 0 ? "(empty)" : string.Join(", ", p.ItemNames);
                    return $"{p.CharacterName}: {items}";
                });
                return string.Join(Environment.NewLine, lines);
            }
        }
    }

    public record ExamineResponse(string Description) : IResponse;

    public record UseItemResponse(string ResultMessage) : IResponse;

    public record VersionResponse(string Version) : IResponse;
}
