using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Questline.Framework.Persistence.Mongo;

public static class MongoConventions
{
    private const string Label = "Questline";

    private static readonly IBsonSerializer[] Serializers =
    [
        new GuidSerializer(GuidRepresentation.Standard),
        // new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)),
        // new DecimalSerializer(BsonType.Decimal128),
        // new TimeSpanSerializer(),
        // new DateTimeSerializer(DateTimeKind.Utc),
        // new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Utc)),
        // new DateTimeOffsetSerializer(BsonType.String),
        // new NullableSerializer<DateTimeOffset>(new DateTimeOffsetSerializer(BsonType.String))
    ];

    private static readonly ConventionPack ConventionPack =
    [
        new CamelCaseElementNameConvention(),
        new IgnoreIfNullConvention(true),
        new IgnoreEmptyCollectionsConvention(),
        new EnumStringRepresentationConvention(),
        new IgnoreExtraElementsConvention(true)
    ];

    public static void RegisterConventions()
    {
        RegisterSerializers();
        RegisterConventionPack();
        RegisterBaseDocuments();

        return;

        static void RegisterConventionPack()
        {
            ConventionRegistry.Register(Label, ConventionPack, _ => true);
        }

        static void RegisterSerializers()
        {
            foreach (var serializer in Serializers)
            {
                BsonSerializer.TryRegisterSerializer((dynamic)serializer);
            }
        }

        static void RegisterBaseDocuments()
        {
            if (BsonClassMap
                .TryRegisterClassMap<Document<string>>(map =>
                {
                    map.AutoMap();
                    map.MapMember(x => x.ConcurrencyTag)
                        .SetElementName("_concurrencyTag");
                }))
            {
                BsonClassMap.LookupClassMap(typeof(Document<string>)).Freeze();
            }
        }
    }

    public sealed class EnumStringRepresentationConvention() : EnumRepresentationConvention(BsonType.String);

    public sealed class IgnoreEmptyCollectionsConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            if (!memberMap.MemberType.IsAssignableTo(typeof(IEnumerable))
                || memberMap.MemberType.IsAssignableTo(typeof(string))
                || memberMap.MemberType.IsAssignableTo(typeof(byte[])))
            {
                return;
            }

            // set the default value to an empty instance
            // so it can be deserialized from null
            memberMap.SetDefaultValue(() => Activator.CreateInstance(memberMap.MemberType));

            // if empty just skip serialization
            memberMap.SetShouldSerializeMethod(item =>
            {
                try
                {
                    if (item is null)
                    {
                        return false;
                    }

                    return memberMap
                        .Getter((dynamic)item)
                        ?
                        .GetEnumerator()
                        .MoveNext() ?? false;
                }
                catch (Exception)
                {
                    return true; // just in case...
                }
            });
        }
    }
}
