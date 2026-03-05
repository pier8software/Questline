using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Engine.Characters.CharacterCreationStateMachine;

public class When_completing_creation
{
    // 3d6 x 6 ability scores = 18 rolls, then 1d8 for HP = 19 rolls total
    private static readonly int[] DefaultRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static Questline.Engine.Characters.CharacterCreationStateMachine CreateStateMachine(params int[] diceRolls) =>
        new(new FakeDice(diceRolls));

    [Fact]
    public void Rolls_3d6_for_each_ability_score()
    {
        // STR: 4+5+6=15, INT: 3+3+3=9, WIS: 2+4+6=12, DEX: 1+1+1=3, CON: 6+6+6=18, CHA: 5+5+5=15
        var sm = CreateStateMachine(4, 5, 6, 3, 3, 3, 2, 4, 6, 1, 1, 1, 6, 6, 6, 5, 5, 5, 7);
        sm.ProcessInput(null);
        sm.ProcessInput("1");
        sm.ProcessInput("1");
        sm.ProcessInput(null);

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Summary.AbilityScores.Strength.ShouldBe(15);
        response.Summary.AbilityScores.Intelligence.ShouldBe(9);
        response.Summary.AbilityScores.Wisdom.ShouldBe(12);
        response.Summary.AbilityScores.Dexterity.ShouldBe(3);
        response.Summary.AbilityScores.Constitution.ShouldBe(18);
        response.Summary.AbilityScores.Charisma.ShouldBe(15);
    }

    [Fact]
    public void Sets_max_hit_points_to_8_for_fighter()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null);
        sm.ProcessInput("1");
        sm.ProcessInput("1");
        sm.ProcessInput(null);

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Summary.MaxHitPoints.ShouldBe(8);
    }

    [Fact]
    public void Rolls_1d8_for_current_hit_points()
    {
        // Last roll (index 18) is 4 for 1d8 HP
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null);
        sm.ProcessInput("1");
        sm.ProcessInput("1");
        sm.ProcessInput(null);

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Summary.CurrentHitPoints.ShouldBe(4);
    }
}
