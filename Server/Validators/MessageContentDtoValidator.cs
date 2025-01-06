namespace Server.Validators;

public class MessageContentDtoValidator : AbstractValidator<MessageContentDto>
{
    public MessageContentDtoValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
    }
}