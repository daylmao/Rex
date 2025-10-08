using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Messages.Queries.GetMessagesByUserId;

public class GetMessagesByUserIdQueryHandler(
    ILogger<GetMessagesByUserIdQueryHandler> logger,
    IMessageRepository messageRepository
) : IQueryHandler<GetMessagesByUserIdQuery, PagedResult<MessageDto>>
{
    public async Task<ResultT<PagedResult<MessageDto>>> Handle(GetMessagesByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("");
            return ResultT<PagedResult<MessageDto>>.Failure(Error.Failure("400", ""));
        }

        var result =await messageRepository.GetMessagesByChatIdAsync(request.ChatId, request.PageNumber,
                request.PageSize, cancellationToken
        );

        var messagesDto = result.Items.Select(m => new MessageDto(
            m.ChatId,
            m.SenderId,
            m.Sender.FirstName,
            m.Sender.ProfilePhoto,
            m.Description,
            m.CreatedAt
        )).ToList();

        if (!messagesDto.Any())
        {
            logger.LogWarning("No messages found for chat ID {ChatId}", request.ChatId);
            return ResultT<PagedResult<MessageDto>>.Failure(Error.Failure("404", "No messages found."));
        }

        var pagedResult = new PagedResult<MessageDto>(
            messagesDto,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );
        return ResultT<PagedResult<MessageDto>>.Success(pagedResult);
    }
}