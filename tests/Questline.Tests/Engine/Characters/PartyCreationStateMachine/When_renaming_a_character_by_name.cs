using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.PartyCreationStateMachine;

public class When_renaming_a_character_by_name
{
    [Fact]
    public void Renames_the_character_with_matching_current_name()
    {
        var rolls = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            rolls.Add(1);
            for (var s = 0; s < 18; s++) rolls.Add(3);
            rolls.Add(2);
            rolls.Add(1);
            rolls.Add(i + 1);
        }
        var sm      = new Questline.Engine.Characters.PartyCreationStateMachine(new FakeDice(rolls.ToArray()));
        var initial = (Responses.PartyRolledResponse)sm.Start();
        var oldName = initial.Members[1].Name;

        var response = sm.ProcessInput($"rename {oldName} Mira");

        response.ShouldBeOfType<Responses.PartyRolledResponse>();
        var rolled = (Responses.PartyRolledResponse)response;
        rolled.Members[1].Name.ShouldBe("Mira");
    }
}
