using Questline.Domain.Characters.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class CharacterBuilder
{
    private string          _id            = Guid.NewGuid().ToString();
    private string          _name          = "Aric";
    private Race            _race          = Race.Human;
    private CharacterClass? _class         = null;
    private string          _occupation    = "Beekeeper";
    private AbilityScores   _abilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));
    private HitPoints       _hitPoints     = new(max: 4, current: 4);

    public static CharacterBuilder New() => new();

    public CharacterBuilder WithId(string id)            { _id = id; return this; }
    public CharacterBuilder WithName(string name)        { _name = name; return this; }
    public CharacterBuilder WithRace(Race race)          { _race = race; return this; }
    public CharacterBuilder WithOccupation(string s)     { _occupation = s; return this; }
    public CharacterBuilder WithAbilityScores(AbilityScores scores) { _abilityScores = scores; return this; }
    public CharacterBuilder WithHitPoints(int max, int current) { _hitPoints = new HitPoints(max, current); return this; }

    public Character Build() =>
        Character.Create(_id, _name, _race, _class, _hitPoints, _abilityScores, _occupation);
}
