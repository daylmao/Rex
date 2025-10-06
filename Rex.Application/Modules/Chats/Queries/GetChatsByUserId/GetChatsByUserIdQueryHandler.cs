using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Chats.Queries.GetChatsByUserId;

public class GetChatsByUserIdQueryHandler(
    ILogger<GetChatsByUserIdQueryHandler> logger,
    IUserRepository userRepository,
    IChatRepository chatRepository,
    IDistributedCache cache
) : IQueryHandler<GetChatsByUserIdQuery, PagedResult<ChatLastMessageDto>>
{
    public async Task<ResultT<PagedResult<ChatLastMessageDto>>> Handle(GetChatsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetChatsByUserIdQuery request is null");
            return ResultT<PagedResult<ChatLastMessageDto>>.Failure(
                Error.Failure("400", "The request is invalid."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found with ID {UserId}", request.UserId);
            return ResultT<PagedResult<ChatLastMessageDto>>.Failure(
                Error.Failure("404", "The user could not be found."));
        }

        var result = await chatRepository.GetChatsWithLastMessageByUserIdAsync(
                request.UserId, request.PageNumber, request.PageSize, cancellationToken) ;

        var chatDtos = result.Items.Select(chat =>
        {
            var lastMessage = chat.Messages?
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            if (chat.Type == ChatType.Private.ToString())
            {
                var otherUser = chat.UserChats?.FirstOrDefault(uc => uc.UserId != request.UserId)?.User;
                return new ChatLastMessageDto(
                    chat.Id,
                    otherUser != null ? $"{otherUser.FirstName} {otherUser.LastName}" : "Unknown user",
                    chat.Type,
                    otherUser?.ProfilePhoto ?? "",
                    lastMessage != null
                        ? new LastMessageDto(
                            lastMessage.Description,
                            lastMessage.CreatedAt,
                            lastMessage.SenderId,
                            lastMessage.Sender != null ? $"{lastMessage.Sender.FirstName} {lastMessage.Sender.LastName}" : "Unknown sender",
                            lastMessage.Sender?.ProfilePhoto ?? ""
                        )
                        : null
                );
            }

            return new ChatLastMessageDto(
                chat.Id,
                chat.Name ?? "Unnamed Group",
                chat.Type,
                "",
                lastMessage != null
                    ? new LastMessageDto(
                        lastMessage.Description,
                        lastMessage.CreatedAt,
                        lastMessage.SenderId,
                        lastMessage.Sender != null ? $"{lastMessage.Sender.FirstName} {lastMessage.Sender.LastName}" : "Unknown sender",
                        lastMessage.Sender?.ProfilePhoto ?? ""
                    )
                    : null
            );
        }).ToList();

        if (!chatDtos.Any())
        {
            logger.LogWarning("No chats found for user ID {UserId}", request.UserId);
            return ResultT<PagedResult<ChatLastMessageDto>>.Failure(
                Error.Failure("404", "No chats were found for this user."));
        }

        
        var pagedResult = new PagedResult<ChatLastMessageDto>(
            chatDtos, result.TotalItems, request.PageNumber, request.PageSize);
        
        return ResultT<PagedResult<ChatLastMessageDto>>.Success(pagedResult);
    }
}
