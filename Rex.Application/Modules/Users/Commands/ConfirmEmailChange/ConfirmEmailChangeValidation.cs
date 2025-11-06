using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.ConfirmEmailChange;

public class ConfirmEmailChangeValidation : AbstractValidator<ConfirmEmailChangeCommand>
{
    public ConfirmEmailChangeValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code must be exactly 6 characters long");
    }
}