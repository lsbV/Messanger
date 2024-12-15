namespace Database.Configurations;

internal class ImageMessageEntityConfiguration : IEntityTypeConfiguration<ImageMessage>
{
    public void Configure(EntityTypeBuilder<ImageMessage> builder)
    {
        builder.HasBaseType<Message>();
        builder.Property(m => m.Image).HasConversion(m => m.Url, m => ImageContent.Of(m)).HasMaxLength(255);
    }
}