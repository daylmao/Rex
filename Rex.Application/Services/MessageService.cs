using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rex.Application.DTOs;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Services;

public class MessageService(
    ILogger<MessageService> logger,
    IUserChatRepository userChatRepository,
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IChatRepository chatRepository,
    IChatNotifier chatNotifier,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService
) : IMessageService
{
    public async Task<ResultT<MessageDto>> SendMessageAsync(Guid chatId, Guid userId, string messageText,
        IEnumerable<IFormFile?> files = null, CancellationToken cancellationToken = default)
    {
        var chat = await chatRepository.GetByIdAsync(chatId, cancellationToken);
        if (chat is null)
        {
            logger.LogWarning("Chat {ChatId} not found for user {UserId}", chatId, userId);
            return ResultT<MessageDto>.Failure(Error.Failure("404", "Chat not found."));
        }

        var belongs = await userChatRepository.IsUserInChatAsync(userId, chatId, cancellationToken);
        if (!belongs)
            return ResultT<MessageDto>.Failure(Error.Failure("403", "You don't have access to this chat."));

        var sender = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (sender is null)
            return ResultT<MessageDto>.Failure(Error.Failure("404", "User not found."));

        var message = new Message
        {
            ChatId = chatId,
            SenderId = userId,
            Description = messageText
        };

        await messageRepository.CreateAsync(message, cancellationToken);

        if (files is not null)
        {
            await ProcessFiles.ProcessFilesAsync(logger, files, chatId,
                fileRepository, entityFileRepository,
                cloudinaryService, TargetType.Message, cancellationToken);
        }

        var dto = new MessageDto(
            message.ChatId,
            message.SenderId,
            $"{sender.FirstName} {sender.LastName}",
            sender.ProfilePhoto,
            message.Description,
            message.CreatedAt
        );

        await chatNotifier.NotifyMessageAsync(chatId, dto);

        logger.LogInformation("User {UserId} sent message to chat {ChatId}", userId, chatId);

        return ResultT<MessageDto>.Success(dto);
    }
}