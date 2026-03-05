using Microsoft.Extensions.DependencyInjection;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Framework.Mediator.RequestSender;

public class When_sending_request
{
    [Fact]
    public async Task Registered_verb_executes_its_handler()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.StartRoom)
            .Build("start");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IGameSession>(fixture.Session)
            .AddSingleton<IPlaythroughRepository>(fixture.PlaythroughRepository)
            .AddSingleton<IRoomRepository>(fixture.RoomRepository)
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .BuildServiceProvider();

        var dispatcher = new Questline.Framework.Mediator.RequestSender(serviceProvider);

        var result = await dispatcher.Send(new Requests.GetRoomDetailsQuery());

        result.ShouldBeOfType<Responses.RoomDetailsResponse>();
    }
}
