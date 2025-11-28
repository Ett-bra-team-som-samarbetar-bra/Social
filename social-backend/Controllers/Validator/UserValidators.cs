public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required")
        .MinimumLength(3).WithMessage("Username must be atleast three characters")
        .MaximumLength(50).WithMessage("Username must be at most 50 characters");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required to log in");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required")
        .MinimumLength(3).WithMessage("Username must be at least three characters")
        .MaximumLength(50).WithMessage("Username must be at most 50 characters long");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email must not be empty")
        .EmailAddress().WithMessage("Email must be a valid emailaddress")
        .MaximumLength(50).WithMessage("Email must be at most 50 characters long");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be emtpy")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters")
        .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches("[0-9]").WithMessage("Password must contain at least one number.")
        .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
        .Matches(@"^\S+$").WithMessage("Password cannot contain whitespace.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("").MaximumLength(300).WithMessage("Description must be at most 100 characters");
    }
}

public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
{
    public UpdatePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Password must not be emtpy")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters")
        .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches("[0-9]").WithMessage("Password must contain at least one number.")
        .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
        .Matches(@"^\S+$").WithMessage("Password cannot contain whitespace.");
    }
}

public class UserIdRequestValidator : AbstractValidator<UserIdRequest>
{
    public UserIdRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be a positive number");
    }
}