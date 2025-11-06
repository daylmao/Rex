using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.ApproveRequest
{
    public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
    {
        public ApproveRequestCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");
        }
    }
}