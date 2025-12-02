public class PostCreateDtoValidator : AbstractValidator<PostCreateDto>
{
    public PostCreateDtoValidator()
    {
        const int TitleMaxLength = 100;
        const int ContentMaxLength = 1000;

        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required")
        .MaximumLength(TitleMaxLength).WithMessage($"Title must be at most {TitleMaxLength} characters long");

        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required")
        .MaximumLength(ContentMaxLength).WithMessage($"Content must be at most {ContentMaxLength} characters long");
    }
}

public class PostEditDtoValidator : AbstractValidator<PostEditDto>
{
    public PostEditDtoValidator()
    {
        const int ContentMaxLength = 1000;

        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required")
        .MaximumLength(ContentMaxLength).WithMessage($"Content must be at most {ContentMaxLength} characters long");
    }
}
