namespace Server.Models.Dtos;

public record GroupChatDto(
    string Id,
    string Type,
    List<PublicUserInfoDto> Participants,
    string ChatName,
    string ChatDescription,
    string ChatImage,
    string JoinMode) : ChatDto(Id, Type);