using FluentValidation;

namespace Rex.Application.Modules.Comments.Commands.CreateComment;

public class CreateCommentValidation : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidation()
    {
        RuleFor(c => c.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleForEach(c => c.Files)
            .Must(file => file.Length > 0)
            .When(c => c.Files is not null && c.Files.Any())
            .WithMessage("Each file must contain data");
    }
}