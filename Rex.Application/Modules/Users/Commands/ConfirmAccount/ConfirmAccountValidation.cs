using FluentValidation;

namespace Rex.Application.Modules.User.Commands.ConfirmAccount;

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