using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroupRoleMember;

public record UpdateGroupRoleMemberCommand(
    Guid UserId,
    Guid GroupId,
    GroupRole Role
    ): ICommand<ResponseDto>;