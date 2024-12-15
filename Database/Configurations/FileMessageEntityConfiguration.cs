namespace Database.Configurations;

internal class FileMessageEntityConfiguration : IEntityTypeConfiguration<FileMessage>
{
    public void Configure(EntityTypeBuilder<FileMessage> builder)
    {
        builder.HasBaseType<Message>();
        builder.Property(m => m.File).HasConversion(m => m.Url, m => FileContent.Of(m));
        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId);
        builder.HasOne(m => m.Recipient).WithMany().HasForeignKey(m => m.RecipientId);
    }
}