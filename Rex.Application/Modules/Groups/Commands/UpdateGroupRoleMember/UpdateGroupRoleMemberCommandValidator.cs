using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroupRoleMember
{
    public class UpdateGroupRoleMemberCommandValidator : AbstractValidator<UpdateGroupRoleMemberCommand>
    {
        public UpdateGroupRoleMemberCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Role must be a valid GroupRole.");
        }
    }
}