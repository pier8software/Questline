using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Questline.Framework.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoPersistence(this IServiceCollection services, string connectionString, string databaseName, string version)
    {
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(databaseName);
            return database.GetCollection<GameStateDocument>("game_states");
        });
        services.AddSingleton<IGameStateRepository>(sp =>
            new MongoGameStateRepository(
                sp.GetRequiredService<IMongoCollection<GameStateDocument>>(),
                version));

        return services;
    }
}
