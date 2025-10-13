using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Message;

namespace Rex.Application.Modules.Messages.Commands.SendMessage;

public record SendMessageCommand(
    Guid ChatId,
    string Message,
    Guid UserId
) : ICommand<MessageDto>;
