using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Cli;

public class GameStartFlowTests
{
    private static (CliApp app, FakeConsole console) CreateCliApp(FakeDice? dice = null)
    {
        var rooms = new GameBuilder()
            .WithRoom("entrance", "Dungeon Entrance", "A dark entrance to the dungeon.", r =>
                r.WithExit(Direction.North, "hallway"))
            .Build();

        var world = new WorldContent(rooms, new(), "entrance");

        dice ??= new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4);
        var factory = new CharacterFactory(dice);

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .AddSingleton<IRequestHandler<Requests.MovePlayerCommand>, MovePlayerCommandHandler>()
            .AddSingleton<IRequestHandler<Requests.QuitGame>, QuitGameHandler>()
            .AddSingleton<IRequestHandler<Requests.StatsQuery>, StatsQueryHandler>()
            .BuildServiceProvider();

        var dispatcher = new RequestSender(serviceProvider);
        var console = new FakeConsole();
        var parser = new Parser();

        var app = new CliApp(console, world, factory, parser, dispatcher);

        return (app, console);
    }

    [Fact]
    public void Prompts_for_character_name()
    {
        var (app, console) = CreateCliApp();
        console.QueueInput("Thorin", "quit");

        app.Run();

        console.AllOutput.ShouldContain("name");
    }

    [Fact]
    public void Displays_welcome_message_with_character_name()
    {
        var (app, console) = CreateCliApp();
        console.QueueInput("Thorin", "quit");

        app.Run();

        console.AllOutput.ShouldContain("Thorin");
    }

    [Fact]
    public void Displays_starting_room_after_character_creation()
    {
        var (app, console) = CreateCliApp();
        console.QueueInput("Thorin", "quit");

        app.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
    }

    [Fact]
    public void Reprompts_on_invalid_name()
    {
        var (app, console) = CreateCliApp();
        console.QueueInput("", "Thorin", "quit");

        app.Run();

        console.AllOutput.ShouldContain("Please give your character a name");
        console.AllOutput.ShouldContain("Thorin");
    }
}
