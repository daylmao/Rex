using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Chats.Queries.GetChatsByUserId;

public record GetChatsByUserIdQuery(
    Guid UserId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<ChatLastMessageDto>>;