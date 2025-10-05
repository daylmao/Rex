using System.Windows.Input;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Chats.Commands.CreatePrivateChat;

public record CreatePrivateChatCommand(
    Guid UserId,
    Guid SecondUserId
    ): ICommand<ResponseDto>;