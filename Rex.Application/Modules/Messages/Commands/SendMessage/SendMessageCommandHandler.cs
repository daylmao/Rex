using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Modules.Messages.Commands.SendMessage;

public class SendMessageCommandHandler(
    ILogger<SendMessageCommandHandler> logger,
    IUserChatRepository userChatRepository,
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService,
    IChatRepository chatRepository,
    IChatNotifier chatNotifier
) : ICommandHandler<SendMessageCommand, MessageDto>
{
    public async Task<ResultT<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received a null SendMessageCommand");
            return ResultT<MessageDto>.Failure(Error.Failure("400", "Invalid request. Please try again."));
        }
        
        var chat = await chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
        if (chat is null)
        {
            logger.LogWarning("Chat {ChatId} not found for user {UserId}", request.ChatId, request.UserId);
            return ResultT<MessageDto>.Failure(Error.Failure("404", "Chat not found."));
        }
        
        var belongs = await userChatRepository.IsUserInChatAsync(request.UserId, request.ChatId, cancellationToken);
        if (!belongs)
            return ResultT<MessageDto>.Failure(Error.Failure("403", "You don't have access to this chat."));

        var sender = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (sender is null)
            return ResultT<MessageDto>.Failure(Error.Failure("404", "User not found."));

        var message = new Message
        {
            ChatId = request.ChatId,
            SenderId = request.UserId,
            Description = request.Message
        };

        await messageRepository.CreateAsync(message, cancellationToken);

        var dto = new MessageDto(
            message.ChatId,
            message.SenderId,
            $"{sender.FirstName} {sender.LastName}",
            sender.ProfilePhoto,
            message.Description,
            message.CreatedAt
        );

        await chatNotifier.NotifyMessageAsync(
            request.ChatId,
            dto);
        
        logger.LogInformation("User {UserId} sent message to chat {ChatId}", request.UserId, request.ChatId);

        return ResultT<MessageDto>.Success(dto);
    }
}
