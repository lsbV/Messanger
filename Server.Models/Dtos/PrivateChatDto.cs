namespace Server.Models.Dtos;

public record PrivateChatDto(string Id, string Type, PublicUserInfoDto User1, PublicUserInfoDto User2) : ChatDto(Id, Type);