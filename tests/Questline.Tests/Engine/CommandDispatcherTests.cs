using Microsoft.Extensions.DependencyInjection;
using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine;

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
            .AddSingleton<ICommandHandler<LookCommand>, LookCommandHandler>()
            .BuildServiceProvider();


        var dispatcher = new CommandDispatcher(serviceProvider);

        var result = dispatcher.Dispatch(state, new LookCommand());

        result.ShouldBeOfType<LookResult>();
    }
}
