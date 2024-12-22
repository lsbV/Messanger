using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Database.MongoDbConfigurations;

public static class MessageConfiguration
{
    public static void ConfigureMessageEntity(this MongoClient client)
    {
        BsonClassMap.RegisterClassMap<Message>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
                .SetSerializer(new GuidFactorySerializer<MessageId>((value) => value.Value.ToString(), value => new MessageId(Guid.Parse(value))));

            cm.MapMember(c => c.SenderId)
                .SetSerializer(new GuidFactorySerializer<UserId>((value) => value.Value.ToString(), value => new UserId(Guid.Parse(value))));

            cm.MapMember(c => c.RecipientId)
                .SetSerializer(new GuidFactorySerializer<ChatId>((value) => value.Value.ToString(), value => new ChatId(Guid.Parse(value))));

            cm.MapMember(c => c.Status)
                .SetSerializer(new EnumSerializer<MessageStatus>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<MessageContent>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminatorIsRequired(true);
            cm.SetDiscriminator(nameof(MessageContent));
        });

        BsonClassMap.RegisterClassMap<TextContent>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator(nameof(TextContent));
        });

        BsonClassMap.RegisterClassMap<ImageContent>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator(nameof(ImageContent));
        });

        BsonClassMap.RegisterClassMap<VideoContent>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator(nameof(VideoContent));
        });

        BsonClassMap.RegisterClassMap<AudioContent>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator(nameof(AudioContent));
        });

        BsonClassMap.RegisterClassMap<FileContent>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator(nameof(FileContent));
        });


    }

}
public class GuidFactorySerializer<T>(
    Func<T, string> serializeFactoryMethod,
    Func<string, T> deserializeFactoryMethod
) : IBsonSerializer<T>
{
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        context.Writer.WriteString(serializeFactoryMethod(value));
    }

    public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return deserializeFactoryMethod(context.Reader.ReadString());
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        Serialize(context, args, (T)value);
    }

    public Type ValueType { get; } = typeof(T);
}