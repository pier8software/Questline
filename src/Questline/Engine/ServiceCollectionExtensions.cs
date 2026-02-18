using Microsoft.Extensions.DependencyInjection;
using Questline.Engine.Handlers;
using Questline.Engine.Parsers;
using Questline.Framework.FileSystem;
using Questline.Framework.Mediator;
using static Questline.Engine.Messages.Requests;

namespace Questline.Engine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestlineEngine(this IServiceCollection services)
    {
        var loader = new GameContentLoader(new JsonFileLoader());
        var state = loader.Load();

        services.AddSingleton(state);
        services.AddSingleton(new Parser());
        services.AddSingleton<GameEngine>();

        RegisterCommandHandlers(services);

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        services.AddSingleton<IRequestHandler<GetRoomDetailsQuery>, GetRoomDetailsHandler>();
        services.AddSingleton<IRequestHandler<MovePlayerCommand>, MovePlayerCommandHandler>();
        services.AddSingleton<IRequestHandler<TakeItemCommand>, TakeItemHandler>();
        services.AddSingleton<IRequestHandler<DropItemCommand>, DropItemCommandHandler>();
        services.AddSingleton<IRequestHandler<GetPlayerInventoryQuery>, GetPlayerInventoryQueryHandler>();
        services.AddSingleton<IRequestHandler<QuitGame>, QuitGameHandler>();
        services.AddSingleton<IRequestHandler<UseItemCommand>, UseItemCommandHandler>();
        services.AddSingleton<IRequestHandler<ExamineCommand>, ExamineCommandHandler>();
        services.AddSingleton<IRequestHandler<VersionQuery>, VersionQueryHandler>();

        services.AddSingleton<RequestSender>();
    }
}
