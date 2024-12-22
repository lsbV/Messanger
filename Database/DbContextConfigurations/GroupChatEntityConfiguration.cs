using Database.Models;

namespace Database.DbContextConfigurations;

internal class GroupChatEntityConfiguration : IEntityTypeConfiguration<GroupChat>
{
    public void Configure(EntityTypeBuilder<GroupChat> builder)
    {
        builder.HasBaseType<Chat>();

        builder
            .Property(c => c.Id)
            .HasConversion(c => c.Value, c => new ChatId(c));

        builder
            .Property(c => c.ChatName)
            .HasConversion(c => c.Value, c => new ChatName(c))
            .HasMaxLength(30);

        builder
            .Property(c => c.ChatDescription)
            .HasConversion(c => c.Value, c => new ChatDescription(c))
            .HasMaxLength(300);

        builder
            .Property(c => c.ChatImage)
            .HasConversion(c => c.Url, c => new ChatImage(c))
            .HasMaxLength(300);

        builder
            .Property(c => c.JoinMode)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(c => c.Users)
            .WithMany(u => u.GroupChats)
            .UsingEntity<GroupChatUser>();
    }
}