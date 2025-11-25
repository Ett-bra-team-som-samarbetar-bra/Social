/* namespace Api.Controllers.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MaximumLength(40);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MaximumLength(40);
    }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MinimumLength(4)
            .MaximumLength(40);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(4)
            .MaximumLength(40);

        RuleFor(x => x.Email)
            .NotEmpty().NotNull()
            .EmailAddress()
            .MaximumLength(60);
    }
}

public class EditValidator : AbstractValidator<RegisterRequest>
{
    public EditValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().NotNull()
            .EmailAddress()
            .MaximumLength(60);
    }
}
 */