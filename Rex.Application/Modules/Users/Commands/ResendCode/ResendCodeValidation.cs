using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.ResendCode;

public class ResendCodeValidation : AbstractValidator<ResendCodeCommand>
{
    public ResendCodeValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
    }
}
