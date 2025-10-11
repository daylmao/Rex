using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Message;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Messages.Queries.GetMessagesByChatId;

public record GetMessagesByUserIdQuery(
    Guid ChatId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<MessageDto>>;