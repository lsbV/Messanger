

internal static class MappingExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        var contentAndType = message.Content switch
        {
            TextContent text => (text.Value, nameof(TextContent)),
            _ => throw new ArgumentOutOfRangeException(nameof(message.Content))

        };
        var dto = new MessageDto(
            message.Id.Value.ToString(),
            message.SenderId.Value.ToString(),
            message.RecipientId.Value.ToString(),
            message.Content.ToDto(),
            message.Status.ToString(),
            message.CreatedAt.ToString(CultureInfo.CurrentCulture),
            message.EditedAt?.ToString()
        );

        return dto;

    }

    public static ContentDto ToDto(this MessageContent content)
    {
        return content switch
        {
            TextContent text => new TextContentDto("Text", text.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
    }

    public static ChatDto ToDto(this Chat chat)
    {
        ChatDto dto = chat switch
        {
            PrivateChat privateChat => new PrivateChatDto(
                privateChat.Id.Value.ToString(),
                nameof(PrivateChat),
                privateChat.User1.ToDto(),
                privateChat.User2.ToDto()

            ),
            GroupChat groupChat => new GroupChatDto(
                groupChat.Id.Value.ToString(),
                nameof(GroupChat),
                groupChat.Users.Select(u => u.ToDto()).ToList(),
                groupChat.ChatName.Value,
                groupChat.ChatDescription.Value,
                groupChat.ChatImage.Url,
                groupChat.JoinMode.ToString()
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(chat))
        };
        return dto;
    }

    public static PublicUserInfoDto ToDto(this User? user)
    {
        return user == null
            ? throw new ArgumentNullException(nameof(user))
            : new PublicUserInfoDto(
                user.Id.Value.ToString(),
                user.Name.Value,
                user.Avatar.Url
            );
    }

}