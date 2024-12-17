namespace Database.DbContextConfigurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasConversion(u => u.Value, u => UserId.Of(u));
        builder.Property(u => u.Name).HasConversion(u => u.Value, u => UserName.Of(u));
        builder.Property(u => u.Email).HasConversion(u => u.Value, u => Email.Of(u));
        builder.Property(u => u.AuthorizationVersion).HasConversion(u => u.Value, u => AuthorizationVersion.Of(u));
    }
}