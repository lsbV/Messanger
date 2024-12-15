namespace Database.Configurations;

internal class VideoMessageEntityConfiguration : IEntityTypeConfiguration<VideoMessage>
{
    public void Configure(EntityTypeBuilder<VideoMessage> builder)
    {
        builder.HasBaseType<Message>();
        builder.Property(m => m.Video).HasConversion(m => m.Url, m => VideoContent.Of(m)).HasMaxLength(255);
    }
}