namespace SocialBackend.Controllers;

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.SendingUserId).NotEmpty().GreaterThan(0).WithMessage("SendingUserId must be a real number greater than 0");

        RuleFor(x => x.SendingUserName).NotEmpty()
        .MinimumLength(3).WithMessage("SendingUsername must be atleast three characters")
        .MaximumLength(50).WithMessage("SendingUsername must be at most 50 characters");

        RuleFor(x => x.ReceivingUserId).NotEmpty().GreaterThan(0).WithMessage("ReceivingUserId must be a real number greater than 0");

        RuleFor(x => x.ReceivingUserName).NotEmpty()
        .MinimumLength(3).WithMessage("SendingUsername must be atleast three characters")
        .MaximumLength(50).WithMessage("SendingUsername must be at most 50 characters");

        RuleFor(x => x.CreatedAt).NotEmpty()
        .NotEqual(default(DateTime)).WithMessage("Created at must be a real date")
        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future");

        RuleFor(x => x.Content).NotEmpty()
        .MaximumLength(300).WithMessage("Content cannot contain more than 300 characters");
    }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ReceivingUserId)
            .GreaterThan(0).WithMessage("ReceivingUserId must be greater than 0");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty")
            .MaximumLength(300).WithMessage("Message content cannot exceed 300 characters");
    }
}