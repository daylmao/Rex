using FluentValidation;

namespace Rex.Application.Modules.Comments.Commands.PinComment;

public class PinCommentValidation : AbstractValidator<PinCommentCommand>
{
    public PinCommentValidation()
    {
        RuleFor(c => c.CommentId)
            .NotEmpty().WithMessage("CommentId is required");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(c => c.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(c => c.Pin)
            .NotNull().WithMessage("Pin flag must be specified");
    }
}