using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_input_starts_with_actor_name
{
    private readonly Questline.Engine.Parsers.Parser _parser = new();

    [Fact]
    public void Routes_command_to_named_character()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "party-1", members: [aric, mira]);

        var result = _parser.Parse("aric look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
        ((CharacterActor)result.Actor!).Character.ShouldBe(aric);
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Matches_actor_name_case_insensitively()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("ARIC look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
    }
}
