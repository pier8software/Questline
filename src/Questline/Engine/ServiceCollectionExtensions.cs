using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain;
using Questline.Engine.Commands;
using Questline.Engine.InputParsers;
using Questline.Framework.Content;

namespace Questline.Engine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestlineEngine(this IServiceCollection services)
    {
        // Load adventure from content files
        var contentPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "content", "adventures",
            "five-room-dungeon");
        contentPath = Path.GetFullPath(contentPath);

        var loader = new FileSystemAdventureLoader();
        var adventure = loader.Load(contentPath);

        GameState state = new(adventure.World, new Player { Id = "player1", Location = adventure.StartingRoomId });

        // Build Parser
        var parser = new ParserBuilder()
            .RegisterCommand<LookCommand>(["look", "l"], _ => new LookCommand())
            .RegisterCommand<GoCommand>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new GoCommand(dir);
            })
            .RegisterCommand<GetCommand>(["get", "take"],
                args => args.Length == 0
                    ? new ParseError("Item name required.")
                    : new GetCommand(string.Join(" ", args)))
            .RegisterCommand<DropCommand>(["drop"], args =>
                args.Length == 0 ? new ParseError("Item name required.") : new DropCommand(string.Join(" ", args)))
            .RegisterCommand<InventoryCommand>(["inventory", "inv", "i"], _ => new InventoryCommand())
            .RegisterCommand<QuitCommand>(["quit", "exit", "q"], _ => new QuitCommand())
            .Build();

        services.AddSingleton(state);
        services.AddSingleton<CommandDispatcher>();
        services.AddSingleton(parser);
        services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<GameLoop>();

        return services;
    }
}
