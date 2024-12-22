namespace Database.DbContextConfigurations;

internal class ChatEventEntityConfiguration : IEntityTypeConfiguration<ChatEvent>
{
    public void Configure(EntityTypeBuilder<ChatEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(e => e.Value, e => new ChatEventId(e));

        builder.Property(e => e.ChatId)
            .HasConversion(e => e.Value, e => new ChatId(e));

        builder.Property(e => e.CreatedAt)
            .HasConversion(e => e, e => DateTime.SpecifyKind(e, DateTimeKind.Utc));

        builder.HasOne<Chat>()
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.ChatId);

        builder.HasIndex(e => e.CreatedAt);

        builder.HasDiscriminator<string>("event_type")
            .HasValue<UserJoinedGroupChatEvent>("user_joined_group_chat")
            .HasValue<UserLeftGroupChatEvent>("user_left_group_chat")
            .HasValue<GroupChatImageUpdatedEvent>("group_chat_image_updated")
            .HasValue<GroupChatNameUpdatedEvent>("group_chat_name_updated")
            .HasValue<GroupChatDescriptionUpdatedEvent>("group_chat_description_updated");

        builder.UseTphMappingStrategy();
    }
}

internal class UserJoinedGroupChatEventEntityConfiguration : IEntityTypeConfiguration<UserJoinedGroupChatEvent>
{
    public void Configure(EntityTypeBuilder<UserJoinedGroupChatEvent> builder)
    {
        builder.HasBaseType<ChatEvent>();

        builder.Property(e => e.UserId)
            .HasConversion(e => e.Value, e => new UserId(e))
            .HasColumnName("joined_user_id");
    }
}

internal class UserLeftGroupChatEventEntityConfiguration : IEntityTypeConfiguration<UserLeftGroupChatEvent>
{
    public void Configure(EntityTypeBuilder<UserLeftGroupChatEvent> builder)
    {
        builder.HasBaseType<ChatEvent>();

        builder.Property(e => e.UserId)
            .HasConversion(e => e.Value, e => new UserId(e))
            .HasColumnName("left_user_id");
    }
}

internal class GroupChatImageUpdatedEventEntityConfiguration : IEntityTypeConfiguration<GroupChatImageUpdatedEvent>
{
    public void Configure(EntityTypeBuilder<GroupChatImageUpdatedEvent> builder)
    {
        builder.HasBaseType<ChatEvent>();

        builder.Property(e => e.ChatImage)
            .HasConversion(e => e.Url, e => new ChatImage(e))
            .HasMaxLength(300);
    }
}

internal class GroupChatNameUpdatedEventEntityConfiguration : IEntityTypeConfiguration<GroupChatNameUpdatedEvent>
{
    public void Configure(EntityTypeBuilder<GroupChatNameUpdatedEvent> builder)
    {
        builder.HasBaseType<ChatEvent>();

        builder.Property(e => e.ChatName)
            .HasConversion(e => e.Value, e => new ChatName(e))
            .HasMaxLength(100);
    }
}

internal class GroupChatDescriptionUpdatedEventEntityConfiguration : IEntityTypeConfiguration<GroupChatDescriptionUpdatedEvent>
{
    public void Configure(EntityTypeBuilder<GroupChatDescriptionUpdatedEvent> builder)
    {
        builder.HasBaseType<ChatEvent>();

        builder.Property(e => e.ChatDescription)
            .HasConversion(e => e.Value, e => new ChatDescription(e))
            .HasMaxLength(500);
    }
}