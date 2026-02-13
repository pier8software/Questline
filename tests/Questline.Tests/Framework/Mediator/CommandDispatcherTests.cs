using Microsoft.Extensions.DependencyInjection;
using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Tests.Framework.Mediator;

public class CommandDispatcherTests
{
    [Fact]
    public void Registered_verb_executes_its_handler()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "start" });

        var serviceProvider = new ServiceCollection()
            .AddSingleton<ICommandHandler<Commands.LookCommand>, LookCommandHandler>()
            .BuildServiceProvider();


        var dispatcher = new CommandDispatcher(serviceProvider);

        var result = dispatcher.Dispatch(state, new Commands.LookCommand());

        result.ShouldBeOfType<Results.LookResult>();
    }
}
