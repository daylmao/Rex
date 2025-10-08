using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Messages.Commands.SendMessage;

public class SendMessageCommandHandler(
    IMessageService messageService
) : ICommandHandler<SendMessageCommand, MessageDto>
{
    public async Task<ResultT<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
            return ResultT<MessageDto>.Failure(Error.Failure("400", "Invalid request. Please try again."));

        return await messageService.SendMessageAsync(request.ChatId, request.UserId, request.Message, cancellationToken: cancellationToken);
    }
}
