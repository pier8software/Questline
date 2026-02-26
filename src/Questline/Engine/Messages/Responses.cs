using Questline.Domain.Characters.Data;
using Questline.Domain.Players.Entity;
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

    public record ItemTakenResponse(string ItemName) : IResponse;

    public record ItemDroppedResponse(string ItemName) : IResponse;

    public record PlayerInventoryResponse(IReadOnlyList<string> Items) : IResponse;

    public record ExamineResponse(string Description) : IResponse;

    public record UseItemResponse(string ResultMessage) : IResponse;

    public record VersionResponse(string Version) : IResponse;
}
