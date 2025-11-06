using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.GroupUserModeration
{
    public class GroupUserModerationCommandValidator : AbstractValidator<GroupUserModerationCommand>
    {
        public GroupUserModerationCommandValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("MemberId is required.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid moderation status.");
        }
    }
}