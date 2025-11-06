using FluentValidation;

namespace Rex.Application.Modules.Comments.Commands.CreateCommentReply;

public class CreateCommentReplyValidation : AbstractValidator<CreateCommentReplyCommand>
{
    public CreateCommentReplyValidation()
    {
        RuleFor(r => r.ParentCommentId)
            .NotEmpty().WithMessage("ParentCommentId is required");

        RuleFor(r => r.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(r => r.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleForEach(r => r.Files)
            .Must(file => file.Length > 0)
            .When(r => r.Files is not null && r.Files.Any())
            .WithMessage("Each file must contain data");
    }
}