using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Message;
using Rex.Application.Interfaces;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Messages.Commands.SendFileMessage;

public class SendFileMessageCommandHandler(
    ILogger<SendFileMessageCommandHandler> logger,
    IMessageService messageService
    ): ICommandHandler<SendFileMessageCommand, MessageDto>
{
    public async Task<ResultT<MessageDto>> Handle(SendFileMessageCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
            return ResultT<MessageDto>.Failure(Error.Failure("400", "Invalid request. Please try again."));
        
        return await messageService.SendMessageAsync(request.ChatId, request.UserId, request.Message, request.Files, cancellationToken);
        
    }
}