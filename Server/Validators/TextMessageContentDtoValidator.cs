namespace Server.Validators;

public class TextMessageContentDtoValidator : AbstractValidator<TextMessageContentDto>
{
    public TextMessageContentDtoValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }

}