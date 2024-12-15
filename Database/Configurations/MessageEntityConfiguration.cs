using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Configurations;

internal class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasConversion(m => m.Value, m => MessageId.Of(m));
        builder.Property(m => m.SenderId).HasConversion(m => m.Value, m => UserId.Of(m));
        builder.Property(m => m.RecipientId).HasConversion(m => m.Value, m => ChatId.Of(m));
        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(m => m.Recipient).WithMany().HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Property(m => m.Status).HasConversion(new EnumToStringConverter<MessageStatus>());
        builder.Property(m => m.CreatedAt).HasConversion(new DateTimeToStringConverter());
        builder.Property(m => m.EditedAt).HasConversion(new DateTimeToStringConverter());
        builder.HasDiscriminator<string>("message_type")
            .HasValue<TextMessage>("text")
            .HasValue<ImageMessage>("image")
            .HasValue<VideoMessage>("video")
            .HasValue<FileMessage>("file");
    }
}