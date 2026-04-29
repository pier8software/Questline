using Microsoft.Extensions.DependencyInjection;
using Questline.Domain.Adventures.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Parsers;
using Questline.Engine.Persistence.Adventures;
using Questline.Engine.Persistence.Playthroughs;
using Questline.Engine.Persistence.Rooms;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;
using Questline.Framework.Persistence;
using static Questline.Engine.Messages.Requests;

namespace Questline.Engine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuestlineEngine(this IServiceCollection services)
    {
        services.AddSingleton<Parser>();
        services.AddSingleton<IDice, Dice>();
        services.AddSingleton<PartyCreationStateMachine>();
        services.AddSingleton<GameEngine>();

        services.AddSingleton<IGameSession, GameSession>();

        services.AddSingleton<IPersistenceMapper<Adventure, AdventureDocument>, AdventureMapper>();
        services.AddSingleton<IPersistenceMapper<Room, RoomDocument>, RoomMapper>();
        services.AddSingleton<IPersistenceMapper<Playthrough, PlaythroughDocument>, PlaythroughMapper>();

        services.AddSingleton<IAdventureRepository, AdventureRepository>();
        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddSingleton<IPlaythroughRepository, PlaythroughRepository>();

        RegisterCommandHandlers(services);

        return services;
    }

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
        services.AddSingleton<IRequestHandler<StatsQuery>, StatsQueryHandler>();

        services.AddSingleton<RequestSender>();
    }
}
