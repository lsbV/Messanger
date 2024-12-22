using Core.Exceptions;

namespace ChatComponent.Exceptions;

public class ChatNotFoundException(ChatId id)
    : EntityNotFoundException<ChatId>(id, nameof(Chat));