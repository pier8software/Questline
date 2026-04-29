using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_input_has_no_actor_prefix
{
    private readonly Questline.Engine.Parsers.Parser _parser = new();

    [Fact]
    public void Returns_party_actor()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<PartyActor>();
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Treats_unmatched_first_token_as_verb_attempt()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("borin look", party);

        // 'borin' isn't a PC and isn't a verb, so the parser fails.
        result.IsSuccess.ShouldBeFalse();
    }
}
