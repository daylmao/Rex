using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Chats.Commands.CreatePrivateChat;

public record CreatePrivateChatCommand(
    Guid UserId,
    Guid SecondUserId
    ): ICommand<ResponseDto>;