using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.InactiveAccount;

public class InactiveAccountValidation : AbstractValidator<InactiveAccountCommand>
{
    public InactiveAccountValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}