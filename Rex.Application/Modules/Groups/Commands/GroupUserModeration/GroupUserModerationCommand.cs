using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.GroupUserModeration;

public record GroupUserModerationCommand(
    Guid MemberId,
    Guid GroupId,
    GroupUserModerationStatus Status
    ): ICommand<ResponseDto>;