using FluentValidation;
using Rex.Enum;

namespace Rex.Application.Modules.Reactions.Commands.RemoveLike;

public class RemoveLikeValidation : AbstractValidator<RemoveLikeCommand>
{
    public RemoveLikeValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(n => n.ReactionTargetType)
            .IsInEnum().WithMessage("Invalid reaction target type");
    }
}