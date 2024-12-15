namespace Database.Configurations;

internal class ChatEntityConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.ChatId);
        builder.Property(c => c.ChatId).HasConversion(c => c.Value, c => ChatId.Of(c));
        builder.HasDiscriminator<string>("chat_type")
            .HasValue<PrivateChat>("private")
            .HasValue<GroupChat>("group");
    }
}