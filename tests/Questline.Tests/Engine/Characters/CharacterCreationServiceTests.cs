using Questline.Domain.Characters.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Engine.Characters;

public class CharacterCreationStateMachineTests
{
    // 3d6 x 6 ability scores = 18 rolls, then 1d8 for HP = 19 rolls total
    private static readonly int[] DefaultRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static CharacterCreationStateMachine CreateStateMachine(params int[] diceRolls) =>
        new(new FakeDice(diceRolls));

    [Fact]
    public void First_call_rolls_ability_scores_and_prompts_class_selection()
    {
        var sm = CreateStateMachine(DefaultRolls);

        var response = sm.ProcessInput(null); // select class prompt

        response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        response.Message.ShouldContain("class");
    }

    [Fact]
    public void Selecting_fighter_prompts_race_selection()
    {
        var sm = CreateStateMachine(DefaultRolls);

        sm.ProcessInput(null); // select class prompt
        var response = sm.ProcessInput("1");

        response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        response.Message.ShouldContain("race");
    }

    [Fact]
    public void Selecting_human_prompts_hit_points()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter

        var response = sm.ProcessInput("1");

        response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        response.Message.ShouldContain("continue");
    }

    [Fact]
    public void Hit_points_step_rolls_and_prompts_name()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human

        var response = sm.ProcessInput(null); // roll HP

        response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        response.Message.ShouldContain("name");
    }

    [Fact]
    public void Entering_valid_name_completes_creation()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human
        sm.ProcessInput(null); // roll HP

        var response = sm.ProcessInput("Thorin");

        var complete = response.ShouldBeOfType<Responses.CharacterCreationCompleteResponse>();
        complete.Character.Name.ShouldBe("Thorin");
        complete.Character.Race.ShouldBe(Race.Human);
        complete.Character.Class.ShouldBe(CharacterClass.Fighter);
        complete.Character.Level.ShouldBe(1);
    }

    [Fact]
    public void Rolls_3d6_for_each_ability_score()
    {
        // STR: 4+5+6=15, INT: 3+3+3=9, WIS: 2+4+6=12, DEX: 1+1+1=3, CON: 6+6+6=18, CHA: 5+5+5=15
        var sm = CreateStateMachine(4, 5, 6, 3, 3, 3, 2, 4, 6, 1, 1, 1, 6, 6, 6, 5, 5, 5, 7);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human
        sm.ProcessInput(null); // roll HP

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Character.AbilityScores.Strength.Score.ShouldBe(15);
        response.Character.AbilityScores.Intelligence.Score.ShouldBe(9);
        response.Character.AbilityScores.Wisdom.Score.ShouldBe(12);
        response.Character.AbilityScores.Dexterity.Score.ShouldBe(3);
        response.Character.AbilityScores.Constitution.Score.ShouldBe(18);
        response.Character.AbilityScores.Charisma.Score.ShouldBe(15);
    }

    [Fact]
    public void Sets_max_hit_points_to_8_for_fighter()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human
        sm.ProcessInput(null); // roll HP

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Character.HitPoints.MaxHitPoints.ShouldBe(8);
    }

    [Fact]
    public void Rolls_1d8_for_current_hit_points()
    {
        // Last roll (index 18) is 4 for 1d8 HP
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human
        sm.ProcessInput(null); // roll HP

        var response = (Responses.CharacterCreationCompleteResponse)sm.ProcessInput("Thorin");

        response.Character.HitPoints.CurrentHitPoints.ShouldBe(4);
    }

    [Fact]
    public void Invalid_name_returns_validation_error()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null); // select class prompt
        sm.ProcessInput("1");  // select fighter
        sm.ProcessInput("1");  // select human
        sm.ProcessInput(null); // roll HP

        var response = sm.ProcessInput("");

        response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        response.Message.ShouldContain("Please give your character a name.");
    }
}
