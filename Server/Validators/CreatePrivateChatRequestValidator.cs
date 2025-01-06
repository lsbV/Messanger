namespace Server.Validators;

public class CreatePrivateChatRequestValidator : AbstractValidator<CreatePrivateChatRequest>
{
    public CreatePrivateChatRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Message).NotNull();
    }
}