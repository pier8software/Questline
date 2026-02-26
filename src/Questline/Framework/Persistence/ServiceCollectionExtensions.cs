using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Questline.Framework.Persistence.Mongo;

namespace Questline.Framework.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoPersistence(this IServiceCollection services, string connectionString,
        string databaseName)
    {
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddSingleton<IDataContext, MongoDataContext>();

        return services;
    }
}
