using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.RequestToJoinGroup
{
    public class RequestToJoinGroupCommandValidator : AbstractValidator<RequestToJoinGroupCommand>
    {
        public RequestToJoinGroupCommandValidator()
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