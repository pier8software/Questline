using Microsoft.Extensions.DependencyInjection;
using Questline.Domain;
using Questline.Domain.Entities;
using Questline.Domain.Handlers;
using Questline.Domain.Messages;
using Questline.Domain.Shared;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Framework.Mediator;

public class CommandDispatcherTests
{
    [Fact]
    public void Registered_verb_executes_its_handler()
    {
        var world = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "start" });

        var serviceProvider = new ServiceCollection()
            .AddSingleton<ICommandHandler<Commands.ViewRoom>, ViewRoomHandler>()
            .BuildServiceProvider();


        var dispatcher = new CommandDispatcher(serviceProvider);

        var result = dispatcher.Dispatch(state, new Commands.ViewRoom());

        result.ShouldBeOfType<Results.RoomViewed>();
    }
}
