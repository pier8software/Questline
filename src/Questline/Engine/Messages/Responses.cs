using Questline.Domain.Characters.Data;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Responses
{
    public record LoginResponse(string PlayerName) : IResponse;

    public record GetAdventuresResponse(Resources.AdventureSummary[] Adventures) : IResponse;

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

    public record GameStartedResponse(
        CharacterSummary      Character,
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

    public record GameQuitResponse : IResponse;

    public enum CharacterCreationStep
    {
        SelectClass,
        SelectRace,
        RollHitPoints,
        EnterName
    }

    public record CharacterCreationOption(string Value, string Label);

    public record CharacterCreationResponse(
        CharacterCreationStep                   Step,
        string                                  Prompt,
        IReadOnlyList<CharacterCreationOption>? Options = null) : IResponse;

    public record CharacterCreationCompleteResponse(CharacterSummary Summary) : IResponse;
}
