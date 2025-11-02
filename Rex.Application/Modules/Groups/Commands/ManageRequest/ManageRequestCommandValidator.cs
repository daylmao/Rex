using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.ManageRequest
{
    public class ManageRequestCommandValidator : AbstractValidator<ManageRequestCommand>
    {
        public ManageRequestCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid manage request status.");
        }
    }
}