using MongoDB.Driver;

namespace Questline.Framework.Persistence.Mongo;

public static class MongoCollectionExtensions
{
    public static IMongoCollection<T> Documents<T>(
        this IMongoDatabase database) where T : Document =>
        database.GetCollection<T>(GetCollectionName(typeof(T).Name));

    public static async ValueTask<T> Load<T>(
        this IMongoCollection<T> collection,
        string                   id,
        CancellationToken        cancellationToken = default) where T : Document
    {
        var document = await collection
            .Find(x => x.Id == id)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        if (document is null)
        {
            throw new KeyNotFoundException($"Document with ID '{id}' not found.");
        }

        return document;
    }

    public static async Task<ReplaceOneResult> StoreDocument<T>(
        this IMongoCollection<T> collection,
        T                        document,
        CancellationToken        cancellationToken = default) where T : Document
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document), "Document cannot be null.");
        }

        var replaceOptions = new ReplaceOptions { IsUpsert = true };
        if (document.ConcurrencyTag.GetValueOrDefault() != Guid.Empty)
        {
            replaceOptions.IsUpsert = false;
        }

        document.ConcurrencyTag = Guid.NewGuid();
        document.UpdatedAt      = TimeProvider.System.GetUtcNow().UtcDateTime;

        var result = await collection
            .ReplaceOneAsync(
                Builders<T>.Filter.Eq(x => x.Id, document.Id),
                document,
                replaceOptions,
                cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private static string GetCollectionName(string documentTypeName)
    {
        var documentSuffix = "document";
        if (documentTypeName.EndsWith(documentSuffix, StringComparison.OrdinalIgnoreCase) &&
            documentTypeName.Length > documentSuffix.Length)
        {
            return documentTypeName[..^documentSuffix.Length];
        }

        return documentTypeName;
    }
}
