using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.PartyCreationStateMachine;

public class When_rolling_initial_party
{
    [Fact]
    public void Rolls_four_level_zero_characters()
    {
        // 88 dice slots: 4 PCs × (1 race + 18 ability + 1 hp + 1 occ + 1 name)
        // Using race=1 (Human) for all PCs, ability dice = 3, hp = 2, occ = 1, name = i (unique per PC).
        var diceValues = ProduceRolls();
        var dice       = new FakeDice(diceValues);

        var sm       = new Questline.Engine.Characters.PartyCreationStateMachine(dice);
        var response = sm.Start();

        response.ShouldBeOfType<Responses.PartyRolledResponse>();
        var rolled = (Responses.PartyRolledResponse)response;
        rolled.Members.Count.ShouldBe(4);
        rolled.Members.ShouldAllBe(m => m.Level == 0);
        rolled.Members.ShouldAllBe(m => m.Class == "Level 0");
        rolled.Members.Select(m => m.Name).Distinct().Count().ShouldBe(4); // names unique
    }

    private static int[] ProduceRolls()
    {
        var rolls = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            rolls.Add(1);                                    // race: AllRaces[0] = Human
            for (var s = 0; s < 18; s++) rolls.Add(3);       // 6 × 3d6, each die = 3
            rolls.Add(2);                                    // hp = 2
            rolls.Add(1);                                    // occupation index 1 (first entry)
            rolls.Add(i + 1);                                // name index — different per PC
        }
        return rolls.ToArray();
    }
}
