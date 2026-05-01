using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Parties.Entity.Party;

public class When_party_has_no_living_members
{
    [Fact]
    public void Returns_empty_alive_collection()
    {
        var dead   = CharacterBuilder.New().WithName("Aric").WithHitPoints(max: 4, current: 0).Build();
        var party  = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [dead]);

        party.MembersAlive.ShouldBeEmpty();
    }

    [Fact]
    public void Returns_only_living_members()
    {
        var alive = CharacterBuilder.New().WithName("Mira").WithHitPoints(max: 4, current: 4).Build();
        var dead  = CharacterBuilder.New().WithName("Aric").WithHitPoints(max: 4, current: 0).Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [dead, alive]);

        party.MembersAlive.ShouldBe([alive]);
    }
}
