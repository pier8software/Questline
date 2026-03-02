using MongoDB.Driver.Core.Misc;
using Humanizer;
using static Questline.Framework.Persistence.Mongo.MongoNamingConventions;
using static System.StringComparison;

namespace Questline.Framework.Persistence.Mongo;

public enum MongoNamingConventions
{
    Undefined,
    PascalCase,
    CamelCase,
    SnakeCase,
    KebabCase
}

public record MongoCollectionName
{
    private static readonly string[] ForbiddenSuffixes =
    [
        "Document",
        "View",
        "Projection",
        "ProjectionDocument",
        "ProjectedDocument",
        "EntityProjection",
        "ProjectedEntity",
        "Documents",
        "Entities",
        "Views",
        "Projections",
        "ProjectionDocuments",
        "EntityProjections",
        "ProjectedEntities"
    ];

    private MongoCollectionName(string value, string originalValue, MongoNamingConventions convention)
    {
        Value         = value;
        OriginalValue = originalValue;
        Convention    = convention;
    }

    public string                 Value         { get; private init; }
    public string                 OriginalValue { get; private init; }
    public MongoNamingConventions Convention    { get; private init; }

    public static MongoCollectionName For(string name, MongoNamingConventions convention = KebabCase)
    {
        Ensure.IsNotNullOrEmpty(name, nameof(name));

        var collectionName = CleanupName(name);

        return convention switch
        {
            PascalCase => new MongoCollectionName(collectionName.Pascalize().Pluralize(), name, PascalCase),
            CamelCase  => new MongoCollectionName(collectionName.Camelize().Pluralize(), name, CamelCase),
            SnakeCase  => new MongoCollectionName(collectionName.Underscore().Pluralize().ToLower(), name, SnakeCase),
            KebabCase  => new MongoCollectionName(collectionName.Kebaberize().Pluralize().ToLower(), name, KebabCase),
            _          => new MongoCollectionName(collectionName, name, Undefined)
        };

        static string CleanupName(string collectionName)
        {
            foreach (var suffix in ForbiddenSuffixes)
            {
                if (collectionName.EndsWith(suffix, OrdinalIgnoreCase)
                 && collectionName.Length > suffix.Length)
                {
                    collectionName = collectionName[..^suffix.Length];
                }
            }

            return collectionName;
        }
    }

    public static MongoCollectionName For(Type type, MongoNamingConventions convention = KebabCase) =>
        For(type.Name, convention);

    public static MongoCollectionName For<T>(MongoNamingConventions convention = KebabCase) =>
        For(typeof(T), convention);

    public override string ToString() =>
        Value;

    public static implicit operator string(MongoCollectionName _) =>
        _.ToString();
}
