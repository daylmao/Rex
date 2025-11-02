using FluentValidation;

namespace Rex.Application.Modules.Posts.Commands.DeletePost;

public class DeletePostValidation : AbstractValidator<DeletePostCommand>
{
    public DeletePostValidation()
    {
        RuleFor(n => n.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(n => n.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}