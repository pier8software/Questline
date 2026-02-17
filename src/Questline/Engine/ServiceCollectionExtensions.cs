using Microsoft.Extensions.DependencyInjection;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Rooms.Handlers;
using Questline.Domain.Shared.Handlers;
using Questline.Engine.InputParsers;
using Questline.Framework.FileSystem;
using Questline.Framework.Mediator;
using static Questline.Domain.Rooms.Messages.Requests;
using static Questline.Domain.Rooms.Messages.Responses;
using static Questline.Domain.Players.Messages.Requests;
using static Questline.Domain.Players.Messages.Responses;
using static Questline.Domain.Shared.Messages.Requests;
using static Questline.Domain.Shared.Messages.Responses;

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
        services.AddSingleton<IRequestHandler<GetRoomDetailsQuery, RoomDetailsResponse>, GetRoomDetailsHandler>();
        services.AddSingleton<IRequestHandler<MovePlayerCommand, PlayerMovedResponse>, MovePlayerCommandHandler>();
        services.AddSingleton<IRequestHandler<TakeRoomItemCommand, ItemTakenResponse>, TakeRoomItemHandler>();
        services.AddSingleton<IRequestHandler<DropItemCommand, ItemDroppedResponse>, DropItemCommandHandler>();
        services
            .AddSingleton<IRequestHandler<GetPlayerInventoryQuery, PlayerInventoryResponse>,
                GetPlayerInventoryQueryHandler>();
        services.AddSingleton<IRequestHandler<QuitGame, GameQuited>, QuitGameHandler>();

        services.AddSingleton<RequestSender>();
    }
}
