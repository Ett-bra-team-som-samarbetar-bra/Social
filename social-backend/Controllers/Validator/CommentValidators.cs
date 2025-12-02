public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
{
    public CommentCreateDtoValidator()
    {
        const int MaximumLength = 100;

        RuleFor(x => x.Content)
        .NotEmpty().WithMessage("Content cannot be empty")
        .NotNull().WithMessage("Content cannot be null")
        .MaximumLength(MaximumLength).WithMessage($"Content cannot exceed {MaximumLength} characters");
    }
}
