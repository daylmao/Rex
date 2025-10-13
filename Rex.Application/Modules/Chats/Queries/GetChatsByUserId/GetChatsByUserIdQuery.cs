using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Challenge;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Chats.Queries.GetChatsByUserId;

public record GetChatsByUserIdQuery(
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? SearchTerm = null
    ): IQuery<PagedResult<ChatLastMessageDto>>;