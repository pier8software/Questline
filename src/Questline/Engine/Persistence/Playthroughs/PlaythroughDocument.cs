using Questline.Engine.Persistence.Rooms;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughDocument : Document
{
    public string                               AdventureId       { get; set; } = null!;
    public string                               StartingRoomId    { get; set; } = null!;
    public string                               CharacterName     { get; set; } = null!;
    public string                               Race              { get; set; } = null!;
    public string                               Class             { get; set; } = null!;
    public int                                  Level             { get; set; }
    public int                                  Experience        { get; set; }
    public AbilityScoresDocument                AbilityScores     { get; set; } = null!;
    public HitPointsDocument                    HitPoints         { get; set; } = null!;
    public string                               Location          { get; set; } = null!;
    public List<ItemDocument>                   Inventory         { get; set; } = [];
    public List<string>                         UnlockedBarriers  { get; set; } = [];
    public Dictionary<string, List<ItemDocument>> RoomItems       { get; set; } = new();
}

public class AbilityScoresDocument
{
    public int Strength     { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom       { get; set; }
    public int Dexterity    { get; set; }
    public int Constitution { get; set; }
    public int Charisma     { get; set; }
}

public class HitPointsDocument
{
    public int MaxHitPoints     { get; set; }
    public int CurrentHitPoints { get; set; }
}
