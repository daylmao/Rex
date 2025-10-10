using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Groups.Commands.RejectRequest;

public record RejectRequestCommand(
    Guid UserId,
    Guid GroupId
    ) : ICommand<ResponseDto>;