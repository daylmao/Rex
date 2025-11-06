using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.ConfirmAccount;

public class ConfirmAccountValidation: AbstractValidator<ConfirmAccountCommand>
{
    public ConfirmAccountValidation()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required");
        
        RuleFor(c => c.Code)
            .NotEmpty().WithMessage("Code is required");
    }
}