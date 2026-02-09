using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

ServiceCollection services = new();

// Build the test world
var rustyKey = new Item { Id = "key", Name = "rusty key", Description = "A rusty iron key." };
var torch = new Item { Id = "torch", Name = "torch", Description = "A flickering wooden torch." };

var world = new WorldBuilder()
    .WithRoom("entrance", "Dungeon Entrance", "A dark entrance to the dungeon. Cold air drifts from the north.", r =>
        r.WithExit(Direction.North, "hallway")
         .WithItem(rustyKey))
    .WithRoom("hallway", "Torch-Lit Hallway", "A hallway lined with flickering torches. Passages lead north and south.",
        r =>
        {
            r.WithExit(Direction.South, "entrance");
            r.WithExit(Direction.North, "chamber");
            r.WithItem(torch);
        })
    .WithRoom("chamber", "Great Chamber", "A vast chamber with vaulted ceilings. The only exit is to the south.", r =>
        r.WithExit(Direction.South, "hallway"))
    .Build();

GameState state = new(world, new Player { Id = "player1", Location = "entrance" });

// Configure dispatcher
CommandDispatcher dispatcher = new();
dispatcher.Register(["look", "l"], new LookCommandHandler(), _ => new LookCommand());
dispatcher.Register(["go", "walk", "move"], new GoCommandHandler(), args =>
{
    if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
    {
        return null;
    }

    return new GoCommand(dir);
});
dispatcher.Register(["get", "take"], new GetCommandHandler(), args =>
    args.Length == 0 ? null : new GetCommand(string.Join(" ", args)));
dispatcher.Register(["drop"], new DropCommandHandler(), args =>
    args.Length == 0 ? null : new DropCommand(string.Join(" ", args)));
dispatcher.Register(["inventory", "inv", "i"], new InventoryCommandHandler(),
    _ => new InventoryCommand());
dispatcher.Register(["quit", "exit", "q"], new QuitCommandHandler(), _ => new QuitCommand());

// Register services
services.AddSingleton(state);
services.AddSingleton(dispatcher);
services.AddSingleton(new Parser());
services.AddSingleton<IConsole, SystemConsole>();
services.AddSingleton<GameLoop>();

// Run
var provider = services.BuildServiceProvider();
var gameLoop = provider.GetRequiredService<GameLoop>();
gameLoop.Run();
