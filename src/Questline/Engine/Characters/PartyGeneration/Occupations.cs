namespace Questline.Engine.Characters.PartyGeneration;

public static class Occupations
{
    public static readonly IReadOnlyList<string> All =
    [
        "Apprentice scribe",
        "Beekeeper",
        "Charcoal-burner",
        "Cooper",
        "Cordwainer",
        "Dyer",
        "Falconer's hand",
        "Fishmonger",
        "Goose-girl",
        "Herbalist",
        "Hireling porter",
        "Inn-keeper",
        "Lamplighter",
        "Mason's apprentice",
        "Miller",
        "Outlaw",
        "Pilgrim",
        "Smithy boy",
        "Tinker",
        "Wandering minstrel"
    ];

    public static string Pick(IDice dice) =>
        All[dice.Roll(All.Count) - 1];
}
