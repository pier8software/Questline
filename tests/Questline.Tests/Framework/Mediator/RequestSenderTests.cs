using Microsoft.Extensions.DependencyInjection;
using Questline.Domain;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Handlers;
using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Framework.Mediator;

public class RequestSenderTests
{
    [Fact]
    public void Registered_verb_executes_its_handler()
    {
        var world = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "start" });

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .BuildServiceProvider();


        var dispatcher = new RequestSender(serviceProvider);

        var result = dispatcher.Send(state, new Requests.GetRoomDetailsQuery());

        result.ShouldBeOfType<Responses.RoomDetailsResponse>();
    }
}
