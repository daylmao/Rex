using FluentValidation;

namespace Rex.Application.Modules.Reactions.Commands.AddLike;

public class AddLikeValidation : AbstractValidator<AddLikeCommand>
{
    public AddLikeValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(n => n.ReactionTargetType)
            .IsInEnum().WithMessage("Invalid reaction target type");
    }
}