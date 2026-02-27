using Microsoft.Extensions.DependencyInjection;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Framework.Mediator;

public class RequestSenderTests
{
    [Fact]
    public async Task Registered_verb_executes_its_handler()
    {
        var fixture = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build("start");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IGameSession>(fixture.Session)
            .AddSingleton<IPlaythroughRepository>(fixture.PlaythroughRepository)
            .AddSingleton<IRoomRepository>(fixture.RoomRepository)
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .BuildServiceProvider();

        var dispatcher = new RequestSender(serviceProvider);

        var result = await dispatcher.Send(new Requests.GetRoomDetailsQuery());

        result.ShouldBeOfType<Responses.RoomDetailsResponse>();
    }
}
