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
        services.AddSingleton<ICommandHandler<Commands.ViewRoom>, ViewRoomHandler>();
        services.AddSingleton<ICommandHandler<Commands.MovePlayer>, MovePlayerHandler>();
        services.AddSingleton<ICommandHandler<Commands.TakeItem>, TakeItemHandler>();
        services.AddSingleton<ICommandHandler<Commands.DropItem>, DropItemHandler>();
        services.AddSingleton<ICommandHandler<Commands.LoadInventory>, LoadInventoryHandler>();
        services.AddSingleton<ICommandHandler<Commands.QuitGame>, QuitGameHandler>();

        services.AddSingleton<CommandDispatcher>();
    }

    private static void RegisterInputParser(IServiceCollection services)
    {
        var parser = new ParserBuilder()
            .RegisterCommand<Commands.ViewRoom>(["look", "l"], _ => new Commands.ViewRoom())
            .RegisterCommand<Commands.MovePlayer>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new Commands.MovePlayer(dir);
            })
            .RegisterCommand<Commands.TakeItem>(["get", "take"],
                args => args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Commands.TakeItem(string.Join(" ", args)))
            .RegisterCommand<Commands.DropItem>(["drop"], args =>
                args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Commands.DropItem(string.Join(" ", args)))
            .RegisterCommand<Commands.LoadInventory>(["inventory", "inv", "i"], _ => new Commands.LoadInventory())
            .RegisterCommand<Commands.QuitGame>(["quit", "exit", "q"], _ => new Commands.QuitGame())
            .Build();

        services.AddSingleton(parser);
    }
}
