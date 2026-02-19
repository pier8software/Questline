using Questline.Domain.Characters.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class StatsQueryHandlerTests
{
    [Fact]
    public void Returns_character_name_in_stats()
    {
        var stats = new CharacterStats(8, 6, 14, 10, 12, 8, 15, 11);
        var character = new Character("Thorin", Race.Human, CharacterClass.Fighter, Stats: stats);
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .WithCharacter(character)
            .BuildState("player1", "tavern");

        var handler = new StatsQueryHandler();
        var result = handler.Handle(state, new Requests.StatsQuery());

        result.Message.ShouldContain("Thorin");
    }

    [Fact]
    public void Returns_race_and_class()
    {
        var stats = new CharacterStats(8, 6, 14, 10, 12, 8, 15, 11);
        var character = new Character("Thorin", Race.Human, CharacterClass.Fighter, Stats: stats);
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .WithCharacter(character)
            .BuildState("player1", "tavern");

        var handler = new StatsQueryHandler();
        var result = handler.Handle(state, new Requests.StatsQuery());

        result.Message.ShouldContain("Human");
        result.Message.ShouldContain("Fighter");
    }

    [Fact]
    public void Returns_level()
    {
        var stats = new CharacterStats(8, 6, 14, 10, 12, 8, 15, 11);
        var character = new Character("Thorin", Race.Human, CharacterClass.Fighter, Stats: stats);
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .WithCharacter(character)
            .BuildState("player1", "tavern");

        var handler = new StatsQueryHandler();
        var result = handler.Handle(state, new Requests.StatsQuery());

        result.Message.ShouldContain("Level 1");
    }

    [Fact]
    public void Returns_all_ability_scores()
    {
        var stats = new CharacterStats(8, 6, 14, 10, 12, 8, 15, 11);
        var character = new Character("Thorin", Race.Human, CharacterClass.Fighter, Stats: stats);
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .WithCharacter(character)
            .BuildState("player1", "tavern");

        var handler = new StatsQueryHandler();
        var result = handler.Handle(state, new Requests.StatsQuery());

        result.Message.ShouldContain("STR: 14");
        result.Message.ShouldContain("INT: 10");
        result.Message.ShouldContain("WIS: 12");
        result.Message.ShouldContain("DEX: 8");
        result.Message.ShouldContain("CON: 15");
        result.Message.ShouldContain("CHA: 11");
    }

    [Fact]
    public void Returns_health_values()
    {
        var stats = new CharacterStats(8, 6, 14, 10, 12, 8, 15, 11);
        var character = new Character("Thorin", Race.Human, CharacterClass.Fighter, Stats: stats);
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .WithCharacter(character)
            .BuildState("player1", "tavern");

        var handler = new StatsQueryHandler();
        var result = handler.Handle(state, new Requests.StatsQuery());

        result.Message.ShouldContain("HP: 6/8");
    }
}
