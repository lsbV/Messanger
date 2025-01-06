namespace Server.Validators;

public class CreateGroupChatRequestValidator : AbstractValidator<CreateGroupChatRequest>
{
    public CreateGroupChatRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.JoinMode).NotEmpty().IsEnumName(typeof(GroupChatJoinMode));
        RuleFor(x => x.Members).NotEmpty().ForEach(x => x.NotEmpty());
    }
}