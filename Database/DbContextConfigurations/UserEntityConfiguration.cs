using System.Text.Json;

namespace Database.DbContextConfigurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .HasConversion(u => u.Value, u => new UserId(u));

        builder
            .Property(u => u.Name)
            .HasConversion(u => u.Value, u => new UserName(u))
            .HasMaxLength(50);

        builder
            .Property(u => u.Email)
            .HasConversion(u => u.Value, u => new Email(u))
            .HasMaxLength(255);

        builder
            .Property(u => u.AuthorizationVersion)
            .HasConversion(u => u.Value, u => new AuthorizationVersion(u));

        builder
            .Property(u => u.Avatar)
            .HasConversion(u => u.Url, u => new Avatar(u))
            .HasMaxLength(255);

        builder
            .Property(u => u.Password)
            .HasConversion(
                u => JsonSerializer.Serialize(u, (JsonSerializerOptions)null!),
                u => JsonSerializer.Deserialize<Password>(u, (JsonSerializerOptions)null!)!)
            .HasMaxLength(255);
    }
}