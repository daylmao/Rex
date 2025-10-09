using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Messages.Queries.GetMessagesByUserId;

public class GetMessagesByUserIdQueryHandler(
    ILogger<GetMessagesByUserIdQueryHandler> logger,
    IMessageRepository messageRepository,
    IDistributedCache cache,
    IFileRepository fileRepository
) : IQueryHandler<GetMessagesByUserIdQuery, PagedResult<MessageDto>>
{
    public async Task<ResultT<PagedResult<MessageDto>>> Handle(GetMessagesByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received null request for GetMessagesByUserIdQuery.");
            return ResultT<PagedResult<MessageDto>>.Failure(Error.Failure("400", "Request cannot be null."));
        }

        var result = await messageRepository.GetMessagesByChatIdAsync(request.ChatId, request.PageNumber,
            request.PageSize, cancellationToken
        );

        if (!result.Items.Any())
        {
            logger.LogInformation("No messages found for chat ID {ChatId}", request.ChatId);
            return ResultT<PagedResult<MessageDto>>.Success(
                new PagedResult<MessageDto>([], result.TotalItems, result.ActualPage, result.TotalPages)
            );
        }

        var files = await cache.GetOrCreateAsync(
            $"files:chat:{request.ChatId}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await fileRepository.GetFilesByTargetIdsAsync(result.Items.Select(m => m.Id).ToList(),
                TargetType.Message, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        var filesByMessageId = files
            .SelectMany(f => f.EntityFiles, (file, entityFile) => new
            {
                entityFile.TargetId,
                File = new FileDetailDto(file.Url, file.Type)
            })
            .GroupBy(x => x.TargetId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.File).ToList());

        var elements = result.Items.Select(m => new MessageDto(
            m.ChatId,
            m.SenderId,
            m.Sender.FirstName,
            m.Sender.ProfilePhoto,
            m.Description,
            m.CreatedAt,
            filesByMessageId.GetValueOrDefault(m.Id) ?? []
        )).ToList();

        var pagedResult = new PagedResult<MessageDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        return ResultT<PagedResult<MessageDto>>.Success(pagedResult);
    }
}