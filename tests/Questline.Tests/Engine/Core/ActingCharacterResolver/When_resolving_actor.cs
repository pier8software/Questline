using Questline.Domain.Parties.Entity;
using Questline.Engine.Core;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Engine.Core.ActingCharacterResolver;

public class When_resolving_actor
{
    [Fact]
    public void Party_actor_resolves_to_marching_order_leader()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "p", members: [aric, mira]);

        var resolved = Questline.Engine.Core.ActingCharacterResolver.Resolve(new PartyActor(), party);

        resolved.ShouldBe(aric);
    }

    [Fact]
    public void Character_actor_resolves_to_named_character()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "p", members: [aric, mira]);

        var resolved = Questline.Engine.Core.ActingCharacterResolver.Resolve(new CharacterActor(mira), party);

        resolved.ShouldBe(mira);
    }

    [Fact]
    public void No_actor_throws()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "p", members: [aric]);

        Should.Throw<InvalidOperationException>(
            () => Questline.Engine.Core.ActingCharacterResolver.Resolve(new NoActor(), party));
    }
}
