namespace Database.Configurations;

internal class TextMessageEntityConfiguration : IEntityTypeConfiguration<TextMessage>
{
    public void Configure(EntityTypeBuilder<TextMessage> builder)
    {
        builder.HasBaseType<Message>();
        builder.Property(m => m.Text).HasConversion(m => m.Value, m => TextContent.Of(m)).HasMaxLength(255);
    }
}