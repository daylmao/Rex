using FluentValidation;

namespace Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;

public class ManageFriendshipRequestValidation : AbstractValidator<ManageFriendshipRequestCommand>
{
    public ManageFriendshipRequestValidation()
    {
        RuleFor(n => n.RequesterId)
            .NotEmpty().WithMessage("RequesterId is required");

        RuleFor(n => n.TargetUserId)
            .NotEmpty().WithMessage("TargetUserId is required");

        RuleFor(n => n)
            .Must(n => n.RequesterId != n.TargetUserId)
            .WithMessage("RequesterId and TargetUserId cannot be the same");

        RuleFor(n => n.Status)
            .IsInEnum().WithMessage("Invalid friendship status value");
    }
}