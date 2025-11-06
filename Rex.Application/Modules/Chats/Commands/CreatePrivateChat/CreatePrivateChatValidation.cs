using FluentValidation;

namespace Rex.Application.Modules.Chats.Commands.CreatePrivateChat;

public class CreatePrivateChatValidation : AbstractValidator<CreatePrivateChatCommand>
{
    public CreatePrivateChatValidation()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(c => c.SecondUserId)
            .NotEmpty().WithMessage("SecondUserId is required")
            .NotEqual(c => c.UserId).WithMessage("You cannot start a chat with yourself");
    }
}