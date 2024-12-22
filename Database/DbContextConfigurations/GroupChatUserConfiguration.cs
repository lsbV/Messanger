using Database.Models;

namespace Database.DbContextConfigurations;

internal class GroupChatUserConfiguration : IEntityTypeConfiguration<GroupChatUser>
{
    public void Configure(EntityTypeBuilder<GroupChatUser> builder)
    {
        builder
            .HasKey(gcu => new { gcu.GroupChatId, gcu.UserId });

        builder
            .Property(m => m.GroupChatRole)
            .HasConversion(
                c => c.ToString(),
                c => Enum.Parse<GroupChatRole>(c))
            .IsRequired();

        builder
            .Property(m => m.GroupChatId)
            .HasConversion(
                c => c.Value,
                c => new ChatId(c));

        builder
            .Property(m => m.UserId)
            .HasConversion(
                c => c.Value,
                c => new UserId(c));
    }
}