using Microsoft.Extensions.DependencyInjection;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Parsers;
using Questline.Framework.FileSystem;
using Questline.Framework.Mediator;
using Questline.Framework.Persistence;
using static Questline.Engine.Messages.Requests;

namespace Questline.Engine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestlineEngine(this IServiceCollection services)
    {
        services.AddSingleton<IGameContentLoader, GameContentLoader>();
        services.AddSingleton<JsonFileLoader>();
        services.AddSingleton<Parser>();
        services.AddSingleton<IDice, Dice>();
        services.AddSingleton<CharacterCreationStateMachine>();
        services.AddSingleton<GameEngine>();

        //xRegisterPersistence(services);

        RegisterCommandHandlers(services);

        return services;
    }

    private static void RegisterPersistence(IServiceCollection services) =>
        services.AddMongoPersistence("mongodb://localhost:27017", "questline");

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        services.AddSingleton<IRequestHandler<LoginCommand>, LoginCommandHandler>();
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
