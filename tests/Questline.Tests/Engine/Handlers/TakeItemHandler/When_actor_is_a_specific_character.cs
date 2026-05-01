using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.TakeItemHandler;

public class When_actor_is_a_specific_character
{
    private readonly Questline.Engine.Handlers.TakeItemHandler _handler;
    private readonly Character                                 _leader;
    private readonly Character                                 _mira;
    private readonly Playthrough                               _playthrough;

    public When_actor_is_a_specific_character()
    {
        _leader = CharacterBuilder.New().WithName("Aric").Build();
        _mira   = CharacterBuilder.New().WithName("Mira").Build();

        var room = Rooms.Cellar.WithItem(Items.BrassLamp).Build();

        var party = new Party(id: Guid.NewGuid().ToString(), members: [_leader, _mira]);

        _playthrough = new Playthrough
        {
            Id             = "test-playthrough",
            Username       = "test-user",
            AdventureId    = "test-adventure",
            StartingRoomId = room.Id,
            Location       = room.Id,
            Party          = party
        };

        var playthroughRepo = new FakePlaythroughRepository(_playthrough);
        var roomRepo        = new FakeRoomRepository(new Dictionary<string, Questline.Domain.Rooms.Entity.Room>
        {
            [room.Id] = room
        });
        var session = new FakeGameSession(_playthrough.Id);

        _handler = new Questline.Engine.Handlers.TakeItemHandler(session, playthroughRepo, roomRepo);
    }

    [Fact]
    public async Task Named_character_receives_the_item()
    {
        await _handler.Handle(new CharacterActor(_mira), new Requests.TakeItemCommand("brass lamp"));

        _mira.Inventory.ShouldContain(i => i.Name == "brass lamp");
        _leader.Inventory.ShouldBeEmpty();
    }

    [Fact]
    public async Task Response_names_the_acting_character()
    {
        var result = await _handler.Handle(new CharacterActor(_mira), new Requests.TakeItemCommand("brass lamp"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.CharacterName.ShouldBe("Mira");
        takeResult.Message.ShouldBe("Mira picks up the brass lamp.");
    }
}
