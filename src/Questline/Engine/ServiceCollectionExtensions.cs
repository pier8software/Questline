using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Handlers;
using Questline.Domain.Messages;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Queries;
using Questline.Domain.Rooms.Handlers;
using Questline.Domain.Rooms.Queries;
using Questline.Engine.InputParsers;
using Questline.Framework.FileSystem;
using Questline.Framework.Mediator;

namespace Questline.Engine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestlineEngine(this IServiceCollection services)
    {
        var loader = new GameContentLoader(new JsonFileLoader());
        var state = loader.Load();

        services.AddSingleton(state);

        RegisterInputParser(services);
        RegisterCommandHandlers(services);

        services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<GameLoop>();

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        services.AddSingleton<ICommandHandler<Domain.Rooms.Messages.Commands.ViewRoom>, ViewRoomQuery>();
        services.AddSingleton<ICommandHandler<Domain.Players.Messages.Commands.MovePlayer>, MovePlayerHandler>();
        services.AddSingleton<ICommandHandler<Domain.Rooms.Messages.Commands.TakeRoomItem>, TakeRoomItemHandler>();
        services.AddSingleton<ICommandHandler<Domain.Players.Messages.Commands.DropPlayerItem>, DropPlayerItemHandler>();
        services.AddSingleton<ICommandHandler<Domain.Players.Messages.Commands.LoadPlayerInventory>, LoadPlayerInventoryQuery>();
        services.AddSingleton<ICommandHandler<Commands.QuitGame>, QuitGameHandler>();

        services.AddSingleton<CommandDispatcher>();
    }

    private static void RegisterInputParser(IServiceCollection services)
    {
        var parser = new ParserBuilder()
            .RegisterCommand<Domain.Rooms.Messages.Commands.ViewRoom>(["look", "l"], _ => new Domain.Rooms.Messages.Commands.ViewRoom())
            .RegisterCommand<Domain.Players.Messages.Commands.MovePlayer>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new Domain.Players.Messages.Commands.MovePlayer(dir);
            })
            .RegisterCommand<Domain.Rooms.Messages.Commands.TakeRoomItem>(["get", "take"],
                args => args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Domain.Rooms.Messages.Commands.TakeRoomItem(string.Join(" ", args)))
            .RegisterCommand<Domain.Players.Messages.Commands.DropPlayerItem>(["drop"], args =>
                args.Length == 0
                    ? new ParseError("Item name required.")
                    : new Domain.Players.Messages.Commands.DropPlayerItem(string.Join(" ", args)))
            .RegisterCommand<Domain.Players.Messages.Commands.LoadPlayerInventory>(["inventory", "inv", "i"], _ => new Domain.Players.Messages.Commands.LoadPlayerInventory())
            .RegisterCommand<Commands.QuitGame>(["quit", "exit", "q"], _ => new Commands.QuitGame())
            .Build();

        services.AddSingleton(parser);
    }
}
