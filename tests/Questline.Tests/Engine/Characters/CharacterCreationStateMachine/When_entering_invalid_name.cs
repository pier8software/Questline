using Questline.Engine.Characters;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Engine.Characters;

public class When_entering_invalid_name
{
    private static readonly int[] DefaultRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    [Fact]
    public void Invalid_name_returns_validation_error()
    {
        var sm = new CharacterCreationStateMachine(new FakeDice(DefaultRolls));
        sm.ProcessInput(null);
        sm.ProcessInput("1");
        sm.ProcessInput("1");
        sm.ProcessInput(null);

        var response = sm.ProcessInput("");

        var creation = response.ShouldBeOfType<Responses.CharacterCreationResponse>();
        creation.Prompt.ShouldContain("Please give your character a name.");
    }
}
