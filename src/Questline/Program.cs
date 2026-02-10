using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;
using Questline.Framework.Content;

ServiceCollection services = new();

// Load adventure from content files
var contentPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "content", "adventures", "five-room-dungeon");
contentPath = Path.GetFullPath(contentPath);

var loader = new FileSystemAdventureLoader();
var adventure = loader.Load(contentPath);

GameState state = new(adventure.World, new Player { Id = "player1", Location = adventure.StartingRoomId });

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
