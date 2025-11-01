using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Chats.Commands.CreatePrivateChat;

public class CreatePrivateChatCommandHandler(
    ILogger<CreatePrivateChatCommandHandler> logger,
    IUserChatRepository userChatRepository,
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IFriendShipRepository friendShipRepository,
    IChatNotifier chatNotifier,
    IDistributedCache cache
    ) : ICommandHandler<CreatePrivateChatCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received null request to create private chat.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong. The request is invalid."));
        }

        if (request.UserId == request.SecondUserId)
        {
            logger.LogWarning("User {UserId} attempted to create a chat with themselves.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "You cannot start a chat with yourself."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var secondUser = await userRepository.GetByIdAsync(request.SecondUserId, cancellationToken);

        if (user is null || secondUser is null)
        {
            logger.LogWarning("One or both users not found: User1={UserId}, User2={SecondUserId}", request.UserId, request.SecondUserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "One or both users could not be found."));
        }

        var friendshipExists = await friendShipRepository.FriendShipExistAsync(request.UserId, secondUser.Id, cancellationToken);
        if (!friendshipExists)
        {
            logger.LogWarning("Failed to create chat between users {UserId} and {SecondUserId} because friendship is inactive.", 
                request.UserId, request.SecondUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You cannot create a chat because the friendship is no longer active."));
        }

        var chatExists = await chatRepository.GetOneToOneChat(request.UserId, request.SecondUserId, cancellationToken);
        if (chatExists is not null)
        {
            logger.LogInformation("Chat already exists between users {UserId} and {SecondUserId} with ChatId {ChatId}", 
                request.UserId, request.SecondUserId, chatExists.Id);
            return ResultT<ResponseDto>.Failure(Error.Conflict("409", "A chat with this user already exists."));
        }

        var chat = new Models.Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private.ToString()
        };

        await chatRepository.CreateAsync(chat, cancellationToken);
        logger.LogInformation("Created new private chat with ID {ChatId} between users {UserId} and {SecondUserId}",
            chat.Id, request.UserId, request.SecondUserId);

        var userChats = new List<Models.UserChat>
        {
            new() { ChatId = chat.Id, UserId = request.UserId },
            new() { ChatId = chat.Id, UserId = request.SecondUserId }
        };

        await userChatRepository.CreateRangeAsync(userChats, cancellationToken);
        logger.LogInformation("Linked users {UserId} and {SecondUserId} to chat {ChatId}", request.UserId, request.SecondUserId, chat.Id);

        await cache.IncrementVersionAsync("chat", request.UserId, logger, cancellationToken);
        await cache.IncrementVersionAsync("chat", request.SecondUserId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for users {UserId} and {SecondUserId}", request.UserId, request.SecondUserId);
        
        await chatNotifier.NotifyChatCreatedAsync(
            new[] { request.UserId, request.SecondUserId },
            chat,
            cancellationToken
        );

        logger.LogInformation("Notification sent for chat {ChatId} creation to users {UserId} and {SecondUserId}", chat.Id, request.UserId, request.SecondUserId);
        
        return ResultT<ResponseDto>.Success(new ResponseDto("Chat created successfully!"));
    }
}
