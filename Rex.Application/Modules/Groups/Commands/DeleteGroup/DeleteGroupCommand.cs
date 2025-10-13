using System.Windows.Input;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Groups.Commands.DeleteGroup;

public record DeleteGroupCommand(
    Guid GroupId,
    Guid UserId
    ): ICommand<ResponseDto>;