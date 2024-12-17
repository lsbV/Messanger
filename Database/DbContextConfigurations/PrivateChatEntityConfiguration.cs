namespace Database.DbContextConfigurations;

internal class PrivateChatEntityConfiguration : IEntityTypeConfiguration<PrivateChat>
{
    public void Configure(EntityTypeBuilder<PrivateChat> builder)
    {
        builder.HasBaseType<Chat>();
        builder.Property(c => c.Id).HasConversion(c => c.Value, c => new ChatId(c));

        builder.Property(c => c.UserId1).HasConversion(c => c.Value, c => UserId.Of(c));
        builder.Property(c => c.UserId2).HasConversion(c => c.Value, c => UserId.Of(c));

        builder.HasOne(pc => pc.User1).WithMany().HasForeignKey(c => c.UserId1)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(pc => pc.User2).WithMany().HasForeignKey(c => c.UserId2)
            .OnDelete(DeleteBehavior.NoAction);

        builder.UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}