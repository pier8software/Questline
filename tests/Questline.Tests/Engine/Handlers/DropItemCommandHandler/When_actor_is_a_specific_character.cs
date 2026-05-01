using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.DropItemCommandHandler;

public class When_actor_is_a_specific_character
{
    private readonly Questline.Engine.Handlers.DropItemCommandHandler _handler;
    private readonly Character                                        _leader;
    private readonly Character                                        _mira;

    public When_actor_is_a_specific_character()
    {
        _leader = CharacterBuilder.New().WithName("Aric").Build();
        _mira   = CharacterBuilder.New().WithName("Mira").Build();
        _mira.AddInventoryItem(Items.BrassLamp.Build());

        var room = Rooms.Cellar.Build();

        var party = new Party(id: Guid.NewGuid().ToString(), members: [_leader, _mira]);

        var playthrough = new Playthrough
        {
            Id             = "test-playthrough",
            Username       = "test-user",
            AdventureId    = "test-adventure",
            StartingRoomId = room.Id,
            Location       = room.Id,
            Party          = party
        };

        var playthroughRepo = new FakePlaythroughRepository(playthrough);
        var roomRepo        = new FakeRoomRepository(new Dictionary<string, Questline.Domain.Rooms.Entity.Room>
        {
            [room.Id] = room
        });
        var session = new FakeGameSession(playthrough.Id);

        _handler = new Questline.Engine.Handlers.DropItemCommandHandler(session, playthroughRepo, roomRepo);
    }

    [Fact]
    public async Task Named_character_loses_the_item()
    {
        await _handler.Handle(new CharacterActor(_mira), new Requests.DropItemCommand("brass lamp"));

        _mira.Inventory.ShouldBeEmpty();
        _leader.Inventory.ShouldBeEmpty();
    }

    [Fact]
    public async Task Response_names_the_acting_character()
    {
        var result = await _handler.Handle(new CharacterActor(_mira), new Requests.DropItemCommand("brass lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.CharacterName.ShouldBe("Mira");
        dropResult.Message.ShouldBe("Mira drops the brass lamp.");
    }

    [Fact]
    public async Task Cannot_drop_item_carried_by_leader_when_acting_as_mira()
    {
        _leader.AddInventoryItem(Items.RustyKey.Build());

        var result = await _handler.Handle(new CharacterActor(_mira), new Requests.DropItemCommand("rusty key"));

        result.ShouldBeOfType<ErrorResponse>();
        _leader.Inventory.ShouldContain(i => i.Name == "rusty key");
    }
}
