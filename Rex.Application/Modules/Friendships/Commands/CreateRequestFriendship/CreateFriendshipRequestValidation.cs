using FluentValidation;

namespace Rex.Application.Modules.Friendships.Commands.CreateRequestFriendship;

public class CreateFriendshipRequestValidation : AbstractValidator<CreateFriendshipRequestCommand>
{
    public CreateFriendshipRequestValidation()
    {
        RuleFor(n => n.RequesterId)
            .NotEmpty().WithMessage("RequesterId is required");

        RuleFor(n => n.TargetUserId)
            .NotEmpty().WithMessage("TargetUserId is required");

        RuleFor(n => n)
            .Must(n => n.RequesterId != n.TargetUserId)
            .WithMessage("RequesterId and TargetUserId cannot be the same");
    }
}