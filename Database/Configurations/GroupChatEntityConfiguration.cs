namespace Database.Configurations;

internal class GroupChatEntityConfiguration : IEntityTypeConfiguration<GroupChat>
{
    public void Configure(EntityTypeBuilder<GroupChat> builder)
    {
        builder.HasBaseType<Chat>();
        builder.Property(c => c.ChatName).HasConversion(c => c.Value, c => ChatName.Of(c));
        builder.HasMany<User>().WithMany().UsingEntity<GroupChatUser>();
    }
}
public record GroupChatUser(ChatId GroupChatId, UserId UserId);