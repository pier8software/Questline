using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_required_actor_is_missing
{
    private record FakeActorOnlyRequest : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new FakeActorOnlyRequest();
    }

    [Fact]
    public void Returns_failure_when_no_actor_prefix_present()
    {
        var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["picklock"] = (_ => ParseResult.Success(new FakeActorOnlyRequest()), true)
        };
        var parser = new Questline.Engine.Parsers.Parser(verbs);
        var aric   = CharacterBuilder.New().WithName("Aric").Build();
        var party  = new Party(id: "p", members: [aric]);

        var result = parser.Parse("picklock", party);

        result.IsSuccess.ShouldBeFalse();
        result.Error!.Message.ShouldContain("Which character");
    }

    [Fact]
    public void Succeeds_when_actor_prefix_is_present()
    {
        var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["picklock"] = (_ => ParseResult.Success(new FakeActorOnlyRequest()), true)
        };
        var parser = new Questline.Engine.Parsers.Parser(verbs);
        var aric   = CharacterBuilder.New().WithName("Aric").Build();
        var party  = new Party(id: "p", members: [aric]);

        var result = parser.Parse("aric picklock", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
    }
}
