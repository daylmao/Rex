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
    IChatNotifier chatNotifier
    ) : ICommandHandler<CreatePrivateChatCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong. The request is invalid."));

        if (request.UserId == request.SecondUserId)
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "You cannot start a chat with yourself."));

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var secondUser = await userRepository.GetByIdAsync(request.SecondUserId, cancellationToken);

        if (user is null || secondUser is null)
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "One or both users could not be found."));

        var chatExists = await chatRepository.GetOneToOneChat(request.UserId, request.SecondUserId, cancellationToken);
        if (chatExists is not null)
            return ResultT<ResponseDto>.Failure(Error.Conflict("409", "A chat with this user already exists."));

        var chat = new Models.Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private.ToString()
        };

        await chatRepository.CreateAsync(chat, cancellationToken);
        logger.LogInformation("Created new chat with ID {ChatId} between users {UserId} and {SecondUserId}",
            chat.Id, request.UserId, request.SecondUserId);

        var userChats = new List<Models.UserChat>
        {
            new() { ChatId = chat.Id, UserId = request.UserId },
            new() { ChatId = chat.Id, UserId = request.SecondUserId }
        };

        await userChatRepository.CreateRangeAsync(userChats, cancellationToken);
        logger.LogInformation("Associated users {UserId} and {SecondUserId} with chat ID {ChatId}",
            request.UserId, request.SecondUserId, chat.Id);
        
        await chatNotifier.NotifyChatCreatedAsync(
            new[] { request.UserId, request.SecondUserId },
            chat,
            cancellationToken
        );
        
        return ResultT<ResponseDto>.Success(new ResponseDto("Chat created successfully!"));
    }
}
