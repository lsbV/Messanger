namespace Database.Configurations;

internal class GroupChatUserEntityConfiguration : IEntityTypeConfiguration<GroupChatUser>
{
    public void Configure(EntityTypeBuilder<GroupChatUser> builder)
    {
        builder.HasKey(gcu => new { gcu.UserId, gcu.GroupChatId });
        builder.HasOne<User>().WithMany().HasForeignKey(gcu => gcu.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<GroupChat>().WithMany().HasForeignKey(gcu => gcu.GroupChatId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}