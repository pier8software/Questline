using Questline.Domain.Playthroughs.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Cli.Game;

public class ResponseFormatter
{
    public string Format(IResponse response) => response switch
    {
        Responses.GameStartedResponse                 => FormatGameStarted(),
        Responses.LoggedInResponse r                  => FormatLogin(r),
        Responses.StartMenuResponse                   => FormatStartMenu(),
        Responses.NewGameResponse r                   => FormatNewGame(r),
        Responses.SavedPlaythroughsResponse r         => FormatSavedPlaythroughs(r),
        Responses.NoSavedGamesResponse                => "No saved games found.",
        Responses.NewAdventureSelectedResponse        => FormatAdventureSelected(),
        Responses.PartyRolledResponse r               => r.Message,
        Responses.PartyAcceptedResponse r             => r.Message,
        Responses.PartyCreationErrorResponse r        => r.Message,
        Responses.AdventureStartedResponse r          => FormatAdventureStarted(r),
        Responses.PlayerMovedResponse r =>
            FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers),
        Responses.RoomDetailsResponse r =>
            FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers),
        Responses.ItemTakenResponse r   => r.Message,
        Responses.ItemDroppedResponse r => r.Message,
        Responses.InventoryResponse r   => r.Message,
        Responses.ExamineResponse r  => r.Description,
        Responses.UseItemResponse r  => r.ResultMessage,
        Responses.VersionResponse r  => $"Questline v{r.Version}",
        Responses.StatsResponse r    => r.Message,
        Responses.GameQuitedResponse => "Goodbye!",
        ErrorResponse r              => r.ErrorMessage,
        _                            => response.ToString() ?? ""
    };

    private static string FormatGameStarted() =>
        string.Join("\n", "Welcome adventure!", "type 'login <username>' to begin your adventure!");

    private static string FormatStartMenu() =>
        string.Join("\n", "What would you like to do?", "\t1. New Game", "\t2. Load Game");

    private static string FormatNewGame(Responses.NewGameResponse response)
    {
        var idx        = 1;
        var adventures = string.Join("\n", response.Adventures.Select(a => $"\t{idx++}. {a.Name}"));
        return $"Select an adventure to begin your journey:\n{adventures}";
    }

    private static string FormatSavedPlaythroughs(Responses.SavedPlaythroughsResponse response)
    {
        var idx   = 1;
        var saves = string.Join("\n", response.Playthroughs.Select(p =>
            $"\t{idx++}. {p.CharacterName} - {p.AdventureName} ({p.Location})"));
        return $"Select a saved game:\n{saves}";
    }

    private string FormatLogin(Responses.LoggedInResponse response)
    {
        var idx        = 1;
        var adventures = string.Join("\n", response.Adventures.Select(a => $"\t{idx++}. {a.Name}"));
        return $"Welcome back {response.Player.Name}!\nSelect an adventure to begin your journey:\n{adventures}";
    }

    private static string FormatAdventureSelected() =>
        "Select your party and get ready for adventure!";

    private static string FormatAdventureStarted(Responses.AdventureStartedResponse r)
    {
        var roomView = FormatRoomView(r.RoomName, r.Description, r.Exits, r.Items, r.LockedBarriers);
        return $"Welcome {r.Character.Name}! Your adventure begins...\n{roomView}";
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
