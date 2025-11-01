using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Posts.Commands.DeletePost;

public record DeletePostCommand(
    Guid PostId,
    Guid GroupId,
    Guid UserId
    ): ICommand<ResponseDto>;