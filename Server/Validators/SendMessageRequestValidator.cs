namespace Server.Validators;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.Content).NotNull().SetInheritanceValidator(v =>
        {
            v.Add(new TextMessageContentDtoValidator());

        });
    }
}