using Questline.Domain.Characters.Entity;

namespace Questline.Engine.Characters.PartyGeneration;

public static class Names
{
    private static readonly IReadOnlyList<string> Human =
        ["Aric", "Borin", "Cedric", "Dunwald", "Edric", "Faelan", "Galen", "Hadric",
         "Mira", "Niamh", "Orla", "Petra", "Rosa", "Sela", "Talia", "Una"];

    private static readonly IReadOnlyList<string> Dwarf =
        ["Brokk", "Durin", "Falin", "Garr", "Korin", "Morrick", "Nain", "Thrain",
         "Brida", "Dagna", "Hilda", "Mara", "Rilda", "Sigrun", "Thora", "Vala"];

    private static readonly IReadOnlyList<string> Elf =
        ["Aelric", "Belion", "Caelan", "Daerion", "Elond", "Faerion", "Galin", "Lirael",
         "Aelyn", "Belira", "Caelith", "Erendel", "Larael", "Mirien", "Saelin", "Tarael"];

    private static readonly IReadOnlyList<string> Halfling =
        ["Bilbo", "Cosmo", "Drogo", "Frodo", "Gilly", "Hobson", "Otho", "Pipin",
         "Belba", "Cora", "Daisy", "Esmera", "Lily", "Marigold", "Pansy", "Ruby"];

    public static string Pick(Race race, IDice dice)
    {
        var pool = race switch
        {
            Race.Human    => Human,
            Race.Dwarf    => Dwarf,
            Race.Elf      => Elf,
            Race.Halfling => Halfling,
            _             => Human
        };
        return pool[dice.Roll(pool.Count) - 1];
    }
}
