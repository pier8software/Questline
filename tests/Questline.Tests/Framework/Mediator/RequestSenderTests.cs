using Microsoft.Extensions.DependencyInjection;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Framework.Mediator;

public class RequestSenderTests
{
    [Fact]
    public void Registered_verb_executes_its_handler()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .BuildState("player1", "start");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .BuildServiceProvider();

        var dispatcher = new RequestSender(serviceProvider);

        var result = dispatcher.Send(state, new Requests.GetRoomDetailsQuery());

        result.ShouldBeOfType<Responses.RoomDetailsResponse>();
    }
}
