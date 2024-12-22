namespace Database.DbContextConfigurations;

internal class ChatEntityConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(c => c.Value, c => new ChatId(c));
        builder.HasMany(c => c.Events).WithOne().HasForeignKey(e => e.ChatId);

        builder.UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.UseTpcMappingStrategy();
    }
}