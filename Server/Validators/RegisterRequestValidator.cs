namespace Server.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().Matches(RawPassword.Regex);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(15);
        RuleFor(x => x.Avatar).NotNull().Must(x => x.ContentType.Contains("image"));
    }
}