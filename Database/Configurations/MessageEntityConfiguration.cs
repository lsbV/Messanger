using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Configurations;

internal class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasConversion(m => m.Value, m => MessageId.Of(m));
        builder.Property(m => m.SenderId).HasConversion(m => m.Value, m => UserId.Of(m));
        builder.Property(m => m.RecipientId).HasConversion(m => m.Value, m => new ChatId(m));
        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(m => m.Recipient).WithMany().HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Property(m => m.Status).HasConversion(new EnumToStringConverter<MessageStatus>());
        builder.Property(m => m.CreatedAt).HasConversion(new DateTimeToStringConverter());
        builder.Property(m => m.EditedAt).HasConversion(new DateTimeToStringConverter());

        builder.Property(m => m.Content)
            .HasConversion(
                messageContent => ConvertContent(messageContent),
                content => ConvertContent(content)
            );

    }

    public static MessageContent ConvertContent(string content)
    {
        return content.Split(':', 2)[0] switch
        {
            "Text" => TextContent.Of(content.Split(':', 2)[1]),
            "Image" => ImageContent.Of(content.Split(':', 2)[1]),
            "Video" => VideoContent.Of(content.Split(':', 2)[1]),
            "Audio" => AudioContent.Of(content.Split(':', 2)[1]),
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
    }

    public static string ConvertContent(MessageContent content)
    {
        return content switch
        {
            TextContent textContent => $"Text:{textContent.Value}",
            ImageContent imageContent => $"Image:{imageContent.Url}",
            VideoContent videoContent => $"Video:{videoContent.Url}",
            AudioContent audioContent => $"Audio:{audioContent.Url}",
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
    }
}