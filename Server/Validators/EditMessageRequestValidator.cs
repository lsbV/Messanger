namespace Server.Validators;

public class EditMessageRequestValidator : AbstractValidator<EditMessageRequest>
{
    public EditMessageRequestValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.Content).NotNull().SetInheritanceValidator(v =>
        {
            v.Add(new TextMessageContentDtoValidator());
        });
    }
}