using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.InputParsers;
using Questline.Engine.Messages;
using Questline.Framework.Content;
using Questline.Framework.Mediator;

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
        services.AddSingleton(state);

        RegisterInputParser(services);
        RegisterCommandHandlers(services);

        services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<GameLoop>();

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        services.AddSingleton<ICommandHandler<Commands.LookCommand>, LookCommandHandler>();
        services.AddSingleton<ICommandHandler<Commands.GoCommand>, GoCommandHandler>();
        services.AddSingleton<ICommandHandler<Commands.GetCommand>, GetCommandHandler>();
        services.AddSingleton<ICommandHandler<Commands.DropCommand>, DropCommandHandler>();
        services.AddSingleton<ICommandHandler<Commands.InventoryCommand>, InventoryCommandHandler>();
        services.AddSingleton<ICommandHandler<Commands.QuitCommand>, QuitCommandHandler>();

        services.AddSingleton<CommandDispatcher>();
    }

    private static void RegisterInputParser(IServiceCollection services)
    {
        var parser = new ParserBuilder()
            .RegisterCommand<Commands.LookCommand>(["look", "l"], _ => new Commands.LookCommand())
            .RegisterCommand<Commands.GoCommand>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new Commands.GoCommand(dir);
            })
            .RegisterCommand<Commands.GetCommand>(["get", "take"],
                args => args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Commands.GetCommand(string.Join(" ", args)))
            .RegisterCommand<Commands.DropCommand>(["drop"], args =>
                args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Commands.DropCommand(string.Join(" ", args)))
            .RegisterCommand<Commands.InventoryCommand>(["inventory", "inv", "i"], _ => new Commands.InventoryCommand())
            .RegisterCommand<Commands.QuitCommand>(["quit", "exit", "q"], _ => new Commands.QuitCommand())
            .Build();

        services.AddSingleton(parser);
    }
}
