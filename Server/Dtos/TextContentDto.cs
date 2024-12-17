internal record TextContentDto(
    string Type,
    string Text) : ContentDto(Type);