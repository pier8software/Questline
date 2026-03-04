using Questline.Engine.Characters;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Engine.Characters;

public class When_stepping_through_creation
{
    // 3d6 x 6 ability scores = 18 rolls, then 1d8 for HP = 19 rolls total
    private static readonly int[] DefaultRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static CharacterCreationStateMachine CreateStateMachine(params int[] diceRolls) =>
        new(new FakeDice(diceRolls));

    [Fact]
    public void First_call_rolls_ability_scores_and_prompts_class_selection()
    {
        var sm = CreateStateMachine(DefaultRolls);

        var response = sm.ProcessInput(null);

        var creation = response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        creation.Prompt.ShouldContain("class");
    }

    [Fact]
    public void Selecting_fighter_prompts_race_selection()
    {
        var sm = CreateStateMachine(DefaultRolls);

        sm.ProcessInput(null);
        var response = sm.ProcessInput("1");

        var creation = response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        creation.Prompt.ShouldContain("race");
    }

    [Fact]
    public void Selecting_human_prompts_hit_points()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null);
        sm.ProcessInput("1");

        var response = sm.ProcessInput("1");

        var creation = response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        creation.Prompt.ShouldContain("continue");
    }

    [Fact]
    public void Hit_points_step_rolls_and_prompts_name()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput(null);
        sm.ProcessInput("1");
        sm.ProcessInput("1");

        var response = sm.ProcessInput(null);

        var creation = response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        creation.Prompt.ShouldContain("name");
    }

    [Fact]
    public void Entering_valid_name_completes_creation()
    {
        var sm = CreateStateMachine(DefaultRolls);
        sm.ProcessInput("1");
        sm.ProcessInput("1");
        sm.ProcessInput(null);

        var response = sm.ProcessInput("Thorin");

        var complete = response.ShouldBeOfType<Responses.CharacterCreationCompleteResponse>();
        complete.Summary.Name.ShouldBe("Thorin");
        complete.Summary.Race.ShouldBe("Human");
        complete.Summary.Class.ShouldBe("Fighter");
        complete.Summary.Level.ShouldBe(1);
    }
}
