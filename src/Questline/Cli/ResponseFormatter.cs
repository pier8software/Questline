using Questline.Domain.Characters.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Cli;

public class ResponseFormatter
{
    public string Format(IResponse response) => response switch
    {
        Responses.GameStartedResponse                 => FormatGameStarted(),
        Responses.LoggedInResponse r                  => FormatLogin(r),
        Responses.NewAdventureSelectedResponse        => FormatAdventureSelected(),
        Responses.CharacterCreationResponse r         => FormatCharacterCreation(r),
        Responses.CharacterCreationCompleteResponse r => FormatCharacterCreationComplete(r),
        Responses.AdventureStartedResponse r          => FormatAdventureStarted(r),
        Responses.PlayerMovedResponse r =>
            FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers),
        Responses.RoomDetailsResponse r =>
            FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers),
        Responses.ItemTakenResponse r   => $"You pick up the {r.ItemName}.",
        Responses.ItemDroppedResponse r => $"You drop the {r.ItemName}.",
        Responses.PlayerInventoryResponse r => r.Items.Count == 0
            ? "You are not carrying anything."
            : $"You are carrying: {string.Join(", ", r.Items)}",
        Responses.ExamineResponse r  => r.Description,
        Responses.UseItemResponse r  => r.ResultMessage,
        Responses.VersionResponse r  => $"Questline v{r.Version}",
        Responses.GameQuitedResponse => "Goodbye!",
        ErrorResponse r              => r.ErrorMessage,
        _                            => response.ToString() ?? ""
    };

    private static string FormatGameStarted() =>
        string.Join("\n", "Welcome adventure!", "type 'login <username>' to begin your adventure!");

    private string FormatLogin(Responses.LoggedInResponse response)
    {
        var idx        = 1;
        var adventures = string.Join("\n", response.Adventures.Select(a => $"\t{idx++}. {a.Name}"));
        return $"Welcome back {response.Player.Name}!\nSelect an adventure to begin your journey:\n{adventures}";
    }

    private string FormatAdventureSelected() =>
        string.Join("\n", "Lets create a character!", "Select your character's class:", "\t1. Fighter");

    private static string FormatCharacterCreation(Responses.CharacterCreationResponse r)
    {
        if (r.Options is { Count: > 0 })
        {
            var options = string.Join("\n", r.Options.Select(o => $"\t{o.Value}. {o.Label}"));
            return $"{r.Prompt}\n{options}";
        }

        return r.Prompt;
    }

    private static string FormatCharacterCreationComplete(Responses.CharacterCreationCompleteResponse r) =>
        $"You have created your character.\n{FormatCharacterSummary(r.Summary)}";

    private static string FormatAdventureStarted(Responses.AdventureStartedResponse r)
    {
        var roomView = FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers);
        return $"Welcome {r.Character.Name}! Your adventure begins...\n{roomView}";
    }

    private static string FormatCharacterSummary(CharacterSummary c)
    {
        return $"""
                Name: {c.Name}
                Race: {c.Race}
                Class: {c.Class}
                Level: {c.Level}
                HP: {c.CurrentHitPoints}/{c.MaxHitPoints}
                XP: {c.Experience}

                Ability Scores:
                  STR: {c.AbilityScores.Strength}
                  INT: {c.AbilityScores.Intelligence}
                  WIS: {c.AbilityScores.Wisdom}
                  DEX: {c.AbilityScores.Dexterity}
                  CON: {c.AbilityScores.Constitution}
                  CHA: {c.AbilityScores.Charisma}
                """;
    }

    private static string FormatRoomView(
        string                name,
        string                description,
        IReadOnlyList<string> exits,
        IReadOnlyList<string> items,
        IReadOnlyList<string> lockedBarriers)
    {
        var parts = new List<string> { name, description };

        if (items.Count > 0)
        {
            parts.Add($"You can see: {string.Join(", ", items)}");
        }

        if (lockedBarriers.Count > 0)
        {
            parts.AddRange(lockedBarriers);
        }

        parts.Add($"Exits: {string.Join(", ", exits)}");

        return string.Join("\n", parts);
    }
}
