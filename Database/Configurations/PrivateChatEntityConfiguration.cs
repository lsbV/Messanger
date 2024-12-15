namespace Database.Configurations;

internal class PrivateChatEntityConfiguration : IEntityTypeConfiguration<PrivateChat>
{
    public void Configure(EntityTypeBuilder<PrivateChat> builder)
    {
        builder.HasBaseType<Chat>();
        builder.Property(c => c.UserId1).HasConversion(c => c.Value, c => UserId.Of(c));
        builder.Property(c => c.UserId2).HasConversion(c => c.Value, c => UserId.Of(c));
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId1)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId2)
            .OnDelete(DeleteBehavior.NoAction);
    }
}