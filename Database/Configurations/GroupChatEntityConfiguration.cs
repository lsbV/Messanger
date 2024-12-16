namespace Database.Configurations;

internal class GroupChatEntityConfiguration : IEntityTypeConfiguration<GroupChat>
{
    public void Configure(EntityTypeBuilder<GroupChat> builder)
    {
        builder.HasBaseType<Chat>();
        builder.Property(c => c.Id).HasConversion(c => c.Value, c => new ChatId(c));
        builder.Property(c => c.ChatName).HasConversion(c => c.Value, c => ChatName.Of(c));

        builder.UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.Users).WithMany(u => u.GroupChats).UsingEntity<GroupChatUser>();
    }
}

internal class ChatEntityConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(c => c.Value, c => new ChatId(c));
        builder.UseTpcMappingStrategy();

    }
}
public record GroupChatUser(ChatId GroupChatId, UserId UserId, Role Role);

internal class GroupChatUserConfiguration : IEntityTypeConfiguration<GroupChatUser>
{
    public void Configure(EntityTypeBuilder<GroupChatUser> builder)
    {
        builder.HasKey(gcu => new { gcu.GroupChatId, gcu.UserId });
        builder.HasOne<User>().WithMany().HasForeignKey(gcu => gcu.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<GroupChat>().WithMany().HasForeignKey(gcu => gcu.GroupChatId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(m => m.Role)
            .HasConversion(r => r.Value, r => Role.Of(r))
            .HasMaxLength(20).IsRequired();
        builder.Property(m => m.GroupChatId)
            .HasConversion(c => c.Value, c => new ChatId(c));
        builder.Property(m => m.UserId)
            .HasConversion(c => c.Value, c => UserId.Of(c));


    }
}