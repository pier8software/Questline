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

        services.AddMongoPersistence("mongodb://localhost:27017", "questline", "0.6.0");

        RegisterCommandHandlers(services);

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        RegisterHandler<GetRoomDetailsQuery, GetRoomDetailsHandler>(services);
        RegisterHandler<MovePlayerCommand, MovePlayerCommandHandler>(services);
        RegisterHandler<TakeItemCommand, TakeItemHandler>(services);
        RegisterHandler<DropItemCommand, DropItemCommandHandler>(services);
        RegisterHandler<GetPlayerInventoryQuery, GetPlayerInventoryQueryHandler>(services);
        RegisterHandler<QuitGame, QuitGameHandler>(services);
        RegisterHandler<UseItemCommand, UseItemCommandHandler>(services);
        RegisterHandler<ExamineCommand, ExamineCommandHandler>(services);
        RegisterHandler<VersionQuery, VersionQueryHandler>(services);

        services.AddSingleton<RequestSender>();
    }

    private static void RegisterHandler<TRequest, THandler>(IServiceCollection services)
        where TRequest : IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        services.AddSingleton<THandler>();
        services.AddSingleton<IRequestHandler<TRequest>>(sp =>
            new AutoSaveDecorator<TRequest>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<IGameStateRepository>()));
    }
}
