using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Parties.Entity.Party;

public class When_finding_member_by_name
{
    [Fact]
    public void Returns_member_with_matching_name_case_insensitively()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [aric, mira]);

        party.FindByName("aric").ShouldBe(aric);
        party.FindByName("MIRA").ShouldBe(mira);
    }

    [Fact]
    public void Returns_null_when_no_member_matches()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [aric]);

        party.FindByName("borin").ShouldBeNull();
    }
}
