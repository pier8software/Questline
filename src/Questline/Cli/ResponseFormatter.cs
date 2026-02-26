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
        Responses.NewAdventureSelectedResponse r      => FormatAdventureSelected(r),
        Responses.CharacterCreationResponse r         => FormatCharacterCreation(r),
        Responses.CharacterCreationCompleteResponse r => FormatCharacterCreationComplete(r),
        Responses.GameQuitedResponse                  => "Goodbye!",
        ErrorResponse r                               => r.ErrorMessage,
        _                                             => response.ToString() ?? ""
    };

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

    private string FormatAdventureSelected(Responses.NewAdventureSelectedResponse response) =>
        string.Join("\n", "Lets create a character!", "Select your character's class:", "\t1. Fighter");

    private static string FormatGameStarted() =>
        string.Join("\n", "Welcome adventure!", "type 'login <username>' to begin your adventure!");

    private string FormatLogin(Responses.LoggedInResponse response) =>
        string.Join("\n", $"Welcome back {response.Player.Name}!", "Select an adventure to begin your journey:",
            response.Adventures.Select((a, i) => $"{i + 1}. {a.Name}"));
}
