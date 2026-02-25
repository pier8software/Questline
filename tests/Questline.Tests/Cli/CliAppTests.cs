using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Cli;

public class CliAppTests
{
    // 3d6 x 6 ability scores = 18 rolls, then 1d8 for HP = 19 rolls total
    private static readonly int[] DefaultDiceRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    // Character creation inputs: select class (Fighter), select race (Human), continue (HP roll), enter name
    private static readonly string[] CharacterCreationInputs = ["1", "1", "", "Hero"];

    private static (CliApp app, FakeConsole console, FakeGameStateRepository repository) CreateCliApp()
    {
        var worldContent = new GameBuilder()
            .WithRoom("entrance", "Dungeon Entrance", "A dark entrance to the dungeon.", r =>
                r.WithExit(Direction.North, "hallway"))
            .WithRoom("hallway", "Torch-Lit Hallway", "A hallway lined with flickering torches.", r =>
            {
                r.WithExit(Direction.South, "entrance");
                r.WithExit(Direction.North, "chamber");
            })
            .WithRoom("chamber", "Great Chamber", "A vast chamber with vaulted ceilings.", r =>
                r.WithExit(Direction.South, "hallway"))
            .BuildWorldContent("entrance");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .AddSingleton<IRequestHandler<Requests.MovePlayerCommand>, MovePlayerCommandHandler>()
            .AddSingleton<IRequestHandler<Requests.QuitGame>, QuitGameHandler>()
            .BuildServiceProvider();

        var console = new FakeConsole();
        var repository = new FakeGameStateRepository();

        var dice = new FakeDice(DefaultDiceRolls);
        var stateMachine = new CharacterCreationStateMachine(dice);

        var contentLoader = new FakeGameContentLoader(worldContent);
        var dispatcher = new RequestSender(serviceProvider);
        var parser = new Parser();
        var gameEngine = new GameEngine(parser, dispatcher, contentLoader, repository);

        var app = new CliApp(console, stateMachine, gameEngine);

        return (app, console, repository);
    }

    [Fact]
    public void Displays_initial_room_on_start()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "quit"]);

        loop.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public void Displays_command_prompt()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "quit"]);

        loop.Run();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public void Look_command_displays_room_info()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "look", "quit"]);

        loop.Run();

        var output = console.AllOutput;
        var count = CountOccurrences(output, "Dungeon Entrance");
        count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Go_command_moves_and_displays_new_room()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "go north", "quit"]);

        loop.Run();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public void Unknown_command_displays_error()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "dance", "quit"]);

        loop.Run();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public void Quit_command_exits_gracefully()
    {
        var (loop, console, _) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "quit"]);

        loop.Run();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public void Null_input_exits_loop()
    {
        var (loop, _, _) = CreateCliApp();
        // No input queued, ReadLine returns null at name prompt

        loop.Run();

        // Should not hang — exits when input is null
    }

    [Fact]
    public void Saves_world_state_before_character_creation()
    {
        var (loop, console, repository) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "quit"]);

        loop.Run();

        repository.SaveCalls.Count.ShouldBeGreaterThanOrEqualTo(2);
        repository.SaveCalls[0].Player.ShouldBeNull();
    }

    [Fact]
    public void Saves_character_to_profile_after_creation()
    {
        var (loop, console, repository) = CreateCliApp();
        console.QueueInput([..CharacterCreationInputs, "quit"]);

        loop.Run();

        repository.SaveCalls.Count.ShouldBeGreaterThanOrEqualTo(2);
        repository.SaveCalls[1].Player.ShouldNotBeNull();
        repository.SaveCalls[1].Player!.Character.Name.ShouldBe("Hero");
    }

    private static int CountOccurrences(string text, string search)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(search, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += search.Length;
        }

        return count;
    }
}
