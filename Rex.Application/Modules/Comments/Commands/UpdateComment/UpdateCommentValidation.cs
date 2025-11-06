using FluentValidation;

namespace Rex.Application.Modules.Comments.Commands.UpdateComment;

public class UpdateCommentValidation : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentValidation()
    {
        RuleFor(c => c.CommentId)
            .NotEmpty().WithMessage("CommentId is required");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description cannot be empty")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        When(c => c.NewFiles is not null && c.NewFiles.Any(), () =>
        {
            RuleForEach(c => c.NewFiles)
                .Must(f => f.Length > 0).WithMessage("File cannot be empty");
        });

        When(c => c.FilesToDelete is not null && c.FilesToDelete.Any(), () =>
        {
            RuleForEach(c => c.FilesToDelete)
                .NotEmpty().WithMessage("Invalid file ID to delete");
        });
    }
}