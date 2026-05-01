using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.PartyCreationStateMachine;

public class When_rerolling
{
    [Fact]
    public void Replaces_the_entire_party_with_fresh_rolls()
    {
        // Provide enough dice for two parties (88 + 88 = 176 slots).
        var rolls = new List<int>();
        for (var p = 0; p < 2; p++)
            for (var i = 0; i < 4; i++)
            {
                rolls.Add(1);
                for (var s = 0; s < 18; s++) rolls.Add(3);
                rolls.Add(2);
                rolls.Add(p == 0 ? 1 : 2);                     // different occupation per party
                rolls.Add(i + 1 + p * 4);                     // name index — different per party-PC
            }

        var sm = new Questline.Engine.Characters.PartyCreationStateMachine(new FakeDice(rolls.ToArray()));

        var first   = (Responses.PartyRolledResponse)sm.Start();
        var second  = (Responses.PartyRolledResponse)sm.ProcessInput("reroll");

        second.Members.Count.ShouldBe(4);
        // The two parties should have different first-PC names because we used different name indices.
        second.Members[0].Name.ShouldNotBe(first.Members[0].Name);
    }
}
